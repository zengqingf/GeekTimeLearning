

namespace Tenmove.Runtime
{
    public enum AssetLoadErrorCode
    {
        OK = 0,
        Unknown,

        NotReady,
        NullAsset,
        NotExist,
        PackageError,
        DependencyLoadError,
        TaskTypeError,
        TypeError,
        InvalidParam,
    }
}