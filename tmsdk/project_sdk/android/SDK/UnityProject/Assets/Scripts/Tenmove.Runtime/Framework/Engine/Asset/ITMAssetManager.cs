using System;
using Tenmove.Runtime.Math;

namespace Tenmove.Runtime
{
    public enum AssetRunMode
    {
        None = 0x00,
        Package = 0x01,
        Updatable = 0x02,
    }

    public sealed class UnloadUnusedAssetEventArgs : BaseEventArgs
    {
        public UnloadUnusedAssetEventArgs(bool ignoreTime = false, Type type = null)
        {
            IgnoreTime = ignoreTime;
            Type = type;
        }

        public bool IgnoreTime
        {
            private set;
            get;
        }

        public Type Type
        {
            private set;
            get;
        }
    }

    public interface ITMAssetManager
    {
        bool IsAssetLoaderReady
        {
            get;
        }

        /// <summary>
        /// 获取当前资源模式
        EnumHelper<AssetRunMode> AssetMode
        {
            get;
        }

        /// <summary>
        /// 是否有资源在异步加载中
        /// </summary>
        /// <returns></returns>
        bool IsAsyncInLoading
        {
            get;
        }
    
        /// <summary>
        /// 获取资源只读区路径。
        /// </summary>
        string ReadOnlyPath
        {
            get;
        }

        /// <summary>
        /// 获取资源读写区路径。
        /// </summary>
        string ReadWritePath
        {
            get;
        }

        /// <summary>
        /// 获取资源包跟目录。
        /// </summary>
        string PackageRootFolder
        {
            get;
        }

        /// <summary>
        /// 获取当前变体。
        /// </summary>
        string CurrentVariant
        {
            get;
        }

        string ApplicationVersion
        {
            get;
        }

        /// <summary>
        /// 获取当前资源内部版本号。
        /// </summary>
        int InternalResourceVersion
        {
            get;
        }
        /// <summary>
        /// 获取已准备完毕资源数量。
        /// </summary>
        int AssetCount
        {
            get;
        }

        /// <summary>
        /// 获取已准备完毕资源数量。
        /// </summary>
        int AssetPackageCount
        {
            get;
        }

        /// <summary>
        /// 获取等待更新资源数量。
        /// </summary>
        int UpdateWaitingCount
        {
            get;
        }
        /// <summary>
        /// 获取正在更新资源数量。
        /// </summary>
        int UpdatingCount
        {
            get;
        }

        /// <summary>
        /// 获取加载资源代理总数量。
        /// </summary>
        int LoadAgentTotalCount
        {
            get;
        }

        /// <summary>
        /// 获取基础加载资源代理总数量。
        /// </summary>
        int LoadAgentBaseCount
        {
            get;
        }

        /// <summary>
        /// 获取额外每优先级级加载资源代理数量。
        /// </summary>
        int LoadAgentExtraCount
        {
            get;
        }

        /// <summary>
        /// 获取可用加载资源代理的数量。
        /// </summary>
        int LoadAgentFreeCount
        {
            get;
        }

        /// <summary>
        /// 获取正在加载资源代理的数量。
        /// </summary>
        int LoadAgentWorkingCount
        {
            get;
        }

        /// <summary>
        /// 获取等待加载资源任务数量。
        /// </summary>
        int LoadTaskWaitingCount
        {
            get;
        }

        /// <summary>
        /// 获取或设置资源对象池自动释放可释放对象的间隔秒数。
        /// </summary>
        float AssetPoolAutoPurgeInterval
        {
            get;
        }

        /// <summary>
        /// 获取资源对象池过期时间（单位：秒）。
        /// </summary>
        float AssetExpireTime
        {
            get;
        }

        /// <summary>
        /// 获取已加载的资源的数量。
        /// </summary>
        int AssetLoadedCount
        {
            get;
        }

        /// <summary>
        /// 获取可释放的资源的数量。
        /// </summary>
        int AssetCanReleaseCount
        {
            get;
        }

        /// <summary>
        /// 获取资源对象池优先级。
        /// </summary>
        int AssetPoolPriority
        {
            get;
        }

        /// <summary>
        /// 获取或设置资源对象池自动释放可释放对象的间隔秒数。
        /// </summary>
        float AssetPackagePoolAutoPurgeInterval
        {
            get;
        }

        /// <summary>
        /// 获取资源包对象池过期时间（单位：秒）。
        /// </summary>
        float AssetPackageExpireTime
        {
            get;
        }

        /// <summary>
        /// 获取已加载的资源包的数量。
        /// </summary>
        int AssetPackageLoadedCount
        {
            get;
        }

        /// <summary>
        /// 获取可释放的资源包的数量。
        /// </summary>
        int AssetPackageCanReleaseCount
        {
            get;
        }

        /// <summary>
        /// 获取资源包对象池优先级。
        /// </summary>
        int AssetPackagePoolPriority
        {
            get;
        }

        /// <summary>
        /// 获取远端资源更新服务器URI。
        /// </summary>
        string RemoteResServerURI
        {
            get;
        }

        /// <summary>
        /// 获取资源池中所有资源的当前信息。
        /// </summary>
        ObjectDesc[] AssetPoolObjectInfos
        {
            get;
        }
        
        /// <summary>
        /// 获取资源包池中所有资源的当前信息。
        /// </summary>
        ObjectDesc[] AssetPackagePoolObjectInfos
        {
            get;
        }

        /// <summary>
        /// 资源初始化完成事件。
        /// </summary>
        event EventHandler<AssetInitCompleteEventArgs> AssetInitComplete;

        /// <summary>
        /// 版本资源列表更新成功事件。
        /// </summary>
        event EventHandler<VersionListUpdateSuccessEventArgs> VersionListUpdateSuccess;

        /// <summary>
        /// 版本资源列表更新失败事件。
        /// </summary>
        event EventHandler<VersionListUpdateFailureEventArgs> VersionListUpdateFailure;

        /// <summary>
        /// 资源检查完成事件。
        /// </summary>
        event EventHandler<AssetCheckCompleteEventArgs> AssetCheckComplete;

        /// <summary>
        /// 资源更新开始事件。
        /// </summary>
        event EventHandler<AssetUpdateStartEventArgs> AssetUpdateStart;

        /// <summary>
        /// 资源更新改变事件。
        /// </summary>
        event EventHandler<AssetUpdateChangedEventArgs> AssetUpdateChanged;

        /// <summary>
        /// 资源更新成功事件。
        /// </summary>
        event EventHandler<AssetUpdateSuccessEventArgs> AssetUpdateSuccess;

        /// <summary>
        /// 资源更新失败事件。
        /// </summary>
        event EventHandler<AssetUpdateFailureEventArgs> AssetUpdateFailure;

        /// <summary>
        /// 资源更新全部完成事件。
        /// </summary>
        event EventHandler<AssetUpdateFinishEventArgs> AssetUpdateFinish;

        /// <summary>
        /// 创建资产加载器。
        /// </summary>
        /// <param name="asyncResLoaderTypeName">异步资源加载器类型名。</param>
        /// <param name="syncResLoaderTypeName">同步资源加载器类型名。</param>
        /// <param name="assetByteLoaderTypeName">字节资源加载器类型名。</param>
        void CreateAssetLoader(string asyncResLoaderTypeName, string syncResLoaderTypeName,string assetByteLoaderTypeName);

        /// <summary>
        /// 设置资产加载代理器数量。
        /// </summary>
        void SetAssetLoadAgentCount(int baseCount, int extraCountPerPriority);

        /// <summary>
        /// 设置资源只读区路径。
        /// </summary>
        /// <param name="readOnlyPath">资源只读区路径。</param>
        void SetReadOnlyPath(string readOnlyPath);

        /// <summary>
        /// 设置资源读写区路径。
        /// </summary>
        /// <param name="readWritePath">资源读写区路径。</param>
        void SetReadWritePath(string readWritePath);

        void AddResourceOnlyPath(string resourceOnlyPath);
        /// <summary>
        /// 设置资源资源模式。
        /// </summary>
        /// <param name="assetmode">资源资源模式。</param>
        void SetAssetMode(uint assetmode);

        /// <summary>
        /// 设置当前变体。
        /// </summary>
        /// <param name="currentVariant">当前变体。</param>
        void SetCurrentVariant(string currentVariant);

        /// <summary>
        /// 设置资源包跟目录。
        /// </summary>
        /// <param name="packageRoot">资源包根目录（应用工作目录的相对路径）。</param>
        void SetPackageRootFolder(string packageRoot);

        /// <summary>
        /// 设置资源池优先级。
        /// </summary>
        /// <param name="priority">优先级。</param>
        void SetAssetPoolPriority(int priority);

        /// <summary>
        /// 设置资源池自动清洗时间间隔。
        /// </summary>
        /// <param name="intervalInSeconds">间隔时间（单位：秒）。</param>
        void SetAssetPoolAutoPurgeInterval(float intervalInSeconds);

        /// <summary>
        /// 设置资源对象过期时间。
        /// </summary>
        /// <param name="expireTime">过期时间（单位：秒）。</param>
        void SetAssetExpireTime(float expireTime);

        /// <summary>
        /// 设置资源包对象池优先级。
        /// </summary>
        /// <param name="priority">优先级。</param>
        void SetAssetPackagePoolPriority(int priority);

        /// <summary>
        /// 设置资源包池自动清洗时间间隔。
        /// </summary>
        /// <param name="intervalInSeconds">间隔时间（单位：秒）。</param>
        void SetAssetPackagePoolAutoPurgeInterval(float intervalInSeconds);

        /// <summary>
        /// 设置资源包对象过期时间。
        /// </summary>
        /// <param name="expireTime">过期时间（单位：秒）。</param>
        void SetAssetPackageExpireTime(float expireTime);

        /// <summary>
        /// 同步加载字节资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <param name="uFlag">加载标志。</param>
        /// <returns>返回要加载的资源内容</returns>
        byte[] LoadAssetByte(string assetName, object userData, uint uFlag = 0u);

        /// <summary>
        /// 同步加载字节资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="fileLoadCallback">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>如果资源加载失败将返回int(~0)</returns>
        int LoadAssetByteSync(string assetName, AssetLoadCallbacks<byte[]> asyncLoadCallback, object userData);

        /// <summary>
        /// 异步加载字节资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="fileLoadCallback">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <param name="uFlag">加载标志。</param>
        /// <returns>如果资源加载失败将返回int(~0)</returns>
        int LoadAssetByteAsync(string assetName, AssetLoadCallbacks<byte[]> asyncLoadCallback, object userData, int priorityLevel = 0);

        /// <summary>
        /// 同步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <param name="uFlag">加载标志。</param>
        /// <returns>返回要加载的资源</returns>
        object LoadAsset(string assetName, System.Type assetType, object userData, uint uFlag = 0u);

        /// <summary>
        /// 同步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="transform">初始变换。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <param name="uFlag">加载标志。</param>
        /// <returns>返回要加载的资源</returns>
        object LoadAsset(string assetName, System.Type assetType, Transform transform, object userData, uint uFlag = 0u);

        /// <summary>
        /// 同步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="assetLoadCallback">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>如果资源加载失败将返回int(~0)</returns>
        int LoadAssetSync(string assetName, System.Type assetType, AssetLoadCallbacks<object> assetLoadCallback, object userData);
        
        /// <summary>
        /// 同步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="transform">初始变换。</param>
        /// <param name="assetLoadCallback">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>如果资源加载失败将返回int(~0)</returns>
        int LoadAssetSync(string assetName, System.Type assetType, Transform transform, AssetLoadCallbacks<object> assetLoadCallback, object userData);

        /// <summary>
        /// 异步步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="assetLoadCallback">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <param name="uFlag">加载标志。</param>
        /// <returns>如果资源加载失败将返回int(~0)</returns>
        int LoadAssetAsync(string assetName, System.Type assetType, AssetLoadCallbacks<object> assetLoadCallback, object userData, int priorityLevel = 0);

        /// <summary>
        /// 异步步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="transform">初始变换。</param>
        /// <param name="assetLoadCallback">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <param name="uFlag">加载标志。</param>
        /// <returns>如果资源加载失败将返回int(~0)</returns>
        int LoadAssetAsync(string assetName, System.Type assetType, Transform transform, AssetLoadCallbacks<object> assetLoadCallback, object userData, int priorityLevel = 0);

        /// <summary>
        /// 预加载一个资源，并设置其lable，然后锁定资源（因为预加载资源外部没有引用，所以必须锁定，否则预加载就没有意义）
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="lableName">资源标签名。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="uFlag">加载标志。</param>
        /// <returns>返回要加载的资源</returns>
        bool PreloadAssetAndLock(string assetName, string lableName, System.Type assetType, AssetLockPhase lockPhase, uint uFlag = 0u);

        /// <summary>
        /// 开始清理没用的资源
        /// </summary>
        void BeginClearUnusedAssets(bool releaseAll);

        /// <summary>
        /// 结束清理没用的资源
        /// </summary>
        /// <returns>返回true说明清理完成，反之未完成</returns>
        bool EndClearUnusedAssets();

        bool BuildAssetTree(ITMAssetTreeData assetTreeData);

        /// <summary>
        /// 锁定或解锁定一个资源
        /// </summary>
        /// <param name="assetName"> 资源名 </param>
        /// <param name="bLock"> 是否锁定 </param>
        void LockAsset(string assetName, bool bLock);

        /// <summary>
        /// 当GC管理器准备清理资源的注册回调
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="args">事件参数</param>
        void OnUnloadUnusedAsset(object sender, UnloadUnusedAssetEventArgs args);

        /// <summary>
        /// 清理所有已经加载的资源（强制释放）
        /// </summary>
        void ClearAllAsset();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ITMAssetPackage ExtractAssetPackageByName(string name);


        /// <summary>
        /// 设置资源加载Callback，外部可以监听，Editor下用。
        /// </summary>
        /// <param name="_callback"></param>
        void SetAssetLoadCallback(Action<string, bool> _callback);
    }
}

