using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Closure_1
{
    class Program
    {
        static void Main(string[] args)
        {
            ////在循环外部生成 委托类
            ////输出  9 9  ... 9
            //List<Action> a = new List<Action>();
            //for (int i = 0; i < 9; i++)
            //{
            //    a.Add(() => { Console.WriteLine(i); });
            //}
            //for (int i = 0; i < 9; i++)
            //{
            //    a[i].Invoke();
            //}
            //Console.ReadKey();

            ////转存值类型
            ////在循环内部生成 委托类
            //List<Action> a = new List<Action>();
            //for (int i = 0; i < 9; i++)
            //{
            //    int j = i;
            //    a.Add(() => { Console.WriteLine(j); });
            //}
            //for (int i = 0; i < 9; i++)
            //{
            //    a[i].Invoke();
            //}
            //Console.ReadKey();

            ////使用不同作用域的变量 会生成多个闭包类
            //List<Action> a = new List<Action>();
            //int[] arr = new int[10];
            //for (int i = 0; i < arr.Length; i++)
            //{
            //    arr[i] = i;
            //}
            //for (int i = 0; i < 9; i++)
            //{
            //    a.Add(() => { Console.WriteLine(arr[i]); });
            //}
            //for (int i = 0; i < 9; i++)
            //{
            //    a[i].Invoke();
            //}
            //Console.ReadKey();


            ////3个不同生命周期的变量被闭包，生成3个闭包类
            //List<Action> a = new List<Action>();
            //for (int i = 0; i < 1; i++)
            //{
            //    for (int j = 0; j < 1; j++)
            //    {
            //        for (int k = 0; k < 1; k++)
            //        {
            //            a.Add(() => { Console.WriteLine(i + j + k); });
            //        }
            //    }
            //}
            //for (int i = 0; i < a.Count; i++)
            //{
            //    a[i].Invoke();
            //}
            //Console.ReadKey();


            ////生成1个闭包类，但是生成2个委托方法
            //List<Action> a1 = new List<Action>();
            //List<Action> a2 = new List<Action>();
            //for (int i = 0; i < 1; i++)
            //{
            //    a1.Add(() => { Console.WriteLine("a1"); });
            //    a2.Add(() => { Console.WriteLine("a2"); });
            //}
            //for (int i = 0; i < a1.Count; i++)
            //{
            //    a1[i].Invoke();
            //}
            //for (int i = 0; i < a2.Count; i++)
            //{
            //    a2[i].Invoke();
            //}
            //Console.ReadKey();
        }
    }
}
