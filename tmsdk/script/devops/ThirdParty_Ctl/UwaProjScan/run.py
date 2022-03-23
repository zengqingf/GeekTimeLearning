# -*- encoding: utf-8 -*-
import sys,os,shutil
sys.path.append(os.path.abspath(os.path.join(__file__,'..','..','..')))
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
from comlib import SVNManager,TMUnityManager,ThreadManager,BinManager
from comlib import com
from comlib import 一点五优化群优化机器人
from comlib import Path

from comlib.conf.loader import Loader
from comlib.conf.ref import envconf,uwaprojscanconf

import time


rootpath = ''
projectpath = ''

uploaderpath = os.path.join(com.get_file_dirname(__file__),'UwaDataUploader','UwaDataUploader.exe')
timemark = com.getlocaltime('-')


def run_UWA_projscan(projectpath):
    SVNManager.update_safe(projectpath)

    env = Loader.getenvconf()
    unityM = TMUnityManager(env.enginepath,projectpath,timemark)
    # 配置uwa
    uwaconf = Loader.load(uwaprojscanconf)
    dllpath = os.path.join(thisdir,uwaconf.name)
    dllpath_in_project = os.path.join(unityM.unityAssetPath,uwaconf.name)
    com.dumpfile_json({'user':uwaconf.user,'password':uwaconf.password,'project':uwaconf.project},os.path.join(thisdir,'UwaDataUploader','config.json'))
    Path.ensure_pathnewest(dllpath,dllpath_in_project,ignore=shutil.ignore_patterns('.svn'))

    subp,code,respath = unityM.UWA_projscan()
    okfile = os.path.join(projectpath,'UwaScan','table.particleeffect.csv')
    def waitok():
        com.logout('等待table.particleeffect.csv文件生成')
        while not os.path.exists(okfile):
            time.sleep(3)
        com.logout('等待uwa扫描结果写入')
        # 文件大，等待20s确保文件写入完毕
        time.sleep(20)
        com.logout('工程执行成功')
        
    thds = ThreadManager.go(waitok,count=1)
    ThreadManager.waitall(thds)

    time.sleep(3)

    BinManager.kill_all_unity()
    time.sleep(3)
    subp.kill()
    time.sleep(3)

    base_cmd = com.getvalue4plat(uploaderpath,'mono '+uploaderpath)
    cmd = base_cmd + ' ' + respath
    out,code = com.cmd(cmd,errException=Exception("Upload结果失败"))
    
    Path.ensure_pathnotexsits(dllpath_in_project)

    resfile = os.path.join(com.get_file_dirname(__file__),'UwaDataUploader','result.json')
    
    # "资源检测结果上传成功，查看报告地址：https://www.uwa4d.com/u/pipeline/overview?project=${states.projctid}"
    js = com.loadfile_json(resfile)
    Path.ensure_pathnotexsits(resfile)
    if js['status'] == 'failed':
        DingERROR(f"资源上传失败,{js['reason']}")
    Ding(js['projctid'])

def Ding(projctid):
    content = "资源检测结果上传成功，"
    content += 一点五优化群优化机器人.markdown_textlink('>>查看报告<<',f'https://www.uwa4d.com/u/pipeline/overview?project={projctid}')
    data = 一点五优化群优化机器人.build_markdown("UWA资源检测结果",content)
    一点五优化群优化机器人.send(data)
    data = 一点五优化群优化机器人.build_text('uwa资源扫描报告出来了',Loader.根据中文名获取电话('王玺竣'),Loader.根据中文名获取电话('金星萌'))
    一点五优化群优化机器人.send(data)
def DingERROR(msg):
    content = msg
    data = 一点五优化群优化机器人.build_markdown("UWA资源检测结果",content)
    一点五优化群优化机器人.send(data)
    raise Exception(msg)
if __name__ == "__main__":
    # 在192.168.2.228上运行
    # unitypath = '/Applications/Unity2018.4.23f1'

    run_UWA_projscan(sys.argv[1])
    # run_UWA_projscan(r'E:\UnityProject\projscan')
    # target_jenkins_projpath = sys.argv[1]
    # unitypath = sys.argv[2]

        
