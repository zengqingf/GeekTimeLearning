using System;
using System.Collections.Generic;
using System.Text;

namespace Observer_1
{
    class EventObserver_1
    {
        public static void OnFallsIll(object sender, FallsIllEventArgs eventArgs)
        {
            Console.WriteLine($"A doctor has been called to {eventArgs.Address}");
        }
    }

    public class Person
    {
        public event EventHandler<FallsIllEventArgs> FallsIll;

        public void OnFallsIll()
        {
            FallsIll?.Invoke(this, new FallsIllEventArgs("China Beijing"));
        }
    }


    // 用委托 替换 被观察者的接口或者虚类
    public class FallsIllEventArgs : EventArgs
    {
        public readonly string Address;

        public FallsIllEventArgs(string address)
        {
            this.Address = address;
        }
    }
}
