

using System.Collections.Generic;

namespace Tenmove.Runtime
{
    public partial class GraphicManager : BaseModule, ITMGraphicManager 
    {
        private readonly ITMNativeObjectManager m_NativeNodeObjManager;
        static private readonly byte m_RequestAllocType = 3;
        private uint m_RequestAllocCount;

        private class DummyObjectLoadDesc
        {
            public OnNativeObjLoaded OnLoaded { set; get; }
            public string ResPath { set; get; }
            public int RequestID { set; get; }
        }

        private readonly List<DummyObjectLoadDesc> m_NativeObjCallbackList;

        private int m_LoadAllocCount;

        public GraphicManager()
        {
            m_NativeObjCallbackList = new List<DummyObjectLoadDesc>();
            m_LoadAllocCount = 0;

            m_NativeNodeObjManager = Utility.Assembly.CreateInstance("Tenmove.Runtime.Unity.UnityNodeObjectManager") as ITMNativeObjectManager; 
        }

        public override int Priority
        {
            get
            {
                return 0;
            }
        }

        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            for(int i = 0,icnt = m_NativeObjCallbackList.Count;i<icnt;++i)
            {
                DummyObjectLoadDesc curOnLoaded = m_NativeObjCallbackList[i];
                if (null != curOnLoaded && null != curOnLoaded.OnLoaded)
                    curOnLoaded.OnLoaded(NativeOperateState.OK, null, curOnLoaded.ResPath, curOnLoaded.RequestID, string.Empty);
            }

            m_NativeObjCallbackList.Clear();
        }

        public override void Shutdown()
        {
        }

        public ITMNativeObjectManager NativeObjectManager
        {
            get { return m_NativeNodeObjManager; }
        }

        private int _DummyLoadObject(string resPath, string[] components, OnNativeObjLoaded onLoaded, EnumHelper<ObjectLoadFlag> loadFlag)
        {
            int requestID = (int)Runtime.Utility.Handle.AllocHandle(m_RequestAllocType, ref m_RequestAllocCount);
            m_NativeObjCallbackList.Add(new DummyObjectLoadDesc() { OnLoaded = onLoaded,ResPath = resPath,RequestID = requestID });
            return requestID;
        }
    }
}