using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;  .NET Framework 4.x版本为4.6.1  改为 3.5  此项不可用

namespace TestLib_1
{

    /*
     如果 将生成的dll导入 Unity后报错
     Unhandled Exception: System.Reflection.ReflectionTypeLoadException: The classes in the module cannot be loaded的错误
     可能原因：如果Unity版本是5.6.4f1，支持的.NET Framework的版本为3.5。而创建的项目的默认.NET Framework 4.x版本为4.6.1。
                选择项目，然后右键选择属性 -> 应用程序，将目标框架改为 .NET Framework 3.5或以下
         
         */
    public class Class1
    {
        public static string GetTestClassName()
        {
            return "测试DLL";
        }

        public static TimeSpan GetTimespan(DateTime time)
        {
            return time - DateTime.Parse("1970/01/01");
        }
    }
}
