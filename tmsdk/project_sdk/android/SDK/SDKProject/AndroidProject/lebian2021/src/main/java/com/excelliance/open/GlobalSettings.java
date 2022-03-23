package com.excelliance.open;

import com.excelliance.lbsdk.LBConfig;

public class GlobalSettings {
    public static int SELECTED_AREA = LBConfig.AREA_CN;//Changed!!!      //LBConfig.AREA_CN（大陆）;LBConfig.AREA_OVS（海外）;LBConfig.AREA_TW（台湾，仅限游戏使用）;
    public static boolean USE_LEBIAN = true;
    public static boolean USE_REGENG = true;// 使用热更功能设置为true；不使用热更功能设置为false
    public static boolean DOWNLOAD_AFTER_QUIT = true;
    public static boolean AUTO_CHECK_NEWVER_ONSTART = true;
    public static boolean ENABLE_CRASH_REPORT = true;
    public static int NEWV_LOCATION = LBConfig.LOCATION_AUTO;
    public static boolean ENABLE_EXIT_BUTTON = false;

    public static boolean USE_BWBX = true;//Changed!!!     // 分包设置为true；不使用分包设置为false
    public static boolean HANDLE_ASSETMANAGER_BWBX = false;
    public static boolean SHOW_LOADING_PROGRESS_BWBX = true;
    public static boolean SHOW_LOADING_PROGRESS_BWBX_IMMEDIATELY = false;
    public static boolean SHOW_FIRST_DIALOG_WITHOUT_WIFI_BWBX = false;//Changed!!!  关闭后流量网络进游戏就不会提示玩家下载了
    public static boolean SHOW_FIRST_DIALOG_ALWAYS_BWBX = false;
    public static boolean CHECK_OLD_USER_AUTO = true;
    public static boolean CHOOSE_BY_USER_BWBX = false;//二次加载开关，启动游戏立即二次加载
    public static boolean TWLOAD_API_CONTROL = false;//通过调用接口控制二次加载时机，打开此开关后如果是二次加载完整资源则CHOOSE_BY_USER_BWBX也需要打开，如果是二次加载+边玩边下则不需要打开CHOOSE_BY_USER_BWBX，并且需要主动调用接口LebianSdk.twiceLoad
    public static boolean CHOOSE_BY_USER_BWBX_FORCE = false;
    public static boolean DOWNLOAD_FULL_AFTER_SECOND_DIALOG_BWBX = false;
    public static boolean SHOW_DIALOG_NETWORK_WEAK_BWBX = true;//当网络差的情况是否显示二次加载的弹窗，默认显示
    public static boolean USE_HIDDEN_DIR_BY_DEFAULT = false;//默认情况下使用隐藏目录
    public static boolean SHOW_DIALOG_WITH_WIFI_WHEN_TWLOAD_BWBX = false;//wifi情况下是否显示流量下的第一个弹窗，默认不显示
    public static int SECOND_DIALOG_INTERVAL_BWBX = 0;
    public static boolean SHOW_DIALOG_BUTTON_BY_OLD_USER = false;//老用户弹窗类型选择开关，默认为false，弹窗按钮显示为“是”和“否”
    public static boolean CHOOSE_BY_USER_BWBX_LATER = false;//二次加载时，是否现在后台下载
    public static boolean SHOW_DIALOG_WITHOUT_WIFI_WHEN_LBRES_LATER = true;//先玩再下二次加载时，流量情况下是否提示，默认提示
    public static boolean CHOOSE_RESTART_GAME_AFTER_DECOMPRESS = true;//二次加载中重启游戏是否放在解压资源之后，默认否
    public static boolean USE_ANIMAL_WHEN_SWITCH_VIEW = true;//多张背景图轮播时是否开启淡入淡出动画，默认开启
    public static boolean SHOW_DIALOG_RESPATCH_IN_WIFI = false;//wifi情况下载资源差分包是否提示，默认false不提示
    public static boolean SHOW_PROGRESS_IN_DOWNLOADING_ANIMATION = false;//咖啡杯动画显示下载进度，默认false不提示
    /**
     下载界面的图片采用的缩放模式(默认参数为零)
     0：等比平铺屏幕，多余部分会被剪切
     1：等比缩放，不足的部分会用黑色填充
     2：平铺屏幕，非等比缩放，图片会发生变形
     */
    public static int SCALE_MODE_IN_NEXTCHAPTER = 0;

    public static int USE_HTTP_OR_HTTPS = LBConfig.HTTP;//使用http请求还是https请求，默认情况下使用http请求

    public static void refreshState() {
        //USE_LEBIAN = !(new java.io.File("/sdcard/disable_LB_flag").exists());
        android.util.Log.d("GlobalSettings", "USE_LEBIAN=" + USE_LEBIAN);
    }
}

