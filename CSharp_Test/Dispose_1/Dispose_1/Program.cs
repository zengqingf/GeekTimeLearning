using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;

namespace Dispose_1
{
    /*
     
        GC开始的条件：


        系统具有较低的物理内存；
        由托管堆上已分配的对象使用的内存超出了可接受的范围；
        手动调用GC.Collect方法，但几乎所有的情况下，我们都不必调用，因为垃圾回收器会自动调用它，
        但在上面的例子中，为了体验一下不及时回收垃圾带来的危害，所以手动调用了GC.Collect，大家也可以仔细体会一下运行这个方法带来的不同。
         
         
         */


    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");



            using (var mc = new MyClass())
            {

            }

            //相当于
            MyClass mc1 = null;
            try
            {
                mc1 = new MyClass();
            }
            finally
            {
                if (mc1 != null)
                {
                    mc1.Dispose();
                }

                //如果 MyClass中的Dispose()是通过  IDisposable.Dispose实现的 
                IDisposable disposable = mc1 as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }

            MyClass mc11 = new MyClass();
            mc11.Dispose();

            //同时管理多个相同类型的对象
            using (MyClass mc2 = new MyClass(), mc3 = new MyClass())
            {

            }

            //如果类型不一致
            using (var client = new HttpClient())
            {
                using (var stream = File.Create(""))
                { }
            }


        }



        /*
         实现IDisposable接口：
         1.实现Dispose方法；
         2.提取一个受保护的Dispose虚方法，在该方法中实现具体的释放资源的逻辑；
         3.添加析构函数；
         4.添加一个私有的bool类型的字段，作为释放资源的标记
         
         */


        /// <summary>
        /// 1. 实现IDisposable接口 为了使用 using语法糖
        /// </summary>
        public class MyClass : IDisposable
        {
            /// <summary>
            /// 模拟一个非托管资源
            /// </summary>
            private IntPtr NativeResource { get; set; } = Marshal.AllocHGlobal(100);

            /// <summary>
            /// 模拟一个托管资源
            /// </summary>
            public Random ManagedResource { get; set; } = new Random();

            /// <summary>
            /// 释放标记
            /// </summary>
            private bool disposed;

            /// <summary>
            /// 终结器 或者 析构方法
            /// 为了防止忘记显式调用Dispose方法
            /// </summary>
            ~MyClass()
            {
                //必须为false
                Dispose(false);
            }

            /// <summary>
            /// 执行与释放或重置 非托管资源 关联的应用程序定义的任务
            /// </summary>
            public void Dispose()
            {
                //必须为true
                Dispose(true);

                //通知垃圾回收器不再调用终结器 Finalize
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// 非必须，只是符合 其他语言规范
            /// </summary>
            public void Close()
            {
                Dispose();
            }

            /// <summary>
            /// 非密封类 可重写的Dispose方法， 方便子类继承时重写
            /// </summary>
            /// <param name="disposing"></param>
            protected virtual void Dispose(bool disposing)
            {
                if (disposed)
                {
                    return;
                }
                //清理托管资源
                if (disposing)
                {
                    if (ManagedResource != null)
                    {
                        ManagedResource = null;
                    }
                }

                //清理非托管资源
                if (NativeResource != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(NativeResource);
                    NativeResource = IntPtr.Zero;
                }

                //通知自己已经被释放
                disposed = true;
            }
        }
    }
}
