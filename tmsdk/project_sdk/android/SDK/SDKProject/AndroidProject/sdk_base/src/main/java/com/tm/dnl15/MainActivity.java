package com.tm.dnl15;

import android.os.Bundle;


import com.example.administrator.myapplication.BaseActivity;
import com.tm.dnl.util.AndroidBug5497Workaround;

/**
 * Created by SmileMe on 2019/6/17.
 */
public class MainActivity extends BaseActivity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        AndroidBug5497Workaround.assistActivity(this);
    }
}
