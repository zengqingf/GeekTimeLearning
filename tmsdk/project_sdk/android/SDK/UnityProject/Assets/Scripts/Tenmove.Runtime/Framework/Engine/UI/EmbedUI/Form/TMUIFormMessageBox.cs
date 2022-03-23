using Tenmove.Runtime.Graphic;


namespace Tenmove.Runtime.EmbedUI
{
    public delegate void OnClickHandled();

    public class UIFormMessageBox : UIForm
    {
        private IUIText m_TitleText;
        private IUIText m_MessageText;

        private IUIText m_Btn0Text;
        private IUIText m_Btn1Text;
        private IUIText m_Btn2Text;
        private IUIButton m_Btn0;
        private IUIButton m_Btn1;
        private IUIButton m_Btn2;
        private IUIImage m_Btn0Image;
        private IUIImage m_Btn1Image;
        private IUIImage m_Btn2Image;

        private MessageBoxCallback m_Btn0Callback;
        private MessageBoxCallback m_Btn1Callback;
        private MessageBoxCallback m_Btn2Callback;

        private event OnClickHandled m_OnClickHandled;

        private MessageBox.Params m_MessageBoxParams;

        public UIFormMessageBox()
        {
            m_TitleText = null;
            m_MessageText = null;
            m_Btn0Text = null;
            m_Btn1Text = null;
            m_Btn2Text = null;
            m_Btn0 = null;
            m_Btn1 = null;
            m_Btn2 = null;
            m_Btn0Image = null;
            m_Btn1Image = null;
            m_Btn2Image = null;
            m_Btn0Callback = null;
            m_Btn1Callback = null;
            m_Btn2Callback = null;
        }

        public event OnClickHandled OnClickHandled
        {
            add { m_OnClickHandled += value; }
            remove { m_OnClickHandled -= value; }
        }

        public void Bind(MessageBox.Params @params)
        {
            m_MessageBoxParams = @params;

            if(null != m_NativeUIForm)
                _InitBindParams(m_MessageBoxParams);
        }

        public void Unbind()
        {
            m_Btn0Callback = null;
            m_Btn1Callback = null;
            m_Btn2Callback = null;
        }

        protected void _OnBtn0Click(int actionID)
        {
            if (null != m_Btn0Callback)
                m_Btn0Callback();

            if (null != m_OnClickHandled)
                m_OnClickHandled();
        }

        protected void _OnBtn1Click(int actionID)
        {
            if (null != m_Btn1Callback)
                m_Btn1Callback();

            if (null != m_OnClickHandled)
                m_OnClickHandled();
        }

        protected void _OnBtn2Click(int actionID)
        {
            if (null != m_Btn2Callback)
                m_Btn2Callback();

            if (null != m_OnClickHandled)
                m_OnClickHandled();
        }

        protected sealed override void _InitWidgets()
        {
            m_TitleText = m_NativeUIForm.QureyUIControl<IUIText>("Dialog/Content/TitleBar/TitleText");
            m_MessageText = m_NativeUIForm.QureyUIControl<IUIText>("Dialog/Content/MessageContent/MessageText");

            m_Btn0Text = m_NativeUIForm.QureyUIControl<IUIText>("Dialog/Content/ButtonRect/Btn0/Text");
            m_Btn1Text = m_NativeUIForm.QureyUIControl<IUIText>("Dialog/Content/ButtonRect/Btn1/Text");
            m_Btn2Text = m_NativeUIForm.QureyUIControl<IUIText>("Dialog/Content/ButtonRect/Btn2/Text");

            m_Btn0 = m_NativeUIForm.QureyUIControl<IUIButton>("Dialog/Content/ButtonRect/Btn0");
            m_Btn1 = m_NativeUIForm.QureyUIControl<IUIButton>("Dialog/Content/ButtonRect/Btn1");
            m_Btn2 = m_NativeUIForm.QureyUIControl<IUIButton>("Dialog/Content/ButtonRect/Btn2");

            m_Btn0Image = m_NativeUIForm.QureyUIControl<IUIImage>("Dialog/Content/ButtonRect/Btn0");
            m_Btn1Image = m_NativeUIForm.QureyUIControl<IUIImage>("Dialog/Content/ButtonRect/Btn1");
            m_Btn2Image = m_NativeUIForm.QureyUIControl<IUIImage>("Dialog/Content/ButtonRect/Btn2");

            if (null != m_Btn0)
                m_Btn0.OnClick += _OnBtn0Click;
            if (null != m_Btn1)
                m_Btn1.OnClick += _OnBtn1Click;
            if (null != m_Btn2)
                m_Btn2.OnClick += _OnBtn2Click;

            _InitBindParams(m_MessageBoxParams);
        }

        protected override void _DeinitWidgets()
        {
            if (null != m_Btn0)
                m_Btn0.OnClick -= _OnBtn0Click;
            if (null != m_Btn1)
                m_Btn1.OnClick -= _OnBtn1Click;
            if (null != m_Btn2)
                m_Btn2.OnClick -= _OnBtn2Click;
        }

        protected void _InitBindParams(MessageBox.Params @params)
        {
            m_Btn0Callback = @params.OnBtn0Callback;
            if (null != m_Btn0Callback)
            {
                m_Btn0.SetVisible(true);

                if (RGBA.Black != @params.Btn0Color && null != m_Btn0Image)
                    m_Btn0Image.SetColor(@params.Btn0Color);

                if (!string.IsNullOrEmpty(@params.Btn0Text) && null != m_Btn0Text)
                    m_Btn0Text.SetText(@params.Btn0Text);
            }
            else
                m_Btn0.SetVisible(false);

            m_Btn1Callback = @params.OnBtn1Callback;
            if (null != m_Btn1Callback)
            {
                m_Btn1.SetVisible(true);

                if (RGBA.Black != @params.Btn1Color && null != m_Btn1Image)
                    m_Btn1Image.SetColor(@params.Btn1Color);

                if (!string.IsNullOrEmpty(@params.Btn1Text) && null != m_Btn1Text)
                    m_Btn1Text.SetText(@params.Btn1Text);
            }
            else
                m_Btn1.SetVisible(false);

            m_Btn2Callback = @params.OnBtn2Callback;
            if (null != m_Btn2Callback)
            {
                m_Btn2.SetVisible(true);

                if (RGBA.Black != @params.Btn2Color && null != m_Btn2Image)
                    m_Btn2Image.SetColor(@params.Btn2Color);

                if (!string.IsNullOrEmpty(@params.Btn2Text) && null != m_Btn2Text)
                    m_Btn2Text.SetText(@params.Btn2Text);
            }
            else
                m_Btn2.SetVisible(false);

            if (null != m_TitleText)
                m_TitleText.SetText(@params.Title);
            if (null != m_MessageText)
                m_MessageText.SetText(@params.Message);
        }
    }
}
