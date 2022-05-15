    
    /*
    ref: https://www.cnblogs.com/suoluo/p/5535420.html
    NGUI有一个UICenterOnChild脚本，可以轻松实现ScrollView中拖动子物体后保持一个子物体位于中心位置。
    然而UGUI就没这么方便了，官方并没有类似功能的脚本。网上找到一些运行效果都不对，可能因为UGUI需要配置的东西太多，RectTransfrom不同设置效果就不一样。
    故自己实现了该功能，使用时的配置如下：
1. 仅适用于水平方向拖动的ScrollRect。
2. ScrollRect中的Grid必须使用GridLayoutGroup。
3. 由于需要知道ScrollRect的宽度以便计算中心位置，故ScrollRect的Anchors的四个小三角中的上面或者下面的一对角不得分离，不然宽度计算出错，即需要：Anchors.Min.x == Anchors.Max.x。最好四角合一。
4. 由于是通过设置ScrollRect's content的localPosition实现，故需要将ScrollRect的中心点Pivot与content的中心点均置于自身最左边(0, 0.5)。
5. 由于第一个与最后一个子物体需要停留在中间，故ScrollRect的Movement Type需要设置为Unrestricted。该项会在运行时自动设置。
    */
    /// <summary>
    /// 
    /// 拖动ScrollRect结束时始终让一个子物体位于中心位置。
    ///
    /// </summary>
    public class CenterOnChild : MonoBehaviour, IEndDragHandler, IDragHandler
    {
        //将子物体拉到中心位置时的速度
        public float centerSpeed = 9f;

        //注册该事件获取当拖动结束时位于中心位置的子物体
        public delegate void OnCenterHandler (GameObject centerChild);
        public event OnCenterHandler onCenter;

        private ScrollRect _scrollView;
        private Transform _container;

        private List<float> _childrenPos = new List<float> ();
        private float _targetPos;
        private bool _centering = false;

        void Awake ()
        {
            _scrollView = GetComponent<ScrollRect> ();
            if (_scrollView == null)
            {
                Debug.LogError ("CenterOnChild: No ScrollRect");
                return;
            }
            _container = _scrollView.content;

            GridLayoutGroup grid;
            grid = _container.GetComponent<GridLayoutGroup> ();
            if (grid == null)
            {
                Debug.LogError ("CenterOnChild: No GridLayoutGroup on the ScrollRect's content");
                return;
            }

            _scrollView.movementType = ScrollRect.MovementType.Unrestricted;

            //计算第一个子物体位于中心时的位置
            float childPosX = _scrollView.GetComponent<RectTransform> ().rect.width * 0.5f - grid.cellSize.x * 0.5f;
            _childrenPos.Add (childPosX);
            //缓存所有子物体位于中心时的位置
            for (int i = 0; i < _container.childCount - 1; i++)
            {
                childPosX -= grid.cellSize.x + grid.spacing.x;
                _childrenPos.Add (childPosX);
            }
        }

        void Update ()
        {
            if (_centering)
            {
                Vector3 v = _container.localPosition;
                v.x = Mathf.Lerp (_container.localPosition.x, _targetPos, centerSpeed * Time.deltaTime);
                _container.localPosition = v;
                if (Mathf.Abs (_container.localPosition.x - _targetPos) < 0.01f)
                {
                    _centering = false;
                }
            }
        }

        public void OnEndDrag (PointerEventData eventData)
        {
            _centering = true;
            _targetPos = FindClosestPos (_container.localPosition.x);
        }

        public void OnDrag (PointerEventData eventData)
        {
            _centering = false;
        }

        private float FindClosestPos (float currentPos)
        {
            int childIndex = 0;
            float closest = 0;
            float distance = Mathf.Infinity;

            for (int i = 0; i < _childrenPos.Count; i++)
            {
                float p = _childrenPos[i];
                float d = Mathf.Abs (p - currentPos);
                if (d < distance)
                {
                    distance = d;
                    closest = p;
                    childIndex = i;
                }
            }

            GameObject centerChild = _container.GetChild (childIndex).gameObject;
            if (onCenter != null)
                onCenter (centerChild);

            return closest;
        }
    }