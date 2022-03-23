# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
# from comlib.wraps import singleton


import logging
from enum import Enum

import shutil
from datetime import datetime

# 这个模块不能依赖任何其他自己的模块！！！

__all__ = ['Log','LogLevel']

class LogLevel(Enum):
    CRITICAL = 50
    FATAL = CRITICAL
    ERROR = 40
    WARNING = 30
    WARN = WARNING
    INFO = 20
    DEBUG = 10
    NOTSET = 0

defaultLoggerName = 'Main'
class 日志分隔类型(Enum):
    大括号分隔=0
    空格分隔=1
    竖线分隔=2

日志分隔 = 日志分隔类型.竖线分隔
# 日志文件默认寿命
logFileLife = 6
logFileRootDirPath = os.path.join(workdir,'_log')
def LogClean():
    '''
    日志文件自洁，自动删除旧文件
    '''
    if not os.path.exists(logFileRootDirPath):
        return
    now = datetime.now()
    tsp = int(now.timestamp()*1000)
    for logFileDirPath in [os.path.join(logFileRootDirPath,p) for p in os.listdir(logFileRootDirPath)]:
        time = os.path.getmtime(logFileDirPath)
        deltaDay = int((tsp - int(time * 1000)) / 1000 / 60 / 60 / 24)
        if deltaDay > logFileLife:
            shutil.rmtree(logFileDirPath)

# @singleton
class InternalLog:
    def __init__(self) -> None:
        if 日志分隔 == 日志分隔类型.大括号分隔:
            logging.basicConfig(format='[%(asctime)s] [%(filename)-8s:%(lineno)-4s] [%(name)-8s] [%(levelname)-7s]|%(message)s')
        elif 日志分隔 == 日志分隔类型.空格分隔:
            logging.basicConfig(format='%(asctime)s %(filename)-8s:%(lineno)-4s %(name)-8s %(levelname)-7s|%(message)s')
        elif 日志分隔 == 日志分隔类型.竖线分隔:
            logging.basicConfig(format='%(name)-8s %(levelname)-7s [%(asctime)s] %(message)s |<%(filename)-8s:%(lineno)-4s>')
        
        

    def getLogger(self,loggerName=None):
        if loggerName == None:
            loggerName = defaultLoggerName 
        log = logging.getLogger(loggerName)
        log.setLevel(logging.DEBUG)
        return log
    def setLoggerLevel(self,level:LogLevel,loggerName=None):
        self.getLogger(loggerName).setLevel(level)
        # log.setLevel(logging.DEBUG)

useLogger = InternalLog()
LogClean()

class Log:
    @staticmethod
    def getLogFilePath(filename):
        '''
        生成日志文件储存路径
        '''
        from com import getlocaltime
        dirname = os.path.join(logFileRootDirPath,getlocaltime('-'))
        if not os.path.exists(dirname):
            os.makedirs(dirname)
        return os.path.join(dirname,filename)
    @staticmethod
    def getLogger(loggerName):
        return useLogger.getLogger(loggerName)
    @staticmethod
    def debug(msg,loggerName=None,stacklevel=2):
        useLogger.getLogger(loggerName).debug(msg,stacklevel=stacklevel)
    @staticmethod
    def info(msg,loggerName=None,stacklevel=2):
        useLogger.getLogger(loggerName).info(msg,stacklevel=stacklevel)
    @staticmethod
    def warning(msg,loggerName=None,stacklevel=2):
        useLogger.getLogger(loggerName).warning(msg,stacklevel=stacklevel)
    @staticmethod
    def error(msg,loggerName=None,stacklevel=2):
        useLogger.getLogger(loggerName).error(msg,stacklevel=stacklevel)
    @staticmethod
    def critical(msg,loggerName=None,stacklevel=2):
        useLogger.getLogger(loggerName).critical(msg,stacklevel=stacklevel)
    @staticmethod
    def setLoggerLevel(level:LogLevel,loggerName=None):
        useLogger.setLoggerLevel(level,loggerName)
