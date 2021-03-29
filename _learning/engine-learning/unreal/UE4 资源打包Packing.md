# UE4 资源打包Packing

* ref

  [UE4 资源热更打包工具 HotPatcher](https://imzlp.com/posts/17590/)

  [UE4资源加载（四）UnrealPak打包](https://www.dazhuanlan.com/2019/12/08/5dec71c68a74a/)

  [UE4 项目资源打包,Pak包生成](https://www.cnblogs.com/bodboy/p/6110528.html)

  [UE4打包后视频或者其他资源未打包进去](https://blog.csdn.net/u010925014/article/details/89638878)

  [UE4资源————Pak](https://www.233tw.com/unreal/8887)



* 资源

  * 目录结构

    ``` text
    You can add additional directories to stage via your project settings. Go to "Project Settings", then "Packaging", and you'll find "Additional Non-Asset Directories to Package" and "Additional Non-Asset Directories to Copy" under the advanced section.
    
    Which you use depends on whether your files are loaded via the Unreal File System (UFS), or whether they're loaded via a third-party IO API (including the STL).
    
    "Additional Non-Asset Directories to Package" will add the files to your .pak file, which is what you want when you're using the UFS API.
    
    "Additional Non-Asset Directories to Copy" will stage your files individually (outside the .pak file), which is what you want when you're using a third-party file IO API.
    ```

    

  * json存取

    ``` text
    Create new folder (like Data) inside your Content folder and place your .json file inside
    
    Ignore the editor converting the file (the original .json should still be where you put it).
    
    Inside project settings add your Data folder to the list of folders to copy (if you add it to the packaged it should only embed the .uasset file into game packages, which you don't want)
    
    Use the FPaths class and it's static methods to reference the folder, like so:
    
    FString path = FPaths::Combine(*FPaths::GameDir(), *FString("Data")); Note: Not at my dev machine at the moment so haven't tested that code - it might need adjusting
    ```

    ``` c++
    //使用绝对路径   content 外部访问 
    void UUnrealShooterDataSingleton::ParseJSON()
     {
         FString JsonString;
         const FString fileName = "D:/Projects/UnrealProjects/UnrealShooter/Shared/UnrealShooterData.json";
         FFileHelper::LoadFileToString(JsonString, *fileName);
         TSharedPtr<FJsonObject> JsonObject;
         TSharedRef<TJsonReader<>> Reader = TJsonReaderFactory<>::Create(JsonString);
     
         if (FJsonSerializer::Deserialize(Reader, JsonObject))
         {
             const TArray<TSharedPtr<FJsonValue>> SequencesJSON = JsonObject->GetArrayField(TEXT("sequences"));
             const TArray<TSharedPtr<FJsonValue>> WavesJSON = JsonObject->GetArrayField(TEXT("waves"));
             const TArray<TSharedPtr<FJsonValue>> TargetsJSON = JsonObject->GetArrayField(TEXT("targets"));
             const TArray<TSharedPtr<FJsonValue>> LocationsJSON = JsonObject->GetArrayField("locations");
     
             UUnrealShooterDataSingleton::ParseLocations(LocationsJSON);
             UUnrealShooterDataSingleton::ParseTargets(TargetsJSON);
             UUnrealShooterDataSingleton::ParseWaves(WavesJSON);
             UUnrealShooterDataSingleton::ParseSequences(SequencesJSON);
         }
     }
    ```

    




---



* 命令

  * 解pak

    ``` powershell
    Engine\Binaries\Win64\UnrealPak.exe somepak.pak -extract X:\extract\here 
    ```

    



