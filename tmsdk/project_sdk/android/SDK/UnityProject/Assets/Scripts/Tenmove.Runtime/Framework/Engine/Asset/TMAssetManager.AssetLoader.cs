using System;
using System.Collections.Generic;
using Tenmove.Runtime.Math;

namespace Tenmove.Runtime
{
    internal partial class AssetManager
    {
        private partial class AssetLoader
        {
            public static readonly int INVALID_HANDLE = ~0;

            private readonly AssetManager m_AssetManager;

            private readonly TaskPool<LoadTaskBase> m_TaskPool;
            private readonly ITMReferencePool<Asset> m_AssetPool;
            private readonly ITMReferencePool<AssetPackage> m_AssetPackagePool;
            private readonly string m_ResAsyncLoaderTypeName;
            private readonly string m_ResSyncLoaderTypeName;
            private readonly string m_AssetByteLoaderTypeName;

            private readonly ResSyncLoader m_ResSyncLoader;
            private readonly LinkedList<ResAsyncLoader> m_ResAsyncLoaderList;
            private readonly AssetByteLoader m_AssetByteLoader;

#if UNITY_EDITOR
            private Action<string, bool> m_AssetLoadCallback;
            public void SetAssetLoadCallback(Action<string, bool> _callback)
            {
                m_AssetLoadCallback = _callback;
            }

            public void TriggerAssetLoadCallback(string assetName, bool bSync)
            {
                if(m_AssetLoadCallback != null)
                    m_AssetLoadCallback(assetName, bSync);
            }
#endif

            private struct AsyncLoadFailedCallbackContext
            {
                public OnAssetLoadFailure OnAssetLoadFailure { set; get; }
                public string AssetPath { set; get; }
                public int TaskGroupID { set; get; }
                public AssetLoadErrorCode AssetLoadErrorCode { set; get; }
                public string Message { set; get; }
                public object UserData { set; get; }
            }
            private readonly List<AsyncLoadFailedCallbackContext> m_LoadFailedCallbackContext;

            private struct SyncLoadCallbackContext
            {
                public string AssetPath { set; get; }
                public AssetLoadCallbacks<object> AsyncLoadCallbacks { set; get; }
                public object UserData { set; get; }
                public object Asset { set; get; }
                public int TaskGroupID { set; get; }
            }
            private readonly List<SyncLoadCallbackContext> m_SyncLoadCallbackContext;

            private class AssetByteLoadCallbackContext
            {
                public AssetLoadCallbacks<byte[]> AsyncLoadCallbacks { set; get; }
                public object UserData { set; get; }
            }
            
            private int m_TaskGroupIDCnt = 0;

            private readonly string[] m_InvalidDependencyAssetName = new string[0];

            public AssetLoader(AssetManager assetManager, ITMReferencePool<Asset> assetPool, ITMReferencePool<AssetPackage> assetPackagePool, string resAsyncLoaderTypeName, string resSyncLoaderTypeName,string assetByteLoaderTypeName)
            {
                if (null == assetManager)
                    TMDebug.AssertFailed("Asset manager can not be null!");

                if (null == assetPool)
                    TMDebug.AssertFailed("Asset pool can not be null!");

                if (null == assetPackagePool)
                    TMDebug.AssertFailed("Asset package pool can not be null!");

                if (string.IsNullOrEmpty(resAsyncLoaderTypeName))
                    TMDebug.AssertFailed("Res-async loader type name can not be null or empty!");

                if (string.IsNullOrEmpty(resAsyncLoaderTypeName))
                    TMDebug.AssertFailed("Res-sync loader type name can not be null or empty!");

                m_AssetManager = assetManager;
                m_TaskPool = new TaskPool<LoadTaskBase>();
                m_AssetPool = assetPool;
                m_AssetPackagePool = assetPackagePool;
                m_ResAsyncLoaderTypeName = resAsyncLoaderTypeName;
                m_ResSyncLoaderTypeName = resSyncLoaderTypeName;
                m_AssetByteLoaderTypeName = assetByteLoaderTypeName;
                m_ResAsyncLoaderList = new LinkedList<ResAsyncLoader>();
                m_ResSyncLoader = _CreateResourceLoader(resSyncLoaderTypeName) as ResSyncLoader;
                if (null == m_ResSyncLoader)
                    TMDebug.AssertFailed("Create resource sync-loader has failed!");

                m_AssetByteLoader = _CreateAssetByteLoader(assetByteLoaderTypeName);

                m_LoadFailedCallbackContext = new List<AsyncLoadFailedCallbackContext>();
                m_SyncLoadCallbackContext = new List<SyncLoadCallbackContext>();
                m_TaskGroupIDCnt = 0;
            }

            public string ReadOnlyPath
            {
                get { return m_AssetManager.ReadOnlyPath; }
            }

            public string ReadWritePath
            {
                get { return m_AssetManager.ReadWritePath; }
            }

            public string PackageRootFolder
            {
                get { return m_AssetManager.PackageRootFolder; }
            }

            public int LoadAgentTotalCount
            {
                get { return m_TaskPool.TotalAgentCount; }
            }

            public int LoadAgentBaseCount
            {
                get { return m_TaskPool.LoadAgentBaseCount; }
            }

            public int LoadAgentExtraCount
            {
                get { return m_TaskPool.LoadAgentExtraCount; }
            }

            public int FreeAgentCount
            {
                get { return m_TaskPool.FreeAgentCount; }
            }

            public int WorkingAgentCount
            {
                get { return m_TaskPool.WorkingAgentCount; }
            }

            public int WaitingTaskCount
            {
                get { return m_TaskPool.WaitingTaskCount; }
            }

            public ResSyncLoader ResSyncLoader
            {
                get { return m_ResSyncLoader; }
            }

            public bool IsAssetExist(string assetName, System.Type assetType, bool loadFromPackage)
            {
                string mainResPath;
                string subResName;
                _ParseAssetPath(assetName, out mainResPath, out subResName);

                int assetPackageID = INVALID_HANDLE;
                string assetNameInPackage = null;
                AssetPackageDesc? assetPackageDesc = null;
                return _CheckAsset(mainResPath, loadFromPackage, out assetPackageDesc, out assetNameInPackage, out assetPackageID);
            }

            public byte[] LoadAssetByte(string assetName, object userData, bool loadFromPackage, uint uFlag = 0u)
            {
                object nativeAsset = LoadAsset(assetName, m_AssetByteLoader.NativeByteAssetType, Transform.Identity, userData, loadFromPackage, uFlag);
                return m_AssetByteLoader.LoadAssetByte(nativeAsset).AssetBytes;
            }

            public int LoadAssetByteSync(string assetName, AssetLoadCallbacks<byte[]> fileLoadCallback, object userData, bool loadFromPackage)
            {
                return LoadAssetSync(assetName, m_AssetByteLoader.NativeByteAssetType, Transform.Identity,
                    new AssetLoadCallbacks<object>(_OnByteAssetLoadSuccess, _OnByteAssetLoadFailure),
                    new AssetByteLoadCallbackContext() { AsyncLoadCallbacks = fileLoadCallback, UserData = userData }, loadFromPackage);
            }

            public int LoadAssetByteAsync(string assetName, AssetLoadCallbacks<byte[]> fileLoadCallback, object userData, bool loadFromPackage, int priorityLevel = 0)
            {
                return LoadAssetAsync(assetName, m_AssetByteLoader.NativeByteAssetType,Transform.Identity,
                    new AssetLoadCallbacks<object>(_OnByteAssetLoadSuccess, _OnByteAssetLoadFailure),
                    new AssetByteLoadCallbackContext() { AsyncLoadCallbacks = fileLoadCallback, UserData = userData }, loadFromPackage, priorityLevel);
            }

            public object LoadAsset(string assetName, System.Type assetType, object userData, bool loadFromPackage, uint uFlag = 0u)
            {
                return _LoadAsset(assetName, assetType, false, Transform.Identity, userData, loadFromPackage, uFlag);
            }

            public object LoadAsset(string assetName, System.Type assetType, Transform transform, object userData, bool loadFromPackage, uint uFlag = 0u)
            {
                return _LoadAsset(assetName, assetType, true, transform, userData, loadFromPackage, uFlag);
            }

            public int LoadAssetSync(string assetName, System.Type assetType, AssetLoadCallbacks<object> assetLoadCallback, object userData, bool loadFromPackage)
            {
                return _LoadAssetSync(assetName, assetType, false, Transform.Identity, assetLoadCallback, userData, loadFromPackage);
            }

            public int LoadAssetSync(string assetName, System.Type assetType, Transform transform, AssetLoadCallbacks<object> assetLoadCallback, object userData, bool loadFromPackage)
            {
                return _LoadAssetSync(assetName, assetType, true, transform, assetLoadCallback, userData, loadFromPackage);
            }

            public int LoadAssetAsync(string assetName, System.Type assetType,AssetLoadCallbacks<object> assetLoadCallback, object userData, bool loadFromPackage, int priorityLevel = 0)
            {
                return _LoadAssetAsync(assetName, assetType, false, Transform.Identity, assetLoadCallback, userData, loadFromPackage, priorityLevel);
            }

            public int LoadAssetAsync(string assetName, System.Type assetType,Transform transform, AssetLoadCallbacks<object> assetLoadCallback, object userData, bool loadFromPackage, int priorityLevel = 0)
            {
                return _LoadAssetAsync(assetName, assetType, true, transform, assetLoadCallback, userData, loadFromPackage, priorityLevel);
            }

            /// <summary>
            /// 预加载一个资源，并设置其lable，然后锁定资源（因为预加载资源外部没有引用，所以必须锁定，否则预加载就没有意义）
            /// </summary>
            public bool PreloadAssetAndLock(string assetName, string lableName, System.Type assetType, bool loadFromPackage, AssetLockPhase lockPhase, uint uFlag = 0u)
            {
                string mainResPath;
                string subResName;
                _ParseAssetPath(assetName, out mainResPath, out subResName);

                Asset asset = m_AssetPool.Spawn(assetName);
                if (null != asset)
                {
                    TMAssetManagerHelper.AddAssetLable(assetName, lableName);
                    TMAssetManagerHelper.LockLable(lableName, lockPhase);
                    return true;
                }

                object assetObj = null;
                AssetPackage assetPackage = null;
                string assetNameInPackage = null;
                int assetPackageID = INVALID_HANDLE;

                AssetPackageDesc? assetPackageDesc = null;
                if (!_CheckAsset(mainResPath, loadFromPackage, out assetPackageDesc, out assetNameInPackage, out assetPackageID))
                {
                    TMDebug.LogWarningFormat("Asset with name [{0}] check failed!", assetName);
                    return false;
                }

                if (loadFromPackage)
                {
                    List<AssetPackage> assetPackageList = null;
                    if (assetPackageDesc.HasValue && !string.IsNullOrEmpty(assetNameInPackage))
                    {
                        if (!_LoadPackageSync(assetPackageID, false, null, ref assetPackageList, ref assetPackage))
                        {
                            TMDebug.LogErrorFormat("Can not load dependency package '{0}' when load asset '{1}'!", assetPackageDesc.Value.PackageName.Name, assetName);
                            return false;
                        }
                    }
                }

                if (null == assetPackage)
                    assetObj = m_ResSyncLoader.LoadAsset(null, mainResPath, subResName, assetType);
                else
                    assetObj = m_ResSyncLoader.LoadAsset(assetPackage.Object, assetNameInPackage, subResName, assetType);

                if (null == assetObj)
                    return false;

                asset = new Asset(assetName, assetObj as ITMAssetObject, assetPackage, m_AssetPackagePool);
                m_AssetPool.Register(asset, true);

                TMAssetManagerHelper.AddAssetLable(assetName, lableName);
                TMAssetManagerHelper.LockLable(lableName, lockPhase);

                return true;
            }

            public void SetAssetLoadAgentCount(int baseCount, int extraCountPerPriority)
            {
                _ClearAssetLoadAgent();

                if (baseCount > 64)
                    baseCount = 64;

                // 创建Normal优先级可用Agent
                for (int i = 0, icnt = baseCount; i < icnt; ++i)
                {
                    LoadTaskAgent agent = new LoadTaskAgent(m_AssetPool, m_AssetPackagePool, this);
                    m_TaskPool.AddAgent(agent);
                }

                // 创建其他优先级额外可用Agent
                for (int i = (int)AssetLoadPriority.Normal + 1, icnt = (int)AssetLoadPriority.Max_Num; i < icnt; ++i)
                {
                    for (int j = 0; j < extraCountPerPriority; ++j)
                    {
                        LoadTaskAgent agent = new LoadTaskAgent(m_AssetPool, m_AssetPackagePool, this);
                        m_TaskPool.AddAgent(agent);
                    }
                }

                m_TaskPool.LoadAgentBaseCount = baseCount;
                m_TaskPool.LoadAgentExtraCount = extraCountPerPriority;
            }

            /// <summary>
            /// 加载资源器轮询。
            /// </summary>
            /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
            /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
            public void Update(float elapseSeconds, float realElapseSeconds)
            {
                for(int i = 0,icnt = m_LoadFailedCallbackContext.Count;i<icnt;++i)
                {
                    m_LoadFailedCallbackContext[i].OnAssetLoadFailure(
                        m_LoadFailedCallbackContext[i].AssetPath,
                        m_LoadFailedCallbackContext[i].TaskGroupID,
                        m_LoadFailedCallbackContext[i].AssetLoadErrorCode,
                        m_LoadFailedCallbackContext[i].Message,
                        m_LoadFailedCallbackContext[i].UserData );
                }
                m_LoadFailedCallbackContext.Clear();

                for(int i = 0,icnt = m_SyncLoadCallbackContext.Count;i<icnt;++i)
                {
                    SyncLoadCallbackContext context = m_SyncLoadCallbackContext[i];
                    object asset = context.Asset;
                    if (null != asset)
                    {
                        context.AsyncLoadCallbacks.OnAssetLoadSuccess(
                            context.AssetPath,
                            asset,
                            context.TaskGroupID,
                            0,
                            context.UserData
                            );
                    }
                    else
                    {
                        context.AsyncLoadCallbacks.OnAssetLoadFailure(
                            context.AssetPath,
                            context.TaskGroupID,
                            AssetLoadErrorCode.NotExist,
                            string.Format("Sync load asset with path '{0}' has failed!", context.AssetPath),
                            context.UserData
                            );
                    }
                }
                m_SyncLoadCallbackContext.Clear();

                m_TaskPool.Update(elapseSeconds, realElapseSeconds);
            }

            private void _ClearAssetLoadAgent()
            {
                m_TaskPool.ClearFreeAgent();
            }

            private int _LoadAssetSync(string assetName, System.Type assetType, bool overrideTransform, Transform transform, AssetLoadCallbacks<object> assetLoadCallback, object userData, bool loadFromPackage)
            {
                int taskID = m_TaskGroupIDCnt++;
                SyncLoadCallbackContext context = new SyncLoadCallbackContext()
                {
                    AssetPath = assetName,
                    Asset = _LoadAsset(assetName, assetType, overrideTransform,transform, userData, loadFromPackage),
                    AsyncLoadCallbacks = assetLoadCallback,
                    UserData = userData,
                    TaskGroupID = taskID,
                };

                m_SyncLoadCallbackContext.Add(context);
                return taskID;
            }

            private object _LoadAsset(string assetName, System.Type assetType, bool overrideTransform,Transform transform, object userData, bool loadFromPackage, uint uFlag = 0u)
            {
#if ENABLE_PROFILER
                TMProfiler.BeginSample(TMProfileModule.Engine, "LoadAssetSync");
                try
                {
#endif
                    string mainResPath;
                    string subResName;
                    _ParseAssetPath(assetName, out mainResPath, out subResName);

                    // 检测Package是否正在异步加载队列，如果在，强制让其以同步方式完成Task
                    if (!_CheckPackageInLoading(assetName))
                    {
                        Debugger.LogError("Asset '{0}' in async-loading, force sync failed!", assetName);
                        return false;
                    }

                    Asset asset = m_AssetPool.Spawn(assetName);
                    if (null != asset)
                        return asset.CreateAssetInst(overrideTransform, transform);

                    object assetObj = null;
                    AssetPackage assetPackage = null;
                    string assetNameInPackage = null;
                    int assetPackageID = INVALID_HANDLE;

                    AssetPackageDesc? assetPackageDesc = null;
                    if (!_CheckAsset(mainResPath, loadFromPackage, out assetPackageDesc, out assetNameInPackage, out assetPackageID))
                    {
                        TMDebug.LogWarningFormat("Asset with name [{0}] check failed!", assetName);
                        return null;
                    }

                    if (loadFromPackage)
                    {
                        List<AssetPackage> assetPackageList = null;
                        if (assetPackageDesc.HasValue && !string.IsNullOrEmpty(assetNameInPackage))
                        {
                            if (!_LoadPackageSync(assetPackageID, false, userData, ref assetPackageList, ref assetPackage))
                            {
                                TMDebug.LogErrorFormat("Can not load dependency package '{0}' when load asset '{1}'!", assetPackageDesc.Value.PackageName.Name, assetName);
                                return null;
                            }
                        }
                    }

                    if (null == assetPackage)
                        assetObj = m_ResSyncLoader.LoadAsset(null, mainResPath, subResName, assetType);
                    else
                        assetObj = m_ResSyncLoader.LoadAsset(assetPackage.Object, assetNameInPackage, subResName, assetType);

                    if (null == assetObj)
                        return null;

#if UNITY_EDITOR
                    TriggerAssetLoadCallback(assetName, true);
#endif

                    asset = new Asset(assetName, assetObj as ITMAssetObject, assetPackage, m_AssetPackagePool);
                    m_AssetPool.Register(asset, true);
                    return asset.CreateAssetInst(overrideTransform, transform);
#if ENABLE_PROFILER
                } 
                finally
                {
                    TMProfiler.EndSample(TMProfileModule.Engine, "LoadAssetSync");
                }
#endif
          }

            private int _LoadAssetAsync(string assetName, System.Type assetType,bool overrideTransform,Transform transform, AssetLoadCallbacks<object> assetLoadCallback, object userData, bool loadFromPackage, int priorityLevel = 0)
            {
                string mainResPath;
                string subResName;
                _ParseAssetPath(assetName, out mainResPath, out subResName);

                AssetPackageDesc? assetPackageDesc = null;
                string assetNameInPackage = null;
                int assetPackageID = INVALID_HANDLE;

                if (!_CheckAsset(mainResPath, loadFromPackage, out assetPackageDesc, out assetNameInPackage, out assetPackageID))
                {
                    m_LoadFailedCallbackContext.Add(new AsyncLoadFailedCallbackContext()
                    {
                        OnAssetLoadFailure = assetLoadCallback.OnAssetLoadFailure,
                        AssetPath = assetName,
                        TaskGroupID = INVALID_HANDLE,
                        AssetLoadErrorCode = AssetLoadErrorCode.NotExist,
                        Message = string.Format("Asset with name [{0}] check asset has failed!", assetName),
                        UserData = userData
                    });

                    return INVALID_HANDLE;
                }

                AssetTask mainTask = new AssetTask(this, m_AssetPool, m_AssetPackagePool, mainResPath, mainResPath, subResName, assetType, overrideTransform, transform, assetLoadCallback, assetPackageDesc.Value,
                    assetNameInPackage, m_TaskGroupIDCnt++, userData);

                if (loadFromPackage)
                {
                    if (!_LoadPackageAsync(assetPackageID, false, mainTask, userData, priorityLevel))
                    {
                        string errorMessage = string.Format("Can not load package '{0}' when load asset '{1}'!", assetPackageDesc.Value.PackageName.Name, assetName);
                        m_LoadFailedCallbackContext.Add(new AsyncLoadFailedCallbackContext()
                        {
                            OnAssetLoadFailure = assetLoadCallback.OnAssetLoadFailure,
                            AssetPath = assetName,
                            TaskGroupID = INVALID_HANDLE,
                            AssetLoadErrorCode = AssetLoadErrorCode.DependencyLoadError,
                            Message = errorMessage,
                            UserData = userData
                        });

                        return INVALID_HANDLE;
                    }
                }

                m_TaskPool.AddTask(mainTask, priorityLevel);
                return mainTask.TaskGroupID;
            }

            private bool _LoadPackageSync(int assetPackageDescID,bool asDependency, object userData, ref List<AssetPackage> assetPackageList, ref AssetPackage assetPackage)
            {
                AssetPackageDesc assetPackageDesc;
                if (m_AssetManager.m_AssetPackageDescTable.TryGetValueAt(assetPackageDescID, out assetPackageDesc))
                {
                    // 检测Package是否正在异步加载队列，如果在，强制让其以同步方式完成Task
                    if(!_CheckPackageInLoading(assetPackageDesc.PackageName.Name))
                    {
                        Debugger.LogError("Package '{0}' in async-loading, force sync failed!", assetPackageDesc.PackageName.Name);
                        return false;
                    }

                    AssetPackage curAssetPackage = m_AssetPackagePool.Spawn(assetPackageDesc.PackageName.Name);
                    if (null != curAssetPackage)
                    {
                        if (asDependency)
                        {
                            assetPackageList.Add(curAssetPackage);
                            return true;
                        }
                        else
                        {
                            if(!curAssetPackage.AsDependency)
                            {
                                assetPackage = curAssetPackage;
                                return true;
                            }
                        }
                    }

                    if (!asDependency)
                    {
                        /// 要重新加载依赖
                        assetPackageList = new List<AssetPackage>();
                        int[] dependencyPackageIDs = assetPackageDesc.DependencyPackageIDs;
                        for (int i = 0, icnt = dependencyPackageIDs.Length; i < icnt; ++i)
                        {
                            if (!_LoadPackageSync(dependencyPackageIDs[i], true, userData, ref assetPackageList, ref assetPackage))
                            {
                                TMDebug.LogErrorFormat("Can not load dependency package when load asset package'{0}'!", assetPackageDesc.PackageName.Name);
                                return false;
                            }
                        }
                    }

                    if (null == curAssetPackage)
                    {
                        string packageFullName = assetPackageDesc.PackageName.Name;
                        /// string packageLoadPath = Utility.Path.Combine(
                        ///     assetPackageDesc.StorageInReadOnly ? ReadOnlyPath : ReadWritePath,
                        ///     Utility.Path.Combine(PackageRootFolder, packageFullName));
                        /// object package = m_ResSyncLoader.LoadPackage(packageLoadPath);

                        string packageLoadPath = Utility.Path.Combine(ReadWritePath,
                            Utility.Path.Combine(PackageRootFolder, packageFullName));
                        if (!_IsAssetExistWithPath(packageLoadPath))
                        {
                            packageLoadPath = Utility.Path.Combine(ReadOnlyPath,
                                Utility.Path.Combine(PackageRootFolder, packageFullName));
                        }
                        object package = m_ResSyncLoader.LoadPackage(packageLoadPath);

                        if (null == package)
                            return false;

                    #if UNITY_EDITOR
                        TriggerAssetLoadCallback(packageFullName, true);
                    #endif

                        curAssetPackage = new AssetPackage(packageFullName, package, m_ResSyncLoader, m_AssetPackagePool);
                        m_AssetPackagePool.Register(curAssetPackage, true);
                    }
                   
                    if(asDependency)
                        assetPackageList.Add(curAssetPackage);
                    else
                        curAssetPackage.AddDependency(assetPackageList);

                    assetPackage = curAssetPackage;
                    return true;
                }
                else
                    TMDebug.LogErrorFormat("Can not load dependency package when load package '{0}'!", assetPackageDesc.PackageName.Name, assetPackageDesc.PackageName.Name);

                return false;
            }

            private bool _LoadPackageAsync(int assetPackageID,bool asDependency,LoadTaskBase mainTask, object userData,int priorityLevel)
            {
                TMDebug.Assert(null != mainTask, "Main task can not be null!");

                AssetPackageDesc assetPackageDesc;
                if (m_AssetManager.m_AssetPackageDescTable.TryGetValueAt(assetPackageID, out assetPackageDesc))
                {
                    bool bNeedCheckDependency = true;

/*
                    AssetPackage curAssetPackage = m_AssetPackagePool.Spawn(assetPackageDesc.PackageName.Name);
                    if (asDependency && null != curAssetPackage)
                    {
                        if (asDependency)
                        {
                            return true;
                        }
                        else
                        {
                            bNeedCheckDependency = curAssetPackage.AsDependency;
                        }
                    }*/

                    PackageTask packageLoadTask = new PackageTask(this,m_AssetPool,m_AssetPackagePool,assetPackageDesc.PackageName.Name, mainTask,assetPackageDesc,userData, asDependency);
                    if (!asDependency && bNeedCheckDependency)
                    {
                        int[] dependencyPackages = assetPackageDesc.DependencyPackageIDs;
                        for(int i = 0,icnt = dependencyPackages.Length;i<icnt;++i)
                        {
                            if (!_LoadPackageAsync(dependencyPackages[i], true, packageLoadTask, userData, priorityLevel))
                            {
                                TMDebug.LogErrorFormat("Can not load dependency package when load package '{1}'!", assetPackageDesc.PackageName.Name);
                                return false;
                            }
                        }
                    }

                    m_TaskPool.AddTask(packageLoadTask, priorityLevel);
                    return true;
                }
                else
                    TMDebug.LogErrorFormat("Can not load dependency package when load package '{0}'!", assetPackageDesc.PackageName.Name);

                return false;
            }

            private bool _CheckAsset(string assetName,bool loadFromPackage, out AssetPackageDesc? assetPackageDesc, out string assetNameInPackage,out int assetPackageID)
            {
                assetPackageDesc = default(AssetPackageDesc);
                assetNameInPackage = null;
                assetPackageID = INVALID_HANDLE;

                if (string.IsNullOrEmpty(assetName))
                {
#if UNITY_EDITOR
                    TMDebug.LogErrorFormat("Asset name is empty!");
#endif
                    return false;
                }

                if (loadFromPackage)
                {
                    AssetDesc targetAssetDesc;
                    AssetPackageDesc targetAssetPackageDesc;
                    if (m_AssetManager.m_AssetDescTable.TryGetValue(assetName, out targetAssetDesc))
                    {
                        if (0 <= targetAssetDesc.AssetPackageID && targetAssetDesc.AssetPackageID < m_AssetManager.m_AssetPackageDescTable.Count)
                        {
                            if (m_AssetManager.m_AssetPackageDescTable.TryGetValueAt(targetAssetDesc.AssetPackageID, out targetAssetPackageDesc))
                            {
                                assetPackageDesc = targetAssetPackageDesc;
                                assetPackageID = targetAssetDesc.AssetPackageID;

                                if (assetPackageDesc.Value.AssetPackageUsage.HasFlag((int)AssetPackageUsage.LoadAssetWithGUIDName))
                                {
                                    assetNameInPackage = targetAssetDesc.AssetGUIDName;
                                }
                                else
                                {
                                    /*
                                    int assetNameInPackageIndex = assetName.LastIndexOf('/');
                                    if (assetNameInPackageIndex + 1 >= assetName.Length)
                                    {
                                        TMDebug.LogWarningFormat("Invalid asset name [{0}]!", assetName);
                                        return false;
                                    }
                                    assetNameInPackage = assetName.Substring(assetNameInPackageIndex + 1);
                                    */
                                    //assetNameInPackage = Utility.Path.Combine("Assets/Resources", assetName);
                                    assetNameInPackage =assetName;
                                }

                                return true;
                            }
                            else
                                TMDebug.LogWarningFormat("Can not find asset package with ID '{0}' at asset package table!", targetAssetDesc.m_AssetPackageID);
                        }
                    }
                    else
                        TMDebug.LogWarningFormat("Can not find asset with path '{0}' at asset table!", assetName);

                    return false;
                }
                else
                    return _IsAssetExist(assetName);
            }

            private bool _IsAssetExist(string assetPath)
            {
                return true;
            }

            private ITMResourceLoader _CreateResourceLoader(string resLoaderTypeName)
            {
                ITMResourceLoader targetLoader = (ITMResourceLoader)Utility.Assembly.CreateInstance(resLoaderTypeName);
                if (null != targetLoader)
                {
                    return targetLoader;
                }
                else
                    TMDebug.AssertFailed(string.Format("Can not create resource loader with type '{0}'!", resLoaderTypeName));

                return null;
            }

            private AssetByteLoader _CreateAssetByteLoader(string assetByteLoader)
            {
                AssetByteLoader targetLoader = (AssetByteLoader)Utility.Assembly.CreateInstance(assetByteLoader);
                if (null != targetLoader)
                {
                    return targetLoader;
                }
                else
                    TMDebug.AssertFailed(string.Format("Can not create asset byte loader with type '{0}'!", assetByteLoader));

                return null;
            }

            public ResAsyncLoader AllocateResAsyncLoader()
            {
                LinkedListNode<ResAsyncLoader> first = m_ResAsyncLoaderList.First;
                if (null != first)
                {
                    m_ResAsyncLoaderList.RemoveFirst();
                    return first.Value;
                }

                return _CreateResourceLoader(m_ResAsyncLoaderTypeName) as ResAsyncLoader;
            }

            public void RecycleAsyncLoader(ResAsyncLoader resAsyncLoader)
            {
                if (null == resAsyncLoader)
                {
                    TMDebug.LogErrorFormat("Resource loader can not be null!");
                    return;
                }

                m_ResAsyncLoaderList.AddLast(resAsyncLoader);
            }

            public void Shutdown()
            {
                if (null != m_ResSyncLoader)
                    m_ResSyncLoader.Reset();

                LinkedListNode<ResAsyncLoader> cur = m_ResAsyncLoaderList.First;
                while(null != cur)
                {
                    LinkedListNode<ResAsyncLoader> next = cur.Next;
                    cur.Value.Reset();
                    m_ResAsyncLoaderList.Remove(cur);
                    cur = next;
                }
                
                if(null != m_TaskPool)
                    m_TaskPool.Shutdown();
            }

            void _ParseAssetPath(string assetPath, out string mainRes, out string subRes)
            {
                mainRes = assetPath;
                subRes = "";
                try
                {
                    
                    int idxSpliter = assetPath.LastIndexOf(':');
                    if (0 <= idxSpliter && idxSpliter < assetPath.Length)
                    {
                        mainRes = assetPath.Substring(0, idxSpliter);
                        subRes = assetPath.Substring(idxSpliter + 1);
                    }
                    else
                    {
                        mainRes = assetPath;
                        subRes = "";
                    }
                }
                catch(System.Exception e)
                {
                    Debugger.LogWarning("Pares path has failed with exception:{0}", e.Message);
                }
            }

            string _GetAssetPackageNameByPackageID(int packageID)
            {
                if(0<= packageID && packageID < m_AssetManager.m_AssetPackageDescTable.Count)
                {
                    return m_AssetManager.m_AssetPackageDescTable[packageID].PackageName.Name;
                }

                return "";
            }

            void _OnByteAssetLoadSuccess(string path, object asset, int taskGrpID, float duration, object userData)
            {
                if(null == userData)
                {
                    Debugger.LogWarning("Byte asset load callback with a null user data!");
                    return;
                }

                AssetByteLoadCallbackContext loadContext = userData as AssetByteLoadCallbackContext;
                if (null != loadContext)
                {
                    AssetByte asetByte = m_AssetByteLoader.LoadAssetByte(asset);
                    loadContext.AsyncLoadCallbacks.OnAssetLoadSuccess(path, asetByte.AssetBytes, taskGrpID, duration, loadContext.UserData);
                }
                else
                    Debugger.LogWarning("User data type [{0}] is not a correct type [{1}]!", userData.GetType(), typeof(AssetByteLoadCallbackContext));
            }

            void _OnByteAssetLoadFailure(string path, int taskGrpID, AssetLoadErrorCode errorCode, string message, object userData)
            {
                if (null == userData)
                {
                    Debugger.LogWarning("Byte asset load callback with a null user data!");
                    return;
                }

                AssetByteLoadCallbackContext loadContext = userData as AssetByteLoadCallbackContext;
                if (null != loadContext)
                {
                    loadContext.AsyncLoadCallbacks.OnAssetLoadFailure(path, taskGrpID,errorCode, message, loadContext.UserData);
                }
                else
                    Debugger.LogWarning("User data type [{0}] is not a correct type [{1}]!", userData.GetType(), typeof(AssetByteLoadCallbackContext));
            }

            void _OnByteAssetLoadUpdate(string path, int taskGrpID, float progress, object userData)
            {
            }
        }
    }
}