//
//  TDataMasterDefines.h
//  TDM
//
//  Created by Morris on 16/8/23.
//  Copyright © 2016年 TDataMaster. All rights reserved.
//

#ifndef TDataMasterDefines_h
#define TDataMasterDefines_h

#include <string.h>

#define _TDM_Name_Space  namespace TDM
#define _USE_TDM_Name_Space  using namespace TDM;

#define MINIMUM_JSON_LEN 2 // The minimum json string length is {}

#if defined(_WIN32) || defined(_WIN64) || defined(_PLAY_STATION_)
	#ifdef WIN_LIB_API
		#define TDM_EXPORT
	#else
		#ifdef TDM_BUILD
			#define TDM_EXPORT __declspec(dllexport)
		#else
			#define TDM_EXPORT __declspec(dllimport)
		#endif
	#endif
#else
    #if __GNUC__ >= 4
        #define TDM_EXPORT __attribute__ ((visibility ("default")))
    #else
        #define TDM_EXPORT
    #endif
#endif


_TDM_Name_Space
{
    typedef enum
    {
        kGenderUnknow,
        kGenderMale,
        kGenderFemale,
        kGenderOthers
    }UserGender;
    
    typedef enum
    {
        kLogDebug,
        kLogInfo,
        kLogWarning,
        kLogError,
        kLogNone,
    }LogLevel;

    typedef enum{
        Default = -1,
        SUCCESS = 0,
        Encrypted = 1,
        NoPermission = 2,
        Disabled = 3,
        NoStart = 4,
        Collecting = 5,
        ParamError = 6,
        UnSupportSysVersion = 7,
        NotEnoughSpace = 8,
        FieldExist = 9,
        UnInitialized = 10,
        ExceptionOccur = 100,
    }TDMDeviceInfoStatus;
}

// Delete single pointer and delete multiple pointers
#define TDM_SAFE_DELETE(ptr) \
    do                   \
    {                    \
        if (ptr != NULL) \
        {                \
            delete ptr;  \
            ptr = NULL;  \
        }                \
    } while (0)

#define TDM_SAFE_DELETE_ARR(ptr) \
    do                       \
    {                        \
        if (ptr != NULL)     \
        {                    \
            delete[] ptr;    \
            ptr = NULL;      \
        }                    \
    } while (0)

// malloc or free mem
#define TDM_SAFE_MALLOC(num, type) (type *) calloc((num) , sizeof(type))

#define TDM_SAFE_FREE(ptr)   \
    do                   \
    {                    \
        if (ptr != NULL) \
        {                \
            free(ptr);   \
            ptr = NULL;  \
        }                \
    } while (0)

#endif /* TDataMasterDefines_h */
