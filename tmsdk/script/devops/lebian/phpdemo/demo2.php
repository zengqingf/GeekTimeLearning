<?php
header("Content-Type:text/html; charset=utf-8");
include dirname(__FILE__).'/config/custom.config.php';
include dirname(__FILE__).'/example/lblogin.class.php';
include dirname(__FILE__).'/example/channelmanage.class.php';

//登录===============================================================================================
$lblogin = new lblogin();
$userinfo = $lblogin->login($config_custom['custname'], $config_custom['custpwd']);
if (empty($userinfo)) die("网络错误1");
echo "登录结果：".$userinfo."<br>";
$userinfo = json_decode($userinfo, true);

if ($userinfo['code'] > 0) die($userinfo['msg']);

//重要的账号信息，以下只列举本例用到的信息，其它请参考API文档或查看返回数据
$chid = $userinfo['data']['id']; //账号对应的mainchid
$ustate = $userinfo['data']['ustate']; //0-正常使用，未修改密码；2-正常使用，已修改密码；100-停用
if ($ustate == 100) die("账号已停用");
$is_mhot = $userinfo['data']['mhot']; //是否支持热更，本例假设支持热更
$rsa_public_key = $userinfo['data']['public_key']; //rsa公钥，用于加密

//下文演示新增渠道、获取渠道列表、更新版本====================================================================
$chmgr = new channelmanage($rsa_public_key);

//新增渠道---------------------------------------------------------------------------------
$adddata['chid'] = $chid;
$adddata['chname'] = '我是渠道名称';
$adddata['apkfile'] = dirname(__FILE__).'/testapk/demo-v1.apk'; //使用您自己的包
//$adddata['apkfileid'] = $fileid; //也可以单独上传文件，把得到的fileid写到这里
$adddata['desc'] = '我是版本备注';

$addret = $chmgr->add($adddata);

if (empty($addret)) die("网络错误2");
echo "新增渠道：".$addret."<br>";
$addret = json_decode($addret, true);
if ($addret['code'] > 0) die($addret['msg']);

//获取渠道列表-------------------------------------------------------------------------------
$params['chid'] = $chid;
$params['chname'] = '我是渠道名称'; //渠道名称以'我是渠道名称'举例(模糊匹配)
$chinfo = $chmgr->getchannelinfo($params);
if (empty($chinfo)) die("网络错误3");
echo "渠道列表：".$chinfo."<br>";
$chinfo = json_decode($chinfo, true);
if ($chinfo['code'] > 0) die($chinfo['msg']);

//更新版本---------------------------------------------------------------------------------
//文件上传
$apkfile = dirname(__FILE__).'/testapk/demo-v2.apk'; //使用您自己的包
$apkfileid = $chmgr->uploadfile($chid, $apkfile);
if (empty($apkfileid)) die('文件上传失败');

//更新
$subchid = $chinfo['data']['list'][0]['subchid']; //假设上面获取渠道列表时，返回了一个'我是渠道名称'渠道
$uparams['chid'] = $chid;
$uparams['subchid'] = $subchid;
$uparams['apkfileid'] = $apkfileid;
$uparams['force'] = 0; //是否强更
$uparams['install'] = 0; //是否覆盖安装
$uparams['ntfymode'] = 0; //是否状态栏提示
$uparams['multimode'] = 0; //是否是自动更新模式，此处设为否，自动模式下，所有设置都使用默认值
$uparams['checked'] = 0; //是否经过二次确认，第一次请求时，应设为否
$update_result = $chmgr->update($uparams);
if (empty($update_result)) die("网络错误4");
echo "更新渠道：".$update_result."<br>";
$update_result = json_decode($update_result, true);

if ($update_result['code'] == 180) {
  if ($update_result['data']['samevercode'] == 1) { //情景：本次要更新的版本的versioncode和上一个版本的相同，假如versioncode：6
    if ($update_result['data']['delflag'] == 0) { //上一个版本未删除
      $uparams['insteadup'] = 1; //需选择替换上一个版本时，已经升级到上一个6的用户，是否继续升级这次的6，这里举例能升级
    } else { //上一个版本已删除
      $uparams['delup'] = 0; //需选择已经升级到上一个6的用户，是否继续升级这次的6，这里举例不能升级
    }
  }
  $uparams['checked'] = 1; //表示已经确认过了
  $update_result = $chmgr->update($uparams);
  if (empty($update_result)) die("网络错误4");
  echo "更新确认：".$update_result."<br>";
  $update_result = json_decode($update_result, true);
}
die($update_result['msg']);