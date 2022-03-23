using System;

namespace Tenmove.Runtime
{
    public sealed class LoadResourceUpdateEventArgs : BaseEventArgs
    {
        public LoadResourceUpdateEventArgs(ResourceLoadMode mode, float progress)
        {
            Mode = mode;
            Progress = progress;
        }

        public ResourceLoadMode Mode
        {
            get;
            private set;
        }

        public float Progress
        {
            get;
            private set;
        }
    }

    public sealed class LoadResourceCompleteEventArgs : BaseEventArgs
    {
        public LoadResourceCompleteEventArgs(object asset,bool sharedAsset, bool forceSynced)
        {
            Asset = asset;
            SharedAsset = sharedAsset;
            ForceSynced = forceSynced;
        }

        public object Asset
        {
            get;
            private set;
        }

        public bool SharedAsset
        {
            get;
            private set;
        }

        public bool ForceSynced
        {
            get;
            private set;
        }
    }

    public sealed class LoadResourceFailedEventArgs : BaseEventArgs
    {
        public LoadResourceFailedEventArgs(AssetLoadErrorCode errorCode, string msg)
        {
            ErrorCode = errorCode;
            Message = msg;
        }

        public AssetLoadErrorCode ErrorCode
        {
            get;
            private set;
        }

        public string Message
        {
            get;
            private set;
        }
    }

    public sealed class LoadPackageCompleteEventArgs : BaseEventArgs
    {
        public LoadPackageCompleteEventArgs(object package, bool forceSynced)
        {
            Package = package;
            HasSyncRequest = forceSynced;
        }

        public object Package
        {
            get;
            private set;
        }

        public bool HasSyncRequest
        {
            get;
            private set;
        }
    }

    public delegate void ResAsyncLoadCallback(string fileURI, byte[] bytes, float duration, string errorMessage);

    public abstract class ResAsyncLoader : ITMResourceLoader
    {
        public abstract event EventHandler<LoadResourceUpdateEventArgs> UpdateResourceEventHandler;
        public abstract event EventHandler<LoadResourceCompleteEventArgs> LoadResourceCompleteEventHandler;
        public abstract event EventHandler<LoadResourceFailedEventArgs> LoadResourceFailedEventHandler;
        public abstract event EventHandler<LoadPackageCompleteEventArgs> LoadPackageCompleteEventHandler;

        abstract public void LoadPackage(string fullpath);
        abstract public bool ForceGetPackageAsyncResult();
        abstract public bool ForceGetAssetAsyncResult();
        abstract public void LoadAsset(object resource, string assetName,string subResName,Type assetType);
        abstract public void LoadFile(string filepath,bool readWritePath, ResAsyncLoadCallback callback);
        abstract public void UnloadPackage(object package);
        abstract public void Reset();
        abstract public void Update();
        abstract public bool HasSyncRequested();
    }
}

