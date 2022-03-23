# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,errorcatch_func,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace
from comlib.conf.loader import Loader
from comlib.conf.ref import *

from comlib.wraps import instance,singleton

from comlib.xmlm import XMLFile
from typing import List
from xml.etree.ElementTree import Element
from comlib.dictm import DictUtil


class SCMState:
    def __init__(self) -> None:
        self.state = ''
        self.p1 = ''
        self.p2 = ''
        self.who = ''
        self.path = ''

        self.isPathContainSpace = False
    def _parsepath(self,pathstrList:list):
        tmp = ' '.join(pathstrList)
        if tmp.startswith('http://'):
            self.path = com.urldecode(tmp)
        else:
            self.path = tmp

class SVNState(SCMState):
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
class SCMState_Local:
    def __init__(self,xmlf:XMLFile,entry:Element) -> None:
        self.path = None
        self.item = None
        self.revision_wc = None
        self.revision_remote = None
        self.author = None
        
class SVNState_Local(object):
    def __init__(self,xmlf:XMLFile,entry:Element) -> None:
        self.path = entry.attrib['path']

        wcstatus = xmlf.find(entry,'wc-status')
        
        # normal unversioned modified external missing deleted
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
class GitState(SCMState):
    def __init__(self,stateStrInfo:str) -> None:
        # ?? 未纳入版本控制
        # M  暂存区和仓库存在差异
        #  M 工作区和暂存区存在差异
        # MM 暂存区和仓库存在差异，工作区和暂存区存在差异
        # A  添加入暂存区，工作区和暂存区无差异
        # AM 添加入暂存区，工作区和暂存区有差异
        # AD 添加入暂存区，工作区内被删除
        #  D 工作区内被删除
        # D  缓存区内被删除

        stateStrInfo = stateStrInfo.strip()
        workcloneflag = stateStrInfo[0]
        bufferflag = stateStrInfo[1]
        if workcloneflag == 'M' or bufferflag == 'M':
            self.state = 'modify'
        elif workcloneflag == 'A':
            self.state = 'add'
        elif workcloneflag == '?':
            self.state = 'noVersion'
        elif workcloneflag == 'D':
            self.state = 'delete'
        else:
            raise Exception(f'self.state未赋值 info   {stateStrInfo}')
        
        self.path = stateStrInfo[2::]

# class GitState_Local
        


class LogCommitState():
    def __init__(self,pathentry:Element) -> None:
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
        relpath = SCMManager().url2relpath(svnUnity工程根目录,containFirstSep=True)
        return self.path.replace(relpath,本地unity工程根路径,1)

class Log():
    def __init__(self,xmlf:XMLFile,logentry:Element) -> None:
        self.revision = int(logentry.attrib['revision'])
        authorE = xmlf.find(logentry,'author')
        self.author = authorE.text
        pathsE = xmlf.find(logentry,'paths')
        pathEs = xmlf.findall(pathsE,'path')
        
        self.commitStates:List[LogCommitState] = []
        for pathE in pathEs:
            self.commitStates.append(LogCommitState(pathE))

        self.msg = xmlf.find(logentry,'msg').text


class SCMData():
    def __init__(self,data:dict) -> None:
        self.url = data['url']
        self.scm = data['scm']
        if self.scm not in ('svn','git'):
            raise Exception(f'{self.scm}不支持')

class SCM():
    def __init__(self,username,password) -> None:
        self.username = username
        self.password = password
    def basecmd(self,cmd):
        pass
    def update(self,localpath,revision='HEAD',opt=''):
        pass
    def update_safe(self,localpath,revision='HEAD'):
        pass
    def checkout(self,data:SCMData,localpath,revision='HEAD',keep=None):
        pass
    def revert(self,localpath):
        pass
    def cleanup(self,localpath):
        pass
    def merge(self,localpath,fromRevision,toRevision):
        pass
    def rollback(self,localpath,fromRevision,toRevision):
        pass
    def cat(self,remotePath):
        pass
    def commit(self,localpath):
        pass
class SVN(SCM):
    def __init__(self,username,password) -> None:
        super().__init__(username,password)
    def basecmd(self, cmd,getstdout=True, errException=Exception(f'svn命令执行失败'),**kv):
        com.logout(f'[run] svn {cmd}')
        cmd = f'svn --username {self.username} --password {self.password} --no-auth-cache {cmd}'
        out,code = com.cmd(cmd,getstdout=getstdout,encoding='utf-8',errException=errException,showlog=False,**kv)
        return out.strip(),code
    def update(self, localpath, revision='HEAD', opt='',quiet=True):
        cmd = f'update -r {revision} "{localpath}" --accept tf --force {opt}'
        if quiet:
            cmd += ' -q'
        self.basecmd(cmd)
    def update_safe(self, localpath, revision='HEAD'):
        self.cleanup(localpath) # 去锁、删文件
        self.revert(localpath) #去除本地修改
        self.update(localpath,revision=revision)
        self.revert(localpath) #去除冲突


    def checkout(self, data: SCMData, localpath, revision='HEAD', keep=None):
        url = data.url
        def basecheckout(opt=''):
            self.basecmd(f'checkout -r {revision} "{url}" "{localpath}" {opt}')

        def update_exclude(abspath,*excludes):
            pathstr = com.safepath(*excludes)
            self.basecmd(f'update --set-depth exclude {pathstr}')


        if keep == None:
            basecheckout()
            return
        from comlib.pathm import Path
        Path.ensure_dirnewest(localpath)
        # 需要重新update --set-depth infinity的文件夹
        needreset = []
        basecheckout('--depth immediates')
        # filter(lambda x: o com.listdir_fullpath(path))
        needreset += com.listdir_fullpath(localpath,
        filterfunc=lambda x: os.path.isdir(x) and not x.endswith('.svn'))
        # 整合相同路径
        # 根据深度从深到浅进行排序
        keep.sort(key=lambda x: x[0].count(os.path.sep),reverse=True)
        # 依次拉取后排除
        def checkoutFolderTree(tree):
            nonlocal needreset
            treesplit = tree.split(os.path.sep)
            curpath = localpath
            cururl = url
            for index in range(0,treesplit.__len__()):
                sp = treesplit[index]
                curpath = os.path.join(curpath,sp)
                if os.listdir(curpath).__len__() == 0:
                    cururl = '/'.join([cururl,sp])
                    # SVNManager._base_cmd(f'checkout -r {revision} "{cururl}" "{curpath}" --depth immediates')
                    basecheckout('--depth immediates')

                    # 最后一层要排除，所以不能这里加
                    # if sp != treesplit[-1]:
                    # 排除路径上除了排除路径树全要设置infinity
                    needreset += com.listdir_fullpath(curpath,
                    filterfunc=lambda x: os.path.isdir(x))
            # 移除
            tmpp = localpath
            for sp in treesplit:
                tmpp = os.path.abspath(os.path.join(tmpp,sp))
                if tmpp in needreset:
                    needreset.remove(tmpp)
            pass
        for dirname_rel,basenames in keep:
            dirname_abs = os.path.join(localpath,dirname_rel)

            checkoutFolderTree(dirname_rel)
            excludes = list(map(lambda x: os.path.join(dirname_abs,x),
            filter(lambda x: x not in basenames and not x.endswith('.svn'),os.listdir(dirname_abs))))
            # 最后一层这里加
            for exclude in excludes:
                if os.path.isdir(exclude) and exclude in needreset:
                    needreset.remove(exclude)

            update_exclude(dirname_abs,*excludes)
            excludenames = list(filter(lambda x: x not in basenames,os.listdir(dirname_abs)))

            pass
        needreset.sort(key=lambda x: x.count(os.path.sep),reverse=True)
        for p in needreset:
            self.update(p,revision,opt='--set-depth infinity')

    def revert(self, localpath):
        self.basecmd(f'revert -R "{localpath}"')
    def cleanup(self, localpath):
        self.basecmd(f'cleanup "{localpath}" --remove-unversioned --include-externals')
    






    def add(self,localpath):
        self.basecmd(f'add -q {localpath}')
    
    def export(self,url,localpath):
        self.basecmd(f'export --force "{url}" "{localpath}"')
    
    def export_ex(self,localpath,savepath):
        """
        根据本地路径下载svn上的文件到指定路径
        """
        svnurl = self.get_abs_url(localpath)
        self.export(svnurl,savepath)


    def getlog(self,lastRevision,curRevision,url,containLastRivision=True) ->List[Log]:
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
        xmlstr,code = self.basecmd(f'log -r {lastRevision}:{curRevision} -v --xml {url}')
        xmlf = XMLFile(xmlstr)
        logs = []
        logentrys = xmlf.findall(xmlf.root,'logentry')
        for logentry in logentrys:
            logs.append(Log(xmlf,logentry))
        return logs

    def changelist(self,name,svnworkspace,*path):
        pathstr = com.safepath(*path)
        self.basecmd(f'changelist {name} {pathstr}',cwd=svnworkspace)
        return name

    def getStates(self,path,depth='infinity',ext='') ->List[SVNState_Local]:
        states:List[SVNState_Local] = []

        statusstr,code = self.basecmd(f'status {ext} --depth={depth} --xml "{path}"')
        statusxml = XMLFile(statusstr)
        target = statusxml.find(statusxml.root,'target')
        entrys = statusxml.findall(target,'entry')
        for entry in entrys:
            states.append(SVNState_Local(statusxml,entry))
        return states


    def version(self,path):
        return self.basecmd(f'info --show-item last-changed-revision "{path}"')[0]
    
    def author(self,path):
        return self.basecmd(f'info --show-item last-changed-author "{path}"')[0]    
    

    def copy(self,frompath,topath,version,message):
        self.basecmd(f'copy "{frompath}" "{topath}" -r {version} -m "{message}"')
    def remove(self,path,message):
        self.basecmd(f'remove "{path}" -m "{message}"')
    def merge(self,frompath,topath,versions,acceptOpt='p'):
        isRange = False
        if ':' in versions:
            isRange = True
        if isRange:
            self.basecmd(f'merge {frompath} -r {versions} {topath} --accept {acceptOpt}',errException=StopException('svn merge失败',locals()))
        else:
            self.basecmd(f'merge {frompath} -c {versions} {topath} --accept {acceptOpt}',errException=StopException('svn merge失败',locals()))


    def commit(self,filepath, commen):
        self.basecmd(f'commit "{filepath}" -m "{commen}"')
    def commit_changelist(self,name,msg,svnworkdir):
        self.basecmd(f'commit -m "{msg}" --cl {name}',cwd=svnworkdir)

    def rollback(self,localpath,fromRevision,toRevision):
        self.cleanup(localpath)
        self.revert(localpath)
        self.merge(localpath,localpath,f'{fromRevision}:{toRevision}')


    def get_branch_Desc(self,localpath):
        out = self.get_rel_url(localpath)
        tmp = out.split('/')
        return '-'.join(tmp[1:3]) # ^/branches/RELEASE_version_PRE_merge/Program/Client


    def get_root(self,localpath):
        out,code = self.basecmd(f'svn info --show-item wc-root {localpath}',errException=Exception("SVN获取工作根目录失败"))
        return out


    def get_abs_url(self,localpath):
        out,code = self.basecmd(f'svn info --show-item url {localpath}',errException=Exception("SVN获取绝对URL失败"))
        return out


    def get_rel_url(self,localpath):
        out,code = self.basecmd(f'svn info --show-item relative-url {localpath}',errException=Exception("SVN获取相对URL失败"))
        return out


    def isInversion(self,localpath):
        out,code = self.basecmd(f'info "{localpath}"',errException=None)
        if code == 0:
            return True
        return False
    # def isExsits(self,localpath):
    #     self.isInversion(localpath)

    def upgrade(self,localpath):
        if os.path.isfile(localpath):
            localpath = os.path.dirname(localpath)
        subp,code = com.cmd(f'svn upgrade {localpath}',getPopen=True)
        out,err = subp.communicate()
        subp.wait()
        if subp.poll() != 0:
            import re
            print(err)
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
    def getchangelist(self,url,fromver,tover)->list:
        out,code = self.basecmd(f'diff -r {fromver}:{tover} --summarize {url} 2>&1')
        states = []
        datas = out.split(com.getcmddivide())
        for statestr in datas:
            states.append(SVNState(statestr))
        return states
        
    def isroot(self,localpath):
        root = self.get_root(localpath)
        if os.path.abspath(localpath) == os.path.abspath(root):
            return True
        return False


    def scmurl2relpath(self,url,containFirstSep=False):
        relpath = '/'.join(url.split('/')[5::])
        if containFirstSep:
            relpath = '/' + relpath
        return relpath



class GIT(SCM):
    def __init__(self,username,password) -> None:
        super().__init__(username,password)
        # status


    def _login(self):
        self.basecmd(f'config --global user.name "{self.username}"')
        self.basecmd(f'config --global user.email "{self.password}"')


    def basecmd(self, cmd):
        cmd = f' {cmd}'
        out,code = com.cmd(cmd,getstdout=True,showlog=False)
        return out,code


    def update(self, localpath, revision, opt):
        pass
    def update_safe(self, localpath, revision):
        pass
    def checkout(self, data: SCMData, localpath, revision, keep):
        pass
    def revert(self, localpath):
        pass
    def cleanup(self, localpath):
        pass
    def cat(self, remotePath):
        pass
    def commit(self,localpath,msg):
        self.basecmd(f'commit "{localpath}" -m "{msg}"')

        self._push(localpath,msg)
        pass
    def _push(self,localpath,msg):
        return self.basecmd(f'push "{localpath}"')
    def add(self,localpath):
        return self.basecmd(f'add "{localpath}"')
    def log(self,localpath,fromrevision,torevision):
        # --name-only 输出改动文件
        out,code = self.basecmd(f'log --pretty=format:"^%H$%T$%an$%cn$%s" --name-only')
        logdatas = out.split('^')
        for logdata in logdatas:
            if com.isNoneOrEmpty(logdata):
                continue
            tmp = logdata.split(com.getcmddivide())
            data = tmp[0]
            files = tmp[1::]
            tmp = data.split('$')
            revision = tmp[0]
            treerevision = tmp[1]
            author = tmp[2]
            commiter = tmp[3]
            msg = tmp[4]




        pass

    def tag(self,tagname,msg,revision=None,push=True):
        cmd = f'tag -a {tagname} -m "{msg}"'
        if revision != None:
            cmd += f' {revision}'
        self.basecmd(cmd)
        if push:
            self.basecmd(f'push origin')
    


# @instance
pwdFilePath = os.path.join(com.gethomepath(),'Documents','sec','pwd_sync.txt')
scmusername,scmpwd = com.readall(pwdFilePath).splitlines()

@singleton
class SCMManager():
    def __init__(self) -> None:
        self.svn = SVN(scmusername,scmpwd)
        self.git = GIT(scmusername,scmpwd)


    def update(self,data:SCMData,localpath,revision='HEAD'):
        eval(f'self.{data.scm}.update')(localpath,revision)
    def update_safe(self,data:SCMData,localpath,revision='HEAD'):
        eval(f'self.{data.scm}.update_safe')(localpath,revision)
    def checkout(self,data:SCMData,localpath,revision='HEAD',keep=None):
        eval(f'self.{data.scm}.checkout')(data,localpath,revision,keep)
    def revert(self,data:SCMData,localpath):
        eval(f'self.{data.scm}.revert')(localpath)
    def cleanup(self,data:SCMData,localpath):
        eval(f'self.{data.scm}.cleanup')(localpath)
    def cat(self,data:SCMData,remotePath):
        eval(f'self.{data.scm}.cat')(remotePath)
    
@workspace
def test_checkout():
    data = SCMData({'url':'svn://192.168.2.177/sdk/test/test_checkout',
    'scm':'svn'})
    # SCMManager.checkout('1',)
    keep = [['',['dontcheckout']],['checkout',['dontcheckout','dontcheckout.txt']],['checkout/checkout',['dontcheckout']]]
    SCMManager().checkout(data,'test',keep=keep)
    assert not os.path.exists(os.path.join('test','dontcheckout'))
    assert not os.path.exists(os.path.join('test','checkout','dontcheckout'))
    assert not os.path.exists(os.path.join('test','checkout','dontcheckout.txt'))
    assert not os.path.exists(os.path.join('test','checkout','checkout','dontcheckout'))


    data = SCMData({'url':'svn://192.168.2.177/sdk/test/test_checkout',
    'scm':'git'})
    keep = [['',['dontcheckout']],['checkout',['dontcheckout','dontcheckout.txt']],['checkout/checkout',['dontcheckout']]]
    # git不支持排除文件
    SCMManager().checkout(data,'test',keep=keep)
@workspace
def test_commit():
    data = SCMData({'url':'svn://192.168.2.177/sdk/test/test_checkout',
    'scm':'svn'})
    

    
    data = SCMData({'url':'svn://192.168.2.177/sdk/test/test_checkout',
    'scm':'git'})
    pass

def test_update():
    data = SCMData({'url':'http://192.168.2.61:8080/svn/DNF_SDK/DevOps_Scripts/python',
    'scm':'svn'})
    SCMManager().update(data,r'E:\test')
def test_updatesafe():
    data = SCMData({'url':'http://192.168.2.61:8080/svn/DNF_SDK/DevOps_Scripts/python',
    'scm':'svn'})
    SCMManager().update_safe(data,r'E:\test')
def test_revert():
    data = SCMData({'url':'http://192.168.2.61:8080/svn/DNF_SDK/DevOps_Scripts/python',
    'scm':'svn'})
    p = r'E:\test'
    SCMManager().revert(data,p)
def test_cleanup():
    data = SCMData({'url':'http://192.168.2.61:8080/svn/DNF_SDK/DevOps_Scripts/python',
    'scm':'svn'})
    p = r'E:\test'
    SCMManager().cleanup(data,p)
if __name__ == "__main__":
    test_checkout()
    # test_updatesafe()
    pass