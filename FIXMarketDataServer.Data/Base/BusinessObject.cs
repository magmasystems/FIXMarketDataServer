using System.ComponentModel;

namespace MagmaTrader.Data
{
	public class BusinessObject : INotifyPropertyChanged
	{
		#region Events
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region Property Notification
		protected void NotifyPropertyChanged(string prop)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
		}
		#endregion

		#region Validation
		public virtual bool IsValid
		{
			get
			{
				return this.Validate();
			}
		}

		public virtual bool Validate()
		{
			return true;
		}
		#endregion
	}
}
