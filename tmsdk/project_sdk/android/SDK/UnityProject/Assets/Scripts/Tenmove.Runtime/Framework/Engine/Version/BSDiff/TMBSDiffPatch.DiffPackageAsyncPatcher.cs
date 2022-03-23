

using System.IO;

namespace Tenmove.Runtime
{
    public partial class BSDiffPatch 
    {
        private class DiffPackageAsyncPatcher
        {
            public enum PatchState
            {
                Patching,
                Verifying,
                Finish,
                Terminated,
            }

            private Stream m_OldFileStream = null;
            private Stream m_NewFileStream = null;
            private MemoryStream m_DiffDataStream = null;

            private MemoryStream m_CtrlBlockStream = null;
            private MemoryStream m_DiffBlockStream = null;
            private MemoryStream m_ExtraBlockStream = null;

            private MemoryStream m_CtrlBlockUnzipStream = null;
            private BinaryReader m_CtrlBlockUnzipReader = null;

            private GZip.GZipStream m_DiffBlockUnzipStream = null;
            private GZip.GZipStream m_ExtraBlockUnzipStream = null;

            private long m_OldFileSize = 0;
            private long m_NewFileSize = 0;

            private long m_CtrlDataBlockLen = 0;
            private long m_DiffDataBlockLen = 0;

            private int m_NewPosition = 0;
            private int m_OldPosition = 0;
            private int[] m_CtrlBlock = null;
            private long m_TimeSlice = 0;

            private string m_PatchFilePath = null;
            private string m_PatchFileMD5 = null;
            private IDataVerifier<string> m_MD5Verifier = null;
            private Stream m_VerifyingStream = null;

            private PatchState m_PatchState = PatchState.Terminated;
            public PatchResult Result
            {
                get 
                {
                    switch(m_PatchState)
                    {
                        case PatchState.Patching: return PatchResult.None;
                        case PatchState.Verifying: return PatchResult.None;
                        case PatchState.Terminated: return PatchResult.Crash;
                        case PatchState.Finish: return PatchResult.OK;
                        default: throw new TMEngineException(string.Format("Unknown patch state:{0}",m_PatchState));
                    }
                }
            }

            public bool BeginPatch(string oldfilepath, string newfilepath, byte[] diffbuf,string patchFileMD5, float timeSlice)
            {
                if(string.IsNullOrEmpty(oldfilepath))
                {
                    Debugger.LogWarning("Old file path can not be null or empty!");
                    return false;
                }

                if (string.IsNullOrEmpty(newfilepath))
                {
                    Debugger.LogWarning("New file path can not be null or empty!");
                    return false;
                }

                if (null == diffbuf)
                {
                    Debugger.LogWarning("Diff buffer can not be null!");
                    return false;
                }

                m_PatchFilePath = newfilepath;
                m_OldFileStream = Utility.File.OpenRead(oldfilepath);
                m_NewFileStream = Utility.File.OpenWrite(newfilepath, true);
                m_DiffDataStream = Utility.Memory.OpenStream(diffbuf) as MemoryStream;
                m_PatchFileMD5 = patchFileMD5;
                m_OldFileSize = m_OldFileStream.Length;

                byte[] headDataCache = new byte[8];
                m_DiffDataStream.Read(headDataCache, 0, headDataCache.Length);
                if (_IsBSDiffFile(headDataCache))
                {
                    m_DiffDataStream.Read(headDataCache, 0, headDataCache.Length);
                    m_CtrlDataBlockLen = _DecodeLongType(headDataCache);
                    m_DiffDataStream.Read(headDataCache, 0, headDataCache.Length);
                    m_DiffDataBlockLen = _DecodeLongType(headDataCache);
                    m_DiffDataStream.Read(headDataCache, 0, headDataCache.Length);
                    m_NewFileSize = _DecodeLongType(headDataCache);
                    
                    long diffFileHeaderLen = m_DiffDataStream.Position;
                    int streamOffset = (int)diffFileHeaderLen;
                    int streamLength = (int)m_CtrlDataBlockLen;
                    m_CtrlBlockStream = Utility.Memory.OpenStream(diffbuf, streamOffset, streamLength) as MemoryStream;

                    m_CtrlBlockUnzipStream = Utility.Memory.OpenStream(1024 * 16) as MemoryStream;
                    using (GZip.GZipStream ctrlBlockUnzipStream = new GZip.GZipStream(m_CtrlBlockStream, GZip.CompressionMode.Decompress))
                    {
                        byte[] cache = new byte[1024 * 16];
                        int n;
                        while ((n = ctrlBlockUnzipStream.Read(cache, 0, cache.Length)) != 0)
                        {
                            m_CtrlBlockUnzipStream.Write(cache, 0, n);
                        }
                    }
                    m_CtrlBlockUnzipStream.Flush();

                    m_CtrlBlockUnzipStream.Seek(0, SeekOrigin.Begin);
                    m_CtrlBlockUnzipReader = new BinaryReader(m_CtrlBlockUnzipStream);
                    streamOffset = (int)(diffFileHeaderLen + m_CtrlDataBlockLen);
                    streamLength = (int)m_DiffDataBlockLen;
                    m_DiffBlockStream = Utility.Memory.OpenStream(diffbuf, streamOffset, streamLength) as MemoryStream;
                    m_DiffBlockUnzipStream = new GZip.GZipStream(m_DiffBlockStream, GZip.CompressionMode.Decompress);

                    streamOffset = (int)(diffFileHeaderLen + m_CtrlDataBlockLen + m_DiffDataBlockLen);
                    streamLength = (int)(diffbuf.Length - streamOffset);
                    m_ExtraBlockStream = Utility.Memory.OpenStream(diffbuf, streamOffset, streamLength) as MemoryStream;
                    m_ExtraBlockUnzipStream = new GZip.GZipStream(m_ExtraBlockStream, GZip.CompressionMode.Decompress);

                    m_NewPosition = 0;
                    m_OldPosition = 0;
                    if (null == m_CtrlBlock)
                        m_CtrlBlock = new int[3];

                    m_TimeSlice = Utility.Time.SecondsToTicks(timeSlice);
                    m_PatchState = PatchState.Patching;
                    
                    return true;
                }
                else
                    Debugger.LogError("Corrupt by wrong patch file.");

                return false;
            }

            public bool EndPatch()
            {
                switch(m_PatchState)
                {
                    case PatchState.Patching: _OnPatching(); return false;
                    case PatchState.Verifying: _OnVerifying(); return false;
                    case PatchState.Finish: return true;
                    case PatchState.Terminated: return true;
                    default: throw new TMEngineException("Invalid asynchronize patch state!");
                }
            }

            private void _OnPatching()
            {
                long curTimeSlice = m_TimeSlice;
                /// int nbytes;
                while (m_NewPosition < m_NewFileSize && curTimeSlice > 0)
                {
                    long ticksNow = Utility.Time.GetTicksNow();

                    for (int i = 0; i <= 2; i++)
                    {
                        m_CtrlBlock[i] = m_CtrlBlockUnzipReader.ReadInt32();
                    }

                    if (m_NewPosition + m_CtrlBlock[0] > m_NewFileSize)
                    {
                        _Clear();
                        m_PatchState = PatchState.Terminated;
                        return;
                    }

                    /// Read m_CtrlBlock[0] bytes from diffBlock stream
                    byte[] buffer = new byte[m_CtrlBlock[0]];
                    if (!_ReadBufFromFile(m_DiffBlockUnzipStream, buffer, 0, m_CtrlBlock[0]))
                    {
                        _Clear();
                        m_PatchState = PatchState.Terminated;
                        return;
                    }

                    byte[] oldBuffer = new byte[m_CtrlBlock[0]];
                    if (m_OldFileStream.Read(oldBuffer, 0, (int)m_CtrlBlock[0]) < m_CtrlBlock[0])
                    {
                        _Clear();
                        m_PatchState = PatchState.Terminated;
                        return;
                    }

                    for (int i = 0; i < m_CtrlBlock[0]; i++)
                    {
                        if ((m_OldPosition + i >= 0) && (m_OldPosition + i < m_OldFileSize))
                        {
                            buffer[i] += oldBuffer[i];
                        }
                    }

                    m_NewFileStream.Write(buffer, 0, buffer.Length);

                    m_NewPosition += m_CtrlBlock[0];
                    m_OldPosition += m_CtrlBlock[0];

                    if (m_NewPosition + m_CtrlBlock[1] > m_NewFileSize)
                    {
                        _Clear();
                        m_PatchState = PatchState.Terminated;
                        return;
                    }

                    buffer = new byte[m_CtrlBlock[1]];
                    if (!_ReadBufFromFile(m_ExtraBlockUnzipStream, buffer, 0, m_CtrlBlock[1]))
                    {
                        _Clear();
                        m_PatchState = PatchState.Terminated;
                        return;
                    }
                    m_NewFileStream.Write(buffer, 0, buffer.Length);
                    m_NewFileStream.Flush();

                    m_NewPosition += m_CtrlBlock[1];
                    m_OldPosition += m_CtrlBlock[2];
                    m_OldFileStream.Seek(m_OldPosition, SeekOrigin.Begin);

                    curTimeSlice -= Utility.Time.GetTicksNow() - ticksNow;
                }

                if (m_NewPosition < m_NewFileSize)
                    m_PatchState = PatchState.Patching;
                else
                {
                    _ClearPatch();
                    m_PatchState = PatchState.Verifying;
                }
            }

            private void _OnVerifying()
            {
                if (null == m_MD5Verifier)
                {
                    m_MD5Verifier = new MD5Verifier();
                    m_VerifyingStream = Utility.File.OpenRead(m_PatchFilePath);
                    if (!m_MD5Verifier.BeginVerify(m_VerifyingStream, 16 * 1024, Utility.Time.TicksToSeconds(m_TimeSlice * 2)))
                        m_PatchState = PatchState.Terminated;
                }
                else
                {
                    if (m_MD5Verifier.EndVerify())
                    {
                        string md5 = m_MD5Verifier.GetVerifySum();
                        m_VerifyingStream.Close();
                        m_VerifyingStream = null;
                        m_MD5Verifier = null;
                        if (!m_PatchFileMD5.Equals(md5, System.StringComparison.OrdinalIgnoreCase))
                        {/// MD5校验失败
                            Debugger.LogWarning("MD5 check has failed file is incorrect[File:'{0}']!", m_PatchFilePath);
                            m_PatchState = PatchState.Terminated;
                        }
                        else
                        {
                            //Debugger.LogInfo("MD5 check succeed, patch file path '{0}', md5 code:{1}.", m_PatchFilePath, md5);
                            _Clear();
                            m_PatchState = PatchState.Finish;
                        }
                    }
                }
            }

            private void _Clear()
            {
                _ClearPatch();
                _ClearVerify();
            }

            private void _ClearPatch()
            {
                if (null != m_OldFileStream)
                {
                    m_OldFileStream.Close();
                    m_OldFileStream = null;
                }

                if (null != m_NewFileStream)
                {
                    m_NewFileStream.Close();
                    m_NewFileStream = null;
                }

                if (null != m_DiffDataStream)
                {
                    m_DiffDataStream.Close();
                    m_DiffDataStream = null;
                }


                if (null != m_CtrlBlockStream)
                {
                    m_CtrlBlockStream.Close();
                    m_CtrlBlockStream = null;
                }


                if (null != m_DiffBlockStream)
                {
                    m_DiffBlockStream.Close();
                    m_DiffBlockStream = null;
                }


                if (null != m_ExtraBlockStream)
                {
                    m_ExtraBlockStream.Close();
                    m_ExtraBlockStream = null;
                }

                m_OldFileSize = 0;
                m_NewFileSize = 0;

                m_CtrlDataBlockLen = 0;
                m_DiffDataBlockLen = 0;

                m_NewPosition = 0;
                m_OldPosition = 0;
                m_CtrlBlock = null;
            }

            private void _ClearVerify()
            {
                if (null != m_VerifyingStream)
                {
                    m_VerifyingStream.Dispose();
                    m_VerifyingStream = null;
                }
                m_MD5Verifier = null;
            }
        }
    }
}