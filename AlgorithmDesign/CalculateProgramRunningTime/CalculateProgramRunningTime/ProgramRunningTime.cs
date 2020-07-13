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

        public ProgramRunningTime(SimpleMethodHandler simpleMethod)
        {
            this.mSimpleSyncMethod = simpleMethod;
        }

        public double StartCalculate()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (this.mSimpleSyncMethod != null)
            {
                this.mSimpleSyncMethod();
            }
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            return ts.TotalMilliseconds;
        }
    }
}
