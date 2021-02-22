@echo

REM 本地环境： “E:\ws\mjx\base_learning\GeekTimeLearning\Java_Test\_command_projects\oop-module\”
REM    JDK   :   C:\Program Files\AdoptOpenJDK\jdk-11.0.9.101-hotspot\bin

REM 将bin目录下的所有class 先打包成 jar  注意 -C后的路径 最后需要指定当前路径 用 " ." 或者 " .\." 等表示当前路径的方式即可
.\jar --create --file E:\ws\mjx\base_learning\GeekTimeLearning\Java_Test\_command_projects\oop-module\hello.jar --main-class com.itranswarp.sample.Main -C E:\ws\mjx\base_learning\GeekTimeLearning\Java_Test\_command_projects\oop-module\bin .

REM 创建模块
.\jmod create --class-path E:\ws\mjx\base_learning\GeekTimeLearning\Java_Test\_command_projects\oop-module\hello.jar E:\ws\mjx\base_learning\GeekTimeLearning\Java_Test\_command_projects\oop-module\hello.jmod

REM 运行模块 注意--module-path后 不能是 .jmod  会报错 JMOD format not supported at execution
.\java --module-path E:\ws\mjx\base_learning\GeekTimeLearning\Java_Test\_command_projects\oop-module\hello.jar --module hello.world

REM 打包Jre

.\jlink --module-path E:\ws\mjx\base_learning\GeekTimeLearning\Java_Test\_command_projects\oop-module\hello.jmod --add-modules java.base,java.xml,hello.world --output E:\ws\mjx\base_learning\GeekTimeLearning\Java_Test\_command_projects\oop-module\jre\

REM 测试运行Jre
REM E:\ws\mjx\base_learning\GeekTimeLearning\Java_Test\_command_projects\oop-module\jre\bin\java --module hello.world
REM output : Hello, xml!

REM 模块间的访问权限：将需要导出的类 exports
REM module hello.world {
REM 	exports com.itranswarp.sample;
	
REM 	requires java.base;
REM 	requires java.xml;
REM }
REM

pause