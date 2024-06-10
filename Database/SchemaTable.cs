using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net6_wpf_webview2_template.Database
{
    public class SchemaTable
    {
        [JsonProperty("tableName")]
        public string TableName { get; set; }
        [JsonProperty("tableComment")]
        public string TableComment { get; set; }
    }
}
