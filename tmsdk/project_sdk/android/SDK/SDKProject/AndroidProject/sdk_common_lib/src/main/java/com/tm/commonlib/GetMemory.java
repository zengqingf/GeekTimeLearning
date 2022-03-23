package com.tm.commonlib;

import android.app.Activity;
import android.app.ActivityManager;
import android.content.Context;
import android.os.Debug;
import android.text.format.Formatter;

import java.io.BufferedReader;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.IOException;
import java.util.List;

/**
 * Created by SmileMe on 2020/3/3.
 */
public class GetMemory {
    private static final String FILENAME_PROC_MEMINFO = "/proc/meminfo";// 系统内存信息文件

    private Context appContext;
    private Activity appActivity;
    private ActivityManager appActivityManager;

    public GetMemory(Context context, Activity activity)
    {
        this.appContext = context;
        this.appActivity = activity;
        if(context != null)
        {
            this.appActivityManager =  (ActivityManager) context.getSystemService(Context.ACTIVITY_SERVICE);
        }
    }

    /*
    * 获取当前app 占用内存
    *
    * @return
    * */
    public String getCurrentProcessMemory()
    {
        if(appActivityManager == null)
        {
            return "Unavailable";
        }
        String pkgName = appContext.getPackageName();
        List<ActivityManager.RunningAppProcessInfo> appList = appActivityManager.getRunningAppProcesses();
        if(appList == null || appList.isEmpty())
        {
            return "Unavailable";
        }
        for (ActivityManager.RunningAppProcessInfo appInfo : appList){
            if(appInfo.processName.equals(pkgName)){
                int[] pidArray = new int[] {appInfo.pid};
                Debug.MemoryInfo[] memoryInfos = appActivityManager.getProcessMemoryInfo(pidArray);
                if(memoryInfos == null || memoryInfos.length <= 0) {
                    continue;
                }
                if(memoryInfos[0] == null){
                    continue;
                }
                float temp = (float)memoryInfos[0].getTotalPrivateDirty() / 1024.0f;
                return String.format("%.2f", temp) + "MB";
            }
        }
        return "Unavailable";
    }

    /**
     * 获取手机总可用内存大小
     *
     * @return
     */
    public String getTotalMemory() {
        String str1 = "/proc/meminfo";
        String str2;
        String[] arrayOfString;

        try {
            FileReader localFileReader = new FileReader(FILENAME_PROC_MEMINFO);
            BufferedReader localBufferedReader = new BufferedReader(
                    localFileReader, 8192);
            str2 = localBufferedReader.readLine();// 读取meminfo第一行，系统总内存大小
            if(str2 == null || str2.isEmpty())
            {
                return "Unavailable";
            }
            arrayOfString = str2.split("\\s+");
            float temp = 0f;
            if(arrayOfString != null && arrayOfString.length > 0) {
                temp = Integer.valueOf(arrayOfString[1])/1048576.0f;
            }
            localBufferedReader.close();
            return String.format("%.2f", temp)+"GB";
        } catch (FileNotFoundException e) {
            e.printStackTrace();
            return "Unavailable";
        } catch (IOException e) {
            e.printStackTrace();
            return "Unavailable";
        }
    }

    /**
     * 获取系统可用内存信息
     *
     * @return
     */
    public String getAvailMemory() {
        if(appContext == null || appActivityManager == null)
        {
            return "Unavailable";
        }
        ActivityManager.MemoryInfo mi = new ActivityManager.MemoryInfo();
        appActivityManager.getMemoryInfo(mi);
        // 转化为mb
        return Formatter.formatFileSize(appContext, mi.availMem);
    }

    /**
     * 获取系统总内存信息 - 方法2
     *
     * @return
     */
    public String getTotalMemory2() {
        if(appContext == null || appActivityManager == null)
        {
            return "Unavailable";
        }
        ActivityManager.MemoryInfo mi = new ActivityManager.MemoryInfo();
        appActivityManager.getMemoryInfo(mi);
        // 转化为mb
        return Formatter.formatFileSize(appContext, mi.totalMem);
    }
}
