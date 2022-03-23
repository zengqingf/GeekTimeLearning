

using System;

namespace Tenmove.Runtime.EmbedUI
{
    public interface IUIToggle : IUIControl
    {
        bool IsOn
        {
            get;
        }

        int ToggleGroupID
        {
            get;
        }

        event UIAction<bool> OnValueChanged;
    }
}