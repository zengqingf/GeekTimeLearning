using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using System;

namespace GameClient
{
/*
1·在遇到yield break或者返回IEnumerator的函数体结束前，不管yield return 的值为多少，MoveNext都是会返回True。

2·在第一次调用MoveNext之前，返回IEnumerable的代码都不会执行，即使你有主动去调用它。

3·执行到yield return的地方，代码就暂停了，并返回相应的值，在下一次调用MoveNext时，从上次暂停的地方继续执行。

4·yield return 代码不能放入try...catch块中，但是能放入try...finally块中。


之前我遇到因为在协程里面用try-catch，catch没有捕获异常，然后就卡死了。具体代码如下
public IEnumerator _Co()
{
		try
		{
				// 这里有很多的流程，很多的嵌套，很多的yield
		}
		catch // 这里没有写 (Exception e) 之类的，或者写了，下面没有输出这个e 
		{
				// 这里什么都没写
		}
}

建议写法

public IEnumerator _Co()
{
		_process1();
		yield return null;
		...
		...
		_process2();
		...
		...
		_process3()		
}

private void _process1()
{
		try
		{
				...
		}
		catch(Exception e)
		{
				print e
		}
}

*/


    public class StopCoroutine : MonoBehaviour
    {

        List<int> m_List = null;
        Dictionary<int, int> m_dict = null;
        bool isLateUpdate = false;
        bool isUpdate = false;

        UnityEngine.Coroutine coroutine = null;

        void Awake() {
            m_List = new List<int>(3){0, 1, 2};
            m_dict = new Dictionary<int, int>(3){

                {0, 1}, {1, 2}, {2, 3}
            };
        }
        void Start(){
            //StartCoroutine(MyCoroutine());

            //coroutine = StartCoroutine(Coroutine());

            //StartCoroutine(Coroutine1());

            //ErrorMethod();

            RightMethod();
        }

        void Update() {
            if(!isUpdate)
            {
                //StopCoroutine(Coroutine());  //这样不能停

                //需要判空
                if(coroutine != null)
                {
                    StopCoroutine(coroutine);
                    Log.I("StopCoroutine in Update");
                }

                isUpdate = true;
            }
        }

        void LateUpdate() {
            if(!isLateUpdate)
            {
                isLateUpdate = true;
                //StartCoroutine(MyCoroutine());
            }
        }

        IEnumerator MyCoroutine()
        {
            Log.I("MyCoroutine region 1 ");

            //一下几种都不可行  无法正常迭代
            //yield return m_List.GetEnumerator().Current;
            //yield return m_dict.GetEnumerator().Current.Value;
            //yield return new MyListIterator(m_List);

            yield return new MyListIterator(m_List).GetEnumerator();

            Log.I("MyCoroutine region 2 ");
        }


        //测试 
        // 当等待一个AsyncRequest时 等待时该AsyncRequest 是否已执行完成会影响该协程下一次被调用的时机
        // 若未完成，会在AsyncRequest完成那一帧 FixedUpdate之前调用 
        // 若已完成，则直接会在下一帧的Update之后调用 
        IEnumerator Coroutine1()
        {
            yield return null;
            Log.I("After yield return null");
            yield return Resources.LoadAsync<GameObject>("Plane");
            Log.I("After Resources.LoadAsync");
            ResourceRequest asyncReq = Resources.LoadAsync<GameObject>("Plane1");
            yield return new WaitForSeconds(1f);
            Log.I("After WaitForSeconds");
            yield return asyncReq;
            Log.I("After asyncReq");
        }




        //测试 StopCoroutine 嵌套协程 的两种情况
        IEnumerator Coroutine()
        {
            Log.I("Coroutine1 region 1");
            //yield return Coroutine2();   //停掉Coroutine Coroutine2也会停
            yield return StartCoroutine(Coroutine2());    //停掉Coroutine  Coroutine2不会停
            Log.I("Coroutine1 region 2");
        }

        IEnumerator Coroutine2()
        {
            Log.I("Coroutine2 region 1");
            yield return Resources.LoadAsync<GameObject>("Plane");
            Log.I("Coroutine2 region 2");
        }


        //测试需要同步执行完 嵌套协程中的所有流程
        // 迭代所有IEnumerator

        IEnumerator Coroutine3()
        {
            Log.I("Coroutine3 region 1");
            yield return "Hello";
            Log.I("Coroutine3 region 2");
            //yield return Coroutine2();   //停掉Coroutine Coroutine2也会停
            //yield return StartCoroutine(Coroutine4());    //停掉Coroutine  Coroutine2不会停
            yield return Coroutine4();
            Log.I("Coroutine3 region 3");
        }

        IEnumerator Coroutine4()
        {
            Log.I("Coroutine4 region 1");
            yield return "1";
            Log.I("Coroutine4 region 2");
            yield return "2";
            Log.I("Coroutine4 region 3");
        }


        void ErrorMethod()
        {
            IEnumerator itr = Coroutine3();
            while(itr.MoveNext())
            {
                Log.I(string.Format("Current1 value is {0}", itr.Current.ToString()));
            }
        }

        void RightMethod()
        {
            IEnumerator itr = Coroutine3();
            while(itr.MoveNext())
            {
                Log.I(string.Format("Current1 value is {0}", itr.Current.ToString()));
                if(itr.Current is IEnumerator)
                {
                    IEnumerator itr2 = itr.Current as IEnumerator;
                    while(itr2.MoveNext())
                    {
                        Log.I(string.Format("Current2 value is {0}", itr2.Current.ToString()));
                    }
                }
            }
        }

    }

    public class MyListIterator : IEnumerable
    {
         List<int> m_List = null;
         public MyListIterator(List<int> list)
         {
             m_List = list;
         }

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < m_List.Count; i++)
            {
                yield return m_List[i];
            }
        }
    }


    
}
