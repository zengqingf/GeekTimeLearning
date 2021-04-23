i#!/bin/bash

echo "准备关闭stf provider 端口 5037"
stf_port5037_pid=`ps -ef | grep 5037 | grep stf | awk '{print $2}'`
echo "`kill -s 9 ${stf_port5037_pid}`占用端口5037的进程${stf_port5037_pid}已关闭......"

echo "`docker stop stf`已关闭......"
stf_contain_id=`docker ps -a --filter name=stf -q`
echo "`docker rm $stf_contain_id`容器已删除"

stf_image_id=`docker images | grep devicefarmer/stf | awk '{print $3}'`
echo "`docker rmi $stf_image_id`镜像已删除"

if [ ! -f "./stf-setup-run-linux-centos7.sh" ]; then
	echo "找不到stf重新启动脚本"
	exit 1
fi
if [ ! -x "./stf-setup-run-linux-centos7.sh" ]; then
   chmod +x "./stf-setup-run-linux-centos7.sh"
fi
sh stf-setup-run-linux-centos7.sh
