# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com

from comlib import SVNManager,Path
from comlib.conf.loader import Loader
from comlib.conf.ref import projectconf


import stat

def cover(args):
    # 初始化
    preconf = args[0]
    fromUrl = args[1]
    toUrl = args[2]
    fromVersion = args[3]
    
    if preconf != 'None':
        tmp = preconf.split('->')
        fromChinese = tmp[0]
        toChinese = tmp[1]
        fromUrl = chinese2url(fromChinese)
        toUrl = chinese2url(toChinese)
    else:
        descconf = Loader.load(projectconf)
        fromChinese = url2chinese(fromUrl)
        toChinese = url2chinese(toUrl)
    if fromVersion == 'HEAD':
        fromVersion = SVNManager.version(fromUrl)

    if SVNManager.isExsits(toUrl):
        # svn操作
        toVersion = SVNManager.version(toUrl)
        # 备份到tags
        message = f'需求修改([svn]/[备份]): 因{fromChinese}版本号{fromVersion}覆盖操作，从{toChinese}版本号{toVersion}备份'
        tmp = toUrl.rsplit('/',2)
        repoRootUrl = SVNManager.get_repoRoot(fromUrl)
        relUrl = SVNManager.get_rel_url(toUrl)

        backupUrl = f'{repoRootUrl}/tags/{relUrl.replace("/","-")}#{com.timemark}_R{fromVersion}=》{toVersion}'
        SVNManager.copy(toUrl,backupUrl,toVersion,message)
        # 移除旧分支
        message = f'需求修改([svn]/[删除]): 因从{fromChinese}版本号{fromVersion}对{toChinese}版本号{toVersion}进行覆盖操作，删除旧分支，备份地址{backupUrl}'
        SVNManager.remove(toUrl,message)
        # 覆盖
        message = f'需求修改([svn]/[覆盖]): 从{fromChinese}版本号{fromVersion}对{toChinese}版本号{toVersion}进行覆盖操作，备份地址{backupUrl}'
        SVNManager.copy(fromUrl,toUrl,fromVersion,message)
    else:
        # 新建
        message = f'需求修改([svn]/[拷贝]): 从{fromChinese}版本号{fromVersion}拷贝到{toChinese}'
        SVNManager.copy(fromUrl,toUrl,fromVersion,message)


def rollback(args):
    backupUrl = args[0]
    version = args[1]
    backupName = os.path.basename(backupUrl)
    tmp = backupName.split('#')

    repoRootUrl = SVNManager.get_repoRoot(backupUrl)
    relUrl = tmp[0].replace('-','/')
    sourceUrl = f'{repoRootUrl}/{relUrl}'
    args = ['None',backupUrl,sourceUrl,version]

    cover(args)

def url2chinese(url):
    descconf = Loader.load(projectconf)
    chinese = url
    for branch in descconf.branchs:
        if branch['url'] == url:
            chinese = branch['chinese']
    return chinese
def chinese2url(chinese):
    descconf = Loader.load(projectconf)
    url = chinese
    for branch in descconf.branchs:
        if branch['chinese'] == chinese:
            url = branch['url']
    return url
def merge(args):
    frombranchUrl = args[0]
    frombranchChinese = url2chinese(frombranchUrl)
    tobranchUrl = args[1]
    versions = args[2]

    tobranchName = tobranchUrl.split('/')[-1]
    Path.ensure_svn_pathexsits(tobranchName,tobranchUrl)
    SVNManager.merge(frombranchUrl,tobranchUrl,versions)
    SVNManager.commit(tobranchName,f'需求修改([svn]/[合并]): 从{frombranchChinese}版本号{versions}合并')
if __name__ == "__main__":
    # 三个操作
    # 覆盖分支 备份分支回滚 版本合并
    method = sys.argv[1]
    args = sys.argv[2::]
    if method == 'cover':
        cover(args)
    elif method == 'rollback':
        rollback()
    elif method == 'merge':
        merge()
    pass