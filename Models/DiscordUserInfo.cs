using Newtonsoft.Json;

namespace SnackShackAPI.Models
{
    public class DiscordUserInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
