using System;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    internal partial class AssetManager
    {
        private partial class AssetLoader
        {
            private enum LoadState
            {
                None,
                WaitForDependency,
                WaitForTarget,
            }

            private struct AssetNameRecord
            {
                public AssetNameRecord(string name, int hashCode, LoadTaskAgent agent)
                {
                    if (string.IsNullOrEmpty(name))
                        TMDebug.AssertFailed("Name can not be null or empty!");

                    m_AssetName = name;
                    m_AssetHashCode = hashCode;
                    m_LoadingAgent = agent;
                }

                public readonly string m_AssetName;
                public readonly int m_AssetHashCode;
                public readonly LoadTaskAgent m_LoadingAgent;
            }

            private readonly List<AssetNameRecord> m_LoadingAssetRecords = new List<AssetNameRecord>();
            private readonly List<AssetNameRecord> m_LoadingPackageRecords = new List<AssetNameRecord>();

            private void _RegisterLoadingRecord(string recordName, List<AssetNameRecord> loadingRecordsList, LoadTaskAgent agent)
            {
                TMDebug.Assert(!string.IsNullOrEmpty(recordName));
                int newAssetHashCode = recordName.GetHashCode();
#if UNITY_EDITOR
                for (int i = 0, icnt = loadingRecordsList.Count; i < icnt; ++i)
                {
                    if (loadingRecordsList[i].m_AssetHashCode == newAssetHashCode && loadingRecordsList[i].m_AssetName == recordName)
                    {
                        TMDebug.AssertFailed("Asset name is already in loading list!");
                        return;
                    }
                }
#endif
                loadingRecordsList.Add(new AssetNameRecord(recordName, newAssetHashCode, agent));
            }

            private void _UnregisterLoadingRecord(string recordName, List<AssetNameRecord> loadingRecordsList)
            {
                int assetHashCode = recordName.GetHashCode();
                for (int i = 0, icnt = loadingRecordsList.Count; i < icnt; ++i)
                {
                    if (loadingRecordsList[i].m_AssetHashCode == assetHashCode && loadingRecordsList[i].m_AssetName == recordName)
                    {
                        loadingRecordsList.RemoveAt(i);
                        return;
                    }
                }

                TMDebug.LogWarningFormat("Asset with name [{0}] is not in loading list!", recordName);
            }

            private bool _IsRecordInLoading(string recordName, List<AssetNameRecord> loadingRecordsList)
            {
                int assetHashCode = recordName.GetHashCode();
                for (int i = 0, icnt = loadingRecordsList.Count; i < icnt; ++i)
                {
                    if (loadingRecordsList[i].m_AssetHashCode == assetHashCode && loadingRecordsList[i].m_AssetName == recordName)
                    {
                        return true;
                    }
                }

                return false;
            }

            private void _RegisterLoadingAsset(string assetName, LoadTaskAgent agent)
            {
                _RegisterLoadingRecord(assetName, m_LoadingAssetRecords, agent);
            }

            private void _UnregisterLoadingAsset(string assetName)
            {
                _UnregisterLoadingRecord(assetName, m_LoadingAssetRecords);
            }

            private bool _IsAssetInLoading(string assetName)
            {
                return _IsRecordInLoading(assetName, m_LoadingAssetRecords);
            }

            private void _RegisterLoadingPackage(string packageName, LoadTaskAgent agent)
            {
                TMDebug.Assert(!m_AssetPackagePool.CanSpawn(packageName), "Asset package '{0}' has already loaded!", packageName);
                _RegisterLoadingRecord(packageName, m_LoadingPackageRecords, agent);
            }

            private void _UnregisterLoadingPackage(string packageName)
            {
                _UnregisterLoadingRecord(packageName, m_LoadingPackageRecords);
            }
            private bool _IsPackageInLoading(string packageName)
            {
                return _IsRecordInLoading(packageName, m_LoadingPackageRecords);
            }

            /// <summary>
            /// 如果一个Package在异步加载，强制其同步
            /// </summary>
            /// <param name="packageName"></param>
            /// <returns> 不在异步加载队列，或者在但是强制其同步成功，返回true。</returns>
            private bool _CheckPackageInLoading(string packageName)
            {
                int assetHashCode = packageName.GetHashCode();
                for (int i = 0, icnt = m_LoadingPackageRecords.Count; i < icnt; ++i)
                {
                    if (m_LoadingPackageRecords[i].m_AssetHashCode == assetHashCode && m_LoadingPackageRecords[i].m_AssetName == packageName)
                    {
                        return m_LoadingPackageRecords[i].m_LoadingAgent.ForceGetPackageAsyncResult();
                    }
                }

                return true;
            }

            /// <summary>
            /// 如果一个Package在异步加载，强制其同步
            /// </summary>
            /// <param name="packageName"></param>
            /// <returns> 不在异步加载队列，或者在但是强制其同步成功，返回true。</returns>
            private bool _CheckAssetInLoading(string assetName)
            {
                int assetHashCode = assetName.GetHashCode();
                for (int i = 0, icnt = m_LoadingAssetRecords.Count; i < icnt; ++i)
                {
                    if (m_LoadingAssetRecords[i].m_AssetHashCode == assetHashCode && m_LoadingAssetRecords[i].m_AssetName == assetName)
                    {
                        return m_LoadingAssetRecords[i].m_LoadingAgent.ForceGetAssetAsyncResult();
                    }
                }

                return true;
            }

            public bool IsAsyncInLoading
            {
                get { return m_LoadingAssetRecords.Count > 0 || m_LoadingPackageRecords.Count > 0; }
            }

            private class LoadTaskAgent
            {
                private readonly AssetLoader m_AssetLoader;
                private readonly ITMReferencePool<Asset> m_AssetPool;
                private readonly ITMReferencePool<AssetPackage> m_AssetPackagePool;

                private ResAsyncLoader m_ResAsyncLoader;
                private LoadTaskBase m_CurrentTask = null;
                private LoadState m_State = LoadState.None;

                public LoadTaskAgent(ITMReferencePool<Asset> assetPool, ITMReferencePool<AssetPackage> assetPackagePool, AssetLoader assetLoader)
                {
                    if (null == assetPool)
                        TMDebug.AssertFailed("Asset pool is null!");

                    if (null == assetPackagePool)
                        TMDebug.AssertFailed("Asset package pool is null!");

                    if (null == assetLoader)
                        TMDebug.AssertFailed("Asset loader is null!");

                    m_AssetPool = assetPool;
                    m_AssetPackagePool = assetPackagePool;
                    m_AssetLoader = assetLoader;
                    m_ResAsyncLoader = m_AssetLoader.AllocateResAsyncLoader();

                    m_CurrentTask = null;
                    m_State = LoadState.None;
                }

                public LoadTaskBase Task
                {
                    get { return m_CurrentTask; }
                }

                public bool HasSyncRequested()
                {
                    return m_ResAsyncLoader.HasSyncRequested();
                }

                public void Initialize()
                {
                    m_ResAsyncLoader.UpdateResourceEventHandler += _OnAssetLoadAgentUpdate;
                    m_ResAsyncLoader.LoadResourceCompleteEventHandler += _OnAssetLoadAgentLoadAssetComplete;
                    m_ResAsyncLoader.LoadResourceFailedEventHandler += _OnAssetLoadAgentLoadAssetFailed;
                    m_ResAsyncLoader.LoadPackageCompleteEventHandler += _OnAssetLoadAgentLoadPackageComplete;
                }

                public void Shutdown()
                {
                    m_ResAsyncLoader.UpdateResourceEventHandler -= _OnAssetLoadAgentUpdate;
                    m_ResAsyncLoader.LoadResourceCompleteEventHandler -= _OnAssetLoadAgentLoadAssetComplete;
                    m_ResAsyncLoader.LoadResourceFailedEventHandler -= _OnAssetLoadAgentLoadAssetFailed;
                    m_ResAsyncLoader.LoadPackageCompleteEventHandler -= _OnAssetLoadAgentLoadPackageComplete;

                    m_AssetLoader.RecycleAsyncLoader(m_ResAsyncLoader);
                    m_ResAsyncLoader = null;
                }

                public void Start(LoadTaskBase task)
                {
                    if (null == task)
                    {
                        TMDebug.AssertFailed("Task is null!");
                    }

                    m_CurrentTask = task;
                    m_State = LoadState.None;
                    m_CurrentTask.OnStart(this);
                }

                public void SetAgentState(LoadState state)
                {
                    m_State = state;
                }

                public void LoadAssetAsync(AssetPackage package,string assetName,string subResName,Type assetType)
                {
                    if (null != package)
                        m_ResAsyncLoader.LoadAsset(package.Object, assetName, subResName, assetType);
                    else
                        m_ResAsyncLoader.LoadAsset(null, assetName, subResName, assetType);
                }

                public void LoadPackageAsync(string packageFullPath)
                {
                    m_ResAsyncLoader.LoadPackage(packageFullPath);
                }

                public bool ForceGetPackageAsyncResult()
                {
                    return m_ResAsyncLoader.ForceGetPackageAsyncResult();
                }

                public bool ForceGetAssetAsyncResult()
                {
                    return m_ResAsyncLoader.ForceGetAssetAsyncResult();
                }

                public void Reset()
                {
                    m_ResAsyncLoader.Reset();
                    m_CurrentTask = null;
                    m_State = LoadState.None;
                }

                public void Update(float logicDeltaTime, float realDeltaTime)
                {
                    m_ResAsyncLoader.Update();

                    switch (m_State)
                    {
                        case LoadState.None:
                            {/// Do nothing;
                                return;
                            }

                        case LoadState.WaitForDependency:
                            {
                                m_CurrentTask.OnWaitDependency(this);
                                return;
                            }

                        case LoadState.WaitForTarget:
                            {
                                m_CurrentTask.OnWaitTarget(this);
                                return;
                            }

                        default:
                            {
                                TMDebug.AssertFailed(string.Format("Invalid state code:{0}", m_State));
                                return;
                            }
                    }
                }

                private void _OnAssetLoadAgentUpdate(object sender, LoadResourceUpdateEventArgs args)
                {
                    m_CurrentTask.OnLoadUpdate(this, args.Progress);
                }

                private void _OnAssetLoadAgentLoadAssetComplete(object sender, LoadResourceCompleteEventArgs args)
                {
                    AssetPackage assetPackage = m_CurrentTask.GetMainPackage();
                    Asset newAsset = new Asset(m_CurrentTask.AssetName, args.Asset as ITMAssetObject, assetPackage, m_AssetPackagePool);
                    m_AssetPool.Register(newAsset, true);
                    m_CurrentTask.OnTargetLoadReady(this, newAsset);
                }

                private void _OnAssetLoadAgentLoadAssetFailed(object sender, LoadResourceFailedEventArgs args)
                {
                    m_CurrentTask.OnLoadFailure(this, args.ErrorCode, args.Message);
                }

                private void _OnAssetLoadAgentLoadPackageComplete(object sender, LoadPackageCompleteEventArgs args)
                {
                    string packageName = m_CurrentTask.AssetPackageDesc.PackageName.Name;
                    AssetPackage newPackage = new AssetPackage(packageName,
                        args.Package, m_ResAsyncLoader, m_AssetPackagePool);

                    m_AssetPackagePool.Register(newPackage, true);
                    m_CurrentTask.OnTargetLoadReady(this, newPackage);
                }
            }
        }
    }
}