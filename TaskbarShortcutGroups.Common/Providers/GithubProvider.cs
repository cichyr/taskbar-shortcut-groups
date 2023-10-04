namespace TaskbarShortcutGroups.Common.Providers;

/// <summary>
/// The provider used to fetch data from Github.
/// </summary>
public class GithubProvider
{
    private const string RepositoryUrl = "https://github.com/cichyr/taskbar-shortcut-groups";

    /// <summary>
    /// Returns the newest application version available on GitHub.
    /// </summary>
    /// <returns></returns>
    public async Task<HttpResponseMessage> FetchNewestReleaseVersion()
    {
        var githubClientHandler = new HttpClientHandler();
        githubClientHandler.UseProxy = false;
        using var githubClient = new HttpClient(githubClientHandler);
        githubClient.BaseAddress = new Uri("https://api.github.com/");
        githubClient.DefaultRequestHeaders.Add("accept", "application/vnd.github+json");
        githubClient.DefaultRequestHeaders.Add("User-Agent", "TaskbarShortcutGroup");
        var apiAddress = new Uri($"{RepositoryUrl}/releases");
        var releases = await githubClient.GetAsync(apiAddress);
        var content = await releases.Content.ReadAsStringAsync();
        return releases;
    }
}