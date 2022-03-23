package com.tm.base.ue4;

import android.app.Activity;
import android.content.Context;

import com.alibaba.android.arouter.facade.annotation.Route;
import com.tm.sdk.open.src.router.IContextControl;
import com.tm.sdk.open.src.router.RouterTable;

@Route(path = RouterTable.PATH_CONTEXT_CONTROL)
public class ContextControl implements IContextControl {
    private Context currentContext;

    @Override
    public void setCurrentContext(Context context) {
        this.currentContext = context;
    }

    @Override
    public Context getCurrentContext() {
        return currentContext;
    }

    @Override
    public void exit() {
        Activity activity = (Activity)currentContext;
        if(null != activity) {
            activity.finish();
        }
        System.exit(0);
    }

    @Override
    public void init(Context context) {
        //this.currentContext = context;
        //注意 只获取一次！！！  不可用
    }
}
