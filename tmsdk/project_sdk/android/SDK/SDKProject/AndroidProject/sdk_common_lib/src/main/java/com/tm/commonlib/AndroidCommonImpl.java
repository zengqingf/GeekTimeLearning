package com.tm.commonlib;

import android.annotation.SuppressLint;
import android.annotation.TargetApi;
import android.app.Activity;
import android.app.ActivityManager;
import android.app.Application;
import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.ClipData;
import android.content.ClipDescription;
import android.content.ClipboardManager;
import android.content.ComponentName;
import android.content.ContentResolver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.media.AudioAttributes;
import android.os.BatteryManager;
import android.os.Build;
import android.os.PowerManager;
import android.os.VibrationEffect;
import android.os.Vibrator;
import android.provider.Settings;
import android.telecom.TelecomManager;
import android.telephony.TelephonyManager;
import android.util.Log;
import android.view.Window;
import android.view.WindowManager;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Dictionary;
import java.util.Enumeration;
import java.util.List;


/**
 * Android Plugin for Unity
 * Created by mjx on 2017/7/20.
 */
public class AndroidCommonImpl {

    private static Context appContext;
    private static Activity appActivity;
    private static ContentResolver appContentResolver;
    private static Window appWindow;
    private static TelephonyManager telephonyManager;
    private static GetMemory getMemory;

    public static void SetCommonContext(Object object)
    {
        appContext = ((Activity)object).getApplicationContext();
        appActivity = (Activity)object;
        appContentResolver = ((Activity)object).getContentResolver();
        appWindow = ((Activity)object).getWindow();

        getMemory = new GetMemory(appContext, appActivity);
    }

    public static boolean IsBatteryCharging()
    {
       if(appContext==null) {
          // Log.v("commonjar","context is null!!!!!!!!!!!!!!!");
           return false;
       }
        try {
            IntentFilter intentFilter = new IntentFilter(Intent.ACTION_BATTERY_CHANGED);
            Intent batteryStatus = appContext.registerReceiver(null, intentFilter);
            int status = batteryStatus.getIntExtra(BatteryManager.EXTRA_STATUS, -1);
            return status == BatteryManager.BATTERY_STATUS_CHARGING;
        }catch(Exception e)
        {
            Log.e("common",e.toString());
            return false;
        }
    }

    public static float GetBatteryLevel()
    {
        if(appContext == null)
            return 0;
        try {
            IntentFilter intentFilter = new IntentFilter(Intent.ACTION_BATTERY_CHANGED);
            Intent batteryStatus = appContext.registerReceiver(null, intentFilter);
            int level = batteryStatus.getIntExtra(BatteryManager.EXTRA_LEVEL, -1);
            int scale = batteryStatus.getIntExtra(BatteryManager.EXTRA_SCALE, -1);
            if(scale == 0)
                return 0;
            return  level / (float) scale;
        }catch (Exception e)
        {
            Log.e("common",e.toString());
            return 0;
        }
    }

    private static ClipboardManager clipboardManager = null;
    public static void SetTextTOClipboard(final String text) throws Exception
    {
        if(clipboardManager == null)
            clipboardManager = (ClipboardManager)appActivity.getSystemService(Activity.CLIPBOARD_SERVICE);
        ClipData clipData = ClipData.newPlainText("data",text);
        clipboardManager.setPrimaryClip(clipData);
    }
    public static String GetClipboardText()
    {
        if(appActivity == null)
            return "";
        if(clipboardManager == null)
            clipboardManager = (ClipboardManager)appActivity.getSystemService(Activity.CLIPBOARD_SERVICE);
        if(clipboardManager!=null && clipboardManager.hasPrimaryClip() &&
                clipboardManager.getPrimaryClipDescription().hasMimeType(ClipDescription.MIMETYPE_TEXT_PLAIN))
        {
            ClipData clipData = clipboardManager.getPrimaryClip();
            ClipData.Item item = clipData.getItemAt(0);
            return item.getText().toString();
        }
        return "";
    }

    /*
        获取屏幕亮度
    * */
    public static int GetScreenBrightness()
    {
        if(appContentResolver == null)
            return -1;
        try {
            int screenBrightness = Settings.System.getInt(appContentResolver, Settings.System.SCREEN_BRIGHTNESS);
            return screenBrightness;
        }
        catch(Exception e) {
            return -1;
        }
    }

    /*
    修改屏幕亮度
    * */
    public static void SetScreenBrightness(float value)
    {
        if(appWindow!=null) {
            try {
                WindowManager.LayoutParams mParams = appWindow.getAttributes();
                float res = (value < 0 ? 0 : value) * (1f / 255.0f);
                mParams.screenBrightness = res;
                appWindow.setAttributes(mParams);

                Settings.System.putInt(appContentResolver, Settings.System.SCREEN_BRIGHTNESS, (int) value);
            }catch(Exception e)
            {
                e.printStackTrace();
            }
        }
    }

    /*
        是否开启了自动亮度调节
    * */
    public static boolean IsAutoBrightnessOn()
    {
        boolean automicBrightnessOn = false;
        if(appContentResolver == null)
            return false;
        try
        {
            automicBrightnessOn = Settings.System.getInt(appContentResolver,
                    Settings.System.SCREEN_BRIGHTNESS_MODE)== Settings.System.SCREEN_BRIGHTNESS_MODE_AUTOMATIC;
        }catch(Exception e)
        {
            e.printStackTrace();
        }
        return automicBrightnessOn;
    }

    /*
        开启自动亮度调节
    * */
    public static void StartAutoBrightness()
    {
        if(appContentResolver == null)
            return;
        Settings.System.putInt(appContentResolver,Settings.System.SCREEN_BRIGHTNESS_MODE,
                Settings.System.SCREEN_BRIGHTNESS_MODE_AUTOMATIC);
    }

    /*
        停止自动亮度调节
    * */
    public static void StopAutoBrightness()
    {
        if(appContentResolver == null)
            return;
        Settings.System.putInt(appContentResolver,Settings.System.SCREEN_BRIGHTNESS_MODE,
                Settings.System.SCREEN_BRIGHTNESS_MODE_MANUAL);
    }

    public static void KeepScreenOn(boolean keepOn)
    {
        if(appWindow!=null)
        {
            if(keepOn)
                appWindow.addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
            else
                appWindow.clearFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
        }
    }

    private static TelephonyManager getTelephonyManager()
    {
        if(appContext == null)
            return null;
        if(telephonyManager == null)
        {
            telephonyManager = (TelephonyManager)appContext.getSystemService(Context.TELEPHONY_SERVICE);
        }
        return telephonyManager;
    }

    @TargetApi(26)
    @SuppressLint({"Need new api 26", "MissingPermission"})
    public static String GetSystemIMEI()
    {
        String imei = "";
        try
        {
            if(getTelephonyManager()!=null)
            {
                Log.d("common","current build sdk version = "+Build.VERSION.SDK_INT);
                if(Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) //N = api 26
                {
                    imei = getTelephonyManager().getImei();
                }else {
                    imei = getTelephonyManager().getDeviceId();
                }
            }
        }catch (Exception e)
        {
            Log.e("common",e.getMessage());
        }
        return imei;
    }

    public static String getCurrentProcessMemory()
    {
        if(null == getMemory)
        {
            return "Unavailable";
        }
        return getMemory.getCurrentProcessMemory();
    }

    public static String getAvailMemory()
    {
        if(null == getMemory)
        {
            return "Unavailable";
        }
        return getMemory.getAvailMemory();
    }

    public static boolean isSimulator() {
        return null != appContext && GetSimulatorInfo.isSimulator(appContext);
    }

    public static String getSimulatorName()
    {
        String info = "Unavailable";
        if(null == appContext)
        {
            return info;
        }
        List infos = GetSimulatorInfo.getSimulatorInfo(appContext);
        if(null == infos  || infos.isEmpty())
        {
            return info;
        }
        info = "";
        for (int i = 0; i< infos.size(); i++)
        {
            info += infos.get(i);
        }
        return info;
    }

    
    public static void initNotch(Activity context){
        Notch.initNotch(context);
    }
    public static boolean hasNotch(){
        return Notch.hasNotch(appActivity);
    }
    public static int[] getNotchsize(){
        return Notch.getNotchsize(appActivity);
    }

    public static int getSystemVersion(){
        return android.os.Build.VERSION.SDK_INT;
    }


    /*
       设备震动
       需要添加震动权限
       <uses-permission android:name="android.permission.VIBRATE" />
  * */
    @TargetApi(26)
    public static void setVibrate(long duration, int amplitude)
    {
        if(appContext == null) return;
        Vibrator vibrator = (Vibrator)appContext.getSystemService(Context.VIBRATOR_SERVICE);
        vibrator.cancel();
        if(Build.VERSION.SDK_INT >= 26) { //Build.VERSION_CODES.O
            if(amplitude <= 0 || amplitude > 255)
            {
                amplitude =  VibrationEffect.DEFAULT_AMPLITUDE;
            }
            vibrator.vibrate(VibrationEffect.createOneShot(duration, amplitude));
            Log.d("H5", "setVibrate - 26up | " + duration + " | " + amplitude);
        }else{
            vibrator.vibrate(duration);
            Log.d("H5", "setVibrate - 26down | " + duration);
        }
    }



    /*UniWebView Util
    public static void BindAndroid5497(Object object)
    {
        AndroidBug5497Workaround.assistActivity(object);
    }

    public static void UnBindAndroid5497()
    {
        AndroidBug5497Workaround.clearActivity();
    }
    */
    /*
    private static NotificationManager _notificationManager;
    private static List<Integer> notificationIds = new ArrayList<Integer>();

    public static void SetNotification(int nid,String content,String title,int hour)
    {
        if(appActivity == null)
            return;
        if(notificationIds == null)
            return;
        if(!notificationIds.contains(nid)) {
            notificationIds.add(nid);
        }
        Intent intent = new Intent();
        Log.e("common","mainActivity is running ? = "+isProcessRunning(appContext,"com.example.administrator.myapplication.MainActivity"));
        if(!isProcessRunning(appContext,"com.example.administrator.myapplication.MainActivity")) {
            // test : com.zhihu.android.app.ZhihuApplication
            ComponentName componentName = new ComponentName(appActivity.getPackageName(), "com.example.administrator.myapplication.MainActivity");
            intent.setComponent(componentName);
        }
        PendingIntent pendingIntent = PendingIntent.getActivity(appContext,0,intent,PendingIntent.FLAG_UPDATE_CURRENT);
        long current = System.currentTimeMillis();
        long zero = current/(1000*3600*24)*(1000*3600*24) - java.util.TimeZone.getDefault().getRawOffset();
        long whenTime = zero + hour * 3600 * 1000;
        NotificationCompat.Builder builder = new NotificationCompat.Builder(appContext)
                //必须设置图标 0x7f020002 是unity导出android工程后  R文件中 drawable 中 app_icon 的id
                .setSmallIcon(0x7f020002)
                .setContentTitle(title)
                .setContentText(content)
                .setDefaults(NotificationCompat.DEFAULT_ALL)
                .setAutoCancel(false)//点击通知后，状态栏自动删除通知
                .setContentIntent(pendingIntent)
                .setWhen(whenTime);
        SimpleDateFormat sformat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
        String timeStr = sformat.format(whenTime);
        Log.e("common",timeStr);
       // Notification notification = builder.build();

       // notification.when = zero + hour * 3600 * 1000;
        //notification.when = System.currentTimeMillis();
        _notificationManager = (NotificationManager) appActivity.getSystemService(Context.NOTIFICATION_SERVICE);
        _notificationManager.notify(nid,builder.build());
    }

    public static void RemoveNotification(int nid)
    {
        if(notificationIds ==null)
            return;
        if(_notificationManager == null)
            return;
        if(notificationIds.contains(nid))
        {
            _notificationManager.cancel(nid);
            notificationIds.remove(nid);
        }
        if(notificationIds.size() == 0)
            _notificationManager = null;
    }

    public static void RemoveAllNotification()
    {
        if(notificationIds ==null)
            return;
        if(_notificationManager == null)
            return;
        _notificationManager.cancelAll();
        notificationIds.clear();
        _notificationManager = null;
    }

    public static boolean isProcessRunning(Context context,String processName)
    {
        boolean isRunning = false;
        ActivityManager am = (ActivityManager) context.getSystemService(Context.ACTIVITY_SERVICE);
        List<ActivityManager.RunningAppProcessInfo> lists = am.getRunningAppProcesses();
        for (ActivityManager.RunningAppProcessInfo info : lists)
        {
            if(info.processName.equals(processName)) {
                isRunning = true;
                break;
            }
        }
        return isRunning;
    }
    */
}
