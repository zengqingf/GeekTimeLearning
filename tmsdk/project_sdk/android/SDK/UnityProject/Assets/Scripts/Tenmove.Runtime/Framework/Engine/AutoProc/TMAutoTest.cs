


using System.Collections.Generic;

namespace Tenmove.Runtime
{
    public delegate void AutoTestFunc();
    public delegate bool AutoTestChecker();

    public partial class AutoTest
    {
        public class BreakPoint : System.Exception { public bool Terminate { set; get; } }

        static private AutoTest sm_Instance;
        private bool m_IsTestCaseRuning;

        private class ProbeDesc
        {
            private readonly uint m_CountToActive;
            private readonly float m_TimeToActive;

            private uint m_TriggerCount;
            private bool m_HasActived;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="countToActive">第几次进入到这个探针的时候触发</param>
            /// <param name="timeToActive">进入到这个探针后多长时间内触发</param>
            public ProbeDesc(uint countToActive, float timeToActive)
            {
                m_CountToActive = countToActive;


                m_TriggerCount = 0;
                m_HasActived = false;
            }

            public void Trigger()
            {
                ++m_TriggerCount;
            }

            public bool IsActive
            {
                get
                {
                    if(!m_HasActived)
                    {
                        m_HasActived = m_TriggerCount >= m_CountToActive;
                        return m_HasActived;
                    }

                    return false;
                }
            }
        }

        private readonly Dictionary<uint, ProbeDesc> m_ProbeRegTable;

        private readonly List<TestCase> m_TestCaseList;

        public AutoTest()
        {
            m_IsTestCaseRuning = false;
            m_ProbeRegTable = new Dictionary<uint, ProbeDesc>();
            m_TestCaseList = new List<TestCase>();
        }

        private static AutoTest _Instance
        {
            get
            {
                if (null == sm_Instance)
                    sm_Instance = new AutoTest();

                return sm_Instance;
            }
        }

        private static bool _IsTestCaseRuning
        {
            get { return _Instance.m_IsTestCaseRuning; }
        }


        public static void Test(AutoTestFunc func )
        {
            try
            {
                _Instance.m_IsTestCaseRuning = true;
                func();
                _Instance.m_IsTestCaseRuning = false;
            }
            catch (BreakPoint bp)
            {
                Debugger.LogInfo("Break point!");
            }
        }

        public void CreateTestCase()
        {

        }

        public static void RunTestCase(TestCase testCase)
        {
            if(null != testCase)
            {
                _Instance.m_IsTestCaseRuning = true;
                testCase.Begin();
                testCase.End();
                _Instance.m_IsTestCaseRuning = false;
            }
        }

        public static void InsertProbe(uint waterMark)
        {
#if !RELEASE
            new Probe(waterMark);
#endif
        }

        public static void ThrowProbe(System.Exception e)
        {
#if !RELEASE
            if (e.GetType() == typeof(AutoTest.BreakPoint))
                throw e;
#endif
        }

        private static bool _IsProbeActived(uint waterMark)
        {
            return _Instance.__IsProbeActived(waterMark);
        }

        private bool __IsProbeActived(uint waterMark)
        {
            return true;

            ProbeDesc desc = null;
            if (m_ProbeRegTable.TryGetValue(waterMark, out desc))
            {
                desc.Trigger();
                return desc.IsActive;
            }

            return false;
        }
    }
}