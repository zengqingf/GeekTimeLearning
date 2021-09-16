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
    /// 
    /// https://docs.microsoft.com/zh-cn/dotnet/csharp/programming-guide/arrays/
    /// https://www.runoob.com/csharp/csharp-array.html
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            int[] int_array = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };    

            //数组越界
            //for (int i = 0; i <= int_array.Length; i++)
            //{
            //    int_array[i] = 0;
            //    Console.WriteLine("111");
            //}

            //数组扩容搬移
            //创建时指定容量
            //ArrayList arraylist = new ArrayList(9);


            //ProgramRunningTime runningTime = new ProgramRunningTime(ForeachArray);
            //Console.WriteLine(runningTime.StartCalculate());


            //多维数组(矩形数组)
            int[,] array_r4_c2 = new int[4, 2];
            int[,] array_2D_r4_c2 = new int[,] { 
                { 1, 2 },  //[0, 0] [0, 1]
                { 3, 4 },  
                { 5, 6 },  //[2, 0] [2, 1]
                { 7, 8 } };
            int[,] array_2Da_r4_c2 = new int[4, 2] { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
            int[,] array_2Db_r4_c2 = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };

            Console.WriteLine(array_2D_r4_c2[2,1]);
            array_2D_r4_c2[2, 1] = 524;
            Console.WriteLine(array_2D_r4_c2[2, 1]);


            var rank = array_2D_r4_c2.Rank;
            Console.WriteLine("维数：{0}", rank);
            Console.WriteLine("第1维有{0}个元素", array_2D_r4_c2.GetLength(0));
            Console.WriteLine("第2维有{0}个元素", array_2D_r4_c2.GetLength(1));
            for (int i = 0; i < array_2D_r4_c2.GetLength(0); i++)
            {
                for (int j = 0; j < array_2D_r4_c2.GetLength(1); j++)
                {
                    Console.WriteLine("序号[{0}, {1}]的值为{2}", i, j, array_2D_r4_c2[i,j]);
                }
            }

            int[,,] array_3D = new int[2, 2, 3];
            int[,,] array_3Da = new int[,,] { 
                { { 1, 2, 3 }, { 4, 5, 6 } }, 
                { { 7, 8, 9 }, { 10, 11, 12 } }
            };
            int[,,] array_3Db = new int[2,2,3] {
                { { 1, 2, 3 }, { 4, 5, 6 } },
                { { 7, 8, 9 }, { 10, 11, 12 } }
            };
            Console.WriteLine(array_3Da[0, 0, 0]); //1
            Console.WriteLine(array_3Da[1, 0, 0]); //7
            Console.WriteLine(array_3Da[0, 1, 0]); //4
            Console.WriteLine(array_3Da[0, 0, 1]); //2
            //画个长方体，长（长指从左到右）宽高分别为3，2，2， 前一个面是维度1  后一个面是维度2            
            //假设1在前一个面的左上角，向右依次是 1，2，3；向下依次是4，5，6
            //后一个面，1对面的是7，向右依次是7，8，9；4对面是10，向右依次是10，11，12

            var array3DLength = array_3Da.Length;
            var total = 1;
            Console.WriteLine(array3DLength);
            for (int i = 0; i < array_3Da.Rank; i++)
            {
                total *= array_3Da.GetLength(i);
            }
            Console.WriteLine("{0} = {1}", array3DLength, total);


            //交错数组
            //元素为数组的数组
            //是一维数组
            //[][] 这两个括号 但不是二维数组
            int[][] jaggedArray = new int[3][];
            //必须初始化交错数组的元素才能使用
            jaggedArray[0] = new int[5];
            jaggedArray[1] = new int[2];
            jaggedArray[2] = new int[4];

            jaggedArray[0] = new int[] { 1,3,5,7,9};
            jaggedArray[1] = new int[] { 11, 12 };
            jaggedArray[2] = new int[] { 0, 2, 4, 6 };

            int[][] jaggedArray_a = new int[][]
                {
                    new int[] { 1,3,5,7,9},
                    new int[] { 11, 12 },
                    new int[] { 0, 2, 4, 6 }
                };

            int[][] jaggedArray_b =
                {
                    new int[] { 1,3,5,7,9},
                    new int[] { 11, 12 },
                    new int[] { 0, 2, 4, 6 }
                };

            //jaggedArray_a[0][1]可以理解为 (jaggedArray_a[0])[1]  先计算jaggedArray_a[0] 是一个数组，然后获取这个数组的第2个元素
            Console.WriteLine("jagged array [0][1] , value : {0}",jaggedArray_a[0][1]);
            Console.WriteLine("jagged array [2][1] , value : {0}", jaggedArray_a[2][1]);

            Console.WriteLine("jagged array [1][1] , value : {0}", jaggedArray_a[1][1]);
            jaggedArray_a[1][1] = 524;
            Console.WriteLine("jagged array [1][1] , value : {0}", jaggedArray_a[1][1]);


            int[][,] jaggedArray_c = new int[3][,]
                {
                    new int[,] { {1,3}, {5,7} },
                    new int[,] { {0,2}, {4,6}, {8,10} },
                    new int[,] { {11,22}, {99,88}, {0,9} }
                };

            Console.WriteLine("jagged array [0][0, 1] , value : {0}", jaggedArray_c[0][0, 1]);
            Console.WriteLine("jagged array [1][1, 0] , value : {0}", jaggedArray_c[1][1, 0]);
            Console.WriteLine("jagged array length : {0}", jaggedArray_c.Length);
            Console.WriteLine("jagged array rank : {0}", jaggedArray_c.Rank);

            for (int i = 0; i < jaggedArray_a.Length; i++)
            {
                Console.Write("element({0}): ", i);
                for (int j = 0; j < jaggedArray_a[i].Length; j++)
                {
                    Console.Write("{0}{1}", jaggedArray_a[i][j], j==(jaggedArray_a.Length - 1)?"":" ");
                }
                Console.WriteLine();
            }


            //内存寻址
            //a[k]_address = base_address + k * type_size   type_size指偏移量
            // m * n 二维数组  i < m ; j < n
            //a[i][j]_address = base_address + (i * n + j) * type_size
            //m * n * q 三维数组  i < m ; j < n ; k < q
            //a[i][j][k]_address = base_address + (i * n * q + j * q + k) * type_size


            SequenceList<int> seqList = new SequenceList<int>();
            for (int i = 0; i < 10; i++)
            {
                seqList.Append(i);
            }
            Console.WriteLine("顺序表数量：" + seqList.Count);
            Console.WriteLine("顺序表容量：" + seqList.Capacity);

            Console.WriteLine("顺序表内容:" + seqList.ToString());

            seqList.InsertBefore(10, 0);
            seqList.InsertAfter(11, 3);
            seqList.InsertBefore(12, 11);
            seqList.InsertBefore(13, 13);
            seqList.InsertAfter(14, 13);

            ProgramRunningTime pRunningTime = new ProgramRunningTime(()=> {
                seqList.Capacity = 10000;
            });
            Console.WriteLine("顺序表扩容耗时：" + pRunningTime.StartCalculate());

            Console.WriteLine("顺序表数量：" + seqList.Count);
            Console.WriteLine("顺序表容量：" + seqList.Capacity);

            Console.WriteLine("顺序表内容:" + seqList.ToString());

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
