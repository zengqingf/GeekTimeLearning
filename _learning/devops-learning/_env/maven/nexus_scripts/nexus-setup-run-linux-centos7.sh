#!/bin/bash

#检查docker是否安装
#docker version
systemctl start docker

if [ $? != 0 ]; then
    #docker安装-安装软件包
    yum install -y yum-utils device-mapper-persistent-data lvm2

    #docker安装-设置稳定的仓库
    yum-config-manager --add-repo https://download.docker.com/linux/centos/docker-ce.repo

    #docker安装---列出并排序存储库中可用的docker版本
    yum list docker-ce --showduplicates | sort -r

    #docker安装---通过其完整的软件包名称安装特定版本
    echo "Please enter the version number of docker(e.g. 19.03.5):"
    read version
    echo "准备安装docker.........."
    yum install docker-ce-$version docker-ce-cli-$version containerd.io
    echo "docker安装完毕.........."

    #docker安装---查看当前docker的版本信息
    echo "---------- docker versionInfo ----------"
    docker version
    echo "----------//docker versionInfo ----------"
else 
	echo "docker已安装.........."
fi

#docker安装---启动docker
systemctl start docker
echo "docker已启动.........."

function dockerpull_by_nametag_with_checkexist()
{
	image_name_tag=$1
	image_id=`docker images -q $image_name_tag`
	if [ x"$image_id" == x"" ]; then
		echo "拉取$image_name_tag镜像......"
		docker pull $image_name_tag
	else 
		echo "$image_name_tag镜像已存在......"
	fi
}

dockerpull_by_nametag_with_checkexist sonatype/nexus3

function check_docker_run_status_by_containername()
{
	container_name=$1
	echo "检查$container_name容器启动状态......"
	container_run_id=`docker ps --filter name=$container_name -q`
	if [ x"$container_run_id" == x"" ]; then
		echo "$container_name容器未启动......"

		echo "检查$container_name容器ID挂载状态......"
		container_exist_id=`docker ps -a --filter name=$container_name -q`
		if [ x"$container_exist_id" != x"" ]; then
			echo "$container_name容器ID已挂载 准备删除......"
			docker rm $container_exist_id
		else
			echo "$container_name容器ID未挂载......"
		fi

		return 1
	else
		echo "$container_name容器已启动......"
		return 0
	fi
}

check_docker_run_status_by_containername nexus
if [ $? == 1 ]; then
#启动nexus
echo "正在启动nexus服务......"
docker run -d -p 8081:8081 --name nexus -v /root/nexus-data:/var/nexus-data --restart=always sonatype/nexus3
echo "启动nexus服务完成......"
fi

docker ps

