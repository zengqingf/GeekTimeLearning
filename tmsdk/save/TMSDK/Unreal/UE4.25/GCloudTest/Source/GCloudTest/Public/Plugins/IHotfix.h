#pragma once

#include "HAL/Platform.h"
#include "GCloudDolphinInterface.h"
#include "Containers/UnrealString.h"

#include <functional>

enum TM_HotfixState
{
	TM_LocalFileCheck = 0,
	TM_GetVersionInfo,
	TM_AppUpdate,
	TM_AppMerge,
	TM_AppCheck,
	TM_ResUpdate,
	TM_ResCheck,
	TM_ResExtract,

	TM_Success,
	TM_Fail,

};

class HotfixCallBack
{
public:
	virtual ~HotfixCallBack() {}
	virtual void StartUpdate() = 0;
	virtual void ProgressUpdate(TM_HotfixState state, uint64 cur, uint64 max) = 0;
	virtual void OptionalUpdate(std::function<void()> ContinueUpdateAction, std::function<void()> ExitUpdateAction) = 0;
	virtual void Finish(const char* newVersionName) = 0;
	virtual void Error() = 0;
};