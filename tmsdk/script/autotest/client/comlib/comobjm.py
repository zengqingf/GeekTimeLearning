import sys,os
# 类型声明
from typing import Type,TypeVar,List,Tuple,Dict
T = TypeVar('T')
from enum import Enum,unique
import re,textwrap,glob
# 唯一2个不用依赖其他模块的模块
from comlib.logm import LogLevel,Log
from comlib import com
workdir = os.path.abspath(os.getcwd())

G_ip = com.get_host_ip()
G_timemark = com.getlocaltime('-')
G_timemark_datetime = com.getdatetimenow()
G_timemark_tsp = com.gettsp()
G_username = com.whoami()
G_projconfDir = os.path.join(workdir,'projconf')