using System;
using System.Collections.Generic;

namespace Tenmove.Runtime.EmbedUI
{
    public class UINodeParams : GeObjectParams
    {
        public ITMUIManager UIManager { set; get; }
    }

    public class UINode : GeNode
    {
        protected ITMUIManager m_UIManager;
        protected readonly List<UIWidget> m_UIWidgetList;

        public UINode()
        {
            //Debugger.Assert(null != uiManager, "UI manager can not be null!");

            m_UIManager = null;
            m_UIWidgetList = new List<UIWidget>();
        }

        protected override bool _OnInit(GeObjectParams objDesc)
        {
            UINodeParams uiNodeDesc = objDesc as UINodeParams;
            if (null != uiNodeDesc)
            {
                m_UIManager = uiNodeDesc.UIManager;
                return true;
            }
            else
                Debugger.LogWarning("Object desc type '{0}' is invalid! (Expected type:{1})", objDesc.GetType(), typeof(UINodeParams));

            return false;
        }

        protected override void _OnDeinit()
        {
            base._OnDeinit();
        }
    }
}