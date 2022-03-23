

namespace Tenmove.Runtime
{
    public static partial class Utility
    {
        public static class Handle
        {
            static public uint AllocHandle(byte handleType, ref uint handleAllocCount)
            {
                if (handleAllocCount + 1 >= uint.MaxValue >> 8)
                    handleAllocCount = 0;
                return (handleAllocCount++) | (((uint)handleType & 0xff) << 24);
            }
        }
    }
}