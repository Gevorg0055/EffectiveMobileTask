using System.Text.Json.Serialization;

namespace EffectiveMobileTask
{
    public class JsonAddresses
    {
        [JsonPropertyName("address")]
        public LastAssignableHost? CalculatedAssignableHost { get; set; }
    }
}
