package com.tm.commonlib;

import android.annotation.SuppressLint;
import android.annotation.TargetApi;
import android.app.Activity;
import android.content.Context;
import android.graphics.Rect;
import android.util.DisplayMetrics;
import android.util.Log;


import java.lang.reflect.Field;
import java.lang.reflect.Method;
import java.util.List;

import android.os.*;
import android.util.TypedValue;
import android.view.DisplayCutout;
import android.view.View;
import android.view.WindowManager;


/**
 * Created by tengmu on 2019/7/29.
 */
public class Notch {

    @TargetApi(28)
    public static void initNotch(Activity context){
        if (android.os.Build.VERSION.SDK_INT >= 28){//android P
            WindowManager.LayoutParams lp = context.getWindow().getAttributes();
            lp.layoutInDisplayCutoutMode = WindowManager.LayoutParams.LAYOUT_IN_DISPLAY_CUTOUT_MODE_SHORT_EDGES;
            context.getWindow().setAttributes(lp);
        }


    }

    public static boolean hasNotch(Context context) {
        boolean ret = false;
        if (android.os.Build.VERSION.SDK_INT >= 28){//android P
            return hasNotchAtAPI28(context);
        }
        if (android.os.Build.VERSION.SDK_INT < 24){//android N
            return false;
        }


        return hasNotchAt24_28(context);

    }
    public static boolean hasNotchAt24_28(Context context) {
        boolean ret = false;
        if (android.os.Build.VERSION.SDK_INT < 24){//android N
            return false;
        }

        switch (getDeviceBrand()){
            case "HUAWEI":
                ret = hasNotchAtHuawei(context);
                break;
            case "VIVO":
                ret = hasNotchAtVivo(context);

                break;
            case "OPPO":
                ret = hasNotchAtOPPO(context);

                break;
            case "XIAOMI":
                ret = hasNotchAtXiaoMi(context);
                break;
            case "MEIZU":
                ret = hasNotchAtMeiZu(context);
                break;
            default:
                break;
        }

        return ret;

    }

    public static int[] getNotchsize(Context context) {
        int[] ret = new int[]{0, 0};

        if (!hasNotch(context))
            return ret;

        if (android.os.Build.VERSION.SDK_INT >= 28){//android P
            return getNotchSizeAtAPI28(context);
        }

        if (android.os.Build.VERSION.SDK_INT < 24){//android N
            return ret;
        }

        return getNotchsizeAt24_28(context);


    }
    public static int[] getNotchsizeAt24_28(Context context) {
        int[] ret = new int[]{0, 0};

        if (android.os.Build.VERSION.SDK_INT < 24){//android N
            return ret;
        }
        if (!hasNotch(context))
            return ret;

        switch (getDeviceBrand()){
            case "HUAWEI":
                ret = getNotchSizeAtHuawei(context);
                break;
            case "VIVO":
                ret = getNotchSizeAtVivo(context);

                break;
            case "OPPO":
                ret = getNotchSizeAtOPPO(context);

                break;
            case "XIAOMI":
                ret = getNotchSizeAtXiaoMi(context);

                break;
            case "MEIZU":
                ret = getNotchSizeAtMeiZu(context);
                break;
            default:
                break;
        }

        return new int[]{ret[0],ret[1]};
    }
    @TargetApi(28)
    public static boolean hasNotchAtAPI28(Context context) {
        Activity activity = (Activity)context;
        final View decorView = activity.getWindow().getDecorView();
        if(decorView == null){
            return false;
        }

        DisplayCutout displayCutout = decorView.getRootWindowInsets().getDisplayCutout();

        if (displayCutout == null){
            return  false;
        }
        List<Rect> rects = displayCutout.getBoundingRects();
        if (rects == null || rects.size() == 0) {
            return false;
        } else {
            return true;
        }

    }
    @TargetApi(28)
    public static int[] getNotchSizeAtAPI28(Context context) {
        int[] ret = new int[]{0,0};

        DisplayMetrics dm = context.getResources().getDisplayMetrics();
        int screenWidth = dm.widthPixels;
        int screenHeight = dm.heightPixels;

        Activity activity = (Activity)context;
        final View decorView = activity.getWindow().getDecorView();
        if(decorView == null){
            return ret;
        }
        DisplayCutout displayCutout = decorView.getRootWindowInsets().getDisplayCutout();
        if(displayCutout == null){
            return ret;
        }
        List<Rect> rects = displayCutout.getBoundingRects();
        if (rects == null || rects.size() == 0) {
            return ret;
        } else {
            Log.e("TAG", "刘海屏数量:" + rects.size());
            for (Rect rect : rects) {

                Log.e("TAG", "刘海屏区域：" + rect);
                //opengl原始数据,需要转化到unity 的 ui坐标系，
                return  new int[]{rect.left,rect.top,rect.right,rect.bottom};
                //return new int[]{0,screenHeight/2 - rect.bottom,rect.right - rect.left - screenWidth/2,screenHeight/2 - rect.top};
                //return new int[]{rect.right - rect.left,rect.bottom-rect.top};
            }
        }
        return ret;
    }
    public static boolean hasNotchAtHuawei(Context context) {
        boolean ret = false;
        try {
            ClassLoader classLoader = context.getClassLoader();
            Class HwNotchSizeUtil = classLoader.loadClass("com.huawei.android.util.HwNotchSizeUtil");
            Method get = HwNotchSizeUtil.getMethod("hasNotchInScreen");
            ret = (boolean) get.invoke(HwNotchSizeUtil);
        } catch (ClassNotFoundException e) {
            //Log.e("Notch", "hasNotchAtHuawei ClassNotFoundException");
        } catch (NoSuchMethodException e) {
            //Log.e("Notch", "hasNotchAtHuawei NoSuchMethodException");
        } catch (Exception e) {
            Log.e("Notch", "hasNotchAtHuawei Exception");
        } finally {
            return ret;
        }
    }
    //获取刘海尺寸：width、height
    //int[0]值为刘海宽度 int[1]值为刘海高度
    public static int[] getNotchSizeAtHuawei(Context context) {
        int[] ret = new int[]{0, 0};
        try {
            ClassLoader cl = context.getClassLoader();
            Class HwNotchSizeUtil = cl.loadClass("com.huawei.android.util.HwNotchSizeUtil");
            Method get = HwNotchSizeUtil.getMethod("getNotchSize");
            ret = (int[]) get.invoke(HwNotchSizeUtil);
        } catch (ClassNotFoundException e) {
            Log.e("Notch", "getNotchSizeAtHuawei ClassNotFoundException");
        } catch (NoSuchMethodException e) {
            Log.e("Notch", "getNotchSizeAtHuawei NoSuchMethodException");
        } catch (Exception e) {
            Log.e("Notch", "getNotchSizeAtHuawei Exception");
        } finally {
            return ret;
        }
    }
    public static final int VIVO_NOTCH = 0x00000020;//是否有刘海
    public static final int VIVO_FILLET = 0x00000008;//是否有圆角

    public static boolean hasNotchAtVivo(Context context) {
        boolean ret = false;
        try {
            ClassLoader classLoader = context.getClassLoader();
            Class FtFeature = classLoader.loadClass("android.util.FtFeature");
            Method method = FtFeature.getMethod("isFeatureSupport", int.class);
            ret = (boolean) method.invoke(FtFeature, VIVO_NOTCH);
        } catch (ClassNotFoundException e) {
            Log.e("Notch", "hasNotchAtVivo ClassNotFoundException");
        } catch (NoSuchMethodException e) {
            Log.e("Notch", "hasNotchAtVivo NoSuchMethodException");
        } catch (Exception e) {
            Log.e("Notch", "hasNotchAtVivo Exception");
        } finally {
            return ret;
        }
    }
    public static int[] getNotchSizeAtVivo(Context context) {
        int[] ret = new int[]{dp2px(context,27), dp2px(context,100)};//dp=100，27

        return ret;
    }

    public static boolean hasNotchAtOPPO(Context context) {
        boolean ret = false;
        try {
            ret = context.getPackageManager().hasSystemFeature("com.oppo.feature.screen.heteromorphism");
        }
         catch (Exception e) {
            Log.v("Notch", "hasNotchAtOPPO Exception");
        } finally {
            return ret;
        }
    }
    public static int[] getNotchSizeAtOPPO(Context context) {
        int[] ret = new int[]{80,324};//px=324,80
        return ret;
    }

    public static boolean hasNotchAtXiaoMi(Context context) {
        boolean ret = false;
        try {
            ret = SystemProperties.getInt("ro.miui.notch", 0) == 1;
        }
        catch (Exception e) {
            Log.v("Notch", "hasNotchAtXiaoMi Exception");
        } finally {
            return ret;
        }
    }
    public static int[] getNotchSizeAtXiaoMi(Context context) {
        int[] ret = new int[]{80,324};//px=324,80

        try{
            int resourceId = context.getResources().getIdentifier("notch_width", "dimen", "android");
            if (resourceId > 0) {
                ret[0] = context.getResources().getDimensionPixelSize(resourceId);
            }

            resourceId = context.getResources().getIdentifier("notch_height", "dimen", "android");
            if (resourceId > 0) {
                ret[1] = context.getResources().getDimensionPixelSize(resourceId);
            }
        }
        catch (Exception e){
            ret = new int[]{0,0};
        }finally {
            return ret;
        }
    }
    public static boolean hasNotchAtMeiZu(Context context) {

        boolean ret = false;
        try {
            Class clazz = Class.forName("flyme.config.FlymeFeature");
            Field field = clazz.getDeclaredField("IS_FRINGE_DEVICE");
            ret = (boolean) field.get(null);
        } catch (Exception e) {
            Log.e("Notch", e.toString());
        }finally {
            return  ret;
        }
    }
    public static int[] getNotchSizeAtMeiZu(Context context) {
        int[] ret = new int[]{51,534};

        try {
            int fhid = context.getResources().getIdentifier("fringe_height", "dimen", "android");
            if (fhid > 0) {
                ret[0] = context.getResources().getDimensionPixelSize(fhid);
            }
            int fwid = context.getResources().getIdentifier("fringe_width", "dimen", "android");
            if (fwid > 0) {
                ret[1] = context.getResources().getDimensionPixelSize(fwid);
            }

        }catch (Exception e){
            Log.e("Notch", e.toString());
        }finally {
            return  ret;
        }

    }
    /**
     * dp转px
     * @param context
     * @param dpValue
     * @return
     */
    private static int dp2px(Context context, int dpValue) {
        return (int) TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, dpValue,context.getResources().getDisplayMetrics());
    }

    /**
     * 获取手机厂商
     *
     * @return  手机厂商
     */

//    public final static int DEVICE_BRAND_OPPO = 0x0001;
//    public final static int DEVICE_BRAND_HUAWEI = 0x0002;
//    public final static int DEVICE_BRAND_VIVO = 0x0003;


    public static String getDeviceBrand() {
        String brand = android.os.Build.BRAND.trim().toUpperCase();
//        if (brand.contains("HUAWEI")) {
//            Log.d("device brand", "HUAWEI");
//            return DEVICE_BRAND_HUAWEI;
//        }else if (brand.contains("OPPO")) {
//            Log.d("device brand", "OPPO");
//            return DEVICE_BRAND_OPPO;
//        }else if (brand.contains("VIVO")) {
//            Log.d("device brand", "VIVO");
//            return DEVICE_BRAND_VIVO;
//        }
        return brand;
    }





}
