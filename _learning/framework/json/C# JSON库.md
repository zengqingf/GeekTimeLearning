# C# JSON库

* SimpleJSON

  [github - SimpleJSON - Bunny83](https://github.com/Bunny83/SimpleJSON)

* Newtonsoft.Json

  [github - Newtonsoft.Json - JamesNK](https://github.com/JamesNK/Newtonsoft.Json)

* LitJSON

  [github - litjson - LitJSON](https://github.com/LitJSON/litjson)

* Swifter.Json

  [github - Swifter.Json - Dogwei](https://github.com/Dogwei/Swifter.Json)

* CSharpJson

  [github - CSharpJson - NingShenTian](https://github.com/NingShenTian/CsharpJson)

* MiniJSON

  [github - minijson - darktable](https://gist.github.com/darktable/1411710)

* MojoJson

  [github - MojoJson - scottcgi](https://github.com/scottcgi/MojoJson)



---



* ref

  [Json解析C#的四个库](https://yaojiaxinpc.github.io/3JsonDeal/)

  [Json序列化之.NET开源类库Newtonsoft.Json的研究](https://www.cnblogs.com/yunfeifei/p/4086014.html)



* QA

  [C# 序列化 只读属性](https://blog.csdn.net/lj22377/article/details/47253725)

  ``` c#
          [XmlElement("MinAge")]
          public int MinAge { get; set; }
   
   
          [XmlElement("MaxAge")]
          public int MaxAge { get; set; }
   
   
          [XmlElement("DiffAge")]
          public int DiffAge {
              get
              {
                  return MaxAge - MinAge;
              }
              set { }
          }
  //对LitJson有效
  ```

  



---



### C++ Json

* ref

  [github - minijson - zsmj2017](https://github.com/zsmj2017/MiniJson)

  [github - json-tutorial - miloyip](https://github.com/miloyip/json-tutorial)

  [github - json - Yuan-Hang](https://github.com/Yuan-Hang/Json)




---





### Unity

* JsonUtility 问题

  [Serialize and Deserialize Json and Json Array in Unity](https://stackoverflow.com/questions/36239705/serialize-and-deserialize-json-and-json-array-in-unity)

  [Unity C# JsonUtility is not serializing a list](https://stackoverflow.com/questions/41787091/unity-c-sharp-jsonutility-is-not-serializing-a-list)

  ``` text
  Remove { get; set; } from the player class. If you have { get; set; }, it won't work. Unity's JsonUtility does NOT work with class members that are defined as properties.
  ```

  ``` text
  1.Not including [Serializable]. You get empty json if you don't include this.
  2.Using property (get/set) as your variable. JsonUtility does not support this.
  3.Trying to serializing a collection other than List.
  4.Your json is multi array which JsonUtility does not support and needs a wrapper to work.
  5.Like the example, given above in the SpriteData class, the variable must be a public variable. If it is a private variable, add [SerializeField] at the top of it.
  ```

  [JsonUtility array not supported?](https://answers.unity.com/questions/1123326/jsonutility-array-not-supported.html)

  ``` text]
  Serializing/deserializing arrays and lists as top-level elements is not supported right now. It's on the to-do list...
  
  Note that you can work around it in two ways for now:
  
  Use ToJson() for each element in the array separately, and just stitch them together with "[", "," and "]"
  
  Wrap the array in a structure like this:
  
  [Serializable] public struct MyObjectArrayWrapper { public MyObject[] objects; }
  ```

  

* xml序列化问题

  [XML 序列化](https://docs.microsoft.com/zh-cn/dotnet/standard/serialization/introducing-xml-serialization)

  ``` text
  序列化数据只包含数据本身和类的结构。 类型标识和程序集信息不包括在内。
  只能序列化公共属性和字段。 属性必须具有公共访问器（get 和 set 方法）。 如果必须序列化非公共数据，请使用 DataContractSerializer 类而不使用 XML 序列化。
  类必须具有无参数构造函数才能被 XmlSerializer 序列化。
  方法不能被序列化。
  如下所述，如果实现 IEnumerable 或 ICollection 的类满足某些要求，XmlSerializer 则可以处理这些类 。
  实现 IEnumerable 的类必须实现采用单个参数的公共 Add 方法 。 Add 方法的参数必须与从“IEnumerator.Current”属性返回的类型一致（多态），该属性是从 GetEnumerator 方法返回的 。
  除了实现 IEnumerable 之外，还能实现 ICollection 的类（如 CollectionBase）必须具有采用整型的公共“Item”索引属性（在 C# 中为索引器），而且它必须有一个“integer”类型的公共“Count”属性 。 传递给 Add 方法的参数必须与从“Item”属性返回的类型相同，或者为此类型的基之一 。
  对于实现 ICollection 的类，可从已编制索引的“Item”属性检索要序列化的值，而不是通过调用 GetEnumerator 进行检索 。 此外，除了返回另一个集合类（实现 ICollection 的一个类）的公共字段外，公共字段和属性不会被序列化。 有关示例，请参阅 XML 序列化示例。
  ```

  