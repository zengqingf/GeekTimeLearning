using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
物体惯性旋转方式：

1. 旋转物体
2. 旋转相机（用单独相机照模型）

@注：触控屏获取 Input.GetAxis(“Mouse X”) 获取不到，
     使用 Input.touches[0].deltaPosition.x 来代替
*/

/*
物体自动旋转

脚本放在 Cube 上
*/
//物体添加刚体，忽略重力
// 自动围绕 Y 轴旋转
public class Spin : MonoBehaviour
{
	public Vector3 rotationsPerSecond = new Vector3(0f, 0.1f, 0f);
	public bool ignoreTimeScale = false;

	Rigidbody mRb;
	Transform mTrans;

	void Start ()
	{
		mTrans = transform;
		mRb = GetComponent<Rigidbody>();
	}

	void Update ()
	{
		if (mRb == null)
		{
			ApplyDelta(ignoreTimeScale ? RealTime.deltaTime : Time.deltaTime);
		}
	}

	void FixedUpdate ()
	{
		if (mRb != null)
		{
			ApplyDelta(Time.deltaTime);
		}
	}

	public void ApplyDelta (float delta)
	{
		delta *= Mathf.Rad2Deg * Mathf.PI * 2f;
		Quaternion offset = Quaternion.Euler(rotationsPerSecond * delta);

		if (mRb == null)
		{
			mTrans.rotation = mTrans.rotation * offset;
		}
		else
		{
			mRb.MoveRotation(mRb.rotation * offset);
		}
	}
}


public class RealTime : MonoBehaviour
{
	static RealTime mInst;

	float mRealTime = 0f;
	float mRealDelta = 0f;

	/// <summary>
	/// Real time since startup.
	/// </summary>

	static public float time
	{
		get
		{
#if UNITY_EDITOR
			if (!Application.isPlaying) return Time.realtimeSinceStartup;
#endif
			if (mInst == null) Spawn();
			return mInst.mRealTime;
		}
	}

	/// <summary>
	/// Real delta time.
	/// </summary>

	static public float deltaTime
	{
		get
		{
#if UNITY_EDITOR
			if (!Application.isPlaying) return 0f;
#endif
			if (mInst == null) Spawn();
			return mInst.mRealDelta;
		}
	}

	static void Spawn ()
	{
		GameObject go = new GameObject("_RealTime");
		DontDestroyOnLoad(go);
		mInst = go.AddComponent<RealTime>();
		mInst.mRealTime = Time.realtimeSinceStartup;
	}

	void Update ()
	{
		float rt = Time.realtimeSinceStartup;
		mRealDelta = Mathf.Clamp01(rt - mRealTime);
		mRealTime = rt;
	}
}


/*
左右惯性滑动，上下滑动，同时有角度限制

脚本放在照射 Cube 的相机上，实际转的是相机，不是模型。
通过勾选脚本中的 Is Touch 选择是鼠标操作还是触控屏操作。
*/
public class ModelTest : MonoBehaviour
{
    public Transform target;
    public bool _isTouch;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;//X旋转速度
    public float ySpeed = 120.0f;//Y旋转速度
    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;
    public float distanceMin = 0.5f;
    public float distanceMax = 15f;
    public float smoothTime = 2f;

    public float rotationYAxis = 0.0f;
    public float rotationXAxis = 0.0f;
    public float velocityX = 0.0f;
    public float velocityY = 0.0f;

    private float roteSpeed = 0;//旋转速度

    private Vector2 oldPosition1;
    private Vector2 oldPosition2;

    private bool _isDistanceAnim;

    private float _msgDistance;

    private bool _isAutoRote;
    private Vector3 _mousePos;
    private bool _moveMouse;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        angles.x = 90;
        rotationYAxis = angles.y;
        rotationXAxis = angles.x;

        Init();
    }

    private void Init()
    {
        velocityX = 0;
        velocityY = 0;

        rotationYAxis = transform.rotation.eulerAngles.x;
        rotationXAxis = transform.rotation.eulerAngles.y;

        distance = distanceMax;
        roteSpeed = distanceMin * 0.2f;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        if (_isTouch)
        {
            if (Input.touchCount == 1)
            {
                //旋转
                Touch t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Began)//有人操作旋转
                {
                    _mousePos = t.position;
                    _isAutoRote = false;
                }
                if (t.phase == TouchPhase.Moved)
                {
                    float d = Vector3.Distance(t.position, _mousePos);
                    if (d > 50f)
                    {
                        //小于50，表示移动距离很小，防止触摸屏误触点击
                        Debug.Log(d);
                        //开始旋转
                    }
                }
                if (t.phase == TouchPhase.Ended)
                {
                    //停止
                }

                velocityX += xSpeed * t.deltaPosition.x * roteSpeed * 0.02f;
                velocityY -= ySpeed * t.deltaPosition.y * roteSpeed * 0.02f;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                //有人操作旋转
                _isAutoRote = false;
                _mousePos = Input.mousePosition;
                _moveMouse = false;
            }
            if (Input.GetMouseButtonUp(0))
            {
                _moveMouse = false;
            }
            if (Input.GetMouseButton(0))
            {
                velocityX += xSpeed * Input.GetAxis("Mouse X") * distance * 0.2f;
                velocityY -= ySpeed * Input.GetAxis("Mouse Y") * distance * 0.2f;

                if (Vector3.Distance(_mousePos, Input.mousePosition) > 10f && !_moveMouse)
                {
                    _moveMouse = true;
                }
            }
        }

        rotationXAxis += velocityX;
        rotationYAxis += velocityY;
        rotationYAxis = ClampAngle(rotationYAxis, yMinLimit, yMaxLimit);
        Quaternion toRotation = Quaternion.Euler(rotationYAxis, rotationXAxis, 0);
        Quaternion rotation = toRotation;

        transform.rotation = toRotation;

        velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
        velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);

        if (_isTouch)
        {
            Zoom();
        }
        else
        {
            if (!_isDistanceAnim)
            {
                distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * ((distanceMax - distanceMin) * 0.1f), distanceMin, distanceMax);
                //缩放
            }
        }

        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + target.position;
        transform.position = position;
    }


    private void Zoom()
    {
        if (Input.touchCount < 2)
        {
            return;
        }

        if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
        {
            Vector2 tempPosition1 = Input.GetTouch(0).position;
            Vector2 tempPosition2 = Input.GetTouch(1).position;
            if (isEnlarge(oldPosition1, oldPosition2, tempPosition1, tempPosition2))
            {
                if (distance > distanceMin)
                {
                    distance -= (distanceMax - distanceMin) * 0.1f;
                }
            }
            else
            {
                if (distance < distanceMax)
                {
                    distance += (distanceMax - distanceMin) * 0.1f;
                }
            }
            oldPosition1 = tempPosition1;
            oldPosition2 = tempPosition2;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

    bool isEnlarge(Vector2 oP1, Vector2 oP2, Vector2 nP1, Vector2 nP2)
    {
        var leng1 = Mathf.Sqrt((oP1.x - oP2.x) * (oP1.x - oP2.x) + (oP1.y - oP2.y) * (oP1.y - oP2.y));
        var leng2 = Mathf.Sqrt((nP1.x - nP2.x) * (nP1.x - nP2.x) + (nP1.y - nP2.y) * (nP1.y - nP2.y));
        if (leng1 < leng2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}


//操作方式
public enum ControlTypeTest
{
    mouseControl,
    touchControl,
}

/*
支持左右惯性旋转
绑定在 Cube 上， Rot Target 也是自身。
支持鼠标和手指滑动
*/
public class Test : MonoBehaviour
{
    public ControlTypeTest controlType;
    public Transform rotTarget;

    //旋转速度加成系数
    public float rotSpeedScalar=20f;
    private float currentSpeed = 0;
    private void Start()
    {
        float a = float.Parse("-0.1994895");
        Debug.Log(a);
    }
    void Update()
    {
        if (controlType == ControlTypeTest.mouseControl)
        {
            //鼠标操作
            if (Input.GetMouseButton(0))
            {
                //拖动时速度
                //鼠标或手指在该帧移动的距离*deltaTime为手指移动的速度,此处为Input.GetAxis("Mouse X") / Time.deltaTime
                //不同帧率下lerp的第三个参数(即混合比例)也应根据帧率而不同--
                //考虑每秒2帧和每秒100帧的情况，如果此参数为固定值，那么在2帧的情况下，一秒后达到目标速度的0.75,而100帧的情况下，一秒后则基本约等于目标速度
                currentSpeed = Mathf.Lerp(currentSpeed, Input.GetAxis("Mouse X") / Time.deltaTime,0.5f * Time.deltaTime);
            }
            else
            {
                //放开时速度
                currentSpeed = Mathf.Lerp(currentSpeed, 0, 0.5f * Time.deltaTime);
            }
        }
        else if (controlType == ControlTypeTest.touchControl)
        {
                
            //触摸操作
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                //在安卓设备上也可以用Mouse X,根据实验，touch[0].deltaPosition.x的值总是比Mouse X的值大很多，所以此处使用Mouse X
                currentSpeed = Mathf.Lerp(currentSpeed, Input.touches[0].deltaPosition.x / Time.deltaTime/15, 0.5f * Time.deltaTime);
                Debug.Log(1111);
            }
            else
            {
                //放开时速度
                currentSpeed = Mathf.Lerp(currentSpeed, 0, 0.5f * Time.deltaTime);
            }
        }
        rotTarget.Rotate(Vector3.down, Time.deltaTime * currentSpeed * rotSpeedScalar);
    }
}


/*
上下左右自由惯性旋转 使用 Rigidbody.AddTorque
手指触摸，脚本放在 Cube 上。
*/
public class Test2 : MonoBehaviour
{
    #region Rigibody.AddTorque 惯性旋转
    public Rigidbody rigid;
    float h;
    float v;
    Vector3 torque;
    private void Start()
    {
        //rigid = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            //h = Input.GetAxis("Mouse X");
            //v = Input.GetAxis("Mouse Y");
            h = Input.touches[0].deltaPosition.x / 15;
            v = Input.touches[0].deltaPosition.y / 15;
            Debug.Log(h + "   " + v);
        }
        else
        {
            h = 0;
            v = 0;
        }
        torque = new Vector3(v, -h, 0);
    }
    private void FixedUpdate()
    {
        rigid.AddTorque(torque * 1, ForceMode.Force);
    }
    #endregion
}


/*
物体自动左右旋转，有手指滑动，按照手指滑动运行，没有手指，继续自动转
脚本绑定在 Cube 上，相机为照射 Cube 的相机。
*/
public class Test2 : MonoBehaviour
{
#region
    public Camera MainCamera;
    public float ZoomMin;      //滚轮的最小值
    public float ZoomMax;      //滚轮的最大值
    private float normalDistance;   //设置摄像机的景深值
    private float MouseWheelSencitivity = 10.0f;    //鼠标灵敏度,就是缩放的速度的快慢

    private float axisX;
    private float axisY;
    public float speed = 6f;
    private float tempSpeed;

    private bool RoationOnly;

    void Start()
    {
        normalDistance = 50.0f;
        ZoomMin = 20.0f;
        ZoomMax = 100.0f;
        RoationOnly = true;
    }

    void Update()
    {
        Roation();
        this.transform.Rotate(new Vector3(0, axisX, 0) * Rigid(), Space.World);     //物体旋转的方法
    }

    //自动旋转物体的方法，放在Update中调用
    void Roation()
    {
        if (RoationOnly)
        {
            gameObject.transform.Rotate(Vector3.up * Time.deltaTime * 10);
        }
    }

    /***
    * 
    * 鼠标左键控制物体360°旋转+惯性
    * **/
    float Rigid()
    {
        if (Input.touchCount <= 0)
            return 0;
        //if (Input.GetTouch(0).phase == TouchPhase.Ended)
        //{

        //    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        //}

        if (Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            RoationOnly = false;    //当鼠标按下的时候，停止自动旋转

            //axisX = Input.GetAxis("Mouse X");
            //axisY = Input.GetAxis("Mouse Y");
            axisX = Input.touches[0].deltaPosition.x / 30;
            axisY = Input.touches[0].deltaPosition.y / 30;
            if (tempSpeed < speed)
            {
                tempSpeed += speed * Time.deltaTime * 5;
            }
            else
            {
                tempSpeed = speed;
            }
        }
        else
        {
            RoationOnly = true;     //当鼠标没有按下的时候，恢复自动旋转
            if (tempSpeed > 0)
            {
                tempSpeed -= speed * Time.deltaTime;
            }
            else
            {
                tempSpeed = 0;
            }
        }
        return tempSpeed;
    }
#endregion
}