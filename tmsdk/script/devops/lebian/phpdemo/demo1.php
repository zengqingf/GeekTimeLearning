<?php
header("Content-Type:text/html; charset=utf-8");
include dirname(__FILE__).'/config/custom.config.php';
include dirname(__FILE__).'/example/lblogin.class.php';
include dirname(__FILE__).'/example/channelmanage.class.php';

//登录===============================================================================================
$lblogin = new lblogin();
//$userinfo = $lblogin->login($argv[1], $argv[2]);

$argv_plat = $argv[1];
$argv_account = $argv[2];
$argv_password = $argv[3];
$argv_packpath = $argv[4];
$argv_md5 = $argv[5];


$userinfo = $lblogin->login($argv_account, $argv_password);

if (empty($userinfo)) throw new Exception("网络错误1");
echo "登录结果：".$userinfo."<br>";
$userinfo = json_decode($userinfo, true);

if ($userinfo['code'] > 0) throw new Exception($userinfo['msg']);

//重要的账号信息，以下只列举本例用到的信息，其它请参考API文档或查看返回数据

$chid = $userinfo['data']['id']; //账号对应的mainchid
$plat = $userinfo['data']['plat']; // a=android i=ios
$ustate = $userinfo['data']['ustate']; //0-正常使用，未修改密码；2-正常使用，已修改密码；100-停用
if ($ustate == 100) throw new Exception("账号已停用");
$is_mhot = $userinfo['data']['mhot']; //是否支持热更，本例假设支持热更
$rsa_public_key = $userinfo['data']['public_key']; //rsa公钥，用于加密

$chmgr = new channelmanage($rsa_public_key,$chid,$plat);

print_r($res);
ob_flush();
flush();
echo $argv_packpath;
$r = $chmgr->makeFenBao($argv_packpath);
print_r($r);
ob_flush();
flush();
if ($r['code'] != 0){
    throw new Exception('出错了'); 
}
$timeout = 1800;//30min
$timespan = 5;
$curtime = $timeout;
$fenbao_url = '';
$filename = 'None';
while ($curtime > 0) {
    sleep($timespan);
    $curtime = $curtime-$timespan;

    $res = $chmgr->getFenBaoList();
    $len = count($res);
    if ($len == 0) throw new Exception('makeFenBao ERROR');
    if ($len >= 1)
    {
        $arr = $res['data']['list'];
        for ($i=0; $i < $len; $i++) { 
            if ($arr[$i]['bmd5'] == $argv_md5)
            {
                print_r($arr[$i]);
                ob_flush();
                flush();
                if ($arr[$i]['state'] == 3)
                {
                    $fenbao_url = $arr[$i]['download'];
                    $filename = $arr[$i]['oldname'];
                    break; 
                }
            }
        }  
    } 
    if ($fenbao_url != '')
    {
        echo $fenbao_url;
        ob_flush();
        flush();
        break;
    }

}
if ($fenbao_url != '')
{
    $val = array();
    $val['fenbao_url'] = $fenbao_url;
    $str_json = json_encode($val);
    file_put_contents('val.json',$str_json);

}
