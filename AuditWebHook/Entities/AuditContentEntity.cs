using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditWebHook.Entities
{
    public struct AuditContentEntity
    {
        [JsonProperty(PropertyName = "clientId")]
        public string ClientId { get; set; }

        [JsonProperty(PropertyName = "contentCreated")]
        public DateTime ContentCreated { get; set; }

        [JsonProperty(PropertyName = "contentExpiration")]
        public DateTime ContentExpiration { get; set; }

        [JsonProperty(PropertyName = "contentId")]
        public string ContentId { get; set; }

        [JsonProperty(PropertyName = "contentType")]
        public string ContentType { get; set; }

        [JsonProperty(PropertyName = "contentUri")]
        public string ContentUri { get; set; }

        [JsonProperty(PropertyName = "tenantId")]
        public string TenantId { get; set; }


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
