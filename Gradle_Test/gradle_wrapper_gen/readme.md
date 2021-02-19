* 在项目根目录 执行 'gradle wrapper' 
* 接上一条，指定使用Gradle版本 'gradle wrapper --gradle-version <Gradle 版本>'  （相当于修改 gralde-wrapper.properties 中的 distributionUrl ）
* 接上一条， distributionUrl 默认格式是 'https\://services.gradle.org/distributions/gradle-${gradleVersion}-bin.zip'  可以将bin替换为all 这样可以看到Gradle源码
* 如果执行 ./gradlew 时，卡了，可以修改 distributionUrl 为别的镜像地址
* gralde-wrapper.properties 配置含义：从distributionUrl 下载gradle-xxx-yyy.zip存放到 zipStoreBase + zipStorePath 目录下，然后安装到 distributionBase + distributionPath 目录下
* distributionBase， zipStoreBase 都可以取两个值：GRADLE_USER_HOME 或者 PROJECT
    > 如果取值为PROJECT，则计算路径是 基于project目录来计算。
    > 如果取值为GRADLE_USER_HOME，则计算路径是基于 用户的 home目录来计算的。(windows操作系统，默认基于 c:\users\<user_home>\.gradle |  对于linux 系统，默认基于 $USER_HOME\.gradle )

---

* 如果想要使用你已经安装的gradle，怎么办呢？ [Gradle: Gradle Wrapper](https://www.cnblogs.com/f1194361820/p/9121898.html)
    ``` text
    这个在使用gradle-wrapper时是行不通的，除非你不使用gradle-wrapper来构建
    如何使用已经离线的下载的zip安装包呢？
    修改gradle-wrapper.properties中配置项为本地URL即可。

    例如我之前通过在线方式安装了gradle-4.7-bin.zip，
    我拿到该zip文件，copy一份放到D盘，修改名称为：gradle-4.7-bin-test.zip，
    然后修改gradle-wrapper.properties中配置项为：distributionUrl=file\:///d:/gradle-4.7-bin-test.zip
    然后使用gradlew 执行一个task，就自动安装好了。
    ``


---

* eg...
.\gradlew.bat == .\gradlew
    1. 查看所有可执行的Tasks:   '.\gradlew.bat -q :tasks --all'  ==  '.\gradlew.bat tasks' == '.\gradlew.bat :tasks'
    2. Gradle Help 帮助信息:   '.\gradlew.bat help --tasks 任务名' 如 '.\gradlew.bat help --tasks tasks' 显示tasks 任务的帮助信息
    3. 强制刷新依赖： '.\gradlew.bat --refresh-dependencies assemble'  即默认缓存不会清理 强制刷新可能清缓存重新下载依赖
    4. 多任务调用  '.\gradlew.bat clean jar'  先清理class文件 再生成jar
    5. 调用任务缩写 '.\gradlew connectCheck' == '.\gradlew cc'