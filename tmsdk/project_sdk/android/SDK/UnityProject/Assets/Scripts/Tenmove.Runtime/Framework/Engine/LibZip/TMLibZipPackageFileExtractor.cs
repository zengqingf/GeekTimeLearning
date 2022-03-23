using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Tenmove.Runtime
{
    internal class LibZipPackageFileExtractor
    {
        public static bool ExtractFile(string packagePath,string fileName,string outputfilePath,int cacheSize = 16384)
        {
            Debugger.Assert(!string.IsNullOrEmpty(packagePath), "Package path can not be null or empty!");
            Debugger.Assert(!string.IsNullOrEmpty(fileName), "File name can not be null or empty!");
            Debugger.Assert(!string.IsNullOrEmpty(outputfilePath), "Output folder path can not be null or empty!");

            Debugger.LogWarning("Extract file '{0}' from package '{1}' to path '{2}'.", fileName, packagePath, outputfilePath);
            if (cacheSize < 4096)
            {
                Debugger.LogWarning("Cache size can not less than 4KB,Force set to 4KB!");
                cacheSize = 4096;
            }

            string dstFolderPath = Utility.Path.GetDirectoryName(outputfilePath);
            if (!Utility.Directory.Exists(dstFolderPath))
            {
                try
                {
                    Utility.Directory.CreateDirectory(dstFolderPath);
                }
                catch (System.Exception e)
                {
                    Debugger.LogError("Create folder with exception:'{0}'.", e.Message);
                    return false;
                }
            }

            // open the zip file
            IntPtr packagePtr = LibZip.zip_open(packagePath, 0, IntPtr.Zero);
            if (IntPtr.Zero != packagePtr)
            {
                IntPtr filePtr = LibZip.zip_fopen(packagePtr, fileName, 0);
                if(IntPtr.Zero != filePtr)
                {
                    try
                    {
                        byte[] cache = new byte[cacheSize];
                        using (Stream file = Utility.File.OpenWrite(outputfilePath, true))
                        {
                            Int64 readBytes = 0;
                            while ((readBytes = LibZip.zip_fread(filePtr, cache, cacheSize)) > 0)
                            {
                                file.Write(cache, 0, (int)readBytes);
                            }

                            file.Flush();
                        }
                    }
                    catch(Exception e)
                    {
                        Debugger.LogError("Write file '{0}' with exception:'{1}', terminated!", fileName, e.Message);
                        return false;
                    }
                    finally
                    {
                        LibZip.zip_fclose(filePtr);
                        LibZip.zip_close(packagePtr);
                    }

                    return true;
                }
                else
                    Debugger.LogWarning("Can not open zip file with file name:'{0}'.", fileName);
            }
            else
                Debugger.LogWarning("Can not open zip package:'{0}'.", packagePath);

            return false;
        }

        public static bool IsFileInZip(string packagePath, string fileName)
        {
            Debugger.Assert(!string.IsNullOrEmpty(packagePath), "Package path can not be null or empty!");
            Debugger.Assert(!string.IsNullOrEmpty(fileName), "File name can not be null or empty!");

            // open the zip file
            IntPtr packagePtr = LibZip.zip_open(packagePath, 0, IntPtr.Zero);
            if (IntPtr.Zero != packagePtr)
            {
                IntPtr filePtr = LibZip.zip_fopen(packagePtr, fileName, 0);
                if (IntPtr.Zero != filePtr)
                {
                    LibZip.zip_fclose(filePtr);
                    LibZip.zip_close(packagePtr);
                    return true;
                }
                
                LibZip.zip_close(packagePtr);
            }

            return false;
        }
    }
}