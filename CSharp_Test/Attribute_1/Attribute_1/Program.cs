using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Attribute_1
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    Console.WriteLine("[CSharp Program] - Main() args{0}, value is {1}", i, args[i]);
                }                
            }
            S1 s = new S1(); // 必须new 否则b会报错 未赋值
            s.a = new byte[64];
            
            for (int i = 0; i < 16; i++)
            {
                s.b[i] = i;
                Console.Write(s.b[i] + " ");
            }

            for (int i = 0; i < s.a.Length; i++)
            {
                Console.Write(s.a[i]);
            }


            FileStream fs = new FileStream("D:/_WorkSpace/_Git/MJX/GeekTimeLearning/CSharp_Test/Attribute_1/test.txt", FileMode.Create);
            fs.Write(s.a, 0, s.a.Length);

            Console.ReadKey();
        }

        // StructLayout使设计者可以控制类或结构的数据字段的物理布局
        // Explicit与FieldOffset一起可以控制每个数据成员的精确位置
        [StructLayout(LayoutKind.Explicit)]
        struct S1
        {
            // FieldOffset控制字段所在的物理位置偏移为0
            [FieldOffset(0)]
            public byte[] a;

            //同样偏移为0，开始位置与a重叠了
            [FieldOffset(0)]
            public int[] b;
        }
    }
}
