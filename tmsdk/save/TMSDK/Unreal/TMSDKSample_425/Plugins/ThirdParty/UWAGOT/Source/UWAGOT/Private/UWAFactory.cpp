#include "UWAFactory.h"
#include "Runtime/Engine/Public/UnrealEngine.h"

DEFINE_LOG_CATEGORY(LogUWA);

void UWALog::Log(EUWALog::LogType type, const char * fmt, ...)
{
	va_list args;
	va_start(args, fmt);
	int n = ::vsnprintf(_logTemp, 256, fmt, args);
    va_end(args);

	FString log(_logTemp);

	switch (type)
	{
	case EUWALog::Warning:
		UE_LOG(LogUWA, Warning, TEXT("%s"), *log);
		break;
	case EUWALog::Error:
		UE_LOG(LogUWA, Error, TEXT("%s"), *log);
		break;
	default:
		UE_LOG(LogUWA, Log, TEXT("%s"), *log);
		break;
	}
}

IUWALog * UWAFactory::GetLog()
{
	return &LogInstance;
}
