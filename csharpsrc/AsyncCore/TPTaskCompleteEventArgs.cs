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

		public object Result
		{
			get
			{ return m_objResult; }
			set
			{ m_objResult = value; }
		}

		public Guid TaskID
		{
			get
			{ return m_taskID; }
			set
			{ m_taskID = value; }
		}

		// Cannot be set
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
