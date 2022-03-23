


using System;
using System.IO;

namespace Tenmove.Runtime
{
    public class LibZipFileStream : Stream
    {
        private readonly string m_PackagePath;
        private readonly string m_FileName;

        private IntPtr m_PackageHandle;
        private IntPtr m_FileHandle;
        private zip_stat m_FileDesc;

        public LibZipFileStream(string packagePath,string fileName)
        {
            Debugger.Assert(!string.IsNullOrEmpty(packagePath), "Package path can not be null or empty!");
            Debugger.Assert(!string.IsNullOrEmpty(fileName), "File name can not be null or empty!");

            m_PackagePath = packagePath;
            m_FileName = fileName;
            m_PackageHandle = LibZipPackage.Open(packagePath);
            if (IntPtr.Zero != m_PackageHandle)
            {
                m_FileHandle = LibZip.zip_fopen(m_PackageHandle, fileName, 0);
                if (IntPtr.Zero != m_FileHandle)
                {
                    LibZip.zip_stat(m_PackageHandle, fileName, 0, ref m_FileDesc);
                    //Debugger.LogInfo("Open file '{0}' in package '{1}' has succeed!", fileName, packagePath);
                    return;
                }
                else
                    Debugger.LogWarning("Open zip file with file '{0}' has failed![Package:{1}]",fileName, packagePath);

                LibZipPackage.Close(m_PackageHandle);
                m_PackageHandle = IntPtr.Zero;
            }
            else
                Debugger.LogWarning("Open zip package with path '{0}' has failed!", packagePath);
        }

        public override bool CanRead { get { return true; } }
        public override bool CanSeek { get { return true; } }
        public override bool CanWrite { get { return false; } }

        public override long Length
        {
            get
            {
                return m_FileDesc.size;
            }
        }

        public override long Position
        {
            get { return LibZip.zip_ftell(m_FileHandle); }
            set { Seek(value, SeekOrigin.Begin); }
        }

        public override void Flush()
        {
            throw new System.NotImplementedException();
        }

        public sealed override int Read(byte[] buffer, int offset, int count)
        {
            int res = (int)LibZip.zip_fread(m_FileHandle, buffer, count);
            //Debugger.LogInfo("Read zip stream '{0}' in package:'{1}' read bytes[{2}].",m_PackagePath,m_FileName,res);
            return res;
        }

        public sealed override long Seek(long offset, SeekOrigin origin)
        {
            int whence = 0;
            switch(origin)
            {
                case SeekOrigin.Begin: whence = 0;break;
                case SeekOrigin.Current: whence = 1;break;
                case SeekOrigin.End: whence = 2; break;
            }

            long res = LibZip.zip_fseek(m_FileHandle, offset, whence);
            if (-1 == res)
                Debugger.LogWarning("Zip file stream seek can only used in uncompressed package!");
            return res;
        }

        public override void SetLength(long value)
        {
            throw new System.NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new System.NotImplementedException();
        }

        public override void Close()
        {
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (IntPtr.Zero != m_FileHandle)
            {
                LibZip.zip_fclose(m_FileHandle);
                m_FileHandle = IntPtr.Zero;
            }

            if (IntPtr.Zero != m_PackageHandle)
            {
                LibZipPackage.Close(m_PackageHandle);
                m_PackageHandle = IntPtr.Zero;
            }
        }
    }
}