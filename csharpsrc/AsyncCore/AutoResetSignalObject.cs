using System;
using System.Threading;

namespace PSL.AsyncCore
{
	/// <summary>
	/// Summary description for AutoResetSignalObject.
	/// </summary>
	public class AutoResetSignalObject:ThreadsafeSignalObject
	{
		private AutoResetEvent m_signal = null;
		
		public AutoResetSignalObject( bool bInitialState )
		{
			this.m_signal = new AutoResetEvent( bInitialState );
		}

		public override void Signal()
		{
			lock( this )
			{
				this.m_signal.Set();
			}
		}

		public override void Reset()
		{
			lock( this )
			{
				this.m_signal.Reset();
			}
		}

		public override void WaitOnSignal()
		{
			this.m_signal.WaitOne();
		}

		public override void WaitOnSignal( int nMillisecondsTimeout, bool bExitContext )
		{
			this.m_signal.WaitOne( nMillisecondsTimeout, bExitContext );
		}

		public override void WaitOnSignal( TimeSpan timeout, bool bExitContext )
		{
			this.m_signal.WaitOne( timeout, bExitContext );
		}
	}
}
