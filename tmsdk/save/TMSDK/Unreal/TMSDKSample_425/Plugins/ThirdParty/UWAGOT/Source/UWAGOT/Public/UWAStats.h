// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include <vector>
#include <map>
#include <set>
#include <string>

#include "ThirdParty/UWALib/Public/IUWAStats.h"
#include "ThirdParty/UWALib/Public/uwa.h"

using std::vector;
using std::map;
using std::set;
using std::string;


#if	STATS
#include "CoreMinimal.h"
#include "Runtime/Core/Public/Stats/Stats2.h"
#include "Runtime/Core/Public/Stats/StatsFile.h"
#include "Runtime/Core/Public/Stats/StatsData.h"
#include "Runtime/Core/Public/Stats/StatsMallocProfilerProxy.h"
#include "Runtime/Core/Public/Async/TaskGraphInterfaces.h"
#endif


static void UWAStatCmd(FString Cmd);


#if	STATS
class FUWAAsyncStatsWrite;
class FUWAStatsWriteFile;

struct FUWA_StatsWriteFile : public FStatsWriteFile {
	friend class FUWAStatsWriteFile;

	// set callback for complete collecting all stats data
	virtual void SetDataDelegate(bool bSet) override;

	FUWAStatsWriteFile* StatsWriteFile;
};

/**
 * This class is used to save stats data to file
 */
class FUWAStatsWriteFile
{
	friend class FUWAAsyncStatsWrite;
protected:
	// get stats to FRawStatStackNode hierachy structure
	void GetRawStackStats(int64 TargetFrame);

	// get data from stat thread
	void GetCondensedHistory(int64 TargetFrame);

	UWAStatMessage ConvertMsg(FStatMessage msg);

	// send async save task
	void MySendTask(bool last);// override;

public:
	FUWAStatsWriteFile()
	{
		StatMsgMap = ThreadStatMsg::Create();
		MyAsyncTask = nullptr;
		_statsWriteFile.StatsWriteFile = this;
	}
	~FUWAStatsWriteFile()
	{
		ThreadStatMsg::Delete(&StatMsgMap);
	}
	// write data of a frame to file
	void MyWriteFrame(int64 TargetFrame);

	// start profile and save data to file
	void MyStart();

	// stop profile and close file
	void MyStop();

private:
	ThreadStatMsg *StatMsgMap;

	map<string, int32> StatsIdMap;
	map<string, int32> SampleIdMap;

	FAsyncTask<FUWAAsyncStatsWrite>* MyAsyncTask;
	int64 CurrentFrame;
	bool IsIdMapDirty;

	FUWA_StatsWriteFile _statsWriteFile;
};

struct FUWA_CommandStatsFile : public FCommandStatsFile
{
	friend class FUWACommandStatsFile;
};

class FUWACommandStatsFile
{
public:
	static FUWACommandStatsFile& MyGet()
	{
		static FUWACommandStatsFile Instance;
		return Instance;
	}

	// start profile
	void MyStart();

	// stop profile
	void MyStop();

private:
	FUWACommandStatsFile();

	FUWAStatsWriteFile* MyCurrentStatsFile;
	FUWA_CommandStatsFile CommandStatsFile;
};


// This class is used to save file using multithreading
class FUWAAsyncStatsWrite : public FNonAbandonableTask
{
public:
	FORCEINLINE TStatId GetStatId() const
	{
		return TStatId();
	}
	void DoWork();
	FUWAAsyncStatsWrite(FUWAStatsWriteFile* InStatsWriteFile, double FrequencyMHz, bool InIsSaveIdMap, int64 CurrentFrame);
	static VMsgMap* AllStatMsg;

private:

	double FrequencyMHz;
	bool IsSaveIdMap;
	int64 CurrentFrame;
};
VMsgMap* FUWAAsyncStatsWrite::AllStatMsg = nullptr;

#endif