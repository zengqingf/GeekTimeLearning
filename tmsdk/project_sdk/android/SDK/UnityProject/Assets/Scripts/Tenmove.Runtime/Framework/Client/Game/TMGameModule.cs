/*
 * ��Ϸģ�飨Ϊ��Ϸ�ṩ�ײ㹦�ܵ�ģ���Լ���Ϸ�ĸ���ϵͳ���米�������а������У����졭����
 * 
 *     
 * 
 * 
 */

using System.Collections;

namespace Tenmove.Runtime
{
    public abstract partial class GameModule : ClientModule
    {
        public GameModule(ClientPhase clientPhase,int level,int moduleID)
            : base(clientPhase,level,moduleID)
        {

        }
    }

    public class GameModule<TSingleton> : GameModule where TSingleton : ClientModule.Singleton<TSingleton>
    {
        private readonly TSingleton m_Singleton;
        public override int Priority
        {
            get
            {
                return m_Singleton.Priority;
            }
        }

        public GameModule(ClientPhase clientPhase, int level, int moduleID)
            : base(clientPhase, level, moduleID)
        {
            m_Singleton = Utility.Assembly.CreateInstance(typeof(TSingleton), this) as TSingleton;
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