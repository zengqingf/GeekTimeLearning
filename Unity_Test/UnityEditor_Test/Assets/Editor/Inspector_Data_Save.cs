using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour {

    public int wheelCount = 0;

    // Use this for initialization
    void Start () {

    }
}

using UnityEngine;
using System.Collections;
using UnityEditor;

//指定要编辑的脚本为  Car.CS
[CustomEditor(typeof(Car))]
[CanEditMultipleObjects] // 5.1.2版本只有要在多个对象上挂Car.CS要加该句
public class CarEditor : Editor {

    private Car _car;  // 定义一个 Car 实例

    public override void OnInspectorGUI()
    {
        _car = (Car)target;  //获取 Car 实例（选中对象上挂载的 Car脚本）

        //将Car 的属性 wheelCount，以滑动条的形式显示在Inspector 面板
        _car.wheelCount = EditorGUILayout.IntSlider("WheelNumber", _car.wheelCount, 0, 20);

        //当Inspector 面板发生变化时保存数据
        //关闭Editor重启后数据也会被保存
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

}
