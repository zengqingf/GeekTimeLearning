


namespace Tenmove.Runtime
{
    public partial class AutoTest
    {
        public class TestCase
        {
            private readonly AutoTestFunc m_MainFunc;
            private readonly AutoTestChecker m_ResultChecker;

            public TestCase(AutoTestFunc main, AutoTestChecker checker)
            {
                Debugger.Assert(null != main, "Auto test function can not be null!");
                Debugger.Assert(null != checker, "Auto test result checker function can not be null!");

                m_MainFunc = main;
                m_ResultChecker = checker;
            }

            public void Begin()
            {
                try
                {
                    m_MainFunc();
                }
                catch (BreakPoint bp)
                {
                    Debugger.LogInfo("Break point!");
                }
            }

            public bool End()
            {
                return m_ResultChecker();
            }
        }
    }
}