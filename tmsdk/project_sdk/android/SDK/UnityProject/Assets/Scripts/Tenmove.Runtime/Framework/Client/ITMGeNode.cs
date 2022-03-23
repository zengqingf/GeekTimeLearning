

using System;
using System.Collections.Generic;
using Tenmove.Runtime.Math;

namespace Tenmove.Runtime
{
    public delegate void OnAttachToParent(GeNode node, GeNode parent);
    public delegate void OnDetachFromParent(GeNode node, GeNode parent);

    public interface ITMGeNode : ITMGeObject
    {
        event OnAttachToParent OnAttachToParent;
        event OnDetachFromParent OnDetachFromParent;

        string Name
        {
            get;
        }

        ITMGeNode Parent
        {
            get;
        }

        void SetActive(bool isActive);
        void SetName(string name);
        void SetParent(ITMGeNode node,bool keepTransform);

        int GetChildCount();
        ITMGeNode GetChildByIndex(int index);
    }
}