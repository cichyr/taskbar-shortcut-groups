using System.Text.Json.Serialization;

namespace TaskbarShortcutGroups.Common.Models.Serialization;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ShortcutGroupsDefinition))]
[JsonSerializable(typeof(GithubRelease[]))]
internal partial class SourceGenerationContext : JsonSerializerContext;