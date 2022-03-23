#include "LBInterface.h"
#import <LBSDK/LBInit.h>

void _DownloadFullResource(){
    [LBInit downloadFullResource];
}
bool _IsSplitPackage(){
    return [LBInit isSplitPackage];
}
bool _IsDownloadFinished(){
    return [LBInit isDownloadFinished];
}
int _BackgroundDownloadProgress(){
    return [LBInit backgroundDownloadProgress];
}
long _GetTotalSize(){
    return [LBInit getTotalSize];
}
long _GetCurrentDlSize(){
    return [LBInit getCurrentDlSize];
}
void _OpenDownload(){
    [LBInit openDownload];
}
void _CloseDownload(){
    [LBInit closeDownload];
}
char* _GetResCachePath(){
    return [LBInit getResCachePath];
}
int _SingleFileDownload(){
    return [LBInit singleFileDownload];
}
void _QueryUpdate(){
    [LBInit queryUpdate];
}
int _GetCurrentLBVercode(){
    return [LBInit getCurrentLBVercode];
}