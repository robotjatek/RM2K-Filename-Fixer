using System.Text.Json.Serialization;

namespace RMFileNameFixer.Model
{
    public class RmFixerOptions
    {
        [JsonPropertyName("locale")]
        public required Dictionary<string, Dictionary<string, char>> Locale { get; init; }

        [JsonPropertyName("fallback_locale")]
        public required string FallbackLocale { get; init; }

        [JsonPropertyName("rm_folders")]
        public required string[] RmFolders { get; init; }
    }
}
