# iOS



### 更新扩展

* 全面屏刘海屏

  [How to Add iPhoneX Launch Image](https://stackoverflow.com/questions/46191522/how-to-add-iphonex-launch-image?noredirect=1&lq=1)

  ``` tex
  add the following json object to Contents.json file which on LaunchImage.launchimage folder in your old project。Once Xcode refreshes, just drop in a 1125×2436px image. If you need landscape, you can add another with the orientation.
  	{
        "extent" : "full-screen",
        "idiom" : "iphone",
        "subtype" : "2436h",
        "minimum-system-version" : "11.0",
        "orientation" : "portrait",
        "scale" : "3x"
      }
  ```

  



---



### 命令行

* xcodebuild

  * 问题1

    ``` tex
    Xcode脚本自动化打包问题：xcrun: error: unable to find utility "PackageApplication", not a developer tool or in PATH
    
    拷贝PackageApplication到
    /Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/Developer/usr/bin/
    
    再执行
    sudo xcode-select -switch /Applications/Xcode.app/Contents/Developer/
    
    chmod +x /Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/Developer/usr/bin/PackageApplication
    ```

    