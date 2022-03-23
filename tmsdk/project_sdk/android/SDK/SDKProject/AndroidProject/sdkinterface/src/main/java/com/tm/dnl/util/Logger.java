package com.tm.dnl.util;

import android.content.Context;
import android.util.Log;
import android.widget.Toast;


/**
 * Created by SmileMe on 2019/7/22.
 */
public class Logger {

    public static String LOG_TAG = "dnl_default";               //调用前进行初始化
    //public static boolean IsDebug = false;                      //调用前进行初始化

    public static void LogErrorDebug(String msg, Object...args)
    {
        if(GlobalParams.IsDebug) {
            String log = String.format(msg, args);
            Log.e(LOG_TAG, log);
        }
    }

    public static void LogError(String msg, Object...args)
    {
        String log = String.format(msg, args);
        Log.e(LOG_TAG, log);
    }

    public static void LogWarningDebug(String msg, Object...args)
    {
        if(GlobalParams.IsDebug) {
            String log = String.format(msg, args);
            Log.w(LOG_TAG, log);
        }
    }

    public static void LogWarning(String msg, Object...args)
    {
        String log = String.format(msg, args);
        Log.w(LOG_TAG, log);
    }

    public static void LogDebug(String msg, Object...args)
    {
        if(GlobalParams.IsDebug) {
            String log = String.format(msg, args);
            Log.d(LOG_TAG, log);
        }
    }

    public static void Log(String msg, Object...args)
    {
        String log = String.format(msg, args);
        Log.d(LOG_TAG, log);
    }

    public static void LogToastDebug(Context context, String toast, boolean isShortToast)
    {
        if(GlobalParams.IsDebug) {
            Log.d(LOG_TAG,toast);
            int toastType = Toast.LENGTH_LONG;
            if(isShortToast)
            {
                toastType = Toast.LENGTH_SHORT;
            }
            Toast.makeText(context, toast, toastType).show();
        }
    }

    public static void LogToast(Context context, String toast, boolean isShortToast)
    {
        Log.d(LOG_TAG,toast);
        int toastType = Toast.LENGTH_LONG;
        if(isShortToast)
        {
            toastType = Toast.LENGTH_SHORT;
        }
        Toast.makeText(context, toast, toastType).show();
    }
}
