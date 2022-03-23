package com.tm.base;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.KeyEvent;

import com.tm.sdk.open.src.ControlUtil;
import com.tm.sdk.open.src.inter.ILifeCycle;
import com.tm.sdk.open.src.type.LifeCycleType;
import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

public class BaseActivity extends UnityPlayerActivity {

    public UnityPlayer getUnityPlayer() {
        return super.mUnityPlayer;
    }

    public static Activity getCurrentActivity()
    {
        return UnityPlayer.currentActivity;
    }

    private ILifeCycle lifeCycle;
    private ILifeCycle getLifeCycle() {
        if(null == lifeCycle) {
            throw new NullPointerException("lifeCycle is null!");
        }
        return lifeCycle;
    }

    @Override
    protected void onCreate(Bundle bundle) {
        super.onCreate(bundle);
        ControlUtil.setCurrentContext(this);

        BaseApplication baseApplication = (BaseApplication) getApplication();
        if(baseApplication != null) {
            lifeCycle = baseApplication.getLifeCycle();
        }
        getLifeCycle().onLiftCycle(LifeCycleType.OnCreate, this);
    }

    @Override
    protected void onStart() {
        super.onStart();
    }

    @Override
    protected void onResume() {
        super.onResume();
    }

    @Override
    protected void onPause() {
        super.onPause();
    }

    @Override
    protected void onStop() {
        super.onStop();
    }

    @Override
    protected void onRestart() {
        super.onRestart();
        getLifeCycle().onLiftCycle(LifeCycleType.OnRestart, this);
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
    }

    @Override
    protected void onNewIntent(Intent intent) {
        super.onNewIntent(intent);
        getLifeCycle().onLiftCycle(LifeCycleType.OnNewIntent, this);
    }

    @Override
    public void onBackPressed() {
        super.onBackPressed();
        getLifeCycle().onLiftCycle(LifeCycleType.OnBackPressed, this);
    }

    @Override
    public boolean onKeyDown(int i, KeyEvent keyEvent) {
        return super.onKeyDown(i, keyEvent);
    }

    @Override
    public boolean dispatchKeyEvent(KeyEvent event) {
        //拦截返回键
        if (event.getKeyCode() == KeyEvent.KEYCODE_BACK){
            //判断触摸UP事件才会进行返回事件处理
            if (event.getAction() == KeyEvent.ACTION_UP) {
                onBackPressed();
            }
            //只要是返回事件，直接返回true，表示消费掉
            return true;
        }
        return super.dispatchKeyEvent(event);
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        //TODO add permission delegate to lifecycleimpl
    }

    @SuppressLint("NewApi")
    @Override
    public void onRequestPermissionsResult(int requestCode,
                                           String[] permissions, int[] grantResults) {
        //TODO add permission delegate to lifecycleimpl
    }
}
