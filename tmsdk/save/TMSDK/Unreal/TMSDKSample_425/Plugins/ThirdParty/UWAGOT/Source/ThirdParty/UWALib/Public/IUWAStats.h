#pragma once
#include "uwa.h"
#include <vector>
#include <map>
#include <set>
#include <unordered_set>
#include <unordered_map>
#include <stack>
//#include <stdio.h>

using std::vector;
using std::string;
using std::map;
using std::stack;
using std::set;
using std::unordered_set;
using std::unordered_map;

struct EUWAThreadType
{
	enum Type
	{
		Invalid,
		Game,
		Renderer,
		Other,
	};
};

struct EUWAStatOperation
{
	enum Type
	{
		/** Indicates begin of the cycle scope. */
		CycleScopeStart = 4,
		/** Indicates end of the cycle scope. */
		CycleScopeEnd = 5,

		// these are special ones for processed data
		ChildrenStart = 11,
		ChildrenEnd = 12,
		Leaf = 13,
		MaxVal = 14,
	};
};
/**
 * Message flags
 */
struct EUWAStatMetaFlags
{
	enum Type
	{
		/** if true, then this message contains and int64 cycle or IsPackedCCAndDuration. */
		IsCycle = 0x04,
		/** if true, then this message contains a memory stat. */
		IsMemory = 0x08,
		/** if true, then this is actually two uint32s, the cycle count and the call count, see FromPackedCallCountDuration_Duration. */
		IsPackedCCAndDuration = 0x10,
	};
};

struct EUWAStatDataType
{
	enum Type
	{
		/** int64. */
		ST_int64 = 2,
		/** double. */
		ST_double = 3,
		/** FName. */
		ST_FName = 4,
		/** Memory pointer, stored as uint64. */
		ST_Ptr = 5,
	};
};

/*
 * Never keep STL containers as exported class members.
 * Client application may be compiled with different STL version than yours,
 * with undefined runtime behavior.
 * It is easy to replace vector<int> member with pointer vector<int>*.
 * Initialize it in the class constructor, and release in the class destructor.
 */

struct UWAStatMessage;
struct UWAStatNode;
class VMsgMapTool;

typedef map<EUWAThreadType::Type, vector<UWAStatMessage>*> ThreadStatMsgMap;
typedef map<int32, UWAStatNode*> NamedStatNodeMap;
struct DLL_API ThreadStatMsg
{
	friend class VMsgMapTool;
	static ThreadStatMsg* Create();
	static void Delete(ThreadStatMsg**);

	vector<UWAStatMessage>* Get(EUWAThreadType::Type, bool);
	void Clear(bool);
	void Exchange(ThreadStatMsg*);

private:

	ThreadStatMsg();
	~ThreadStatMsg();
	ThreadStatMsgMap* Map;
};


struct UWAStatNode
{
	UWAStatNode(string);
	UWAStatNode(int32 statId);
	UWAStatNode(UWAStatMessage*, int);
	void Append(UWAStatMessage*, int);
	void Append(UWAStatMessage*, UWAStatMessage*, int);
	void Clear();

	~UWAStatNode();
	int32 StatId;
	int32 Value;
	int32 Call;
	int32 Depth;
	bool Dirty;

	NamedStatNodeMap* pChildren;
};

struct DLL_API UWAStatMessage
{
public:
	UWAStatMessage(const char*, uint64, int, int, int);
	UWAStatMessage(const UWAStatMessage &);
	~UWAStatMessage();

	int64 GetValue(int);
	int64 GetCall();

	EUWAStatOperation::Type OperType;
	EUWAStatMetaFlags::Type MetaFlags;
	EUWAStatDataType::Type DataType;


	static void SetFrequency(double);
	char* Name;

private:
	
	int64 ToMicrosecond(int64);
	int64 Value;
};

class DLL_API VMsgMap
{
public:

	void Exchange(void*);
	void Output(int32);
	bool Dirty;

	static VMsgMap* Create(char*, int);
	static void Delete(VMsgMap**);

private:
	VMsgMap(char*, int32);
	~VMsgMap();
	VMsgMapTool* Tool;
};