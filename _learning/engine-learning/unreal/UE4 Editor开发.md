### Editor开发

* 引擎内置模板

  ``` text
  /Engine/Binaries/Win64/SlateViewer.exe
  ```







---



### API

* 判断是否在运行状态

  ``` c++
  GetWorld()->HasBegunPlay()；
  /*
  在游戏运行时为True；
  区别于 GIsEditor， true if we are in the editor；note that this is still true when using play in editor
  */
  ```

  
