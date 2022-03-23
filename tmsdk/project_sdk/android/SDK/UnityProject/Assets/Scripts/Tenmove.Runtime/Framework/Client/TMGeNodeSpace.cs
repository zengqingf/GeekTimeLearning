

using System.Collections.Generic;
using Tenmove.Runtime.Math;

namespace Tenmove.Runtime
{
    public class GeNodeSpace : GeNode,ITMGeNodeSpace
    {
        public new enum Dirty
        {
            None = 0,
            Position = 1 << 3,
            Rotation = 1 << 4,
            Scale = 1 << 5,
            MirrorX = 1 << 6,
            MirrorY = 1 << 7,
            MirrorZ  = 1 << 8,
            Mirror = MirrorX | MirrorY | MirrorZ,
            Transform = Position | Rotation | Scale | Mirror,
        }

        private ITMNativeNodeSpace m_NativeNodeSpace;
        
        private Vec3 m_Position;
        private Quat m_Rotation;
        private Vec3 m_LocalPosition;
        private Quat m_LocalRotation;
        private Vec3 m_LocalScale;
        private bool m_MirrorX;
        private bool m_MirrorY;
        private bool m_MirrorZ;

        public GeNodeSpace()
        {
            m_NativeNodeSpace = null;

            m_Position = Vec3.Zero;
            m_Rotation = Quat.Identity;
            m_LocalPosition = Vec3.Zero;
            m_LocalRotation = Quat.Identity;
            m_LocalScale = Vec3.One;
            m_MirrorX = false;
            m_MirrorY = false;
            m_MirrorZ = false;
        }

        public Vec3 Position
        {
            get { return m_Position; }
        }

        public Quat Rotation
        {
            get { return m_Rotation; }
        }


        public Vec3 LocalPosition
        {
            get { return m_LocalPosition; }
        }

        public Quat LocalRotation
        {
            get { return m_LocalRotation; }
        }

        public Vec3 LocalScale
        {
            get { return m_LocalScale; }
        }

        public bool MirrorX
        {
            get { return m_MirrorX; }
        }

        public bool MirrorY
        {
            get { return m_MirrorY; }
        }

        public bool MirrorZ
        {
            get { return m_MirrorZ; }
        }

        public void SetPosition(Vec3 position)
        {
            m_Position = position;
            m_DirtyFlags += (uint)Dirty.Position;
        }

        public void SetRotation(Quat rotation)
        {
            m_Rotation = rotation;
            m_DirtyFlags += (uint)Dirty.Rotation;
        }

        public void SetLocalPosition(Vec3 localPosition)
        {
            if (localPosition != m_LocalPosition)
            {
                m_LocalPosition = localPosition;
                m_DirtyFlags += (uint)Dirty.Position;
            }
        }

        public void SetLocalRotation(Quat localRotation)
        {
            if (localRotation != m_LocalRotation)
            {
                m_LocalRotation = localRotation;
                m_DirtyFlags += (uint)Dirty.Rotation;
            }
        }

        public void SetLocalScale(Vec3 localScale)
        {
            m_LocalScale = localScale;
            m_DirtyFlags += (uint)Dirty.Scale;
        }

        public void SetMirrorX(bool mirror)
        {
            m_MirrorX = mirror;
            m_DirtyFlags += (uint)Dirty.MirrorX;
        }

        public void SetMirrorY(bool mirror)
        {
            m_MirrorY = mirror;
            m_DirtyFlags += (uint)Dirty.MirrorY;
        }

        public void SetMirrorZ(bool mirror)
        {
            m_MirrorZ = mirror;
            m_DirtyFlags += (uint)Dirty.MirrorZ;
        }

        protected override void _OnNativeObjectCreated(ITMNativeObject nativeObject)
        {
            base._OnNativeObjectCreated(nativeObject);
            m_NativeNodeSpace = nativeObject.QureyComponent<ITMNativeNodeSpace>();
            if (null != m_NativeNodeSpace && !m_DirtyFlags.HasFlag((uint)Dirty.Transform))
            {
                m_NativeNodeSpace.ExtractTransform(out m_Position, out m_Rotation);
                m_NativeNodeSpace.ExtractLocalTransform(out m_LocalPosition, out m_LocalRotation, out m_LocalScale);
            }
        }

        protected override void _OnFlush()
        {
            base._OnFlush();

            if (null != m_NativeNodeSpace)
            {
                if (m_DirtyFlags.HasFlag((uint)Dirty.Transform))
                {
                    m_LocalScale.x = (m_MirrorX ? -1 : 1) * Utility.Math.Abs(m_LocalScale.x);
                    m_LocalScale.y = (m_MirrorY ? -1 : 1) * Utility.Math.Abs(m_LocalScale.y);
                    m_LocalScale.z = (m_MirrorZ ? -1 : 1) * Utility.Math.Abs(m_LocalScale.z);

                    m_NativeNodeSpace.FlushTransform(m_Position, m_Rotation);
                    m_NativeNodeSpace.FlushLocalTransform(m_LocalPosition, m_LocalRotation, m_LocalScale);

                    m_DirtyFlags -= (uint)Dirty.Transform;
                }
            }
        }
    }
}