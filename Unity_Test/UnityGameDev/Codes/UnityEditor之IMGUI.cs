/*
ref: https://www.cnblogs.com/suoluo/p/10769289.html
Unity编辑器的扩展：IMGUI

所有关于 Editor 的相关 UI，包括 Inspector、Hierarchy、Window、Game 视图上动态创建的那些半透明 UI、还有 Scene 视图上可添加的辅助显示 UI，叫做 IMGUI，全称 Immediate Mode GUI。该名字来源于两类型的 UI 系统：immediate 和 retained。

retained：当你设置好各种组件如 Text、Button 等的信息，或修改它们的相关属性后，这些组件的相关信息和改动就被保存（retained）下来了，系统会根据这些新的信息来绘制响应事件等，你可以随时去查询如 Text 文本内容或颜色等信息。UGUI 即是典型的 retained mode GUI。
immediate：跟上面的相反，系统不会自动保存 UI 控件上的各种信息，不会用上次的状态继续工作，而是反复的询问你这些控件应当是处于什么位置什么文本等状态信息。因此任何的用户交互结果是立即呈现返回给用户，而不是当用户需要的时候自行查询。如：
bool selected = false;
void OnGUI()
{
    selected = GUILayout.Toggle(selected, "A Toggle text");
    if (selected)
    {
        DoSomething()
    }

    //if (GUILayout.Toggle(selected, "A Toggle text");)
    //{
    //  DoSomething()
    //}
}


OnGUI 会被反复调用以更新绘制 UI，通常控件的返回值需要自行保存下来再传入控件中以更新控件状态，如果像注释中的代码那样则 Toggle 的状态改变后下一次更新则又变为旧的了，感受就像功能失效了一下。
IMGUI 是十分低效的，它是纯代码驱动的，对于美术而言基本无法使用（非可视化的，稍复杂点的 UI 程序写起来也很蛋疼...）。但是对于非实时交互的情况下却是一种可选的方式，比如 Inspector 上的 UI，它本身就是对代码脚本的扩展，通常不是美术人员所写脚本，控件可立即展示对应的脚本状态的修改。
关于IMGUI的基本介绍请看官方文档
*/


/*
相关类介绍
Editor 类和 EditorWindow 类都继承自同一个基类:ScriptableObject，因此他们都可以针对某种脚本类来进行操作。
Editor 类只能定制针对脚本的扩展，从脚本内容在 Inspector 里的显示布局，到变量在 Scene 视图的可视化编辑
EditorWindow 主要是扩展编辑器的功能，不必针对某种脚本（虽然可以做到），而且它有独立的窗口，使用 OnGUI 函数来绘制 2D 的 UI。
能在 Game 视图上显示的 ingame GUI 主要是 GUI 和 GUILayout 两个类，另外与之对应的 editor-only 的类是 EditorGUI 和 EditorGUILayout 两个类，两套类提供的控件功能都差不多，可以混合搭配一起使用。
GUI 和 EditorGUI 提供的接口为 Fixed Layout 的，基本上都需要传入一个 Rect 变量来指定控件的位置和大小，当窗口大小改时控件会保持不变。可以将代码放到 GUI.BeginGroup() 和 GUI.EndGroup() 之间将控件进行分组或划分子区域进行布局。
GUILayout 和 EditorGUILayout 则是与前面两个对应的 Auto Layout 类，不需要指定控件位置和大小，会根据当前显示区域的大小自动调整布局适应变化。多个控件默认是从上往下的顺序排列，可以用 GUILayout.BeginHoriztontal(), GUILayout.EndHorizontal(), GUILayout.BeginVertical(), GUILayout.EndVertical()（或者对应的 EditorGUILayout 类）将代码写到这些调用之间进行水平或者垂直排列控件，将这些布局互相组合或嵌套即可排布出复杂的 UI 界面。
GUIUtility 类提供了一些工具方法，如获取控件 id、转换 Screen 和 GUI 之间的坐标等。
EditorGUIUtility 类是针对 Editor 提供一些工具方法，除了 GUIUtility 那些方法外还增加了很多有用的方法，如获取内建资源图标、高亮选中某个物体、产生复制粘贴命令等。
Event：该类包含了所有的用户输入如按钮或鼠标点击等和 UI 相关布局和绘制事件。调用 Event.current 获得当前事件信息，查看 EventType 枚举定义可查询全部可用类型事件。这个信息在 OnGUI,OnInspector,OnSceneGUI 里都可以使用以处理一些特定逻辑。
*/

/*
自定义控件

例如：
GUILayout.Button(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
大部分控件都可以传入 GUIContent、GUIStyle 来指定控件的风格外观，自动布局的控件还可以传入多个 GUILayoutOption 组合来设定大小。

GUIContent：该类定义了控件需要显示什么（what to render），包含三个基本要素：图片 image、文本 text、鼠标停留的提示信息 tooltip（play mode 运行时 tooltip 无效）。也可以用控件的其它重载分别传入这几项中的一个或多个内容。可以用
EditorGUIUtility.IconContent(name)获得一个内置图标，如下可获得一个播放按钮：
GUILayout.Button(EditorGUIUtility.IconContent("PlayButton")

icon集合：http://www.xuanyusong.com/archives/3777


GUIStyle：该类定义了控件要如何显示（how to render），包括 Normal Hover Active 等状态切换显示、文字大小颜色、指定图标显示位置等各种信息。每种类型的控件都会有默认的外观风格，通常可以在现有的控件风格上进行修改：
//默认控件文本会显示在图标之后，下面可获得图上字下的按钮风格
GUIStyle style = new GUIStyle(GUI.skin.button); //或者传入 unity 的默认风格名称 new GUIStyle("button")
style.imagePosition = ImagePosition.ImageAbove;

GUILayoutOption：该类为自动布局 GUILayout 和 EditorGUILayout 的控件提供一系列预定条件，如最小宽度、最大高度、是否横向拉伸等。
GUISkin：是一系列GUIStyle的集合，可对每种控件分别指定样式，可设置一整套风格统一完全不同于默认风格的UI。通过 Assets->Create->GUI Skin 可创建，代码中 GUI.skin = customSkin; 即可一次应用所有的 GUIStyle。
*/


/*
当要实现一个自定义功能的控件，大体的处理流程为以下所示代码，该代码为 GUILayout.RepeatButton 的主要代码，该代码在 OnGUI 中被调用：
*/
public static class TestEditor
{
    private static bool DoRepeatButton(
    Rect position,
    GUIContent content,
    GUIStyle style,
    FocusType focusType)
    {
        GUIUtility.CheckOnGUI();
        //分配一个唯一 id 值给该控件，传入的第一个参数为任意唯一的值，此处为一个 string 的 hash。
        int controlId = GUIUtility.GetControlID(GUI.s_RepeatButtonHash, focusType, position);
        //获得对应的各种事件并处理关心的。
        switch (Event.current.GetTypeForControl(controlId))
        {
            case EventType.MouseDown:
                //鼠标的点击位置在当前控件上。
                if (position.Contains(Event.current.mousePosition))
                {
                    //保存当前的 id 为 hot 的控件，全局只能有一个为 hot 控件。
                    GUIUtility.hotControl = controlId;
                    //消耗掉当前事件防止后续控件处理无效逻辑。（故所有在该控件之后的控件全部无法再判断点击事件）
                    Event.current.Use();
                }
                return false;
            case EventType.MouseUp:
                //仅当前的 hot 控件为本控件时才处理对应逻辑，忽略掉其它。
                if (GUIUtility.hotControl != controlId)
                    return false;
                //当前 hot 控件功能结束后一定要置 0 恢复，否则当前 UI 是冻结的，其它控件全部无法响应。
                GUIUtility.hotControl = 0;
                Event.current.Use();
                return position.Contains(Event.current.mousePosition);
            case EventType.Repaint:
                //该事件处理显示相关。
                style.Draw(position, content, controlId);
                return controlId == GUIUtility.hotControl && position.Contains(Event.current.mousePosition);
            default:
                return false;
        }
    }
}
/*
关于 ControlID 相关概念，我个人理解感觉作用不大，GetControlID 获得的 id 与控件其实并没有直接的联系，连续多次调用便能获得多个不同的值，在每次 OnGUI 结束后 id 栈信息就会清空以便每次重入时能产生与之前一致的 id。id 与控件的关系为手动关联起来的，因代码的顺序执行，当前环境的后续以该 id 相关的处理 “认为” 即是对应该控件。如以上代码，GUI.xxx 等内置的控件中全部都有对应的一个 id，该 id 外部是无法访问的，故在 GUI.Button 之后立即调用 GetControlID 是得不到 Button 的 id 的，仅是又产生了一个新的值，同样不能再拿到控件内部已处理过的事件。
GetTypeForControl 获得传入 id 对应的事件类型，该信息与实际控件同样是无直接关联的，必须同时判断 controlPosition.Contains(Event.current.mousePosition) 才能得知为当前控件上的事件，经测试发现它与 Event.current.type 并没有什么区别，即使当前 hotControl 或 keyboardControl 不是当前 id，它好像没有针对传入的 id 作任何有效过滤。
比较有用的可能是 GUIUtility.GetStateObject，它给 id 绑定了一个自定义的数据信息以供后续逻辑处理，从而不需要自己维护与各控件相关的数据。
*/


/*
Getting control 0’s position in a group with only 0 controls when doing Repaint.

OnGUI 循环实际上是被一系列的 Event 所调用，如，IMGUI 会在 EventType.Layout 中收集所有控件的包含关系及占用的空间大小位置等信息，然后在 EventType.Repaint 事件中才实际以 Layout 中统计的信息来分配空间绘制显示。
如果某逻辑在 Layout 与 Repaint 之间导致了 UI 数据不一致时就会出现上面类似的报错，有时也会看到一个 if 判断肯定是进不去的但实际却进去了等现象也是在不同的事件中情况会不一样。
此时需要仔细检查逻辑是否有意外导致 Layout 布局在即将 Repaint 显示时不一致。
可以将可能有问题的代码写在下面代码块中：

if (Event.current.type == EventType.Repaint)
{
     //
}
或
if (Event.current.type == EventType.Layout)
{
     //
}

GUIUtility.ExitGUI 也可处理该报错，但后续代码也得不到执行了……该接口在 2018 上有文档说明，在 5.x 上面搜不到，但实际上仍是可调用的。
NullReferenceException: Object reference not set to an instance of an object
UnityEngine.GUILayoutUtility.BeginLayoutGroup (UnityEngine.GUIStyle style, UnityEngine.GUILayoutOption[] options, System.Type layoutType) (at /Users/builduser/buildslave/unity/build/Runtime/IMGUI/Managed/GUILayoutUtility.cs:296)
该报错有一种情况是当调用了 EditorUtility.DisplayProgressBar 显示进度条时会出现，它会主动调用到 OnGUI，猜测是当前的 OnGUI 还未结束就再次进入 OnGUI 导致的类似上一个问题中的在函数重入时数据不一致布局信息错乱。
暂时未找到解决方法，只有在卡顿死等与进度展示伴随报错二者中选择了。



GUIContent 中设置的 tooltip 功能只在非运行起来时可用，将编辑器运行后即失效，最后查到该问题是 By Design...
任一个 GUIStyle 可用于任一类似的控件，在美术不提供图的情况下混用 style 即可搭出较好的效果：
//传入默认风格名称即可将整块垂直显示区域添加一个类似文本框的背景以区分其他区域
EditorGUILayout.BeginVertical("textfield");
...
EditorGUILayout.EndVertical();

//将按钮风格显示成工具栏按钮的样式有时可能会更美观
if (GUILayout.Button("My Button", EditorStyles.toolbarButton))
{
}

//把单选的组按钮 button 改成选框 toggle 样式
GUILayout.SelectionGrid(m_selectedIndex, m_Names, 1, EditorStyles.toggleGroup)

获得一个内置的默认风格有三种方式："textfield"、GUI.skin.textField、EditorStyles.textField。
文本内容想要其中部分文字添加一个颜色以突出显示，需要开启富文本支持 myGUIStyle.richText = true;

*/

/*
由于 Unity 编译顺序决定了 Runtime 脚本是无法调用 Editor 代码的，有些逻辑因历史原因不方便修改，但非要运行时调用编辑器脚本，有个办法是在编辑器脚本初始化时去绑定运行时脚本中的静态委托或事件：
*/
public class MyRuntimeScript : MonoBehaviour
{
#if UNITY_EDITOR
    public static System.Action<GameObject> onEvent;
#endif
    //...
#if UNITY_EDITOR
    if (onEvent != null)
        onEvent(gameObject);
#endif
}

public class MyEditorScript
{
    [InitializeOnLoadMethod]
    static void Init()
    {
        MyRuntimeScript.onEvent = go =>
        {
            //...
        };
    }
}

/*
有时需要在自动布局（GUILayout/EditorGUILayout）中插入固定布局（GUI/EditorGUI）控件，如以 UV 坐标显示一张图片时只有固定布局接口 GUI.DrawTextureWithTexCoords，需要传入固定布局接口一个 Rect，但此时很难确定自动布局下当前位置坐标与大小，可调用 Rect rect = GUILayoutUtility.GetRect(new GUIContent(), GUIStyle.none); 或 Rect rect = GUILayoutUtility.GetRect(0f, 10f, GUILayout.ExpandWidth(true)); 获得一片当前空间可用的一块区域，后续可基于该 rect 进行坐标计算。另 rect.Contains(Event.current.mousePosition)可判断鼠标是否在某区域内。
GUILayout.FlexibleSpace()可以将空白区域全部占满。自动布局 Layout.XXX 控件默认是会占据尽量大的空间（通常是整个窗口的宽度），连续两个控件想一个在最左边一个最右边时，在之间插入该调用即可，同理三个控件之间插入即可实现平均占据整行空间的排列，这仅靠 BeginHorizontal() 或 BeginVertical() 组合是比较难实现的。
*/