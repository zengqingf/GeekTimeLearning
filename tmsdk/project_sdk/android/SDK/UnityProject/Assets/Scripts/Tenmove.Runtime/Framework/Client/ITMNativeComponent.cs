

namespace Tenmove.Runtime
{
    public interface ITMNativeComponent
    {
        bool AllowMultiple
        {
            get;
        }

        void DispatchCommand(ITMNativeCommand cmd);
    }
}