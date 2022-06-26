/*
重写Inspector

获取属性 serializedObject.FindProperty("age");
显示属性,类型会自动获取 EditorGUILayout.PropertyField(ageProp, new GUIContent("age"));
应用修改项 serializedObject.ApplyModifiedProperties();
对象需要一直更新 serializedObject.Update();
*/

using UnityEngine;
using System;
using System.Collections;
[Serializable]
public class RoleController :MonoBehaviour
{
    public string roleName = "asadasd";
    public int age;
    public float range;
    public float jumpHight;
    public Texture2D pic;
    public string picName;
    public moveType ac;
    public bool isBoy;
}
public enum moveType
{
    jump,
    move,
    attack
}


using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RoleController))]
[CanEditMultipleObjects]
public class EditorInspector : Editor
{
    private RoleController role;

    bool toggle;
    SerializedProperty ageProp;
    SerializedProperty nameProp;
    SerializedProperty picProp;
    SerializedProperty rangeProp;
    SerializedProperty boolProp;
    SerializedProperty enumProp;
    void OnEnable()
    {
        // Setup the SerializedProperties.
        ageProp = serializedObject.FindProperty("age");
        nameProp = serializedObject.FindProperty("roleName");
        picProp = serializedObject.FindProperty("pic");
        rangeProp = serializedObject.FindProperty("range");
        boolProp = serializedObject.FindProperty("isBoy");
        enumProp = serializedObject.FindProperty("ac");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        role = target as RoleController;
        GUILayout.Space(6f);

        //role.age = EditorGUILayout.IntSlider("Age", role.age, 0, 100);
        //role.roleName = EditorGUILayout.TextField("角色名字", role.roleName);

        EditorGUILayout.PropertyField(ageProp, new GUIContent("age"));
        EditorGUILayout.PropertyField(nameProp, new GUIContent("name"));
        EditorGUILayout.PropertyField(boolProp, new GUIContent("isBoy"));
        EditorGUILayout.PropertyField(enumProp, new GUIContent("enum"));

        if (EditorGUILayout.Foldout(toggle, "折叠"))
        {
            EditorGUILayout.PropertyField(picProp, new GUIContent("pic"));
        }
        EditorGUILayout.Slider(rangeProp, 0, 100, new GUIContent("range"));
        ProgressBar(rangeProp.floatValue/100, "range");
        serializedObject.ApplyModifiedProperties();
    }

    // Custom GUILayout progress bar.
    void ProgressBar(float value, string label)
    {
        // Get a rect for the progress bar using the same margins as a textfield:
        Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
        EditorGUI.ProgressBar(rect, value, label);
        EditorGUILayout.Space();
    }
}