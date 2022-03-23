// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"

#include "HttpModule.h"
#include "JsonObjectConverter.h"
#include "TestActor.h"

#include "Interfaces/IHttpResponse.h"

/**
 * Http Post Test
 * https://www.cnblogs.com/FlyingZiming/p/14560156.html
 */
class TMSDKSAMPLE_425_API Sample_4
{
public:
	Sample_4();
	~Sample_4();

	void CreateTestHttpLoginReq();
	void CreateUploadFileReq();
	FString CreateZipFile();

public:
	const FString kUrlAddressString = "http://httpbin.org/post";

	uint64 CurrentTime();

private:

	//Test1
	void _onTestHttpLoginRes(FHttpRequestPtr HttpRequest, FHttpResponsePtr HttpResponse, bool bSucceeded);
	void _setDefaultHttpHeader(TSharedRef<IHttpRequest> req);


	//Test2
	void _downloadByHttp(const FString& url);
	void _uploadByHttp(const FString& url, const FString& content);
	void _onHttpReqProgressComplete(FHttpRequestPtr HttpRequest, FHttpResponsePtr HttpResponse, bool bSucceeded);
	void _onHttpReqProgress(FHttpRequestPtr HttpRequest, int32 bytesSend, int32 bytesRecv);


	//Common
	bool _checkResposeValid(FHttpResponsePtr response, bool bSucceeded);

public:

	template<typename StructType>
	void GetJsonStringFromStruct(const StructType& structData, FString& jsonOutput)
	{
		FJsonObjectConverter::UStructToJsonObjectString(StructType::StaticStruct(), &structData, jsonOutput, 0, 0);
	}

	//Response to Json
	template<typename StructType>
	void GetStructFromHttpResponse(FHttpResponsePtr response, StructType& structOutput)
	{
		FString jsonStr = response->GetContentAsString();
		FJsonObjectConverter::JsonObjectStringToUStruct<StructType>(jsonStr, &structOutput, 0, 0);
	}
};