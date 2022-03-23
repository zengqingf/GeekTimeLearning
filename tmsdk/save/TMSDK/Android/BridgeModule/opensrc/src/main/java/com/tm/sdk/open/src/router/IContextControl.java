package com.tm.sdk.open.src.router;

import android.content.Context;

import com.alibaba.android.arouter.facade.template.IProvider;

public interface IContextControl extends IProvider {
    void setCurrentContext(Context context);
    Context getCurrentContext();
    void exit();
}
