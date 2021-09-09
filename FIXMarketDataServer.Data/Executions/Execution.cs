using System;

namespace MagmaTrader.Data
{
	public class Execution : Order
	{
		protected string m_ExecID;
		public string ExecID
		{
			get { return this.m_ExecID; }
			set { this.m_ExecID = value; this.NotifyPropertyChanged("ExecID"); }
		}

		protected ExecutionType m_ExecType;
		public ExecutionType ExecType
		{
			get { return this.m_ExecType; }
			set { this.m_ExecType = value; this.NotifyPropertyChanged("ExecType"); }
		}

		protected DateTime m_TransactTime;
		public DateTime TransactTime
		{
			get { return this.m_TransactTime; }
			set { this.m_TransactTime = value; this.NotifyPropertyChanged("TransactTime"); }
		}

		public override string ToString()
		{
			return string.Format("Side {0}, ExecQty {1}, LvQty {2}, Qty {3}, AvgPx {4:##.00}, Time {5}",
				this.Side, this.ExecutedQuantity, this.LeavesQuantity, this.Quantity, this.AveragePrice, this.TransactTime.ToShortTimeString());
		}
	}
}
