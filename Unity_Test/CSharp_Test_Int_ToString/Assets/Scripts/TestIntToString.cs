using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameFramework;

namespace GameClient
{
    public class TestIntToString : MonoBehaviour
    {
        public Text duration;
        public Text duration2;
        public Text duration3;

        private string headStr;
        private float timer;
        private int second_timer;

        private float timer_inner;
        private int second_timer_inner;

        private void Awake()
        {
            using(zstring.Block()){}
        }

        private void Start() 
        {
            timer = 0f;
            headStr = "当前经过的帧时间：";
        }

        private void Update() 
        {
            timer += Time.deltaTime;
            second_timer = (int)timer;
            if(duration)
            {
                duration.text = headStr + second_timer.ToString();
            }

            if(duration2)
            {
                duration2.text = headStr + _IntToZString(second_timer);
            }

            if(duration3)
            {
                //duration3.text = _StrAddZString(headStr, _IntToZString(second_timer));
                using(zstring.Block())
                {
                    timer_inner += Time.deltaTime;
                    second_timer_inner = (int)timer_inner;
                    zstring a = headStr;
                    zstring b = second_timer_inner;
                    duration3.text = zstring.Format("{0}{1}", a, b);
                }                         
            }
        }


        private static string _IntToZString(int num)
        {
            string str = "";
            using(zstring.Block())
            {
                zstring zStr = num;
                str = zStr.ToString();
            }
            return str;
        }

        private static string _StrAddZString(string str1, string str2)
        {
            string str = "";
            using (zstring.Block())
            {
                zstring a = str1;
                zstring b = a + str2;
                str = b;
            }
            return str;
        }

        private static string _FinalStrAddZString(string str1, string str2)
        {
            string str = "";
            using (zstring.Block())
            {
                zstring a = str1;
                zstring b = a + str2;
                str = b.Intern();
            }
            return str;
        }
    }
}
