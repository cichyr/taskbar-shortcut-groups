using TaskbarShortcutGroups.Common.Extensions;

namespace TaskbarShortcutGroups.Common.Services;

/// <summary>
/// This service is meant to manage fire-and-forget tasks to prevent memory leaks and handle closing the application before task ends.
/// </summary>
public class TaskService : ITaskService
{
    private readonly HashSet<TaskInfo> taskInfos = [];
    private readonly HashSet<Task> tasks = [];

    /// <inheritdoc />
    public void RegisterTask(Task task, CancellationTokenSource? cancellationTokenSource = null)
    {
        var taskInfo = new TaskInfo {Task = task, CancellationTokenSource = cancellationTokenSource};
        tasks.Add(task);
        tasks.Add(taskInfo.Task.ContinueWith(_ => FinalizeTask(taskInfo)));
        taskInfos.Add(taskInfo);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        taskInfos.ForEach(x => x.CancellationTokenSource?.Cancel());
        Task.WaitAll(tasks.WhereNotNull().ToArray());
        GC.SuppressFinalize(this);
    }

    private void FinalizeTask(TaskInfo taskInfo)
    {
        tasks.Remove(taskInfo.Task!);
        taskInfos.Remove(taskInfo);
        taskInfo.Dispose();
    }

    private record TaskInfo : IDisposable
    {
        private readonly object lockObject = new();
        private CancellationTokenSource? cancellationTokenSource;
        private bool isDisposed;
        private Task? task;

        public Task? Task
        {
            get
            {
                lock (lockObject)
                {
                    return task;
                }
            }
            init
            {
                lock (lockObject)
                {
                    task = value;
                }
            }
        }

        public CancellationTokenSource? CancellationTokenSource
        {
            get
            {
                lock (lockObject)
                {
                    return cancellationTokenSource;
                }
            }
            init
            {
                lock (lockObject)
                {
                    cancellationTokenSource = value;
                }
            }
        }

        public void Dispose()
        {
            lock (lockObject)
            {
                if (isDisposed)
                    return;
                isDisposed = true;
                task?.Dispose();
                task = null;
                cancellationTokenSource?.Dispose();
                cancellationTokenSource = null;
            }
        }
    }
}