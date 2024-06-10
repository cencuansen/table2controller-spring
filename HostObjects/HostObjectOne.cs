using JinianNet.JNTemplate;
using net6_wpf_webview2_template.ConfigDto;
using net6_wpf_webview2_template.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Template = net6_wpf_webview2_template.ConfigDto.Template;

namespace net6_wpf_webview2_template.HostObjects
{
    public class HostObjectOne
    {
        private readonly DapperHelper dapperHelper = new();
        private readonly MyConfig myConfig;
        private readonly string exePath = AppDomain.CurrentDomain.BaseDirectory;

        public HostObjectOne()
        {
        }

        public async Task<string> SayHello()
        {
            return await Task.Run(() => "hello");
        }

        public async Task<string> GetConfig()
        {
            return await ReadConfig();
        }

        public string GetTemplateNames(string projectName)
        {
            string folderPath = $"{exePath}Template/{projectName}";
            return JsonConvert.SerializeObject(Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly).Select(Path.GetFileName).ToArray());
        }

        public async Task<string> GetTemplateText(string projectName, string fileName)
        {
            try
            {
                string filePath = $"{exePath}Template/{projectName}/{fileName}";
                string content = await File.ReadAllTextAsync(filePath);
                return content;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public async Task<string> SelectFolder()
        {
            try
            {
                FolderBrowserDialog dialog = new();
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    return "";
                }
                return dialog.SelectedPath.Trim();
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public async Task<string> Generate(string info)
        {
            try
            {
                Project projectInfo = JsonConvert.DeserializeObject<Project>(info);
                string templateFileBasePath = $"{exePath}Template/{projectInfo.Name}/";
                string temporaryPath = templateFileBasePath + "temporary_" + DateTimeOffset.Now.ToUnixTimeSeconds();

                foreach (Template template in projectInfo.Templates)
                {
                    string templateFilePath = templateFileBasePath + template.Name;
                    var jNTemplate = Engine.LoadTemplate(templateFilePath);
                    jNTemplate.Set(nameof(projectInfo.Variables.Author), projectInfo.Variables.Author);
                    jNTemplate.Set(nameof(projectInfo.Variables.BusinessName), projectInfo.Variables.BusinessName);
                    jNTemplate.Set(nameof(projectInfo.Variables.TableName), projectInfo.Variables.TableName);
                    jNTemplate.SetStaticType("Util", typeof(Util));
                    string result = jNTemplate.Render();
                    Console.WriteLine(result);
                }

                return "True";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> TestConnect(string connectionConfig)
        {
            Connection connection = JsonConvert.DeserializeObject<Connection>(connectionConfig) ?? new Connection();
            dapperHelper.ConnectionString = $"Host={connection.Host};Port={connection.Port};Username={connection.User};Password={connection.Password};Database={connection.DatabaseName}";
            dapperHelper.DatabaseType = (DatabaseType)Enum.Parse(typeof(DatabaseType), connection.Type);
            try
            {
                await dapperHelper.TestConnect();
                return "True";
            }
            catch (Exception ex)
            {
                return "False";
            }
        }

        public async Task<string> GetAllTables(string connectionConfig)
        {
            Connection connection = JsonConvert.DeserializeObject<Connection>(connectionConfig) ?? new Connection();
            dapperHelper.ConnectionString = $"Host={connection.Host};Port={connection.Port};Username={connection.User};Password={connection.Password};Database={connection.DatabaseName}";
            dapperHelper.DatabaseType = (DatabaseType)Enum.Parse(typeof(DatabaseType), connection.Type);
            try
            {
                IList<SchemaTable> tables = await dapperHelper.AllTables();
                return JsonConvert.SerializeObject(tables);
            }
            catch (Exception ex)
            {
                return "[]";
            }
        }

        public async Task<string> WriteConfig(string content)
        {
            try
            {
                await File.WriteAllTextAsync($"{exePath}appsettings.json", JsonConvert.SerializeObject(JsonConvert.DeserializeObject(content), Formatting.Indented));
                return "True";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private async Task<string> ReadConfig()
        {
            string config = await File.ReadAllTextAsync($"{exePath}appsettings.json");
            config = config.Replace("\r\n", "");
            return config;
        }
    }
}
