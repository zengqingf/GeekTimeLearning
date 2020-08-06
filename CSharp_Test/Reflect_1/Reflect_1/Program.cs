using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

namespace Reflect_1
{
    class Program
    {
        static void Main(string[] args)
        {
            //Test 1
            Assembly ass = Assembly.Load("CalculateProgramRunningTime");
            Type prtType = ass.GetType("CalculateProgramRunningTime.ProgramRunningTime");
            object aProgramRunningTime = ass.CreateInstance("CalculateProgramRunningTime.ProgramRunningTime");

            //Type prtType = Type.GetType("CalculateProgramRunningTime.ProgramRunningTime");
            //object[] constructParams = new object[] { };
            //object aProgramRunningTime = Activator.CreateInstance(prtType);

            MethodInfo printMethod = prtType.GetMethod("_TestCalculate", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
            double pt = (double)printMethod.Invoke(aProgramRunningTime, null);
            Console.WriteLine("[Test] - Print : {0}", pt);

            Console.ReadKey();
        }
    }

    public class SDKInterface
    {
        private static SDKInterface _instance;
        private static SDKInterface _createInstanceByClassName(string name)
        {
            Assembly ass = Assembly.GetAssembly(typeof(SDKInterface));
            return ass.CreateInstance(name) as SDKInterface;
        }

        private static SDKInterface _getSDKInstance()
        {
            return _createInstanceByClassName("SDKInterfaceAndroid"); // just an example
        }

        public static SDKInterface instance
        {
            get {
                if (_instance == null)
                {
                    _instance = _getSDKInstance();
                }
                return _instance;
            }
        }
    }
}
