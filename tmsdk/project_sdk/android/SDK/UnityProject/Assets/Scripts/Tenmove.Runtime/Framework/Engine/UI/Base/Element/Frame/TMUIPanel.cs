using System;

namespace Tenmove.Runtime.EmbedUI
{
    internal abstract class UIPanel
    {
        //public abstract void OnHide();
        //public abstract void OnDisplay();

        public abstract void Hide();
        public abstract void Show();

        public abstract bool RegisterEventHandler(ITMUIEventHandler eventHandler);
        public abstract bool DeregisterEventHandler(ITMUIEventHandler eventHandler);

        public abstract void Update(float logicDelta, float realDelta);

        public abstract TUIWidget GetWidget<TUIWidget>(string widgetPath) where TUIWidget : IUIObject;
    }
}