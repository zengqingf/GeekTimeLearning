using System;

namespace String_1
{
    /**
     * ref : https://www.jianshu.com/p/af6eb8d3d4bf
     * 
     * https://docs.microsoft.com/en-us/dotnet/api/system.string.intern?view=netcore-3.1
     * 
     * 
     * **/
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            string a = "hello world";
            string b = a;
            a = "hello";
            Console.WriteLine("{0}, {1}", a, b);
            Console.WriteLine(a == b);
            Console.WriteLine(object.ReferenceEquals(a, b));


            Console.WriteLine((a + " world") == b);  //内容一致
            Console.WriteLine(object.ReferenceEquals((a + " world"), b));  //引用不一致

            string hello = "hello";
            string helloWorld = "hello world";
            string helloWorld2 = hello + " world";

            Console.WriteLine("{0}, {1} : {2}, {3}", helloWorld, helloWorld2,
                helloWorld == helloWorld2,
                object.ReferenceEquals(helloWorld, helloWorld2));

            string helloWorld3 = "hello world";
            Console.WriteLine("{0}, {1} : {2}, {3}", helloWorld, helloWorld3,
                helloWorld == helloWorld3,
                object.ReferenceEquals(helloWorld, helloWorld3));

            Console.WriteLine("------------------------String.Intern----------------------");

            //String.Intern        将一个字符串作为参数使用这个接口  如果这个字符串已经存在池中 就返回使用这个存在的引用  如果不存在就将它加入到池中 并返回引用
            Console.WriteLine(object.ReferenceEquals(string.Intern(helloWorld), string.Intern(helloWorld2)));

            string abc = new string(new char[] { 'a', 'b', 'c'});
            object o = string.Copy(abc);
            Console.WriteLine(object.ReferenceEquals(abc, o));
            string.Intern(o.ToString());    
            Console.WriteLine(object.ReferenceEquals(o, string.Intern(abc)));
            object o2 = string.Copy(abc);
            string.Intern(o2.ToString());    //没有使用 o2 = string.Intern(o2.ToString())   只是尝试将"abc"存到内部池   o2还是一份新的引用拷贝
            Console.WriteLine(object.ReferenceEquals(o2, string.Intern(abc)));


            Console.WriteLine("------------------------String.IsInterned----------------------");

            //String.IsInterned    判断一个字符串是否在内部池中 如果传入的字符串已经在池中 则返回这个字符串对象的引用 如果不在池中 则返回null

            string s = new string(new char[] { 'x', 'y', 'z'});
            Console.WriteLine(string.IsInterned(s) ?? "not interned");
            string.Intern(s);
            Console.WriteLine(string.IsInterned(s) ?? "not interned");
            Console.WriteLine(object.ReferenceEquals(string.IsInterned(new string(new char[] { 'x', 'y', 'z'})),s));

            //加上下面这句后 上面的  Console.WriteLine(object.ReferenceEquals(string.IsInterned(new string(new char[] { 'x', 'y', 'z'})),s));
            //执行结果也是 False  下面的也是False

            //"xyz"在代码中时，CLR会将程序中的字符变量自动添加到内部池中 
            // 上文中调用 string.Intern(s)时 s没有加入到内部池中  而是一份新的字符串值
            //Console.WriteLine(object.ReferenceEquals("xyz", s));

            //使用这句替换上句时  结果一致
            //  CLR 会将 使用 + 运算符连接的字符串视为常量  ("xyz")  而string.Format需要在运行时才能知道结果
            //Console.WriteLine(object.ReferenceEquals("x" + "y" + "z", s));

            //Console.WriteLine(object.ReferenceEquals(string.Format("{0}{1}{2}", "x", "y", "z"), s));


            Console.WriteLine("------------------------ +运算符 ----------------------");

            //看看下面这句的IL代码  编译阶段识别为 xyz  并且存到Interned
            /*
             		// Console.WriteLine("xyz");
		            IL_020f: nop
		            IL_0210: ldstr "xyz"
		            IL_0215: call void [System.Console]System.Console::WriteLine(string)
             */
            Console.WriteLine("x" + "y" + "z");

            Console.ReadKey();
        }
    }
}
