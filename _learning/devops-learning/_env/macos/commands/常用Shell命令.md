# 常用Shell命令

## macOS环境

1. 获取mac地址

   ``` shell
   ifconfig en0 ether | grep ether  | awk '{print $2}'
   ```

2. 获取ip

   ``` shell
   ifconfig en0 inet | grep inet | awk '{print $2}'
   ```

3. 树形结构展示

   ``` shell
   tree --help
   ```

   