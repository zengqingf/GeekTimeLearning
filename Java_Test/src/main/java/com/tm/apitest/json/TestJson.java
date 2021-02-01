package com.tm.apitest.json;

import com.alibaba.fastjson.JSON;
import com.alibaba.fastjson.JSONObject;

public class TestJson {

    public static String serializeRes()
    {
        Result res = new Result(0, "sth error!");
        res.result = new Result2(3.14f);
        String text = JSON.toJSONString(res);
        return text;
    }
}
