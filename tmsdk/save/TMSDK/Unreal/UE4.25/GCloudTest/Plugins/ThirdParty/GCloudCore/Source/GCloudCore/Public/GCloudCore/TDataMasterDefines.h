//
//  TDataMasterDefines.h
//  TDM
//
//  Created by Morris on 16/8/23.
//  Copyright © 2016年 GCloud. All rights reserved.
//

#ifndef TDataMasterDefines_h
#define TDataMasterDefines_h

#define _TDM_Name_Space  namespace TDM

#if defined(_WIN32) || defined(_WIN64)
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
  
}

#endif /* TDataMasterDefines_h */
