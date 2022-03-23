
namespace Tenmove.Runtime.EmbedUI
{
    public interface IUIButton : IUIControl
    {
        event UIAction OnClick;
    }
}