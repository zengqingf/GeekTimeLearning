/*
当场景中的3D物体需要响应点击，但同时有UI显示时，存在判断点击是在3D物体上还是UI上的问题，办法如下：

1. 射线检测所有2D 3D物体，有2D物体被检测到时表明当前有UI。但无论Physics2D.Raycast()还是Physics.Raycast()都只能检测到含有Collider组件的物体，普通UI如Image Button等一般用射线是不起作用的。EventSystem.current.RaycastAll()可以将当前屏幕上的所有可检测的物体全部检测到，该方法需要自己构造一个PointerEventData参数传入检测的点坐标：
    PointerEventData pointerData = new PointerEventData (EventSystem.current);
    pointerData.position = Input.mousePosition;
    List<RaycastResult> results = new List<RaycastResult> ();
    EventSystem.current.RaycastAll (pointerData, results);


2. 在3D物体的Camera上挂上PhysicsRaycaster组件即可让3D物体上的脚本响应IPointerClickHandler等事件了（原理同Canvas上都挂有GraphicRaycaster组件一下）。
　　当3D物体之上有UI时则会自动被UI事件所拦截，因为一般UI的显示级别(Camera或overlay)是高于场景中的3D物体的。
　　这样就当点击在UI上时只响应UI事件了，如果同时也要响应3D物体上的事件则只能再次用射线检测点击处是否有3D物体（3D物体一般都有Collider）。

3. ref: www.xuanyusong.com/archives/3327
    void Update ()
    {
            if (Input.GetMouseButtonDown (0) || (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began))
            {
#if UNITY_IOS || UNITY_ANDROID
                if (EventSystem.current.IsPointerOverGameObject (Input.GetTouch (0).fingerId))
#else
                if (EventSystem.current.IsPointerOverGameObject ())
#endif
                    Debug.Log ("当前触摸在UI上");

                else
                    Debug.Log ("当前没有触摸在UI上");
            }
        
    }
    存在问题：
    EventSystem.current.IsPointerOverGameObject ()只能检测到有event存在的组件如Button等，普通Image等UI是不能被检测的，故该方法仅用于简单几个按钮等可交互的UI与3D物体间的判断，当一个大的UI窗口打开时点击在非按钮上如背景图时则失效。


另：
   让3D物体响应点击有个MonoBehaviour自带的消息函数MonoBehaviour.OnMouseDown()可简单实现，但脚本中最好不要有OnMouseXXX的消息处理函数，此类函数会影响游戏性能。（当存在此类函数时build打包结束时有黄色警告提示性能影响）

新手引导案例实践：
　　Graphics Raycaster的Raycast是个虚函数，可以写个Graphics Raycaster的派生类，在默认的Raycast操作执行完以后，用自定义的layer进行筛选，把不需要响应的gameobject去掉。这样就可以实现只响应某个layer的需求了。新手引导中只要把需要响应的gameobject设置为特定layer就行了。
　　实现ICanvasRaycastFilter接口并挂在UI组件上，当有任何UI事件触发时可以实现自定义的判断，如：  

    public class CustomRay : MonoBehaviour, ICanvasRaycastFilter
    {
        public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            //....
            ...
            return true;
            //....
            ...
            return false;
        }
    }
*/
