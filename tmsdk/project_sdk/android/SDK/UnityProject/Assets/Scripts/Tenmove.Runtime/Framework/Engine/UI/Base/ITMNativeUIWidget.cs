


using System;

namespace Tenmove.Runtime.EmbedUI
{
    public interface ITMNativeUIWidget : ITMNativeUIComponent
    {
        Type WidgetType
        {
            get;
        }

        string NodePath
        {
            get;
        }
    }
}