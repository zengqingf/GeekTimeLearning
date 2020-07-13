using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CalculateProgramRunningTime;

namespace Array_CSharp_Learning
{
    /// <summary>
    /// 线性表 - 数组
    /// 
    /// 连续的内存空间 + 相同的数据类型
    /// 
    /// 数据支持任意访问，根据下标随机访问的时间复杂度为 O(1)
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            int[] int_array = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            int test_num = 0;

            //数组越界
            //for (int i = 0; i <= int_array.Length; i++)
            //{
            //    int_array[i] = 0;
            //    Console.WriteLine("111");
            //}

            //数组扩容搬移
            //创建时指定容量
            ArrayList arraylist = new ArrayList(9);


            ProgramRunningTime runningTime = new ProgramRunningTime(ForeachArray);
            Console.WriteLine(runningTime.StartCalculate());

            Console.ReadKey();
        }


        public static void ForeachArray()
        {
            int[] int_array = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            for (int i = 0; i < int_array.Length; i++)
            {
                Console.WriteLine("array index: {0}, value: {1}", i, int_array[i]);
            }
        }
    }
}
