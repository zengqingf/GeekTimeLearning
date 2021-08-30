using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OOP_Test_1
{
    /*
     
        封装  Encapsulation  ：  访问权限控制   隐藏信息  保护数据  暴露需要的信息  减少调用者负担   提高易用性

        抽象  Abstraction   :  如何隐藏方法的具体实现  调用者只需要关心提供的功能 而非具体实现

        继承  Inheritance   : is - a 关系
         
        多态  Polymorphism  : 子类替换父类   在子类中重写  遵从了对修改关闭 对扩展开放的原则



        面向过程 vs 面向对象    ==>    面向对象拥有两个特性 （继承 + 多态）

        避免 将面向对象 写成 面向过程    1. 滥用 getter、setter 方法  2. 滥用全局变量和全局方法  （细化Constants全局数据类  拆分Utils工具类）  3. 定义数据和方法分离的类  （MVC）
    
         */






    class Program
    {
        static void Main(string[] args)
        {

        }
    }


    //****************************************************************************//


    #region 四个点

    /// <summary>
    /// GeekTime ie. 封装    
    /// 
    /// 对访问做限制、暴露有限的，单一的方法
    /// </summary>
    public class Wallet
    {
        private string id;
        private long createTime;
        private decimal balance;
        private long balanceLastModifiedTime;

        // ...省略其他属性...

        public Wallet()
        {
            this.id = "";//IdGenerator.getInstance().generate();
            this.createTime = 0;// System.currentTimeMillis(); 
            this.balance = decimal.Zero;
            this.balanceLastModifiedTime = 0;// System.currentTimeMillis();
        }

        // 注意：下面对get方法做了代码折叠，是为了减少代码所占文章的篇幅                                  
        public String getId() { return this.id; }
        public long getCreateTime() { return this.createTime; }
        public decimal getBalance() { return this.balance; }
        public long getBalanceLastModifiedTime() { return this.balanceLastModifiedTime; }

        public void increaseBalance(decimal increasedAmount)
        {
            if (increasedAmount.CompareTo(decimal.Zero) < 0)
            {                
                //throw new InvalidAmountException("...");
            }
            this.balance += (increasedAmount);
            this.balanceLastModifiedTime = 0;// System.currentTimeMillis();
        }

        public void decreaseBalance(decimal decreasedAmount)
        {
            if (decreasedAmount.CompareTo(decimal.Zero) < 0)
            {
                //throw new InvalidAmountException("...");
            }
            if (decreasedAmount.CompareTo(this.balance) > 0)
            {
                //throw new InsufficientAmountException("...");
            }
            this.balance -=(decreasedAmount);
            this.balanceLastModifiedTime = 0;// System.currentTimeMillis();
        }
    }



    // GeekTime ie. 抽象      通用 abstract /  interface 来实现

    public class Picture { }
    public class Image { }
    public class PictureMetaInfo { }

    public interface IPictureStorage
    {
        void savePicture(Picture picture);                       //命名需要抽象     getAliyunPictureUrl()不具有抽象
        Image getPicture(String pictureId);
        void deletePicture(String pictureId);
        void modifyMetaInfo(String pictureId, PictureMetaInfo metaInfo);
    }


    /*
     调用者只需要了解IPictureStorage暴露了哪些方法，而不需要了解具体实现

        其实方法（函数）本身也是抽象，通过函数的命名、注释或者文档即可了解
         
         */
    public class PictureStorage : IPictureStorage
    {
        // ...省略其他属性...
      public void savePicture(Picture picture) {}
      public Image getPicture(String pictureId) { return null;  }
      public void deletePicture(String pictureId) {  }
      public void modifyMetaInfo(String pictureId, PictureMetaInfo metaInfo) {  }
    }


    // GeekTime ie. 继承        少用继承 多用组合


    // GeekTime ie. 多态        形式： 1. 继承加方法重写   2. 接口类语法  
    
    // 3. duck-typing语法 (python ruby)   鸭子类型：不使用继承下使用多态  两个类具有相同方法，即可使用多态，不要求两个类之间有任何关系

    //形式 1
    public class DynamicArray
    {
        private static readonly int DEFAULT_CAPACITY = 10;
        protected int size = 0;
        protected int capacity = DEFAULT_CAPACITY;
        protected int[] elements = new int[DEFAULT_CAPACITY];

        public int Size() { return this.size; }
        public int Get(int index) { return elements[index]; }
        //...省略n多方法...

        public void add(int e)
        {
            ensureCapacity();
            elements[size++] = e;
        }

        protected void ensureCapacity()
        {
            //...如果数组满了就扩容...代码省略...
        }
    }

    public class SortedDynamicArray : DynamicArray
    {
        public void add(int e)
        {
            ensureCapacity();
            int i;
            for (i = size - 1; i >= 0; --i)
            { //保证数组中的数据有序
                if (elements[i] > e)
                {
                    elements[i + 1] = elements[i];
                }
                else
                {
                    break;
                }
            }
            elements[i + 1] = e;
            ++size;
        }
    }

    public class Example
    {
        public static void test(DynamicArray dynamicArray)
        {
            dynamicArray.add(5);
            dynamicArray.add(1);
            dynamicArray.add(3);
            for (int i = 0; i < dynamicArray.Size(); ++i)
            {
                Console.WriteLine(dynamicArray.Get(i));
            }
        }

        public static void main(string[] args)
        {
            DynamicArray dynamicArray = new SortedDynamicArray();
            test(dynamicArray); // 打印结果：1、3、5
        }
    }



    //形式2
    public interface Iterator
    {
        bool hasNext();
        string next();
        string remove();
    }

    public class Array : Iterator
    {
        private string[] data;

        public bool hasNext() { return false; }
        public string next() { return ""; }
        public string remove() { return ""; }
        //...省略其他方法...
    }

    public class LinkedListNode { }

    public class LinkedList : Iterator
    {
        private LinkedListNode head;

        public bool hasNext() { return false; }
        public string next() { return ""; }
        public string remove() { return ""; }
          //...省略其他方法... 
    }

    public class Demo
    {
        private static void print(Iterator iterator)
        {
            while (iterator.hasNext())
            {
                Console.WriteLine(iterator.next());
            }
        }

        public static void main(String[] args)
        {
            Iterator arrayIterator = new Array();
            print(arrayIterator);

            Iterator linkedListIterator = new LinkedList();
            print(linkedListIterator);
        }
    }

    #endregion

    #region 注意点


    /*
     1. 滥用 getter、setter 方法
     给不应该暴露的属性设置setter方法
     getter方法返回容器或者对象，外部还是可以修改容器和对象的数据 ---> 
                                                                       返回不可修改的容器 （java : Collections.unmodifiableList()）
                                                                       返回对象的拷贝 (不能是浅拷贝) 这样修改对象也不会修改源对象了
     
         



    2. 滥用全局变量和全局方法
    单例类对象  静态成员变量  常量  (Constants)  静态方法 （Utils）
         
    Constants类拆分为多个单一的类，当然最好是将各自的常量定义到属于它的类中   如 RedisConfig

    Utils也要细化成不同功能的类 不要设计大而全的类

    Utils是面向过程的  能实现方法复用  但是用之前需要考虑这些方法是否能定义到其他类中 而不是一味的使用Utils



    3. 定义数据和方法分离的类
    MVC 贫血模型
         
         */



    #endregion

    #region 抽象类和接口


    /*
     
        抽象类  is - a 
        接口    has - a

     java
     1. 抽象类 不能被 实例化  只能被继承 
     2. 抽象类 可以包含属性和方法  方法既可以包含代码实现 也可以不包含（即抽象方法）
     3. 子类继承抽象类，必须实现抽象类中的所有抽象方法

    接口不能包含属性（也就是成员变量）。
    接口只能声明方法，方法不能包含代码实现。
    类实现接口的时候，必须实现接口中声明的所有方法。


    c#
    抽象类
    (1) 抽象方法只作声明，而不包含实现，可以看成是没有实现体的虚方法
    (2) 抽象类不能被实例化
    (3) 抽象类可以但不是必须有抽象属性和抽象方法，但是一旦有了抽象方法，就一定要把这个类声明为抽象类
    (4) 具体派生类必须覆盖基类的抽象方法
    (5) 抽象派生类可以覆盖基类的抽象方法，也可以不覆盖。如果不覆盖，则其具体派生类必须覆盖它们

    接口
    (1) 接口不能被实例化
    (2) 接口只能包含方法声明
    (3) 接口的成员包括方法、属性、索引器、事件
    (4) 接口中不能包含常量、字段(域)、构造函数、析构函数、静态成员
    (5) 接口中的所有成员默认为public，因此接口中不能有private修饰符
    (6) 派生类必须实现接口的所有成员
    (7) 一个类可以直接实现多个接口，接口之间用逗号隔开
    (8) 一个接口可以有多个父接口，实现该接口的类必须实现所有父接口中的所有成员
     

    相同点：
    (1) 都可以被继承
    (2) 都不能被实例化
    (3) 都可以包含方法声明
    (4) 派生类必须实现未实现的方法
    区 别：
    (1) 抽象基类可以定义字段、属性、方法实现。接口只能定义属性、索引器、事件、和方法声明，不能包含字段。
    (2) 抽象类是一个不完整的类，需要进一步细化，而接口是一个行为规范。微软的自定义接口总是后带able字段，证明其是表述一类“我能做。。。”
    (3) 接口可以被多重实现，抽象类只能被单一继承
    (4) 抽象类更多的是定义在一系列紧密相关的类间，而接口大多数是关系疏松但都实现某一功能的类中
    (5) 抽象类是从一系列相关对象中抽象出来的概念， 因此反映的是事物的内部共性；接口是为了满足外部调用而定义的一个功能约定， 因此反映的是事物的外部特性
    (6) 接口基本上不具备继承的任何具体特点,它仅仅承诺了能够调用的方法    
    (7) 接口可以用于支持回调,而继承并不具备这个特点
    (8) 抽象类实现的具体方法默认为虚的，但实现接口的类中的接口方法却默认为非虚的，当然您也可以声明为虚的 

    (9) 如果抽象类实现接口，则可以把接口中方法映射到抽象类中作为抽象方法而不必实现，而在抽象类的子类中实现接口中方法



        对于接口  可以称为  协议

        抽象类 更多为了代码复用
        接口 更侧重解耦


        抽象类是自下而上的设计思路  现有子类代码重复 再抽象成上层的父类（抽象类）
        接口自上而下   先设计接口 再考虑具体实现

     */


    public enum Level { }
    public class MessageQueueClient {
        public void send(string msg) { }
    }

    public interface ILogger
    {
        void Print();
    }

    // 抽象类
    public abstract class Logger : ILogger
    {
        private string name;
        private bool enabled;
        private Level minPermittedLevel;

        public Logger(string name, bool enabled, Level minPermittedLevel)
        {
            this.name = name;
            this.enabled = enabled;
            this.minPermittedLevel = minPermittedLevel;
        }

        public void log(Level level, string message)
        {
            bool loggable = enabled && ((int)minPermittedLevel <= (int)level);
            if (!loggable) return;
            doLog(level, message);
        }

        //定义为virtual
        public virtual void Print()
        {
            
        }

        protected abstract void doLog(Level level, string message);
    }
    // 抽象类的子类：输出日志到文件
    public class FileLogger : Logger
    {
        private StreamWriter fileWriter;

         public FileLogger(string name, bool enabled,
            Level minPermittedLevel, string filepath):base(name, enabled, minPermittedLevel)
        {
            this.fileWriter = new StreamWriter(filepath);
        }

        protected override void doLog(Level level, String mesage)
        {
            // 格式化level和message,输出到日志文件
            fileWriter.Write("");
        }
    }
    // 抽象类的子类: 输出日志到消息中间件(比如kafka)
    public class MessageQueueLogger: Logger
    {
        private MessageQueueClient msgQueueClient;

        public MessageQueueLogger(string name, bool enabled,
          Level minPermittedLevel, MessageQueueClient msgQueueClient) : base(name, enabled, minPermittedLevel)
        {
            this.msgQueueClient = msgQueueClient;
        }

        protected override void doLog(Level level, string mesage)
        {
            // 格式化level和message,输出到消息中间件
            msgQueueClient.send(mesage);
        }
    }



    //用普通类模拟接口

    public class MockInterface
    {
        protected MockInterface() { }
        public void FuncA()
        {
            throw new NotImplementedException();
        }
    }

    public class SpecificInterface : MockInterface
    {
        public void FunB()
        {
            FuncA();
        }
    }

    #endregion


    #region 基于接口而非实现  /  基于抽象而非实现

    /*
     1. 函数命名不能暴露任何实现细节
     2. 封装具体的实现细节
     3. 为实现类定义抽象的接口，使用者依赖接口，而不是具体的实现类

        4. 尽量不要从实现类中反推出接口 容易使接口定义不够抽象

        5. 接口的定义只是表明做什么，而不是怎么做
        
        6. 接口设计需要考虑是否足够通用，是否能做到在替换具体的接口实现时，不需要改动任何接口的定义

        7. 不需要为每个类都定义接口

        8. 设计初衷：将接口和实现分离，封装不稳定的实现，暴露稳定的接口，
        上游系统面向接口而非实现，当实现发生改变时，上游系统代码基本不需要改动
         
         */


    public interface ImageStore
    {
        string Upload(Image image, string bucketName);   //需要抽象出来  同时将命名也抽象
        Image Download(string url);
    }

    public class AliyunImageStore: ImageStore
    {
        //...省略属性、构造函数等...

        public string Upload(Image image, string bucketName)
        {
            createBucketIfNotExisting(bucketName);
            String accessToken = generateAccessToken();
            //...上传图片到阿里云...
            //...返回图片在阿里云上的地址(url)...
            return "";
        }

        public Image Download(string url)
        {
            string accessToken = generateAccessToken();
            //...从阿里云下载图片...
            return null;
        }

        private void createBucketIfNotExisting(string bucketName)
        {
            // ...创建bucket...
            // ...失败会抛出异常..
        }

        private string generateAccessToken()
        {
            // ...根据accesskey/secrectkey等生成access token
            return "";
        }
    }

    // 上传下载流程改变：私有云不需要支持access token
    public class PrivateImageStore : ImageStore
    {
        public string Upload(Image image, string bucketName)
        {
            createBucketIfNotExisting(bucketName);
            //...上传图片到私有云...
            //...返回图片的url...
            return "";
        }

        public Image Download(string url)
        {
            //...从私有云下载图片...
            return null;
        }

        private void createBucketIfNotExisting(string bucketName)
        {
            // ...创建bucket...
            // ...失败会抛出异常..
        }
    }

    // ImageStore的使用举例
    public class ImageProcessingJob
    {
        private static readonly string BUCKET_NAME = "ai_images_bucket";
        //...省略其他无关代码...
  
        public void process()
        {
            Image image = null;//处理图片，并封装为Image对象
            ImageStore imageStore = new PrivateImageStore();
            imageStore.Upload(image, BUCKET_NAME);
        }
    }


    #endregion


    #region 少用继承，多用组合

    /*
        1. 原因：继承层次过深，过复杂，影响代码可维护性 可读性
     */

        
    //1. 继承使用1
    public abstract class Bird {
        public virtual void Fly() { }
    }

    //鸵鸟
    public class Ostrich : Bird
    {
        public override void Fly()
        {
            //TODO Not Support
            //设计不太合理
            //违反了 迪米特法则（最少知识原则）， 暴露了不该暴露的接口 给外部，增加了类的误用概率
        }
    }

    //2. 细分多个抽象类 去满足需求
    /*
      base  :                                                   AbstractBird

      child :                AbstractFlyableBird                                            AbstractUnFlyableBird

      child child : AbstractFlyableTweetableBird  AbstractFlyableUnTweetableBird           ...

    将父类的实现暴露给子类  子类实现依赖于父类  高耦合
     
     */

    //3. 组合1

    public interface Flyable
    {
        void Fly();
    }

    public interface Tweetable
    {
        void Tweent();
    }

    public interface Egglayable
    {
        void LayEgg();
    }

    public class Ostrich2 : Tweetable, Egglayable
    {
        public void LayEgg()
        {
            throw new NotImplementedException();
        }

        public void Tweent()
        {
            throw new NotImplementedException();
        }
    }

    public class Sparrow : Tweetable, Egglayable, Flyable
    {
        public void Fly()
        {
            throw new NotImplementedException();
        }

        public void LayEgg()
        {
            throw new NotImplementedException();
        }

        public void Tweent()
        {
            throw new NotImplementedException();
        }
    }

    //4. 如果接口的具体实现可复用
    public class BirdAbility
    {
        public delegate void FlyHandler();
        public delegate void EggLayHandler();
        public delegate void TweetHandler();

        public FlyHandler fly;
        public EggLayHandler layEgg;
        public TweetHandler tweet;
    }

    //5. Java中实现委托
    public class FlyAbility : Flyable
    {
        public void Fly()
        {
            throw new NotImplementedException();
        }
    }

    public class Sparrow2 : Flyable
    {
        private FlyAbility flyAbility = new FlyAbility();

        public void Fly()
        {
            flyAbility.Fly();
        }
    }

    //6. 区分组合和聚合

    public class Url
    {
        //...省略属性和方法
    }

    public class Crawler
    {
        private Url url; // 组合
        public Crawler()
        {
            this.url = new Url();
        }
        //...
    }

    public class PageAnalyzer
    {
        private Url url; // 组合
        public PageAnalyzer()
        {
            this.url = new Url();
        }
        //..
    }

    public class UrlAnalyzer
    {
        private Url url; // 聚合
        public UrlAnalyzer(Url url)
        {
            this.url = url;
        }
    }

    #endregion

    #region 贫血模型 vs 充血模型



    #endregion

    //****************************************************************************//
}
