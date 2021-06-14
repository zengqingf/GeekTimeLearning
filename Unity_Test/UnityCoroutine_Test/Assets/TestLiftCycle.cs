using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

namespace GameClient
{


    /*
        Monobehaviour not enabled时  Awake() 方法还是会执行  但GameObject not active 时  Awake和及其他生命周期函数都不会执行

        
    

    */
    public class TestLiftCycle : MonoBehaviour
    {
        //Test FixedUpdate vs Update
        private float updateCount = 0;
        private float fixedUpdateCount = 0;
        private float updateUpdateCountPerSecond;
        private float updateFixedUpdateCountPerSecond;
        IEnumerator _Loop()
        {
            while(true)
            {
                yield return new WaitForSeconds(1f);
                updateUpdateCountPerSecond = updateCount;
                updateFixedUpdateCountPerSecond = fixedUpdateCount;

                updateCount = 0;
                fixedUpdateCount = 0;
            }
        }

        private void Reset() {
            Debug.Log("Reset");
        }

        //Unity life cycle
        void Awake()
        {
            Debug.LogFormat("Awake : this gameObject is {0}, this monobehaviour is {1}", 
            this.gameObject.activeSelf && this.gameObject.activeInHierarchy ? "active" : "not active",
            this.enabled ? "enabled" : "not enabled");


            Application.targetFrameRate = 10;
            StartCoroutine(_Loop());
        }

        private void OnEnable() {
            Debug.Log("OnEnable");
        }
        
        //Unity life cycle
        void Start () 
        {
            Debug.Log("Start");
        }

        private void FixedUpdate() {
            Debug.Log("FixedUpdate, delta time : " + Time.deltaTime);

            fixedUpdateCount += 1;            
        }
        
        private void OnTriggerEnter(Collider other) {
            Debug.Log("OnTriggerEnter");
        }

        private void OnTriggerExit(Collider other) {
            Debug.Log("OnTriggerExit");
        }

        private void OnTriggerStay(Collider other) {
            Debug.Log("OnTriggerStay");
        }

        private void OnCollisionEnter(Collision other) {
            Debug.Log("OnCollisionEnter");
        }

        private void OnCollisionExit(Collision other) {
            Debug.Log("OnCollisionExit");
        }

        private void OnCollisionStay(Collision other) {
            Debug.Log("OnCollisionStay");
        }

        private void OnMouseEnter() {
            Debug.Log("OnMouseEnter");
        }

        private void Update() {
            Debug.Log("Update, delta time : " + Time.deltaTime);

            updateCount +=1;
        }

        private void LateUpdate() {
            Debug.Log("LateUpdate");
        }

        private void OnDrawGizmos() {
            Debug.Log("OnDrawGizmos");
        }

        bool isGUI = true;
        private void OnGUI() {  
            if(isGUI)
            {
                Debug.Log("OnGUI");
                isGUI = false;
            }


            GUIStyle fontSize = new GUIStyle(GUI.skin.GetStyle("label"));
            fontSize.fontSize = 24;
            GUI.Label(new Rect(100, 100, 200, 50), "Update: " + updateUpdateCountPerSecond.ToString(), fontSize);
            GUI.Label(new Rect(100, 150, 200, 50), "FixedUpdate: " + updateFixedUpdateCountPerSecond.ToString(), fontSize);
        }

        private void OnApplicationPause(bool pauseStatus) {
            Debug.Log("OnApplicationPause : "+pauseStatus);
        }

        private void OnDisable() {
            Debug.Log("OnDisable");
        }

        private void OnDestroy() {
            Debug.Log("OnDestroy");
        }

        private void OnApplicationQuit() {
            Debug.Log("OnApplicationQuit");
        }
    }
}
