package com.excelliance.open;

import android.content.Context;
import android.util.Log;

import com.excelliance.lbsdk.LBConfig;

import org.xmlpull.v1.XmlPullParser;
import org.xmlpull.v1.XmlPullParserFactory;

import java.io.InputStream;


public class GlobalSettings {
    //注意，所有开关设置全部转移到golbalSettings.xml中，GlobalSettings.java不需要做任何修改
    public static int ENGINE;
    public static int SELECTED_AREA;
    public static boolean USE_LEBIAN;
    public static boolean USE_REGENG;
    public static boolean DOWNLOAD_AFTER_QUIT;
    public static boolean AUTO_CHECK_NEWVER_ONSTART;
    public static boolean ENABLE_CRASH_REPORT;
    public static int NEWV_LOCATION;
    public static boolean ENABLE_EXIT_BUTTON;

    public static boolean USE_BWBX;
    public static boolean HANDLE_ASSETMANAGER_BWBX;
    public static boolean SHOW_LOADING_PROGRESS_BWBX;
    public static int SHOW_LOADING_AFTER_DOWNLOAD_MISSING_TIME;
    public static boolean SHOW_FIRST_DIALOG_WITHOUT_WIFI_BWBX;
    public static boolean SHOW_FIRST_DIALOG_ALWAYS_BWBX;
    public static boolean CHECK_OLD_USER_AUTO;
    public static int OLD_USER_MISSING_CONDITION;
    public static int SHOW_OLD_USER_COUNT;
    public static boolean CHOOSE_BY_USER_BWBX;
    public static boolean TWLOAD_API_CONTROL;
    public static boolean CHOOSE_BY_USER_BWBX_FORCE;
    public static boolean DOWNLOAD_FULL_AFTER_SECOND_DIALOG_BWBX;
    public static boolean SHOW_DIALOG_NETWORK_WEAK_BWBX;
    public static boolean SHOW_DIALOG_WITH_WIFI_WHEN_TWLOAD_BWBX;
    public static int SECOND_DIALOG_INTERVAL_BWBX;
    public static boolean SHOW_DIALOG_BUTTON_BY_OLD_USER;
    public static boolean CHOOSE_BY_USER_BWBX_LATER;
    public static boolean SHOW_DIALOG_WITHOUT_WIFI_WHEN_LBRES_LATER;
    public static boolean USE_ANIMAL_WHEN_SWITCH_VIEW;
    public static boolean SHOW_DIALOG_RESPATCH_IN_WIFI;
    public static boolean SHOW_PROGRESS_IN_DOWNLOADING_ANIMATION;
    public static boolean LOADING_LOCAL_RES;
    public static int MISSING_SIZE_LIMIT;

    /**
     下载界面的图片采用的缩放模式(默认参数为零)
     0：等比平铺屏幕，多余部分会被剪切
     1：等比缩放，不足的部分会用黑色填充
     2：平铺屏幕，非等比缩放，图片会发生变形
     */
    public static int SCALE_MODE_IN_NEXTCHAPTER;

    public static int USE_HTTP_OR_HTTPS;

    public static void refreshState() {
        //USE_LEBIAN = !(new java.io.File("/sdcard/disable_LB_flag").exists());
        android.util.Log.d("GlobalSettings", "USE_LEBIAN=" + USE_LEBIAN);
        if (USE_HTTP_OR_HTTPS == LBConfig.HTTPS_SELF && SELECTED_AREA == LBConfig.AREA_TW) {
            USE_HTTP_OR_HTTPS = 2;
        }
    }

    public static void init(Context context){
        try {
            InputStream is = context.getAssets().open("lebian/globalSettings.xml");

            XmlPullParserFactory xppf = XmlPullParserFactory.newInstance();
            XmlPullParser xpp = xppf.newPullParser();
            xpp.setInput(is, "UTF-8");

            int eventType = xpp.getEventType();
            while (eventType != XmlPullParser.END_DOCUMENT) {
                switch (eventType) {
                    case XmlPullParser.START_DOCUMENT:
                        break;

                    case XmlPullParser.START_TAG:
                        if (xpp.getName().equals("setting")) {
                            String key = xpp.getAttributeValue(null, "key");
                            String value = xpp.getAttributeValue(null, "value");
                            switch (Settings.getSetting(key)) {
                                case engine:
                                    ENGINE = Integer.parseInt(value);
                                    break;
                                case selected_area:
                                    SELECTED_AREA = (value.equalsIgnoreCase("0")  || value.equalsIgnoreCase("1") || value.equalsIgnoreCase("2") || value.equalsIgnoreCase("3")) ? Integer.parseInt(value) : LBConfig.AREA_CN;
                                    break;
                                case use_lebian:
                                    USE_LEBIAN = (value.equalsIgnoreCase("true")  || value.equalsIgnoreCase("false")) ? Boolean.parseBoolean(value) : true;
                                    break;
                                case use_http_or_https:
                                    USE_HTTP_OR_HTTPS = (value.equalsIgnoreCase("0")  || value.equalsIgnoreCase("1")||value.equalsIgnoreCase("2")) ? Integer.parseInt(value) : LBConfig.HTTP;
                                    break;
                                case enable_crash_report:
                                    ENABLE_CRASH_REPORT = (value.equalsIgnoreCase("true")  || value.equalsIgnoreCase("false")) ? Boolean.parseBoolean(value) : true;
                                    break;
                                case use_animal_when_switch_view:
                                    USE_ANIMAL_WHEN_SWITCH_VIEW = (value.equalsIgnoreCase("true")  || value.equalsIgnoreCase("false")) ? Boolean.parseBoolean(value) : true;
                                    break;
                                case scale_mode_in_nextchapter:
                                    SCALE_MODE_IN_NEXTCHAPTER = (value.equalsIgnoreCase("0")  || value.equalsIgnoreCase("1") || value.equalsIgnoreCase("2")) ? Integer.parseInt(value) : 0;
                                    break;
                                case enable_exit_button:
                                    ENABLE_EXIT_BUTTON = (value.equalsIgnoreCase("true")  || value.equalsIgnoreCase("false")) ? Boolean.parseBoolean(value) : false;
                                    break;
                                //热更
                                case use_regeng:
                                    USE_REGENG = (value.equalsIgnoreCase("true")  || value.equalsIgnoreCase("false")) ? Boolean.parseBoolean(value) : true;
                                    break;
                                case download_after_quit:
                                    DOWNLOAD_AFTER_QUIT = (value.equalsIgnoreCase("true")  || value.equalsIgnoreCase("false")) ? Boolean.parseBoolean(value) : true;
                                    break;
                                case auto_check_newver_onstart:
                                    AUTO_CHECK_NEWVER_ONSTART = (value.equalsIgnoreCase("true")  || value.equalsIgnoreCase("false")) ? Boolean.parseBoolean(value) : true;
                                    break;
                                case newv_location:
                                    NEWV_LOCATION = (value.equalsIgnoreCase("4")  || value.equalsIgnoreCase("5") || value.equalsIgnoreCase("6")) ? Integer.parseInt(value) : LBConfig.LOCATION_AUTO;
                                    break;
                                //分包
                                case use_streaming:
                                    USE_BWBX = (value.equalsIgnoreCase("true")  || value.equalsIgnoreCase("false")) ? Boolean.parseBoolean(value) : false;
                                    break;
                                case streaming_handle_assetmanager:
                                    HANDLE_ASSETMANAGER_BWBX = (value.equalsIgnoreCase("true")  || value.equalsIgnoreCase("false")) ? Boolean.parseBoolean(value) : false;
                                    break;
                                case streaming_show_loading_progress:
                                    SHOW_LOADING_PROGRESS_BWBX = (value.equalsIgnoreCase("true")  || value.equalsIgnoreCase("false")) ? Boolean.parseBoolean(value) : true;
                                    break;
                                case streaming_show_loading_after_download_missing_time:
                                    try {
                                        SHOW_LOADING_AFTER_DOWNLOAD_MISSING_TIME = Integer.parseInt(value);
                                    } catch (NumberFormatException e) {
                                        SHOW_LOADING_AFTER_DOWNLOAD_MISSING_TIME = 15;
                                        e.printStackTrace();
                                    }
                                    break;
                                case streaming_show_first_dialog_without_wifi:
                                    SHOW_FIRST_DIALOG_WITHOUT_WIFI_BWBX = (value.equalsIgnoreCase("true")  || value.equalsIgnoreCase("false")) ? Boolean.parseBoolean(value) : true;
                                    break;
                                case streaming_show_first_dialog_always:
                                    SHOW_FIRST_DIALOG_ALWAYS_BWBX = (value.equalsIgnoreCase("true")  || value.equalsIgnoreCase("false")) ? Boolean.parseBoolean(value) : false;
                                    break;
                                case streaming_check_old_user_auto:
                                    CHECK_OLD_USER_AUTO = (value.equalsIgnoreCase("true")  || value.equalsIgnoreCase("false")) ? Boolean.parseBoolean(value) : true;
                                    break;
                                case streaming_old_user_missing_condition:
                                    try {
                                        OLD_USER_MISSING_CONDITION = Integer.parseInt(value);
                                    } catch (NumberFormatException e) {
                                        OLD_USER_MISSING_CONDITION = 20;
                                        e.printStackTrace();
                                    }
                                    break;
                                case streaming_show_old_user_count:
                                    try {
                                        SHOW_OLD_USER_COUNT = Integer.parseInt(value);
                                    } catch (NumberFormatException e) {
                                        SHOW_OLD_USER_COUNT = 3;
                                        e.printStackTrace();
                                    }
                                    break;
                                case streaming_twiceload:
                                    CHOOSE_BY_USER_BWBX = (value.equalsIgnoreCase("true")  || value.equalsIgnoreCase("false")) ? Boolean.parseBoolean(value) : false;
                                    break;
                                case streaming_twload_api_control:
                                    TWLOAD_API_CONTROL = (value.equalsIgnoreCase("true")  || value.equalsIgnoreCase("false")) ? Boolean.parseBoolean(value) : false;
                                    break;
                                case streaming_twiceload_force:
                                    CHOOSE_BY_USER_BWBX_FORCE = (value.equalsIgnoreCase("true")  || value.equalsIgnoreCase("false")) ? Boolean.parseBoolean(value) : false;
                                    break;
                                case streaming_download_full_after_second_dialog:
                                    DOWNLOAD_FULL_AFTER_SECOND_DIALOG_BWBX = (value.equalsIgnoreCase("true")  || value.equalsIgnoreCase("false")) ? Boolean.parseBoolean(value) : false;
                                    break;
                                case streaming_network_weak:
                                    SHOW_DIALOG_NETWORK_WEAK_BWBX = (value.equalsIgnoreCase("true")  || value.equalsIgnoreCase("false")) ? Boolean.parseBoolean(value) : true;
                                    break;
                                case streaming_show_dialog_with_wifi_when_twload:
                                    SHOW_DIALOG_WITH_WIFI_WHEN_TWLOAD_BWBX = (value.equalsIgnoreCase("true")  || value.equalsIgnoreCase("false")) ? Boolean.parseBoolean(value) : false;
                                    break;
                                case streaming_second_dialog_interval:
                                    try {
                                        SECOND_DIALOG_INTERVAL_BWBX = Integer.parseInt(value);
                                    } catch (NumberFormatException e) {
                                        SECOND_DIALOG_INTERVAL_BWBX = 0;
                                        e.printStackTrace();
                                    }
                                    break;
                                case streaming_show_dialog_button_by_old_user:
                                    SHOW_DIALOG_BUTTON_BY_OLD_USER = (value.equalsIgnoreCase("true")  || value.equalsIgnoreCase("false")) ? Boolean.parseBoolean(value) : false;
                                    break;
                                case streaming_twiceload_later:
                                    CHOOSE_BY_USER_BWBX_LATER = (value.equalsIgnoreCase("true")  || value.equalsIgnoreCase("false")) ? Boolean.parseBoolean(value) : false;
                                    break;
                                case streaming_show_dialog_without_wifi_when_twiceload_later:
                                    SHOW_DIALOG_WITHOUT_WIFI_WHEN_LBRES_LATER = (value.equalsIgnoreCase("true")  || value.equalsIgnoreCase("false")) ? Boolean.parseBoolean(value) : true;
                                    break;
                                case streaming_show_dailog_respatch_in_wifi:
                                    SHOW_DIALOG_RESPATCH_IN_WIFI = (value.equalsIgnoreCase("true")  || value.equalsIgnoreCase("false")) ? Boolean.parseBoolean(value) : false;
                                    break;
                                case streaming_show_progress_in_downloading_animation:
                                    SHOW_PROGRESS_IN_DOWNLOADING_ANIMATION = (value.equalsIgnoreCase("true")  || value.equalsIgnoreCase("false")) ? Boolean.parseBoolean(value) : false;
                                    break;
                                case streaming_load_local_res:
                                    LOADING_LOCAL_RES = (value.equalsIgnoreCase("true")  || value.equalsIgnoreCase("false")) ? Boolean.parseBoolean(value) : false;
                                    break;
                                case streaming_missing_size_limit:
                                    try {
                                        MISSING_SIZE_LIMIT = Integer.parseInt(value);
                                    } catch (NumberFormatException e) {
                                        MISSING_SIZE_LIMIT = 5;
                                        e.printStackTrace();
                                    }
                                    break;
                            }
                        }
                        break;

                    case XmlPullParser.END_TAG:
                        break;
                }
                eventType = xpp.next();
            }
        }catch (Exception e){
            e.getStackTrace();
            Log.e("GlobalSettings","parse settings error",e);
        }
    }

    public enum Settings {
        selected_area,use_regeng,use_streaming,streaming_twiceload,download_after_quit,auto_check_newver_onstart,
        newv_location,streaming_handle_assetmanager,streaming_show_loading_progress,streaming_show_loading_after_download_missing_time,
        streaming_show_first_dialog_without_wifi,streaming_show_first_dialog_always,streaming_check_old_user_auto,
        streaming_old_user_missing_condition,streaming_show_old_user_count,streaming_twload_api_control,streaming_twiceload_force,
        streaming_download_full_after_second_dialog,streaming_network_weak,streaming_show_dialog_with_wifi_when_twload,
        streaming_second_dialog_interval,streaming_show_dialog_button_by_old_user,streaming_twiceload_later,
        streaming_show_dialog_without_wifi_when_twiceload_later,streaming_show_dailog_respatch_in_wifi,
        streaming_show_progress_in_downloading_animation,streaming_load_local_res,streaming_missing_size_limit,
        use_lebian,enable_crash_report,use_animal_when_switch_view,scale_mode_in_nextchapter,enable_exit_button,
        use_http_or_https,engine;

        public static Settings getSetting(String setting){
            return valueOf(setting.toLowerCase());
        }
    }
}

