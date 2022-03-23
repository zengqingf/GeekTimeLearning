

using System;
using Tenmove.Runtime.Math;

namespace Tenmove.Runtime
{

    public delegate void OnFlushParent(ITMNativeNode parent, bool keepTransform);
    public delegate void OnRootNodeChanged(ITMNativeNode root);

    public interface ITMNativeNode : ITMNativeComponent
    {
        event OnFlushParent OnFlushParent;
        event OnRootNodeChanged OnRootNodeChanged;
        void FlushParent(ITMNativeNode parent, bool keepTransform);
    }
}

