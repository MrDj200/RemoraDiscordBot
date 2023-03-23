using System.Text.Json.Serialization;

namespace BotConsole.Models
{
    public class VRCXMeta
    {
        public string? application { get; set; }
        public int version { get; set; }

        [JsonPropertyName("author")]
        public VRCAuthor? author { get; set; }

        [JsonPropertyName("world")]
        public VRCWorld? world { get; set; }

        [JsonPropertyName("players")]
        public VRCPlayer[]? players { get; set; }
    }


    public class VRCAuthor
    {
        public string? id { get; set; }
        public string? displayName { get; set; }
    }
        
    public class VRCWorld
    {
        public string? name { get; set; }
        public string? id { get; set; }
        public string? instanceId { get; set; }
    }

    public class VRCPlayer
    {
        public string? id { get; set; }
        public string? displayName { get; set; }
    }
}