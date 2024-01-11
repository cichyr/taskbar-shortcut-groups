using System.Text.Json.Serialization;

namespace TaskbarShortcutGroups.Common.Models.Serialization;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ShortcutGroupsDefinition))]
internal partial class SourceGenerationContext : JsonSerializerContext;