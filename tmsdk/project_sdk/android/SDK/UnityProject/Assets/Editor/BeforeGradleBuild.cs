using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Android;
using System;
using System.IO;
using UnityEditor.Callbacks;

public class BeforeGradleBuild : IPostGenerateGradleAndroidProject
{
    public int callbackOrder
    {
        get
        {
            return 999;
        }
    }

    public void OnPostGenerateGradleAndroidProject(string path)
    {
        string gradlePropertiesFile = Path.Combine(path, "gradle.properties");
        if (File.Exists(gradlePropertiesFile))
        {
            File.Delete(gradlePropertiesFile);
        }
        int cpuCount = Environment.ProcessorCount;
        StreamWriter writer = new StreamWriter(gradlePropertiesFile, true);
        string jvmArgs = string.Format("org.gradle.jvmargs=-Xmx3072M -Xms2048M -Xmn2G " +
            "-XX:+UseParallelGC " +
            "-XX:ParallelGCThreans={0} " +
            "-XX:+UseParallelOldGC " +
            "-XX:UseAdaptiveSizePolicy=100" +
            "-XX:+HeapDumpOnOutOfMemoryError" +
            "-Dfile.encoding=UTF-8",
            cpuCount);
        writer.WriteLine(jvmArgs);//jvm调优
        writer.WriteLine("org.gradle.daemon=true");//gradle 守护模式，减少jvm启动时间
        writer.WriteLine("org.gradle.parallel=true");//gradle 并行编译，多任务并行
        writer.WriteLine("org.gradle.configureondemand=true"); //gradle 孵化功能
        writer.WriteLine("android.enableD8.desugaring=true"); //D8 编译，下一代android java编译器，更快！
        writer.Flush();
        writer.Close();
        //Debug.LogError(DateTime.Now);
        start = DateTime.Now;
    }
    static DateTime start;
    /// <summary>
    /// Build完成后的回调
    /// </summary>
    /// <param name="target">打包的目标平台</param>
    /// <param name="pathToBuiltProject">包体的完整路径</param>
    [PostProcessBuild(1)]
    public static void AfterBuild(BuildTarget target, string pathToBuiltProject)
    {
        var delta = DateTime.Now - start;
        Debug.LogError(delta.TotalMilliseconds);


    }
}
