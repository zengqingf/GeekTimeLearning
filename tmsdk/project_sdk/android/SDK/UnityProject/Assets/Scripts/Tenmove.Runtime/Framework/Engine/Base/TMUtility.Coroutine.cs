
using System.Collections;

namespace Tenmove.Runtime
{
    public static partial class Utility
    {
        public static class Coroutine
        {
            static public void ExcuteCoroutineAsFunction(IEnumerator routine)
            {
                if (null != routine)
                {
                    IEnumerator subRoutine = null;
                    while (routine.MoveNext())
                    {
                        subRoutine = routine.Current as IEnumerator;
                        if (null != subRoutine)
                            ExcuteCoroutineAsFunction(subRoutine);
                    }
                }
            }
        }
    }
}