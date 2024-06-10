using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net6_wpf_webview2_template.ConfigDto
{
    public class Connection
    {
        public bool Active { get; set; }
        public string Type { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string DatabaseName { get; set; }
    }
}
