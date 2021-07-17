# UE4源码修改

### UE4.25

* fix

  * fix android assets目录读写闪退问题

    ``` tex
    Engine\Source\Runtime\Core\Private\Android\AndroidPlatformFile.cpp
    
    	// Handle that covers the entire content of an asset.
    	FFileHandleAndroid(const FString & path, AAsset * asset)
    		: File(MakeShareable(new FileReference(path, asset)))
    		, Start(0), Length(0), CurrentOffset(0)
    	{
    #if UE_ANDROID_FILE_64 && PLATFORM_32BITS
    		File->Handle = AAsset_openFileDescriptor64(File->Asset, &Start, &Length);
    #else
    		off_t OutStart = Start;
    		off_t OutLength = Length;
    		File->Handle = AAsset_openFileDescriptor(File->Asset, &OutStart, &OutLength);
    		Start = OutStart;
    		Length = OutLength;
    #endif
    		//### FIX(): android assets dir
    		CurrentOffset = Start;
    		
    		CheckValid();
    		LogInfo();
    	}
    ```

    

