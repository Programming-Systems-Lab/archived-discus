using System;
using System.Threading;

namespace PSL.AsyncCore
{
	/// <summary>
	/// Summary description for ManualResetSignalObject.
	/// </summary>
	public class ManualResetSignalObject:ThreadsafeSignalObject
	{
		private ManualResetEvent m_signal = null;
		private bool m_bAllowReset = true;
		
		public ManualResetSignalObject( bool bInitialState )
		{
			this.m_signal = new ManualResetEvent( bInitialState );
		}
		
		public bool AllowReset
		{
			get
			{ return this.m_bAllowReset; }

			set
			{
				lock( this )
				{
					this.m_bAllowReset = value;
				}
			}
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
			if( !m_bAllowReset )
				return;

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
