// Fill out your copyright notice in the Description page of Project Settings.


#include "DeviceInfoWidgetBase.h"
#if PLATFORM_ANDROID
# include "Android/AndroidPlatformMisc.h"
#endif
#if PLATFORM_IOS
# include "IOS/IOSPlatformMisc.h"
#endif

FString UDeviceInfoWidgetBase::GetNetWorkType()
{
	FString type = "Unkown";
	ENetworkConnectionType T = ENetworkConnectionType::None;
#if PLATFORM_ANDROID
	T = FAndroidMisc::GetNetworkConnectionType();
#endif
#if PLATFORM_IOS
	T = FIOSPlatformMisc::GetNetworkConnectionType();
#endif
	switch (T)
	{
	case ENetworkConnectionType::None:				type = "None";
		break;
	case ENetworkConnectionType::AirplaneMode:		type = "AirplaneMode";
		break;
	case ENetworkConnectionType::Ethernet:			type = "Ethernet";
		break;
	case ENetworkConnectionType::Cell:				type = "4G";
		break;
	case ENetworkConnectionType::WiFi:				type = "WiFi";
		break;
	case ENetworkConnectionType::WiMAX:				type = "WiMAX";
		break;
	case  ENetworkConnectionType::Bluetooth:		type = "Bluetooth";
		break;
	}
	return type;
}

FString UDeviceInfoWidgetBase::GetTime()
{
	/*
	FDateTime* date = new FDateTime();
	int32 year = date->GetDay();
	int32 month = date->GetMonth();
	int32 day = date->GetDay();
	int32 hour = date->GetHour();
	int32 minute = date->GetMinute();
	int32 second = date->GetSecond();
	UE_LOG(LogTemp, Warning, TEXT("Ê±¼ä:%d, %d, %d, %d, %d, %d"), year, month, day, hour, minute, second);*/
	FString TimeN = FDateTime::Now().ToString(TEXT("%H.%M.%S"));
	FString TimeN2 = FDateTime::Now().ToString();
	UE_LOG(LogTemp,Warning,TEXT("now time is %s"),*TimeN);
	UE_LOG(LogTemp,Warning,TEXT("now time2 is %s"),*TimeN2);
	TimeString = TimeN;
	return  TimeN;
}

int32 UDeviceInfoWidgetBase::GetBatteryLevel()
{
	int32 BatteryLevel = -1;
#if PLATFORM_ANDROID
	BatteryLevel = FAndroidMisc::GetBatteryLevel();
#endif
#if PLATFORM_IOS
	BatteryLevel = FIOSPlatformMisc::GetBatteryLevel();
#endif
	return BatteryLevel;
}
