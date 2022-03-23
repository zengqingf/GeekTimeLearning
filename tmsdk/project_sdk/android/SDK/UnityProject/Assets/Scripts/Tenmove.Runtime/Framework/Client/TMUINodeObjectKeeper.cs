


using System;

namespace Tenmove.Runtime.EmbedUI
{
    public class UINodeObjectKeeper<TGeObject> : GeObjectKeeper<TGeObject> where TGeObject : GeObject
    {
        public UINodeObjectKeeper(ITMGeObjectManager objectManager)
            : base(objectManager)
        {
        }

        protected override TGeObject _CreateObject(Type objectType, string objName, GeObjectParams desc, string[] componentes)
        {
            TGeObject newObject = m_ObjectManager.CreateObject(objectType, objName, desc,componentes) as TGeObject;
            return newObject;
        }

        protected override T _CreateObject<T>(string objName, GeObjectParams desc, string[] componentes)
        {
            T newObject = m_ObjectManager.CreateObject<T>(objName, desc,componentes);
            return newObject;
        }

        protected override T _LoadObject<T>(string resPath, string objName, EnumHelper<ObjectLoadFlag> loadFlags, GeObjectParams desc, string[] componentes)
        {
            T newNode = m_ObjectManager.LoadObject<T>(resPath, loadFlags, desc, componentes);
            newNode.SetName(objName);
            return newNode;
        }

        protected override T _LoadObject<T>(string resPath, string objName, Math.Transform transform, EnumHelper<ObjectLoadFlag> loadFlags, GeObjectParams desc, string[] componentes)
        {
            T newNode = m_ObjectManager.LoadObject<T>(resPath, transform, loadFlags, desc, componentes);
            newNode.SetName(objName);
            return newNode;
        }
    }
}