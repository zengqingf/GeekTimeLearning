// Fill out your copyright notice in the Description page of Project Settings.


#include "Plugins/GCloud_Maple.h"
#include "IGCloud.h"
#include "GCloudCore/ALog.h"




GCloud_Maple::GCloud_Maple()
{
	GCloud::InitializeInfo info;
	info.GameId = 948137280;
	info.GameKey = "8ab7d865a7a686ae4c94ff9b3d3fccd4";
	IGCloud::GetInstance().Initialize(info);
}

GCloud_Maple::~GCloud_Maple()
{
}

void GCloud_Maple::Init()
{
	mObserver = this;
	ITDir::GetInstance().AddObserver(mObserver);
	ITDir::GetInstance().EnableManualUpdate(true);
	TDirInitInfo initInfo;
	initInfo.OpenID = "948137280";
	initInfo.Url = "pre-dir.6.948137280.cs.gcloud.qq.com";
	ITDir::GetInstance().Initialize(initInfo);
}

void GCloud_Maple::UnInit()
{
	if (mObserver)
	{
		ITDir::GetInstance().RemoveObserver(mObserver);
	}	
}

void GCloud_Maple::Update()
{
	ITDir::GetInstance().UpdateByManual();
}

void GCloud_Maple::QueryTree(int treeId)
{
	{
		if (ITDir::GetInstance().IsConnected())
		{
			ITDir::GetInstance().QueryTree(treeId);
		}
	}
}



void GCloud_Maple::QueryLeaf(int treeId,int LeafId)
{
	ITDir::GetInstance().QueryLeaf(treeId, LeafId);
	//if (node->IsLeaf()) {
	//	//ITDir::GetInstance().QueryLeaf(treeId, node->Id);
	//}
}

//callback

void GCloud_Maple::OnQueryAllProc(const Result& result, const TreeCollection* treeList)
{

}

void GCloud_Maple::OnQueryTreeProc(const Result& result, const TreeInfo* nodeList)
{
	if (result.IsSuccess() && nodeList) {
		int treeId = nodeList->GetTreeId();
		for (int j = 0; j < nodeList->NodeList.Count(); j++) 
		{
			NodeWrapper* nodeWrapper = (NodeWrapper*)nodeList->NodeList.ObjectAtIndex(j);
			TreeNodeBase* node = nodeWrapper->GetNode();
			if (node->ParentId == treeId)
			{
				if (!mRDir.Contains(node->Id))
				{
					mRDir.Add(node->Id, AStingToFString(node->Name));
				}
				UE_LOG(LogTemp, Warning, TEXT("QU Node: id:%d, pid:%d, name:%s, type:%d\n"), node->Id, node->ParentId, *AStingToFString(node->Name), node->GetType());
			}
			else if (node->IsLeaf())
			{
				UE_LOG(LogTemp, Warning, TEXT("FU Node: id:%d, pid:%d, name:%s, type:%d\n"), node->Id, node->ParentId, *AStingToFString(node->Name), node->GetType());
				LeafNode* leaf = (LeafNode*)node;

				ServerInfo sf = {node->Id,AStingToFString(node->Name), leaf->Flag ,AStingToFString(leaf->Url) };

				if (mRDir.Contains(node->ParentId))
				{
					if (!mServerDir.Contains(mRDir[node->ParentId]))
					{
						mServerDir.Add(mRDir[node->ParentId]);
					}
					mServerDir[mRDir[node->ParentId]].Add(sf);
				}

				UE_LOG(LogTemp, Warning, TEXT("flag:%d, url:%s\n\n"), leaf->Flag, *AStingToFString(leaf->Url));
			}
			else
			{
				CategoryNode* category = (CategoryNode*)node;
				//  Category Node infomation
			}
		}
	}
	else
	{
		XLogError("OnQueryTreeProc error:%d, %s\n", result.ErrorCode, result.Reason.c_str());
	}
	//for (auto rd :mServerDir)
	//{
	//	UE_LOG(LogTemp, Warning, TEXT("Contains  Property: Key %s"), *rd.Key);
	//	for (auto d : rd.Value)
	//	{
	//		UE_LOG(LogTemp, Warning, TEXT("Contains: ID %d, name: %s flag: %d  url:%s "), d.ID ,*d.ServerName, d.Flag, *d.Url);
	//	}
	//}

	//DelegateServerDirRes.IsBound()
	DelegateServerDirRes.ExecuteIfBound(mServerDir);
}

void GCloud_Maple::OnQueryLeafProc(const Result& result, const NodeWrapper* node)
{
	XLogInfo("OnQueryLeafProc result:%d, ext:%d, %s\n", result.ErrorCode, result.Extend, result.Reason.c_str());
	UE_LOG(LogTemp, Warning, TEXT("OnQueryLeafProc result:%d, ext:%d, %s\n"), result.ErrorCode, result.Extend, *AStingToFString(result.Reason));

	if (result.IsSuccess() && node) {

		XLogInfo("Node: id:%d, pid:%d, name:%s, type:%d, url:%s\n", node->Leaf.Id, node->Leaf.ParentId, node->Leaf.Name.c_str(), node->Leaf.GetType(), node->Leaf.Url.c_str());
		UE_LOG(LogTemp, Warning, TEXT("Node: id:%d, pid:%d, name:%s, type:%d, url:%s\n"), node->Leaf.Id, node->Leaf.ParentId, *AStingToFString(node->Leaf.Name), node->Leaf.GetType(), *AStingToFString(node->Leaf.Url));


		AString buffer;
		node->Encode(buffer);
		AString str;
		char buf[5];
		int i = 0;
		for (; i < buffer.size(); i++) {
			sprintf(buf, "%02x,", (unsigned char)buffer.at(i));
			str += buf;
		}

		printf("node len:%d, i:%d, hex:%s\n", buffer.size(), i, str.c_str());
		UE_LOG(LogTemp, Warning,TEXT("node len:%d, i:%d, hex:%s\n"), buffer.size(), i, *AStingToFString(str));

	}
	else
	{
		XLogError("OnQueryLeafProc error:%d, %s\n", result.ErrorCode, result.Reason.c_str());
	}
}

FString GCloud_Maple::AStingToFString(AString aString)
{
	return  UTF8_TO_TCHAR(aString.c_str());
}
