//
//  WWWTask.hpp
//  WWW
//
//  Created by vforkk on 11/17/16.
//  Copyright © 2016 vforkk. All rights reserved.
//

#ifndef WWWTask_hpp
#define WWWTask_hpp

#include <set>
#include <map>
#include <string>
#if defined(_WIN32) || defined(_WIN64)
#include<windows.h>
#endif

#include "AMutex.h"
#include "IniFile.h"
#include "OperationQueue.h"

#include "UrlRequest.h"
#include "WWW.h"

#ifdef ANDROID
#undef __APPLE__
#endif

#ifdef __ORBIS__
#include "AThreadBase.h"
#include <scetypes.h>
#endif

//ms
#define DEFAULT_TIME_OUT 20000

namespace ABase
{
    class WWWTaskBase : virtual public WWWTask
    {
    public:
        WWWTaskBase(const char* url);
        virtual ~WWWTaskBase();
        
    public:
        void SetHttpHeader(const char* name, const char* value);
        std::map<std::string, std::string> GetHeaders(){return _headers;};
        void SetUrl(const char* url);
        const char* GetUrl();
        static bool IsObjectRelease(WWWTaskBase* task);
        
    protected:
        std::string _url;
        std::map<std::string, std::string> _headers;

    private:
        static std::set<WWWTaskBase*> _set;
    public:
        static CMutex _sMutex;
    };
    
    
    
    
    
    class DataTaskImpl
    : public WWWTaskBase
    , public DataTask
#ifdef ANDROID
    , UrlRequest::Listener
#elif defined(__ORBIS__)
	, CThreadBase
#endif
    {
    public:
        DataTaskImpl(const char* url);
        virtual ~DataTaskImpl();
        
    public:
        void SetListener(DataTask::Listener* listener);
        void RemoveListener(DataTask::Listener* listener);
		DataTask::Listener*  GetListener(){return _listener;}
        
    public:
        void Get();
        void Post(const char* postData, int postLength);
        void Cancel();
        
    private:
        void _init();
        void _uninit();
        
    private:
#ifdef ANDROID
        void onUrlRequestResponse(int status, UrlResponse *response);
#endif
        void FinishedCallback( WWWErrorCode errorCode, int httpStatus , const char* data, long long totalSize);
    private:
        DataTask::Listener* _listener;
        bool _isCancel;
#if defined(_WIN32) || defined(_WIN64)
	public:
		HANDLE	_hThread;
		void OnThreadProc(const char* postData, int postLength);
#endif

#if defined(__ORBIS__)
		void OnThreadProc();
		void OnThreadStart(){}
		void OnThreadExit(){}
		void OnThreadPause(){}
		void OnThreadResume(){}
		void OnThreadError(int Error){}
		int32_t http_get(int libhttpCtxId, const char *targetUrl);
#endif

        
    private:
		CMutex _mutex;
        volatile bool _isRequesting;
#ifdef ANDROID
        UrlRequest* _impl;
#else
        void* _impl;
#endif

        
    };
    
    
    class DownloadFileTaskImpl
    : public WWWTaskBase
    , public DownloadFileTask
#ifdef ANDROID
    , UrlRequest::TaskListener
#endif
    {
    public:
        DownloadFileTaskImpl(const char* url, const char* saveFilePath);
        virtual ~DownloadFileTaskImpl();
        
    public:
        void Get(){}
        void Post(const char* postData, int postLength){}
 
    public:
        void Pause();
        void Resume();
        void Cancel();
    
    public:
        void SetListener(DownloadFileTask::Listener* listener);
        void RemoveListener(DownloadFileTask::Listener* listener);
        DownloadFileTask::Listener* GetListener(){return _listener;}
        
    public:
        const char* GetLocalFilePath(){return _saveFilePath.c_str();}
       
    private:
        void _init();
        void _uninit();

#ifdef ANDROID
    private:
        void onTaskBegan(long totalSize);
        void onTaskProgress(long currentSize, long totalSize);
        void onTaskFinished(int error, long totalSize);

#endif

        
    public:
        CMutex _mutex;
        
    private:
        std::string _saveFilePath;
        DownloadFileTask::Listener* _listener;
#ifdef ANDROID
        UrlRequest* _impl;
#else
        void* _impl;
#endif
    public:
        volatile bool _isRequesting;
        
    };


}

#endif /* WWWTask_hpp */
