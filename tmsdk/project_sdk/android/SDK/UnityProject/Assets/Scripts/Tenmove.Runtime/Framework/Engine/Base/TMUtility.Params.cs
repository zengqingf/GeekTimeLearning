
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    public static partial class Utility
    {
        public static class Params
        {
            public static object[] MergeArgs(object newArg,object[] args)
            {
                List<object> argList = FrameStackList<object>.Acquire();
                argList.Add(newArg);
                argList.AddRange(args);
                object[] newArgs = argList.ToArray();
                FrameStackList<object>.Recycle(argList);
                return newArgs;
            }

            public static object[] MergeArgs(object[] args,object newArg)
            {
                List<object> argList = FrameStackList<object>.Acquire();
                argList.AddRange(args);
                argList.Add(newArg);
                object[] newArgs = argList.ToArray();
                FrameStackList<object>.Recycle(argList);
                return newArgs;
            }
        }
    }
}

