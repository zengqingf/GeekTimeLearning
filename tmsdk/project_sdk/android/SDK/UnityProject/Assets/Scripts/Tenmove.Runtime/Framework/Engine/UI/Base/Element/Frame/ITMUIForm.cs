using System;

namespace Tenmove.Runtime.EmbedUI
{
    public enum FrameEvent
    {
        OnOpen,
        OnClose,
    }

    public delegate void OnUIFormReady(IUIForm form);

    public interface IUIForm
    {
        event OnUIFormReady OnFormReady;

        int MutexGroupID
        {
            get;
        }

        bool IsGlobalFrame
        {
            get;
        }

        bool IsPopUpFrame
        {
            get;
        }

        void Hide();
        void Show();
        
        void RecycleAsset();

        bool RegisterEventHandler(ITMUIEventHandler eventHandler);
        bool DeregisterEventHandler(ITMUIEventHandler eventHandler);
        

        void SetAudio(FrameEvent eventType, string audioRes);

        TUIWidget GetWidget<TUIWidget>(string widgetPath) where TUIWidget : class,IUIWidget;

        void Close();
    }
}