using System;
using System.Collections.Generic;
using System.Text;

namespace Iterator_1
{
    class IteratorWorkflow
    {
        static readonly string Padding = new string(' ',30);

        static IEnumerable<int> GetEnumerable()
        {
            Console.WriteLine("{0}Start of GetEnumerator()", Padding);

            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine("{0}About to yield {1}", Padding, i);
                yield return i;
                Console.WriteLine("{0}After yield", Padding);
            }

            Console.WriteLine("{0}Yielding final value", Padding);
            yield return -1;

            Console.WriteLine("{0}End of GetEnumerator()", Padding);
        }

        public static void Test()
        {
            IEnumerable<int> iterable = GetEnumerable();
            IEnumerator<int> iterator = iterable.GetEnumerator();

            Console.WriteLine("Starting to iterate");
            while (true)
            {
                Console.WriteLine("Calling MoveNext()...");
                bool result = iterator.MoveNext();
                Console.WriteLine("... MoveNext result={0}", result);
                if (!result)
                {
                    break;
                }
                Console.WriteLine("Fetching Current...");
                Console.WriteLine("... Current result={0}", iterator.Current);
            }
        }

    }
}
