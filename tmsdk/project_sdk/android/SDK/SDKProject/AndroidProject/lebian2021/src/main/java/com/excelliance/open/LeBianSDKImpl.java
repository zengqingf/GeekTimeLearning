package com.excelliance.open;

import android.app.Activity;
import android.content.ContentResolver;
import android.content.Context;
import android.util.Log;
import android.view.Window;

import com.excelliance.lbsdk.IQueryUpdateCallback;
import com.excelliance.lbsdk.LebianSdk;

/**
 * Android Plugin for Unity
 * Created by Administrator on 2017/9/17.
 */
public class LeBianSDKImpl
{
    private static Context appContext;
    private static Activity appActivity;
   // private static ContentResolver appContentResolver;
   // private static Window appWindow;

    // TODO: 2019/3/20
    private static String versionDesc = "LEBIAN Android SDK V20.2.1-20190718-Other-国内";


    public static void SetCommonContext(Object object)
    {
        appContext = ((Activity)object).getApplicationContext();
        appActivity = (Activity)object;
    //    appContentResolver = ((Activity)object).getContentResolver();
    //    appWindow = ((Activity)object).getWindow();
    }

    public static void RequestUpdate()
    {
        if(appContext!=null) {
            LebianSdk.queryUpdate(appContext, new IQueryUpdateCallback() {
                @Override
                public void onUpdateResult(int i) {
                    ShowLogByLB("Lebain Sdk version is "+versionDesc);
                    final String s;
                    switch (i)
                    {
                        case -2:s="SDK未准备好";
                            break;
                        case -1:s="请求失败";
                            break;
                        case 1:s="未知错误";
                            break;
                        case 2:s="没有更新";
                            break;
                        case 3:s="有非强更版本";
                            break;
                        case 4:s="有强更版本";
                            break;
                        default:s = "其他";
                            break;

                    }
                    ShowLogByLB(String.format(" LebianSdk.queryUpdate result = %s tag = %d", s, i));
                }
            },
                    null);
        }
    }

    public static void DownloadFullResNotify()
    {
        if(appContext!=null) {

        //关闭后流量网络进游戏就不会提示玩家下载了
        //GlobalSettings.SHOW_FIRST_DIALOG_WITHOUT_WIFI_BWBX = false;

        //此时再提示玩家下载
        LebianSdk.downloadFullRes(appContext);

        ShowLogByLB("DownloadFullResNotify :  GlobalSettings.SHOW_FIRST_DIALOG_WITHOUT_WIFI_BWBX is "+GlobalSettings.SHOW_FIRST_DIALOG_WITHOUT_WIFI_BWBX);
        ShowLogByLB("LebianSdk.downloadFullRes....");
        }
    }

    public static boolean IsAfterUpdate()
    {
        ShowLogByLB("invoke [LeBianSDKImpl] method : IsAfterUpdate");
        return LebianSdk.afterUpdate();
    }

    public static String GetFullResPath()
    {
        ShowLogByLB("invoke [LeBianSDKImpl] method : GetFullResPath");
        return LebianSdk.getResCachePath(appContext);
    }

    private static void ShowLogByLB(String log)
    {
        Log.w("LeBian",String.format("[LB][lb][lebian][LeBian]  %s",log));
    }
}
