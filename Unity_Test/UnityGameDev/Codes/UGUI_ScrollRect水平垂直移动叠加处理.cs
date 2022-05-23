/*
   当scrollRect中的一个个子元素也分别是scorllRect时（子scorllRect和父scorllRect运动方向不一样），根据手指的移动判断是响应子scorllRect的drag事件，还是响应父scorllRect的drag事件。
*/

public class VHScrollRect : ScrollRect
{
    
    public ScrollRect parentScroll;
 
    public bool isVertical = false;
 
    private bool isSelf = false;
 
    public override void OnBeginDrag(PointerEventData eventData)
    {
        Vector2 touchDeltaPosition;
#if UNITY_EDITOR
        float delta_x = Input.GetAxis("Mouse X");
        float delta_y = Input.GetAxis("Mouse Y");
        touchDeltaPosition = new Vector2(delta_x, delta_y);
#endif
 
#if UNITY_ANDROID && !UNITY_EDITOR
        touchDeltaPosition = Input.GetTouch(0).deltaPosition;
#endif
        if (isVertical)
        {
            if (Mathf.Abs(touchDeltaPosition.x) < Mathf.Abs(touchDeltaPosition.y))
            {
                isSelf = true;
                base.OnBeginDrag(eventData);
            }
            else
            {
                isSelf = false;
                parentScroll.OnBeginDrag(eventData);
            }
        }
        else
        {
            if (Mathf.Abs(touchDeltaPosition.x) > Mathf.Abs(touchDeltaPosition.y))
            {
                isSelf = true;
                base.OnBeginDrag(eventData);
            }
            else
            {
                isSelf = false;
                parentScroll.OnBeginDrag(eventData);
            }
        }
    }
 
 
    public override void OnDrag(PointerEventData eventData)
    {
        if (isSelf)
        {
            base.OnDrag(eventData);
        }
        else
        {
            parentScroll.OnDrag(eventData);
        }
    }
 
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (isSelf)
        {
            base.OnEndDrag(eventData);
        }
        else
        {
            parentScroll.OnEndDrag(eventData);
        }
    }
	
}