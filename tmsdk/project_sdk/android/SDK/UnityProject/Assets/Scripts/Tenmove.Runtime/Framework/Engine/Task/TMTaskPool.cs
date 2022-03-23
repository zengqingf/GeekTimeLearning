

using System.Collections.Generic;

namespace Tenmove.Runtime
{
    internal partial class TaskPool
    {
        private readonly Stack<TaskAgent> m_FreeAgentList;
        private readonly LinkedList<TaskAgent> m_WorkingAgentList;
        private readonly LinkedList<TaskBase> m_WaitingTaskList;
        private readonly int m_QuotaValue;

        private int m_TaskAllocCount;

        public TaskPool(int quotaValue = 3)
        {
            Debugger.Assert(quotaValue >= 0, "Agent quota value must be a positive value!");
            if(quotaValue > 16)
            {
                quotaValue = 16;
                Debugger.LogInfo("Agent quota value should be limited at 16, force assigned to 16!");
            }

            m_QuotaValue = quotaValue;
            m_FreeAgentList = new Stack<TaskAgent>();
            m_WorkingAgentList = new LinkedList<TaskAgent>();
            m_WaitingTaskList = new LinkedList<TaskBase>();
            m_TaskAllocCount = 0;
        }

        /// <summary>
        /// 获取等待任务数量。
        /// </summary>
        public int WaitingTaskCount
        {
            get
            {
                return m_WaitingTaskList.Count;
            }
        }

        /// <summary>
        /// 获取闲置代理数量。
        /// </summary>
        public int FreeAgentCount
        {
            get
            {
                return m_FreeAgentList.Count;
            }
        }

        public static int InvalidID
        {
            get { return TaskBase.InvalidID; }
        }

        public int AddTask<T>(T data, TaskProcedure<T> procedure, string tag, TaskPriority priority = TaskPriority.Normal) where T: TaskObject
        {
            Task<T> newTask = new Task<T>(_AllocTaskID(), priority, data, procedure,tag);
            m_WaitingTaskList.AddLast(newTask);
            return newTask.TaskID;
        }

        public bool IsTaskStart(int taskID)
        {
            for (LinkedListNode<TaskBase> it = m_WaitingTaskList.First; null != it; it = it.Next)
            {
                if (it.Value.TaskID == taskID)
                    return true;
            }

            for (LinkedListNode<TaskAgent> it = m_WorkingAgentList.First; null != it; it = it.Next)
            {
                if (it.Value.Task.TaskID == taskID)
                    return true;
            }

            return false;
        }

        public TaskBase RemoveTask(int taskID)
        {
            for (LinkedListNode<TaskBase> it = m_WaitingTaskList.First; null != it; it = it.Next)
            {
                TaskBase waitingTask = it.Value;

                if (waitingTask.TaskID == taskID)
                {
                    m_WaitingTaskList.Remove(it);
                    return waitingTask;
                }
            }

            for (LinkedListNode<TaskAgent> it = m_WorkingAgentList.First; null != it; it = it.Next)
            {
                TaskAgent workingAgent = it.Value;
                if (workingAgent.Task.TaskID == taskID)
                {
                    TaskBase task = workingAgent.Task;
                    workingAgent.Reset();
                    m_FreeAgentList.Push(workingAgent);
                    m_WorkingAgentList.Remove(it);
                    return task;
                }
            }

            return default(TaskBase);
        }



        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            /// 检查并执行当前的任务，完成任务的Agent压入空闲Agent栈
            LinkedListNode<TaskAgent> curAgent = m_WorkingAgentList.First;
            while (null != curAgent)
            {
                if (curAgent.Value.Task.IsDone)
                {
                    LinkedListNode<TaskAgent> next = curAgent.Next;
                    curAgent.Value.Reset();
                    m_FreeAgentList.Push(curAgent.Value);
                    m_WorkingAgentList.Remove(curAgent);
                    curAgent = next;
                    continue;
                }

                curAgent.Value.OnUpdate(elapseSeconds, realElapseSeconds);
                curAgent = curAgent.Next;
            }

            /// 动态修正值
            int adjustValue = WaitingTaskCount / 1000;

            /// 检查任务队列中的下一个任务
            LinkedListNode<TaskBase> curTask = m_WaitingTaskList.First;
            while(null != curTask)
            {
                LinkedListNode<TaskBase> next = curTask.Next;
                int agentQuota = ((int)curTask.Value.Priority + 1) * m_QuotaValue + adjustValue;
                if(m_WorkingAgentList.Count < agentQuota)
                {
                    TaskAgent agent = _AllocTaskAgent();
                    agent.Start(curTask.Value);
                    m_WorkingAgentList.AddLast(agent);
                    m_WaitingTaskList.Remove(curTask);
                    curTask = next;
                    continue;
                }

                curTask = next;
            }
        }

        public void TerminateTaskByTag(string tag)
        {
            LinkedListNode<TaskBase> cur = m_WaitingTaskList.First;
            LinkedListNode<TaskBase> next;
            while (null != cur)
            {
                next = cur.Next;
                if (null == cur.Value || cur.Value.Tag == tag)
                {
                    if(null != cur.Value)
                        cur.Value.OnTerminate();
                    m_WaitingTaskList.Remove(cur);
                }
                cur = next;
            }

            LinkedListNode<TaskAgent> curAgent = m_WorkingAgentList.First;
            LinkedListNode<TaskAgent> nextAgent;
            while (null != curAgent)
            {
                nextAgent = curAgent.Next;
                if (null == curAgent.Value || curAgent.Value.Task.Tag == tag)
                {
                    if (null != curAgent.Value)
                    {
                        curAgent.Value.Terminate();
                        curAgent.Value.Reset();
                    }

                    m_FreeAgentList.Push(curAgent.Value);
                    m_WorkingAgentList.Remove(curAgent);
                }

                curAgent = nextAgent;
            }
        }

        public void TerminateAll()
        {
            LinkedListNode<TaskBase> cur = m_WaitingTaskList.First;
            LinkedListNode<TaskBase> next;
            while (null != cur)
            {
                next = cur.Next;
                TaskBase waitingTask = cur.Value;
                if(null != cur.Value)
                    cur.Value.OnTerminate();

                m_WaitingTaskList.Remove(cur);
                cur = next;
            }

            LinkedListNode<TaskAgent> curAgent = m_WorkingAgentList.First;
            LinkedListNode<TaskAgent> nextAgent;
            while (null != curAgent)
            {
                nextAgent = curAgent.Next;
                if (null != curAgent.Value)
                {
                    curAgent.Value.Terminate();
                    curAgent.Value.Reset();
                }

                m_FreeAgentList.Push(curAgent.Value);
                m_WorkingAgentList.Remove(curAgent);
                curAgent = nextAgent;
            }
        }

        private TaskAgent _AllocTaskAgent()
        {
            if (m_FreeAgentList.Count > 0)
                return m_FreeAgentList.Pop();

            return new TaskAgent();
        }

        private int _AllocTaskID()
        {
            int taskID = m_TaskAllocCount++;
            if (TaskBase.InvalidID == taskID)
                return _AllocTaskID();

            return taskID;
        }
    }
}