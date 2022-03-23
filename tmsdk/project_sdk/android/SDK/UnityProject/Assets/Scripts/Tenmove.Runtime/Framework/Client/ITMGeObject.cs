

using System;
using System.Collections.Generic;
using Tenmove.Runtime.Math;

namespace Tenmove.Runtime
{
    public delegate void OnObjectReady(bool success,ITMGeObject obj);

    public class GeObjectParams
    {
        static public readonly GeObjectParams Default = new GeObjectParams();

        public GeObjectParams()
        {
        }
    }

    public interface ITMGeObject
    {
        ulong ObjectID
        {
            get;
        }

        bool HasNative
        {
            get;
        }

        event OnObjectReady OnObjectReady;

        bool Init(ulong id, ITMGeObjectManager objManager, GeObjectParams objParams);
        void Destroy();
    }
}