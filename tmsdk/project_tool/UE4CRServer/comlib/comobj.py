# -*- encoding: utf-8 -*-
# 
# 
#      这是开放给外部用的，comlib库用comobjm！！！
# 
# 

from comlib.comobjm import *
comobjdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(comobjdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH



from comlib import com
# 配置
from comlib.conf.loader import Loader
from comlib.conf.ref import *
# 必要的库
from comlib import LogLevel,Log,workspace,Path,JsonFile,XMLFile,DictUtil,Version,SecretManager
# 虚幻
from comlib import UE4Engine,UE4Project
# 工作流
try:
    from comlib import SVNManager,GitManager,GitlabManager,GitWorkflow,SVNWorkflow
except ImportError as ie:
    Log.warning('NOT MODULE requests')


G_ftp = Loader.获取ftp实例()
