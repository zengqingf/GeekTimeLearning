

using System;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    public class LibZipPackage
    {
        private class ZipDesc
        {
            public int ReferenceCount { set; get; }
            public string PackagePath { set; get; }
            public IntPtr PackageHandle { set; get; }
        }

        static readonly LinkedList<ZipDesc> sm_ZipPackageList = new LinkedList<ZipDesc>();

        static public IntPtr Open(string packagePath)
        {
            LinkedListNode<ZipDesc> cur = sm_ZipPackageList.First;
            while (null != cur)
            {
                if (cur.Value.PackagePath == packagePath)
                {
                    ++cur.Value.ReferenceCount;
                    return cur.Value.PackageHandle;
                }

                cur = cur.Next;
            }

            // open the zip file
            IntPtr packagePtr = LibZip.zip_open(packagePath, 0, IntPtr.Zero);
            if (IntPtr.Zero != packagePtr)
            {
                sm_ZipPackageList.AddLast(new ZipDesc() { PackageHandle = packagePtr, ReferenceCount = 1, PackagePath = packagePath });
                return packagePtr;
            }
            else
                Debugger.LogWarning("Open package with path '{0}' has failed!", packagePath);

            return IntPtr.Zero;
        }

        static public void Close(IntPtr packageHandle)
        {
            LinkedListNode<ZipDesc> cur = sm_ZipPackageList.First;
            while (null != cur)
            {
                if (cur.Value.PackageHandle == packageHandle)
                {
                    --cur.Value.ReferenceCount;
                    if(0 <= cur.Value.ReferenceCount)
                    {
                        LibZip.zip_close(cur.Value.PackageHandle);
                        sm_ZipPackageList.Remove(cur);
                    }

                    return;
                }

                cur = cur.Next;
            }
        }
    }
}