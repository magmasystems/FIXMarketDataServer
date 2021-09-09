using System.Collections.Generic;

namespace MagmaTrader.Data
{
	public class Order : BusinessObject
	{
		#region Properties - These fields are sent as input for new orders
		protected Symbol m_Symbol;
		public Symbol Symbol
		{
			get { return this.m_Symbol; }
			set { this.m_Symbol = value; this.NotifyPropertyChanged("Symbol"); }
		}

		protected string m_ClOrderID;
		public string ClOrderID
		{
			get { return this.m_ClOrderID; }
			set { this.m_ClOrderID = value; }
		}

		protected string m_OrderID;
		public string OrderID
		{
			get { return this.m_OrderID; }
			set { this.m_OrderID = value; }
		}

		protected double m_Price;
		public double Price
		{
			get { return this.m_Price; }
			set { this.m_Price = value; this.NotifyPropertyChanged("Price"); }
		}

		protected int m_Quantity;
		public int Quantity
		{
			get { return this.m_Quantity; }
			set { this.m_Quantity = value; this.NotifyPropertyChanged("Quantity"); }
		}

		protected Side m_Side;
		public Side Side
		{
			get { return this.m_Side; }
			set { this.m_Side = value; this.NotifyPropertyChanged("Side"); }
		}

		protected TimeInForce m_TIF;
		public TimeInForce TIF
		{
			get { return this.m_TIF; }
			set { this.m_TIF = value; this.NotifyPropertyChanged("TIF"); }
		}

		protected OrderType m_Type;
		public OrderType Type
		{
			get { return this.m_Type; }
			set { this.m_Type = value; this.NotifyPropertyChanged("Type"); }
		}

		protected OrderAttributes m_Attributes;
		public OrderAttributes Attributes
		{
			get { return this.m_Attributes; }
			set { this.m_Attributes = value; this.NotifyPropertyChanged("Attributes"); }
		}
		#endregion

		#region Properties - These are used once the order is in the OMS
		protected OrderState m_OrderState;
		public OrderState OrderState
		{
			get { return this.m_OrderState; }
			set { this.m_OrderState = value; this.NotifyPropertyChanged("OrderState"); }
		}
	
		protected ExecutionState m_ExecutionState;
		public ExecutionState ExecutionState
		{
			get { return this.m_ExecutionState; }
			set { this.m_ExecutionState = value; this.NotifyPropertyChanged("ExecutionState"); }
		}

		protected string m_ParentOrderID;
		public string ParentOrderID
		{
			get { return this.m_ParentOrderID; }
			set { this.m_ParentOrderID = value; this.NotifyPropertyChanged("ParentOrderID"); }
		}

		protected List<string> m_ChildOrderIDs;
		public List<string> ChildOrdersIDs
		{
			get { return this.m_ChildOrderIDs; }
			set { this.m_ChildOrderIDs = value; this.NotifyPropertyChanged("ChildOrdersIDs"); }
		}

		protected int m_ExecutedQuantity;
		public int ExecutedQuantity
		{
			get { return this.m_ExecutedQuantity; }
			set { this.m_ExecutedQuantity = value; this.NotifyPropertyChanged("ExecutedQuantity"); }
		}

		protected int m_LeavesQuantity;
		public int LeavesQuantity
		{
			get { return this.m_LeavesQuantity; }
			set { this.m_LeavesQuantity = value; this.NotifyPropertyChanged("LeavesQuantity"); }
		}

		protected double m_AveragePrice;
		public double AveragePrice
		{
			get { return this.m_AveragePrice; }
			set { this.m_AveragePrice = value; this.NotifyPropertyChanged("AveragePrice"); }
		}
		#endregion

		#region Extra info for FIX messages that change the order state
		protected string m_OrderRequestID;
		public string OrderRequestID
		{
			get { return this.m_OrderRequestID; }
			set { this.m_OrderRequestID = value; this.NotifyPropertyChanged("OrderRequestID"); }
		}
		#endregion

		#region Constructors
		public Order() : this(orderID:OrderIDGenerator.Generate())
		{
		}

		public Order(string orderID)
		{
			this.ClOrderID = orderID;
			this.OrderID = string.Empty;
			this.Quantity = 0;
			this.Price = 0;
			this.Side = Side.Undefined;
			this.Attributes = OrderAttributes.Undefined;
			this.TIF = TimeInForce.Undefined;
			this.Type = OrderType.Undefined;

			this.OrderState = OrderState.Undefined;
			this.ExecutionState = ExecutionState.Undefined;
			this.ParentOrderID = string.Empty;
			this.ChildOrdersIDs = null;

			this.ExecutedQuantity = 0;
			this.LeavesQuantity = 0;
			this.AveragePrice = 0;
		}

		// This copies the attributes from the ExecutionReport to the Order
		public void Copy(Order order)
		{
			/*
			this.Quantity = order.Quantity;
			this.Price = order.Price;
			this.Side = order.Side;
			this.Attributes = order.Attributes;
			this.TIF = order.TIF;
			this.Type = order.Type;
			*/

			this.OrderID = order.OrderID;
			this.OrderState = order.OrderState;
			this.ExecutionState = order.ExecutionState;
			this.ParentOrderID = order.ParentOrderID;

			this.LeavesQuantity = order.LeavesQuantity;
			this.ExecutedQuantity = order.ExecutedQuantity;
			this.AveragePrice = order.AveragePrice;
			
			if (order.ChildOrdersIDs != null)
				this.ChildOrdersIDs = new List<string>(order.ChildOrdersIDs);
		}

		#endregion

		#region Overrides
		public override string ToString()
		{
			string orderId = this.ClOrderID.Length < 12 ? this.ClOrderID : this.ClOrderID.Substring(12);
			return string.Format("ID {0}, Symbol {1}, Side {2}, Quantity {3}, Price {4:###.##}", 
				orderId, (this.Symbol != null) ? this.Symbol.Name : "", this.Side, this.Quantity, this.Price);
		}
		#endregion

		#region Methods
		public bool IsTerminal
		{
			get
			{
				switch (this.OrderState)
				{
					case OrderState.Cancelled:
					case OrderState.DoneForDay:
					case OrderState.Filled:
					case OrderState.Rejected:
					case OrderState.Stopped:
						return true;
					default:
						return false;
				}
			}
		}
		#endregion
	}
}
