#pragma once
#include "IUWALog.h"

class IUWAFactory {
public :
	static IUWAFactory* Instance;
	static IUWALog* Log();

	virtual ~IUWAFactory(){}
protected:
	virtual IUWALog* GetLog() = 0;
};

#define UWA_LOG(type, fmt, ...) IUWAFactory::Log()->Log(EUWALog::type, fmt, ##__VA_ARGS__)
