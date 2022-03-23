


using System;

namespace Tenmove.Runtime
{
    public class SpaceNodeObjectKeeper<TGeObject> : GeObjectKeeper<TGeObject> where TGeObject : GeObject
    {
        public SpaceNodeObjectKeeper(ITMGeObjectManager objectManager)
            : base(objectManager)
        {
        }

        protected override TGeObject _CreateObject(Type objectType, string objName, GeObjectParams objParams, string[] componentes)
        {
            TGeObject newObject = m_ObjectManager.CreateObject(objectType, objName, objParams, componentes) as TGeObject;
            return newObject;
        }

        protected override T _CreateObject<T>(string objName, GeObjectParams objParams, string[] componentes)
        {
            T newObject = m_ObjectManager.CreateObject<T>(objName, objParams, componentes);
            return newObject;
        }

        protected override T _LoadObject<T>(string resPath, string objName, EnumHelper<ObjectLoadFlag> loadFlags, GeObjectParams objParams, string[] componentes)
        {
            T newNode = m_ObjectManager.LoadObject<T>(resPath, loadFlags, objParams, componentes);
            newNode.SetName(objName);
            return newNode;
        }

        protected override T _LoadObject<T>(string resPath, string objName, Math.Transform transform, EnumHelper<ObjectLoadFlag> loadFlags, GeObjectParams objParams, string[] componentes)
        {
            T newNode = m_ObjectManager.LoadObject<T>(resPath ,transform, loadFlags, objParams, componentes);
            newNode.SetName(objName);
            return newNode;
        }
    }
}