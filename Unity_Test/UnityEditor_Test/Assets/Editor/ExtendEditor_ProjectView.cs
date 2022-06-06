using UnityEngine;
using UnityEditor;

    /*
    扩展 Project视图 右键选中资源弹出的菜单
    */
public class ProjectMouseRightMenu
{

    [MenuItem("Assets/My Tools/Tools 1", false, 2)]
    static void MyTools1()
    {
        Debug.Log(Selection.activeObject.name);
    }

    [MenuItem("Assets/My Tools/Tools 2", false, 1)]     //排序：数值越小，越靠前
    static void MyTools2()
    {
        Debug.Log(Selection.activeObject.name);
    }
}

    /*
    扩展 Project视图 Create按钮的创建菜单
    */
public class ProjectCreateButtonMenu
{

    [MenuItem("Assets/Create/My Create/Cube", false, 2)]
    static void CreateCube()
    {
       GameObject.CreatePrimitive(PrimitiveType.Cube);  //创建立方体   //用于创建Unity的基础模型
    }

    [MenuItem("Assets/Create/My Create/Sphere", false, 1)]   //排序：数值越小，越靠前
    static void CreateSphere()
    {
       GameObject.CreatePrimitive(PrimitiveType.Sphere);  //创建球体
    }
}

public class ProjectExtendLayout
{
    [InitializeOnLoadMethod]                                        //在C#代码每次编译完成后会首先调用
    static void InitializeOnLoadMethod()
    {
        EditorApplication.projectWindowItemOnGUI = delegate(string guid,        //使用GUI方法绘制自定义的UI元素（按钮、文本、图片、滚动条、下拉框）
                Rect selectionRect) {
                    //在Project视图中选择一个资源
                    if (Selection.activeObject &&
                        guid == AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(Selection.activeObject))) {
                            //设置扩展按钮区域
                            float width = 50f;
                            SelectionRect.x += (selectionRect.width - width);
                            SelectionRect.y += 2f;
                            SelectionRect.width = width;
                            GUI.color = Color.red;
                            //点击事件
                            if(GUI.Button(selectionRect, "click")) {
                                Debug.LogFormat("click : {0}", Selection.activeObject.name);
                            }
                            GUI.color = Color.white;
                        }
                }
    }
}

public class ProjectOperationListener
{
    [InitializeOnLoadMethod]
    static void InitializeOnLoadMethod()
    {
        //全局监听Project 视图下的资源是否发生变化（添加、删除或移动）
        EditorApplication.projectWidowChanged = delegate() {
            Debug.Log("project view change");
        };
    }

    //监听“双击鼠标左键，打开资源”事件
    public static bool IsOpenForEdit(string assetPath, out string msg)
    {
        msg = null;
        Debug.LogFormat("assetPath : {0}", assetPath);
        //true: 资源可以打开；false: 不允许在Unity中打开
        return true;
    }

    //监听“资源即将被创建”事件
    public static void OnWillCreateAsset(string path)
    {}

    //监听资源即将被保存
    public static string[] OnWillSaveAssets(string[] paths)
    {
        if (paths != null) {
            Debug.LogFormat("path: {0}", string.Join(",", paths));
        }
        return paths;
    }

    //监听“资源即将被移动”
    public static AssetMoveResult OnWillMoveAsset(string oldPaht, string newPath)
    {
        //AssetMoveResult.DidMove 表示该资源可以移动
        return AssetMoveResult.DidMove;
    }

    public static AssetDeleteResult OnWillDeleteAsset(string assetPath. RemoveAssetOptions option)
    {
        //AssetDeleteResult.DidNotDelete 表示该资源不能被删除
        return AssetDeleteResult.DidNotDelete
    }
}