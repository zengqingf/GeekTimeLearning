<?php

class curl {

  public static function curlpost($url,$data) {
    $client_ip = curl::getip();
    // $client_ip = gethostbyname($_SERVER['COMPUTERNAME']);
    // $client_ip = "192.168.2.120";

    // print_r($url."\n");

    // print_r($client_ip."\n");
    // post方式
    $ch=curl_init();
    curl_setopt($ch,CURLOPT_URL,$url);
    curl_setopt($ch,CURLOPT_HEADER,0);
    curl_setopt ($ch, CURLOPT_HTTPHEADER, array('CLIENT-IP:'.$client_ip, 'X-FORWARDED-FOR:'.$client_ip));
    //如果成功只将结果返回，不自动输出返回的内容。
    curl_setopt($ch,CURLOPT_RETURNTRANSFER,1);
    //设置是通过post还是get方法
    curl_setopt($ch,CURLOPT_POST,1);
    //传递的变量
    curl_setopt($ch,CURLOPT_POSTFIELDS,$data);

    $data = curl_exec($ch);
    curl_close($ch);
    return $data;
  }

  public static function composeencdata($data, $key) {
    //导入rsa类
    require_once(dirname(__FILE__) . "/Rsa.class.php");

    $key_config['public_key'] = $key;
    $rsa = new Rsa($key_config);
    //公钥加密
    $pdata = $rsa->publicKeyEncode(json_encode($data));
    $encdata['chid'] = empty($data['chid']) ? 0 : $data['chid'];
    $encdata['encdata'] = $pdata;
    $result = base64_encode(json_encode($encdata));
    return $result;
  }

  /**
   * 获取客户端IP地址
   */
  public static function getip() {
    $onlineip = '';
    if (getenv('HTTP_CLIENT_IP') && strcasecmp(getenv('HTTP_CLIENT_IP'), 'unknown')) {
      $onlineip = getenv('HTTP_CLIENT_IP');
    } elseif (getenv('HTTP_X_FORWARDED_FOR') && strcasecmp(getenv('HTTP_X_FORWARDED_FOR'), 'unknown')) {
      $onlineip = getenv('HTTP_X_FORWARDED_FOR');
    } elseif (getenv('REMOTE_ADDR') && strcasecmp(getenv('REMOTE_ADDR'), 'unknown')) {
      $onlineip = getenv('REMOTE_ADDR');
    } elseif (isset($_SERVER['REMOTE_ADDR']) && $_SERVER['REMOTE_ADDR'] && strcasecmp($_SERVER['REMOTE_ADDR'], 'unknown')) {
      $onlineip = $_SERVER['REMOTE_ADDR'];
    }
    return $onlineip;
  }
}
?>
