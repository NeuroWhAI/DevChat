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
        public ProjectManager()
        {
            this.ProjectDir = "Projects";
            this.ConfigDir = "Configs";
        }

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
            return Path.Combine(ConfigDir, name + ".json");
        }

        public bool Exists(string name)
        {
            return File.Exists(GetConfigPath(name));
        }

        public void CreateProject(string name, string gitUrl, IPushMessage output)
        {
            if (Exists(name))
            {
                output.PushMessage($"Project {name} already exists.");

                return;
            }


            string configPath = GetConfigPath(name);
            string projPath = GetProjectPath(name);

            File.Create(configPath).Close();
            Directory.CreateDirectory(projPath);


            // Clone git repo
            Shell.WorkingDirectory = projPath;
            string cloneResult = Shell.Execute("git", $"clone \"{gitUrl}\" .");

            output.PushMessage(cloneResult);


            output.PushMessage("Complete!");
        }

        public void DeleteProject(string name, IPushMessage output)
        {
            if (Exists(name))
            {
                string projPath = GetProjectPath(name);
                string configPath = GetConfigPath(name);

                NoException(() => DeleteDirectory(projPath));
                NoException(() => File.Delete(configPath));


                output.PushMessage("Complete!");
            }
            else
            {
                output.PushMessage($"Project {name} does not exists.");
            }
        }

        private void NoException(Action action, bool log = false)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                if (log)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        private void DeleteDirectory(string path)
        {
            foreach (string file in Directory.GetFiles(path))
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in Directory.GetDirectories(path))
            {
                DeleteDirectory(dir);
            }

            var dirInfo = new DirectoryInfo(path);
            dirInfo.Attributes = FileAttributes.Normal;

            Directory.Delete(path);
        }
    }
}
