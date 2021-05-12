#!/bin/bash
echo "`docker stop spug`已关闭......"
spug_contain_id=`docker ps -a --filter name=spug -q`
echo "`docker rm $spug_contain_id`容器已删除"

if [ ! -f "./spug-setup-run-linux-centos7.sh" ]; then
	echo "找不到spug重新启动脚本"
	exit 1
fi
if [ ! -x "./spug-setup-run-linux-centos7.sh" ]; then
   chmod +x "./spug-setup-run-linux-centos7.sh"
fi
sh spug-setup-run-linux-centos7.sh
