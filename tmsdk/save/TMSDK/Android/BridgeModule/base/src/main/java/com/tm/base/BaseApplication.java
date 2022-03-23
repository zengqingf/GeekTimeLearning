package com.tm.base;

import android.app.Application;
import android.text.TextUtils;
import android.util.Log;

import com.alibaba.android.arouter.launcher.ARouter;
import com.alibaba.fastjson.JSONArray;
import com.alibaba.fastjson.JSONObject;
import com.tm.sdk.commonlib.output.LoggerWrapper;
import com.tm.sdk.commonlib.util.FileUtils;
import com.tm.sdk.open.src.ControlUtil;
import com.tm.sdk.open.src.inter.ILifeCycle;
import com.tm.sdk.open.src.inter.IService;
import com.tm.sdk.open.src.type.LifeCycleType;

public class BaseApplication extends Application {

    private final String SERVICE_LIST_FILE = "service_list";
    private final String SERVICE_PACKAGE_NAME_HEAD = "com.tm.sdk";

    private ILifeCycle lifeCycle;

    @Override
    public void onCreate() {
        super.onCreate();

        // 这两行必须写在init之前，否则这些配置在init过程中将无效
        if (BuildConfig.DEBUG) {
            // 打印日志
            ARouter.openLog();
            // 开启调试模式(如果在InstantRun模式下运行，必须开启调试模式！线上版本需要关闭,否则有安全风险)
            ARouter.openDebug();
        }
        Log.d("Base","### try init Arouter");
        try {
            // 尽可能早，推荐在Application中初始化
            ARouter.init(this);
        }catch (Exception e)
        {
            e.printStackTrace();
        }
        Log.d("Base","### after init Arouter");
        ControlUtil.setCurrentContext(this);
        _initLifeCycle();
    }

    private void _initLifeCycle() {
        try {
            String serviceClazzName = String.format("%s.platform.%s", getPackageName(), "ServiceImpl");
            Class<?> serviceCls = Class.forName(serviceClazzName);
            Log.d("Base", String.format("### create IService, serviceCls is %s", serviceCls != null ? serviceCls.toString() : "null"));
            ControlUtil.create((Class<? extends IService>) serviceCls);

            String clazzName = String.format("%s.platform.%s", getPackageName(), "LifeCycleImpl");
            Class<?> cls = Class.forName(clazzName);
            lifeCycle = (ILifeCycle)cls.newInstance();
            Log.d("Base", String.format("life cycle is %s", lifeCycle != null ? clazzName : "null"));
            lifeCycle.onLiftCycle(LifeCycleType.OnAppCreate, this);

            //init from service_list
            _initExtendServices();

        } catch (ClassNotFoundException e) {
            e.printStackTrace();
        } catch (IllegalAccessException e) {
            e.printStackTrace();
        } catch (InstantiationException e) {
            e.printStackTrace();
        }
    }

    public ILifeCycle getLifeCycle() {
        return lifeCycle;
    }



    private void _initExtendServices() {
        String fileContent = FileUtils.readFileInAssets(SERVICE_LIST_FILE, this);
        Log.d("Base", String.format("read service_list in assets,  %s", fileContent));
        if(TextUtils.isEmpty(fileContent)) {
            return;
        }
        JSONObject jObj = JSONObject.parseObject(fileContent);
        if(jObj == null) {
            return;
        }
        JSONArray jArray = jObj.getJSONArray("service_list");
        if(jArray == null || jArray.size() <= 0) {
            return;
        }
        for(int i = 0; i < jArray.size(); i++) {
            JSONObject obj = (JSONObject) jArray.get(i);
            if(obj == null) {
                continue;
            }
            Boolean enabled = (Boolean) obj.get("enabled");
            if(!enabled.booleanValue()) {
                continue;
            }
            String name = (String)obj.get("name");
            if(TextUtils.isEmpty(name)) {
                continue;
            }
            String service = (String)obj.get("service");
            if(TextUtils.isEmpty(service)) {
                continue;
            }
            _createServiceClass(String.format("%s.%s", name, service));
        }
    }

    private void _createServiceClass(String serviceClazzName) {
        String clazzName = String.format("%s.%s", SERVICE_PACKAGE_NAME_HEAD, serviceClazzName);
        try {
            Class<?> serviceCls = Class.forName(clazzName);
            Log.d("Base", String.format("### create IService, serviceCls is %s", serviceCls != null ? serviceCls.toString() : "null"));
            ControlUtil.create((Class<? extends IService>) serviceCls);
        }catch (Exception e){
            e.printStackTrace();
        }
    }
}
