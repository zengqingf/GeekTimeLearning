# UE4 UI开发



### UMG

* By C++

  * link

    [UE4 C++ —— UMG和C++交互](https://blog.csdn.net/niu2212035673/article/details/82792910)

    ``` tex
    交互方法
    一，强转子集
    GetRootWidget()        //获取根节点
    GetChildAt()            //获取子节点
    UMG控件呈树状结构，根据根节点可以获取到所有的子节点
    二，反射绑定
    UPROPERTY(Meta = (BindWidget))
    UButton *ButtonOne;
    绑定的类型和名称必须和蓝图内的一致
    三，根据控件名获取
    GetWidgetFromName()
    获取到UWidget*类型，强转成指定类型
    ```

    示例见：TMSDK/Unreal/UE4.25/GCloudTest





* DPI缩放

  UMG窗口，选择分辨率Apple iPhone 5s，并且调整分辨率到640 * 1136，放入一张640*1136的Image，发现Image小于屏幕区域

  ![](UE4 UI开发.assets/202111120951913.png)

  ![](UE4 UI开发.assets/202111120951529.png)

  ``` tex
  调整DPI（Project Settings中User Interface->DPI Scaling）
  DPI Curve 中新增Key，调整Resolution 为 640.0, Scale 为 1.0
  
  可以发现Image和屏幕大小一致了
  ```





* RichTextBlock 富文本实现

  ![](UE4 UI开发.assets/202111181158658.png)
  
  ``` tex
  新建杂项（Miscellaneous）-> Data Table
  Add Row Name即为 标签名
  如上： <n>加油</><red>从现在开始</>
  
  Row Name设置尾Default时，默认输入文本能正常显示，不乱码
  
  通过设置TArray<Decorator Classes> 可以时RichTextBlock支持带图片的文本
  1. 创建新的蓝图类（或者C++类，然后建蓝图继承C++），选择RichTextBlockImageDecorator作为父类。
  	（RichTextBlockImageDecorator类不会出现在父类列表中。相反，您可以将其扩展为C++类。
  	然后，在C++文件中分配数据表资源，或在标题顶部向UCLASS宏添加 可设蓝图（Blueprintable），然后将子类扩展为蓝图。）
  2. 新建DataTable (RichImageRow)，Add Row Name -> 添加Image -> 修改Row Name为 TestImage
  2. 打开1中得到的新蓝图，并将DataTable分配给1中蓝图或C++类的（Image Set） 属性。
  3. 装饰器（Decorator）类设置妥当后，将其添加到UMG中的富文本块 装饰器类数组。然后，使用标记 <img id="TestImage"/> 
  	（其中"TestImage"是您分配的行名称）从数据表插入图像，重新编译控件后，图像就会立即显示出来。
  ```
  
  * ref 
  
    [尝试扩展 UE4 的 Rich Text Block - 类似Unity的RichText用法](https://qiita.com/Naotsun/items/aae717f11b31b53d4804)
  
    [CustomRichText](https://github.com/Naotsun19B/CustomRichText)



* 界面层级

  ``` tex
  处于同一嵌套层级的UI，通过Set ZOrder，可以设置哪个Widget的前后关系
  
  动态创建的子 Widget，通过add child节点而非add to viewport，挂载到指定父widget下，该子widget能显示在父widget之前
  ```



* [UserWidget] - SizeBox

  ``` tex
  注意特点：
  Single Child 只支持一个child
  Fixed Size	可以固定大小 常用于容器内单一Item(Entry)的根节点
  ```




* [UserWidget] - ScaleBox

  ``` tex
  只有一个子元素，并且保证子元素按比例缩放
  子元素需要设置为居中，保证能按比例缩放
  ```

  













---



### 3D UI

* 问题

  * Cylinder collision (widget component) is incorrect for widget interaction component

    ``` c++
    //获取Cylinder WidgetComponent的点击结果
     if (auto WidgetComp = Cast<UWidgetComponent>(HitResult.GetComponent()))
     {
          auto cylinderHitLocation = WidgetComp->GetCylinderHitLocation(HitResult.Location, GetForwardVector());
          // do something using CylinderHitLocation.Key;
     }
    ```




* 相机

  * 移动相机

    ``` tex
    一、创建一个Arrow组件来标记要移动的位置（Arrow的用法之一就是用来标注坐标）。
    二、使用TimeLine时间轴结合插值Lerp来移动相机
    ```

    







---



### UE4 UI优化

* 点击事件

  ``` tex
  Visibility（ESlateVisibility）
  
  Visible					Visible and hit-testable (can interact with cursor). Default value.
  						可见，可点击
  
  Collapsed				Not visible and takes up no space in the layout (obviously not hit-testable).
  						不可见，不占用布局空间
  
  Hidden					Not visible but occupies layout space (obviously not hit-testable).
  						不可见，占用布局空间
  
  HitTestInvisible		Visible but not hit-testable (cannot interact with cursor) and
  						children in the hierarchy (if any) are also not hit-testable.
  						可见，当前和所有Child的Widget均不可点击
  						
  SelfHitTestInvisible	Visible but not hit-testable (cannot interact with cursor) and 
  						doesn't affect hit-testing on children (if any).
  						可见，当前Widget不可点击，Child Widget可点击
  						
  						
  很多 Widget 默认属性是 Visible,需要手动设置成 HitTestInvisible 和 SelfHitTestInvisible。
  如果大量 Widget 设置成 Visible，那么引擎在点击响应时的效率就会大大下降，这也会增加游戏线程的开销。
  
  Collapsed 不占用布局空间（Layout Space），因此在隐藏后不会进行 Prepass 的计算，性能优于 Hidden。
  可以使用	 Widget Reflector 	帮助检查是否有错误设置的 Visibility 属性。
  
  ```
  
  ``` lua
  --屏蔽点击事件向下传递至3D场景
  function FrameViewBase:OnMouseButtonDown(Geometry, MouseEvent)
      return UE4.UWidgetBlueprintLibrary.Handled()
  end
  
  function FrameViewBase:OnMouseButtonUp(Geometry, MouseEvent)
      return UE4.UWidgetBlueprintLibrary.Handled()
  end
  ```
  
  



* 容器选择

  ``` tex
  UE4.15以前，Canvas Panel不参与合批，推荐使用Overlay，但是Overlay不支持子空间的位置和大小直接设置和适配
  UE4.15后支持 同ZOrder的Canvas Panel批次合并
  ```

  ``` tex
  Canvas Panel vs. Overlay
  
  Canvas Panel允许指定部件位置 锚点 覆盖层级(ZOrder)
  			允许放置多个子组件
  			适用于手动设置布局，不适用于程序化生成一个Widget并将其放置于一个容器中（除非需要绝对布局）
  		CanvasPanel底下的部件会有Slot(Canvas Panel Slot) ，
  		CanvasPanel和子部件的关系用这个Slot的属性来设置：
  		1.Anchors: CanvasPanel的锚点，可以分别设置x，y在区间[0,1] 的任意值，表示屏幕的百分比位置。
  		2.PositionX
  		  PositionY : 部件锚点(一般是左上角)相对于屏幕锚点的位置偏移
  		3.SizeX
  		  SizeY: 部件的大小
  		4.Alignment: 对齐方式(相当于部件的锚点)，可以分别设置x，y在区间[0,1] 的任意值，表示部件的百分比位置。
  		5.SizeToContent: 当AutoSize为true时，使用部件的实际大小而无视SizeX,Y。（可看作部件大小的语法糖）
  		6.ZOrder: 设置层级，数字越大的放在上面
  		
          //确定部件位置：1.CanvasPanel锚点 + 2.两个锚点间的位置偏移 + 4.部件锚点
          //确定部件大小：3. 5.
          //确定部件层级：6.
          
  Overlay允许组件一个堆在另一个上
  	   每层使用简单的流式布局
  	   Overlay底下的部件会有Slot(Overlay Slot) ，Overlay和子部件的关系用这个Slot的属性来设置：
            1.Padding
              Left
              Top
              Right
              Bottom
              内边距（Overlay的各边到子部件各边的距离）
            2.Horizontal Alignment
              Horizontal Alignment Left
              Horizontal Alignment Center
              Horizontal Alignment Right
              Horizontal Alignment Fill
              Vertical Alignment
              Vertical Alignment Top
              Vertical Alignment Center
              Vertical Alignment Bottom
              Vertical Alignment Fill
              对齐方式，与Padding的值是叠加的。先计算对齐方式，再加上内边距。
              （Fill会使用Overlay的Size代替部件的实际Size）
              
          //确定部件位置：1.内边距 + 2.对齐方式
          //确定部件大小：无
          //确定部件层级：无
  ```

  ``` tex
  Overlay作用1：
  	动态挂载时，用Overlay作为挂载的根节点，可以设置Overlay的大小，使挂载对象使用填充方式挂载，如图所示
  ```
  
  ![umg_canvaspanel_vs_overlay_02](_pic/umg_canvaspanel_vs_overlay_01.png)
  
  ![umg_canvaspanel_vs_overlay_02](_pic/umg_canvaspanel_vs_overlay_02.png)





* 合批

  ![](UE4 UI开发.assets/202111121005173.png)

  ``` tex
  开启Canvas Panel支持合批的开关
  Project Settings-> Engine -> Slate Settins -> Constraint Canvas -> Explicit Canvas Child ZOrder
  
  使用相同Zorder的Canvas作为容器，然后设置相同ZOrder的批次合并
  ```




* 拼界面

  ``` tex
  1. 窗口元素尽量扁平化
  	(不要过多的使用容器套娃),套用越多,函数调用递归性越强,执行代码越多,查找Vtable也越多.有可能会造成CPU缓存缺失.
  	窗口小部件树越小，函数调用越少
  	窗口小部件树越扁平，递归越少
  
  
  ```




* UE4 Widget UI优化

  ref: [Unreal Engine 4 中的 UI 优化技巧](https://gameinstitute.qq.com/community/detail/113852)

  



---





### UE4 Actor交互

* actor with actor

  ``` c++
  //UE4.25.4
  
  class AGameActor::AActor
  {
  public:
  	typedef std::function<void(bool)> OverlapHandle;
  	void SetOverlapHandle(OverlapHandle oHandle);
  private:
  	UFUNCTION()
  	void OnOverlapStart(AActor* OverlappedActor, AActor* OtherActor);
  	UFUNCTION()
  	void OnOverlapEnd(AActor* OverlappedActor, AActor* OtherActor);
  private:
      bool mCanOverlap = false;
      OverlapHandle mOverlapHandle;
  }
  void AGameActor::DeInit()
  {
      if(mCanTouch)
  	{
  #if WITH_EDITOR
  		OnReleased.Clear();
  #else
  		OnInputTouchBegin.Clear();
  #endif
  	}
  	if(mCanOverlap)
  	{
  		OnActorBeginOverlap.Clear();
  		OnActorEndOverlap.Clear();
  	}
  }
  void AGameActor::SetOverlapHandle(OverlapHandle oHandle)
  {
  	if(nullptr != oHandle)
  	{
  		mOverlapHandle = oHandle;
  		mCanOverlap = true;
  		//this->bGenerateOverlapEventsDuringLevelStreaming = 1;
  		OnActorBeginOverlap.AddDynamic(this, &AGameActor::AGameActor::OnOverlapStart);
  		OnActorEndOverlap.AddDynamic(this, &AGameActor::OnOverlapEnd);
  	}
  }
  void AGameActor::OnOverlapStart(AActor* OverlappedActor, AActor* OtherActor)
  {
  	if(nullptr != mOverlapHandle)
  	{
  		mOverlapHandle(true);
  	}
  }
  void AGameActor::OnOverlapEnd(AActor* OverlappedActor, AActor* OtherActor)
  {
  	if(nullptr != mOverlapHandle)
  	{
  		mOverlapHandle(false);
  	}
  }
  ```

  ![](UE4 UI开发.assets/202110141610389.jpg)

  ![UE4_Actor_Overlay_2](UE4 UI开发.assets/202110141610498.jpg)

  

* mouse or touch with actor

  ``` c++
  class AGameActor::AActor
  {
  public:
  	typedef std::function<void()> TouchHandle;
  	void SetTouchHandle(TouchHandle tHandle);
  private:
  	UFUNCTION()
  	void OnTouch(ETouchIndex::Type FingerIndex, AActor* TouchedActor);
  	UFUNCTION()
  	void OnMouseReleased(AActor* TouchedActor , FKey ButtonPressed);
  private:
  	bool mCanTouch = false;
  	TouchHandle mTouchHandle;
  }
  void AGameActor::SetTouchHandle(TouchHandle tHandle)
  {
  	if(nullptr != tHandle)
  	{
  		mTouchHandle = tHandle;
  		mCanTouch = true;
  #if WITH_EDITOR
  		OnReleased.AddDynamic(this, &AGameActor::OnMouseReleased);  //OnClicked是双击效果
  		#else
  		OnInputTouchBegin.AddDynamic(this, &AGameActor::OnTouch);
  #endif
  	}
  }
  void AGameActor::OnTouch(ETouchIndex::Type FingerIndex, AActor* TouchedActor)
  {
  	if(nullptr != mTouchHandle)
  	{
  		mTouchHandle();
  	}
  }
  void AGameActor::OnMouseReleased(AActor* TouchedActor, FKey ButtonPressed)
  {
  	if(nullptr != mTouchHandle)
  	{
  		mTouchHandle();
  	}
  }
  ```

  ![](UE4 UI开发.assets/202110141611738.jpg)

  ![UE4_Actor_Touch_MouseClick_2](UE4 UI开发.assets/202110141611740.jpg)



