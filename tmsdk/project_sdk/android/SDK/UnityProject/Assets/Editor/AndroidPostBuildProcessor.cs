using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Callbacks;
using UnityEngine;

public class AndroidPostBuildProcessor : IPostGenerateGradleAndroidProject
{
    //[PostProcessBuild(999)]
    //public static void PostGenerateGradleAndroidProject(BuildTarget buildTarget, string path)
    //{
    //    Debug.Log("### [AndroidPostBuildProcessor] - Bulid path : " + path);
    //    string gradlePropertiesFile = path + "/gradle.properties";
    //    if (File.Exists(gradlePropertiesFile))
    //    {
    //        File.Delete(gradlePropertiesFile);
    //    }
    //    StreamWriter writer = File.CreateText(gradlePropertiesFile);
    //    writer.WriteLine("android.enableD8.desugaring=false");
    //    writer.Flush();
    //    writer.Close();
    //}

    public int callbackOrder
    {
        get
        {
            return 999;
        }
    }


    void IPostGenerateGradleAndroidProject.OnPostGenerateGradleAndroidProject(string path)
    {
        //Debug.LogError("Bulid path : " + path);
        //string gradlePropertiesFile = path + "/gradle.properties";
        //if (File.Exists(gradlePropertiesFile))
        //{
        //    File.Delete(gradlePropertiesFile);
        //}
        //StreamWriter writer = File.AppendText(gradlePropertiesFile);
        //writer.WriteLine("android.enableD8.desugaring=false");
        //writer.Flush();
        //writer.Close();


        //string proguardunityFile = path + "/proguard-unity.txt";
        //if (File.Exists(proguardunityFile))
        //{
        //    File.Delete(proguardunityFile);
        //}
        //StreamWriter writer2 = File.AppendText(proguardunityFile);
        //writer2.WriteLine("-keepattributes *Annotation*");
        //writer2.WriteLine("-keep @**annotation** class * {*;}");
        //writer2.Flush();
        //writer2.Close();
    }
}
