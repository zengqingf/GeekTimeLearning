


using Tenmove.Runtime.EmbedUI;

namespace Tenmove.Runtime
{
    public abstract class UIObject : GeObject
    {
        protected ITMUIManager m_UIManager;

        protected UIObject()
        {
            m_UIManager = null;
        }
    }
}