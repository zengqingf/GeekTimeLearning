<?php
header("Content-Type:text/html; charset=utf-8");
include dirname(__FILE__).'/../config/custom.config.php';
include dirname(__FILE__).'/../example/lblogin.class.php';
include dirname(__FILE__).'/../example/channelmanage.class.php';


class lbinterface {
    public $chmgr;

    public $rsa_public_key;
    public $chid;
    public $plat;
  
    public function __construct($account, $password) {
        //登录===============================================================================================
        $lblogin = new lblogin();
        $count = 0;
        print_r("登陆中"."\n");
        while (true) {
            $count += 1;
            $userinfo = $lblogin->login($account, $password);
            if (empty($userinfo)) {
                if ($count > 10) {
                    throw new Exception("网络错误1");
                }
                else{
                    print_r("登陆重试"."\n");
                    sleep(3);
                }
            }
            else{
                break;
            }
        }
        
        echo "登录结果：".$userinfo."<br>";
        $userinfo = json_decode($userinfo, true);

        if ($userinfo['code'] > 0) throw new Exception($userinfo['msg']);

        //重要的账号信息，以下只列举本例用到的信息，其它请参考API文档或查看返回数据

        $this->chid = $userinfo['data']['id']; //账号对应的mainchid
        $this->plat = $userinfo['data']['plat']; // a=android i=ios
        $ustate = $userinfo['data']['ustate']; //0-正常使用，未修改密码；2-正常使用，已修改密码；100-停用
        if ($ustate == 100) throw new Exception("账号已停用");
        $is_mhot = $userinfo['data']['mhot']; //是否支持热更，本例假设支持热更
        $this->rsa_public_key = $userinfo['data']['public_key']; //rsa公钥，用于加密
        
        $this->chmgr = new channelmanage($this->rsa_public_key,$this->chid,$this->plat);

    }

    public function makefenbao($param){
        $packpath = $param[0];
        $md5 = $param[1];
        $r = $this->chmgr->makeFenBao($packpath);
        $timeout = 1800;//30min
        $timespan = 5;
        $curtime = $timeout;
        $fenbao_url = '';
        $filename = 'None';
        while ($curtime > 0) {
            sleep($timespan);
            $curtime = $curtime-$timespan;
        
            $res = $this->chmgr->getFenBaoList();
            $len = count($res);
            if ($len == 0) throw new Exception('makeFenBao ERROR');
            if ($len >= 1)
            {
                $arr = $res['data']['list'];
                for ($i=0; $i < $len; $i++) { 
                    if ($arr[$i]['bmd5'] == $md5)
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
                echo $fenbao_url.PHP_EOF;
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
    }

    
    public function getchannelinfo() {
        $params['action'] = "channellist";
        $params['chid'] = $this->chid;
        $params['pagesize'] = 100;
        $params['pagecode'] = 1;
    
        $cdata['data'] = curl::composeencdata($params, $this->rsa_public_key);
    
        $ret = curl::curlpost("http://lbapi.loveota.com/lbota.php", $cdata);
        $ret_decode = json_decode($ret, true);
        // print_r($ret_decode);

        return $ret_decode;
    }
    public function uploadregeng($param){
        $ClientChId = $param[0];
        $packId = $param[1];
        $apkpath = $param[2];
        $force = $param[3];
        $install = $param[4];

        $params['subchid'] = -1;
        $info = $this->getchannelinfo();
        $list = $info['data']['list'];

        $chname = '乐变渠道中文名';
        for ($i=0; $i < count($list); $i++) { 
            // print_r($list[$i]['cpmeta']."\n");
            if ($list[$i]['cpmeta'] == $ClientChId && $list[$i]['pkgname'] == $packId) {
                $params['subchid'] = $list[$i]['subchid'];
                $chname = $list[$i]['chname'];
                print_r($list[$i]);
                break;
            }
        }
        // return;
        if ($params['subchid'] == -1) {
            $this->STOP('没找到渠道 '.$ClientChId);
            return;
        }
        $params['apkfileid'] = 0;
        $params['apkfileid'] = $this->uploadfile($apkpath,3); // 3是apk热更新
        if ($params['apkfileid'] == 0){
            $this->STOP('上传文件失败 '.$apkpath);
            return;
        }
        $params['action'] = "updatechannel";
        $params['chid'] = $this->chid;
        $params['force'] = $force; // 是否强制更新 （0：否 1：是）
        $params['install'] = $install; // 是否覆盖安装 （0：否 1：是）
        $params['ntfymode'] = 0; // 是否状态栏显示 （0：否 1：是）
        $params['multimode'] = 0; // 是否是批量更新模式 （0：否 1：是）批量更新时，部分设置会使用默认选项
        $params['checked'] = 0; // 是否经过用户二次确认 （0：否 1：是），请置为0，何时置1参考下文备注
        print_r("热更请求参数"."\n");
        print_r($params);
        
        $cdata['data'] = curl::composeencdata($params, $this->rsa_public_key);
    
        $ret = curl::curlpost("http://lbapi.loveota.com/lbota.php", $cdata);
        $ret_decode = json_decode($ret, true);
        print_r("热更第一次请求返回"."\n");

        print_r($ret_decode);



        if ($ret_decode['code'] == 0){
            print_r('OKOKOKOKOKOKOKOKOKOKOKOKOKOKOKOKOKOKOKOKOKOKOKOK');
            
        }
        elseif ($ret_decode['code'] == 180) {
            print_r('!!!!需要二次确认!!!! '.$apkpath);
            print_r("\n");
            if ($ret_decode['data']['signmatch'] == 0){
                $this->STOP('签名不一致 '.$apkpath);
                return;
            }
            if (count($ret_decode['data']['libdiff']) != 0){
                $this->STOP('部分机型可能无法热更 '.$apkpath);
                return;
            }



            if (count($ret_decode['data']['newperm']) != 0) {
                print_r('新增权限 ');
                print_r("\n");

                for ($i=0; $i < count($ret_decode['data']['newperm']); $i++) { 
                    print_r($ret_decode['data']['newperm'][$i]);
                }

            }
            if ($ret_decode['data']['samevercode'] == 1){
                print_r('上一个版本的versioncode和本次上传的相同 '.$apkpath);
                print_r("\n");

                if ($ret_decode['data']['delflag'] == 0)                {
                    $params['insteadup'] = 1; // 替换同版本时上一版本是否升级（0：否 1：是），checked真时必填
                    print_r("同版本上一版本将会升级"."\n");
                }
                elseif ($ret_decode['data']['delflag'] == 1){
                    $params['delup'] = 1; // 已删除的同版本是否升级（0：否 1：是），checked真时必填
                    print_r("已删除的同版本将会升级"."\n");
                }
                else{
                    $this->STOP('未知错误码delflag = '.$ret_decode['data']['delflag']);
                    return;
                }
            }

            $params['checked'] = 1; // 是否经过用户二次确认 （0：否 1：是），请置为0，何时置1参考下文备注

            $cdata['data'] = curl::composeencdata($params, $this->rsa_public_key);
    
            $ret = curl::curlpost("http://lbapi.loveota.com/lbota.php", $cdata);
            $ret_decode = json_decode($ret, true);
            print_r("热更二次确认返回"."\n");
            print_r($ret_decode);


        }
        else{
            $this->STOP('未知错误码'.$ret_decode['code']);
            return;
        }
        $filepath = dirname(__FILE__).'/../data.json';
        $data = array();
        $data['subchid'] = $params['subchid'];
        $data['cpmeta'] = $ClientChId;
        $data['chname'] = $chname;
        file_put_contents($filepath,json_encode($data));


        // $fs = fopen('yuxiazai.txt','a');
        // fwrite($fs,$params['subchid']." ".$ClientChId."\n");
        // fclose($fs);
        // $fs = fopen('shangxian.txt','a');
        // fwrite($fs,$params['subchid']." ".$ClientChId."\n");
        // fclose($fs);
    }
    public function predownload($ps){
        // $fs = fopen('yuxiazai.txt','r') or $this->STOP("预下载文件不存在");
        // $rawstr = fread($fs,filesize("yuxiazai.txt"));
        // fclose($fs);


        // $chids = explode("\n",$rawstr);
        
        // $subchids = array();
        // for ($i=0; $i < count($chids); $i++) { 
        //     if ($chids[$i] != "") {
        //         $tmp = explode(" ",$chids[$i]);

        //         $subchid = $tmp[0];
        //         $ClientChId = $tmp[1];
        //         array_push($subchids,$subchid);
        //     }
        // }
        // $modsubchids = $param


        // $modsubchids = join("+",$subchids);
        $params['action'] = "multiconfig";
        $params['chid'] = $this->chid;
        // $params['m_subchid'] = join("+",$subchids); //渠道对应的subchid，多个渠道用+号连接
        $params['m_subchid'] = $ps[0]; //渠道对应的subchid，多个渠道用+号连接 这里在python直接处理好
        $params['m_function'] = 13; // 预下载
        $params['pred'] = 1; // 是否开启预下载 （0：否 1：是），function=13时必须

        print_r("预下载请求参数"."\n");
        print_r($params);

        $cdata['data'] = curl::composeencdata($params, $this->rsa_public_key);

        $ret = curl::curlpost("http://lbapi.loveota.com/lbota.php", $cdata);
        $ret_decode = json_decode($ret, true);
        print_r("预下载请求返回"."\n");

        print_r($ret_decode);
        if (file_exists('yuxiazai.txt')){
            unlink('yuxiazai.txt');
        }
        
    }
    public function release($ps){
        // $fs = fopen('shangxian.txt','r') or $this->STOP("上线文件不存在");
        // $rawstr = fread($fs,filesize("shangxian.txt"));
        // fclose($fs);

        // $chids = explode("\n",$rawstr);
        // $subchids = array();
        // for ($i=0; $i < count($chids); $i++) { 
        //     if ($chids[$i] != "") {
        //         $tmp = explode(" ",$chids[$i]);
                
        //         $subchid = $tmp[0];
        //         $ClientChId = $tmp[1];
        //         array_push($subchids,$subchid);
        //     }
        // }
        // $modsubchids = join("+",$subchids);

        $params['action'] = "multiconfig";
        $params['chid'] = $this->chid;
        $params['m_subchid'] = $ps[0]; //渠道对应的subchid，多个渠道用+号连接 这里在python直接处理好
        $params['m_function'] = 50; // 上线
        $params['release'] = 1; // 上下线操作 （0：下线 1：上线），function=50时必须

        print_r("上线请求参数"."\n");
        print_r($params);

        $cdata['data'] = curl::composeencdata($params, $this->rsa_public_key);
    
        $ret = curl::curlpost("http://lbapi.loveota.com/lbota.php", $cdata);
        $ret_decode = json_decode($ret, true);
        print_r("上线请求返回"."\n");

        print_r($ret_decode);
        // if (file_exists('yuxiazai.txt')){
        //     unlink('yuxiazai.txt');
        // }
        // if (file_exists('shangxian.txt')){
        //     unlink('shangxian.txt');
        // }

    }
    public function STOP($msg){
        print_r("ERRORERRORERRORERRORERRORERRORERRORERRORERRORERRORERROR\n");
        print_r("ERRORERRORERRORERRORERRORERRORERRORERRORERRORERRORERROR\n");
        print_r("ERRORERRORERRORERRORERRORERRORERRORERRORERRORERRORERROR\n");
        print_r("ERRORERRORERRORERRORERRORERRORERRORERRORERRORERRORERROR\n");
        throw new Exception($msg);
    }
    public function uploadfile($apkpath,$part){
        return $this->chmgr->uploadfile($this->chid,$apkpath,$part);
    }
}

?>