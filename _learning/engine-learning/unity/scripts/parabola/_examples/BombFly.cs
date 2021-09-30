
//https://blog.csdn.net/hiramtan/article/details/51753448
/*
比如迫击炮发射的子弹是抛物线运行的,并且在垂直方向上子弹做自由落体运动,如果垂直方向上不做自由落体(仅仅是一个匀速的抛物线),
有很多简便的方法可以实现,比如设置一个起始点,一个最高点,一个目标点,让物体按照这三个点移动就可以,稍微复杂点的用dotween,itween设置个路径点,按照路径点移动.
*/

/*
迫击炮抛物线击中目标单位,需要的参数是:1.目标单位; 2.炮弹的水平速度.
按照初中的物理知识,抛物线运动在水平方向是匀速的,垂直方向上是v=a*t(v是速度,a是加速度,t是时间).
也就是说只要得到垂直方向初始化速度,让水平方向和垂直方向各自运行就可以了.
如果知道目标(距离)和水平速度,就能得到飞行的时间.time=distance/speed; 在time这段时间内,炮弹用1/2的时间上升,1/2的时间下降.
炮弹发出时在垂直方向上的速度是多少呢?我们可以反过来看,在最顶点时炮弹速度是0,经过1/2的时间下落到平面,根据v=a*t(也就是speed=重力加速度*1/2*时间),这样就得到了垂直方向的初始化速度.
*/

/*

现实生活中炮弹发出时朝向天空,在最高点时炮弹旋转是水平的,在下落过程中炮弹朝向要轰炸的单位.下面为炮弹添加旋转逻辑:
首先根据上文水平方向速度和垂直方向的速度,能得到tan值,然后计算出调用系统math.atan的到弧度,然后弧度转换为角度.这个角度就是炮弹发出时与地面(水平方向)的夹角.然后在各角度在time/2的时间里变为0(最顶点角度为0)然后在下落的过程中角度在继续增大.最终逻辑如下:
*/
public class BombFly : MonoBehaviour
{
    public const float g = 9.8f;
 
    public GameObject target;
    public float speed = 10;
    private float verticalSpeed;
    private Vector3 moveDirection;
 
    private float angleSpeed;
    private float angle;
    void Start()
    {
       float tmepDistance = Vector3.Distance(transform.position, target.transform.position);
        float tempTime = tmepDistance / speed;
        float riseTime, downTime;
        riseTime = downTime = tempTime / 2;
         verticalSpeed = g * riseTime;
        transform.LookAt(target.transform.position);
 
        //添加旋转角度
        float tempTan = verticalSpeed / speed;
        double hu = Math.Atan(tempTan);
        angle = (float)(180 / Math.PI * hu);
        transform.eulerAngles = new Vector3(-angle, transform.eulerAngles.y, transform.eulerAngles.z);
        angleSpeed = angle / riseTime;
 
        moveDirection = target.transform.position - transform.position;
    }
    private float time;
    void Update()
    {
        if (transform.position.y < target.transform.position.y)
        {
            //finish
            return;
        }
        time += Time.deltaTime;
        float test = verticalSpeed - g * time;
        transform.Translate(moveDirection.normalized * speed * Time.deltaTime, Space.World);
        transform.Translate(Vector3.up * test * Time.deltaTime,Space.World);
        float testAngle = -angle + angleSpeed * time;
        transform.eulerAngles = new Vector3(testAngle, transform.eulerAngles.y, transform.eulerAngles.z);
    }
}