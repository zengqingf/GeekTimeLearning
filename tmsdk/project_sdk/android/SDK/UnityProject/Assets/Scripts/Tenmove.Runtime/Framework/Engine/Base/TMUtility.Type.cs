

namespace Tenmove.Runtime
{
    public static partial class Utility
    {
        public static class Type
        {
            static public T Cast<T>(object obj) where T : class
            {
                if (null == obj)
                {
                    Debugger.LogWarning("Parameter 'obj' can not be null!");
                    return null;
                }

                T typeObj = obj as T;
                if (null != typeObj)
                    return typeObj;
                else
                    Debugger.LogWarning("Object [Type:{0}] can not convert to specific type:{1}", obj.GetType(), typeof(T));

                return null;
            }
        }
    }
}