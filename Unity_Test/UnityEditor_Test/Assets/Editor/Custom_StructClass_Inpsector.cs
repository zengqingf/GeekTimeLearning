/*
用PropertyDrawer自定义struct/class的外观
https://www.cnblogs.com/yangrouchuan/p/6698844.html

OnGUI提供了三个参数，依次解释一下：
position:该属性在Editor中被分配到的位置、大小。注意这里的x，y对应的是左上角，跟游戏中的左下角不同（因为Inspector是从上到下绘制）。大小的宽度由Inspector的宽度决定，而高度需要通过在类中override一个方法来自定义高度，否则默认为一行高。

property:待绘制的属性本身。Unity在编辑器的API中大部分的实际的值都是用一个SerializedProperty表示的，实际上就是对值的一个包装。通过这个包装，当我们修改值的时候，Unity可以知道这次操作，类似刷新界面、Undo、prefab修改之类的信息都可以帮我们处理好。坏处在于我们得通过类似FindPropertyRelative的方法，用字符串去寻找内部的值（SerializedProperty是个嵌套结构，内部的数据也是SerializedProperty）。在Unity升级C#来支持nameof之前，我们只能尽量避免修改字段的名字了。同时，我们绘制这些property的时候可以直接用EditorGUI.PropertyField(property)，而不用类似的 x = EditorGUI.IntField(x)这样的调用。

label:这个值在MonoBehaviour里的字段名。


在PropertyDrawer中不能使用带Layout的类，即EditorGUILayout、GUILayout

用PropertyField绘制的时候，并没有设置Label宽度的办法。
    EditorGUIUtility.labelWidth这个属性。代表的是Label的宽度。
    比较奇葩的是它是一个可写的属性，修改之后，之后绘制的label的宽度就变成了写进去的值了。
    不得不说，包括indentLevel在内，这些API设计的都很有想法。
    最后解决办法就是在PropertyField绘制之前，先把labelWidth改小，这样绘制出来的PropertyField前面的Label宽度就变小了。绘制完之后调回去即可。
*/
[CustomPropertyDrawer(typeof(TileCoord))]
public class TileCoordEditor : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        var x = property.FindPropertyRelative("x");
        var y = property.FindPropertyRelative("y");
        float LabelWidth = EditorGUIUtility.labelWidth;
        var labelRect = new Rect(position.x, position.y, LabelWidth, position.height);
        var xRect = new Rect(position.x + LabelWidth, position.y, (position.width - LabelWidth) / 2 - 20, position.height);
        var yRect = new Rect(position.x + LabelWidth + (position.width - LabelWidth) / 2 - 20 , position.y, (position.width - LabelWidth) / 2 - 20, position.height);
        
        EditorGUIUtility.labelWidth = 12.0f;
        EditorGUI.LabelField(labelRect, label);
        EditorGUI.PropertyField(xRect, x);
        EditorGUI.PropertyField(yRect, y);
        EditorGUIUtility.labelWidth = LabelWidth;
    }
//需要自定义高度
  //  public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
  //      return 
  //  }


      public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        var x = property.FindPropertyRelative("x");
        var y = property.FindPropertyRelative("y");
        float LabelWidth = 50;
        var labelRect = new Rect(position.x, position.y, LabelWidth, position.height);
        var xRect = new Rect(position.x + LabelWidth, position.y, (position.width - LabelWidth) / 2 , position.height);
        var yRect = new Rect(position.x + LabelWidth + (position.width - LabelWidth) / 2  , position.y, (position.width - LabelWidth) / 2 , position.height);
        
        EditorGUI.LabelField(labelRect, label);
        EditorGUI.PropertyField(xRect, x);
        EditorGUI.PropertyField(yRect, y);
    }
}
