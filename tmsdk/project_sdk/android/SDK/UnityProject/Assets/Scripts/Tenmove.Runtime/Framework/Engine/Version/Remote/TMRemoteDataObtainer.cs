

using System;
using System.IO;
using System.Net;

namespace Tenmove.Runtime
{
    public enum RemoteDataObtainFlag
    {
        None = 0x00,
        FetchFromOrigin = 0x01,
    }

    public enum RequestState
    {
        None,
        Finish,
        Continue,
        Terminated,
    }

    public delegate void OnFetchRequest(object userData,RequestState result,byte[] data,int len);

    public class RemoteDataObtainer : IDisposable
    {
        protected const int CONST_REQUEST_TIME_LIMIT = 1000; /// 1000 ms
        protected const int CONST_DEFAULT_CACHE_SIZE = 256 * 1024; /// 256K

        private bool m_Disposed;
        private byte[] m_DataCache;
        private long m_RequestDataLength;
        private long m_RequestDataOffset;
        private long m_TotalDataLength;
        private string m_RequestUrl;
        private int m_RequestRetryCount;
        private EnumHelper<RemoteDataObtainFlag> m_DataObtainFlag;

        private HttpWebRequest m_HttpRequest;
        private WebResponse m_WebResponse;
        private System.IO.Stream m_ResponseStream;
        private readonly System.Net.Security.RemoteCertificateValidationCallback m_TrustAllCertificatePolicy;
        private uint m_ReponseBytes;

        private RequestState m_RequestState;
        private Stream m_DataStream;

        public RemoteDataObtainer()
        {
            m_Disposed = false;
            m_DataCache = null;
            m_RequestDataLength = 0;
            m_RequestDataOffset = 0;
            m_TotalDataLength = 0;
            m_RequestUrl = string.Empty;
            m_RequestRetryCount = 1;
            m_DataObtainFlag = new EnumHelper<RemoteDataObtainFlag>(RemoteDataObtainFlag.None);

            m_HttpRequest = null;
            m_WebResponse = null;
            m_ResponseStream = null;

            m_TrustAllCertificatePolicy = new System.Net.Security.RemoteCertificateValidationCallback(_CheckValidationResult);
            
            m_DataStream = null;
            m_RequestState = RequestState.None;

            m_ReponseBytes = 0;
        }

        public float Progress
        {
            get
            {
                if (m_TotalDataLength > 0)
                    return m_RequestDataOffset * 1.0f / m_TotalDataLength;
                else
                    return 0.0f;
            }
        }

        public RequestState State
        {
            get { return m_RequestState; }
        }

        public long DataLength
        {
            get { return m_TotalDataLength; }
        }

        public uint ReponseBytes
        {
            get { return m_ReponseBytes; }
        }

        public long TotalBytes
        {
            get { return m_TotalDataLength; }
        }

        public long DownloadBytes
        {
            get { return m_RequestDataOffset; }
        }

        public void CreateRequest(string srcUrl,long offset, int cacheSize ,Stream stream, int retryCount = 1, uint flag = (uint)RemoteDataObtainFlag.None)
        {
            if(null != m_HttpRequest)
            {
                Debugger.LogError("Create request failed,You must close exist request first before you create a new request!");
                return;
            }

            if (cacheSize < 4096)
                cacheSize = CONST_DEFAULT_CACHE_SIZE;

            m_RequestRetryCount = retryCount;
            m_DataObtainFlag = new EnumHelper<RemoteDataObtainFlag>(flag);

            string UrlWithTimeStamp = null;
            if (m_DataObtainFlag.HasFlag(RemoteDataObtainFlag.FetchFromOrigin))
                UrlWithTimeStamp = _AddTimeStampWithURL(srcUrl);
            else
                UrlWithTimeStamp = srcUrl;

            m_ReponseBytes = 0;
            int retryCnt = 0;
            do
            {
                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = m_TrustAllCertificatePolicy;
                    m_HttpRequest = HttpWebRequest.Create(UrlWithTimeStamp) as HttpWebRequest;

                    Debugger.LogInfo("Create http web request with url:{0}", UrlWithTimeStamp);
                    m_HttpRequest.Timeout = CONST_REQUEST_TIME_LIMIT;
                    m_RequestUrl = srcUrl;

                    if (offset < 0)
                        offset = 0;
                    m_HttpRequest.AddRange((int)offset);
                    m_RequestDataOffset = offset;

                    m_HttpRequest.BeginGetResponse(_Thread_OnResponseCallback, this);

                    if (null == m_DataCache || m_DataCache.Length < cacheSize)
                        m_DataCache = new byte[cacheSize];

                    m_DataStream = stream;
                    m_RequestState = RequestState.Continue;
                    return;
                }
                catch (Exception e)
                {
                    ++retryCnt;
                    Debugger.LogError("Get http download request count length has failed! Exception:{0}", e.Message);
                }
            }
            while (retryCnt < m_RequestRetryCount);

            Debugger.LogWarning("Create http web request has failed!(URL:'{0}')", srcUrl);
            m_RequestState = RequestState.Terminated;
        }

        public void ResetResponseBytes()
        {
            m_ReponseBytes = 0;
        }

        public void Close()
        {
            if (null != m_ResponseStream)
            {
                m_ResponseStream.Close();
                m_ResponseStream.Dispose();
                m_ResponseStream = null;
            }

            if (null != m_WebResponse)
            {
                m_WebResponse.Close();
                m_WebResponse = null;
            }

            if (null != m_HttpRequest)
            {
                m_HttpRequest.Abort();
                m_HttpRequest = null;
            }

            m_DataStream = null;
            m_RequestState = RequestState.None;
            m_RequestUrl = null;
            m_RequestDataLength = 0;
            m_RequestRetryCount = 0;
            m_RequestDataOffset = 0;
            m_DataObtainFlag = new EnumHelper<RemoteDataObtainFlag>( RemoteDataObtainFlag.None);
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Dispose()
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


        protected string _AddTimeStampWithURL(string srcUrl)
        {
            string resURL = null;
            uint timeStamp = (uint)Utility.Time.GetTicksNow() / 10000;
            if (srcUrl.Contains("?"))
                resURL = string.Format("{0}&tsp={1}", srcUrl, timeStamp);
            else
                resURL = string.Format("{0}?tsp={1}", srcUrl, timeStamp);

            return resURL;
        }

        static private void _Thread_OnResponseCallback(IAsyncResult result)
        {
            RemoteDataObtainer _this = (RemoteDataObtainer)result.AsyncState;
            try
            {
                HttpWebRequest httpRequest = _this.m_HttpRequest;

                _this.m_WebResponse = (HttpWebResponse)httpRequest.EndGetResponse(result);
                _this.m_RequestDataLength = _this.m_WebResponse.ContentLength;
                _this.m_TotalDataLength = _this.m_RequestDataLength + _this.m_RequestDataOffset;
                Stream responseStream =  _this.m_WebResponse.GetResponseStream();
                _this.m_ResponseStream = responseStream;

                /// 开始读取数据
                responseStream.ReadTimeout = CONST_REQUEST_TIME_LIMIT;
                responseStream.BeginRead(_this.m_DataCache, 0, _this.m_DataCache.Length, _Thread_OnReadStreamCallback, _this);

                return;
            }
            catch (System.Exception e)
            {
                Debugger.LogWarning("Get web request reponse [url:{0}] has exception: {1}", _this.m_RequestUrl, e.Message);
                _this.m_RequestState = RequestState.Terminated;
            }
        }

        //static int test = 1;

        static private void _Thread_OnReadStreamCallback(IAsyncResult result)
        {
            RemoteDataObtainer _this = (RemoteDataObtainer)result.AsyncState;
            try
            {
                Stream responseStream = _this.m_ResponseStream;
                int readBytes = responseStream.EndRead(result);
                if (readBytes > 0)
                {
                    if (null != _this.m_DataStream)
                    {
                        _this.m_DataStream.Write(_this.m_DataCache, 0, readBytes);
                        _this.m_DataStream.Flush();
                    }
                    else
                        Debugger.LogWarning("Can not write data into stream cause data stream is null!");
                    
                    _this.m_RequestDataOffset += readBytes;
                    _this.m_RequestState = RequestState.Continue;
                    _this.m_ReponseBytes += (uint)readBytes;

                    //if (_this.m_RequestDataOffset > 1024 * 1024 * (test))
                    //    throw new Exception("#####");

                    /// 继续读取数据
                    responseStream.BeginRead(_this.m_DataCache, 0, _this.m_DataCache.Length, _Thread_OnReadStreamCallback, _this);
                }
                else/// 流到结尾
                {
                    if (null != _this.m_DataStream)
                        _this.m_DataStream.Flush();
                    
                    _this.m_RequestState = RequestState.Finish;
                }
            }
            catch (System.Exception e)
            {
                Debugger.LogWarning("Read reponse stream has exception: {0}", e);
                _this.m_RequestState = RequestState.Terminated;
            }
        }

        static protected bool _CheckValidationResult(object sender,
            System.Security.Cryptography.X509Certificates.X509Certificate certificate,
            System.Security.Cryptography.X509Certificates.X509Chain chain,
            System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
