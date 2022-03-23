package com.tm.sdk.ss79.platform;

import android.app.Activity;
import android.app.Application;
import android.content.Context;
import android.content.Intent;
import android.util.Log;
import android.view.MotionEvent;
import android.view.View;


import com.tm.sdk.commonlib.output.LoggerWrapper;
import com.tm.sdk.open.src.ControlUtil;
import com.tm.sdk.open.src.inter.ILifeCycle;
import com.tm.sdk.open.src.type.LifeCycleType;

//SDK
import com.sswl.template.SSWLSdkApi;

public class LifeCycleImpl implements ILifeCycle {

    @Override
    public void onLiftCycle(LifeCycleType type, Context context) {
        switch (type) {
            case OnAppCreate:
                Log.d(Global.LOG_TAG, "on appcreate");
                LoggerWrapper.getInstance().create(Global.LOG_TAG, Global.IS_DEBUG);
                SSWLSdkApi.getInstance().initApplication((Application) context);
                break;
            case OnRestart:
                SSWLSdkApi.getInstance().onRestart((Activity) context) ;
                break;
            default:
                break;
        }
    }

    @Override
    public void onBackPressed() {
        SSWLSdkApi.getInstance().exit((Activity) ControlUtil.getCurrentContext());
    }

    @Override
    public void onNewIntent(Intent intent) {
        SSWLSdkApi.getInstance().onLaunchNewIntent((Activity) ControlUtil.getCurrentContext(), intent);
        SSWLSdkApi.getInstance().onNewIntent((Activity) ControlUtil.getCurrentContext(), intent);
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        SSWLSdkApi.getInstance().onActivityResult((Activity)ControlUtil.getCurrentContext(),requestCode, resultCode, data);
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults) {
        SSWLSdkApi.getInstance().onRequestPermissionsResult((Activity)ControlUtil.getCurrentContext(),requestCode,permissions,grantResults);
    }

    @Override
    public boolean onTouchEvent(View view, MotionEvent motionEvent) {
        return false;
    }
}
