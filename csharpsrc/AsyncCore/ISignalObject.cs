using System;

namespace PSL.AsyncCore
{
	/// <summary>
	/// Summary description for ISignalObject.
	/// </summary>
	public interface ISignalObject
	{
		void Signal();
		void Reset();
		void WaitOnSignal();
		void WaitOnSignal( int nMillisecondsTimeout, bool bExitContext );
		void WaitOnSignal( TimeSpan timeout, bool bExitContext );
	}
}
