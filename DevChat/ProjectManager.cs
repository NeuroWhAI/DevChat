using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DevChat
{
    class ProjectManager
    {
        private string m_projectDir = "";
        public string ProjectDir
        {
            get { return m_projectDir; }
            set
            {
                m_projectDir = value;
                Directory.CreateDirectory(value);
            }
        }

        private string m_configDir = "";
        public string ConfigDir
        {
            get { return m_configDir; }
            set
            {
                m_configDir = value;
                Directory.CreateDirectory(value);
            }
        }

        public string GetProjectPath(string name)
        {
            return Path.Combine(ProjectDir, name);
        }

        public string GetConfigPath(string name)
        {
            return Path.Combine(ConfigDir, name);
        }

        public bool Exists(string name)
        {
            return Directory.Exists(GetConfigPath(name));
        }
    }
}
