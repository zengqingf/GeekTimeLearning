using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

public sealed class DictionaryStringKeyActionValue<T> : Dictionary<string, Action<T>>
{

}

public class TestRPC : MonoBehaviour
{
    private readonly DictionaryStringKeyActionValue<object> requestCallback = new DictionaryStringKeyActionValue<object>();


    // Start is called before the first frame update
    void Start()
    {
        TestAsync<int>();
    }


    async void TestAsync<T>()
    {
        T data = await TestTask<T>();
        print("执行完毕 收到数据为："+ data);
    }

    Task<T> TestTask<T>()
    {
        _ClientSendMsg();
        TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
        print("Msg Send End");

        requestCallback["C_Response"] = (data)=>{

            print("异步结束");
            tcs.SetResult((T)data);   //给返回值赋值， data类型 取决于 Task<类型>
        };

        return tcs.Task;
    }



/***********************************   模拟客户端  *************************************/

    void _ClientSendMsg()
    {
        print("Client Send Msg");
        _ServerMethod();
    }

    void _ClientResponse<T>(T data)
    {
        print("Client Recv Msg From Server");
        if(requestCallback["C_Response"] != null)
        {
            requestCallback["C_Response"](data);
        }
    }

/***********************************   模拟服务器  *************************************/
    void _ServerMethod()
    {
        print("Server Recv Msg");
        Invoke("_ServerSendMsg", 2);
    }

    void _ServerSendMsg()
    {
        _ClientResponse<int>(100);
    }


    //使用泛型替换object
    void TestGeneric<T>(T data)
    {

    }

    void TestGeneric<T>(List<T> list)
    {

    }

    void TestGeneric<K, V>(Dictionary<K, V> dict)
    {

    }
}
