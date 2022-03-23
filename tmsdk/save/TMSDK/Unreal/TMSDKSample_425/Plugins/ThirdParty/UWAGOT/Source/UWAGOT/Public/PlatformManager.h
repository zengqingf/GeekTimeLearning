// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"

#if PLATFORM_ANDROID
#include "Runtime/Launch/Public/Android/AndroidJNI.h"
#include "Android/AndroidApplication.h"	
#endif

class IPlatformInterface;
class FPlatformAndroid;
class FPlatformIOS;
class FPlatformWindows;

/**
 * This class is the main manager for different platforms, including windows, ios, android and mac, et al.
 */
class UWAGOT_API FPlatformManager
{
public:

	static FPlatformManager& Get()
	{
		static FPlatformManager Instance;
		return Instance;
	}

	FPlatformManager();
	~FPlatformManager();

	// initialize folder et al.
	void Initialize() const;

	// utility for getting or creating profile data sub directory
	FString GetOrCreateProfileDataSubDirectory(FString SubDirectory = TEXT("")) const;

	// utility for getting directory of current test
	FString GetCurrentDataDirectory() const;

	// start profile event, used for doing something at the beginning of test
	void Start() const;

	// set mode event, used for doing something at the time of set mode
	void SetMode() const;

	// stop profile event, used for doing something at the end of test
	void Stop() const;

	// Screen shot implementation using native method
	bool NativeScreenShot(int32 FrameIndex) const;

	// Hardware info implementation using native method
	bool NativeHardwareInfo(int32 FrameIndex) const;

private:
	TSharedPtr<IPlatformInterface, ESPMode::ThreadSafe> Platform;
};

// parent class of different platform
class IPlatformInterface
{
public:
	IPlatformInterface();
	virtual ~IPlatformInterface();
	virtual void Initialize();
	FString CreateProfileDataSubDirectory(FString SubDirectory = TEXT(""));
	FString GetCurrentDataDirectory() const;
	virtual void Start();
	virtual void SetMode();
	virtual void Stop();
	virtual bool NativeScreenShot(int32 FrameIndex);
	virtual bool NativeHardwareInfo(int32 FrameIndex);

protected:
	void CreateProfileDataDirectory();
	FString GetProfileDataDirectory();
	void CreateDirectoryForCurrentData();
	void SaveDone() const;
	void SaveMode() const;

	// Platform Relative Method
	virtual void SaveSystemInfo() = 0;
	virtual FString GetPlatformRootDirectory() = 0;
	virtual FString GetPackageName() = 0;

protected:
	
	static const FString UWADirectory;
	static FString RootDirectory;
	static int32 ScreenShotInterval;
	FString CurrentDataDirectory;
	FDateTime StartTime;
	FDateTime StopTime;
	int32 CurrentFrameId;
};

// Android platform implementation
class FPlatformAndroid : public IPlatformInterface
{
public:
	virtual void Initialize() override;
	virtual FString GetPlatformRootDirectory() override;
	virtual void SaveSystemInfo() override;
	virtual void Start() override;
	virtual void SetMode() override;
	virtual void Stop() override;
	virtual bool NativeScreenShot(int32 FrameIndex) override;
	virtual bool NativeHardwareInfo(int32 FrameIndex) override;
	virtual FString GetPackageName() override;

private:
	void InitJNI();
	FString GetVersion();
	void LoadDex();

#if PLATFORM_ANDROID
	FString ConvertJStringToFString(jstring JStr);
	jstring ConvertFStringToJString(FString FStr);

	jmethodID GetVersionMethod;
	jmethodID LoadDexMethod;
	jmethodID CaptureScreenMethod;
	jmethodID CheckInfoToWriteMethod;
	jmethodID StopTrackMethod;
	jmethodID CopySystemInfoMethod;
	jmethodID GetApplicationNameMethod;
	jmethodID GetPackageNameMethod;
	jmethodID GetOrientationMethod;
	jmethodID GetBundleVersionMethod;
#endif

	bool bIsAndroidFiveOrHigher;
};

// IOS platform implementation
class FPlatformIOS : public IPlatformInterface
{
public:
	virtual void Initialize() override;
	virtual FString GetPlatformRootDirectory() override;
	virtual void SaveSystemInfo() override;
	virtual FString GetPackageName() override;
};

// Windows platform implementation
class FPlatformWindows : public IPlatformInterface
{
public:
	virtual void Initialize() override;
	virtual FString GetPlatformRootDirectory() override;
	virtual void SaveSystemInfo() override;
	virtual FString GetPackageName() override;
};
