


using Tenmove.Runtime.Graphic;

namespace Tenmove.Runtime.EmbedUI
{
    public partial class MessageBox
    {
        public struct Params
        {
            private MessageBoxType m_Type;
            private string m_Title;
            private string m_Message;
            private bool m_IsModal;
            private string m_Btn0Text;
            private string m_Btn1Text;
            private string m_Btn2Text;
            private MessageBoxCallback m_OnBtn0Callback;
            private MessageBoxCallback m_OnBtn1Callback;
            private MessageBoxCallback m_OnBtn2Callback;
            private RGBA m_Btn0Color;
            private RGBA m_Btn1Color;
            private RGBA m_Btn2Color;

            public static Params OK(string title, string message, MessageBoxCallback onOKCallback, bool isModal = true)
            {
                return _CreateMessageBox(MessageBoxType.OK, title, message, isModal,"确定", string.Empty, string.Empty, onOKCallback, null, null, RGBA.Black, RGBA.Black, RGBA.Black);
            }

            public static Params OK(string title, string message, string btnOKText, MessageBoxCallback onOKCallback, RGBA okColor, bool isModal = true)
            {
                return _CreateMessageBox(MessageBoxType.OK, title, message, isModal, btnOKText, string.Empty, string.Empty, onOKCallback, null, null, okColor, RGBA.Black, RGBA.Black);
            }

            public static Params OKCancel(string title, string message, MessageBoxCallback onOK, MessageBoxCallback onCancel, bool isModal = true)
            {
                return _CreateMessageBox(MessageBoxType.OKCancel, title, message, isModal, "确定", "取消", string.Empty, onOK, onCancel, null, RGBA.Black, RGBA.Black, RGBA.Black);
            }

            public static Params OKCancel(string title, string message, string btnOKText, string btnCancelText, MessageBoxCallback onOK, MessageBoxCallback onCancel, RGBA okColor, RGBA cancelColor, bool isModal = true)
            {
                return _CreateMessageBox(MessageBoxType.OKCancel, title, message, isModal, btnOKText, btnCancelText, string.Empty, onOK, onCancel, null, okColor , cancelColor, RGBA.Black);
            }

            public static Params CancelOK(string title, string message, MessageBoxCallback onCancel, MessageBoxCallback onOK, bool isModal = true)
            {
                return _CreateMessageBox(MessageBoxType.CancelOK, title, message, isModal, "取消", "确定", string.Empty, onCancel, onOK, null, RGBA.Black, RGBA.Black, RGBA.Black);
            }

            public static Params CancelOK(string title, string message, string btnCancelText, string btnOKText, MessageBoxCallback onCancel, MessageBoxCallback onOK, RGBA cancelColor, RGBA okColor, bool isModal = true)
            {
                return _CreateMessageBox(MessageBoxType.CancelOK, title, message, isModal, btnCancelText, btnOKText, string.Empty, onCancel, onOK, null, cancelColor, okColor,RGBA.Black);
            }

            public static Params Custom(string title, string message, string btn0Text, string btn1Text, string btn2Text, MessageBoxCallback onBtn0Callback, MessageBoxCallback onBtn1Callback, MessageBoxCallback onBtn2Callback, bool isModal = true)
            {
                return _CreateMessageBox(MessageBoxType.Custom, title, message, isModal, btn0Text, btn1Text, btn2Text, onBtn0Callback, onBtn1Callback, onBtn2Callback, RGBA.Black, RGBA.Black, RGBA.Black);
            }

            public static Params Custom(string title, string message, string btn0Text, string btn1Text, string btn2Text, MessageBoxCallback onBtn0Callback, MessageBoxCallback onBtn1Callback, MessageBoxCallback onBtn2Callback, Graphic.RGBA btn0Color, Graphic.RGBA btn1Color, Graphic.RGBA btn2Color, bool isModal = true)
            {
                return _CreateMessageBox(MessageBoxType.Custom,  title,  message,isModal,  btn0Text,  btn1Text,  btn2Text,  onBtn0Callback,  onBtn1Callback,  onBtn2Callback, btn0Color,  btn1Color, btn2Color);
            }

            private static Params _CreateMessageBox(MessageBoxType type, string title, string message, bool isModal, string btn0Text, string btn1Text, string btn2Text, MessageBoxCallback onBtn0Callback, MessageBoxCallback onBtn1Callback, MessageBoxCallback onBtn2Callback,RGBA btn0Color, RGBA btn1Color, RGBA btn2Color)
            {
                return new Params
                {
                    m_Type = type,
                    m_Title = title,
                    m_Message = message,
                    m_IsModal = isModal,
                    m_Btn0Text = btn0Text,
                    m_Btn1Text = btn1Text,
                    m_Btn2Text = btn2Text,
                    m_OnBtn0Callback = onBtn0Callback,
                    m_OnBtn1Callback = onBtn1Callback,
                    m_OnBtn2Callback = onBtn2Callback,
                    m_Btn0Color = btn0Color,
                    m_Btn1Color = btn1Color,
                    m_Btn2Color = btn2Color,
                };
            }

            public MessageBoxType Type { get { return m_Type; } }
            public string Title { get { return m_Title; } }
            public string Message { get { return m_Message; } }
            public bool IsModal { get { return m_IsModal; } }
            public string Btn0Text { get { return m_Btn0Text; } }
            public string Btn1Text { get { return m_Btn1Text; } }
            public string Btn2Text { get { return m_Btn2Text; } }
            public MessageBoxCallback OnBtn0Callback { get { return m_OnBtn0Callback; } }
            public MessageBoxCallback OnBtn1Callback { get { return m_OnBtn1Callback; } }
            public MessageBoxCallback OnBtn2Callback { get { return m_OnBtn2Callback; } }
            public RGBA Btn0Color { get { return m_Btn0Color; } }
            public RGBA Btn1Color { get { return m_Btn1Color; } }
            public RGBA Btn2Color { get { return m_Btn2Color; } }
        }
    }
}