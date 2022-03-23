

using System.Collections.Generic;

namespace Tenmove.Runtime.EmbedUI
{
    public delegate void MessageBoxCallback();

    public enum MessageBoxType
    {
        OK,
        OKCancel,
        CancelOK,
        Custom,
    }

    public partial class MessageBox
    {
        private static MessageBox m_Instance = null;
        private readonly Manager m_MessageBoxManager;

        public MessageBox()
        {
            m_MessageBoxManager = new Manager();
        }

        private static MessageBox _Instance
        {
            get
            {
                if (null == m_Instance)
                    m_Instance = new MessageBox();

                return m_Instance;
            }
        }

        public static void OK(string title, string message, MessageBoxCallback onOKCallback, bool isModal = true)
        {
            _Instance.m_MessageBoxManager.CreateMessageBox(Params.OK(title, message, onOKCallback, isModal));
        }

        public static void OK(string title, string message, string btnOKText, MessageBoxCallback onOKCallback, bool isModal = true)
        {
            _Instance.m_MessageBoxManager.CreateMessageBox(Params.OK(title, message, btnOKText, onOKCallback,Graphic.RGBA.Black, isModal));
        }

        public static void OK(string title, string message, string btnOKText, MessageBoxCallback onOKCallback, Graphic.RGBA btnOKColor, bool isModal = true)
        {
            _Instance.m_MessageBoxManager.CreateMessageBox(Params.OK(title, message, btnOKText, onOKCallback, btnOKColor, isModal));
        }

        public static void OKCancel(string title, string message, MessageBoxCallback onOK, MessageBoxCallback onCancel, bool isModal = true)
        {
            _Instance.m_MessageBoxManager.CreateMessageBox(Params.OKCancel(title, message, onOK, onCancel, isModal));
        }

        public static void OKCancel(string title, string message, string btnOKText, string btnCancelText, MessageBoxCallback onOK, MessageBoxCallback onCancel, bool isModal = true)
        {
            _Instance.m_MessageBoxManager.CreateMessageBox(Params.OKCancel(title, message, btnOKText, btnCancelText, onOK, onCancel,Graphic.RGBA.Black, Graphic.RGBA.Black, isModal));
        }

        public static void OKCancel(string title, string message, string btnOKText, string btnCancelText, MessageBoxCallback onOK, MessageBoxCallback onCancel, Graphic.RGBA btnOKColor, Graphic.RGBA btnCancelColor, bool isModal = true)
        {
            _Instance.m_MessageBoxManager.CreateMessageBox(Params.OKCancel(title, message, btnOKText, btnCancelText, onOK, onCancel, btnOKColor, btnCancelColor, isModal));
        }

        public static void CancelOK(string title, string message, MessageBoxCallback onCancel, MessageBoxCallback onOK, bool isModal = true)
        {
            _Instance.m_MessageBoxManager.CreateMessageBox(Params.CancelOK(title, message, onCancel, onOK, isModal));
        }

        public static void CancelOK(string title, string message, string btnCancelText, string btnOKText, MessageBoxCallback onCancel, MessageBoxCallback onOK, bool isModal = true)
        {
            _Instance.m_MessageBoxManager.CreateMessageBox(Params.CancelOK(title, message, btnCancelText, btnOKText, onCancel, onOK, Graphic.RGBA.Black, Graphic.RGBA.Black, isModal));
        }

        public static void CancelOK(string title, string message, string btnCancelText, string btnOKText, MessageBoxCallback onCancel, MessageBoxCallback onOK, Graphic.RGBA btnCancelColor, Graphic.RGBA btnOKColor, bool isModal = true)
        {
            _Instance.m_MessageBoxManager.CreateMessageBox(Params.CancelOK(title, message, btnCancelText, btnOKText, onCancel, onOK, btnCancelColor, btnOKColor, isModal));
        }

        public static void Custom(string title, string message, string btn0Text, string btn1Text, string btn2Text, MessageBoxCallback onBtn0Callback, MessageBoxCallback onBtn1Callback, MessageBoxCallback onBtn2Callback, bool isModal = true)
        {
            _Instance.m_MessageBoxManager.CreateMessageBox(Params.Custom(title, message,  btn0Text,  btn1Text,  btn2Text,  onBtn0Callback,  onBtn1Callback,  onBtn2Callback, isModal));
        }

        public static void Custom(string title, string message, string btn0Text, string btn1Text, string btn2Text, MessageBoxCallback onBtn0Callback, MessageBoxCallback onBtn1Callback, MessageBoxCallback onBtn2Callback, Graphic.RGBA btn0Color, Graphic.RGBA btn1Color, Graphic.RGBA btn2Color, bool isModal = true)
        {
            _Instance.m_MessageBoxManager.CreateMessageBox(Params.Custom(title, message, btn0Text, btn1Text, btn2Text, onBtn0Callback, onBtn1Callback, onBtn2Callback, btn0Color, btn1Color, btn2Color, isModal));
        }
    }
}