
using System;
using System.IO;

namespace Tenmove.Runtime
{
    internal class RemoteFileDownloader : IDisposable
    {
        public enum DownloadState
        {
            None,
            Downloading,
            Verifying,
            Done,
            Terminated,
        }

        private RemoteDataObtainer m_RemoteDataObtainer = null;
        private IDataVerifier<string> m_MD5Verifier = null;
        private string m_NativeFilePath = null;
        private string m_RemoteFileURL = null;
        private string m_NativeFileMD5 = null;
        private int m_CacheSize = 0;
        private int m_RetryCount = 1;
        private int m_CurRetry = 0;
        private long m_TimeStamp = 0; 
        private long m_RemoteFileSize = 0;
        private long m_NativeFileOffset = -1;
        private Stream m_NativeFile = null;
        private DownloadState m_DownloadState = DownloadState.None;

        private long m_TimeSlice  = 0;
        private float m_Progress = 0;

        private bool m_Disposed = false;

        public DownloadState State
        {
            get { return m_DownloadState; }
        }
        
        public float Progress
        {
            get { return m_Progress; }
        }

        public long TotalBytes
        {
            get { return null == m_RemoteDataObtainer ? 0 : m_RemoteDataObtainer.TotalBytes; }
        }

        public long DownloadBytes
        {
            get { return null == m_RemoteDataObtainer ? 0 : m_RemoteDataObtainer.DownloadBytes ; }
        }

        public uint ReponseBytes
        {
            get { return null == m_RemoteDataObtainer ? 0 : m_RemoteDataObtainer.ReponseBytes; }
        }

        public bool CreateRequest(string fileURL, string savePath,long fileSize, string fileMD5, int cacheSize = 512 * 1024,float timeSlice = 0.01f, int retryCount = 1)
        {
            if (!string.IsNullOrEmpty(savePath))
            {
                if (!string.IsNullOrEmpty(fileURL))
                {
                    m_RemoteFileSize = fileSize;
                    if (timeSlice < 0.0f)
                        timeSlice = 0.0f;
                    m_TimeSlice = Utility.Time.SecondsToTicks(timeSlice);
                    m_CacheSize = cacheSize;
                    m_RetryCount = retryCount;
                    m_NativeFileMD5 = fileMD5;
                    if (_PrepareDownloadRequest(savePath, fileURL, cacheSize,false))
                        return true;
                }
                else
                    Debugger.LogWarning( "Remote URL can not be null!");
            }
            else
                Debugger.LogWarning( "Save path can not be null!");

            m_DownloadState = DownloadState.Terminated;
            return false;
        }

        public bool StepDownload()
        {
            switch (m_DownloadState)
            {
                case DownloadState.None:
                    {
                        return false;
                    }
                case DownloadState.Downloading:
                    {
                        _OnDownloading();
                        return false;
                    }
                case DownloadState.Verifying:
                    {
                        _OnVerifying();
                        return false;
                    }
                case DownloadState.Done:
                    {
                        return true;
                    }
                case DownloadState.Terminated:
                    {
                        Close();
                        return true;
                    }
                default:
                    {
                        Debugger.AssertFailed("Unknown download state [{0}]!",m_DownloadState);
                        return true;
                    }
            }
        }

        public void ResetResponseBytes()
        {
            if (null != m_RemoteDataObtainer)
                m_RemoteDataObtainer.ResetResponseBytes();
        }

        public void Close()
        {
            if (null != m_RemoteDataObtainer)
            {
                m_RemoteDataObtainer.Close();
                m_RemoteDataObtainer.Dispose();
                m_RemoteDataObtainer = null;
            }

            if (null != m_NativeFile)
            {
                m_NativeFile.Close();
                m_NativeFile.Dispose();
                m_NativeFile = null;
            }

            m_MD5Verifier = null;
            m_NativeFilePath = null;
            m_RemoteFileURL = null;
            m_NativeFileMD5 = null;
            m_NativeFileOffset = -1;
            m_CacheSize = 0;
            m_RetryCount = 1;
            m_CurRetry = 0;
            m_DownloadState = DownloadState.None;
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        public  void Dispose()
        {
            _Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        /// <param name="disposing">释放资源标记。</param>
        private void _Dispose(bool disposing)
        {
            if (m_Disposed)
            {
                return;
            }

            if (disposing)
            {
                Close();
            }

            m_Disposed = true;
        }

        protected bool _PrepareDownloadRequest(string validSavePath, string validFileURL, int cacheSize,bool overwrite)
        {
            Debugger.Assert(!string.IsNullOrEmpty(validSavePath) && !string.IsNullOrEmpty(validFileURL),
                "Destinate path and download URI both can not be null or empty!");

            m_TimeStamp = Utility.Time.GetTicksNow();
            if (null == m_RemoteDataObtainer)
                m_RemoteDataObtainer = new RemoteDataObtainer();

            m_Progress = 0;
            m_NativeFile = Utility.File.OpenWrite(validSavePath, overwrite);
            if (null != m_NativeFile)
            {
                m_NativeFilePath = validSavePath;
                m_RemoteFileURL = validFileURL;
                m_NativeFileOffset = m_NativeFile.Length;

                if (m_NativeFileOffset < m_RemoteFileSize)
                {
                    m_RemoteDataObtainer.CreateRequest(validFileURL, m_NativeFileOffset, cacheSize, m_NativeFile);
                    m_DownloadState = DownloadState.Downloading;

                    if(RequestState.Continue == m_RemoteDataObtainer.State)
                        m_CurRetry = 0;
                }
                else
                    _OnFetchRequestFinished();
            }
            else
                Debugger.LogWarning( "Create file or write file with path [{0}] has failed!", validFileURL);

            return true;
        }

        private void _OnDownloading()
        {
            if (null != m_NativeFile && null != m_RemoteDataObtainer)
            {
                m_Progress = m_RemoteDataObtainer.Progress * 0.8f;
                switch (m_RemoteDataObtainer.State)
                {
                    case RequestState.Finish:
                        {
                            m_NativeFile.Flush();
                            _OnFetchRequestFinished();
                        }
                        break;
                    case RequestState.Continue:
                        {
                            m_DownloadState = DownloadState.Downloading;
                            return;
                        }
                    case RequestState.Terminated:
                        {
                            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                                return;

                            if (Utility.Time.GetTicksNow() < m_TimeStamp + Utility.Time.SecondsToTicks(2))
                                return;

                            if (m_CurRetry < m_RetryCount)
                                _OnRetryDownload();
                            else
                            {
                                m_NativeFilePath = null;
                                m_RemoteFileURL = null;

                                m_RemoteDataObtainer.Close();
                                m_RemoteDataObtainer = null;

                                m_NativeFile.Close();
                                m_NativeFile = null;
                                m_NativeFileOffset = 0;

                                m_Progress = 0.0f;
                                m_DownloadState = DownloadState.Terminated;
                            }
                        }
                        break;
                }
            }
            else
                Debugger.LogWarning("Request is invalid,Download can not in process! (Destinate file stream or source url stream is null!)");
        }

        private void _OnVerifying()
        {
            if (null == m_MD5Verifier)
            {
                m_Progress = 0.8f;
                m_MD5Verifier = new MD5Verifier();
                m_NativeFile = Utility.File.OpenRead(m_NativeFilePath);
                m_MD5Verifier.BeginVerify(m_NativeFile, m_CacheSize, Utility.Time.TicksToSeconds(m_TimeSlice));
            }
            else
            {
                m_Progress = 0.8f + m_MD5Verifier.Progress * 0.2f;
                if (m_MD5Verifier.EndVerify())
                {
                    string md5 = m_MD5Verifier.GetVerifySum();
                    Debugger.LogInfo("Download file MD5:{0}!", md5);
                    m_NativeFile.Close();
                    m_NativeFile = null;
                    m_MD5Verifier = null;
                    m_Progress = 1.0f;
                    if (!m_NativeFileMD5.Equals(md5, System.StringComparison.OrdinalIgnoreCase))
                    {/// MD5校验失败
                        m_DownloadState = DownloadState.None;
                        Debugger.LogWarning("MD5 check has failed file is incorrect, retry downloading from remote [URL:{0}]!", m_RemoteFileURL);
                        _OnRetryDownload();
                    }
                    else
                    {
                        Debugger.LogInfo("MD5 check succeed, file from url '{0}' download completed, save to local path '{1}', md5 code:{2}.", m_RemoteFileURL, m_NativeFilePath,md5);
                        m_DownloadState = DownloadState.Done;
                    }
                }
            }
        }
        
        private void _OnRetryDownload()
        {
            Debugger.LogWarning("Retry download with url:{0} Retry count:{1},Max retry count:{2}.",m_RemoteFileURL,m_CurRetry,m_RetryCount);
            ++m_CurRetry;
            if (m_CurRetry < m_RetryCount)
            {
                if (null != m_NativeFile)
                    m_NativeFile.Close();

                if (null != m_RemoteDataObtainer)
                    m_RemoteDataObtainer.Close();
                 
                //Utility.File.Delete(m_NativeFilePath);
                _PrepareDownloadRequest(m_NativeFilePath, m_RemoteFileURL, m_CacheSize, false);
            }
            else
            {
                Debugger.LogWarning("Reached maximum retry count,download has been terminated![Download URL:{0}]", m_RemoteFileURL);
                m_DownloadState = DownloadState.Terminated;
            }
        }

        private void _OnFetchRequestFinished()
        {
            m_Progress = 0.8f;
            if (!string.IsNullOrEmpty(m_NativeFileMD5))
                m_DownloadState = DownloadState.Verifying;
            else
                m_DownloadState = DownloadState.Done;

            m_NativeFile.Close();
            m_NativeFile = null;

            m_RemoteDataObtainer.Close();
            m_RemoteDataObtainer = null;
        }
    }
}