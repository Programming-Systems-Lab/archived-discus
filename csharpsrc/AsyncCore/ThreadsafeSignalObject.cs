using System;
using System.Threading;

namespace PSL.AsyncCore
{
	/// <summary>
	/// Summary description for ThreadsafeSignalObject.
	/// </summary>
	public abstract class ThreadsafeSignalObject:ISignalObject
	{
		public ThreadsafeSignalObject()
		{
		}

		public abstract void Signal();
		public abstract void Reset();
		public abstract void WaitOnSignal();
		public abstract void WaitOnSignal( int nMillisecondsTimeout, bool bExitContext );
		public abstract void WaitOnSignal( TimeSpan timeSpan, bool bExitContext );
	}
}
