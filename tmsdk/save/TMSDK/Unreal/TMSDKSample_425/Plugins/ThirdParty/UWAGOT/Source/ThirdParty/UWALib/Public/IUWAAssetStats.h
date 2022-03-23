#pragma once

#include "uwa.h"

#include <unordered_map>
#include <vector>
#include "FileWriter.h"

struct DLL_API BasicSample
{
	BasicSample()
	{
		Name = nullptr;
		Format = nullptr;
	}
	~BasicSample()
	{
		if (Name != nullptr) delete[] Name;
		if (Format != nullptr) delete [] Format;
	}

	char* Name;
	int32 Size;
	int32 HashCode;
	char* Format;
	void SetName(const char* str);
	const char* GetName()const;
	void SetFormat(const char* str);
};

struct DLL_API Texture2D : BasicSample
{
	int32 NumMips;
	int32 DroppedMips;
	int32 CurSizeX;
	int32 CurSizeY;
	static Texture2D* Get();
	static void Destory(Texture2D* target);
};
struct DLL_API RenderTexture : BasicSample
{
	int32 Width;
	int32 Height;
	static RenderTexture* Get();
	static void Destory(RenderTexture* target);
};
struct DLL_API Mesh : BasicSample
{
	int32 NumTris = 0, NumVerts = 0, NumNormal = 0, NumTangent = 0, NumColors = 0, NumBoneWeight = 0;
	static Mesh* Get();
	static void Destory(Mesh* target);
};
struct DLL_API Shader : BasicSample
{
	static Shader* Get();
	static void Destory(Shader* target);
};
struct DLL_API Animation : BasicSample
{
	float Length;
	int32 Frames;
	int32 FrameRate;
	static Animation* Get();
	static void Destory(Animation* target);
};
struct DLL_API Sound : BasicSample
{
	float Length;
	int32 Sample;
	static Sound* Get();
	static void Destory(Sound* target);
};
struct DLL_API Particle : BasicSample
{
	int32 ParticleCount = 0;
	bool bIsActive;
	static Particle* Get();
	static void Destory(Particle* target);
};
struct DLL_API Material : BasicSample
{
	static Material* Get();
	static void Destory(Material* target);
};
struct DLL_API Font : BasicSample
{
	static Font* Get();
	static void Destory(Font* target);
};

class DLL_API UWAAssetStats
{
public:
	struct FAssetInfo
	{
		FAssetInfo();
		int32 hashCode;
		int32 dupCount;
		int32 totalSize;
	};
	static UWAAssetStats& Get();
	UWAAssetStats();
	~UWAAssetStats() = default;

	//save init info before other function
	bool Initialize(const char* filename);
	//MAIN LOOP IN ASSET,take asset sample at specified frame
	bool TakeSample(int32 FrameIndex, const char* str, FileWriter& fileWriter);
	int32 GetHashCode(const string& str);
	int32 GetAssetHashCode(const char* Name, const char* Type, int32 Size);

	void DumpAssetInfoMap(FileWriter& Ar);
	bool DumpMethodInfoMap(FileWriter& fileWriter);
	void AddMethodInfo(int32 Hash, string& Info);
	void AddAssetInfo(int32 Hash, int32 Size);

	void SaveDex(const char *);

	void SampleTexture2D(Texture2D* sample);
	void SampleRenderTexture(RenderTexture* sample);
	void SampleMesh(Mesh* sample);
	void SampleShader(Shader* sample);
	void SampleAnimation(Animation* sample);
	void SampleSound(Sound* sample);
	void SampleParticleSystem(Particle* sample);
	void SampleMeshCollider(RenderTexture* sample);
	void SampleMaterial(Material* sample);
	void SampleFont(Font* sample);
	bool MidContains(int32 HashCode);
	bool GetbAssetInfoChanged() const;
	bool& SetbAssetInfoChanged();
	int32 GetCurrentMid() const;
	int32 GetLastAssetInfoMapDumpCount() const;
	int32 GetFrameInterval()const;
private:
	std::unordered_map<int32, int32> hashCode2MidMap;
	int32 CurrentMid;
	std::vector<string> MethodInfo2WriteVec;
	std::unordered_map<int32, FAssetInfo> hashCode2AssetInfoMap;
	bool bAssetInfoChanged;
	int32 LastAssetInfoMapDumpCount;
	int32 FrameInterval;
};
