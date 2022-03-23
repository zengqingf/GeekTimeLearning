
using System;

namespace Tenmove.Runtime
{
    public partial class AutoTest
    {
        public struct Probe
        {
            public Probe(uint waterMark)
            {
                if (_IsTestCaseRuning && _IsProbeActived(waterMark))
                    throw new BreakPoint();
            }
        }
    }
}