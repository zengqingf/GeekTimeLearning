

using System;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    public class GeObjectManager : BaseModule, ITMGeObjectManager
    {
        private readonly ITMNativeObjectManager m_NativeObjManager;
        private readonly LinkedList<GeObject> m_ObjectList;
        private ulong m_NodeObjAllocCount;

        private uint m_FrameCount;

        private delegate void ExtractNativeObjectsFunc<T>(ITMNativeObject root, T value, List<ITMNativeObject> extractedObjests);

        private class NativeObjectCache
        {
            public NativeObjectCache(GeObject obj, int requestID)
            {
                m_Object = obj;
                m_RequestID = requestID;
            }

            public GeObject Object
            {
                get { return m_Object; }
            }

            public int RequestID
            {
                get { return m_RequestID; }
            }

            private GeObject m_Object;
            private int m_RequestID;
        }

        private readonly LinkedList<NativeObjectCache> m_ObjectCacheList;

        static bool mUseNative = true;

        public static bool UseNative
        {
            set
            {
                mUseNative = value;
            }
        }
        public GeObjectManager()
        {
            m_ObjectList = new LinkedList<GeObject>();
            m_ObjectCacheList = new LinkedList<NativeObjectCache>();

            if (mUseNative)
            {
                ITMGraphicManager graphicManger = ModuleManager.GetModule<ITMGraphicManager>();
                if (null != graphicManger)
                    m_NativeObjManager = graphicManger.NativeObjectManager;

            }
            else
            {
                m_NativeObjManager = null;
            }

            m_FrameCount = 0;
        }


        public GeObject CreateObject(Type objectType, string name, GeObjectParams objParams, string[] componentes = null)
        {
            GeObject obj = Utility.Assembly.CreateInstance(objectType) as GeObject;
            if (obj.Init(_AllocObjectID(), this, objParams))
            {
                if (obj.HasNative && mUseNative)
                    obj.OnNativeObjectCreated(NativeOperateState.OK, m_NativeObjManager.CreateNativeObject(name, componentes), "", ~0, "");
                else
                    obj.SetName(name);

                m_ObjectList.AddLast(obj);
                return obj;
            }
            else
                Debugger.LogWarning("Create object with type '{0}' has failed!", objectType);

            return null;
        }

        public TGeObject CreateObject<TGeObject>(string name, GeObjectParams objParams, string[] componentes = null) where TGeObject : GeObject, new()
        {
            TGeObject obj = new TGeObject();
            if (obj.Init(_AllocObjectID(), this, objParams))
            {
                if (obj.HasNative && mUseNative)
                    obj.OnNativeObjectCreated(NativeOperateState.OK, m_NativeObjManager.CreateNativeObject(name, componentes), "", ~0, "");
                else
                    obj.SetName(name);

                m_ObjectList.AddLast(obj);
                return obj;
            }
            else
                Debugger.LogWarning("Create object with type '{0}' has failed!", typeof(TGeObject));

            return null;

        }

        public TGeObject LoadObject<TGeObject>(string resPath, EnumHelper<ObjectLoadFlag> flags, GeObjectParams objParams, string[] componentes = null) where TGeObject : GeObject, new()
        {
            return _LoadObject<TGeObject>(resPath, false, Math.Transform.Identity, flags, objParams, componentes);
        }

        public TGeObject LoadObject<TGeObject>(string resPath, Math.Transform transform, EnumHelper<ObjectLoadFlag> flags, GeObjectParams objParams, string[] componentes = null) where TGeObject : GeObject, new()
        {
            return _LoadObject<TGeObject>(resPath, true, transform, flags, objParams, componentes);
        }

        public void ExtractNodesWithTag<TGeNode>(List<TGeNode> extractNodes, GeNode root, string tag, GeObjectParams objParams) where TGeNode : GeNode, new()
        {
            if (mUseNative)
            {
                _ExtractNodesWith(m_NativeObjManager.ExtractNativeObjectsByTag, extractNodes, root, tag, objParams);
            }
        }

        public void ExtractNodesWithName<TGeNode>(List<TGeNode> extractNodes, GeNode root, string name, GeObjectParams objParams) where TGeNode : GeNode, new()
        {
            if (mUseNative)
            {
                _ExtractNodesWith(m_NativeObjManager.ExtractNativeObjectsByName, extractNodes, root, name, objParams);
            }

        }

        public void ExtractNodesWithComponent<TGeNode, TComponent>(List<TGeNode> extractNodes, GeNode root, GeObjectParams objParams) where TGeNode : GeNode, new() where TComponent : ITMNativeComponent
        {
            if (mUseNative)
            {
                _ExtractNodesWith(m_NativeObjManager.ExtractNativeObjectsByComponent, extractNodes, root, typeof(TComponent), objParams);
            }
        }

        public void DestroyObject(GeObject node)
        {
            LinkedListNode<GeObject> cur = m_ObjectList.First;
            LinkedListNode<GeObject> next;
            while (null != cur)
            {
                next = cur.Next;
                if (node.ObjectID == cur.Value.ObjectID)
                {
                    cur.Value.Dispose();
                    m_ObjectList.Remove(cur);
                    return;
                }

                cur = next;
            }
        }

        public override int Priority
        {
            get
            {
                return 0;
            }
        }

        public override void Shutdown()
        {
            LinkedListNode<GeObject> cur = m_ObjectList.First;
            LinkedListNode<GeObject> next;
            while (null != cur)
            {
                next = cur.Next;
                cur.Value.Dispose();
                m_ObjectList.Remove(cur);

                cur = next;
            }
        }

        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            LinkedListNode<GeObject> cur = m_ObjectList.First;
            while (null != cur)
            {
                if (cur.Value.NeedUpdate)
                    cur.Value.Update(elapseSeconds, realElapseSeconds, m_FrameCount);
                cur = cur.Next;
            }
            ++m_FrameCount;
        }


        private TGeObject _LoadObject<TGeObject>(string resPath, bool overrideTransform, Math.Transform transform, EnumHelper<ObjectLoadFlag> flags, GeObjectParams objParams, string[] componentes = null) where TGeObject : GeObject, new()
        {
            TGeObject obj = new TGeObject();
            if (obj.Init(_AllocObjectID(), this, objParams))
            {
                if (obj.HasNative && mUseNative)
                {
                    if (!string.IsNullOrEmpty(resPath))
                    {
                        int requestID = overrideTransform ? m_NativeObjManager.LoadNativeNodeObject(resPath, transform, componentes, _OnNativeObjectLoaded, flags)
                            : m_NativeObjManager.LoadNativeNodeObject(resPath, componentes, _OnNativeObjectLoaded, flags);
                        m_ObjectCacheList.AddLast(new NativeObjectCache(obj, requestID));
                    }
                }
                m_ObjectList.AddLast(obj);

                return obj;
            }
            else
                Debugger.LogWarning("Create object with type '{0}' has failed!", typeof(TGeObject));

            return null;
        }

        protected void _OnNativeObjectLoaded(NativeOperateState state, ITMNativeObject nodeObj, string resPath, int requestID, string errorMsg)
        {
            LinkedListNode<NativeObjectCache> cur = m_ObjectCacheList.First;
            while (null != cur)
            {
                NativeObjectCache curObjCache = cur.Value;
                if (curObjCache.RequestID == requestID)
                {
                    if (!curObjCache.Object.HasDestroyed)
                        curObjCache.Object.OnNativeObjectCreated(state, nodeObj, resPath, requestID, errorMsg);
                    else
                    {
                        if (null != nodeObj)
                            nodeObj.Destroy();
                    }

                    m_ObjectCacheList.Remove(cur);
                    break;
                }
                cur = cur.Next;
            }
        }

        private object[] _AssembleObjectArgList(params object[] args)
        {
            List<object> argList = FrameStackList<object>.Acquire();
            argList.Add(m_NodeObjAllocCount++);
            argList.Add(this);
            argList.AddRange(args);
            object[] newArgs = argList.ToArray();
            FrameStackList<object>.Recycle(argList);

            return newArgs;
        }

        private void _ExtractNodesWith<TGeNode, T>(ExtractNativeObjectsFunc<T> extractAction, List<TGeNode> extractNodes, GeNode root, T value, GeObjectParams objParams)
            where TGeNode : GeNode, new()
        {
            if (null == extractAction)
            {
                Debugger.LogWarning("Invalid parameter:[extractAction] can not be null!");
                return;
            }

            if (null == root)
            {
                Debugger.LogWarning("Invalid parameter:[root] can not be null!");
                return;
            }

            List<ITMNativeObject> nativeNodeList = FrameStackList<ITMNativeObject>.Acquire();
            extractAction(root.Native, value, nativeNodeList);
            for (int i = 0, icnt = nativeNodeList.Count; i < icnt; ++i)
            {
                ITMNativeObject nativeNode = nativeNodeList[i];

                TGeNode node = new TGeNode();
                if (node.Init(_AllocObjectID(), this, objParams))
                {
                    node.OnNativeObjectCreated(NativeOperateState.OK, nativeNode, "", ~0, "");
                    m_ObjectList.AddLast(node);
                    extractNodes.Add(node);
                }
            }
        }

        private ulong _AllocObjectID()
        {
            return m_NodeObjAllocCount++;
        }
    }
}