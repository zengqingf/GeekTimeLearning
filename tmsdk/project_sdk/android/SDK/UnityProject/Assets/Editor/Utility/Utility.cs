
using UnityEditor;
using UnityEngine;

public class Utility
{
        [MenuItem("Assets/CopyAssetsPath", false)]
        public static void CopyAssetsPath()
        {
            UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), UnityEditor.SelectionMode.Assets);
            if (selection.Length > 0)
            {
                if (selection[0] is Texture2D)
                {
                    string path = FileTools.GetAssetPath(Selection.activeObject);
                    GUIUtility.systemCopyBuffer = path + ":" + Selection.activeObject.name;
                }
                else
                {
                    string path = FileTools.GetAssetPath(selection[0]);
                    if (path.Contains(".prefab"))
                        path = path.Replace(".prefab", "");
                    GUIUtility.systemCopyBuffer = path;
                }
            }
        }
}