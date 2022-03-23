// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "Modules/ModuleManager.h"


/*
 *跨模块调用
 *ref: https://zhuanlan.zhihu.com/p/31296877
 * 
 */
DECLARE_MULTICAST_DELEGATE(FOnTMSDKSample)  /*跨模块调用：在子模块中申请主模块的*/

class TMSDKCOMMON_API FTMSDKCommonModule : public IModuleInterface  
{
public:

	/** IModuleInterface implementation */
	virtual void StartupModule() override;
	virtual void ShutdownModule() override;


	void _testCallSelf();
	
	static FOnTMSDKSample OnSample;
	static FOnTMSDKSample GetSampleDelegate()
	{
		return OnSample;							//使用这个，其他模块调用绑定会失效
	}
	//测试调用 简单地放在主模块注册回调后调用 正常应该在这个模块中调用
	static void TestCallMain();
};
