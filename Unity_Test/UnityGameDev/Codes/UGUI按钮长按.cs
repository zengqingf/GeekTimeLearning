
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class RepeatPressEventTrigger :MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IPointerExitHandler
{
    public float interval=0.1f;  //长按时间间隔，调用LongPress()事件的时间间隔（在周期时间内调用）

    private bool isPointDown=false;
    private float lastInvokeTime;

    // Use this for initialization
    void Start ()
    {
    }

    // Update is called once per frame
    void Update ()
    {

    }

    public void OnPointerDown (PointerEventData eventData)
    {
        m_OnLongpress.Invoke();

        isPointDown = true;

        lastInvokeTime = Time.time;
    }

    public void OnPointerUp (PointerEventData eventData)
    {
        isPointDown = false;
    }

    public void OnPointerExit (PointerEventData eventData)
    {
        isPointDown = false;
    }
}
