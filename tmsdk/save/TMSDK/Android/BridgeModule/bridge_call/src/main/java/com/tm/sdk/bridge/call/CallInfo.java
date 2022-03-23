package com.tm.sdk.bridge.call;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;

import com.alibaba.fastjson.JSONObject;

import java.util.ArrayList;
import java.util.List;

/* 思路参考：https://juejin.cn/post/6844904165760565261
*
* to json format:
{
    "name": "methodName",
    "callback": false,
    "args": [
        "{\"name\":\"argName1\",\"value\":\"value1\"}",
        "{\"name\":\"argName2\",\"value\":\"value2\"}"
    ],
    "callbackId": "callbackID"
}
*
* */

public class CallInfo implements ICallInfo {

    //指令名，方法名
    //回调情况下：为回调ID
    private String name;
    //参数List
    private List<CallArg> args;
    //是否是回调指令
    //notice : isCallback 在Json parse string后 会变成 callback ！！！
    private boolean callback = false;
    //回调ID  由调用方生成， 被调用方根据回调ID发送回调指令
    //32位随机字符串
    private String callbackId;

    public CallInfo() {
        args = new ArrayList<>();
    }

    CallInfo(@Nullable Builder builder) {
        this();
        if(null != builder) {
            this.name = builder.name;
            this.args = builder.args;
            this.callback = builder.isCallback;
            this.callbackId = builder.callbackId;
        }
    }

    public CallInfo clear() {
        name = "";
        callback = false;
        callbackId = "";
        if(args != null) {
            this.args.clear();
        }
        return this;
    }

/************************************* 注意：Json解析时这个必须加 **********************************/
    public CallInfo setCallback(boolean isCallback) {
        this.callback = isCallback;
        return this;
    }

    public CallInfo setCallbackId(@Nullable String callbackId) {
        this.callbackId = callbackId;
        return this;
    }

    public CallInfo setName(@NonNull String name) {
        this.name = name;
        return this;
    }

    public CallInfo setArgs(List<CallArg> args) {
        if(args == null) {
            return this;
        }
        this.args = args;
        return this;
    }
    /************************************* 注意：Json解析时这个必须加 ******************************/


    @NonNull
    @Override
    public String getName() {
        return name;
    }

    @Nullable
    @Override
    public List<CallArg> getArgs() {
        return args;
    }

    @Override
    public boolean isCallback() {
        return callback;
    }

    @Override
    public String getCallbackId() {
        return callbackId;
    }

    @Override
    public String toString() {
        return JSONObject.toJSONString(this);
    }

    public static class Builder {
        private String name;
        private List<CallArg> args;
        private boolean isCallback = false;
        private String callbackId;

        private Builder(){
            args = new ArrayList<>();
        }
        public static Builder create() {
            return new Builder();
        }

        public Builder setName(@Nullable String name) {
            this.name = name;
            return this;
        }

        public Builder setArgs(List<CallArg> args) {
            if(args == null) {
                return this;
            }
            this.args = args;
            return this;
        }

        public Builder setIsCallback(boolean isCallback) {
            this.isCallback = isCallback;
            return this;
        }

        public Builder setCallbackId(@Nullable String callbackId) {
            this.callbackId = callbackId;
            return this;
        }

        public ICallInfo build() {
            return new CallInfo(this);
        }

        public ICallInfo build(@Nullable String param) {
            return JSONObject.parseObject(param, CallInfo.class);
        }
    }
}