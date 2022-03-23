

namespace Tenmove.Runtime
{
    public class Task<T> : TaskBase where T: TaskObject
    {
        private T m_TaskData;
        private readonly TaskProcedure<T> m_Procedure;
        
        public Task(int taskID,TaskPriority priority,T data,TaskProcedure<T> procedure,string tag)
            : base(taskID, priority,tag)
        {
            Debugger.Assert(null != procedure, "Task procedure can not be null!");

            m_TaskData = data;
            m_Procedure = procedure;
        }

        public T Content
        {
            get { return m_TaskData; }
        }
              
        public override bool IsDone
        {
            get { return m_Procedure.IsDone; }
        }

        public sealed override void Start()
        {
            m_Procedure.Start(this);
        }

        public sealed override void OnProcess()
        {
            m_Procedure.OnProcess();
        }

        public sealed override void OnTerminate()
        {
            m_Procedure.Terminate();
        }

        public sealed override void Reset()
        {
            m_Procedure.Reset();
        }
    }
}