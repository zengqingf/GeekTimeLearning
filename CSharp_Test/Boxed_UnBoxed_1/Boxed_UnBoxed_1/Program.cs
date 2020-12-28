using System;

namespace Boxed_UnBoxed_1
{
    class Program
    {
        static void Main(string[] args)
        {
            Test_Int_2_String();

            Test_Enum_2_String();


            Console.ReadKey();
        }

        /*
            装箱  值类型 -> 引用类型
            拆箱  引用类型 -> 值类型

            值类型   存放于内存栈上 （stack 堆栈）  在作用域结束时释放
            引用类型 开辟内存栈的指针， 数据则存放于内存堆上 （heap 堆）     资源释放由CLR的GC处理
           
            补充下： class 和 struct
            struct 对于轻量级数据组  一些字段 属性的序列化 没有进一步抽象 继承的需求
            struct 是一种值类型 封装小型相关变量组  分配效率高
            struct 还可以包括 构造函数、常量、字段、方法、属性、索引器、运算符、事件和嵌套类型，如果需要使用上述多个字段，建议还是用class



            性能 
            装箱  值类型转换成引用类型 在内存堆栈和堆上都要开辟空间  额外增加了内存消耗  同时也增加了CPU计算

         */


        #region 值类型  ToString()

        /// <summary>
        /// int tostring 不会发生装箱
        /// </summary>
        static void Test_Int_2_String()
        {
            int a = 123;
            //public override string ToString() { return Number.FormatInt32(this, null, NumberFormatInfo.CurrentInfo);}
            string b = a.ToString();
            Console.WriteLine("(int) to string -> (string) {0}", b);
        }

        enum TestEnum
        {
            Test1,
            Test2,

            Count
        }

        /// <summary>
        /// enum tostring 会产生装箱  如果需要频繁enum 调用 tostring  需要避免
        /// 
        /// 但是要是经常使用枚举的ToString取得枚举的定义值，则不建议使用。这里是非常不合时宜的。可以直接使用静态类代替即可（使用空间换取时间）
        /// </summary>
        static void Test_Enum_2_String()
        {
            //public override string ToString() { return InternalFormat((RuntimeType) base.GetType(), this.GetValue());}
            //this.GetValue()  ->   return  (bool) *(((sbyte*) ptrRef));
            //但是传入InternalFormat 需要传一个object  产生了装箱
            string test = TestEnum.Test1.ToString();            
            Console.WriteLine("(enum) to string -> (string) {0}", test);

            //推荐：使用静态字符串数组替换
            string test1 = TestEnumStr[(int)TestEnum.Test1];
        }

        public static string[] TestEnumStr = new string[(int)TestEnum.Count]
            {
                "Test1", "Test2"
            };

        /*
        private static string InternalFormat(RuntimeType eT, object value)
        {
            //1. 使用了反射
            if (eT.IsDefined(typeof(FlagsAttribute), false))
            {
                return InternalFlagsFormat(eT, value);
            }
            string name = GetName(eT, value);
            if (name == null)
            {
                return value.ToString();
            }
            return name;
        }
        */



        #endregion
    }
}
