
using System.Diagnostics;

namespace Tenmove.Runtime
{
    public enum TMProfileModule
    {
        Client,
        Engine,
    }

    public static class TMProfiler
    {
        /// <summary>
        /// 性能分析辅助器接口。
        /// </summary>
        public interface IProfilerHelper
        {
            /// <summary>
            /// 开始采样。
            /// </summary>
            /// <param name="name">采样名称。</param>
            void BeginSample(string name);

            /// <summary>
            /// 结束采样。
            /// </summary>
            void EndSample();

            /// <summary>
            /// 设置用户自定义数据
            /// </summary>
            /// <param name="dataName"></param>
            /// <param name="dataValue"></param>
            void SetUserDefinedData(string dataName, int dataValue);
            void IncreUserDefinedData(string dataName, int dataValue);
            void DecreUserDefinedData(string dataName, int dataValue);

            /// <summary>
            /// 添加该帧的事件标志，比如调用了UnloadUnusedAsset
            /// </summary>
            /// <param name="marker"></param>
            void AddFrameMarker(string marker);

            /// <summary>
            /// 设置该帧开始的标志，持续直到调用ClearLabel。
            /// </summary>
            /// <param name="labelName"></param>
            /// <param name="valueName"></param>
            void SetLabel(string labelName, string valueName);

            void ClearLabel(string labelName);
        }

        private static IProfilerHelper s_ProfilerHelper = null;

        private static int m_EnabledModule = ~0;

        /// <summary>
        /// 设置性能分析辅助器。
        /// </summary>
        /// <param name="profilerHelper">要设置的性能分析辅助器。</param>
        [Conditional("ENABLE_PROFILER")]
        public static void SetProfilerHelper(IProfilerHelper profilerHelper)
        {
            s_ProfilerHelper = profilerHelper;
        }

        [Conditional("ENABLE_PROFILER")]
        public static void SetModuleProfilable(TMProfileModule module)
        {
            int flag = 1 << (int)module;

            m_EnabledModule |= flag;
        }

        public static bool IsModuleProfilable(TMProfileModule module)
        {
            int flag = 1 << (int)module;

            return (m_EnabledModule & flag) != 0;
        }

        /// <summary>
        /// 开始采样。
        /// </summary>
        /// <param name="name">采样名称。</param>
        [Conditional("ENABLE_PROFILER")]
        public static void BeginSample(TMProfileModule module, string name)
        {
            if (s_ProfilerHelper != null && IsModuleProfilable(module))
            {
                s_ProfilerHelper.BeginSample(name);
            }     
        }

        /// <summary>
        /// 结束采样。
        /// </summary>
        [Conditional("ENABLE_PROFILER")]
        public static void EndSample(TMProfileModule module, string name)
        {
            if (s_ProfilerHelper != null && IsModuleProfilable(module))
            {
                s_ProfilerHelper.EndSample();
            }
        }

        /// <summary>
        /// 设置用户自定义数据
        /// </summary>
        /// <param name="dataName"></param>
        /// <param name="dataValue"></param>
        [Conditional("ENABLE_PROFILER")]
        public static void SetUserDefinedData(string dataName, int dataValue)
        {
            if (s_ProfilerHelper != null)
                s_ProfilerHelper.SetUserDefinedData(dataName, dataValue);
        }

        /// <summary>
        /// 设置用户自定义数据
        /// </summary>
        /// <param name="dataName"></param>
        /// <param name="dataValue"></param>
        [Conditional("ENABLE_PROFILER")]
        public static void IncreUserDefinedData(string dataName, int dataValue = 1)
        {
            if (s_ProfilerHelper != null)
                s_ProfilerHelper.IncreUserDefinedData(dataName, dataValue);
        }

        /// <summary>
        /// 设置用户自定义数据
        /// </summary>
        /// <param name="dataName"></param>
        /// <param name="dataValue"></param>
        [Conditional("ENABLE_PROFILER")]
        public static void DecreUserDefinedData(string dataName, int dataValue = 1)
        {
            if (s_ProfilerHelper != null)
                s_ProfilerHelper.DecreUserDefinedData(dataName, dataValue);
        }

        /// <summary>
        /// 添加该帧的事件标志，比如调用了UnloadUnusedAsset
        /// </summary>
        /// <param name="marker"></param>
        [Conditional("ENABLE_PROFILER")]
        public static void AddFrameMarker(string marker)
        {
            if (s_ProfilerHelper != null)
                s_ProfilerHelper.AddFrameMarker(marker);
        }

        /// <summary>
        /// 设置该帧开始的标志，持续直到调用ClearLabel。
        /// </summary>
        /// <param name="labelName"></param>
        /// <param name="valueName"></param>
        [Conditional("ENABLE_PROFILER")]
        public static void SetLabel(string labelName, string valueName)
        {
            if (s_ProfilerHelper != null)
                s_ProfilerHelper.SetLabel(labelName, valueName);
        }

        [Conditional("ENABLE_PROFILER")]
        public static void ClearLabel(string labelName)
        {
            if (s_ProfilerHelper != null)
                s_ProfilerHelper.ClearLabel(labelName);
        }
    }
}
