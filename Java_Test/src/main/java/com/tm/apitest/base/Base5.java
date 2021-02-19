/*
* 包 ： 命令空间
* 一个类的完整类名是 ： 包名.类型
* JVM只看完整包名，只要包名不同，类就不同
*
* 包是多层结构， 用 . 分割
*
* ！！！ 要特别注意：包没有父子关系。java.util和java.util.zip是不同的包，两者没有任何继承关系。
*
* 所有Java文件对应的目录层次要和包的层次一致
* 例：
* package_sample/src/hong/Person.java
* package_sample/src/ming/Person.java
* package_sample/src/mr/jun/Array.java
*
* 需要编译的结果：
 * package_sample/bin/hong/Person.class
 * package_sample/bin/ming/Person.class
 * package_sample/bin/mr/jun/Array.class
*
* 执行操作如下：
* cd package_sample/src
* javac -d ../bin ming/Person.java hong/Person.java mr/jun/Array.json
*
*
* import 方法 导入包
*
* 1. 直接写完整类名
*
* 2. 不推荐写法
* 在写import的时候，可以使用*，表示把这个包下面的所有class都导入进来（但不包括子包的class）
* 不推荐写法
*
* 3. import static 导入一个类的静态字段和静态方法
*
*
* Java编译器最终编译的.class 文件只使用 完整类名
*
* 如果是完整类名，就直接根据完整类名查找这个class；
  如果是简单类名，按下面的顺序依次查找：
     查找当前package是否存在这个class
     查找import的包是否包含这个class
     查找java.lang包是否包含这个class
     *
     *
  编写class的时候，编译器会自动帮我们做两个import动作：
  * 默认自动import当前package的其他class；
  * 默认自动import java.lang.*。

* ！！！ 注意 自动导入的是java.lang包， 类似 java.lang.reflect 等包需要手动导入
*
*
* 如果有两个class名称相同，例如，mr.jun.Arrays和java.util.Arrays，那么只能import其中一个，另一个必须写完整类名。

* 为了避免名字冲突，我们需要确定唯一的包名。推荐的做法是使用倒置的域名来确保唯一性。
* 子包就可以根据功能自行命名。
* 要注意不要和java.lang包的类重名，即自己的类不要使用这些名字：
* 要注意也不要和JDK常用类重名：
 * */


package com.tm.apitest.base;

import static java.lang.System.*;

//类不能用private 和 protected 修饰
class Base5 {

    //包的作用域
    void testField() {
        System.out.println("测试作用域");

        out.println("省略了 System 包名");
    }

    private void testField2() {
        out.println("测试作用域2");
    }

    protected void testField3() {
        out.println("测试作用域3");
    }

    public void testField4() {
        out.println("测试作用域4");
    }
}
