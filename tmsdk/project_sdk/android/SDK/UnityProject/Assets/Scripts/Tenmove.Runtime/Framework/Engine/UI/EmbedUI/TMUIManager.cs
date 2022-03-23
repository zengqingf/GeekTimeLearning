

using System;
using System.Collections.Generic;

namespace Tenmove.Runtime.EmbedUI
{
    internal class UIManager : BaseModule,ITMUIManager
    {
        private readonly ITMGeObjectManager m_ObjectManager;

        private readonly UINodeObjectKeeper<UIWidget> m_UIWidgetKeeper;
        private readonly UINodeObjectKeeper<UIForm> m_UIFormKeeper;

        private readonly UINode m_UIRoot;
        private readonly UINode[] m_UILayerRoot;
        private readonly string[] m_UILayerNameTable;

        private UINode m_BackgroundClear;

        public UIManager()
        {
            m_ObjectManager = ModuleManager.GetModule<ITMGeObjectManager>();

            m_UIWidgetKeeper = new UINodeObjectKeeper<UIWidget>(m_ObjectManager);
            m_UIFormKeeper = new UINodeObjectKeeper<UIForm>(m_ObjectManager);

            m_UIRoot = m_ObjectManager.LoadObject<UINode>("Base/UI/Prefabs/Root/EmbedUIRoot.prefab", Math.Transform.Identity, 0, new UINodeParams { UIManager = this });
            m_UIRoot.SetName("Tenmove.EmbedUI");
            m_UIRoot.OnObjectReady += _OnUIRootLoaded;

            m_UILayerRoot = new UINode[(int)UIFormLayer.MaxLayer];
            m_UILayerNameTable = new string[(int)UIFormLayer.MaxLayer] { "None","Bottom","Middle","Top","TopMost"};
            m_UILayerRoot[(int)UIFormLayer.None] = m_UIRoot;
        }

        public override int Priority
        {
            get
            {
                return 0;
            }
        }

        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        public override void Shutdown()
        {
            m_UIFormKeeper.ProceedAll(x => { x.OnObjectReady -= _AttachFormToUILayer; });
            m_UIFormKeeper.DestroyAll();

            if (null != m_UIRoot)
            {
                m_UIRoot.OnObjectReady -= _OnUIRootLoaded;
                m_ObjectManager.DestroyObject(m_UIRoot);
            }
            for (UIFormLayer i = UIFormLayer.None, jcnt = UIFormLayer.MaxLayer; i < jcnt; ++i)
                m_UILayerRoot[(int)i] = null;
        }

        public TUIWidget LoadWidget<TUIWidget>(string widgetRes, string widgetName, bool loadFormPool, string[] components, UIWidgetParams objParams) where TUIWidget : UIWidget,new()
        {
            TUIWidget newWidget = m_ObjectManager.LoadObject<TUIWidget>(widgetRes,Math.Transform.Identity, loadFormPool ? (uint)ObjectLoadFlag.LoadFromPool : 0, objParams, components);
            if (null != newWidget)
            {
                newWidget.SetName(widgetName);
                newWidget.Node.SetParent(m_UIRoot, false);

                return newWidget;
            }
            else
                Debugger.LogWarning("Load space node with path '{0}' has failed!", widgetRes);

            return null;
        }

        public UIWidget CreateWidget(Type widgetType, string widgetName, UINode uiNode, ITMNativeUIComponent component)
        {
            UIWidget newWidget = m_UIWidgetKeeper.CreateObject(widgetType, widgetName, GeObjectParams.Default) as UIWidget;
            if(null != newWidget)
            {
                newWidget.SetName(widgetName);
                newWidget.Node.SetParent(m_UIRoot, false);
                return newWidget;
            }
            else
                Debugger.LogWarning("Create widget with type '{0}' has failed!", widgetType);

            return null;
        }

        public TUIWidget CreateWidget<TUIWidget>(string widgetName, UINode uiNode, ITMNativeUIComponent component) where TUIWidget : UIWidget,new()
        {
            TUIWidget newWidget = m_UIWidgetKeeper.CreateObject<TUIWidget>(widgetName, GeObjectParams.Default);
            newWidget.Node.SetParent(m_UIRoot, false);
            return newWidget;
        }

        public TUIForm CreateForm<TUIForm>(string formRes, UIFormParams uiFormParams) where TUIForm : UIForm,new()
        {
            uiFormParams.UIManager = this;
            TUIForm newForm = m_UIFormKeeper.LoadObject<TUIForm>(formRes,typeof(TUIForm).Name,0, uiFormParams);
            newForm.OnObjectReady += _AttachFormToUILayer;
            return newForm;
        }

        public void DestroyForm(UIForm form)
        {
            if(null != form)
            {
                form.OnObjectReady -= _AttachFormToUILayer;
                m_UIFormKeeper.DestroyObject(form);
            }
        }

        public void EnableBackgroundClear(bool enable)
        {
            if (null != m_BackgroundClear)
                m_BackgroundClear.SetActive(enable);
        }

        private void _AttachFormToUILayer(bool isSuccess, ITMGeObject obj)
        {
            if (isSuccess && null != obj)
            {
                UIForm form = obj as UIForm;
                form.SetParent(m_UILayerRoot[(int)form.UIFormLayer], false);
            }
        }

        private void _OnUIRootLoaded(bool isSuccess, ITMGeObject obj)
        {
            if (isSuccess && null != obj)
            {
                UINode root = obj as UINode;

                UINodeParams nodeParams = new UINodeParams() { UIManager = this };
                List<UINode> uiNodes = FrameStackList<UINode>.Acquire();
                m_ObjectManager.ExtractNodesWithComponent<UINode, ITMNativeNodeUI>(uiNodes, root, nodeParams);
                for(int i = 0,icnt = uiNodes.Count;i<icnt;++i)
                {
                    UINode curNode = uiNodes[i];
                    for (UIFormLayer j = UIFormLayer.Bottom, jcnt = UIFormLayer.MaxLayer; j < jcnt; ++j)
                    {
                        if (m_UILayerNameTable[(int)j].Equals(curNode.Name,System.StringComparison.OrdinalIgnoreCase))
                        {
                            m_UILayerRoot[(int)j] = curNode;
                            break;
                        }
                    }
                }

                uiNodes.Clear();

                m_ObjectManager.ExtractNodesWithName<UINode>(uiNodes,root, "UICamera", nodeParams);
                if (uiNodes.Count > 0)
                    m_BackgroundClear = uiNodes[0];
                FrameStackList<UINode>.Recycle(uiNodes);
            }
        }
    }
}