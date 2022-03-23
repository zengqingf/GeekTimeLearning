package com.tm.sdk.mg.platform;

import android.app.Application;
import android.content.Context;
import android.content.Intent;
import android.os.Handler;
import android.os.Message;
import android.util.Log;
import android.view.MotionEvent;
import android.view.View;

import androidx.annotation.NonNull;

import com.tm.sdk.commonlib.output.LoggerWrapper;
import com.tm.sdk.open.src.ControlUtil;
import com.tm.sdk.open.src.inter.ICallbackHandler;
import com.tm.sdk.open.src.inter.ILifeCycle;
import com.tm.sdk.open.src.type.LifeCycleType;

//第三方SDK的引用尽量放一起
import com.mgbase.utils.BaseAnchorImpl;
import com.mgpay.utils.MGPayAnchor;
import com.mgpay.utils.PreferenceUtils;
import com.tm.sdk.platform.CallbackMethodType;
import com.xy.xypay.utils.XYPaySDK;

public class LifeCycleImpl implements ILifeCycle {

    private static final int CALL_QUIT = 1;//退出游戏
    private static final int CALL_RESUME = 2;//返回按键处理
    private static final int CALL_AUTH_NAME = 3;//实名
    Handler mgHandler;

    @Override
    public void onLiftCycle(LifeCycleType type, Context context) {
        switch (type) {
            case OnAppCreate:
                Log.d(Global.LOG_TAG, "on appcreate");
                LoggerWrapper.getInstance().create(Global.LOG_TAG, Global.IS_DEBUG);

                //notice : this is error ,  packagename is not used online
                //XYPaySDK.init((Application) context, new BaseAnchorImpl(), new MGPayAnchor());
                break;
            case OnCreate:
                //notice : this is error ,  packagename is not used online
                //_onCreate(context);
                break;
            default:
                break;
        }
    }

    private void _onCreate(Context context) {
        mgHandler = new Handler(new Handler.Callback() {
            @Override
            public boolean handleMessage(@NonNull Message message) {

                switch(message.what) {
                    case CALL_QUIT:
                        PreferenceUtils.setUid(context, "");
                        XYPaySDK.hideFloatWindow(context);
                        ICallbackHandler handler = ControlUtil.createCallHandler(CallbackMethodType.OnLogout.value);
                        ControlUtil.doCall(handler);
                        break;
                    case CALL_RESUME:
                        XYPaySDK.showFloatWindow(context);
                        break;
                    case CALL_AUTH_NAME:
                        //TODO 添加实名认证接口
                        break;
                }
                return false;
            }
        });
        XYPaySDK.setHandler(mgHandler);
    }

    @Override
    public void onBackPressed() {

    }

    @Override
    public void onNewIntent(Intent intent) {

    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {

    }

    @Override
    public void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults) {

    }

    @Override
    public boolean onTouchEvent(View view, MotionEvent motionEvent) {
        return false;
    }
}
