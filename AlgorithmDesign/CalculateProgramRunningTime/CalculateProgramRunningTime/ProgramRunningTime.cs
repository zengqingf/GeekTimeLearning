using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculateProgramRunningTime
{
    public class ProgramRunningTime
    {
        public delegate void SimpleMethodHandler();
        private SimpleMethodHandler mSimpleSyncMethod;
        private Stopwatch sw = null;

        public ProgramRunningTime(SimpleMethodHandler simpleMethod)
        {
            sw = new Stopwatch();
            ResetHandler(simpleMethod);
        }

        public ProgramRunningTime()
        {
            sw = new Stopwatch();
        }

        public void ResetHandler(SimpleMethodHandler simpleMethod)
        {
            this.mSimpleSyncMethod = simpleMethod;
        }

        public double StartCalculate()
        {
            sw.Reset();
            sw.Start();
            if (this.mSimpleSyncMethod != null)
            {
                this.mSimpleSyncMethod();
            }
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            return ts.TotalMilliseconds;
        }

        private double _TestCalculate()
        {
            this.mSimpleSyncMethod = () =>
            {
                Console.WriteLine("[Test Calculate Program Running Time] - Print");
                for (int i = 0; i < 100; i++)
                {
                    Console.Write("{0} ", i);
                }
            };

            return StartCalculate();
        }

        private static string _TestPrivateStaticCalculate()
        {
            return "Invoke _TestPrivateStaticCalculate";
        }

        public static string _TestPublicStaticCalculate()
        {
            return "Invoke _TestPublicStaticCalculate";
        }
    }
}
