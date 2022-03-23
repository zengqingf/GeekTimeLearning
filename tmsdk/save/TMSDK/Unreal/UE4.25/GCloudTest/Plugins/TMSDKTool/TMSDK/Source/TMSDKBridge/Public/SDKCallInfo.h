#pragma once

#ifndef _SDKCALLINFO_H_
#define _SDKCALLINFO_H_

#include "CoreMinimal.h"
#include "SDKCallArg.h"

class TMSDKBRIDGE_API SDKCallInfo
{
public:
	SDKCallInfo(int argCount);
	~SDKCallInfo();

	void ConvertToJson(FString &json);
	inline void SetName(const FString& _name) { this->name = _name; }
	inline void SetIsCallback(bool _isCallback) { this->callback = _isCallback; }
	inline void SetCallbackId(const FString& _callbackId) { this->callbackId = _callbackId; }

	//template<typename T>
	//void AddArg(const FString& key, T value) {
	//	SDKCallArgTemp<T> argTemp(key, value);  //局部变量不能返回出去
	//	//args.Emplace(&argTemp);  //会导致调用析构！
	//	args.Add(&argTemp);
	//}

	template<typename T>
	void AddArgTemp(SDKCallArgTemp<T> *argTemp) {
		args.Add(argTemp);
	}

	inline const FString& GetName() { return name; }
	inline bool IsCallback() { return callback; }
	inline const FString& GetCallbackId() { return callbackId; }
	inline const TArray<SDKCallArg *> GetArgs() { return args; }

private:
	FString name;
	TArray<SDKCallArg *> args;
	bool callback;
	FString callbackId;
	int argIndex;
};

#endif //_SDKCALLINFO_H_