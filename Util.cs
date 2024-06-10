using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net6_wpf_webview2_template
{
    public class Util
    {
        public static string CamelCase(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;
            if (str.Contains("_"))
            {
                string[] words = str.Split('_');
                words[0] = words[0].ToLower();
                for (int i = 1; i < words.Length; i++)
                {
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
                }
                return string.Join("", words);
            }
            return char.ToLower(str[0]) + str.Substring(1);
        }
    }
}
