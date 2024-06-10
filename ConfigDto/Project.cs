using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net6_wpf_webview2_template.ConfigDto
{
    public class Project
    {
        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("connections")]
        public List<Connection> Connections { get; set; }

        [JsonProperty("templates")]
        public List<Template> Templates { get; set; }

        [JsonProperty("ignoreFields")]
        public List<string> IgnoreFields { get; set; }

        [JsonProperty("tablePrefix")]
        public string TablePrefix { get; set; }

        [JsonProperty("variables")]
        public Variable Variables { get; set; }
    }
}
