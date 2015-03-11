using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace KONE.SFDC.ApexTestGetter.Util
{
    class PropertyFile
    {
        readonly string filePath;
        readonly string login;
        readonly string password;
        readonly string url;
        List<string> lines;

        public string Login
        {
            get { return login; }
        }
        public string Password
        {
            get { return password; }
        }
        public string ServerUrl
        {
            get { return url; }
        }

        /// <summary>
        /// Initializes build.properties reader class
        /// </summary>
        /// <param name="path">Path to the build.properties file</param>
        public PropertyFile(string path)
        {
            filePath = path;
            lines = new List<string>();

            using (StreamReader file = new StreamReader(path))
            {
                string line;

                while((line = file.ReadLine()) != null)
                {
                    lines.Add(line);
                }

                file.Close();
            }

            if(lines.Count > 0)
            {
                login = parseRegEx(@"sf.username\s*=\s*(\S+)");
                password = parseRegEx(@"sf.password\s*=\s*(\S+)");
                url = parseRegEx(@"sf.serverurl\s*=\s*(\S+)");
            }

            lines.Clear();
        }

        private string parseRegEx(string expression)
        {
            string result = null;

            Regex rx = new Regex(expression);
            foreach (string line in lines)
            {
                Match match = rx.Match(line);
                if (match.Success)
                    result = match.Groups[1].Value;
            }

            return result;
        }
    }
}
