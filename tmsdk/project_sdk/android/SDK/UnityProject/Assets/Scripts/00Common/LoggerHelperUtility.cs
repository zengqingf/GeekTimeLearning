using Tenmove.Log;

public static class LoggerHelperUtility
{
    public static void D(this ILog log, string format, params object[] para)
    {
        if (null == log) { return; }
        log.uD(format, para);
    }

    public static void E(this ILog log, string format, params object[] para)
    {
        if (null == log) { return; }
        log.uE(format, para);
    }

    public static void W(this ILog log, string format, params object[] para)
    {
        if (null == log) { return; }
        log.uW(format, para);
    }

    public static void I(this ILog log, string format, params object[] para)
    {
        if (null == log) { return; }
        log.uI(format, para);
    }
}
