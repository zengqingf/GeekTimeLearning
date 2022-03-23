# -*- encoding: utf-8 -*-
import sys,os
sys.path.append(os.path.abspath(os.path.join(__file__,'..','..')))
from comlib.exception import errorcatch,add_exitfunc,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com

import pathlib,time
# basedir = os.path.abspath(os.path.dirname(__file__))
basedir = os.path.dirname(__file__)
bindir = os.path.join(basedir,'bin')
binconfdir = os.path.join(bindir,'conf')
multiplatdir = os.path.join(bindir,'multiplat')
curplatdir = os.path.join(bindir,com.getvalue4plat('win','linux','mac'))


def make_runable(filepath):
    st_mode = os.stat(filepath)[0]

    os.chmod(filepath,st_mode | 0b1001001) # +x       -rw------- => -rwx--x--x

def find(binfile):
    
    if '.' not in binfile and sys.platform == 'win32':
        binfile += '.exe'
    if binfile in os.listdir(curplatdir):
        curpath = os.path.join(curplatdir,binfile)
        make_runable(curpath)
        return curpath
    else:
        raise StopException(f'[bin] -{binfile}- not find',locals(),'None')
    # return None
    
def find_multi(binfile):
    if binfile in os.listdir(multiplatdir):
        curpath = os.path.join(multiplatdir,binfile)
        make_runable(curpath)
        return curpath
    else:
        raise StopException(f'[bin] multiplat -{binfile}- not find',locals(),'None')
    # return None

class BinManager(object):
    @staticmethod
    def get_conf_dir(binfile):
        binfile = os.path.splitext(binfile)[0]
        if binfile in os.listdir(binconfdir):
            return os.path.join(binconfdir,binfile)
        else:
            return None
    @staticmethod
    def IIFSPackager(cmdparams,**cmdkv):
        if sys.platform == 'win32':
            exePath = find('Packager.com')
        else:
            exePath = find('Packager')
        out,code = com.cmd(f'{exePath} {cmdparams}',**cmdkv)

        return out,code
    @staticmethod
    def buglysymupload(cmdparames,usePopen=False,**cmdkv):
        path = find_multi('buglySymbolAndroid.jar')
        if usePopen:
            out = com.cmd_subp(f'java -jar {path} {cmdparames}',**cmdkv)
            code = 0
        else:
            out,code = com.cmd(f'java -jar {path} {cmdparames}',**cmdkv)
        return out,code
    @staticmethod
    def JenkinsCli(cliname,cmdparames,usePopen=False,**cmdkv):
        path = find_multi(cliname)
        if usePopen:
            out = com.cmd_subp(f'java -jar {path} {cmdparames}',encoding='utf-8',**cmdkv)
            code = 0
        else:
            out,code = com.cmd(f'java -jar {path} {cmdparames}',encoding='utf-8',**cmdkv)
        return out,code
    @staticmethod
    def clash():
        clashpath = find('clash')
        confpath = BinManager.get_conf_dir('clash')
        sub,code = com.cmd(f'{clashpath} -d {confpath}',getPopen=True)
        # add_exitfunc(lambda : sub.terminate())
        # 等3s启动
        time.sleep(3)
        return sub,code
    @staticmethod
    def ndkstack(cmdparames,**cmdkv):
        ndkstackpath = find('ndk-stack')
        out,code = com.cmd(f'{ndkstackpath} {cmdparames}',**cmdkv)
        # if 'getPopen' in cmdkv.keys():
        #     add_exitfunc(lambda : out.terminate())
        return out,code
    @staticmethod
    def baidupcs_go(cmdparames,**cmdkv):
        BaiduPCSpath = find('BaiduPCS')
        return com.cmd(f'{BaiduPCSpath} {cmdparames}',**cmdkv)
    @staticmethod
    def aapt(cmdparames,**cmdkv):
        '''
        cmdkv 是 com.cmd的kv
        '''
        aaptpath = find('aapt')
        return com.cmd(f'{aaptpath} {cmdparames}',encoding='utf-8',**cmdkv)
    @staticmethod
    def zipalign(cmdparames,**cmdkv):
        '''
        cmdkv 是 com.cmd的kv
        '''
        zipalignpath = find('zipalign')
        return com.cmd(f'{zipalignpath} {cmdparames}',**cmdkv)
    @staticmethod
    def apksigner(cmdparames,**cmdkv):
        '''
        cmdkv 是 com.cmd的kv
        '''
        apksignerfile = find_multi('apksigner.jar')
        # 给予默认 1G 堆内存 1M 栈内存，见 apksigner.bat 源码
        return com.cmd(f'java -Xmx1024M -Xss1m -jar {apksignerfile} {cmdparames}',**cmdkv)
    @staticmethod
    def apktool(cmdparames,**cmdkv):
        path = find_multi('apktool.jar')
        return com.cmd(f'java -jar -Duser.language=en -Dfile.encoding=UTF8 {path} {cmdparames}',**cmdkv)
    @staticmethod
    def jenkins10(cmdparames,**cmdkv):
        '''
        cmdkv 是 com.cmd的kv
        '''
        jenkins10path = find_multi('jenkins10.jar')
        return com.cmd(f'java -jar {jenkins10path} {cmdparames}',**cmdkv)
    @staticmethod
    def bang10(cmdparames,**cmdkv):
        '''
        cmdkv 是 com.cmd的kv
        '''
        bangbangpath = find_multi('bang10.jar')
        return com.cmd(f'java -jar {bangbangpath} {cmdparames}',**cmdkv)
    @staticmethod
    def bang20(cmdparames,**cmdkv):
        '''
        cmdkv 是 com.cmd的kv
        '''
        bangbangpath = find_multi('bang20.jar')
        bangproppath = find_multi('bang20_log4j.properties')
        return com.cmd(f'java -jar -Dlog4j.configuration=file:"{bangproppath}" {bangbangpath} {cmdparames}',**cmdkv)
    @staticmethod
    def kill_all_unity(**cmdkv):
        '''
        执行命令 pkill Unity
        '''
        # path = find('kill_all_unity.sh')
        # return com.cmd(f'{path}',**cmdkv)
        return com.cmd(f'pkill Unity')
    @staticmethod
    def ios_resigner(IPA_PATH,BUNDLE_ID,PROVISION_PATH,P12_PATH,PASSWORD,**kv):
        '''
        只能在mac上运行
        '''
        filepath = find('signer_ios.sh')
        return com.cmd(f'sh -e "{filepath}" "{IPA_PATH}" "{BUNDLE_ID}" "{PROVISION_PATH}" "{P12_PATH}" "{PASSWORD}"',**kv)
    @staticmethod
    def importp12(P12FILE,P12PASSWORD,MOBILEPROVISIONFILE,LOGINPASSWORD,**kv):
        '''
        只能在mac上运行
        '''
        filepath = find('importp12.sh')
        return com.cmd(f'sh -e "{filepath}" "{P12FILE}" "{P12PASSWORD}" "{MOBILEPROVISIONFILE}" "{LOGINPASSWORD}"',**kv)

if __name__ == "__main__":
    # confpath = get_conf_dir('clash')
    # print(confpath)
    # BinManager.aapt('-h',getstdout=False)
    # BinManager.jenkins10('-h',getstdout=False)
    # BinManager.bang10('-h',getstdout=False)
    # BinManager.bang20('--help',getstdout=False)
    pass