

using System.IO;
using System.IO.Compression;

namespace Tenmove.Runtime
{
    public partial class BSDiffPatch : ITMDiffBuilder,ITMDiffPatcher
    {
        private const int CONST_DEFAULT_CACHE_SIZE = 256 * 1024;/// 256KB

        /// <summary>
        /// TMDIFF01
        /// </summary>
        private static readonly byte[] CONST_DIFF_FILE_HEAD_TAG = new byte[] { 0x54, 0x4D, 0x44, 0x49, 0x46, 0x46, 0x30, 0x31 };


        private Stream m_DiffFileStream;
        private DiffPackageAsyncPatcher m_DiffPackageAsyncPatcher;

        public PatchResult Result
        {
            get { return m_DiffPackageAsyncPatcher.Result; }
        }

        public BSDiffPatch()
        {
            m_DiffFileStream = null;
            m_DiffPackageAsyncPatcher = null;
        }

        public bool GenerateDiffFile(string srcFilePath, string dstFilePath, string diffFilePath)
        {
            using (Stream srcFile = Utility.File.OpenRead(srcFilePath))
            {
                if (null != srcFile)
                {
                    byte[] srcBuf = new byte[srcFile.Length];
                    using (Stream dstFile = Utility.File.OpenRead(dstFilePath))
                    {
                        if (null != dstFile)
                        {
                            byte[] dstBuf = new byte[dstFile.Length];
                            using (Stream diffFile = Utility.File.OpenWrite(diffFilePath, true))
                            {
                                if (null != diffFile)
                                {
                                    srcFile.Read(srcBuf, 0, srcBuf.Length);
                                    dstFile.Read(dstBuf, 0, dstBuf.Length);

                                    DiffPackageBuilder diffPackageBuilder = new DiffPackageBuilder();
                                    byte[] diffBuf = diffPackageBuilder.Build(srcBuf, dstBuf, null);

                                    diffFile.Write(diffBuf, 0, diffBuf.Length);
                                    diffFile.Flush();
                                    diffFile.Close();

                                    return true;
                                }
                            }

                            dstFile.Close();
                        }
                        else
                        {
                        }
                    }

                    srcFile.Close();
                }
                else
                {
                }
            }

            return false;
        }

        public bool RebuildFileByDiff(string srcFilePath, string diffFilePath, string dstFilePath, string dstFileMD5)
        {
            using (Stream diffFile = Utility.File.OpenRead(diffFilePath))
            {
                if (null != diffFile)
                {
                    byte[] diffBuf = new byte[diffFile.Length];
                    diffFile.Read(diffBuf, 0, diffBuf.Length);

                    DiffPackagePatcher diffPackagePatcher = new DiffPackagePatcher();
                    diffPackagePatcher.Patch(srcFilePath, dstFilePath, diffBuf, 0);

                    diffFile.Close();
                }
            }

            return false;
        }
        public bool BeginRebuildAsync(string srcFilePath, string diffFilePath, string dstFilePath, string dstFileMD5,float timeSlice)
        {
            if (null != m_DiffFileStream)
                return false;

            if (null == m_DiffPackageAsyncPatcher)
                m_DiffPackageAsyncPatcher = new DiffPackageAsyncPatcher();
            
            m_DiffFileStream = Utility.File.OpenRead(diffFilePath);
            if (null != m_DiffFileStream)
            {
                byte[] diffBuf = new byte[m_DiffFileStream.Length];
                
                int totalRead = 0;
                while (totalRead < m_DiffFileStream.Length)
                {
                    int bytesRead = m_DiffFileStream.Read(diffBuf,totalRead, (int)(m_DiffFileStream.Length - totalRead));
                    if (bytesRead <= 0)
                        break;
                    totalRead += bytesRead;
                }
                m_DiffFileStream.Close();
                
                m_DiffPackageAsyncPatcher.BeginPatch(srcFilePath, dstFilePath, diffBuf,dstFileMD5, timeSlice);

                return true;
            }

            return false;
        }

        public bool EndRebuildAsync()
        {
            if (null != m_DiffPackageAsyncPatcher)
                return m_DiffPackageAsyncPatcher.EndPatch();
            else
                return true;
        }
        

        static protected int _GetMin(int x, int y)
        {
            return x < y ? x : y;
        }

        static protected long _DecodeLongType(byte[] buf)
        {
            long y;

            y = buf[7] & 0x7F;
            y = y * 256; y += buf[6];
            y = y * 256; y += buf[5];
            y = y * 256; y += buf[4];
            y = y * 256; y += buf[3];
            y = y * 256; y += buf[2];
            y = y * 256; y += buf[1];
            y = y * 256; y += buf[0];

            if (0 != (buf[7] & 0x80)) y = -y;

            return y;
        }

        static protected void _EncodeLongType(long x, byte[] buf)
        {
            long y;

            if (x < 0) y = -x; else y = x;

            buf[0] = (byte)(y % 256); y -= buf[0]; y = y / 256;
            buf[1] = (byte)(y % 256); y -= buf[1]; y = y / 256;
            buf[2] = (byte)(y % 256); y -= buf[2]; y = y / 256;
            buf[3] = (byte)(y % 256); y -= buf[3]; y = y / 256;
            buf[4] = (byte)(y % 256); y -= buf[4]; y = y / 256;
            buf[5] = (byte)(y % 256); y -= buf[5]; y = y / 256;
            buf[6] = (byte)(y % 256); y -= buf[6]; y = y / 256;
            buf[7] = (byte)(y % 256);

            if (x < 0) buf[7] |= 0x80;
        }

        static bool _IsBSDiffFile(byte[] fileHeader)
        {
            if (fileHeader.Length < CONST_DIFF_FILE_HEAD_TAG.Length)
                return false;

            for (int i = 0, icnt = CONST_DIFF_FILE_HEAD_TAG.Length; i < icnt; ++i)
            {
                if (CONST_DIFF_FILE_HEAD_TAG[i] != fileHeader[i])
                    return false;
            }

            return true;
        }

        static bool _ReadBufFromFile(GZip.GZipStream s, byte[] dstbuf, long offset, long len)
        {
            int total_bytes_read = 0;
            while (total_bytes_read < len)
            {
                int bytes_has_read = s.Read(dstbuf, (int)(offset + total_bytes_read), (int)(len - total_bytes_read));
                if (bytes_has_read <= 0)
                {
                    return false;
                }
                total_bytes_read += bytes_has_read;
            }
            return true;
        }
    }
}