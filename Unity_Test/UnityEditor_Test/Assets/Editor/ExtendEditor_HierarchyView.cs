using UnityEngine;
using UnityEditor;

//扩展Create菜单（GameObject/xx/xx ）
public class HierarchyCreateMenu
{
    [MenuItem("GameObject/My Create/Cube", false, 0)]
    static void CreateCube()
    {
        GameObject.CreatePrimitive(PrimitiveType.Cube); //创建立方体
    }
}

//扩展布局
public class HierarchyLayout
{
	[InitializeOnLoadMethod]
	static void InitializeOnLoadMethod()
	{
        //监听回调，重写Hierarchy视图
		EditorApplication.hierarchyWindowItemOnGUI = delegate(int instanceID, Rect selectionRect) {
			//在Hierarchy视图中选择一个资源
			if(Selection.activeObject && 
				instanceID ==Selection.activeObject.GetInstanceID()){
				//设置拓展按钮区域
				float width=50f;
				float height=20f;
				selectionRect.x += (selectionRect.width - width);
				selectionRect.width = width;
				selectionRect.height= height;
				//绘制自定义按钮，添加点击事件
				if(GUI.Button(selectionRect,AssetDatabase.LoadAssetAtPath<Texture>("Assets/unity.png"))){
					Debug.LogFormat("click : {0}",Selection.activeObject.name);
				}
			}	
		};
	}
}


//重写Hierarchy右键点击菜单，覆盖原有的
public class HierarchyOverrideRightClickMenu
{
	[MenuItem("Window/Test/mjx")]
	static void Test()
	{
	}

	[MenuItem("Window/Test/ziyan")]
	static void Test1()
	{
	}
	[MenuItem("Window/Test/橘子/1010")]
	static void Test2()
	{
	}


	[InitializeOnLoadMethod]
	static void StartInitializeOnLoadMethod()
	{
		EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
	}

	static void OnHierarchyGUI(int instanceID, Rect selectionRect)
	{
        //获取当前（鼠标抬起）事件
        //获取选中对象
		if (Event.current != null && selectionRect.Contains(Event.current.mousePosition)
			&& Event.current.button == 1 && Event.current.type <= EventType.MouseUp)
		{
			GameObject selectedGameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
			//这里可以判断selectedGameObject的条件
			if (selectedGameObject)
			{
				Vector2 mousePosition = Event.current.mousePosition;
                //弹出自定义菜单栏
				EditorUtility.DisplayPopupMenu(new Rect(mousePosition.x, mousePosition.y, 0, 0), "Window/Test",null);
                //不再执行原有操作，实现重写
				Event.current.Use();
			}			
		}
	}
}

using UnityEngine.UI;
public class HierarchyOverrideSysytemMenu
{
	[MenuItem("GameObject/UI/Image")]
	static void CreatImage()
	{
		if(Selection.activeTransform)
		{
			if(Selection.activeTransform.GetComponentInParent<Canvas>())
			{
				Image image = new GameObject("image").AddComponent<Image>();
				image.raycastTarget = false;                    //屏蔽射线点击
				image.transform.SetParent(Selection.activeTransform,false);
				//设置选中状态
				Selection.activeTransform = image.transform;
			}
		}
	}
}