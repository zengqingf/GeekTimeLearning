public static class ProjectExtends{

    /*
    清理missing的引用ß
    */
    [MenuItem("Edit/Cleanup Missing Scripts")]
    static void CleanupMissingScripts ()
    {
        for(int i = 0; i < Selection.gameObjects.Length; i++)
        {
            var gameObject = Selection.gameObjects[i];

            // We must use the GetComponents array to actually detect missing components
            var components = gameObject.GetComponents<Component>();

            // Create a serialized object so that we can edit the component list
            var serializedObject = new SerializedObject(gameObject);
            // Find the component list property
            var prop = serializedObject.FindProperty("m_Component");

            // Track how many components we've removed
            int r = 0;

            // Iterate over all components
            for(int j = 0; j < components.Length; j++)
            {
                // Check if the ref is null
                if(components[j] == null)
                {
                    // If so, remove from the serialized component array
                    prop.DeleteArrayElementAtIndex(j-r);
                    // Increment removed count
                    r++;
                }
            }

            // Apply our changes to the game object
            serializedObject.ApplyModifiedProperties();
            //这一行一定要加！！！
            EditorUtility.SetDirty(gameObject);
        }
    }


    [MenuItem("Tools/Check Text Count")]
    public static void CheckText ()
    {
        //查找指定路径下指定类型的所有资源，返回的是资源GUID
        string[] guids = AssetDatabase.FindAssets ("t:GameObject", new string[] { "Assets/Resources/UI" });
        //从GUID获得资源所在路径
        List<string> paths = new List<string> ();
        guids.ToList ().ForEach (m => paths.Add (AssetDatabase.GUIDToAssetPath (m)));
        //从路径获得该资源
        List<GameObject> objs = new List<GameObject> ();
        paths.ForEach (p => objs.Add (AssetDatabase.LoadAssetAtPath (p, typeof (GameObject)) as GameObject));
        //下面就可以对该资源做任何你想要的操作了，如查找已丢失的脚本、检查赋值命名等，这里查找所有的Text组件个数
        List<Text> texts = new List<Text> ();
        objs.ForEach (o => texts.AddRange (o.GetComponentsInChildren<Text> (true)));
        Debug.Log ("Text count:" + texts.Count);
    }

}