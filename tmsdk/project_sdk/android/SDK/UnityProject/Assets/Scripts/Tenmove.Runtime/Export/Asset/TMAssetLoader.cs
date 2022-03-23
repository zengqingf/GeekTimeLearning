
using System;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    /// <summary>
    /// 资源加载优先级。
    /// </summary>
    public enum AssetLoadPriority
    {
        Normal = 0,  // 普通等级 
        High,        // 高等级

        Max_Num      // 等级数量，外部不允许使用
    }

    public static class AssetLoader
    {
        public static int INVALID_HANDLE = ~0;
        private static Tenmove.Runtime.ITMAssetManager m_AssetManager = null;

        private static Tenmove.Runtime.ITMAssetManager _GetAssetManager()
        {
            if (null == m_AssetManager)
                m_AssetManager = Tenmove.Runtime.ModuleManager.GetModule<Tenmove.Runtime.ITMAssetManager>();

            return m_AssetManager;
        }

        public static string ReadWritePath
        {
            /// 不是写错
            get { return _GetAssetManager().ReadWritePath.ToString(); }
        }

        public static string ReadOnlyPath
        {
            /// 不是写错
            get { return _GetAssetManager().ReadOnlyPath.ToString(); }
        }

        public static bool IsAssetManagerReady()
        {
            Tenmove.Runtime.ITMAssetManager assetManager = _GetAssetManager();
            if (null != assetManager)
                return assetManager.IsAssetLoaderReady;

            return false;
        }

        public static int LoadAsset(string path, System.Type type, object userData, AssetLoadCallbacks<object> callbacks, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
        {
            if (string.IsNullOrEmpty(path))
                return INVALID_HANDLE;

            Tenmove.Runtime.ITMAssetManager assetManager = _GetAssetManager();
            if (null != assetManager)
            {
                if (isAsync)
                    return assetManager.LoadAssetAsync(path, type,callbacks, userData, (int)priority);
                else
                    return assetManager.LoadAssetSync(path, type, callbacks, userData);
            }

            return INVALID_HANDLE;
        }

        public static int LoadAsset(string path, System.Type type, Math.Transform transform, object userData, AssetLoadCallbacks<object> callbacks, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
        {
            if(string.IsNullOrEmpty(path))
                return INVALID_HANDLE;

            Tenmove.Runtime.ITMAssetManager assetManager = _GetAssetManager();
            if (null != assetManager)
            {
                if (isAsync)
                    return assetManager.LoadAssetAsync(path, type, transform, callbacks, userData, (int)priority);
                else
                    return assetManager.LoadAssetSync(path, type, transform, callbacks, userData);
            }

            return INVALID_HANDLE;
        }

        public static int LoadAsset(string path, System.Type type,object userData, OnAssetLoadSuccess<object> onSuccess, OnAssetLoadFailure onFailure, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
        {
            if (string.IsNullOrEmpty(path))
                return INVALID_HANDLE;

            Tenmove.Runtime.ITMAssetManager assetManager = _GetAssetManager();
            if (null != assetManager)
            {
                if (isAsync)
                    return assetManager.LoadAssetAsync(path, type, new AssetLoadCallbacks<object>(onSuccess, onFailure), userData, (int)priority);
                else
                {
                    object asset = assetManager.LoadAsset(path, type, userData, 0);
                    if (null != asset)
                        onSuccess(path, asset, INVALID_HANDLE, 0, userData);
                    else
                        return assetManager.LoadAssetAsync(path, type, new AssetLoadCallbacks<object>(onSuccess, onFailure), userData, (int)priority);
                }
            }

            return INVALID_HANDLE;
        }

        public static int LoadAsset(string path, System.Type type, Math.Transform transform, object userData, OnAssetLoadSuccess<object> onSuccess, OnAssetLoadFailure onFailure, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
        {
            if (string.IsNullOrEmpty(path))
                return INVALID_HANDLE;

            Tenmove.Runtime.ITMAssetManager assetManager = _GetAssetManager();
            if (null != assetManager)
            {
                if (isAsync)
                    return assetManager.LoadAssetAsync(path, type, transform, new AssetLoadCallbacks<object>(onSuccess, onFailure), userData, (int)priority);
                else
                {
                    object asset = assetManager.LoadAsset(path, type, transform, userData, 0);
                    if (null != asset)
                        onSuccess(path, asset, INVALID_HANDLE, 0, userData);
                    else
                        return assetManager.LoadAssetAsync(path, type,transform, new AssetLoadCallbacks<object>(onSuccess, onFailure), userData, (int)priority);
                }
            }

            return INVALID_HANDLE;
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="userData"></param>
        /// <param name="onSuccess"></param>
        /// <returns></returns>
        public static int LoadAsset(string path, System.Type type, object userData, OnAssetLoadSuccess<object> onSuccess, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
        {
            return LoadAsset(path, type, userData, onSuccess, _OnLoadFailureDefault, isAsync, priority);
        }

        public static int LoadAsset(string path, System.Type type, Math.Transform transform, object userData, OnAssetLoadSuccess<object> onSuccess, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
        {
            return LoadAsset(path, type,transform ,userData, onSuccess, _OnLoadFailureDefault, isAsync, priority);
        }

        /// <summary>
        /// 加载资源内容
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="userData"></param>
        /// <param name="onSuccess"></param>
        /// <returns></returns>
        public static int LoadAssetByte(string path, object userData, OnAssetLoadSuccess<byte[]> onSuccess, bool isAsync = true)
        {
            return LoadAssetByte(path, userData, onSuccess, _OnLoadFailureDefault, isAsync);
        }

        public static int LoadAssetByte(string path, object userData, OnAssetLoadSuccess<byte[]> onSuccess, OnAssetLoadFailure onFailure, bool isAsync = true)
        {
            if (string.IsNullOrEmpty(path))
                return INVALID_HANDLE;

            Tenmove.Runtime.ITMAssetManager assetManager = _GetAssetManager();
            if (null != assetManager)
            {
                if (isAsync)
                    return assetManager.LoadAssetByteAsync(path, new AssetLoadCallbacks<byte[]>(onSuccess, onFailure), userData, 0);
                else
                {

                    byte[] asset = assetManager.LoadAssetByte(path, userData, 0);
                    if (null != asset)
                        onSuccess(path, asset, INVALID_HANDLE, 0, userData);
                    else
                        return assetManager.LoadAssetByteAsync(path, new AssetLoadCallbacks<byte[]>(onSuccess, onFailure), userData, 0);
                }
            }

            return INVALID_HANDLE;
        }

        public static int LoadAssetAsGameObject(string path, object userData, AssetLoadCallbacks<object> callbacks, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
        {
            return LoadAsset(path, typeof(UnityEngine.GameObject), userData, callbacks, isAsync, priority);
        }

        public static int LoadAssetAsGameObject(string path, Math.Transform transform, object userData, AssetLoadCallbacks<object> callbacks, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
        {
            return LoadAsset(path, typeof(UnityEngine.GameObject),transform, userData, callbacks, isAsync, priority);
        }

        public static int LoadAssetAsGameObject(string path, object userData, OnAssetLoadSuccess<object> onSuccess, OnAssetLoadFailure onFailure, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
        {
            return LoadAsset(path, typeof(UnityEngine.GameObject), userData, onSuccess, onFailure, isAsync, priority);
        }

        public static int LoadAssetAsGameObject(string path, Math.Transform transform, object userData, OnAssetLoadSuccess<object> onSuccess, OnAssetLoadFailure onFailure, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
        {
            return LoadAsset(path, typeof(UnityEngine.GameObject), transform, userData, onSuccess, onFailure, isAsync, priority);
        }

        public static int LoadAssetAsGameObject(string path, object userData, OnAssetLoadSuccess<object> onSuccess, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
        {
            return LoadAsset(path, typeof(UnityEngine.GameObject), userData, onSuccess, isAsync, priority);
        }

        public static int LoadAssetAsGameObject(string path, Math.Transform transform, object userData, OnAssetLoadSuccess<object> onSuccess, bool isAsync = true, AssetLoadPriority priority = AssetLoadPriority.Normal)
        {
            return LoadAsset(path, typeof(UnityEngine.GameObject), transform, userData, onSuccess, isAsync, priority);
        }

        /// <summary>
        /// 预加载一个资源，并设置其lable，然后锁定资源（因为预加载资源外部没有引用，所以必须锁定，否则预加载就没有意义）
        /// 对于GameObject，不会实例化。
        /// </summary>
        public static bool PreloadAssetAndLock(string path, string lableName, System.Type assetType, AssetLockPhase lockPhase)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            Tenmove.Runtime.ITMAssetManager assetManager = _GetAssetManager();
            if (null != assetManager)
            {
                return assetManager.PreloadAssetAndLock(path, lableName, assetType, lockPhase, 0);
            }

            return false;
        }

        private static void _OnLoadFailureDefault(string path, int taskID, AssetLoadErrorCode errorCode, string message, object userData)
        {
            Tenmove.Runtime.TMDebug.LogWarningFormat("Load asset with path '{0}' has failed!(Task ID:{1},Error msg:{2})", path, taskID, message);
        }

        public static void PurgeUnusedAsset()
        {
            ITMAssetManager assetManager = _GetAssetManager();
            if (null != assetManager)
            {
                assetManager.BeginClearUnusedAssets(false);
                assetManager.EndClearUnusedAssets();
            }

            return;
        }
    }
}
