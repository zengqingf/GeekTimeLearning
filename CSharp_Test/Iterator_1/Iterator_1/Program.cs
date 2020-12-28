using System;
using System.Collections;
using System.Collections.Generic;

namespace Iterator_1
{
    /**
     * 迭代器
     * 
     * ref : https://www.cnblogs.com/minotauros/p/10439094.html
     * 
     * https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/iterators
     * 
     * https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/yield
     * 
     * 
     * 
     * 协程
     * 
     * Unity协程在主线程执行
     * 在Update方法之后执行 是一个函数 可以暂停执行 知道给定的yieldinstruction(中断指令)完成
     * 
     * **/


    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            IteratorWorkflow.Test();
            Console.ReadKey();
            return;

            // 对于实现了接口IEnumerable或IEnumerable<T>的类型的对象，可以使用foreach对其进行遍历：
            foreach (var item in MyIterator())
            {
                Console.WriteLine("for each item is {0}", item);
            }

            //对于实现了泛型接口的类型的对象在手动迭代时还需要使用using语句以在其迭代完成后调用Dispose()方法：
            using (IEnumerator<int> enumerator = MyIterator().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Console.WriteLine("manual for each item is {0}", enumerator.Current);
                }
            }

            //对于只实现了接口IEnumerator或IEnumerator<T>的类型的对象，只能手动进行迭代：
            using (IEnumerator<int> enumerator = MyIterator2())
            {
                while (enumerator.MoveNext())
                {
                    Console.WriteLine("manual for each item is {0}", enumerator.Current);
                }
            }


            //在使用foreach遍历时，如果使用var，会由编译器自动推断item为int类型，并调用泛型接口的实现，
            //如果将item的类型指定为object类型，则依然会调用泛型接口的实现，但会产生装箱操作
            foreach (var item in new MyIterator())
            {
                Console.WriteLine("for each item is {0}", item);
            }


            //在使用foreach遍历时，如果将其转换为IEnumerable类型的对象，
            //则会调用非泛型接口的实现，<<<编译器>>> 将自动推断item为object类型，并产生装箱操作：
            foreach (var item in (IEnumerable)new MyIterator()) //或显式指定为object：object item
            {
                Console.WriteLine("for each item is {0}", item);
            }

            //直接调用迭代器方法或迭代器类型中自动生成的Reset()方法时会抛出异常NotSupportedException，
            //若要从头开始重新迭代，必须获取新的迭代器，或在迭代器类型中手动实现Reset()方法；



            A a1 = new A();
            var aa1 = a1._Coroutine();
            while (aa1.MoveNext())
            {
                var current = aa1.Current;
                Console.WriteLine("A Current Value is {0}", current);
            }

            Aa a2 = new Aa();
            var aa2 = new Aa._Internal_Compile_Generate_Coroutine();
            while (aa2.MoveNext())
            {
                var current = aa2.Current;
                Console.WriteLine("Aa Current Value is {0}", current);
            }

            Console.ReadKey();
        }


        static IEnumerable<int> MyIterator()
        {
            yield return 10;

            yield return 20;
        }

        static IEnumerator<int> MyIterator2()
        {
            yield return 30;

            yield return 40;
        }
    }


    public class MyIterator : IEnumerable<int>
    {
        public IEnumerator<int> GetEnumerator()
        {
            yield return 10;

            yield return 20;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            //Console.WriteLine("Non-Generic GetEnumerator");
            //return GetEnumerator();  // 不是yield return

            //或者手动迭代
            using (IEnumerator<int> iterator = this.GetEnumerator())
            {
                while (iterator.MoveNext())
                {
                    yield return iterator.Current;
                }
            }
        }
    }

    public class MyIterator2 : IEnumerator<int>
    {
        public int Current => throw new NotImplementedException();

        object IEnumerator.Current => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }

    public class A
    {
        public IEnumerator _Coroutine()
        {
            //code region 0
            Console.WriteLine("class A, method _Coroutine, step1");
            yield return 1;
            Console.WriteLine("class A, method _Coroutine, step2");
            //code region 1
            yield break;
            Console.WriteLine("class A, method _Coroutine, step3");
            //code region 2
        }
    }

    /// <summary>
    /// 类A 可以转换为 以下类 
    /// </summary>
    public class Aa
    {
        public class _Internal_Compile_Generate_Coroutine : IEnumerator
        {
            private int _case = 0;
            private object _current;

            public object Current
            {
                get { return _current; }
            }

            public bool MoveNext()
            {
                switch (_case)
                {
                    case 0:
                        //code region 0
                        Console.WriteLine("class Aa, method _Internal_Compile_Generate_Coroutine, step1");
                        _case++;
                        _current = 1;
                        return true;
                    case 1:
                        //code region 1
                        Console.WriteLine("class Aa, method _Internal_Compile_Generate_Coroutine, step2");
                        _case++;
                        return false;
                    case 2:
                        //code region 2
                        Console.WriteLine("class Aa, method _Internal_Compile_Generate_Coroutine, step3");
                        _case++;
                        return true;
                }
                return false;
            }

            public void Reset()
            {
                _case = 0;
                _current = null;
            }
        }
    }
}