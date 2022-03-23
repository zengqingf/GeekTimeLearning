#pragma once

#ifndef _SDKCALLARG_H_
#define _SDKCALLARG_H_

#include "CoreMinimal.h"
#include "Json.h"
//#include "JsonLibUtil.h"

//特化模板类需要用到的基类 需要前置！
class TMSDKBRIDGE_API SDKCallArg
{
public:
	virtual ~SDKCallArg() {
		(*jsonObj).Values.Empty();
		jsonObj.Reset();
	}

	SDKCallArg() {
		jsonObj = MakeShareable(new FJsonObject());
	}

	TSharedPtr<FJsonObject> GetJsonObj() const
	{
		return jsonObj;
	}

protected:
	TSharedPtr<FJsonObject> jsonObj;
};

template<class T>
class TMSDKBRIDGE_API SDKCallArgTemp : public SDKCallArg
{
public:
	SDKCallArgTemp(const FString& n, T v) : name(n), value(v) {
		jsonObj = MakeShareable(new FJsonObject());
	}
	//~SDKCallArgTemp() {
	//	value = nullptr;
	//	name = nullptr;
	//}

protected:
	FString name;
	T value;
};

template<>
class TMSDKBRIDGE_API SDKCallArgTemp<FString> : public SDKCallArg
{
public:

	SDKCallArgTemp(const FString& n, const FString& v) : name(n), value(v) {
		(*jsonObj).SetStringField("name", n);
		(*jsonObj).SetStringField("value", v);
	}
	~SDKCallArgTemp() {
		value.Empty();
		name.Empty();
	}

protected:
	FString name;
	FString value;
};

template<>
class TMSDKBRIDGE_API SDKCallArgTemp<bool> : public SDKCallArg
{
public:
	SDKCallArgTemp(const FString& n, bool v) : name(n), value(v) {
		(*jsonObj).SetStringField("name", n);
		(*jsonObj).SetBoolField("value", v);
	}
	~SDKCallArgTemp() {
		name.Empty();
		value = false;
	}

protected:
	FString name;
	bool value;
};

template<>
class TMSDKBRIDGE_API SDKCallArgTemp<double> : public SDKCallArg
{
public:

	SDKCallArgTemp(const FString& n, double v) : name(n), value(v) {
		(*jsonObj).SetStringField("name", n);
		(*jsonObj).SetNumberField("value", v);
	}
	~SDKCallArgTemp() {
		name.Empty();
		value = 0;
	}

protected:
	FString name;
	double value;
};

template<>
class TMSDKBRIDGE_API SDKCallArgTemp<float> : public SDKCallArg
{
public:

	SDKCallArgTemp(const FString& n, float v) : name(n), value(v) {
		(*jsonObj).SetStringField("name", n);
		(*jsonObj).SetNumberField("value", v);
	}
	~SDKCallArgTemp() {
		name.Empty();
		value = 0;
	}

protected:
	FString name;
	float value;
};

template<>
class TMSDKBRIDGE_API SDKCallArgTemp<uint32> : public SDKCallArg
{
public:

	SDKCallArgTemp(const FString& n, uint32 v) : name(n), value(v) {
		(*jsonObj).SetStringField("name", n);
		(*jsonObj).SetNumberField("value", v);
	}
	~SDKCallArgTemp() {
		name.Empty();
		value = 0;
	}

protected:
	FString name;
	uint32 value;
};

template<>
class TMSDKBRIDGE_API SDKCallArgTemp<uint64> : public SDKCallArg
{
public:

	SDKCallArgTemp(const FString& n, uint64 v) : name(n), value(v) {
		(*jsonObj).SetStringField("name", n);
		(*jsonObj).SetNumberField("value", v);
	}
	~SDKCallArgTemp() {
		name.Empty();
		value = 0;
	}

protected:
	FString name;
	uint64 value;
};

#endif //_SDKCALLARG_H_