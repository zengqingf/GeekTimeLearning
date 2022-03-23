namespace Tenmove
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    public class TMPathUtil
    {
        private static string mRoot = "..";

        public static string Root
        {
            get
            {
                return mRoot;
            }

            set
            {
                mRoot = value;
            }
        }

        public enum Type
        {
            Logs,
            ScreenShots,
            Infos,
            BugReportZip,
            EditorBugReportZip,
        }


        public static string GetDirectoryName(string path)
        {
            string dirpath = Path.GetDirectoryName(path);
            return dirpath.Replace("\\", "/");
        }

        public static bool MakeParentRootExist(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            string rootPath = filePath.Remove(filePath.Length - fileName.Length, fileName.Length);

            return CreateRootDir(rootPath);
        }

        public static bool CreateRootDir(string rootPath)
        {
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
                return true;
            }
            return false;
        }

        public static string GetTypeRootPath(Type type)
        {
            var rootPath = Path.Combine(mRoot, type.ToString());

            CreateRootDir(rootPath);

            return rootPath;
        }

        public static string GetTypeRootPathWithFileName(Type type, string fileName)
        {
            var root = GetTypeRootPath(type);

            return Path.Combine(root, fileName);
        }

        private static string _SplitFileName(string filename)
        {
            string[] splits = filename.Split('_');

            if (null != splits && splits.Length > 0)
            {
                return splits[0];
            }

            return string.Empty;
        }

        public static string GetCurrentDateTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
        }


        public static string GetLatestDirRoot(Type type)
        {
            string rootPath = GetTypeRootPath(type);
            var allDirInRoot = Directory.GetDirectories(rootPath);
            int maxValue = 0;
            string path = string.Empty;
            for (int i = 0; i < allDirInRoot.Length; ++i)
            {
                int v = 0;

                if (int.TryParse(_SplitFileName(Path.GetFileName(allDirInRoot[i])), out v))
                {
                    if (maxValue < v)
                    {
                        maxValue = v;
                        path = allDirInRoot[i];
                    }
                }
            }

            return path;
        }

        public static string CreateTypeNumberDir(Type type, ref string dirName)
        {
            string rootPath = GetTypeRootPath(type);

            var allDirInRoot = Directory.GetDirectories(rootPath);

            if (string.IsNullOrEmpty(dirName))
            {
                int maxValue = 0;
                for (int i = 0; i < allDirInRoot.Length; ++i)
                {
                    int v = 0;

                    if (int.TryParse(_SplitFileName(Path.GetFileName(allDirInRoot[i])), out v))
                    {
                        if (maxValue < v)
                        {
                            maxValue = v;
                        }
                    }
                }

                maxValue++;

                dirName = string.Format("{0:D6}_{1}", maxValue, GetCurrentDateTime());
            }

            var dir = System.IO.Path.Combine(rootPath, dirName);

            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }

            return dir;
        }
    }
}
