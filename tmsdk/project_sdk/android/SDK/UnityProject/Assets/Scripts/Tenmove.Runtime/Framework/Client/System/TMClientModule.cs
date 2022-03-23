

using System.Collections;

namespace Tenmove.Runtime
{
    public abstract partial class ClientModule
    {
        static public readonly int InvalidID = ~0;

        private readonly ClientPhase m_ClientPhase;
        private readonly int m_Level;
        private readonly int m_ModuleID;

        public ClientModule(ClientPhase clientPhase,int level,int moduleID)
        {
            Debugger.Assert(null != clientPhase, "Client phase can not be null!");
            Debugger.Assert(level >= 0, "Level must be zero or a positive number!");
            Debugger.Assert(InvalidID != moduleID, "Module ID is invalid!");

            m_ClientPhase = clientPhase;
            m_Level = level;
            m_ModuleID = moduleID;
        }

        public abstract int Priority
        {
            get;
        }

        public int Level
        {
            get { return m_Level; }
        }

        public int ModuleID
        {
            get { return m_ModuleID; }
        }    

        public ClientPhase CurrentPhase
        {
            get { return m_ClientPhase.CurrentPhase; }
        }

        internal abstract IEnumerator OnCreate(ITMProgressSegment progress);

        internal abstract void OnDestroy();

        internal abstract void OnReady();

        internal abstract void Update(float elapseSeconds, float realElapseSeconds);

        protected uint _StartCoroutine(IEnumerator routine)
        {
            return m_ClientPhase._StartCoroutine(routine);
        }

        protected void _StopCoroutine(IEnumerator routine)
        {
            m_ClientPhase._StopCoroutine(routine);
        }

        protected void _SyncExcuteCoroutine(IEnumerator routine)
        {
            m_ClientPhase._SyncExcuteCoroutine(routine);
        }
        
        protected TNative _CreateNative<TNative>(string nativePath, string name) where TNative : ITMNative
        {
            return m_ClientPhase._CreateNative<TNative>(nativePath, name);
        }
    }

    public class ClientModule<TSingleton> : ClientModule where TSingleton : ClientModule.Singleton<TSingleton>
    {
        private readonly TSingleton m_Singleton;
        public override int Priority
        {
            get
            {
                return m_Singleton.Priority;
            }
        }

        public ClientModule(ClientPhase clientPhase, int level, int moduleID)
            : base(clientPhase, level, moduleID)
        {
            m_Singleton = Utility.Assembly.CreateInstance(typeof(TSingleton),this) as TSingleton;
        }

        internal override IEnumerator OnCreate(ITMProgressSegment progress)
        {
            yield return m_Singleton.Init(progress);
        }

        internal override void OnDestroy()
        {
            Utility.Coroutine.ExcuteCoroutineAsFunction(m_Singleton.Deinit());
        }

        internal override void OnReady()
        {
        }

        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
            m_Singleton.Update(elapseSeconds, realElapseSeconds);
        }
    }
}