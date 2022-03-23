<?php
header("Content-Type:text/html; charset=utf-8");


// echo dirname(__FILE__);
// echo "1111111111111111111111111111";
include dirname(__FILE__).'/example/lbinterface.class.php';

// print_r($_SERVER);
// // var_dump($_SERVER);
// print_r(gethostbyname($_SERVER['COMPUTERNAME']));
// throw new Exception(count($_SERVER)."\n");

$argv_action = $argv[1];
$argv_account = $argv[2];
$argv_password = $argv[3];
$param = array();
if ($argc > 4) {
    $param = array_slice($argv,4);
}
$interface = new lbinterface($argv_account,$argv_password);


echo "\n";
print_r("\$interface->$argv_action(\$param);\n");

eval("\$interface->$argv_action(\$param);");

// eval("print_r(sizeof(\$param));")

// $argv_packpath = $argv[4];
// $argv_md5 = $argv[5];





?>

