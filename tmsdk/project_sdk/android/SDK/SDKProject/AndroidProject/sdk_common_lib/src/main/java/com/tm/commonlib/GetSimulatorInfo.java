package com.tm.commonlib;

import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.pm.ResolveInfo;
import android.os.Build;
import android.text.TextUtils;

import java.io.File;
import java.util.ArrayList;
import java.util.List;

/**
 * Created by SmileMe on 2020/3/3.
 */
public class GetSimulatorInfo {
    private static final String[] PKG_NAMES = {"com.mumu.launcher",
            "com.ami.duosupdater.ui",
            "com.ami.launchmetro",
            "com.ami.syncduosservices",
            "com.bluestacks.home",
            "com.bluestacks.windowsfilemanager",
            "com.bluestacks.settings",
            "com.bluestacks.bluestackslocationprovider",
            "com.bluestacks.appsettings",
            "com.bluestacks.bstfolder",
            "com.bluestacks.BstCommandProcessor",
            "com.bluestacks.s2p",
            "com.bluestacks.setup",
            "com.bluestacks.appmart",
            "com.kaopu001.tiantianserver",
            "com.kpzs.helpercenter",
            "com.kaopu001.tiantianime",
            "com.android.development_settings",
            "com.android.development",
            "com.android.customlocale2",
            "com.genymotion.superuser",
            "com.genymotion.clipboardproxy",
            "com.uc.xxzs.keyboard",
            "com.uc.xxzs",
            "com.blue.huang17.agent",
            "com.blue.huang17.launcher",
            "com.blue.huang17.ime",
            "com.microvirt.guide",
            "com.microvirt.market",
            "com.microvirt.memuime",
            "cn.itools.vm.launcher",
            "cn.itools.vm.proxy",
            "cn.itools.vm.softkeyboard",
            "cn.itools.avdmarket",
            "com.syd.IME",
            "com.bignox.app.store.hd",
            "com.bignox.launcher",
            "com.bignox.app.phone",
            "com.bignox.app.noxservice",
            "com.android.noxpush",
            "com.haimawan.push",
            "me.haima.helpcenter",
            "com.windroy.launcher",
            "com.windroy.superuser",
            "com.windroy.launcher",
            "com.windroy.ime",
            "com.android.flysilkworm",
            "com.android.emu.inputservice",
            "com.tiantian.ime",
            "com.microvirt.launcher",
            "me.le8.androidassist",
            "com.vphone.helper",
            "com.vphone.launcher",
            "com.duoyi.giftcenter.giftcenter",
            "com.tencent.tinput",                                                   //腾讯手游助手
            "com.fiftyone"                                                           //51安卓
    };

    private static final String[] PATHS = {"/sys/devices/system/cpu/cpu0/cpufreq/scaling_cur_freq", "/system/lib/libc_malloc_debug_qemu.so", "/sys/qemu_trace", "/system/bin/qemu-props", "/dev/socket/qemud", "/dev/qemu_pipe", "/dev/socket/baseband_genyd", "/dev/socket/genyd"};

    private static final String[] FILES = {"/data/data/com.android.flysilkworm", "/data/data/com.bluestacks.filemanager"};

    public static boolean isSimulator(Context paramContext) {
        try {
            List pathList = new ArrayList();
            pathList = getInstalledSimulatorPackages(paramContext);
            if (pathList.size() == 0) {
                for (int i = 0; i < PATHS.length; i++)
                    if (i == 0) {
                        if (new File(PATHS[i]).exists()) continue;
                        pathList.add(Integer.valueOf(i));
                    } else {
                        if (!new File(PATHS[i]).exists()) continue;
                        pathList.add(Integer.valueOf(i));
                    }
            }
            if (pathList.size() == 0) {
                pathList = loadApps(paramContext);
            }
            return (pathList.size() == 0 ? null : pathList.toString()) != null;
        } catch (Exception e) {
            e.printStackTrace();
        }
        return false;
    }

    public static List getSimulatorInfo(Context paramContext) {
        List pathList = new ArrayList();
        List simulatorMaps = new ArrayList();

        try {
            pathList = getInstalledSimulatorPackages(paramContext);
            String brand = getSimulatorBrand(pathList);
            if (TextUtils.isEmpty(brand)) {
                List<String> list = loadApps(paramContext);
                if (list.size() > 0) {
                    simulatorMaps.add(list.get(0));
                }
            } else {
                simulatorMaps.add(brand);
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
        return simulatorMaps;
    }

    private static List getInstalledSimulatorPackages(Context context) {
        ArrayList localArrayList = new ArrayList();
        try {
            for (int i = 0; i < PKG_NAMES.length; i++)
                try {
                    context.getPackageManager().getPackageInfo(PKG_NAMES[i], PackageManager.GET_ACTIVITIES);
                    localArrayList.add(PKG_NAMES[i]);
                } catch (PackageManager.NameNotFoundException localNameNotFoundException) {
                }
            if (localArrayList.size() == 0) {
                for (int i = 0; i < FILES.length; i++) {
                    if (new File(FILES[i]).exists())
                        localArrayList.add(FILES[i]);
                }
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
        return localArrayList;
    }

    public static List loadApps(Context context) {
        Intent intent = new Intent(Intent.ACTION_MAIN, null);
        intent.addCategory(Intent.CATEGORY_LAUNCHER);
        List<String> list = new ArrayList<>();
        List<ResolveInfo> apps = context.getPackageManager().queryIntentActivities(intent, 0);
        //for循环遍历ResolveInfo对象获取包名和类名
        for (int i = 0; i < apps.size(); i++) {
            ResolveInfo info = apps.get(i);
            String packageName = info.activityInfo.packageName;
            CharSequence cls = info.activityInfo.name;
            CharSequence name = info.activityInfo.loadLabel(context.getPackageManager());
            if (!TextUtils.isEmpty(packageName)) {
                if (packageName.contains("bluestacks")) {
                    list.add("蓝叠");
                    return list;
                }
            }
        }
        return list;
    }

    private static String getSimulatorBrand(List<String> list) {
        if (list.size() == 0)
            return "";
        String pkgName = list.get(0);
        if (pkgName.contains("mumu")) {
            return "mumu";
        } else if (pkgName.contains("ami")) {
            return "AMIDuOS";
        } else if (pkgName.contains("bluestacks")) {
            return "蓝叠";
        } else if (pkgName.contains("kaopu001") || pkgName.contains("tiantian")) {
            return "天天";
        } else if (pkgName.contains("kpzs")) {
            return "靠谱助手";
        } else if (pkgName.contains("genymotion")) {
            if (Build.MODEL.contains("iTools")) {
                return "iTools";
            } else if ((Build.MODEL.contains("ChangWan"))) {
                return "畅玩";
            } else {
                return "genymotion";
            }
        } else if (pkgName.contains("uc")) {
            return "uc";
        } else if (pkgName.contains("blue")) {
            return "blue";
        } else if (pkgName.contains("microvirt")) {
            return "逍遥";
        } else if (pkgName.contains("itools")) {
            return "itools";
        } else if (pkgName.contains("syd")) {
            return "手游岛";
        } else if (pkgName.contains("bignox")) {
            return "夜神";
        } else if (pkgName.contains("haimawan")) {
            return "海马玩";
        } else if (pkgName.contains("windroy")) {
            return "windroy";
        } else if (pkgName.contains("flysilkworm")) {
            return "雷电";
        } else if (pkgName.contains("emu")) {
            return "emu";
        } else if (pkgName.contains("le8")) {
            return "le8";
        } else if (pkgName.contains("vphone")) {
            return "vphone";
        } else if (pkgName.contains("duoyi")) {
            return "多益";
        } else if (pkgName.contains("tencent")){
            return "tencent";
        } else if(pkgName.contains("fiftyone")){
            return "51安卓";
        }
        return "";
    }
}
