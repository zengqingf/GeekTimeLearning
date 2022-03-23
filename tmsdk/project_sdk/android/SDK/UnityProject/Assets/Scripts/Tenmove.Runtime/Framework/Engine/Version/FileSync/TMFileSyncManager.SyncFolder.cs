

using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tenmove.Runtime
{
    internal partial class FileSyncManager
    {
        [Procedure(Log = true)]
        protected class SyncFolder : TaskObject,ITMFileSyncFolder
        {
            private static readonly string INTERNAL_FILE_VERIFICATION = "verification.md5";
            private static readonly string INTERNAL_SYNC_TARGET_VERIFICATION = "synctarget.url";
            private static readonly string INTERNAL_FILE_REMOTE_CONFIG = "config.xml";
            private static readonly string INTERNAL_FILE_FILELIST = "filelist.lst";
            private static readonly string INTERNAL_FILE_SYNCLIST = "syncfile.lst";

            private static readonly string NULL_VERSION_MD5 = "00000000000000000000000000000000";

            private enum VersionType
            {
                None,
                Old,
                Release,
                Preview,
            }

            private enum PrewarmState
            {
                Off,
                OnForAll,
                OnForTest,
            }

            private class VersionDesc
            {
                /// <summary>
                /// 当前版本的远端资源校验和MD5，根据这个值判断本地资源和远端资源是否一致
                /// </summary>
                public string RemoteMD5 { set; get; }
                /// <summary>
                /// 当前版本的程序版本号，这个值判断已安装的程序和上线版本是否一致
                /// </summary>
                public long Program { set; get; }
            }

            /// <summary>
            /// 当前的同步上下文
            /// </summary>
            private class SyncContext
            {
                private readonly SyncConfig m_Config;
                private readonly bool m_PreviewSameWithRelease;

                /// <summary>
                /// 挂起的版本
                /// </summary>
                private readonly string m_HangUpMD5;
                /// <summary>
                /// 本地当前正在运行的版本
                /// </summary>
                private readonly string m_NativeMD5;
                
                private VersionType m_Native;
                private VersionType m_HangUp;

                public SyncContext(SyncConfig config,string nativeMD5,string hangUpMD5)
                {
                    Debugger.Assert(null != config,"Config can not be null!");

                    m_Config = config;
                    m_NativeMD5 = nativeMD5;
                    m_HangUpMD5 = hangUpMD5;

                    NeedForceSync = false;

                    m_PreviewSameWithRelease = m_Config.VersionPreview.RemoteMD5.Equals(m_Config.VersionRelease.RemoteMD5, System.StringComparison.OrdinalIgnoreCase);

                    /// 如果远端配置文件Release跟Preview是一样的MD5号 则优先识别为Release版本 这点非常重要
                    /// 如果远端预览版本跟发布版本一致的话 为了避免问题 此时将预热和推送预览标志置为上线状态
                    /// 如果发布版本跟预览版本一致的话 将预览版本视为Disable状态 此时将暂停一切预览版本的预热活动
                    m_Native = CheckVersionType(m_NativeMD5);
                    m_HangUp = CheckVersionType(m_HangUpMD5);
                }

                /// <summary>
                /// 当前版本的本地资源校验和MD5，根据这个值判断本地资源和远端资源是否一致
                /// </summary>
                public string NativeMD5
                {
                    get { return m_NativeMD5; }
                }

                /// <summary>
                /// 当前正在执行同步的版本的MD5
                /// </summary>
                public string HangUpMD5
                {
                    get { return m_HangUpMD5; }
                }

                public VersionType Native
                {
                    get { return m_Native; }
                }

                public VersionType HangUp
                {
                    get { return m_HangUp; }
                }

                public bool IsWhiteListUser
                {
                    get { return m_Config.IsWhiteListUser; }
                }

                public float TimeSlice
                {
                    get { return Utility.Math.Max(m_Config.TimeSlice * (NeedForceSync ? 8.0f : 0.02f),float.Epsilon * 100.0f); }
                }

                public int CacheSize
                {
                    get { return m_Config.CacheSize; }
                }

                public bool PushPreview
                {
                    get { return m_Config.PushPreview; }
                }

                public string VersionPreviewMD5
                {
                    get { return m_Config.VersionPreview.RemoteMD5; }
                }

                public string VersionReleaseMD5
                {
                    get { return m_Config.VersionRelease.RemoteMD5; }
                }

                public bool PreviewSameWithRelease
                {
                    get
                    {
                        return m_PreviewSameWithRelease;
                    }
                }

                public PrewarmState PrewarmState
                {
                    get { return m_Config.PreWarmState; }
                }

                public bool NeedSyncPreview
                {
                    get
                    {
                        return !PreviewSameWithRelease && !PreviewIsNull &&/// 预览版本跟发布版本不一致且预发布版本不是空版本
                            IsWhiteListUser && PushPreview;/// 用户是白名单用户且开启推送预览版本
                    }
                }

                public bool NeedPrewarmPreview
                {
                    get
                    {
                        return !PreviewSameWithRelease && !PreviewIsNull && (PrewarmState.OnForAll == m_Config.PreWarmState ||
                            (IsWhiteListUser && PrewarmState.OnForTest == m_Config.PreWarmState));
                    }
                }

                public bool ReleaseIsNull
                {
                    get { return VersionReleaseMD5.Equals(NULL_VERSION_MD5, System.StringComparison.OrdinalIgnoreCase); }
                }

                public bool PreviewIsNull
                {
                    get { return VersionPreviewMD5.Equals(NULL_VERSION_MD5, System.StringComparison.OrdinalIgnoreCase); }
                }

                /// <summary>
                /// 需要强制同步，这时候玩家必须等待更新同步完成才能进入游戏系统
                /// </summary>
                public bool NeedForceSync { set; get; }
                
                public string PullTargetURL { set; get; }

                public VersionType CheckVersionType(string checkVersionMD5)
                {
                    VersionType resType = VersionType.None;
                    if (string.IsNullOrEmpty(checkVersionMD5))
                        resType = VersionType.None;
                    else if (checkVersionMD5.Equals(m_Config.VersionRelease.RemoteMD5, System.StringComparison.OrdinalIgnoreCase))
                        resType = VersionType.Release;
                    else if (checkVersionMD5.Equals(m_Config.VersionPreview.RemoteMD5, System.StringComparison.OrdinalIgnoreCase))
                        resType = VersionType.Preview;
                    else
                    {
                        bool isOldVersion = false;
                        for (int i = 0, icnt = m_Config.VersionOld.Count; i < icnt; ++i)
                        {
                            if (checkVersionMD5.Equals(m_Config.VersionOld[i].RemoteMD5, System.StringComparison.OrdinalIgnoreCase))
                            {
                                isOldVersion = true;
                                break;
                            }
                        }

                        resType = isOldVersion ? VersionType.Old : VersionType.None;
                    }

                    return resType;
                }
            }

            private class SyncConfig
            {
                private readonly List<VersionDesc> m_VersionOld = new List<VersionDesc>();

                /// <summary>
                /// 强制拉取
                /// 为true拉取过程中需要强制用户等待,为false拉取过程中不需要强制用户等待
                /// </summary>
                public bool ForcePull { set; get; }

                /// <summary>
                /// 预热状态
                /// On:表示开启预热，如果预热目标版本的MD5不是当前的远端版本MD5则执行后台静默拉取
                /// Off:表示关闭预热。
                /// Fallback:表示回退预热,会清除已经下载的东西。
                /// </summary>
                public PrewarmState PreWarmState { set; get; }

                /// <summary>
                /// 当前线上的版本描述
                /// </summary>
                public VersionDesc VersionRelease { set; get; }

                /// <summary>
                /// 当前预览的版本描述
                /// </summary>
                public VersionDesc VersionPreview { set; get; }

                public List<VersionDesc> VersionOld { get { return m_VersionOld; } }

                /// <summary>
                /// 推送预览
                /// 为true则白名单用户会同步至预览版本
                /// </summary>
                public bool PushPreview { set; get; }

                public float TimeSlice { set; get; }

                public int CacheSize { set; get; }

                /// <summary>
                /// 白名单用户
                /// </summary>
                public bool IsWhiteListUser { set; get; }
            }

            private class HttpRequestContext
            {
                public static readonly HttpRequestContext Null = new HttpRequestContext() { Stream = null, Obtainer = null };

                public MemoryStream Stream { set; get; }
                public RemoteDataObtainer Obtainer { set; get; }
                public bool IsFinished { set; get; }
            }

            public class SyncFolderPhase : ITMFileSyncPhase
            {
                public FileSyncPhaseType PhaseType { set; get; }

                public PhaseState PhaseState { set; get; }

                public float Progress { set; get; } 
            }

            private abstract class SyncFile:TaskObject
            {
                public enum SyncState
                {
                    Wait,
                    Run,
                    Done,
                    Terminated,
                }
                protected SyncState m_SyncState;
                protected readonly SyncFolder m_SyncFolder;

                public SyncFile(SyncFolder ownerFolder)
                {
                    Debugger.Assert(null != ownerFolder, "Synchronize folder can not be null!");

                    m_SyncState = SyncState.Wait;
                    m_SyncFolder = ownerFolder;
                }

                public SyncState State
                {
                    get { return m_SyncState; }
                }

                public SyncFolder Folder
                {
                    get { return m_SyncFolder; }
                }

                protected float _TimeSlice
                {
                    get { return m_SyncFolder.Context.TimeSlice; }
                }

                protected int _CacheSize
                {
                    get { return m_SyncFolder.Context.CacheSize; }
                }

                public abstract void StartSynchronize();
            }

            /// <summary>
            /// 对于新增类型的文件什么都不做
            /// </summary>
            private class SyncFileAdd : SyncFile
            {
                private readonly string m_FileName;
                private readonly string m_FileMD5;
                private readonly long m_FileSize;

                public SyncFileAdd(SyncFolder syncFolder,string fileName,string fileMD5,long fileSize)
                    : base(syncFolder)
                {
                    Debugger.Assert(!string.IsNullOrEmpty(fileName),"File name can not be null or empty!");
                    Debugger.Assert(!string.IsNullOrEmpty(fileMD5),"File md5 code can not be null or empty!");
                    Debugger.Assert(fileSize > 0, "File size must be a positive value!");
                    
                    m_FileName = fileName;
                    m_FileMD5 = fileMD5;
                    m_FileSize = fileSize;
                }

                public sealed override void StartSynchronize()
                {
                    m_SyncState = SyncState.Done;
                }

                protected sealed override void _OnTerminate()
                {
                }
            }

            private class SyncFileDiff : SyncFile
            {
                private readonly string m_FileName;
                private readonly string m_OriginFilePath;
                private readonly string m_OriginFilePackagePath;
                private readonly string m_BackupFilePath;
                private readonly string m_PatchedFilePath;
                private readonly string m_DiffFilePath;

                private readonly string m_DstFileMD5;
                private readonly string m_SrcFileMD5;
                
                private readonly long m_DiffFileSize;
                
                private IDataVerifier<string> m_MD5Verifier;
                private ITMDiffPatcher m_DiffPatcher;
                private FileStream m_SrcFileStream;

                public SyncFileDiff(SyncFolder syncFolder, string fileName,string srcFileMD5, string dstFileMD5, long diffFileSize)
                    : base(syncFolder)
                {
                    Debugger.Assert(!string.IsNullOrEmpty(fileName), "File name can not be null or empty!");
                    Debugger.Assert(!string.IsNullOrEmpty(dstFileMD5), "Destinate file md5 code can not be null or empty!");
                    Debugger.Assert(!string.IsNullOrEmpty(srcFileMD5), "Source file md5 code can not be null or empty!");
                    Debugger.Assert(diffFileSize > 0, "File size must be a positive value!");
                    
                    m_FileName = fileName;
                    m_SrcFileMD5 = srcFileMD5;
                    m_DstFileMD5 = dstFileMD5;
                    m_DiffFileSize = diffFileSize;

                    m_OriginFilePath = Utility.Path.Combine(m_SyncFolder.m_SyncFolderFullPath,m_FileName);

                    string backupFolderFullPath = Utility.Path.Combine(m_SyncFolder.m_SyncFolderFullPath,"temp", "old");
                    m_BackupFilePath = Utility.Path.Combine(backupFolderFullPath, Utility.Path.ChangeExtension(m_FileName, "old"));
                    if (!Utility.Directory.Exists(backupFolderFullPath))
                        Utility.Directory.CreateDirectory(backupFolderFullPath);

                    string patchedFolderFullPath = Utility.Path.Combine(m_SyncFolder.m_SyncFolderFullPath, "temp", "new");
                    m_PatchedFilePath = Utility.Path.Combine(patchedFolderFullPath, m_FileName);
                    if (!Utility.Directory.Exists(patchedFolderFullPath))
                        Utility.Directory.CreateDirectory(patchedFolderFullPath);
                    
                    m_DiffFilePath = Utility.Path.Combine(m_SyncFolder.m_SyncFolderFullPath, "temp/dif", string.Format("{0}_{1}.dif", m_SrcFileMD5, m_DstFileMD5));

                    m_SrcFileStream = null;
                    m_MD5Verifier = null;
                    m_DiffPatcher = null;
                }

                public sealed override void StartSynchronize()
                {
                    TaskProcedure<SyncFileDiff> syncFileProc = new TaskProcedure<SyncFileDiff>(_VerifySrcFileMD5,null, _OnPatchFileTerminated);
                    m_SyncFolder.m_FileSyncManager.m_SyncTaskPool.AddTask(this, syncFileProc,m_SyncFolder.FolderName);
                    m_SyncState = SyncState.Run;
                }

                /// <summary>
                /// 第一步验证源文件的MD5码
                /// </summary>
                /// <param name="task"></param>
                /// <returns></returns>
                private static ProcStepResult<SyncFileDiff> _VerifySrcFileMD5(Task<SyncFileDiff> task)
                {
                    if (Utility.File.Exists(task.Content.m_BackupFilePath))
                    {/// 如果存在XXXX.old说明过程重启了
                        return new ProcStepResult<SyncFileDiff>() { NextStep = _PatchingFile, State = ProcedureStepState.Done };
                    }
                    else
                    {
                        if (Utility.File.Exists(task.Content.m_PatchedFilePath))
                        {/// 说明过程重启且这个文件PathPatch存在 说明这个过程完成了或者中断了，检查已经存在的Patch文件的正确性
                            return new ProcStepResult<SyncFileDiff>() { NextStep = _VerifyPatchFileMD5, State = ProcedureStepState.Done };
                        }

                        if (null == task.Content.m_MD5Verifier)
                        {
                            task.Content.m_MD5Verifier = new MD5Verifier();
                            if(!Utility.File.Exists(task.Content.m_OriginFilePath))
                            {/// 如果不存在则从包体内解压出来
                                _ExtractFileFromPackage(task.Content.Folder.m_ApplicationPackageDataPath, task.Content.m_FileName, task.Content.m_OriginFilePath);
                            }

                            task.Content.m_SrcFileStream = Utility.File.OpenRead(task.Content.m_OriginFilePath);
                            if (null != task.Content.m_SrcFileStream)
                            {
                                if (task.Content.m_MD5Verifier.BeginVerify(task.Content.m_SrcFileStream, 64 * 1024, task.Content._TimeSlice))
                                {
                                    task.Content.m_SyncState = SyncState.Run;
                                    return ProcStepResult<SyncFileDiff>.Continue;
                                }
                                else
                                {
                                    Debugger.LogWarning("Begin verify file '{0}' has failed!", task.Content.m_OriginFilePath);
                                    task.Content.m_SrcFileStream.Dispose();
                                    task.Content.m_SrcFileStream = null;
                                }
                            }
                            else
                                Debugger.LogWarning("Open source file '{0}' has failed!", task.Content.m_OriginFilePath);

                            task.Content.m_SyncState = SyncState.Terminated;
                            return ProcStepResult<SyncFileDiff>.Terminated;
                        }
                        else
                        {
                            if (task.Content.m_MD5Verifier.EndVerify())
                            {
                                task.Content.m_SrcFileStream.Dispose();
                                task.Content.m_SrcFileStream = null;

                                string md5Sum = task.Content.m_MD5Verifier.GetVerifySum();
                                if (md5Sum.Equals(task.Content.m_SrcFileMD5, System.StringComparison.OrdinalIgnoreCase))
                                {
                                    if (Utility.File.Exists(task.Content.m_OriginFilePath))
                                    {
                                        Utility.File.Copy(task.Content.m_OriginFilePath, task.Content.m_BackupFilePath, true);
                                        task.Content.m_MD5Verifier = null;
                                        return new ProcStepResult<SyncFileDiff>() { NextStep = _PatchingFile, State = ProcedureStepState.Done };
                                    }
                                    else
                                    {
                                        Debugger.LogWarning("Source file '{0}' does not exist!", task.Content.m_OriginFilePath);
                                        task.Content.m_SyncState = SyncState.Terminated;
                                        return ProcStepResult<SyncFileDiff>.Terminated;
                                    }
                                }
                                else
                                {
                                    Debugger.LogWarning("Source file '{0}' md5 sum [{1}] is mis-match,file crashed!", task.Content.m_OriginFilePath, md5Sum);
                                    task.Content.m_SyncState = SyncState.Terminated;
                                    return ProcStepResult<SyncFileDiff>.Terminated;
                                }
                            }
                            else
                            {
                                task.Content.m_SyncState = SyncState.Run;
                                return ProcStepResult<SyncFileDiff>.Continue;
                            }
                        }
                    }
                }

                private static ProcStepResult<SyncFileDiff> _PatchingFile(Task<SyncFileDiff> task)
                {
                    if(null == task.Content.m_DiffPatcher)
                    {
                        task.Content.m_DiffPatcher = new BSDiffPatch();
                        if (task.Content.m_DiffPatcher.BeginRebuildAsync(task.Content.m_BackupFilePath, task.Content.m_DiffFilePath, task.Content.m_PatchedFilePath, task.Content.m_DstFileMD5, task.Content._TimeSlice))
                        {
                            task.Content.m_SyncState = SyncState.Run;
                            return ProcStepResult<SyncFileDiff>.Continue;
                        }
                        else
                        {
                            Debugger.LogWarning("Begin patch has failed![Old file:'{0}'. Diff file:'{1}'. New file:'{2}'. MD5:'{3}'.].", task.Content.m_BackupFilePath, task.Content.m_DiffFilePath, task.Content.m_PatchedFilePath, task.Content.m_DstFileMD5);

                            task.Content.m_SyncState = SyncState.Terminated;
                            return ProcStepResult<SyncFileDiff>.Terminated;
                        }
                    }
                    else
                    {
                        if (task.Content.m_DiffPatcher.EndRebuildAsync())
                        {
                            if (PatchResult.OK == task.Content.m_DiffPatcher.Result)
                            {
                                return new ProcStepResult<SyncFileDiff>() { NextStep = _ClearCaches, State = ProcedureStepState.Done };
                            }
                            else
                            {/// 失败的话或者直接拉取新包
                                task.Content.m_SyncState = SyncState.Terminated;
                                return ProcStepResult<SyncFileDiff>.Terminated;
                            }
                        }
                        else
                        {
                            task.Content.m_SyncState = SyncState.Run;
                            return ProcStepResult<SyncFileDiff>.Continue;
                        }
                    }

                }

                private static ProcStepResult<SyncFileDiff> _VerifyPatchFileMD5(Task<SyncFileDiff> task)
                {
                    if (null == task.Content.m_MD5Verifier)
                    {
                        task.Content.m_MD5Verifier = new MD5Verifier();
                        task.Content.m_SrcFileStream = Utility.File.OpenRead(task.Content.m_PatchedFilePath);
                        if (null != task.Content.m_SrcFileStream)
                        {
                            if (task.Content.m_MD5Verifier.BeginVerify(task.Content.m_SrcFileStream, 16 * 1024, task.Content._TimeSlice))
                            {
                                task.Content.m_SyncState = SyncState.Run;
                                return ProcStepResult<SyncFileDiff>.Continue;
                            }
                            else
                            {
                                Debugger.LogWarning("Begin verify file '{0}' has failed!", task.Content.m_PatchedFilePath);
                                task.Content.m_SrcFileStream.Dispose();
                                task.Content.m_SrcFileStream = null;
                            }
                        }
                        else
                            Debugger.LogWarning("Open source file '{0}' has failed!", task.Content.m_PatchedFilePath);

                        task.Content.m_SyncState = SyncState.Terminated;
                        return ProcStepResult<SyncFileDiff>.Terminated;
                    }
                    else
                    {
                        if (task.Content.m_MD5Verifier.EndVerify())
                        {
                            task.Content.m_SrcFileStream.Close();
                            task.Content.m_SrcFileStream = null;

                            string md5Sum = task.Content.m_MD5Verifier.GetVerifySum();
                            if (md5Sum.Equals(task.Content.m_DstFileMD5, System.StringComparison.OrdinalIgnoreCase))
                            {
                                return new ProcStepResult<SyncFileDiff>() { State = ProcedureStepState.Done, NextStep = _ClearCaches };
                            }
                            else
                            {/// MD5不匹配说明Patch过程中被终止，删掉目标文件重新Patch
                                Debugger.LogWarning("Source file '{0}' md5 sum [{1}] is mis-match,file crashed!", task.Content.m_PatchedFilePath, md5Sum);
                                try
                                {
                                    Utility.File.Delete(task.Content.m_PatchedFilePath);
                                }
                                catch (System.Exception e)
                                {
                                    //AutoTest.ThrowProbe(e);
                                    Debugger.LogWarning("Delete file '{0}' has failed! Exception:{1}.", task.Content.m_PatchedFilePath, e);
                                    task.Content.m_SyncState = SyncState.Terminated;
                                    return ProcStepResult<SyncFileDiff>.Terminated;
                                }

                                return new ProcStepResult<SyncFileDiff>() { State = ProcedureStepState.Done, NextStep = _VerifySrcFileMD5 };
                            }
                        }
                        else
                        {
                            task.Content.m_SyncState = SyncState.Run;
                            return ProcStepResult<SyncFileDiff>.Continue;
                        }
                    }
                }

                private static ProcStepResult<SyncFileDiff> _ClearCaches(Task<SyncFileDiff> task)
                {
                    if(Utility.File.Exists(task.Content.m_BackupFilePath))
                        Utility.File.Delete(task.Content.m_BackupFilePath);

                    if(Utility.File.Exists(task.Content.m_DiffFilePath))
                        Utility.File.Delete(task.Content.m_DiffFilePath);

                    task.Content.m_SyncState = SyncState.Done;
                    return ProcStepResult<SyncFileDiff>.Finished;
                }


                protected sealed override void _OnTerminate()
                {
                    if(null != m_SrcFileStream)
                    {
                        m_SrcFileStream.Dispose();
                        m_SrcFileStream = null;
                        m_MD5Verifier = null;
                    }
                }
            }

            private class SyncFileDel : SyncFile
            {
                private readonly string m_FileName;
                private SyncState m_FileState;

                public SyncFileDel(SyncFolder syncFolder, string fileName)
                    : base(syncFolder)
                {
                    Debugger.Assert(!string.IsNullOrEmpty(fileName), "File name can not be null or empty!");
                    
                    m_FileName = fileName;
                    m_FileState = SyncState.Wait;
                }

                public sealed override void StartSynchronize()
                {
                    TaskProcedure<SyncFileDel> syncFileProc = new TaskProcedure<SyncFileDel>(_DeleteFile,null, null);
                    m_SyncFolder.m_FileSyncManager.m_SyncTaskPool.AddTask(this, syncFileProc, m_SyncFolder.FolderName);

                    m_SyncState = SyncState.Run;
                }

                private static ProcStepResult<SyncFileDel> _DeleteFile(Task<SyncFileDel> task)
                {
                    if(Utility.Directory.Exists(task.Content.m_SyncFolder.m_SyncFolderFullPath))
                    {
                        string nativeFilePath = Utility.Path.Combine(task.Content.m_SyncFolder.m_SyncFolderFullPath, task.Content.m_FileName);
                        Utility.File.Delete(nativeFilePath);
                    }

                    task.Content.m_SyncState = SyncState.Done;
                    return ProcStepResult<SyncFileDel>.Finished;
                }

                protected sealed override void _OnTerminate()
                {
                }
            }

            /// <summary>
            /// 同步配置
            /// 这个配置是从远端拉取的config.xml中读取出来的配置
            /// </summary>
            private SyncConfig m_SyncConfig;
            private SyncContext m_SyncContext;
            private int m_SyncFolderTaskID;

            private HttpRequestContext m_RemoteDataContext;

            private readonly FileSyncManager m_FileSyncManager;

            private readonly string m_RemoteFolderRootURL;
            private readonly string m_NativeFolderRootPath;
            private readonly string m_ApplicationPackageDataPath;
            private readonly long m_CurrentProgram;

            private readonly string m_SyncFolderName;
            private readonly string m_SyncFolderFullPath;
            private readonly int m_SyncFolderFullPathHash;
            private readonly string m_SyncFolderFullURL;

            private string m_SyncPackageMD5Sum;
            private long m_SyncPackageFileLength;

            private ITMFileDownloader m_SyncPackageDownloader;
            private LibZipUnpacker m_SyncPackageUnpacker;

            private Stream m_VerifyStream;
            private IDataVerifier<string> m_MD5Verifier;

            private class SyncFileDesc
            {
                public string Name { set; get; }
                public string MD5 { set; get; }
            }
            private readonly LinkedList<SyncFileDesc> m_VerifyFileList;
            private LinkedListNode<SyncFileDesc> m_CurVerifyFile;
            private int m_VerifyCount;

            private readonly LinkedList<SyncFileAdd> m_SyncAddList;
            private readonly LinkedList<SyncFileDiff> m_SyncDiffList;
            private readonly LinkedList<SyncFileDel> m_SyncDelList;
            private int m_FinishedSyncFileCount;

            private int m_MaxSyncFileNum;
            private readonly LinkedList<SyncFile> m_RunningSyncFiles;

            private SyncFolderPhase m_SyncFolderPhase;
            
            private long m_LastTimeStamp;
            private bool m_AllowSynchronizeWithoutWIFI;
            private bool m_OnQureyUser;

            private event OnEndForceWaiting m_OnEndForceWaiting;
            private event OnSyncFolderFinished m_OnSyncFolderFinished;
            private event OnQureyUserAction m_OnQureyUserAction;
            private event OnQureyNetState m_OnQureyNetState;

            public SyncFolder(FileSyncManager fileSyncManager,string syncFolderName,string remoteFolderRootURL,string nativeFolderRootPath,string packagePath, long currentProgram, int maxSyncFileNum)
            {
                Debugger.Assert(null != fileSyncManager, "File synchronize manager can not be null!");
                Debugger.Assert(!string.IsNullOrEmpty(syncFolderName), "Synchronize native folder name can not be null or empty!");
                Debugger.Assert(!string.IsNullOrEmpty(nativeFolderRootPath), "Synchronize native folder root path can not be null or empty!");
                Debugger.Assert(!string.IsNullOrEmpty(remoteFolderRootURL), "Synchronize remote folder root URL can not be null or empty!");
                Debugger.Assert(!string.IsNullOrEmpty(packagePath), "Application package data path can not be null or empty!");

                m_SyncAddList = new LinkedList<SyncFileAdd>();
                m_SyncDiffList = new LinkedList<SyncFileDiff>();
                m_SyncDelList = new LinkedList<SyncFileDel>();
                m_FinishedSyncFileCount = 0;
                m_FileSyncManager = fileSyncManager;
                m_SyncConfig = null;
                m_CurrentProgram = currentProgram;
                m_ApplicationPackageDataPath = packagePath;

                if (maxSyncFileNum <= 0)
                    maxSyncFileNum = 3;

                if (maxSyncFileNum > 8)
                    maxSyncFileNum = 8;

                m_MaxSyncFileNum = maxSyncFileNum;
                m_RunningSyncFiles = new LinkedList<SyncFile>();
                m_VerifyFileList = new LinkedList<SyncFileDesc>();
                m_CurVerifyFile = null;

                m_RemoteFolderRootURL = remoteFolderRootURL;
                m_SyncFolderName = syncFolderName;
                m_NativeFolderRootPath = nativeFolderRootPath;
                m_SyncFolderFullPath = Utility.Path.Combine(nativeFolderRootPath, syncFolderName);
                m_SyncFolderFullPathHash = m_SyncFolderFullPath.GetHashCode();
                m_SyncFolderFullURL = Utility.Path.Combine(remoteFolderRootURL, syncFolderName);
                m_RemoteDataContext = HttpRequestContext.Null;
                m_SyncPackageDownloader = null;

                m_SyncFolderPhase = new SyncFolderPhase();
                m_SyncFolderTaskID = TaskPool.InvalidID;
                BytesPerSecond = 0;
                
                m_LastTimeStamp = 0;
                m_AllowSynchronizeWithoutWIFI = false;
                m_OnQureyUser = false;
            }

            public event OnEndForceWaiting OnEndForceWaiting
            {
                add { m_OnEndForceWaiting += value; }
                remove { m_OnEndForceWaiting -= value; }
            }

            public event OnSyncFolderFinished OnSyncFolderFinished
            {
                add { m_OnSyncFolderFinished += value; }
                remove { m_OnSyncFolderFinished -= value; }
            }

            public event OnQureyUserAction OnQureyUserAction
            {
                add { m_OnQureyUserAction += value; }
                remove { m_OnQureyUserAction -= value; }
            }

            public event OnQureyNetState OnQureyNetState
            {
                add { m_OnQureyNetState += value; }
                remove { m_OnQureyNetState -= value; }
            }

            private SyncContext Context
            {
                get { return m_SyncContext; }
            }

            public ITMFileSyncPhase CurrentSyncPhase
            {
                get { return m_SyncFolderPhase; }
            }

            public string FolderName { get { return m_SyncFolderName; } }

            public string RemoteFolderFullURL { get { return m_SyncFolderFullURL; } }

            public string NativeFolderFullPath { get { return m_SyncFolderFullPath; } }
            public int NativeFolderFullPathHash { get { return m_SyncFolderFullPathHash; } }

            public string NativeVersionType { get { return null != m_SyncContext ? m_SyncContext.Native.ToString() : string.Empty; } }

            public string HangUpVersionType { get { return null != m_SyncContext ? m_SyncContext.HangUp.ToString() : string.Empty; } }

            public string ReleaseMD5 { get { return null != m_SyncContext ? m_SyncContext.VersionReleaseMD5 : string.Empty; } }

            public string PreviewMD5 { get { return null != m_SyncContext ? m_SyncContext.VersionPreviewMD5 : string.Empty; } }

            public string SyncTargetMD5 { get { return null != m_SyncContext ? (m_SyncContext.NeedSyncPreview ? PreviewMD5 : ReleaseMD5) : string.Empty; } }

            public bool IsNeedPrewarm { get { return null != m_SyncContext ? m_SyncContext.NeedPrewarmPreview : false; } }

            public string PullTargetURL { get { return null != m_SyncContext ? m_SyncContext.PullTargetURL : string.Empty; } }

            public bool IsForceSync { get { return null != m_SyncContext ? m_SyncContext.NeedForceSync : false; } }

            /// <summary>
            /// 需要安装新的程序包
            /// </summary>
            public bool NeedInstallPackage
            {
                get
                {
                    VersionType native = m_SyncContext.CheckVersionType(
                        _GetFileContentString(Utility.Path.Combine(m_SyncFolderFullPath, INTERNAL_FILE_VERIFICATION)));
                    switch (native)
                    { 
                        case VersionType.Preview:
                            {
                                Debugger.LogWarning("Current program:{0} Preview program:{1}", m_CurrentProgram, m_SyncConfig.VersionPreview.Program);
                                return m_CurrentProgram != m_SyncConfig.VersionPreview.Program;
                            }
                        case VersionType.Release:
                            {
                                Debugger.LogWarning("Current program:{0} Release program:{1}", m_CurrentProgram, m_SyncConfig.VersionRelease.Program);
                                return m_CurrentProgram != m_SyncConfig.VersionRelease.Program;
                            }
                    }
 
                    return false;
                }
            }

            public uint BytesPerSecond { get; private set; }

            public long DownloadBytes { get; private set; }
            public long TotalBytes { get; private set; }

            public void StartSynchronize()
            {
                TaskProcedure<SyncFolder> syncFloderSync = new TaskProcedure<SyncFolder>(_CreateFolderSyncRemoteConfigRequest,_EndWithFileSyncFolder,_TerminatedWithFileSyncFolder);
                if(!m_FileSyncManager.m_SyncTaskPool.IsTaskStart(m_SyncFolderTaskID))
                    m_SyncFolderTaskID = m_FileSyncManager.m_SyncTaskPool.AddTask(this, syncFloderSync, FolderName);
            }

            protected sealed override void _OnTerminate()
            {
            }


            /// <summary>
            /// 创建拉取同步目录远端校验和MD5的请求
            /// </summary>
            /// <param name="task"></param>
            /// <returns></returns>
            static private ProcStepResult<SyncFolder> _CreateFolderSyncRemoteConfigRequest(Task<SyncFolder> task)
            {
                task.Content.m_SyncFolderPhase.PhaseType = FileSyncPhaseType.FetchRemoteConfig;
                task.Content.m_SyncFolderPhase.PhaseState = PhaseState.None;

                //AutoTest.InsertProbe(0x12313178);
                string configUrl = Utility.Path.Combine(task.Content.m_SyncFolderFullURL,INTERNAL_FILE_REMOTE_CONFIG);
                task.Content.m_SyncFolderPhase.Progress = 0.0f;
                if (_CreateRemoteDataRequest(configUrl, out task.Content.m_RemoteDataContext))
                {
                    task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Begin;
                    ProcStepResult<SyncFolder> result = new ProcStepResult<SyncFolder>();
                    result.NextStep = _ProcessFolderSyncRemoteConfigRequest;
                    result.State = ProcedureStepState.Done;
                    return result;
                }
                else
                {
                    task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Terminated;
                    return ProcStepResult<SyncFolder>.Terminated;
                }
            }

            /// <summary>
            /// 处理拉取同步目录远端校验和MD5的请求
            /// </summary>
            /// <param name="task"></param>
            /// <returns></returns>
            static private ProcStepResult<SyncFolder> _ProcessFolderSyncRemoteConfigRequest(Task<SyncFolder> task)
            {
                task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Processing;
                if (_OnFetchRemoteDataRequest(task.Content.m_RemoteDataContext))
                {/// 说明获取完毕准备剖析
                    try
                    {
                        //AutoTest.InsertProbe(0x12345678);
                        task.Content.m_SyncConfig = _ParseConfigXML(task.Content.m_RemoteDataContext.Stream, task.Content.m_FileSyncManager._IsWhiteNameUser);
                        if (null != task.Content.m_SyncConfig)
                        {
                            task.Content.m_SyncFolderPhase.Progress = 1.0f;
                            task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Finished;
                            return new ProcStepResult<SyncFolder>() { NextStep = _InitSyncContext, State = ProcedureStepState.Done };
                        }
                        else
                            Debugger.LogWarning("Parse config xml has failed![Parse content:'{0}']", Encoding.ASCII.GetString(task.Content.m_RemoteDataContext.Stream.GetBuffer()));

                        return ProcStepResult<SyncFolder>.Terminated;
                    }
                    catch (System.Exception e)
                    {
                        //AutoTest.ThrowProbe(e);
                        
                        Debugger.LogWarning("Parse config xml has an exception! URL:{0} exception:'{1}'.", task.Content.m_SyncFolderFullURL, e);
                        return ProcStepResult<SyncFolder>.Terminated;
                    }
                    finally
                    {
                        if (null != task.Content.m_RemoteDataContext.Stream)
                        {
                            task.Content.m_RemoteDataContext.Stream.Dispose();
                            task.Content.m_RemoteDataContext.Stream = null;
                        }

                        if (null != task.Content.m_RemoteDataContext.Obtainer)
                        {
                            task.Content.m_RemoteDataContext.Obtainer.Dispose();
                            task.Content.m_RemoteDataContext.Obtainer = null;
                        }
                    }
                }
                else
                    return ProcStepResult<SyncFolder>.Continue;
            }

            static private ProcStepResult<SyncFolder> _InitSyncContext(Task<SyncFolder> task)
            {
                /// 检查当前的同步状态
                /// 最新：远端上线版本和本地版本一致，没有需要预热的版本【其中有两种情况，预热关闭或者预热版本跟上线版本一致】
                /// 过期：远端上线版本和本地版本不一致，需要立即同步。【立即同步模式有两种，强制等待，会让用户等待更新完毕，非强制等待会允许用户使用，但是同步中的目录资源将禁止访问】
                /// 同步中：同步正在进行还没有结束。【结束后会有版本校验和生成，并且立马将新文件覆盖进正式目录，立即处理删除文件】
                /// 预热中：预热正在进行还没有结束。【结束后会校验预热资源列表中的校验和，将新文件保存在临时目录，挂起要删除的文件】（）
                /// 就绪：预热完成但是远端版本还没有上线。【预热会将新的文件准备好，上个版本需要删除的文件处于挂起状态】（状态判断，synctarget文件被删除，temp目录下有verification文件生成）
                /// 首先检查本地版本资源校验和文件是否存在（可能的状态：） 

                /// 本地会存有一个文件，里面是需要同步的目录中的当前资源文件的MD5校验和,如果没有这个文件或者校验和不匹配，说明需要同步资源。
                string nativeMD5Path = Utility.Path.Combine(task.Content.m_SyncFolderFullPath, INTERNAL_FILE_VERIFICATION);
                if(!Utility.File.Exists(nativeMD5Path))
                {
                    _ExtractFileFromPackage(task.Content.m_ApplicationPackageDataPath, INTERNAL_FILE_VERIFICATION, nativeMD5Path);
                }
                
                string nativeMD5 = _GetFileContentString(Utility.Path.Combine(task.Content.m_SyncFolderFullPath, INTERNAL_FILE_VERIFICATION));

                /// 如果本地的synctarget.md5文件
                string hangUpMD5 = _GetFileContentString(Utility.Path.Combine(task.Content.m_SyncFolderFullPath, "temp", INTERNAL_FILE_VERIFICATION));
                /// 本地如果正在同步过程中，将会在同步缓存目录（temp）下放置一个synctarget.md5文件里面有当前正在同步的远端目录的md5
                string lastSyncTargetURL = _GetFileContentString(Utility.Path.Combine(task.Content.m_SyncFolderFullPath, "temp", INTERNAL_SYNC_TARGET_VERIFICATION));

                task.Content.m_SyncContext = new SyncContext(task.Content.m_SyncConfig,
                    nativeMD5, hangUpMD5);

                Debugger.LogWarning("Native md5:{0},hang up:{1},Need sync preview:{2},Native Version Type:{3},HangUp Version Type:{4}",
                    task.Content.m_SyncContext.NativeMD5,
                    task.Content.m_SyncContext.HangUpMD5,
                    task.Content.m_SyncContext.NeedSyncPreview,
                    task.Content.Context.Native,
                    task.Content.Context.HangUp);

                if (task.Content.Context.NeedSyncPreview)
                {/// 白名单用户且开启预览版测试且预览版跟发布版本不是同一个版本
                    /// 先检查挂起的版本
                    switch (task.Content.Context.HangUp)
                    {
                        case VersionType.Old:
                        case VersionType.Release:
                            {/// 挂起的版本是不需要的版本
                             /// 清理缓存
                                task.Content._ClearSyncCaches();
                                switch (task.Content.Context.Native)
                                {
                                    case VersionType.Old:
                                    case VersionType.Release:
                                        {
                                            /// 强制同步
                                            task.Content.Context.NeedForceSync = true;
                                            task.Content.Context.PullTargetURL = Utility.Path.Combine(task.Content.m_SyncFolderFullURL, "diff",
                                                task.Content.Context.NativeMD5, task.Content.Context.VersionPreviewMD5);
                                        }
                                        break;
                                    case VersionType.None:
                                        {
                                            /// 强制同步
                                            task.Content.Context.NeedForceSync = true;
                                            task.Content.Context.PullTargetURL = Utility.Path.Combine(task.Content.m_SyncFolderFullURL, "full",
                                                task.Content.Context.VersionPreviewMD5);
                                        }
                                        break;
                                    case VersionType.Preview:
                                        {/// 当前版本就是预览版 什么也不做
                                            task.Content.Context.NeedForceSync = false;
                                            return ProcStepResult<SyncFolder>.Finished;
                                        }
                                }
                            }
                            break;
                        case VersionType.None:
                            {/// 没有挂起的版本
                             /// 检查正在同步的版本
                                string synchingURL = string.Empty;
                                switch (task.Content.Context.Native)
                                {
                                    case VersionType.Old:
                                    case VersionType.Release:
                                        {
                                            /// 强制同步
                                            task.Content.Context.NeedForceSync = true;
                                            synchingURL = Utility.Path.Combine(task.Content.m_SyncFolderFullURL, "diff",
                                                task.Content.Context.NativeMD5, task.Content.Context.VersionPreviewMD5);
                                        }
                                        break;
                                    case VersionType.None:
                                        {
                                            /// 强制同步
                                            task.Content.Context.NeedForceSync = true;
                                            synchingURL = Utility.Path.Combine(task.Content.m_SyncFolderFullURL, "full",
                                                task.Content.Context.VersionPreviewMD5);
                                        }
                                        break;
                                    case VersionType.Preview:
                                        {/// 当前版本就是预览版
                                            /// 清理缓存
                                            task.Content._ClearSyncCaches();
                                            task.Content.Context.NeedForceSync = false;

                                            return ProcStepResult<SyncFolder>.Finished;
                                        }
                                }

                                if(!synchingURL.Equals(lastSyncTargetURL))
                                {/// 不一致则清理缓存重新开始同步
                                    /// 清理缓存
                                    task.Content._ClearSyncCaches();
                                }

                                /// 继续或开始同步
                                task.Content.Context.PullTargetURL = synchingURL;
                            }
                            break;
                        case VersionType.Preview:
                            {/// 挂起的版本正是预览版本
                             /// 执行覆盖上线操作
                             /// 强制同步
                                task.Content.Context.NeedForceSync = true;                           
                                return new ProcStepResult<SyncFolder>() { NextStep = _BeginOnlineOperation, State = ProcedureStepState.Done };
                            }
                    }                    
                }
                else
                {/// 强制同步的版本是线上版本 根据选项开启预览版的预热
                    if (task.Content.Context.ReleaseIsNull)
                    {/// 如果当前发布版本是空版本 说明这是第一个版本 跳过更新
                        return ProcStepResult<SyncFolder>.Finished;
                    }
                    else
                    {
                        /// 先检查挂起的版本
                        switch (task.Content.Context.HangUp)
                        {
                            case VersionType.Release:
                                {
                                    switch (task.Content.Context.Native)
                                    {
                                        case VersionType.None:
                                        case VersionType.Old:
                                        case VersionType.Preview:
                                            {/// 本地版本是不需要的版本
                                                task.Content.Context.NeedForceSync = true;
                                                return new ProcStepResult<SyncFolder>() { NextStep = _BeginOnlineOperation, State = ProcedureStepState.Done };
                                            }
                                            break;
                                        case VersionType.Release:
                                            {/// 本地版本就是Release版本
                                                task.Content._ClearSyncCaches();
                                                if (task.Content.Context.NeedPrewarmPreview)
                                                {
                                                    task.Content.Context.NeedForceSync = false;
                                                    task.Content.Context.PullTargetURL = Utility.Path.Combine(task.Content.m_SyncFolderFullURL, "diff",
                                                        task.Content.Context.NativeMD5, task.Content.Context.VersionPreviewMD5);
                                                }
                                                else
                                                {
                                                    return ProcStepResult<SyncFolder>.Finished;
                                                }
                                            }
                                            break;
                                    }
                                }
                                break;
                            case VersionType.Preview:
                                {
                                    switch (task.Content.Context.Native)
                                    {
                                        case VersionType.None:
                                            {/// 本地版本未知
                                             /// 清理缓存
                                                task.Content._ClearSyncCaches();
                                                /// 强制同步
                                                task.Content.Context.NeedForceSync = true;
                                                task.Content.Context.PullTargetURL = Utility.Path.Combine(task.Content.m_SyncFolderFullURL, "full",
                                                    task.Content.Context.VersionReleaseMD5);
                                            }
                                            break;
                                        case VersionType.Old:
                                        case VersionType.Preview:
                                            {/// 本地版本不是线上版本
                                             /// 清理缓存
                                                task.Content._ClearSyncCaches();
                                                /// 强制同步
                                                task.Content.Context.NeedForceSync = true;
                                                task.Content.Context.PullTargetURL = Utility.Path.Combine(task.Content.m_SyncFolderFullURL, "diff",
                                                    task.Content.Context.NativeMD5, task.Content.Context.VersionReleaseMD5);
                                            }
                                            break;
                                        case VersionType.Release:
                                            {/// 本地版本就是线上版本且挂起的版本是预览版本 什么也不做
                                                task.Content.Context.NeedForceSync = false;
                                                return ProcStepResult<SyncFolder>.Finished;
                                            }
                                    }
                                }
                                break;
                            case VersionType.Old:
                                {/// 挂起的版本是老版本
                                 /// 清理缓存
                                    task.Content._ClearSyncCaches();
                                    switch (task.Content.Context.Native)
                                    {
                                        case VersionType.None:
                                            {/// 本地版本未知
                                             /// 清理缓存
                                             /// 强制同步
                                                task.Content.Context.NeedForceSync = true;
                                                task.Content.Context.PullTargetURL = Utility.Path.Combine(task.Content.m_SyncFolderFullURL, "full",
                                                    task.Content.Context.VersionReleaseMD5);
                                            }
                                            break;
                                        case VersionType.Old:
                                        case VersionType.Preview:
                                            {/// 本地版本不是线上版本
                                             /// 清理缓存
                                                task.Content._ClearSyncCaches();
                                                /// 强制同步
                                                task.Content.Context.NeedForceSync = true;
                                                task.Content.Context.PullTargetURL = Utility.Path.Combine(task.Content.m_SyncFolderFullURL, "diff",
                                                    task.Content.Context.NativeMD5, task.Content.Context.VersionReleaseMD5);
                                            }
                                            break;
                                        case VersionType.Release:
                                            {/// 本地版本就是线上版本且挂起的版本是预览版本 什么也不做
                                                task.Content._ClearSyncCaches();
                                                task.Content.Context.NeedForceSync = false;
                                                task.Content.Context.PullTargetURL = Utility.Path.Combine(task.Content.m_SyncFolderFullURL, "diff",
                                                    task.Content.Context.NativeMD5, task.Content.Context.VersionPreviewMD5);
                                            }
                                            break;
                                    }
                                }
                                break;
                            case VersionType.None:
                                {/// 如果挂起的版本是空
                                    string synchingURL = string.Empty;
                                    switch (task.Content.Context.Native)
                                    {
                                        case VersionType.None:
                                            {
                                                task.Content.Context.NeedForceSync = true;
                                                synchingURL = Utility.Path.Combine(task.Content.m_SyncFolderFullURL, "full",
                                                    task.Content.Context.VersionReleaseMD5);
                                            }
                                            break;
                                        case VersionType.Old:
                                        case VersionType.Preview:
                                            {
                                                task.Content.Context.NeedForceSync = true;
                                                synchingURL = Utility.Path.Combine(task.Content.m_SyncFolderFullURL, "diff",
                                                    task.Content.Context.NativeMD5, task.Content.Context.VersionReleaseMD5);
                                            }
                                            break;
                                        case VersionType.Release:
                                            {
                                                task.Content.Context.NeedForceSync = false;
                                                if (task.Content.Context.NeedPrewarmPreview)
                                                {
                                                    synchingURL = Utility.Path.Combine(task.Content.m_SyncFolderFullURL, "diff",
                                                        task.Content.Context.NativeMD5, task.Content.Context.VersionPreviewMD5);
                                                }
                                                else
                                                {
                                                    return ProcStepResult<SyncFolder>.Finished;
                                                }
                                            }
                                            break;
                                    }

                                    if (!synchingURL.Equals(lastSyncTargetURL))
                                    {/// 不一致则清理缓存重新开始同步
                                     /// 清理缓存
                                        task.Content._ClearSyncCaches();
                                    }

                                    /// 继续或开始同步
                                    task.Content.Context.PullTargetURL = synchingURL;
                                }
                                break;
                        }
                    }
                }

                ProcStepResult<SyncFolder> result = new ProcStepResult<SyncFolder>();
                result.State = ProcedureStepState.Done;
                result.NextStep = _CreateFetchSyncPackageMD5Request;

                if(!task.Content.Context.NeedForceSync)
                {
                    if (null != task.Content.m_OnEndForceWaiting)
                        task.Content.m_OnEndForceWaiting();
                }

                return result;
            }

            /// <summary>
            /// 创建拉取远端更新包校验和的请求，更新包的Url组合方式是：远端服务器根目录 + 平台 + 同步目录名 + 资源校验和.md5
            /// 如果远端校验和和本地校验和一致的话则跳过该步骤。
            /// </summary>
            /// <param name="task"></param>
            /// <returns></returns>
            static private ProcStepResult<SyncFolder> _CreateFetchSyncPackageMD5Request(Task<SyncFolder> task)
            {
                task.Content.m_SyncFolderPhase.PhaseType = FileSyncPhaseType.FetchSyncPackage;
                task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Begin;
                task.Content.m_SyncFolderPhase.Progress = 0.0f;

                /// 有个问题如果在压缩文件下载完毕后解压过程中或者解压结束后应用非正常终止，下一次继续的时候该如何处理
                /// 由于存在断点续传的问题如果存在syncfile.lst且zip文件不存在 则说明压缩包下载成功且解压成功，
                /// 跳过压缩包拉取和解压的步骤，否则执行上述步骤
                /// 检查syncfile.lst文件和zip文件
                string tempFolderPath = Utility.Path.Combine(task.Content.m_SyncFolderFullPath, "temp");
                if (Utility.Directory.Exists(tempFolderPath))
                {
                    string syncPackageFullPath = Utility.Path.Combine(tempFolderPath, Utility.Path.ChangeExtension(Utility.Path.GetFileName(task.Content.Context.PullTargetURL),
                        "zip"));

                    if (!Utility.File.Exists(syncPackageFullPath) && Utility.File.Exists(Utility.Path.Combine(tempFolderPath, INTERNAL_FILE_SYNCLIST)))
                    {/// 说明压缩包解压成功 跳过拉取压缩包过程 如果压缩包还存在说明压缩包未下载完或者压缩包解压未完成

                        if (task.Content._ParseSyncDescFile())
                            return new ProcStepResult<SyncFolder>() { NextStep = _PatchSyncFile, State = ProcedureStepState.Done };

                        task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Terminated;
                        return ProcStepResult<SyncFolder>.Terminated;
                    }
                }
                else
                    Utility.Directory.CreateDirectory(tempFolderPath);

                string syncTargetURL = Utility.Path.Combine(tempFolderPath, INTERNAL_SYNC_TARGET_VERIFICATION);
                using (Stream stream = Utility.File.OpenWrite(syncTargetURL, true))
                {
                    byte[] urlData = System.Text.Encoding.ASCII.GetBytes(task.Content.Context.PullTargetURL);
                    stream.Write(urlData, 0, urlData.Length);
                    stream.Flush();
                }

                /// 要根据状态判定到底拉取那个版本
                /// 如果是过期和正在同步的状态则拉取发布版本的同步包。
                /// 如果是预热状态则拉取预发布版本的同步包。
                string remoteSyncPackageMD5URL = Utility.Path.ChangeExtension(task.Content.Context.PullTargetURL, "md5");
                if (_CreateRemoteDataRequest(remoteSyncPackageMD5URL, out task.Content.m_RemoteDataContext))
                {
                    task.Content.m_SyncFolderPhase.Progress = 0.01f;
                    ProcStepResult<SyncFolder> result = new ProcStepResult<SyncFolder>();
                    result.NextStep = _ProcessFetchSyncPackageMD5Request;
                    result.State = ProcedureStepState.Done;
                    return result;
                }
                else
                {
                    task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Terminated;
                    return ProcStepResult<SyncFolder>.Terminated;
                }
            }

            static private ProcStepResult<SyncFolder> _ProcessFetchSyncPackageMD5Request(Task<SyncFolder> task)
            {
                //AutoTest.InsertProbe(0x2321afbc);
                
                if (_OnFetchRemoteDataRequest(task.Content.m_RemoteDataContext))
                {/// 说明获取完毕准备剖析
                    try
                    {
                        task.Content.m_SyncFolderPhase.Progress = 0.02f;
                        string[] splits = ASCIIEncoding.ASCII.GetString(task.Content.m_RemoteDataContext.Stream.GetBuffer()).Trim().Split(',');
                        if (null != splits && 2 == splits.Length)
                        {
                            task.Content.m_SyncPackageMD5Sum = splits[0];
                            if (long.TryParse(splits[1], out task.Content.m_SyncPackageFileLength))
                            {
                                if (null != task.Content.m_OnQureyNetState)
                                {
                                    NetState curState = task.Content.m_OnQureyNetState();
                                    _NetStateHasChange(curState, task.Content);
                                }

                                task.Content.m_SyncFolderPhase.Progress = 0.03f;
                                ProcStepResult<SyncFolder> result = new ProcStepResult<SyncFolder>();
                                result.NextStep = _CreateFetchSyncPackageRequest;
                                result.State = ProcedureStepState.Done;
                                return result;
                            }
                            else
                                Debugger.LogWarning("Parse sync package zip file size has failed!");
                        }
                        else
                            Debugger.LogWarning("Parse sync package md5 sum has failed!");

                        task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Terminated;
                        return ProcStepResult<SyncFolder>.Terminated;
                    }
                    catch (System.Exception e)
                    {
                        //AutoTest.ThrowProbe(e);
                        Debugger.LogWarning("Parse sync package md5 sum has failed! exception:'{0}'.", e);
                        task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Terminated;
                        return ProcStepResult<SyncFolder>.Terminated;
                    }
                    finally
                    {
                        if (null != task.Content.m_RemoteDataContext.Stream)
                        {
                            task.Content.m_RemoteDataContext.Stream.Dispose();
                            task.Content.m_RemoteDataContext.Stream = null;
                        }

                        if (null != task.Content.m_RemoteDataContext.Obtainer)
                        {
                            task.Content.m_RemoteDataContext.Obtainer.Dispose();
                            task.Content.m_RemoteDataContext.Obtainer = null;
                        }
                    }
                }
                else
                {
                    task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Processing;
                    return ProcStepResult<SyncFolder>.Continue;
                }
            }

            /// <summary>
            /// 创建拉取远端更新包校验和的请求，更新包的Url组合方式是：远端服务器根目录 + 平台 + 同步目录名 + 资源校验和.md5
            /// </summary>
            /// <param name="task"></param>
            /// <returns></returns>
            static private ProcStepResult<SyncFolder> _CreateFetchSyncPackageRequest(Task<SyncFolder> task)
            {
                if (task.Content.m_OnQureyUser)
                {
                    task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Processing;
                    return ProcStepResult<SyncFolder>.Continue;
                }
                else
                {
                    ITMFileDownloader fileDownloader = task.Content.m_FileSyncManager._CreateFileDownloader();
                    if (null != fileDownloader)
                    {
                        task.Content.m_SyncFolderPhase.Progress = 0.04f;
                        string remoteSyncPackageURL = Utility.Path.ChangeExtension(task.Content.Context.PullTargetURL, "zip");
                        string nativeDescFilePath = Utility.Path.Combine(task.Content.m_SyncFolderFullPath,
                            "temp", Utility.Path.GetFileName(remoteSyncPackageURL));

                        if (fileDownloader.CreateDownloadRequest(remoteSyncPackageURL, nativeDescFilePath, task.Content.m_SyncPackageFileLength, task.Content.m_SyncPackageMD5Sum, task.Content.Context.CacheSize, task.Content.Context.TimeSlice, 3))
                        {
                            task.Content.m_SyncFolderPhase.Progress = 0.05f;
                            task.Content.m_SyncPackageDownloader = fileDownloader;

                            ProcStepResult<SyncFolder> result = new ProcStepResult<SyncFolder>();
                            result.NextStep = _ProcessFetchSyncPackageRequest;
                            result.State = ProcedureStepState.Done;
                            return result;
                        }
                        else
                        {
                            task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Terminated;
                            return ProcStepResult<SyncFolder>.Terminated;
                        }
                    }
                    else
                    {
                        task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Processing;
                        return ProcStepResult<SyncFolder>.Continue;
                    }
                }
            }

            /// <summary>
            /// 处理拉取远端更新包校验和的请求
            /// </summary>
            /// <param name="task"></param>
            /// <returns></returns>
            static private ProcStepResult<SyncFolder> _ProcessFetchSyncPackageRequest(Task<SyncFolder> task)
            {
                task.Content.m_SyncFolderPhase.Progress = 0.05f + (task.Content.m_SyncPackageDownloader.Progress + 0.005f) * 0.95f;
                if (task.Content.m_SyncPackageDownloader.StepDownload())
                {
                    if (task.Content.m_SyncPackageDownloader.IsSuccess)
                    {
                        /// 下载结束 下载速度归零
                        task.Content.BytesPerSecond = 0;
                        task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Finished;
                        task.Content.m_SyncFolderPhase.Progress = 1.0f;
                        ProcStepResult<SyncFolder> result = new ProcStepResult<SyncFolder>();
                        result.NextStep = _UnpackSyncPackage;
                        result.State = ProcedureStepState.Done;
                        return result;
                    }
                    else
                    {
                        task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Terminated;
                        return ProcStepResult<SyncFolder>.Terminated;
                    }
                }
                else
                {                     
                    _UpdateDownloadSpeed(task.Content);
                    task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Processing;
                    return ProcStepResult<SyncFolder>.Continue;
                }
            }

            static private ProcStepResult<SyncFolder> _UnpackSyncPackage(Task<SyncFolder> task)
            {
                task.Content.m_SyncFolderPhase.PhaseType = FileSyncPhaseType.UnzipSyncPackage;

                if (null == task.Content.m_SyncPackageUnpacker)
                {
                    task.Content.m_SyncFolderPhase.Progress = 0.0f;
                    task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Begin;
                    string tempFolderPath = Utility.Path.Combine(task.Content.m_SyncFolderFullPath, "temp");
                    string syncPackageFullPath = Utility.Path.Combine(tempFolderPath, Utility.Path.ChangeExtension(Utility.Path.GetFileName(task.Content.Context.PullTargetURL),
                        "zip"));

                    task.Content.m_SyncPackageUnpacker = new LibZipUnpacker(syncPackageFullPath, tempFolderPath);
                    return ProcStepResult<SyncFolder>.Continue;
                }
                else
                {
                    task.Content.m_SyncFolderPhase.Progress = task.Content.m_SyncPackageUnpacker.Progress * 0.95f;
                    if (task.Content.m_SyncPackageUnpacker.UnpackAll(0.004f))
                    {
                        if (task.Content.m_SyncPackageUnpacker.HasError)
                        {
                            task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Terminated;
                            return ProcStepResult<SyncFolder>.Terminated;
                        }
                        else
                        {
                            task.Content.m_SyncFolderPhase.Progress = 0.98f;
                            /// 删除zip文件
                            Utility.File.Delete(task.Content.m_SyncPackageUnpacker.PackageFilePath);
                            task.Content.m_SyncPackageUnpacker = null;
                            if (task.Content._ParseSyncDescFile())
                            {
                                task.Content.m_SyncFolderPhase.Progress = 1.0f;
                                task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Finished;
                                return new ProcStepResult<SyncFolder>() { NextStep = _PatchSyncFile, State = ProcedureStepState.Done };
                            }

                            task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Terminated;
                            return ProcStepResult<SyncFolder>.Terminated;
                        }
                    }
                    else
                    {
                        task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Processing;
                        return ProcStepResult<SyncFolder>.Continue;
                    }
                }
            }

            static private ProcStepResult<SyncFolder> _PatchSyncFile(Task<SyncFolder> task)
            {
                task.Content.m_SyncFolderPhase.PhaseType = FileSyncPhaseType.PatchSyncPackage;

                /// 更新正在运行的任务
                LinkedListNode<SyncFile> curSyncFile = task.Content.m_RunningSyncFiles.First;
                LinkedListNode<SyncFile> nextSyncFile;
                while (null != curSyncFile)
                {
                    nextSyncFile = curSyncFile.Next;
                    if (SyncFile.SyncState.Done == curSyncFile.Value.State)
                    {
                        ++task.Content.m_FinishedSyncFileCount;
                        task.Content.m_RunningSyncFiles.Remove(curSyncFile);
                    }

                    curSyncFile = nextSyncFile;
                }

                while (task.Content.m_RunningSyncFiles.Count < task.Content.m_MaxSyncFileNum )
                {
                    if (task.Content.m_FinishedSyncFileCount + task.Content.m_RunningSyncFiles.Count >= task.Content.m_SyncDiffList.Count)
                        break;

                    task.Content.m_SyncFolderPhase.Progress = (task.Content.m_FinishedSyncFileCount * 1.0f) / task.Content.m_SyncDiffList.Count;
                    _StartSyncFileFormList(task.Content, task.Content.m_SyncDiffList);
                }

                if (task.Content.m_FinishedSyncFileCount == task.Content.m_SyncDiffList.Count)
                {
                    task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Finished;
                    task.Content.m_SyncFolderPhase.Progress = 1.0f;
                    return new ProcStepResult<SyncFolder>() { NextStep = _VerifySyncCalMD5Sum, State = ProcedureStepState.Done };
                }
                else
                {
                    task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Processing;
                    return ProcStepResult<SyncFolder>.Continue;
                }
            }

            /// <summary>
            /// 验证同步这个步骤会生成资源的校验和
            /// </summary>
            /// <param name="task"></param>
            /// <returns></returns>
            static private ProcStepResult<SyncFolder> _VerifySyncCalMD5Sum(Task<SyncFolder> task)
            {
                if (null == task.Content.m_CurVerifyFile)
                {
                    task.Content.m_SyncFolderPhase.PhaseType = FileSyncPhaseType.VerifySyncFile;
                    task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Begin;

                    task.Content.m_VerifyFileList.Clear();
                    task.Content.m_VerifyCount = 0;
                    string checkListFilePath = Utility.Path.Combine(task.Content.m_SyncFolderFullPath,
                        Utility.Path.Combine("temp", "checkfile.lst"));

                    task.Content.m_SyncFolderPhase.Progress = 0.01f;
                    using (Stream descFile = Utility.File.OpenRead(checkListFilePath))
                    {
                        if (null != descFile)
                        {
                            StreamReader descReader = new StreamReader(descFile);
                            string data = "";
                            int line = 0;
                            while (descReader.Peek() >= 0)
                            {
                                data = descReader.ReadLine();
                                ++line;

                                task.Content.m_VerifyFileList.AddLast(new SyncFileDesc() { Name = data, MD5 = string.Empty });
                            }
                        }
                    }
                    task.Content.m_CurVerifyFile = task.Content.m_VerifyFileList.First;

                    task.Content.m_SyncFolderPhase.Progress = 0.03f;
                    task.Content.m_SyncFolderPhase.PhaseState =  PhaseState.Processing;
                    return ProcStepResult<SyncFolder>.Continue;
                }
                else
                {
                    if(null == task.Content.m_MD5Verifier)
                    {               
                        string filePath = string.Empty;
                        do
                        {
                            filePath = Utility.Path.Combine(task.Content.m_SyncFolderFullPath, "temp/new", task.Content.m_CurVerifyFile.Value.Name);
                            if (Utility.File.Exists(filePath))
                            {
                                task.Content.m_VerifyStream = Utility.File.OpenRead(filePath);
                                break;
                            }

                            filePath = Utility.Path.Combine(task.Content.m_SyncFolderFullPath, task.Content.m_CurVerifyFile.Value.Name);
                            if (Utility.File.Exists(filePath))
                            {
                                task.Content.m_VerifyStream = Utility.File.OpenRead(filePath);
                                break;
                            }

                            if(_IsFileInPackage(task.Content.m_ApplicationPackageDataPath, task.Content.m_CurVerifyFile.Value.Name))
                            {
                                //task.Content.m_VerifyStream = _CreateFileStreamInPackage(
                                //    task.Content.m_ApplicationPackageDataPath, task.Content.m_CurVerifyFile.Value.Name);                               
                                _ExtractFileFromPackage(task.Content.m_ApplicationPackageDataPath, task.Content.m_CurVerifyFile.Value.Name, filePath);
                                if (Utility.File.Exists(filePath))
                                {
                                    task.Content.m_VerifyStream = Utility.File.OpenRead(filePath);
                                    break;
                                }
                            }

                            /// 出错了我靠这个文件居然没有
                            Debugger.LogWarning("Can not find target file [name:'{0}'] in folder[path:'{1}']", task.Content.m_CurVerifyFile.Value.Name, task.Content.m_SyncFolderFullPath);

                            task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Terminated;
                            return ProcStepResult<SyncFolder>.Terminated;
                        }
                        while (false);

                        ++task.Content.m_VerifyCount;
                        task.Content.m_MD5Verifier = new MD5Verifier();
                        if (null != task.Content.m_VerifyStream)
                        {
                            if (task.Content.m_MD5Verifier.BeginVerify(task.Content.m_VerifyStream, task.Content.Context.CacheSize, task.Content.Context.TimeSlice))
                            {
                                task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Processing;
                                return ProcStepResult<SyncFolder>.Continue;
                            }
                            else
                            {
                                Debugger.LogWarning("Begin verify file '{0}' has failed!", filePath);
                                task.Content.m_VerifyStream.Dispose();
                                task.Content.m_VerifyStream = null;
                            }
                        }
                        else
                            Debugger.LogWarning("Open source file '{0}' has failed!", filePath);

                        task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Terminated;
                        return ProcStepResult<SyncFolder>.Terminated;
                    }
                    else
                    {
                        if (task.Content.m_MD5Verifier.EndVerify())
                        {
                            task.Content.m_VerifyStream.Dispose();
                            task.Content.m_VerifyStream = null;

                            task.Content.m_CurVerifyFile.Value.MD5 = task.Content.m_MD5Verifier.GetVerifySum();
                            task.Content.m_MD5Verifier = null;


                            task.Content.m_SyncFolderPhase.Progress = 0.03f + (task.Content.m_VerifyCount * 0.9f / task.Content.m_VerifyFileList.Count);
                            if (null != task.Content.m_CurVerifyFile.Next)
                                task.Content.m_CurVerifyFile = task.Content.m_CurVerifyFile.Next;
                            else
                            {
                                task.Content.m_SyncFolderPhase.Progress = 0.93f;
                                return new ProcStepResult<SyncFolder>() { NextStep = _VerifySyncGenMD5Sum, State = ProcedureStepState.Done };
                            }
                        }

                        task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Processing;
                        return ProcStepResult<SyncFolder>.Continue;
                    }
                }
            }

            static private ProcStepResult<SyncFolder> _VerifySyncGenMD5Sum(Task<SyncFolder> task)
            {
                task.Content.m_SyncFolderPhase.Progress = 0.94f;
                Stream stream = Utility.Memory.OpenStream(task.Content.m_VerifyFileList.Count * 32);
                byte[] byteBuf = null;
                LinkedListNode<SyncFileDesc> cur = task.Content.m_VerifyFileList.First;
                stream.Seek(0, SeekOrigin.Begin);
                while (null != cur)
                {
                    byteBuf = System.Text.Encoding.ASCII.GetBytes(cur.Value.MD5);
                    stream.Write(byteBuf, 0, byteBuf.Length);
                    cur = cur.Next;
                }

                task.Content.m_SyncFolderPhase.Progress = 0.95f;
                MD5Verifier verifier = new MD5Verifier();
                if(verifier.BeginVerify(stream,128 * 1024, task.Content.Context.TimeSlice))
                {
                    bool isEnd = false;
                    do
                    {/// 危险需要加入熔断措施 异常捕获退出 或者时间机制
                        isEnd = verifier.EndVerify();
                    }
                    while(!isEnd);

                    task.Content.m_SyncFolderPhase.Progress = 0.96f;
                    string md5Sum = verifier.GetVerifySum();
                    string syncTargetMD5 = Utility.Path.GetFileName(task.Content.Context.PullTargetURL);
                    if (syncTargetMD5.Equals(md5Sum,System.StringComparison.OrdinalIgnoreCase))
                    {
                        task.Content.m_SyncFolderPhase.Progress = 0.97f;
                        string verfication = Utility.Path.Combine(task.Content.m_SyncFolderFullPath, "temp", INTERNAL_FILE_VERIFICATION);
                        using (Stream streamVerification = Utility.File.OpenWrite(verfication, true))
                        {
                            byte[] md5 = System.Text.Encoding.ASCII.GetBytes(md5Sum);
                            streamVerification.Write(md5, 0, md5.Length);
                            streamVerification.Flush();
                        }

                        task.Content.m_SyncFolderPhase.Progress = 0.98f;
                        string synctarget = Utility.Path.Combine(task.Content.m_SyncFolderFullPath, "temp", INTERNAL_SYNC_TARGET_VERIFICATION);
                        if (Utility.File.Exists(synctarget))
                            Utility.File.Delete(synctarget);

                        task.Content.m_SyncFolderPhase.Progress = 1.0f;
                        task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Finished;
                        /// 如果是非测试状态（）Release版本同步
                        if (task.Content.Context.NeedForceSync)
                        {
                            return new ProcStepResult<SyncFolder>() { NextStep = _BeginOnlineOperation, State = ProcedureStepState.Done };
                        }
                        else
                        {
                            task.Content.m_SyncFolderPhase.PhaseType = FileSyncPhaseType.Finished;
                            return ProcStepResult<SyncFolder>.Finished;
                        }
                    }
                    else
                    {
                        Debugger.LogWarning("MD5 sum is miss match native md5:'{0}' remote md5:'{1}'!", md5Sum, task.Content.m_SyncConfig.VersionRelease.RemoteMD5);
                        task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Terminated;
                        return ProcStepResult<SyncFolder>.Terminated;
                    }
                }
                else
                {
                    Debugger.LogWarning("Begin verify memory stream has failed!");
                    task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Terminated;
                    return ProcStepResult<SyncFolder>.Terminated;
                }
            }

            static private void _StartSyncFileFormList<T>(SyncFolder folder,LinkedList<T> syncList) where T:SyncFile
            {
                LinkedListNode<T> cur = syncList.First;
                while (null != cur)
                {
                    T curFile = cur.Value;
                    if (SyncFile.SyncState.Wait == curFile.State)
                    {
                        curFile.StartSynchronize();
                        folder.m_RunningSyncFiles.AddLast(curFile);
                        break;
                    }

                    cur = cur.Next;
                }
            }

            /// <summary>
            /// </summary>
            /// <returns>同步目录中的本地资源校验和，如果没有该文件则返回null</returns>
            private static string _GetFileContentString(string filePath)
            {
                string md5Sum = string.Empty;
                if (Utility.File.Exists(filePath))
                {
                    using (Stream verifiyFile = Utility.File.OpenRead(filePath))
                    {
                        if (null != verifiyFile)
                        {
                            StreamReader verifyReader = new StreamReader(verifiyFile);
                            md5Sum = verifyReader.ReadToEnd().Trim();
                            verifyReader.Close();
                        }
                    }
                }

                return md5Sum;
            }

            private static string _CalculateURLMD5(string url)
            {
                string md5Sum = string.Empty;
                using (Stream stream = Utility.Memory.OpenStream(32))
                {
                    byte[] content = System.Text.Encoding.ASCII.GetBytes(url);
                    stream.Write(content, 0, content.Length);
                    stream.Flush();

                    stream.Seek(0, SeekOrigin.Begin);
                    MD5Verifier md5Verifier = new MD5Verifier();
                    if (md5Verifier.BeginVerify(stream, 64 * 1024, 0.01f))
                    {
                        /// 注意卡死的问题 还是要想出一个机制预防卡死 目前想出来的是 根据每个循环的累加值 做个保险栓 超过需要验证的总数据长度的两倍的时候熔断
                        while (md5Verifier.EndVerify())
                            md5Sum = md5Verifier.GetVerifySum();
                    }
                    else
                        Debugger.LogWarning("Verify md5 sum has failed!");
                }

                return md5Sum;
            }


            private static bool _CreateRemoteDataRequest(string url,out HttpRequestContext context)
            {
                context = new HttpRequestContext { Stream = new MemoryStream(1024), Obtainer = new RemoteDataObtainer() };
                context.Obtainer.CreateRequest(url, 0, 4096, context.Stream, 3, (uint)RemoteDataObtainFlag.FetchFromOrigin);
                return true;
            }

            private static bool _OnFetchRemoteDataRequest(HttpRequestContext context)
            {
                if(null != context)
                {
                    Debugger.Assert(null != context.Obtainer, "Obtainer can not be null!");
                    Debugger.Assert(null != context.Stream, "Memory stream can not be null!");

                    switch (context.Obtainer.State)
                    {
                        case RequestState.Finish:
                            {
                                //AutoTest.InsertProbe(0x9231243a);
                                context.Stream.Flush();
                                context.Stream.Seek(0, SeekOrigin.Begin);
                                return true;
                            }
                        case RequestState.Continue:
                            {
                                return false;
                            }
                        case RequestState.Terminated:
                            {
                                return true;
                            }
                    }
                }

                Debugger.LogWarning("Error:HttpRequestContext is null!");
                return true;
            }


            private static SyncConfig _ParseConfigXML(Stream xmlData,bool isWhiteListUser)
            {
                /// <?xml version="1.0" encoding="utf-8"?>
                /// <config>
                ///     <!-- 版本是资源校验和 强制拉取如果为true，则在上线的时候会弹出下载界面强制用户等待下载，
                ///     如果为false，则不会要求用户等下下载,只有白名单的用户才能看得到preview版本 -->  
                ///     <version force_pull="true" time_slice="0.01" cache = "524288">
                ///         <!-- 线上版本，正常玩家可以看到并且正在运行的版本 -->
                ///         <release>
                ///             <md5 program="12314">be2335d6d1652cb0abe14fa315e0db22</md5>
                ///         </release>
                ///         <!-- 预览版本，上线前白名单测试用户能够更新到的版本 是否推送 为否白名单用户也不会更新
                /// 		当前预热状态：on[开启] off[关闭] rollback[回滚] -->
                ///         <preview push="true" prewarm="on_for_all" >
                ///             <md5 program="12523">3c2ffa277e99a16c079a50f1d1e29c2f</md5>
                ///         </preview>
                ///         <old>
                ///             <md5 program="12523">3c2ffa277e99a16c079a50f1d1e29c2f</md5>
                ///             <md5 program="34251">be2335d6d1652cb0abe14fa315e0db22</md5>
                ///             <!-- 初始版本标志现在热更新服务器上还没有最新版本 -->
                ///             <md5 program="0">00000000000000000000000000000000</md5>
                ///         </old>
                ///     </version>
                /// </config>
                /// release对应的是线上正式版的数据
                /// preview对应的是待审核版和预热的数据

                XmlParser xmlParser = new XmlParser();
                xmlData.Seek(0, SeekOrigin.Begin);
                if (xmlParser.Parse(xmlData))
                {
                    ITMXmlElement config = xmlParser["config"][0];

                    ITMXmlElement version = config["version"][0];
                    bool force_pull = version.Attribute["force_pull"].Value.Bool;
                    float time_slice = version.Attribute["time_slice"].Value.Float32;
                    int cache = version.Attribute["cache"].Value.Int32;

                    ITMXmlElement release = version["release"][0];
                    ITMXmlElement release_md5 = release["md5"][0];
                    string release_target_md5 = release_md5.Value;
                    long release_program = release_md5.Attribute["program"].Value.Int64;

                    ITMXmlElement preview = version["preview"][0];
                    bool preview_push = preview.Attribute["push"].Value.Bool;
                    string prewarm_state = preview.Attribute["prewarm"].Value.String;
                    PrewarmState prewarmState = PrewarmState.OnForAll;
                    switch (prewarm_state)
                    {
                        case "on_for_test": prewarmState = PrewarmState.OnForTest; break;
                        case "on_for_all": prewarmState = PrewarmState.OnForAll; break;
                        case "off": prewarmState = PrewarmState.Off; break;
                    }

                    ITMXmlElement preview_md5 = preview["md5"][0];
                    string preview_target_md5 = preview_md5.Value;
                    long preview_program = preview_md5.Attribute["program"].Value.Int64;

                    VersionDesc versionRelease = new VersionDesc() { RemoteMD5 = release_target_md5,Program = release_program };
                    VersionDesc versionPreview = new VersionDesc() { RemoteMD5 = preview_target_md5, Program = preview_program };
                    SyncConfig syncCfg = new SyncConfig() {
                        ForcePull = force_pull,
                        VersionRelease = versionRelease,
                        VersionPreview = versionPreview,
                        PushPreview = preview_push,
                        PreWarmState = prewarmState,
                        IsWhiteListUser = isWhiteListUser,
                        CacheSize = cache,
                        TimeSlice = time_slice
                    };

                    ITMXmlElement[] old = version["old"][0]["md5"];
                    for (int i = 0, icnt = old.Length; i < icnt; ++i)
                    {
                        ITMXmlElement curOld = old[i];
                        long old_program = curOld.Attribute["program"].Value.Int64;
                        if (XmlParser.NullElement != curOld)
                            syncCfg.VersionOld.Add(new VersionDesc() { RemoteMD5 = curOld.Value,Program = old_program });
                    }

                    return syncCfg;
                }

                return null;
            }

            /// <summary>
            /// 清除同步数据缓存
            /// </summary>
            private void _ClearSyncCaches()
            {
                string tempFolderPath = Utility.Path.Combine(m_SyncFolderFullPath, "temp");
                Debugger.LogProcedure(this,"Clear folder synchronize caches:'{0}'!", tempFolderPath);
                if (Utility.Directory.Exists(tempFolderPath))
                    Utility.Directory.Delete(tempFolderPath);
            }

            private bool _ParseSyncDescFile()
            {
                /// 格式如下
                /// add:file,md5,size
                /// dif:file,md5,md5,size
                /// del:file
                string nativeDescFilePath = Utility.Path.Combine(m_SyncFolderFullPath,
                    Utility.Path.Combine("temp", INTERNAL_FILE_SYNCLIST));
                using (Stream descFile = Utility.File.OpenRead(nativeDescFilePath))
                {
                    if (null != descFile)
                    {
                        m_SyncAddList.Clear();
                        m_SyncDiffList.Clear();
                        m_SyncDelList.Clear();
                        m_FinishedSyncFileCount = 0;

                        StreamReader descReader = new StreamReader(descFile);
                        string data = "";
                        int line = 0;
                        while (descReader.Peek() >= 0)
                        {
                            data = descReader.ReadLine();
                            ++line;

                            string[] content = data.Split(':');
                            if (null != content && 2 == content.Length)
                            {
                                string op = content[0];
                                string operand = content[1];

                                if (op.Equals("add", System.StringComparison.OrdinalIgnoreCase))
                                {/// 添加操作
                                    string[] subcontent = operand.Split(',');
                                    if (null != subcontent && 3 == subcontent.Length)
                                    {
                                        long fileSize = 0;
                                        if (long.TryParse(subcontent[2], out fileSize))
                                        {
                                            SyncFileAdd newFile = new SyncFileAdd(this, subcontent[0], subcontent[1], fileSize);
                                            m_SyncAddList.AddLast(newFile);
                                        }
                                        else
                                            Debugger.LogWarning("Parse file length data has failed![content:'{0}' line:{1}]", subcontent[2], line);
                                    }
                                    else
                                        Debugger.LogWarning("Parse add file content data has failed![content:'{0}' line:{1}]", operand, line);
                                }
                                else if (op.Equals("dif", System.StringComparison.OrdinalIgnoreCase))
                                {
                                    string[] subcontent = operand.Split(',');
                                    if (null != subcontent && 4 == subcontent.Length)
                                    {
                                        long fileSize = 0;
                                        if (long.TryParse(subcontent[3], out fileSize))
                                        {
                                            SyncFileDiff newFile = new SyncFileDiff(this, subcontent[0], subcontent[1], subcontent[2], fileSize);
                                            m_SyncDiffList.AddLast(newFile);
                                        }
                                        else
                                            Debugger.LogWarning("Parse file length data has failed![content:'{0}' line:{1}]", subcontent[3], line);
                                    }
                                    else
                                        Debugger.LogWarning("Parse add file content data has failed![content:'{0}' line:{1}]", operand, line);
                                }
                                else if (op.Equals("del", System.StringComparison.OrdinalIgnoreCase))
                                {
                                    SyncFileDel newFile = new SyncFileDel(this, operand);
                                    m_SyncDelList.AddLast(newFile);
                                }
                                else
                                {
                                    Debugger.LogWarning("Unknown sync command:[{0}], skip.", op);
                                    continue;
                                }
                            }
                        }

                        return true;
                    }
                    else
                        Debugger.LogWarning("Create file stream for read has failed! [file:'{0}']", nativeDescFilePath);
                }

                return false;
            }

            static private ProcStepResult<SyncFolder> _BeginOnlineOperation(Task<SyncFolder> task)
            {
                task.Content.m_SyncFolderPhase.PhaseType = FileSyncPhaseType.OnlineOperate;
                task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Begin;
                task.Content.m_SyncFolderPhase.Progress = 0.0f;

                /// 把new里面的东西覆盖至原目录，把其中的synclist.lst里面需要删除的文件删除掉。
                if (task.Content._ParseSyncDescFile())
                {
                    task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Processing;
                    task.Content.m_SyncFolderPhase.Progress = 0.01f;
                    return new ProcStepResult<SyncFolder>() { NextStep = _DeleteSyncFile, State = ProcedureStepState.Done };
                }

                task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Terminated;
                return ProcStepResult<SyncFolder>.Terminated;
            }

            static private ProcStepResult<SyncFolder> _DeleteSyncFile(Task<SyncFolder> task)
            {
                /// 更新正在运行的任务
                LinkedListNode<SyncFile> curSyncFile = task.Content.m_RunningSyncFiles.First;
                LinkedListNode<SyncFile> nextSyncFile;
                while (null != curSyncFile)
                {
                    nextSyncFile = curSyncFile.Next;
                    if (SyncFile.SyncState.Done == curSyncFile.Value.State)
                    {
                        ++task.Content.m_FinishedSyncFileCount;
                        task.Content.m_RunningSyncFiles.Remove(curSyncFile);
                    }

                    curSyncFile = nextSyncFile;
                }

                while (task.Content.m_RunningSyncFiles.Count < task.Content.m_MaxSyncFileNum)
                {
                    if (task.Content.m_FinishedSyncFileCount + task.Content.m_RunningSyncFiles.Count >= task.Content.m_SyncDelList.Count)
                        break;

                    task.Content.m_SyncFolderPhase.Progress = 0.01f + (task.Content.m_FinishedSyncFileCount * 0.94f) / task.Content.m_SyncDelList.Count;
                    _StartSyncFileFormList(task.Content, task.Content.m_SyncDelList);
                }

                if (task.Content.m_FinishedSyncFileCount == task.Content.m_SyncDelList.Count)
                {
                    task.Content.m_SyncFolderPhase.Progress = 0.95f;
                    return new ProcStepResult<SyncFolder>() { NextStep = _ReplaceSyncFile, State = ProcedureStepState.Done };
                }
                else
                    return ProcStepResult<SyncFolder>.Continue;
            }

            static private ProcStepResult<SyncFolder> _ReplaceSyncFile(Task<SyncFolder> task)
            {
                string srcPath = string.Empty;
                string dstPath = string.Empty;

                srcPath = Utility.Path.Combine(task.Content.m_SyncFolderFullPath, "temp",INTERNAL_FILE_FILELIST);
                dstPath = Utility.Path.Combine(task.Content.m_SyncFolderFullPath, INTERNAL_FILE_FILELIST);
                if (Utility.File.Exists(srcPath))
                    Utility.File.Copy(srcPath, dstPath,true);

                task.Content.m_SyncFolderPhase.Progress = 0.96f;

                srcPath = Utility.Path.Combine(task.Content.m_SyncFolderFullPath, "temp", INTERNAL_FILE_VERIFICATION);
                dstPath = Utility.Path.Combine(task.Content.m_SyncFolderFullPath, INTERNAL_FILE_VERIFICATION);
                if (Utility.File.Exists(srcPath))
                    Utility.File.Copy(srcPath, dstPath, true);

                task.Content.m_SyncFolderPhase.Progress = 0.97f;

                srcPath = Utility.Path.Combine(task.Content.m_SyncFolderFullPath, "temp/new");
                dstPath = task.Content.m_SyncFolderFullPath;
                if (Utility.Directory.Exists(srcPath))
                {
                    string[] files = Utility.Directory.GetFiles(srcPath, "*.*", false);
                    for(int i = 0,icnt = files.Length;i<icnt;++i)
                    {
                        string srcFile = files[i];
                        string dstFile = Utility.Path.Combine(dstPath, Utility.Path.GetFileName(srcFile));
                        Utility.File.Copy(srcFile, dstFile, true);
                        Utility.File.Delete(srcPath);
                    }
                }

                task.Content.m_SyncFolderPhase.Progress = 0.98f;

                task.Content._ClearSyncCaches();

                task.Content.m_SyncFolderPhase.Progress = 0.99f;

                if (null != task.Content.m_OnSyncFolderFinished)
                    task.Content.m_OnSyncFolderFinished();

                if(_CheckAndInstallAppPackage(task))
                    return ProcStepResult<SyncFolder>.Continue;
                else
                    HAL.Platform.RestartClientApplication();

                task.Content.m_SyncFolderPhase.Progress = 1.00f;
                task.Content.m_SyncFolderPhase.PhaseType = FileSyncPhaseType.Finished;
                task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Finished;
                return ProcStepResult<SyncFolder>.Finished;
            }

            static private void _UpdateDownloadSpeed(SyncFolder syncFolder)
            {
                if (null != syncFolder)
                {
                    if (null != syncFolder.m_SyncPackageDownloader && syncFolder.m_SyncPackageDownloader.Progress < 0.99f)
                    {
                        syncFolder.TotalBytes = syncFolder.m_SyncPackageDownloader.TotalBytes;
                        long ticksNow = Utility.Time.GetTicksNow();
                        if (Utility.Time.TicksToSeconds(ticksNow - syncFolder.m_LastTimeStamp) > 1.0f)
                        {
                            syncFolder.DownloadBytes = syncFolder.m_SyncPackageDownloader.DownloadBytes;
                            syncFolder.BytesPerSecond = syncFolder.m_SyncPackageDownloader.ReponseBytes;
                            syncFolder.m_SyncPackageDownloader.ResetResponseBytes();
                            syncFolder.m_LastTimeStamp = ticksNow;
                        }
                    }
                    else
                        syncFolder.BytesPerSecond = 0;
                }
            }

            static private void _EndWithFileSyncFolder(Task<SyncFolder> task)
            {
                task.Content.m_SyncFolderPhase.PhaseType = FileSyncPhaseType.Finished;
                task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Finished;
                _CheckAndInstallAppPackage(task);

                if (null != task.Content.m_OnEndForceWaiting)
                    task.Content.m_OnEndForceWaiting();
            }

            static private void _TerminatedWithFileSyncFolder(Task<SyncFolder> task)
            {
                task.Content.m_SyncFolderPhase.PhaseType = FileSyncPhaseType.Terminated;
                task.Content.m_SyncFolderPhase.PhaseState = PhaseState.Finished;

                Debugger.LogWarning("Terminate file folder synchronize!");
                //if (null != task.Content.m_OnEndForceWaiting)
                //    task.Content.m_OnEndForceWaiting();

                if (null != task.Content.m_OnQureyUserAction)
                {
                    task.Content.m_OnQureyUser = true;
                    task.Content.m_OnQureyUserAction("文件更新遇到问题，是否重试？",
                        new List<string>(new string[] { "重试", "取消" }),
                        new List<Function>(new Function[] 
                        {
                            ()=>
                            {
                                Debugger.LogInfo("User retry file folder synchronize!");
                                SyncFolder syncFolder = task.Content;
                                /// 中止当前同步目录正在执行的所有任务
                                syncFolder.m_FileSyncManager._TerminateSyncFolderTask(syncFolder.FolderName);
                                /// 重新创建更新任务
                                syncFolder.StartSynchronize();
                                task.Content.m_OnQureyUser = false;
                            },
                            ()=>
                            {
                                Debugger.LogInfo("User cancel file folder synchronize!");
                                if (null != task.Content.m_OnEndForceWaiting)
                                    task.Content.m_OnEndForceWaiting();
                                task.Content.m_OnQureyUser = false;
                            }
                        }));                
                }
            }

            static private bool _CheckAndInstallAppPackage(Task<SyncFolder> task)
            {
                if (task.Content.NeedInstallPackage)
                {
                    string srcApkPath = Utility.Path.Combine(task.Content.m_SyncFolderFullPath, "application.pck");
                    string dstApkPath = Utility.Path.Combine(task.Content.m_SyncFolderFullPath, "application.apk");
                    if (Utility.File.Exists(srcApkPath))
                    {
                        Utility.File.Copy(srcApkPath, dstApkPath, true);
                        HAL.Platform.InstallPackage(Utility.Path.Combine(task.Content.m_SyncFolderFullPath, "application.apk"));
                        return true;
                    }
                    else
                        Debugger.LogWarning("Application package version is low but not exist apk file in update package!");
                }

                return false;
            }

            static private void _OnPatchFileTerminated(Task<SyncFileDiff> task)
            {
                SyncFolder syncFolder = task.Content.Folder;
                /// 中止当前同步目录正在执行的所有任务
                syncFolder.m_FileSyncManager._TerminateSyncFolderTask(syncFolder.FolderName);
                /// 清除所有的本地资源
                syncFolder._ClearSyncCaches();
                /// 清除资源目录中的版本控制文件
                string versionFilePath = Utility.Path.Combine(syncFolder.m_SyncFolderFullPath,SyncFolder.INTERNAL_FILE_VERIFICATION);
                if (Utility.File.Exists(versionFilePath))
                    Utility.File.Delete(versionFilePath);

                /// 重新创建更新任务
                syncFolder.StartSynchronize();
            }

            static private void _LoadAppPackageFile()
            {

            }

            static private void _ExtractFileFromPackage(string appPackageDataPath,string fileName,string outputFilePath)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                LibZipPackageFileExtractor.ExtractFile(appPackageDataPath,
                    Utility.Path.Combine("assets/AssetBundles", fileName),outputFilePath);
#else
                /// 非android目录 m_ApplicationPackageDataPath就是StreamingAsset目录
                string srcFilePath = Utility.Path.Combine(appPackageDataPath, "AssetBundles", fileName);
                if (Utility.File.Exists(srcFilePath))
                {
                    string dstFilePath = outputFilePath;
                    string dstFolderPath = Utility.Path.GetDirectoryName(dstFilePath);
                    if (!Utility.Directory.Exists(dstFolderPath))
                    {
                        Debugger.LogWarning("Create directory:{0}", dstFolderPath);
                        Utility.Directory.CreateDirectory(dstFolderPath);
                    }

                    Utility.File.Copy(srcFilePath, dstFilePath);
                }
                else
                    Debugger.LogWarning("File with path '{0}' does not exist!", srcFilePath);
#endif
            }

            static private bool _IsFileInPackage(string appPackageDataPath,string fileName)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return LibZipPackageFileExtractor.IsFileInZip(appPackageDataPath,
                    Utility.Path.Combine("assets/AssetBundles", fileName));
#else
                /// 非android目录 m_ApplicationPackageDataPath就是StreamingAsset目录
                string srcFilePath = Utility.Path.Combine(appPackageDataPath, "AssetBundles", fileName);
                return Utility.File.Exists(srcFilePath);
#endif
            }

            static private Stream _CreateFileStreamInPackage(string appPackageDataPath, string fileName)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                LibZipFileStream stream = new LibZipFileStream(appPackageDataPath, 
                    Utility.Path.Combine("assets/AssetBundles", fileName));
                return stream;
#else
                /// 非android目录 m_ApplicationPackageDataPath就是StreamingAsset目录
                string filePath = Utility.Path.Combine(appPackageDataPath, "AssetBundles", fileName);
                if (Utility.File.Exists(filePath))
                    return Utility.File.OpenRead(filePath);
                else
                {
                    Debugger.LogWarning("File with path '{0}' does not exist!", filePath);
                    return null;
                }
#endif
            }

            static private void _NetStateHasChange(NetState state,SyncFolder folder)
            {
                if (null == folder)
                    return;

                if (NetState.Cellular == state && !folder.m_AllowSynchronizeWithoutWIFI)
                {
                    string downloadInfo = string.Empty;
                    if (folder.m_SyncPackageFileLength > 0)
                    {
                        float needDownload = folder.m_SyncPackageFileLength * 1.0f / (1024 * 1024);
                        downloadInfo = string.Format("(更新总大小{0}MB)", needDownload.ToString("0.0"));
                    }

                    string message = string.Format("检测到当前设备未连接到无线局域网（Wifi），是否继续更新？{0}", downloadInfo);
                    if (null != folder.m_OnQureyUserAction)
                    {
                        folder.m_OnQureyUser = true;
                        folder.m_OnQureyUserAction(message,
                            new List<string>(new string[] { "取消更新", "继续更新" }),
                            new List<Function>(new Function[]
                            {
                            ()=>
                            {
                                folder.m_AllowSynchronizeWithoutWIFI = false;
                                Debugger.LogInfo("User cancel file folder synchronize!");
                                /// 强制更新的状态下需要退出游戏
                                if(folder.IsForceSync)
                                {
                                    HAL.Platform.QuitApplication();
                                    folder.m_FileSyncManager._TerminateSyncFolderTask(folder.FolderName);
                                    folder.m_SyncFolderPhase.PhaseType = FileSyncPhaseType.UserCancel;
                                    folder.m_SyncFolderPhase.PhaseState = PhaseState.Finished;
                                }
                                else
                                { 
                                    /// 中止当前同步目录正在执行的所有任务
                                    folder.m_FileSyncManager._TerminateSyncFolderTask(folder.FolderName);
                                    if (null != folder.m_OnEndForceWaiting)
                                        folder.m_OnEndForceWaiting();
                                }

                                folder.m_OnQureyUser = false;
                            },
                            ()=>
                            {
                                folder.m_AllowSynchronizeWithoutWIFI = true;
                                folder.m_OnQureyUser = false;
                            }
                            }));
                    }
                }
            }
        }
    }
}