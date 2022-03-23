package com.tm.sdk.commonlib.workaround;

import android.app.Activity;
import android.graphics.Rect;
import android.view.View;
import android.view.ViewGroup;
import android.view.ViewTreeObserver;
import android.widget.FrameLayout;
import android.widget.LinearLayout;

/**
 * Android Plugin for Unity
 * Created by Administrator on 2017/9/15.
 *
 * ref :
 * [github - AndroidBug5487Workaround - madebycm](https://github.com/madebycm/AndroidBug5497Workaround/blob/master/AndroidBug5497Workaround.java)
 *
 * 修复的问题：
 * 沉浸式状态栏
 * 华为手机不兼容问题 （不完善，需要按机型适配）
 * https://www.jianshu.com/p/a95a1b84da11
 *
 * 修复的问题：
 * 更新了一个版本 2020 12 10
 * https://issuetracker.google.com/issues/36911528
 */
public class AndroidBug5497Workaround {

    // For more information, see https://code.google.com/p/android/issues/detail?id=5497
    // To use this class, simply invoke assistActivity() on an Activity that already has its content view set.

      /*
   public static AndroidBug5497Workaround temp5497;
    public static void assistActivity (Object object) {
        if(temp5497 == null) {
            Activity activity = (Activity) object;
            if(activity != null) {
                temp5497 = new AndroidBug5497Workaround(activity);
            }
        }
        SetLogByUniWebView2("AndroidBug5497Workaround assistActivity !!!!!");
    }

    public static void clearActivity()
    {
        if(temp5497!=null)
        {
            temp5497 = null;
        }
        SetLogByUniWebView2("AndroidBug5497Workaround clearActivity !!!!!");
    }*/

    /*
    * 在你的Activity的oncreate()方法里调用AndroidBug5497Workaround.assistActivity(this);即可。注意：在setContentView(R.layout.xxx)之后调用。
    * */
    public static void assistActivity (Activity activity) {
        new AndroidBug5497Workaround(activity);
    }

    private View mChildOfContent;
    private int usableHeightPrevious;
    private FrameLayout.LayoutParams frameLayoutParams;

    private LinearLayout linearLayout;
    private ViewGroup.LayoutParams viewLayoutParams;

    private AndroidBug5497Workaround(Activity activity) {
        if(null == activity)
            return;
        FrameLayout content = (FrameLayout) activity.findViewById(android.R.id.content);
        if(content!=null) {
            mChildOfContent = content.getChildAt(0);
            if(mChildOfContent == null)
                return;
            mChildOfContent.getViewTreeObserver().addOnGlobalLayoutListener(new ViewTreeObserver.OnGlobalLayoutListener() {
                public void onGlobalLayout() {
                    possiblyResizeChildOfContent();
                }
            });
        }
        frameLayoutParams = (FrameLayout.LayoutParams) mChildOfContent.getLayoutParams();

        linearLayout = (LinearLayout) activity.findViewById(android.R.id.content);
        viewLayoutParams = linearLayout.getLayoutParams();
    }

    boolean isKeyBoardShow = false;
    private void possiblyResizeChildOfContent() {
        int usableHeightNow = computeUsableHeight();
        if (usableHeightNow != this.usableHeightPrevious) {
            //Activity根View的高度 无keyboard
            int usableHeightSansKeyboard = this.mChildOfContent.getRootView().getHeight();
            //Activity根view的高度 - 当前可视区域
            int heightDifference = usableHeightSansKeyboard - usableHeightNow;
            //当 高度差 大约 1/4 的根View高度  认为键盘显示
            if (heightDifference * 4 > usableHeightSansKeyboard && !this.isKeyBoardShow) {
                this.isKeyBoardShow = true;

                //UnitySendMsg.SendMsgToUnity("OnKeyBoardShowOn", String.format("%d",previous - usableHeightNow));

                frameLayoutParams.height = usableHeightSansKeyboard - heightDifference;

            } else if ((usableHeightNow - this.usableHeightPrevious) * 4 > usableHeightSansKeyboard && this.isKeyBoardShow) {
                this.isKeyBoardShow = false;

                //UnitySendMsg.SendMsgToUnity("OnKeyBoardShowOn", "0");

                frameLayoutParams.height = usableHeightSansKeyboard;
            }

            mChildOfContent.requestLayout();

            viewLayoutParams.height = usableHeightNow;
            linearLayout.setLayoutParams(viewLayoutParams);

            usableHeightPrevious = usableHeightNow;
        }
    }

    /*
    计算 Activity 可视区域
    * */
    private int computeUsableHeight() {
        Rect r = new Rect();
        mChildOfContent.getWindowVisibleDisplayFrame(r);
        return (r.bottom - r.top);// 全屏模式下： return r.bottom
    }
}
