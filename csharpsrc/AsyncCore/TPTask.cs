using System;
using System.Collections;
using System.Threading;

namespace PSL.AsyncCore
{
	/// <summary>
	/// Internal class used by the TaskPool for initialization.
	/// Encapsulates a TPTask and its associated thread.
	/// </summary>
	internal sealed class TPTaskInit
	{
		private TPTask m_task = null;
		private Thread m_worker = null;
		
		/// <summary>
		/// Ctor
		/// </summary>
		internal TPTaskInit()
		{}
		
		/// <summary>
		/// Property gets/sets the TPTask instance 
		/// </summary>
		internal TPTask Task
		{
			get
			{ return m_task; }
			set
			{ m_task = value; }
		}

		/// <summary>
		/// Property gets/sets the thread associated with a TPTask
		/// </summary>
		internal Thread Worker
		{
			get
			{ return m_worker; }
			set
			{ m_worker = value; }
		}
	}//End-TPTaskInit

	
	/// <summary>
	/// A TPTask forces a thread to follow the leaders-followers design pattern
	/// </summary>
	internal sealed class TPTask
	{
		/// <summary>
		/// TaskCompletion event handler delegate
		/// </summary>
		public delegate void TPTaskCompleteHandler( TPTaskCompleteEventArgs tceArg );

		/// <summary>
		/// TaskCompletion event 
		/// </summary>
		public static event TPTaskCompleteHandler TPTaskComplete;
				
		// Data used by each TPTask
		
		/// <summary>
		/// Condition variable used to synchronize TPTasks running in the TaskPool
		/// </summary>
		private AutoResetEvent m_condition = null;

		/// <summary>
		/// After a thread becomes the leader, if there are no
		/// pending requests the leader will exit the TaskPool
		/// in some cases we may want to keep the thread in the TaskPool
		/// until the TaskPool is explicitly shutdown
		/// </summary>
		private bool m_bRecycleTask = false;
		
		private Guid m_taskID = Guid.Empty;
		
		// Data shared by all TPTasks

		/// <summary>
		/// Indicates whether a leader TPTask thread exists in this process.
		/// </summary>
		private static bool bLeaderAvailable = false;

		/// <summary>
		/// Returns the number of TPTask threads running in this process.
		/// </summary>
		private static int nNumTasks = 0;

		/// <summary>
		/// Queue used to hold TaskRequests to service
		/// <seealso cref="TaskRequest"/>
		/// </summary>
		internal static Queue requestQ = new Queue();
		
		/// <summary>
		/// Ctor
		/// Construct TPTask passing in a shared condition used to signal threads
		/// </summary>
		/// <param name="condition">Condition variable shared by all TPTasks in
		/// a TaskPool</param>
		public TPTask( ref AutoResetEvent condition )
		{
			m_condition = condition;
		}

		/// <summary>
		/// Property gets/sets the TPTasks recyclable property.
		/// A recyclable TPTask thread will remain in the TaskPool
		/// after servicing a TaskRequest.
		/// </summary>
		public bool RecycleTask
		{
			get
			{ return m_bRecycleTask; }

			set
			{ m_bRecycleTask = value; }
		}

		/// <summary>
		/// Property gets/sets the TaskID of a TPTask, set when a TPTask 
		/// is servicing a TaskRequest. The TPTask assumes the TaskID of the 
		/// TaskRequest it is servicing.
		/// </summary>
		public Guid TaskID
		{
			get
			{ return m_taskID; }
			set
			{ m_taskID = value; }
		}

		/// <summary>
		/// Property gets the number of TaskRequests queued for servicing
		/// </summary>
		public static int QueuedRequests
		{
			get
			{ return requestQ.Count; }
		}
		
		/// <summary>
		/// Property returns the number of TPTask threads running 
		/// </summary>
		public static int Tasks
		{
			get
			{ return nNumTasks; }
		}
		
		/// <summary>
		/// Procedure enforces the leader-followers behavior of TPTask threads
		/// </summary>
		/// <remarks></remarks>
		public void Service()
		{
			// Every thread that comes thru here ups the thread count
			Interlocked.Increment( ref nNumTasks );

			while( true )
			{
				// Begin critical section
				Monitor.Enter( this );
				// While a leader has been selected...wait around
				while( bLeaderAvailable )
				{
					m_condition.WaitOne();
				}
				
				// Assert self as leader before leaving critical section
				bLeaderAvailable = true;
				// Leave critical section
				Monitor.Exit( this );

				bool bExitLoop = false;
				
				// Nothing else to do so this thread can exit or be recycled
				if( requestQ.Count == 0 )
					bExitLoop = true; 
				
				// Begin critical section
				Monitor.Enter( this );
				
				// Signal self is no longer the leader
				bLeaderAvailable = false;
			
				Monitor.Exit( this );
				
				// Signal a follower to become the new leader
				m_condition.Set();
				
				if( bExitLoop )
				{
					// If this task is not marked as recyclable
					// then it must exit if no requests are 
					// pending
					if( !m_bRecycleTask )
					{
						// Thread on its way out so decrease thread count
						Interlocked.Decrement( ref nNumTasks );
						break;
					}
				}
				else
				{
					try
					{
						// Dequeue Service request
						TaskRequest req = (TaskRequest) requestQ.Dequeue();
						// Set taskID
						this.TaskID = req.TaskID;
						// Execute Callback
						object objRes = req.TaskCb( req.Context );
						// If any one subscribed, then fire LFTaskDone event
						if( TPTaskComplete != null )
						{
							TPTaskCompleteEventArgs tceArg = new TPTaskCompleteEventArgs();
							tceArg.TaskID = req.TaskID;
							tceArg.Result = objRes;
							TPTaskComplete( tceArg );
						}
					}
					catch( Exception e )// Catch any exceptions thrown
					{
						// If any one subscribed, then fire LFTaskFailed event
						if( TPTaskComplete != null )
						{
							TPTaskCompleteEventArgs tceArg = new TPTaskCompleteEventArgs();
							tceArg.TaskID = this.TaskID;
							// Signal that we had errors so that
							// clients can check the
							// ExceptionMessage property
							tceArg.HasErrors = true;
							tceArg.Result = e.Message;
							TPTaskComplete( tceArg );
						}
					}
					finally
					{
						// Reset task ID
						this.TaskID = Guid.Empty;
					}
				}
			}// End-while(true)
		}// End-Method Service() 
	}
}
