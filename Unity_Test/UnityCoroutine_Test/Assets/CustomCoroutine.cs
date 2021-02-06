using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using System;
using UnityEngine.Networking;

namespace GameClient
{
    /*
        普通协程有以下这些缺点：
                嵌套协程依赖StartCoroutine，从而代码依赖MonoBehaviour
                执行异步逻辑的时候，需要写while等待。代码臃肿难看
                协程运行结果无法直接返回，需要另外处理，特别费力
        见：CustomCoroutine.NormalCoroutine
    
    
    */


    public class CustomCoroutine : MonoBehaviour
    {
        private void Update() {
            if(Input.GetMouseButtonUp(0))
            {
                Log.I("Left mouse button up");
                StartCoroutine(waitForMouseDown());
            }
        }

        IEnumerator waitForMouseDown()
        {
            yield return new WaitForMouseDown();
            Log.I("Right mouse button pressed");
        }



        IEnumerator NormalCoroutine()
        {

            //嵌套协程
            yield return StartCoroutine(InternalCoroutine());

            //协程等待异步逻辑完成
            var running = false;
            DoSomething(() => {
                running = true;
            });

            while(running == false)
            {
                yield return 0;
            }

            //协程返回值
            running = false;
            var rtn = "return str";
            DoSomething2(s =>{
                running = true;
                rtn = s;
            });

            while(running == false)
            {
                yield return null;
            }
        }

        void DoSomething(System.Action action)
        {
            if(action != null)
            {
                action();
            }
        }

        void DoSomething2(System.Action<string> action)
        {
            string rtn = "do some thing 2";
            if(action != null)
            {
                action(rtn);
            }
        }


        IEnumerator InternalCoroutine()
        {
            yield return null;
        }
    }

    /*
    var download = new DownloadCoroutine(...);
    yield return download;
    Debug.Log(download.Code);

    public class DownloadCoroutine : CustomYieldInstruction
    {
        public int Code = -1;

        public DownloadCoroutine(string fromPath, string toPath, OnDownloadProgressDelegate onProgress)
        {
            Debug.Log("下载文件 ： " + fromPath + " -> " + toPath);

            var httpRequest = new HTTPRequest(new Uri(fromPath), HTTPMethods.Get, (request, response) =>
            {
                if (response == null)
                {
                    Code = 500;
                    return;
                }

                Debug.Log("下载完成 ： " + response.StatusCode);
                File.WriteAllBytes(toPath, response.Data);
                Code = response.StatusCode;
            });

            httpRequest.OnProgress = onProgress;
            httpRequest.Send();
        }

        public override bool keepWaiting
        {
            get { return Code == -1; }
        }
    }
    */
    
    


    public class WaitForMouseDown : CustomYieldInstruction
    {
        public override bool keepWaiting
        {
            get{

                return !Input.GetMouseButtonDown(1);
            }
        }

        public WaitForMouseDown()
        {
            Log.I("Waiting for Mouse right button down");
        }
    }


    public class WaitWhile_1 : CustomYieldInstruction
    {
        Func<bool> m_Predicate;
        public override bool keepWaiting
        {
            get{
                if(m_Predicate != null)
                {
                    return m_Predicate();
                }
                return false;
            }
        }

        public WaitWhile_1(Func<bool> predicate)
        {
            this.m_Predicate = predicate;
        }
    }

    public class WaitWhile_2 : IEnumerator
    {
        Func<bool> m_Predicate;
        public object Current { get { return null; } }

        public bool MoveNext() { return m_Predicate(); }

        public void Reset() {}

        public WaitWhile_2(Func<bool> predicate) { m_Predicate = predicate; }
    }




    
}
