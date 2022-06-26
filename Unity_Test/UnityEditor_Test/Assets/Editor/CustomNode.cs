/*
ref: 自定义节点编辑器
https://blog.csdn.net/feng888668/article/details/38701269


画曲线
Handles.DrawBezier (Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, Color color, Texture2D texture, float width)
startPosition：起点坐标
endPosition：终点坐标
startTangent：起点的正切值
endTangent：终点的正切值
color：线条颜色
texture：贴图
width：线条宽度
*/



using UnityEngine;
using UnityEditor;

//创建可拖动窗口
public class NodeEditor : EditorWindow {
	//窗口的矩形
	Rect windowRect = new Rect(50,50,150,100);
    Rect windowRect2 = new Rect(100, 100, 150, 100);

	//窗口的ID
	int windownID = 0;
    int windownID2 = 0;
    
	[MenuItem("Window/NodeEditor")]
	static void ShowEditor() {
		NodeEditor editor = EditorWindow.GetWindow<NodeEditor>();
	}
	
    void OnGUI() {
        BeginWindows();
		//绘画窗口
		windowRect = GUI.Window(windownID,windowRect,DrawNodeWindow,"Demo Window");
        windowRect2 = GUI.Window(windownID2,windowRect2,DrawNodeWindow,"Demo Window2");

        EndWindows();

        //连接窗口
        DrawNodeCurve(windowRect,windowRect2);
	}
	
	//绘画窗口函数
	void DrawNodeWindow(int id) {
        //创建一个GUI Button
		if (GUILayout.Button("Link")) {
			Debug.Log("Clikc Link Button");</span></span>
		}
		//设置改窗口可以拖动
		GUI.DragWindow();
	}

    void DrawNodeCurve(Rect start, Rect end , Color color) {
		Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
		Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
		Vector3 startTan = startPos + Vector3.right * 50;
		Vector3 endTan = endPos + Vector3.left * 50;
		Handles.DrawBezier(startPos, endPos, startTan, endTan, color, null, 4);
	}
}
