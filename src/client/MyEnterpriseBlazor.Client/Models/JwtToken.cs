using System.Text.Json.Serialization;

namespace MyEnterpriseBlazor.Client.Models
{
    public class JwtToken
    {
        [JsonPropertyName("id_token")]
        public string IdToken { get; set; }
    }
}
