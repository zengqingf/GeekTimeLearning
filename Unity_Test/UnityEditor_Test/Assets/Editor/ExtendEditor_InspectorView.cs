using UnityEngine;
using UnityEditor;


//扩展原生组件
[CustomEditor(typeof(Camera))]              //自定义组建类型
public class CameraExtend : Editor
{
	public override void OnInspectorGUI(){  //重新绘制
		if (GUILayout.Button ("拓展按钮")) {
		}
		base.OnInspectorGUI ();             //是否绘制父类所有元素
	}
}


//扩展继承组件（保留系统内部绘制方法，通过反射调用内部未公开方法）
[CustomEditor(typeof(Transform))]
public class TransformExtend :Editor
{
	private Editor m_Editor;
	void OnEnable()
	{
        //通过反射得到TransformInspector对象，然后调用其内部的OnInspectorGUI()方法
		m_Editor = Editor.CreateEditor(target, 
			Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.TransformInspector",true));
	}

	public override void OnInspectorGUI(){
		if (GUILayout.Button ("拓展按钮")) {        //先绘制自定义按钮，再绘制原有元素，可以调整显示上下
		}
		//调用系统绘制方法
		m_Editor.OnInspectorGUI();
//		base.OnInspectorGUI ();
	}
}
