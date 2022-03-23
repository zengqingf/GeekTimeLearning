#pragma once

#include "UWALib/Public/IUWAFactory.h"
#include "UWALib/Public/IUWALog.h"
#include "UWALib/Public/IUWAStats.h"
#include "Core.h"

DECLARE_LOG_CATEGORY_EXTERN(LogUWA, Log, All);

class UWALog : public IUWALog {
public:
	// Inherited via IUWALog
	virtual void Log(EUWALog::LogType type, const char * fmt, ...) override;
	UWALog()
	{
		_logLen = 256;
		_logTemp = new char[_logLen];
	}
	virtual ~UWALog()
	{
		delete _logTemp;
		_logTemp = nullptr;
	}
private:
	int _logLen;
	char* _logTemp;
};

class UWAFactory : public IUWAFactory {
public:
	static UWAFactory& Get()
	{
		static UWAFactory FactoryInstance;
		return FactoryInstance;
	}

	virtual ~UWAFactory() {}
protected:
	// Inherited via IUWAFactory
	virtual IUWALog * GetLog() override;

private:
	UWALog LogInstance;
};