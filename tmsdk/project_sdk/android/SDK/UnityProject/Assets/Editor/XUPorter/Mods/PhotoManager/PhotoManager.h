#import <Foundation/Foundation.h>
#import <Photos/PHFetchResult.h>
#import <Photos/PHAsset.h>
#import <Photos/PHCollection.h>

#ifdef __cplusplus
extern "C"{
#endif
    void _InitAlartText(char *title,char *message,char *btnText);
    void _SaveImage2Album(uint8_t *bytes,int length,char *suc,char *fail);
    void *_SavePhoto(void *data);
    void showAlart(bool isSuc);
    void createdAssets();
    void createdCollection();
    void saveImageIntoAlbum();
#ifdef __cplusplus
} // extern "C"
#endif
