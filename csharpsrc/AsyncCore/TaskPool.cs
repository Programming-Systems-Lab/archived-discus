using System;
using System.Threading;
using System.Collections;
using System.Diagnostics;

namespace PSL.AsyncCore
{
	/// <summary>
	/// A TaskPool is a threadpool implemented using the leaders-followers
	/// design pattern. The TaskPool is an event based entity, clients of the
	/// TaskPool send task requests and get notified on task completion. 
	/// </summary>
	public sealed class TaskPool
	{
		/// <summary>
		/// Minimum number of TPTask threads running in the TaskPool 
		/// on initialization
		/// </summary>
		/// <remarks>There is currently no TaskPool method to increase
		/// or decrease the number of TPTask threads in the TaskPool</remarks>
		public const int MIN_ACTIVE_TASKS = 4;
		
		/// <summary>
		/// Maximum number of TPTask threads that can be running in the
		/// TaskPool
		/// </summary>
		public const int MAX_ACTIVE_TASKS = 10;
		
		/// <summary>
		/// Max number of milliseconds to wait for a thread to finish 
		/// at TaskPool shutdown before we kill it
		/// set at max RPC timeout of 2 mins = 120 secs
		/// </summary>
		public const int MAX_THREAD_WAIT = 120000;

		/// <summary>
		/// Links TaskIDs to client notification callbacks
		/// For every Task request we store the callback provided by the
		/// client so they can be notified on completion
		/// </summary>
		private static Hashtable ClientNotification = new Hashtable();

		/// <summary>
		///	Stores the details of the TPTasks threads in the TaskPool 
		/// </summary>
		private static ArrayList lstTasks = new ArrayList();
		
		/// <summary>
		/// Condition variable used to synchronize TPTask threads running in 
		/// the TaskPool and ensure they abide by the leaders-followers model
		/// </summary>
		private static AutoResetSignalObject leaderSignal = new AutoResetSignalObject( false );
		private static ManualResetSignalObject workSignal = new ManualResetSignalObject( false );
			
		/// <summary>
		/// Indicates whether the TaskPool has been initialized or not
		/// </summary>
		private static bool bInitialized = false;
		
		/// <summary>
		/// Indicates whether the TaskPool is in the process of shutting down
		/// </summary>
		private static bool bShuttingDown = false;

		/// <summary>
		/// Never instantiate this class, there should be one TaskPool per
		/// process
		/// </summary>
		private TaskPool()
		{
		}

		/// <summary>
		/// Property indicating the number of active TPTask threads in the 
		/// TaskPool
		/// </summary>
		public static int Threads
		{
			get
			{ return TPTask.Tasks; }
		}
		
		/// <summary>
		/// Property indicating whether the TaskPool has been initialized 
		/// </summary>
		public static bool IsInitialized
		{
			get
			{ return bInitialized; }
		}
		
		/// <summary>
		/// Property indicating whether the TaskPool is shutting down 
		/// </summary>
		public static bool IsShuttingDown
		{
			get
			{ return bShuttingDown; }
		}
		
		
		/// <summary>
		/// TaskPool subscribes to the TPTaskComplete event. When a TPTask 
		/// thread is finished servicing a request, locate the 
		/// subscription of the client that queued the request and 
		/// notify them via the callback they provided as part of
		/// their subscription. 
		/// <seealso cref="TPTask"/>
		/// <seealso cref="TPTaskCompleteEventArgs"/>
		/// </summary>
		/// <param name="tceArg">TPTask completion event arguement</param>
		private static void OnTPTaskComplete( TPTaskCompleteEventArgs tceArg )
		{
			try
			{
				lock( ClientNotification.SyncRoot )
				{
					if( ClientNotification.ContainsKey( tceArg.TaskID ) )
					{
						TaskRequest.NotificationCallback objNotify = (TaskRequest.NotificationCallback) ClientNotification[tceArg.TaskID];
						if( objNotify != null )
						{
							// Notifiy specific client
							objNotify( tceArg );
							// Remove subscription
							ClientNotification.Remove( tceArg.TaskID );
						}
					}
				}
			}
			catch( Exception /*e*/ )
			{}
		}
		
		/// <summary>
		/// Function queues a TaskRequest to the TaskPool
		/// </summary>
		/// <param name="req">The TaskRequest to queue</param>
		/// <returns>A Guid used as a TaskID - if the TaskPool is in the process of
		/// Shutting down then Guid.Empty is returned</returns>
		/// <exception cref="ArgumentNullException">Thrown if TaskRequest
		/// is null or not valid</exception>
		public static Guid QueueTask( TaskRequest req )
		{
			if( req == null || !req.IsValid )
				throw new ArgumentNullException( "TaskRequest is null or contains null valued callbacks" );
			
			// If taskpool is shutting down then return Guid.Empty
			if( bShuttingDown )
				return Guid.Empty;
			
			if( !bInitialized )
				Initialize();

			Guid taskID = Guid.Empty;

			try
			{
				/*lock( TPTask.requestQ.SyncRoot )
				{
					// Generate Guid - task id if none exists
					if( req.TaskID == Guid.Empty )
						req.TaskID = Guid.NewGuid();
					
					taskID = req.TaskID;

					// Add request item to TPTask requestQ
					TPTask.requestQ.Enqueue( req );
				}// End-lock on TPTask.requestQ
				*/
				
				// Generate Guid - task id if none exists
				if( req.TaskID == Guid.Empty )
					req.TaskID = Guid.NewGuid();
					
				taskID = req.TaskID;

				TPTask.QueueTaskRequest( req );
				
				TaskPool.workSignal.Signal();
				
				// Tag taskID to notification callback
				lock( ClientNotification.SyncRoot )
				{
					if( taskID != Guid.Empty && req.NotifyCb != null )
						ClientNotification.Add( taskID, req.NotifyCb );
				}// End-lock on ClientNotification
			}
			catch( Exception /*e*/ )
			{}

			// Return taskID to client
			return taskID;	
		}
				
		// Get Taskpool started by kicking off some tasks
		/// <summary>
		/// Procedure gets the TaskPool started by kicking off a few
		/// TPTask threads.
		/// </summary>
		public static void Initialize()
		{
			if( TPTask.Tasks != 0 )
				return;

			try
			{
				lock( lstTasks.SyncRoot )
				{
					// If shutting down then exit
					if( bShuttingDown )
						return;
			
					// If already initialized then exit
					if( bInitialized )
						return;
					
					// Hook up event handlers and flag initialized
					if( !bInitialized )
					{
						TPTask.TPTaskComplete += new TPTask.TPTaskCompleteHandler( OnTPTaskComplete );
						bInitialized = true;
					}

					for( int i = 0; i < MIN_ACTIVE_TASKS; i++ )
					{
						TPTaskInit init = new TPTaskInit();
						// Create new TPTask
						init.Task = new TPTask( ref leaderSignal, ref workSignal );
						// Keep thread in ThreadPool
						init.Task.RecycleTask = true;
						// Create worker thread
						init.Worker = new Thread( new ThreadStart( init.Task.Service ) );
						// Start worker 
						init.Worker.Start();
						// Add this task to our list of tasks
						lstTasks.Add( init );
					}
				}//End-Lock on lstTasks
			}//End-try
			catch( Exception /*e*/ )
			{}
		}

		/// <summary>
		/// Procedure shuts down the TaskPool by allowing the TPTask threads
		/// running to finish what they are doing and exit the TaskPool(up to MAX_THREAD_WAIT) 
		/// before aborting each one.
		/// </summary>
		/// <remarks>To shutdown the TaskPool, iterate thru the list of 
		/// TPTasks and set their RecycleTask property to false, 
		/// so they terminate after they finish processing their current
		/// request and wait (up to MAX_THREAD_WAIT) on each TPTask thread 
		/// to exit before aborting the TPTask thread.
		/// </remarks>
		public static void Shutdown()
		{
			lock( lstTasks.SyncRoot )
			{
				// Signal that we are shutting down
				bShuttingDown = true;

				workSignal.AllowReset = false;
				workSignal.Signal();
				
				for( int i = 0; i < lstTasks.Count; i++ )
				{
					try
					{
						TPTaskInit init = (TPTaskInit) lstTasks[i];
						// Do not recycle task
						init.Task.RecycleTask = false;
						// Wait on thread to exit.
						// Instead of waiting indefinitely
						// we may have have to interrupt it and 
						// catch the exception
						init.Worker.Join( MAX_THREAD_WAIT );
						if( init.Worker.IsAlive )
							init.Worker.Abort();
					}
					catch( Exception e )
					{
						string strMsg = e.Message;
					}
				}
								
				// Clear all tasks
				lstTasks.Clear();
				// Clear all client subscriptions
				ClientNotification.Clear();
				// Set threadpool as uninitialized
				bInitialized = false;
			}//End-lock lstTasks
		}//End-Shutdown
	}//End-TaskPool
}//End-namespace PSL.AsyncCore
