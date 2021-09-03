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
    return $type;
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


$DEVICETYPE=get_device_type();


echo '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
  <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
  <meta http-equiv="Content-Style-Type" content="text/css" />
  <meta name="generator" content="pandoc" />
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>'.$DEVICETYPE.'</title>
  <style type="text/css">code{white-space: pre;}</style>
  <link rel="stylesheet" href="css/custom.css" type="text/css" />
</head>';


echo "<html><body>";
// ios

if ($DEVICETYPE !== "android")
{
    echo '<h2 id="title">iOS</h2>';
    echo '<p><a id="get_udid" href="http://192.168.2.65/dnf/guideudid/guide.html">iOS获取UDID</a></p>';
    echo '<br />';

    echo '<p><a id="install_cer" href="http://192.168.2.65/dnf/ssl_2019_2_147/ca/ca.crt">首次安装证书</a></p>';
    echo '<p><a id="install_cer_guide" href="http://192.168.2.65/dnf/guide/guide.html">安装证书流程</a></p>';
    echo '<br />';
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

function pc_exe($title, $apk_list, $cnt)
{
    //if (strncmp(get_device_type(), "ios", count("ios")) == 0)

    echo '<h2 id="pc">'.$title.'</h2>';

    $len = min($cnt, count($apk_list));

    clean_zip_with_max_count($apk_list, 10);

    for ($i = 0; $i < $len; $i++)
    {
        $value = $apk_list[$i];
        $bname = basename($value);
        $url = constrict_url_path("http://192.168.2.147/dnl/" . $value);
        echo '<p><a id="pc" href="'. $url . '">' . get_show_name($bname, $value) . '</a></p>';
    }

    if ($len <= 0) echo '<p><a> 什么都木有 :) </a></p>';
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
                    <string>com.tm.dnl2</string>
                    <key>bundle-version</key>
                    <string>1.0.0</string>
                    <key>kind</key>
                    <string>software</string>
                    <key>title</key>
                    <string>DNL(内部链接下载中..)</string>
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

        //echo '<p>'.$title.$value.'</p>';
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


if ($DEVICETYPE !== "android")
{

    echo '<div id="rootlayout">';
    echo '<div id="ioslayout">';

    $DIRS = get_dirs(new RecursiveDirectoryIterator("./__zip"));
    $len = count($DIRS);


    for ($i = 0; $i < $len; $i++)
    {
        $basename = basename($DIRS[$i]);
        $parentdir = dirname($DIRS[$i]);

        $dir = new RecursiveDirectoryIterator($DIRS[$i]);
        $ziparray = get_files($dir);

        if (strncmp($basename, "DNL2_ios", strlen("DNL2_ios")) == 0)
        {
            ios_ipa($basename, $ziparray, 25, $parentdir . "/" . "plist_".$basename);
        }
    }
    echo '</div>';
    echo '</div>';
}
/*
echo '<h2> 打版本url </h2>';

echo '<p><a id="iosdev" href="http://192.168.2.65:8080/job/DNF_iOS_SVN_DEV/buildWithParameters?token=iosdev">打版本(调试)</a></p>';
echo '<p><a id="iosdev" href="http://192.168.2.65:8080/job/DNF_iOS_SVN_DEV/buildWithParameters?token=iosdev&SDKXY=true&Marco=LOG_ERROR;&Mode=release&SDK=DNF">打版本XY正式</a></p>';

*/

$cnt = 4;
#ios_ipa("稳定的版本", $release_ipa_array, $cnt, "plist_release");



echo '</div>';

if ($DEVICETYPE !== "ios")
{
    echo '<div id="androidlayout">';

    $DIRS = get_dirs(new RecursiveDirectoryIterator("./__zip"));
    $len = count($DIRS);
    for ($i = 0; $i < $len; $i++)
    {
        $basename = basename($DIRS[$i]);
        $parentdir = dirname($DIRS[$i]);

        $dir = new RecursiveDirectoryIterator($DIRS[$i]);
        $ziparray = get_files($dir);

        if (strncmp($basename, "DNL2_android", strlen("DNL2_android")) == 0)
        {
            android_apk($basename, $ziparray, 20);
        }
    }


/*
echo '<h2> 打版本url </h2>';

echo '<p><a id="androidbanshu" href="http://192.168.2.65:8080/job/DNF_Android_SVN_BANSHU_2/buildWithParameters?token=banshu">打版本版署版本</a></p>';
 */

    echo '</div>';
}
echo '</div>';

echo '<div>';
if ($DEVICETYPE !== "ios" && $DEVICETYPE !== "android")
{
    echo '<div id="pclayout">';
    $DIRS = get_dirs(new RecursiveDirectoryIterator("./__zip"));
    $len = count($DIRS);
    for ($i = 0; $i < $len; $i++)
    {
        $basename = basename($DIRS[$i]);
        $parentdir = dirname($DIRS[$i]);

        $dir = new RecursiveDirectoryIterator($DIRS[$i]);
        $ziparray = get_files($dir);

        if (strncmp($basename, "DNL2_WIN", strlen("DNL2_WIN")) == 0 ||
            strncmp($basename, "DNL2_MAC", strlen("DNL2_MAC")) == 0
        )
        {
            pc_exe($basename, $ziparray, 20);
        }
    }

    echo '</div>';

}
echo '</div>';


echo "</body></html>";
?>
