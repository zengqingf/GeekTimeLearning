using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CalculateProgramRunningTime;

namespace Hashtable_CSharp
{
    /**
     * 1. 单线程程序中推荐使用 Dictionary, 有泛型优势, 且读取速度较快, 容量利用更充分.
     * 2. 多线程程序中推荐使用 Hashtable, 
     *    默认的 Hashtable 允许单线程写入, 多线程读取, 对 Hashtable 进一步调用 Synchronized() 方法可以获得完全线程安全的类型. 
     *    而 Dictionary 非线程安全, 必须人为使用 lock 语句进行保护, 效率大减.
     * 
     * 3. Dictionary 有按插入顺序排列数据的特性 (注: 但当调用 Remove() 删除过节点后顺序被打乱), 
     *    因此在需要体现顺序的情境中使用 Dictionary 能获得一定方便.
     * 
     * 4. 
     * 
     * 
     * 
     * 
     * 
     * **/


    class Hashtable_1
    {
        Hashtable ht = new Hashtable();
        Dictionary<string, int> dic = new Dictionary<string, int>();
        int testCount = 1000000;

        ProgramRunningTime prt = new ProgramRunningTime();

        public void PrintTest1Time()
        {
            prt.ResetHandler(_TestHashtableAddNum);
            Console.WriteLine("Hashtable add num, time : {0}", prt.StartCalculate());

            prt.ResetHandler(_TestDictionaryAddNum);
            Console.WriteLine("Dictionary add num, time : {0}", prt.StartCalculate());

            prt.ResetHandler(_TestHashtableContainNum);
            Console.WriteLine("Hashtable contain num, time : {0}", prt.StartCalculate());

            prt.ResetHandler(_TestDictionaryContainNum);
            Console.WriteLine("Dictionary contain num, time : {0}", prt.StartCalculate());
        }

        private void _TestHashtableAddNum()
        {
            for (int i = 0; i < testCount; i++)
            {
                ht.Add(i.ToString(), i);
            }
        }

        private void _TestDictionaryAddNum()
        {
            for (int i = 0; i < testCount; i++)
            {
                dic.Add(i.ToString(), i);
            }
        }

        private void _TestHashtableContainNum()
        {
            for (int i = 0; i < testCount; i++)
            {
                ht.ContainsKey(i.ToString());
            }
        }

        private void _TestDictionaryContainNum()
        {
            for (int i = 0; i < testCount; i++)
            {
                dic.ContainsKey(i.ToString());
            }
        }
    }
}
