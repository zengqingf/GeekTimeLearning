using System;
using System.Collections.Generic;

namespace Observer_1
{
    /*
     * 
     *  ref : https://docs.microsoft.com/en-us/dotnet/api/system.iobserver-1?view=netframework-4.0
     *  ref : https://docs.microsoft.com/en-us/dotnet/api/system.iobservable-1?view=netframework-4.0
     
        IObservable<out T> : 被观察者   提供订阅方法

         
        IObserver<int T> :  观察者
         
         */


    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Observable");


            //Test 1
            //LocationTracker provider = new LocationTracker();
            //LocationReporter receiver1 = new LocationReporter("Test 1");
            //receiver1.Subscribe(provider);
            //LocationReporter receiver2 = new LocationReporter("Test 2");
            //receiver2.Subscribe(provider);

            //provider.Transmission(new Location(47.6456, -122.1312));
            //receiver1.Unsubscribe();
            //provider.Transmission(new Location(47.6677, -122.1199));
            //provider.Transmission(default(Location));
            //provider.EndTransmission();

            //Test 2
            //WeatherData weatherData = new WeatherData();
            //Console.WriteLine("--------公告板1加入观察者-------");
            //CurrentConditionDisplay display1 = new CurrentConditionDisplay(weatherData);
            //weatherData.SetMessureMents(10, 20, 30);
            //Console.WriteLine("--------公告板2加入观察者-------");
            //ForecastDisplay display2 = new ForecastDisplay(weatherData);
            //weatherData.SetMessureMents(15, 25, 35);
            //Console.WriteLine("--------公告板1退出观察者-------");
            //weatherData.RemoveObserver(display1);
            //weatherData.SetMessureMents(19, 29, 39);

            //Test 3
            var person = new Person();
            person.FallsIll += EventObserver_1.OnFallsIll;
            person.OnFallsIll();
            person.FallsIll -= EventObserver_1.OnFallsIll;

            Console.ReadKey();
        }
    }

    /*****************************************  Example 1  ************************************/

    public struct Location
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }

    public class LocationTracker : BaseProvider<Location>
    {

    }

    public class LocationUnknownException : UnknownException
    {

    }

    public class LocationReporter : BaseReceiver<Location>
    {
        private string instName;
        public LocationReporter(string name)
        {
            this.instName = name;
        }

        public override void OnCompleted()
        {
            base.OnCompleted();
            Console.WriteLine("The Location Tracker has completed transmitting data to {0}.", this.instName);            
        }

        public override void OnError(Exception e)
        {
            base.OnError(e);
            Console.WriteLine("{0}: The location cannot be determined.", this.instName);
        }

        public override void OnNext(Location value)
        {
            base.OnNext(value);
            Console.WriteLine("{2}: The current location is {0}, {1}", value.Latitude, value.Longitude, this.instName);
        }
    }


    /*                            被观察者 向 观察者 提供数据 传递的是被观察者本身                          */

    public class LocationTracker2 : BaseProvider<LocationTracker>
    {

    }

    public class LocationReporter2 : BaseReceiver<LocationTracker2>
    {

    }
    /*                            End                            */

    public class UnknownException : Exception
    {
        internal UnknownException() { }
    }

    public class BaseProvider<T> : IObservable<T>
    { 
        private List<IObserver<T>> observers;

        public BaseProvider()
        {
            observers = new List<IObserver<T>>();
        }

        //订阅接口
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
            return new Unsubscriber(observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<T>> _observers;
            private IObserver<T> _observer;

            public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observers != null && _observers.Contains(_observer))
                {
                    _observers.Remove(_observer);
                }
            }
        }

        public void Transmission(T value)
        {
            foreach (var observer in observers)
            {
                if (value == null)
                {
                    observer.OnError(new UnknownException());
                }
                else
                {
                    observer.OnNext(value);
                }
            }
        }

        public void EndTransmission()
        {         
            //foreach (var observer in observers)
            //注意 ： 需要转换成数组 否则 不能在对List的循环中 增删对象
            foreach(var observer in observers.ToArray())
            {
                if (observers.Contains(observer))
                    observer.OnCompleted();
            }
            observers.Clear();
        }
    }

    public class BaseReceiver<T> : IObserver<T>
    {
        private IDisposable unsubscriber;

        public BaseReceiver()
        {

        }

        public virtual void Subscribe(IObservable<T> provider)
        {
            if (provider != null)
                unsubscriber = provider.Subscribe(this);
        }

        public virtual void OnCompleted()
        {
            Console.WriteLine("OnCompleted");
            this.Unsubscribe();
        }

        public virtual void OnError(Exception error)
        {
            Console.WriteLine("OnError : {0}", error.ToString());
        }

        public virtual void OnNext(T value)
        {
            Console.WriteLine("OnNext : {0}", value);
        }

        public virtual void Unsubscribe()
        {
            if (unsubscriber != null)
            {
                unsubscriber.Dispose();
            }
        }
    }

    /*************************************************************************************/
}
