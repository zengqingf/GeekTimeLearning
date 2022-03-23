/*
 * 客户端基础功能的全局访问类，也是客户端的根模块，提供全局功能，以后全局的功能就放在这里：
 * 
 * 1.GC
 * 2.资源卸载
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using Tenmove.Runtime.EmbedUI;

namespace Tenmove.Runtime
{
    public class ClientRoot : ClientModule.Singleton<ClientRoot>
    {
        private ITMClientEngine m_ClientEngine;
        private ITMUIManager m_UIManager;
        private ITMFileSyncManager m_FileSyncManager;

        public ClientRoot(ClientModule clientModule)
           : base(clientModule)
        {
            m_ClientEngine = null;
            m_UIManager = null;
            m_FileSyncManager = null;
        }

        public override int Priority
        {
            get { return 10000; }
        }

        public string ReadOnlyPath
        {
            get { return m_ClientEngine.ReadOnlyPath; }
        }

        public string ReadWritePath
        {
            get { return m_ClientEngine.ReadWritePath; }
        }

        public bool IsWifiUsed
        {
            get { return m_ClientEngine.IsWifiUsed; }
        }

        public NetState NetState
        {
            get { return m_ClientEngine.NetState; }
        }

        public ITMUIManager EmbedUI
        {
            get { return m_UIManager; }
        }

        public EngineVersion Version
        {
            get { return m_ClientEngine.Version; }
        }

        public bool IsSyncFolderPaused
        {
            get { return null != m_FileSyncManager ? m_FileSyncManager.IsPaused : false; }
        }

        public string EntrySystem
        {
            get { return m_ClientEngine.EntrySystem; }
        }

        public string AssetMD5Sum
        {
            get { return m_ClientEngine.AssetMD5Sum; }
        }

        public bool IsObjectPoolDisabled
        {
            get { return m_ClientEngine.IsObjectPoolDisabled; }
        }

        public ITMFileSyncFolder CreateSyncFolder(string assetName, string nativeFolderRoot)
        {
            if (null != m_FileSyncManager && null != m_ClientEngine)
            {
                if (!string.IsNullOrEmpty(m_ClientEngine.SyncFileServerURL))
                    return m_FileSyncManager.CreateSyncFolder(assetName, m_ClientEngine.SyncFileServerURL, nativeFolderRoot, m_ClientEngine.PackageDataPath, Version.ClientCode);
                else
                    Debugger.LogWarning("File synchronize server url is null! Create synchronize folder has failed!");
            }

            return null;
        }

        public void PauseSyncFolder(bool isPause)
        {
            if (null != m_FileSyncManager)
                m_FileSyncManager.SetPause(isPause);
        }

        public void ClearUnusedAsset(List<string> keys = null)
        {
            m_ClientEngine.ClearUnusedAsset(keys);
        }

        public void Collect(int generation)
        {
            m_ClientEngine.Collect(generation);
        }

        public void RebootAssetModule()
        {
            m_ClientEngine.RebootAssetModule();
        }

        public bool IsUnLoadingAssets()
        {
            return m_ClientEngine.IsUnloadingAssets;
        }

        public byte[] ConvertMD5Value(string md5String)
        {
            byte[] res = new byte[16];
            if (32 == md5String.Length)
            {
                for (int i = 0, icnt = res.Length; i < icnt; ++i)
                {
                    int curValue = Convert.ToInt32(md5String.Substring(i*2,2),16);
                    res[i] = (byte)curValue;
                }
            }

            return res;
        }

        public void DisableObjectPool(bool disable)
        {
            if (null != m_ClientEngine)
                m_ClientEngine.DisableObjectPool(disable);
        }

        protected sealed override IEnumerator _OnInit(ITMProgressSegment progress)
        {
            m_ClientEngine = _CreateNative<ITMClientEngine>("Base/TMEngine", "Tenmove.Engine");

            m_FileSyncManager = ModuleManager.GetModule<ITMFileSyncManager>();
            m_FileSyncManager.SetDownloaderCount(5);

            m_UIManager = ModuleManager.GetModule<ITMUIManager>();
            yield return m_ClientEngine.FetchFileSyncServerRootConfig();
        }

        protected sealed override IEnumerator _OnDeinit()
        {
            yield return null;
        }
    }
}