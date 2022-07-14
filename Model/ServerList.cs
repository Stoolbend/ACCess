using System.Text.Json.Serialization;

namespace ACCess.Model
{
    public class ServerList
    {
        [JsonPropertyName("leagueServerIP")]
        public string LeagueServerIp { get; set; }
    }
}
