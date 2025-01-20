using Newtonsoft.Json;

namespace SnackShackAPI.Models
{
    public class DiscordGuild
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
