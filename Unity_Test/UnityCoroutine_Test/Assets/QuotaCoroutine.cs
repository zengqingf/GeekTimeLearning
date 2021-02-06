using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.Profiling;

namespace GameClient
{
    public class QuotaCoroutine : MonoBehaviour
    {
        static float frameQuotaSec = 0.001f;
        static LinkedList<IEnumerator> s_talks = new LinkedList<IEnumerator>();

        private void Start() {
            StartQuotaCoroutine(Task(1, 20)); 
            StartQuotaCoroutine(Task(2, 20));       
        }

        private void Update() {
            ScheduleTask();
        }

        void StartQuotaCoroutine(IEnumerator task)
        {
            s_talks.AddLast(task);
        }

        static void ScheduleTask()
        {
            float timeStart = Time.realtimeSinceStartup;
            while(s_talks.Count > 0)
            {
                var t = s_talks.First.Value;
                bool taskFinish = false;
                while(Time.realtimeSinceStartup - timeStart < frameQuotaSec)
                {
                    Profiler.BeginSample(string.Format("Quata Task Step, f : {0}", Time.frameCount));
                    taskFinish = !t.MoveNext();
                    Profiler.EndSample();

                    if(taskFinish)
                    {
                        s_talks.RemoveFirst();
                        break;
                    }
                }

                if(!taskFinish)
                {
                    return;
                }
            }
        }


        IEnumerator Task(int taskId, int stepCount)
        {
            int i = 0;
            while(i < stepCount)
            {
                Log.I(string.Format("{0}.{1}", taskId, i));
                i++;
                yield return null;
            }
        }
    }
}
