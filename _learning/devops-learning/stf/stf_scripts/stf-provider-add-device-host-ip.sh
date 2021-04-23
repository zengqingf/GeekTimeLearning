#!/bin/bash

IP=`hostname -I | awk '{print $1}'`
echo $IP

echo "开启STF远程主机连接设备......"
echo "如果连接成功，不要关闭这个命令窗口"
TargetIP=$1
if [ x"$TargetIP" == x"" ]; then
	echo "没有输入目标主机IP"
	exit 1
fi
echo "目标主机IP: ${TargetIP}, Port: 5037"
stf_contain_id=`docker ps --filter name=stf -q`
if [ x"$stf_contain_id" == x"" ]; then
	echo "stf没有启动"
	exit 1
fi
docker exec -it ${stf_contain_id} /bin/sh -c "stf provider --name `hostname` --min-port 7400 --max-port 7700 --connect-sub tcp://$IP:7114 --connect-push tcp://$IP:7116 --group-timeout 900 --public-ip $IP --storage-url http://$IP:7100/ --adb-host ${TargetIP} --adb-port 5037 --vnc-initial-size 600x800 --mute-master never --allow-remote"
