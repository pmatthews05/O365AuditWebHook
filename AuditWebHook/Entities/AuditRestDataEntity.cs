using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AuditWebHook.Entities
{
    public struct AuditRestDataEntity
    {
        [JsonProperty(PropertyName = "restResponse")]
        public string RestResponse { get; set; }
        [JsonProperty(PropertyName = "webHeaderCollections")]
        public WebHeaderCollection WebHeaderCollections { get; set; }
    }
}
