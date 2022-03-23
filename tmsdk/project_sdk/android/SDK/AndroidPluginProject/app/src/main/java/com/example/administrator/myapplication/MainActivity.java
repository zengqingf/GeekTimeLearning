package com.example.administrator.myapplication;

import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;

import com.unity3d.player.UnityPlayerActivity;
import com.unity3d.player.UnityPlayer;
import android.util.Log;
import android.content.Intent;


import com.kingnet.xyplatform.source.bean.XYSDKPayBean;
import com.kingnet.xyplatform.source.app.XYGameSDKStatusCode;
import com.kingnet.xyplatform.source.app.XYSDKGames;
import com.kingnet.xyplatform.source.interfaces.XYSDKCallBackListener;
import com.kingnet.xyplatform.source.interfaces.XYSDKExitAccountListener;
import com.kingnet.xyplatform.source.interfaces.XYSDKExitAppListener;

import android.widget.Toast;

public class MainActivity extends UnityPlayerActivity  {

    public final static  String SDKCALLBACK_GAMEOBJECT_NAME = "Singleton of SDKCallback";
    public final static  String SDKCALLBACK_LOGIN = "OnLogin";
    public final static  String SDKCALLBACK_LOGOUT = "OnLogout";
    public final static  String SDKCALLBACK_ONPAY = "OnPayResult";//0-成功 1-失败 2-取消

    private String savedAPPName;
    private String savedAPPDesc;
    private String savedPrice;
    private String savedExtra;
    private int savedServerID;

    protected void onCreate(Bundle savedInstanceState) {
        // call UnityPlayerActivity.onCreate()
        super.onCreate(savedInstanceState);

        Log.e("test", "onCreate called");
    }
    @Override
    protected void onResume() {
        Log.e("test", "onResume called");
        // TODO Auto-generated method stub
        super.onResume();
        if (XYSDKGames.isReady)
            XYSDKGames.showHoverImageView();


    }

    @Override
    protected void onPause() {
        Log.e("test", "onPause called");
        // TODO Auto-generated method stub
        super.onPause();
        if (XYSDKGames.isReady)
            XYSDKGames.hideHoverImageView();
    }

    @Override
    protected void onDestroy() {
        Log.e("test", "onDestroy called");
        super.onDestroy();
        if (XYSDKGames.isReady)
            XYSDKGames.removeHoverImageView();
    }

    public void onBackPressed()
    {
        // instead of calling UnityPlayerActivity.onBackPressed() we just ignore the back button event
        // super.onBackPressed();
    }

    public void showShortText(String text) {
        Toast.makeText(this, text, Toast.LENGTH_SHORT).show();
    }

    public void InitXY(boolean debug)
    {
        Log.e("test", "InitXY called");
        XYSDKGames.initSDK(getApplicationContext());
        if (debug)
        {
            XYSDKGames.setXYDebugPay();//设置为测试支付环境
        }


        XYSDKGames.XYExitAccount(MainActivity.this, new XYSDKExitAccountListener() {
            @Override
            public void exit(String account, String uidString) {
                showShortText("退出账号");
                UnityPlayer.UnitySendMessage(SDKCALLBACK_GAMEOBJECT_NAME, SDKCALLBACK_LOGOUT, "");
            }
        });

    }

    public void Login()
    {
        Log.e("test", "Login called");

        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                XYSDKGames.XYLogin(MainActivity.this, new XYSDKCallBackListener() {

                    @Override
                    public void callback(int code, String msg) {
                        switch (code) {
                            case XYGameSDKStatusCode.CONNECTION_PARAMS_ERROR:
                                //清除
                                break;
                            case XYGameSDKStatusCode.SUCCESS:

                                showShortText("登录成功");

                                UnityPlayer.UnitySendMessage(SDKCALLBACK_GAMEOBJECT_NAME, SDKCALLBACK_LOGIN, XYSDKGames.getXYUid()+","+XYSDKGames.getXYUserToken());

                                XYSDKGames.addHoverImageView();
                                XYSDKGames.showMIUIDialog(MainActivity.this, "");
                                break;
                            default:
                                break;
                        }
                    }
                } , null);
            }
        });
    }

    public void Pay(String appName, String appDesc, String price, String extra, int serverID)
    {
        savedAPPName = appName;
        savedAPPDesc = appDesc;
        savedPrice = price;
        savedExtra = extra;
        savedServerID = serverID;
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                XYSDKGames.XYPayResult(MainActivity.this, new XYSDKPayBean(savedAPPName,savedAPPDesc, savedPrice, savedExtra, savedServerID), new XYSDKCallBackListener() {

                    @Override
                    public void callback(int code, String msg) {
                        switch (code) {
                            case XYGameSDKStatusCode.PAY_SUCCESS_CODE:
                                UnityPlayer.UnitySendMessage(SDKCALLBACK_GAMEOBJECT_NAME, SDKCALLBACK_ONPAY, "0");
                                Toast.makeText(MainActivity.this, "购买成功", Toast.LENGTH_SHORT).show();
                                break;
                            case XYGameSDKStatusCode.PAY_FAIL_CODE:
                                UnityPlayer.UnitySendMessage(SDKCALLBACK_GAMEOBJECT_NAME, SDKCALLBACK_ONPAY, "1");
                                Toast.makeText(MainActivity.this, "购买失败", Toast.LENGTH_SHORT).show();
                                break;
                            case XYGameSDKStatusCode.PAY_CANCLE_CODE:
                                UnityPlayer.UnitySendMessage(SDKCALLBACK_GAMEOBJECT_NAME, SDKCALLBACK_ONPAY, "2");
                                Toast.makeText(MainActivity.this, "购买取消", Toast.LENGTH_SHORT).show();
                                break;
                            default:
                                break;
                        }
                    }
                });
            }
        });


    }

//    public void shareText(String subject, String body) {
//        Intent sharingIntent = new Intent(android.content.Intent.ACTION_SEND);
//        sharingIntent.setType("text/plain");
//        sharingIntent.putExtra(android.content.Intent.EXTRA_SUBJECT, subject);
//        sharingIntent.putExtra(android.content.Intent.EXTRA_TEXT, body);
//        startActivity(Intent.createChooser(sharingIntent, "Share via"));
//    }
}
