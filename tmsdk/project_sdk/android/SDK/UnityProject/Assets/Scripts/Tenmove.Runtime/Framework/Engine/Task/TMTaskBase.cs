

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
        /// ������������ȼ����ɻ�ȡ��Agent��Դ�����᲻ͬ�����ȼ�Խ�߿ɻ�ȡ��Agent��������
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