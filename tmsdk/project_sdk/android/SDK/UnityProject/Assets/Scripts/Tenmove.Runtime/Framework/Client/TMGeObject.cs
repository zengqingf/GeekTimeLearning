

using System.Collections.Generic;

namespace Tenmove.Runtime
{

    public abstract class GeObject : ITMGeObject
    {
        public enum Dirty
        {
            None = 0,
            Name = 1 << 0,
        }

        public enum ObjectState
        {
            None,
            /// <summary>
            /// 构造调用后
            /// </summary>
            Constucted,
            /// <summary>
            /// 初始化后（后面池化的对象会从这里重新初始化）
            /// </summary>
            Inited,
            /// <summary>
            /// 反初始化后（后面池化的对象会从这里反初始化后回到池子中）
            /// </summary>
            Deinited,
            /// <summary>
            /// 销毁后（对象从逻辑上被销毁）
            /// </summary>
            Destroyed,
            /// <summary>
            /// 释放后（对象会释放所有Native对象，真正的从内存中移除）
            /// </summary>
            Disposed,
        }

        static public readonly ulong InvalidID = ~0u;
                
        private ulong m_ObjectID;
        protected ITMGeObjectManager m_ObjectManager;
        protected EnumHelper<uint> m_DirtyFlags;
        protected string m_Name;

        private ObjectState m_ObjectState;
        private event OnObjectReady m_OnObjectReady;

        private uint m_UpdateFrame;

        public GeObject()
        {
            m_ObjectState = ObjectState.Constucted;

            m_ObjectID = InvalidID;
            m_ObjectManager = null;
            m_Name = string.Empty;

            m_UpdateFrame = 0;
        }

        public ulong ObjectID
        {
            get { return m_ObjectID; }
        }

        public abstract bool HasNative
        {
            get;
        }

        public abstract bool NeedUpdate
        {
            get;
        }

        public string Name
        {
            get { return m_Name; }
        }

        public bool HasDestroyed
        {
            get { return ObjectState.Destroyed <= m_ObjectState; }
        }

        public event OnObjectReady OnObjectReady
        {
            add { m_OnObjectReady += value; }
            remove { m_OnObjectReady -= value; }
        }

        public virtual void SetName(string name)
        {
            m_Name = name;
            m_DirtyFlags += (uint)Dirty.Name;
        }

        public void Update(float elapseSeconds, float realElapseSeconds, uint frameIdx)
        {
            if (frameIdx == m_UpdateFrame)
                return;

            _OnUpdate( elapseSeconds, realElapseSeconds);
            m_UpdateFrame = frameIdx;
        }

        internal void OnNativeObjectCreated(NativeOperateState state, ITMNativeObject nodeObj, string resPath, int requestID, string errorMsg)
        {
            bool isSucceed = NativeOperateState.OK == state;
            if (isSucceed)
            {
                _OnNativeObjectCreated(nodeObj);
                if(null != nodeObj && !m_DirtyFlags.HasFlag((uint)Dirty.Name))
                    m_Name = nodeObj.Name;
            }
            else
                Debugger.LogWarning("Create native node object with path '{0}' has failed! [Internal error:{1}]", resPath, errorMsg);

            _PostNativeObjectCreated();
            _OnObjectReady(isSucceed);
        }

        public bool Init(ulong id, ITMGeObjectManager objManager, GeObjectParams objDesc)
        {
            Debugger.Assert(InvalidID != id, "ID is invalid!");
            Debugger.Assert(null != objManager, "Object manager can not be null!");

            m_ObjectID = id;
            m_ObjectManager = objManager;

            if (_OnInit(objDesc))
            {
                m_ObjectState = ObjectState.Inited;
                return true;
            }
            else
                return false;
        }

        public void Deinit()
        {
            _OnDeinit();
            m_ObjectState = ObjectState.Deinited;
        }

        public void Destroy()
        {
            if (ObjectState.Destroyed > m_ObjectState)
            {
                if (ObjectState.Deinited > m_ObjectState)
                    Deinit();

                m_ObjectState = ObjectState.Destroyed;
                m_ObjectManager.DestroyObject(this);
            }
        }

        public void Dispose()
        {
            if (ObjectState.Disposed > m_ObjectState)
            {
                if (ObjectState.Destroyed > m_ObjectState)
                {
                    Debugger.LogWarning("You can not invoke 'Dispose()' manually!");
                    return;
                }

                _OnDispose();

                m_ObjectID = InvalidID;
                m_ObjectManager = null;
                m_ObjectState = ObjectState.Disposed;
            }
            else
                Debugger.LogWarning("Attention! 'Dispose' has already invoked!,There must be a bug in program!");
        }

        protected abstract bool _OnInit(GeObjectParams objDesc);
        protected abstract void _OnDeinit();

        protected virtual void _OnNativeObjectCreated(ITMNativeObject nativeObject)
        {
        }

        protected virtual void _PostNativeObjectCreated()
        {
        }

        protected void _OnObjectReady(bool success)
        {
            if (null != m_OnObjectReady)
                m_OnObjectReady(success, this);
        }

        protected virtual void _OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }

        protected virtual void _OnDispose()
        {
        }
    }

}