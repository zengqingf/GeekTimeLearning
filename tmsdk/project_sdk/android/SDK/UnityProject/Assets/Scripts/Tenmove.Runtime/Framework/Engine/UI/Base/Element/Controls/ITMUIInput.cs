

using System;

namespace Tenmove.Runtime.EmbedUI
{
    public interface IUIInput : IUIControl
    {
        string Text
        {
            get;
        }

        void SetText(string text);
    }
}