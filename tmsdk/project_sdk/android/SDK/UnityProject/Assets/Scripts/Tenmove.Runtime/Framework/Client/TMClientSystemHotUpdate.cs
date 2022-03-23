/*
 * 客户端热更新系统
 * 
 *     负责客户端的热更新。和游戏系统是同级系统，是根系统的子系统。
 * 
 */

using System.Collections;
using System.Collections.Generic;
using Tenmove.Runtime.EmbedUI;
using Tenmove.Runtime.HAL;

namespace Tenmove.Runtime
{
    public class ClientSystemHotUpdate : ClientSystem
    {
        private UIFormHotUpdate m_FormHotUpdate;
        private ITMFileSyncFolder m_FileSyncFolder;
        private bool m_ProcessHotUpdate;
         
        private readonly string[] StateInfoTable = new string[(int)FileSyncPhaseType.MaxPhaseType] 
        {
            "正在拉取更新配置...",
            "正在下载更新包...",
            "正在解压更新包（此过程不消耗流量）...",
            "正在更新文件（此过程不消耗流量）...",
            "正在校验新文件（此过程不消耗流量）...",
            "正在应用更新文件（此过程不消耗流量）...",
            "更新完成！正在初始化游戏，请稍等...",
            "更新失败！正在初始化游戏，如果不能正常游戏，请到应用商店获取最新安装包。",
            "更新已被用户取消，需要完成更新才能正常游戏！",
        };

        public ClientSystemHotUpdate(ITMClientApplication clientApp, ClientSystem parent)
            : base(clientApp, parent)
        {
            m_FormHotUpdate = null;
            m_FileSyncFolder = null;
            m_ProcessHotUpdate = false;
        }

        protected override bool _CanEnter()
        {
            return true;
        }

        protected override bool _CanLeave()
        {
            return true;
        }

        protected override void _OnEnter(ITMProgressOperation progress)
        {
            progress.AddProgressSegment(1, _PrepareHotUpdate);
            // throw new System.NotImplementedException();
            //  UIManager.Instance.OpenFrame<HotFixFrame>();
        }

        protected override void _OnLeave(ITMProgressOperation progress)
        {
            progress.AddProgressSegment(1, _ClearHotUpdate);
        }

        protected override void _OnReady()
        {
        }

        protected sealed override void _OnEnterSubPhase(ITMProgressOperation progress)
        {
        }

        protected sealed override void _OnLeaveSubPhase(ITMProgressOperation progress)
        {
        }

        protected sealed override void _OnSubPhaseEntered(ITMProgressOperation progress)
        {
            //progress.AddProgressSegment(1, _HideHotUpdate);
        }

        protected sealed override void _OnSubPhaseLeaved(ITMProgressOperation progress)
        {
            progress.AddProgressSegment(1, _PrepareHotUpdate);
        }

        protected override void _OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base._OnUpdate(elapseSeconds, realElapseSeconds);

            if(null != m_FileSyncFolder)
            {
                if (null != m_FormHotUpdate && m_ProcessHotUpdate)
                {
                    m_FormHotUpdate.SetInfoMessage(StateInfoTable[(int)m_FileSyncFolder.CurrentSyncPhase.PhaseType]);
                    m_FormHotUpdate.SetProgress(m_FileSyncFolder.CurrentSyncPhase.Progress);
                    m_FormHotUpdate.SetNetRechabilityStateIcon(ClientRoot.Instance.IsWifiUsed);
                    m_FormHotUpdate.SetNetSpeedInfo(m_FileSyncFolder.BytesPerSecond);
                    m_FormHotUpdate.SetDownloadBytes(m_FileSyncFolder.DownloadBytes, m_FileSyncFolder.TotalBytes);
                }
            }
        }

        private IEnumerator _PrepareHotUpdate(ITMProgressSegment progressSeg)
        {
            if (null == m_FormHotUpdate)
                m_FormHotUpdate = ClientRoot.Instance.EmbedUI.CreateForm<UIFormHotUpdate>("Base/UI/Prefabs/HotUpdateFrame.prefab", new UIFormParams() { UILayer = UIFormLayer.Middle });
            else
                m_FormHotUpdate.SetActive(true);

            if (null != m_FormHotUpdate)
                m_FormHotUpdate.OnFormReady += _OnHotUpdateFormReady;

            //string assetServerURL = Utility.Path.Combine("http://101.37.173.236:58981/mg", _GetPlatformString());
            //string assetServerURL = Utility.Path.Combine("http://static.aldzn.xyimg.net/dnl2",_GetDistributorString(), _GetPlatformString());
            m_FileSyncFolder = ClientRoot.Instance.CreateSyncFolder("AssetBundles", ClientRoot.Instance.ReadWritePath);
            if (null != m_FileSyncFolder)
            {
                m_ProcessHotUpdate = true;
                m_FileSyncFolder.OnEndForceWaiting += _OnEndForceWaiting;
                m_FileSyncFolder.OnSyncFolderFinished += _OnHotUpdateFinished;
                m_FileSyncFolder.OnQureyUserAction += _OnQureyUserSeletion;
                m_FileSyncFolder.OnQureyNetState += _OnQureyNetState;
            }
            else
            {
                MessageBox.OK("游戏更新", "连接热更新服务器失败！点击【跳过】，跳过更新！","跳过",
                    ()=>
                    {
                        _OnEndForceWaiting();
                    });
            }

            yield return null;
        }

        private IEnumerator _ClearHotUpdate(ITMProgressSegment progressSeg)
        {
            if (null != m_FileSyncFolder)
            {
                m_FileSyncFolder.OnEndForceWaiting -= _OnEndForceWaiting;
                m_FileSyncFolder.OnSyncFolderFinished -= _OnHotUpdateFinished;
                m_FileSyncFolder.OnQureyUserAction -= _OnQureyUserSeletion;
                m_FileSyncFolder.OnQureyNetState -= _OnQureyNetState;
            }

            if (null != m_FormHotUpdate)
                m_FormHotUpdate.OnFormReady -= _OnHotUpdateFormReady;

            yield return _HideHotUpdate();
        }

        private IEnumerator _HideHotUpdate()
        {
            yield return _HideHotUpdateFrameDelay(0);
        }

        private void _OnHotUpdateFormReady(IUIForm form)
        {
            UIFormHotUpdate hotUpdateForm = form as UIFormHotUpdate;
            if (null != hotUpdateForm)
            {
                hotUpdateForm.SetInfoMessage("");
                hotUpdateForm.SetProgress(0.0f);
                hotUpdateForm.OnFormReady -= _OnHotUpdateFormReady;
            }
        }

        private void _OnQureyUserSeletion(string message,List<string> selectMsg,List<Function> selectionCallback)
        {
            List<string> btnText = FrameStackList<string>.Acquire();            
            List<MessageBoxCallback> callback = FrameStackList<MessageBoxCallback>.Acquire();
            for(int i = 0;i<3;++i)
            {
                if (i < selectionCallback.Count)
                {
                    btnText.Add(selectMsg[i]);
                    Function func = selectionCallback[i];
                    callback.Add(
                        ()=>
                        {
                            if (null != func)
                                func();
                        });
                }
                else
                {
                    btnText.Add(string.Empty);
                    callback.Add(null);
                }
            }

            MessageBox.Custom("游戏更新", message, btnText[0], btnText[1], btnText[2], callback[0], callback[1], callback[2]);

            FrameStackList<MessageBoxCallback>.Recycle(callback);
            FrameStackList<string>.Recycle(btnText);
        }

        private NetState _OnQureyNetState()
        {
            return ClientRoot.Instance.NetState;
        }

        private void _OnEndForceWaiting()
        {
            Debugger.LogWarning("Enter game client system.");
            ITMProgress progress = EnterSystem("Client.Unity.ClientSystemGame", true);
            progress.ProgressUpdateEventHandler += Progress_ProgressUpdateEventHandler;

            if (null != m_FileSyncFolder)
            {
                m_ProcessHotUpdate = false;
                m_FileSyncFolder.OnEndForceWaiting -= _OnEndForceWaiting;
            }
        }

        private void _OnHotUpdateFinished()
        {
            ClientRoot.Instance.RebootAssetModule();

            Debugger.LogWarning("Enter game client system.");
            ITMProgress progress = EnterSystem("Client.Unity.ClientSystemGame", true);
            progress.ProgressUpdateEventHandler += Progress_ProgressUpdateEventHandler;

            if (null != m_FileSyncFolder)
            {
                m_ProcessHotUpdate = false;
                m_FileSyncFolder.OnSyncFolderFinished -= _OnHotUpdateFinished;
                m_FileSyncFolder.OnQureyUserAction -= _OnQureyUserSeletion;
                m_FileSyncFolder.OnQureyNetState -= _OnQureyNetState;
                m_FileSyncFolder.OnEndForceWaiting -= _OnEndForceWaiting;
            }
        }

        private void Progress_ProgressUpdateEventHandler(object sender, ProgressUpdateEventArgs e)
        {
            if (null != m_FormHotUpdate && !m_ProcessHotUpdate)
            {
                m_FormHotUpdate.SetInfoMessage("正在初始化游戏，请稍后...");
                m_FormHotUpdate.SetProgress(e.Progress);
                m_FormHotUpdate.SetNetRechabilityStateIcon(false);
                m_FormHotUpdate.SetNetSpeedInfo(0);
                m_FormHotUpdate.SetDownloadBytes(0, 0);

                if (e.Progress >= 1.0f)
                    _StartCoroutine(_HideHotUpdateFrameDelay(0.1f));
            }
        }

        private IEnumerator _HideHotUpdateFrameDelay(float seconds)
        {
            if (seconds < 0)
                seconds = 0;

            if (seconds > 0)
            {
                long timeStramp = Utility.Time.GetTicksNow();
                while (Utility.Time.TicksToSeconds(Utility.Time.GetTicksNow() - timeStramp) < seconds)
                    yield return null;
            }

            if (null != m_FormHotUpdate)
                m_FormHotUpdate.SetActive(false);
        }
    }
}