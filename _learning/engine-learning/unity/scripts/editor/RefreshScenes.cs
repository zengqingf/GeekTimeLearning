public class RefreshScene : Editor {

    private static readonly string scenePath = "Scenes";
    // 添加菜单选项
    [MenuItem("Tool/RefreshScene")]
    static void RefreshAllScene()
    {
        // 设置场景 *.unity 路径
        string path = Path.Combine(Application.dataPath, scenePath);
        // 遍历获取目录下所有 .unity 文件
        string[] files = Directory.GetFiles(path, "*.unity", SearchOption.AllDirectories);

        // 定义 场景数组
        EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[files.Length];
        for (int i = 0; i < files.Length; ++i)
        {
            string scenePath = files[i];
            // 通过scene路径初始化
            scenes[i] = new EditorBuildSettingsScene(scenePath, true);
        }

        // 设置 scene 数组
        EditorBuildSettings.scenes = scenes;
    }
}