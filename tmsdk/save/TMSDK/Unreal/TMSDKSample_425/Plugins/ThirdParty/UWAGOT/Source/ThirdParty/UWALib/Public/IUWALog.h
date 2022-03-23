#pragma once
#include <stdio.h>
#include <string>

using std::string;

namespace EUWALog
{
	enum LogType : int
	{
		Log,
		Warning,
		Error
	};
}

class IUWALog {
public :
	virtual void Log(EUWALog::LogType type, const char* fmt, ...) = 0;
};