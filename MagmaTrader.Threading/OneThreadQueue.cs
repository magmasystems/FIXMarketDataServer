using System.Collections.Generic;
using System.Threading;

namespace MagmaTrader.Threading
{
	/// <summary>
	/// a simple thread queue
	/// to use:
	/// create an instance of this by supplying a constructor with an implementation the interface, 
	/// Implement your call back to process the passed object
	/// call StatrtWork Once
	/// call QueueObj to stuff the objects to que
	/// </summary>
	public class OneThreadQueue<T> : IThreadQueue<T> where T : class
	{
		#region Private fields
		private readonly Queue<T> m_queue;
		private readonly IThreadQueueCallback<T> m_userCallback;
		private readonly AutoResetEvent m_autoResetEvent;
		volatile bool m_stopped;
		#endregion

		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		/// <param name="callBack"></param>
		/// <param name="LinkListInitialCapacity"></param>
		public OneThreadQueue(IThreadQueueCallback<T> callBack, int LinkListInitialCapacity = 0)
		{
			this.m_queue = new Queue<T>(LinkListInitialCapacity);
			this.m_userCallback = callBack;
			this.m_autoResetEvent = new AutoResetEvent(false);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="callBack"></param>
		/// <param name="startIt"></param>
		/// <param name="LinkListInitialCapacity"></param>
		public OneThreadQueue(IThreadQueueCallback<T> callBack, bool startIt, int LinkListInitialCapacity) : this(callBack, LinkListInitialCapacity)
		{
			if (startIt)
				Start();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="callBack"></param>
		/// <param name="startIt"></param>
		public OneThreadQueue(IThreadQueueCallback<T> callBack, bool startIt) : this(callBack)
		{
			if (startIt)
				Start();
		}
		#endregion

		#region Public properties
		/// <summary>
		/// 
		/// </summary>
		public Thread Thread { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int StopTimeout
		{
			get
			{
				return 0;
			}

			set
			{

			}
		}

		/// <summary>
		/// 
		/// </summary>
		public int MaximumWriteQueueCount
		{
			get
			{
				return 0;
			}

			set
			{

			}
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsStarted
		{
			get
			{
				return !this.m_stopped;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsBackground
		{
			get
			{
				return this.Thread.IsBackground;
			}

			set
			{
				this.Thread.IsBackground = value;
			}
		}

		public int Count
		{
			get { return this.m_queue.Count; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// call once to start the first worker
		/// </summary>
		public void Start()
		{
			this.Thread = null;
			this.m_stopped = false;
			this.Thread = new Thread(WorkLoop);
			this.Thread.Start();
			//m_thread.IsBackground = true;
		}

		/// <summary>
		/// 
		/// </summary>
		public void Stop()
		{
			lock (this.m_queue)
			{
				this.m_stopped = true;
				this.m_queue.Clear();
				this.m_autoResetEvent.Set();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void Restart()
		{
			lock (this.m_queue)
			{
				this.m_queue.Clear();
			}
		}

		/// <summary>
		/// add object to be worked on
		/// </summary>
		/// <param name="obj"></param>
		public void QueueObject(T obj)
		{
			if (this.m_stopped)
				return;

			lock (this.m_queue)
			{
				this.m_queue.Enqueue(obj);
				if (this.m_queue.Count == 1)
				{
					//  Tell the readers that we put something on the queue.
					if (!this.m_stopped && this.m_autoResetEvent != null)
						this.m_autoResetEvent.Set();
				}
				return;
			}
		}
		#endregion

		#region Protected members
		/// <summary>
		/// called by internal callback to get the next thread with the next object going 
		/// </summary>
		protected void WorkLoop()
		{
			while (!this.m_stopped)
			{
				T obj = default(T);

				lock (this.m_queue)
				{
					// There is an item in the queue, so remove it
					if (this.m_queue.Count > 0)
					{
						obj = this.m_queue.Dequeue();
					}
					else
					{
						// No item in the queue. Tell the readers that there is nothing on the by setting the event to false
						if (!this.m_stopped)
							this.m_autoResetEvent.Reset();
					}
				}

				// Amit Chauhan
				// Sleeping for 0 ms to give other threads a chance to take a lock
				// on the m_queue. Useful under high frequency updates to queue.
				Thread.Sleep(0);

				if (obj != null)
				{
					// Send the de-queued object to the object that created this threadqueue
					this.m_userCallback.OnThreadCallback(obj);
				}
				else
				{
					// Wait for something to be put on the queue
					if (!this.m_stopped)
						this.m_autoResetEvent.WaitOne();
				}
			}
		}

		/// <summary>
		/// this calls the user call back and then calls for more work
		/// </summary>
		/// <param name="obj"></param>
		protected void ThreadCallback(object obj)
		{
			WorkLoop();
		}
		#endregion
	}
}
