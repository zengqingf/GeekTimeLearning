# VisualStduio下载和安装

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