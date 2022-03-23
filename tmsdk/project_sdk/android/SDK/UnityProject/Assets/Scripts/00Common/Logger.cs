using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.UI;
using System.Reflection;
using System;
using System.Text.RegularExpressions;

public class LoggerModelAttribute : Attribute
{
    public LoggerModelAttribute(string name)
    {
        mName = name;
    }

    public string mName;
}

/*
搭配ConsolePro3的日志筛选使用
*/
public class Logger
{
    public enum LogLevel
    {
        NORMAL = 0, //白色log
        PROCESS,    //记录流程的日志,每个系统都要加，需要查流程的时候会打开这个选项
        WARNING,    //警告
        NET,        //网络专用
        ERROR,      //错误,在手机包上也会默认打开
        NONE,       //什么日志都不打
    }

    public static bool enable = false;
    private static LogLevel logLevel = LogLevel.ERROR;

    public static LogLevel GetLevel()
    {
        return logLevel;
    }
    public static void SetLevel(LogLevel lv)
    {
        logLevel = lv;

        Logger.LogProcessFormat("[LOG]SetLevel:{0}", lv);
    }

    private static bool CanPrint(LogLevel lv)
    {
        return logLevel <= lv;
    }

    private static string GetCurrentTime()
    {
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff ");// + Time.frameCount.ToString();
    }

    private static Tenmove.Log.ILog skLog = Tenmove.Log.LoggerHelper.DefaultLogger;

    #region not use
    [Conditional("LOG_DIALOG"), Conditional("UNITY_ANDROID"), Conditional("UNITY_IOS"), Conditional("LOG_ERROR")]
    public static void DisplayLog(string info, UnityEngine.Events.UnityAction onOKCallBack = null)
    {
        MessageBox(info, onOKCallBack);
    }

    [Conditional("UNITY_EDITOR")]
    public static void EditorLogWarning(string str, params object[] args)
    {
        skLog.W(str, args);

        var tag = "";
        //if (LoggerUnit.GetTag(ref tag))
        {
#if !LOGIC_SERVER || LOGIC_SERVER_TEST
            UnityEngine.Debug.LogWarningFormat(_formatString(LogType.Log, str, args));
#else
            LogicServer.LogConsole(LogicServer.LogicServerLogType.Info, _formatString(LogType.Log, str, args));
#endif
        }
    }

    [Conditional("LOG_DIALOG")]
    public static void MessageBox(string info, UnityEngine.Events.UnityAction onOKCallBack = null)
    {
        //         if (!Application.isPlaying)
        //         {
        //             return;
        //         }
        // 
        //         var root = Utility.FindGameObject("AlertBoxCanvas", false);
        //         if (root == null)
        //         {
        //             root = new GameObject("AlertBoxCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        //             root.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        //             root.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        //             root.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
        //             root.transform.SetAsLastSibling();
        //         }
        //         //var goDebugObj = AssetLoader.instance.LoadObject("UI/Prefabs/CommonMsgBoxOK") as GameObject;
        //         Utility.AttachTo(goDebugObj, root);
        // 
        //         if (onOKCallBack != null)
        //         {
        //             var button = Utility.FindComponent<UnityEngine.UI.Button>(goDebugObj, "loading/Panel/button", false);
        //             button.onClick.AddListener(onOKCallBack);
        //         }
        // 
        //         goDebugObj.GetComponent<AlertBox>().SetMessage(info);
        //         goDebugObj = null;
    }

    [Conditional("WORK_DEBUG"), Conditional("LOG_DIALOG"), Conditional("LOG_ERROR")]
    public static void ShowDailog(string str, params object[] args)
    {
        var tag = "";
        //if (LoggerUnit.GetTag(ref tag, true, 3))
        {
            var info = string.Format(str, args).TrimEnd();
            DisplayLog(string.Format("{0}\n{1}", info, tag));
        }
    }
    #endregion

    private static string _formatString(LogType type, string str, params object[] args)
    {
        //string colorstr = sColorList[(int)type];

        string var = string.Format("[{0}]", GetCurrentTime()) + string.Format(str, args);

        return var + "\n";

        //var = var.Replace("\n", string.Format("</color>\n<color={0}>", colorstr));
        //return string.Format("<color={0}>{1}</color>\n", colorstr, var.Trim()); 
    }

    private static object[] skEmpty = new object[0];

    //[Conditional("WORK_DEBUG"), Conditional("LOG_NORMAL")]
    public static void Log(string str)
    {
        skLog.D(str, skEmpty);

        if (!CanPrint(LogLevel.NORMAL))
            return;

        var tag = "";
        //if (LoggerUnit.GetTag(ref tag))
        {

#if !LOGIC_SERVER || LOGIC_SERVER_TEST
            UnityEngine.Debug.LogFormat("{0} {1}", tag, _formatString(LogType.Log, str, new object[] { }));
#else 
            LogicServer.LogConsole(LogicServer.LogicServerLogType.Info, string.Format("{0} {1}", tag,  _formatString(LogType.Log, str, new object[] { })));
#endif
        }
    }

    //  [Conditional("WORK_DEBUG"), Conditional("LOG_NORMAL")]
    public static void LogFormat(string str, params object[] args)
    {
        skLog.D(str, args);

        if (!CanPrint(LogLevel.NORMAL))
            return;

        var tag = "";
        //if (LoggerUnit.GetTag(ref tag))
        {

#if !LOGIC_SERVER || LOGIC_SERVER_TEST
            UnityEngine.Debug.LogFormat("{0} {1}", tag, _formatString(LogType.Log, str, args));
#else 
            LogicServer.LogConsole(LogicServer.LogicServerLogType.Info, string.Format("{0} {1}", tag,  _formatString(LogType.Log, str, args)));
#endif
        }
    }

    //  [Conditional("WORK_DEBUG"), Conditional("LOG_WARNNING"), Conditional("LOG_NORMAL")]
    public static void LogWarning(string str)
    {
        skLog.W(str, skEmpty);

        if (!CanPrint(LogLevel.WARNING))
            return;

        var tag = "";
        //if (LoggerUnit.GetTag(ref tag))
        {

#if !LOGIC_SERVER || LOGIC_SERVER_TEST
            UnityEngine.Debug.LogWarningFormat("{0} {1}", tag, _formatString(LogType.Warning, str, new object[] { }));
#else 
            LogicServer.LogConsole(LogicServer.LogicServerLogType.Warning, string.Format("{0} {1}", tag,  _formatString(LogType.Log, str, new object[] { })));
#endif
        }
    }

    //   [Conditional("WORK_DEBUG"), Conditional("LOG_WARNNING"), Conditional("LOG_NORMAL")]
    public static void LogWarningFormat(string str, params object[] args)
    {
        skLog.W(str, args);
        if (!CanPrint(LogLevel.WARNING))
            return;

        var tag = "";
        //if (LoggerUnit.GetTag(ref tag))
        {

#if !LOGIC_SERVER || LOGIC_SERVER_TEST
            UnityEngine.Debug.LogWarningFormat("{0} {1}", tag, _formatString(LogType.Warning, str, args));
#else 
            LogicServer.LogConsole(LogicServer.LogicServerLogType.Warning, string.Format("{0} {1}", tag,  _formatString(LogType.Log, str, args)));
#endif
        }
    }

    //  [Conditional("WORK_DEBUG"), Conditional("LOG_PROCESS")]
    public static void LogProcessFormat(string str, params object[] args)
    {
        skLog.I(str, args);

        if (!CanPrint(LogLevel.PROCESS))
            return;

        var tag = "";
        //if (LoggerUnit.GetTag(ref tag))

#if !LOGIC_SERVER || LOGIC_SERVER_TEST
        UnityEngine.Debug.LogWarningFormat("{0} {1}", tag, _formatString(LogType.Warning, str, args));
#else
            LogicServer.LogConsole(LogicServer.LogicServerLogType.Warning, string.Format(str, args));
#endif
    }

    //   [Conditional("WORK_DEBUG"), Conditional("LOG_ERROR"), Conditional("LOG_WARNNING"), Conditional("LOG_NORMAL")]
    public static void LogErrorCode(uint error)
    {
        //         var tag = "";
        //         var str = Utility.ProtocolErrorString(error);
        // 
        //         //if (LoggerUnit.GetTag(ref tag))
        //         {
        // 
        // #if !LOGIC_SERVER || LOGIC_SERVER_TEST
        //             UnityEngine.Debug.LogErrorFormat("{0} {1}", tag, _formatString(LogType.Error, str));
        // #else
        //             LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format("{0} {1}", tag, _formatString(LogType.Error, str)));
        // #endif
        //         }
        // 
        //         ShowDailog(str, new object[] { });
    }

    [Conditional("WORK_DEBUG"), Conditional("LOG_ERROR"), Conditional("LOG_WARNNING"), Conditional("LOG_NORMAL")]
    public static void LogError(string str)
    {
        skLog.E(str, skEmpty);

        if (!CanPrint(LogLevel.ERROR))
            return;

        string tag = "";
#if !LOGIC_SERVER || LOGIC_SERVER_TEST
        UnityEngine.Debug.LogErrorFormat("{0} {1}", tag, _formatString(LogType.Error, str, new object[] { }));
#else
        LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, str);
#endif
        //ShowDailog(str, new object[] { });
    }

    [Conditional("WORK_DEBUG"), Conditional("LOG_ERROR"), Conditional("LOG_WARNNING"), Conditional("LOG_NORMAL")]
    public static void LogErrorFormat(string str, params object[] args)
    {
        skLog.E(str, args);

        if (!CanPrint(LogLevel.ERROR))
            return;

        var tag = "";
#if !LOGIC_SERVER || LOGIC_SERVER_TEST
        UnityEngine.Debug.LogErrorFormat("{0} {1}", tag, _formatString(LogType.Error, str, args));
#else
        LogicServer.LogConsole(LogicServer.LogicServerLogType.Error, string.Format(str, args));
#endif
    }

    [Conditional("WORK_DEBUG")]
    public static void LogForNet(string str, params object[] args)
    {
        skLog.I(str, args);

        if (!CanPrint(LogLevel.NET))
            return;

#if !LOGIC_SERVER || LOGIC_SERVER_TEST
        UnityEngine.Debug.LogWarningFormat(str, args);
#else
        LogicServer.LogConsole(LogicServer.LogicServerLogType.Info, string.Format(str, args));
#endif
    }
}
