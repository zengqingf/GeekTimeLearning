package com.tm.sdk.commonlib.util;

import android.content.Context;
import android.content.res.AssetManager;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;

public class FileUtils {

    /*
    * 读取assets下的文件
    * @return String
    * */
    public static String readFileInAssets(String fileName, Context context) {
        StringBuilder sb = new StringBuilder();
        try {
            AssetManager assetManager = context.getAssets();
            BufferedReader bf = new BufferedReader(new InputStreamReader(assetManager.open(fileName)));
            String line;
            while((line = bf.readLine()) != null) {
                sb.append(line);
            }
        }
        catch (IOException e){
            e.printStackTrace();
        }
        return sb.toString();
    }
}
