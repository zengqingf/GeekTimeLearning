

namespace Tenmove.Runtime.EmbedUI
{
    internal class UIFormProgress : UIForm
    {
        protected IUISlider m_ProgressBar;
        protected IUIText m_ProgressInfo;
        protected IUIText m_ProgressValue;

        public UIFormProgress()
        {
            m_ProgressBar = null;
            m_ProgressInfo = null;
            m_ProgressValue = null;
        }

        public override bool NeedUpdate
        {
            get { return m_DirtyFlags != 0; }
        }

        public void SetProgress(float progress)
        {
            if (null != m_ProgressBar)
                m_ProgressBar.SetValue(progress);

            if (null != m_ProgressValue)
            {
                int value = Utility.Math.Clamp((int)((progress * 1000 + 5) / 10), 0, 100);
                m_ProgressValue.SetText(string.Format("{0}%", value));
            }
        }

        public void SetInfoMessage(string infoMsg)
        {
            if (null != m_ProgressInfo)
                m_ProgressInfo.SetText(infoMsg);
        }


        protected override void _InitWidgets()
        {
            m_ProgressBar = m_NativeUIForm.QureyUIControl<IUISlider>("Progress");
            m_ProgressInfo = m_NativeUIForm.QureyUIControl<IUIText>("Progress/ProgressInfo");
            m_ProgressValue = m_NativeUIForm.QureyUIControl<IUIText>("Progress/ProgressValue");
        }

        protected override void _DeinitWidgets()
        {
        }
    }
}
