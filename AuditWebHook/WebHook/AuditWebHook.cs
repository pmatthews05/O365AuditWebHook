using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditWebHook.WebHooks
{
    public struct AuditWebhook
    {
        [JsonProperty(PropertyName = "webhook")]
        public WebHook WebHook { get; set; }
    }

    public struct WebHook
    {
        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }
        [JsonProperty(PropertyName = "authId")]
        public string AuthId { get; set; }
        [JsonProperty(PropertyName = "expiration")]
        public DateTime? Expiration { get; set; }
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
    }
}
