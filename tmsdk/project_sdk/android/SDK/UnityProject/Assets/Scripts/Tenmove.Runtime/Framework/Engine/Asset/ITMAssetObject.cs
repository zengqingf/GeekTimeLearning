
namespace Tenmove.Runtime
{
    public interface ITMAssetObject
    {
        object CreateAssetInst(bool overrideTransform,Math.Transform transform);

        bool Lock();
        void Unlock();

        bool IsWeakRefAsset
        {
            get;
        }

        bool IsInUse
        {
            get;
        }

        int SpawnCount
        {
            get;
        }
    }
}
