using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;


public class Notchtest : MonoBehaviour
{
    public RectTransform test;
    public int[] notch;
    public int[] notch2;

    public int notchw = 100;
    public int notchh = 400;


    public int minxp = 10;
    public int minyp = 10;
    public int maxxp = 10;
    public int maxyp = 10;

    // Start is called before the first frame update
    bool hasn = false;
    AndroidJavaClass java;
    AndroidJavaObject currentActivity;
    private void Awake()
    {
        //Screen.SetResolution((int)(Screen.width * 0.5f), (int)(Screen.height * 0.5f), true);
        //PlayerSettings.Android.renderOutsideSafeArea = true;
    }
    void Start()
    {
        
        java = new AndroidJavaClass("com.tm.commonlib.AndroidCommonImpl");
        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

        java.CallStatic("SetCommonContext",currentActivity);
        hasn = java.CallStatic<bool>("hasNotch");

        Debug.LogError(hasn);
        if (hasn)
        {
                    notch = java.CallStatic<int[]>("getNotchsize");

            Debug.LogError(string.Format("{0}    {1}", notch[0], notch[1]));
            Debug.LogError(string.Format("{0}    {1}     {2}    {3}", notch[0], notch[1], notch[2], notch[3]));
        }
        //int minx = (int)(-Screen.width * 0.5f);
        //int miny = (int)(notchh*0.5f-Screen.height * 0.5f);
        //int maxx = (int)(-Screen.width * 0.5f+notchw);
        //int maxy = (int)(notchh*0.5f);
        //notch = new int[4] { minx, miny, maxx, maxy };
        notch = new int[4] { 0, 0, 112, 112 };
        notch2 = new int[4] { Screen.width - 112, Screen.height - 112 , Screen.width, Screen.height};
        Debug.LogError(string.Format("{0}    {1}     {2}    {3}", Opengl2directx(notch)[0], Opengl2directx(notch)[1], Opengl2directx(notch)[2], Opengl2directx(notch)[3]));
        Debug.LogError(string.Format("{0}    {1}     {2}    {3}", Opengl2directx(notch2)[0], Opengl2directx(notch2)[1], Opengl2directx(notch2)[2], Opengl2directx(notch2)[3]));

    }
    bool istop = true;
    public int speed = 200;
    public bool re = false;

    public int[] Opengl2directx(int[] n)
    {
        return new int[4] { n[0], Screen.height - n[1], n[2], Screen.height - n[3] };
    }
    public int[] Directx2ui(int[] n)
    {
        return new int[4] { n[0] - Screen.width/2, n[1] - Screen.height/2, n[2] - Screen.width/2, n[3] - Screen.height/2 };

    }
    public int[] Resort(int[] n)
    {
        int tmp = 0;
        if (n[0] > n[2])
        {
            tmp = n[0];
            n[0] = n[2];
            n[2] = tmp;
        }

        if (n[1] > n[3])
        {
            tmp = n[1];
            n[1] = n[3];
            n[3] = tmp;
        }
        Debug.LogError(string.Format("{0}    {1}     {2}    {3}", n[0], n[1], n[2], n[3]));

        return n;
    }
    // Update is called once per frame
    void Update()
    {
        //    Debug.DrawLine(new Vector3(Directx2ui(Opengl2directx(notch))[0], Directx2ui(Opengl2directx(notch))[1], 0), new Vector3(Directx2ui(Opengl2directx(notch))[2], Directx2ui(Opengl2directx(notch))[3], 0), Color.red);
        ////Debug.DrawLine(new Vector3(minxp, minyp, 0), new Vector3(maxxp, maxyp, 0), Color.green);
        //Debug.DrawLine(new Vector3(notch2[0], notch2[1]), new Vector3(notch2[2], notch2[3]),Color.blue);
        //    Debug.DrawLine(new Vector3(Directx2ui(Opengl2directx(notch2))[0], Directx2ui(Opengl2directx(notch2))[1], 0), new Vector3(Directx2ui(Opengl2directx(notch2))[2], Directx2ui(Opengl2directx(notch2))[3], 0), Color.green);
        ////Debug.DrawLine(-new Vector3(minxp, minyp, 0), -new Vector3(maxxp, maxyp, 0), Color.green);

        //var iii = Resort(Opengl2directx(notch2));


        if (!hasn)
        {
            return;
        }
        
        notch = java.CallStatic<int[]>("getNotchsize");
        if (notch.Length == 2)
        {
            Debug.LogError(string.Format("{0}    {1}       {2}", notch[0], notch[1], Screen.orientation.ToString()));
        }
        else if (notch.Length == 4)
        {
            Debug.LogError(string.Format("{0}    {1}     {2}    {3}    {4}", notch[0], notch[1], notch[2], notch[3],Screen.orientation.ToString()));
        }

        return;

        if (test.localPosition.y <= -Screen.height * 0.5f)
        {
            istop = false;
        }
        else if (test.localPosition.y >= Screen.height * 0.5f)
            istop = true;

        if (istop)
        {
            test.localPosition += new Vector3(0, -Time.deltaTime * speed, 0);

        }
        else
        {
            test.localPosition += new Vector3(0, Time.deltaTime * speed, 0);

        }
        Notch(test);
    }
    public bool inNotch = false;
    public bool Notch(RectTransform rt)
    {
        int minx1 = (int)(-Screen.width * 0.5f);
        int miny1 = (int)(-notch[1] * 0.5f);
        int maxx1 = (int)(-Screen.width * 0.5f + notch[0]);
        int maxy1 = (int)(notch[1] * 0.5f);
        //notch = new int[4] { minx1, miny1, maxx1, maxy1 };

        //Vector2 min = new Vector2(notch[0], notch[1]);
        //Vector2 max = new Vector2(notch[2], notch[3]);

        int minx = Mathf.CeilToInt(rt.localPosition.x - rt.rect.width * 0.5f);
        int miny = Mathf.CeilToInt(rt.localPosition.y - rt.rect.height * 0.5f);
        int maxx = Mathf.CeilToInt(rt.localPosition.x + rt.rect.width * 0.5f);
        int maxy = Mathf.CeilToInt(rt.localPosition.y + rt.rect.height * 0.5f);

        if ((miny1 <= miny && miny <= maxy1) || (miny1 <= maxy && maxy <= maxy1))//在范围内
        {
            if (!inNotch)
            {
                inNotch = true;
                rt.position += new Vector3(notch[0], 0, 0);
            }
            return true;
        }
        else
        {
            if (inNotch)
            {
                inNotch = false ;
                rt.position -= new Vector3(notch[0], 0, 0);
            }
            return false;
        }


        return false;
    }

}
