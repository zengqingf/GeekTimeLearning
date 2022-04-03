### 3D 图形学

* 基础

  ``` tex
  1、向量的长度、大小
  称为向量的模
  |v| = 根号 vx^2+vy^2
  
  三维向量  
  |v| = 根号 vx^2+vy^2+vz^2
  
  Unity中  直接使用magnitude (就是模的意思)  
  如果要进行向量间的大小比较时，  使用性能更好的 sqrMagnitude  不需要开平方  
  
  
  几何意义：
  表示向量的长度、大小；
  表示两个对象间的距离，一个对象的运动速度
  
  Unity中计算两个对象的距离，还可以通过Distance，
  如果需要将一个对象的运动速度限定在一个指定长度内，可以使用 ClampMagnitude( Vector3 vetor,float maxLength )
  
  2、标量和向量相乘
  
  结果是得到一个与原向量平行的向量，当向量为负值时，方向相反
  
  Unity中简单应用—— gameObject.transform.position+=_Speed * _SpeedScale;   速度* 加速比率
  
  3、标准化向量（Normalize）
  将不为零的向量归一化，只需要得到向量的方向
  
  Unity中  使用 属性 normalized 或者方法 Normalize（）
  
  4、向量加法&向量减法
  如果第一个向量表示点的位置，那么向量加法可以表示为这个点根据第二个向量的方向和大小进行移动
  
  如果第一个向量表示点的位置，那么向量减法表示
  ```


* 基础2

  ``` tex
  1、向量的长度、大小
  称为向量的模
  |v| = 根号 vx^2+vy^2
  
  三维向量  
  |v| = 根号 vx^2+vy^2+vz^2
  
  如果需要将一个对象的运动速度限定在一个指定长度内，可以使用 ClampMagnitude( Vector3 vetor,float maxLength )
  
  2、标量和向量相乘
  结果是得到一个与原向量平行的向量，当向量为负值时，方向相反
  Unity中简单应用—— gameObject.transform.position+=_Speed * _SpeedScale;   速度* 加速比率
  
  3、标准化向量（Normalize）
  将不为零的向量归一化，只需要得到向量的方向
  Unity中  使用 属性 normalized 或者方法 Normalize（）
  
  4、向量加法&向量减法
  如果第一个向量表示点的位置，那么向量加法可以表示为这个点根据第二个向量的方向和大小进行移动
  如果第一个向量表示点的位置，那么向量减法表示一个对象从一个位置到另一个位置的方向和距离
  或者计算一个对象与另一个对象之间的距离和相对方位
  
  5、向量点乘（内积）
  点乘就是对应分量的乘积  = 结果为一个标量   反应了两个向量的相似程度，结果越大，越接近
  点乘的点 不能省略
  a · b =|a| |b| cos0
  0=90度时，结果等于0
  可以计算一个向量在另一个向量上的投影
  
  Unity中 使用函数  Vector3.Dot( a , b )
  需要计算一个向量在另一个向量上的投影，使用 Project（Vector3 vector, Vector3 onNormal）
  需要实现一个对象朝一个对象移动，MoveTowards（Vector3 current, Vector3 target, float maxDistanceDelta）
  只能在三维空间使用，表达式 a * b 
  与点乘不同，叉乘结果是一个向量且不满足交换律
  其结果是一个垂直与原来的两个向量的向量  （左手坐标系）
  叉乘的结果的向量长度  为  |a*b|=|a| |b| sin0
  上述公式的结果还可以计算由 a  b 向量构成的平行四边形面积，或者三角形面积的一半
  
  Unity中使用 Cross（Vector3 lhs,Vector3 rhs）
  也可以计算某一面的法向量
  例如，现在有三个点 a b c 计算这三个点构成的面的法向量
      1、分别以a为起点 做两个向量  b-a  和  c-a  ,都是从a出发的
      2、使用上述函数
      3、得到的向量进行归一化
  
  1、局部坐标系和世界坐标系
  2、父子物体
     B是以A的局部坐标系作为参考坐标系，那么B为A的子物体
  3、多边形、边、顶点、面片
  Unity会将模型（如用3DMAX和MAYA建立的模型）的面片（Meshes）转化为三边面，每个面成为一个多边形（Polygon）,这三边面由三条边组成（Edge）组成，每条边由两个顶点（Vertice）组成
  多边形数量和顶点数量是影响游戏渲染速度的一个重要因素
  有多种增强方法，LOD（层级细节）、Occlusion Culling（遮挡剔除）
  
  
  Unity图形渲染
  
  Mesh Fillter 网格过滤器   确定3D模型的外观形状和大小
  Mesh Renderer  网格渲染其  获取过滤器中的形状，并根据Transform组件的位置进行渲染
      （上述两个需要同时使用）
  Skinned Mesh Renderer  蒙皮网格过滤器    一般针对人物角色模型  （Quality:定义了一个顶点可以包含多少个骨骼的信息、Update When OffScrenn  在屏幕外是否仍然更新骨骼动画  Root Bone 指定蒙皮骨骼  Bounds  模型占用边框大小）
  
  贴图：贴图尺寸，一般模型贴图的尺寸为2的n次幂，因为Unity默认对导入的贴图尺寸将被缩放为2 的 n次幂
  Mipmap  二维贴图
  材质球
  最终呈现颜色=（漫反射（Diffuse）+凹凸（Bumped）+高光（Specular）+自发光+反射+透明）*透明度
  ```

* 坐标系

  ``` tex
  1、对于模型空间和世界空间，Unity使用的是左手坐标系
  2、对于观察空间，即以摄像机为原点的坐标系；
  ```

  