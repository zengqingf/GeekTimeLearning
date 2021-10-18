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







---



### Lua







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

  ![](https://raw.githubusercontent.com/MJX1010/PicGoRepo/main/img/202110141610389.jpg)

  ![UE4_Actor_Overlay_2](https://raw.githubusercontent.com/MJX1010/PicGoRepo/main/img/202110141610498.jpg)

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

  ![](https://raw.githubusercontent.com/MJX1010/PicGoRepo/main/img/202110141611738.jpg)

  ![UE4_Actor_Touch_MouseClick_2](https://raw.githubusercontent.com/MJX1010/PicGoRepo/main/img/202110141611740.jpg)
