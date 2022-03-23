# define _DLLExport __declspec (dllexport)

#ifdef __cplusplus
extern "C"{
#endif
    void _DownloadFullResource();
    bool _IsSplitPackage();
    bool _IsDownloadFinished();
    int _BackgroundDownloadProgress();//
    long _GetTotalSize();
    long _GetCurrentDlSize();
    void _OpenDownload();
    void _CloseDownload();
    char* _GetResCachePath();
    int _SingleFileDownload();//
    void _QueryUpdate();
    int _GetCurrentLBVercode();//
#ifdef __cplusplus
} // extern "C"
#endif
