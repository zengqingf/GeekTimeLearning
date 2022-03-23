

using System;
using System.Collections;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    /// <summary>
    /// 进入客户端阶段前事件参数类
    /// </summary>
    public sealed class OnEnterPhaseEventArgs : BaseEventArgs
    {
        public OnEnterPhaseEventArgs(ITMProgressOperation progressOperation)
        {
            ProgressOperation = progressOperation;
        }

        public ITMProgressOperation ProgressOperation
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// 进入客户端阶段后事件参数类
    /// </summary>
    public sealed class OnPhaseEnteredEventArgs : BaseEventArgs
    {
        public OnPhaseEnteredEventArgs(ITMProgressOperation progressOperation)
        {
            ProgressOperation = progressOperation;
        }

        public ITMProgressOperation ProgressOperation
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// 退出客户端阶段前事件参数类
    /// </summary>
    public sealed class OnLeavePhaseEventArgs : BaseEventArgs
    {
        public OnLeavePhaseEventArgs(ITMProgressOperation progressOperation)
        {
            ProgressOperation = progressOperation;
        }

        public ITMProgressOperation ProgressOperation
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// 退出客户端阶段后事件参数类
    /// </summary>
    public sealed class OnPhaseLeavedEventArgs : BaseEventArgs
    {
        public OnPhaseLeavedEventArgs(ITMProgressOperation progressOperation)
        {
            ProgressOperation = progressOperation;
        }

        public ITMProgressOperation ProgressOperation
        {
            get;
            private set;
        }
    }

    public abstract class ClientPhase
    {
        protected readonly ITMClientApplication m_ClientApplication;
        private readonly int m_Level;

        protected ClientPhase(ITMClientApplication clientApp,int level)
        {
            Debugger.Assert(null != clientApp,"Client application can not be null!");

            m_Level = level;
            m_ClientApplication = clientApp;
        }

        public int Level
        {
            get { return m_Level; }
        }

        public abstract ClientPhase ParentPhase
        {
            get;
        }

        /// <summary>
        /// 当前的客户端阶段（GameSystem或者ClientSystem）
        /// </summary>
        public ClientPhase CurrentPhase
        {
            get
            {
                return m_ClientApplication.CurrentPhase;
            }
        }

        public abstract ClientPhase ActivedPhase
        {
            get;
        }

        public abstract event EventHandler<OnEnterPhaseEventArgs> OnEnterPhaseHandler;
        public abstract event EventHandler<OnPhaseEnteredEventArgs> OnPhaseEnteredHandler;
        public abstract event EventHandler<OnLeavePhaseEventArgs> OnLeavePhaseHandler;
        public abstract event EventHandler<OnPhaseLeavedEventArgs> OnPhaseLeavedHandler;

        public abstract void Update(float elapseSeconds, float realElapseSeconds);

        public abstract bool Enter(ITMProgressOperation progress);
        public abstract bool Leave(ITMProgressOperation progress);

        public abstract TClientModule GetModule<TClientModule>() where TClientModule : ClientModule;

        public abstract bool _EnterSubPhase<TClientModule, TClientPhase>(ITMProgressOperation progress, params object[] args)
            where TClientModule : ClientModule
            where TClientPhase : ClientPhase<TClientModule>;

        public abstract bool _EnterSubPhase(Type subPhaseType, ITMProgressOperation progress, params object[] args);

        public abstract bool _ExitSubPhase(ITMProgressOperation progress);

        public abstract IEnumerator _Close(ITMProgressSegment progressSeg);

        public abstract uint _StartCoroutine(IEnumerator routine);
        public abstract void _StopCoroutine(IEnumerator routine);

        public abstract void _SyncExcuteCoroutine(IEnumerator routine);
        public abstract TNative _CreateNative<TNative>(string nativePath, string name) where TNative : ITMNative;
    }

    public delegate void OnModuleCreate<TClientModule>(TClientModule clientModule) where TClientModule : ClientModule;

    public abstract class ClientPhase<TModule> : ClientPhase where TModule:ClientModule
    {
        private readonly Progress.ProgressSegment DummyProgress = new Progress.ProgressSegment(new Progress(), 1, null);
        private readonly List<TModule> m_Modules;
        private int m_ModuleAllocCount;
        protected readonly ClientPhase m_ParentPhase;
        protected ClientPhase m_SubPhase;

        private event EventHandler<OnEnterPhaseEventArgs> m_OnEnterPhaseEventHandler;
        private event EventHandler<OnPhaseEnteredEventArgs> m_OnPhaseEnteredEventHandler;
        private event EventHandler<OnLeavePhaseEventArgs> m_OnLeavePhaseEventHandler;
        private event EventHandler<OnPhaseLeavedEventArgs> m_OnPhaseLeavedEventHandler;

        protected ClientPhase(ITMClientApplication clientApp, ClientPhase parentPhase)
            : base(clientApp, null == parentPhase ? 0:parentPhase.Level + 1)
        {
            m_ModuleAllocCount = 0;
            m_Modules = new List<TModule>();
            m_ParentPhase = parentPhase;
            m_SubPhase = null;
        }

        public sealed override ClientPhase ActivedPhase
        {
            get
            {
                if (null != m_SubPhase)
                    return m_SubPhase.ActivedPhase;
                return this;
            }
        }

        public sealed override ClientPhase ParentPhase
        {
            get { return m_ParentPhase; }
        }

        public sealed override event EventHandler<OnEnterPhaseEventArgs> OnEnterPhaseHandler
        {
            add { m_OnEnterPhaseEventHandler += value; }
            remove { m_OnEnterPhaseEventHandler -= value; }
        }

        public sealed override event EventHandler<OnPhaseEnteredEventArgs> OnPhaseEnteredHandler
        {
            add { m_OnPhaseEnteredEventHandler += value; }
            remove { m_OnPhaseEnteredEventHandler -= value; }
        }

        public sealed override event EventHandler<OnLeavePhaseEventArgs> OnLeavePhaseHandler
        {
            add { m_OnLeavePhaseEventHandler += value; }
            remove { m_OnLeavePhaseEventHandler -= value; }
        }

        public sealed override event EventHandler<OnPhaseLeavedEventArgs> OnPhaseLeavedHandler
        {
            add { m_OnPhaseLeavedEventHandler += value; }
            remove { m_OnPhaseLeavedEventHandler -= value; }
        }

        public sealed override TClientModule GetModule<TClientModule>()
        {
            Type moduleType = typeof(TClientModule);
            for (int i = 0, icnt = m_Modules.Count; i < icnt; ++i)
            {
                ClientModule curModule = m_Modules[i];
                if (moduleType == curModule.GetType())
                    return curModule as TClientModule;
            }

            if (null != m_ParentPhase)
                return m_ParentPhase.GetModule<TClientModule>();

            return null;
        }

        public void DestroyModule(TModule clientModule)
        {
            if (null != clientModule)
            {
                for (int i = 0, icnt = m_Modules.Count; i < icnt; ++i)
                {
                    ClientModule curModule = m_Modules[i];
                    if (curModule.ModuleID == clientModule.ModuleID)
                    {
                        curModule.OnDestroy();
                        m_Modules.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        public sealed override bool Enter(ITMProgressOperation progress)
        {
            if (_CanEnter())
            {
                if (null != m_OnEnterPhaseEventHandler)
                    m_OnEnterPhaseEventHandler(this, new OnEnterPhaseEventArgs(progress));

                _OnEnter(progress);
                progress.AddFinishCallback(_OnReady);


                if (null != m_OnPhaseEnteredEventHandler)
                    m_OnPhaseEnteredEventHandler(this, new OnPhaseEnteredEventArgs(progress));

                return true;
            }
            else
                Debugger.LogWarning("Can not start phase because new phase [{0}] can not enter yet!", this.GetType());

            return false;
        }

        public sealed override bool Leave(ITMProgressOperation progress)
        {
            if (_CanLeave())
            {
                if (null != m_OnLeavePhaseEventHandler)
                    m_OnLeavePhaseEventHandler(this, new OnLeavePhaseEventArgs(progress));

                _OnLeave(progress);
                progress.AddProgressSegment(0.05f, _Close);

                if (null != m_OnPhaseLeavedEventHandler)
                    m_OnPhaseLeavedEventHandler(this, new OnPhaseLeavedEventArgs(progress));

                return true;
            }
            else
                Debugger.LogWarning("Can not start phase because last phase [{0}] can not exit!", this.GetType());

            return false;
        }

        public sealed override void Update(float elapseSeconds, float realElapseSeconds)
        {
            /// 首先更新这个Phase的模块
            for (int i = 0, icnt = m_Modules.Count; i < icnt; ++i)
            {
                m_Modules[i].Update(elapseSeconds, realElapseSeconds);
            }

            /// 再更新这个Phase的系统
            _OnUpdate(elapseSeconds, realElapseSeconds);

            /// 接下来更新子Phase
            if (null != m_SubPhase)
                m_SubPhase.Update(elapseSeconds, realElapseSeconds);
        }

        public sealed override bool _EnterSubPhase<TClientModule, TClientPhase>(ITMProgressOperation progress,params object[] args)
        {
            if (null != m_SubPhase)
            {
                _OnLeaveSubPhase(progress);
                if (m_SubPhase.Leave(progress))
                    _OnSubPhaseLeaved(progress);
                else
                    return false;
            }

            _OnEnterSubPhase(progress);

            List<object> argList = FrameStackList<object>.Acquire();
            argList.Add(m_ClientApplication);
            argList.Add(this);
            argList.AddRange(args);
            object[] newArgs = argList.ToArray();
            FrameStackList<object>.Recycle(argList);

            Type subPhaseType = typeof(TClientPhase);
            TClientPhase newPhase = Utility.Assembly.CreateInstance(subPhaseType, newArgs) as TClientPhase;

            if (newPhase.Enter(progress))
            {
                /// 是否需要加入回调等进入完毕后在切换
                m_SubPhase = newPhase;
                _OnSubPhaseEntered(progress);
                return true;
            }

            return false;
        }

        public sealed override bool _EnterSubPhase(Type subPhaseType, ITMProgressOperation progress, params object[] args)
        {
            if (null != m_SubPhase)
            {
                _OnLeaveSubPhase(progress);
                if (m_SubPhase.Leave(progress))
                    _OnSubPhaseLeaved(progress);
                else
                    return false;
            }

            _OnEnterSubPhase(progress);

            List<object> argList = FrameStackList<object>.Acquire();
            argList.Add(m_ClientApplication);
            argList.Add(this);
            argList.AddRange(args);
            object[] newArgs = argList.ToArray();
            FrameStackList<object>.Recycle(argList);

            ClientPhase newPhase = Utility.Assembly.CreateInstance(subPhaseType, newArgs) as ClientPhase;
            if (newPhase.Enter(progress))
            {
                /// 是否需要加入回调等进入完毕后在切换
                m_SubPhase = newPhase;
                _OnSubPhaseEntered(progress);
                return true;
            }

            return false;
        }

        public sealed override bool _ExitSubPhase(ITMProgressOperation progress)
        {
            if (null != m_SubPhase)
            {
                _OnLeaveSubPhase(progress);
                if (m_SubPhase.Leave(progress))
                    _OnSubPhaseLeaved(progress);
                else
                    return false;

                m_SubPhase = null;
            }

            return true;
        }

        public sealed override IEnumerator _Close(ITMProgressSegment progressSeg)
        {
            if (null != m_SubPhase)
            {
                yield return m_SubPhase._Close(progressSeg);
                m_SubPhase = null;
            }

            _DestroyAllModules();
        }

        public sealed override uint _StartCoroutine(IEnumerator routine)
        {
            return m_ClientApplication.StartCoroutine(routine);
        }

        public sealed override void _StopCoroutine(IEnumerator routine)
        {
            m_ClientApplication.StopCoroutine(routine);
        }

        public sealed override void _SyncExcuteCoroutine(IEnumerator routine)
        {
            if (null != routine)
            {
                IEnumerator subRoutine = null;
                while (routine.MoveNext())
                {
                    subRoutine = routine.Current as IEnumerator;
                    if (null != subRoutine)
                        _SyncExcuteCoroutine(subRoutine);
                }
            }
        }

        public sealed override TNative _CreateNative<TNative>(string nativePath, string name)
        {
            return m_ClientApplication.CreateNative<TNative>(nativePath, name);
        }

        protected abstract bool _CanEnter();
        protected abstract bool _CanLeave();

        /// <summary>
        /// 进入系统流程
        /// 在此可以加载和初始化本系统的模块
        /// 注意：要保证上个系统的析构和新系统的初始化顺序该函数内只能添加处理协程函数！
        /// 例如：
        ///     protected sealed override void _OnEnter(ITMProgressOperation progress)
        ///     {
        ///         progress.AddProgressSegment(2, _CreateTable);
        ///         progress.AddProgressSegment(2, _CreateBattle);
        ///         progress.AddProgressSegment(1, _CreateBattleUI);
        ///     }
        /// </summary>
        /// <param name="progress"></param>
        protected abstract void _OnEnter(ITMProgressOperation progress);

        /// <summary>
        /// 退出系统流程
        /// 在此可以添加退出系统和析构操作
        /// 注意：要保证上个系统的析构和新系统的初始化顺序该函数内只能添加处理协程函数！
        /// 例如：
        ///     protected sealed override void _OnLeave(ITMProgressOperation progress)
        ///     {
        ///         progress.AddProgressSegment(2, _DestroyBattle);
        ///         progress.AddProgressSegment(1, _DestroyBattleUI);
        ///     }
        /// </summary>
        /// <param name="progress"></param>
        protected abstract void _OnLeave(ITMProgressOperation progress);

        /// <summary>
        /// 系统进入完毕
        /// OnEnter所有的加载和初始化操作完毕会调用该函数
        /// </summary>
        protected abstract void _OnReady();

        /// <summary>
        /// 更新流程
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝的时间</param>
        /// <param name="realElapseSeconds">真实流逝的时间</param>
        protected abstract void _OnUpdate(float elapseSeconds, float realElapseSeconds);

        /// <summary>
        /// 进入子系统前回到用该函数
        /// 注意：要保证上个系统的析构和新系统的初始化顺序该函数内只能添加处理协程函数！
        /// 例如：
        ///     protected sealed override void _OnEnterSubPhase(ITMProgressOperation progress)
        ///     {
        ///         progress.AddProgressSegment(2, _DestroyTown);
        ///         progress.AddProgressSegment(1, _DestroyTownUI);
        ///     }
        /// </summary>
        /// <param name="progress"></param>
        protected abstract void _OnEnterSubPhase(ITMProgressOperation progress);

        /// <summary>
        /// 退出子系统前会调用该函数
        /// 注意：要保证上个系统的析构和新系统的初始化顺序该函数内只能添加处理协程函数！
        /// 例如：
        ///     protected sealed override void _OnLeaveSubPhase(ITMProgressOperation progress)
        ///     {
        ///         progress.AddProgressSegment(1, _CreateTown);
        ///         progress.AddProgressSegment(1, _CreateTownUI);
        ///     }
        /// </summary>
        /// <param name="progress"></param>
        protected abstract void _OnLeaveSubPhase(ITMProgressOperation progress);

        /// <summary>
        /// 进入子系统后调用该函数
        /// 注意：要保证上个系统的析构和新系统的初始化顺序该函数内只能添加处理协程函数！
        /// 例如：
        ///     protected sealed override void _OnSubPhaseEntered(ITMProgressOperation progress)
        ///     {
        ///         progress.AddProgressSegment(2, _DestroyTown);
        ///         progress.AddProgressSegment(1, _DestroyTownUI);
        ///     }
        /// </summary>
        /// <param name="progress"></param>
        protected abstract void _OnSubPhaseEntered(ITMProgressOperation progress);

        /// <summary>
        /// 退出子系统后会调用该函数
        /// 注意：要保证上个系统的析构和新系统的初始化顺序该函数内只能添加处理协程函数！
        /// 例如：
        ///     protected sealed override void _OnSubPhaseLeaved(ITMProgressOperation progress)
        ///     {
        ///         progress.AddProgressSegment(1, _CreateTown);
        ///         progress.AddProgressSegment(1, _CreateTownUI);
        ///     }
        /// </summary>
        /// <param name="progress"></param>
        protected abstract void _OnSubPhaseLeaved(ITMProgressOperation progress);

        protected IEnumerator _CreateModule<TClientModule>(ITMProgressSegment progress,OnModuleCreate<TClientModule> onCreated,params object[] args) where TClientModule : TModule
        {
            Type moduleType = typeof(TClientModule);
            for (int i = 0, icnt = m_Modules.Count; i < icnt; ++i)
            {
                ClientModule curModule = m_Modules[i];
                if (moduleType == curModule.GetType())
                {
                    Debugger.LogWarning("Client phase [{0}] already has module [{1}]", this.GetType(), moduleType);
                    if(null != onCreated)
                        onCreated(curModule as TClientModule);
                }
            }

            List<object> argList = FrameStackList<object>.Acquire();
            argList.Add(this);
            argList.Add(Level);
            argList.Add(m_ModuleAllocCount++);
            argList.AddRange(args);
            object[] newArgs = argList.ToArray();
            FrameStackList<object>.Recycle(argList);

            TClientModule target = Utility.Assembly.CreateInstance(moduleType, newArgs) as TClientModule;
            m_Modules.Add(target);

            yield return target.OnCreate(null == progress ? DummyProgress : progress);
            target.OnReady();
            if(null != onCreated)
                onCreated(target);
        }

        protected void _DestroyAllModules()
        {
            for (int i = m_Modules.Count - 1; 0 <= i; --i)
                m_Modules[i].OnDestroy();
            m_Modules.Clear();
        }
    }
}