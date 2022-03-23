using System;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    public abstract class RecyclePoolBase
    {
        private readonly int m_PoolID;

        protected int m_UsingObjectCount;
        protected int m_AcquireCount;
        protected int m_RecycleCount;
        protected int m_CreateCount;
        protected int m_ReleaseCount;
        protected int m_ReserveCount;
        protected int m_Priority;
        protected long m_TimeStamp;

        public RecyclePoolBase(int poolID, int reserveCount)
        {
            if (~0 == poolID)
                TMDebug.AssertFailed("Pool ID is invalid!");

            m_PoolID = poolID;

            m_UsingObjectCount = 0;
            m_AcquireCount = 0;
            m_RecycleCount = 0;
            m_CreateCount = 0;
            m_ReleaseCount = 0;
            m_ReserveCount = reserveCount;
        }

        public int ReserveCount
        {
            get { return m_ReserveCount; }
        }

        public float ExpireTime
        {
            get { return Utility.Time.TicksToSeconds(m_TimeStamp); }
        }

        public int Priority
        {
            get { return m_Priority; }
        }

        public abstract int UnusedObjectCount
        {
            get;
        }

        public int UsingObjectCount
        {
            get { return m_UsingObjectCount; }
        }

        public int AcquireCount
        {
            get { return m_AcquireCount; }
        }

        public int RecycleCount
        {
            get { return m_RecycleCount; }
        }

        public int CreateCount
        {
            get { return m_CreateCount; }
        }

        public int ReleaseCount
        {
            get { return m_ReleaseCount; }
        }

        public int PoolID
        {
            get { return m_PoolID; }
        }

        public void SetReserveCount(int reserveCount)
        {
            if (reserveCount < 0)
            {
                TMDebug.LogWarningFormat("Reserve count can not less than zero!");
                return;
            }

            m_ReserveCount = reserveCount;
        }

        public void SetExpireTime(float expireTime)
        {
            m_TimeStamp = Utility.Time.SecondsToTicks(expireTime);
        }


        public void SetPriority(int priority)
        {
            m_Priority = priority;
        }

        public abstract I QureyInterface<I>() where I : class;

        /// <summary>
        /// ��ϴ����
        /// </summary>
        /// <param name="clearAll">�Ƿ��������ж�����������������У����Ϊ������Reserve��ָ���Ķ������</param>
        public abstract void PurgePool(bool clearAll);

        /// <summary>
        /// �رճ��ӣ����������еĶ���
        /// </summary>
        public abstract void Shutdown();
    }
}

