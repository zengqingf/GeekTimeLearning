

namespace Tenmove.Runtime.EmbedUI
{
    public enum UIEvent
    {
        Unknown,
        OnClick,
        OnValueChanged,
        OnDrag,
    }

    public abstract class UIEventArgs : BaseEventArgs
    {
    }
   
    public class UIClickEventArgs : UIEventArgs
    { }
}