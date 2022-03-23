

namespace Tenmove.Runtime
{
    /// <summary>
    /// 继承该抽象类禁止出现任何Unity相关的类，结构，枚举等定义
    /// </summary>
    public interface ITMNativeCommand
    {
    }
     
    public abstract class NativeCommand : ITMNativeCommand
    {
        public NativeCommand()
        {
            TimeStamp = Utility.Time.GetTicksNow();
        }
         
        public long TimeStamp { get; private set; }
    }

}