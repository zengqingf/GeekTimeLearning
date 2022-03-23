<?php
header("Content-Type:text/html; charset=utf-8");
require_once dirname(__FILE__).'/../config/config.php';
require_once dirname(__FILE__) . '/../common/curl.class.php';

class lblogin {

  public function __construct() {
  }

  /**
   * 对应lbapi.doc中2.1.2
   */
  public function login($username = '', $password = '') {
    $curlPostarr['action'] = ACTION_LOGIN;
    $curlPostarr['acc'] = $username;
    $curlPostarr['pwd'] = md5($password);
    $curlPost = "data=".rawurlencode(base64_encode(json_encode($curlPostarr)));//普通加密方式
    return curl::curlpost(URL_LOGIN,$curlPost);
  }
}