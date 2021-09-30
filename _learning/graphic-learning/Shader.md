# Shader



## 一、Unity Shader



* ref

  [Shader学习方法](https://www.cnblogs.com/Esfog/p/How_To_Learn_Shader.html)

  [shader-潜水的...](http://blog.sina.com.cn/s/articlelist_2312702844_6_1.html)

  [猫都能学会的Unity3D Shader入门指南](https://onevcat.com/2013/07/shader-tutorial-1/)

  [UnityShader之旅](https://blog.csdn.net/dbtxdxy/category_2938139.html)

  [风宇冲](http://blog.sina.com.cn/s/blog_471132920101d5kh.html)

  [冯乐乐-UnityShader](https://blog.csdn.net/candycat1992/category_1782159.html)

  [浅墨-Unity Shader](https://blog.csdn.net/poem_qianmo/category_2681301.html)

  [苍白的茧-Unity渲染实例](http://www.dreamfairy.cn/blog/)

  

  [UnityShader新手](https://www.cnblogs.com/polobymulberry/category/663137.html)

  [UnityShader初识](https://blog.csdn.net/LIQIANGEASTSUN/article/details/42294817)

  [UnityShader实战](https://blog.csdn.net/wolf96/category_9263644.html)

  [UnityShader实战2](https://blog.csdn.net/u011047171/category_5579049.html)

  [Unity Shaders and Effects Cookbook ](https://blog.csdn.net/huutu/category_9265579.html)

  [Unity后处理](https://blog.csdn.net/puppet_master/category_6441122.html)

  [UnityShader示例](https://blog.csdn.net/mobilebbki399/category_6015395.html)

  [Unity中的Cg Shader解读](https://blog.csdn.net/mobanchengshuang/category_9263040.html)

  [Unity Shader实例2](https://blog.csdn.net/liqiangeastsun/category_2808191.html)

  [Shader](https://blog.csdn.net/sinat_20559947/category_5825515.html)

  [UnityShader渲染](https://blog.csdn.net/u010133610/category_9567150.html)

  [UnityShader实例3](https://www.cnblogs.com/jqm304775992/category/744596.html)

  [UnityShader渲染2](https://www.cnblogs.com/zhanlang96/tag/shader/)

  

  [Unity渲染路径](https://www.cnblogs.com/MrZivChu/p/RenderingPath.html)

  

  [Unity forum - Shader](https://forum.unity.com/forums/shaders.16/)

​		[A gentle introduction to shaders in Unity3D](http://www.alanzucconi.com/2015/06/10/a-gentle-introduction-to-shaders-in-unity3d/)



​		[blog-computer graphics](https://www.iquilezles.org/www/index.htm)

​		[DirectX Rendering Pipeline](https://www.cnblogs.com/clayman/archive/2010/12/31/1923354.html)



​		[(译）OpenGL ES2.0 – Iphone开发指引](https://www.cnblogs.com/zilongshanren/archive/2011/08/08/2131019.html)

​		[opengl es](https://www.cnblogs.com/kesalin/tag/opengl%20es/)

​		[OpenGL ES入门详解](https://blog.csdn.net/wangyuchun_799/article/details/7736928)



​		[图形学基础](https://www.cnblogs.com/mengdd/category/397109.html)



​		



---

### API

* 修改shader属性 / 更换shader

  ``` c#
  gameObject.render.materials.GetFloat("propName");
  gameObject.render.materials.SetFloat("propName", value);
  gameObject.renderer.material.SetTexture("_propName", tex);
  gameObject.renderer.material.GetTexture("_propName");
  
  gameObject.GetComponent<MeshRenderer>().material.SetTexture("_Tex", text32);
  
  //无法找到_Color时
  renderer.material.color=颜色 ==> renderer.material.SetColor("_Color",颜色);
  
  //shader代码 _TransVal("Transparency_Value", Range(0,1)) = 0.5
  renderer.material.SetFloat("_TransVal", TranValue);
  ```

  ![](https://raw.githubusercontent.com/MJX1010/PicGoRepo/main/img/202109291050108.png)

  ``` c#
  //更换shader
  renderer.material.shader = Shader.Find("Custom/SimpleAlpha");
  ```

  

* sharedMaterial vs. Material

  ``` tex
  多个物体用的是同一个material，当用gameObject.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_Color",newColor)改变其中一个物体的颜色时，其余物体的颜色也跟着变了。
  
  要想其余物体颜色不跟着变，应该用gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color",newColor)，
  这样会为此gameObject新创建一个material（即此gameObject不再使用公用material）
  代价是这个gameObject便不能与其余gameObject一起batch了。
  ```



* 绘制mesh

  [Unity Mesh](https://www.jianshu.com/p/7cd99a05cfea)

  [Programmatically Generating Meshes In Unity3D](http://kobolds-keep.net/?p=33)

  ``` c#
  public class MeshDemo : MonoBehaviour
  {
          // Use this for initialization
          void Start ()
          {
              this.GetTriangle ();
          }
      	//绘制三角形
          public GameObject GetTriangle ()
          {
              GameObject go = new GameObject ("Triangle");
              MeshFilter filter = go.AddComponent<MeshFilter> ();
               
              // 构建三角形的三个顶点，并赋值给Mesh.vertices
              Mesh mesh = new Mesh ();
              filter.sharedMesh = mesh;
              mesh.vertices = new Vector3[] {
                  new Vector3 (0, 0, 1),
                  new Vector3 (0, 2, 0),
                  new Vector3 (2, 0, 5),
              };
  
              // 构建三角形的顶点顺序，因为这里只有一个三角形，
              // 所以只能是(0, 1, 2)这个顺序。
              mesh.triangles = new int[3] {0, 1, 2};
  
              mesh.RecalculateNormals ();
              mesh.RecalculateBounds ();
  
              // 使用Shader构建一个材质，并设置材质的颜色。
              Material material = new Material (Shader.Find ("Diffuse"));
              material.SetColor ("_Color", Color.yellow);
  
              // 构建一个MeshRender并把上面创建的材质赋值给它，
              // 然后使其把上面构造的Mesh渲染到屏幕上。
              MeshRenderer renderer = go.AddComponent<MeshRenderer> ();
              renderer.sharedMaterial = material;
  
              return go;
          }
      	//绘制五边形
          public GameObject GetPentagon ()
          {
              GameObject go = new GameObject ("Pentagon");
              MeshFilter filter = go.AddComponent<MeshFilter> ();
  
              Mesh mesh = new Mesh ();
              filter.sharedMesh = mesh;
              mesh.vertices = new Vector3[] {
                  new Vector3 (0, 0, 0),
                  new Vector3 (0, 2, 0),
                  new Vector3 (2, 0, 0),
                  new Vector3 (2, -2, 0),
                  new Vector3 (1, -2, 0),
              };
  
              mesh.triangles = new int[9] {0, 1, 2, 0, 2, 3, 0, 3, 4};
  
              mesh.RecalculateNormals ();
              mesh.RecalculateBounds ();
  
              Material material = new Material (Shader.Find ("Diffuse"));
              material.SetColor ("_Color", Color.yellow);
  
              MeshRenderer renderer = go.AddComponent<MeshRenderer> ();
              renderer.sharedMaterial = material;
  
              return go;
          }
  }
  ```

  

* ColorHex to RGB

  ``` c#
  //ref: https://www.jianshu.com/p/f165d90a491f
  //例如将 #00FFF4FF 转换成 Color，或者将一个color转换成#00FFF4FF格式
  
    /// <summary>
    /// color 转换hex
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static string ColorToHex(Color color)
    {
        int r = Mathf.RoundToInt(color.r * 255.0f);
        int g = Mathf.RoundToInt(color.g * 255.0f);
        int b = Mathf.RoundToInt(color.b * 255.0f);
        int a = Mathf.RoundToInt(color.a * 255.0f);
        string hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);
        return hex;
    }
  
    /// <summary>
    /// hex转换到color
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    public Color HexToColor(string hex)
    {
        byte br = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte bg = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte bb = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        byte cc = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        float r = br / 255f;
        float g = bg / 255f;
        float b = bb / 255f;
        float a = cc / 255f;
        return new Color(r, g, b, a);
    }
  
  ```



* 点乘Dot 叉乘Cross 判断方向朝向

  [[Unity游戏开发]向量在游戏开发中的应用](https://blog.csdn.net/wenxin2011/article/details/50810102)

  [[Unity游戏开发]向量在游戏开发中的应用（二）](https://blog.csdn.net/wenxin2011/article/details/50972976)

  [[Unity游戏开发]向量在游戏开发中的应用（三）](https://blog.csdn.net/wenxin2011/article/details/51088236)

  

  ``` c#
  using UnityEngine;
  using System.Collections;
  //物体移动，追踪，判断两物体移动方向是否相同，两物体移动方向夹角，
  //以及物体 A 朝 物体 B 顺时针方向还是逆时针方向移动。物体 A 在 物体 B 的前后左右方向
  public class VectorDotCross : MonoBehaviour {
  
      // 关于点积
      private void Dot()
      {
          /*
          点积 
          点积的计算方式为:  a·b=|a|·|b|cos<a,b>  其中|a|和|b|表示向量的模，
          <a,b>表示两个向量的夹角。 通过点积判断当两个向量的方向向是否相同
          （大致相同即两个向量的夹角在 90 度范围内）
          两个向量的 点积 大于 0 则两个向量夹角小于 90 度， 否则 两个向量的
          夹角大于 90 度，
          */
          // 定义两个向量 a、b
          Vector3 a = new Vector3(1, 1, 1);
          Vector3 b = new Vector3(1, 5, 1);
  
          // 计算 a、b 点积结果
          float result = Vector3.Dot(a, b);
  
          // 通过向量直接获取两个向量的夹角（默认为 角度）， 此方法范围 [0 - 180]
          float angle = Vector3.Angle(a, b);
  
          // 下面获取夹角的方法，只是展示用法，太麻烦不必使用
          // 通过向量点积获取向量夹角，需要注意，必须将向量转换为单位向量才行
          // 计算 a、b 单位向量的点积
          result = Vector3.Dot(a.normalized, b.normalized);
          // 通过反余弦函数获取 向量 a、b 夹角（默认为 弧度）
          float radians = Mathf.Acos(result);
          // 将弧度转换为 角度
          angle = radians * Mathf.Rad2Deg;
      }
  
      // 关于叉乘
      private void Cross()
      {
          /*
            叉积 
            叉积的定义： c = a x b  其中a,b,c均为向量。两个向量的叉积是向量， 向量的模为  |c|=|a||b|sin<a,b>
            且 向量 c 垂直于 a、b， c 垂直于 a、b 组成的平面， a x b = - b x a;
          */
          // 定义两个向量 a、b
          Vector3 a = new Vector3(1, 1, 1);
          Vector3 b = new Vector3(1, 5, 1);
  
          //计算向量 a、b 的叉积，结果为 向量 
          Vector3 c = Vector3.Cross(a, b);
  
          // 下面获取夹角的方法，只是展示用法，太麻烦不必使用
          // 通过反正弦函数获取向量 a、b 夹角（默认为弧度）
          float radians = Mathf.Asin(Vector3.Distance(Vector3.zero, Vector3.Cross(a.normalized, b.normalized)));
          float angle = radians * Mathf.Rad2Deg;
  
          // 判断顺时针、逆时针方向，是在 2D 平面内的，所以需指定一个平面，下面以X、Z轴组成的平面为例（忽略 Y 轴）
          // 以 Y 轴为纵轴
          // 在 X、Z 轴平面上，判断 b 在 a 的顺时针或者逆时针方向
          if (c.y > 0)
          {
              // b 在 a 的顺时针方向
          }
          else if (c.y == 0)
          {
              // b 和 a 方向相同（平行）
          }
          else
          {
              // b 在 a 的逆时针方向
          }
      }
  
      // 获取两个向量的夹角  Vector3.Angle 只能返回 [0, 180] 的值
      // 如真实情况下向量 a 到 b 的夹角（80 度）则 b 到 a 的夹角是（-80）
      // 通过 Dot、Cross 结合获取到 a 到 b， b 到 a 的不同夹角
      private void GetAngle(Vector3 a, Vector3 b)
      {
          Vector3 c = Vector3.Cross(a, b);
          float angle = Vector3.Angle(a, b);
  
          // b 到 a 的夹角
          float sign = Mathf.Sign(Vector3.Dot(c.normalized, Vector3.Cross(a.normalized, b.normalized)));
          float signed_angle = angle * sign;
  
          Debug.Log("b -> a :" + signed_angle);
  
          // a 到 b 的夹角
          sign = Mathf.Sign(Vector3.Dot(c.normalized, Vector3.Cross(b.normalized, a.normalized)));
          signed_angle = angle * sign;
  
          Debug.Log("a -> b :" + signed_angle);
      }
  
  }
  ```

  

* 欧拉角，万向锁，四元数

  [游戏动画中欧拉角与万向锁的理解](https://blog.csdn.net/huazai434/article/details/6458257)

  [三维旋转：旋转矩阵，欧拉角，四元数](https://www.cnblogs.com/yiyezhai/p/3176725.html)

  [理解矩阵乘法](http://www.ruanyifeng.com/blog/2015/09/matrix-multiplication.html)

  [unity 旋转欧拉角 万向锁 解释](https://blog.csdn.net/fengya1/article/details/50721768)

  * 四元数和矩阵

    ``` tex
    Quaternion和Rotation Matrix的相互转换
    设有Quaternion q(x, y, z, w), rotation axis(x, y, z), rotation angle (theta)，则Quaternion和Axis-Angle有如下对应关系，
    
    q.x = sin(theta / 2) * axis.x
    q.y = sin(theta / 2) * axis.y
    q.z = sin(theta / 2) * axis.z
    q.w = cos(theta / 2)
    由Rotation Matrix求Quaternion
    
    如何由给定的rotation matrix提取出旋转轴和旋转角度？
    使用函数D3DXQuaternionRotationMatrix可以直接由Rotation matrix求出对应的Quaternion
    由Quaternion求Rotation Matrix
    可以利用上面的公式先求出旋转轴和旋转角度
    
    axis.x = q.x / sin(theta / 2)
    axis.y = q.y / sin(theta / 2)
    axis.z = q.z / sin(theta / 2)
    theta = 2 * arccos(q.w)
    或者D3DX函数D3DXQuaternionToAxisAngle求出旋转轴和旋转角度
    得到了旋转轴axis和旋转角度angle以后就可以利用D3DXMatrixRotationAxis来求取旋转矩阵，也可以手动求解，看这篇博文。也可以一步到位，用D3DXMatrixRotationQuaternion函数直接求得旋转矩阵
    ```

    

  * 区分

    | 任务/性质                    | 旋转矩阵                               | 欧拉角                         | 四元数                                  |
    | ---------------------------- | -------------------------------------- | ------------------------------ | --------------------------------------- |
    | 在坐标系间(物体和惯性)旋转点 | 能                                     | 不能(必须转换到矩阵)           | 不能(必须转换到矩阵)                    |
    | 连接或增量旋转               | 能,但经常比四元数慢,小心矩阵蠕变的情况 | 不能                           | 能,比矩阵快                             |
    | 插值                         | 基本上不能                             | 能,但可能遭遇万向锁或其他问题  | Slerp提供了平滑插值                     |
    | 易用程度                     | 难                                     | 易                             | 难                                      |
    | 在内存或文件中存储           | 9个数                                  | 3个数                          | 4个数                                   |
    | 对给定方位的表达方式是否唯一 | 是                                     | 不是,对同一方位有无数多种方法  | 不是,有两种方法,它们互相为互            |
    | 可能导致非法                 | 矩阵蠕变                               | 任意三个数都能构成合法的欧拉角 | 可能会出现误差积累,从而产生非法的四元数 |

  

* Bezier曲线（贝塞尔）

  [使用Unity画一条平滑曲线（贝塞尔曲线）并使小球沿曲线运动](https://blog.csdn.net/xiexian1204/article/details/49592765)

  ``` c#
  using UnityEngine;
  using System.Collections.Generic;
  public class Bezier {
      public List<Vector3> vertexs;
      //vertexCount:值越大则越光滑
      public Bezier(Vector3 p0, Vector3 p1, Vector3 p2, float vertexCount)
      {
          vertexs = new List<Vector3>();
   
          float interval = 1 / vertexCount;
          for (int i = 0; i < vertexCount; i++)
          {
              vertexs.Add(GetPoint(p0, p1, p2, i * interval));
          }
      }
   
      //vertexCount:值越大则越光滑
      public Bezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float vertexCount)
      {
          vertexs = new List<Vector3>();
   
          float interval = 1 / vertexCount;
          for (int i = 0; i < vertexCount; i++)
          {
              vertexs.Add(GetPoint(p0, p1, p2, p3, i * interval));
          }
      }
   
      //t在[0,1]范围
      private Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
      {
          float a = 1 - t;
          Vector3 target = p0 * Mathf.Pow(a, 2) + 2 * p1 * t * a + p2 * Mathf.Pow(t, 2);
          return target;
      }
   
      //t在[0,1]范围
      private Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
      {
          float a = 1 - t;
          Vector3 target = p0 * Mathf.Pow(a, 3) + 3 * p1 * t * Mathf.Pow(a, 2) + 3 * p2 * Mathf.Pow(t, 2) * a + p3 * Mathf.Pow(t, 3);
          return target;
      }
  }
  ```

  



---



### Unity示例

* uv移动

  [示例脚本](./scripts/uv_offset)

* 树叶随风动

  [示例脚本](./scripts/tree_leaf_wing)





---



### OpenGL音视频

* ref

  [雷霄驿-最简单的视音频播放示例](https://blog.csdn.net/leixiaohua1020/article/details/40246783)

  [opengl-tutorial](http://www.opengl-tutorial.org/cn/)