using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genericity_1
{
    class Program
    {
        static void Main(string[] args)
        {
            //var array = new ArrayList();
            //array.Add(1);
            //array.Add(2);

            //存值和遍历时 会产生box和unbox
            //打印时 会产生 box
            //foreach (int v in array)
            //{
            //    Console.WriteLine("ArrayList value is {0}", v);
            //}

            //在v之后 不作为 int类型使用时 使用var 能减少两次拆箱过程
            //以及 打印时 不产生box
            //foreach (var v in array)
            //{
            //    Console.WriteLine("ArrayList value is {0}", v);
            //}

            //----------------------------------------------------------------------//

            //var list = new List<int>();
            //list.Add(1);
            //list.Add(2);

            //打印时产生box
            //foreach (int v in list)
            //{
            //    Console.WriteLine("List<int> value is {0}", v);
            //}

            //打印时产生box
            //foreach (var v in list)
            //{
            //    Console.WriteLine("List<int> value is {0}", v);
            //}



            //-----------------------------------------------------------------------------//



            //对于值类型，实现基类的虚方法和IEquatable<T>接口对于避免装箱十分有必要 !!!
            MyStruct myS1 = new MyStruct();
            myS1.myNum = 1;
            MyStruct myS2 = new MyStruct();
            myS2.myNum = 1;
            
            bool res = MyFunc<MyStruct>(myS1, myS2);
            Console.WriteLine("MyFunc MyStruct result is {0}", res);

            Console.ReadKey();
        }

        //添加T类型约束 必须加约束 才能
        static bool MyFunc<T>(T obj1, T obj2) where T : IEquatable<T>
        {
            return obj1.Equals(obj2);
        }

        //这样会对obj2产生装箱  obj1不装箱 被加入前缀 constrained 指令 （会判断obj1类型定义中是否存在重写的Equals方法，否则，则装箱后调用基类ValueType中的虚方法 ）
        //为了避免obj2装箱 需要实现IEquatable<T>
        public struct MyStruct : IEquatable<MyStruct>
        {
            public int myNum;
            public override bool Equals(object obj)  //调用时会对实参装箱
            {
                if (!(obj is MyStruct))
                {
                    return false;
                }
                MyStruct other = (MyStruct)obj; //拆箱
                return this.myNum == other.myNum;
            }

            public bool Equals(MyStruct other)  //重载Equals方法，避免装箱          //实现了IEquatable<T>中的方法       
            {
                return this.myNum == other.myNum;
            }

            public static bool operator ==(MyStruct myS, MyStruct otherS)
            {
                return myS.Equals(otherS);
            }

            public static bool operator !=(MyStruct myS, MyStruct otherS)
            {
                return myS.Equals(otherS);
            }
        }
    }
}
