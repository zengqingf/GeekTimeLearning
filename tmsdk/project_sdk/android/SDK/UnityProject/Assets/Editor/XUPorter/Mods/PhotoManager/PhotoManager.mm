#import "PhotoManager.h"
#import "Photos/PHAssetChangeRequest.h"
#import "Photos/PHAssetCollectionChangeRequest.h"
#include <pthread.h>

typedef enum {
    NORMAL,
    ERROR,
    FINISH,
}takePhotoState;
struct Imgdata {
    uint8_t *bytes;
    int length;
};
UIImage *img  = NULL;
PHFetchResult<PHAsset *>* Assets = nil;
PHAssetCollection * Collection = nil;
takePhotoState state = FINISH;
NSString *alartTitle = nil;
NSString *alartMessageSuc = nil;
NSString *alartMessageFail = nil;
NSString *alartBtnText = nil;
bool running = false;

void _InitAlartText(char *suc,char *fail){
    alartTitle = @"截图";
    alartMessageSuc = [[NSString alloc]initWithUTF8String:suc];
    alartMessageFail = [[NSString alloc]initWithUTF8String:fail];
    alartBtnText = @"确定";
}
void _SaveImage2Album(uint8_t *bytes,int length,char *suc,char *fail)
{
    if (running) {
        return;
    }
    _InitAlartText(suc,fail);

    pthread_t thd;
    Imgdata * data = (Imgdata *)malloc(sizeof(Imgdata));
    data->bytes = bytes;
    data->length = length;
    pthread_create(&thd, NULL, _SavePhoto, (void *)data);
}
void *_SavePhoto(void *data)
{
    running = true;
    //NSString *strReadAddr = [NSString stringWithUTF8String:readAddr];

    //img = [UIImage imageWithContentsOfFile:strReadAddr];
    if(data == nil){
        return NULL;
    }
    Imgdata *b = (Imgdata *)data;

    img= [UIImage imageWithData:[NSData dataWithBytesNoCopy:b->bytes length:b->length  freeWhenDone:NO]];

    if(img == NULL){
        running = false;
        return NULL;
    }
    PHAuthorizationStatus status = [PHPhotoLibrary authorizationStatus];
    if (status == PHAuthorizationStatusRestricted || status == PHAuthorizationStatusDenied){
        showAlart(false);
        running = false;
        return NULL;
    }
    else{
        state = NORMAL;
        createdAssets();
        
    }
    
}

void showAlart(bool isSuc)
{
    UIAlertView * alart;
    if (isSuc) {
        alart = [[UIAlertView alloc]initWithTitle:alartTitle message:alartMessageSuc delegate:nil cancelButtonTitle:alartBtnText otherButtonTitles:nil, nil];
    }
    else{
        alart = [[UIAlertView alloc]initWithTitle:alartTitle message:alartMessageFail delegate:nil cancelButtonTitle:alartBtnText otherButtonTitles:nil, nil];
    }
    
    [alart show];
}

void createdAssets()
{
    if(state != NORMAL){
        NSLog(@"Permission denied or Storage filled");
        running = false;
        return;
    }
    
    __block NSString *createdAssetId = nil;
    
    // 添加图片到【相机胶卷】
    [[PHPhotoLibrary sharedPhotoLibrary]performChanges:^{
        createdAssetId = [PHAssetChangeRequest creationRequestForAssetFromImage:img].placeholderForCreatedAsset.localIdentifier;
    } completionHandler:^(BOOL success,NSError * _Nullable error){
        if(nil != error || !success){
            state = ERROR;
            running = false;
        }
        
        dispatch_sync(dispatch_get_main_queue(), ^{
            if (createdAssetId == nil){
                Assets = nil;
            }
            else{
                // 在保存完毕后取出图片
                Assets = [PHAsset fetchAssetsWithLocalIdentifiers:@[createdAssetId] options:nil];
            }
            createdCollection();
        });
    }];
}


void createdCollection()
{
    if(state != NORMAL){
        NSLog(@"Permission denied or Storage filled");
        running = false;
        return;
    }
    // 获取软件的名字作为相册的标题
    NSString *title = [NSBundle mainBundle].infoDictionary[(NSString *)kCFBundleNameKey];
    
    // 获得所有的自定义相册
    PHFetchResult<PHAssetCollection *> *collections = [PHAssetCollection fetchAssetCollectionsWithType:PHAssetCollectionTypeAlbum subtype:PHAssetCollectionSubtypeAlbumRegular options:nil];
    for (PHAssetCollection *collection in collections) {
        if ([collection.localizedTitle isEqualToString:title]) {
            Collection = collection;
            saveImageIntoAlbum();
            running = false;
            return;
        }
    }
    
    // 代码执行到这里，说明还没有自定义相册
    
    __block NSString *createdCollectionId = nil;
    
    // 创建一个新的相册
    [[PHPhotoLibrary sharedPhotoLibrary]performChanges:^{
        createdCollectionId = [PHAssetCollectionChangeRequest creationRequestForAssetCollectionWithTitle:title].placeholderForCreatedAssetCollection.localIdentifier;
    } completionHandler:^(BOOL success, NSError * _Nullable error) {
        if(nil != error || !success){
            state = ERROR;
        }
        else{
            dispatch_sync(dispatch_get_main_queue(), ^{
                if(createdCollectionId == nil) Collection = nil;
                Collection = [PHAssetCollection fetchAssetCollectionsWithLocalIdentifiers:@[createdCollectionId] options:nil].firstObject;
                saveImageIntoAlbum();
            });
        }
    }];
}

void saveImageIntoAlbum()
{
    if(state != NORMAL){
        NSLog(@"Permission denied or Storage filled");
        running = false;
        return;
    }
    
    if (Assets == nil || Collection == nil) {
        NSLog(@"Assets is nil or Collection is nil");
        running = false;
        return;
    }
    [[PHPhotoLibrary sharedPhotoLibrary] performChanges:^{
        PHAssetCollectionChangeRequest *request = [PHAssetCollectionChangeRequest changeRequestForAssetCollection:Collection];
        [request insertAssets:Assets atIndexes:[NSIndexSet indexSetWithIndex:0]];
    } completionHandler:^(BOOL success, NSError * _Nullable error) {
        if(nil != error || !success){
            state = ERROR;
        }
        else{
            state = FINISH;
        }
        dispatch_sync(dispatch_get_main_queue(), ^{
            if(state == ERROR){
                showAlart(false);
            }
            if(state == FINISH){
                showAlart(true);
            }
            running = false;
        });
        running = false;
    }];
}


