<?php
system('echo \"123456\" | sudo -S shutdown -r now',$result);
print $result;//输出命令的结果状态码
?>