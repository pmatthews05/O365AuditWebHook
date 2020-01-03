using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditWebHook.Queue
{
    public struct AuditContentQueue
    {
        [JsonProperty(PropertyName = "contentType")]
        public string ContentType { get; set; }
        [JsonProperty(PropertyName = "contentUri")]
        public string ContentUri { get; set; }
        [JsonProperty(PropertyName = "tenantID")]
        public string TenantID { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
