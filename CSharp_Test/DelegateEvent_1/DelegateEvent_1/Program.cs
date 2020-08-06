using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelegateEvent_1
{
    class Program
    {
        static void Main(string[] args)
        {
            TestEventArgs e = new TestEventArgs("Hello World");
            DelegateInterface.Instance.Invoke(e);
            Console.ReadKey();
        }
    }

    public class SubDelegateInterface : DelegateInterface, IDelegateInterface
    {
        public override void Init()
        {
            Register(this);
            Register(this);
            //Detach(this);          
            DetachAll();
        }

        public override void Invoke(TestEventArgs args)
        {
            base.Invoke(args);
        }

        public void OnEventCallback(object sender, TestEventArgs e)
        {
            if (e != null)
            {
                Console.WriteLine("[Program - OnEventCallback] sender is {0}, args is {1}", sender.ToString(), e.str);
            }
        }
    }

    public class DelegateInterface
    {
        private static DelegateInterface instance;
        public static DelegateInterface Instance
        {
            get {
                if (instance == null)
                {
                    instance = new SubDelegateInterface();
                    instance.Init();
                }
                return instance;
            }
        }

        public virtual void Init()
        {
             
        }

        private List<EventHandler<TestEventArgs>> testDels = new List<EventHandler<TestEventArgs>>();

        private event EventHandler<TestEventArgs> TestEventHandler; //= delegate { }; //add emoty delegate

        public event EventHandler<TestEventArgs> TestEvent
        {
            add {
                TestEventHandler += value;
                testDels.Add(value);
            }
            remove {
                TestEventHandler -= value;
                testDels.Remove(value);
            }
        }

        public void RemoveAllEvents()
        {
            foreach (EventHandler<TestEventArgs> eDel in testDels)
            {
                TestEventHandler -= eDel;

                //Delegate.RemoveAll(TestEventHandler, eDel);  //不可用
            }
            ShowTestEventHandleInvocationsCount();
            testDels.Clear();
        }

        public void Register(IDelegateInterface imple)
        {
            if (imple == null)
            {
                return;
            }
            //TestEventHandler += imple.OnEventCallback;            
            TestEvent += imple.OnEventCallback;
            ShowTestEventHandleInvocationsCount();
        }

        public void Detach(IDelegateInterface imple)
        {
            if (imple == null)
            {
                return;
            }
            //TestEventHandler -= imple.OnEventCallback;
            TestEvent -= imple.OnEventCallback;
            ShowTestEventHandleInvocationsCount();
        }

        public void DetachAll()
        {
            //TestEvent = null; //wrong

            RemoveAllEvents();

            //TestEventHandler = null;
        }

        public virtual void Invoke(TestEventArgs args)
        {
            //TestEventHandler += (obj, e) => { };   //also a delegate to event

            //需要赋值当前引用 如果多线程中 TestEventHandler 被修改 可以避免空引用
            EventHandler<TestEventArgs> handler = TestEventHandler;
            if (handler != null && args != null)
            {
                handler(this, args);
            }
            ShowTestEventHandleInvocationsCount();
        }

        private void ShowTestEventHandleInvocationsCount()
        {
            //Console.WriteLine("TestEventArgs Invocation list count is {0}", TestEventHandler.GetInvocationList().Length);
        }
    }

    public class TestEventArgs : EventArgs
    {
        public string str;
        public TestEventArgs(string str)
        {
            this.str = str;
        }
    }

    public interface IDelegateInterface
    {
        void OnEventCallback(object sender, TestEventArgs e);
    }
}
