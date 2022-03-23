

using System.Collections.Generic;
using Tenmove.Runtime.EmbedUI;

namespace Tenmove.Runtime
{
    public class UIWidgetParams : GeObjectParams
    {
        public UIWidgetParams(ITMUIManager uiManager, ITMNativeUIComponent component, UINode node)
        {
            UIManager = uiManager;
            Component = component;
            Node = node;
        }

        public ITMUIManager UIManager
        {
            private set;
            get;
        }

        public ITMNativeUIComponent Component
        {
            private set;
            get;
        }

        public UINode Node
        {
            private set;
            get;
        }
    }

    public class UIWidget : UIObject, IUIWidget
    {
        //public abstract void RegisterEventHandler<TUIEventHandler>(TUIEventHandler handler);

        protected readonly Dictionary<int, string> m_EvnetAudioMap;
        protected UINode m_UINode;

        public UIWidget()
        {            
            m_EvnetAudioMap = new Dictionary<int, string>();
        }

        public UINode Node
        {
            get { return m_UINode; }
        }

        public override bool HasNative
        {
            get { return true; }
        }

        public override bool NeedUpdate
        {
            get { return false; }
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }

        public void SetGray(bool isGray)
        {

        }

        protected sealed override bool _OnInit(GeObjectParams objParams)
        {
            UIWidgetParams widgetDesc = objParams as UIWidgetParams;
            if(null != widgetDesc)
            {
                Debugger.Assert(null != widgetDesc.Node, "UI node can not be null!");
                m_UINode = widgetDesc.Node;
                return true;
            }
            else
                Debugger.LogWarning("Object desc type '{0}' is invalid! (Expected type:{1})", objParams.GetType(), typeof(UIWidgetParams));

            return false;
        }

        protected override void _OnDeinit()
        {
        }

        protected void _SetAudio(int eventType,string audioRes)
        {
            string audioResValue = null;
            if(m_EvnetAudioMap.TryGetValue(eventType,out audioResValue))
            {
                m_EvnetAudioMap[eventType] = audioRes;
                return;
            }

            m_EvnetAudioMap.Add(eventType, audioRes);
        }

    }   
}