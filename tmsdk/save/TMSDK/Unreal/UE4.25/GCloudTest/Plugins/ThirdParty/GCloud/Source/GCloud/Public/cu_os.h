/*
 * cu_os.h
 *
 *  Created on: 2014-3-20
 *      Author: mickeyxu
 */

#ifndef CU_OS_H_
#define CU_OS_H_

#include <string.h>
#if defined(_WIN32) || defined(_WIN64)
#pragma warning(push)
#pragma warning(disable:4668)
#endif

#if 0

//#define CU_OS_APPLE
#define CU_OS_ANDROID
#define CU_OS_LINUX
#define CU_OS_WIN32
#define CU_OS_MAC
#define CU_OS_IOS
#define CU_OS_ORBIS

#endif
// TODO check for the os type and try to distinguish the different os.

#ifdef WIN32
    #define CU_OS_WIN32 1
#else
    #ifdef __APPLE__
        #include "TargetConditionals.h"
        #if TARGET_OS_IOS || TARGET_OS_IPHONE
            #define CU_OS_IOS 1
        #else
            #define CU_OS_MAC 1
        #endif
    #else
        #ifdef ANDROID
            #define CU_OS_ANDROID 1
        #elif defined(__ORBIS__)
			#define CU_OS_ORBIS 1	
		#else	
            #define CU_OS_LINUX 1
            //#error "Unknown OS defined"
        #endif
    #endif
#endif

#if !defined(CU_OS_LINUX) && !defined(CU_OS_MAC) && !defined(CU_OS_ANDROID) && !defined(CU_OS_IOS) && !defined(CU_OS_WIN32) && !defined(CU_OS_ORBIS)
    #error "unknown os"
#endif

#if defined(CU_OS_LINUX) || defined(CU_OS_MAC) || defined(CU_OS_ANDROID) || defined(CU_OS_IOS) || defined(CU_OS_ORBIS)
    #define CU_OS_UNIX 1
#else
    #define CU_OS_UNIX 0
#endif

#ifdef CU_OS_WIN32
#define STDCALL __stdcall
#else
#ifdef __APPLE__
#define STDCALL
#else
#define STDCALL
#endif
//#error "stdcall not define"
#endif

#if defined(_WIN32) || defined(_WIN64)
#pragma warning(pop)
#endif

#endif /* CU_OS_H_ */
