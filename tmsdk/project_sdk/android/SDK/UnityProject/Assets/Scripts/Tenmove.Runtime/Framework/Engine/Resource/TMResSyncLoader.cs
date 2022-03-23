﻿using System;

namespace Tenmove.Runtime
{
    public abstract class ResSyncLoader : ITMResourceLoader
    {
        abstract public object LoadPackage(string fullpath);
        abstract public object LoadAsset(object package, string assetName,string subResName,Type assetType);
        abstract public bool LoadFile(string filepath,bool readWritePath,out byte[] data);
        abstract public void UnloadPackage(object package);
        abstract public void Reset();
    }
}

