using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ryan_DeYong_Programming_Portfolio_Reloaded
{
    class TemplateEngine
    {
        public TemplateEngine() {
        }

        private const string path_offset = "master_directory/templates/";

        public string ReplaceTemplatesInString(string s) {
            Regex re = new Regex("%%.*?%%");
            MatchCollection matches = re.Matches(s);
            for (int i = 0; i < matches.Count; i++) {
                string match_str = matches[i].ToString();
                string match_file = match_str.Replace("%%", "");
                string file_path = path_offset + match_file;
                string file_contents = "TEMPLATE_LOAD_ERROR";
                if (!File.Exists(file_path))
                {
                }
                else
                {
                    file_contents = File.ReadAllText(file_path);
                }
                s = re.Replace(s, file_contents, 1);
            }
            return s;
        }
    }
}
