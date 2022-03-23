


using System.Collections.Generic;

namespace Tenmove.Runtime
{
    public interface ITMFileSyncManager
    {
        int MaxDownloaderCount
        {
            get;
        }

        int WaitingTaskCount
        {
            get;
        }

        int FreeAgentCount
        {
            get;
        }

        bool IsPaused
        {
            get;
        }

        List<ITMFileSyncFolder> AllSyncFolder
        {
            get;
        }

        void SetDownloaderCount(int downloaderCount);
        ITMFileSyncFolder CreateSyncFolder(string folderName,string syncFolderURL, string nativeFolderRoot,string packageDataPath, long currentProgram);

        void SetPause(bool isPause);
    }
}