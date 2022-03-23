
using System;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    internal partial class RecycleBin : BaseModule, ITMRecycleBin
    {
        private readonly Dictionary<Type, RecyclePoolBase> m_RecycablePoolTable;
        private readonly ITMRecyclePoolManager m_RecycablePoolManager;

        private Type m_CachedObjectType = null;
        private RecyclePoolBase m_CachedObjectPool = null;

        public RecycleBin()
        {
            ITMRecyclePoolManager recycablePoolManager = ModuleManager.GetModule<ITMRecyclePoolManager>();
            if (null == recycablePoolManager)
                TMDebug.AssertFailed("Recyclable pool manager can not be null!");

            m_RecycablePoolManager = recycablePoolManager;
            m_RecycablePoolTable = new Dictionary<Type, RecyclePoolBase>();
        }

        public sealed override int Priority
        {
            get { return 0; }
        }

        public sealed override void Update(float elapseSeconds, float realElapseSeconds)
        {

        }

        /// <summary>
        /// �رջ���վ�����������еĶ���
        /// </summary>
        public sealed override void Shutdown()
        {
            Dictionary<Type, RecyclePoolBase>.Enumerator it = m_RecycablePoolTable.GetEnumerator();
            while (it.MoveNext())
            {
                RecyclePoolBase curPool = it.Current.Value;
                if (null != curPool)
                    m_RecycablePoolManager.DestroyRecyclePool(curPool);
            }

            m_RecycablePoolTable.Clear();
            m_CachedObjectType = null;
            m_CachedObjectPool = null;
        }

        /// <summary>
        /// ����ָ�����͵Ŀɻ��ն���������
        /// </summary>
        /// <param name="type">ָ������</param>
        /// <param name="reserveCount">��������</param>
        /// <returns></returns>
        public void SetReserveCountOfType(Type type, int reserveCount)
        {
            if(null == type)
            {
                TMDebug.LogWarningFormat("Type can not be null!");
                return;
            }

            if(type == m_CachedObjectType)
            {
                m_CachedObjectPool.SetReserveCount(reserveCount);
                return;
            }

            RecyclePoolBase dstPool = null;
            if(m_RecycablePoolTable.TryGetValue(type,out dstPool))
            {
                m_CachedObjectType = type;
                m_CachedObjectPool = dstPool;
                dstPool.SetReserveCount(reserveCount);
                return;
            }

            TMDebug.LogWarningFormat("Can not find pool for type '{0}'!", type.Name);
        }

        /// <summary>
        /// ��ȡָ�����͵Ŀɻ��ն���������
        /// </summary>
        /// <param name="type">ָ������</param>
        /// <returns>ָ�����͵Ķ���������</returns>
        public int GetResserveCountOfType(Type type)
        {
            if (null == type)
            {
                TMDebug.LogWarningFormat("Type can not be null!");
                return 0;
            }

            if (type == m_CachedObjectType)
                return m_CachedObjectPool.ReserveCount;

            RecyclePoolBase dstPool = null;
            if (m_RecycablePoolTable.TryGetValue(type, out dstPool))
            {
                m_CachedObjectType = type;
                m_CachedObjectPool = dstPool;

                return dstPool.RecycleCount;
            }

            TMDebug.LogWarningFormat("Can not find pool for type '{0}'!", type.Name);
            return 0;
        }

        /// <summary>
        /// �������
        /// </summary>
        /// <returns>��������Ķ���</returns>
        public T Acquire<T>() where T : Recyclable, new()
        {
            Type dstType = typeof(T);
            RecyclePoolBase dstPool = null;

            if (dstType == m_CachedObjectType && null != m_CachedObjectPool)
            {
                return m_CachedObjectPool.QureyInterface<ITMRecyclePool<T>>().Acquire();
            }

            if (m_RecycablePoolTable.TryGetValue(dstType, out dstPool))
            {
                m_CachedObjectType = dstType;
                m_CachedObjectPool = dstPool;

                return dstPool.QureyInterface<ITMRecyclePool<T>>().Acquire();
            }

            dstPool = m_RecycablePoolManager.CreateRecyclePool<T>(null);
            if (null != dstPool)
            {
                m_CachedObjectType = dstType;
                m_CachedObjectPool = dstPool;

                m_RecycablePoolTable.Add(dstType, dstPool);

                return dstPool.QureyInterface<ITMRecyclePool<T>>().Acquire();
            }
            else
                TMDebug.LogErrorFormat("Create recyclable object pool has failed!");

            return null;
        }

        /// <summary>
        /// ���ն���
        /// </summary>
        /// <param name="obj">Ҫ�����յĶ���</param>
        public void Recycle<T>(T obj) where T : Recyclable, new()
        {
            Type dstType = typeof(T);
            if (dstType == m_CachedObjectType)
            {
                m_CachedObjectPool.QureyInterface<ITMRecyclePool<T>>().Recycle(obj);
                return;
            }

            RecyclePoolBase dstPool = null;
            if (m_RecycablePoolTable.TryGetValue(dstType, out dstPool))
            {
                m_CachedObjectType = dstType;
                m_CachedObjectPool = dstPool;

                dstPool.QureyInterface<ITMRecyclePool<T>>().Recycle(obj);
                return;
            }

            TMDebug.AssertFailed(string.Format("Can not find recyclable object pool with type '{0}'!", dstType.Name));
        }

        /// <summary>
        /// ��������ָ�����͵Ķ���
        /// </summary>
        /// <param name="type">Ҫ������������</param>
        public void ClearAllObjectOfType<T>() where T : Recyclable,new()
        {
            Type dstType = typeof(T);

            if (dstType == m_CachedObjectType)
            {
                m_CachedObjectPool.PurgePool(true);
                return;
            }

            RecyclePoolBase dstPool = null;
            if (m_RecycablePoolTable.TryGetValue(dstType, out dstPool))
            {
                m_CachedObjectType = dstType;
                m_CachedObjectPool = dstPool;

                dstPool.PurgePool(true);
                return;
            }

            TMDebug.AssertFailed(string.Format("Can not find recyclable object pool with type '{0}'!", dstType.Name));
        }

        /// <summary>
        /// ��ϴ����
        /// </summary>
        /// <param name="clearAll">�Ƿ��������ж�����������������У����Ϊ������Reserve��ָ���Ķ������</param>
        public void Purge(bool clearAll)
        {
            Dictionary<Type, RecyclePoolBase>.Enumerator it = m_RecycablePoolTable.GetEnumerator();
            while(it.MoveNext())
            {
                RecyclePoolBase curPool = it.Current.Value;
                if (null != curPool)
                    curPool.PurgePool(clearAll);
            }
        }
    }

}

