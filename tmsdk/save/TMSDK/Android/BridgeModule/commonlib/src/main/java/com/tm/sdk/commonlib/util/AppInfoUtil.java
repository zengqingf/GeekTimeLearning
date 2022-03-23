package com.tm.sdk.commonlib.util;

import android.content.Context;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.content.pm.Signature;
import android.os.Build;
import android.os.Bundle;

import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;

public class AppInfoUtil {

    public final static String MD5 = "MD5";
    public final static String SHA1 = "SHA1";
    public final static String SHA256 = "SHA256";

    /*
     * 获取包的全部签名
     * */
    public static Signature[] getSignatures(Context context, String packageName){
        try {
            PackageManager manager = context.getPackageManager();
            if(manager != null) {
                PackageInfo packageInfo = manager.getPackageInfo(packageName, PackageManager.GET_SIGNATURES);

                //https://developer.android.google.cn/reference/android/content/pm/SigningInfo
                if(Build.VERSION.SDK_INT >= Build.VERSION_CODES.P) {
                    packageInfo = manager.getPackageInfo(packageName, PackageManager.GET_SIGNING_CERTIFICATES);
                    if(packageInfo.signingInfo.hasMultipleSigners()) {
                        return packageInfo.signingInfo.getApkContentsSigners();
                    }else {
                        return packageInfo.signingInfo.getSigningCertificateHistory();
                    }
                }

                return packageInfo.signatures;
            }
        }catch (PackageManager.NameNotFoundException e){
            e.printStackTrace();
        }
        return null;
    }

    /*
     * 获取包的第一个签名的字节数组
     * */
    public static byte[] getFirstSignatureBytes(Context context){
        String pkgName = getPackageName(context);
        Signature[] signatures = getSignatures(context, pkgName);
        if(signatures == null || signatures.length <= 0)
        {
            return new byte[0];
        }
        return signatures[0].toByteArray();
    }

    public static String getSignatureString(byte[] bytes, String digestType)
    {
        try {
            if(bytes == null)
                return "";
            MessageDigest md = MessageDigest.getInstance(digestType);
            if(md != null) {
                md.update(bytes);
                byte[] b = md.digest();
                StringBuilder sb = new StringBuilder();

                //转换成16进制
                int i;
                for (byte value : b){
                    i = value;
                    if(i < 0)
                        i += 256;
                    if(i < 16)
                        sb.append("0");
                    sb.append(Integer.toHexString(i));
                }
                /*
                for (byte value : b){
                    sb.append((Integer.toHexString((value & 0xFF) | 0x100)).substring(1, 3));
                }
                */
                return sb.toString();
            }
        } catch (NoSuchAlgorithmException e) {
            e.printStackTrace();
        }
        return "";
    }

    public static String getSignatureMD5(Context context){
        return getSignatureString(getFirstSignatureBytes(context), MD5);
    }

    public static String getSignatureSHA1(Context context){
        return getSignatureString(getFirstSignatureBytes(context), SHA1);
    }

    public static String getSignatureSHA256(Context context){
        return getSignatureString(getFirstSignatureBytes(context), SHA256);
    }

    public static String getPackageName(Context context) {
        try {
            return context.getPackageName();
        }catch (Exception e) {
            e.printStackTrace();
        }
        return "";
    }
}
