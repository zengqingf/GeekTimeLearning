


using System;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    public abstract class GeObjectKeeper<TGeObject> where TGeObject : GeObject
    {
        public delegate void ProceedFunc(TGeObject obj);
        public delegate void ProceedFunc<T>(TGeObject obj, T param);
        public delegate void ProceedFunc<T0, T1>(TGeObject obj, T0 param0, T1 param1);

        public delegate bool MatchFunc<T>(TGeObject obj, T value);

        private readonly List<TGeObject> m_Objestes;
        protected readonly ITMGeObjectManager m_ObjectManager;

        protected GeObjectKeeper(ITMGeObjectManager objectManager)
        {
            Debugger.Assert(null != objectManager, "Object manager can not be null!");

            m_ObjectManager = objectManager;
            m_Objestes = new List<TGeObject>();
        }

        public TGeObject CreateObject(Type objectType,string objName, GeObjectParams objParams, string[] componentes = null)
        {
            _PurgeObjectes(m_Objestes);

            TGeObject newObject = _CreateObject(objectType, objName, objParams, componentes);
            m_Objestes.Add(newObject);
            return newObject;
        }

        public T CreateObject<T>(string objName, GeObjectParams objParams, string[] componentes = null) where T : TGeObject,new()
        {
            _PurgeObjectes(m_Objestes);

            T newObject = _CreateObject<T>(objName, objParams, componentes);
            m_Objestes.Add(newObject);
            return newObject;
        }

        public T LoadObject<T>(string resPath, string objName, EnumHelper<ObjectLoadFlag> loadFlags, GeObjectParams objParams, string[] componentes = null) where T : GeNode, TGeObject, new()
        {
            _PurgeObjectes(m_Objestes);

            T newNode = _LoadObject<T>(resPath, objName, loadFlags, objParams, componentes);
            m_Objestes.Add(newNode);
            return newNode;
        }

        public T LoadObject<T>(string resPath, string objName, Math.Transform transform, EnumHelper<ObjectLoadFlag> loadFlags, GeObjectParams objParams, string[] componentes = null) where T : GeNode, TGeObject, new()
        {
            _PurgeObjectes(m_Objestes);
        
            T newNode = _LoadObject<T>(resPath, objName, transform, loadFlags, objParams, componentes);
            m_Objestes.Add(newNode);
            return newNode;
        }

        public void DestroyObject(TGeObject effect)
        {
            _DestroyObject(m_Objestes, effect);
        }

        public void DestroyAll()
        {
            _DestroyAllObjectes(m_Objestes);
        }

        public void ProceedAll(ProceedFunc action)
        {
            if (null == action)
                return;

            _ProceedAll(m_Objestes, action);
        }

        public void ProceedAll<T>(ProceedFunc<T> action, T param)
        {
            if (null == action)
                return;

            _ProceedAll(m_Objestes, action, param);
        }

        public void ProceedAll<T0, T1>(ProceedFunc<T0, T1> action, T0 param0, T1 param1)
        {
            if (null == action)
                return;

            _ProceedAll(m_Objestes, action, param0, param1);
        }

        public TGeObject FindFirst<T>(MatchFunc<T> func, T key)
        {
            return _FindFirst(m_Objestes, func, key);
        }

        protected abstract TGeObject _CreateObject(Type objectType,string objName, GeObjectParams desc, string[] componentes);
        protected abstract T _CreateObject<T>(string objName, GeObjectParams desc, string[] componentes) where T : TGeObject, new();
        protected abstract T _LoadObject<T>(string resPath, string objName, EnumHelper<ObjectLoadFlag> loadFlags, GeObjectParams desc, string[] componentes) where T : GeNode, TGeObject, new();
        protected abstract T _LoadObject<T>(string resPath, string objName, Math.Transform transform, EnumHelper<ObjectLoadFlag> loadFlags, GeObjectParams desc, string[] componentes) where T : GeNode,TGeObject, new();

        protected void _DestroyObject(List<TGeObject> objects, TGeObject destroyNode)
        {
            for (int i = 0, icnt = objects.Count; i < icnt; ++i)
            {
                TGeObject curObj = objects[i];
                if (destroyNode.ObjectID == curObj.ObjectID)
                {
                    curObj.Destroy();
                    return;
                }
            }
        }

        protected void _PurgeObjectes(List<TGeObject> objects)
        {
            objects.RemoveAll(_ObjectPurgePredicate);
        }

        private bool _ObjectPurgePredicate(TGeObject node)
        {
            return node.HasDestroyed;
        }

        protected void _DestroyAllObjectes(List<TGeObject> objects)
        {
            for (int i = 0, icnt = objects.Count; i < icnt; ++i)
            {
                TGeObject curObj = objects[i];
                if (null != curObj)
                    curObj.Destroy();
            }

            objects.Clear();
        }

        protected void _ProceedAll(List<TGeObject> objects, ProceedFunc action)
        {
            for (int i = 0, icnt = objects.Count; i < icnt; ++i)
            {
                TGeObject curObj = objects[i];
                if (null != curObj)
                    action(curObj);
            }
        }

        protected void _ProceedAll<T>(List<TGeObject> nodes, ProceedFunc<T> action, T param)
        {
            for (int i = 0, icnt = nodes.Count; i < icnt; ++i)
            {
                TGeObject curNode = nodes[i];
                if (null != curNode)
                    action(curNode, param);
            }
        }

        protected void _ProceedAll<T0, T1>(List<TGeObject> nodes, ProceedFunc<T0, T1> action, T0 param0, T1 param1)
        {
            for (int i = 0, icnt = nodes.Count; i < icnt; ++i)
            {
                TGeObject curNode = nodes[i];
                if (null != curNode)
                    action(curNode, param0, param1);
            }
        }

        protected TGeObject _FindFirst<T>(List<TGeObject> nodes, MatchFunc<T> action, T value)
        {
            for (int i = 0, icnt = nodes.Count; i < icnt; ++i)
            {
                TGeObject curNode = nodes[i];
                if (null != curNode)
                    if (action(curNode, value))
                        return curNode;
            }

            return null;
        }
    }
}