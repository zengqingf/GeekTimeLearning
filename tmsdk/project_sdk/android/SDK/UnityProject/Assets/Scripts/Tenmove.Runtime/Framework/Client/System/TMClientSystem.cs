


using System;
using System.Collections;

namespace Tenmove.Runtime
{
    public abstract class ClientSystem : ClientPhase<ClientModule>,ITMClientSystem
    {
        public ClientSystem(ITMClientApplication clientApp, ClientSystem parentSystem)
            : base(clientApp, parentSystem)
        {
        }

        public ITMProgress EnterSystem<TModule, TSystem>(bool subSystem, params object[] args) 
            where TModule : ClientModule
            where TSystem : ClientPhase<TModule>
        {
            return m_ClientApplication.EnterPhase<TModule, TSystem>(subSystem ? this : m_ParentPhase, args);
        }

        public ITMProgress EnterSystem(Type systemType, bool subSystem, params object[] args)
        {
            return m_ClientApplication.EnterPhase(systemType, subSystem ? this : m_ParentPhase, args);
        }

        public ITMProgress EnterSystem(string systemTypeName, bool subSystem, params object[] args)
        {
            return m_ClientApplication.EnterPhase(systemTypeName, subSystem ? this : m_ParentPhase, args);
        }

        public ITMProgress ExitSystem()
        {
            return m_ClientApplication.ExitPhase(this);
        }

        public void CreateModule<TModule>(OnModuleCreate<TModule> onCreated, params object[] args) where TModule : ClientModule
        {
            _StartCoroutine(_CreateModule(null,onCreated, args));
        }

        public IEnumerator CreateModule<TModule>(ITMProgressSegment progressSeg,params object[] args) where TModule : ClientModule
        {
            yield return _CreateModule(progressSeg, (OnModuleCreate<TModule>)null, args);
        }

        protected override void _OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }
    }
}