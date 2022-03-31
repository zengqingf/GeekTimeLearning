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

  