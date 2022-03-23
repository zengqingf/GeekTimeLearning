

using System;
using Tenmove.Runtime.Graphic;

namespace Tenmove.Runtime.EmbedUI
{
    public interface IUIImage : IUIControl
    {
        void SetImage(string imageRes,bool asyncLoad);
        void SetColor(RGBA color);
    }
}