docker pull registry.aliyuncs.com/openspug/spug

docker stop spug
docker rm spug

# need make dir "/Docker_TM/spug" and chmod 777 

docker run -d --restart=always --name=spug -p 8000:80 -v /Docker_TM/spug/:/data registry.aliyuncs.com/openspug/spug

# add new account
#  docker exec spug init_spug tengmu 123456
#  docker restart spug

