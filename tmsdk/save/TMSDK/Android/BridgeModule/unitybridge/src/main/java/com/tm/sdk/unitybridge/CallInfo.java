package com.tm.sdk.unitybridge;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;

import com.alibaba.fastjson.JSONObject;

import java.util.ArrayList;
import java.util.List;

public class CallInfo implements ICallInfo {

    private String name;
    //private JSONObject args = new JSONObject();
    private List<CallArg> args;
    private boolean isCallback = false;
    private String callbackId;

    public CallInfo() {
        args = new ArrayList<>();
    }

    CallInfo(@Nullable Builder builder) {
        this();
        if(null != builder) {
            setIsCallback(builder.isCallback)
                    .setCallbackId(builder.callbackId)
                    .setName(builder.name)
                    .setArgs(builder.args);
        }
    }

    public CallInfo setIsCallback(boolean isCallback) {
        this.isCallback = isCallback;
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

    /*
    public CallInfo setArgs(@Nullable JSONObject params) {
        if(null != params) {
            //this.args.putAll(params);
        }
        return this;
    }

    public CallInfo addArgs(@NonNull String key, @Nullable Object value) {
        //this.args.put(key, value);
        return this;
    }*/

    public CallInfo setArgs(List<CallArg> args) {
        this.args = args;
        return this;
    }

    public <T> CallInfo addArgs(String key, T value) {
        CallArg arg = new CallArg(key, value);
        args.add(arg);
        return this;
    }

    public CallInfo clearArgs() {
        this.args.clear();
        return this;
    }

    public CallInfo clear() {
        name = "";
        isCallback = false;
        callbackId = "";

        clearArgs();
        return this;
    }

    @NonNull
    @Override
    public String getName() {
        return name;
    }

    /*
    @Nullable
    @Override
    public JSONObject getArgs() {
        return args;
    }*/

    @Nullable
    @Override
    public List<CallArg> getArgs() {
        return args;
    }

    @Override
    public boolean isCallback() {
        return isCallback;
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

        public Builder addArgs(@NonNull String key, @Nullable Object value) {
            CallArg arg = new CallArg(key, value);
            this.args.add(arg);
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

        public CallInfo build() {
            return new CallInfo(this);
        }

        public CallInfo build(@Nullable String param) {
            return JSONObject.parseObject(param, CallInfo.class);
        }
    }
}
