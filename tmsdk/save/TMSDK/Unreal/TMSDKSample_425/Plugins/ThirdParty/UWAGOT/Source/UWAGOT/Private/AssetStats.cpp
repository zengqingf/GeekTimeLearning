// Fill out your copyright notice in the Description page of Project Settings.

#include "AssetStats.h"
#include "Runtime/Core/Public/Misc/Paths.h"
#include "UObject/UObjectIterator.h"
#include "Engine/Texture.h"
#include "Engine/Texture2D.h"
#include "RenderUtils.h"
#include "Runtime/Engine/Classes/Engine/StaticMesh.h"
#include "Runtime/Engine/Classes/Engine/SkeletalMesh.h"
#include "Runtime/Engine/Classes/Engine/TextureRenderTarget2D.h"
#include "Runtime/Engine/Classes/Materials/Material.h"
#include "Runtime/Engine/Classes/Animation/AnimSequence.h"
#include "Runtime/Engine/Classes/Sound/SoundWave.h"
#include "Runtime/Engine/Classes/Particles/ParticleEmitter.h"
#include "Runtime/Engine/Classes/Particles/ParticleSystem.h"
#include "Runtime/Engine/Classes/Particles/ParticleSystemComponent.h"
#include "Runtime/Engine/Classes/Particles/ParticleLODLevel.h"
#include "Runtime/Engine/Classes/Engine/Font.h"
#include "Runtime/Engine/Public/StaticMeshResources.h"
#include "Runtime/Launch/Resources/Version.h"
#include "PlatformManager.h"
#include "UWAMacros.h"

#if ENGINE_MAJOR_VERSION == 4 && ENGINE_MINOR_VERSION >= 21
#include "Runtime/Engine/Public/Rendering/SkeletalMeshRenderData.h"
#elif ENGINE_MAJOR_VERSION == 4 && ENGINE_MINOR_VERSION == 20
#include "Runtime/Engine/Public/Rendering/SkeletalMeshRenderData.h"
#elif ENGINE_MAJOR_VERSION == 4 && ENGINE_MINOR_VERSION == 19
#include "Runtime/Engine/Public/Rendering/SkeletalMeshRenderData.h"
#endif

FAssetStats::FAssetStats() = default;

FAssetStats::~FAssetStats() = default;

void FAssetStats::Initialize()
{
	FString FileName = FPlatformManager::Get().GetCurrentDataDirectory() + TEXT("asset_map.txt");
	if (!UWAAssetStats::Get().Initialize(TCHAR_TO_UTF8(*FileName)))
	{
		UE_LOG(LogTemp, Log, TEXT("Can not create file %s"), *FileName);
		return;
	}
}

void FAssetStats::TakeSample(int32 FrameIndex)
{
	if (!IsTakeSample(FrameIndex))
		return;
	FString FileName = FPlatformManager::Get().GetCurrentDataDirectory() + FString::Printf(TEXT("%d.at"), FrameIndex);

	FileWriter fileWriter(TCHAR_TO_UTF8(*FileName));

	if (!fileWriter)
	{
		UE_LOG(LogTemp, Warning, TEXT("Can not create file %s"), *FileName);
		return;
	}
	UWAAssetStats::Get().TakeSample(FrameIndex, TCHAR_TO_UTF8(*FileName), fileWriter);
	SampleTexture2D(fileWriter);
	SampleRenderTexture(fileWriter);
	SampleMesh(fileWriter);
	SampleShader(fileWriter);
	SampleAnimation(fileWriter);
	SampleSound(fileWriter);
	SampleParticleSystem(fileWriter);
	SampleMaterial(fileWriter);
	SampleFont(fileWriter);

	static int SampleIndex = 0;
	if (SampleIndex % SAVE_IDMAP_INTERVAL == 0 && UWAAssetStats::Get().GetbAssetInfoChanged())
	{
		DumpMethodInfoMap();
		UWAAssetStats::Get().SetbAssetInfoChanged() = false;
	}
	SampleIndex++;
}

bool FAssetStats::IsTakeSample(int32 FrameIndex) const
{
	return FrameIndex % UWAAssetStats::Get().GetFrameInterval() == 0;
}

void FAssetStats::DumpMethodInfoMap()
{
	FString FileName = FPlatformManager::Get().GetCurrentDataDirectory() + TEXT("asset_map.txt");
	FileWriter fileWriter(TCHAR_TO_UTF8(*FileName),8);
	if (!fileWriter)
	{
		UE_LOG(LogTemp, Warning, TEXT("Can not create file %s"), *FileName);
	}
	else
		UWAAssetStats::Get().DumpMethodInfoMap(fileWriter);
}
void FAssetStats::SampleTexture2D(FileWriter & Ar)
{
	if (!Ar) return;
	Ar.Println("1,10,0,0,0");

	for (TObjectIterator<UTexture2D> It; It; ++It)
	{
		UTexture2D* Texture = *It;
		Texture2D* sample = Texture2D::Get();
		sample->SetName(TCHAR_TO_UTF8(*Texture->GetPathName()));
		sample->NumMips = Texture->GetNumMips();
		sample->SetFormat(TCHAR_TO_UTF8(GetPixelFormatString(Texture->GetPixelFormat())));
		sample->DroppedMips = Texture->GetNumMips() - Texture->GetNumResidentMips();
		sample->CurSizeX = FMath::Max<int32>(Texture->GetSizeX() >> sample->DroppedMips, 1);
		sample->CurSizeY = FMath::Max<int32>(Texture->GetSizeY() >> sample->DroppedMips, 1);
		sample->Size = Texture->CalcTextureMemorySizeEnum(TMC_ResidentMips) + 512;
		UWAAssetStats::Get().SampleTexture2D(sample);
		Texture2D::Destory(sample);
	}
	UWAAssetStats::Get().DumpAssetInfoMap(Ar);
	Ar.Flush();
}

void FAssetStats::SampleRenderTexture(FileWriter & Ar)
{
	if (!Ar) return;
	Ar.Println("1,8,0,0,0");

	for (TObjectIterator<UTextureRenderTarget2D> It; It; ++It)
	{
		UTextureRenderTarget2D* RenderTexture = *It;
		struct RenderTexture* sample = RenderTexture::Get();
		sample->SetName(TCHAR_TO_UTF8(*RenderTexture->GetPathName()));
		sample->Size = RenderTexture->GetResourceSizeBytes(EResourceSizeMode::Type::Exclusive);
		sample->Width = RenderTexture->SizeX;
		sample->Height = RenderTexture->SizeY;
		sample->SetFormat(TCHAR_TO_UTF8(GPixelFormats[RenderTexture->GetFormat()].Name));
		UWAAssetStats::Get().SampleRenderTexture(sample);
		RenderTexture::Destory(sample);
	}
	UWAAssetStats::Get().DumpAssetInfoMap(Ar);
	Ar.Flush();
}

void FAssetStats::SampleMesh(FileWriter & Ar)
{
	if (!Ar) return;
	Ar.Println("1,5,0,0,0");
	for (TObjectIterator<UStaticMesh> It; It; ++It)
	{
		UStaticMesh* Mesh = *It;
		struct Mesh* sample = Mesh::Get();
		sample->SetName(TCHAR_TO_UTF8(*Mesh->GetPathName()));
		sample->Size = Mesh->GetResourceSizeBytes(EResourceSizeMode::Type::Exclusive);
		sample->HashCode = UWAAssetStats::Get().GetAssetHashCode(sample->GetName(), "Mesh", sample->Size);
		// this asset has a method id ? 
		if (!UWAAssetStats::Get().MidContains(sample->HashCode))
		{
			if (Mesh->RenderData)
			{
				sample->NumTris = Mesh->RenderData->LODResources[0].GetNumTriangles();
				sample->NumVerts = Mesh->RenderData->LODResources[0].GetNumVertices();
				sample->NumNormal = Mesh->RenderData->LODResources[0].GetNumVertices();
				sample->NumTangent = Mesh->RenderData->LODResources[0].GetNumVertices();
				sample->NumColors = Mesh->RenderData->LODResources[0].GetNumVertices();
			}
			UWAAssetStats::Get().SampleMesh(sample);
		}
		// this asset has been appeared in this frame ?
		UWAAssetStats::Get().AddAssetInfo(sample->HashCode, sample->Size);
		Mesh::Destory(sample);
	}

	for (TObjectIterator<USkeletalMesh> It; It; ++It)
	{
		USkeletalMesh* Mesh = *It;
		struct Mesh* sample = Mesh::Get();
		sample->SetName(TCHAR_TO_UTF8(*Mesh->GetPathName()));
		sample->Size = Mesh->GetResourceSizeBytes(EResourceSizeMode::Type::Exclusive);

		sample->HashCode = UWAAssetStats::Get().GetAssetHashCode(sample->GetName(), "Mesh", sample->Size);

		// this asset has a method id ?
		if (!UWAAssetStats::Get().MidContains(sample->HashCode))
		{
#if ENGINE_MAJOR_VERSION == 4 && ENGINE_MINOR_VERSION >= 21
			FSkeletalMeshRenderData* RenderData = Mesh->GetResourceForRendering();
			if (RenderData)
			{
				FSkeletalMeshLODRenderData& SourceModel = RenderData->LODRenderData[0];
				sample->NumTris = SourceModel.GetTotalFaces();
				sample->NumVerts = SourceModel.GetNumVertices();
				sample->NumNormal = SourceModel.GetNumVertices();
				sample->NumTangent = SourceModel.GetNumVertices();
				sample->NumColors = SourceModel.StaticVertexBuffers.ColorVertexBuffer.GetNumVertices();
				sample->NumBoneWeight = SourceModel.RequiredBones.Num();
			}
#elif ENGINE_MAJOR_VERSION == 4 && ENGINE_MINOR_VERSION == 20
			FSkeletalMeshRenderData* RenderData = Mesh->GetResourceForRendering();
			if (RenderData)
			{
				FSkeletalMeshLODRenderData& SourceModel = RenderData->LODRenderData[0];
				sample->NumTris = SourceModel.GetTotalFaces();
				sample->NumVerts = SourceModel.GetNumVertices();
				sample->NumNormal = SourceModel.GetNumVertices();
				sample->NumTangent = SourceModel.GetNumVertices();
				sample->NumColors = SourceModel.StaticVertexBuffers.ColorVertexBuffer.GetNumVertices();
				sample->NumBoneWeight = SourceModel.RequiredBones.Num();
			}
#elif ENGINE_MAJOR_VERSION == 4 && ENGINE_MINOR_VERSION == 19
			FSkeletalMeshRenderData* RenderData = Mesh->GetResourceForRendering();
			if (RenderData)
			{
				FSkeletalMeshLODRenderData& SourceModel = RenderData->LODRenderData[0];
				sample->NumTris = SourceModel.GetTotalFaces();
				sample->NumVerts = SourceModel.GetNumVertices();
				sample->NumNormal = SourceModel.GetNumVertices();
				sample->NumTangent = SourceModel.GetNumVertices();
				sample->NumColors = SourceModel.StaticVertexBuffers.ColorVertexBuffer.GetNumVertices();
				sample->NumBoneWeight = SourceModel.RequiredBones.Num();
			}
#elif ENGINE_MAJOR_VERSION == 4 && ENGINE_MINOR_VERSION == 18
			FSkeletalMeshResource* RenderData = Mesh->GetResourceForRendering();
			if (RenderData)
			{
				FStaticLODModel& SourceModel = RenderData->LODModels[0];
				sample->NumTris = SourceModel.GetTotalFaces();
				sample->NumVerts = SourceModel.NumVertices;
				sample->NumNormal = SourceModel.NumVertices;
				sample->NumTangent = SourceModel.NumVertices;
				sample->NumColors = SourceModel.ColorVertexBuffer.GetNumVertices();
				sample->NumBoneWeight = SourceModel.RequiredBones.Num();
			}
#endif
			UWAAssetStats::Get().SampleMesh(sample);
		}
		// this asset has been appeared in this frame ?
		UWAAssetStats::Get().AddAssetInfo(sample->HashCode, sample->Size);
		Mesh::Destory(sample);
	}
	UWAAssetStats::Get().DumpAssetInfoMap(Ar);
	Ar.Flush();
}
void FAssetStats::SampleShader(FileWriter & Ar)
{
	if (!Ar) return;
	Ar.Println("1,9,0,0,0");

	TArray<UObject*> Objects;
	GetObjectsOfClass(UMaterial::StaticClass(), Objects, true, RF_ClassDefaultObject, EInternalObjectFlags::None);

	for (auto Object : Objects)
	{
		UMaterial* Material = StaticCast<UMaterial*>(Object);
		FMaterialResource* MatRes = nullptr;
		FMaterialShaderMap* ShaderMap = nullptr;
		int32 enumLevel = 0;
#if ENGINE_MAJOR_VERSION == 4 && ENGINE_MINOR_VERSION >= 25
		while (enumLevel != ERHIFeatureLevel::Num && (ShaderMap == nullptr || ShaderMap->GetShaderNum() == 0))
#else
		while (enumLevel != ERHIFeatureLevel::Num && (ShaderMap == nullptr || ShaderMap->GetNumShaders() == 0))
#endif
		{
			MatRes = Material->GetMaterialResource(static_cast<ERHIFeatureLevel::Type>(enumLevel));
			if(MatRes)
			{
				ShaderMap = MatRes->GetGameThreadShaderMap();
			}
			++enumLevel;
		}
#if ENGINE_MAJOR_VERSION == 4 && ENGINE_MINOR_VERSION >= 25
		TMap<FShaderId, TShaderRef<FShader>> ShaderList;
#elif ENGINE_MAJOR_VERSION == 4 && ENGINE_MINOR_VERSION <=24
		TMap<FShaderId, FShader*> ShaderList;
#endif

		if (ShaderMap != nullptr)
		{
			ShaderMap->GetShaderList(ShaderList);
			for (auto Item : ShaderList)
			{
#if ENGINE_MAJOR_VERSION ==4 && ENGINE_MINOR_VERSION >=25
				const auto FshaderType = Item.Value.GetType();
				if (FshaderType)
				{
					Shader* sample = Shader::Get();
					sample->SetName(TCHAR_TO_UTF8(FshaderType->GetName()));
					sample->Size = FshaderType->GetTypeSize();
					UWAAssetStats::Get().SampleShader(sample);
					Shader::Destory(sample);
				}
#else
				Shader* sample = Shader::Get();
				sample->SetName(TCHAR_TO_UTF8(Item.Value->GetType()->GetName()));
				sample->Size = Item.Value->GetSizeBytes();
				UWAAssetStats::Get().SampleShader(sample);
				Shader::Destory(sample);
#endif
			}
		}
	}
	UWAAssetStats::Get().DumpAssetInfoMap(Ar);
	Ar.Flush();
}

void FAssetStats::SampleAnimation(FileWriter & Ar)
{
	if (!Ar) return;

	Ar.Println("1,1,0,0,0");

	for (TObjectIterator<UAnimSequence> It; It; ++It)
	{
		UAnimSequence* AnimSequence = *It;
		Animation* sample = Animation::Get();

		sample->SetName(TCHAR_TO_UTF8(*AnimSequence->GetPathName()));

#if ENGINE_MAJOR_VERSION == 4 && ENGINE_MINOR_VERSION > 18
		sample->Size = AnimSequence->GetResourceSizeBytes(EResourceSizeMode::Type::EstimatedTotal);
#elif ENGINE_MAJOR_VERSION == 4 && ENGINE_MINOR_VERSION == 18
		uint32 Size = AnimSequence->GetResourceSizeBytes(EResourceSizeMode::Type::Inclusive);
		sample->Size = AnimSequence->GetResourceSizeBytes(EResourceSizeMode::Type::Inclusive);
#endif

		sample->HashCode = UWAAssetStats::Get().GetAssetHashCode(sample->GetName(), "AnimationClip", sample->Size);

		// this asset has a method id ?
		if (!UWAAssetStats::Get().MidContains(sample->HashCode))
		{
			sample->Length = AnimSequence->SequenceLength;
#if ENGINE_MAJOR_VERSION ==4 && ENGINE_MINOR_VERSION >=23
			sample->Frames = AnimSequence->GetRawNumberOfFrames();
#else
			sample->Frames = AnimSequence->NumFrames;
#endif
			sample->FrameRate = sample->Frames / sample->Length;
			UWAAssetStats::Get().SampleAnimation(sample);
		}

		// this asset has been appeared in this frame ?
		UWAAssetStats::Get().AddAssetInfo(sample->HashCode, sample->Size);
		Animation::Destory(sample);
	}

	UWAAssetStats::Get().DumpAssetInfoMap(Ar);
	Ar.Flush();
}

void FAssetStats::SampleSound(FileWriter & Ar)
{
	if (!Ar) return;

	Ar.Println("1,2,0,0,0");

	for (TObjectIterator<USoundWave> It; It; ++It)
	{
		USoundWave* Sound = *It;

		struct Sound* sample = Sound::Get();

		sample->SetName(TCHAR_TO_UTF8(*Sound->GetPathName()));
		sample->Size = Sound->GetResourceSizeBytes(EResourceSizeMode::Type::Exclusive);

		sample->HashCode = UWAAssetStats::Get().GetAssetHashCode(sample->GetName(), "AudioClip", sample->Size);

		// this asset has a method id ?
		if (!UWAAssetStats::Get().MidContains(sample->HashCode))
		{
			sample->Length = Sound->Duration;

#if ENGINE_MAJOR_VERSION == 4 && ENGINE_MINOR_VERSION >= 21
			sample->Sample = Sound->GetSampleRateForCurrentPlatform();
#elif ENGINE_MAJOR_VERSION == 4 && ENGINE_MINOR_VERSION == 20
			sample->Sample = Sound->GetSampleRateForCurrentPlatform();
#elif ENGINE_MAJOR_VERSION == 4 && ENGINE_MINOR_VERSION == 19
			sample->Sample = Sound->SampleRate;
#elif ENGINE_MAJOR_VERSION == 4 && ENGINE_MINOR_VERSION == 18
			sample->Sample = Sound->SampleRate;
#endif
			UWAAssetStats::Get().SampleSound(sample);
		}

		// this asset has been appeared in this frame ?
		UWAAssetStats::Get().AddAssetInfo(sample->HashCode, sample->Size);
		Sound::Destory(sample);
	}

	UWAAssetStats::Get().DumpAssetInfoMap(Ar);
	Ar.Flush();
}

void FAssetStats::SampleParticleSystem(FileWriter & Ar)
{
	if (!Ar) return;

	Ar.Println("1,7,0,0,0");

	for (TObjectIterator<UParticleSystemComponent> It; It; ++It)
	{
		UParticleSystemComponent* ParticleSystem = *It;
		Particle* sample = Particle::Get();
		UParticleSystem* Template = ParticleSystem->Template;
		if (!Template) continue;

		sample->SetName(TCHAR_TO_UTF8(*Template->GetPathName()));

		FResourceSizeEx CompResSize = FResourceSizeEx(EResourceSizeMode::Type::Exclusive);
		ParticleSystem->GetResourceSizeEx(CompResSize);

		sample->Size = CompResSize.GetTotalMemoryBytes();

		sample->HashCode = UWAAssetStats::Get().GetAssetHashCode(sample->GetName(), "ParticleSystem", sample->Size);

		// this asset has a method id ?
		if (!UWAAssetStats::Get().MidContains(sample->HashCode))
		{
			sample->bIsActive = ParticleSystem->IsActive();

			if (sample->bIsActive)
			{
				for (int32 EmitterIndex = 0; EmitterIndex < ParticleSystem->EmitterInstances.Num(); EmitterIndex++)
				{
					FParticleEmitterInstance* Instance = ParticleSystem->EmitterInstances[EmitterIndex];
					if (Instance && Instance->SpriteTemplate)
					{
						UParticleLODLevel* SpriteLODLevel = Instance->SpriteTemplate->GetCurrentLODLevel(Instance);
						if (SpriteLODLevel && SpriteLODLevel->bEnabled)
						{
							sample->ParticleCount += Instance->ActiveParticles;
						}
					}
				}
			}
			UWAAssetStats::Get().SampleParticleSystem(sample);
		}

		// this asset has been appeared in this frame ?
		UWAAssetStats::Get().AddAssetInfo(sample->HashCode, sample->Size);
		Particle::Destory(sample);
	}

	UWAAssetStats::Get().DumpAssetInfoMap(Ar);
	Ar.Flush();
}

void FAssetStats::SampleMeshCollider(FileWriter & Ar)
{

}

void FAssetStats::SampleMaterial(FileWriter & Ar)
{
	if (!Ar) return;

	Ar.Println("1,4,0,0,0");

	for (TObjectIterator<UMaterial> It; It; ++It)
	{
		UMaterial* Material = *It;

		struct Material* sample = Material::Get();

		sample->SetName(TCHAR_TO_UTF8(*Material->GetPathName()));
		sample->Size = Material->GetResourceSizeBytes(EResourceSizeMode::Type::Exclusive);
		UWAAssetStats::Get().SampleMaterial(sample);
		Material::Destory(sample);
	}

	UWAAssetStats::Get().DumpAssetInfoMap(Ar);
	Ar.Flush();
}

void FAssetStats::SampleFont(FileWriter & Ar)
{
	if (!Ar) return;

	Ar.Println("1,3,0,0,0");
	for (TObjectIterator<UFont> It; It; ++It)
	{
		UFont* Font = *It;
		struct Font* sample = Font::Get();

		sample->SetName(TCHAR_TO_UTF8(*Font->GetPathName()));
#if ENGINE_MAJOR_VERSION == 4 && ENGINE_MINOR_VERSION >= 21 
		sample->Size = Font->GetResourceSizeBytes(EResourceSizeMode::Type::EstimatedTotal);
#elif ENGINE_MAJOR_VERSION == 4 && ENGINE_MINOR_VERSION == 20
		sample->Size = Font->GetResourceSizeBytes(EResourceSizeMode::Type::EstimatedTotal);
#elif ENGINE_MAJOR_VERSION == 4 && ENGINE_MINOR_VERSION == 19
		sample->Size = Font->GetResourceSizeBytes(EResourceSizeMode::Type::EstimatedTotal);
#elif ENGINE_MAJOR_VERSION == 4 && ENGINE_MINOR_VERSION == 18
		sample->Size = Font->GetResourceSizeBytes(EResourceSizeMode::Type::Inclusive);
#endif
		UWAAssetStats::Get().SampleFont(sample);
		Font::Destory(sample);
	}

	UWAAssetStats::Get().DumpAssetInfoMap(Ar);
	Ar.Flush();
}