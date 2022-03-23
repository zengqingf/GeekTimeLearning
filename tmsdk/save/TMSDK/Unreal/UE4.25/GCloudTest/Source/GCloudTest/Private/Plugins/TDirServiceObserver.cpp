// Fill out your copyright notice in the Description page of Project Settings.


#include "Plugins/TDirServiceObserver.h"
#include "Plugins/TMSDKDefine.h"
#include "JsonLibUtil.h"


TDirServiceObserver::~TDirServiceObserver()
{
	m_serverInfos.Empty();
	m_selectedLeafIds.Empty();
	m_queryCount = 0;
}

//大区下的分区信息
void TDirServiceObserver::OnQueryTreeProc(const Result& result, const TreeInfo* tree)
{
	UE_LOG(LogTemp, Log, TEXT("### OnQueryTreeProc result:%d, ext:%d, %s\n"), result.ErrorCode, result.Extend, *AStringToFString(result.Reason));
	if (result.IsSuccess() && tree) {
		int treeId = tree->GetTreeId();
		UE_LOG(LogTemp, Log, TEXT("### on query tree proc, tree id: %d..."), treeId);
		if (treeId != m_regionId)
		{
			return;
		}
		SeqId queryIndex;
		m_selectedLeafIds.Empty();
		for (int j = 0; j < tree->NodeList.Count(); j++) {
			NodeWrapper* nodeWrapper = (NodeWrapper*)tree->NodeList.ObjectAtIndex(j);
			TreeNodeBase* node = nodeWrapper->GetNode();

			//获取叶子节点（服节点）数据
			//如果叶子节点的父节点（分区节点）是指定节点
			if (node->ParentId == m_subareaId)
			{
				UE_LOG(LogTemp, Log, TEXT("### Base Node: id:%d, parent id:%d, name:%s, type:%d\n"), node->Id, node->ParentId, *AStringToFString(node->Name), node->GetType());

				//如果是叶子节点
				if (node->IsLeaf())
				{
					LeafNode* leaf = (LeafNode*)node;
					m_selectedLeafIds.Add(node->Id);

					UE_LOG(LogTemp, Log, TEXT("### Leaf Node: flag:%d, url:%s\n"), leaf->Flag, *AStringToFString(leaf->Url));
				}
				//else
				//{
				//	CategoryNode* category = (CategoryNode*)node;
				//	UE_LOG(LogTemp, Log, TEXT("### Category Node detail info: tag:%d\n"), node->Tag);
				//}
			}
		}
		m_queryCount = m_selectedLeafIds.Num();
		for (int id : m_selectedLeafIds)
		{
			queryIndex = ITDir::GetInstance().QueryLeaf(treeId, id);
			UE_LOG(LogTemp, Log, TEXT("### QueryLeaf seq index:%d, treed id:%d, leafnode id:%d\n"), queryIndex, treeId, id);
		}
	}
	else
	{
		UE_LOG(LogTemp, Error, TEXT("### OnQueryTreeProc error:%d, %s\n"), result.ErrorCode, *AStringToFString(result.Reason));
		PluginManager::Instance().EventManager().TriggerEvent(SDKEventType::DirPullError);
	}
}

//分区下的服信息
void TDirServiceObserver::OnQueryLeafProc(const Result& result, const NodeWrapper* node)
{
	UE_LOG(LogTemp, Log, TEXT("### OnQueryLeafProc result: %d, ext: %d, ext2: %d %s\n"),
		result.ErrorCode, result.Extend, result.Extend2, *AStringToFString(result.Reason));
	if (result.IsSuccess() && node) {
		UE_LOG(LogTemp, Log, TEXT("### Leaf Node detail info: id: %d, parent id: %d, name: %s, type: %d, url: %s\n"), 
			node->Leaf.Id, node->Leaf.ParentId, *AStringToFString(node->Leaf.Name), node->Leaf.GetType(), *AStringToFString(node->Leaf.Url));

		DirServerInfo serverInfo;
		serverInfo.name = AStringToFString(node->Leaf.Name);
		serverInfo.ip = AStringToFString(node->Leaf.Url);
		m_serverInfos.Add(serverInfo);
		--m_queryCount;
		UE_LOG(LogTemp, Log, TEXT("### query cout: %d"), m_queryCount);

		if (m_queryCount <= 0)
		{
			DirInfoEventParam param;
			param.fstr_0 = _getDirServerJsonInfo();
			PluginManager::Instance().EventManager().TriggerEvent(SDKEventType::DirPullFinish, param);
		}

		//test demo
		//AString buffer;
		//node->Encode(buffer);
		//AString str;
		//char buf[5];
		//int i = 0;
		//for (; i < buffer.size(); i++){
		//	sprintf(buf, "%02x,", (unsigned char)buffer.at(i));
		//	str += buf;
		//}
		//UE_LOG(LogTemp, Log, TEXT("### Leaf Node len: %d, i: %d, hex: %s\n"), buffer.size(), i, *AStringToFString(str));
	}
	else
	{
		UE_LOG(LogTemp, Error, TEXT("### OnQueryLeafProc error: %d, %s\n"), result.ErrorCode, *AStringToFString(result.Reason));
		PluginManager::Instance().EventManager().TriggerEvent(SDKEventType::DirPullError);
	}
}

void TDirServiceObserver::OnQueryAllProc(const Result& result, const TreeCollection* treeList)
{
	//throw std::logic_error("The method or operation is not implemented.");
}

void TDirServiceObserver::OnQueryFriendProc(const Result& result, const QueryFriendsResult& queryFriendsResult)
{
	//throw std::logic_error("The method or operation is not implemented.");
}

const FString& TDirServiceObserver::_getDirServerJsonInfo()
{
	TSharedRef<FCondensedJsonStringWriter> jsonWriter = FCondensedJsonStringWriterFactory::Create(&m_serverJsonStr);
	jsonWriter->WriteArrayStart();
	for (int32 i = 0; i< m_serverInfos.Num(); ++i)
	{
		jsonWriter->WriteObjectStart();
		jsonWriter->WriteValue(TEXT("name"), m_serverInfos[i].name);
		jsonWriter->WriteValue(TEXT("ip"), m_serverInfos[i].ip);
		jsonWriter->WriteObjectEnd();
	}
	jsonWriter->WriteArrayEnd();
	jsonWriter->Close();

	return m_serverJsonStr;
}

