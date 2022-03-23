// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GCloudPublicDefine.h"
#include "ITDir.h"
using namespace GCloud;



//typedef struct _tagTDirInitInfo : public ABase::ApolloBufferBase
//{
//public:
//	AString OpenID; // �û���ΨһID������Ȩ֮����Եõ��������
//	AString Url; // ����Ŀ¼�����̨�ķ�������ַ���������ʱ����ĵ�ַ��
//	bool EnableManualUpdate; // �Ƿ�����Ϸ�Լ�������������ģ��Ļص���Ŀǰֻ��Ϊtrue
//}TDirInitInfo;

struct ServerInfo;
typedef TMap<FString, TArray<ServerInfo>> ServerDir;
DECLARE_DELEGATE_OneParam(FDelegateServerDirRes, ServerDir);

class UESDKTEST_API GCloud_Maple : public TDirObserver
{
public:
	GCloud_Maple();
	~GCloud_Maple();
public:
	void Init();
	void UnInit();
	void Update();
	/// <summary>
	/// ��ȡ����Ŀ¼��(����)
	/// </summary>
	void QueryTree(int treeId);
	/// <summary>
	/// ��ȡĳ��ָ��������Ϣ
	/// </summary>
	/// <param name="treeId"></param>
	/// <param name="node"></param>
	//void QueryLeaf(int treeId, TreeNodeBase* node = nullptr);
	void QueryLeaf(int treeId, int LeafId);

	/// <summary>
/// Callback of QueryAll
/// </summary>
	virtual void OnQueryAllProc(const Result& result, const TreeCollection* treeList) override;

	/// <summary>
	/// Callback of QueryTree
	/// </summary>
	virtual void OnQueryTreeProc(const Result& result, const TreeInfo* nodeList) override;

	/// <summary>
	/// Callback of QueryLeaf
	/// </summary>
	virtual void OnQueryLeafProc(const Result& result, const NodeWrapper* node) override;

	FDelegateServerDirRes DelegateServerDirRes;
private:

	TDirObserver* mObserver;
	uint64_t gameID;
	AString gameKey;
	TMap<int, FString> mRDir;
	TMap<FString,TArray<ServerInfo>> mServerDir;

	FString AStingToFString(AString aString);
};

 struct ServerInfo
{
	int ID;
	FString ServerName;
	unsigned int Flag;
	FString State;
	FString Url;
};

 UENUM()
 enum class ServerState
 {
	 TnFLagHeavy = 0x10,
	 TnFlagCrown = 0x20,
	 TnFlagFine = 0x40,
	 TnFlagUnavailable = 0x80,
 };

 UENUM()
 enum class ServerTag
 {
	 TnTagHot = 0x01,
	 TnTagRecommend = 0x02,
	 TnTagNew = 0x04,
	 TnTagLimited = 0x08,
	 TnTagExperience = 0x10,
 };
