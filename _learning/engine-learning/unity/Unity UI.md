### Unity UI适配

* UGUI Text自适应

  ``` tex
  Unity 3D UGUI 的text不支持根据文本内容自适应显示宽度
  解决：
  加个content size fitter组件，那他就会自己适应长度了，其他锚点之类的设置好就不用自己算大小了
  ```

  ![image-20220331233622890](Unity UI.assets/image-20220331233622890-8740984.png)

* 屏幕自适应

  ``` tex
  用UGUI做东西的时候，自适应选择scalewithscreensize,默认是基于高度进行等比缩放，
  UGUI的Anchor，即当前图片相对于父节点的位置，将anchor的四个角与自己的四个角关联在一起，既可以实现非等比缩放。
  即物体的大小就等于四个anchor所形成的区域，一般是屏幕的百分比。
  如果要使屏幕在宽高比低于某个标准值的时候表现为顶部和底部出现黑边，大于标准值横向拉伸，可以在Canvas下面添加一个panel，动态的去改变该panel的大小即可。
  
  private readonly float _refWidth = 960.0f;
  private readonly float _refHeight = 640.0f;
  private readonly float _refRatio = 960.0f / 640.0f;
  
  // Use this for initialization
  void Start ()
  {
  if (Screen.width * 1.0f / Screen.height > _refRatio)
  {
  GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / (Screen.height / _refHeight), _refHeight);
  }
  else
  {
  GetComponent<RectTransform>().sizeDelta = new Vector2(_refWidth, _refHeight);
  }
  }
  ```

  