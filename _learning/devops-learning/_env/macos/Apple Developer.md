# Apple Developer

* 证书

  [Apple PKI](https://www.apple.com/certificateauthority/)

  ![](https://i.loli.net/2021/05/10/2PoCgAOci8EQb6H.jpg)

  **测试时间2021-5-10**

  [Apple开发证书和发布证书不受信任问题](https://blog.csdn.net/Dancen/article/details/114406431)

  ``` text
  mac 钥匙串中新增的证书 显示不受信任
  
  目前的Apple全球开发者关系认证中间证书WWDRCA（也就是我们上面下载的那个）将于2023年2月7日到期。Apple发布了新的WWDRCA，更新后的证书过期时间为2030年2月20日，新证书将用于签署2021年1月28日后为苹果开发者项目颁发的新软件签名证书。
  
  解决：
  安装WWDR3 （AppleWWDRCAG3.cer）
  ```

  [WDRC 证书一定要在系统钥匙串](https://juejin.cn/post/6844903728324018190)

  ``` text
  q:error: Signing certificate is invalid. Signing certificate "iPhone Distribution: ******", serial number "******", is not valid for code signing. It may have been revoked or expired. (in target '*****')
  
  解决：
  保留登录的 AppleWWDRCAG.cer 
  删原先的 系统的 AppleWWDRCAG.cer 
  安装 AppleWWDRCAG.cer 到系统
  新下载的 AppleWWDRCAG3.cer 安装到登录和系统
  
  注意：
  1. 证书过期后添加 需要删掉过期的
  2. 证书需要移动位置（如从登录移动到系统，需要删掉原来的，重新安装）
  ```

  