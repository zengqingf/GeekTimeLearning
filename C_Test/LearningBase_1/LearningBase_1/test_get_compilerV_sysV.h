#pragma warning(disable: 4996)

#include <windows.h>
#include <stdio.h>
#include <stdbool.h>

#ifndef TEST_GET_COMPILERV_SYSV
#define TEST_GET_COMPILERV_SYSV

int GetSystemBits(void); //判断系统位数32 or 64
int GetOsInfo(void); //系统信息
bool GetCompilerVer(void);//编译器信息
void PrintCVersion(void);


void PrintCVersion(void)
{
	//With -std=c11 in gcc, 201112L is used for __STDC_VERSION__
#if __STDC_VERSION__ >=  201710L
	printf("We are using C18!\n");
#elif __STDC_VERSION__ >= 201112L
	printf("We are using C11!\n");
#elif __STDC_VERSION__ >= 199901L
	printf("We are using C99!\n");
#else
	printf("We are using C89/C90!\n");
#endif
}

bool GetCompilerVer(void)
{
	//MSVC++ 14.0 _MSC_VER == 1900 (Visual Studio 2015)
	//MSVC++ 12.0 _MSC_VER == 1800 (Visual Studio 2013)
	//MSVC++ 11.0 _MSC_VER == 1700 (Visual Studio 2012)
	//MSVC++ 10.0 _MSC_VER == 1600 (Visual Studio 2010)
	//MSVC++ 9.0 _MSC_VER == 1500 (Visual Studio 2008)
	//MSVC++ 8.0 _MSC_VER == 1400 (Visual Studio 2005)
	//MSVC++ 7.1 _MSC_VER == 1310 (Visual Studio 2003)
	//MSVC++ 7.0 _MSC_VER == 1300
	//MSVC++ 6.0 _MSC_VER == 1200
	//MSVC++ 5.0 _MSC_VER == 1100
#ifdef __GNUC__
	printf("\nCompiled by gcc-%d.%d.%d\n", __GNUC__, __GNUC_MINOR__, __GNUC_PATCHLEVEL__);
#elif _MSC_VER
// printf("\nCompiled by %d\n", _MSC_VER);
	if (_MSC_VER == 1900)
		printf("\nCompiled by VC2015\n");
	else if (_MSC_VER == 1800)
		printf("\nCompiled by VC2013\n");
	else if (_MSC_VER == 1700)
		printf("\nCompiled by VC2012\n");
	else if (_MSC_VER == 1600)
		printf("\nCompiled by VC2010\n");
	else if (_MSC_VER == 1500)
		printf("\nCompiled by VC2008\n");
	else if (_MSC_VER == 1400)
		printf("\nCompiled by VC2005\n");
	else if (_MSC_VER == 1310)
		printf("\nCompiled by VC2003\n");
	else if (_MSC_VER == 1200)
		printf("\nCompiled by VC6.0");
	else
		printf("\nCompiled by Other VC compiler\n");

#endif // __GNUC__

#ifdef __GLIBC__
	printf("Glibc version :%d \n", __GLIBC__);//C Libraries
#elif __GLIBCXX__
	printf("Glibc version :%d \n", __GLIBCXX__);//C++ Libraries
#endif // __GLIBC__

	return true;
}

int GetSystemBits()
{
	SYSTEM_INFO si;
	GetSystemInfo(&si);
	if (si.wProcessorArchitecture == PROCESSOR_ARCHITECTURE_AMD64 ||
		si.wProcessorArchitecture == PROCESSOR_ARCHITECTURE_IA64)
	{
		return 64;
	}
	return 32;
}

int GetOsInfo(void)
{
	SYSTEM_INFO sysInfo;
	OSVERSIONINFOEX osVersion;

	GetSystemInfo(&sysInfo);

	// printf("OemId : %u\n", sysInfo.dwOemId);
	// printf("处理器架构 : %u\n", sysInfo.wProcessorArchitecture);
	// printf("页面大小 : %u\n", sysInfo.dwPageSize);
	// printf("应用程序最小地址 : %u\n", sysInfo.lpMinimumApplicationAddress);
	// printf("应用程序最大地址 : %u\n", sysInfo.lpMaximumApplicationAddress);
	// printf("处理器掩码 : %u\n", sysInfo.dwActiveProcessorMask);
	// printf("处理器数量 : %u\n", sysInfo.dwNumberOfProcessors);
	// printf("处理器类型 : %u\n", sysInfo.dwProcessorType);
	// printf("虚拟内存分配粒度 : %u\n", sysInfo.dwAllocationGranularity);
	// printf("处理器级别 : %u\n", sysInfo.wProcessorLevel);
	// printf("处理器版本 : %u\n", sysInfo.wProcessorRevision);
	// typedef struct _OSVERSIONINFOEX
	// {
	// DWORD dwOSVersionInfoSize;
	// DWORD dwMajorVersion;
	// DWORD dwMinorVersion;
	// DWORD dwBuildNumber;
	// DWORD dwPlatformId;
	// TCHAR szCSDVersion[128];
	// WORD wServicePackMajor;
	// WORD wServicePackMinor;
	// WORD wSuiteMask;
	// BYTE wProductType;
	// BYTE wReserved;
	// } OSVERSIONINFOEX, *POSVERSIONINFOEX, *LPOSVERSIONINFOEX;
	const char* osName = NULL;
	const char* vmark = NULL;

	osVersion.dwOSVersionInfoSize = sizeof(osVersion);

	if (GetVersionEx((OSVERSIONINFO*)&osVersion))
	{
		//osName
		switch (osVersion.dwMajorVersion) //判断主版本号
		{
		case 4:
			switch (osVersion.dwMinorVersion) //判断次版本号
			{
			case 0:
				if (osVersion.dwPlatformId == VER_PLATFORM_WIN32_NT)
					osName = "Microsoft Windows NT 4.0"; //1996年7月发布
				else if (osVersion.dwPlatformId == VER_PLATFORM_WIN32_WINDOWS)
					osName = "Microsoft Windows 95";
				break;
			case 10:
				osName = "Microsoft Windows 98";
				break;
			case 90:
				osName = "Microsoft Windows Me";
				break;
			}
			break;

		case 5:
			switch (osVersion.dwMinorVersion) //再比较dwMinorVersion的值
			{
			case 0:
				osName = "Microsoft Windows 2000"; //1999年12月发布
				break;

			case 1:
				osName = "Microsoft Windows XP"; //2001年8月发布
				break;

			case 2:
				if (osVersion.wProductType == VER_NT_WORKSTATION
					&& sysInfo.wProcessorArchitecture == PROCESSOR_ARCHITECTURE_AMD64)
				{
					osName = "Microsoft Windows XP Professional x64 Edition";
				}
				else if (GetSystemMetrics(SM_SERVERR2) == 0)
					osName = "Microsoft Windows Server 2003"; //2003年3月发布
				else if (GetSystemMetrics(SM_SERVERR2) != 0)
					osName = "Microsoft Windows Server 2003 R2";
				break;
			}
			break;

		case 6:
			switch (osVersion.dwMinorVersion)
			{
			case 0:
				if (osVersion.wProductType == VER_NT_WORKSTATION)
					osName = "Microsoft Windows Vista";
				else
					osName = "Microsoft Windows Server 2008"; //服务器版本
				break;
			case 1:
				if (osVersion.wProductType == VER_NT_WORKSTATION)
					osName = "Microsoft Windows 7";
				else
					osName = "Microsoft Windows Server 2008 R2";
				break;
			case 2:
				if (osVersion.wProductType == VER_NT_WORKSTATION)
					osName = "Windows 8";
				else if (osVersion.wProductType != VER_NT_WORKSTATION)
					osName = "Windows Server 2012";
				break;
			case 3:
				if (osVersion.wProductType == VER_NT_WORKSTATION)
					osName = "Windows 8.1";
				else if (osVersion.wProductType != VER_NT_WORKSTATION)
					osName = "Windows Server 2012 R2";
				break;
			}
			break;
		case 10:
			switch (osVersion.dwMinorVersion)
			{
			case 0:
				if (osVersion.wProductType == VER_NT_WORKSTATION)
					osName = "Windows 10";
				else if (osVersion.wProductType != VER_NT_WORKSTATION)
					osName = "Windows Server 2016";
				break;

			}
			break;
		}

		//osMark
		switch (osVersion.dwMajorVersion)
		{
			//先判断操作系统版本
		case 5:
			switch (osVersion.dwMinorVersion)
			{
			case 0: //Windows 2000
				if (osVersion.wSuiteMask == VER_SUITE_ENTERPRISE)
					vmark = "Advanced Server";
				break;
			case 1: //Windows XP
				if (osVersion.wSuiteMask == VER_SUITE_EMBEDDEDNT)
					vmark = "Embedded";
				else if (osVersion.wSuiteMask == VER_SUITE_PERSONAL)
					vmark = "Home Edition";
				else
					vmark = "Professional";
				break;
			case 2:
				if (GetSystemMetrics(SM_SERVERR2) == 0
					&& osVersion.wSuiteMask == VER_SUITE_BLADE) //Windows Server 2003
					vmark = "Web Edition";
				else if (GetSystemMetrics(SM_SERVERR2) == 0
					&& osVersion.wSuiteMask == VER_SUITE_COMPUTE_SERVER)
					vmark = "Compute Cluster Edition";
				else if (GetSystemMetrics(SM_SERVERR2) == 0
					&& osVersion.wSuiteMask == VER_SUITE_STORAGE_SERVER)
					vmark = "Storage Server";
				else if (GetSystemMetrics(SM_SERVERR2) == 0
					&& osVersion.wSuiteMask == VER_SUITE_DATACENTER)
					vmark = "Datacenter Edition";
				else if (GetSystemMetrics(SM_SERVERR2) == 0
					&& osVersion.wSuiteMask == VER_SUITE_ENTERPRISE)
					vmark = "Enterprise Edition";
				else if (GetSystemMetrics(SM_SERVERR2) != 0
					&& osVersion.wSuiteMask == VER_SUITE_STORAGE_SERVER)
					vmark = "Storage Server";
				break;
			}
			break;

		case 6:
			switch (osVersion.dwMinorVersion)
			{
			case 0:
				if (osVersion.wProductType != VER_NT_WORKSTATION
					&& osVersion.wSuiteMask == VER_SUITE_DATACENTER)
					vmark = "Datacenter Server";
				else if (osVersion.wProductType != VER_NT_WORKSTATION
					&& osVersion.wSuiteMask == VER_SUITE_ENTERPRISE)
					vmark = "Enterprise";
				else if (osVersion.wProductType == VER_NT_WORKSTATION
					&& osVersion.wSuiteMask == VER_SUITE_PERSONAL) //Windows Vista
					vmark = "Home";
				break;
			case 1:
				if (osVersion.wProductType == VER_NT_WORKSTATION
					&& osVersion.wSuiteMask == VER_SUITE_SINGLEUSERTS)
					vmark = "Ultimate";

				break;
			}
			break;
		}

	}//if(GetVersionEx((OSVERSIONINFO *)&os))

	printf("%s ", osName);
	printf("%s", vmark);

	printf(" SP%u.%u", osVersion.wServicePackMajor, osVersion.wServicePackMinor);
	printf(" %d bit", GetSystemBits());

	printf(" Version %u.%u", osVersion.dwMajorVersion, osVersion.dwMinorVersion);
	printf(" Build %u\n", osVersion.dwBuildNumber);
	//printf("PlatformId : %u\n", osVersion.dwPlatformId);
	//printf("%x\n",osVersion.wSuiteMask);
	return 0;
}

#endif //TEST_GET_COMPILERV_SYSV