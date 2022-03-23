/*
 * 客户端根系统
 * 
 *     负责创建游戏客户端的最基础模块，加载基础配置，异常捕获等等。是整个客户端的顶级系统
 * 
 */

using System.Collections;
using Tenmove.Runtime.EmbedUI;

namespace Tenmove.Runtime
{
    public class ClientSystemRoot : ClientSystem
    {
        private UIFormProgress m_FormBoot;

        public ClientSystemRoot(ITMClientApplication clientApp, ClientSystem parent)
            : base(clientApp, parent)
        {

        }

        protected sealed override bool _CanEnter()
        {
            return true;
        }

        protected sealed override bool _CanLeave()
        {
            return true;
        }

        protected sealed override void _OnEnter(ITMProgressOperation progress)
        {
            progress.AddProgressSegment(2, _CreateModules);
            progress.AddProgressSegment(1, _CreateBootFrame);
        }

        protected sealed override void _OnLeave(ITMProgressOperation progress)
        {
            progress.AddProgressSegment(1, _DestroyBootFrame);      
        }

        protected sealed override void _OnReady()
        {            
            /// 直接开始游戏系统
            ITMProgress progress = EnterSystem(ClientRoot.Instance.EntrySystem, true);
            progress.ProgressUpdateEventHandler += Progress_ProgressUpdateEventHandler;
        }

        protected sealed override void _OnEnterSubPhase(ITMProgressOperation progress)
        {
            progress.AddProgressSegment(1, _CreateBootFrame);
        }

        protected sealed override void _OnLeaveSubPhase(ITMProgressOperation progress)
        {
        }

        protected sealed override void _OnSubPhaseEntered(ITMProgressOperation progress)
        {
        }

        protected sealed override void _OnSubPhaseLeaved(ITMProgressOperation progress)
        {
        }

        private void Progress_ProgressUpdateEventHandler(object sender, ProgressUpdateEventArgs e)
        {
            Debugger.LogInfo("{0} Progress:{1}%", CurrentPhase.GetType(), (int)(e.Progress * 100));
            if (null != m_FormBoot)
            {
                m_FormBoot.SetProgress(e.Progress);
                if(e.Progress >= 1.0f)
                    _StartCoroutine(_DestroyBootFrameDelay(0.1f));
            }
        }

        private IEnumerator _CreateModules(ITMProgressSegment progress)
        {
            yield return CreateModule<ClientModule<ClientRoot>>(progress);


            //yield return CreateModule<ClientModule<EmbedUI.EmbedUIModule>>(progress);            
        }

        private IEnumerator _CreateBootFrame(ITMProgressSegment progress)
        {
            if (null != m_FormBoot)
                m_FormBoot.Destroy();
            m_FormBoot = ClientRoot.Instance.EmbedUI.CreateForm<UIFormProgress>("Base/UI/Prefabs/BootFrame.prefab", new UIFormParams() { UILayer = UIFormLayer.Middle });
            if (null != m_FormBoot)
            {
                m_FormBoot.SetActive(true);
                m_FormBoot.SetInfoMessage("正在启动游戏，请稍后...");
                while (!m_FormBoot.IsFormReady)
                    yield return null;
            }
        }

        private IEnumerator _DestroyBootFrame(ITMProgressSegment progress)
        {
            yield return _DestroyBootFrameDelay(0);
        }

        private IEnumerator _DestroyBootFrameDelay(float seconds)
        {
            if (seconds < 0)
                seconds = 0;

            long timeStramp = Utility.Time.GetTicksNow();
            while (Utility.Time.TicksToSeconds(Utility.Time.GetTicksNow() - timeStramp) < seconds)
                yield return null;

            if (null != m_FormBoot)
            {
                m_FormBoot.Destroy();
                m_FormBoot = null;
            }
        }
    }
}