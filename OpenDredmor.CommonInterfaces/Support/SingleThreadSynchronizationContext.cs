using System.Collections.Concurrent;

namespace OpenDredmor.CommonInterfaces.Support;

public class SingleThreadSynchronizationContext : SynchronizationContext
{
    sealed record WorkItem(SendOrPostCallback Callback, object? State, ManualResetEventSlim? ResetEvent)
    {
        public void Execute()
        {
            Callback(State);
            ResetEvent?.Set();
        }
    }
    readonly ConcurrentQueue<WorkItem> workItems = [];
    readonly Thread thread = Thread.CurrentThread;

    public override void Post(SendOrPostCallback d, object? state)
    {
        workItems.Enqueue(new(d, state, null));
    }

    WorkItem? ExecuteAndReturnNextWorkItem()
    {
        if (workItems.TryDequeue(out var currentItem))
            currentItem.Execute();
        return currentItem;
    }

    public override void Send(SendOrPostCallback d, object? state)
    {
        if (Thread.CurrentThread == thread)
        {
            WorkItem requestedWorkItem = new(d, state, null);
            workItems.Enqueue(requestedWorkItem);

            WorkItem? executedWorkItem;
            do
            {
                executedWorkItem = ExecuteAndReturnNextWorkItem();
            } while (executedWorkItem is not null && executedWorkItem != requestedWorkItem);
        }
        else
        {
            using ManualResetEventSlim resetEvent = new(false);
            workItems.Enqueue(new(d, state, resetEvent));
            resetEvent.Wait();
        }
    }

    public void ExecutePendingWorkItems()
    {
        while (ExecuteAndReturnNextWorkItem() is not null) { }
    }
}
