

namespace Tenmove.Runtime
{
    public interface ITMDiffBuilder
    {
        bool GenerateDiffFile(string srcFilePath, string dstFilePath, string diffFilePath);
    }
}