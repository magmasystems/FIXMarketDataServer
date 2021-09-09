using System;
using System.Diagnostics;

namespace MagmaTrader.MarketData
{
	public interface IQuoteChooserStrategy
	{
		int NextIndex { get; }
		int MaxSize { get; set; }
	}


	public class QuoteChooserStrategyFactory
	{
		static public IQuoteChooserStrategy Create(string name, int maxSize, object userData = null)
		{
			switch (name.ToLower())
			{
				case "roundrobin":
					return new RoundRobinQuoteChooserStrategy(maxSize);
				case "random":
					return new RandomQuoteChooserStrategy(maxSize);
				case "adv":
					return new WeightedVolumeChooserStrategy(maxSize, userData as long[]);
				default:
					return new RandomQuoteChooserStrategy(maxSize);
			}
		}
	}

	public class RoundRobinQuoteChooserStrategy : IQuoteChooserStrategy
	{
		private int m_maxSize;
		private int m_currentIndex;

		public RoundRobinQuoteChooserStrategy(int maxSize)
		{
			Debug.Assert(maxSize > 0);
			this.m_maxSize = maxSize;
			this.m_currentIndex = 0;
		}

		public int NextIndex
		{
			get
			{
				int idx = this.m_currentIndex;
				if (++this.m_currentIndex >= this.m_maxSize)
					this.m_currentIndex = 0;
				return idx;
			}
		}

		public int MaxSize
		{
			get { return this.m_maxSize; }
			set { this.m_maxSize = value; }
		}
	}

	public class RandomQuoteChooserStrategy : IQuoteChooserStrategy
	{
		private int m_maxSize;
		private readonly Random m_rnd = new Random(DateTime.Now.Millisecond);

		public RandomQuoteChooserStrategy(int maxSize)
		{
			Debug.Assert(maxSize > 0);
			this.m_maxSize = maxSize;
		}

		public int NextIndex
		{
			get
			{
				return this.m_rnd.Next(0, this.m_maxSize);
			}
		}

		public int MaxSize
		{
			get { return this.m_maxSize; }
			set { this.m_maxSize = value; }
		}
	}


	// TODO - implement this properly
	public class WeightedVolumeChooserStrategy : IQuoteChooserStrategy
	{
		private int m_maxSize;
		private readonly Random m_rnd = new Random(DateTime.Now.Millisecond);
		private readonly long[] m_volumes;

		public WeightedVolumeChooserStrategy(int maxSize, long[] volumes)
		{
			Debug.Assert(maxSize > 0);
			this.m_maxSize = maxSize;
			this.m_volumes = (long[]) volumes.Clone();
		}

		public int NextIndex
		{
			get
			{
				// First, generate a uniform random number.
				double r = this.m_rnd.NextDouble();

				// Now, we apply a function to change i into a random number within the distribution curve
				int idx = this.ApplyDistribution(r);
				return idx;
			}
		}

		public int MaxSize
		{
			get { return this.m_maxSize; }
			set { this.m_maxSize = value; }
		}

		public int ApplyDistribution(double r)
		{
			// TODO - generate a random number according to the distribution defined
			// by the m_volumes array.
			// http://www.ece.virginia.edu/mv/edu/prob/stat/random-number-generation.pdf

			int ri = (int) (r * this.m_maxSize);
			return ri;
		}
	}
}
