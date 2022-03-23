

using System;
using System.Collections;

namespace Tenmove.Runtime
{
    public abstract class GameSystem : ClientPhase<GameModule>,ITMGameSystem
    {
        public GameSystem(ITMClientApplication clientApp, ClientPhase parentSystem)
            : base(clientApp, parentSystem)
        {
        }

        public ITMProgress EnterSystem<TGameSystem>(bool enterSubSystem,params object[] args) where TGameSystem : GameSystem
        {
            return m_ClientApplication.EnterPhase<GameModule, TGameSystem>(enterSubSystem ? this : m_ParentPhase, args);
        }

        public ITMProgress ExitSystem()
        {
            return m_ClientApplication.ExitPhase(this);
        }

        public ITMProgress ExitToSystem(Type targetSystemType)
        {
            return m_ClientApplication.ExitToPhase(targetSystemType, this);
        }

        public ITMProgress ExitToSystem(string targetSystemTypeName)
        {
            return m_ClientApplication.ExitToPhase(targetSystemTypeName, this);
        }

        public void CreateModule<TGameModule>(OnModuleCreate<TGameModule> onCreated,params object[] args) where TGameModule : GameModule
        {
            _StartCoroutine(_CreateModule(null,onCreated,args));
        }

        public IEnumerator CreateModule<TGameModule>(ITMProgressSegment progressSeg, params object[] args) where TGameModule : GameModule
        {
            yield return _CreateModule(progressSeg, (OnModuleCreate<TGameModule>)null, args);
        }

        protected override void _OnReady()
        {
        }

        protected override void _OnEnterSubPhase(ITMProgressOperation progress)
        {
        }

        protected override void _OnLeaveSubPhase(ITMProgressOperation progress)
        {
        }

        protected override void _OnSubPhaseEntered(ITMProgressOperation progress)
        {
        }

        protected override void _OnSubPhaseLeaved(ITMProgressOperation progress)
        {
        }

        protected override void _OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }

        protected override void _OnLeave(ITMProgressOperation progress)
        {
            // 清除lock生命周期为一个GameSystem的资源lable
            TMAssetManagerHelper.ClearAllLables(AssetLockPhase.DuringGameSystem);
        }
    }
}