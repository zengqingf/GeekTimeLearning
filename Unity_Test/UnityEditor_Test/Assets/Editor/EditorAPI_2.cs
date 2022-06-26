/*
https://blog.csdn.net/jjiss318/article/details/7435708

ScriptableObject, ScriptableWizard, EditorWindow、Editor
其中EditorWindow和Editor都继承了ScriptableObject，而ScritableWizard则继承了EditorWindow派。
*/

//1. ScriptableObject
/*
这个扩展脚本从菜单的“GameObject->Add Child”启动，功能是给Hierarchy窗口中选中的对GameObject添加一个名字为“_Child”的子GameObject，这样可以免去从Hierarchy窗口的根节点拖拽新创建的GameObject到当前选中节点的麻烦，
因为在Unity3D编辑器中，创建一个EmptyObject会在Hierarchy窗口的根节点出现，无论当前选中的节点对象是哪个
*/
using UnityEngine;
using UnityEditor;
using System.Collections;
 
public class AddChild : ScriptableObject
{
    [MenuItem ("GameObject/Add Child ^n")]
    static void MenuAddChild()
    {
        Transform[] transforms = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable);
 
        foreach(Transform transform in transforms)
        {
            GameObject newChild = new GameObject("_Child");
            newChild.transform.parent = transform;
        }
    }
}


//2 .ScriptableWizard      
/*
需要对扩展的参数进行设置，然后再进行功能触发的，可以从这个类进行派生。它已经定制好了四个消息响应函数，开发者对其进行填充即可。

(1) OnWizardUpdate  
当扩展窗口打开时或用户对窗口的内容进行改动时，会调用此函数。一般会在这里面显示帮助文字和进行内容有效性验证；

(2)OnWizardCreate  
这是用户点击窗口的Create按钮时进行的操作，从ScriptableWizard的名字可以看出，这是一种类似向导的窗口
只不过Unity3D中的ScriptableWizard窗口只能进行小于或等于两个按钮的定制，一个就是所谓的Create按钮，另外一个则笼统称之为Other按钮。ScriptableWizard.DisplayWizard这个静态函数用于对ScriptableWizard窗口标题和按钮名字的定制。

(3) OnDrawGizmos
在窗口可见时，每一帧都会调用这个函数。在其中进行Gizmos的绘制，也就是辅助编辑的线框体。Unity的Gizmos类提供了DrawRayDrawLine ,DrawWireSphere ,DrawSphere ,DrawWireCube ,DrawCubeDrawIcon ,DrawGUITexture 功能。这个功能在Unity3D 的3.4版本中测试了一下，发现没有任何Gizmos绘制出来

(4) OnWizardOtherButton
本文在(2) 中已经提及，ScriptableWizard窗口最多可以定制两个按钮，一个是Create，另外一个称之为Other，这个函数会在other按钮被点击时调用。
*/
using UnityEditor;
using UnityEngine;
using System.Collections;
 
/// <summary>
/// 对于选定GameObject，进行指定component的批量添加
/// </summary>
public class AddRemoveComponentsRecursively : ScriptableWizard
{
    public string componentType = null;
 
    /// <summary>
    /// 当没有任何GameObject被选中的时候，将菜单disable（注意，这个函数名可以随意取）
    /// </summary>
    /// <returns></returns>
    [MenuItem("GameObject/Add or remove components recursively...", true)]
    static bool CreateWindowDisabled()
    {
        return Selection.activeTransform;
    }
 
    /// <summary>
    /// 创建编辑窗口（注意，这个函数名可以随意取）
    /// </summary>
    [MenuItem("GameObject/Add or remove components recursively...")]
    static void CreateWindow()
    {
        // 定制窗口标题和按钮，其中第二个参数是Create按钮，第三个则属于other按钮
        // 如果不想使用other按钮，则可调用DisplayWizard的两参数版本
        ScriptableWizard.DisplayWizard<AddRemoveComponentsRecursively>(
            "Add or remove components recursivly",
            "Add", "Remove");
    }
 
    /// <summary>
    /// 窗口创建或窗口内容更改时调用
    /// </summary>
    void OnWizardUpdate()
    {
        helpString = "Note: Duplicates are not created";
 
        if (string.IsNullOrEmpty(componentType))
        {
            errorString = "Please enter component class name";
            isValid = false;
        }
        else
        {
            errorString = "";
            isValid = true;
        }
    }
 
    /// <summary>
    /// 点击Add按钮（即Create按钮）调用
    /// </summary>
    void OnWizardCreate()
    {
        int c = 0;
        Transform[] ts = Selection.GetTransforms(SelectionMode.Deep);
        foreach (Transform t in ts)
        {
            if (t.gameObject.GetComponent(componentType) == null)
            {
                if (t.gameObject.AddComponent(componentType) == null)
                {
                    Debug.LogWarning("Component of type " + componentType + " does not exist");
                    return;
                }
                c++;
            }
        }
        Debug.Log("Added " + c + " components of type " + componentType);
    }
 
    /// <summary>
    /// 点击Remove（即other按钮）调用
    /// </summary>
    void OnWizardOtherButton()
    {
        int c = 0;
        Transform[] ts = Selection.GetTransforms(SelectionMode.Deep);
        foreach (Transform t in ts)
        {
            if (t.GetComponent(componentType) != null)
            {
                DestroyImmediate(t.GetComponent(componentType));
                c++;
            }
        }
        Debug.Log("Removed " + c + " components of type " + componentType);
        Close();
    }
}


//3 . EditorWindow
/*
较复杂的功能，需要多个灵活的控件，实现自由浮动和加入其他窗口的tab，可以从这个类派生，这种窗口的窗体功能和Scene，Hierarchy等窗口完全一致。下面这个例子实现了GameObject的空间对齐和拷贝（也就是将GameObject A作为基准，选中其他的GameObject进行对准或空间位置拷贝），对齐和拷贝提高了了开发者摆放物件的效率；另外还有随机和噪声，后两者用于摆放大量同类物件的时候可以使用，比如一大堆散落的瓶子。
*/
// /
//
// Transform Utilities.
//
// This window contains four useful tools for asset placing and manipulation: Align, Copy, Randomize and Add noise.
//
// Put this into Assets/Editor and once compiled by Unity you find
// the new functionality in Window -> TransformUtilities, or simply press Ctrl+t (Cmd+t for Mac users)
// 
// Developed by Daniel 
// http://www.silentkraken.com
// e-mail: seth@silentkraken.com
//
// /
 
using UnityEngine;
using UnityEditor;
 
public class TransformUtilitiesWindow : EditorWindow 
{
    //Window control values
	public int toolbarOption = 0;
	public string[] toolbarTexts = {"Align", "Copy", "Randomize", "Add noise"};
 
    private bool xCheckbox = true;
    private bool yCheckbox = true;
    private bool zCheckbox = true;
 
    private Transform source;
    private float randomRangeMin = 0f;
    private float randomRangeMax = 1f;
    private int alignSelectionOption = 0;
    private int alignSourceOption = 0;
 
    /// <summary>
    /// Retrives the TransformUtilities window or creates a new one
    /// </summary>
    [MenuItem("Window/TransformUtilities %t")]
    static void Init()
    {
        TransformUtilitiesWindow window = (TransformUtilitiesWindow)EditorWindow.GetWindow(typeof(TransformUtilitiesWindow));
        window.Show();
    }
    
    /// <summary>
    /// Window drawing operations
    /// </summary>
    void OnGUI () 
	{
        toolbarOption = GUILayout.Toolbar(toolbarOption, toolbarTexts);
        switch (toolbarOption)
        {
            case 0:
                CreateAxisCheckboxes("Align");
                CreateAlignTransformWindow();
                break;
            case 1:
                CreateAxisCheckboxes("Copy");
                CreateCopyTransformWindow();
                break;
            case 2:
                CreateAxisCheckboxes("Randomize");
                CreateRandomizeTransformWindow();
                break;
            case 3:
                CreateAxisCheckboxes("Add noise");
                CreateAddNoiseToTransformWindow();
                break;
        }
    }
 
    /// <summary>
    /// Draws the 3 axis checkboxes (x y z)
    /// </summary>
    /// <param name="operationName"></param>
    private void CreateAxisCheckboxes(string operationName)
    {
        GUILayout.Label(operationName + " on axis", EditorStyles.boldLabel);
 
        GUILayout.BeginHorizontal();
            xCheckbox = GUILayout.Toggle(xCheckbox, "X");
            yCheckbox = GUILayout.Toggle(yCheckbox, "Y");
            zCheckbox = GUILayout.Toggle(zCheckbox, "Z");
        GUILayout.EndHorizontal();
 
        EditorGUILayout.Space();
    }
 
    /// <summary>
    /// Draws the range min and max fields
    /// </summary>
    private void CreateRangeFields()
    {
        GUILayout.Label("Range", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        randomRangeMin = EditorGUILayout.FloatField("Min:", randomRangeMin);
        randomRangeMax = EditorGUILayout.FloatField("Max:", randomRangeMax);
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }
 
    /// <summary>
    /// Creates the Align transform window
    /// </summary>
    private void CreateAlignTransformWindow()
    {
        //Source transform
        GUILayout.BeginHorizontal();
        GUILayout.Label("Align to: \t");
        source = EditorGUILayout.ObjectField(source, typeof(Transform)) as Transform;
        GUILayout.EndHorizontal();
 
        string[] texts = new string[4] { "Min", "Max", "Center", "Pivot" };
 
        //Display align options
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Selection:", EditorStyles.boldLabel);
        alignSelectionOption = GUILayout.SelectionGrid(alignSelectionOption, texts, 1);
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        GUILayout.Label("Source:", EditorStyles.boldLabel);
        alignSourceOption = GUILayout.SelectionGrid(alignSourceOption, texts, 1);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
 
        EditorGUILayout.Space();
 
        //Position
        if (GUILayout.Button("Align"))
        {
            if (source != null)
            {
                //Add a temporary box collider to the source if it doesn't have one
                Collider sourceCollider = source.collider;
                bool destroySourceCollider = false;
                if (sourceCollider == null)
                {
                    sourceCollider = source.gameObject.AddComponent<BoxCollider>();
                    destroySourceCollider = true;
                }
 
                foreach (Transform t in Selection.transforms)
                {
                    //Add a temporary box collider to the transform if it doesn't have one
                    Collider transformCollider = t.collider;
                    bool destroyTransformCollider = false;
                    if (transformCollider == null)
                    {
                        transformCollider = t.gameObject.AddComponent<BoxCollider>();
                        destroyTransformCollider = true;
                    }
 
                    Vector3 sourceAlignData = new Vector3();
                    Vector3 transformAlignData = new Vector3();
 
                    //Transform
                    switch (alignSelectionOption)
                    {
                        case 0: //Min
                            transformAlignData = transformCollider.bounds.min;
                            break;
                        case 1: //Max
                            transformAlignData = transformCollider.bounds.max;
                            break;
                        case 2: //Center
                            transformAlignData = transformCollider.bounds.center;
                            break;
                        case 3: //Pivot
                            transformAlignData = transformCollider.transform.position;
                            break;
                    }
 
                    //Source
                    switch (alignSourceOption)
                    {
                        case 0: //Min
                            sourceAlignData = sourceCollider.bounds.min;
                            break;
                        case 1: //Max
                            sourceAlignData = sourceCollider.bounds.max;
                            break;
                        case 2: //Center
                            sourceAlignData = sourceCollider.bounds.center;
                            break;
                        case 3: //Pivot
                            sourceAlignData = sourceCollider.transform.position;
                            break;
                    }
 
                    Vector3 tmp = new Vector3();
                    tmp.x = xCheckbox ? sourceAlignData.x - (transformAlignData.x - t.position.x) : t.position.x;
                    tmp.y = yCheckbox ? sourceAlignData.y - (transformAlignData.y - t.position.y) : t.position.y;
                    tmp.z = zCheckbox ? sourceAlignData.z - (transformAlignData.z - t.position.z) : t.position.z;
 
                    //Register the Undo
                    Undo.RegisterUndo(t, "Align " + t.gameObject.name + " to " + source.gameObject.name);
                    t.position = tmp;
                    
                    //Ugly hack!
                    //Unity needs to update the collider of the selection to it's new position
                    //(it stores in cache the collider data)
                    //We can force the update by a change in a public variable (shown in the inspector), 
                    //then a call SetDirty to update the collider (it won't work if all inspector variables are the same).
                    //But we want to restore the changed property to what it was so we do it twice.
                    transformCollider.isTrigger = !transformCollider.isTrigger;
                    EditorUtility.SetDirty(transformCollider);
                    transformCollider.isTrigger = !transformCollider.isTrigger;
                    EditorUtility.SetDirty(transformCollider);
 
                    //Destroy the collider we added
                    if (destroyTransformCollider)
                    {
                        DestroyImmediate(transformCollider);
                    }
                }
 
                //Destroy the collider we added
                if (destroySourceCollider)
                {
                    DestroyImmediate(sourceCollider);
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "There is no source transform", "Ok");
                EditorApplication.Beep();
            }
        }
    }
 
    /// <summary>
    /// Creates the copy transform window
    /// </summary>
    private void CreateCopyTransformWindow()
    {
        //Source transform
        GUILayout.BeginHorizontal();
            GUILayout.Label("Copy from: \t");
            source = EditorGUILayout.ObjectField(source, typeof(Transform)) as Transform;
        GUILayout.EndHorizontal();
 
        EditorGUILayout.Space();
 
        //Position
        if (GUILayout.Button("Copy Position"))
        {
            if (source != null)
            {
                foreach (Transform t in Selection.transforms)
                {
                    Vector3 tmp = new Vector3();
                    tmp.x = xCheckbox ? source.position.x : t.position.x;
                    tmp.y = yCheckbox ? source.position.y : t.position.y;
                    tmp.z = zCheckbox ? source.position.z : t.position.z;
 
                    Undo.RegisterUndo(t, "Copy position");
                    t.position = tmp;
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "There is no source transform", "Ok");
                EditorApplication.Beep();
            }
        }
 
        //Rotation
        if (GUILayout.Button("Copy Rotation"))
        {
            if (source != null)
            {
                foreach (Transform t in Selection.transforms)
                {
                    Vector3 tmp = new Vector3();
                    tmp.x = xCheckbox ? source.rotation.eulerAngles.x : t.rotation.eulerAngles.x;
                    tmp.y = yCheckbox ? source.rotation.eulerAngles.y : t.rotation.eulerAngles.y;
                    tmp.z = zCheckbox ? source.rotation.eulerAngles.z : t.rotation.eulerAngles.z;
                    Quaternion tmp2 = t.rotation;
                    tmp2.eulerAngles = tmp;
 
                    Undo.RegisterUndo(t, "Copy rotation");
                    t.rotation = tmp2;
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "There is no source transform", "Ok");
                EditorApplication.Beep();
            }
        }
 
        //Local Scale
        if (GUILayout.Button("Copy Local Scale"))
        {
            if (source != null)
            {
                foreach (Transform t in Selection.transforms)
                {
                    Vector3 tmp = new Vector3();
                    tmp.x = xCheckbox ? source.localScale.x : t.localScale.x;
                    tmp.y = yCheckbox ? source.localScale.y : t.localScale.y;
                    tmp.z = zCheckbox ? source.localScale.z : t.localScale.z;
 
                    Undo.RegisterUndo(t, "Copy local scale");
                    t.localScale = tmp;
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "There is no source transform", "Ok");
                EditorApplication.Beep();
            }
        }
    }
 
    /// <summary>
    /// Creates the Randomize transform window
    /// </summary>
    private void CreateRandomizeTransformWindow()
    {
        CreateRangeFields();
 
        //Position
        if (GUILayout.Button("Randomize Position"))
        {
            foreach (Transform t in Selection.transforms)
            {
                Vector3 tmp = new Vector3();
                tmp.x = xCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : t.position.x;
                tmp.y = yCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : t.position.y;
                tmp.z = zCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : t.position.z;
 
                Undo.RegisterUndo(t, "Randomize position");
                t.position = tmp;
            }
        }
 
        //Rotation
        if (GUILayout.Button("Randomize Rotation"))
        {
            foreach (Transform t in Selection.transforms)
            {
                Vector3 tmp = new Vector3();
                tmp.x = xCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : t.rotation.eulerAngles.x;
                tmp.y = yCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : t.rotation.eulerAngles.y;
                tmp.z = zCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : t.rotation.eulerAngles.z;
                Quaternion tmp2 = t.rotation;
                tmp2.eulerAngles = tmp;
 
                Undo.RegisterUndo(t, "Randomize rotation");
                t.rotation = tmp2;
            }
        }
 
        //Local Scale
        if (GUILayout.Button("Randomize Local Scale"))
        {
            foreach (Transform t in Selection.transforms)
            {
                Vector3 tmp = new Vector3();
                tmp.x = xCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : t.localScale.x;
                tmp.y = yCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : t.localScale.y;
                tmp.z = zCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : t.localScale.z;
 
                Undo.RegisterUndo(t, "Randomize local scale");
                t.localScale = tmp;
            }
        }
    }
 
    /// <summary>
    /// Creates the Add Noise To Transform window
    /// </summary>
    private void CreateAddNoiseToTransformWindow()
    {
        CreateRangeFields();
 
        //Position
        if (GUILayout.Button("Add noise to Position"))
        {
            foreach (Transform t in Selection.transforms)
            {
                Vector3 tmp = new Vector3();
                tmp.x = xCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : 0;
                tmp.y = yCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : 0;
                tmp.z = zCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : 0;
 
                Undo.RegisterUndo(t, "Add noise to position");
                t.position += tmp;
            }
        }
 
        //Rotation
        if (GUILayout.Button("Add noise to Rotation"))
        {
            foreach (Transform t in Selection.transforms)
            {
                Vector3 tmp = new Vector3();
                tmp.x = xCheckbox ?  t.rotation.eulerAngles.x + Random.Range(randomRangeMin, randomRangeMax) : 0;
                tmp.y = yCheckbox ?  t.rotation.eulerAngles.y + Random.Range(randomRangeMin, randomRangeMax) : 0;
                tmp.z = zCheckbox ?  t.rotation.eulerAngles.z + Random.Range(randomRangeMin, randomRangeMax) : 0;
 
                Undo.RegisterUndo(t, "Add noise to rotation");
                t.rotation = Quaternion.Euler(tmp);
            }
        }
 
        //Local Scale
        if (GUILayout.Button("Add noise to Local Scale"))
        {
            foreach (Transform t in Selection.transforms)
            {
                Vector3 tmp = new Vector3();
                tmp.x = xCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : 0;
                tmp.y = yCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : 0;
                tmp.z = zCheckbox ? Random.Range(randomRangeMin, randomRangeMax) : 0;
 
                Undo.RegisterUndo(t, "Add noise to local scale");
                t.localScale += tmp;
            }
        }
    } 
}

//4. Editor
//对某自定义组件进行观察的Inspector窗口，可以从它派生
using System;
using UnityEngine;
 
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Star : MonoBehaviour {
 
	[Serializable]
	public class Point {
		public Color color;
		public Vector3 offset;
	}
 
	public Point[] points;
	public int frequency = 1;
	public Color centerColor;
 
	private Mesh mesh;
	private Vector3[] vertices;
	private Color[] colors;
	private int[] triangles;
 
	void Start () {
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Star Mesh";
 
		if(frequency < 1){
			frequency = 1;
		}
		if(points == null || points.Length == 0){
			points = new Point[]{ new Point()};
		}
 
		int numberOfPoints = frequency * points.Length;
		vertices = new Vector3[numberOfPoints + 1];
		colors = new Color[numberOfPoints + 1];
		triangles = new int[numberOfPoints * 3];
		float angle = -360f / numberOfPoints;
		colors[0] = centerColor;
		for(int iF = 0, v = 1, t = 1; iF < frequency; iF++){
			for(int iP = 0; iP < points.Length; iP += 1, v += 1, t += 3){
				vertices[v] = Quaternion.Euler(0f, 0f, angle * (v - 1)) * points[iP].offset;
				colors[v] = points[iP].color;
				triangles[t] = v;
				triangles[t + 1] = v + 1;
			}
		}
		triangles[triangles.Length - 1] = 1;
 
		mesh.vertices = vertices;
		mesh.colors = colors;
		mesh.triangles = triangles;
	}
}
using UnityEditor;
using UnityEngine;
 
[CustomEditor(typeof(Star))]
public class StarInspector : Editor {
 
	private static GUIContent
		insertContent = new GUIContent("+", "duplicate this point"),
		deleteContent = new GUIContent("-", "delete this point"),
		pointContent = GUIContent.none;
 
	private static GUILayoutOption
		buttonWidth = GUILayout.MaxWidth(20f),
		colorWidth = GUILayout.MaxWidth(50f);
 
	private SerializedObject star;
	private SerializedProperty
		points,
		frequency,
		centerColor;
 
	void OnEnable () { … }
 
	public override void OnInspectorGUI () {
		star.Update();
 
		GUILayout.Label("Points");
		for(int i = 0; i < points.arraySize; i++){
			EditorGUILayout.BeginHorizontal();
			SerializedProperty point = points.GetArrayElementAtIndex(i);
			EditorGUILayout.PropertyField(point.FindPropertyRelative("offset"), pointContent);
			EditorGUILayout.PropertyField(point.FindPropertyRelative("color"), pointContent, colorWidth);
 
			if(GUILayout.Button(insertContent, EditorStyles.miniButtonLeft, buttonWidth)){
				points.InsertArrayElementAtIndex(i);
			}
			if(GUILayout.Button(deleteContent, EditorStyles.miniButtonRight, buttonWidth)){
				points.DeleteArrayElementAtIndex(i);
			}
 
			EditorGUILayout.EndHorizontal();
		}
 
		EditorGUILayout.PropertyField(frequency);
		EditorGUILayout.PropertyField(centerColor);
 
		star.ApplyModifiedProperties();
	}
}