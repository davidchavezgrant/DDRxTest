using System.Reactive.Concurrency;

using ReactiveUI;

namespace DynamicDataTest.Web.Data.Trades;

internal sealed class SchedulerProvider
{
	public IScheduler MainThread => RxApp.MainThreadScheduler;
	public IScheduler Background => RxApp.TaskpoolScheduler;
}