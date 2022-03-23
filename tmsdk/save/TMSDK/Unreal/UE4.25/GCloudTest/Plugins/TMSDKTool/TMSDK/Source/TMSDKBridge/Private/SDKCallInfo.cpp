#include "SDKCallInfo.h"
#include "JsonLibUtil.h"

SDKCallInfo::SDKCallInfo(int argCount)
{
	name.Empty();
	args.Reserve(argCount);
	callback = false;
	callbackId.Empty();
	argIndex = 0;
} 

SDKCallInfo::~SDKCallInfo()
{
	name.Empty();
	args.Empty();	
	callbackId.Empty();
	argIndex = 0;
}

void SDKCallInfo::ConvertToJson(FString &json)
{
	//FJsonObject* jsonObj = new FJsonObject();
	TSharedPtr<FJsonObject> jsonObj = MakeShareable(new FJsonObject());
	jsonObj->SetStringField("name", name);
	TArray<TSharedPtr<FJsonValue>> argJValuePtrArray;
	for (auto& arg : args) {
		TSharedPtr<FJsonObject> aObjPtr = arg->GetJsonObj();

		//error:
		//FJsonValueObject aObjValue(aObjPtr);
		//TSharedPtr<FJsonValue> aObjValuePtr = MakeShareable(&aObjValue);

		TSharedPtr<FJsonValueObject> aObjValuePtr = MakeShareable(new FJsonValueObject(aObjPtr));
		argJValuePtrArray.Add(aObjValuePtr);
	}
	jsonObj->SetArrayField("args", argJValuePtrArray);
	jsonObj->SetStringField("callbackId", callbackId);
	jsonObj->SetBoolField("callback", callback);

	//test
	/*
	if (name.Equals("initCrashReport"))
	{
		TArray<TSharedPtr<FJsonValue>> js1 = jsonObj->GetArrayField("args");
		for (auto& j1 : js1) {
			TSharedPtr<FJsonObject> jo_ = j1->AsObject();
			if (jo_->HasField("appId"))
			{
				UE_LOG(LogTemp, Log, TEXT("TESTTEST : %s"), *(jo_->GetStringField("appId")));
			}
			else if (jo_->HasField("isDebug"))
			{
				UE_LOG(LogTemp, Log, TEXT("TESTTEST : %s"), jo_->GetBoolField("isDebug") ? TEXT("true"): TEXT("false"));
			}
		}
	}
	else if (name.Equals("setUserId"))
	{
		TArray<TSharedPtr<FJsonValue>> js1 = jsonObj->GetArrayField("args");
		for (auto& j1 : js1) {
			TSharedPtr<FJsonObject> jo_ = j1->AsObject();
			if (jo_->HasField("userId"))
			{
				UE_LOG(LogTemp, Log, TEXT("TESTTEST : %s"), *(jo_->GetStringField("userId")));
			}
		}
	}
	*/

	//jsonObj变为智能指针后 不需要delete销毁！
	JsonLibUtil::ToJson<TSharedPtr<FJsonObject>>(jsonObj, json);
	//delete jsonObj;
}