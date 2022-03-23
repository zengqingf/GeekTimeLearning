

using System.Collections.Generic;

namespace Tenmove.Runtime
{
    internal partial class FileSyncManager : BaseModule,ITMFileSyncManager
    {
        private readonly TaskPool m_SyncTaskPool;
        
        private readonly Stack<RemoteFileDownloader> m_FreeDownloaderPool;
        private readonly List<ITMFileSyncFolder> m_SyncFolderList;

        private int m_CurDownloaderNum;
        private int m_MaxDownloaderCount;
        private bool m_IsPause;

        /// <summary>
        /// 这是一个Wrapper
        /// </summary>
        interface ITMFileDownloader
        {
            bool IsSuccess
            {
                get;
            }

            float Progress
            {
                get;
            }

            uint ReponseBytes
            {
                get;
            }

            long DownloadBytes
            {
                get;
            }

            long TotalBytes
            {
                get;
            }

            bool CreateDownloadRequest(string srcUrl, string savePath, long fileSize, string fileMD5, int cacheSize,float timeSlice, int retryCount);
            bool StepDownload();
            void ResetResponseBytes();
        }

        private class FileDownloaderWrapper : ITMFileDownloader
        {
            private readonly FileSyncManager m_FileSyncManager;
            private RemoteFileDownloader m_FileDownloader;
            private bool m_IsSuccess;

            public FileDownloaderWrapper(FileSyncManager fileSyncManager)
            {
                Debugger.Assert(null != fileSyncManager, "File sync manager can not be null!");
                m_FileSyncManager = fileSyncManager;
                m_FileDownloader = null;
                m_IsSuccess = false;
            }

            public bool IsSuccess
            {
                get { return m_IsSuccess; }
            }

            public float Progress
            {
                get { return null != m_FileDownloader ? m_FileDownloader.Progress : 0.0f; }
            }

            public uint ReponseBytes
            {
                get { return null != m_FileDownloader ? m_FileDownloader.ReponseBytes : 0; }
            }

            public long TotalBytes
            {
                get { return null != m_FileDownloader ? m_FileDownloader.TotalBytes : 0; }
            }

            public long DownloadBytes
            {
                get { return null != m_FileDownloader ? m_FileDownloader.DownloadBytes : 0; }
            }

            public void Fill(RemoteFileDownloader downloader)
            {
                Debugger.Assert(null != downloader, "Remote file downloader can not be null!");
                m_FileDownloader = downloader;
            }

            public bool CreateDownloadRequest(string srcUrl, string savePath,long fileSize, string fileMD5, int cacheSize,float timeSlice, int retryCount)
            {
                if (null != m_FileDownloader)
                {
                    bool res = m_FileDownloader.CreateRequest(srcUrl, savePath,fileSize, fileMD5, cacheSize,timeSlice, retryCount);
                    if (res)
                        m_IsSuccess = false;
                    return res;
                }
                return false;
            }

            public bool StepDownload()
            {
                if (null != m_FileDownloader)
                {
                    bool res = m_FileDownloader.StepDownload();
                    if (res)
                    {
                        m_IsSuccess = RemoteFileDownloader.DownloadState.Done == m_FileDownloader.State;
                        m_FileSyncManager._RecycleRemoteFileDownloader(m_FileDownloader);
                        m_FileDownloader = null;
                    }

                    return res;
                }

                return true;
            }

            public void ResetResponseBytes()
            {
                if (null != m_FileDownloader)
                    m_FileDownloader.ResetResponseBytes();
            }
        }

        public FileSyncManager()
        {
            m_SyncTaskPool = new TaskPool();
            m_FreeDownloaderPool = new Stack<RemoteFileDownloader>();
            m_SyncFolderList = new List<ITMFileSyncFolder>();
            m_IsPause = false;
        }

        public override int Priority
        {
            get { return 0; }
        }

        public int MaxDownloaderCount
        {
            get { return m_MaxDownloaderCount; }
        }

        public int FreeAgentCount
        {
            get { return m_SyncTaskPool.FreeAgentCount; }
        }

        public int WaitingTaskCount
        {
            get { return m_SyncTaskPool.WaitingTaskCount; }
        }

        public bool IsPaused
        {
            get { return m_IsPause; }
        }

        public List<ITMFileSyncFolder> AllSyncFolder
        {
            get { return m_SyncFolderList; }
        }

        private bool _IsWhiteNameUser
        {
            get { return true; }
        }

        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (m_IsPause)
                return;

            m_SyncTaskPool.Update(elapseSeconds, realElapseSeconds);
        }

        public void SetDownloaderCount(int downloaderCount)
        {
            if (downloaderCount < 1)
            {
                Debugger.LogWarning("Down loader count can not less than 1, force set to default value 5!");
                downloaderCount = 5;
            }

            m_MaxDownloaderCount = downloaderCount;
        }

        public ITMFileSyncFolder CreateSyncFolder(string folderName,string syncFolderURL, string nativeFolderRoot, string packageDataPath , long currentProgram)
        {
            string fullPath = Utility.Path.Combine(nativeFolderRoot, folderName);
            int fullPathHash = fullPath.GetHashCode();

            ITMFileSyncFolder folder = null;
            for(int i = 0,icnt = m_SyncFolderList.Count;i<icnt;++i)
            {
                folder = m_SyncFolderList[i];
                if (folder.NativeFolderFullPathHash == fullPathHash && folder.NativeFolderFullPath == fullPath)
                {
                    folder.StartSynchronize();
                    return folder;
                }
            }

            folder = new SyncFolder(this, folderName, syncFolderURL, nativeFolderRoot, packageDataPath,currentProgram, 6);
            m_SyncFolderList.Add(folder);
            folder.StartSynchronize();

            return folder;
        }

        public void SetPause(bool isPause)
        {
            m_IsPause = isPause;
        }

        public override void Shutdown()
        {
        }

        private ITMFileDownloader _CreateFileDownloader()
        {
            RemoteFileDownloader newDownloader = _AcquireRemoteFileDownloader();
            if (null != newDownloader)
            {
                FileDownloaderWrapper wrapper = new FileDownloaderWrapper(this);
                wrapper.Fill(newDownloader);
                return wrapper;
            }

            return null;
        }

        private bool _CanCreateFileDownloader()
        {
            return m_FreeDownloaderPool.Count > 0;
        }

        private RemoteFileDownloader _AcquireRemoteFileDownloader()
        {
            int count = m_MaxDownloaderCount - m_CurDownloaderNum;
            if (count > 0)
            {
                while (count-- > 0)
                    m_FreeDownloaderPool.Push(new RemoteFileDownloader());

                m_CurDownloaderNum = m_MaxDownloaderCount;
            }
            
            if (m_FreeDownloaderPool.Count > 0)
                return m_FreeDownloaderPool.Pop();

            return null;
        }

        private void _RecycleRemoteFileDownloader(RemoteFileDownloader fileDownloader)
        {
            if (m_CurDownloaderNum <= m_MaxDownloaderCount)
                m_FreeDownloaderPool.Push(fileDownloader);
            else
                --m_CurDownloaderNum;
        }

        private void _TerminateSyncFolderTask(string folderName)
        {
            m_SyncTaskPool.TerminateTaskByTag(folderName);
        }
    }
}