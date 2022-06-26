

/*
深度拷贝与粘贴组件都使用了递归调用。
粘贴的递归过程中，首先粘贴了当前层级的所有组件，方法仍旧使用的是第一种拷贝粘贴的方法。
然后遍历子对象中的对象，递归调用。
*/

using UnityEngine;
using UnityEditor;
using System.Collections;

//copy当前
public class CopyAllComponent : EditorWindow
{
    static Component[] copiedComponents;
    [MenuItem("GameObject/Copy Current Components #&C")]
    static void Copy()
    {
        copiedComponents = Selection.activeGameObject.GetComponents<Component>();
    }

    [MenuItem("GameObject/Paste Current Components #&P")]
    static void Paste()
    {
        foreach (var targetGameObject in Selection.gameObjects)
        {
            if (!targetGameObject || copiedComponents == null) continue;
            foreach (var copiedComponent in copiedComponents)
            {
                if (!copiedComponent) continue;
                UnityEditorInternal.ComponentUtility.CopyComponent(copiedComponent);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(targetGameObject);
            }
        }
    }

}


using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class DeepCopyAllComponent : EditorWindow
{
    [MenuItem("GameObject/Copy All Components #%&C")]
    static void Copy()
    {
        GetAllChilds(Selection.activeGameObject,pri_my_list);
    }

    [MenuItem("GameObject/Paste All Components #%&P")]
    static void Paste()
    {
        GameObject tmpGameObj = Selection.activeGameObject;
        PasteChildComponent(tmpGameObj, pri_my_list);

    }


    public class MyComponentList
    {
        public MyComponentList()
        {
        }

        public List<Component> gameObjList;
        public List<MyComponentList> nextList;
    }

    private static void PasteChildComponent(GameObject gameObj, MyComponentList next)
    {
        if (next.gameObjList != null)
        {
            foreach (var copiedComponent in next.gameObjList)
            {
                if (!copiedComponent) continue;

                UnityEditorInternal.ComponentUtility.CopyComponent(copiedComponent);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(gameObj);
            }
        }

        if (next.nextList != null)
        {
            List<Transform> TmpListTrans = new List<Transform>();
            foreach (Transform item in gameObj.transform)
            {
                TmpListTrans.Add(item);
            }
            int i = 0;
            foreach (var item in next.nextList)
            {
                if (i < TmpListTrans.Count)
                {
                    PasteChildComponent(TmpListTrans[i].gameObject, item);
                }
                i++;
            }
        }
    }


    static MyComponentList pri_my_list = new MyComponentList();

    private static void GetAllChilds(GameObject transformForSearch, MyComponentList next)
    {
        List<Component> childsOfGameobject = new List<Component>();
        next.gameObjList = childsOfGameobject;
        next.nextList = new List<MyComponentList>();

        foreach (var item in transformForSearch.GetComponents<Component>())
        {
            childsOfGameobject.Add(item);
        }

        foreach (Transform item in transformForSearch.transform)
        {
            MyComponentList tmpnext = new MyComponentList();
            GetAllChilds(item.gameObject, tmpnext);
            next.nextList.Add(tmpnext);
        }
        return;
    }

}