//
// Copyright (c) 2004-2010 StreamBase Systems, Inc. All rights reserved.
//
package com.streambase.contrib.feedsimplugin.itch31;

import java.io.BufferedReader;
import java.io.FileInputStream;
import java.io.FileReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.HashMap;
import java.util.zip.GZIPInputStream;

import com.streambase.sb.Schema;
import com.streambase.sb.StreamBaseException;
import com.streambase.sb.Tuple;
import com.streambase.sb.TupleException;
import com.streambase.sb.feedsim.FeedSimTupleInputStream;
import com.streambase.sb.util.Msg;
import com.streambase.sb.util.Util;

/**
 * 
 * The class is a FeedSimulation custom reader plugin. It reads non-compressed, and gzip compressed
 * NASDAQ TotalView ITCH 3.1 formated files and emits messages onto a wide schema stream.
 * 
 *  By default, the reader is configured to skip over the stock directory, trading actions, etc. messages
 *  and begin sending the messages immediately after the "Start of System Hours" message.
 *
 */
public class FeedSimITCH31Plugin extends FeedSimTupleInputStream {
    private HashMap<String, TupleBuilder> tupleBuilderMap = null;

    private static final boolean skipToSS = Util.getBooleanSystemProperty("streambase.feedsimITCH32.skip-to-ss", true);
    
    private String path;
    private BufferedReader reader = null;
    private char [] currentLine=null;
    private int pos=0;
    
    private double currentTime=0;
	
    /**
     * The constructor uses the file extension to determines if the data is compressed or not. 
     * @param path
     * @throws IOException
     */
    public FeedSimITCH31Plugin(String path) throws IOException {
    	super(path);
    	this.path=path;
    	String extention = null;
    	if (path.length() > 4) {
    		extention=path.substring(path.length()-3, path.length());
    	}
    	
    	if ((extention != null) && ((".gz".endsWith(extention) || ".gzip".endsWith(extention)))) {
    		reader = new BufferedReader(new InputStreamReader(new GZIPInputStream(new FileInputStream(path))));
    	} else {
    		reader = new BufferedReader(new FileReader(path));
    	}
    	
    	// Create a map of methods that know how to convert message types
        tupleBuilderMap = new HashMap<String, TupleBuilder>();
        createBuilderMap();
        
        if (skipToSS) {
        	// Skip to start of day, keep track of time along the way
        	String line = reader.readLine();
        	while (line != null) {
	        	checkTimeUpdate(line);
	        	if ("SS".equals(line.substring(0, 2))) {
	        		break;
	        	}
	            line = reader.readLine();
        	}
        }
    }
 
    public void close() throws IOException {
    	reader.close();
    	super.close();
    }
    
    // read just reads a tuple and converts it to a string, then returns one character at a time.
    public int read() {
        if (currentLine == null) {
            	Tuple t;
				try {
					t = readTuple();
				} catch (StreamBaseException e) {
					return 0;
				}
            	if (t==null)
            		return -1;
            	
                String s=t.toString();
                if (s == null) {
                    return -1;
                }
                currentLine = s.toCharArray();
                if (currentLine.length == 0) {
                    return -1;
                }
        } else {
            if (pos == currentLine.length) {
                pos=0;
                currentLine=null;
                return '\n';
            }
        }
        
        return currentLine[pos++];
    }
    
    public Schema getSchema() {
    	return ITCH31SchemaHelper.getSchema();
    }
    
    // readTuple is the performance path for reading data 
    public Tuple readTuple() throws StreamBaseException {

        int linenum = 0;        
        Tuple tuple = null;
        try {
            String line;
            
            while (null != (line = reader.readLine())) {
                
            	String msgType = line.substring(0, 1);
                
            	if (checkTimeUpdate(line)) {
            		// This was a time update message, get another message
            		continue;
            	}
            	
            	TupleBuilder builder = null;
                if (null == (builder=tupleBuilderMap.get(msgType))) {
                	// This message type not handled.
                	Msg.warn(Msg.format("FeedSimITCH31Plugin: Unknown message type: {0}", msgType));
                	continue;
                }
                
                 try {
                    tuple = builder.buildTuple(ITCH31SchemaHelper.createTuple(), line);
                } catch (TupleException te) {
                	Msg.debug(te);
                	Msg.error(Msg.format("FeedSimITCHPlugin: [{0}:{1}] {2}", path, linenum, te.getMessage()));
                    throw new StreamBaseException(te);
                } catch (ArrayIndexOutOfBoundsException a) {
                   	Msg.debug(a);
                	Msg.error(Msg.format("FeedSimITCHPlugin: [{0}:{1}] {2}", path, linenum, a.getMessage()));
                    throw new StreamBaseException(a);
                } catch (NumberFormatException e) {
                    Msg.debug(e);
                    Msg.error(Msg.format("FeedSimITCHPlugin: [{0}:{1}] {2}", path, linenum, e.getMessage()));
                } 
                linenum++;
                break;
            }
        } catch (IOException e) {
            Msg.debug(e);                
            Msg.error(Msg.format("FeedSimITCHPlugin: [{0}:{1}] {2}", path, linenum, e));                
        }
        
        return tuple;
    }
 
    private void createBuilderMap() {
    	// Add Order
        tupleBuilderMap.put("A",
                new TupleBuilder() {
                    public Tuple buildTuple(Tuple dataTuple, String line) throws TupleException
                    {
                    	dataTuple.setDouble(ITCH31SchemaHelper.TIMESTAMP_IDX, currentTime);
                    	if (line.length() != 36) {
                    		Msg.warn(Msg.format("FeedSimITCHPlugin: Add Order Message (A) incorrect size. Expected 36 got {0}", line.length()));
                    		return null;
                    	}
                        dataTuple.setString(ITCH31SchemaHelper.MSGTYPE_IDX, line.substring(0,1));
                        dataTuple.setLong(ITCH31SchemaHelper.ORDER_REFERENCENUM_IDX, Long.parseLong(line.substring(1,13).trim()));
                        dataTuple.setString(ITCH31SchemaHelper.BUY_SELL_INDICATOR_IDX, line.substring(13,14));
                        dataTuple.setInt(ITCH31SchemaHelper.SHARES_IDX, Integer.parseInt(line.substring(14,20).trim()));
                        dataTuple.setString(ITCH31SchemaHelper.STOCK_IDX, line.substring(20,26).trim());
                        dataTuple.setDouble(ITCH31SchemaHelper.PRICE_IDX, getPrice(line.substring(26,36).trim()));
        		        return dataTuple;
                    }
                });
	    
        // Add Order MPID attribution
	    tupleBuilderMap.put("F",
                new TupleBuilder() {
                    public Tuple buildTuple(Tuple dataTuple, String line) throws TupleException
                    {
                    	dataTuple.setDouble(ITCH31SchemaHelper.TIMESTAMP_IDX, currentTime);
                    	if (line.length() != 40) {
                    		Msg.warn(Msg.format("FeedSimITCHPlugin: Add Order Message MPID Attribtuion (F) incorrect size. Expected 40 got {0}", line.length()));
                    		return null;
                    	}
                        dataTuple.setString(ITCH31SchemaHelper.MSGTYPE_IDX, line.substring(0,1));
                        dataTuple.setLong(ITCH31SchemaHelper.ORDER_REFERENCENUM_IDX, Long.parseLong(line.substring(1,13).trim()));
                        dataTuple.setString(ITCH31SchemaHelper.BUY_SELL_INDICATOR_IDX, line.substring(13,14));
                        dataTuple.setInt(ITCH31SchemaHelper.SHARES_IDX, Integer.parseInt(line.substring(14,20).trim()));
                        dataTuple.setString(ITCH31SchemaHelper.STOCK_IDX, line.substring(20,26).trim());
                        dataTuple.setDouble(ITCH31SchemaHelper.PRICE_IDX, getPrice(line.substring(26,36).trim()));
                        dataTuple.setString(ITCH31SchemaHelper.ATTRIBUTION_IDX, line.substring(36,40));
                		return dataTuple;
                    }
                });
	    
	    // Order Replace
	    tupleBuilderMap.put("U",
                new TupleBuilder() {
                    public Tuple buildTuple(Tuple dataTuple, String line) throws TupleException
                    {
                    	dataTuple.setDouble(ITCH31SchemaHelper.TIMESTAMP_IDX, currentTime);
                    	if (line.length() != 41) {
                    		Msg.warn(Msg.format("FeedSimITCHPlugin: Order Replace Message (U) incorrect size. Expected 41 got {0}", line.length()));
                    		return null;
                    	}
                    	dataTuple.setString(ITCH31SchemaHelper.MSGTYPE_IDX, line.substring(0,1));
                    	dataTuple.setLong(ITCH31SchemaHelper.ORDER_REFERENCENUM_IDX, Long.parseLong(line.substring(1,13).trim()));
                    	dataTuple.setLong(ITCH31SchemaHelper.NEW_ORDER_REFERENCENUM_IDX, Long.parseLong(line.substring(13,25).trim()));
                    	dataTuple.setInt(ITCH31SchemaHelper.SHARES_IDX, Integer.parseInt(line.substring(25,31).trim()));
                    	dataTuple.setDouble(ITCH31SchemaHelper.PRICE_IDX, getPrice(line.substring(31,41).trim()));
                        return dataTuple;
                    }
                });
	    
	    // Order Delete
	    tupleBuilderMap.put("D",
                new TupleBuilder() {
                    public Tuple buildTuple(Tuple dataTuple, String line) throws TupleException
                    {
                    	dataTuple.setDouble(ITCH31SchemaHelper.TIMESTAMP_IDX, currentTime);
                    	if (line.length() != 13) {
                    		Msg.warn(Msg.format("FeedSimITCHPlugin: Delete Order Message (D) incorrect size. Expected 13 got {0}", line.length()));
                    		return null;
                    	}
                    	dataTuple.setString(ITCH31SchemaHelper.MSGTYPE_IDX, line.substring(0,1));
                    	dataTuple.setLong(ITCH31SchemaHelper.ORDER_REFERENCENUM_IDX, Long.parseLong(line.substring(1,13).trim()));
                        return dataTuple;
                    }
                });
	    
	    // Order Cancel
	    tupleBuilderMap.put("X",
                new TupleBuilder() {
                    public Tuple buildTuple(Tuple dataTuple, String line) throws TupleException
                    {
                    	dataTuple.setDouble(ITCH31SchemaHelper.TIMESTAMP_IDX, currentTime);
                    	if (line.length() != 19) {
                    		Msg.warn(Msg.format("FeedSimITCHPlugin: Order Cancel Message (X) incorrect size. Expected 19 got {0}", line.length()));
                    		return null;
                    	}
                    	dataTuple.setString(ITCH31SchemaHelper.MSGTYPE_IDX, line.substring(0,1));
                    	dataTuple.setLong(ITCH31SchemaHelper.ORDER_REFERENCENUM_IDX, Long.parseLong(line.substring(1,13).trim()));
                    	dataTuple.setInt(ITCH31SchemaHelper.CANCELED_SHARES_IDX, Integer.parseInt(line.substring(13,19).trim()));
                        return dataTuple;
                    }
                });
	    
	    // Order Executed
	    tupleBuilderMap.put("E",
                new TupleBuilder() {
                    public Tuple buildTuple(Tuple dataTuple, String line) throws TupleException
                    {
                    	dataTuple.setDouble(ITCH31SchemaHelper.TIMESTAMP_IDX, currentTime);
                    	if (line.length() != 31) {
                    		Msg.warn(Msg.format("FeedSimITCHPlugin: Order Executed Message (E) incorrect size. Expected 31 got {0}", line.length()));
                    		return null;
                    	}
                    	dataTuple.setString(ITCH31SchemaHelper.MSGTYPE_IDX, line.substring(0,1));
                    	dataTuple.setLong(ITCH31SchemaHelper.ORDER_REFERENCENUM_IDX, Long.parseLong(line.substring(1,13).trim()));
                    	dataTuple.setInt(ITCH31SchemaHelper.EXECUTED_SHARES_IDX, Integer.parseInt(line.substring(13,19).trim()));
                    	dataTuple.setLong(ITCH31SchemaHelper.MATCH_NUMBER_IDX, Long.parseLong(line.substring(19,31).trim()));
                        return dataTuple;
                    }
                });	    
	    
	    // Order Executed with Price
	    tupleBuilderMap.put("C",
                new TupleBuilder() {
                    public Tuple buildTuple(Tuple dataTuple, String line) throws TupleException
                    {
                    	dataTuple.setDouble(ITCH31SchemaHelper.TIMESTAMP_IDX, currentTime);
                    	if (line.length() != 42) {
                    		Msg.warn(Msg.format("FeedSimITCHPlugin: Order Executed with Price Message (C) incorrect size. Expected 42 got {0}", line.length()));
                    		return null;
                    	}
                    	dataTuple.setString(ITCH31SchemaHelper.MSGTYPE_IDX, line.substring(0,1));
                    	dataTuple.setLong(ITCH31SchemaHelper.ORDER_REFERENCENUM_IDX, Long.parseLong(line.substring(1,13).trim()));
                    	dataTuple.setInt(ITCH31SchemaHelper.EXECUTED_SHARES_IDX, Integer.parseInt(line.substring(13,19).trim()));
                    	dataTuple.setLong(ITCH31SchemaHelper.MATCH_NUMBER_IDX, Long.parseLong(line.substring(19,31).trim()));
                    	dataTuple.setString(ITCH31SchemaHelper.PRINTABLE_IDX, line.substring(31,32));
                    	dataTuple.setDouble(ITCH31SchemaHelper.EXECUTION_PRICE_IDX, getPrice(line.substring(32,42).trim()));
                    	 return dataTuple;
                    }
                });
	    
	    // Trade Message
	    tupleBuilderMap.put("P",
                new TupleBuilder() {
                    public Tuple buildTuple(Tuple dataTuple, String line) throws TupleException
                    {
                    	dataTuple.setDouble(ITCH31SchemaHelper.TIMESTAMP_IDX, currentTime);
                    	if (line.length() != 48) {
                    		Msg.warn(Msg.format("FeedSimITCHPlugin: Trade Message (P) incorrect size. Expected 48 got {0}", line.length()));
                    		return null;
                    	}
                    	dataTuple.setString(ITCH31SchemaHelper.MSGTYPE_IDX, line.substring(0,1));
                    	dataTuple.setLong(ITCH31SchemaHelper.ORDER_REFERENCENUM_IDX, Long.parseLong(line.substring(1,13).trim()));
                    	dataTuple.setString(ITCH31SchemaHelper.BUY_SELL_INDICATOR_IDX, line.substring(13,14));
                        dataTuple.setInt(ITCH31SchemaHelper.SHARES_IDX, Integer.parseInt(line.substring(14,20).trim()));
                        dataTuple.setString(ITCH31SchemaHelper.STOCK_IDX, line.substring(20,26).trim());
                        dataTuple.setDouble(ITCH31SchemaHelper.PRICE_IDX, getPrice(line.substring(26,36).trim()));
                    	dataTuple.setLong(ITCH31SchemaHelper.MATCH_NUMBER_IDX, Long.parseLong(line.substring(36,48).trim()));
                     	return dataTuple;
                    }
                });
	    
	    // Cross Trade Message
	    tupleBuilderMap.put("Q",
                new TupleBuilder() {
                    public Tuple buildTuple(Tuple dataTuple, String line) throws TupleException
                    {
                    	dataTuple.setDouble(ITCH31SchemaHelper.TIMESTAMP_IDX, currentTime);
                    	if (line.length() != 39) {
                    		Msg.warn(Msg.format("FeedSimITCHPlugin: Cross Trade Message (Q) incorrect size. Expected 39 got {0}", line.length()));
                    		return null;
                    	}
                    	dataTuple.setString(ITCH31SchemaHelper.MSGTYPE_IDX, line.substring(0,1));
                        dataTuple.setInt(ITCH31SchemaHelper.SHARES_IDX, Integer.parseInt(line.substring(1,10).trim()));
                        dataTuple.setString(ITCH31SchemaHelper.STOCK_IDX, line.substring(10,16).trim());
                        dataTuple.setDouble(ITCH31SchemaHelper.CROSSPRICE_IDX, getPrice(line.substring(16,26).trim()));
                    	dataTuple.setLong(ITCH31SchemaHelper.MATCH_NUMBER_IDX, Long.parseLong(line.substring(26,38).trim()));
                    	dataTuple.setString(ITCH31SchemaHelper.CROSSTYPE_IDX, line.substring(38,39).trim());
                     	return dataTuple;
                    }
                });
	    
	    // Broken Trade Message
	    tupleBuilderMap.put("B",
                new TupleBuilder() {
                    public Tuple buildTuple(Tuple dataTuple, String line) throws TupleException
                    {
                    	dataTuple.setDouble(ITCH31SchemaHelper.TIMESTAMP_IDX, currentTime);
                    	if (line.length() != 13) {
                    		Msg.warn(Msg.format("FeedSimITCHPlugin: Broken Trade Message (C) incorrect size. Expected 13 got {0}", line.length()));
                    		return null;
                    	}
                    	dataTuple.setString(ITCH31SchemaHelper.MSGTYPE_IDX, line.substring(0,1));
                    	dataTuple.setLong(ITCH31SchemaHelper.MATCH_NUMBER_IDX, Long.parseLong(line.substring(1,13).trim()));
                     	return dataTuple;
                    }
                });
	    
	    
	    // NOII Message
	    tupleBuilderMap.put("I",
                new TupleBuilder() {
                    public Tuple buildTuple(Tuple dataTuple, String line) throws TupleException
                    {
                    	dataTuple.setDouble(ITCH31SchemaHelper.TIMESTAMP_IDX, currentTime);
                    	if (line.length() != 58) {
                    		Msg.warn(Msg.format("FeedSimITCHPlugin: NOII Message (I) incorrect size. Expected 58 got {0}", line.length()));
                    		return null;
                    	}
                    	dataTuple.setString(ITCH31SchemaHelper.MSGTYPE_IDX, line.substring(0,1));
                    	dataTuple.setInt(ITCH31SchemaHelper.PAIRED_SHARES_IDX, Integer.parseInt(line.substring(1,10).trim()));
                    	dataTuple.setInt(ITCH31SchemaHelper.IMBALANCED_SHARES_IDX, Integer.parseInt(line.substring(10,19).trim()));
                    	dataTuple.setString(ITCH31SchemaHelper.IMBALANCE_DIRECTION_IDX, line.substring(19,20).trim());
                    	dataTuple.setString(ITCH31SchemaHelper.STOCK_IDX, line.substring(20,26).trim());                    	 
                        dataTuple.setDouble(ITCH31SchemaHelper.FAR_PRICE_IDX, getPrice(line.substring(26,36).trim()));
                        dataTuple.setDouble(ITCH31SchemaHelper.NEAR_PRICE_IDX, getPrice(line.substring(36,46).trim()));
                        dataTuple.setDouble(ITCH31SchemaHelper.CURRENT_REFERENCE_PRICE_IDX, getPrice(line.substring(46,56).trim()));
                        dataTuple.setString(ITCH31SchemaHelper.CROSSTYPE_IDX, line.substring(56,57));
                        dataTuple.setString(ITCH31SchemaHelper.PRICE_VARIATION_INDICATOR_IDX, line.substring(57,58));
                     	return dataTuple;
                    }
                });
	    
	    // Market Participant Position
	    tupleBuilderMap.put("L",
                new TupleBuilder() {
                    public Tuple buildTuple(Tuple dataTuple, String line) throws TupleException
                    {
                    	dataTuple.setDouble(ITCH31SchemaHelper.TIMESTAMP_IDX, currentTime);
                    	if (line.length() != 14) {
                    		Msg.warn(Msg.format("FeedSimITCHPlugin: Market Participant Position Message (L) incorrect size. Expected 14 got {0}", line.length()));
                    		return null;
                    	}
                    	dataTuple.setString(ITCH31SchemaHelper.MSGTYPE_IDX, line.substring(0,1));
                    	dataTuple.setString(ITCH31SchemaHelper.MPID_IDX, line.substring(1,5));
                    	dataTuple.setString(ITCH31SchemaHelper.STOCK_IDX, line.substring(5,11));
                        dataTuple.setString(ITCH31SchemaHelper.PRIMARY_MARKET_MAKER_IDX, line.substring(11,12));
                        dataTuple.setString(ITCH31SchemaHelper.MARKET_MAKER_MODE_IDX, line.substring(12,13));
                        dataTuple.setString(ITCH31SchemaHelper.MARKET_PARTICIPANT_STATE_IDX, line.substring(13,14));
                     	return dataTuple;
                    }
                });
	    
	    // Stock Trading Action
	    tupleBuilderMap.put("H",
                new TupleBuilder() {
                    public Tuple buildTuple(Tuple dataTuple, String line) throws TupleException
                    {
                    	dataTuple.setDouble(ITCH31SchemaHelper.TIMESTAMP_IDX, currentTime);
                    	if (line.length() != 13) {
                    		Msg.warn(Msg.format("FeedSimITCHPlugin: Stock Trading Action Message (H) incorrect size. Expected 13 got {0}", line.length()));
                    		return null;
                    	}
                    	dataTuple.setString(ITCH31SchemaHelper.MSGTYPE_IDX, line.substring(0,1));
                    	dataTuple.setString(ITCH31SchemaHelper.STOCK_IDX, line.substring(1,7));
                        dataTuple.setString(ITCH31SchemaHelper.TRADINGSTATE_IDX, line.substring(7,8));
                        // skip reserved field
                        dataTuple.setString(ITCH31SchemaHelper.REASON_IDX, line.substring(9,13));
                     	return dataTuple;
                    }
                });
	    
    }
	
    interface TupleBuilder {
        Tuple buildTuple(Tuple dataTuple, String line) throws TupleException;
    }

    private boolean checkTimeUpdate(String line) {
    	try {
    		String msgType = line.substring(0, 1);
		    if ("T".equals(msgType)) {
	           	if (line.length() != 6) {
	        		Msg.warn(Msg.format("FeedSimITCH31Plugin: Time message (T) expected length {0}, got {1}: {2}",
	        				6, line.length(), line));
	        		return true;
	           	}
	
		    	currentTime = Double.parseDouble(line.substring(1,6));
		    	return true;
		    }
		    if ("M".equals(msgType)) {
	        	if (line.length() != 4) {
	        		Msg.warn(Msg.format("FeedSimITCH31Plugin: Millisecond message (M) expected length {0}, got {1}: {2}",
	        				4, line.length(), line));
	        		return true;
	        	}
		    	long truncate = (long)currentTime;
		    	currentTime = truncate + (Double.parseDouble(line.substring(1,4))/1000);
		    	return true;
		    }
    	} catch (NumberFormatException e) {
    		Msg.debug(e);
    		Msg.warn(Msg.format("FeedSimITCH31Plugin: Problem processing time message {0}: {1}",
    				line, e.getMessage()));
    	}
	    return false;
    }
    
    // Price is a string of numbers with the decimal point implied at 4 places in. 
    private double getPrice(String price) {
    	double p=Double.parseDouble(price);
    	return p/10000;
    }
}