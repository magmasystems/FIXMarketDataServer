using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace MagmaTrader.MarketData
{
	abstract public class MarketDataGenerator<TQuote> : IMarketDataGenerator<TQuote>
	{
		#region Events
		public event Action<TQuote, int> QuoteGenerated = (q, i) => { };
		#endregion

		#region Variables
		protected readonly ObservableCollection<TQuote> m_quoteCache;
		protected readonly IQuoteChooserStrategy m_chooser;
		protected readonly Random m_rnd = new Random(DateTime.Now.Millisecond);
		#endregion

		#region Constructors
		protected MarketDataGenerator(ObservableCollection<TQuote> quoteCache, int interval = 500)
		{
			this.m_quoteCache = quoteCache;
			this.m_chooser = QuoteChooserStrategyFactory.Create("Random", quoteCache.Count);

			this.m_interval = interval;
		}
		#endregion

		#region Cleanup
		public virtual void Dispose()
		{
			this.Stop();
		}
		#endregion

		#region Methods
		public virtual void IncrementChooserSize()
		{
			this.m_chooser.MaxSize++;
		}
		#endregion

		#region Timer and Quote Generation
		public DispatcherTimer Timer { get; set; }

		public int m_interval;
		public int Interval
		{
			get { return this.m_interval; }
			set
			{
				this.m_interval = value;
				this.ResetTimerInterval();
			}
		}

		public void Start()
		{
			if (this.Timer == null)
			{
				this.Timer = new DispatcherTimer(TimeSpan.FromMilliseconds(this.Interval), DispatcherPriority.Normal, this.OnTimerCallback, Dispatcher.CurrentDispatcher);
			}
			this.Timer.Start();
		}

		public void Stop()
		{
			if (this.Timer != null)
			{
				this.Timer.Stop();
			}
		}

		protected void ResetTimerInterval()
		{
			if (this.Timer != null && this.Timer.IsEnabled)
			{
				this.Timer.Stop();
				this.Timer.Interval = TimeSpan.FromMilliseconds(this.Interval);
				this.Timer.Start();
			}
		}

		protected void OnTimerCallback(object sender, EventArgs e)
		{
			this.GenerateTick();
		}

		public abstract void GenerateTick();

		protected void FireQuoteGeneratedEvent(TQuote newQuote, int idxQuote)
		{
			this.QuoteGenerated(newQuote, idxQuote);
		}
		#endregion
	}
}
