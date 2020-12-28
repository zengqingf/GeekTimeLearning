using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CalculateProgramRunningTime;

namespace Hashtable_CSharp
{
    class Hashtable_2
    {
        //HashSet<>
        //SortedDictionary<>
        //SortedSet<>
        //SortedList<>
        //ConcurrentDictionary



        /**
         * 
         * SortedList vs SortedDictionary
         * 
         * SortedList and SortedDictionary are best used when you need order to the items that you’re storing. 
         * Here is a pretty detailed comparison from Microsoft:
         * 
         * The SortedList<TKey, TValue> generic class is an array of key/value pairs with O(logn) retrieval, 
         * where n is the number of elements in the dictionary. 
         * In this, it is similar to the SortedDictionary<TKey, TValue> generic class. 
         * The two classes have similar object models, and both have O(logn) retrieval. 
         * Where the two classes differ is in memory use and speed of insertion and removal:

SortedList<TKey, TValue> uses less memory than SortedDictionary<TKey, TValue>.
SortedDictionary<TKey, TValue> has faster insertion and removal operations for unsorted data, 
            O(logn) as opposed to O(n) for SortedList<TKey, TValue>.
If the list is populated all at once from sorted data, 
            SortedList<TKey, TValue> is faster than SortedDictionary<TKey, TValue>.
Another difference between the SortedDictionary<TKey, TValue> and SortedList<TKey, TValue> classes is that 
            SortedList<TKey, TValue> supports efficient indexed retrieval of keys and values through the collections returned by the Keys and Values properties. 
            It is not necessary to regenerate the lists when the properties are accessed, 
            because the lists are just wrappers for the internal arrays of keys and values.
         * 
         * 
         * 
         * ref https://www.growingwiththeweb.com/2013/02/what-data-structure-net-collections-use.html
         *  Type	        Data structure	    Notes
            List<T>	        Array	            A regular list using a dynamic array
            SortedSet<T>	Red-black tree	    A list stored using a red-black tree
         * 
         * 
            Type	    Get ([i])	    Find	            Add	                Insert	        Remove
            List	    O(1)	        O(n)	            O(1)*	            O(n)	        O(n)O
            SortedSet	N/A	            O(logn)	            O(logn)	            O(logn)	        O(logn)
         * List.Add is O(n)O(n) when adding beyond the array’s capacity
         * 
         * 
         * 
         * 
         * Type	                            Datastructure	    Notes
            HashSet<T>	                    Hash table	        A hash table where the key is the object itself
            Dictionary<TKey, TValue>	    Hash table	        A hash table using a key not necessarily on the object being stored
            SortedList<TKey, TValue>	    Array	            The same as Dictionary only items and their keys are stored sorted arrays
            SortedDictionary<TKey, TValue>	Red-black tree	    The same as Dictionary only items and their keys are stored in a red-black tree. Uses SortedSet behind the scenes
         * 
         * 
         * Type	                Find by key	            Remove	            Add
            HashSet	            O(1)*	            O(1)*	                O(1)**
            Dictionary	        O(1)*	            O(1)*	                O(1)**
            SortedList	        O(logn)	            O(n)	                O(n)
            SortedDictionary	O(logn)	            O(logn)	                O(logn)
         * 
         * 
         *  * O(n) with collision(冲突)
            ** O(n) with collision or when adding beyond the array’s capacity
         * 
         * **/

        private void _InitTest()
        {
            //泛型的键值集合/有序/Hash算法/占内存较大/不排序,不受装填因子的限制,对读写操作效率较高
            Dictionary<int, string> dc = new Dictionary<int, string>();
            dc.Add(1, "111111");
            dc.Add(2, "222222");
            dc.Add(3, "333333");
            dc.Add(5, "5555555");
            dc.Add(4, "4444444");
            dc.Add(10, "101010");
            dc.Add(35, "353535");

            //泛型的键值集合/int排序
            SortedDictionary<int, string> sd = new SortedDictionary<int, string>();
            sd.Add(1, "111111");
            sd.Add(2, "222222");
            sd.Add(3, "333333");
            sd.Add(5, "5555555");
            sd.Add(4, "4444444");
            sd.Add(10, "101010");
            sd.Add(35, "353535");

            //泛型的键值集合/string排序
            SortedDictionary<string, string> sd2 = new SortedDictionary<string, string>();
            sd2.Add("1111", "aaaaa");
            sd2.Add("22222", "bbbbbb");
            sd2.Add("ccccc", "333333");
            sd2.Add("555555", "dddddd");
            sd2.Add("444444", "cccccc");

            //弱类型的键集合/无序/Hash算法/占内存较小/不排序,扩容时会对所有的数据需要重新进行散列计算,所以较适用于读取操作频繁，写入操作较少的操作类型
            Hashtable ht = new Hashtable();
            ht.Add(1, "111111");
            ht.Add(2, "222222");
            ht.Add(3, "333333");
            ht.Add(5, "5555555");
            ht.Add(4, "4444444");
            ht.Add(10, "101010");
            ht.Add(35, "353535");

            //弱类型的键集合/无序/Hash算法/占内存较小/不排序,扩容时会对所有的数据需要重新进行散列计算,所以较适用于读取操作频繁，写入操作较少的操作类型
            Hashtable ht2 = new Hashtable();
            ht2.Add("1111", "aaaaa");
            ht2.Add("22222", "bbbbbb");
            ht2.Add("ccccc", "333333");
            ht2.Add("555555", "dddddd");
            ht2.Add("444444", "cccccc");

            //范型int排序(使用内存比SortedDictionary少,对频繁插入移除操作较慢,比较适合排序数据一次性填充列表)
            SortedList<int, string> sl = new SortedList<int, string>();
            sl.Add(1, "111111");
            sl.Add(2, "222222");
            sl.Add(3, "333333");
            sl.Add(5, "5555555");
            sl.Add(4, "4444444");
            sl.Add(10, "101010");
            sl.Add(35, "353535");

            //范型string排序(使用内存比SortedDictionary少,对频繁插入移除操作较慢,比较适合排序数据一次性填充列表)
            SortedList<string, string> sl2 = new SortedList<string, string>();
            sl2.Add("1111", "aaaaa");
            sl2.Add("22222", "bbbbbb");
            sl2.Add("ccccc", "333333");
            sl2.Add("555555", "dddddd");
            sl2.Add("444444", "cccccc");
            //sl2.Add("ccccc", "333333");//相同Key不能加入

            //int类型/过滤重复/排序
            SortedSet<int> ss = new SortedSet<int>();
            ss.Add(1);
            ss.Add(2);
            ss.Add(3);
            ss.Add(5);
            ss.Add(4);
            ss.Add(10);
            ss.Add(35);
            ss.Add(5);//相同数据被过滤了(可以直接加,成功返回True,失败返回False)

            //int类型/过滤重复/不排序
            var set = new HashSet<int>() { 3, 8, 2, 1, 3, 3, 6, 8, 7, 2, 8 };
            //int类型/过滤重复/排序
            var set2 = new SortedSet<int> { 3, 8, 2, 1, 3, 3, 6, 8, 7, 2, 8 };
            //string类型/过滤重复/不排序
            var set3 = new HashSet<string>() { "3", "8", "2", "1", "3", "3", "6", "8", "7", "2", "8" };
            //string类型/过滤重复/排序
            var set4 = new SortedSet<string> { "3", "8", "2", "1", "3", "3", "6", "8", "7", "2", "8" };

            //过滤重复排序
            SortedSet<Person> ss2 = new SortedSet<Person>(new SortAge());
            ss2.Add(new Person { FirstName = "Homer", LastName = "Simpson", Age = 47 });
            ss2.Add(new Person { FirstName = "Marge", LastName = "Simpson", Age = 45 });
            ss2.Add(new Person { FirstName = "Lisa", LastName = "Simpson", Age = 9 });
            ss2.Add(new Person { FirstName = "Bart", LastName = "Simpson", Age = 8 });
            ss2.Add(new Person { FirstName = "Saku", LastName = "Simpson", Age = 1 });
            ss2.Add(new Person { FirstName = "Mikko", LastName = "Simpson", Age = 32 });
            ss2.Add(new Person { FirstName = "Bart2", LastName = "Simpson", Age = 8 });//被过滤了

            //不过滤重复排序
            List<Person> l = new List<Person>();
            l.Add(new Person { FirstName = "Homer", LastName = "Simpson", Age = 47 });
            l.Add(new Person { FirstName = "Marge", LastName = "Simpson", Age = 45 });
            l.Add(new Person { FirstName = "Lisa", LastName = "Simpson", Age = 9 });
            l.Add(new Person { FirstName = "Bart", LastName = "Simpson", Age = 8 });
            l.Add(new Person { FirstName = "Saku", LastName = "Simpson", Age = 1 });
            l.Add(new Person { FirstName = "Mikko", LastName = "Simpson", Age = 32 });
            l.Add(new Person { FirstName = "Bart2", LastName = "Simpson", Age = 8 });//不过滤
            l.Sort(new SortAge());




            //多线程

            ConcurrentDictionary<string, string> concurrentDic = new ConcurrentDictionary<string, string>();
            concurrentDic.TryAdd("1111", "aaaaa");
            concurrentDic.TryAdd("22222", "bbbbbb");
            concurrentDic.TryAdd("ccccc", "333333");
            concurrentDic.TryAdd("555555", "dddddd");
            concurrentDic.TryAdd("444444", "cccccc");


            Hashtable htSync = Hashtable.Synchronized(new Hashtable());
        }
    }


    /**
     * 多线程
     * 
     * ref:https://blog.csdn.net/sinat_31465609/article/details/87801418
     * 
     * **/
    public class ConcurrentDictionaryVsHashtable
    {
        private static readonly ConcurrentDictionary<string, string> _ccDic = new ConcurrentDictionary<string, string>();

        private int _runCount;

        public void InvokeTasks()
        {
            var task1 = Task.Run(() => _PrintValue("Juxing"));
            var task2 = Task.Run(() => _PrintValue("Ziyan"));
            Task.WaitAll(task1, task2);

            _PrintValue("Juxing love Ziyan");
            Console.WriteLine(string.Format("运行次数：{0}", _runCount));
        }

        private void _PrintValue(string valueToPrint)
        {
            var valueFound = _ccDic.GetOrAdd("key",
                x =>
                {
                    Interlocked.Increment(ref _runCount);
                    Thread.Sleep(10);

                    return valueToPrint;
                });
            Console.WriteLine(valueFound);
        }
    }

    public class ConcurrentDictionaryVsHashtable_Lazy
    {
        private static readonly ConcurrentDictionary<string, Lazy<string>> _ccLazyDic = new ConcurrentDictionary<string, Lazy<string>>();

        private int _runCount;

        public void InvokeTasks()
        {
            var task1 = Task.Run(() => _PrintValue("Juxing"));
            var task2 = Task.Run(() => _PrintValue("Ziyan"));
            Task.WaitAll(task1, task2);

            _PrintValue("Juxing love Ziyan");
            Console.WriteLine(string.Format("运行次数：{0}", _runCount));
        }

        private void _PrintValue(string valueToPrint)
        {
            var valueFound = _ccLazyDic.GetOrAdd("key",
                x => new Lazy<string>(
                    ()=>
                    {
                        Interlocked.Increment(ref _runCount);
                        Thread.Sleep(10);

                        return valueToPrint;
                    }));
            Console.WriteLine(valueFound.Value);
        }
    }

    public class DictionaryVsHashtable
    {
        Hashtable _hashtable;
        Dictionary<string, string> _dictionary;
        ConcurrentDictionary<string, string> _conDictionary;
        Hashtable _syncHashtable;

        ProgramRunningTime runningTime = new ProgramRunningTime();
        double rTime = 0;

        public void Compare(int dataCount)
        {
            _hashtable = new Hashtable();
            _dictionary = new Dictionary<string, string>();
            _conDictionary = new ConcurrentDictionary<string, string>();
            _syncHashtable = Hashtable.Synchronized(new Hashtable());

            // Hashtable
            runningTime.ResetHandler(() =>
            {
                for (int i = 0; i < dataCount; i++)
                {
                    _hashtable.Add("key" + i.ToString(), "Value" + i.ToString());
                }
            });
            Console.WriteLine("HashTable插" + dataCount + "条耗时(毫秒)：" + runningTime.StartCalculate());

            //Dictionary
            runningTime.ResetHandler(() =>
            {
                for (int i = 0; i < dataCount; i++)
                {
                    _dictionary.Add("key" + i.ToString(), "Value" + i.ToString());
                }
            });
            Console.WriteLine("Dictionary插" + dataCount + "条耗时(毫秒)：" + runningTime.StartCalculate());

            //ConcurrentDictionary
            runningTime.ResetHandler(() =>
            {
                for (int i = 0; i < dataCount; i++)
                {
                    _conDictionary.TryAdd("key" + i.ToString(), "Value" + i.ToString());
                }
            });
            Console.WriteLine("ConcurrentDictionary插" + dataCount + "条耗时(毫秒)：" + runningTime.StartCalculate());


            // Hashtable Sync
            runningTime.ResetHandler(() =>
            {
                for (int i = 0; i < dataCount; i++)
                {
                    _syncHashtable.Add("key" + i.ToString(), "Value" + i.ToString());
                }
            });
            Console.WriteLine("HashTable Sync 插" + dataCount + "条耗时(毫秒)：" + runningTime.StartCalculate());



            // Hashtable
            runningTime.ResetHandler(() =>
            {
                for (int i = 0; i < _hashtable.Count; i++)
                {
                    var key = _hashtable[i];
                }
            });
            Console.WriteLine("HashTable遍历时间(毫秒)：" + runningTime.StartCalculate());

            //Dictionary
            runningTime.ResetHandler(() =>
            {
                for (int i = 0; i < _hashtable.Count; i++)
                {
                    var key = _dictionary["key" + i.ToString()];
                }
            });
            Console.WriteLine("Dictionary遍历时间(毫秒)：" + runningTime.StartCalculate());

            //ConcurrentDictionary
            runningTime.ResetHandler(() =>
            {
                for (int i = 0; i < _hashtable.Count; i++)
                {
                    var key = _conDictionary["key" + i.ToString()];
                }
            });
            Console.WriteLine("ConcurrentDictionary遍历时间(毫秒)：" + runningTime.StartCalculate());

            // Hashtable Sync
            runningTime.ResetHandler(() =>
            {
                for (int i = 0; i < _syncHashtable.Count; i++)
                {
                    var key = _syncHashtable[i];
                }
            });
            Console.WriteLine("HashTable Sync遍历时间(毫秒)：" + runningTime.StartCalculate());
        }
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }

    public class SortAge : IComparer<Person>
    {
        public int Compare(Person firstPerson, Person secondPerson)
        {
            if (firstPerson.Age > secondPerson.Age)
                return 1;
            if (firstPerson.Age < secondPerson.Age)
                return -1;
            else
                return 0;
        }
    }
}
