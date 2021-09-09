namespace MagmaTrader.Threading
{
	public interface IThreadQueue<T> where T : class
	{
		#region Methods
		void Start();
		void Stop();
		void Restart();
		void QueueObject(T obj);
		#endregion

		#region Properties
		int StopTimeout { get; set; }
		int MaximumWriteQueueCount { get; set; }
		bool IsStarted { get; }
		bool IsBackground { get; set; }
		int Count { get; }
		#endregion
	}

	public interface IThreadQueueCallback<T> where T : class
	{
		void OnThreadCallback(T obj);
	}
}
