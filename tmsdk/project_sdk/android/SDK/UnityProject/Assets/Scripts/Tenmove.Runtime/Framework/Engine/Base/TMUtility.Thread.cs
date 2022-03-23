

namespace Tenmove.Runtime
{
    public static partial class Utility
    {
        public static class Thread
        {
            static private int m_MainThreadID = ~0;

            static public void MarkMainThread()
            {
                if(~0 == m_MainThreadID)
                    m_MainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
                else
                    Debugger.LogWarning("Main thread has already marked!");
            }

            static public bool IsMainThread()
            {
                if (~0 == m_MainThreadID)
                    throw new System.Exception("Main thread ID is not mark yet!");

                return m_MainThreadID == System.Threading.Thread.CurrentThread.ManagedThreadId;
            }
        }
    }
}