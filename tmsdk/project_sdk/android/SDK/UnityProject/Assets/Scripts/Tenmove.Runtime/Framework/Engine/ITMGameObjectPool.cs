


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
        /// 获取所有GameObject池子数
        /// </summary>
        /// <returns>所有GameObject池子数</returns>
        int GameObjectPoolCount
        {
            get;
        }

        /// <summary>
        /// 设置对应资源的对象池相关参数
        /// </summary>
        /// <param name="prefabRes">目标GameObject的资源路径</param>
        /// <param name="objectUsage">目标GameObject的资源使用方式</param>
        /// <param name="reserouceCount">目标GameObject预保留数量</param>
        /// <param name="expireTime">目标GameObject的过期时间</param>
        /// <param name="priority">目标GameObject的加载优先级</param>
        void SetObjectPoolDesc(string prefabRes, GameObjectUsage objectUsage, int reserveCount, float expireTime, int priority);

        /// <summary>
        /// 预创建count个GameObject在对象池中
        /// </summary>
        /// <param name="prefabRes"> 资源名 </param>
        /// <param name="count"> 创建数量 </param>
        /// <param name="flag"></param>
        bool EnsureGameObjectPoolSync(string prefabRes, int count = 1, uint flag = 0, Action<object, object, int> loadAction = null, object userData = null);

        /// <summary>
        /// 异步获取目标资源类型的GameObject
        /// </summary>
        /// <param name="prefabRes">目标GameObject的资源路径</param>
        /// <param name="userData">自定义数据</param>
        /// <param name="callbacks">异步完成回调</param>
        /// <param name="flag">请求标志</param>
        /// <returns>本次请求ID</returns>
        int AcquireGameObjectAsync(string prefabRes, object userData, AssetLoadCallbacks<object> callbacks, uint flag);

        /// <summary>
        /// 异步获取目标资源类型的GameObject
        /// </summary>
        /// <param name="prefabRes">目标GameObject的资源路径</param>
        /// <param name="transform">目标GameObject的初始变换</param>
        /// <param name="userData">自定义数据</param>
        /// <param name="callbacks">异步完成回调</param>
        /// <param name="flag">请求标志</param>
        /// <returns>本次请求ID</returns>
        int AcquireGameObjectAsync(string prefabRes, Math.Transform transform, object userData, AssetLoadCallbacks<object> callbacks, uint flag);

        /// <summary>
        /// 回收使用完毕的GameObject
        /// </summary>
        /// <param name="obj">需要回收的游戏对象</param>
        void RecycleGameObject(object obj);

        /// <summary>
        /// 清理对象池
        /// </summary>
        /// <param name="clearAll">是否清理所有的对象 true表示不预留任何对象，false保留预留数量的对象</param>
        void PurgePool(bool clearAll);

        /// <summary>
        /// 清理指定对象
        /// </summary>
        /// <param name="prefabRes">需要清理GameObject的资源路径</param>
        void ClearGameObject(string prefabRes);

        /// <summary>
        /// 获取所有目标GameObject的池子的信息
        /// </summary>
        /// <param name="poolInfoList">所有的池子信息</param>
        void GetAllPoolInfo(ref List<GameObjectPoolInfo> poolInfoList);

        /// <summary>
        /// 当GC管理器准备清理对象池的注册回调
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="args">事件参数</param>
        void OnPurgePool(object sender, PurgePoolEventArgs args);
    }
}