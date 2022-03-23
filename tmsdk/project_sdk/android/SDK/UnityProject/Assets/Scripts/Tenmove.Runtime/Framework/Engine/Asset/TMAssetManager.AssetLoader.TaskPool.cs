using System.Collections.Generic;

namespace Tenmove.Runtime
{
    internal partial class AssetManager
    {
        private partial class AssetLoader
        {
            private class TaskPool<T> where T : LoadTaskBase
            {
                private readonly Stack<LoadTaskAgent> m_FreeAgents;
                private readonly LinkedList<LoadTaskAgent> m_WorkingAgents;
                private readonly List<LinkedList<T>> m_WaitingTasks;

                public int LoadAgentBaseCount
                {
                    get;
                    set;
                }

                public int LoadAgentExtraCount
                {
                    get;
                    set;
                }

                /// <summary>
                /// ��ʼ������ص���ʵ����
                /// </summary>
                public TaskPool()
                {
                    m_FreeAgents = new Stack<LoadTaskAgent>();
                    m_WorkingAgents = new LinkedList<LoadTaskAgent>();

                    int totalPriorityNum = (int)AssetLoadPriority.Max_Num;
                    m_WaitingTasks = new List<LinkedList<T>>(totalPriorityNum);
                    for(int i = 0; i < totalPriorityNum; ++i)
                    {
                        m_WaitingTasks.Add(new LinkedList<T>());
                    }
                }

                /// <summary>
                /// ��ȡ���������������
                /// </summary>
                public int TotalAgentCount
                {
                    get
                    {
                        return FreeAgentCount + WorkingAgentCount;
                    }
                }

                /// <summary>
                /// ��ȡ�����������������
                /// </summary>
                public int FreeAgentCount
                {
                    get
                    {
                        return m_FreeAgents.Count;
                    }
                }

                /// <summary>
                /// ��ȡ�������������������
                /// </summary>
                public int WorkingAgentCount
                {
                    get
                    {
                        return m_WorkingAgents.Count;
                    }
                }

                /// <summary>
                /// ��ȡ�ȴ�����������
                /// </summary>
                public int WaitingTaskCount
                {
                    get
                    {
                        int iTotal = 0;
                        for (int i = 0,icnt = Utility.Math.Min(m_WaitingTasks.Count, (int)AssetLoadPriority.Max_Num); i < icnt; ++i)
                        {
                            iTotal += m_WaitingTasks[i].Count;
                        }

                        return iTotal;
                    }
                }

                //float interval = 1.0f;
                /// <summary>
                /// �������ѯ��
                /// </summary>
                /// <param name="elapseSeconds">�߼�����ʱ�䣬����Ϊ��λ��</param>
                /// <param name="realElapseSeconds">��ʵ����ʱ�䣬����Ϊ��λ��</param>
                public void Update(float elapseSeconds, float realElapseSeconds)
                {
                    LinkedListNode<LoadTaskAgent> current = m_WorkingAgents.First;
                    while (current != null)
                    {
                        if (current.Value.Task.IsDone)
                        {
                            LinkedListNode<LoadTaskAgent> next = current.Next;
                            current.Value.Reset();
                            m_FreeAgents.Push(current.Value);
                            m_WorkingAgents.Remove(current);
                            current = next;
                            continue;
                        }

                        current.Value.Update(elapseSeconds, realElapseSeconds);
                        current = current.Next;
                    }

                    // ����UnloadUnusedAssets����ͣ��Դ����
                    if (ClientRoot.Instance.IsUnLoadingAssets())
                    {
                        return;
                    }

                    int maxTaskCountPerFrame = TotalAgentCount * (WaitingTaskCount / 1000  + 1);
                    while (FreeAgentCount > 0 && WaitingTaskCount > 0 && maxTaskCountPerFrame > 0)
                    {
                        // �Ӹ����ȼ�Task��ʼ����
                        for(int priority = (int)AssetLoadPriority.Max_Num - 1; priority >= 0; --priority)
                        {
                            if(m_WaitingTasks[priority].Count > 0)
                            {
                                int canUseAgentCount = LoadAgentBaseCount + LoadAgentExtraCount * priority;

                                // �����ǰ����Agent�������������Agent������ֱ���˳�whileѭ����
                                if(m_WorkingAgents.Count >= canUseAgentCount)
                                {
                                    maxTaskCountPerFrame = 0;
                                    break;
                                }

                                LoadTaskAgent agent = m_FreeAgents.Pop();
                                LinkedListNode<LoadTaskAgent> agentNode = m_WorkingAgents.AddLast(agent);

                                T task = m_WaitingTasks[priority].First.Value;
                                m_WaitingTasks[priority].RemoveFirst();
                                agent.Start(task);

                                if (task.IsDone)
                                {
                                    --maxTaskCountPerFrame;
                                    agent.Reset();
                                    m_FreeAgents.Push(agent);
                                    m_WorkingAgents.Remove(agentNode);
                                }
                            }
                        }
                    }
                }

                /// <summary>
                /// �رղ���������ء�
                /// </summary>
                public void Shutdown()
                {
                    while (FreeAgentCount > 0)
                    {
                        m_FreeAgents.Pop().Shutdown();
                    }

                    for (LinkedListNode<LoadTaskAgent> it = m_WorkingAgents.First; null != it; it = it.Next)
                    {
                        LoadTaskAgent workingAgent = it.Value;
                        workingAgent.Shutdown();
                    }
                    m_WorkingAgents.Clear();

                    for (int i = 0, icnt = m_WaitingTasks.Count; i < icnt; ++i)
                        m_WaitingTasks[i].Clear();
                }

                /// <summary>
                /// �����������
                /// </summary>
                /// <param name="agent">Ҫ���ӵ��������</param>
                public void AddAgent(LoadTaskAgent agent)
                {
                    if (agent == null)
                    {
                        throw new TMEngineException("Task agent is invalid.");
                    }

                    agent.Initialize();
                    m_FreeAgents.Push(agent);
                }

                public void ClearFreeAgent()
                {
                    while (FreeAgentCount > 0)
                    {
                        m_FreeAgents.Pop().Shutdown();
                    }

                    LoadAgentBaseCount = 0;
                    LoadAgentExtraCount = 0;
                }

                /// <summary>
                /// ��������
                /// </summary>
                /// <param name="task">Ҫ���ӵ�����</param>
                public void AddTask(T task, int priorityLevel)
                {
                    int realPriority = priorityLevel;
                    if (realPriority < 0)
                        realPriority = 0;
                    else if (realPriority >= (int)AssetLoadPriority.Max_Num)
                        realPriority = (int)AssetLoadPriority.Max_Num - 1;

                    // ��������Priority���������Priority������ӵ����Priority����ͷ����������ӵ�β����
                    if (priorityLevel > realPriority)
                        m_WaitingTasks[realPriority].AddFirst(task);
                    else
                        m_WaitingTasks[realPriority].AddLast(task);
                }

                /// <summary>
                /// �Ƴ�����
                /// </summary>
                /// <param name="serialId">Ҫ�Ƴ���������б�š�</param>
                /// <returns>���Ƴ�������</returns>
                public LoadTaskBase RemoveTask(int taskID)
                {
                    for (int priority = (int)AssetLoadPriority.Max_Num - 1; priority >= 0; --priority)
                    {
                        for (LinkedListNode<T> it = m_WaitingTasks[priority].First; null != it; it = it.Next)
                        {
                            T waitingTask = it.Value;

                            if (waitingTask.TaskID == taskID)
                            {
                                m_WaitingTasks[priority].Remove(it);
                                return waitingTask;
                            }
                        }
                    }

                    for (LinkedListNode<LoadTaskAgent> it = m_WorkingAgents.First; null != it; it = it.Next)
                    {
                        LoadTaskAgent workingAgent = it.Value;
                        if (workingAgent.Task.TaskID == taskID)
                        {
                            LoadTaskBase task = workingAgent.Task;
                            workingAgent.Reset();
                            m_FreeAgents.Push(workingAgent);
                            m_WorkingAgents.Remove(workingAgent);
                            return task;
                        }
                    }

                    return default(LoadTaskBase);
                }

                public bool RemoveTaskGroup(int groupId)
                {
                    // ��Task�Ƿ񱻳ɹ��Ƴ�����Task������DependencyTask�ģ��������Waiting�������У��϶���һ������Task��
                    bool bMainTaskRemoved = false;

                    for (int priority = (int)AssetLoadPriority.Max_Num - 1; priority >= 0; --priority)
                    {
                        LinkedListNode<T> waitingTaskNode = m_WaitingTasks[priority].First;
                        LinkedListNode<T> nextNode;

                        while (waitingTaskNode != null)
                        {
                            nextNode = waitingTaskNode.Next;
                            if (waitingTaskNode.Value.TaskGroupID == groupId)
                            {
                                //waitingTaskNode.Value.Removed = true;
                                bMainTaskRemoved = true;

                                m_WaitingTasks[priority].Remove(waitingTaskNode);
                            }

                            waitingTaskNode = nextNode;
                        }

                    }

                    // �Ѿ�������Task��Ҫ���꣬����ɾ���������Ա��ΪRequestAbort��
                    for (LinkedListNode<LoadTaskAgent> it = m_WorkingAgents.First; null != it; it = it.Next)
                    {
                        LoadTaskAgent workingAgent = it.Value;
                        if (workingAgent.Task.TaskGroupID == groupId)
                        {
                            /// Mark abort
                        }
                    }

                    return bMainTaskRemoved;
                }

                /// <summary>
                /// �Ƴ���������
                /// </summary>
                public void RemoveAllTasks()
                {
                    for(int i = 0,icnt = m_WaitingTasks.Count;i<icnt;++i)
                        m_WaitingTasks[i].Clear();

                    for (LinkedListNode<LoadTaskAgent> it = m_WorkingAgents.First; null != it; it = it.Next)
                    {
                        LoadTaskAgent workingAgent = it.Value;

                        workingAgent.Reset();
                        m_FreeAgents.Push(workingAgent);
                    }
                    m_WorkingAgents.Clear();
                }

                public void BeginCleanRuningTask()
                {

                }

                public bool EndCleanRuningTask()
                {
                    return true;
                }
            }
        }
    }
}