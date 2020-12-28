using System;

namespace OOP_Test_2
{
    /*
       SOLID 原则

        S:  Single Respponsibility Principle    SRP    单一职责原则
         
         */


    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }


    /************************************************ SRP *********************************************/

        /*
         *  模块比类更粗粒度，以下按类说明，方便编码
         * 
            拆分技巧：
            1.类中代码行数、函数、属性过多
            2.类依赖的其他类过多，或者依赖类的其他类过多
            3.
         */

    public class UserInfo
    {
        private long userId;
        private string userName;
        private string email;
        private string telephone;
        private long createTime;
        private long lastLoginTime;
        private string avatarUrl;
        private string provinceOfAddress; //省
        private string cityOfAddress;     //市
        private string regionOfAddress;   //区
        private string detailOfAddress;   //详细地址

        //...
    }

    //根据实际需求拆分
    //持续重构：
    //可以先写粗粒度的类（模块），满足需求，随着业务发展，拆分成多个细粒度的类（模块）

    public class UserInfo2
    {
        private long userId;
        private string userName;
        private long createTime;
        private long lastLoginTime;
        private string avatarUrl;

        private AccountVerifyInfo accVerifyInfo;

        private AddressInfo addressInfo;
        //...
    }

    //新增需求：用户地址用于电商物流，拆分出物流信息
    public class AddressInfo
    {
        private string provinceOfAddress; //省
        private string cityOfAddress;     //市
        private string regionOfAddress;   //区
        private string detailOfAddress;   //详细地址
    }

    //新增需求：统一的账号登录  身份认证信息
    public class AccountVerifyInfo
    {
        private string email;
        private string telephone;
    }

    /********************************************************************************************************/
}
