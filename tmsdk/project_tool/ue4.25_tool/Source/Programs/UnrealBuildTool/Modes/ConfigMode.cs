using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.DotNETCommon;



namespace UnrealBuildTool
{
    public enum ConfigType
    {
        Default,
        Platform,
        User,
        Local
        
    }
    /// <summary>
    /// Builds a target
    /// </summary>
    [ToolMode("Config", ToolModeOptions.XmlConfig | ToolModeOptions.BuildPlatforms | ToolModeOptions.SingleInstance | ToolModeOptions.StartPrefetchingEngine | ToolModeOptions.ShowExecutionTime)]
    class ConfigMode : ToolMode
    {
        
        [CommandLine("-targetPlatform=")]
        public string targetPlatform = "";

        [CommandLine("-projectPath=")]
        public DirectoryReference projectPath = null;

        [CommandLine("-doFile=")]
        public string doFile = null;


        //加载顺序 defaultFile > platformFile > localFile > userFile
        ConfigFile defaultFile = null;
        ConfigFile platformFile = null;
        ConfigFile userFile = null;
        //这个就不管了，改了不在版本控制容易出错，而且不支持!+-操作
        ConfigFile localFile = null;


        public override int Execute(CommandLineArguments Arguments)
        {
            Arguments.ApplyTo(this);
            //手写改动基本需求是AppData的config/User{xxx}.ini、工程config/Default{xxx}.ini（global）
            //、工程config/{Platform}/{Platform}{xxx}.ini（global）、saved/config/{Platform}目录（local）

            //在ConfigHierarchyType中加需要加载的配置
            //加载完毕遍历Files，取出需求的三个文件
            //根据参数，修改对应的文件并重新储存

            

            var configs = ConfigCache.ReadHierarchy(ConfigHierarchyType.Engine, projectPath, Name2Platform(targetPlatform));



            foreach (var item in configs.Files)
            {



                string filepath = item.Location.FullName;
                Log.TraceInformation(item.Location.FullName);
                string filename = Path.GetFileNameWithoutExtension(filepath);
                bool isProjectConfigFile = filepath.StartsWith(projectPath.FullName);

                //eg:
                //ConfigMode.Execute: E:\softwave\UE_4.25\Engine\Config\Base.ini
                //ConfigMode.Execute: E:\softwave\UE_4.25\Engine\Config\BaseEngine.ini
                //ConfigMode.Execute: E:\softwave\UE_4.25\Engine\Config\Android\BaseAndroidEngine.ini
                //ConfigMode.Execute: D:\_workspace\_project\ue4.25\uwatest\Config\DefaultEngine.ini
                //ConfigMode.Execute: E:\softwave\UE_4.25\Engine\Config\Android\AndroidEngine.ini
                //ConfigMode.Execute: D:\_workspace\_project\ue4.25\uwatest\Config\Android\AndroidEngine.ini
                //ConfigMode.Execute: C:\Users\tengmu\AppData\Local\Unreal Engine\Engine\Config\UserEngine.ini



                if (filename.StartsWith("User"))
                {
                    userFile = item;
                }

                if (!isProjectConfigFile)
                {
                    continue;
                }

                if (filename.StartsWith("Default"))
                {
                    defaultFile = item;
                }
                else if (filename.StartsWith(targetPlatform, StringComparison.InvariantCultureIgnoreCase))
                {
                    platformFile = item;
                }
                else
                {
                    localFile = item;
                }

            }
            ProcessDoFile();

            return 0;
        }

        public void ProcessDoFile()
        {
            var dict = Json.Deserialize<Dictionary<string,object>>(File.ReadAllText(doFile));

            
            void Do(ConfigType configType)
            {
                var memberName = ConfigType2MemberName(configType);
                if (dict.TryGetValue(memberName, out var defaultFileMod))
                {
                    var tmp = defaultFileMod as object[];
                    var data = tmp[0] as Dictionary<string, object>;

                    var doFileActionData = new DoFileActionData(data);
                    var targetConfigFile = ProcessDofileActionData(configType, doFileActionData);
                    targetConfigFile.Write(targetConfigFile.Location);

                }
            }
            Do(ConfigType.Default);
            Do(ConfigType.Local);
            Do(ConfigType.Platform);
            Do(ConfigType.User);




        }
        
        private ConfigFile ProcessDofileActionData(ConfigType configType, DoFileActionData doFileActionData)
        {
            var targetConfigFile = ConfigType2ConfigFile(configType);
            var actionType = (ConfigLineAction)Enum.Parse(typeof(ConfigLineAction), doFileActionData.ConfigLineAction);
            var section = targetConfigFile.FindOrAddSection(doFileActionData.section);

            ConfigLine configLine;
            switch (actionType)
            {
                case ConfigLineAction.Set:
                    if (section.TryGetLine(actionType,doFileActionData.key,out configLine))
                    {
                        configLine.Key = doFileActionData.key;
                        configLine.Value = doFileActionData.value;
                    }
                    else
                    {
                        section.Lines.Add(new ConfigLine(actionType, doFileActionData.key, doFileActionData.value));
                    }
                    break;
                case ConfigLineAction.Add:    
                    section.Lines.Add(new ConfigLine(actionType, doFileActionData.key, doFileActionData.value));
                    break;
                case ConfigLineAction.RemoveKey:
                    section.Lines.Add(new ConfigLine(actionType, doFileActionData.key, doFileActionData.value));
                    break;
                case ConfigLineAction.RemoveKeyValue:
                    section.Lines.Add(new ConfigLine(actionType, doFileActionData.key, doFileActionData.value));
                    break;
                default:
                    break;
            }
            return targetConfigFile;
        }
        public ConfigFile ConfigType2ConfigFile(ConfigType configType)
        {
            switch (configType)
            {
                case ConfigType.Default:
                    return defaultFile;
                case ConfigType.Platform:
                    return platformFile;
                case ConfigType.User:
                    return userFile;
                case ConfigType.Local:
                    return localFile;
                default:
                    throw new Exception($"未知ConfigType={configType.ToString()}");
            }
        }
        public string ConfigType2MemberName(ConfigType configType)
        {
            switch (configType)
            {
                case ConfigType.Default:
                    return "defaultFile";
                case ConfigType.Platform:
                    return "platformFile";
                case ConfigType.User:
                    return "userFile";
                case ConfigType.Local:
                    return "localFile";
                default:
                    throw new Exception($"未知ConfigType={configType.ToString()}");
            }
        }
        class DoFileActionData
        {
            public string ConfigLineAction;
            public string section;
            public string key;
            public string value;
            public DoFileActionData(Dictionary<string,object> data)
            {
                ConfigLineAction = data["ConfigLineAction"] as string;
                section = data["section"] as string;
                key = data["key"] as string;
                value = data["value"] as string;
            }
        }
        public UnrealTargetPlatform Name2Platform(string name)
        {
            switch (name.ToLower())
            {
                case "win64":
                    return UnrealTargetPlatform.Win64;
                case "mac":
                    return UnrealTargetPlatform.Mac;
                case "android":
                    return UnrealTargetPlatform.Android;
                case "ios":
                    return UnrealTargetPlatform.IOS;

                default:
                    throw new Exception($"未知平台名{name}");
            }
        }
    }
    [Serializable]
    public class Data
    {
        public string ConfigLineAction;
        public string section;
        public string key;
        public string value;
    }
    public static class ConfigFileExt
    {
        public static bool TryGetLine(this ConfigFileSection configFile, ConfigLineAction configLineAction, string key, out ConfigLine configLine)
        {
            foreach (ConfigLine Line in configFile.Lines)
            {
                if (Line.Key.Equals(key) && Line.Action == configLineAction)
                {
                    configLine = Line;
                    return true;
                }
            }
            configLine = null;
            return false;
        }
    }
}
