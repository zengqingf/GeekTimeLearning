

namespace Tenmove.Runtime
{
    public enum PatchResult
    {
        None,
        OK,
        Crash,
    }

    public interface ITMDiffPatcher
    {
        PatchResult Result
        {
            get;
        }

        bool RebuildFileByDiff(string srcFilePath, string diffFilePath, string dstFilePath, string dstFileMD5);
        bool BeginRebuildAsync(string srcFilePath, string diffFilePath, string dstFilePath, string dstFileMD5, float timeSlice);
        bool EndRebuildAsync();
    }
}