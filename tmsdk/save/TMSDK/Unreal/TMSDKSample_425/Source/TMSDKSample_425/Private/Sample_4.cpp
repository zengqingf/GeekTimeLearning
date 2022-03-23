// Fill out your copyright notice in the Description page of Project Settings.


#include "Sample_4.h"

#include "Sample_5.h"

#include "FileUtilities/ZipArchiveWriter.h"

#ifdef _WIN32
# define WIN32_LEAN_AND_MEAN
#include<windows.h>
#else
#include<sys/time.h>
#endif

Sample_4::Sample_4()
{
}

Sample_4::~Sample_4()
{
}

void Sample_4::CreateTestHttpLoginReq()
{
	FRequest_Login loginReq;
	loginReq.email = "mjx@123.com";
	loginReq.password = "123qwe";
	FString jsonTest;
	GetJsonStringFromStruct<FRequest_Login>(loginReq, jsonTest);
	UE_LOG(LogTemp, Warning, TEXT("### request: %s"), *jsonTest);

	//设置超时
	float tempTimeout = FHttpModule::Get().GetHttpTimeout();
	FHttpModule::Get().SetHttpTimeout(3);

	TSharedRef<IHttpRequest> req = FHttpModule::Get().CreateRequest();
	_setDefaultHttpHeader(req);
	req->SetURL(*kUrlAddressString);
	req->SetVerb(TEXT("Post"));
	req->SetContentAsString(jsonTest);
	req->OnProcessRequestComplete().BindRaw(this, &Sample_4::_onTestHttpLoginRes);

	req->ProcessRequest();

	//最好放在回调里
	req->CancelRequest();
	FHttpModule::Get().SetHttpTimeout(tempTimeout);
}

/*
http://39.108.138.140:59970 上传地址
http://39.108.138.140:59971  下载地址
*/
void Sample_4::CreateUploadFileReq()
{
	//FString testZipFile = GetProjectSavePath_TM() / TEXT("test_save_dir/test.zip");
	FString testZipFile = CreateZipFile();

	if (!IFileManager::Get().FileExists(*testZipFile))
	{
		UE_LOG(LogTemp, Error, TEXT("### file: %s not found !"), *testZipFile);
		return;
	}
	TArray<uint8> testZipFileRawData;
	FFileHelper::LoadFileToArray(testZipFileRawData, *testZipFile);

	FString testZipFileBase = FPaths::GetCleanFilename(testZipFile);

	TSharedRef<IHttpRequest> httpReq = FHttpModule::Get().CreateRequest();
	FString url = FString::Printf(TEXT("%s?file=%s&dataString=fk&deviceId=fk&serverId=fk&lastMoveLogTime=fk"), TEXT("http://39.108.138.140:59970"), *testZipFileBase);  //?file=%s&dataString=fk&deviceId=fk&serverId=fk&lastMoveLogTime=fk
	//FString url = FString::Printf(TEXT("%s/upload/"), TEXT("http://39.108.138.140:59970"));
	httpReq->SetURL(url);

	//FString boundaryStr = TEXT("&dataString=fk&deviceId=fk&serverId=fk&lastMoveLogTime=fk");
	//httpReq->SetHeader(TEXT("Content-Type"), TEXT("multipart/form-data; boundary =") + boundaryStr);
	//httpReq->SetHeader(TEXT("Content-Type"), TEXT("application/json; charset=utf-8"));
	httpReq->SetHeader(TEXT("Content-Type"), TEXT("application/json"));
	httpReq->SetVerb(TEXT("POST"));
	httpReq->SetContent(testZipFileRawData);
	httpReq->OnProcessRequestComplete().BindRaw(this, &Sample_4::_onHttpReqProgressComplete);
	httpReq->OnRequestProgress().BindRaw(this, &Sample_4::_onHttpReqProgress);
	httpReq->ProcessRequest();
}

FString Sample_4::CreateZipFile()
{
	FString compressDir = GetProjectSavePath_TM() / TEXT("test_save_dir/");
	IFileManager::Get().MakeDirectory(*compressDir, true);

	FString latestDir;
	uint64 latestTimestamp = 0; //要赋初值！
	IFileManager::Get().IterateDirectory(*compressDir, [&](const TCHAR* FilenameOrDirectory, bool bIsDirectory) -> bool
		{
			if (bIsDirectory)
			{
				//just enable in windows
				//uint64 dirTimestamp = platformFile.GetTimeStamp(FilenameOrDirectory).GetTicks();
				//uint64 dirTimestamp = platformFile.GetTimeStamp(FilenameOrDirectory).ToUnixTimestamp();

				FString cleanDirName = FPaths::GetCleanFilename(FilenameOrDirectory);
				uint64 dirTimestamp = FCString::Atoi64(*cleanDirName);

				UE_LOG(LogTemp, Warning, TEXT("### iter visitor zip dir: %s, timestamp: %lld"), FilenameOrDirectory, dirTimestamp);

				if (latestTimestamp < dirTimestamp)
				{
					latestTimestamp = dirTimestamp;
					latestDir = FilenameOrDirectory;
				}
			}
			return true;
		});


	//FString tempFileBaseName = FString::Printf(TEXT("%lld"), CurrentTime());
	//FString compressPath = (compressDir / tempFileBaseName) + TEXT(".zip");

	if (latestDir.IsEmpty() || !FPaths::DirectoryExists(latestDir))
	{
		return TEXT("");
	}

	FString rootPath = FPaths::GetPath(latestDir);
	FString rootBaseName = FPaths::GetBaseFilename(rootPath);
	UE_LOG(LogTemp, Log, TEXT("### compress root path: %s, root base name: %s"), *rootPath, *rootBaseName);

	FString tempFileBaseName = FString::Printf(TEXT("%s_%lld"), *rootBaseName, CurrentTime());
	FString zipPath = (compressDir / tempFileBaseName) + TEXT(".zip");

	IPlatformFile& platformFile = FPlatformFileManager::Get().GetPlatformFile();
	IFileHandle* zipFile = platformFile.OpenWrite(*zipPath);
	if (zipFile)
	{
//#if WITH_ENGINE
		FZipArchiveWriter* zipWriter = new FZipArchiveWriter(zipFile);

		if (!latestDir.IsEmpty())
		{
			UE_LOG(LogTemp, Warning, TEXT("### ready zip dir: %s, timestamp: %lld"), *latestDir, latestTimestamp);

			TArray<FString> filesToArchive;
			IFileManager::Get().FindFilesRecursive(filesToArchive, *latestDir, TEXT("*.*"), true, true, true);

			for (FString fileName : filesToArchive)
			{
				//@注意：文件夹不能添加到FZipArchiveWriter中
				if (IFileManager::Get().DirectoryExists(*fileName))
				{
					continue;
				}
				TArray<uint8> fileData;
				FFileHelper::LoadFileToArray(fileData, *fileName);
				FPaths::MakePathRelativeTo(fileName, *latestDir);

				zipWriter->AddFile(fileName, fileData, FDateTime::Now());
			}
		}
		else
		{
			IFileManager::Get().Delete(*zipPath);
		}

		delete zipWriter;
		zipWriter = nullptr;

//#endif //WITH_ENGINE
	}
	else
	{
		UE_LOG(LogTemp, Error, TEXT("### failed to create *.log .zip output file: %s"), *zipPath);
	}

	return zipPath;
}

/*
时间戳输出为1629273816926  多了后三位！！！
*/
uint64 Sample_4::CurrentTime()
{
#ifdef _WIN32
	FILETIME fileTime;
	ULARGE_INTEGER li;
	const static uint64 TIME_DIFF = 116444736000000000;
	;
	::GetSystemTimeAsFileTime(&fileTime);
	li.LowPart = fileTime.dwLowDateTime;
	li.HighPart = fileTime.dwHighDateTime;
	return ((uint64)li.QuadPart - TIME_DIFF) / 10000;
#else
	struct timeval tv;
	gettimeofday(&tv, NULL);
	return (uint64)tv.tv_sec * 1000 + (uint64)tv.tv_usec / 1000;  
#endif
}

void Sample_4::_onTestHttpLoginRes(FHttpRequestPtr req, FHttpResponsePtr res, bool bSucceeded)
{
	if (!_checkResposeValid(res, bSucceeded))
	{
		return;
	}
	UE_LOG(LogTemp, Warning, TEXT("### response: %s"), *(res->GetContentAsString()));
}

void Sample_4::_setDefaultHttpHeader(TSharedRef<IHttpRequest> req)
{
	req->SetHeader(TEXT("User-Agent"), TEXT("X-UnrealEngine-Agent"));
	req->SetHeader(TEXT("Content-Type"), TEXT("application/json; charset=utf-8"));
	req->SetHeader(TEXT("Accepts"), TEXT("application/json; charset=utf-8"));
}

void Sample_4::_downloadByHttp(const FString& url)
{
	TSharedRef<IHttpRequest> httpReq = FHttpModule::Get().CreateRequest();
	httpReq->SetURL(url);
	httpReq->SetVerb(TEXT("Get"));
	httpReq->SetHeader(TEXT("Content-Type"), TEXT("application/x-www-form-urlencoded"));
	httpReq->OnProcessRequestComplete().BindRaw(this, &Sample_4::_onHttpReqProgressComplete);
	httpReq->ProcessRequest();
}

void Sample_4::_uploadByHttp(const FString& url, const FString& content)
{
	TSharedRef<IHttpRequest> httpReq = FHttpModule::Get().CreateRequest();
	httpReq->SetURL(url);
	httpReq->SetVerb(TEXT("Post"));
	httpReq->SetHeader(TEXT("Content-Type"), TEXT("application/x-www-form-urlencoded"));
	httpReq->SetContentAsString(content);  //SetContent(TArray<int>)
	httpReq->OnProcessRequestComplete().BindRaw(this, &Sample_4::_onHttpReqProgressComplete);
	httpReq->OnRequestProgress().BindRaw(this, &Sample_4::_onHttpReqProgress);
	httpReq->ProcessRequest();
}

//请求完成
void Sample_4::_onHttpReqProgressComplete(FHttpRequestPtr request, FHttpResponsePtr response, bool bSucceeded)
{
	if (!_checkResposeValid(response, bSucceeded))
	{
		return;
	}
	UE_LOG(LogTemp, Log, TEXT("### Http Response success: %d, url: %s"), response->GetResponseCode(), *(response->GetURL()));

	FString fileName = FPaths::GetCleanFilename(request->GetURL());
	FFileHelper::SaveArrayToFile(response->GetContent(), *FString::Printf(TEXT("%s%s"), *FPaths::ProjectSavedDir(), *fileName));
}

//请求执行进度
void Sample_4::_onHttpReqProgress(FHttpRequestPtr request, int32 bytesSend, int32 bytesRecv)
{
	UE_LOG(LogTemp, Warning, TEXT("### req progress ---> send bytes: %d, recv bytes: %d"), bytesSend, bytesRecv);
}


bool Sample_4::_checkResposeValid(FHttpResponsePtr response, bool bSucceeded)
{
	if (!bSucceeded || !response.IsValid())
		return false;
	if (EHttpResponseCodes::IsOk(response->GetResponseCode()))
	{
		return true;
	}
	else
	{
		UE_LOG(LogTemp, Error, TEXT("### Http Response error code: %d"), response->GetResponseCode());
		return false;
	}
}
