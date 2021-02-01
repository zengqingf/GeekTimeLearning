# Tenmove - STF API Reference

[官方API地址：github - openstf/stf/doc](https://github.com/openstf/stf/blob/master/doc/API.md#delete-userdevicesserial)

### 环境

* stf 网页：192.168.2.168:7100

* jenkins网页登陆：
  账号：jenkins
  邮箱：jenkins@tenmove.com

* jenkins 账号token:
  652325f2e60243f796c6e03f32d69447f5abd047804f4b2c8622a188668a3293

  在内网ftp 192.168.2.148/__info/stf中

* 测试设备 荣耀V9  DUK-AL20
  序列号 serial:   6EB0217808004166



---



1. 占用设备

   ``` shell
   curl -X POST --header "Content-Type: application/json" --data '{"serial":"EP7351U3WQ"}' -H "Authorization: Bearer YOUR-TOKEN-HERE" https://stf.example.org/api/v1/user/devices
   
   
   e.g.	curl -X POST --header "Content-Type: application/json" --data '{"serial":"6EB0217808004166"}' -H "Authorization: Bearer 652325f2e60243f796c6e03f32d69447f5abd047804f4b2c8622a188668a3293" http://192.168.2.168:7100/api/v1/user/devices
   ```

2. 释放设备

   ``` shell
   curl -X DELETE -H "Authorization: Bearer YOUR-TOKEN-HERE" https://stf.example.org/api/v1/user/devices/{serial}
   
   
   e.g.	curl -X DELETE -H "Authorization: Bearer 652325f2e60243f796c6e03f32d69447f5abd047804f4b2c8622a188668a3293" http://192.168.2.168:7100/api/v1/user/devices/6EB0217808004166
   ```

   

