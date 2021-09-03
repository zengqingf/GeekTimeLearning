using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor;
using System.IO;
using XUPorterJSON;

namespace HeroGo
{
    public class UtilityTools
    {
		public static void GetSkillFiles(ArrayList fileDic, string fullPath, bool recursive = false, int level=1)
        {
            DirectoryInfo dirOutPath = new DirectoryInfo(fullPath);

            DirectoryInfo[] dirList = dirOutPath.GetDirectories();

            if (recursive)
            {
                foreach (DirectoryInfo info in dirList)
                {
					GetSkillFiles(fileDic, fullPath + "/" + info.Name, recursive, level+1);
                }
            }

            FileInfo[] objFileList = dirOutPath.GetFiles();
            foreach (FileInfo objFile in objFileList)
            {
                string fileName = objFile.Name;
				if (recursive && level > 1)
                    fileName = dirOutPath.Name + "/" + fileName;
                if (!fileName.Contains(".meta") && !fileName.Contains(".json"))
                {
                    fileDic.Add(fileName.Replace(".asset", ""));
                }
            }
        }

        public static void GenSkillFileList2(string fullPath)
        {
			string folderName = Utility.GetPathLastName(fullPath);
			string jsonFileName = fullPath + "/"+folderName+"_FileList.json";

            //先删掉这个文件
            if (File.Exists(jsonFileName))
                File.Delete(jsonFileName);

            ArrayList fileDic = new ArrayList();

            GetSkillFiles(fileDic, fullPath, true, 1);

            string json = MiniJSON.jsonEncode(fileDic);

            File.WriteAllText(jsonFileName, json);

            AssetDatabase.ImportAsset(jsonFileName);


            Debug.Log("Write " + jsonFileName + " succeed!!!!");
        }


		static string[] SpecialEffectNames = {
			"p_Xueren_guo",
			"P_Dungeon_Ruolan_L_mutong",
			"Common_xuli_red",
		};

		[MenuItem("Assets/特效数据预Cook", false)]
		public static void CookEffectData()
		{
			UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), UnityEditor.SelectionMode.Assets);
			string fullPath = FileTools.GetAssetFullPath(selection[0]);

			DirectoryInfo mydir = new DirectoryInfo(fullPath);
			if(mydir.Exists)
			{
				string[] pathList = Directory.GetFiles(fullPath, "*.prefab", SearchOption.AllDirectories);
				for(int i=0; i<pathList.Length; ++i)
				{
					var path = pathList[i];
					if (!path.Contains(".meta") && (
						path.Contains("Eff_") || path.Contains("EffUI") || path.Contains("eff_") || path.Contains("Topicon_") ||
						IsInSpecial(path)
					))
					{
						DoCookData(path);
					}
				}
			}
			else {
				DoCookData(fullPath);
			}
				
			AssetDatabase.SaveAssets();

			Logger.LogErrorFormat("特效数据预Cook成功!!!");
		}

		static bool IsInSpecial(string path)
		{
			for(int i=0; i<SpecialEffectNames.Length; ++i)
			{
				if (path.Contains(SpecialEffectNames[i]))
					return true;
			}

			return false;
		}

		static void DoCookData(string path)
		{
			//if (!path.Contains(".meta") && (path.Contains("Eff_") || path.Contains("EffUI") || path.Contains("eff_") || path.Contains("Topicon_")))
			{
				GameObject eff = AssetDatabase.LoadAssetAtPath<GameObject>(path);
				var proxy = eff.GetComponent<GeEffectProxy>();
				if (proxy == null)
					proxy = eff.AddComponent<GeEffectProxy>();
				proxy.DoCookData();

				EditorUtility.SetDirty(eff);
			}
		}
			

        [MenuItem("Assets/GenSkillFileList", false)]
        public static void GenSkillFileList()
        {
            UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), UnityEditor.SelectionMode.Assets);
            string fullPath = FileTools.GetAssetFullPath(selection[0]);

            GenSkillFileList2(fullPath);
        }

        //[MenuItem("Assets/GenALLSkillFileList", false)]
        public static void GenAllSkillFileList()
        {
            UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), UnityEditor.SelectionMode.Assets);
            string fullPath = FileTools.GetAssetFullPath(selection[0]);

/*            DirectoryInfo dirOutPath = new DirectoryInfo(fullPath);
            DirectoryInfo[] dirList = dirOutPath.GetDirectories("*", SearchOption.TopDirectoryOnly);

            foreach (DirectoryInfo info in dirList)
            {
                GenSkillFileList2(fullPath + "/" + info.Name);
            }*/

			GenSkillFileListRecursion(fullPath);
        }

		public static void GenSkillFileListRecursion(string fullPath)
		{
			DirectoryInfo dirOutPath = new DirectoryInfo(fullPath);
			DirectoryInfo[] dirList = dirOutPath.GetDirectories("*", SearchOption.TopDirectoryOnly);

			foreach (DirectoryInfo info in dirList)
			{
				string newPath = fullPath + "/" + info.Name;
				GenSkillFileList2(newPath);
				GenSkillFileListRecursion(newPath);
			}
		}

        [MenuItem("Assets/CopyAssetsPath", false)]
        public static void CopyAssetsPath()
        {
            UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), UnityEditor.SelectionMode.Assets);
            if (selection.Length > 0)
            {
                if(selection[0] is Texture2D)
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

        [MenuItem("GameObject/DummyRectTransform", false,0)]
        public static void DummyGameObjectRectTransform()
        {
            UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.GameObject), UnityEditor.SelectionMode.Unfiltered);
            if (selection.Length > 0)
            {
                GameObject obj = (selection[0] as GameObject);
                
                if(obj != null)
                {                    
                    var rectTransform = obj.GetComponent<RectTransform>();
                    UnityEngine.Debug.Log(ObjectDumper.Dump(rectTransform));
                }
            }
        }

        [MenuItem("FB/FBDungeonData", false,0)]
        public static void GenerateFBDDungeonData()
        {
            string[] allIds = AssetDatabase.FindAssets("t:DDungeonData", new string[] {"Assets/Resources/Data"});

            for (int i = 0; i < allIds.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(allIds[i]);

                UnityEngine.Debug.LogErrorFormat("{0}", path);

                DDungeonData data = AssetDatabase.LoadAssetAtPath<DDungeonData>(path);

                FlatBuffers.FlatBufferBuilder builder = new FlatBuffers.FlatBufferBuilder(1);
                
				FlatBuffers.Offset<FBDungeonData.DDungeonData> sdata = FBDungeonDataTools.CreateFBDungeonData(builder, data);
                FBDungeonData.DDungeonData.FinishDDungeonDataBuffer(builder, sdata);

                string outfilename = path + ".bytes";

                using (var ms = new MemoryStream(builder.DataBuffer.Data, builder.DataBuffer.Position, builder.Offset))
                {
                    File.WriteAllBytes( outfilename, ms.ToArray());
                }
            }
        }
        

        [MenuItem("FB/FBSceneData", false,0)]
        public static void GenerateFBDSceneData()
        {
            string[] allIds = AssetDatabase.FindAssets("t:DSceneData", new string[] {"Assets/Resources/Data/SceneData"});

            for (int i = 0; i < allIds.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(allIds[i]);

                DSceneData data = AssetDatabase.LoadAssetAtPath<DSceneData>(path);

                FlatBuffers.FlatBufferBuilder builder = new FlatBuffers.FlatBufferBuilder(1);
                var sdata = FBSceneDataTools.CreateFBSceneData(builder, data);

                FBSceneData.DSceneData.FinishDSceneDataBuffer(builder, sdata);

                string outfilename = path + ".bytes";
                using (var ms = new MemoryStream(builder.DataBuffer.Data, builder.DataBuffer.Position, builder.Offset)) {
                    File.WriteAllBytes( outfilename, ms.ToArray());
                }
            }
        }
        
        /*
        [MenuItem("Assets/ModifyPrefabRoot", false)]
        public static void ModifyPrefabRoot()
        {
            UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.GameObject), UnityEditor.SelectionMode.Assets);
            for (int i = 0; i < selection.Length; ++i)
            {
                GameObject obj = (selection[i] as GameObject);
                
                if(obj != null)
                {                    
                    var transforms = obj.transform;
                    bool bInit = (transforms.localPosition == Vector3.zero) && (transforms.localRotation == Quaternion.identity) && (transforms.localScale == Vector3.one);

                    if(bInit == false)
                    {
                        GameObject select = PrefabUtility.InstantiatePrefab(selection[i]) as GameObject;
                        GameObject gameObj = new GameObject();
                        gameObj.name = select.name;
                        select.name  = select.name + "RootModify";
                        select.transform.SetParent(gameObj.transform,false);
                        PrefabUtility.ReplacePrefab(gameObj,selection[i],ReplacePrefabOptions.Default);
                        Editor.DestroyImmediate(gameObj);
                    }
                }
            }
        }
		*/

        [MenuItem("Assets/预览预制体", false)]
        public static GameObject PreviewPrefab()
        {
            UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), UnityEditor.SelectionMode.Assets);
            GameObject select = PrefabUtility.InstantiatePrefab(selection[0]) as GameObject;
            if (select != null)
            {
                GameObject root = Utility.FindGameObject("UIRoot", false);

                if (root != null)
                {
                    GameObject.DestroyImmediate(root);
                    root = null;
                }

                root = AssetLoader.instance.LoadResAsGameObject("Base/UI/Prefabs/Root/UIRoot");
                root.SetActive(true);
                root.name = "UIRoot";

                GameObject layer = Utility.FindGameObject(root, "UI2DRoot/Top");

                Utility.AttachTo(select, layer);
                EditorGUIUtility.PingObject(select);
				return select;
            }
            else
            {
                Logger.LogError("请选择一个Prefab对象");
				return null;
            }

        }
    }
}
