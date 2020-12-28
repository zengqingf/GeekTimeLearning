using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Observer_1
{
    class CommonObserver_1
    {

    }

    //依赖接口而非具体对象
    //有缺点 ： 如果观察者和主题（通知者、被观察者）之间互相不认识或者无法认识

    //主题接口
    public interface ISubject
    {
        void RegisterObserver(IObserver obj);
        void RemoveObserver(IObserver obj);
        void NotifyObserver();
    }

    //或者
    abstract class Subject
    {
        private List<Observer> observers = new List<Observer>();
        public void Attach(Observer observer)
        {
            if (observers != null && !observers.Contains(observer))
            {
                observers.Add(observer);
            }
        }

        public void Detach(Observer observer)
        {
            observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (var observer in observers)
            {
                observer.Update();
            }
        }
    }

    //观察者接口
    public interface IObserver
    {
        void Update();
    }

    //或者
    abstract class Observer
    {
        public abstract void Update();
    }

    //具体主题
    public class WeatherData : ISubject
    {
        private ArrayList observers;
        public float temperature { get; private set; }
        public float humidity { get; private set; }
        public float pressure { get; private set; }

        public WeatherData()
        {
            observers = new ArrayList();
        }
        public void RegisterObserver(IObserver o)
        {
            observers.Add(o);
        }

        public void RemoveObserver(IObserver o)
        {
            int i = observers.IndexOf(o);
            if (i >= 0)
            {
                observers.RemoveAt(i);
            }
        }

        public void NotifyObserver()
        {
            for (int i = 0; i < observers.Count; i++)
            {
                IObserver observer = (IObserver)observers[i];
                observer.Update();
            }
        }

        public void messurementsChanged()
        {
            NotifyObserver();
        }

        public void SetMessureMents(float temperature, float humidity, float pressure)
        {
            this.temperature = temperature;
            this.humidity = humidity;
            this.pressure = pressure;
            messurementsChanged();
        }
    }

    //具体观察者

    public interface IDisplayElement
    {
        void Display();
    }

    public class CurrentConditionDisplay : IObserver, IDisplayElement
    {
        private float temperature;
        private float humidity;
        private float pressure;
        private ISubject data;

        public void Display()
        {
            Console.WriteLine($"公告板1 当前天气 => temperature:{temperature},humidity:{humidity},pressure:{pressure}");
        }

        public void Update()
        {
            WeatherData weatherData = data != null ? data as WeatherData : null;
            if (weatherData != null)
            {                
                this.temperature = weatherData.temperature;
                this.humidity = weatherData.humidity;
                this.pressure = weatherData.pressure;
            }
            Display();
        }

        public CurrentConditionDisplay(ISubject data)
        {
            this.data = data;
            if (data != null)
            {
                data.RegisterObserver(this);
            }
        }
    }

    public class ForecastDisplay : IObserver, IDisplayElement
    {
        private float temperature;
        private float humidity;
        private float pressure;
        private ISubject data;

        public void Display()
        {
            Console.WriteLine($"公告板2 明天天气 => temperature:{temperature},humidity:{humidity},pressure:{pressure}");
        }

        public void Update()
        {
            WeatherData weatherData = data != null ? data as WeatherData : null;
            if (weatherData != null)
            {
                this.temperature = weatherData.temperature;
                this.humidity = weatherData.humidity;
                this.pressure = weatherData.pressure;
            }
            Display();
        }

        public ForecastDisplay(ISubject data)
        {
            this.data = data;
            if (data != null)
            {
                data.RegisterObserver(this);
            }
        }
    }
}
