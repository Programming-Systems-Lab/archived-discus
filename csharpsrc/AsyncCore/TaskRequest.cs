using System;

namespace PSL.AsyncCore
{
	/// <summary>
	/// A TaskRequest encapsulates a function call to make (delegate to invoke)
	/// and a notification callback which will be invoked after the delegate is
	/// invoked with the results of the delegate invocation.
	/// </summary>
	public sealed class TaskRequest
	{
		/// <summary>
		/// Notification callback
		/// </summary>
		public delegate void NotificationCallback( TPTaskCompleteEventArgs tceArg );
		
		/// <summary>
		/// Delegate wrapping the function to be called
		/// </summary>
		public delegate object TaskCallback( object objCtx );
		
		/// <summary>
		/// Task callback (delegate) instance
		/// </summary>
		private TaskCallback m_objTaskCb = null;
		
		/// <summary>
		/// TaskID of this request - usually set by client
		/// </summary>
		private Guid m_taskID = Guid.Empty;
		
		/// <summary>
		/// Notification callback (delegate) instance
		/// </summary>
		private NotificationCallback m_objNotifyCb = null;
		
		/// <summary>
		/// Context object instance passed to TaskCallback instance and
		/// used when the TaskCallback is invoked.
		/// </summary>
		private object m_objCtx = null;

		/// <summary>
		/// Ctor
		/// </summary>
		public TaskRequest()
		{
		}

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="objTaskCb">TaskCallback to be used in this TaskRequest</param>
		public TaskRequest( TaskCallback objTaskCb )
		{
			m_objTaskCb = objTaskCb;
		}

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="objTaskCb">TaskCallback to be used in this TaskRequest</param>
		/// <param name="objCtx">Context object used with the TaskCallback</param>
		public TaskRequest( TaskCallback objTaskCb, object objCtx )
		{
			m_objTaskCb = objTaskCb;
			m_objCtx = objCtx;
		}

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="objTaskCb">TaskCallback to be used in this TaskRequest</param>
		/// <param name="objNotifyCb">Notification callback which will be invoked
		/// after the TaskCallback is invoked</param>
		public TaskRequest( TaskCallback objTaskCb, NotificationCallback objNotifyCb )
		{
			m_objTaskCb = objTaskCb;
			m_objNotifyCb = objNotifyCb;
		}

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="objTaskCb">TaskCallback to be used in this TaskRequest</param>
		/// <param name="objNotifyCb">Notification callback which will be invoked
		/// after the TaskCallback is invoked</param>
		/// <param name="objCtx">Context object used with the TaskCallback</param>
		public TaskRequest( TaskCallback objTaskCb, NotificationCallback objNotifyCb, object objCtx )
		{
			m_objTaskCb = objTaskCb;
			m_objNotifyCb = objNotifyCb;
			m_objCtx = objCtx;
		}

		/// <summary>
		/// Property indicates whether a TaskRequest is valid. A TaskRequest needs
		/// to have a TaskCallback and a Notification Callback to be considered 
		/// valid
		/// </summary>
		public bool IsValid
		{
			get
			{ return m_objTaskCb != null; } 
		}
		
		/// <summary>
		/// Property gets/sets the context object of the TaskRequest
		/// </summary>
		public object Context
		{
			get
			{ return m_objCtx; }
			set
			{ m_objCtx = value; }
		}

		/// <summary>
		/// Property gets/sets the TaskCallback of this TaskRequest
		/// </summary>
		public TaskCallback TaskCb
		{
			get
			{ return m_objTaskCb; }
			set
			{ 
				if( value == null )
					throw new ArgumentNullException( "TaskCb", "Property cannot be set to null" );
			
				m_objTaskCb = value; 
			}
		}

		/// <summary>
		/// Property gets/sets the Notification callback of this TaskRequest
		/// </summary>
		public NotificationCallback NotifyCb
		{
			get
			{ return m_objNotifyCb; }
			set
			{	
				if( value == null )
					throw new ArgumentNullException( "NotifyCb", "Property cannot be set to null" );
				m_objNotifyCb = value; 
			}
		}

		/// <summary>
		/// Property gets/sets the TaskID of the TaskRequest
		/// </summary>
		public Guid TaskID
		{
			get
			{ return m_taskID; }
			set
			{ m_taskID = value; }
		}
	}
}
