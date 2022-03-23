package com.example.administrator.myapplication;

import android.os.Bundle;

import com.unity3d.player.UnityPlayerActivity;
import android.util.Log;
import android.content.Intent;

public class BaseActivity extends UnityPlayerActivity  {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        // call UnityPlayerActivity.onCreate()
        super.onCreate(savedInstanceState);

        Log.e("test", "onCreate called");
    }

    // Add restart application function for android logic hot-fix -By Simon.King
    public void restartApplication()
    {
        new Thread()
        {
            public void run()
            {
                Intent launch= getBaseContext().getPackageManager().getLaunchIntentForPackage(getBaseContext().getPackageName());
                launch.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
                startActivity(launch);
                android.os.Process.killProcess(android.os.Process.myPid());
            }
        }.start();
        finish();
    }


}
