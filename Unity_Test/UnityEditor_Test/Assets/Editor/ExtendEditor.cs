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