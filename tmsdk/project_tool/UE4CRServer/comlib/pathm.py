# -*- encoding: utf-8 -*-
import sys,os
sys.path.append(os.path.abspath(os.path.join(__file__,'..','..')))


import shutil
# from comlib.ftpm import FTPManager
from comlib.exception import errorcatch,LOW,NORMAL,HIGH
from comlib.svnm import SVNManager
from comlib import com

@errorcatch(HIGH)
class Path(object):
    @staticmethod
    def ensure_pathexsits_nocopy(dst):
        if not os.path.exists(dst):
            raise Exception(f'{dst} 目标不存在')
        
    
    @staticmethod
    def ensure_pathexsits(src,dst,ignore=None):
        '''
        对文件操作必须指定dst的文件名，不能使用文件夹名
        '''
        dstdirname = os.path.dirname(dst)
        if not os.path.exists(dstdirname):
            os.makedirs(dstdirname)
        
        if os.path.isdir(src):
            shutil.copytree(src,dst,ignore=ignore)
        else:
            shutil.copy2(src,dst)
    @staticmethod
    def ensure_pathnewest(src,dst,ignore=None):
        Path.ensure_pathnotexsits(dst)
        Path.ensure_pathexsits(src,dst,ignore=ignore)
    @staticmethod
    def ensure_dirnewest(dst):
        Path.ensure_pathnotexsits(dst)
        os.makedirs(dst)
    @staticmethod
    def ensure_direxsits(dst):
        if not os.path.lexists(dst):
            os.makedirs(dst)
    @staticmethod
    def ensure_pathnotexsits(*dsts):
        for dst in dsts:
            if os.path.lexists(dst):
                if os.path.islink(dst):
                    os.remove(dst)
                elif os.path.isdir(dst):
                    shutil.rmtree(dst)
                else:
                    os.remove(dst)
    @staticmethod
    def ensure_svn_pathexsits(dst,svnurl,checkout_keep=None,version='HEAD'):
        # TODO:分支不同则切换分支
        if not os.path.lexists(dst) or not os.path.exists(os.path.join(dst,'.svn')):
            Path.ensure_direxsits(dst)
            SVNManager.check_out_keep(svnurl,dst,keep=checkout_keep)
        else:
            curRevision = SVNManager.version(dst)
            if version == 'HEAD':
                SVNManager.update_safe(dst,version=version)
            elif int(version) < int(curRevision):
                SVNManager.rollback(dst,curRevision,version)
            else:
                SVNManager.update_safe(dst,version=version)
    @staticmethod
    def getPath_Roaming():
        '''
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)\n
        windows:~\AppData\Roaming\n
        mac:~/.config\n
        linux:~/.config\n
        '''
        userhome = com.gethomepath()
        return com.getvalue4plat(os.path.join(userhome,'AppData\\Roaming'),os.path.join(userhome,'.config'))

    @staticmethod
    def getPath_LocalData():
        '''
        windows:~\AppData\Local\n
        mac:~/Library/Application Support\n
        linux:???\n
        '''
        userhome = com.gethomepath()
        return com.getvalue4plat(os.path.join(userhome,'AppData\\Local'),os.path.join(userhome,'Library/Application Support'))
    @staticmethod
    def getPath_Desktop():
        userhome = com.gethomepath()
        return os.path.join(userhome,'Desktop')

		# public static DirectoryReference GetUserSettingDirectory()
		# {
		# 	if (BuildHostPlatform.Current.Platform == UnrealTargetPlatform.Mac)
		# 	{
		# 		return new DirectoryReference(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library", "Application Support", "Epic"));
		# 	}
		# 	else if (Environment.OSVersion.Platform == PlatformID.Unix)
		# 	{
		# 		return new DirectoryReference(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Epic"));
		# 	}
		# 	else
		# 	{
		# 		// Not all user accounts have a local application data directory (eg. SYSTEM, used by Jenkins for builds).
		# 		string DirectoryName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
		# 		if(String.IsNullOrEmpty(DirectoryName))
		# 		{
		# 			return null;
		# 		}
		# 		else
		# 		{
		# 			return new DirectoryReference(DirectoryName);
		# 		}
		# 	}
		# }