/*
 * 游戏单例模块
 * 
 */

using System.Collections;

namespace Tenmove.Runtime
{
    public abstract partial class ClientModule
    {
        public abstract class Singleton<TSingleton> where TSingleton : class
        {
            protected readonly ClientModule m_ClientModule;

            static private TSingleton sm_Instance;
            static public TSingleton Instance
            {
                get { return sm_Instance; }
            }

            protected Singleton(ClientModule module)
            {
                Debugger.Assert(null != module, "Client module can not be null!");
                Debugger.Assert(null == sm_Instance, "Static module instance supposed to be null!");
                m_ClientModule = module;
                sm_Instance = this as TSingleton;
            }

            public abstract int Priority
            {
                get;
            }

            public IEnumerator Init(ITMProgressSegment progress)
            {
                yield return _OnInit(progress);
            }

            public IEnumerator Deinit()
            {
                yield return _OnDeinit();
                sm_Instance = null;
            }

            public virtual void Update(float elapseSeconds, float realElapseSeconds) { }

            protected abstract IEnumerator _OnInit(ITMProgressSegment progress);
            protected abstract IEnumerator _OnDeinit();

            protected uint _StartCoroutine(IEnumerator routine)
            {
                return m_ClientModule._StartCoroutine(routine);
            }

            protected void _StopCoroutine(IEnumerator routine)
            {
                m_ClientModule._StopCoroutine(routine);
            }

            protected void _SyncExcuteCoroutine(IEnumerator routine)
            {
                m_ClientModule._SyncExcuteCoroutine(routine);
            }

            protected TNative _CreateNative<TNative>(string nativePath, string name) where TNative : ITMNative
            {
                return m_ClientModule._CreateNative<TNative>(nativePath, name);
            }
        }
    }
}