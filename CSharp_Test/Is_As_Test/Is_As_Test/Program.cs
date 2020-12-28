using System;

namespace Is_As_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            P p = new P();

            P1 p1 = new P1();

            P2 p2 = new P2();
            

            Console.WriteLine(p is IP);
            Console.WriteLine(p1 is P);
            Console.WriteLine(p2.cP is P);

            p2.cP = p1;
            IP tP = p2.cP as IP;
            Console.WriteLine(tP is P1);

            p2.cP = p;
            Console.WriteLine(p2.cP is P1);

            p2.cP = p1;
            Console.WriteLine(p2.cP is P);

            Console.ReadKey();
        }
    }

    public interface IP { }
    public class P : IP{ }
    public class P1 : P { }
    public class P2 {
        public IP cP;
    }
    public class P3 { }
}
