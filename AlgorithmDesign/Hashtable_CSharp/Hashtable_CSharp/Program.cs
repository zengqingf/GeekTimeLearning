using System;

namespace Hashtable_CSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //Test1
            //Hashtable_1 hashtable_1 = new Hashtable_1();
            //hashtable_1.PrintTest1Time();

            //Test2
            //DictionaryVsHashtable compare = new DictionaryVsHashtable();
            //compare.Compare(5000000);

            //Test3
            //ConcurrentDictionaryVsHashtable compare2 = new ConcurrentDictionaryVsHashtable();
            //compare2.InvokeTasks();

            //Test4
            ConcurrentDictionaryVsHashtable_Lazy compare3 = new ConcurrentDictionaryVsHashtable_Lazy();
            compare3.InvokeTasks();

            Console.ReadKey();
        }


    }
}
