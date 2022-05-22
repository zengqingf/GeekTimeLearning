/*
ref:https://www.cnblogs.com/suoluo/p/7439944.html
Unity实现支持泛型的事件管理以减少使用object作为参数带来的频繁装拆箱

如果不用C#自身的event关键字而是要自己实现一个可统一管理游戏中各种消息事件通知管理的系统模块EventManger时，通常都是把事件delegate的参数定义为object类型以适应所有的数据类型，然而这样做的后果就是在使用过程中存在很频繁的装拆箱操作。
实际是有办法实现支持泛型的事件管理的，关键点在于所有形式的delegate方法都是可以保存在类型为Delegate的变量上的，保存和调用时将Delegate强转为目标delegate就行了。
简单示例如下：
*/

public static class MyGenericsEvent
{
    public delegate void Act ();
    public delegate void Act<T, U>(T t, U u);


    Dictionary<int, Delegate> eventTable = new Dictionary<int, Delegate>();

    public void AddListener<T, U>(int eventType, Act<T, U> listenerBeingAdded)
    {
        if (!eventTable.ContainsKey(eventType))
        {
            eventTable.Add(eventType, null);
        }

        Delegate d = eventTable[eventType];
        if (d != null && d.GetType() != listenerBeingAdded.GetType())
        {
            Debug.LogError(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
        }
        else
        {
            eventTable[eventType] = (Act<T, U>)eventTable[eventType] + listenerBeingAdded;
        }
    }

    public void RemoveListen<T, U>(int eventType, Act<T, U> listenerBeingRemoved)
    {
        if (eventTable.ContainsKey(eventType))
        {
            Delegate d = eventTable[eventType];

            if (d == null)
            {
                Debug.LogError(string.Format("Attempting to remove listener with for event type \"{0}\" but current listener is null.", eventType));
            }
            else if (d.GetType() != listenerBeingRemoved.GetType())
            {
                Debug.LogError(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
            }
            else
            {
                eventTable[eventType] = (Act<T, U>)eventTable[eventType] - listenerBeingRemoved;
                if (eventTable[eventType] == null)
                {
                    eventTable.Remove(eventType);
                }
            }
        }
        else
        {
            Debug.LogError(string.Format("Attempting to remove listener for type \"{0}\" but Messenger doesn't know about this event type.", eventType));
        }
    }

    public void Dispatch<T, U>(int eventType, T param1, U param2)
    {
        Delegate d;
        if (eventTable.TryGetValue(eventType, out d))
        {
            ((Act<T, U>)d)(param1, param2);
        }
    }

    [ContextMenu("Test")]
    void Test ()
    {
        AddListener<int, string>(1, MyCallback);
        Dispatch(1, 22, "333");
        RemoveListen<int, string>(1, MyCallback);
    }

    private void MyCallback (int n, string s)
    {
        Debug.Log(string.Format("param1 {0}, parma2 {1}", n, s));
    }
}

/*
预定义多个不同参数个数的delegate，再分别重载几个Add、Remove、Dispatch支持不同类型delegate的方法就可以实现整套支持不同参数类型不同参数个数的消息管理功能了。

以上方法可以完全避免参数传递之间的拆装箱，但是稍微有点麻烦之处在于需要重载很多Add、Remove、Dispatch函数。有个简单点的作法是直接将Delegate作为这三个函数的参数而不是具体的delegate，但调用时直接传入具名函数是不能自动转换为Delegate的，需要对每个delegate作个简单的封装，
具体如下：
*/
public static class MyGenericsEvent_Fixed
{
    public delegate void Act ();
    public delegate void Act<T, U>(T t, U u);


    Dictionary<int, Delegate> eventTable = new Dictionary<int, Delegate>();

    public void AddListener (int eventType, Delegate listenerBeingAdded)
    {
        if (!eventTable.ContainsKey(eventType))
        {
            eventTable.Add(eventType, null);
        }
        Delegate d = eventTable[eventType];
        if (d != null && d.GetType() != listenerBeingAdded.GetType())
        {
            Debug.LogError(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType(), listenerBeingAdded.GetType()));
        }
        else
        {
            eventTable[eventType] = Delegate.Combine(eventTable[eventType], listenerBeingAdded);
        }
    }

    public void RemoveListen (int eventType, Delegate listenerBeingRemoved)
    {
        if (eventTable.ContainsKey(eventType))
        {
            Delegate d = eventTable[eventType];
            if (d == null)
            {
                Debug.LogError(string.Format("Attempting to remove listener with for event type \"{0}\" but current listener is null.", eventType));
            }
            else if (d.GetType() != listenerBeingRemoved.GetType())
            {
                Debug.LogError(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
            }
            else
            {
                eventTable[eventType] = Delegate.Remove(eventTable[eventType], listenerBeingRemoved);
                if (eventTable[eventType] == null)
                {
                    eventTable.Remove(eventType);
                }
            }
        }
        else
        {
            Debug.LogError(string.Format("Attempting to remove listener for type \"{0}\" but Messenger doesn't know about this event type.", eventType));
        }
    }

    public void Dispatch (int eventType, params object[] n)
    {
        Delegate d;
        if (eventTable.TryGetValue(eventType, out d))
        {
            d.DynamicInvoke(n);
        }
    }

    [ContextMenu("Test")]
    void Test ()
    {
        AddListener(1, ToDelegate<int, string>(MyCallback));
        AddListener(1, ToDelegate(MyCallback1));
        //或者
        //Delegate dele = new Action<int, string>(MyCallback);
        //AddListener(1, dele);
        Dispatch(1, 22, "333");
        RemoveListen(1, ToDelegate<int, string>(MyCallback));
        //RemoveListen(1, dele);
    }

    private Act<T, U> ToDelegate<T, U>(Act<T, U> act) { return act; }
    private Act ToDelegate(Act act) { return act; }

    private void MyCallback (int n, string s)
    {
        Debug.Log(string.Format("param1 {0}, parma2 {1}", n, s));
    }

    private void MyCallback1 ()
    {
        Debug.Log("no param");
    }
}

/*
此方法就需要对每个delegate写个简单的Getter函数（不同参数个数的原型都需要分别包装），或者new一个Action类似的Delegate（不同参数个数直接指明即可），让调用者自己给出具体的函数类别。相比方法1可以省去不少Add、Remove、Dispatch重载代码，但调用者调用时变得稍麻烦一些，同时由于Dispatch只能接受object[]参数导致了拆装箱，故还是推荐方法1。
*/