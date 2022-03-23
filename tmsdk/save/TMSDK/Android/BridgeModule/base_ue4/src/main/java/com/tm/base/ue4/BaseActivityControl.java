package com.tm.base.ue4;

import android.content.Context;
import android.view.MotionEvent;
import android.view.View;

import com.tm.sdk.open.src.ControlUtil;
import com.tm.sdk.open.src.inter.ILifeCycle;
import com.tm.sdk.open.src.type.LifeCycleType;

public final class BaseActivityControl {
    public static void onCreate(Context context) {
        ControlUtil.setCurrentContext(context);
        ILifeCycle lifeCycle = BaseApplicationControl.getLifeCycle();
        if(lifeCycle != null) {
            lifeCycle.onLiftCycle(LifeCycleType.OnCreate, context);
        }
    }

    public static boolean onTouch(View view, MotionEvent motionEvent) {
        ILifeCycle lifeCycle = BaseApplicationControl.getLifeCycle();
        if(lifeCycle != null) {
            return lifeCycle.onTouchEvent(view, motionEvent);
        }

        /**
         *  注意返回值
         *  true：view继续响应Touch操作；
         *  false：view不再响应Touch操作，故此处若为false，只能显示起始位置，不能显示实时位置和结束位置
         */
        return false;
    }
}
