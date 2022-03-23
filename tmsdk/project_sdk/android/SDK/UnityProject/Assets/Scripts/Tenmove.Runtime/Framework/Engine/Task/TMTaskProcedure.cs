


using System.Collections;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    public enum ProcedureStepState
    {
        /// <summary>
        /// 完成
        /// </summary>
        Done,
        /// <summary>
        /// 处理中
        /// </summary>
        Running,
        /// <summary>
        /// 出错被终结
        /// </summary>
        Terminated,
    }

    public abstract class TaskProcedure
    {
    }

    public abstract class TaskObject
    {
        private bool m_IsTerminated;

        public bool IsTerminated
        {
            get { return m_IsTerminated; }
        }

        public void OnTerminate()
        {
            _OnTerminate();
            m_IsTerminated = true;
        }

        protected abstract void _OnTerminate();
    }

    public class ProcStepResult<T> where T : TaskObject
    {
        public static readonly ProcStepResult<T> Terminated = new ProcStepResult<T>() { NextStep = null, State = ProcedureStepState.Terminated };
        public static readonly ProcStepResult<T> Continue = new ProcStepResult<T>() { NextStep = null, State = ProcedureStepState.Running };
        public static readonly ProcStepResult<T> Finished = new ProcStepResult<T>() { NextStep = null, State = ProcedureStepState.Done };

        public ProcedureStep<T> NextStep { set; get; }
        public ProcedureStepState State { set; get; }
    }
    public delegate ProcStepResult<T> ProcedureStep<T>(Task<T> task) where T : TaskObject;
    public delegate void ProcedureTerminated<T>(Task<T> task) where T : TaskObject;
    public delegate void ProcedureEnd<T>(Task<T> task) where T : TaskObject;


    public partial class TaskProcedure<T> : TaskProcedure where T: TaskObject
    {
        private readonly ProcedureStep<T> m_Begin;
        private readonly ProcedureEnd<T> m_End;
        private readonly ProcedureTerminated<T> m_Terminated;
        private ProcedureStep<T> m_Current;
        private bool m_IsDone;

        private Task<T> m_Task;

        public TaskProcedure(ProcedureStep<T> begin, ProcedureEnd<T> end, ProcedureTerminated<T> terminated)
        {
            Debugger.Assert(null != begin, "Procedure step processor can not be null!");

            m_Begin = begin;
            m_End = end;
            m_Terminated = terminated;
            m_Current = null;
            m_IsDone = false;
            m_Task = null;
        }

        public bool IsDone
        {
            get { return m_IsDone; }
        }        

        public bool Start(Task<T> task)
        {
            if(null == task)
            {
                Debugger.LogWarning( "Task can not be null!");
                return false;
            }

            m_Task = task;
            m_IsDone = false;
            m_Current = m_Begin;
            return true;
        }

        public void OnProcess()
        {
            if(null == m_Task)
            {
                Debugger.LogWarning("You must call 'Start()' before move next!");
                return;
            }

            if (null == m_Current)
            {
                m_IsDone = true;
                return;
            }

            ProcStepResult<T> result = m_Current(m_Task);
            if (null != result)
            {
                switch (result.State)
                {
                    case ProcedureStepState.Done:
                        {
                            if (null == result.NextStep)
                            {
                                if (null != m_End)
                                    m_End(m_Task);
                            }
                            m_Current = result.NextStep;
                        }
                        break;
                    case ProcedureStepState.Running: break;
                    case ProcedureStepState.Terminated:
                        {
                            if (null != m_Terminated)
                                m_Terminated(m_Task);
                            m_Current = null;
                        }
                        break;
                }
            }
            else
                Debugger.LogWarning("Warning: Step result can not be null! Terminated...[Method:{0}]", m_Current.Method.Name);
        }

        public void Terminate()
        {
            m_IsDone = false;
            m_Task = null;
            m_Current = null;
        }

        public void Reset()
        {
            if(!m_IsDone)
            {
                Debugger.LogWarning("You must finish procedure before you reset it!");
                return;
            }

            m_IsDone = false;
            m_Task = null;
            m_Current = null;
        }
    }
}