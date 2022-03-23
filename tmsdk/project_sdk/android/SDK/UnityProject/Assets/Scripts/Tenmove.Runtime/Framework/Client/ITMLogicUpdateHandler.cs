

namespace Tenmove.Runtime
{ 
    public interface ITMLogicUpdateHandler
    {
        void OnUpdateLogic(float logicDeltaTime, float realDeltaTime);
    }
}