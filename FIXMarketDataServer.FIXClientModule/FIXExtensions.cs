using MagmaTrader.Data;

namespace FIXMarketDataClient.FIXClientModule
{
	static public class FIXExtensions
	{
		// ReSharper disable RedundantNameQualifier
		static public char ToFIX(MagmaTrader.Data.Side mySide)
		{
			switch (mySide)
			{
				case MagmaTrader.Data.Side.Buy:
					return QuickFix.Side.BUY;
				case MagmaTrader.Data.Side.Sell:
					return QuickFix.Side.SELL;
				case MagmaTrader.Data.Side.ShortSell:
					return QuickFix.Side.SELL_SHORT;
				default:
					return QuickFix.Side.BUY;
			}
		}

		static public MagmaTrader.Data.Side FromFIX(char fixSide)
		{
			switch (fixSide)
			{
				case QuickFix.Side.BUY:
					return MagmaTrader.Data.Side.Buy;
				case QuickFix.Side.SELL:
					return  MagmaTrader.Data.Side.Sell;
				case QuickFix.Side.SELL_SHORT:
					return MagmaTrader.Data.Side.ShortSell;
				default:
					return MagmaTrader.Data.Side.Buy;
			}
		}

	
		static public char ToFIX(MagmaTrader.Data.TimeInForce myTIF)
		{
			switch (myTIF)
			{
				case MagmaTrader.Data.TimeInForce.Day:
					return QuickFix.TimeInForce.DAY;
				case MagmaTrader.Data.TimeInForce.FOK:
					return QuickFix.TimeInForce.FILL_OR_KILL;
				case MagmaTrader.Data.TimeInForce.GTC:
					return QuickFix.TimeInForce.GOOD_TILL_CANCEL;
				case MagmaTrader.Data.TimeInForce.GTD:
					return QuickFix.TimeInForce.GOOD_TILL_DATE;
				case MagmaTrader.Data.TimeInForce.IOC:
					return QuickFix.TimeInForce.IMMEDIATE_OR_CANCEL;
				case MagmaTrader.Data.TimeInForce.OPG:
					return QuickFix.TimeInForce.AT_THE_OPENING;
				case MagmaTrader.Data.TimeInForce.CLOSE:
					return QuickFix.TimeInForce.AT_THE_CLOSE;

				default:
					return QuickFix.TimeInForce.DAY;
			}
		}

		static public char ToFIX(MagmaTrader.Data.OrderType myType)
		{
			switch (myType)
			{
				case MagmaTrader.Data.OrderType.Limit:
					return QuickFix.OrdType.LIMIT;
				case MagmaTrader.Data.OrderType.Market:
					return QuickFix.OrdType.MARKET;
				case MagmaTrader.Data.OrderType.LimitOnClose:
					return QuickFix.OrdType.LIMIT_ON_CLOSE;
				case MagmaTrader.Data.OrderType.MarketOnClose:
					return QuickFix.OrdType.MARKET_ON_CLOSE;
				case MagmaTrader.Data.OrderType.PeggedToMarket:
					return QuickFix.OrdType.PEGGED;
				case MagmaTrader.Data.OrderType.Stop:
					return QuickFix.OrdType.STOP;
				case MagmaTrader.Data.OrderType.StopLimit:
					return QuickFix.OrdType.STOP_LIMIT;
	
				default:
					return QuickFix.OrdType.MARKET;
			}
		}

		static public OrderState FromFIX(QuickFix.OrdStatus status)
		{
			switch (status.getValue())
			{
				case QuickFix.OrdStatus.CANCELED:
					return OrderState.Cancelled;
				case QuickFix.OrdStatus.PENDING_CANCEL:
					return OrderState.PendingCancel;

				case QuickFix.OrdStatus.NEW:
					return OrderState.New;
				case QuickFix.OrdStatus.PENDING_NEW:
					return OrderState.PendingNew;

				case QuickFix.OrdStatus.PARTIALLY_FILLED:
					return OrderState.PartiallyFilled;
				case QuickFix.OrdStatus.FILLED:
					return OrderState.Filled;
				case QuickFix.OrdStatus.DONE_FOR_DAY:
					return OrderState.DoneForDay;

				case QuickFix.OrdStatus.REJECTED:
					return OrderState.Rejected;
				case QuickFix.OrdStatus.STOPPED:
					return OrderState.Stopped;
	
				default:
					return OrderState.Undefined;
			}
		}
	
		// ReSharper restore RedundantNameQualifier
	}
}
