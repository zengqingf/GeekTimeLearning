/*
Unity3D的EditorUtility类
EditorUtility.SetDirty ：这个函数告诉引擎，相关对象所属于的Prefab已经发生了更改。方便，当我们更改了自定义对象的属性的时候，自动更新到所属的Prefab中。

EditorUtility.IsPersistent：这个函数用于判定是否对象是被保存到硬盘中的。

EditorUtility.DisplayDialog：显示一个对话框，类似于MessageBox，有Yes、No按钮

EditorUtility.OpenFilePanel/SaveFilePanel：选择/保存一个文件的文件摄取窗口。

EditorUtility.OpenFolderPanel/SaveFolderPanel：选择/保存一个文件夹的文件夹摄取窗口

EditorUtility.CompressTexture：压缩图片到相应格式

EditorUtility.CloneComponent：复制一个现有的Component

EditorUtility.CopySerialized：拷贝一个Object所有的属性设置等到另外一个Object

EditorUtility.GetMiniThumbnail：得到资源小图标
*/