// Fill out your copyright notice in the Description page of Project Settings.

#include "PlatformManager.h"
#include "ProfileManager.h"
#include "UWAMacros.h"
#include "Runtime/Core/Public/HAL/FileManager.h"
#include "Runtime/Core/Public/Serialization/BufferArchive.h"
#include "Runtime/Core/Public/Misc/FileHelper.h"
#include "Runtime/Engine/Public/UnrealEngine.h"
#include "Runtime/Core/Public/Misc/App.h"

#if PLATFORM_WINDOWS
#include "Runtime/Core/Public/Windows/WindowsPlatformMisc.h"
#include "GenericPlatform/GenericPlatformDriver.h"
#endif

FPlatformManager::FPlatformManager()
{
#if PLATFORM_ANDROID
	Platform = MakeShareable(new FPlatformAndroid());
#elif PLATFORM_IOS
	Platform = MakeShareable(new FPlatformIOS());
#elif PLATFORM_WINDOWS
	Platform = MakeShareable(new FPlatformWindows());
#endif
}

FPlatformManager::~FPlatformManager()
{
}

void FPlatformManager::Initialize() const
{
	Platform->Initialize();
}

FString FPlatformManager::GetOrCreateProfileDataSubDirectory(FString SubDirectory) const
{
	return Platform->CreateProfileDataSubDirectory(SubDirectory);
}

FString FPlatformManager::GetCurrentDataDirectory() const
{
	return Platform->GetCurrentDataDirectory();
}

void FPlatformManager::Start() const
{
	Platform->Start();
}

void FPlatformManager::SetMode() const
{
	Platform->SetMode();
}

void FPlatformManager::Stop() const
{
	Platform->Stop();
}

bool FPlatformManager::NativeScreenShot(int32 FrameIndex) const
{
	return Platform->NativeScreenShot(FrameIndex);
}

bool FPlatformManager::NativeHardwareInfo(int32 FrameIndex) const
{
	return Platform->NativeHardwareInfo(FrameIndex);
}


// Platform Interface -----------------------------------------------------------------
const FString IPlatformInterface::UWADirectory = TEXT("UWA-DataCenter/ProfileData/");
FString IPlatformInterface::RootDirectory = TEXT("");
int32 IPlatformInterface::ScreenShotInterval = SCREEN_SHOT_INVERVAL;
IPlatformInterface::IPlatformInterface()
{
}
IPlatformInterface::~IPlatformInterface()
{
}
void IPlatformInterface::Initialize()
{
	CreateProfileDataDirectory();
}

void IPlatformInterface::CreateProfileDataDirectory()
{
	CreateProfileDataSubDirectory();
}

FString IPlatformInterface::GetProfileDataDirectory()
{
	if (RootDirectory == TEXT(""))
	{
		RootDirectory = GetPlatformRootDirectory();
	}
	return RootDirectory + UWADirectory;
}

void IPlatformInterface::CreateDirectoryForCurrentData()
{
	FString PackageName = GetPackageName();
	CurrentDataDirectory = GetProfileDataDirectory() + 
		FString::Printf(TEXT("%s-%s/"), *PackageName, *(FDateTime::Now().ToString(TEXT("%Y%m%d%H%M%S"))));

	IFileManager& FileManager = IFileManager::Get();
	if (!FileManager.DirectoryExists(*CurrentDataDirectory))
	{
		if (!FileManager.MakeDirectory(*CurrentDataDirectory, true))
		{
			UE_LOG(LogTemp, Log, TEXT("Create Current Data Dir(%s) Failed!!"),
				*CurrentDataDirectory);
			return;
		}
	}

	UE_LOG(LogTemp, Log, TEXT("IPlatformInterface::CreateDirectoryForCurrentData %s Success!!"),
		*CurrentDataDirectory);
}

void IPlatformInterface::SaveDone() const
{
	FString FileName = CurrentDataDirectory + TEXT("done");
	IFileManager& FileManager = IFileManager::Get();
	FArchive* Archive = FileManager.CreateFileWriter(*FileName);
	Archive->Logf(TEXT("%d\n%f"), CurrentFrameId, (StopTime - StartTime).GetTotalSeconds());
	Archive->Flush();
	Archive->Close();
	delete Archive;
}

void IPlatformInterface::SaveMode() const
{
	FString FileName = CurrentDataDirectory + TEXT("mode");
	UE_LOG(LogTemp, Log, TEXT("IPlatformInterface::SaveMode ------------- %s"), *FileName);
	IFileManager& FileManager = IFileManager::Get();
	FArchive* Archive = FileManager.CreateFileWriter(*FileName);
	if (Archive)
	{
		Archive->Logf(TEXT("%s"), *(UProfileManager::Get()->GetModeName()));
		Archive->Flush();
		Archive->Close();
		delete Archive;
	}
	else
	{
		UE_LOG(LogTemp, Error, TEXT("Create Mode Archive failed"));
	}
}

FString IPlatformInterface::CreateProfileDataSubDirectory(FString SubDirectory)
{
	FString ProfileDataDir = GetProfileDataDirectory();
	FString FullDir = ProfileDataDir + SubDirectory;
	IFileManager& FileManager = IFileManager::Get();
	if (!FileManager.DirectoryExists(*FullDir))
	{
		if (!FileManager.MakeDirectory(*FullDir, true))
		{
			UE_LOG(LogTemp, Error, TEXT("Create Profile Data Sub Dir(%s) Failed!!"),
				*FullDir);
			return TEXT("");
		}
	}

	return FullDir;
}

FString IPlatformInterface::GetCurrentDataDirectory() const
{
	return CurrentDataDirectory;
}

void IPlatformInterface::Start()
{
	StartTime = FDateTime::Now();

	SaveSystemInfo();
}

void IPlatformInterface::SetMode()
{
	CreateDirectoryForCurrentData();
	SaveMode();
}

void IPlatformInterface::Stop()
{
	StopTime = FDateTime::Now();

	SaveDone();
}

bool IPlatformInterface::NativeScreenShot(int32 FrameIndex)
{
	CurrentFrameId = FrameIndex;
	return false;
}

bool IPlatformInterface::NativeHardwareInfo(int32 FrameIndex)
{
	return true;
}


// Windows -----------------------------------------------------------------
void FPlatformWindows::Initialize()
{
	IPlatformInterface::Initialize();

}

FString FPlatformWindows::GetPlatformRootDirectory()
{
	return TEXT("C:/");
}

void FPlatformWindows::SaveSystemInfo()
{
#if PLATFORM_WINDOWS
	FString DeviceID = FWindowsPlatformMisc::GetDeviceId();
	FString DeviceName = "<unknown>";
	FString ApplicationName = FApp::GetName();
	FString PackageName = GetPackageName();
	FString OperatingSystem, OSSubVersion = "";

	if (FWindowsPlatformMisc::VerifyWindowsVersion(10, 0)) OperatingSystem = "Windows 10";
	else FWindowsPlatformMisc::GetOSVersions(OperatingSystem, OSSubVersion);

	if (!OSSubVersion.IsEmpty()) OperatingSystem.Append(FString::Printf(TEXT(".%s"), *OSSubVersion));

	if (FWindowsPlatformMisc::Is64bitOperatingSystem()) OperatingSystem += " 64bit";
	else OperatingSystem += " 32bit";

	int32 ProcessorCount = 8;
	//FString CPUVendor = FWindowsPlatformMisc::GetCPUVendor();
	FString CPUBrand = FWindowsPlatformMisc::GetCPUBrand();
	//FString ProcessorType = FString::Printf(TEXT("%s %s"), *CPUVendor, *CPUBrand);
	FString ProcessorType = CPUBrand;
	const FPlatformMemoryConstants& MemoryConstants = FPlatformMemory::GetConstants();
	int64 SystemMemorySize = MemoryConstants.TotalPhysical / 1024 / 1024;
	FString Platform = "WindowsPlayer";
	FString SystemLanguage = "zh-CN";
	FString ScreenResolution = FString::Printf(TEXT("%dx%d"), GSystemResolution.ResX, GSystemResolution.ResY);
	FString ScreenOrientation = "Landscape";
	int32 ProcessorFrequency = 1 / FPlatformTime::GetSecondsPerCycle();
	FString BundleVersion = "";
	FString BundleVersionCode = "";
	FString UnrealVersion = FString::Printf(TEXT("Unreal %d.%d.%d"), ENGINE_MAJOR_VERSION, ENGINE_MINOR_VERSION, ENGINE_PATCH_VERSION);

	FString GraphicsDeviceName = GRHIAdapterName;
	FString GraphicsDeviceVersion = GDynamicRHI == nullptr ? "<unknown>" : FString(GDynamicRHI->GetName());
	FString TestMode = UProfileManager::Get()->GetModeName();

	FString SimCPUBrand = CPUBrand;
	SimCPUBrand = SimCPUBrand.Replace(TEXT("(R)"), TEXT(""));
	SimCPUBrand = SimCPUBrand.Replace(TEXT("(TM)"), TEXT(""));

	int32 iIndex = SimCPUBrand.Find("i7");
	if (iIndex == -1) SimCPUBrand.Find("i5");
	int32 atIndex = SimCPUBrand.Find("@");

	if (iIndex != -1 && atIndex != -1 && atIndex > iIndex + 1)
	{
		SimCPUBrand = SimCPUBrand.Left(iIndex + 2) + SimCPUBrand.Right(SimCPUBrand.Len() - atIndex - 1);
	}

	FString DeviceModel = FString::Printf(TEXT("PC(%s/%s)"), *SimCPUBrand, *GRHIAdapterName);

	FString FileName = CurrentDataDirectory + TEXT("systemInfo");
	IFileManager& FileManager = IFileManager::Get();
	FArchive* Archive = FileManager.CreateFileWriter(*FileName);
	Archive->Logf(TEXT("StartTime:%s"), *(StartTime.ToString(TEXT("%Y%m%d%H%M%S"))));
	Archive->Logf(TEXT("PackageName:%s"), *PackageName);
	Archive->Logf(TEXT("AppName:%s"), *ApplicationName);
	Archive->Logf(TEXT("PluginVersion:2.0.1.0"));
	Archive->Logf(TEXT("BundleVersion:%s"), *BundleVersion);
	Archive->Logf(TEXT("BundleVersionCode:%s"), *BundleVersionCode);
	Archive->Logf(TEXT("UnrealVersion:%s"), *UnrealVersion);
	Archive->Logf(TEXT("TestMode:%s"), *TestMode);
	Archive->Logf(TEXT("Platform:%s"), *Platform);
	Archive->Logf(TEXT("OperatingSystem:%s"), *OperatingSystem);
	Archive->Logf(TEXT("DeviceID:%s"), *DeviceID);
	Archive->Logf(TEXT("DeviceModel:%s"), *DeviceModel);
	Archive->Logf(TEXT("GraphicsDeviceName:%s"), *GraphicsDeviceName);
	Archive->Logf(TEXT("GraphicsDeviceVersion:%s"), *GraphicsDeviceVersion);
	Archive->Logf(TEXT("ProcessorCount:%d"), ProcessorCount);
	Archive->Logf(TEXT("ProcessorFrequency:%d"), ProcessorFrequency);
	Archive->Logf(TEXT("ProcessorType:%s"), *ProcessorType);
	Archive->Logf(TEXT("SystemMemorySize:%lld"), SystemMemorySize);
	Archive->Logf(TEXT("SystemLanguage:%s"), *SystemLanguage);
	Archive->Logf(TEXT("ScreenResolution:%s"), *ScreenResolution);
	Archive->Logf(TEXT("ScreenOrientation:%s"), *ScreenOrientation);
	Archive->Flush();
	Archive->Close();
	delete Archive;
#endif
}

FString FPlatformWindows::GetPackageName()
{
	return FApp::GetProjectName();
}



// Android -----------------------------------------------------------------
void FPlatformAndroid::Initialize()
{
	IPlatformInterface::Initialize();

	InitJNI();

	FString Version = GetVersion();
	FString BigVersion;
	Version.Split(".", &BigVersion, nullptr);
	bIsAndroidFiveOrHigher = FCString::Atoi(*BigVersion) != 4;

	//LoadDex();
}

FString FPlatformAndroid::GetPlatformRootDirectory()
{
	FString Result = TEXT("");
#if PLATFORM_ANDROID
	extern FString GFilePathBase;
	Result = GFilePathBase + "/";
#endif

	return Result;
}

void FPlatformAndroid::SaveSystemInfo()
{
	UE_LOG(LogTemp, Log, TEXT("FPlatformAndroid::SaveSystemInfo()"));
#if PLATFORM_ANDROID
	FString PackageName = "";
	if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
	{
		jstring PackageNameJStr = (jstring)FJavaWrapper::CallObjectMethod(Env, FJavaWrapper::GameActivityThis, GetPackageNameMethod);
		PackageName = ConvertJStringToFString(PackageNameJStr);
	}

	FString ApplicationName = "";
	if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
	{
		jstring ApplicationNameJStr = (jstring)FJavaWrapper::CallObjectMethod(Env, FJavaWrapper::GameActivityThis, GetApplicationNameMethod);
		ApplicationName = ConvertJStringToFString(ApplicationNameJStr);
	}

	FString BundleVersion = "";
	if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
	{
		jstring BundleVersionJStr = (jstring)FJavaWrapper::CallObjectMethod(Env, FJavaWrapper::GameActivityThis, GetBundleVersionMethod);
		BundleVersion = ConvertJStringToFString(BundleVersionJStr);
	}

	FString UnrealVersion = FString::Printf(TEXT("Unreal %d.%d.%d"), ENGINE_MAJOR_VERSION, ENGINE_MINOR_VERSION, ENGINE_PATCH_VERSION);
	FString TestMode = UProfileManager::Get()->GetModeName();
	FString Platform = TEXT("Android");
	FString OperatingSystem = "Android OS " + FAndroidMisc::GetAndroidVersion();
	FString DeviceID = FAndroidMisc::GetDeviceId();
	FString DeviceModel = FAndroidMisc::GetDeviceMake() + " " + FAndroidMisc::GetDeviceModel();
	FString GraphicsDeviceName = FAndroidMisc::GetGPUFamily();
	FString GraphicsDeviceVersion = FAndroidMisc::GetGLVersion();
	int32 ProcessorCount = FAndroidMisc::NumberOfCores();
	int32 ProcessorFrequency = 1 / FPlatformTime::GetSecondsPerCycle();
	FString ProcessorType = "";
	if (FILE* FileGlobalCpuStats = fopen("/proc/cpuinfo", "r"))
	{
		char LineBuffer[256] = { 0 };
		do
		{
#if ENGINE_MAJOR_VERSION == 4 && ENGINE_MINOR_VERSION >= 25
			char *Line = fgets(LineBuffer, UE_ARRAY_COUNT(LineBuffer), FileGlobalCpuStats);
#else
			char *Line = fgets(LineBuffer, ARRAY_COUNT(LineBuffer), FileGlobalCpuStats);
#endif
			if (Line == nullptr)
			{
				break;	// eof or an error
			}
			// count the number of processor entries in loop
			// for Lumin one processor translates to one core
			if (strstr(Line, "Processor") == Line)
			{
				ProcessorType = ANSI_TO_TCHAR(Line);
				break;
			}
		} while (1);
		fclose(FileGlobalCpuStats);
	}
	if (ProcessorType != "")
	{
		FString RightStr;
		ProcessorType.Split(":", nullptr, &RightStr);
		RightStr.TrimStartAndEndInline();
		ProcessorType = RightStr;
	}
	else
	{
		ProcessorType = "<unknown>";
	}
	FPlatformMemoryConstants MemoryStats = FPlatformMemory::GetConstants();
	int64 SystemMemorySize = MemoryStats.TotalPhysical / 1024 / 1024;
	FString SystemLanguage = FAndroidMisc::GetOSLanguage();
	FString ScreenResolution = FString::Printf(TEXT("%dx%d"), GSystemResolution.ResX, GSystemResolution.ResY);
	FString Orientation = "<unknown>";
	if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
	{
		jstring OrientationJStr = (jstring)FJavaWrapper::CallObjectMethod(Env, FJavaWrapper::GameActivityThis, GetOrientationMethod);
		Orientation = ConvertJStringToFString(OrientationJStr);
	}

	FString FileName = CurrentDataDirectory + TEXT("systemInfo");
	IFileManager& FileManager = IFileManager::Get();
	FArchive* Archive = FileManager.CreateFileWriter(*FileName);
	Archive->Logf(TEXT("StartTime:%s"), *(StartTime.ToString(TEXT("%Y%m%d%H%M%S"))));
	Archive->Logf(TEXT("PackageName:%s"), *PackageName);
	Archive->Logf(TEXT("AppName:%s"), *ApplicationName);
	Archive->Logf(TEXT("PluginVersion:2.0.1.0"));
	Archive->Logf(TEXT("BundleVersion:%s"), *BundleVersion);
	Archive->Logf(TEXT("UnrealVersion:%s"), *UnrealVersion);
	Archive->Logf(TEXT("TestMode:%s"), *TestMode);
	Archive->Logf(TEXT("Platform:%s"), *Platform);
	Archive->Logf(TEXT("OperatingSystem:%s"), *OperatingSystem);
	Archive->Logf(TEXT("DeviceID:%s"), *DeviceID);
	Archive->Logf(TEXT("DeviceModel:%s"), *DeviceModel);
	Archive->Logf(TEXT("GraphicsDeviceName:%s"), *GraphicsDeviceName);
	Archive->Logf(TEXT("GraphicsDeviceVersion:%s"), *GraphicsDeviceVersion);
	Archive->Logf(TEXT("ProcessorCount:%d"), ProcessorCount);
	Archive->Logf(TEXT("ProcessorFrequency:%d"), ProcessorFrequency);
	Archive->Logf(TEXT("ProcessorType:%s"), *ProcessorType);
	Archive->Logf(TEXT("SystemMemorySize:%lld"), SystemMemorySize);
	Archive->Logf(TEXT("SystemLanguage:%s"), *SystemLanguage);
	Archive->Logf(TEXT("ScreenResolution:%s"), *ScreenResolution);
	Archive->Logf(TEXT("ScreenOrientation:%s"), *Orientation);
	Archive->Flush();
	Archive->Close();
	delete Archive;
#endif
}

void FPlatformAndroid::Start()
{
	IPlatformInterface::Start();
}

void FPlatformAndroid::SetMode()
{
	IPlatformInterface::SetMode();
	LoadDex();
}

void FPlatformAndroid::Stop()
{
	IPlatformInterface::Stop();
#if PLATFORM_ANDROID
	if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
	{
		FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, StopTrackMethod);
	}
#endif
}

bool FPlatformAndroid::NativeScreenShot(int32 FrameIndex)
{
	IPlatformInterface::NativeScreenShot(FrameIndex);

	if (FrameIndex % ScreenShotInterval == 0 && bIsAndroidFiveOrHigher)
	{
#if PLATFORM_ANDROID
		FString FileName = CurrentDataDirectory + FString::Printf(TEXT("%d.jpg"), FrameIndex);
		//UE_LOG(LogTemp, Log, TEXT("FPlatformAndroid::CaptureScreen ---------- SavePath : %s"), *SavePath);
		if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
		{
			jstring FileNameJStr = ConvertFStringToJString(FileName);
			FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, CaptureScreenMethod, FileNameJStr);
		}
#endif
	}

	return true;
}

bool FPlatformAndroid::NativeHardwareInfo(int32 FrameIndex)
{
#if PLATFORM_ANDROID
	if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
	{
		FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, CheckInfoToWriteMethod, FrameIndex);
	}
#endif
	return true;
}

FString FPlatformAndroid::GetPackageName()
{
	FString PackageName = TEXT("<unknown>");
#if PLATFORM_ANDROID
	if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
	{
		jstring PackageNameJStr = (jstring)FJavaWrapper::CallObjectMethod(Env, FJavaWrapper::GameActivityThis, GetPackageNameMethod);
		PackageName = ConvertJStringToFString(PackageNameJStr);
	}
#endif
	return PackageName;
}

void FPlatformAndroid::InitJNI()
{
#if PLATFORM_ANDROID
	if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
	{
		GetVersionMethod = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_GetSdkVersion", "()Ljava/lang/String;", false);
		LoadDexMethod = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_LoadDex", "(Ljava/lang/String;Ljava/lang/String;)V", false);
		CaptureScreenMethod = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_CaptureScreen", "(Ljava/lang/String;)V", false);
		CheckInfoToWriteMethod = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_CheckInfoToWrite", "(I)V", false);
		StopTrackMethod = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_StopTrack", "()V", false);
		GetApplicationNameMethod = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_GetApplicationName", "()Ljava/lang/String;", false);
		GetPackageNameMethod = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_GetPackageName", "()Ljava/lang/String;", false);
		GetOrientationMethod = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_GetOrientation", "()Ljava/lang/String;", false);
		GetBundleVersionMethod = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_GetBundleVersion", "()Ljava/lang/String;", false);
	}
#endif
}

FString FPlatformAndroid::GetVersion()
{
	FString Version = TEXT("");

#if PLATFORM_ANDROID
	if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
	{
		jstring VersionJStr = (jstring)FJavaWrapper::CallObjectMethod(Env, FJavaWrapper::GameActivityThis, GetVersionMethod);
		Version = ConvertJStringToFString(VersionJStr);
	}
#endif

	return Version;
}

void FPlatformAndroid::LoadDex()
{
#if PLATFORM_ANDROID
	FString DexPath = GetProfileDataDirectory() + TEXT("uwa.dex");
	UWAAssetStats::Get().SaveDex(TCHAR_TO_UTF8(*DexPath));

	if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
	{
		jstring DexPathJStr = ConvertFStringToJString(UWADirectory);
		jstring CurrentDataDirJStr;
		if (UProfileManager::Get()->ProfileMode == EProfileMode::PM_Overview)
			CurrentDataDirJStr = ConvertFStringToJString(CurrentDataDirectory);
		else
			CurrentDataDirJStr = ConvertFStringToJString(FString(""));

		FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, LoadDexMethod, DexPathJStr, CurrentDataDirJStr);
	}
#endif
}

#if PLATFORM_ANDROID
FString FPlatformAndroid::ConvertJStringToFString(jstring JStr)
{
	FString FStr = TEXT("");
	if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
	{
		const char *NativeVersionString = Env->GetStringUTFChars(JStr, 0);
		FStr = FString(NativeVersionString);
		Env->ReleaseStringUTFChars(JStr, NativeVersionString);
		Env->DeleteLocalRef(JStr);
	}
	return FStr;
}

jstring FPlatformAndroid::ConvertFStringToJString(FString FStr)
{
	jstring JStr = jstring("");
	if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
	{
		JStr = Env->NewStringUTF(TCHAR_TO_UTF8(*FStr));
	}
	return JStr;
}
#endif

// IOS -----------------------------------------------------------------
void FPlatformIOS::Initialize()
{
	IPlatformInterface::Initialize();

}

FString FPlatformIOS::GetPlatformRootDirectory()
{
	return FString();
}

void FPlatformIOS::SaveSystemInfo()
{
}

FString FPlatformIOS::GetPackageName()
{
	return FString();
}
