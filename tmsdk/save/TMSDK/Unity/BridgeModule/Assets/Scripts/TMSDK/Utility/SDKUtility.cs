using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

namespace TMSDKClient
{
    public static class SDKUtility
    {
        public static readonly string SDKDEBUG_HEAD = "###_SDK_###";

        public static bool OPEN_SDK_LOG_WHOLE = false;

        public static string GetDescriptionByName<T>(this T enumItemName)
        {
            FieldInfo fi = enumItemName.GetType().GetField(enumItemName.ToString());
            if (fi == null)
            {
                return enumItemName.ToString();
            }
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return enumItemName.ToString();
            }
        }

        public static void SDKDebugFormat(DebugType debugType, string logHead, params object[] logContents)
        {
            string logFormat = null;
            if (!OPEN_SDK_LOG_WHOLE)
            {
                logFormat = string.Format("{0}.{1}", SDKDEBUG_HEAD, logHead);
                _ShowSDKLog(debugType, logFormat, logContents);
                return;
            }
            StackTrace st = new StackTrace(true);
            if (st == null || st.GetFrame(1) == null)
            {
                return;
            }
            MethodBase mb = st.GetFrame(1).GetMethod();
            if (mb == null)
            {
                return;
            }
            string methodName = mb.Name;
            string classFullName = mb.DeclaringType.FullName;

            logFormat = string.Format("{0},[{1}.{2}],{3}",SDKDEBUG_HEAD, classFullName, methodName, logHead);
            _ShowSDKLog(debugType, logFormat, logContents);
        }

        private static void _ShowSDKLog(DebugType debugType, string logFormat, params object[] logContents)
        {
            switch (debugType)
            {
                case DebugType.NormalMask:
                    //TODO Game.Logger.LogFormat  
                    //Logger.LogFormat(logFormat, logContents);
                    break;
                case DebugType.NormalNoMask:
                    if (logContents == null || logContents.Length <= 0)
                    {
                        UnityEngine.Debug.Log(logFormat);
                    }
                    else
                    {
                        UnityEngine.Debug.LogFormat(logFormat, logContents);
                    }
                    break;
                case DebugType.WarningMask:
                    //TODO Game.Logger.LogWarningFormat  
                    //Logger.LogWarningFormat(logFormat, logContents);
                    break;
                case DebugType.WardingNoMask:
                    if (logContents == null || logContents.Length <= 0)
                    {
                        UnityEngine.Debug.LogWarning(logFormat);
                    }
                    else
                    {
                        UnityEngine.Debug.LogWarningFormat(logFormat, logContents);
                    }
                    break;
                case DebugType.ErrorMask:
                    //TODO Game.Logger.LogErrorFormat  
                    //Logger.LogErrorFormat(logFormat, logContents);
                    break;
                case DebugType.ErrorNoMask:
                    if (logContents == null || logContents.Length <= 0)
                    {
                        UnityEngine.Debug.LogError(logFormat);
                    }
                    else
                    {
                        UnityEngine.Debug.LogErrorFormat(logFormat, logContents);
                    }
                    break;
            }
        }

        public static T GetSDKCallArgValue<T>(string key, object[] args)
        {
            if (string.IsNullOrEmpty(key))
            {
                return default(T);
            }
            if (args == null || args.Length <= 0)
            {
                return default(T);
            }
            for (int i = 0; i < args.Length; i++)
            {
                SDKCallArg<T> arg = (SDKCallArg<T>)args[i];
                if (arg == null)
                    continue;
                if (arg.name.Equals(key))
                {
                    return arg.value;
                }
            }
            return default(T);
        }
    }

}
