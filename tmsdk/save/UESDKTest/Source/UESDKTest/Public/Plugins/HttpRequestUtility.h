// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "HttpModule.h"
#include "Interfaces/IHttpResponse.h"
#include "Interfaces/IHttpRequest.h"

/**
 * 
 */
class UESDKTEST_API HttpRequestUtility : public UObject
{
public:
	HttpRequestUtility();
	~HttpRequestUtility();

public:
	void StartPost(FString Url);
private:
	void OnGetResponse(FHttpRequestPtr RequestPtr, FHttpResponsePtr ResponsePtr, bool bIsSuccess);

private:
	//TSharedRef<IHttpRequest> mHttpReuest;

};
