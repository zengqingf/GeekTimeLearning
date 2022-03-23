

namespace Tenmove.Runtime
{
    public delegate void OnFileLoadSuccess(string path, byte[] data, int taskGrpID, float duration, object userData);
    public delegate void OnFileLoadFailure(string path, int taskGrpID, string message, object userData);
    public delegate void OnFileLoadUpdate(string path, int taskGrpID, float progress, object userData);

    public class FileLoadCallbacks
    {
        readonly OnFileLoadSuccess m_OnFileLoadSuccess;
        readonly OnFileLoadFailure m_OnFileLoadFailure;
        readonly OnFileLoadUpdate  m_OnFileLoadUpdate;

        public FileLoadCallbacks(OnFileLoadSuccess onSuccess, OnFileLoadFailure onFailure)
            : this( onSuccess, onFailure, null)
        {

        }

        public FileLoadCallbacks(OnFileLoadSuccess onSuccess, OnFileLoadFailure onFailure, OnFileLoadUpdate onUpdate)
        {
            TMDebug.Assert(null != onSuccess, "On success callback can not be null!");
            TMDebug.Assert(null != onFailure, "On failure callback can not be null!");

            m_OnFileLoadSuccess = onSuccess;
            m_OnFileLoadFailure = onFailure;
            m_OnFileLoadUpdate = onUpdate;
        }

        public OnFileLoadSuccess OnFileLoadSuccess
        {
            get { return m_OnFileLoadSuccess; }
        }

        public OnFileLoadFailure OnFileLoadFailure
        {
            get { return m_OnFileLoadFailure; }
        }

        public OnFileLoadUpdate OnFileLoadUpdate
        {
            get { return m_OnFileLoadUpdate; }
        }
    }

}