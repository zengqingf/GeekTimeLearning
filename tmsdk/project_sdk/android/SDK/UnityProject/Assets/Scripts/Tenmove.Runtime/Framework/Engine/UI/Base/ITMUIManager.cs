

using System;

namespace Tenmove.Runtime.EmbedUI
{

    public enum UIFormLayer
    {
        None,
        Bottom,
        Middle,
        Top,
        TopMost,

        MaxLayer,
    }


    public interface ITMUIManager
    {
        TUIWidget LoadWidget<TUIWidget>(string widgetRes, string widgetName, bool loadFormPool, string[] components, UIWidgetParams desc) where TUIWidget : UIWidget,new();

        UIWidget CreateWidget(Type widgetType, string widgetName,UINode uiNode, ITMNativeUIComponent component);

        TUIWidget CreateWidget<TUIWidget>(string widgetName, UINode uiNode, ITMNativeUIComponent component) where TUIWidget : UIWidget, new();

        TUIForm CreateForm<TUIForm>(string formRes, UIFormParams uiFormParams) where TUIForm : UIForm, new();

        void DestroyForm(UIForm form);

        void EnableBackgroundClear(bool enable);
    }
}