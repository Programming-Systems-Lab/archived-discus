using System;
using DynamicPxy;
using PSL.AsyncCore;
using System.Threading;

namespace AsyncCoreUnittest
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		private readonly string m_strZip1 = "10025";
		private readonly string m_strZip2 = "98055";

		private object Task1( object objCtx )
		{
			// Do some web service invocations
			WeatherRetriever wr = new WeatherRetriever();
			
			//throw new Exception( "just becuz" );

			object objRes = wr.GetTemperature( this.m_strZip1 );

			return objRes;
		}

		private object Task2( object objCtx )
		{
			// Do some web service invocations

			WeatherRetriever wr = new WeatherRetriever();
			object objRes = wr.GetTemperature( this.m_strZip2 );

			return objRes;
		}

		private object Task3( object objCtx )
		{
			WeatherRetriever wr = new WeatherRetriever();
			object objRes = wr.GetWeather( this.m_strZip2 );

			return objRes;
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Class1 c = new Class1();
			int nThreads = TaskPool.Threads;
			
			AutoResetEvent condition = new AutoResetEvent( false );
			
			
			TaskPool.QueueTask( new TaskRequest( new TaskRequest.TaskCallback( c.Task1 ), new TaskRequest.NotificationCallback( c.OnTaskDone ) ) );
			TaskPool.QueueTask( new TaskRequest( new TaskRequest.TaskCallback( c.Task2 ), new TaskRequest.NotificationCallback( c.OnTaskDone ) ) );
			TaskPool.QueueTask( new TaskRequest( new TaskRequest.TaskCallback( c.Task3 ), new TaskRequest.NotificationCallback( c.OnTaskDone ) ) );
			// Create a task proxy
			/*TaskProxy pxy = new TaskProxy( ref condition );

			// Give the proxy tasks to wait on
			Guid task1 = pxy.AddTaskRequest( new TaskRequest( new TaskRequest.TaskCallback( c.Task1 ), new TaskRequest.NotificationCallback( c.OnTaskDone ) ) );
			Guid task2 = pxy.AddTaskRequest( new TaskRequest( new TaskRequest.TaskCallback( c.Task2 ), new TaskRequest.NotificationCallback( c.OnTaskDone ) ) );
			Guid task3 = pxy.AddTaskRequest( new TaskRequest( new TaskRequest.TaskCallback( c.Task3 ), new TaskRequest.NotificationCallback( c.OnTaskDone ) ) );
			
			// Let the proxy begin to wait on the queued requests
			pxy.WaitOnTasks();
			
			// Do misc operations in the meantime then...

			// Wait until proxy signals results are ready
			condition.WaitOne();
			
			// Get results
			TPTaskCompleteEventArgs e1 = pxy.QueryResult( task1 );
			TPTaskCompleteEventArgs e2 = pxy.QueryResult( task2 );
			TPTaskCompleteEventArgs e3 = pxy.QueryResult( task3 );
			*/
			TaskPool.Shutdown();
		}

		void OnTaskDone( TPTaskCompleteEventArgs tce )
		{
			object objRes = null;
			
			if( !tce.HasErrors )
				objRes = tce.Result;
			else objRes = tce.ExceptionMessage;
			objRes = tce.HasErrors ? tce.ExceptionMessage : tce.Result;

			Console.WriteLine( objRes.ToString() );
		}
	}
}
