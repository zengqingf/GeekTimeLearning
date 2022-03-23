


using System.Collections.Generic;

namespace Tenmove.Runtime
{
    public enum FileSyncPhaseType
    {
        FetchRemoteConfig,
        FetchSyncPackage,
        UnzipSyncPackage,
        PatchSyncPackage,
        VerifySyncFile,
        OnlineOperate,
        Finished,

        Terminated,
        UserCancel,

        MaxPhaseType
    }

    public enum PhaseState
    {
        None,
        Begin,
        Processing,
        Finished,
        Terminated,
    }

    public interface ITMFileSyncPhase
    {
        FileSyncPhaseType PhaseType { get; }
        PhaseState PhaseState { get; }
        float Progress { get; }
    }

    public delegate void OnEndForceWaiting();
    public delegate void OnSyncFolderFinished();
    public delegate void OnQureyUserAction(string message, List<string> selectMsg, List<Function> selectionCallback);
    public delegate NetState OnQureyNetState();

    public interface ITMFileSyncFolder
    {
        event OnEndForceWaiting OnEndForceWaiting;
        event OnSyncFolderFinished OnSyncFolderFinished;
        event OnQureyUserAction OnQureyUserAction;
        event OnQureyNetState OnQureyNetState;

        string FolderName { get; }

        ITMFileSyncPhase CurrentSyncPhase { get; }
        
        string RemoteFolderFullURL { get; }
        string NativeFolderFullPath { get; }
        int NativeFolderFullPathHash { get; }

        string NativeVersionType { get; }
        string HangUpVersionType { get; }

        string ReleaseMD5 { get; }
        string PreviewMD5 { get; }

        string SyncTargetMD5 { get; }

        bool IsNeedPrewarm { get; }
        bool IsForceSync { get; }

        string PullTargetURL { get; }
        uint BytesPerSecond { get; }

        long DownloadBytes { get; }
        long TotalBytes { get;}

        void StartSynchronize();
    }
}