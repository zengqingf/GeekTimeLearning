# VisualStudio下载和安装

* 第一种方法

  通过Unity 2019 下载器 下载

* 第二种方法

  [访问系统要求页面](https://docs.microsoft.com/zh-cn/visualstudio/releases/2019/system-requirements)

  在页面中选择 Download Community 2019 
  下载内容 为 vs_Community.exe

  

  而不是在 https://visualstudio.microsoft.com/zh-hans/vs/ 这里下载的
  vs_community__1682973011.1606719598.exe



---



* 安装

  ``` pow
  ### Unity2019 下载器
  # Downloading https://go.microsoft.com/fwlink/?linkid=2086755
  # Installing Microsoft Visual Studio Community 2019...
  
  Execute: "C:\Users\tenmove\Downloads\vs_community.exe" --productId "Microsoft.VisualStudio.Product.Community" --add "Microsoft.VisualStudio.Workload.ManagedGame" --add "Microsoft.VisualStudio.Workload.NativeDesktop" --add "Microsoft.VisualStudio.Component.VC.Tools.x86.x64" --add "Microsoft.VisualStudio.Component.Windows10SDK.16299.Desktop" --campaign "Unity3d_Unity" --passive --norestart --wait
  
  ```

  

---



* iso包制作

  [Visual Studio 2019 离线安装包(.iso)制作](https://www.jianshu.com/p/16d064a9fbdc)



---



* VA

  * 快捷键

    ``` text
    Ctrl + Home // 跳到文档最前面
    Ctrl + End 　// 跳到文档最后面
    Home 　　 // 跳到一行最前面
    End 　　　　 // 跳到一行最后面
    
    Alt + G： 在定义与声明之间互跳。
    
    Alt + O： 在.h与.cpp之间互跳。（O是字母O，不是数字零）
    
    Alt + Shift + Q：鼠标定位到函数名上，若是在h文件中，按此快捷键会弹出右键菜单，
    里面有个选项–创建定义；若是在cpp文件中，则按此快捷键会弹出
    右键菜单，里面有一个选项–创建声明。
    
    这在定义好接口之后，再来写实现时，配合Alt+O是非常快捷的。
    这种情况下，鼠标右击与Alt+O配合会更快。
    
    Alt + Shift + R：当想改掉一个类名或是其他东西的命名时，可能已经有很多地方引用这个名称了，
    这时按下此快捷键，可以很方便的辅助你重命名。
    
    Alt + Shift + S：方便你寻找某个对象或变量等等。
    
    Alt + Shift + O：定位文件。
    
    Alt + Shift + F：光标放到某个字符串上，按下此键，会找出所有引用了这个字符串的地方。
    ```

  * 技巧

    [Visual Assist 特性和技巧 (2017)](https://zhuanlan.zhihu.com/p/26643499)