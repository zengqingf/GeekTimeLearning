/*
ref: https://www.cnblogs.com/suoluo/p/5578618.html
若有尝试过想在unity的inspector检视面板中像List或者数组那样可以编辑Dictionary变量的童鞋应该知道，Dictionary变量不会出现在inspector中，
unity并不会直接序列化Dictionary类型，但实际上unity有提供接口使之可能：
unity doc: http://docs.unity3d.com/ScriptReference/ISerializationCallbackReceiver.OnBeforeSerialize.html。
注意其中的This interface should be used very carefully. Unity's serializer usually runs on the non main thread, while most of the Unity API can only be called from the main thread.
请慎用该方法，它不是线程安全的。
*/

using UnityEngine;
using System.Collections.Generic;
 
 /// Usage:
 /// 
 /// [System.Serializable]
 /// class MyDictionary : SerializableDictionary<int, GameObject> {}
 /// public MyDictionary dic;
 ///
 [System.Serializable]
 public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
 {
     // We save the keys and values in two lists because Unity does understand those.
     [SerializeField]
     private List<TKey> _keys;
     [SerializeField]
     private List<TValue> _values;
 
     // Before the serialization we fill these lists
     public void OnBeforeSerialize()
     {
　　　　　//官方例子有误，去掉     
     }
 
     // After the serialization we create the dictionary from the two lists
     public void OnAfterDeserialize()
     {
         this.Clear();
         int count = Mathf.Min(_keys.Count, _values.Count);
         for (int i = 0; i < count; ++i)
         {
             this.Add(_keys[i], _values[i]);
         }
     }
 }