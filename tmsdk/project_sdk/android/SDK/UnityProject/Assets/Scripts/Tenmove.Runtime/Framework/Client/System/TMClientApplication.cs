

using System;
using System.Collections;
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    [Procedure(Log = false)]
    public class ClientApplication : ITMClientApplication
    {
        private readonly ITMCoroutineProxy m_CoroutineProxy;
        private readonly ITMNativeFactory m_NativeFactory;

        private readonly List<Type> m_ClientSystemTypeList;

        /// <summary>
        /// 根系统
        /// </summary>
        private ClientPhase m_TopClientPhase;

        public ClientApplication(ITMCoroutineProxy coroutineProxy, ITMNativeFactory nativeFactory)
        {
            Debugger.Assert(null != coroutineProxy, "Coroutine proxy can not be null!");
            Debugger.Assert(null != nativeFactory, "Native factory can not be null!");

            m_CoroutineProxy = coroutineProxy;
            m_NativeFactory = nativeFactory;

            m_ClientSystemTypeList = new List<Type>();
            m_ClientSystemTypeList.AddRange(Utility.Assembly.GetTypesOf(typeof(ClientSystem)));
            m_ClientSystemTypeList.AddRange(Utility.Assembly.GetTypesOf(typeof(GameSystem)));
        }

        public ClientPhase CurrentPhase
        {
            get
            {
                if (null != m_TopClientPhase)
                    return m_TopClientPhase.ActivedPhase;
                else
                    return null;
            }
        }

        public ITMProgress EnterPhase<TModule,TPhase>(ClientPhase parentPhase, params object[] args)
            where TModule : ClientModule
            where TPhase : ClientPhase<TModule>
        {
            Progress newProgress = new Progress();
            if (null == parentPhase)
            {/// 切换顶层级
                if (null != m_TopClientPhase)
                {
                    if (!m_TopClientPhase.Leave(newProgress))
                        return newProgress;
                }

                Type clientPhaseType = typeof(TPhase);
                TPhase newPhase = Utility.Assembly.CreateInstance(clientPhaseType, this, parentPhase) as TPhase;

                if (newPhase.Enter(newProgress))
                {
                    m_TopClientPhase = newPhase;
                    m_CoroutineProxy.StartCoroutine(newProgress.StepProgress());
                }
            }
            else
            {
                if(parentPhase._EnterSubPhase<TModule, TPhase>(newProgress, args))
                    m_CoroutineProxy.StartCoroutine(newProgress.StepProgress());
            }

            return newProgress;
        }

        public ITMProgress EnterPhase(Type phaseType, ClientPhase parentPhase, params object[] args)
        {
            Progress newProgress = new Progress();
            if (null == parentPhase)
            {/// 切换顶层级
                if (null != m_TopClientPhase)
                {
                    if (!m_TopClientPhase.Leave(newProgress))
                        return newProgress;
                }
                
                ClientPhase newPhase = Utility.Assembly.CreateInstance(phaseType, this, parentPhase) as ClientPhase;
                if (newPhase.Enter(newProgress))
                {
                    m_TopClientPhase = newPhase;
                    m_CoroutineProxy.StartCoroutine(newProgress.StepProgress());
                }
            }
            else
            {
                if (parentPhase._EnterSubPhase(phaseType, newProgress, args))
                    m_CoroutineProxy.StartCoroutine(newProgress.StepProgress());
            }

            return newProgress;
        }

        public ITMProgress EnterPhase(string phaseTypeName, ClientPhase parentPhase, params object[] args)
        {
            Type clientPhaseType = null;
            for (int i = 0, icnt = m_ClientSystemTypeList.Count; i < icnt; ++i)
            {
                if (m_ClientSystemTypeList[i].FullName.Contains(phaseTypeName.Trim()))
                {
                    clientPhaseType = m_ClientSystemTypeList[i];
                    break;
                }
            }

            if (null != clientPhaseType)
                return EnterPhase(clientPhaseType, parentPhase, args);
            else
            {
                Progress newProgress = new Progress();
                Debugger.LogWarning("Can not find client system type:'{0}'.", phaseTypeName);
                return newProgress;
            }
        }

        public ITMProgress ExitPhase(ClientPhase phase)
        {
            Progress newProgress = new Progress();
            if (null != phase)
            {
                if(null != phase.ParentPhase)
                {
                    if (phase.ParentPhase._ExitSubPhase(newProgress))
                        m_CoroutineProxy.StartCoroutine(newProgress.StepProgress());
                }
                else
                    Debugger.LogWarning("Current phase is top phase,Top phase can not exit!");
            }

            return newProgress;
        }

        public ITMProgress ExitToPhase(Type targetPhaseType,ClientPhase phase)
        {
            Progress newProgress = new Progress();
            if (null != phase)
            {
                if(targetPhaseType == phase.GetType())
                {
                    Debugger.LogWarning("Current phase '{0}' is target phase:{1}", phase.GetType(), targetPhaseType);
                    return newProgress;
                }

                ClientPhase targetPhase = null;
                ClientPhase curPhase = phase.ParentPhase;
                while(null != curPhase)
                {
                    if(targetPhaseType == curPhase.GetType())
                    {
                        targetPhase = curPhase;
                        break;
                    }

                    curPhase = curPhase.ParentPhase;
                }

                if(null != targetPhase)
                {
                    if(targetPhase._ExitSubPhase(newProgress))
                        m_CoroutineProxy.StartCoroutine(newProgress.StepProgress());
                }
            }

            return newProgress;
        }

        public ITMProgress ExitToPhase(string targetPhaseTypeName, ClientPhase phase)
        {
            Type targetPhaseType = null;
            for (int i = 0, icnt = m_ClientSystemTypeList.Count; i < icnt; ++i)
            {
                if (m_ClientSystemTypeList[i].FullName.Contains(targetPhaseTypeName.Trim()))
                {
                    targetPhaseType = m_ClientSystemTypeList[i];
                    break;
                }
            }

            if (null != targetPhaseType)
                return ExitToPhase(targetPhaseType, phase);
            else
            {
                Progress newProgress = new Progress();
                Debugger.LogWarning("Can not find client system type:'{0}'.", targetPhaseTypeName);
                return newProgress;
            }
        }

        /// <summary>
        /// 协程ID分配的需要做一下
        /// </summary>
        /// <param name="routine"></param>
        /// <returns></returns>
        public uint StartCoroutine(IEnumerator routine)
        {
            m_CoroutineProxy.StartCoroutine(routine);
            return ~0u;
        }

        public void StopCoroutine(IEnumerator routine)
        {
            m_CoroutineProxy.StopCoroutine(routine);
        }

        public TNative CreateNative<TNative>(string nativePath, string name) where TNative: ITMNative
        {
            return m_NativeFactory.CreateNative<TNative>(nativePath, name);
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {            
            if (null != m_TopClientPhase)
                m_TopClientPhase.Update(elapseSeconds, realElapseSeconds);
        }

        public void Shutdown()
        {

        }
    }
}