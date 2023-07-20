using Newtonsoft.Json;

namespace YouTubeBot.Config
{
    internal struct ConfigJSON
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("prefix")]
        public string Prefix { get; set; }
    }
}
