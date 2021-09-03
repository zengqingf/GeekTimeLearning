<?php 
include 'index_android_download_urls.php';
if(isset($_POST['action']))
{
    switch ($_POST['action'])
    {
        case "reqOnlineUrlBtn":
            alert("222");
            reqOnlineUrlBtnClick();
            break;
    }
}

function reqOnlineUrlBtnClick()
{
    alert("333");
    $data = file_get_contents("http://39.108.138.140:58888/config.js");
    $data_array = explode('=', $data);
    $invalidurls = array();
    if(!empty($data_array) && count($data_array) == 2)
    {
        $data_main = $data_array[1];
        $infos = json_decode($data_main, true);
        foreach ($infos as $key => $value) {
            $channel = $key;
            $url = $value["url"];
            $version = $value["version"];
            if(!file_contents_exist($url))
            {
                $invalidurls[] = "渠道：.$channel. 版本号：.$version. 链接：.$url. 失效了！";
            }
        }
    }
    $invalidInfos = "线上链接正常";
    if(!empty($invalidurls))
    {
        for ($i = 0; $i < count($invalidurls); $i++){
            $invalidInfos .= $invalidurls[$i]. "\n";
        }
    }
    echo $invalidInfos;
}

?>
