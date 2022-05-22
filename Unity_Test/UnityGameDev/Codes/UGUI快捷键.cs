/*
ref: https://www.cnblogs.com/suoluo/p/5663207.html

在Editor下监听按键有以下几种方式：
*/

//1. 自定义菜单栏功能：
//api: http://docs.unity3d.com/Documentation/ScriptReference/MenuItem.html
namespace Test1
{
    using UnityEngine;
    using UnityEditor;
    public static class MyMenuCommands {
        [MenuItem("My Commands/First Command _p")]
        static void FirstCommand() {
            Debug.Log("You used the shortcut P");
        }
        [MenuItem("My Commands/Special Command %g")]
        static void SpecialCommand() {
            Debug.Log("You used the shortcut Cmd+G (Mac)  Ctrl+G (Win)");
        }
    }
}

//2. OnSceneGUI在GUI刷新中监听：
namespace Test2
{
    using UnityEngine;
    using UnityEditor;
    [CustomEditor(typeof(MySpecialMonoBehaviour))]
    public class MyCustomEditor : Editor {
        void OnSceneGUI() {
            Event e = Event.current;
            if(EventType.KeyDown == e.type && KeyCode.RightControl == e.keyCode)
            {
                moveMulti = true;
            }
            if(EventType.KeyUp == e.type && KeyCode.RightControl == e.keyCode)
            {
                moveMulti = false;
            }
        }
    }
}


//3. onSceneGUIDelegate注册事件：
namespace Test3
{
    using UnityEditor;
    using UnityEngine;
    [InitializeOnLoad]
    public static class EditorHotkeysTracker
    {
        static EditorHotkeysTracker()
        {
            SceneView.onSceneGUIDelegate += view =>
            {
                var e = Event.current;
                if (e != null && e.keyCode != KeyCode.None)
                    Debug.Log("Key pressed in editor: " + e.keyCode);
            };
        }
    }
}

//ref: http://answers.unity3d.com/questions/381630/listen-for-a-key-in-edit-mode.html

/*
方式二跟三类似Update()函数，当按键按下时可能会被多次执行，且不方便同时监听多个按键，一般来说作为全局快捷键应该同时组合ctrl/shift/alt或别的按键，以防跟普通按键冲突。个人认为方式一是更加简单可靠。
UGUI在想要创建一个Image或者Text时需要从菜单栏中级级点击多次才行（若是创建空物体再添加组件的方式只会更麻烦），而且创建的控件还是位于最外层层级中而不是直接成为我当前选中的物体的子物体，每次都得手动拖到父物体之下。另外一种方式就是右键点击一个物体，重弹出菜单中创建，个人觉得这样子也挺麻烦。
NGUI则大部分的控件创建都有对应的快捷键，且直接将新生成的物体放置到当前选中的控件之下，十分高效快捷。
现通过前面介绍的方式一为UGUI的控件创建添加快捷键，在创建控件的时你还可以同时进行一些默认初始设置，如改变Text的字体为常用字体，设置其对齐方式颜色等等：
*/

namespace Test4
{
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.UI;

    //同时支持在选中物体上右键菜单创建和直接快捷键创建
    public class UGUIHotKey
    {
        private static GameObject CheckSelection (MenuCommand menuCommand)
        {
            GameObject selectedObj = menuCommand.context as GameObject;
            //若当前不是右键点击物体的操作则看当前选中的物体的情况
            if (selectedObj == null)
                selectedObj = Selection.activeGameObject;
            //当前没有选中物体或者选中的物体不在Canvas之下则返回空，按键不响应。（当然也可以不要求存在Canvas，没有时则先创建一个新的Canvas）
            if (selectedObj == null || selectedObj != null && selectedObj.GetComponentInParent<Canvas> () == null)
                return null;
            return selectedObj;
        }

        [MenuItem ("GameObject/UGUI/Image #&i", false, 6)] //参数意义请查阅API文档，上文有链接，函数中的几个其他接口的调用的含义也有介绍
        static void CreateImage (MenuCommand menuCommand)
        {
            GameObject selectedObj = CheckSelection (menuCommand);
            if (selectedObj == null)
                return;
            GameObject go = new GameObject ("Image");
            GameObjectUtility.SetParentAndAlign (go, selectedObj);
            Undo.RegisterCreatedObjectUndo (go, "Create " + go.name);
            Selection.activeObject = go;
            go.AddComponent<Image> ();
        }

        [MenuItem ("GameObject/UGUI/Text #&t", false, 6)]
        static void CreateText (MenuCommand menuCommand)
        {
            GameObject selectedObj = CheckSelection (menuCommand);
            if (selectedObj == null)
                return;
            GameObject go = new GameObject ("Text");
            GameObjectUtility.SetParentAndAlign (go, selectedObj);
            Undo.RegisterCreatedObjectUndo (go, "Create " + go.name);
            Selection.activeObject = go;

            Text t = go.AddComponent<Text> ();
            Font font = AssetDatabase.LoadAssetAtPath ("Assets/ArtSources/Font/xxxx.ttf", typeof (Font)) as Font;
            t.font = font;
            t.fontSize = 24;
            t.alignment = TextAnchor.MiddleCenter;
            t.color = Color.white;
            t.text = "New Text";
            t.rectTransform.sizeDelta = new Vector2 (150f, 30f);
        }
    }
}