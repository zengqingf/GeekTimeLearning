#pragma once

#ifndef _JSONLIBUTIL_H_
#define _JSONLIBUTIL_H_

#include "CoreMinimal.h"
#include "Json.h"

typedef TJsonWriterFactory< TCHAR, TCondensedJsonPrintPolicy<TCHAR> > FCondensedJsonStringWriterFactory;
typedef TJsonWriter< TCHAR, TCondensedJsonPrintPolicy<TCHAR> > FCondensedJsonStringWriter;

typedef TJsonWriterFactory< TCHAR, TPrettyJsonPrintPolicy<TCHAR> > FPrettyJsonStringWriterFactory;
typedef TJsonWriter< TCHAR, TPrettyJsonPrintPolicy<TCHAR> > FPrettyJsonStringWriter;

enum TMSDKBRIDGE_API JsonLibType
{
	Default
};

class TMSDKBRIDGE_API JsonLibUtil
{
private:
	static const JsonLibType type;

public:

	template<typename T, typename U>
	static void ToJson(T obj, U &outStr)
	{
	}

	template<typename T, typename U>
	static void ToObject(T &json, U &obj)
	{
	}

	template<typename T>
	//static void ToJson(FJsonObject* obj, FString& outStr)
	static void ToJson(TSharedPtr<FJsonObject>& jObjPtr, FString& outStr)
	{
		switch (type) {
		case JsonLibType::Default:
			TSharedRef<FCondensedJsonStringWriter> writer = FCondensedJsonStringWriterFactory::Create(&outStr);
			//TSharedPtr<FJsonObject> jObjPtr = MakeShareable(obj);
			if (jObjPtr.IsValid() && jObjPtr->Values.Num() > 0)
			{
				FJsonSerializer::Serialize(jObjPtr.ToSharedRef(), writer);
				jObjPtr.Reset();
				writer->Close();
			}
			break;
		}
	}


	template<typename T>
	//static void ToObject(const FString& json, FJsonObject*& obj)
	static void ToObject(const FString& json, TSharedPtr<FJsonObject>& objPtr)
	{
		if (json.IsEmpty()) 
		{
			return;
		}
		switch (type) {
		case JsonLibType::Default:
			TSharedRef<TJsonReader<>> reader = TJsonReaderFactory<>::Create(json);
			//TSharedPtr<FJsonObject> objPtr = MakeShareable(obj);
			bool bSucc = FJsonSerializer::Deserialize(reader, objPtr);
			if (bSucc && objPtr.IsValid()) 
			{
				UE_LOG(LogTemp, Log, TEXT("To Object : %d, %s"), objPtr->GetIntegerField("code"), *(objPtr->GetStringField("message")));
			}
			break;
		}
	}
};

#endif //_JSONLIBUTIL_H_