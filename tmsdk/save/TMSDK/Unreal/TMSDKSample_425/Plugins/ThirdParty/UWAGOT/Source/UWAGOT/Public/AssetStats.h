// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"

#include "ThirdParty/UWALib/Public/IUWAAssetStats.h"

class UWAGOT_API FAssetStats
{
public:
	static FAssetStats& Get()
	{
		static FAssetStats Instance;
		return Instance;
	}

	FAssetStats();
	~FAssetStats();

	void Initialize();
	//MAIN LOOP IN ASSET,take asset sample at specified frame
	void TakeSample(int32 FrameIndex);

private:
	bool IsTakeSample(int32 FrameIndex) const;
	void DumpMethodInfoMap();

	void SampleTexture2D(FileWriter& Ar);
	void SampleRenderTexture(FileWriter& Ar);
	void SampleMesh(FileWriter& Ar);
	void SampleShader(FileWriter& Ar);
	void SampleAnimation(FileWriter& Ar);
	void SampleSound(FileWriter& Ar);
	void SampleParticleSystem(FileWriter& Ar);
	void SampleMeshCollider(FileWriter& Ar);
	void SampleMaterial(FileWriter& Ar);
	void SampleFont(FileWriter& Ar);
};
