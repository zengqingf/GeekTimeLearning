<?php

$agent = isset($_SERVER['HTTP_USER_AGENT']) ? $_SERVER['HTTP_USER_AGENT'] : "";
//echo $agent;
ini_set('user_agent', $agent);
$file = file_get_contents("http://192.168.2.147/dnl/index.php");
echo $file;

?>