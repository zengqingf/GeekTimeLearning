using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalculateProgramRunningTime;


namespace LinkedList_CSharp_Learning
{
    class Program
    {
        static LinkedList<int> linkedList_int_a = new LinkedList<int>();

        static void Main(string[] args)
        {
            for (int i = 0; i < 10; i++)
            {
                linkedList_int_a.Append(i);
            }

            linkedList_int_a.InsertBefore(10, 0);
            linkedList_int_a.InsertAfter(11, 1);
            linkedList_int_a.InsertBefore(12, 11);
            linkedList_int_a.InsertAfter(13, 12);
            Console.WriteLine("插入操作结果：" + linkedList_int_a.ToString());


            linkedList_int_a.RemoveAt(14);
            linkedList_int_a.RemoveAt(1);
            linkedList_int_a.Remove(11);
            Console.WriteLine("删除操作结果：" + linkedList_int_a.ToString());

            var getIndex1 = linkedList_int_a.GetItemAt(1);
            Console.WriteLine("获取序号1元素值：" + getIndex1);

            int indexOf3 = linkedList_int_a.IndexOf(3);
            Console.WriteLine("获取元素值为3的序号：" + indexOf3);

            bool contain9 = linkedList_int_a.Contains(9);
            Console.WriteLine("是否包含元素9：" + contain9);

            Console.WriteLine("结果：" + linkedList_int_a.ToString());
            ProgramRunningTime runningTime = new ProgramRunningTime(_ReverseLinkList);
            Console.WriteLine("反转耗时："+ runningTime.StartCalculate());
            Console.WriteLine("反转后：" + linkedList_int_a.ToString());
            Console.ReadKey();
        }

        private static void _ReverseLinkList()
        {
            if (linkedList_int_a != null)
            {
                linkedList_int_a.Reverse();
            }
        }
    }
}
