using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EffectiveMobileTask
{
    public class LastAssignableHost
    {
        [JsonPropertyName("last_assignable_host")]
        public string? Last_assignable_host { get; set; }
    }
}
