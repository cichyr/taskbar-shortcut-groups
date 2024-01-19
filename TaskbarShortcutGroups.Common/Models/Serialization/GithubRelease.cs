using System.Text.Json.Serialization;

namespace TaskbarShortcutGroups.Common.Models.Serialization;

public class GithubRelease
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("published_at")]
    public DateTime PublishedAt { get; set; }
    
    [JsonPropertyName("prerelease")]
    public bool IsPreRelease { get; set; }
}