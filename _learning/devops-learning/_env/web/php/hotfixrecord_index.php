<?php
$dir = "./hotfix/dev2/hotfix_records";//目录

$files = array(array());
$size = array();
$time = array();
$name = array();

if (is_dir($dir)) {
    if ($dh = opendir($dir)) {
        $i = 0;
        while (($file = readdir($dh)) !== false) {
            if ($file != "." && $file != ".."  && pathinfo($file, PATHINFO_EXTENSION) == "html") {
                #echo pathinfo($file, PATHINFO_EXTENSION)."<br>";
                #echo pathinfo($file, PATHINFO_BASENAME)."<br>";
                #echo date("Y-m-d H:i:s",filemtime($dir.'/'.$file))."<br>";
                #echo $file."<br>";
                $files[$i]["name"] = $file;//获取文件名称
                $files[$i]["size"] = round((filesize($dir.'/'.$file)/1024),2);//获取文件大小
                $files[$i]["time"] = date("Y-m-d H:i:s",filemtime($dir.'/'.$file));//获取文件最近修改日期
                $i++;
            }
        }
    }
    closedir($dh);

    foreach($files as $k=>$v){
        $size[$k] = $v['size'];
        $time[$k] = $v['time'];
        $name[$k] = $v['name'];
    }
    array_multisort($time,SORT_DESC,SORT_STRING, $files);//按时间排序
    //array_multisort($name,SORT_DESC,SORT_STRING, $files);//按名字排序
    //array_multisort($size,SORT_DESC,SORT_NUMERIC, $files);//按大小排序
    //print_r($files);
    if (count($files) > 0) {
        echo file_get_contents( "http://101.37.173.236:55002/dnf/hotfix/dev2/hotfix_records/".$files[0]['name']);
    }
}
/*
$hotfixrecord_dir = "./hotfix/dev2/hotfix_records";
if (is_dir($hotfixrecord_dir)){
    if($handler = opendir($hotfixrecord_dir)){
        $i = 0;
        while(($file = readdir($handler)) !== false){
            if($file != '.' && $file  != ".."){
                $files[$i]["time"] = date("Y-m-d H:i:s", filemtime($file));
                $i++;
            }
        }
    }
}
closedir($handler);
foreach ($files as $k=>$v){
    $time[$k] = $v["time"];
}
array_multisort($time, SORT_DESC, SORT_STRING, $files);
print_r($files);

*/

/*
$url = "http://192.168.2.148/dnl/index.php";
//1、初始化curl
$ch = curl_init();
//2、设置参数,参数1初始化$ch,参数2设置常量,参数3常量的值
//设置请求url网址
curl_setopt($ch,CURLOPT_URL,$url);
//捕获url响应信息不输出
#curl_setopt($ch,CURLOPT_RETURNTRANSFER,1);
//设置请求头信息
#curl_setopt($ch,CURLOPT_HEADER,0);
//设置传输post数组
//$data = array(
//    'para'=>'你是男的还是女的'
//);
//设置开启POST请求
//curl_setopt($ch,CURLOPT_POST,1);
//传输参数值
//curl_setopt($ch,CURLOPT_POSTFIELDS,$data);
//3、执行curl
$output = curl_exec($ch);
//4、关闭句柄
curl_close($ch);
if($output === False) {
    echo 'error:'.curl_error($ch);
}
echo $output;
*/
?>