using System.Collections.Generic;
using MagmaTrader.Data;
using QuickFix;
using QuickFix44;
using Message = QuickFix.Message;
using Symbol = QuickFix.Symbol;

namespace MagmaTrader.Utilities
{
	public class FIXHelpers
	{
		private const string BAD_MESSAGE_TYPE = "?";
	
		static public bool IsHeartbeat(Message message)
		{
			return GetMessageType(message) == "0";
		}

		static public bool IsTestMessage(Message message)
		{
			return GetMessageType(message) == "1";
		}

		static public string GetMessageType(Message message)
		{
			return message.getHeader().isSetField(MsgType.FIELD) ? message.getHeader().getString(MsgType.FIELD) : BAD_MESSAGE_TYPE;
		}

		static public MarketDataRequest CreateMarketDataRequestMessage(List<Data.Symbol> symbols)
		{
			MarketDataRequest request = new MarketDataRequest(
				new MDReqID(OrderIDGenerator.Generate()),
				new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT_PLUS_UPDATES),
				new MarketDepth(1)                 // 1 = top of book
				);

			request.set(new MDUpdateType(MDUpdateType.FULL_REFRESH));

			// Symbol information
			MarketDataRequest.NoRelatedSym noRelatedSym = new MarketDataRequest.NoRelatedSym();
			foreach (Data.Symbol symbol in symbols)
			{
				noRelatedSym.set(new Symbol(symbol.Name));
				noRelatedSym.set(new SecurityID(symbol.Name));
				noRelatedSym.set(new SecurityIDSource(SecurityIDSource.EXCHANGE_SYMBOL));
				request.addGroup(noRelatedSym);
			}

			// Market Data requested (bid, ask, etc)
			MarketDataRequest.NoMDEntryTypes noMDEntryTypes = new MarketDataRequest.NoMDEntryTypes();
			noMDEntryTypes.set(new MDEntryType(MDEntryType.BID));
			request.addGroup(noMDEntryTypes);
			noMDEntryTypes.set(new MDEntryType(MDEntryType.OFFER));
			request.addGroup(noMDEntryTypes);

			return request;
		}

	}
}
