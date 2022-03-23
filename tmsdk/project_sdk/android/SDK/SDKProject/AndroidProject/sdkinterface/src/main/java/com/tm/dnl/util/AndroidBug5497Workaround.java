package com.tm.dnl.util;

import android.app.Activity;
import android.graphics.Rect;
import android.view.View;
import android.view.ViewTreeObserver;
import android.view.inputmethod.InputMethodManager;
import android.widget.FrameLayout;

/**
 * Android Plugin for Unity
 * Created by Administrator on 2017/9/15.
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

    private static void SetLogByUniWebView2(String msg)
    {
        //Log.e("TAG",msg);
    }

    /*
    * 在你的Activity的oncreate()方法里调用AndroidBug5497Workaround.assistActivity(this);即可。注意：在setContentView(R.layout.xxx)之后调用。
    * */
    public static void assistActivity (Activity activity) {
        new AndroidBug5497Workaround(activity);

    }

    private View mChildOfContent;
    private int usableHeightPrevious;
    private FrameLayout.LayoutParams frameLayoutParams;
    private InputMethodManager inputMethodManager;

    private AndroidBug5497Workaround(Activity activity) {
        inputMethodManager =  (InputMethodManager)activity.getSystemService(activity.INPUT_METHOD_SERVICE);

        FrameLayout content = (FrameLayout) activity.findViewById(android.R.id.content);
        if(content!=null) {
            mChildOfContent = content.getChildAt(0);
            if(mChildOfContent == null)
                return;
            mChildOfContent.getViewTreeObserver().addOnGlobalLayoutListener(new ViewTreeObserver.OnGlobalLayoutListener() {
                public void onGlobalLayout() {
                    possiblyResizeChildOfContent();

                    SetLogByUniWebView("addOnGlobalLayoutListener calling.....");

                }
            });

        }
        frameLayoutParams = (FrameLayout.LayoutParams) mChildOfContent.getLayoutParams();
    }
    boolean isKeyBoardShow = false;
    //boolean isNotificationShow = false;
    int previous = 0;
    private void possiblyResizeChildOfContent() {
        int usableHeightNow = computeUsableHeight();

        SetLogByUniWebView("possiblyResizeChildOfContent . usableHeightNow = " + usableHeightNow);
        if (usableHeightNow != this.usableHeightPrevious) {
            int usableHeightSansKeyboard = this.mChildOfContent.getRootView().getHeight();
            int heightDifference = usableHeightSansKeyboard - usableHeightNow;
            SetLogByUniWebView("usableHeightSansKeyboard = " + usableHeightSansKeyboard);
            SetLogByUniWebView("keyboard probably just became visible , heightDifference =  " + heightDifference);

            if (heightDifference * 4 > usableHeightSansKeyboard && !this.isKeyBoardShow) {
                SetLogByUniWebView("possiblyResizeChildOfContent . usableHeightPrevious = " + this.usableHeightPrevious);
                this.usableHeightPrevious = usableHeightNow;
                this.isKeyBoardShow = true;
                UnitySendMsg.SendMsgToUnity("OnKeyBoardShowOn", String.format("%d",previous - usableHeightNow));

            } else if ((usableHeightNow - this.usableHeightPrevious) * 4 > usableHeightSansKeyboard && this.isKeyBoardShow) {
                SetLogByUniWebView("possiblyResizeChildOfContent . usableHeightPrevious = " + this.usableHeightPrevious);
                this.usableHeightPrevious = usableHeightNow;
                this.isKeyBoardShow = false;
                UnitySendMsg.SendMsgToUnity("OnKeyBoardShowOn", "0");
            }
        }

        previous = usableHeightNow;
    }

    private int computeUsableHeight() {
        Rect r = new Rect();
        mChildOfContent.getWindowVisibleDisplayFrame(r);
        SetLogByUniWebView("全屏模式下： return r.bottom = "+(r.bottom)+",  r.top = "+r.top);
        return (r.bottom - r.top);// 全屏模式下： return r.bottom
    }

    private void SetLogByUniWebView(String msg)
    {
        Logger.LogDebug(msg);
    }



    //        int usableHeightNow = computeUsableHeight();
//        int delta = usableHeightNow - previous;
//        previous = usableHeightNow;
//        SetLogByUniWebView("delta = "+delta);
//        int rootViewHeight = mChildOfContent.getRootView().getHeight();
//        SetLogByUniWebView("RootViewHeight  "+rootViewHeight);
//        SetLogByUniWebView("possiblyResizeChildOfContent . usableHeightNow = "+usableHeightNow);
//
//        if (delta != 0 && delta != rootViewHeight && delta != -rootViewHeight) {
//
//            if (delta * 4 < -rootViewHeight && !isKeyBoardShow) {
//                // keyboard probably just became visible
//                //frameLayoutParams.height = rootViewHeight - heightDifference;
//                //mChildOfContent.requestLayout();
//                isKeyBoardShow = true;
//                UnitySendMsg.SendMsgToUnity(UnitySendMsg.SDKCALLBACK_KEYBOARD,"1");
//            } else if (delta * 4 > rootViewHeight && isKeyBoardShow){
//                // keyboard probably just became hidden
//                //frameLayoutParams.height = rootViewHeight + heightDifference;
//                //mChildOfContent.requestLayout();
//                isKeyBoardShow = false;
//                UnitySendMsg.SendMsgToUnity(UnitySendMsg.SDKCALLBACK_KEYBOARD,"0");
//            }
//
////            if (delta < 150 && delta > 0 && isNotificationShow == true)
////            {
////                SetLogByUniWebView("通知消失"+frameLayoutParams.height);
////                isNotificationShow = false;
////                //frameLayoutParams.height -= delta;
////                SetLogByUniWebView("通知消失"+frameLayoutParams.height);
////            }
////            if ( delta > -150 && delta < 0 && isNotificationShow == false)
////            {
////                SetLogByUniWebView("通知出现"+frameLayoutParams.height);
////
////                isNotificationShow = true;
////                //frameLayoutParams.height = rootViewHeight - delta;
////                SetLogByUniWebView("通知出现"+frameLayoutParams.height);
////            }
//        }
}
