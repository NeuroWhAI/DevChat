using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            LoadFromJObject(json);
        }

        public string ToJson()
        {
            return ConvertToJson().ToString();
        }

        public void SetProperty(string key, string val)
        {
            var json = ConvertToJson();
            if (json[key] != null)
            {
                json[key] = val;
            }

            LoadFromJObject(json);
        }

        private void LoadFromJObject(JObject json)
        {
            this.GitUrl = (string)json["git"];
            this.BuildScript = (string)json["build"];
            this.RunScript = (string)json["run"];
        }

        private JObject ConvertToJson()
        {
            return JObject.FromObject(new
            {
                git = this.GitUrl,
                build = this.BuildScript,
                run = this.RunScript
            });
        }
    }
}
