


using System;
using System.Collections;

namespace Tenmove.Runtime
{
    public interface ITMCoroutineProxy
    {
        void StartCoroutine(IEnumerator routine);
        void StopCoroutine(IEnumerator routine);
    }

    public interface ITMNative
    {
        void Deinit();
    }

    public interface ITMNativeFactory
    {
        TNative CreateNative<TNative>(string nativePath, string name) where TNative : ITMNative; 
    }

    public interface ITMClientApplication
    {
        ClientPhase CurrentPhase
        {
            get;
        }

        ITMProgress EnterPhase<TModule, TPhase>(ClientPhase parentPhase, params object[] args)
            where TModule : ClientModule
            where TPhase : ClientPhase<TModule>;

        ITMProgress EnterPhase(Type systemType, ClientPhase parentPhase, params object[] args);
        ITMProgress EnterPhase(string SystemTypeName, ClientPhase parentPhase, params object[] args);

        ITMProgress ExitPhase(ClientPhase phase);
        ITMProgress ExitToPhase(Type targetPhaseType, ClientPhase phase);
        ITMProgress ExitToPhase(string targetPhaseTypeName, ClientPhase phase);

        uint StartCoroutine(IEnumerator routine);
        void StopCoroutine(IEnumerator routine);

        TNative CreateNative<TNative>(string nativePath, string name) where TNative : ITMNative;
    }
}