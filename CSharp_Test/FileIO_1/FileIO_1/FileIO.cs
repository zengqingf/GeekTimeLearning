using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace FileIO_1
{
    //C#读取大文件 https://www.cnblogs.com/zhao123/p/10985340.html
    class FileIO
    {
        private static readonly object ms_fsLock = new object();

        public static void WriteFile(string targetFileName, byte[] fileContents)
        {
            File.WriteAllBytes(targetFileName, fileContents);
        }

        public static byte[] ReadLargeFile(string fileName)
        {
            long maxFileSize = 3L * 1024 * 1024 * 1024;
            byte[] tempBytes = new byte[maxFileSize];
            //replace to process file by 200MB portions
            int bufferSize = 100 * 1024 * 1024; //100MB
            byte[] bufferBytes = new byte[bufferSize];
            int bytesRead = 0;
            int bytesOffset = 0;
            int tempStartIndex = 0;

            lock (ms_fsLock)
            {
                Console.WriteLine("### read large file is {0}", fileName);
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite))
                {
                    Console.WriteLine("### fs length = {0}", fs.Length);

                    //1
                    //using (BufferedStream bs = new BufferedStream(fs))
                    //{
                    //    while ((bytesRead = bs.Read(bufferBytes, 0, bufferSize)) > 0)
                    //    {
                    //        Array.Copy(bufferBytes, 0, tempBytes, tempStartIndex, bufferBytes.Length);
                    //        tempStartIndex = tempStartIndex + bufferBytes.Length;
                    //    }                 
                    //}

                    //2
                    //tempBytes = readBytes(fs);

                    //3
                    //while ((bytesRead = fs.Read(bufferBytes, 0, bufferSize)) > 0)
                    //{
                    //	Console.WriteLine("### bytesOffset = {0}, bytesRead = {1}", bytesOffset, bytesRead);
                    //	Console.WriteLine("### tempBytes length = {0}, bufferBytes length = {1}", tempBytes.Length, bufferBytes.Length);
                    //	tempStartIndex = tempBytes.Length;
                    //	Array.Resize(ref tempBytes, tempStartIndex + bufferBytes.Length);
                    //	Array.Copy(bufferBytes, 0, tempBytes, tempStartIndex, bufferBytes.Length);
                    //	bytesOffset += bytesRead;
                    //	fs.Seek(bytesOffset, SeekOrigin.Begin);
                    //	Console.WriteLine("### bytesOffset = {0}", bytesOffset);
                    //}

                    //4
                    //string mapFileName = "myfile";
                    //Console.WriteLine("### map file name is {0}", mapFileName);
                    //using (var mmf = MemoryMappedFile.CreateFromFile(fs, mapFileName, fs.Length,
                    //    MemoryMappedFileAccess.ReadWrite, new MemoryMappedFileSecurity() { }, HandleInheritability.Inheritable, true))
                    //{
                    //    using (var mmfReader = mmf.CreateViewAccessor())
                    //    {
                    //        tempBytes = new byte[mmfReader.Capacity];
                    //        mmfReader.ReadArray<byte>(0, tempBytes, 0, tempBytes.Length);
                    //    }
                    //}
                }
            }
            return tempBytes;
        }

        private static byte[] readBytes(Stream stream)
        {
            MemoryStream temp = new MemoryStream();
            copyStream(stream, temp, Int64.MaxValue);
            return temp.GetBuffer();
        }

        private static long copyStream(Stream input, Stream output, long count)
        {
            const int MaxBufferSize = 1048576; // 1 MB
            int bufferSize = (int)Math.Min(MaxBufferSize, count);
            byte[] buffer = new byte[bufferSize];
            long totalBytesRead = 0;
            while (totalBytesRead < count)
            {
                int numberOfBytesToRead = (int)Math.Min(bufferSize, count - totalBytesRead);
                //注意：stream里面配置了_readPos类全局状态，可以缓存当前读取的位置
                int bytesRead = input.Read(buffer, 0, numberOfBytesToRead);
                totalBytesRead += bytesRead;
                output.Write(buffer, 0, bytesRead);
                if (bytesRead <= 0) // no more bytes to read from input stream
                {
                    return totalBytesRead;
                }
            }
            return totalBytesRead;
        }

        private static void copyFileStream(Stream input, Stream output)
        {
            const int MaxBufferSize = 1048576; // 1 MB
            int bufferSize = (int)Math.Min(MaxBufferSize, Int64.MaxValue);
            byte[] buffer = new byte[bufferSize];
            long totalBytesRead = 0;
            while (totalBytesRead < Int64.MaxValue)
            {
                int numberOfBytesToRead = (int)Math.Min(bufferSize, Int64.MaxValue - totalBytesRead);
                int bytesRead = input.Read(buffer, 0, numberOfBytesToRead);
                input.Flush();
                totalBytesRead += bytesRead;
                output.Write(buffer, 0, bytesRead);
                output.Flush();
                if (bytesRead <= 0) // no more bytes to read from input stream
                {
                    input.Close();
                    output.Close();
                    return;
                }
            }
            input.Close();
            output.Close();
            return;
        }


        /*
         https://stackoverflow.com/questions/5208592/reading-parts-of-large-files-from-drive
        //  This really needs to be a member-level variable;
        private static readonly object fsLock = new object();

        //  Instantiate this in a static constructor or initialize() method
        private static FileStream fs = new FileStream("myFile.txt", FileMode.Open);


        public string ReadFile(int fileOffset)
        {

            byte[] buffer = new byte[bufferSize];

            int arrayOffset = 0;

            lock (fsLock)
            {
                fs.Seek(fileOffset, SeekOrigin.Begin);

                int numBytesRead = fs.Read(bytes, arrayOffset, bufferSize);

                //  Typically used if you're in a loop, reading blocks at a time
                arrayOffset += numBytesRead;
            }

            // Do what you want to the byte array and return it

        }
         */
    }
}
