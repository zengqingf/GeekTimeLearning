using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.Assertions.Comparers;

namespace GameClient
{
    /*
    
        monobehaviour enabled 不影响协程的执行
        gameObject SetActive false 会完全停止协程 重新 SetActive true也不能继续执行协程

        虽然协程是在  Monobehaviour中StartCoroutine 启动的   但是协程函数的 地位 和Monobehaviour 是一个层次  都受到 gameObject的控制  

        可能和monobehaviour一样 都是 每帧 “轮询”  yield条件是否满足
    

        waitForSeconds  受 Time.timeScale影响   yield return new WaitForSecond(x) 不会满足  不会进入其之后的流程 ！！！


        yield 后文 在 update 之后  lateupdate 之前执行
    
    */

    public class Coroutine : MonoBehaviour
    {
        private bool isStartCall = false;  //Makesure Update() and LateUpdate() Log only once  
        private bool isUpdateCall = false;
        private bool isLateUpdateCall = false;
        
        private bool isFixedUpdateCall = false;

        private void Awake() 
        {
            // Time timeScale 为0时  yield return new WaitForSecond(x) 不会满足  不会进入其之后的流程
            // Time.timeScale = 0;    
        }


        /*
         Awake : this gameObject is active, this monobehaviour is not enabled
         OnApplicationPause : False
        
         帧1,Start Begin
         帧1,Start Call Begin
         帧1,This is Start Coroutine Call Before
         帧1,Start Call End
         帧1,Start End

         帧2,This is Start Coroutine Call After yield null

         帧49,This is Start Coroutine Call After
         帧49,This is Second Start Coroutine Call Before

         帧110,This is Second Start Coroutine Call After
         帧110,This is Start Coroutine Call Second Start Coroutine Call After
            
         OnApplicationPause : True
             */


        // Use this for initialization  
        void Start()
        {
            Log.I("Start Begin");
            if (!isStartCall)
            {
                Log.I("Start Call Begin");
                StartCoroutine(StartCoroutine());
                Log.I("Start Call End");
                isStartCall = true;
            }
            Log.I("Start End");
        }
        IEnumerator StartCoroutine()
        {

            Log.I("This is Start Coroutine Call Before");

            yield return null;
            Log.I("This is Start Coroutine Call After yield null");

            yield return TestYielders.GetWaitForSeconds(1f);
            Log.I("This is Start Coroutine Call After");

            yield return StartCoroutine(StartCoroutine2());
            //注意下面日志需要等上面 StartCoroutine2 完全执行完后才进入 ！！！
            Log.I("This is Start Coroutine Call Second Start Coroutine Call After");

        }

        IEnumerator StartCoroutine2()
        {
            Log.I("This is Second Start Coroutine Call Before");
            yield return TestYielders.GetWaitForSeconds(1f);
             Log.I("This is Second Start Coroutine Call After");
        }


        // Update is called once per frame  
        void Update()
        {
            return;

            Log.I("Update Begin");
            if (!isUpdateCall)
            {
                Log.I("Update Call Begin");
                StartCoroutine(UpdateCoroutine());
                Log.I("Update Call End");
                isUpdateCall = true;

                //this.enabled = false;                         //还能打印出  "This is Update Coroutine Call Second" 及其后面的日志  
                //this.gameObject.SetActive(false);               //不能打印出  "This is Update Coroutine Call After"  及其后面的日志
            }
            Log.I("Update End");
        }
        IEnumerator UpdateCoroutine()
        {
            Log.I("This is Update Coroutine Call Before");
            yield return TestYielders.GetWaitForSeconds(1f);
            Log.I("This is Update Coroutine Call After");
            yield return TestYielders.GetWaitForSeconds(1f);
            Log.I("This is Update Coroutine Call Second");
        }
        void LateUpdate()
        {
            return;

            Log.I("LateUpdate Begin");
            if (!isLateUpdateCall)
            {
                Log.I("LateUpdate Call Begin");
                StartCoroutine(LateCoroutine());
                Log.I("LateUpdate Call End");
                isLateUpdateCall = true;
            }
            Log.I("LateUpdate End");
        }
        IEnumerator LateCoroutine()
        {
            Log.I("This is Late Coroutine Call Before");
            yield return TestYielders.GetWaitForSeconds(1f);
            Log.I("This is Late Coroutine Call After");
        }

        private void FixedUpdate() 
        {
            return;

            Log.I("FixedUpdate Begin");
            if(!isFixedUpdateCall)
            {
                Log.I("FixedUpdate Call Begin");
                StartCoroutine(FixedCoroytine());
                Log.I("FixedUpdate Call End");
                isFixedUpdateCall = true;
            }
            Log.I("FixedUpdate End");
        }

        IEnumerator FixedCoroytine()
        {
            Log.I("This is Fixed Coroutine Call Before");
            yield return TestYielders.GetWaitForSeconds(1f);
            Log.I("This is Fixed Coroutine Call After");
        }
    }

    public static class TestYielders
    {
        static Dictionary<float, WaitForSeconds> _WaitForSecondsYielders = new Dictionary<float, WaitForSeconds>(100, new FloatComparer());
        static Dictionary<float, WaitForSecondsRealtime> _WaitForSecondsRealtimeYielders = new Dictionary<float, WaitForSecondsRealtime>(100, new FloatComparer());

        static bool nullEnable = false;
        
        public static bool Enabled = true;
        static WaitForEndOfFrame _endOfFrame = new WaitForEndOfFrame();
        public static WaitForEndOfFrame EndOfFrame
        {
            get{ return Enabled ? _endOfFrame : new WaitForEndOfFrame();}
        }

        static WaitForFixedUpdate _fixedUpdate = new WaitForFixedUpdate();
        public static WaitForFixedUpdate FixedUpdate
        {
            get{ return Enabled ? _fixedUpdate : new WaitForFixedUpdate();}
        }

        public static WaitForSeconds GetWaitForSeconds(float seconds)
        {
            if(_WaitForSecondsYielders == null)
            {
                _WaitForSecondsYielders = new Dictionary<float, WaitForSeconds>();
            }

            WaitForSeconds waitForSeconds;
            if(!_WaitForSecondsYielders.TryGetValue(seconds, out waitForSeconds))
            {
                _WaitForSecondsYielders.Add(seconds, waitForSeconds = new WaitForSeconds(seconds));
            }
            return nullEnable ? null : waitForSeconds;
        }
    }


    public static class Log
    {
        public static void I(string content)
        {
            Debug.LogFormat("帧{0},{1}", Time.frameCount, content);
        }
    }
}
