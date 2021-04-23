# Docker for Linux

### 安装

* CentOS

  [CentOS Docker 安装](https://www.runoob.com/docker/centos-docker-install.html)

  ``` shell
  #安装后需要启动docker service
  #否则会报错：Cannot connect to the Docker daemon. Is the docker daemon running on this host?
  service docker start
  ```

  ``` shell
  #docker安装 启动 重启 
  sudo yum update
  sudo yum install docker
  
  su root # 先切换到root用户, 再执行以下命令
  systemctl enable docker # 开机自动启动docker
  systemctl start docker # 启动docker
  systemctl restart docker # 重启dokcer
  ```

  



---



### 基础

* docker基础

  ``` text
  镜像：运行容器的前提，静态只读文件
  容器：镜像的一个运行实例，带有运行时需要的可写文件层，容器中的应用进程处理运行状态
  仓库：集中存放镜像
  
  ---
  在运行一个容器前需要本地有对应的镜像
  如果镜像不存在，Docker会尝试从默认镜像仓库下载（默认使用Docker Hub公共注册服务器中的仓库）
  用户也可以通过配置，使用自定义的镜像仓库
  ---
  
  ```

  

* docker命令

  * 拉取镜像

    ``` shell
    #NAME是镜像仓库名称（用来区分镜像）, TAG是镜像的标签（往往用来表示版本信息）
    #通常情况下，描述一个镜像需要包括“名称+标签”信息
    docker pull NAME[:TAG]
    
    #拉取所有tagged镜像
    docker pull -a NAME[:TAG]
    
    #拉取非官方仓库的镜像
    docker pull 仓库地址（registry/注册服务器）/public/NAME[:TAG]
    ```

    

  * 查看拉好的镜像

    ``` shell
    docker images
    ```

  * 启动所有容器

    ``` shell
    docker start $(docker ps -a | awk '{ print $1}' | tail -n +2);
    ```

  * 关闭所有容器

    ``` shell
    docker stop $(docker ps -a | awk '{ print $1}' | tail -n +2);
    ```

  * 删除所有容器

    ``` shell
    docker rm $(docker ps -a | awk '{ print $1}' | tail -n +2);
    # docker rm  镜像ID  (推荐在删除时使用镜像ID)
    ```

  * 删除所有镜像

    ``` shell
    docker rmi $(docker images | awk '{print $3}' |tail -n +2);
    ```

  * 查看启动的容器

    ``` shell
    docker ps
    docker ps -a #所有容器
    ```

  * 查看log

    ``` shell
    docker logs 实例名或ID
    ```

  * 通过镜像保存/加载镜像

    ``` shell
    #保存本地拉好的镜像到本地其他目录
    docker save 镜像ID(imageID) > /本地目录/镜像名.tar
    
    docker save -o /本地目录/镜像名.tar 镜像ID
    
    #可以打包多个Image
    docker save -o images.tar 镜像1:Tag 镜像2:Tag ...
    
    #通过image保存的镜像会保存操作历史，可以回滚到历史版本
    
    
    #加载镜像到doctor
    docker load < /本地目录/镜像名.tar
    docker load -i /本地目录/镜像名.tar
    docker load -input /本地目录/镜像名.tar
    
    #加载成功后使用docker imgaes查看
    #输出结果  需要修改镜像标签
    #REPOOSITORY           TAG              IMAGE ID			CREATED				SIZE
    #   none                none               镜像ID             xxx ago             xxxGB
    ```

  * 修改本机镜像标签

    ``` shell
    docker tag  镜像ID    镜像名:latest
    ```

  * 通过容器导入/导出镜像

    ``` shell
    #查看全部容器
    docker ps -a 
    docker export containID > filename    #文件可能存放在docker终端目录下
    
    docker import filename [newname]      #通过docker images查看导入的镜像
    
    #通过容器保存的镜像不会保存操作历史
    
    #通过容器加载的镜像，需要在运行时加上相关命令
    #-i 交互式操作  -t终端
    docker run -it 镜像名:container /bin/bash
    ```

  * 通过镜像和通过容器的区别

    [Docker - 实现本地镜像的导出、导入（export、import、save、load）](https://www.hangge.com/blog/cache/detail_2411.html)

    ``` text
    特别注意：两种方法不可混用。
    如果使用 import 导入 save 产生的文件，虽然导入不提示错误，但是启动容器时会提示失败，会出现类似"docker: Error response from daemon: Container command not found or does not exist"的错误。
    
    1.export 导出的镜像文件体积小于 save 保存的镜像
    2.	docker import 可以为镜像指定新名称
    	docker load 不能对载入的镜像重命名
    	
    3.docker export 不支持同时将多个镜像打包到一个文件中
    	docker save 支持
    
    4.export 导出（import 导入）是根据容器拿到的镜像，再导入时会丢失镜像所有的历史记录和元数据信息（即仅保存容器当时的快照状态），
    	所以无法进行回滚操作。
        而 save 保存（load 加载）的镜像，没有丢失镜像的历史，可以回滚到之前的层（layer）。
        
    5.docker export 的应用场景：主要用来制作基础镜像，比如我们从一个 ubuntu 镜像启动一个容器，然后安装一些软件和进行一些设置后，
    	使用 docker export 保存为一个基础镜像。然后，把这个镜像分发给其他人使用，比如作为基础的开发环境。
    	
      docker save 的应用场景：如果我们的应用是使用 docker-compose.yml 编排的多个镜像组合，但我们要部署的客户服务器并不能连外网。这时就可以	使用 docker save 将用到的镜像打个包，然后拷贝到客户服务器上使用 docker load 载入。
    ```


  * docker ps 显示指定列（过滤）

    ``` shell
    docker ps --format "table {{.ID}}\t{{.Names}}\t{{.Ports}}"
    
    .ID	容器ID
    .Image	镜像ID
    .Command	执行的命令
    .CreatedAt	容器创建时间
    .RunningFor	运行时长
    .Ports	暴露的端口
    .Status	容器状态
    .Names	容器名称
    .Label	分配给容器的所有标签
    .Mounts	容器挂载的卷
    .Networks	容器所用的网络名称
    
    docker ps --filter id=a1b2c3 --filter name=stf
    docker ps --filter status=paused / running / created / restarting / removing / exited / dead
    
    docker ps -f before=9c3527ed70ce
    docker ps -f since=6e63f6ff38b0
    ```

  * docker --privileged[=true/false]

    ``` text
    使用该参数，container内的root拥有真正的root权限
    否则，container内的root只是外部的一个普通用户权限
    privileged启动的容器，可以看到很多host上的设备，并且可以执行mount
    甚至允许你在docker容器中启动docker容器
    ```

  * docker exec

    ``` shell
    # 执行多个命令
    
    #1
    docker exec -it 运行中的容器的ID /bin/sh -c "其他命令"
    docker run 运行中的容器ID sh -c "其他命令"
    
    #2
    sudo docker exec myContainer bash -c "cd /home/myuser/myproject && git fetch ssh://gerrit_server:29418/myparent/myproject ${GERRIT_REFSPEC} && git checkout FETCH_HEAD";
    sudo docker exec myContainer bash -c "cd /home/myuser/myproject;git fetch ssh://gerrit_server:29418/myparent/myproject ${GERRIT_REFSPEC};git checkout FETCH_HEAD";
    ```

  * docker 查看容器信息

    ``` shell
    docker inspect 容器名  
    #docker inspect stf
    ```

* docker修改容器后重新做成新镜像

  ``` shell
  #进入容器内部修改 (root)
  docker exec -it -u root 容器ID /bin/bash
  
  #安装vim
  apt-get update
  apt-get install vim
  
  #修改方法1
  #拷贝出来改
  #将容器中的文件拷贝出来
  sudo docker cp 容器ID:/etc/mysql/my.cnf /home/tom/
  #将容器中的文件拷贝回去
  sudo docker cp /home/tom/my.cnf  容器ID:/etc/mysql/
  
  #修改方式2
  #启动容器时 指定外部文件夹挂载（映射）（可能挂不出来，但没有试过root权限）
  #冒号前是本地路径（需要绝对路径），冒号后是容器中的路径
  $ docker run --name stf -v /srv/stf:/app ...
  
  #保存修改后的镜像
  #离开镜像bash
  exit
  docker commit 容器ID 新容器名 (格式：devicefarmer/stf_vim_changed)
  ```

  



---



### 扩展

* docker 制作镜像

  [docker 制作自己的镜像](https://www.cnblogs.com/pjcd-32718195/p/11762079.html)