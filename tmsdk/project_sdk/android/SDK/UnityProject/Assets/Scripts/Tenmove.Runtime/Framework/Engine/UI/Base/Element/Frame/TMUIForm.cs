using System;
using System.Collections.Generic;
using Tenmove.Runtime;

namespace Tenmove.Runtime.EmbedUI
{
    public class UIFormParams : UINodeParams
    {
        public UIFormLayer UILayer { set; get; }
    }


    public abstract class UIForm : UINode, IUIForm
    {
        protected readonly int m_MutexGroupID;
        protected readonly bool m_IsGlobalFrame;
        protected readonly bool m_IsPopUpFrame;
        
        protected ITMNativeUIForm m_NativeUIForm;
        private bool m_IsFormReady;

        private UIFormLayer m_UIFormLayer;
        private event OnUIFormReady m_OnFormReady;

        /// <summary>
        /// key:控件路径
        /// value:控件对象
        /// </summary>
        protected readonly Dictionary<string,List<UIWidget>> m_UIWidgetTable;

        public UIForm()
        {
            m_UIWidgetTable = new Dictionary<string, List<UIWidget>>();
            m_IsFormReady = false;
        }

        public event OnUIFormReady OnFormReady
        {
            add { m_OnFormReady += value; }
            remove { m_OnFormReady -= value; }
        }

        public int MutexGroupID
        {
            get { return m_MutexGroupID; }
        }

        public bool IsGlobalFrame
        {
            get { return m_IsGlobalFrame; }
        }

        public bool IsPopUpFrame
        {
            get { return m_IsPopUpFrame; }
        }

        public UIFormLayer UIFormLayer
        {
            get { return m_UIFormLayer; }
        }

        public bool IsFormReady
        {
            get { return m_IsFormReady; }
        }

        protected virtual void OnHide()
        {
        }

        protected virtual void OnDisplay()
        {
        }

        protected virtual void OnOpen()
        {
        }

        protected virtual void OnClose()
        {
        }

        protected virtual void OnLostFocus()
        {
        }

        protected virtual void OnGetFocus()
        {
        }

        protected virtual void Update(float logicDelta, float realDelta)
        {
        }

        protected abstract void _InitWidgets();
        protected abstract void _DeinitWidgets();

        protected override bool _OnInit(GeObjectParams objDesc)
        {
            if(base._OnInit(objDesc))
            {
                UIFormParams formParams = objDesc as UIFormParams;
                if (null != formParams)
                {
                    m_UIFormLayer = formParams.UILayer;
                    return true;
                }
            }

            return false;
        }

        protected override void _OnDeinit()
        {
            _DeinitWidgets();
            base._OnDeinit();
        }

        public void Hide()
        {
            SetActive(false);
        }

        public void Show()
        {
            SetActive(true);
        }

        public void RecycleAsset()
        {
        }

        public bool RegisterEventHandler(ITMUIEventHandler eventHandler)
        {
            return false;
        }

        public bool DeregisterEventHandler(ITMUIEventHandler eventHandler)
        {
            return false;
        }

        public void SetAudio(FrameEvent eventType, string audioRes)
        {

        }

        public TUIWidget GetWidget<TUIWidget>(string widgetPath) where TUIWidget : class, IUIWidget
        {
            return _GetNodeWidget<TUIWidget>(widgetPath);
        }

        public void Close()
        {

        }

        protected sealed override void _OnNativeObjectCreated(ITMNativeObject nativeObject)
        {
            base._OnNativeObjectCreated(nativeObject);
            
            m_NativeUIForm = nativeObject.QureyComponent<ITMNativeUIForm>();
            if (null != m_NativeUIForm)
            {
                /// 抽取该窗口的所有界面控件(IUIComponent)
                List<UINode> uiNodes = FrameStackList<UINode>.Acquire();
                m_ObjectManager.ExtractNodesWithComponent<UINode, ITMNativeUIWidget>(uiNodes, this, GeObjectParams.Default);

                List<ITMNativeUIWidget> uiComponents = FrameStackList<ITMNativeUIWidget>.Acquire();
                for (int i = 0 ,icnt = uiNodes.Count; i < icnt;++i)
                {
                    UINode curUINode = uiNodes[i];
                    uiComponents.Clear();
                    curUINode.Native.QureyAllComponents(uiComponents);
                    for(int j = 0,jcnt = uiComponents.Count;j<jcnt;++j)
                    {
                        ITMNativeUIWidget curUIComponent = uiComponents[j];
                        UIWidget newWidget = m_UIManager.CreateWidget(curUIComponent.WidgetType , curUIComponent.Name, curUINode, curUIComponent);
                        _AddNodeWidget(curUIComponent.NodePath, newWidget);
                    }
                }
                FrameStackList<ITMNativeUIWidget>.Recycle(uiComponents);
                FrameStackList<UINode>.Recycle(uiNodes);

                _InitWidgets();

                m_IsFormReady = true;

                if (null != m_OnFormReady)
                    m_OnFormReady(this);
            }
        }

        private void _AddNodeWidget(string nodePath,UIWidget widget)
        {
            List<UIWidget> widgets = null;
            if(m_UIWidgetTable.TryGetValue(nodePath,out widgets))
            {
                for(int i = 0,icnt = widgets.Count;i<icnt;++i)
                {
                    if (widget.ObjectID == widgets[i].ObjectID)
                        return;
                }

                widgets.Add(widget);
                return;
            }

            widgets = new List<UIWidget>();
            widgets.Add(widget);
            m_UIWidgetTable.Add(nodePath, widgets);
        }

        private TUIWidget _GetNodeWidget<TUIWidget>(string nodePath) where TUIWidget: class,IUIWidget
        {
            Type dstWidgetType = typeof(TUIWidget);
            List<UIWidget> widgets = null;
            if (m_UIWidgetTable.TryGetValue(nodePath, out widgets))
            {
                for (int i = 0, icnt = widgets.Count; i < icnt; ++i)
                {
                    UIWidget curWidget = widgets[i];
                    if (curWidget.GetType() == dstWidgetType)
                        return curWidget as TUIWidget;
                }
            }

            return null;
        }
    }
}