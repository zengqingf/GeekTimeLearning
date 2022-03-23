# -*- encoding: utf-8 -*-
from comlib.comobjm import *
thisdir = os.path.abspath(os.path.dirname(__file__))
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib.wraps import workspace
from comlib.conf.loader import Loader
from comlib.conf.ref import *
from comlib.dictm import DictUtil

logger = Log.getLogger('Docker')


class DockerManager:
    @staticmethod
    def BaseCmd(cmd,errException=None,**args):
        cmd = f'docker {cmd}'
        logger.info(cmd)
        out,code = com.cmd(cmd,showlog=False,errException=errException,**args)
        return out.strip(),code
    @staticmethod
    def CreateContainer(imgName,containerName:str)->str:
        containerName = containerName.lower()
        out,code = DockerManager.BaseCmd(f'run --interactive --detach --name {containerName} {imgName}',errException=Exception(f'创建镜像{imgName}的容器{containerName}失败'))
        return out
    @staticmethod
    def IsImageExist(imgName):
        cid = DockerManager.GetImageCid(imgName)
        if cid == None:
            return False
        return True
    @staticmethod
    def GetImageCid(imgName):
        out,code = DockerManager.BaseCmd(f'image ls -a -q {imgName}',errException=Exception(f'列出{imgName}失败'))
        cids = out.splitlines()
        if cids.__len__() == 0:
            return None
        return cids[0]
    @staticmethod
    def BuildImage(dockerFilePath,imgName:str,imgSavePath='.',removeOld=True,argkv:dict=None)->str:
        imgName = imgName.lower()
        if removeOld and DockerManager.IsImageExist(imgName):
            DockerManager.RemoveImage(imgName)
        buildArgList = []
        if not DictUtil.isEmpty(argkv):
            for k,v in argkv.items():
                buildArgList.append(f'--build-arg {k}={v}')
        buildArg = ' '.join(buildArgList)
        out,code = DockerManager.BaseCmd(f'build --rm --quiet {buildArg} --tag "{imgName}" -f "{dockerFilePath}" "{imgSavePath}"',errException=Exception(f'创建镜像{dockerFilePath}-{imgName}到{imgSavePath}失败'))
        # sha256:xxxxxxxxxxxxxxxxxxxxxxxxx
        imgId = out.split(':')[-1].strip()
        return imgId
    @staticmethod
    def Copy(src,dst):
        DockerManager.BaseCmd(f'cp {src} {dst}',errException=Exception(f'从{src}拷贝到{dst}失败'))
    @staticmethod
    def ExecInContainer(cmd,containerName):
        DockerManager.BaseCmd(f'exec -t {containerName} {cmd}',getstdout=False,errException=Exception(f'{containerName}执行{cmd}失败'))


    @staticmethod
    def RemoveContainer(containerName):
        if com.isNoneOrEmpty(containerName):
            return
        DockerManager.BaseCmd(f'rm -f {containerName}',errException=Exception(f'删除容器{containerName}失败'))
    @staticmethod
    def RemoveImage(imgName):
        if com.isNoneOrEmpty(imgName):
            return
        DockerManager.BaseCmd(f'rmi -f {imgName}',errException=Exception(f'删除镜像{imgName}失败'))
    @staticmethod
    def RemoveVolume(volumeName):
        if com.isNoneOrEmpty(volumeName):
            return
        DockerManager.BaseCmd(f'volume rm -f {volumeName}',errException=Exception(f'删除卷{volumeName}失败'))
        

