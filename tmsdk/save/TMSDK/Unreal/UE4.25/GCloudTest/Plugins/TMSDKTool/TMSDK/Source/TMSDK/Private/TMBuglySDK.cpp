// Fill out your copyright notice in the Description page of Project Settings.


#include "TMBuglySDK.h"
//#include "Bugly.h"

TMBuglySDK::TMBuglySDK()
{

}

TMBuglySDK::~TMBuglySDK()
{

}

void TMBuglySDK::Init()
{
	//FBuglyModule::Get().GetAgent()->InitReport();
}

void TMBuglySDK::Uninit()
{

}

FString TMBuglySDK::Version() const
{
	return "v3.3.3";
}

void TMBuglySDK::SetUserId(const FString& userId)
{
	//FBuglyModule::Get().GetAgent()->SetUserId(userId);	
}

