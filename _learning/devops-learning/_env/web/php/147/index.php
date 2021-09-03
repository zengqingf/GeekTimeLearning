<?php 
function get_human_filesize($bytes, $decimals = 2) 
{
    $sz = 'BKMGTP';
    $factor = floor((strlen($bytes) - 1) / 3);
    return sprintf("%.{$decimals}f", $bytes / pow(1024, $factor)) . @$sz[$factor];
}

function get_device_type()
{
    //全部变成小写字母
    //$agent = strtolower($_SERVER['HTTP_USER_AGENT']);
    $agent = strtolower(isset($_SERVER['HTTP_USER_AGENT']) ? $_SERVER['HTTP_USER_AGENT'] : "undefined");
    $type ='other';
    //分别进行判断
    if(strpos($agent,'iphone') || strpos($agent,'ipad'))
    {
        $type ='ios';
    }

    if(strpos($agent,'android'))
    {
        $type ='android';
    }

    if(strpos($agent, 'windows') && strpos($agent, '64'))
    {
        $type = 'win64';
    }  

    if(strpos($agent, 'windows') && strpos($agent, '64') === false )
    {
        $type = 'win';
    }

    if(strpos($agent, 'mac'))
    {
        $type = 'mac';
    }

    return$type;
}

function checkstr($str, $containstr)
{
    $tmp = explode($containstr, $str);
    if(count($tmp) > 1)
    {
        return true;
    }
    return false;
}

function constrict_url_path($path)
{
    $path=str_replace("/./", "/", $path);
    return $path;

        //转换dos路径为*nix风格
        $path=str_replace('\\','/',$path);
        //替换$path中的 /xxx/../ 为 / ，直到替换后的结果与原串一样（即$path中没有/xxx/../形式的部分）
        $last='';

        while($path!=$last){
            $last=$path;
            $path=preg_replace('/\/[^\/]+\/\.\.\//','/',$path);
        }

        //替换掉其中的 ./ 部分 及 //  部分
        $last='';

        while($path!=$last){
            $last=$path;
            $path=preg_replace('/([\.\/]\/)+/','/',$path);
        }
        return $path;
}

function get_files($dir) 
{
    $files = array();
 
    for (; $dir->valid(); $dir->next()) {
        if ($dir->isDir() && !$dir->isDot()) {
            //if ($dir->haschildren()) {
            //    $files = array_merge($files, get_files($dir->getChildren()));
            //};
        }else if($dir->isFile()){
            $files[] = $dir->getPathName();
        }
    }
    rsort($files);
    return $files;
}

function get_show_name($name, $path)
{
    return get_human_filesize(filesize($path)) . " --> ". $name;
    return $name;
    $arr = explode('_', $name, 4);
    if (count($arr) >= 4)
    {
        return $arr[2];
    }

    return $name;
}

function get_dirs($dir) 
{
    $files = array();
 
    for (; $dir->valid(); $dir->next()) {
        if ($dir->isDir() && !$dir->isDot()) {
           $files[] = $dir->getPathName();
        }else if($dir->isFile()){
            //$files[] = $dir->getPathName();
        }
    }
    rsort($files);
    return $files;
}


// ipa
//$path = "./ios";
//$dir = new RecursiveDirectoryIterator($path);
//$ipa_array = get_files($dir);
//
//$path = "./ios_release";
//$dir = new RecursiveDirectoryIterator($path);
//$release_ipa_array = get_files($dir);

// plist
$path = "./plist";
$dir = new RecursiveDirectoryIterator($path);
$plist_array = get_files($dir);

// android
//$path = "./android_release";
//$dir = new RecursiveDirectoryIterator($path);
//$android_array = get_files($dir);

// first delete all the plist file
//foreach ($plist_array as $plist_item)
//{
//    //unlink($plist_item);
//}

$MAXKEEPCOUNT=10;


global $DEVICETYPE;
$DEVICETYPE = get_device_type();

echo '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
  <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
  <meta http-equiv="Content-Style-Type" content="text/css" />
  <meta name="generator" content="pandoc" />
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title></title>
  <style type="text/css">code{white-space: pre;}</style>
  <link rel="stylesheet" href="css/custom.css" type="text/css" />
</head>';


echo "<html><body>";


echo '<h1 id="title">### 内网测试包地址 ###</h1>';
echo '<p><a id="dev_apk_ipa_backup" href="http://192.168.2.148/dnl/index.php">1.0内网测试包</a></p>';
echo '<h1 id="title">### 发布内容 ###</h1>';
echo '<p><a id="publish_apk_ipa_backup" href="http://192.168.2.147/dnl/next.php">1.0线上包备份目录</a></p>';
echo '<p><a id="publish_apk_ipa_md5" href="http://101.37.173.236:55002/dnf/hotfixrecord_index.php">1.0线上版本md5查询</a></p>';
echo '<p><a id="publish_hotfix_hack_file" href="http://101.37.173.236:55002/dnf/hotfix/dev2/hotfix_records/IOS_hotfix_updateserverhack/">1.0热更新地址文件</a></p>';

echo '<h2 id="guideinfo">一些常规操作参考</h2>';
echo '<p><a id="guide_svn_show_log" href="http://192.168.2.65/dnf/guidesvnlog/guide.html">查看全部SVN日志</a></p>';

echo '<h2 id="guideinfo">线上配置</h2>';
echo '<p><a id="guide_svn_show_log" href="http://192.168.2.65/dnf/getbaodi/index_android_download_urls.php">保底地址更新和查询</a></p>';

// ios
if ($DEVICETYPE !== "android")
{
    echo '<h2 id="title">iOS端工具</h2>';
    echo '<p><a id="install_cer" href="http://192.168.2.65/dnf/ssl_2019_2_147/ca/ca.crt">首次安装证书</a></p>';
    echo '<p><a id="install_cer_guide" href="http://192.168.2.65/dnf/guide/guide.html">安装证书流程</a></p>';
	echo '<p><a id="install_cer_guide_2" href="http://192.168.2.65/dnf/guideudid/guide.html">安装证书流程-New</a></p>';
}

function clean_zip_with_max_count($list, $max)
{
    $count = count($list);
    for ($i = $max; $i < $count; $i++)
    {
        if (file_exists($list[$i]))
        {
            //unlink($list[$i]);
        }
    }
}

function common_app_showlist($type, $title, $app_list, $cnt)
{
    echo '<h2 id="'.$type.'">'.$title.'</h2>';
    $len = min($cnt, count($app_list));
    clean_zip_with_max_count($app_list, 10);

    for ($i = 0; $i < $len; $i++)
    {
        $value = $app_list[$i];
        $bname = basename($value);
        $url = constrict_url_path("http://192.168.2.147/dnl/" . $value);
        echo '<p><a id="'.$type.'" href="'. $url . '">' . get_show_name($bname, $value) . '</a></p>';
    }

    if ($len <= 0) echo '<p><a> 什么都木有 :) </a></p>';
}

function win_exe($title, $win_exe_list, $cnt)
{
    common_app_showlist("win", $title, $win_exe_list, $cnt);
}

function win64_exe($title, $win64_exe_list, $cnt)
{
    common_app_showlist("win64", $title, $win64_exe_list, $cnt);
}

function mac_app($title, $mac_app_list, $cnt)
{
    common_app_showlist("mac", $title, $mac_app_list, $cnt);
}

function android_apk($title, $apk_list, $cnt)
{
    //if (strncmp(get_device_type(), "ios", count("ios")) == 0)

    echo '<h2 id="android">'.$title.'</h2>';

    $len = min($cnt, count($apk_list));

    clean_zip_with_max_count($apk_list, 10);

    for ($i = 0; $i < $len; $i++)
    {
        $value = $apk_list[$i];
        $bname = basename($value);
        $url = constrict_url_path("http://192.168.2.147/dnl/" . $value);
        echo '<p><a id="android" href="'. $url . '">' . get_show_name($bname, $value) . '</a></p>';
    }

    if ($len <= 0) echo '<p><a> 什么都木有 :) </a></p>';
}



function ios_ipa($title, $the_ipa, $cnt, $plist_dir_name) {
    $plist_str_head = '<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
    <dict>  
        <key>items</key>
        <array>
            <dict>
                <key>assets</key>
                <array>
                    <dict>
                        <key>kind</key>
                        <string>software-package</string>
                        <key>url</key>
                        <string>';
    $plist_str_tail = '</string>
                    </dict>
                </array>
                <key>metadata</key>
                <dict>
                    <key>bundle-identifier</key>
                    <string>com.hegu.dnl</string>
                    <key>bundle-version</key>
                    <string>1.0.0</string>
                    <key>kind</key>
                    <string>software</string>
                    <key>title</key>
                    <string>地下城与勇者(内部链接下载中..)</string>
                </dict>
            </dict>
        </array>
    </dict>
</plist>';

    echo '<h2 id="ios">' . $title . '</h2>';

    $count = count($the_ipa);
    $len = min($cnt, $count);

    clean_zip_with_max_count($the_ipa, 10);

    for ($i = 0; $i < $len; $i++)
    {
        $value = $the_ipa[$i];

        $bname = basename($value);
        $parentdir = dirname($value);

        // open a file to write the right plist

        $plist_name = $plist_dir_name . "/" . $bname . ".plist";

        if (!file_exists($plist_dir_name))
        {
            mkdir($plist_dir_name);
        }


        if (!file_exists($plist_name))
        {
            $plist_file = fopen($plist_name, "w");
            fwrite($plist_file, $plist_str_head . constrict_url_path("https://192.168.2.147/dnl/". $value). $plist_str_tail);
            fclose($plist_file);
        }

        $url = constrict_url_path("https://192.168.2.147/dnl/" . $plist_name);

        echo '<div id="iositemlayout">';
        echo '<a id="ios" href="itms-services://?action=download-manifest&url=' .$url. '">' . get_show_name($bname, $value);

        $dsym_file = "./__dSymbols/" . str_replace(".ipa", "", $bname);
        if (file_exists($dsym_file))
        {
            echo '-[dSYM]';
        }

        echo '</a>';
        echo '</div>';

    }
    if ($len <= 0) echo '<p><a> 什么都木有 :) </a></p>';
}


echo '<h1 id="title">### 预发布内容 ###</h1>';

$DIRS_ALL = get_dirs(new RecursiveDirectoryIterator("./__zip"));
$len_all = count($DIRS_ALL);

$VERSION_TYPE_1_5 = "DNL_1_5";
$DIRS_1_5 = array();

for ($i = 0; $i < $len_all; $i++)
{
    $basename = basename($DIRS_ALL[$i]);
    if(checkstr($basename, $VERSION_TYPE_1_5))
    {
        array_push($DIRS_1_5, $DIRS_ALL[$i]);
    }
}
$len_1_5 = count($DIRS_1_5);

$DIRS_NO_1_5 = array_values(array_diff($DIRS_ALL, $DIRS_1_5));
$len_no_1_5 = count($DIRS_NO_1_5);

echo '<div id="rootlayout">';
echo '<h2 id="version_DNL_1_0">版本：DNL_1_0</h2>';
show_app_urls($DIRS_NO_1_5, $len_no_1_5);
echo '<HR style="border:1 dashed #987cb9" width="80%" color=#987cb9 SIZE=1>';
echo '<h2 id="version_'.$VERSION_TYPE_1_5.'">版本：'.$VERSION_TYPE_1_5.'</h2>';
show_app_urls($DIRS_1_5, $len_1_5);

function show_app_urls($DIRS, $len)
{
    //传参
    global $DEVICETYPE;
	
	$pc_platform = ($DEVICETYPE === "mac" || $DEVICETYPE === "win" || $DEVICETYPE === "win64");

    if ($DEVICETYPE === "ios" || $pc_platform)
    {
        echo '<div id="ioslayout">';

        for ($i = 0; $i < $len; $i++)
        {
            $basename = basename($DIRS[$i]);
            $parentdir = dirname($DIRS[$i]);

            $dir = new RecursiveDirectoryIterator($DIRS[$i]);
            $ziparray = get_files($dir);

            if (strncmp($basename, "ios", strlen("ios")) == 0)
            {
                ios_ipa($basename, $ziparray, 25, $parentdir . "/" . "plist_".$basename);
            }
            else if (strncmp($basename, "mac", strlen("mac")) == 0)
            {
                mac_app($basename, $ziparray, 20);
            }
        }

        echo '</div>';
    }
    /*
    echo '<h2> 打版本url </h2>';

    echo '<p><a id="iosdev" href="http://192.168.2.65:8080/job/DNF_iOS_SVN_DEV/buildWithParameters?token=iosdev">打版本(调试)</a></p>';
    echo '<p><a id="iosdev" href="http://192.168.2.65:8080/job/DNF_iOS_SVN_DEV/buildWithParameters?token=iosdev&SDKXY=true&Marco=LOG_ERROR;&Mode=release&SDK=DNF">打版本XY正式</a></p>';

    */

    #$cnt = 4;
    #ios_ipa("稳定的版本", $release_ipa_array, $cnt, "plist_release");


    if ($DEVICETYPE === "android" || $pc_platform)
    {
        echo '<div id="androidlayout">';

        for ($i = 0; $i < $len; $i++)
        {
            $basename = basename($DIRS[$i]);
            $parentdir = dirname($DIRS[$i]);

            $dir = new RecursiveDirectoryIterator($DIRS[$i]);
            $ziparray = get_files($dir);

            if (strncmp($basename, "android", strlen("android")) == 0)
            {
                android_apk($basename, $ziparray, 20);
            }
            else if (strncmp($basename, "win64", strlen("win64")) == 0)
            {
                win64_exe($basename, $ziparray, 20);
            }
            else if(strncmp($basename, "win", strlen("win")) == 0)
            {
                win_exe($basename, $ziparray, 20);
            }
        }


    /*
    echo '<h2> 打版本url </h2>';

    echo '<p><a id="androidbanshu" href="http://192.168.2.65:8080/job/DNF_Android_SVN_BANSHU_2/buildWithParameters?token=banshu">打版本版署版本</a></p>';
    */

        echo '</div>';
    }
}
echo '</div>';

echo "</body></html>";
?>
