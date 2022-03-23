// Fill out your copyright notice in the Description page of Project Settings.


#include "Sample_2.h"
#include "Json.h"
#include "JsonUtilities.h"

#include "TMSDKManager.h"

Sample_2::Sample_2()
{
	//TMSDKEventManager.GetInstance().RegisterEvent(SDKEventType::GetTouchScreenPos, SDK_EVENT_FUNC(_onTouchScreenPos));
}

Sample_2::~Sample_2()
{
}

void Sample_2::Init()
{
	Test1();
	Test2();

	TMSDKManager::GetInstance().Initialize();
	TMSDKManager::GetInstance().SetUserId("5211314a");
}

void Sample_2::Test1()
{
	//[UE4 JSON封装和解析](https://blog.csdn.net/qq_31930499/article/details/88366530)

	//构造json 数组
	FString Grades = "test";
	TArray<FString> Name;
	Name.Add("zhangsan");
	Name.Add("lisi");
	Name.Add("wangwu");

	FString JsonOutString;
	TSharedRef<FCondensedJsonStringWriter> Writer = FCondensedJsonStringWriterFactory::Create(&JsonOutString);

	Writer->WriteObjectStart();

	//wchar_t  not support on android clang
	//Writer->WriteValue(L"grades", Grades);
	//Writer->WriteValue(L"member", Name); //普通数组

	Writer->WriteValue("grades", Grades);
	Writer->WriteValue("member", Name); //普通数组
	Writer->WriteObjectEnd();
	Writer->Close();

	//print
	UE_LOG(LogTemp, Log, TEXT("UE4 JSON TEST 1 : %s"), *JsonOutString);


	//解析JSON普通数组
	FString grades;
	TArray<FString> member;

	TSharedRef< TJsonReader<> > Reader = TJsonReaderFactory<>::Create(JsonOutString);
	TSharedPtr<FJsonObject> rRoot;
	bool bSuccessful = FJsonSerializer::Deserialize(Reader, rRoot);
	if (bSuccessful) {
		if (rRoot->HasField(TEXT("grades"))) {
			grades = rRoot->GetStringField(TEXT("grades"));
		}

		const TArray< TSharedPtr<FJsonValue> >* members;
		if (rRoot->TryGetArrayField(TEXT("member"), members)) {
			for (int32 i = 0; i < (*members).Num(); ++i) {
				member.Add((*members)[i]->AsString());
			}
		}
	}

	UE_LOG(LogTemp, Log, TEXT("UE4 JSON TEST 2, grades : %s"), *grades);
	for (int i = 0; i < member.Num(); ++i) {
		UE_LOG(LogTemp, Log, TEXT("UE4 JSON TEST 2 : %s"), *member[i]);
	}
}

void Sample_2::Test2()
{
	//构造JSON对象数组
	struct Info {
		FString name;
		int age;
	};

	FString Class = "test";
	TArray<struct Info> Name;
	struct Info info;
	info.name = TEXT("zhangsan");
	info.age = 18;
	Name.Emplace(info);
	info.name = TEXT("lisi");
	info.age = 19;
	Name.Emplace(info);
	info.name = TEXT("wangwu");
	info.age = 20;
	Name.Emplace(info);

	FString JsonOutString;
	TSharedRef<FCondensedJsonStringWriter> Writer = FCondensedJsonStringWriterFactory::Create(&JsonOutString);

	Writer->WriteObjectStart();            //JSON对象开始

	//Writer->WriteValue(L"class", Class);   //JSON对象普通成员
	Writer->WriteValue("class", Class);   //JSON对象普通成员

	//Writer->WriteArrayStart(L"member");    //JSON对象数组成员开始
	Writer->WriteArrayStart("member");    //JSON对象数组成员开始

	for (int32 i = 0; i < Name.Num(); ++i)
	{
		Writer->WriteObjectStart();

		//Writer->WriteValue(L"name", Name[i].name);
		//Writer->WriteValue(L"age", Name[i].age);

		Writer->WriteValue("name", Name[i].name);
		Writer->WriteValue("age", Name[i].age);
		Writer->WriteObjectEnd();
	}
	Writer->WriteArrayEnd();               //JSON对象数组成员结束
	Writer->WriteObjectEnd();              //JSON对象结束
	Writer->Close();

	//print
	UE_LOG(LogTemp, Log, TEXT("UE4 JSON TEST 3 : %s"), *JsonOutString);


	//解析Json对象数组

	TSharedRef< TJsonReader<> > Reader = TJsonReaderFactory<>::Create(JsonOutString);

	FString grades;
	TArray<Info> member;
	TSharedPtr<FJsonObject> rRoot;
	bool bSuccessful = FJsonSerializer::Deserialize(Reader, rRoot);
	if (bSuccessful) {
		if (rRoot->HasField(TEXT("class"))) {
			grades = rRoot->GetStringField(TEXT("class"));
		}

		const TArray< TSharedPtr<FJsonValue> >* members;
		if (rRoot->TryGetArrayField(TEXT("member"), members)) {
			for (int32 i = 0; i < (*members).Num(); ++i) {
				TSharedPtr<FJsonObject> jsonMember = (*members)[i]->AsObject();
				struct Info person;
				person.name = jsonMember->GetStringField(TEXT("name"));
				person.age = jsonMember->GetIntegerField(TEXT("age"));
				//member.Emplace(person); //会导致调用析构！
				member.Add(person);
			}
		}
	}

	//print
	UE_LOG(LogTemp, Log, TEXT("UE4 JSON TEST 4, grades = %s"), *grades);
	for (int i = 0; i < member.Num(); ++i) {
		UE_LOG(LogTemp, Log, TEXT("UE4 JSON TEST 4 : %s %d"), *member[i].name, member[i].age);
	}

	//方法二：
	//FJsonValue 范围 > FJsonObject

	FString vgrades;
	TArray<Info> vmember;
	TSharedPtr<FJsonValue> vRoot;
	bool bSucc = FJsonSerializer::Deserialize(Reader, vRoot);
	if (bSucc) {
		TSharedPtr<FJsonValue> priceValue = vRoot->AsObject()->GetField<EJson::String>(TEXT("class"));
		vgrades = priceValue->AsString();

		TSharedPtr<FJsonValue> memberObj = vRoot->AsObject()->GetField<EJson::Array>(TEXT("member"));
		const TArray<TSharedPtr<FJsonValue> > vmembers = memberObj->AsArray();
		for (int32 i = 0; i < vmembers.Num(); ++i) {
			TSharedPtr<FJsonValue> vjsonMember = vmembers[i];
			struct Info vperson;
			vperson.name = vjsonMember->AsObject()->GetStringField(TEXT("name"));
			vperson.age = vjsonMember->AsObject()->GetIntegerField(TEXT("age"));
			vmember.Add(vperson);
		}
	}

	//print
	UE_LOG(LogTemp, Log, TEXT("UE4 JSON TEST 5, grades = %s"), *vgrades);
	for (int i = 0; i < vmember.Num(); ++i) {
		UE_LOG(LogTemp, Log, TEXT("UE4 JSON TEST 5 : %s %d"), *vmember[i].name, vmember[i].age);
	}
}

void Sample_2::_onTouchScreenPos(const SDKEventParam& param)
{
	UE_LOG(LogTemp, Log, TEXT("### on touch screen pos : x %f, y %f"), param.float_0, param.float_1);
}


void Sample_2::Test3()
{
	//构造JSON对象数组
	struct ServerInfo {
		FString name;
		FString ip;
	};

	TArray<struct ServerInfo> serverInfoArray;
	struct ServerInfo info;
	info.name = TEXT("开发服务器");
	info.ip = TEXT("192.168.2.18:7031");
	serverInfoArray.Emplace(info);
	info.name = TEXT("外网服务器(手机包)");
	info.ip = TEXT("116.62.215.137:7031");
	serverInfoArray.Emplace(info);

	FString jsonOutStr;
	TSharedRef<FCondensedJsonStringWriter> jsonWriter = FCondensedJsonStringWriterFactory::Create(&jsonOutStr);
	jsonWriter->WriteArrayStart();
	for (int32 i = 0; i < serverInfoArray.Num(); ++i)
	{
		jsonWriter->WriteObjectStart();
		jsonWriter->WriteValue("name", serverInfoArray[i].name);
		jsonWriter->WriteValue("ip", serverInfoArray[i].ip);
		jsonWriter->WriteObjectEnd();
	}
	jsonWriter->WriteArrayEnd();
	jsonWriter->Close();                       //@注意：一定要关闭！！！ 否则输出字段为空

	UE_LOG(LogTemp, Log, TEXT("UE4 JSON TEST 3 : %s"), *jsonOutStr);
}