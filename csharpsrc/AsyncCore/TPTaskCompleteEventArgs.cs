using System;

namespace PSL.AsyncCore
{
	/// <summary>
	/// TPTaskCompleteEventArgs - one of these classes is created each time a 
	/// TPTask thread finishes servicing a TaskRequest (invoking its TaskCallback)
	/// <seealso cref="TaskRequest"/>
	/// <seealso cref="TaskPool"/>
	/// </summary>
	public sealed class TPTaskCompleteEventArgs:EventArgs
	{
		// May be object or string (exception message)
				
		private object m_objResult = null;
		private bool m_bErrorsOccurred = false;
		private Guid m_taskID = Guid.Empty;

		/// <summary>
		/// Ctor
		/// </summary>
		public TPTaskCompleteEventArgs()
		{
		}

		/// <summary>
		/// Property indicates whether errors occurred when the TaskRequest
		/// was being serviced.
		/// </summary>
		public bool HasErrors
		{
			get
			{ return m_bErrorsOccurred; }
			set
			{ m_bErrorsOccurred = true; }
		}

		/// <summary>
		/// Property gets/sets the results of a TaskRequest being completed
		/// in the event of errors this is set to the ExceptionMessage property
		/// </summary>
		public object Result
		{
			get
			{ return m_objResult; }
			set
			{ m_objResult = value; }
		}

		/// <summary>
		/// gets/sets the TaskID of a TaskRequest
		/// </summary>
		public Guid TaskID
		{
			get
			{ return m_taskID; }
			set
			{ m_taskID = value; }
		}

		/// <summary>
		/// gets the Exception Message which may have beed set during exection of a
		/// TaskRequest. Cannot be set by client.
		/// </summary>
		public string ExceptionMessage
		{
			get
			{
				if( !m_bErrorsOccurred )
					return "";

				if( m_objResult == null )
					return "";
				else return (string) m_objResult;
			}
		}
	}
}
