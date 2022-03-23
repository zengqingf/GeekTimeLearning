#include "LogMgr.h"
#include <stdio.h>
#include <stdlib.h>
#include <stdarg.h>
#include <string>
using namespace std;

string LogTypeStr[] = {
	"INFO",
	"DEBUG",
	"WARN",
	"ERROR",
	"FATAL",
	"MAX"
};

LogMgr* LogMgr::_instance = NULL;

LogMgr* LogMgr::Instance()
{
	if (_instance == NULL)
	{
		_instance = new LogMgr();
	}

	return _instance;
}

void LogMgr::Destory()
{
	if (_instance != NULL)
	{
		delete _instance;
		_instance = NULL;
	}
}

char buffer[1024];

void LogMgr::LogAP(LOGTYPE type, const char* fmt, va_list ap)
{
	//这里简单先用printf代替
	snprintf(buffer, sizeof(buffer),"[%s]%s\n", LogTypeStr[(int)type].c_str(), fmt);
	vprintf(buffer, ap);
}

bool LogMgr::Log(LOGTYPE type, const char* fmt, ...)
{
	//va_list args;
	//va_start(args, fmt);
	//LogAP(type, fmt, args);
	//va_end(args);

	return true;
}

/*
bool LogMgr::Info(const char* fmt, ...)
{
	va_list args;
	va_start(args, fmt);
	 LogAP(LOG_INFO, fmt, args);
	va_end(args);
	return true;
}

bool LogMgr::Warn(const char* fmt, ...)
{
	return true;
}

bool LogMgr::Debug(const char* fmt, ...)
{
	return true;
}

bool LogMgr::Error(const char* fmt, ...)
{
	return true;
}

bool LogMgr::Fatal(const char* fmt, ...)
{
	return true;
}
*/



