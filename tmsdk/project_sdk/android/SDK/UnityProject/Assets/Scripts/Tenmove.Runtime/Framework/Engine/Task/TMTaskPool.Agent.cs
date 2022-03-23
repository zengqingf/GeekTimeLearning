


namespace Tenmove.Runtime
{
    internal partial class TaskPool
    {
        public class TaskAgent
        {
            private TaskBase m_Task;

            public TaskBase Task
            {
                get { return m_Task; }
            }

            public void Start(TaskBase task)
            {
                if (null != task)
                {
                    m_Task = task;
                    m_Task.Start();
                }
                else
                    Debugger.LogWarning("Task can not be null!");
            }

            public void OnUpdate(float elapseSeconds, float realElapseSeconds)
            {
                m_Task.OnProcess();
            }

            public void Terminate()
            {
                m_Task.OnTerminate();
            }

            public void Reset()
            {
                m_Task.Reset();
                m_Task = null;
            }
        }
    }
}