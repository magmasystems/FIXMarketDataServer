package com.streambase.contrib.feedsimplugin.itch31;

import com.streambase.sb.CompleteDataType;
import com.streambase.sb.Schema;
import com.streambase.sb.Tuple;

public class ITCH31SchemaHelper {
	
	public static final String TIMESTAMP_NAME="timestamp";
	public static final String MSGTYPE_NAME="msgType";
    public static final String EVENTCODE_NAME="eventCode";
    public static final String STOCK_NAME="stock";
    public static final String ORDER_REFERENCENUM_NAME="orderReferenceNum";
    public static final String BUY_SELL_INDICATOR_NAME="buySellIndicator";
    public static final String SHARES_NAME="shares";
    public static final String PRICE_NAME="price";
    public static final String TRADINGSTATE_NAME="tradingState";
    public static final String MPID_NAME="MPID";
    public static final String ATTRIBUTION_NAME="attribution";
    public static final String MARKET_CATEGORY_NAME="marketCategory";
    public static final String FINANCIAL_STATUS_INDICATOR_NAME="financialStatusIndicator";
    public static final String ROUND_LOT_SIZE_NAME="roundLotSize";
    public static final String ROUND_LOTS_ONLY_NAME="RoundLotsOnly";
    public static final String REASON_NAME="reason";
    public static final String PRIMARY_MARKET_MAKER_NAME="primaryMarketMaker";
    public static final String MARKET_MAKER_MODE_NAME="marketMakerMode";
    public static final String MARKET_PARTICIPANT_STATE_NAME="marketParticipantState";
    public static final String MARKET_CATEGOY_NAME="marketCategoy";
    public static final String EXECUTED_SHARES_NAME="executedShares";
    public static final String MATCH_NUMBER_NAME="matchNumber";
    public static final String PRINTABLE_NAME="printable";
    public static final String EXECUTION_PRICE_NAME="executionPrice";
    public static final String CANCELED_SHARES_NAME="canceledShares";
    public static final String NEW_ORDER_REFERENCENUM_NAME="newOrderReferenceNum";
    public static final String CROSSPRICE_NAME="crossPrice";
    public static final String CROSSTYPE_NAME="crossType";
    public static final String PAIRED_SHARES_NAME="pairedShares";
    public static final String IMBALANCED_SHARES_NAME="imbalancedShares";
    public static final String IMBALANCE_DIRECTION_NAME="imbalanceDirection";
    public static final String FAR_PRICE_NAME="farPrice";
    public static final String NEAR_PRICE_NAME="nearPrice";
    public static final String CURRENT_REFERENCE_PRICE_NAME="currentReferencePrice";
    public static final String PRICE_VARIATION_INDICATOR_NAME="priceVariationIndicator";
	
	public static final Schema.Field[] FIELDS = {
		new Schema.Field(TIMESTAMP_NAME, CompleteDataType.forDouble()),
		new Schema.Field(MSGTYPE_NAME, CompleteDataType.forString()),
		new Schema.Field(EVENTCODE_NAME, CompleteDataType.forString()),
		new Schema.Field(STOCK_NAME, CompleteDataType.forString()),
		new Schema.Field(ORDER_REFERENCENUM_NAME, CompleteDataType.forLong()),
		new Schema.Field(BUY_SELL_INDICATOR_NAME, CompleteDataType.forString()),
		new Schema.Field(SHARES_NAME, CompleteDataType.forInt()),
		new Schema.Field(PRICE_NAME, CompleteDataType.forDouble()),
		new Schema.Field(TRADINGSTATE_NAME, CompleteDataType.forString()),
		new Schema.Field(MPID_NAME, CompleteDataType.forString()),
		new Schema.Field(ATTRIBUTION_NAME, CompleteDataType.forString()),
		new Schema.Field(MARKET_CATEGORY_NAME, CompleteDataType.forString()),
		new Schema.Field(FINANCIAL_STATUS_INDICATOR_NAME, CompleteDataType.forString()),
		new Schema.Field(ROUND_LOT_SIZE_NAME, CompleteDataType.forInt()),
		new Schema.Field(ROUND_LOTS_ONLY_NAME, CompleteDataType.forString()),
		new Schema.Field(REASON_NAME, CompleteDataType.forString()),
		new Schema.Field(PRIMARY_MARKET_MAKER_NAME, CompleteDataType.forString()),
		new Schema.Field(MARKET_MAKER_MODE_NAME, CompleteDataType.forString()),
		new Schema.Field(MARKET_PARTICIPANT_STATE_NAME, CompleteDataType.forString()),
		new Schema.Field(MARKET_CATEGOY_NAME, CompleteDataType.forString()),
		new Schema.Field(EXECUTED_SHARES_NAME, CompleteDataType.forInt()),
		new Schema.Field(MATCH_NUMBER_NAME, CompleteDataType.forLong()),
		new Schema.Field(PRINTABLE_NAME, CompleteDataType.forString()),
		new Schema.Field(EXECUTION_PRICE_NAME, CompleteDataType.forDouble()),
		new Schema.Field(CANCELED_SHARES_NAME, CompleteDataType.forInt()),
		new Schema.Field(NEW_ORDER_REFERENCENUM_NAME, CompleteDataType.forLong()),
		new Schema.Field(CROSSPRICE_NAME, CompleteDataType.forDouble()),
		new Schema.Field(CROSSTYPE_NAME, CompleteDataType.forString()),
		new Schema.Field(PAIRED_SHARES_NAME, CompleteDataType.forInt()),
		new Schema.Field(IMBALANCED_SHARES_NAME, CompleteDataType.forInt()),
		new Schema.Field(IMBALANCE_DIRECTION_NAME, CompleteDataType.forString()),
		new Schema.Field(FAR_PRICE_NAME, CompleteDataType.forDouble()),
		new Schema.Field(NEAR_PRICE_NAME, CompleteDataType.forDouble()),
		new Schema.Field(CURRENT_REFERENCE_PRICE_NAME, CompleteDataType.forDouble()),
		new Schema.Field(PRICE_VARIATION_INDICATOR_NAME, CompleteDataType.forString()),
	};
	
	public static final Schema SCHEMA = new Schema("ITCH31Schema", FIELDS);

	public static final int TIMESTAMP_IDX = SCHEMA.getFieldIndex(TIMESTAMP_NAME);
	public static final int MSGTYPE_IDX = SCHEMA.getFieldIndex(MSGTYPE_NAME);
	public static final int EVENTCODE_IDX = SCHEMA.getFieldIndex(EVENTCODE_NAME);
	public static final int STOCK_IDX = SCHEMA.getFieldIndex(STOCK_NAME);
	public static final int ORDER_REFERENCENUM_IDX = SCHEMA.getFieldIndex(ORDER_REFERENCENUM_NAME);
	public static final int BUY_SELL_INDICATOR_IDX = SCHEMA.getFieldIndex(BUY_SELL_INDICATOR_NAME);
	public static final int SHARES_IDX = SCHEMA.getFieldIndex(SHARES_NAME);
	public static final int PRICE_IDX = SCHEMA.getFieldIndex(PRICE_NAME);
	public static final int TRADINGSTATE_IDX = SCHEMA.getFieldIndex(TRADINGSTATE_NAME);
	public static final int MPID_IDX = SCHEMA.getFieldIndex(MPID_NAME);
	public static final int ATTRIBUTION_IDX = SCHEMA.getFieldIndex(ATTRIBUTION_NAME);
	public static final int MARKET_CATEGORY_IDX = SCHEMA.getFieldIndex(MARKET_CATEGORY_NAME);
	public static final int FINANCIAL_STATUS_INDICATOR_IDX = SCHEMA.getFieldIndex(FINANCIAL_STATUS_INDICATOR_NAME);
	public static final int ROUND_LOT_SIZE_IDX = SCHEMA.getFieldIndex(ROUND_LOT_SIZE_NAME);
	public static final int ROUND_TLOTS_ONLY_IDX = SCHEMA.getFieldIndex(ROUND_LOTS_ONLY_NAME);
	public static final int REASON_IDX = SCHEMA.getFieldIndex(REASON_NAME);
	public static final int PRIMARY_MARKET_MAKER_IDX = SCHEMA.getFieldIndex(PRIMARY_MARKET_MAKER_NAME);
	public static final int MARKET_MAKER_MODE_IDX = SCHEMA.getFieldIndex(MARKET_MAKER_MODE_NAME);
	public static final int MARKET_PARTICIPANT_STATE_IDX = SCHEMA.getFieldIndex(MARKET_PARTICIPANT_STATE_NAME);
	public static final int MARKET_CATEGOY_IDX = SCHEMA.getFieldIndex(MARKET_CATEGOY_NAME);
	public static final int EXECUTED_SHARES_IDX = SCHEMA.getFieldIndex(EXECUTED_SHARES_NAME);
	public static final int MATCH_NUMBER_IDX = SCHEMA.getFieldIndex(MATCH_NUMBER_NAME);
	public static final int PRINTABLE_IDX = SCHEMA.getFieldIndex(PRINTABLE_NAME);
	public static final int EXECUTION_PRICE_IDX = SCHEMA.getFieldIndex(EXECUTION_PRICE_NAME);
	public static final int CANCELED_SHARES_IDX = SCHEMA.getFieldIndex(CANCELED_SHARES_NAME);
	public static final int NEW_ORDER_REFERENCENUM_IDX = SCHEMA.getFieldIndex(NEW_ORDER_REFERENCENUM_NAME);
	public static final int CROSSPRICE_IDX = SCHEMA.getFieldIndex(CROSSPRICE_NAME);
	public static final int CROSSTYPE_IDX = SCHEMA.getFieldIndex(CROSSTYPE_NAME);
	public static final int PAIRED_SHARES_IDX = SCHEMA.getFieldIndex(PAIRED_SHARES_NAME);
	public static final int IMBALANCED_SHARES_IDX = SCHEMA.getFieldIndex(IMBALANCED_SHARES_NAME);
	public static final int IMBALANCE_DIRECTION_IDX = SCHEMA.getFieldIndex(IMBALANCE_DIRECTION_NAME);
	public static final int FAR_PRICE_IDX = SCHEMA.getFieldIndex(FAR_PRICE_NAME);
	public static final int NEAR_PRICE_IDX = SCHEMA.getFieldIndex(NEAR_PRICE_NAME);
	public static final int CURRENT_REFERENCE_PRICE_IDX = SCHEMA.getFieldIndex(CURRENT_REFERENCE_PRICE_NAME);
	public static final int PRICE_VARIATION_INDICATOR_IDX = SCHEMA.getFieldIndex(PRICE_VARIATION_INDICATOR_NAME);

	public static Tuple createTuple() {
		return SCHEMA.createTuple();
	}

	public static Schema getSchema() {
		return SCHEMA;
	}
}
