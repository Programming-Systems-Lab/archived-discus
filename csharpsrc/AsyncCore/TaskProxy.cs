using System;
using System.Threading;
using System.Collections;

namespace PSL.AsyncCore
{
	/// <summary>
	/// Status enumeration of TaskProxies
	/// </summary>
	public enum enuTaskProxyStatus
	{
		AcceptingRequests,
		WaitingOnResults,
		ResultsReady
	};
	
	/// <summary>
	/// A TaskProxy will accept a set of TaskRequests,
	/// Queue them in the order received to the TaskPool and keep a hashtable
	/// indexed by the TaskIDs returned from the TaskPool queue operation.
	/// The TaskProxy will provide a callback to be notified on
	/// when a task request is serviced in the TaskPool.
	/// The TaskProxy is created with a condition variable with which
	/// it will signal when all results are available.
	/// Once a TaskProxy starts queuing requests, no more TaskRequests can
	/// be accepted
	/// </summary>
	public sealed class TaskProxy
	{
		/// <summary>
		/// Condition variable used to reference client provided
		/// conditon variable
		/// </summary>
		private AutoResetEvent m_condition = null;
		
		/// <summary>
		/// Queue of TaskReqests
		/// </summary>
		private Queue m_requestQ = new Queue();

		/// <summary>
		/// Hashtable of results indexed by TaskID
		/// </summary>
		private Hashtable m_results = new Hashtable();

		/// <summary>
		/// Number of tasks the TaskProxy has to wait on
		/// </summary>
		private int m_nTasksPending = 0;

		/// <summary>
		/// TaskProxy status, initialzed to enuTaskProxyStatus.AcceptingRequests;
		/// </summary>
		private enuTaskProxyStatus m_status = enuTaskProxyStatus.AcceptingRequests;

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="condition">Reference to a conditon variable
		/// the TaskProxy will signal on when all TaskRequests queued
		/// are ready.</param>
		public TaskProxy( ref AutoResetEvent condition )
		{
			// Set condition to non-signaled
			condition.Reset();
			m_condition = condition;
		}

		/// <summary>
		/// Property gets the number of TasksPending (number of tasks the 
		/// TaskProxy has to wait on)
		/// </summary>
		public int TasksPending
		{
			get
			{ return m_nTasksPending; }
		}
		
		// Do we really want to do this? - Support multiple callers on a 
		// single proxy - if so we may want to add an event to notify clients
		// when a proxy is being reset so that it passes what data it has
		// in the reset event
		//public AutoResetEvent Condition
		//{
		//	get
		//	{ return m_condition; }
		//}
		

		public void WaitOnProxySignal()
		{
			if( m_nTasksPending == 0 )
				return;
			else m_condition.WaitOne();
		}

		public void WaitOnProxySignal( TimeSpan timeout, bool bExitContext )
		{
			if( m_nTasksPending == 0 )
				return;
			else m_condition.WaitOne( timeout, bExitContext );
		}

		public void WaitOnProxySignal( int nTimeout, bool bExitContext )
		{
			if( m_nTasksPending == 0 )
				return;
			else m_condition.WaitOne( nTimeout, bExitContext );
		}

		/// <summary>
		/// Procedure represents the notification callback provided by the
		/// TaskProxy. This callback is invoked whenever a task the TaskProxy
		/// is waiting on has been serviced by the TaskPool. When all the tasks the
		/// proxy is waiting on have been serviced it will signal external clients 
		/// using the condition variable reference passed from the client at 
		/// TaskProxy construction.
		/// </summary>
		/// <param name="tceArg"></param>
		private void OnTPTaskComplete( TPTaskCompleteEventArgs tceArg )
		{
			lock( m_results.SyncRoot )
			{
				// Index results by TaskID
				m_results.Add( tceArg.TaskID, tceArg );
				m_nTasksPending--;

				if( m_nTasksPending == 0 )
				{
					// Indicate results are ready by changing state
					m_status = enuTaskProxyStatus.ResultsReady;
					// Signal to any waiting client
					m_condition.Set();
				}
			}
		}

		/// <summary>
		/// Function adds to the list of TaskRequests a TaskProxy will
		/// wait on.
		/// </summary>
		/// <param name="req">The TaskRequest to wait on</param>
		/// <returns>The Guid (TaskID) associated with the request.
		/// This TaskID is used to query for results once the 
		/// TaskProxy signals the client that all task requests have
		/// been serviced.</returns>
		/// <remarks>Note: The TaskRequest queued may or may not have set
		/// a notification callback, if one has been set the TaskProxy will
		/// REPLACE it with its own notification callback.</remarks>
		/// <exception cref="ArgumentNullException">Thrown if a null
		/// TaskRequest is added</exception>
		public Guid AddTaskRequest( TaskRequest req )
		{
			if( req == null )
				throw new ArgumentNullException( "TaskRequest is null" );
			
			// Assign taskID if we have to
			if( req.TaskID == Guid.Empty )
				req.TaskID = Guid.NewGuid();
			
			Guid taskID = req.TaskID;
			
			// Enqueue task
			lock( m_requestQ.SyncRoot )
			{
				if( m_status != enuTaskProxyStatus.AcceptingRequests )
					throw new InvalidOperationException( "Proxy is no longer accepting requests - Check TaskProxy status" );

				// Link notify callback to TaskProxy
				// note use of = instead of +=
				// single TaskProxy is sole subscriber
				// to this request being serviced
				req.NotifyCb = new TaskRequest.NotificationCallback( this.OnTPTaskComplete );
				m_requestQ.Enqueue( req );
				m_nTasksPending++;
			}

			return taskID;
		}

		/// <summary>
		/// Procedure resets the state of a TaskProxy.
		/// Clears: The request queue, the results table, and the TaskProxy
		/// state.
		/// </summary>
		public void Reset()
		{
			lock( this )
			{
				m_requestQ.Clear();
				m_results.Clear();
				m_status = enuTaskProxyStatus.AcceptingRequests;
				m_condition.Reset();
			}
		}
		
		/// <summary>
		/// Procedure begins the TaskProxy wait operation.
		/// TaskProxy will send all queued tasks to the TaskPool for
		/// servicing and change its state to enuTaskProxyStatus.WaitingOnResults.
		/// Once the state change takes place the TaskProxy will no longer accept
		/// task requests hence calling AddTaskRequest after this state change will
		/// result in an InvalidOperationException being thrown.
		/// </summary>
		public void StartWait()
		{
			lock( m_requestQ.SyncRoot )
			{
				// If we are not about to transition from AcceptingRequests
				// then we have nothing to do here, so exit
				if( m_status != enuTaskProxyStatus.AcceptingRequests )
					return; 
				
				// Pass all the requests we have to the TaskPool
				// do not empty the requestQ, we need to preserve the ordering
				// just in case external clients don't
				IEnumerator it = m_requestQ.GetEnumerator();
				while( it.MoveNext() )
				{
					TaskPool.QueueTask( (TaskRequest) it.Current );
				}
				
				// Once Wait is called, Proxy no longer accepts new requests
				// we indicate this by changing state
				m_status = enuTaskProxyStatus.WaitingOnResults;
			}
		}

		/// <summary>
		/// Function allows clients of a TaskProxy to query for the results
		/// of a specific TaskID.
		/// </summary>
		/// <param name="taskID">TaskID associated with a TaskRequest that
		/// was queued for servicing</param>
		/// <returns>The TaskCompleteEventArgs representing the results
		/// of servicing.</returns>
		public TPTaskCompleteEventArgs QueryResult( Guid taskID )
		{
			if( m_status != enuTaskProxyStatus.ResultsReady )
				throw new InvalidOperationException( "Cannot query TaskProxy for results until results ready - check status first or wait on condition variable for signal" );

			lock( m_results.SyncRoot )
			{
				if( m_results.ContainsKey( taskID ) )
				{
					return (TPTaskCompleteEventArgs) m_results[taskID];
				}
			}
			return null;
		}
	
	}
}
