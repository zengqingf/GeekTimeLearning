using System;
using System.Collections;
using System.Collections.Generic;


namespace TMSDKClient
{
    [Serializable]
    public class SDKCallArg<T>
    {
        public string name;
        public T value;

        public SDKCallArg(string name, T value)
        {
            this.name = name;
            this.value = value;
        }
    }
}
