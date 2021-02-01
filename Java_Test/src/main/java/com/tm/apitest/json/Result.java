package com.tm.apitest.json;

import org.json.JSONObject;

public class Result {

    public static final int RESULT_SUCCESS = 0;
    public static final int RESULT_EXCEPTION = -1;

    /**
     * 错误码
     */
    public int code = 0;

    /**
     * 错误信息
     */
    public String message;

    /**
     * 返回结果
     */
    public Object result;

    public Result(int code, String msg)
    {
        this.code = code;
        this.message = msg;
    }

    public JSONObject toJson() {
        JSONObject jo = new JSONObject();
        jo.put("code", code);
        jo.put("message", message);
        jo.put("result", result);

        return jo;
    }
}
