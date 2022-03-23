// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GCloudPublicDefine.h"
#include "ITDir.h"
using namespace GCloud;



//typedef struct _tagTDirInitInfo : public ABase::ApolloBufferBase
//{
//public:
//	AString OpenID; // 用户的唯一ID，在授权之后可以得到相关数据
//	AString Url; // 连接目录服务后台的服务器地址（申请服务时分配的地址）
//	bool EnableManualUpdate; // 是否由游戏自己驱动区服导航模块的回调，目前只能为true
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
	/// 拉取单个目录树(大区)
	/// </summary>
	void QueryTree(int treeId);
	/// <summary>
	/// 拉取某个指定服的信息
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
