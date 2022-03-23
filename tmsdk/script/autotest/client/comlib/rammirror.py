# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace

from comlib import SVNManager,SVNState,Path,JsonFile

from comlib.conf.loader import Loader
from comlib.conf.ref import *
# 打包脚本调用rollback进行清理

# 打包脚本拉取资源

# 1 输入想放入内存的文件夹
# 2 更新源地址工作拷贝
# 3 根据上次到现在的版本号区间获取svn更变记录
# 4 根据更变记录拷贝文件到内存中
# 5 创建文件夹软连接
# 打包脚本打包

# 打包脚本调用rollback进行回滚
ramdiskcreatedir = com.gethomepath()
class RamMirror(object):
    def __init__(self,*needputdirsargs):
        super().__init__()
        self.needputdirsargs = needputdirsargs
        self.ramsize_gb = 30 # 20G
        self.ramsize_byte = self.ramsize_gb*1024*1024*1024
        # self.ramsize = 64*1024*1024 # 20G
        self.enableMem_gb = com.get_physical_memory_GB()
        self.sectorscount = int(self.ramsize_byte / 512) # 扇区数
        self.attachdir = os.path.join(ramdiskcreatedir,'ramdisk')
        # self.attachdir = r'G:\WXxiaoyouxi\ramdisk'
        self.svnversionfile = os.path.join(self.attachdir,'versionref.json')
        self.checkfile = os.path.join(self.attachdir,'check.json')
        self.diskname = 'ramdisk'
    def getMirrorPath(self,sourcePath):
        return os.path.join(self.attachdir,sourcePath)
    def getBakPath(self,sourcePath):
        return sourcePath+'_bak'

    def init_mirror(self):
        self.needputdirs= [] 
        for putdir in self.needputdirsargs:
            dst = os.path.join(self.attachdir,os.path.basename(putdir))
            remoteurl = putdir
            putdirversion = -1
            if SVNManager.isInversion(putdir):
                remoteurl = SVNManager.get_abs_url(putdir)
                putdirversion = SVNManager.version(putdir)
            self.needputdirs.append((putdir,remoteurl,putdirversion,dst))

        self.branchurl = self.needputdirs[0][1]
        self.branchdesc = Loader.getbranchdesc_url2chinese(self.branchurl)



    def isdisk(self,path):
        flagpath = os.path.join(path,'.fseventsd')
        if os.path.exists(flagpath):
            return True
        return False
    def eject(self):
        com.cmd(f'hdiutil eject "{self.attachdir}"',getstdout=False,errException=StopException('内存盘卸载失败',locals()))

    def create_ramdisk(self):
        if self.ramsize_gb + 5 >= self.enableMem_gb:
            com.logout(f'可用内存{self.enableMem_gb}，危险的内存盘大小{self.ramsize_gb + 5}')
            return False

        devicenode,code = com.cmd(f'hdiutil attach -nomount ram://{self.sectorscount}',errException=StopException('创建内存盘失败',locals()))
        devicenode = devicenode.strip()
        com.cmd(f'newfs_hfs -v ramdisk {devicenode}',getstdout=False,errException=StopException('格式化内存盘失败',locals()))
        com.cmd(f'mount -o noatime -t hfs {devicenode} "{self.attachdir}"',getstdout=False,errException=StopException('挂载内存盘失败',locals()))
        return True
    def recopy(self):
        versionref = {}
        versionref['branchdesc'] = self.branchdesc
        for putdir,putdirurl,version,dst in self.needputdirs:
            versionref[putdirurl] = version
            Path.ensure_pathnewest(putdir,dst)

        com.dumpfile_json(versionref,self.svnversionfile)
    def recreate_ramdisk(self):
        self.eject()
        self.create_ramdisk()
        self.recopy()
    def init_ramdisk_env(self):
        Path.ensure_direxsits(self.attachdir)
        # 不存在则创建
        if not self.isdisk(self.attachdir):
            # 创建内存盘
            self.create_ramdisk()
        # 存在但是为空，复制一份
        if not os.path.exists(self.svnversionfile):
            # 重新拷贝一份
            self.recopy()
        # 存在内容不为空
        else:
            versionref = JsonFile(self.svnversionfile)
            branch = versionref.trygetvalue('branchdesc')
            # 但是分支不一样，需要eject重新生成内存盘再拷贝
            if branch != self.branchdesc:
                self.recreate_ramdisk()
            else:
                for putdir,putdirurl,version,dst in self.needputdirs:
                    # 路径更变 -1是因为多了branchdesc值
                    if not versionref.haskey(putdirurl) or versionref.getkeys().__len__() -1 != self.needputdirs.__len__():
                        self.recreate_ramdisk()
                        break
                    # 目标版本低于内存版本
                    elif version < versionref.trygetvalue(putdirurl):
                        self.recreate_ramdisk()
                        break
    def mirror(self):
        self.init_mirror()
        self.init_ramdisk_env()
        # 进行内容同步
        if os.path.exists(self.svnversionfile):
            versionref = JsonFile(self.svnversionfile)
            for putdir,putdirurl,version,dst in self.needputdirs:
                lastversion = versionref.trygetvalue(putdirurl)
                # lastversion必定不为None，因为前面环境初始化中当路径不存在会重新拷贝
                # 这里lastversion只会比version小或者等于
                if lastversion == version:
                    continue
                elif lastversion < version:
                    versionref.trysetvalue(putdirurl,value=version)
                    changelist = SVNManager.getchangelist(putdirurl,lastversion,version)
                    for stat in changelist:
                        stat:SVNState
                        diskpath =com.convertsep(stat.path.replace(putdirurl,putdir,1))
                        ramfilepath =com.convertsep(stat.path.replace(putdirurl,dst,1))
                        if stat.state == 'delete':
                            com.logout(f'[mirror] [delete] {ramfilepath}')
                            Path.ensure_pathnotexsits(ramfilepath)
                        elif stat.state in ('modify','add'):
                            # 只拷贝文件修改，文件夹状态属性修改无视
                            if stat.state == 'add' and os.path.isdir(diskpath):
                                continue
                            com.logout(f'[mirror] [copy] {ramfilepath}')
                            Path.ensure_pathnewest(diskpath,ramfilepath)
                        else:
                            raise StopException(f'出现未知状态{stat.state}',locals())
                    
                else:
                    raise StopException(f'lastversion={lastversion} < version={version}',locals())
            versionref.save()
        for putdir,putdirurl,version,dst in self.needputdirs:
            # 同步完成后
            # 重命名原文件夹
            os.rename(putdir,f'{putdir}_bak')
            # 创建内存文件夹软连接
            os.symlink(dst,putdir,True)

    def rollback(self):
        com.logout(f'[mirror] ROLLBACK!')
        for putdir in self.needputdirsargs:
            if os.path.exists(f'{putdir}_bak') and os.path.islink(putdir):
                Path.ensure_pathnotexsits(putdir)
                os.rename(f'{putdir}_bak',putdir)
        
    def mirror_simple(self):
        Path.ensure_direxsits(self.attachdir)
        # 不存在则创建
        if not self.isdisk(self.attachdir):
            # 创建内存盘
            if not self.create_ramdisk():
                return False

        checkdict = {}
        if os.path.exists(self.checkfile):
            checkdict = com.loadfile_json(self.checkfile)
        for path in self.needputdirsargs:
            if path not in checkdict or checkdict[path] == False:
                # 在内存盘内再用完整路径创建文件夹，保证不重名
                dst = os.path.join(self.attachdir,path.strip(os.path.sep))
                com.logout(f'[createpath] {path} -> {dst}')
                Path.ensure_pathnewest(path,dst)
                checkdict[path] = True
        com.dumpfile_json(checkdict,self.checkfile)

        
        for path in self.needputdirsargs:
            dst = os.path.join(self.attachdir,path.strip(os.path.sep))
            # 同步完成后
            # 重命名原文件夹
            os.rename(path,f'{path}_bak')
            # 创建内存文件夹软连接
            os.symlink(dst,path,True)

if __name__ == "__main__":
    # sys.argv = ['','run','/Users/hegu/remotedev/test/lebian','/Users/hegu/remotedev/test/androidbuilder']
    # sys.argv = ['','rollback','/Users/hegu/remotedev/test/lebian','/Users/hegu/remotedev/test/androidbuilder']
    # sys.argv = ['','rollback','~/remotedev/test/androidbuilder',
    # '~/remotedev/test/comlib']
    
    method = sys.argv[1]
    rm = RamMirror(*sys.argv[2::])
    if method == 'mirror':
        rm.mirror()
    elif method == 'rollback':
        rm.rollback()
    pass
