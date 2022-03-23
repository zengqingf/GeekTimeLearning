
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Tenmove.Runtime
{
    internal class LibZipUnpacker
    {
        private enum ZipUnpackState
        {
            Ready,
            Running,
            Done,
            Terminated,
        }

        private readonly string m_PackagePath;
        private readonly string m_OutputFolderPath;
        private readonly long m_CacheSize;

        private float m_UnpackProgress;

        private byte[] m_UnpackCache;
        private ZipUnpackState m_State;
        private int m_PackageFileCount;
        private int m_CurPackageFile;

        private IntPtr m_PackagePtr;
        private zip_stat m_LibZipState;

        public LibZipUnpacker(string packagePath, string outputFolderPath, int cacheSize = 256 * 1024)
        {
            Debugger.Assert(!string.IsNullOrEmpty(packagePath), "Package path can not be null or empty!");
            Debugger.Assert(!string.IsNullOrEmpty(outputFolderPath), "Output folder path can not be null or empty!");

            if(cacheSize < 4096)
            {
                Debugger.LogWarning("Cache size can not less than 4KB,Force set to 4K!");
                cacheSize = 4096;
            }

            m_PackagePath = packagePath;
            m_OutputFolderPath = outputFolderPath;
            m_CacheSize = cacheSize;

            m_UnpackCache = new byte[m_CacheSize];
            m_State = ZipUnpackState.Ready;
            m_UnpackProgress = 0;

            m_PackagePtr = IntPtr.Zero;
            m_CurPackageFile = 0;
        }

        ~LibZipUnpacker()
        {
            _Clear();
        }

        public float Progress
        {
            get { return m_UnpackProgress; }
        }

        public bool HasError
        {
            get { return ZipUnpackState.Terminated == m_State; }
        }

        public string PackageFilePath
        {
            get { return m_PackagePath; }
        }

        public bool UnpackAll(float timeSlice)
        {
            switch(m_State)
            {
                case ZipUnpackState.Ready:_OnReady();return false;
                case ZipUnpackState.Running:_OnRunning(timeSlice);return false;
                case ZipUnpackState.Done:_Clear();return true;
                case ZipUnpackState.Terminated: _Clear(); return true;
                default: throw new TMEngineException(string.Format("Unknown unpack state:[{0}]!",m_State));
            }
        }
        
        private void _OnReady()
        {
            // open the zip file
            m_PackagePtr = LibZip.zip_open(m_PackagePath, 0, IntPtr.Zero);
            if (IntPtr.Zero != m_PackagePtr)
            {
                m_CurPackageFile = 0;
                m_PackageFileCount = LibZip.zip_get_num_files(m_PackagePtr);
                m_LibZipState = new zip_stat();

                m_State = ZipUnpackState.Running;
                m_UnpackProgress = 0.01f;
                return;
            }
            else
                Debugger.LogWarning("Can not open zip package:'{0}'.", m_PackagePath);

            m_UnpackProgress = 1.0f;
            m_State = ZipUnpackState.Terminated;
        }

        private void _OnRunning(float timeSlice)
        {
            float timeElapsed = 0.0f;
            for(int i = m_CurPackageFile,icnt = m_PackageFileCount;i<icnt;++i)
            {
                if (timeElapsed > timeSlice)
                {
                    m_CurPackageFile = i;
                    m_State = ZipUnpackState.Running;
                    return;
                }

                long ticksThen = Utility.Time.GetTicksNow();
                IntPtr curZipFilePtr = IntPtr.Zero;
                if (LibZip.zip_stat_index(m_PackagePtr, i, 0, ref m_LibZipState) == 0)
                {
                    string zipStatName = Marshal.PtrToStringAnsi(m_LibZipState.name);

                    if (!zipStatName.EndsWith("/"))
                    {
                        curZipFilePtr = LibZip.zip_fopen_index(m_PackagePtr, i, 0);
                        if (IntPtr.Zero != curZipFilePtr)
                        {
                            string dstFileName = Utility.Path.Combine(m_OutputFolderPath, zipStatName);
                            string dstFolderPath = Path.GetDirectoryName(dstFileName);
                            
                            if (!Utility.Directory.Exists(dstFolderPath))
                            {
                                try
                                {
                                    Utility.Directory.CreateDirectory(dstFolderPath);
                                }
                                catch(System.Exception e)
                                {
                                    Debugger.LogError("Create folder with exception:'{0}'.",e.Message);
                                    m_State = ZipUnpackState.Terminated;
                                    LibZip.zip_fclose(curZipFilePtr);
                                    return;
                                }
                            }
                            
                            using (Stream file = Utility.File.OpenWrite(dstFileName, true))
                            {
                                Int64 readBytes = 0;
                                while ((readBytes = LibZip.zip_fread(curZipFilePtr, m_UnpackCache, m_CacheSize)) > 0)
                                {
                                    try
                                    {
                                        file.Write(m_UnpackCache, 0, (int)readBytes);
                                    }
                                    catch (System.Exception e)
                                    {
                                        Debugger.LogError("Write file '{0}' with exception:'{1}', terminated!", dstFileName, e.Message);
                                        m_State = ZipUnpackState.Terminated;

                                        LibZip.zip_fclose(curZipFilePtr);
                                        return;
                                    }
                                }

                                file.Flush();
                            }
                            
                            LibZip.zip_fclose(curZipFilePtr);
                            curZipFilePtr = IntPtr.Zero;
                        }
                        else
                        {
                            Debugger.LogWarning("Open package file '{0}' has failed.(Package:'{1}')", m_LibZipState.name, m_PackagePath);
                            m_State = ZipUnpackState.Terminated;
                            return;
                        }
                    }
                }

                timeElapsed += Utility.Time.TicksToSeconds(Utility.Time.GetTicksNow() - ticksThen);
                m_UnpackProgress = Utility.Math.Clamp(0.01f + (((i * 1.0f) / m_PackageFileCount) * 0.99f), 0, 1.0f);
            }

            m_State = ZipUnpackState.Done;
            m_UnpackProgress = 1.0f;

            LibZip.zip_close(m_PackagePtr);
            m_PackagePtr = IntPtr.Zero;
        }

        private void _Clear()
        {
            if(IntPtr.Zero != m_PackagePtr)
            {
                LibZip.zip_close(m_PackagePtr);
                m_PackagePtr = IntPtr.Zero;
            }

            m_UnpackProgress = 1;
            m_UnpackCache = null;
            m_PackageFileCount = 0;
            m_CurPackageFile = 0;
        }
    }
}