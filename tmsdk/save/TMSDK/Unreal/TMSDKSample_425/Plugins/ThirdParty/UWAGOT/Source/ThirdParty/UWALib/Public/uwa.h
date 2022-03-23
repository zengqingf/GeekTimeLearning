#pragma once

#include "IUWAFactory.h"


#ifdef UWALIBRARY_EXPORTS

	#if defined(__APPLE__) && defined(__MACH__)
	/* Apple OSX and iOS (Darwin). ------------------------------ */
	#include <TargetConditionals.h>
	#endif

	#if defined(__ANDROID_NDK_BUILD__) || defined(__ANDROID__)  // ANDROID
	#define PLATFORM_ANDROID 1
	#endif

	//#define PLATFORM_ANDROID 0
	// Which platform we are on?
	#if _MSC_VER
	#define PLATFORM_WINDOWS 1
	#elif defined(__APPLE__)
	#if TARGET_OS_MAC
	#if TARGET_OS_IPHONE   // iPhone
	#define PLATFORM_IOS 1
	#else   // Mac OS X
	#define PLATFORM_OSX 1
	#endif
	#endif
	#elif defined(__linux__)
	#define PLATFORM_LINUX 1
	#elif defined(PLATFORM_METRO)
	// these are defined externally
	#else
	#error "Unknown platform!"
	#endif


	#if defined PLATFORM_WINDOWS
		#define DLL_API _declspec(dllexport)
	#elif defined PLATFORM_LINUX
		#define DLL_API __attribute__((visibility("default")))
	#else
		#define DLL_API
	#endif
#else
	#if defined _WIN32 || defined _WIN64
		#define DLL_API _declspec(dllimport)
	#else
		#define DLL_API
	#endif
#endif


typedef unsigned char 		uint8;		// 8-bit  unsigned.
typedef unsigned short int	uint16;		// 16-bit unsigned.
typedef unsigned int		uint32;		// 32-bit unsigned.
typedef unsigned long long	uint64;		// 64-bit unsigned.

typedef	signed char			int8;		// 8-bit  signed.
typedef signed short int	int16;		// 16-bit signed.
typedef signed int	 		int32;		// 32-bit signed.
typedef signed long long	int64;		// 64-bit signed.

#if defined PLATFORM_WINDOWS
#define uintptr __int32
#elif defined PLATFORM_ANDROID || PLATFORM_IOS
#define uintptr(x) (uint32_t)(uintptr_t)(x)
#define max(a,b)            (((a) > (b)) ? (a) : (b))
#define BOOLEAN bool
#define FALSE false
#define TRUE true
#endif
#define MAX_uint32		((uint32)	0xffffffff)

#define LEGACY_DEBUG 0

DLL_API void UwaInit(IUWAFactory * factory);
DLL_API void UwaUpdate(int frame);

DLL_API void StartLuaTest(const char* Finalpath);

#ifndef SCREEN_SHOT_INVERVAL
	#define SCREEN_SHOT_INVERVAL 120
#endif
#ifndef ASSET_TAKE_SAMPLE_INTERVAL
	#define ASSET_TAKE_SAMPLE_INTERVAL 60
#endif
#ifndef SAVE_IDMAP_INTERVAL
	#define SAVE_IDMAP_INTERVAL 60
#endif