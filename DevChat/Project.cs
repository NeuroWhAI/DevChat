using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json;

namespace DevChat
{
    public class Project
    {
        public Project()
        {

        }

        public string GitUrl { get; set; } = "";
        public string BuildScript { get; set; } = "";
        public string RunScript { get; set; } = "";

        public void LoadFromJson(string jsonText)
        {
            var json = JObject.Parse(jsonText);
            this.GitUrl = (string)json["git"];
            this.BuildScript = (string)json["build"];
            this.RunScript = (string)json["run"];
        }

        public string ToJsonText()
        {
            return JObject.FromObject(new
            {
                git = this.GitUrl,
                build = this.BuildScript,
                run = this.RunScript
            }).ToString();
        }
    }
}
