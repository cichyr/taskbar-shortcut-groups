using System.Diagnostics;
using System.Text.Json;
using TaskbarShortcutGroups.Common.Models.Serialization;
using TaskbarShortcutGroups.Common.Services;

namespace TaskbarShortcutGroups.Common.Providers;

public class VersionProvider : IVersionProvider
{
    private const string RepositoryUrl = "repos/cichyr/taskbar-shortcut-groups";
    private string? updatedVersion;
    private FileVersionInfo? versionInfo;

    public VersionProvider(ITaskService taskService)
    {
        ArgumentNullException.ThrowIfNull(taskService);
        var cancellationTokenSource = new CancellationTokenSource();
        taskService.RegisterTask(Task.Run(() => FetchReleaseVersion(cancellationTokenSource.Token), CancellationToken.None), cancellationTokenSource);
    }

    private FileVersionInfo VersionInfo => versionInfo ??= FileVersionInfo.GetVersionInfo(Environment.ProcessPath!);

    public bool UpdateDetected => updatedVersion != null;

    /// <inheritdoc />
    public string NewestVersion
    {
        get => updatedVersion ?? ProductVersion;
        private set
        {
            updatedVersion = value;
            NewerVersionDetected?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <inheritdoc />
    public string ProductVersion => VersionInfo.ProductVersion!;

    /// <inheritdoc />
    public event EventHandler? NewerVersionDetected;

    private async Task FetchReleaseVersion(CancellationToken cancellationToken = default)
    {
        try
        {
            using var githubClient = new HttpClient(); // This is the only request to be sent during app lifetime
            githubClient.BaseAddress = new Uri("https://api.github.com/");
            githubClient.DefaultRequestHeaders.Add("accept", "application/vnd.github+json");
            githubClient.DefaultRequestHeaders.Add("User-Agent", "cichyr_TaskbarShortcutGroups");
            using var responseMessage = await githubClient.GetAsync($"{RepositoryUrl}/releases", cancellationToken);
            if (!responseMessage.IsSuccessStatusCode)
                return;
            var content = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
            var releases = await JsonSerializer.DeserializeAsync(content, SourceGenerationContext.Default.GithubReleaseArray, cancellationToken);
            var newestRelease = releases?.FirstOrDefault(x => !x.IsPreRelease); // Should be returned ordered by date
            if (newestRelease != null && newestRelease.Name != ProductVersion)
                NewestVersion = newestRelease.Name;
        }
        catch (OperationCanceledException)
        {
            // Thrown when cancellation is requested
        }
    }
}