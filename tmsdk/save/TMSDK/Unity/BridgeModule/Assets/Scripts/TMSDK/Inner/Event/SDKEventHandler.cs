using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TMSDKClient
{
    public class SDKEventHandler : ISDKEventHander
    {
        private List<EventHandler<SDKEventArgs>> eventHandlers = new List<EventHandler<SDKEventArgs>>();
        private event EventHandler<SDKEventArgs> eventHandler;
        public event EventHandler<SDKEventArgs> EventHandler
        {
            add
            {
                eventHandler += value;
                eventHandlers.Add(value);
            }
            remove
            {
                eventHandler -= value;
                eventHandlers.Remove(value);
            }
        }

        public void Register(ISDKEventHandle handle)
        {
            if (null == handle)
            {
                return;
            }
            EventHandler += handle.OnEvent;
        }

        public void Detach(ISDKEventHandle handle)
        {
            if (null == handle)
            {
                return;
            }
            EventHandler -= handle.OnEvent;
        }

        public void DetachAll()
        {
            _RemoveAllEvents();
            eventHandler = null;
        }

        private void _RemoveAllEvents()
        {
            foreach (EventHandler<SDKEventArgs> eDel in eventHandlers)
            {
                eventHandler -= eDel;
            }
            eventHandlers.Clear();
        }

        public void Invoke(SDKEventArgs e)
        {
            EventHandler<SDKEventArgs> handler = eventHandler;
            if (handler != null && e != null)
            {
                handler(this, e);
            }
        }
    }
}