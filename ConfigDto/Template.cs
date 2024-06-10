using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net6_wpf_webview2_template.ConfigDto
{
    public class Template
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("nameRule")]
        public string NameRule { get; set; }

        [JsonProperty("sourcePath")]
        public string SourcePath { get; set; }

        [JsonProperty("targetPath")]
        public string TargetPath { get; set; }

        [JsonProperty("finalFileName")]
        public string FinalFileName { get; set; }

        [JsonProperty("tableName")]
        public string TableName { get; set; }

        [JsonProperty("variables")]
        public string Variables { get; set; }
    }
}
