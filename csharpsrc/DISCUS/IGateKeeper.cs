using System;

namespace PSL.DISCUS.Interfaces
{
	/// <summary>
	/// GateKeeper interface
	/// </summary>
	public interface IGatekeeper:IResourceAcquire,IExecuteService,IExecuteWorkflow,ILogger,ITracer
	{
	}
}