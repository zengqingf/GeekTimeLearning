#include "TMSDKEventManager.h"
using namespace TenmoveSDK;

//TMSDKEventManager* TMSDKEventManager::instance = nullptr;

TMSDKEventManager::TMSDKEventManager()
{
	m_eventProcessor = new SDKEventProcessor();
}

TMSDKEventManager::~TMSDKEventManager()
{
	ClearEvent();
	if (m_eventProcessor != nullptr)
	{
		delete m_eventProcessor;
		m_eventProcessor = nullptr;
	}
	UE_LOG(LogTemp, Log, TEXT("### tm sdk event mgr dtor"));
}

//TMSDKEventManager& TMSDKEventManager::GetInstance()
//{
//	if (nullptr == instance)
//	{
//		instance = new TMSDKEventManager();
//	}
//	return *instance;
//}

SDKEventHandle* TMSDKEventManager::RegisterEvent(SDKEventType type, SDKEventHandle::SDKDel sdkDel)
{
	SDKEventHandle* handle = nullptr;
	if(m_eventProcessor)
		handle = m_eventProcessor->AddEventHandle((int)type, sdkDel);
	return handle;
}

void TMSDKEventManager::RemoveEvent(SDKEventHandle* handler)
{
	if(m_eventProcessor)
		m_eventProcessor->RemoveHandler(handler);
}

void TMSDKEventManager::TriggerEvent(SDKEventType type)
{
	SDKEventParam param;
	if(m_eventProcessor)
		m_eventProcessor->HandleEvent((int)type, param);
}

void TMSDKEventManager::TriggerEvent(SDKEventType type, const SDKEventParam& param)
{
	if(m_eventProcessor)
		m_eventProcessor->HandleEvent((int)type, param);
}

void TMSDKEventManager::ClearEvent()
{
	if(m_eventProcessor)
		m_eventProcessor->ClearAll();
}
