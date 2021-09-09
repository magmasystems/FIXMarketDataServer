using System.Collections.Generic;
using MagmaTrader.Data;
using QuickFix44;

namespace FIXMarketDataServer.FIXServerModule
{
	public class FIXSerializers
	{
		static public Order ToOrder(NewOrderSingle message)
		{
			Order order = new Order
			{
				ClOrderID = message.getClOrdID().getValue(),
				OrderID = string.Empty,
				Symbol = new Symbol(message.getSymbol().getValue()),
			    Price = message.getPrice().getValue(),
			    Quantity = (int) message.getOrderQty().getValue(),
				Side = LookupSide(message.getSide().getValue()),
				TIF = LookupTimeInForce(message.getTimeInForce().getValue()),
				Type = LookupOrderType(message.getOrdType().getValue()),
				Attributes = OrderAttributes.Undefined,
			};
			return order;
		}

		static public Order ToOrder(OrderCancelRequest message)
		{
			Order order = new Order
			{
				OrderID = message.getOrigClOrdID().getValue(),		// the original order id
				Symbol = new Symbol(message.getSymbol().getValue()),
				Quantity = (int)message.getOrderQty().getValue(),
				Side = LookupSide(message.getSide().getValue()),
				Attributes = OrderAttributes.Undefined,
				OrderRequestID = message.getOrderID().getValue(),		// the id of the cancel request
			};
			return order;
		}

		static private readonly Dictionary<char, Side> MapFixSideToOurSide = new Dictionary<char, Side>
		{
			{ QuickFix.Side.BUY,  Side.Buy  },
			{ QuickFix.Side.SELL, Side.Sell },
		};
		static private Side LookupSide(char fixSide)
		{
			Side ourSide;
			return MapFixSideToOurSide.TryGetValue(fixSide, out ourSide) ? ourSide : Side.Undefined;
		}

		static private readonly Dictionary<char, TimeInForce> MapFixTimeInForceToOurTimeInForce = new Dictionary<char, TimeInForce>
		{
			{ QuickFix.TimeInForce.DAY,              TimeInForce.Day  },
			{ QuickFix.TimeInForce.GOOD_TILL_CANCEL, TimeInForce.GTC },
		};
		static private TimeInForce LookupTimeInForce(char fixTimeInForce)
		{
			TimeInForce ourTimeInForce;
			return MapFixTimeInForceToOurTimeInForce.TryGetValue(fixTimeInForce, out ourTimeInForce) ? ourTimeInForce : TimeInForce.Undefined;
		}

		static private readonly Dictionary<char, OrderType> MapFixOrderTypeToOurOrderType = new Dictionary<char, OrderType>
		{
			{ QuickFix.OrdType.LIMIT,  OrderType.Limit  },
			{ QuickFix.OrdType.MARKET, OrderType.Market },
		};
		static private OrderType LookupOrderType(char fixOrderType)
		{
			OrderType ourOrderType;
			return MapFixOrderTypeToOurOrderType.TryGetValue(fixOrderType, out ourOrderType) ? ourOrderType : OrderType.Undefined;
		}

	}
}
