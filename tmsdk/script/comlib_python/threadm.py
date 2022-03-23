# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com

import time,threading
from queue import Queue

from typing import List


def getBaseThread(target,args=None,autoStart=False):
    if args == None:
        t = threading.Thread(target=target)
    else:
        t = threading.Thread(target=target,args=args)
    t.setDaemon(True)
    if autoStart:
        t.start()
    return t

        
class Task(object):
    def __init__(self,runFunc,*args,timeSpan=1):
        super().__init__()
        self.runFunc = runFunc
        self.args = args
        self.timeSpan = timeSpan
        self._isquit = False
        self.thd = None

    def run(self):
        self.thd = getBaseThread(target=self._run,autoStart=True)

    def _run(self):
        while not self._isquit:
            self.runFunc(self,*self.args)
            time.sleep(self.timeSpan)
    def quit(self):
        self._isquit = True

@errorcatch(HIGH)
class Factory(object):
    def __init__(self,runFunc,parallelCount=1,inputQueue=None,outputQueue=None,baseName=None):
        super().__init__()
        self.runFunc = runFunc
        self.inputQueue = inputQueue
        self.outputQueue = outputQueue
        self.baseName = baseName

        self.thds = []

        if self.inputQueue == None:
            pass
            # self.inputQueue = Queue()
        if self.outputQueue == None:
            self.outputQueue = Queue()
            
        self.parallelCount = parallelCount

        self.isEnd = False
        self.isStop = False
        self._stopThds = set()
        self._exitThds = set()

        self.next = None
        self.prev = None
    def quit(self):
        self.isEnd = True
    def stop(self):
        self.isStop = True
    def restart(self):
        self.isStop = False
        self._stopThds.clear()
    def isAllStoped(self):
        if self.thds.__len__() == self._exitThds.__len__():
            return True
        if self.thds.__len__() == self._stopThds.__len__():
            return True
        return False
    def isAllExited(self):
        if self.thds.__len__() == self._exitThds.__len__():
            return True
        return False
    def waitForAllStoped(self):
        ThreadManager.waitfor(self.isAllStoped)
    def run(self):
        self.thds = ThreadManager.go(self._runFunc,count=self.parallelCount)
        if self.baseName != None:
            for i in range(0,self.thds.__len__()):
                self.thds[i].setName(f'{self.baseName}{i}')
        return self.thds
    def addNext(self,factory):
        self.next = factory
        factory.inputQueue = self.outputQueue
        factory.prev = self
    def canEnd(self):
        # 手动设置的end值
        if self.isEnd:
            return True
        # 输入队列为空代表永不停止
        if self.inputQueue == None:
            return False

        
        if self.prev == None:
            if self.inputQueue.qsize() == 0:
                return True
        else:
            if self.prev.isAllExited() and self.inputQueue.qsize() == 0:
                return True
        return False
    def _runFunc(self):
        try:
            while not self.isEnd:
                if self.isStop:
                    self._stopThds.add(threading.current_thread().ident)
                    time.sleep(1)
                    continue
                if self.canEnd():
                    self.isEnd = True
                    break
                self.runFunc(self,self.inputQueue,self.outputQueue)
        except Exception as e:
            import traceback
            traceback.print_exc()
        finally:
            self._exitThds.add(threading.current_thread().ident)


class ThreadManager(object):
    
    @staticmethod
    def waitfor(func,waitsec=1):
        while not func():
            time.sleep(waitsec)
    @staticmethod
    def waitall(thds):
        if thds != None:
            for t in thds:
                if t != None:
                    t.join()
    @staticmethod
    def waitall_tasks(tasks:list):
        for task in tasks:
            task.thd.join()
    @staticmethod
    def go(func,argsList=None,count=None)->list:
        '''
        argsList : 有参函数的参数，内容是func参数tuple，一个参数的函数可以直接输入，会自动转为tuple，list长度是并发数\n
        count : 无参函数的并发数，有参函数将会无视\n
        使用方法1：填argsList不填count，并发数是argsList长度，参数是argsList的tuple\n
        使用方法2：不填argsList填count，适用于无参函数，并发数是count\n
        使用方法3：填argsList填count，适用于并发参数值相同的函数，并发数是count
        '''
        thds = []
        if argsList == None:
            for i in range(0,count):
                t = getBaseThread(target=func,autoStart=True)
                thds.append(t)
        elif argsList != None and count == None:
            for args in argsList:
                if isinstance(args,list):
                    args = tuple(args)
                elif not isinstance(args,tuple):
                    args = (args,)
                t = getBaseThread(target=func,args=args,autoStart=True)
                thds.append(t)
        elif argsList != None and count != None:
            if isinstance(argsList,list):
                argsList = tuple(argsList)
            elif not isinstance(argsList,tuple):
                argsList = (argsList,)
            for i in range(0,count):
                t = getBaseThread(target=func,args=argsList,autoStart=True)
                thds.append(t)
        return thds
    @staticmethod
    def taskGo(func,argsList=None,count=None,timeSpan=1,autostart=True) -> List[Task]:
        '''
        区别于ThreadManager.go，这个返回Task列表，不需要写死循环\n
        argsList : 有参函数的参数，内容是func参数tuple，一个参数的函数可以直接输入，会自动转为tuple，list长度是并发数\n
        count : 无参函数的并发数，有参函数将会无视\n
        使用方法1：填argsList不填count，并发数是argsList长度，参数是argsList的tuple\n
        使用方法2：不填argsList填count，适用于无参函数，并发数是count\n
        使用方法3：填argsList填count，适用于并发参数值相同的函数，并发数是count
        '''
        tasks = []
        if argsList == None:
            for i in range(0,count):
                t = Task(func,timeSpan=timeSpan)
                if autostart:
                    t.run()
                tasks.append(t)
        elif argsList != None and count == None:
            for args in argsList:
                if not isinstance(args,tuple) and not isinstance(args,list):
                    args = (args,)
                t = Task(func,*args,timeSpan=timeSpan)
                if autostart:
                    t.run() 
                tasks.append(t)
        elif argsList != None and count != None:
            if not isinstance(argsList,tuple) and not isinstance(argsList,list):
                argsList = (argsList,)
            for i in range(0,count):
                t = Task(func,*argsList,timeSpan=timeSpan)
                if autostart:
                    t.run()
                tasks.append(t)
        return tasks
