

using System;
using Tenmove.Runtime.Math;

namespace Tenmove.Runtime
{
    public delegate void OnFlushTransform(Vec3 pos, Quat rot);
    public delegate void OnFlushLocalTransform(Vec3 pos, Quat rot, Vec3 scale);

    public interface ITMNativeNodeSpace : ITMNativeNode
    {
        event OnFlushTransform OnFlushTransform;
        event OnFlushLocalTransform OnFlushLocalTransform;

        Vec3 Position
        {
            get;
        }

        Quat Rotation
        {
            get;
        }

        Vec3 Scale
        {
            get;
        }
        
        void FlushTransform(Vec3 pos, Quat rot);
        void FlushLocalTransform(Vec3 pos, Quat rot, Vec3 scale);

        void ExtractTransform(out Vec3 position, out Quat orientation);
        void ExtractLocalTransform(out Vec3 localPosition, out Quat localOrientation, out Vec3 localScale);
    }
}

