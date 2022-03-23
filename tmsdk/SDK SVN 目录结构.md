# 新 SDK SVN 目录结构

**目录均为小写，不包含中文**

* doc(维护文档和注意事项)

* script（存放持续集成和自动化构建脚本）
  * 功能
    * readme      （功能说明）   ---> （功能说明和修改说明 可以合成一个 看内容量）
    * changelog （修改说明）
    * *.py /*.sh / *.bat
* env (存放构建环境安装包)
  * 如python3
    * win / linux / mac
      * versions (安装包版本目录)
      * config script （自启动脚本和安装配置脚本等）
      * depend files (依赖项 ： 安装包/安装脚本/说明等)
  * confscript（配置机器环境的自动化脚本）

* projects_sdk
  * android/unity/ue4/ios （平台）
    * test（测试工程用的目录）
    * proj（工程目录）

* projects_tool
  * 工具工程（录像）
* thirdparty
  * 插件功能
    * versions
    * readme
    * changelog
    * 针对项目
      * 针对项目的key等信息

* sec

  * sign  (签名文件)
  * key  (密钥文件 公钥密钥)
  * password  (密码文件)
* save

  * 各种杂类文件