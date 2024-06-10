using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net6_wpf_webview2_template.ConfigDto
{
    public class Variable
    {
        [JsonProperty("tableName")]
        public string TableName { get; set; }

        [JsonProperty("businessName")]
        public string BusinessName { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }
    }
}
