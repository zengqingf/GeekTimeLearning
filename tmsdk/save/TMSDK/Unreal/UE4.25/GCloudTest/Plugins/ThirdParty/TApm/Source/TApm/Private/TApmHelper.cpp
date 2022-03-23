//------------------------------------------------------------------------------
//
// File: TApmHelper.cpp
// Module: APM
// Version: 1.0
// Author: vincentwgao
//
//------------------------------------------------------------------------------
#pragma once
#include "TApmHelper.h"
#include "Kismet/KismetSystemLibrary.h"
#if PLATFORM_ANDROID
#include "Android/AndroidJNI.h"
#include "Android/AndroidApplication.h"
#endif

#if PLATFORM_IOS
#include <stdlib.h>
#include <pthread.h>
#include <string.h>
#include "Misc/Paths.h"
#include "HAL/FileManager.h"
#include "ApmApiInterfaceObject.h"
#include "APMUEObserver.h"
#endif

namespace GCloud
{
    namespace APM
    {
        const char* ValueKeyCoordinates = "##ValueKeyCoordinates##";
        const char* TApmHelper::TUPLE_KEY = "#KEY#";
        const char* TApmHelper::TUPLE_VALUE = "#VALUE#";
        bool TApmHelper::bEnableApm = false;
        TApmCallback *TApmHelper::sCallback = NULL;
#if PLATFORM_ANDROID
        // cache some methods
        // according to jni specification, jmethod can be cached
        static jmethodID gPostNTLMethod = 0;
        static jmethodID gPutKVIMethod = 0;
        static jmethodID gPutKVSMethod = 0;
        static jmethodID gPutKVDMethod = 0;
        static jmethodID gPostFrameMethod = 0;
        static jmethodID gPostStepEventMethod = 0;
        static jmethodID gPostStreamEventMethod = 0;
        static jmethodID gPostCoordinatesMethod = 0;

        static jmethodID gPostF1Method = 0;
        static jmethodID gPostF2Method = 0;
        static jmethodID gPostF3Method = 0;

        static jmethodID gPostI1Method = 0;
        static jmethodID gPostI2Method = 0;
        static jmethodID gPostI3Method = 0;

        static jmethodID gPostSMethod = 0;

        static jmethodID gBeginTupleWrapMethod = 0;
        static jmethodID gEndTupleWrapMethod = 0;
#endif

        typedef struct _deviceLevelContext
        {
            char* fileDirPath;
            char* configName;
            TApmCallback* callBack;

            _deviceLevelContext()
            {
                configName = NULL;
                fileDirPath = NULL;
                callBack = NULL;
            }
        }DeviceLevelContext;

        static void* device_level_thread(void* ptr)
        {
#if PLATFORM_IOS
            DeviceLevelContext* context = (DeviceLevelContext*)ptr;
            if (context != NULL)
            {
                int deviceLevel = apm_checkDCLSByQccSync(context->fileDirPath, context->configName);

                if (context->fileDirPath != NULL)
                {
                    free((void*)context->fileDirPath);
                }

                if (context->configName != NULL)
                {
                    free((void*)context->configName);
                }

                delete context;
            }
#endif
            return NULL;
        }

        /*deprecated */
        void TApmHelper::EnableDebugMode()
        {

        }
        /*deprecated */
        void TApmHelper::SetTragetFrameRate(int targetFrame)
        {
            SetTargetFramerate(targetFrame);
        }
        /*deprecated */
        void TApmHelper::PostLagStatus(float distance)
        {

        }
        /*deprecated */
        void TApmHelper::MarkAppFinishLaunch()
        {

        }
        /*deprecated */
        void TApmHelper::SetPssManualMode()
        {

        }
        /*deprecated */
        void TApmHelper::RequestPssSample()
        {

        }
        /*deprecated */
        void TApmHelper::PostStreamEvent(int stepId, int status, int code, const char* info)
        {

        }
        /*deprecated */
        char* TApmHelper::GetCurrentSceneName()
        {
            return nullptr;
        }
        /*deprecated */
        int TApmHelper::GetSceneLoadedTime()
        {
            return 0;
        }
        /*deprecated */
        int TApmHelper::GetMaxPss()
        {
            return 0;
        }
        /*deprecated */
        int TApmHelper::GetTotalTime()
        {
            return 0;
        }
        /*deprecated */
        int TApmHelper::GetTotalFrames()
        {
            return 0;
        }
        /*deprecated */
        void TApmHelper::SetCallback(TApmCallback *callback)
        {

        }
        /* 获取SDK的版本信息 */
        const char* TApmHelper::GetSDKVersion()
        {
#if PLATFORM_ANDROID
            char* retValue = NULL;
            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {

                jmethodID GetErrorMsgMethod = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_getSDKVersion", "()Ljava/lang/String;", false);
                jobject jret = FJavaWrapper::CallObjectMethod(Env, FJavaWrapper::GameActivityThis, GetErrorMsgMethod);

                if (jret != nullptr)
                {
                    jstring jstr = (jstring)jret;
                    const char *cstr = NULL;

                    jboolean isCopy;   // 返回JNI_TRUE表示原字符串的拷贝，返回JNI_FALSE表示返回原字符串的指针  
                    cstr = Env->GetStringUTFChars(jstr, &isCopy);

                    if (cstr != NULL)
                    {
                        retValue = (char*)malloc(strlen(cstr) + 1);
                        strcpy(retValue, cstr);
                        Env->ReleaseStringUTFChars(jstr, cstr);
                    }
                }
            }
            return retValue;
#endif

#if PLATFORM_IOS
            return apm_getSDKVersion();
#endif

            return NULL;
        }

        LogObserver TApmHelper::sLogObserver = nullptr;

        void TApmHelper::SetLogObserver(LogObserver logObserver) {
            sLogObserver = logObserver;
        }

        /*depreacted*/
        int TApmHelper::InitContext(const char *appId, const char* engine, bool debug)
        {
            return InitContext(appId, debug);
        }

        int TApmHelper::InitContext(const char *appId, bool debug)
        {
            if (TApmHelper::bEnableApm)
            {
                return -1;
            }

            int retValue = -1;

#if PLATFORM_ANDROID

            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {


                FString engineVerison = UKismetSystemLibrary::GetEngineVersion();
                const char* cEngineVersion = TCHAR_TO_UTF8(*engineVerison);
                if (cEngineVersion != NULL) {
                    jstring jEngineVersion = Env->NewStringUTF(cEngineVersion);
                    jstring jNAPhrase = Env->NewStringUTF("NA");

                    jmethodID SetEngineInfoMethod = FJavaWrapper::FindMethod(Env,
                        FJavaWrapper::GameActivityClassID,
                        "AndroidThunkJava_setEngineMetaInfo",
                        "(ILjava/lang/String;ILjava/lang/String;Ljava/lang/String;Ljava/lang/String;IIII)V",
                        false);
                    FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, SetEngineInfoMethod, 2, jEngineVersion, 0,
                        jNAPhrase, jNAPhrase, jNAPhrase, 0, 0, 0, (!FAndroidMisc::ShouldUseVulkan()) ? 1 : 0);
                    Env->DeleteLocalRef(jEngineVersion);
                    Env->DeleteLocalRef(jNAPhrase);
                }

                jstring jAppID = Env->NewStringUTF(appId);
                jmethodID InitContextMethod = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_initContext", "(Ljava/lang/String;Ljava/lang/String;Z)I", false);
                retValue = FJavaWrapper::CallIntMethod(Env, FJavaWrapper::GameActivityThis, InitContextMethod, jAppID, jAppID, debug);
                Env->DeleteLocalRef(jAppID);

                TApmHelper::bEnableApm = true;
            }
            else
            {
                UE_LOG(LogInit, Warning, TEXT("apm_sdk ENV IS NULL"));
            }



#endif

#if PLATFORM_IOS
            FString engineVerison = UKismetSystemLibrary::GetEngineVersion();
            const char* cEngineVersion = TCHAR_TO_UTF8(*engineVerison);
            if (cEngineVersion != NULL) {
                apm_setEngineMetaInfo(2, cEngineVersion, 0, "NA", "NA", "NA", 0, 0, 0, 1);
            }
            retValue = apm_initContext(appId, "unreal", debug);
            TApmHelper::bEnableApm = true;

            APMUEObserver *observer = APMUEObserver::GetInstance();

            apm_setObserver(observer);

#endif

            return retValue;
        }

        void TApmHelper::SyncServerTime(unsigned time) {
#if PLATFORM_ANDROID
            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                jmethodID Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_syncServerTime", "(J)V", false);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, Method, (long)time);
            }
#endif
#if PLATFORM_IOS
            apm_syncServerTime(time);
#endif

        }

        void TApmHelper::DisableDomCDNDomain() {
#if PLATFORM_ANDROID
            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                jmethodID Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_disableDomCDNDomain", "()V", false);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, Method);
            }
#endif
#if PLATFORM_IOS
            apm_blockDomesticCDNURL();
#endif

        }


        void TApmHelper::SetUserId(const char* openId)
        {
            if (!TApmHelper::bEnableApm)
            {
                return;
            }

#if PLATFORM_ANDROID
            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                jstring jUserId = Env->NewStringUTF(openId);
                jmethodID Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_setOpenId", "(Ljava/lang/String;)V", false);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, Method, jUserId);
                Env->DeleteLocalRef(jUserId);
            }
#endif

#if PLATFORM_IOS
            apm_setOpenId(openId);
#endif
        }

        void TApmHelper::MarkLevelLoad(const char* levelName)
        {

            if (levelName == NULL) return;
            if (!TApmHelper::bEnableApm)
            {
                return;
            }

#if PLATFORM_ANDROID
            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                jstring jLevelName = Env->NewStringUTF(levelName);
                jmethodID Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_markLevelLoad", "(Ljava/lang/String;)V", false);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, Method, jLevelName);
                Env->DeleteLocalRef(jLevelName);
            }
#endif

#if PLATFORM_IOS
            //apm_markLoadlevel(levelName);
            apm_markLevelLoad(levelName);
#endif

        }



        void TApmHelper::MarkLevelLoadCompleted()
        {
            if (!TApmHelper::bEnableApm)
            {
                return;
            }


#if PLATFORM_ANDROID
            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                jmethodID Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_markLevelLoadCompleted", "()V", false);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, Method);
            }
#endif

#if PLATFORM_IOS
            apm_markLevelLoadCompleted();
#endif
        }



        void TApmHelper::MarkLevelFin()
        {
            if (!TApmHelper::bEnableApm)
            {
                return;
            }

#if PLATFORM_ANDROID
            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                do {
                    jmethodID Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_markLevelFin", "()V", false);

                    if (Method == 0) break;
                    FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, Method);
                } while (false);


            }
#endif


#if PLATFORM_IOS
            apm_markLevelFin();
#endif

        }


        void TApmHelper::PostNTL(int latency)
        {
            if (!TApmHelper::bEnableApm)
            {
                return;
            }

#if PLATFORM_ANDROID
            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                if (gPostNTLMethod == 0)
                {
                    gPostNTLMethod = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_postNTL", "(I)V", false);
                }
                if (gPostNTLMethod == 0) return;
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, gPostNTLMethod, latency);

            }
#endif

#if PLATFORM_IOS
            apm_postNetLatency(latency);
#endif

        }

        void TApmHelper::BeginExtTag(const char* tagName)
        {
            if (!TApmHelper::bEnableApm)
            {
                return;
            }
            if (tagName == NULL) return;
#if PLATFORM_ANDROID
            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                jstring jTagName = Env->NewStringUTF(tagName);
                jmethodID Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_beginExtTag", "(Ljava/lang/String;)V", false);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, Method, jTagName);
                Env->DeleteLocalRef(jTagName);
            }
#endif

#if PLATFORM_IOS
            apm_beginExtTag(tagName);
#endif

        }



        void TApmHelper::EndExtTag(const char* tagName)
        {
            if (!TApmHelper::bEnableApm)
            {
                return;
            }
            if (tagName == NULL) return;


#if PLATFORM_ANDROID
            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                jstring jTagName = Env->NewStringUTF(tagName);
                jmethodID Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_endExtTag", "(Ljava/lang/String;)V", false);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, Method, jTagName);
                Env->DeleteLocalRef(jTagName);
            }
#endif

#if PLATFORM_IOS
            apm_endExtTag(tagName);
#endif
        }

        void TApmHelper::BeginTag(const char* tagName)
        {
            if (tagName == NULL) return;
            if (!TApmHelper::bEnableApm)
            {
                return;
            }
#if PLATFORM_ANDROID
            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                jstring jTagName = Env->NewStringUTF(tagName);
                jmethodID Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_beginTag", "(Ljava/lang/String;)V", false);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, Method, jTagName);
                Env->DeleteLocalRef(jTagName);
            }
#endif

#if PLATFORM_IOS
            apm_beginTag(tagName);
#endif

        }

        void TApmHelper::EndTag()
        {
            if (!TApmHelper::bEnableApm)
            {
                return;
            }


#if PLATFORM_ANDROID
            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                jmethodID Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_endTag", "()V", false);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, Method);
            }
#endif

#if PLATFORM_IOS
            apm_endTag();
#endif

        }

#if PLATFORM_IOS
        int TApmHelper::GetDeviceLevelByQcc(const char* configName, const char* filePath) {
            if (!TApmHelper::bEnableApm)
            {
                return 0;
            }

            if (configName == NULL || filePath == NULL)
            {
                return 0;
            }
            FString contentPath = FString(filePath);
            FString fullPath = FPaths::ConvertRelativePathToFull(contentPath);
            FString diskPath = IFileManager::Get().GetFilenameOnDisk(*fullPath);
            FString absolutePath = IFileManager::Get().ConvertToAbsolutePathForExternalAppForRead(*diskPath);
            return apm_checkDCLSByQcc(TCHAR_TO_UTF8(*absolutePath), configName);
        }
#else

        int TApmHelper::GetDeviceLevelByQcc(const char* configName, const char* gpuFamily)
        {
            if (!TApmHelper::bEnableApm)
            {
                return 0;
            }

            if (configName == NULL || gpuFamily == NULL)
            {
                return 0;
            }
#if PLATFORM_ANDROID
            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                jstring jConfigName = Env->NewStringUTF(configName);
                jstring jGpuFamily = Env->NewStringUTF(gpuFamily);
                jmethodID Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_getDeviceLevelByQcc", "(Ljava/lang/String;Ljava/lang/String;)I", false);
                int retValue = FJavaWrapper::CallIntMethod(Env, FJavaWrapper::GameActivityThis, Method, jConfigName, jGpuFamily);
                Env->DeleteLocalRef(jConfigName);
                Env->DeleteLocalRef(jGpuFamily);

                return retValue;
            }
            else
            {
                return 0;
            }
#endif

            return 0;
        }
#endif

        /*deprecated */
        void TApmHelper::SetLocale(const char* locale)
        {

        }

        void TApmHelper::SetQuality(int quality)
        {
#if PLATFORM_ANDROID
            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                jmethodID Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_setQuality", "(I)V", false);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, Method, quality);
            }
#endif

#if PLATFORM_IOS
            //apm_setQuality(quality);
            apm_setQulaity(quality);
#endif
        }

        void TApmHelper::PostEvent(int key, const char* info)
        {
            if (info == NULL) return;
#if PLATFORM_ANDROID
            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                jstring jeventkey = Env->NewStringUTF(info);
                jmethodID Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_postMarkEvent", "(ILjava/lang/String;)V", false);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, Method, key, jeventkey);
                Env->DeleteLocalRef(jeventkey);
            }
#endif

#if PLATFORM_IOS
            apm_postEventIS(key, info);
#endif
        }

        void TApmHelper::SetVersionIden(const char* versionName)
        {
            if (versionName == NULL) return;
#if PLATFORM_ANDROID
            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                jstring jVersionName = Env->NewStringUTF(versionName);
                jmethodID Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_setVersionIden", "(Ljava/lang/String;)V", false);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, Method, jVersionName);
                Env->DeleteLocalRef(jVersionName);
            }
#endif

#if PLATFORM_IOS
            apm_setVersionIden(versionName);
#endif
        }

        void TApmHelper::PostFrame(float deltatime)
        {
#if PLATFORM_ANDROID
            /*if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
            jmethodID Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_postFrame", "(F)V", false);
            FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, Method, deltatime);
            }*/

            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                if (gPostFrameMethod == 0)
                {
                    gPostFrameMethod = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_postFrame", "(F)V", false);
                }

                if (gPostFrameMethod == 0)
                {
                    return;
                }

                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, gPostFrameMethod, deltatime);
            }

#endif

#if PLATFORM_IOS
            apm_postFrame(deltatime);
#endif
        }


        void TApmHelper::PostTrackState(float x, float y, float z, float pitch, float yaw, float roll)
        {
#if PLATFORM_ANDROID
            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                if (gPostCoordinatesMethod == 0)
                {
                    gPostCoordinatesMethod = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_postCoordinates", "(FFFFFF)V", false);
                }

                if (gPostCoordinatesMethod == 0)
                {
                    return;
                }

                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, gPostCoordinatesMethod, x, y, z, pitch, yaw, roll);
            }
#endif

#if PLATFORM_IOS
            apm_postCoordinates(x, y, z, pitch, yaw, roll);
#endif
        }

        void TApmHelper::PostValueF(const char* catgory, const char* key, float a)
        {
            if (catgory == NULL || key == NULL) return;
#if PLATFORM_ANDROID

            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                if (gPostF1Method == 0)
                {
                    gPostF1Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_postValueF", "(Ljava/lang/String;Ljava/lang/String;F)V", false);
                }

                if (gPostF1Method == 0)
                {
                    return;
                }

                jstring jInfo = Env->NewStringUTF(key);
                jstring jCategory = Env->NewStringUTF(catgory);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, gPostF1Method, jCategory, jInfo, a);
                Env->DeleteLocalRef(jInfo);
                Env->DeleteLocalRef(jCategory);
            }

#endif

#if PLATFORM_IOS
            apm_postValueF1(catgory, key, a);
#endif
        }

        void TApmHelper::PostValueF(const char* catgory, const char* key, float a, float b)
        {
            if (catgory == NULL || key == NULL) return;
#if PLATFORM_ANDROID

            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                if (gPostF2Method == 0)
                {
                    gPostF2Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_postValueF", "(Ljava/lang/String;Ljava/lang/String;FF)V", false);
                }

                if (gPostF2Method == 0)
                {
                    return;
                }

                jstring jInfo = Env->NewStringUTF(key);
                jstring jCategory = Env->NewStringUTF(catgory);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, gPostF2Method, jCategory, jInfo, a, b);
                Env->DeleteLocalRef(jInfo);
                Env->DeleteLocalRef(jCategory);
            }
#endif

#if PLATFORM_IOS
            apm_postValueF2(catgory, key, a, b);
#endif
        }

        void TApmHelper::PostValueF(const char* catgory, const char* key, float a, float b, float c)
        {
            if (catgory == NULL || key == NULL) return;
#if PLATFORM_ANDROID

            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                if (gPostF3Method == 0)
                {
                    gPostF3Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_postValueF", "(Ljava/lang/String;Ljava/lang/String;FFF)V", false);
                }

                if (gPostF3Method == 0)
                {
                    return;
                }

                jstring jInfo = Env->NewStringUTF(key);
                jstring jCategory = Env->NewStringUTF(catgory);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, gPostF3Method, jCategory, jInfo, a, b, c);
                Env->DeleteLocalRef(jInfo);
                Env->DeleteLocalRef(jCategory);
            }

#endif

#if PLATFORM_IOS
            apm_postValueF3(catgory, key, a, b, c);
#endif
        }

        void TApmHelper::PostValueI(const char* catgory, const char* key, int a)
        {
            if (catgory == NULL || key == NULL) return;
#if PLATFORM_ANDROID

            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                if (gPostI1Method == 0)
                {
                    gPostI1Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_postValueI", "(Ljava/lang/String;Ljava/lang/String;I)V", false);
                }

                if (gPostI1Method == 0)
                {
                    return;
                }

                jstring jInfo = Env->NewStringUTF(key);
                jstring jCategory = Env->NewStringUTF(catgory);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, gPostI1Method, jCategory, jInfo, a);
                Env->DeleteLocalRef(jInfo);
                Env->DeleteLocalRef(jCategory);
            }

#endif

#if PLATFORM_IOS
            apm_postValueI1(catgory, key, a);
#endif
        }
        void TApmHelper::PostValueI(const char* catgory, const char* key, int a, int b)
        {
            if (catgory == NULL || key == NULL) return;
#if PLATFORM_ANDROID

            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                if (gPostI2Method == 0)
                {
                    gPostI2Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_postValueI", "(Ljava/lang/String;Ljava/lang/String;II)V", false);
                }

                if (gPostI2Method == 0)
                {
                    return;
                }

                jstring jInfo = Env->NewStringUTF(key);
                jstring jCategory = Env->NewStringUTF(catgory);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, gPostI2Method, jCategory, jInfo, a, b);
                Env->DeleteLocalRef(jInfo);
                Env->DeleteLocalRef(jCategory);
            }


#endif

#if PLATFORM_IOS
            apm_postValueI2(catgory, key, a, b);
#endif

        }
        void TApmHelper::PostValueI(const char* catgory, const char* key, int a, int b, int c)
        {
            if (catgory == NULL || key == NULL) return;
#if PLATFORM_ANDROID

            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                if (gPostI3Method == 0)
                {
                    gPostI3Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_postValueI", "(Ljava/lang/String;Ljava/lang/String;III)V", false);
                }

                if (gPostI3Method == 0)
                {
                    return;
                }

                jstring jInfo = Env->NewStringUTF(key);
                jstring jCategory = Env->NewStringUTF(catgory);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, gPostI3Method, jCategory, jInfo, a, b, c);
                Env->DeleteLocalRef(jInfo);
                Env->DeleteLocalRef(jCategory);
            }


#endif

#if PLATFORM_IOS
            apm_postValueI3(catgory, key, a, b, c);
#endif
        }

        void TApmHelper::PostValueS(const char* catgory, const char* key, const char* value)
        {
            if (catgory == NULL || key == NULL) return;
#if PLATFORM_ANDROID

            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                if (gPostSMethod == 0)
                {
                    gPostSMethod = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_postValueS", "(Ljava/lang/String;Ljava/lang/String;Ljava/lang/String;)V", false);
                }

                if (gPostSMethod == 0)
                {
                    return;
                }

                jstring jKey = Env->NewStringUTF(key);
                jstring jCategory = Env->NewStringUTF(catgory);
                jstring jValue = Env->NewStringUTF(value);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, gPostSMethod, jCategory, jKey, jValue);
                Env->DeleteLocalRef(jKey);
                Env->DeleteLocalRef(jCategory);
                Env->DeleteLocalRef(jValue);
            }

#endif

#if PLATFORM_IOS
            apm_postValueS(catgory, key, value);
#endif
        }

        void TApmHelper::BeginTupleWrap(const char* key)
        {
            if (key == NULL) return;
#if PLATFORM_ANDROID

            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                if (gBeginTupleWrapMethod == 0)
                {
                    gBeginTupleWrapMethod = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_beginTupleWrap", "(Ljava/lang/String;)V", false);
                }

                if (gBeginTupleWrapMethod == 0)
                {
                    return;
                }

                jstring jInfo = Env->NewStringUTF(key);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, gBeginTupleWrapMethod, jInfo);
                Env->DeleteLocalRef(jInfo);
            }

#endif

#if PLATFORM_IOS
            apm_beginTupleWrap(key);
#endif
        }
        void TApmHelper::EndTupleWrap()
        {
#if PLATFORM_ANDROID

            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                if (gEndTupleWrapMethod == 0)
                {
                    gEndTupleWrapMethod = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_endTupleWrap", "()V", false);
                }

                if (gEndTupleWrapMethod == 0)
                {
                    return;
                }

                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, gEndTupleWrapMethod);
            }

#endif

#if PLATFORM_IOS
            apm_endTupleWrap();
#endif
        }

        void TApmHelper::BeginExclude()
        {
#if PLATFORM_ANDROID

            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                jmethodID Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_beginExclude", "()V", false);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, Method);
            }

#endif

#if PLATFORM_IOS
            //apm_beginExclude();
            apm_beignExclude();
#endif
        }
        void TApmHelper::EndExclude()
        {
#if PLATFORM_ANDROID

            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                jmethodID Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_endExclude", "()V", false);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, Method);
            }

#endif

#if PLATFORM_IOS
            apm_endExclude();
#endif
        }

        void TApmHelper::InitStepEventContext()
        {
#if PLATFORM_ANDROID
            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                jmethodID Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_initStepEventContext", "()V", false);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, Method);
            }
#endif

#if PLATFORM_IOS
            apm_initStepEventContext();
#endif
        }

        void TApmHelper::LinkLastStepEventSession(FString eventCategory)
        {
#if PLATFORM_ANDROID
            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                jstring jEventCategory = Env->NewStringUTF(TCHAR_TO_UTF8(*eventCategory));
                jmethodID Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_linkLastStepCategorySession", "(Ljava/lang/String;)V", false);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, Method, jEventCategory);
                Env->DeleteLocalRef(jEventCategory);
            }
#endif

#if PLATFORM_IOS
            apm_linkSession(TCHAR_TO_UTF8(*eventCategory));
#endif
        }

        void TApmHelper::PostStepEvent(FString eventCategory, int stepId, int status, int code, FString msg, FString extraKey, bool authorize, bool finish)
        {
#if PLATFORM_ANDROID
            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                if (gPostStepEventMethod == 0)
                {
                    gPostStepEventMethod = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_postStepEvent", "(Ljava/lang/String;IIILjava/lang/String;Ljava/lang/String;ZZ)V", false);

                }

                if (gPostStepEventMethod == 0)
                {
                    return;
                }

                jstring jEventCategory = Env->NewStringUTF(TCHAR_TO_UTF8(*eventCategory));
                jstring jMsg = Env->NewStringUTF(TCHAR_TO_UTF8(*msg));
                jstring jExtraKey = Env->NewStringUTF(TCHAR_TO_UTF8(*extraKey));
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, gPostStepEventMethod, jEventCategory, stepId, status, code, jMsg, jExtraKey, authorize, finish);
                Env->DeleteLocalRef(jEventCategory);
                Env->DeleteLocalRef(jMsg);
                Env->DeleteLocalRef(jExtraKey);
            }
#endif

#if PLATFORM_IOS
            apm_postStepEvent(TCHAR_TO_UTF8(*eventCategory), stepId, status, code, TCHAR_TO_UTF8(*msg), TCHAR_TO_UTF8(*extraKey), authorize, finish);

#endif
        }

        void TApmHelper::ReleaseStepEventContext()
        {
#if PLATFORM_ANDROID
            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                jmethodID Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_releaseStepEventContext", "()V", false);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, Method);
            }
#endif

#if PLATFORM_IOS
            apm_releaseStepEventContext();
#endif

        }

        void TApmHelper::SetTargetFramerate(int target) {
#if PLATFORM_ANDROID
            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                jmethodID Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_setTargetFramerate", "(I)V", false);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, Method, target);
            }
#endif

#if PLATFORM_IOS
            apm_setTargetFramerate(target);
#endif
        }

        void TApmHelper::SetDeviceLevel(int level)
        {
#if PLATFORM_ANDROID
            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                jmethodID Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_setDeviceLevel", "(I)V", false);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, Method, level);
            }
#endif

#if PLATFORM_IOS
            apm_setDefinedDeviceClass(level);
#endif
        }

        void TApmHelper::MarkStartUpFinish()
        {
#if PLATFORM_ANDROID
            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {
                jmethodID Method = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_markStarUpFinish", "()V", false);
                FJavaWrapper::CallVoidMethod(Env, FJavaWrapper::GameActivityThis, Method);
            }
#endif

#if PLATFORM_IOS
            apm_startUpFinish();
#endif
        }


        char* TApmHelper::GetErrorMsg(int code)
        {
#if PLATFORM_ANDROID
            char* retValue = NULL;
            if (JNIEnv* Env = FAndroidApplication::GetJavaEnv())
            {

                jmethodID GetErrorMsgMethod = FJavaWrapper::FindMethod(Env, FJavaWrapper::GameActivityClassID, "AndroidThunkJava_getErrorMsg", "(I)Ljava/lang/String;", false);
                jobject jret = FJavaWrapper::CallObjectMethod(Env, FJavaWrapper::GameActivityThis, GetErrorMsgMethod, code);

                if (jret != nullptr)
                {
                    jstring jstr = (jstring)jret;
                    const char *cstr = NULL;

                    jboolean isCopy;   // 返回JNI_TRUE表示原字符串的拷贝，返回JNI_FALSE表示返回原字符串的指针  
                    cstr = Env->GetStringUTFChars(jstr, &isCopy);

                    if (cstr != NULL)
                    {
                        retValue = (char*)malloc(strlen(cstr) + 1);
                        strcpy(retValue, cstr);
                        Env->ReleaseStringUTFChars(jstr, cstr);
                    }
                }
            }
            return retValue;
#elif PLATFORM_IOS
            return (char*)apm_getErrorMsg(code);
#else
            return NULL;
#endif
        }

        void TApmHelper::PostMarkEvent(int key, const char* value)
        {
#if PLATFORM_ANDROID

#endif

#if PLATFORM_IOS

#endif

        }

    }
}



#if PLATFORM_ANDROID
#include <android/log.h>

//ENGINE_API DECLARE_LOG_CATEGORY_EXTERN(TRILOGGER, Log, All);

JNI_METHOD void Java_com_tencent_gcloud_apm_jni_TTApmNativeHelper_TApmOnLog(JNIEnv* LocalJNIEnv, jclass LocalThiz, jstring data)
{

    UE_LOG(LogTemp, Log, TEXT("Java call UE4 to log"));

    const char *rspInfoCStr = LocalJNIEnv->GetStringUTFChars(data, NULL);

    UE_LOG(LogTemp, Log, TEXT("Java call UE4 to log: %s"), UTF8_TO_TCHAR(rspInfoCStr));

    std::string rspInfoStr = std::string(rspInfoCStr);
    if (GCloud::APM::TApmHelper::sLogObserver) {
        GCloud::APM::TApmHelper::sLogObserver(rspInfoStr.c_str());
    }
    LocalJNIEnv->ReleaseStringUTFChars(data, rspInfoCStr);


}

JNI_METHOD void Java_com_tencent_gcloud_apm_jni_TApmNativeHelper_TApmOnFpsNotify(JNIEnv* LocalJNIEnv, jclass LocalThiz, jstring data)
{
    UE_LOG(LogTemp, Log, TEXT("Java call UE4 to log"));

    const char *rspInfoCStr = LocalJNIEnv->GetStringUTFChars(data, NULL);

    UE_LOG(LogTemp, Log, TEXT("Java call UE4 to log: %s"), UTF8_TO_TCHAR(rspInfoCStr));

    // TODO: start UE4 fps
}

JNI_METHOD void Java_com_tencent_gcloud_apm_jni_TApmNativeHelper_TApmOnMarkLevelLoad(JNIEnv* LocalJNIEnv, jclass LocalThiz, jstring data)
{
    //__android_log_print(ANDROID_LOG_ERROR, "[gpm] Java_com_tencent_gcloud_apm_jni_TApmNativeHelper_TApmOnMarkLevelLoad: ", "empty");
}


JNI_METHOD void Java_com_tencent_gcloud_apm_jni_TApmNativeHelper_TApmOnSetQuality(JNIEnv* LocalJNIEnv, jclass LocalThiz, jstring data)
{
    //_android_log_print(ANDROID_LOG_ERROR, "[gpm] Java_com_tencent_gcloud_apm_jni_TApmNativeHelper_TApmOnSetQuality: ", "empty");
}

#endif



#ifdef PLATFORM_IOS

#if defined(__cplusplus)
extern "C" {
#endif

    unsigned long il2cpp_gc_get_heap_size() {
        return 0;
    }


    unsigned long il2cpp_gc_get_used_size() {
        return 0;
    }

#if defined(__cplusplus)
}
#endif

#endif

