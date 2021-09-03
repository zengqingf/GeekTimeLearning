<?php

use function Sodium\add;

echo '<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
  <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
  <meta http-equiv="Content-Style-Type" content="text/css" />
  <meta name="generator" content="pandoc" />
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>安卓线上强更，整包下载地址</title>
  <style type="text/css">code{white-space: pre;}</style>
  <style>.error {color: #FF0000;}</style>
  <!--<link rel="stylesheet" href="./custom.css" type="text/css" />-->
  
  <script type="text/javascript" language="JavaScript">
  function resetform()
    {
      alert("已重置，请重新输入后提交")
      //document.getElementById("apkform").reset()
      //var form = button.parentNode
      var form = document.getElementById("apkform");
      /*var tagname = null
      while(tagname != "APKFORM"){
          form = form.parentNode
          if(form == null){break}
          tagname = form.tagName
      }
      alert("222 2" + tagname)*/
      if(form != null){
          for (var i = form.elements.length - 1; i>=0; i--){
              if(form.elements[i].type == "text"){
                  if(form.elements[i].getAttribute("readonly") != "readonly"){
                      form.elements[i].value = ""
                  }
              }
          }
      }
    }
    
        var xmlHttp;  
		function createXMLHttpRequest(){ 
			//检查浏览器是否支持 XMLHttpRequest 对象
			if(window.ActiveXObject){  
				xmlHttp = new ActiveXObject("Microsoft.XMLHTTP");  
			}  
			else if(window.XMLHttpRequest){  
				xmlHttp = new XMLHttpRequest();  
			}  
		}  
    function reqOnlineUrl(btn) {
      alert("111")
      /*
      var url = "http://39.108.138.140:58888/config.js"
      var httpReq = new XMLHttpRequest()
      httpReq.open("GET", url, true)
      httpReq.setRequestHeader("Access-Control-Allow-Origin", "*")
      //httpReq.setRequestHeader("Cache-Control", "no-cache")
      httpReq.send()
      httpReq.onreadystatechange = function() {          
        if(httpReq.readyState == 4){
            if(httpReq.status == 200){
                var json = httpReq.responseText
                console.log(json)
            }
         }
      }*/
      
      /*
      $.ajax(
          {
            url: "index_android_download_urls.php",
            type:"POST",
            data:{action:btn.value},
            success:function(res) {
              alert(res)
            }
          }
      );*/
      
            createXMLHttpRequest();  
			var url="index_server_json.php";
			xmlHttp.open("POST",url,true); 
			xmlHttp.setRequestHeader(\'Content-type\', \'application/x-www-form-urlencoded\');
			xmlHttp.onreadystatechange = callback;  
			xmlHttp.send("action=" + btn.value);   
    }
        function callback(){  
			if(xmlHttp.readyState == 4){  
				if(xmlHttp.status == 200){  
					alert(xmlHttp.responseText);   
				}  
			}  
		} 
		
		function updateview()
		{
		    location.replace(location.href);
		}
  </script>
</head>';

session_start();

$versionErrorInfos = array();
$urlErrorInfos = array();

$versionInfos = array();
$urlInfos = array();

$postArray = array();

foreachPostInfos();

if(isset($_POST['code']) && isset($_SESSION['code']))
{
  if($_POST['code'] == $_SESSION['code'])
  {
    unset($_SESSION['code']);
    //生成json
    if($_SERVER["REQUEST_METHOD"] == "POST")
    {
      if(create_post_json())
      {
        //echo '<p>保底json已更新，请同步</p>';
        send_msg_to_dingding();
        echo "<script>alert('保底Json已提交')</script>";
      }else {
        echo "<script>alert('保底Json创建失败，请检查')</script>";
      }
    }

  }else {
    echo "<script>alert('请勿重复提交')</script>";
  }
}

function reqOnlineUrls()
{
    $data = file_get_contents("http://39.108.138.140:58888/config.js");
    $data_sub = substr_byfind( $data, "=");
    //echo ($data_sub);
    $data_sub = str_replace("port:", "\"port\":", $data_sub);
    $data_sub = str_replace("packages:", "\"packages\":", $data_sub);
    if(!empty($data_sub))
    {
        $infos = json_decode($data_sub, true);
        echo '<br><br>';
        echo '<div id="androidlayouts">';
        echo '<h2 id="android">目前线上安卓渠道下载地址</h2>';
        foreach ($infos["packages"] as $key => $value) {
            $channel = $key;
            $url = $value["url"];
            $version = $value["version"];
            echo '<br>';
            echo '<label for="online_channel_'.$channel.'">渠道：</label><input type="text" name="online_channel_'.$channel.'" readonly="readonly" value="'.$channel.'" />';
            echo '<br>';
            echo '<label for="online_version_'.$channel.'">版本号：</label><input type="text" name="online_version_'.$channel.'" readonly="readonly" value="'.$version.'" />';
            echo '<br>';
            echo '<label for="online_url_'.$channel.'">下载地址：</label><input type="text" name="online_url_'.$channel.'" readonly="readonly" value="'.$url.'"  style="width:1500px" />';
            if(!file_contents_exist($url))
            {
                echo '<span class="error">"链接失效，请检查！"</span>';
            }
            echo '<br>';
        }
        echo '<br><br>';
        echo '</div>';
    }
}

function substr_byfind($str, $find)
{
    if(empty($str) || empty($find))
    {
        return -1;
    }
    if(!strpos($str, $find))
    {
        return -1;
    }
    $index = strpos($str, $find);
    return substr($str, $index + 1);
}

function foreachPostInfos()
{
  if($_SERVER["REQUEST_METHOD"] == "POST")
  {
      $data = file_get_contents('channel_info.json');
      $infos = json_decode($data, true);
      $res = true;
      foreach ($infos as $key => $value) {
        $channel = $key;
        //if($_SERVER["REQUEST_METHOD"] == "POST")
        {
          $channel_name = $_POST["channel_$channel"];
          if(!empty($channel_name))
          {
            $array_2 = array();
            $array_2["version"] = receive_post_version($channel_name);
            $array_2["url"] = receive_post_url($channel_name);
            if(empty($array_2["version"]) || empty($array_2["url"])) 
            {
              $res = false;
            }else {
              $GLOBALS['postArray'][$channel_name] = $array_2;
            }
          }
        }
      }
      if(!$res)
      {
          $GLOBALS['postArray'] = array();
      }
  }
}

 if(isset($_POST['submit']))
 {
    echo "<mate http-equiv='refresh' content='0'>";
 }

function send_msg_to_dingding()
{
/*
    $data_str = '{
        \"msgtype\": \"text\",
        \"text\": {
            \"content\": \"保底Url已更新，请同步到线上\"
        },
        \"at\": {
            \"atMobiles\": [18258257734,18196796590,13588189160,17682316225], 
            \"isAtAll\": false
        }
    }';
*/

    $data = array(
        'msgtype' => 'text',
        'text' => array('content' => '保底Url已更新，请同步到线上'),
        'at' => array(
        'atMobiles' => array(18258257734,18196796590,13588189160,17682316225),
        'isAtAll' => false
        )
    );
    $data_str = json_encode($data);
    $dingdingtoken="ac6d1f433f4da9ef867943dcf8e4ad7eaafb2d9c5c70de7bdbbb572c0a5643b6";
    $ch = curl_init();
    curl_setopt($ch, CURLOPT_URL, "https://oapi.dingtalk.com/robot/send?access_token=$dingdingtoken");
    curl_setopt($ch, CURLOPT_POST, 1);
    curl_setopt($ch, CURLOPT_CONNECTTIMEOUT, 5);
    curl_setopt($ch, CURLOPT_HTTPHEADER, array ('Content-Type: application/json;charset=utf-8'));
    curl_setopt($ch, CURLOPT_POSTFIELDS, $data_str);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
    $res = curl_exec($ch);
    curl_close($ch);
    //echo "<script>alert('.$res.')</script>";
}

function receive_post_version($channel)
{
    $versionError = "";
    $version = "";
    
    {
       if(empty($_POST["version_$channel"]))
       {
          $versionError = "$channel ：版本号为空！";
       }else if (!preg_match("/^1.[0-9]\d*.[0-9]\d*.[0-9]\d*$/", $_POST["version_$channel"])) {
          $versionError = "$channel ：版本号格式错误，应该为1.xx.xx.xxxxxxxx";
       }
       else {
          $version = $_POST["version_$channel"];
          $version = test_input($version);
       }
    }
    if(!empty($versionError))
    {
        $GLOBALS["versionErrorInfos"][$channel] = $versionError;
        //echo '<p>111'.$GLOBALS["versionErrorInfos"][$channel].'</p>';
    }
    else
    {      
        $GLOBALS['versionInfos'][$channel] = $version;
        //echo '<p>222'.$GLOBALS['versionInfos'][$channel].'</p>';
    }
    return $version;
}

function receive_post_url($channel)
{
    $urlError = "";
    $url = "";
    //if($_SERVER["REQUEST_METHOD"] == "POST")
    {
       if(empty($_POST["url_$channel"]))
       {
          $urlError = "$channel ：链接为空！";
       }else if (!preg_match("/\b(?:(?:https?|ftp):\/\/|www\.)[-a-z0-9+&@#\/%?=~_|!:,.;]*[-a-z0-9+&@#\/%=~_|]/i", $_POST["url_$channel"])) {
          $urlError = "$channel ：链接格式错误，请检查";
       }else if(!file_contents_exist($_POST["url_$channel"])){
           $urlError = "$channel ：链接失效！请检查";
       }else {
          $url = $_POST["url_$channel"];
          $url = urldecode(test_input($url));
       } 
    }
    if(!empty($urlError))
    {
        $GLOBALS['urlErrorInfos'][$channel] = $urlError;
        //echo '<p>333'.$GLOBALS['urlErrorInfos'][$channel].'</p>';
    }
    else
    {
        $GLOBALS['urlInfos'][$channel] = $url;
        //echo '<p>444'.$GLOBALS['urlInfos'][$channel].'</p>';
    }
    return $url;
}

function create_post_json()
{
  $jpath = "./channels.json";
  if(!empty($GLOBALS['postArray']))
  {
    $json = json_encode($GLOBALS['postArray'], JSON_UNESCAPED_SLASHES | JSON_UNESCAPED_UNICODE | JSON_PRETTY_PRINT);
    if(file_put_contents($jpath, $json, LOCK_EX))
    {      
      return true;
    }
  }
  return false;
}

function test_input($data)
{
    $data = trim($data);
    $data = stripslashes($data);
    $data = htmlspecialchars($data);
    return $data;
}

function foreachApkInfos()
{
  $data = file_get_contents('channel_info.json');
  $infos = json_decode($data, true);
  foreach ($infos as $key => $value) {
    $channel = $key;
    $version = $value["version"];
    $url = urlencode($value["url"]);
    echo '<label for="channel_'.$channel.'">渠道：</label><input type="text" name="channel_'.$channel.'" readonly="readonly" value="'.$channel.'" />';
    echo '<br>';
    $ver = $version;
    if(array_key_exists($channel, $GLOBALS['versionInfos']))
    {  
      //echo '<p>version:'.$GLOBALS['versionInfos'][$channel].'</p>';
      if(!empty($GLOBALS['versionInfos'][$channel]))
      {          
          $ver = $GLOBALS['versionInfos'][$channel];
      }
    }
    echo '<label for="version_'.$channel.'">版本号：</label><input type="text" name="version_'.$channel.'" value="'.$ver.'" />';
      if(!empty($GLOBALS["versionErrorInfos"][$channel]))
      {
          echo '<span class="error">* '.$GLOBALS["versionErrorInfos"][$channel].'</span>';
      }
    echo '<br>';
      $u = $url;
      if(array_key_exists($channel, $GLOBALS['urlInfos']))
      {  
        //echo '<p>url:'.$GLOBALS['urlInfos'][$channel].'</p>';
        if(!empty($GLOBALS['urlInfos'][$channel]))
        {            
            $u = $GLOBALS['urlInfos'][$channel];
        }
      }
    echo '<label for="url_'.$channel.'">下载地址：</label><input type="text" name="url_'.$channel.'" value="'.$u.'"  style="width:1500px" />';
      if(!empty($GLOBALS['urlErrorInfos'][$channel]))
      {
          echo '<span class="error">* '.$GLOBALS['urlErrorInfos'][$channel].'</span>';
      }
    echo '<br><br>';
  }
}

//效率较低

function file_contents_exist($url, $response_code = 200)
{
  $headers = get_headers($url);
  if(substr($headers[0], 9, 3) == $response_code)
  {
    return true;
  }else
  {
    return false;
  }
}

/*
function file_contents_exist($url)
{
  $ch = curl_init();
  curl_setopt($ch, CURLOPT_URL, $url);
  //不下载
  curl_setopt($ch, CURLOPT_NOBODY, true);
  //设置超时
  curl_setopt($ch, CURLOPT_CONNECTTIMEOUT,1);
  curl_setopt($ch, CURLOPT_TIMEOUT,1);
  curl_setopt($ch, CURLOPT_CUSTOMREQUEST, 'GET'); // 发送请求
  $res = curl_exec($ch);
  $exist = false;
  if($res)
  {
      $httpCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
      if($httpCode == 200)
      {
          $exist = true;
      }
  }
  curl_close($ch);
  return $exist;
}
*/

/*
function file_contents_exist($url)
{
    $fileExists = @file_get_contents($url, null, null, -1, 1) ? true : false;
    return $fileExists;
}*/

/*
function foreachOnlineApkInfos()
{
  $post_data = array();
  $data = curl_post_file_contents("http://39.108.138.140:58888/config.js", $post_data);
  $infos = json_decode($data, true);
  echo $data;
}
*/

//来源：https://www.cnblogs.com/woods1815/p/9598751.html
function curl_post_file_contents($furl, $post_data)
{
  $timestamp = time();
  $headers = array(
    "token:'.$timestamp.'",
    "over_time:5000",
  );  
  $curl = curl_init();
  curl_setopt($curl, CURLOPT_URL, $furl);
  //设置头文件的信息作为数据流输出
  curl_setopt($curl, CURLOPT_HEADER, false);
  //设置获取的信息以文件流的形式返回，而不是直接输出。
  curl_setopt($curl, CURLOPT_RETURNTRANSFER, true);
  //设置post方式提交
  curl_setopt($curl, CURLOPT_POST, true);
  // 设置post请求参数
  curl_setopt($curl, CURLOPT_POSTFIELDS, $post_data);
  // 添加头信息
  curl_setopt($curl, CURLOPT_HTTPHEADER, $headers);
  // CURLINFO_HEADER_OUT选项可以拿到请求头信息
  curl_setopt($curl, CURLINFO_HEADER_OUT, true);
  // 不验证SSL
  curl_setopt($curl, CURLOPT_SSL_VERIFYPEER, false);
  curl_setopt($curl, CURLOPT_SSL_VERIFYHOST, false);
  //执行命令
  $data = curl_exec($curl);

  // 打印请求头信息
  //echo curl_getinfo($curl, CURLINFO_HEADER_OUT);

  curl_close($curl);
  return $data;
}

echo "<html><body>";

echo '<div id="rootlayout">';
echo '<div id="ioslayout">';
echo '<h2 id="ios">更新安卓渠道下载地址</h2>';
echo '<form  id="apkform" method="POST" action="'.htmlspecialchars($_SERVER["PHP_SELF"]).'" target="_self">';//target="targetIfr"
foreachApkInfos();
//$code = mt_rand(0, 1000000);
$code =  md5(microtime(true));
$_SESSION['code'] = $code;
echo '<input type="hidden" name="code" value="'.$code.'">';
echo  '<input type="submit" value="提交" style="width:200px" />';
//echo  '<input type="reset" value="重置" style="width:200px" />';
echo  '<input type="button" value="重置" onclick="resetform(this)" style="width:200px" />';
echo '</form>';
echo '<br>';
//echo  '<Button type="button" id="reqOnlineUrlBtn" value="reqOnlineUrlBtn" onclick="reqOnlineUrl(this)" style="width:200px">线上链接有效性检查</Button>';
echo '<Button type="button" onclick="updateview()" style="width:200px">刷新页面</Button>';
echo '<iframe name="targetIfr" style="display: none"></iframe>';
echo '</div>';

reqOnlineUrls();

echo '</div>';
echo "</body></html>";
?>
