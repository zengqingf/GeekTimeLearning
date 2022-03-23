#pragma once

#ifndef _SDKCALLRESULT_H_
#define _SDKCALLRESULT_H_

class TMSDKBRIDGE_API SDKCallResult
{
public:
	const int RESULT_SUCCESS = 0;
	const int RESULT_EXCEPTION = -1;

	virtual ~SDKCallResult() {}

	SDKCallResult() {}

	void SetCode(int c) { code = c; }
	void SetMsg(const FString& msg) { message = msg; }

	int GetCode() const { return code; }
	const FString& GetMsg() const { return message; }

protected:
	int code = 0;
	FString message = "";
};	

template<class T>
class TMSDKBRIDGE_API SDKCallResultTemp : public SDKCallResult
{
public:
	SDKCallResultTemp(const FJsonObject& jObj, const FString& key) {}

	const T& GetObj() const { return obj; }

protected:
	T obj;
};

template<>
class TMSDKBRIDGE_API SDKCallResultTemp<bool> : public SDKCallResult
{
public:
	SDKCallResultTemp(const FJsonObject& jObj, const FString& key)
	{
		obj = jObj.GetBoolField(key);
	}

	bool GetObj() const { return obj; }
protected:
	bool obj;
};

template<>
class TMSDKBRIDGE_API SDKCallResultTemp<FString> : public SDKCallResult
{
public:
	SDKCallResultTemp(const FJsonObject& jObj, const FString& key) 
	{
		obj = jObj.GetStringField(key);
	}

	const FString& GetObj() const { return obj; }
protected:
	FString obj;
};

template<>
class TMSDKBRIDGE_API SDKCallResultTemp<FJsonObject> : public SDKCallResult
{
public:
	SDKCallResultTemp(const FJsonObject& jObj, const FString& key)
	{
		auto& jptr = jObj.GetObjectField(key);
		if (jptr.IsValid())
		{
			obj = *(jptr);
		}
	}

	const FJsonObject& GetObj() const { return obj; }
protected:
	FJsonObject obj;
};

#endif //_SDKCALLRESULT_H_