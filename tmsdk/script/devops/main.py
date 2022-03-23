# -*- encoding: utf-8 -*-
import os,sys
thisdir = os.path.abspath(os.path.dirname(__file__))
from comlib.comobj import *

from argparse import ArgumentParser
from comlib import VirtualEnvManager
logger = Log.getLogger('EnterPointer')

import shlex

class EnterPointer:
    def __init__(self,args_raw=None) -> None:
        if args_raw == None:
            args_raw = ' '.join(sys.argv)
        self.__paresOption(args_raw)
        

        if com.isNoneOrEmpty(self.requirementsFile):
            self.requirementsFile = os.path.join(thisdir,'requirements.txt')
        
    def __paresOption(self,args_raw:str=None):
        mainArgs_raw,self.subArgs_raw = args_raw.split(' -- ')
        mainArgs:List[str] = shlex.split(mainArgs_raw,posix=not com.isWindows())
        
        parser = ArgumentParser()
        parser.add_argument('--recreateEnv',required=False,action='store_true',help='重新创建虚拟环境')
        parser.add_argument('--remoteDebug',required=False,action='store_true',help='远程调试')
        parser.add_argument('--requirementsFile',required=False,default=None,type=com.str2Noneable,help='使用指定的requirements.txt安装插件')
        parser.add_argument('--exit',required=False,action='store_true',help='执行子进程命令时退出当前进程')
        
        args = parser.parse_args(mainArgs[1::])
        
        self.recreateEnv = args.recreateEnv
        self.remoteDebug = args.remoteDebug
        self.requirementsFile = args.requirementsFile
        self.exit = args.exit

    def main(self,args_raw:str=None):
        '''
        功能：
        ------
        部署虚拟环境 \n
        提供远程调试入口
        '''
        # 虚拟环境路径需要在执行的工作目录生成，因为后续会将python提交到这里，提交之后会导致多个环境使用同一份虚拟环境
        vpath = os.path.join(workdir,'venv')
        vem = VirtualEnvManager(vpath)
        vem.create(self.recreateEnv)
        vem.installRequirementsFile(self.requirementsFile)

        # -----------设置环境变量 start-------------
        # 搜索路径
        com.AddPath2Env(thisdir)
        [com.AddPath2Env(path.strip()) for path in com.readlines(os.path.join(thisdir,'searchPath.txt')) if not com.isNoneOrEmpty(path.strip())]
        # 需要转换工作目录，远程调试时工作目录时在main.py所在目录
        os.chdir(thisdir)
        # -----------设置环境变量 end-------------

        if com.isNoneOrEmpty(self.subArgs_raw):
            return
        print('-----------------------------------------------------',flush=True)
        print('-------------------开始运行指定脚本-------------------',flush=True)
        print('-----------------------------------------------------',flush=True)
        
        if self.remoteDebug:
            print('*******************开启远程调试模式*******************',flush=True)
            baseCmd = f'-m ptvsd --host localhost --port 12345 --wait {self.subArgs_raw}'
            if self.exit:
                logger.info(baseCmd)
                os.execv(vem.pythonPath,['python'] + shlex.split(baseCmd,posix=not com.isWindows()))
            else:
                com.cmd(f'{vem.pythonPath} {baseCmd}',getstdout=False,errException=Exception(f'命令执行失败{baseCmd}'))

        else:
            if self.exit:
                logger.info(self.subArgs_raw)
                os.execv(vem.pythonPath,['python'] + shlex.split(self.subArgs_raw,posix=not com.isWindows()))
            else:
                com.cmd(f'{vem.pythonPath} {self.subArgs_raw}',getstdout=False,errException=Exception(f'命令执行失败{self.subArgs_raw}'))


if __name__ == '__main__':
    EnterPointer().main()
    