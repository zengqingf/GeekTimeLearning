

using System;
using System.Collections.Generic;
using Tenmove.Runtime.Math;

namespace Tenmove.Runtime
{
    public interface ITMGeNodeSpace : ITMGeNode
    {
        Vec3 Position
        {
            get;
        }

        Quat Rotation
        {
            get;
        }

        Vec3 LocalPosition
        {
            get;
        }

        Quat LocalRotation
        {
            get;
        }

        Vec3 LocalScale
        {
            get;
        }

        bool MirrorX
        {
            get;
        }

        bool MirrorY
        {
            get;
        }

        bool MirrorZ
        {
            get;
        }

        void SetPosition(Vec3 position);
        void SetRotation(Quat rotation);

        void SetLocalPosition(Vec3 position);
        void SetLocalRotation(Quat rotation);
        void SetLocalScale(Vec3 scale);

        void SetMirrorX(bool mirror);
        void SetMirrorY(bool mirror);
        void SetMirrorZ(bool mirror);
    }
}