

using System;
using System.Collections.Generic;
using Tenmove.Runtime.Math;

namespace Tenmove.Runtime
{
    public interface ITMNativeObject
    {
        string Name
        {
            get;
        }

        long ObjectID
        {
            get;
        }

        void FlushName(string name);

        void DispatchCommand(ITMNativeCommand cmd);

        ITMNativeComponent CreateComponent(string componentTypeName);
        void DestroyComponent(ITMNativeComponent component);

        ITMNativeComponent QureyComponent(string componentTypeName);
        TNativeComponent QureyComponent<TNativeComponent>() where TNativeComponent:class,ITMNativeComponent;
        void QureyAllComponents<TNativeComponent>(List<TNativeComponent> components) where TNativeComponent : class, ITMNativeComponent;

        void QureyComponentsWithInterface<I>(List<I> components) where I : class;

        void SetActive(bool isActive);
        void Destroy();
    }
}

