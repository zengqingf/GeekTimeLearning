

using System.Collections;

namespace Tenmove.Runtime
{
    public interface ITMGameSystem
    {
        ITMProgress EnterSystem<TGameSystem>(bool subSystem, params object[] args) where TGameSystem : GameSystem;

        void CreateModule<TGameModule>(OnModuleCreate<TGameModule> onCreated, params object[] args) where TGameModule : GameModule;
        TClientModule GetModule<TClientModule>() where TClientModule : ClientModule;
        void DestroyModule(GameModule gameModule);
    }
}