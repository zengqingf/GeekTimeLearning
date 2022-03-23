namespace Tenmove.Log
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Threading;

    public static class LoggerHelper
    {

        public static ILog GetLogger(string tag)
        {
            LoggerManager.DefaultLogger.I("Get Logger {0}", tag);

            var iLog = LoggerManager.FindLogger(tag);
            if (null != iLog)
            {
                return iLog;
            }

            return LoggerManager.CreateLogger(tag);
        }

        public static ILog DefaultLogger
        {
            get { return LoggerManager.DefaultLogger; }
        }

        public static void RegExceptionHandler()
        {
            UnityEngine.Application.logMessageReceived += LoggerManager.OnLogCallbackHandler;
            UnityEngine.Application.logMessageReceivedThreaded += LoggerManager.OnLogCallbackHandler;

        }

        public static void UnRegExceptionHandler()
        {
            UnityEngine.Application.logMessageReceived -= LoggerManager.OnLogCallbackHandler;
            UnityEngine.Application.logMessageReceivedThreaded -= LoggerManager.OnLogCallbackHandler;
        }


        public static void Init(LogLevel writeLogFileLevel, LogLevel traceStackLevel)
        {
            LoggerManager.TraceStackLevel = traceStackLevel;
            LoggerManager.WriteLogFileLevel = writeLogFileLevel;
            LoggerManager.Init();
        }

        public static bool IsPlaying
        {
            get
            {
                return LoggerManager.IsPlaying;
            }

            set
            {
                LoggerManager.IsPlaying = value;
            }
        }

        public static LogLevel WriteLogFileLevel
        {
            get
            {
                return LoggerManager.WriteLogFileLevel;
            }

            set
            {
                LoggerManager.WriteLogFileLevel = value;
                LoggerManager.DefaultLogger.I("SetLogWriteLevel {0}", value);
            }
        }

        public static LogLevel TraceStatckLevel
        {
            get
            {
                return LoggerManager.TraceStackLevel;
            }

            set
            {
                LoggerManager.TraceStackLevel = value;
                LoggerManager.DefaultLogger.I("SetLogTraceLevel {0}", value);
            }
        }


        public static void UnInit()
        {
            LoggerManager.UnInit();
        }

        public static void Flush()
        {
            LoggerManager.Flush();
        }
    }

    public enum LogLevel
    {
        /// <summary>
        /// Debug 调试
        /// </summary>
        D = 0,

        /// <summary>
        /// Info 流程
        /// </summary>
        I,

        /// <summary>
        /// Warning 警告
        /// </summary>
        W,

        /// <summary>
        /// Error 错误
        /// </summary>
        E,

        /// <summary>
        /// Excption 异常
        /// </summary>
        X,

        /// <summary>
        /// None
        /// </summary>
        None,
    }

    internal struct LogUnit
    {
        public LogUnit(ILog log, LogLevel level, long ts, int n, string content, string trace)
        {
            Log = log;
            TimeStamp = ts;
            Content = content;
            Level = level;
            ContentNumber = n;

            StackTrace = trace;
        }

        public long TimeStamp { get; private set; }
        public int ContentNumber { get; private set; }
        public string Content { get; private set; }
        public string StackTrace { get; private set; }
        public LogLevel Level { get; private set; }
        public ILog Log { get; private set; }
    }

    public interface ILog
    {
        void Flush();

        void Redirect2(string filename);

        void uD(string format, params object[] para);
        void uE(string format, params object[] para);
        void uW(string format, params object[] para);
        void uI(string format, params object[] para);

        string Tag { get; }
    }

    internal class Logger : ILog
    {
        public Logger(string tag, string path)
        {
            Tag = tag;
        }

        public string Path { get; private set; }
        public string Tag { get; private set; }

        public void Flush()
        {
            throw new NotImplementedException();
        }

        public void Redirect2(string filename)
        {
            throw new NotImplementedException();
        }

        public void uE(string format, params object[] para)
        {
            LoggerManager.Log(this, LogLevel.E, string.Empty, format, para);

            //             if (Tag != "-")
            //             {
            //                 UnityEngine.Debug.LogErrorFormat(format, para);
            //             }
        }

        public void uW(string format, params object[] para)
        {
            LoggerManager.Log(this, LogLevel.W, string.Empty, format, para);
        }

        public void uI(string format, params object[] para)
        {
            LoggerManager.Log(this, LogLevel.I, string.Empty, format, para);
        }

        public void uD(string format, params object[] para)
        {
            LoggerManager.Log(this, LogLevel.D, string.Empty, format, para);
        }
    }

    class AQueueExt
    {
        private const int kSize = 64;

        private const int kQueueSize = 16;

        private object mLockObj = new object();

        private Queue<Queue<LogUnit>> mQueue;
        private List<Queue<LogUnit>> mPool;

        private Queue<LogUnit> mCurrentQueue;

        public AQueueExt(int size = kSize)
        {
            mQueue = new Queue<Queue<LogUnit>>(size);
            mCurrentQueue = _GetQueue();
        }

        private Queue<LogUnit> _GetQueue()
        {
            if (null == mPool)
            {
                mPool = new List<Queue<LogUnit>>(kQueueSize);
            }

            if (mPool.Count <= 0)
            {
                for (int i = 0; i < kQueueSize; ++i)
                {
                    mPool.Add(new Queue<LogUnit>(kSize));
                }
            }

            Queue<LogUnit> q = mPool[0];

            //UnityEngine.Debug.LogFormat("get queue {0}", mPool.Count);

            mPool.RemoveAt(0);

            return q;
        }

        public void PutBackQueue(Queue<LogUnit> u)
        {
            lock (mLockObj)
            {
                mPool.Add(u);
            }
            //UnityEngine.Debug.LogFormat("reycle queue {0}", mPool.Count);
        }

        public void Enqueue(LogUnit u)
        {
            lock (mLockObj)
            {
                if (mCurrentQueue.Count >= kSize)
                {
                    mQueue.Enqueue(mCurrentQueue);
                    mCurrentQueue = _GetQueue();
                    mCurrentQueue.Enqueue(u);
                    Monitor.Pulse(mLockObj);
                }
                else
                {
                    mCurrentQueue.Enqueue(u);
                }

                //mCurrentQueue.Enqueue(u);
                //
                //if (mSeq[mFrontIdx].IsFull)
                //{
                //    _Enqueue();

                //    //u.ContentNumber = number++;

                //    mSeq[mFrontIdx].Enqueue(u);
                //    Monitor.Pulse(mLockObj);
                //}
                //else
                //{
                //    mSeq[mFrontIdx].Enqueue(u);
                //}
            }
        }

        public void EnqueueFront()
        {
            _EnqueueFront();
        }

        private void _EnqueueFront()
        {
            lock (mLockObj)
            {
                if (mCurrentQueue.Count > 0)
                {
                    mQueue.Enqueue(mCurrentQueue);
                    mCurrentQueue = _GetQueue();
                    Monitor.Pulse(mLockObj);
                }
            }
        }


        public Queue<LogUnit> Dequeue()
        {
            Queue<LogUnit> deq = null;
            lock (mLockObj)
            {
                while (mQueue.Count <= 0)
                {
                    Monitor.Wait(mLockObj);
                }

                deq = mQueue.Dequeue();
            }
            return deq;
        }

        //private int _DequeueTail()
        //{
        //    int idx = -1;

        //    lock (mLockObj)
        //    {
        //        while (_IsEmpty)
        //        {
        //            Monitor.Wait(mLockObj);
        //        }

        //        idx = _Dequeue();
        //    }

        //    return idx;
        //}
    }


    static class LoggerManager
    {
        private const int kLogSegSize = 64;

        static LoggerManager()
        {
            TraceStackLevel = LogLevel.E;
            IsPlaying = false;
        }

        public static LogLevel TraceStackLevel
        {
            get; set;
        }

        public static bool IsPlaying
        {
            get; set;
        }

        public static LogLevel WriteLogFileLevel
        {
            get; set;
        }

        private static List<Logger> mLoggers = new List<Logger>(64);

        private static bool skWriteThreadIsQuit = false;

        private static AQueueExt skLogQueue = new AQueueExt(kLogSegSize);//= new LQe<AQueue<LogUnit>>(32);

        private static ILog skDefaultLogger = null;
        public static ILog DefaultLogger
        {
            get
            {
                if (null == skDefaultLogger)
                {
                    skDefaultLogger = CreateLogger("-");
                    skDefaultLogger.I("Create Default Logger {0}", skDefaultLogger.Tag);
                }

                return skDefaultLogger;
            }
        }

        private static void _Thread_WriteFile()
        {
            while (!skWriteThreadIsQuit)
            {
                Queue<LogUnit> seg = null;

                seg = skLogQueue.Dequeue();

                if (seg != null)
                {
                    //UnityEngine.Debug.LogFormat("write log count {0}", seg.Count);

                    while (seg.Count > 0)
                    {
                        var writeUnit = seg.Dequeue();
                        try
                        {
                            _WriteFile(writeUnit);
                        }
                        catch (System.Exception e)
                        {
                            skWriteThreadIsQuit = true;
#if UNITY_EDITOR
                            UnityEngine.Debug.LogWarningFormat("[Editor日志] 写日志报错了！{0}", e.ToString());
#endif
                            break;
                        }
                    }

                    skLogQueue.PutBackQueue(seg);
                }
            }
        }

        private static void _WriteFile(LogUnit unit)
        {
            var dirPath = _GetDir(unit);


            string content = _GetContent(unit);

            //UnityEngine.Debug.Log(content);

            _WriteOneFile(_GetAllLogPath(dirPath), content);
            //System.IO.File.AppendAllText(_GetAllLogPath(dirPath), content);

            if (unit.Level >= LogLevel.W)
            {
                _WriteOneFile(_GetLevelLogPath(dirPath), content);
            }

#if UNITY_EDITOR
            if (unit.Log.Tag != "-")
            {
                _WriteOneFile(_GetPath(dirPath, unit), content);
            }
#endif
        }

        private static void _WriteOneFile(string path, string content)
        {
            File.AppendAllText(path, content);

            return;

            FileStream file = File.Open(path, FileMode.Append, FileAccess.Write);

            byte[] bytes = Encoding.UTF8.GetBytes(content);//Convert.ToBase64String(byte[] inArray)

            file.Write(bytes, 0, bytes.Length);

            file.Flush();

            file.Close();
        }

        private static string _GetAllLogPath(string dir)
        {
            return Path.Combine(dir, "All.log");
        }

        private static string _GetLevelLogPath(string dir)
        {
            return Path.Combine(dir, "AllWarningError.log");
        }

        private static string skCurrentLogDir = string.Empty;


        private static string _GetDir(LogUnit unit)
        {
            return TMPathUtil.CreateTypeNumberDir(TMPathUtil.Type.Logs, ref skCurrentLogDir);
        }

        private static global::System.Text.StringBuilder skBuilder = new global::System.Text.StringBuilder(256);

        static string _GetContent(LogUnit unit)
        {
            System.DateTime dt = new DateTime(unit.TimeStamp);

            skBuilder.Clear();
            skBuilder.Append("[");
            skBuilder.Append(unit.Level);
            skBuilder.Append("]");

            skBuilder.Append(" [");
            skBuilder.AppendFormat("{0,-8} ", unit.ContentNumber);
            skBuilder.Append("]");

            //skBuilder.Append(" [");
            //skBuilder.Append(unit.TimeStamp);
            //skBuilder.Append("]");

            skBuilder.Append(" [");
            skBuilder.Append(dt.ToString("yyyy-MM-dd HH:mm:ss fff"));
            skBuilder.Append("]");

            skBuilder.Append(" [");
            skBuilder.Append(unit.Log.Tag);
            skBuilder.Append("] ");


            skBuilder.Append(unit.Content);
            skBuilder.AppendLine();

            if (!string.IsNullOrEmpty(unit.StackTrace))
            {
                skBuilder.Append(unit.StackTrace);
                skBuilder.AppendLine();
            }

            return skBuilder.ToString();
        }

        static string _GetPath(string dir, LogUnit unit)
        {
            return System.IO.Path.Combine(dir, string.Format("T_{0}.log", unit.Log.Tag));
        }

        private static int skSNumber = 0;

        private static Thread skThreadWriteFile = null;


        public static void Init()
        {
            //skThreadWriteFile = true;
            _CreateWriteThread();
        }



        public static void OnLogCallbackHandler(string condition, string stackTrace, UnityEngine.LogType type)
        {
            if (type == UnityEngine.LogType.Exception
            || type == UnityEngine.LogType.Assert
            )
            {
                Log(DefaultLogger, LogLevel.X, stackTrace, condition);
            }
        }


        private static void _CreateWriteThread()
        {
            if (null == skThreadWriteFile)
            {
                skWriteThreadIsQuit = false;
                skThreadWriteFile = new Thread(_Thread_WriteFile);
                skThreadWriteFile.IsBackground = true;
                skThreadWriteFile.Start();

                //DefaultLogger.I("Create FileWrite Thread");
            }
        }

        private static void CheckWriteThreadAlive()
        {
            if (null == skThreadWriteFile || !skThreadWriteFile.IsAlive)
            {
                _CreateWriteThread();
            }
        }

        public static void UnInit()
        {
            IsPlaying = false;
            number = 0;
            skDefaultLogger = null;
            skCurrentLogDir = string.Empty;


            if (null != mLoggers)
            {
                mLoggers.Clear();
            }

            skWriteThreadIsQuit = true;

            if (null != skThreadWriteFile)
            {
                skThreadWriteFile.Abort();
            }

            skThreadWriteFile = null;
        }

        public static void Flush()
        {
            skLogQueue.EnqueueFront();
        }


        private static int number = 0;

        private static StringBuilder skStackTaceBuilder = new StringBuilder(2048);

        public static void Log(ILog log, LogLevel level, string traceStack, string format, params object[] para)
        {

            CheckWriteThreadAlive();

            if (WriteLogFileLevel == LogLevel.None)
            {
                return;
            }

            if (WriteLogFileLevel <= level)
            {
                if (string.IsNullOrEmpty(traceStack) && TraceStackLevel <= level)
                {

                    StackTrace stackTrace = new StackTrace(true);

                    skStackTaceBuilder.Clear();

                    for (int i = 2; i < stackTrace.FrameCount; ++i)
                    {
                        var frame = stackTrace.GetFrame(i);
                        var methmod = frame.GetMethod();
                        if (null != methmod)
                        {
                            var declaringType = methmod.DeclaringType;
                            if (null != declaringType)
                            {
                                skStackTaceBuilder.Append(declaringType.Name);
                                skStackTaceBuilder.Append(".");
                            }

                            skStackTaceBuilder.Append(methmod.Name);

                            var paras = methmod.GetParameters();
                            if (null != paras)
                            {
                                skStackTaceBuilder.Append("(");
                                for (int j = 0; j < paras.Length; ++j)
                                {
                                    skStackTaceBuilder.Append(paras[j].ParameterType.Name);
                                    skStackTaceBuilder.Append(" ");
                                    skStackTaceBuilder.Append(paras[j].Name);

                                    if (j != (paras.Length - 1))
                                    {
                                        skStackTaceBuilder.Append(",");
                                    }
                                }
                                skStackTaceBuilder.Append(")");
                            }
                            else
                            {
                                skStackTaceBuilder.Append("()");
                            }

                        }
                        //skStackTaceBuilder.Append(frame.GetMethod().);
                        skStackTaceBuilder.Append(" (at ");

                        string filename = frame.GetFileName();
                        if (!string.IsNullOrEmpty(filename))
                        {
                            skStackTaceBuilder.Append(System.IO.Path.GetFileName(filename));
                        }

                        skStackTaceBuilder.Append(":");
                        skStackTaceBuilder.Append(frame.GetFileLineNumber());
                        skStackTaceBuilder.Append(" )");
                        skStackTaceBuilder.AppendLine();
                    }

                    traceStack = skStackTaceBuilder.ToString();
                }

                string content = string.Empty;
                LogLevel curLogLvl = level;

                try
                {
                    skStackTaceBuilder.Clear();
                    skStackTaceBuilder.AppendFormat(format, para);
                    content = skStackTaceBuilder.ToString();
                }
                catch (Exception e)
                {
                    skStackTaceBuilder.Clear();

                    curLogLvl = LogLevel.X;

                    skStackTaceBuilder.Append(format);
                    skStackTaceBuilder.Append(" ");
                    skStackTaceBuilder.Append(e.ToString());

                    content = skStackTaceBuilder.ToString();
                }

                LogUnit unit = new LogUnit(log, level, System.DateTime.Now.Ticks, Interlocked.Increment(ref number), content, traceStack);
                skLogQueue.Enqueue(unit);
            }
        }

        public static ILog CreateLogger(string tag, string path = "")
        {
            Init();

            Logger logger = new Logger(tag, path);
            mLoggers.Add(logger);

            return logger;
        }

        public static ILog FindLogger(string tag)
        {
            for (int i = 0; i < mLoggers.Count; ++i)
            {
                if (mLoggers[i].Tag == tag)
                {
                    return mLoggers[i];
                }
            }

            return null;
        }
    }
}
