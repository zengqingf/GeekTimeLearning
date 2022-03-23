


using System;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    public sealed class PurgePoolEventArgs : BaseEventArgs
    {
        public PurgePoolEventArgs(List<string> assetKeys)
        {
            AssetKeys = assetKeys;
        }

        public List<string> AssetKeys
        {
            private set;
            get;
        }
    }

    public enum GameObjectUsage
    {
        Default,
        UI,
        Scene,
        Actor,
        Spark,
    }

    public struct GameObjectPoolInfo
    {
        public GameObjectPoolInfo(string prefabRes, GameObjectUsage usage, int reserveCount,
            float expireTime, int priority, int unusedObjectCount, int usingObjectCount,
            int acquireCount, int recycleCount, int createCount, int releaseCount)
        {
            m_PrefabResPath = prefabRes;
            m_ObjectUsage = usage;
            m_ReserveCount = reserveCount;
            m_ExpireTime = expireTime;
            m_Priority = priority;
            m_UnusedObjectCount = unusedObjectCount;
            m_UsingObjectCount = usingObjectCount;
            m_AcquireCount = acquireCount;
            m_RecycleCount = recycleCount;
            m_CreateCount = createCount;
            m_ReleaseCount = releaseCount;
        }

        public string m_PrefabResPath;
        public GameObjectUsage m_ObjectUsage;
        public int m_ReserveCount;
        public float m_ExpireTime;
        public int m_Priority;
        public int m_UnusedObjectCount;
        public int m_UsingObjectCount;
        public int m_AcquireCount;
        public int m_RecycleCount;
        public int m_CreateCount;
        public int m_ReleaseCount;
    }

    public interface ITMGameObjectPool
    {
        /// <summary>
        /// ��ȡ����GameObject������
        /// </summary>
        /// <returns>����GameObject������</returns>
        int GameObjectPoolCount
        {
            get;
        }

        /// <summary>
        /// ���ö�Ӧ��Դ�Ķ������ز���
        /// </summary>
        /// <param name="prefabRes">Ŀ��GameObject����Դ·��</param>
        /// <param name="objectUsage">Ŀ��GameObject����Դʹ�÷�ʽ</param>
        /// <param name="reserouceCount">Ŀ��GameObjectԤ��������</param>
        /// <param name="expireTime">Ŀ��GameObject�Ĺ���ʱ��</param>
        /// <param name="priority">Ŀ��GameObject�ļ������ȼ�</param>
        void SetObjectPoolDesc(string prefabRes, GameObjectUsage objectUsage, int reserveCount, float expireTime, int priority);

        /// <summary>
        /// Ԥ����count��GameObject�ڶ������
        /// </summary>
        /// <param name="prefabRes"> ��Դ�� </param>
        /// <param name="count"> �������� </param>
        /// <param name="flag"></param>
        bool EnsureGameObjectPoolSync(string prefabRes, int count = 1, uint flag = 0, Action<object, object, int> loadAction = null, object userData = null);

        /// <summary>
        /// �첽��ȡĿ����Դ���͵�GameObject
        /// </summary>
        /// <param name="prefabRes">Ŀ��GameObject����Դ·��</param>
        /// <param name="userData">�Զ�������</param>
        /// <param name="callbacks">�첽��ɻص�</param>
        /// <param name="flag">�����־</param>
        /// <returns>��������ID</returns>
        int AcquireGameObjectAsync(string prefabRes, object userData, AssetLoadCallbacks<object> callbacks, uint flag);

        /// <summary>
        /// �첽��ȡĿ����Դ���͵�GameObject
        /// </summary>
        /// <param name="prefabRes">Ŀ��GameObject����Դ·��</param>
        /// <param name="transform">Ŀ��GameObject�ĳ�ʼ�任</param>
        /// <param name="userData">�Զ�������</param>
        /// <param name="callbacks">�첽��ɻص�</param>
        /// <param name="flag">�����־</param>
        /// <returns>��������ID</returns>
        int AcquireGameObjectAsync(string prefabRes, Math.Transform transform, object userData, AssetLoadCallbacks<object> callbacks, uint flag);

        /// <summary>
        /// ����ʹ����ϵ�GameObject
        /// </summary>
        /// <param name="obj">��Ҫ���յ���Ϸ����</param>
        void RecycleGameObject(object obj);

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="clearAll">�Ƿ��������еĶ��� true��ʾ��Ԥ���κζ���false����Ԥ�������Ķ���</param>
        void PurgePool(bool clearAll);

        /// <summary>
        /// ����ָ������
        /// </summary>
        /// <param name="prefabRes">��Ҫ����GameObject����Դ·��</param>
        void ClearGameObject(string prefabRes);

        /// <summary>
        /// ��ȡ����Ŀ��GameObject�ĳ��ӵ���Ϣ
        /// </summary>
        /// <param name="poolInfoList">���еĳ�����Ϣ</param>
        void GetAllPoolInfo(ref List<GameObjectPoolInfo> poolInfoList);

        /// <summary>
        /// ��GC������׼���������ص�ע��ص�
        /// </summary>
        /// <param name="sender">�¼�������</param>
        /// <param name="args">�¼�����</param>
        void OnPurgePool(object sender, PurgePoolEventArgs args);
    }
}