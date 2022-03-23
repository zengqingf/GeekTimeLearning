

namespace Tenmove.Runtime
{
    public enum TaskPriority
    {
        Low,
        Normal,
        High,
        Highest,
    }

    public abstract class TaskBase
    {
        public static readonly int InvalidID =~0;

        private readonly int m_TaskID;
        private readonly string m_Tag;
        private readonly TaskPriority m_Priority;
       
        public TaskBase(int taskID,TaskPriority priority,string tag)
        {
            Debugger.Assert(InvalidID != taskID, "Task ID is invalid!");
            Debugger.Assert(!string.IsNullOrEmpty(tag), "tag can not be null or empty string!");

            m_TaskID = taskID;
            m_Priority = priority;
            m_Tag = tag;
        }

        public int TaskID
        {
            get { return m_TaskID; }
        }

        public string Tag
        {
            get { return m_Tag; }
        }

        /// <summary>
        /// 根据任务的优先级，可获取的Agent资源数量会不同，优先级越高可获取的Agent数量更多
        /// </summary>
        public TaskPriority Priority
        {
            get { return m_Priority; }
        }

        public abstract bool IsDone
        {
            get;
        }

        public abstract void Start();
        public abstract void OnProcess();
        public abstract void OnTerminate();
        public abstract void Reset();
    }
}