namespace TaskbarShortcutGroups.Common.Services;

/// <summary>
/// This service is meant to manage fire-and-forget tasks to prevent memory leaks and handle closing the application before task ends.
/// </summary>
public interface ITaskService : IDisposable
{
    /// <summary>
    /// Registers the given fire-and-forget tasks and ensures cleanup after finalization.
    /// </summary>
    /// <param name="task"> The task. </param>
    /// <param name="cancellationTokenSource"> The cancellation token source. </param>
    void RegisterTask(Task task, CancellationTokenSource? cancellationTokenSource = null);
}