#include "TMSDKEventManager.h"
using namespace TenmoveSDK;

TMSDKEventManager::TMSDKEventManager()
{
	m_eventProcessor = new SDKEventProcessor();
}

TMSDKEventManager::~TMSDKEventManager()
{
	ClearEvent();
	delete m_eventProcessor;
	m_eventProcessor = nullptr;
}

TMSDKEventManager& TMSDKEventManager::GetInstance()
{
	static TMSDKEventManager instance;
	return instance;
}

SDKEventHandle* TMSDKEventManager::RegisterEvent(SDKEventType type, SDKEventHandle::SDKDel sdkDel)
{
	SDKEventHandle* handle = nullptr;
	if(m_eventProcessor)
		handle = m_eventProcessor->AddEventHandle((int)type, sdkDel);
	return handle;
}

void TMSDKEventManager::RemoveEvent(SDKEventHandle* handler)
{
	SDKEventHandle* handle = nullptr;
	if(m_eventProcessor)
		m_eventProcessor->RemoveHandler(handler);
	delete handler;
	handler = nullptr;
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
