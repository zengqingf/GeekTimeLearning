# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))

from comlib import TMIPAManager,TMApkManager,HTTPManager

from comlib.comobj import *

class Resigner():
    def __init__(self) -> None:
        self.url = sys.argv[1]
        self.bunldeid = sys.argv[2]
        self.ftproot = sys.argv[3]

        if self.url.endswith('.apk'):
            self.plat = 'android'
        else:
            self.plat = 'ios'

        self.filepath = HTTPManager.download_http(self.url,'.')
        pass

    # deprecated !!!
    @errorcatch(NORMAL)
    @workspace
    def run(self):
        
        channel = sys.argv[4]
        configpath = os.path.join(workdir,'oldtool','config.json')
        config = com.loadfile_json(configpath)
        channelconfig = config['MainScript']['bundleid'][channel]
 
        if self.plat == 'ios':
            zipm = TMIPAManager(self.filepath)
            # bundleid = channelconfig['bundle']
            # codeSign = channelconfig['codeSign']
            # provision_path = os.path.expanduser('~')+'/Library/MobileDevice/Provisioning Profiles/%s.mobileprovision'%channelconfig['provision']
            bundleid = 'com.cheng.kdyzapp.hx'
            provision_path = '/Volumes/TM148/JenkinsRoot/workspace/DNF_Android_BB_Upload_148/_tool/keystore/_ios_appstore/channels/_tengmu/dev/Tengmu_20200429.mobileprovision'
            p12filepath = '/Volumes/TM148/JenkinsRoot/workspace/DNF_Android_BB_Upload_148/_tool/keystore/_ios_appstore/channels/_tengmu/dev/tengmuV1-20200420.p12'
            passfilepath = '/Volumes/TM148/JenkinsRoot/workspace/DNF_Android_BB_Upload_148/_tool/keystore/_ios_appstore/channels/_tengmu/dev/pwd.pass'
            signpath = zipm.sign(bundleid,provision_path,p12filepath,passfilepath)
            Log.info(signpath)
            # ftppath = com.get_ftp_savepath(plat,'debug',channel)
            # ftp147.upload(signpath,ftppath)
        else:
            raise Exception('apk还没写')

    def resign_a8_ios_ipa_ue4_win_build(self):
        
        zipm = TMIPAManager(self.filepath)
        provision_path = f'{workdir}/sec/key/ios/a8_debug_dev.mobileprovision'
        p12filepath = f'{workdir}/sec/key/ios/a8_debug_dev.p12'
        passfilepath = f'{workdir}/sec/key/ios/a8_debug_dev.pass'
        signpath = zipm.sign(self.bunldeid,provision_path,p12filepath,passfilepath)
        Log.info(signpath)
        if self.ftproot:
            pkgname = os.path.basename(self.filepath)
            G_ftp.upload(signpath, f'{self.ftproot}/{pkgname}', overwrite=True)
@workspace
def main():
    resigner = Resigner()
    #resigner.run()
    resigner.resign_a8_ios_ipa_ue4_win_build()
    

if __name__ == "__main__":
    main()
    pass

