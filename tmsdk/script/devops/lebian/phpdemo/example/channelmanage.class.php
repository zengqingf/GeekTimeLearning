<?php
header("Content-Type:text/html; charset=utf-8");
require_once dirname(__FILE__).'/../config/config.php';
require_once dirname(__FILE__) . '/../common/curl.class.php';

class channelmanage {

  public $public_key;
  public $chid;
  public $plat;

  public function __construct($key, $chid, $plat) {
    $this->public_key = $key;
    $this->chid = $chid;
    $this->plat = $plat;

  }

  /**
   * 对应lbapi.doc文档2.3.4
   */
  public function add($config) {
    //文件是否已经上传
    if (empty($config['apkfileid'])) {
      if (empty($config['apkfile'])) return '文件不存在';
      $apkfile = $config['apkfile'];
      if (!file_exists($apkfile)) return '文件不存在';

      //文件上传
      $apkfileid = $this->uploadfile($config['chid'], $apkfile, 3);
    } else {
      $apkfileid = $config['apkfileid'];
    }
    if (empty($apkfileid)) return "文件上传失败";
    $params['action'] = ACTION_ADDCHANNEL;
    $params['chid'] = $config['chid'];
    $params['apkfileid'] = $apkfileid;
    $params['chname'] = $config['chname'];
    $params['desc'] = $config['desc'];
    $cdata['data'] = curl::composeencdata($params, $this->public_key);

    return curl::curlpost(URL_OTA, $cdata);
  }

  /**
   * 对应lbapi.doc文档2.3.1
   */
  public function getchannelinfo($config) {
    $params['action'] = ACTION_CHANNELLIST;
    $params['chid'] = isset($config['chid']) ? $config['chid'] : 0;
    $params['pagesize'] = isset($config['pagesize']) ? $config['pagesize'] : 10;
    $params['pagecode'] = isset($config['pagecode']) ? $config['pagecode'] : 1;
    $params['chname'] = isset($config['chname']) ? $config['chname'] : '';

    $cdata['data'] = curl::composeencdata($params, $this->public_key);

    return curl::curlpost(URL_OTA, $cdata);
  }

  /**
   * 对应lbapi.doc文档2.3.5
   */
  public function update($config) {
    //文件是否已经上传
    if (empty($config['apkfileid'])) {
      if (empty($config['apkfile'])) return '文件不存在';
      $apkfile = $config['apkfile'];
      if (!file_exists($apkfile)) return '文件不存在';

      //文件上传
      $apkfileid = $this->uploadfile($config['chid'], $apkfile, 3);
    } else {
      $apkfileid = $config['apkfileid'];
    }
    if (empty($apkfileid)) return "文件上传失败";
    $params['action'] = ACTION_UPDATECHANNEL;
    $params['chid'] = $config['chid'];
    $params['subchid'] = $config['subchid'];
    $params['apkfileid'] = $apkfileid;
    $params['force'] = $config['force'];
    $params['install'] = $config['install'];
    $params['ntfymode'] = $config['ntfymode'];
    $params['multimode'] = $config['multimode'];
    $params['checked'] = $config['checked'];
    $cdata['data'] = curl::composeencdata($params, $this->public_key);

    return curl::curlpost(URL_OTA, $cdata);
  }

  /**
   * 对应lbapi.doc文档2.3.15
   * 如果同一个渠道有多个包，请按versioncode从低到高顺序依次调用接口
   */
  public function autoupdate($config) {
    //文件是否已经上传
    if (empty($config['apkfileid'])) {
      if (empty($config['apkfile'])) return '文件不存在';
      $apkfile = $config['apkfile'];
      if (!file_exists($apkfile)) return '文件不存在';

      //文件上传
      $apkfileid = $this->uploadfile($config['chid'], $apkfile, 3);
    } else {
      $apkfileid = $config['apkfileid'];
    }
    if (empty($apkfileid)) return "文件上传失败";
    $params['action'] = ACTION_AUTOUPDATE;
    $params['chid'] = $config['chid'];
    $params['fid'] = $apkfileid;
    $params['chname'] = empty($config['chname']) ? '' : $config['chname']; //如果为空，后台将会使用clientchid作为渠道名称
    $cdata['data'] = curl::composeencdata($params, $this->public_key);

    return curl::curlpost(URL_OTA, $cdata);
  }

  /**
   * 对应lbapi.doc文档2.12.1
   */
  public function uploadfile($chid, $file, $part) {
    if (empty($chid) || empty($file) || !file_exists($file)) 
    {
      echo 'eupladfile empty';
      echo $chid;
      return 0;
    }
    $uploadconfig['chid'] = $chid;
    $uploadconfig['part'] = $part; //用途
    $encdata = curl::composeencdata($uploadconfig, $this->public_key);

    print_r($chid."\n");
    print_r($part."\n");
    // print_r($encdata."\n");
    $postdata['data'] = $encdata;
    $postdata['file'] = new CURLFile($file);
    //print_r($postdata['file']->."\n");
    $ret = curl::curlpost("http://lbapi.loveota.com/upload.php", $postdata);
    $ret = json_decode($ret, true);
    $fileid = empty($ret['data']['fileid']) ? 0 : $ret['data']['fileid'];
    print_r($ret);
    print_r($fileid."\n");
    return $fileid;
  }
  public function makeFenBao($packpath)
  {
    $part = 1;
    if ($this->plat == 'i') $part = 11;
    $fileid = $this -> uploadfile($this -> chid,$packpath,$part);
    $params['action'] = 'saveTrial';
    $params['chid'] = $this->chid;
    $params['fid'] = $fileid;

    $cdata['data'] = curl::composeencdata($params, $this->public_key);
    $url = 'http://lbapi.loveota.com/sdktrial.php';
    if ($this->plat == 'i') $url = 'http://lbapi.loveota.com/sdktrialios.php';

    $ret = curl::curlpost($url, $cdata);
    $ret_json = json_decode($ret, true);
    print_r($ret_json);

    return $ret_json;
  }
  public function getFenBaoList()
  {
    $params['action'] = 'getList';
    $params['chid'] = $this->chid;
    $params['pagesize'] = 5;
    $params['pagecode'] = 1;

    $cdata['data'] = curl::composeencdata($params, $this->public_key);

    $url = 'http://lbapi.loveota.com/sdktrial.php';
    if ($this->plat == 'i') $url = 'http://lbapi.loveota.com/sdktrialios.php';

    $ret = curl::curlpost($url, $cdata);
    return json_decode($ret, true);
  }

}
?>