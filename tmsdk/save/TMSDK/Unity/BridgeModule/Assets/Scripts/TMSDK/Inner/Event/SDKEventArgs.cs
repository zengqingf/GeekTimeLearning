using System;

namespace TMSDKClient 
{
    public class SDKEventArgs : EventArgs
    {
        public SDKEventType eventType;
        public bool status;
        public object param1;
        public object param2;

        public SDKEventArgs(SDKEventType eType)
        {
            Reset(eType);
        }

        public SDKEventArgs(SDKEventType eType, bool status)
        {
            Reset(eType, status);
        }

        public SDKEventArgs(SDKEventType eType, bool status, object p1)
        {
            Reset(eType, status, p1);
        }

        public SDKEventArgs(SDKEventType eType, bool status, object p1, object p2)
        {
            Reset(eType, status, p1, p2);
        }

        public SDKEventArgs(SDKEventType eType, object p1)
        {
            Reset(eType, p1);
        }

        public SDKEventArgs(SDKEventType eType, object p1, object p2)
        {
            Reset(eType, p1, p2);
        }

        private void _Clear()
        {
            this.eventType = SDKEventType.None;
            this.status = false;
            this.param1 = string.Empty;
            this.param2 = string.Empty;
        }

        public void Reset(SDKEventType eType)
        {
            _Clear();
            this.eventType = eType;
        }

        public void Reset(SDKEventType eType, bool status)
        {
            _Clear();
            this.eventType = eType;
            this.status = status;
        }

        public void Reset(SDKEventType eType, bool status, object p1)
        {
            _Clear();
            this.eventType = eType;
            this.status = status;
            this.param1 = p1;
        }

        public void Reset(SDKEventType eType, bool status, object p1, object p2)
        {
            _Clear();
            this.eventType = eType;
            this.status = status;
            this.param1 = p1;
            this.param2 = p2;
        }

        public void Reset(SDKEventType eType, object p1)
        {
            _Clear();
            this.eventType = eType;
            this.param1 = p1;
        }

        public void Reset(SDKEventType eType, object p1, object p2)
        {
            _Clear();
            this.eventType = eType;
            this.param1 = p1;
            this.param2 = p2;
        }
    }
}