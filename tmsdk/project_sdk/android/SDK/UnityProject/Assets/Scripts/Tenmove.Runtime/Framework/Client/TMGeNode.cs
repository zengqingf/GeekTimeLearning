

using System.Collections.Generic;
using Tenmove.Runtime.Math;

namespace Tenmove.Runtime
{
    public class GeNode : GeObject,ITMGeNode
    {
        /// <summary>
        /// 注意父类中也有这个枚举
        /// </summary>
        public new enum Dirty
        {
            None = 0,
            Active = 1 << 1,
            Parent = 1 << 2,
        }

        private ITMNativeObject m_NativeObject;
        private ITMNativeNode m_NativeNode;
        private readonly List<ITMGraphicUpdateHandler> m_GraphicHandlers;
        private readonly List<ITMLogicUpdateHandler> m_LogicHandlers;

        private readonly LinkedList<GeNode> m_Children;
        private readonly List<ITMNativeCommand> m_CommandCache;

        private GeNode m_ParentNode;
        private bool m_KeepTransform;

        private bool m_IsActive;

        private event OnAttachToParent m_OnAttachToParent;
        private event OnDetachFromParent m_OnDetachFromParent;

        public GeNode()
        {
            m_NativeObject = null;
            m_NativeNode = null;
            m_ParentNode = null;

            m_Children = new LinkedList<GeNode>();

            m_DirtyFlags = new EnumHelper<uint>((uint)Dirty.None);
            m_CommandCache = new List<ITMNativeCommand>();

            m_GraphicHandlers = new List<ITMGraphicUpdateHandler>();
            m_LogicHandlers = new List<ITMLogicUpdateHandler>(); 

            m_IsActive = true;
        }

        public sealed override bool HasNative
        {
            get { return true; }
        }

        public override bool NeedUpdate
        {
            get { return true; }
        }

        public ITMGeNode Parent
        {
            get { return m_ParentNode; }
        }

        public ITMNativeObject Native
        {
            get { return m_NativeObject; }
        }

        public event OnAttachToParent OnAttachToParent
        {
            add { m_OnAttachToParent += value; }
            remove { m_OnAttachToParent -= value; }
        }

        public event OnDetachFromParent OnDetachFromParent
        {
            add { m_OnDetachFromParent += value; }
            remove { m_OnDetachFromParent -= value; }
        }

        public virtual void Pause() { }
        public virtual void Resume() { }

        public void SetActive(bool isActive)
        {
            m_IsActive = isActive;
            m_DirtyFlags += (uint)Dirty.Active;
        }

        public bool IsActive()
        {
            return m_IsActive;
        }

        public int GetChildCount()
        {
            return m_Children.Count;
        }

        public ITMGeNode GetChildByIndex(int index)
        {
            int count = 0;
            LinkedListNode<GeNode> cur = m_Children.First;
            while (null != cur)
            {
                if (count == index)
                    return cur.Value;
                cur = cur.Next;
                ++count;
            }

            return null;
        }

        public void AddChild(ITMGeNode node)
        {
            if (null == node)
                return;

            GeNode newChild = node as GeNode;
            if (null != newChild)
            {
                Debugger.Assert(null == newChild.Parent, "Child note parent is not null!");
                if (!HasChild(newChild))
                {
                    newChild._OnAttachToParent(this);
                    if (null != m_OnAttachToParent)
                        m_OnAttachToParent(newChild,this);
                    m_Children.AddLast(newChild);
                }
                else
                    Debugger.LogWarning("Node[Name:{0}] is already a child of node[Name:{1}]!", node.Name, Name);
            }
            else
                Debugger.LogWarning("Child node type [{0}] is not type [{1}]!", node.GetType(), typeof(GeNode));
        }

        public void RemoveChild(ITMGeNode node)
        {
            if (null == node)
                return;

            LinkedListNode<GeNode> cur = m_Children.First;
            LinkedListNode<GeNode> next;
            while (null != cur)
            {
                next = cur.Next;
                if (node.ObjectID == cur.Value.ObjectID)
                {
                    cur.Value._OnDetachFromParent(this);
                    if (null != m_OnDetachFromParent)
                        m_OnDetachFromParent(cur.Value, this);
                    m_Children.Remove(cur);
                    return;
                }

                cur = next;
            }

            Debugger.LogWarning("Node [Name:{0}] is not a child of Node[Name:{1}]!", node.Name, Name);
        }

        public bool HasChild(ITMGeNode node)
        {
            if (null == node)
                return false;

            LinkedListNode<GeNode> cur = m_Children.First;
            while (null != cur)
            {
                if (node.ObjectID == cur.Value.ObjectID)
                    return true;

                cur = cur.Next;
            }

            return false;
        }

        public void SetParent(ITMGeNode node, bool keepTransform)
        {
            m_KeepTransform = keepTransform;
            if (null != m_ParentNode)
            {
                m_ParentNode.RemoveChild(this);
                m_ParentNode = null;
            }

            GeNode newParent = node as GeNode;
            if (null != newParent)
            {
                newParent.AddChild(this);
            }

            m_ParentNode = newParent;
            m_DirtyFlags += (uint)Dirty.Parent; 
        }

        public ITMNativeComponent AddComponent(string componentTypeName)
        {
            if (null != m_NativeObject)
                return m_NativeObject.CreateComponent(componentTypeName);
            else
                return null;
        }

        public ITMNativeComponent QureyComponent(string componentTypeName)
        {
            if (null != m_NativeObject)
                return m_NativeObject.QureyComponent(componentTypeName);
            else
                return null;
        }

        public TNativeComponent QureyComponent<TNativeComponent>() where TNativeComponent : class, ITMNativeComponent
        {
            if (null != m_NativeObject)
                return m_NativeObject.QureyComponent<TNativeComponent>();
            else
                return null;
        }

        public void PushCommand(ITMNativeCommand cmd)
        {
            if (null != m_NativeObject)
                m_NativeObject.DispatchCommand(cmd);
            else
                m_CommandCache.Add(cmd);
        }

        public void OnUpdateGraphic(float logicDeltaTime,float realDeltaTime)
        {
            for (int i = 0, icnt = m_GraphicHandlers.Count; i < icnt; ++i)
                m_GraphicHandlers[i].OnUpdateGraphic(logicDeltaTime, realDeltaTime);
        }

        public void OnUpdateLogic(float logicDeltaTime, float realDeltaTime)
        {
            for (int i = 0, icnt = m_LogicHandlers.Count; i < icnt; ++i)
                m_LogicHandlers[i].OnUpdateLogic(logicDeltaTime, realDeltaTime);
        }

        protected override bool _OnInit(GeObjectParams objDesc)
        {
            return true;
        }

        protected override void _OnDeinit()
        {
        }

        protected virtual void _OnAttachToParent(GeNode parent) { }
        protected virtual void _OnDetachFromParent(GeNode parent) { }

        protected override void _OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (m_DirtyFlags != (int)Dirty.None)
                _Flush();
        }

        protected override void _OnDispose()
        {
            base._OnDispose();

            m_OnAttachToParent = null;
            m_OnDetachFromParent = null;

            if (null != m_ParentNode)
            {
                m_ParentNode.RemoveChild(this);
                m_ParentNode = null;
            }

            LinkedListNode<GeNode> cur = m_Children.First;
            while (null != cur)
            {
                cur.Value.Destroy();
                cur = cur.Next;
            }

            if (null != m_NativeObject)
            {
                m_NativeObject.Destroy();
                m_NativeObject = null;
                m_NativeNode = null;
            }
        }

        protected override void _OnNativeObjectCreated(ITMNativeObject nativeObject)
        {
            m_NativeObject = nativeObject;
            if (null != m_NativeObject)
            {
                m_NativeObject.QureyComponentsWithInterface(m_GraphicHandlers);
                m_NativeObject.QureyComponentsWithInterface(m_LogicHandlers);
                m_NativeNode = m_NativeObject.QureyComponent<ITMNativeNode>() as ITMNativeNode;
            }
        }

        protected override void _PostNativeObjectCreated()
        {
            if (null != m_NativeObject)
            {
                _Flush();
                /// 晚一帧显示
                SetActive(m_IsActive);

                for (int i = 0, icnt = m_CommandCache.Count; i < icnt; ++i)
                    m_NativeObject.DispatchCommand(m_CommandCache[i]);
                m_CommandCache.Clear();
            }
        }

        protected virtual void _OnFlush()
        {
        }

        private void _Flush()
        {
            _OnFlush();

            if (null != m_NativeObject)
            {
                if (m_DirtyFlags.HasFlag((int)GeObject.Dirty.Name))
                {
                    m_NativeObject.FlushName(m_Name);
                    m_DirtyFlags -= (uint)GeObject.Dirty.Name;
                }

                if (m_DirtyFlags.HasFlag((int)Dirty.Active))
                {
                    m_NativeObject.SetActive(m_IsActive);
                    m_DirtyFlags -= (uint)Dirty.Active;
                }

                if (m_DirtyFlags.HasFlag((int)Dirty.Parent))
                {
                    Debugger.Assert(null == m_ParentNode || null != m_ParentNode.m_NativeNode, "This is the problem,Parent's native node does not created yet!");
                    m_NativeNode.FlushParent(null != m_ParentNode ? m_ParentNode.m_NativeNode : null, m_KeepTransform);
                    m_DirtyFlags -= (uint)Dirty.Parent;
                }
            }
        }
    }
}