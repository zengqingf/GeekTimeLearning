# Java编译环境

* JDK vs JRE

  [What is the difference between JDK and JRE?](https://stackoverflow.com/questions/1906445/what-is-the-difference-between-jdk-and-jre)

  ``` text
  JRE : Jave Runtime Environment  
  JDK : Java Developmenet Kit
  JDK 包含 JRE
  
  JDK = JRE + Development/debugging tools
  JRE = JVM + Java Packages Classes(like util, math, lang, awt,swing etc)+runtime libraries.
  JVM = Class loader system + runtime data area + Execution Engine.
  ```

  ![CBNux](CBNux.png)

![AaveN](AaveN.png)



* 不同的JDK安装包

  [https://stackoverflow.com/questions/52431764/difference-between-openjdk-and-adoptium-adoptopenjdk](https://stackoverflow.com/questions/52431764/difference-between-openjdk-and-adoptium-adoptopenjdk)

  * OpenJDK Providers and Comparison

    - **AdoptOpenJDK** - [https://adoptopenjdk.net](https://adoptopenjdk.net/)
    - **Amazon – Corretto** - https://aws.amazon.com/corretto
    - **Azul Zulu** - https://www.azul.com/downloads/zulu/
    - **BellSoft Liberica** - https://bell-sw.com/java.html
    - **IBM** - https://www.ibm.com/developerworks/java/jdk
    - **jClarity** - https://www.jclarity.com/adoptopenjdk-support/
    - **OpenJDK Upstream** - https://adoptopenjdk.net/upstream.html
    - **Oracle JDK** - https://www.oracle.com/technetwork/java/javase/downloads
    - **Oracle OpenJDK** - [http://jdk.java.net](http://jdk.java.net/)
    - **ojdkbuild** - https://github.com/ojdkbuild/ojdkbuild
    - **RedHat** - https://developers.redhat.com/products/openjdk/overview
    - **SapMachine** - https://sap.github.io/SapMachine

    ```
    |     Provider      | Free Builds | Free Binary   | Extended | Commercial | Permissive |
    |                   | from Source | Distributions | Updates  | Support    | License    |
    |--------------------------------------------------------------------------------------|
    | AdoptOpenJDK      |    Yes      |    Yes        |   Yes    |   No       |   Yes      |
    | Amazon – Corretto |    Yes      |    Yes        |   Yes    |   No       |   Yes      |
    | Azul Zulu         |    No       |    Yes        |   Yes    |   Yes      |   Yes      |
    | BellSoft Liberica |    No       |    Yes        |   Yes    |   Yes      |   Yes      |
    | IBM               |    No       |    No         |   Yes    |   Yes      |   Yes      |
    | jClarity          |    No       |    No         |   Yes    |   Yes      |   Yes      |
    | OpenJDK           |    Yes      |    Yes        |   Yes    |   No       |   Yes      |
    | Oracle JDK        |    No       |    Yes        |   No**   |   Yes      |   No       |
    | Oracle OpenJDK    |    Yes      |    Yes        |   No     |   No       |   Yes      |
    | ojdkbuild         |    Yes      |    Yes        |   No     |   No       |   Yes      |
    | RedHat            |    Yes      |    Yes        |   Yes    |   Yes      |   Yes      |
    | SapMachine        |    Yes      |    Yes        |   Yes    |   Yes      |   Yes      |
    ```



---



* 编译java

  [命令行编译和执行Java工程](https://www.jianshu.com/p/e42dc0652b6d)

  [第1期：抛开IDE，了解一下javac如何编译](https://zhuanlan.zhihu.com/p/74229762)

  > > 待自己整理文档





---



* Tenmove PC 本地环境

  ``` text
  1. C:\Program Files\AdoptOpenJDK\jdk-11.0.9.101-hotspot
  2. C:\Program Files\Android\jdk\microsoft_dist_openjdk_1.8.0.25
  3. C:\Program Files\Java
  ```

  