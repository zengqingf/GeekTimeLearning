

namespace Tenmove.Runtime
{
    public interface ITMClientSystem
    {
        void CreateModule<TModule>(OnModuleCreate<TModule> onCreated, params object[] args) where TModule : ClientModule;
        TClientModule GetModule<TClientModule>() where TClientModule : ClientModule;
        void DestroyModule(ClientModule clientModule);
    }
}