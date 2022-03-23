// Fill out your copyright notice in the Description page of Project Settings.


#include "Plugins/HttpRequestUtility.h"

HttpRequestUtility::HttpRequestUtility()
{
	//mHttpReuest = FHttpModule::Get().CreateRequest();
}

HttpRequestUtility::~HttpRequestUtility()
{
}

void HttpRequestUtility::StartPost(FString Url)
{
	if (Url.IsEmpty())
	{
		return;
	}
	TSharedRef<IHttpRequest> Request = FHttpModule::Get().CreateRequest();
	Request->SetHeader("Content-Type", "text/javascript;charset=utf-8");
	Request->SetVerb("POST");
	Request->SetURL(Url);
	Request->OnProcessRequestComplete().BindUObject(this, &HttpRequestUtility::OnGetResponse);
	Request->ProcessRequest();
}

void HttpRequestUtility::OnGetResponse(FHttpRequestPtr RequestPtr, FHttpResponsePtr ResponsePtr, bool bIsSuccess)
{
	if (!bIsSuccess)
	{
		UE_LOG(LogTemp, Warning, TEXT("OnGetResponse failed"));
		return;
	}
	FString json = ResponsePtr->GetContentAsString();
	UE_LOG(LogTemp,Warning,TEXT("OnGetResponse %s "),*json);

}
