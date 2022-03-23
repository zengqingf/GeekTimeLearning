


using System;

namespace Tenmove.Runtime.EmbedUI
{
    public interface ITMNativeUIForm : ITMNativeUIComponent
    {
        TUIControl QureyUIControl<TUIControl>(string path) where TUIControl : class,IUIControl;
    }
}