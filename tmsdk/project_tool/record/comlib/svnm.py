# -*- coding:utf-8 -*-
# -*- encoding: utf-8 -*-
import sys,os

thisdir = os.path.abspath(os.path.dirname(__file__))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH

sys.path.append(os.path.abspath(os.path.join(__file__,'..','..')))


from comlib import com
from comlib.logm import Log
from comlib.xmlm import XMLFile
from comlib.dictm import DictUtil
from comlib.conf.loader import Loader
from comlib.conf.ref import *

from comlib.exception import errorcatch,LOW,NORMAL,HIGH

from typing import List
from xml.etree.ElementTree import Element


ln = 'SVN'


class SVNState(object):
    def __init__(self,stateStr:str):
        super().__init__()
        infoList = list(filter(lambda x: x != '',stateStr[1::].split(' ')))
        if stateStr[0] == '?':
            self.state = 'noVersion'
        elif stateStr[0] == 'M':
            self.state = 'modify'
        elif stateStr[0] == 'A':
            self.state = 'add'
        elif stateStr[0] == 'D':
            self.state = 'delete'
        else:
            self.state = 'nothing'
        if self.state != 'noVersion':    
            if com.isnumeric(infoList[0]):
                # 带版本号和提交人的状态
                self.p1 = infoList[0]
                self.p2 = infoList[1]
                self.who = infoList[2]
                self._parsepath(infoList[3::])
            else:
                # 只有路径的状态
                self._parsepath(infoList)
        else:
            self._parsepath(infoList)
            
        self.isPathContainSpace = False
        if ' ' in self.path:
            self.isPathContainSpace = True
    def _parsepath(self,pathstrList:list):
        tmp = ' '.join(pathstrList)
        if tmp.startswith('http://'):
            self.path = com.urldecode(tmp)
        else:
            self.path = tmp
class SVNState_Local(object):
    def __init__(self,xmlf:XMLFile,entry:Element) -> None:
        self.path = entry.attrib['path']

        wcstatus = xmlf.find(entry,'wc-status')
        
        # normal unversioned modified external missing deleted conflicted
        self.item = wcstatus.attrib['item']
        self.revision_wc = DictUtil.tryGetValue(wcstatus.attrib,'revision')
        self.revision_remote = None
        self.author = None

        commitE = xmlf.find(wcstatus,'commit')
        if commitE != None:
            revision_remote = DictUtil.tryGetValue(commitE.attrib,'revision')
            authorE = xmlf.find(commitE,'author')
            if authorE != None:
                self.author = authorE.text
        
class LogCommitState():
    def __init__(self,pathentry:Element,parent) -> None:
        self.parent:SVNLog = parent
        self.path = pathentry.text # /branches/RELEASE_version_ANDROID_CORE_BANQUAN_2020/Program/Client/Assets/Resources/a.txt
        shortac = pathentry.attrib['action']
        if shortac == 'A':
            self.action = 'added'
        elif shortac == 'M':
            self.action = 'modified'
        elif shortac == '?':
            self.action = 'unversioned'
        elif shortac == 'D':
            self.action = 'deleted'
        elif shortac == '!':
            self.action = 'missing'
        elif shortac == 'M':
            self.action = 'modified'
    def getlocalpath(self,svnUnity工程根目录,本地unity工程根路径):
        relpath = SVNManager.svnurl2relpath(svnUnity工程根目录,containFirstSep=True)
        return self.path.replace(relpath,本地unity工程根路径,1)

class SVNLog():
    def __init__(self,xmlf:XMLFile,logentry:Element) -> None:
        self.revision = int(logentry.attrib['revision'])
        authorE = xmlf.find(logentry,'author')
        self.author = authorE.text
        pathsE = xmlf.find(logentry,'paths')
        pathEs = xmlf.findall(pathsE,'path')
        
        self.commitStates:List[LogCommitState] = []
        for pathE in pathEs:
            self.commitStates.append(LogCommitState(pathE,self))

        self.msg = xmlf.find(logentry,'msg').text

pwdFilePath = os.path.join(com.gethomepath(),'Documents','sec','pwd_sync.txt')
Log.debug(f"svn pwd file path {pwdFilePath}",ln)
svnusername,svnpwd = com.readall(pwdFilePath).splitlines()

@errorcatch(HIGH)
class SVNManager:
    '''
    The SVN Manager
    '''
    username = svnusername
    password = svnpwd
    # username = "jenkins"
    # password = "123456"

    # username = Loader.load(scmconf,use_defalt=True).username
    # password = Loader.load(scmconf,use_defalt=True).password
    @staticmethod
    def cat(path):
        return SVNManager._base_cmd(f'cat {path}')
    @staticmethod
    def add(path):
        SVNManager._base_cmd(f'add -q --force {path}')
    @staticmethod
    def setExecutable(*paths):
        for path in paths:
            SVNManager._base_cmd(f'propset svn:executable on {path}')
    @staticmethod
    def list(path,recursive=False)->List[str]:
        cmd = f'list {path}'
        if recursive:
            cmd += ' -R'
        out = SVNManager._base_cmd(cmd)
        files = out.splitlines()
        return files

    @staticmethod
    def export(svnurl,path):
        SVNManager._base_cmd(f'export --force "{svnurl}" "{path}"')
    @staticmethod
    def export_ex(localpath,savepath):
        """
        根据本地路径下载svn上的文件到指定路径
        """
        svnurl = SVNManager.get_abs_url(localpath)
        SVNManager.export(svnurl,savepath)

    @staticmethod
    def svnurl2relpath(svnurl,containFirstSep=False):
        relpath = '/'.join(svnurl.split('/')[5::])
        if containFirstSep:
            relpath = '/' + relpath
        return relpath
    @staticmethod
    def getlog(lastRevision,curRevision,url,containLastRivision=True) ->List[SVNLog]:
        '''
        包含curRevision,lastRevision通过开关包含 \n
        <log>
            ...
            <logentry revision="1598">
                <author>yangduqi</author>
                <date>2020-10-22T08:32:21.852200Z</date>
                <paths>
                    <path action="A" prop-mods="false" text-mods="true" kind="file">/DevOps_Scripts/pyscripts/autotest_server/a.txt</path>
                    <path action="M" prop-mods="false" text-mods="true" kind="file">/DevOps_Scripts/pyscripts/autotest_server/main.py</path>
                </paths>
                <msg>cltest</msg>
            </logentry>
        </log>
        '''
        if not containLastRivision:
            lastRevision = int(lastRevision) + 1
        if lastRevision > curRevision:
            lastRevision = curRevision
        xmlstr = SVNManager._base_cmd(f'log -r {lastRevision}:{curRevision} -v --xml {url}',encoding='utf-8')
        xmlf = XMLFile(xmlstr)
        logs = []
        logentrys = xmlf.findall(xmlf.root,'logentry')
        for logentry in logentrys:
            logs.append(SVNLog(xmlf,logentry))
        return logs
    @staticmethod
    def changelist(name,svnworkspace,*path):
        pathstr = com.safepath(*path)
        SVNManager._base_cmd(f'changelist -R {name} {pathstr}',cwd=svnworkspace)
        return name
    @staticmethod
    def getStates(path,depth='infinity',ext='') ->List[SVNState_Local]:
        states:List[SVNState_Local] = []

        statusstr = SVNManager._base_cmd(f'status {ext} --depth={depth} --xml "{path}"')
        statusxml = XMLFile(statusstr)
        target = statusxml.find(statusxml.root,'target')
        entrys = statusxml.findall(target,'entry')
        for entry in entrys:
            states.append(SVNState_Local(statusxml,entry))
        return states
    @staticmethod
    def hasConflictedFile(path):
        states = SVNManager.getStates(path)
        for state in states:
            if state.item == 'conflicted':
                return True

        return False


    @staticmethod
    def _base_cmd(usercmd,errException=Exception("SVN操作出错"),showSVNCmdLog=True,**kv):
        if showSVNCmdLog:
            Log.info(f'{usercmd}',ln)
        cmd = ' '.join(["svn",
            "--username \"%s\"" % SVNManager.username,
            "--password \"%s\"" % SVNManager.password,
            "--no-auth-cache",
            "--trust-server-cert-failures=\"unknown-ca,cn-mismatch,expired,not-yet-valid,other\"",
            # "--trust-server-cert",
            "--non-interactive",
            "%s" % usercmd])
        # ret = CMDRunner.output_for(cmd, "")

        ret,code = com.cmd(cmd,errException=errException,showlog=False,**kv)
        
        # try:
        #     ret = ret.decode('utf8').encode('utf8')
        # except Exception as e:
        #     ret = str(e) 

        # Logging.info("----------------------->svn<------------------------\n%s" % (ret))
        
        return ret.strip()
    @staticmethod
    def run(cmd,**kv):
        Log.info(f'{cmd}',ln)
        # com.logout(f'[svn] {cmd}')
        cmd = f'svn --username {SVNManager.username} --password {SVNManager.password} --no-auth-cache --trust-server-cert-failures=\"unknown-ca,cn-mismatch,expired,not-yet-valid,other\" --non-interactive {cmd}'
        out,code = com.cmd(cmd,showlog=False,**kv)
        
        return out,code
    @staticmethod
    def isExsits(path):
        out,code = SVNManager.run(f'info {path}')
        if code != 0:
            return False
        return True

    @staticmethod
    def version(path):
        cmd = f'info --show-item last-changed-revision "{path}"'
        return SVNManager._base_cmd(cmd).strip()
    @staticmethod
    def version_repo(path):
        cmd = f'info --show-item revision "{path}"'
        return SVNManager._base_cmd(cmd).strip()
    @staticmethod
    def version_top(path):
        logcmd = f'log -l 1 --xml "{path}"'
        xmldata = SVNManager._base_cmd(logcmd,encoding='utf-8')
        xmlf = XMLFile(xmldata)
        logentry = xmlf.find(xmlf.root,'logentry')
        revision = int(logentry.attrib['revision'])

        return revision
        # cmd = ' '.join(["info --show-item last-changed-revision",
        #     "\"%s\"" % path])
        # return SVNManager._base_cmd(cmd).strip()
    
    @staticmethod
    def author(path):
        cmd = ' '.join(["info --show-item last-changed-author",
            "\"%s\"" % path])
        return SVNManager._base_cmd(cmd).strip()

    @staticmethod
    def check_out(url, path,revision='HEAD'):
        # cmd = ' '.join(["checkout",
        #     "\"%s\"" % url,
        #     "\"%s\"" % path])

        cmd = f'checkout -r {revision} "{url}" "{path}"'
        SVNManager._base_cmd(cmd)

    @staticmethod
    def check_out_depathfile(url, path, file="files"):
        cmd = ' '.join(["co",
            "\"%s\"" % url,
            "\"%s\"" % path,
            "--depth \"%s\" " % file])
        SVNManager._base_cmd(cmd)
    @staticmethod
    def check_out_keep(url,path,keep=None,revision='HEAD'):
        if keep == None:
            SVNManager.check_out(url,path,revision=revision)
            return
        from comlib.pathm import Path
        Path.ensure_dirnewest(path)
        # 需要重新update --set-depth infinity的文件夹
        needreset = []
        SVNManager._base_cmd(f'checkout -r {revision} "{url}" "{path}" --depth immediates')
        # filter(lambda x: o com.listdir_fullpath(path))
        needreset += com.listdir_fullpath(path,
        filterfunc=lambda x: os.path.isdir(x) and not x.endswith('.svn'))
        # 整合相同路径
        # 根据深度从深到浅进行排序
        keep.sort(key=lambda x: x[0].count(os.path.sep),reverse=True)
        # 依次拉取后排除
        def checkoutFolderTree(tree):
            nonlocal needreset
            treesplit = tree.split(os.path.sep)
            curpath = path
            cururl = url
            for index in range(0,treesplit.__len__()):
                sp = treesplit[index]
                curpath = os.path.join(curpath,sp)
                if os.listdir(curpath).__len__() == 0:
                    cururl = '/'.join([cururl,sp])
                    SVNManager._base_cmd(f'checkout -r {revision} "{cururl}" "{curpath}" --depth immediates')
                    # 最后一层要排除，所以不能这里加
                    # if sp != treesplit[-1]:
                    # 排除路径上除了排除路径树全要设置infinity
                    needreset += com.listdir_fullpath(curpath,
                    filterfunc=lambda x: os.path.isdir(x))
            # 移除
            tmpp = path
            for sp in treesplit:
                tmpp = os.path.abspath(os.path.join(tmpp,sp))
                if tmpp in needreset:
                    needreset.remove(tmpp)
            pass
        for dirname_rel,basenames in keep:
            dirname_abs = os.path.join(path,dirname_rel)

            checkoutFolderTree(dirname_rel)
            excludes = list(map(lambda x: os.path.join(dirname_abs,x),
            filter(lambda x: x not in basenames and not x.endswith('.svn'),os.listdir(dirname_abs))))
            # 最后一层这里加
            for exclude in excludes:
                if os.path.isdir(exclude) and exclude in needreset:
                    needreset.remove(exclude)

            SVNManager.update_exclude(dirname_abs,ignore=' '.join(excludes))
            excludenames = list(filter(lambda x: x not in basenames,os.listdir(dirname_abs)))

            pass
        needreset.sort(key=lambda x: x.count(os.path.sep),reverse=True)
        for p in needreset:
            SVNManager.update(p,version=revision,setDepth='infinity',showlog=False)

    @staticmethod
    def copy(frompath,topath,version,message):
        cmd = f'copy "{frompath}" "{topath}" -r {version} -m "{message}"'
        SVNManager._base_cmd(cmd)
    @staticmethod
    def remove(path,message):
        cmd = f'remove "{path}" -m "{message}"'
        SVNManager._base_cmd(cmd)
    @staticmethod
    def merge(frompath,topath,versions,acceptOpt='p'):
        isRange = False
        if ':' in versions:
            isRange = True
        if isRange:
            SVNManager._base_cmd(f'merge {frompath} -r {versions} {topath} --accept {acceptOpt}',errException=StopException('svn merge失败',locals()))
        else:
            SVNManager._base_cmd(f'merge {frompath} -c {versions} {topath} --accept {acceptOpt}',errException=StopException('svn merge失败',locals()))

    @staticmethod
    def delete(path):
        cmd = f'delete --force -q "{path}"'
        SVNManager._base_cmd(cmd,showSVNCmdLog=False)

    @staticmethod
    def commit(filepath, commen, deleteMissing=False):
        if deleteMissing:
            states = SVNManager.getStates(filepath)
            for state in states:
                if state.item == 'missing':
                    SVNManager.delete(state.path)

        cmd = ' '.join(["commit",
            "\"%s\"" % filepath,
            "-m \"%s\"" % commen])
        SVNManager._base_cmd(cmd)
    @staticmethod
    def commit_changelist(name,msg,svnworkdir):
        SVNManager._base_cmd(f'commit -m "{msg}" --cl {name}',cwd=svnworkdir)

    @staticmethod
    def revert(path,showlog=False):
        cmd = ' '.join(["revert",
            "-R",
            "\"%s\"" % path])
        if not showlog:
            cmd = com.getvalue4plat(cmd + ' 1>nul 2>nul',cmd + ' >/dev/null 2>&1')
            
        ret = SVNManager._base_cmd(cmd)
        if showlog:
            # print(ret)
            Log.debug(ret,ln)

    @staticmethod
    def clean_up(path):
        if os.path.isfile(path):
            path = os.path.dirname(path)

        cmd = ' '.join(["cleanup",
            "\"%s\"" % path])
        SVNManager._base_cmd(cmd)

    @staticmethod
    def clean_up_with_unversion(path):
        if os.path.isfile(path):
            path = os.path.dirname(path)

        cmd = ' '.join(["cleanup",
            "\"%s\" --remove-unversioned" % path])
        SVNManager._base_cmd(cmd)

    @staticmethod
    def update_exclude(path, ignore=None, version="HEAD"):
        if not ignore:
            ignore = "infinity"
        else:
            ignore = "exclude " + ignore

        backup = os.getcwd()

        os.chdir(path)
        Log.info("cur path " + os.getcwd(),ln)

        cmd = ' '.join([
            "update",
            "--set-depth " + ignore
            ])
        SVNManager._base_cmd(cmd)

        os.chdir(backup)
        Log.info("cur path " + os.getcwd(),ln)

    @staticmethod
    def update(path, ignore=None, version="HEAD",showlog=False,depth='infinity',setDepth=None):

        # SVNManager.clean_up(path)

        #if not ignore:
        #    ignore = "infinity"run_cmd
        #else:
        #    ignore = "exclude " + ignore

        #cmd = ' '.join([
        #    "update",
        #    "--accept tf",
        #    "--set-depth " + ignore
        #    ])
        #SVNManager._base_cmd(cmd)

        # cmd = ' '.join([
        #     "update",
        #     "-r \"%s\"" % version,
        #     "--depth %s"%depth,
        #     "--accept tf"
        #     ])
        cmd = f'update -r "{version}" --accept tf --force'
        if setDepth != None:
            cmd += f' --set-depth {setDepth}'
        else:
            cmd += f' --depth {depth}'
        cmd += f' "{path}"'
        if not showlog:
            cmd += ' -q'
            # cmd = com.getvalue4plat(cmd + ' 1>nul 2>nul',cmd + ' >/dev/null 2>&1')
        # print(cmd)
        SVNManager._base_cmd(cmd,errException=StopException('svn更新出错',{}))
    @staticmethod
    def upgrade_ALLLLL(*paths):
        for path in paths:
            SVNManager.upgrade(path)
    @staticmethod
    def update_safe(path,version='HEAD',removeUnversioned=False):
        if removeUnversioned:
            SVNManager.clean_up_with_unversion(path)
        else:
            SVNManager.clean_up(path) #去锁
        SVNManager.revert(path) #去除本地修改
        SVNManager.update(path,version=version) #更新
        SVNManager.revert(path) #去除冲突
    @staticmethod
    def rollback(path,fromRevision,toRevision):
        SVNManager.clean_up(path)
        SVNManager.revert(path)
        SVNManager._base_cmd(f'merge -r {fromRevision}:{toRevision} "{path}" "{path}"')
    @staticmethod
    def get_branch_Desc(path):
        out = SVNManager.get_rel_url(path)
        tmp = out.split('/')
        return '-'.join(tmp[1:3]) # ^/branches/RELEASE_version_PRE_merge/Program/Client
    @staticmethod
    def get_root(path):
        cmd = f'info --show-item wc-root {path}'
        out = SVNManager._base_cmd(cmd,getstdout=True,errException=Exception("SVN获取工作根目录失败"))

        return out.strip()
    @staticmethod
    def get_repoRoot(path):
        cmd = f'info --show-item repos-root-url {path}'
        out = SVNManager._base_cmd(cmd,getstdout=True,errException=Exception("SVN获取仓库根目录失败"))
        return out.strip()
    @staticmethod
    def get_abs_url(path):
        cmd = f'info --show-item url {path}'
        out = SVNManager._base_cmd(cmd,getstdout=True,errException=Exception("SVN获取绝对URL失败"))
        return out.strip()
    @staticmethod
    def get_rel_url(path):
        cmd = f'info --show-item relative-url {path}'
        out = SVNManager._base_cmd(cmd,getstdout=True,errException=Exception("SVN获取相对URL失败"))
        return out.strip().replace('^/','',1)
    @staticmethod
    def mkdir(path,msg=None):
        if msg == None:
            msg = f'[机器人] 创建文件夹{path}'
        cmd = f'mkdir --parents -m "{msg}" "{path}"'
        return SVNManager._base_cmd(cmd,errException=Exception(f'创建{path}文件夹失败'))

    @staticmethod
    def isInversion(path):
        '''
        注意：本地add了未提交也算在版本控制中
        '''
        cmd = f'info {path}'
        out,code = SVNManager.run(cmd,logfile=com.getNullfile(),errException=None)
        if code == 0:
            return True
        return False
    @staticmethod
    def upgrade(path):
        if os.path.isfile(path):
            path = os.path.dirname(path)
        subp,code = com.cmd(f'svn upgrade {path}',getPopen=True)
        out,err = subp.communicate()
        subp.wait()
        if subp.poll() != 0:
            import re
            Log.warning(err,ln)
            m = re.search("the root is '(.*?)'",err)
            if m != None:
                root = m.group(1)
                out,code = com.cmd(f'svn upgrade {root}')
            else:
                m = re.search("Working copy database",err)
                if m == None:
                    raise StopException("SVN upgrade 失败",locals())
                else:
                    # 正常
                    pass
    @staticmethod
    def getchangelist(url,fromver,tover)->list:
        out = SVNManager._base_cmd(f'diff -r {fromver}:{tover} --summarize {url} 2>&1',getstdout=True)
        states = []
        datas = out.split(com.getcmddivide())
        for statestr in datas:
            states.append(SVNState(statestr))
        return states
        
    @staticmethod
    def isroot(path):
        root = SVNManager.get_root(path)
        if os.path.abspath(path) == os.path.abspath(root):
            return True
        return False

if __name__ == "__main__":
    SVNManager.update('woshishabi',showlog=False)