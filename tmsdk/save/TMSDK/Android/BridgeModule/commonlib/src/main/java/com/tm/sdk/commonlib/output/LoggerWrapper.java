package com.tm.sdk.commonlib.output;

import android.content.Context;
import android.text.TextUtils;
import android.widget.Toast;

import androidx.annotation.Nullable;

import com.orhanobut.logger.AndroidLogAdapter;
import com.orhanobut.logger.Logger;
import com.orhanobut.logger.PrettyFormatStrategy;
import com.tm.sdk.commonlib.BuildConfig;

import java.util.ArrayList;
import java.util.List;


/*
* TODO 目前不支持运行时修改 loggable
* */
public class LoggerWrapper {

    private static final String LOG_TAG = "DEFAULT_TAG";               //调用前进行初始化

    private static class SingletonHolder {
        private static final LoggerWrapper instance = new LoggerWrapper();
    }
    public static LoggerWrapper getInstance() {
        return SingletonHolder.instance;
    }

    private PrettyFormatStrategy formatStrategy;
    private String logTag;
    private boolean isLoggable;
    private List<String> logTags;

    private LoggerWrapper() {
        logTags = new ArrayList<>();
        isLoggable = BuildConfig.IS_DEBUG;
        logTag = LOG_TAG;
    }

    public void clear() {
        Logger.clearLogAdapters();
        logTag = "";
        isLoggable = false;
        if(logTags != null) {
            logTags.clear();
        }
        formatStrategy = null;
    }

    public void create(String tag, boolean loggable) {
        if(logTags.contains(tag)) {
            return;
        }
        String _tag = logTag == null || logTag.length() <= 0 ? LOG_TAG : logTag;
        logTags.add(_tag);
        formatStrategy = PrettyFormatStrategy.newBuilder()
                .tag(_tag)
                .build();
        Logger.addLogAdapter(new AndroidLogAdapter(formatStrategy) {
            @Override
            public boolean isLoggable(int priority, @Nullable String tag) {
                //TODO 这里还能根据 priority 判断是否需要显示哪些level的log
                if(TextUtils.isEmpty(tag)) {
                    return isLoggable;
                }
                return isLoggable && tag.equals(_tag);
            }
        });
    }

    public void logDebug(String msg, Object...args)
    {
        Logger.d(msg, args);
    }

    public void logDebug(Object object)
    {
        Logger.d(object);
    }

    public void logAssert(String msg, Object...args)
    {
        Logger.wtf(msg, args);
    }

    public void logError(String msg, Object...args)
    {
        Logger.e(msg, args);
    }

    public void logError(Throwable throwable, String msg, Object...args)
    {
        Logger.e(throwable, msg, args);
    }

    public void logWarning(String msg, Object...args)
    {
        Logger.w(msg, args);
    }

    public void logInfo(String msg, Object...args)
    {
        Logger.i(msg, args);
    }

    public void logVerbose(String msg, Object...args)
    {
        Logger.v(msg, args);
    }

    public void logToastDebug(Context context, String toast, boolean isShortToast)
    {
        if(isLoggable) {
            Logger.d(toast);
            int toastType = Toast.LENGTH_LONG;
            if(isShortToast)
            {
                toastType = Toast.LENGTH_SHORT;
            }
            Toast.makeText(context, toast, toastType).show();
        }
    }

    public void logToast(Context context, String toast, boolean isShortToast)
    {
        Logger.d(toast);
        int toastType = Toast.LENGTH_LONG;
        if(isShortToast)
        {
            toastType = Toast.LENGTH_SHORT;
        }
        Toast.makeText(context, toast, toastType).show();
    }
}
