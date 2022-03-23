
namespace Tenmove.Runtime
{
    public static partial class Utility
    {
        public static class Helper
        {
            public delegate void ForEachToDo<T>(T element);
            public delegate bool ForEachIf<T>(T element);
            static public void ForEach<T>(System.Collections.Generic.List<T> list, ForEachToDo<T> eachToDo,ForEachIf<T> eachIf)
            {
                if (null == eachToDo)
                    return;

                for(int i = 0,icnt = list.Count;i<icnt;++i)
                {
                    if (eachIf(list[i]))
                        eachToDo(list[i]);
                }
            }

            static public void ForEach<T>(System.Collections.Generic.List<T> list, ForEachToDo<T> eachToDo)
            {
                if (null == eachToDo)
                    return;

                for (int i = 0, icnt = list.Count; i < icnt; ++i)
                    eachToDo(list[i]);
            }

        }
    }
}