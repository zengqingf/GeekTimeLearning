package com.tm.sdk.open.src.inter;

import android.content.Context;
import android.content.Intent;
import android.view.MotionEvent;
import android.view.View;

import com.tm.sdk.open.src.type.LifeCycleType;

public interface ILifeCycle {
    void onLiftCycle(LifeCycleType type, Context context);

    //extra add
    void onBackPressed();
    void onNewIntent(Intent intent);
    void onActivityResult(int requestCode, int resultCode, Intent data);
    void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults);

    boolean onTouchEvent(View view, MotionEvent motionEvent);
}
