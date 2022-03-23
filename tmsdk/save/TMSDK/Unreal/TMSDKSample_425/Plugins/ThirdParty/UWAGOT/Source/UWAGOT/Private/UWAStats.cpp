// Fill out your copyright notice in the Description page of Project Settings.


#include "UWAStats.h"
#include "Runtime/Core/Public/Misc/Paths.h"
#include "Runtime/Core/Public/HAL/FileManager.h"
#include "PlatformManager.h"
#include "UWAMacros.h"

#if	STATS

FUWAAsyncStatsWrite::FUWAAsyncStatsWrite(FUWAStatsWriteFile* InStatsWriteFile, double InFrequencyMHz, bool InIsSaveIdMap, int64 InCurrentFrame)
	: FrequencyMHz(InFrequencyMHz),
	IsSaveIdMap(InIsSaveIdMap),
	CurrentFrame(InCurrentFrame)
{
	check(!AllStatMsg->Dirty);

	AllStatMsg->Exchange((void*)InStatsWriteFile->StatMsgMap);
}

void FUWAAsyncStatsWrite::DoWork()
{
	AllStatMsg->Output(CurrentFrame);
	check(!AllStatMsg->Dirty);
}

void FUWA_StatsWriteFile::SetDataDelegate(bool bSet)
{
	FStatsThreadState const& Stats = FStatsThreadState::GetLocalState();
	if (bSet)
	{
		DataDelegateHandle = Stats.NewFrameDelegate.AddRaw(StatsWriteFile, &FUWAStatsWriteFile::MyWriteFrame);
	}
	else
	{
		Stats.NewFrameDelegate.Remove(DataDelegateHandle);
	}
}

TMap<FName,string> shortname;
UWAStatMessage FUWAStatsWriteFile::ConvertMsg(FStatMessage msg)
{
	EStatOperation::Type Op = msg.NameAndInfo.GetField<EStatOperation>();
	EStatDataType::Type da = msg.NameAndInfo.GetField<EStatDataType>();
	EStatMetaFlags::Type mt = msg.NameAndInfo.GetField<EStatMetaFlags>();

	const char* nm;
	FName name = msg.NameAndInfo.GetRawName();
	if (shortname.Contains(name))
		nm = shortname[name].c_str();
	else
		nm = shortname.Emplace(name, string(TCHAR_TO_UTF8(*FStatNameAndInfo::GetShortNameFrom(name).ToString()))).c_str();
	
	return UWAStatMessage(nm, msg.GetValue_int64(), (int)Op, (int)da, (int)mt);
}

void FUWAStatsWriteFile::GetRawStackStats(int64 TargetFrame)
{
	const FStatsThreadState& Stats = FStatsThreadState::GetLocalState();
	const FStatPacketArray& Frame = Stats.GetStatPacketArray(TargetFrame);

	for (int32 PacketIndex = 0; PacketIndex < Frame.Packets.Num(); PacketIndex++)
	{
		FStatPacket const& Packet = *Frame.Packets[PacketIndex];
		const FName ThreadName = Stats.GetStatThreadName(Packet);
		FString NameStr = ThreadName.ToString();

		vector<UWAStatMessage>* vec = StatMsgMap->Get((EUWAThreadType::Type)Packet.ThreadType, true);

		for (FStatMessage const& Item : Packet.StatMessages)
		{
			vec->emplace_back(ConvertMsg(Item));
		}
	}
}

void FUWAStatsWriteFile::GetCondensedHistory(int64 TargetFrame)
{
	GetRawStackStats(TargetFrame);
}

void FUWAStatsWriteFile::MyWriteFrame(int64 TargetFrame)
{
	CurrentFrame++;

	GetCondensedHistory(TargetFrame);

	MySendTask(false);
}


void FUWAStatsWriteFile::MySendTask(bool last)
{
	if (MyAsyncTask)
	{
		MyAsyncTask->EnsureCompletion();
		delete MyAsyncTask;
		MyAsyncTask = nullptr;
	}

	if (!last)
	{
		bool IsSaveIdMap = CurrentFrame % SAVE_IDMAP_INTERVAL == 0 && IsIdMapDirty;

		double FrequencyMHz = 0.000001 / FPlatformTime::GetSecondsPerCycle();
		MyAsyncTask = new FAsyncTask<FUWAAsyncStatsWrite>(this, FrequencyMHz, IsSaveIdMap, CurrentFrame);

		MyAsyncTask->StartBackgroundTask();

		if (IsSaveIdMap)
			IsIdMapDirty = false;
	}
}

void FUWAStatsWriteFile::MyStart()
{
	FString PathName = FPlatformManager::Get().GetCurrentDataDirectory();

	double FrequencyMHz = 0.000001 / FPlatformTime::GetSecondsPerCycle();
	UWAStatMessage::SetFrequency(FrequencyMHz);
	FUWAAsyncStatsWrite::AllStatMsg = VMsgMap::Create(TCHAR_TO_ANSI(*PathName), SCREEN_SHOT_INVERVAL);

	IsIdMapDirty = false;
	CurrentFrame = -1;

	{
		_statsWriteFile.SetDataDelegate(true);
		StatsMasterEnableAdd();
	}
}

void FUWAStatsWriteFile::MyStop()
{
	//if (_statsWriteFile.IsValid())
	{

		StatsMasterEnableSubtract();
		_statsWriteFile.SetDataDelegate(false);
		MySendTask(false);
		MySendTask(true);

		VMsgMap::Delete(&FUWAAsyncStatsWrite::AllStatMsg);
	}
}

FUWACommandStatsFile::FUWACommandStatsFile() :
	MyCurrentStatsFile(nullptr)
{}

void FUWACommandStatsFile::MyStart()
{
	UE_LOG(LogTemp, Log, TEXT("FUWACommandStatsFile Start"));
	MyStop();
	MyCurrentStatsFile = new FUWAStatsWriteFile();
	MyCurrentStatsFile->MyStart();

	CommandStatsFile.StatFileActiveCounter.Increment();
}

void FUWACommandStatsFile::MyStop()
{
	UE_LOG(LogTemp, Log, TEXT("FUWACommandStatsFile Stop"));
	if (MyCurrentStatsFile)
	{
		CommandStatsFile.StatFileActiveCounter.Decrement();
		MyCurrentStatsFile->MyStop();
	}
}

static void HandleStatCmd(FString InCmd)
{
	const TCHAR* Cmd = *InCmd;
	UE_LOG(LogTemp, Log, TEXT("UWAStatCmd Cmd : %s"), *InCmd);
	if (FParse::Command(&Cmd, TEXT("Start")))
	{
		FUWACommandStatsFile::MyGet().MyStart();
	}
	else if (FParse::Command(&Cmd, TEXT("Stop")))
	{
		FUWACommandStatsFile::MyGet().MyStop();
		FThreadStats::DisableRawStats();

		if (FStatsMallocProfilerProxy::HasMemoryProfilerToken())
		{
			if (FStatsMallocProfilerProxy::Get()->GetState())
			{
				// Disable memory profiler and restore default stats groups.
				FStatsMallocProfilerProxy::Get()->SetState(false);
				IStatGroupEnableManager::Get().StatGroupEnableManagerCommand(TEXT("default"));
			}
		}

		FStatsThreadState& Stats = FStatsThreadState::GetLocalState();
		Stats.ResetStatsForRawStats();

		// Disable displaying the raw stats memory overhead.
		FSimpleDelegateGraphTask::CreateAndDispatchWhenReady
		(
			FSimpleDelegateGraphTask::FDelegate::CreateRaw(
				&FLatestGameThreadStatsData::Get(),
				&FLatestGameThreadStatsData::NewData,
				(FGameThreadStatsData*)nullptr),
			TStatId(), nullptr, ENamedThreads::GameThread
		);
	}
}

static void UWAStatCmd(FString Cmd)
{
	ENamedThreads::Type ThreadType = ENamedThreads::GameThread;
	if (FPlatformProcess::SupportsMultithreading())
	{
		ThreadType = ENamedThreads::StatsThread;
	}

	// make sure these are initialized on the game thread
	FLatestGameThreadStatsData::Get();
	FStatGroupGameThreadNotifier::Get();

	DECLARE_CYCLE_STAT(TEXT("FSimpleDelegateGraphTask.StatCmd"),
	STAT_FSimpleDelegateGraphTask_StatCmd,
		STATGROUP_TaskGraphTasks);

	auto Delegate = FSimpleDelegateGraphTask::FDelegate::CreateStatic(&HandleStatCmd, Cmd);
	FGraphEventRef CompleteHandle = FSimpleDelegateGraphTask::CreateAndDispatchWhenReady(
		Delegate, TStatId(), nullptr, ThreadType);
}

#else
static void UWAStatCmd(FString Cmd) {}
#endif