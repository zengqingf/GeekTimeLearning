
using System;
using System.Collections;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    public enum NetState
    {
        None,// 没有网络
        Wifi,// Wifi
        Cellular,// 流量网络
        Unknown,
    }

    public struct EngineVersion
    {
        static public readonly EngineVersion Invalid = new EngineVersion(0, 0, 0, 0);

        public EngineVersion(uint serverCode, uint clientCode, uint clientResource, uint clientBuild)
        {
            ServerCode = serverCode;
            ClientCode = clientCode;
            ClientResource = clientResource;
            ClientBuild = clientBuild;
        }

        public uint ServerCode { private set; get; }
        public uint ClientCode { private set; get; }
        public uint ClientResource { private set; get; }
        public uint ClientBuild { private set; get; }
    }

    public interface ITMClientEngine:ITMNative
    {
        string ReadOnlyPath
        {
            get;
        }

        string ReadWritePath
        {
            get;
        }

        bool IsWifiUsed
        {
            get;
        }

        NetState NetState
        {
            get;
        }

        bool IsUnloadingAssets
        {
            get;
        }

        EngineVersion Version
        {
            get;
        }

        string SyncFileServerURL
        {
            get;
        }

        string PackageDataPath
        {
            get;
        }

        string EntrySystem
        {
            get;
        }

        string AssetMD5Sum
        {
            get;
        }

        bool IsObjectPoolDisabled
        {
            get;
        }

        event EventHandler<PurgePoolEventArgs> OnPurgePoolEventHandler;
        event EventHandler<UnloadUnusedAssetEventArgs> OnUnloadUnusedAssetEventHandler;

        void DisableObjectPool(bool disable);
        void ClearUnusedAsset(List<string> keys = null);
        void Collect(int generation);
        void RebootAssetModule();
        IEnumerator FetchFileSyncServerRootConfig();
    }
}