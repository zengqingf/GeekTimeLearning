
using UnityEngine;  
using System.Collections;  
using UnityEditor;  
  
public enum OPTIONS {   
    CUBE = 0,  
    SPHERE,  
    PLANE,  
}  
  
public class Test : MonoBehaviour {  
    public OPTIONS options = OPTIONS.PLANE;  
    public float number;  
    public StaticEditorFlags staticFlagMask = 0;  
}  

//InspectorTest脚本放在Editor文件夹下
using UnityEngine;  
using System.Collections;  
using UnityEditor;  
  
  
[CustomEditor(typeof(Test))]  
public class InspectorTest : Editor {  
  
    public override void OnInspectorGUI()  
    {  
        Test myTest = (Test)target;  
        myTest.options = (OPTIONS)EditorGUILayout.EnumPopup("options", myTest.options);  
        myTest.number = EditorGUILayout.FloatField("number", myTest.number);  
        myTest.staticFlagMask = (StaticEditorFlags)EditorGUILayout.EnumMaskField("static Flags", myTest.staticFlagMask);       
    }  
}  