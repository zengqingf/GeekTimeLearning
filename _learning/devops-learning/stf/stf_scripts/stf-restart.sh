#!/bin/bash
 
#注意：只作为服务器不需要启动
#echo "`docker start adbd`已启动......."

echo "`docker stop rethinkdb`已关闭......"
echo "`docker start rethinkdb`已启动......."

echo "`docker stop stf`已关闭......"
echo "`docker start stf`已启动......."
 
# 展示已启动的容器
docker ps
