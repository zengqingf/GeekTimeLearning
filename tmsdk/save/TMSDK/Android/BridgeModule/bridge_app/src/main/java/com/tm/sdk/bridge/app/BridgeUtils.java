package com.tm.sdk.bridge.app;

import com.tm.sdk.bridge.call.ICallback;
import com.tm.sdk.commonlib.output.LoggerWrapper;

import java.util.ArrayList;
import java.util.List;

public final class BridgeUtils {

    private static IBridgeApp mBridgeApp = null;

    private static final List<PkgApp> pkgApps = new ArrayList<>();

    static {
        pkgApps.add(new PkgApp("com.epicgames.ue4.GameActivity", BridgeUnrealApp.class));
        pkgApps.add(new PkgApp("com.unity3d.player.UnityPlayer", BridgeUnityApp.class));
    }

    public static void Init(ICallback callback) {
        //LoggerWrapper.getInstance().logInfo("Call Init");
        for(PkgApp pkgApp : pkgApps) {
            mBridgeApp = pkgApp.tryCreate();
            if(mBridgeApp != null) {
                if(callback != null) {
                    mBridgeApp.SetCallback(callback);
                }
                //LoggerWrapper.getInstance().logInfo("Call Init : %s", pkgApp.pkgName);
                break;
            }
        }
    }

    public static String Call(String param) {
        //LoggerWrapper.getInstance().logInfo("Call Param : %s", param);
        if(mBridgeApp == null) {
            return "";
        }
        //LoggerWrapper.getInstance().logInfo("Call App : %s", mBridgeApp.toString());
        return mBridgeApp.OnCall(param);
    }

    private static class PkgApp {
        private String pkgName;
        private Class<? extends IBridgeApp> bridgeApp;

        public PkgApp(String pName, Class<? extends IBridgeApp> bApp) {
            this.pkgName = pName;
            this.bridgeApp = bApp;
        }

        public IBridgeApp tryCreate() {
            try {
                Class<?> clazz = Class.forName(pkgName);
                LoggerWrapper.getInstance().logInfo("Call Create App : %s", clazz == null ? pkgName + " not class" : clazz.toString());
                if(clazz != null){
                    return bridgeApp.newInstance();
                }
            } catch (Exception e) {
                return null;
            }
            return null;
        }
    }
}
