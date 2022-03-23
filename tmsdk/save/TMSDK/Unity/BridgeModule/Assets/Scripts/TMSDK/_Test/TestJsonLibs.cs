using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMSDKClient.LitJson;
using SimpleJSON;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

/*
[Performance comparison of Unity NewtonsoftJson, LitJson and SimpleJSON]
(https://programming.vip/docs/performance-comparison-of-unity-newtonsoftjson-litjson-and-simplejson.html)

@注意：
For Unity, SimpleJSON is the smallest size, fastest speed and easiest integration
     */
public class TestJsonLibs : MonoBehaviour
{
    public int count = 10000;
    private Stopwatch watch;

    private void Awake()
    {
        watch = new Stopwatch();
    }

    // Start is called before the first frame update
    void Start()
    {
        string json1 = "{\"id\":10001,\"name\":\"test\"}";
        string json2 = "[1,2,3,4,5,6,7,8,9,10]";
        string json3 = "{\"id\":10000,\"username\":\"zhangyu\",\"password\":\"123456\",\"nickname\":\"Iced Baidu\",\"age\":20,\"gender\":1,\"phone\":12345678910,\"email\":\"zhangyu@xx.com\"}";
        string json4 = "[\"test2\",[[\"key1\",    \"id\"],[\"key2\",    \"hp\"],[\"key3\",    \"mp\"],[\"key4\",    \"exp\"],[\"key5\",    \"money\"],[\"key6\",    \"point\"],[\"key7\",    \"age\"],[\"key8\",    \"sex\"]]]";

        JsonParseTest(json1);
        JsonParseTest(json2);
        JsonParseTest(json3);
        JsonParseTest(json4);       
    }

    private void JsonParseTest(string json)
    {
        print("json:" + json);
        bool isArray = json[0] == '[';
        NewtonsoftJsonTest(json, isArray);
        LiteJsonTest(json);
        SimpleJsonTest(json);
        MiniJsonTest(json);
        print("======================");
    }

    private void NewtonsoftJsonTest(string json, bool isArray)
    {
        watch.Reset();
        watch.Start();
        if (isArray)
        {
            JArray jArray = null;
            for (int i = 0; i < count; i++)
            {
                jArray = JArray.Parse(json);
            }
        }
        else
        {
            JObject jObj = null;
            for (int i = 0; i < count; i++)
            {
                jObj = JObject.Parse(json);
            }
        }
        watch.Stop();
        print("NewtonsoftJson Parse Time(ms):" + watch.ElapsedMilliseconds);
    }


    private void LiteJsonTest(string json)
    {
        watch.Reset();
        watch.Start();
        JsonData jData = null;
        for (int i = 0; i < count; i++)
        {
            jData = JsonMapper.ToObject(json);
        }
        watch.Stop();
        print("LitJson Parse Time(ms):" + watch.ElapsedMilliseconds);
    }

    private void SimpleJsonTest(string json)
    {
        watch.Reset();
        watch.Start();
        JSONNode jNode = null;
        for (int i = 0; i < count; i++)
        {
            jNode = JSON.Parse(json);
        }
        watch.Stop();
        print("SimpleJson Parse Time(ms):" + watch.ElapsedMilliseconds);
    }

    private void MiniJsonTest(string json)
    {
        watch.Reset();
        watch.Start();

        object obj = null;
        for (int i = 0; i < count; i++)
        {
            obj = MiniJSON.Json.Deserialize(json);
        }

        watch.Stop();
        print("MiniJson Parse Time(ms):" + watch.ElapsedMilliseconds);
    }
}
