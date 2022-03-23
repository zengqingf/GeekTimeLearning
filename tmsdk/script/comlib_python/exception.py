# -*- encoding: utf-8 -*-

# ------------------------------------------------
# ************************************************
# 改动必须测试钉钉机器人和这个模块
# ************************************************
# ------------------------------------------------

import sys,os
sys.path.append(os.path.abspath(os.path.join(__file__,'..','..')))

from functools import wraps

import traceback

root = os.path.abspath(os.path.join(__file__,'..','..'))
from subprocess import signal
from typing import Type,Union,TypeVar,Callable

T = TypeVar('T')


debug = False
def getCanDing():
    from comlib.conf.loader import Loader
    from comlib.conf.ref import globalconf
    canDing = True
    gconf:globalconf = Loader.load(globalconf,use_defalt=True)
    canDing = gconf.ding
    return canDing

def mprint(msg):
    if debug:
        print(msg)


__all__=('LOW','NORMAL','HIGH','errorcatch','add_exitfunc')

exitfunc=[]
def add_exitfunc(func):
    exitfunc.append(func)
def invoke_exitfunc():
    for func in exitfunc:
        func()
    # if sys.platform == 'win32':
    #     os.kill(0, signal.CTRL_C_EVENT)
    # else:
    #     os.kill(0,signal.SIGINT)
def globalExcept(excType, excValue, tb):
    
    # 先执行默认
    sys.__excepthook__(excType, excValue, tb)
    if not getCanDing():
        return
    try:
        from comlib.dingrobot import 内网发包群苦工,杨都齐
        stack = ''.join(traceback.format_exception(excType, excValue, tb))
        if ('dingrobot.py' in stack) or ('gettsp_BeiJin' in stack):
            print(stack)
            print('钉钉执行异常')
            exit(255)
        print('意料之外的异常')
        title = '告警级别：非常严重'
        


        content = 内网发包群苦工.markdown_textquote(内网发包群苦工.markdown_textlevel(title,1))
        content += '------------------------------\n'
        content += stack.replace(root + os.sep,'').replace('File ','').replace('\n','\n\n')
        content += '------------------------------\n'
        content += 内网发包群苦工.markdown_textlevel("携带信息:%s\n"%'意料之外的异常',3)
        content += 'root:%s'%root+'\n\n'
        # if isinstance(e,DingException):
        #     content += e.elocals.__str__()+'\n\n'
        #     content += 内网发包群苦工.markdown_textlink('>>跳转<<',e.url)
        data,atdata = 内网发包群苦工.build_markdown(title,content,杨都齐)

        内网发包群苦工.send(atdata)
        内网发包群苦工.send(data)
    except Exception as e:
        stack = traceback.format_exc()
        print(stack)
        print('不可捕获的内部错误')
        
        exit(255)
    finally:
        invoke_exitfunc()

sys.excepthook = globalExcept




HIGH='high'
NORMAL='normal'
LOW='low'


class errorcatch_base(object):
    def __init__(self,level_or_type):
        # 储存值
        self.level_or_type = level_or_type
    def levelinit(self,level):
        self.level = level

    def _init(self,tp):
        pass
class Errorcatch_Cls(errorcatch_base):
    def _init(self,tp):
        mprint(f'{tp}以类装饰')
        return self._init_type(tp)
    def _init_type(self,tp):
        for k,v in tp.__dict__.items():
            if k == '__module__':
                pass
            if callable(v):
                setattr(tp,k,buildcatch(v,self.level))
            elif hasattr(v,'__func__'):
                if callable(v.__func__):
                    setattr(tp,k,buildcatch(v,self.level))
            else:
                mprint(f'{k} not wrap')
        return tp
    def parse(self):
        '''
        真正初始化起点
        '''
        # 写两个为了pylance的智能提示
        if isinstance(self.level_or_type,str):
            self.levelinit(self.level_or_type)
            return self._init
        else:
            self.levelinit(NORMAL)
            return self._init(self.level_or_type)
class Errorcatch_Func(errorcatch_base):
    def _init(self,tp):
        mprint(f'{tp}以方法装饰')
        return self.__init_func(tp)
    def __init_func(self,func):
        @wraps(func)
        def desc(*params,**kv):
            return buildcatch(func,self.level)(*params,**kv)
        return desc
    def parse(self):
        '''
        真正初始化起点
        '''
        # 写两个为了pylance的智能提示
        if isinstance(self.level_or_type,str):
            self.levelinit(self.level_or_type)
            return self._init
        else:
            self.levelinit(NORMAL)
            return self._init(self.level_or_type)

def buildcatch(func,level):
    @wraps(func)
    def catch(*params,**kv):
        try:
            if callable(func):
                return func(*params,**kv)
            elif hasattr(func,'__func__'):
                if callable(func.__func__):
                    return func.__func__(*params,**kv)
            else:
                mprint(f'{func} not catch')
        except StopException as stope:
            stack = get_stack()
            Ding(level,stack,stope)
            invoke_exitfunc()
            exit(1)
            # raise stope
        except DingException as dinge:
            stack = get_stack()
            Ding(level,stack,dinge)
        except Exception as e:
            stack = get_stack()
            if ('dingrobot.py' in stack) or ('gettsp_BeiJin' in stack):
                print(stack)
                print('钉钉执行异常')
                exit(255)
            Ding(level,stack,e)
            invoke_exitfunc()
            exit(255)                
            # raise e
        finally:
            pass
    return catch

def Ding(level,stack,e):
    if not getCanDing():
        return
    try:
        from comlib.dingrobot import 内网发包群苦工,杨都齐
        # print(comlib.__dict__)
        # 内网发包群苦工 = comlib.内网发包群苦工
        # 杨都齐 = comlib.杨都齐
        print(stack)
        mprint('捕获到了%s级别'%level)
        mprint(type(e))
        mprint(e)

        title = '告警级别：%s'
        if level == LOW:
            title = title%'轻微'
        elif level == NORMAL:
            title = title%'中等' 
        elif level == HIGH:
            title = title%'严重' 

        content = 内网发包群苦工.markdown_textquote(内网发包群苦工.markdown_textlevel(title,1))
        content += '------------------------------\n'
        content += stack.replace(root + os.sep,'').replace('File ','').replace('\n','\n\n')
        content += '------------------------------\n'
        content += 内网发包群苦工.markdown_textlevel("携带信息:%s-%s\n"%(e.__str__(),root),3)
        if isinstance(e,DingException):
            content += e.elocals.__str__()+'\n\n'
            content += 内网发包群苦工.markdown_textlink('>>跳转<<',e.url)
        data,atdata = 内网发包群苦工.build_markdown(title,content,杨都齐)

        内网发包群苦工.send(atdata)
        内网发包群苦工.send(data)
    except Exception as e:
        stack = traceback.format_exc()
        print(stack)
        print('不可捕获的内部错误')
        invoke_exitfunc()
        exit(255)
def get_stack():
    ss = traceback.extract_stack()
    # innerstack = traceback.extract_tb()
    innerstack = traceback.format_exc()
    # innerstack2 = traceback.

    filter_ss = filter_stack(ss)
    
    stack = ''.join(traceback.format_list(filter_ss))
    fullstacks = stack+innerstack
    return fullstacks
def filter_stack(stacks):
    startindex = 0
    endindex = 0
    for i in range(0,stacks.__len__()):
        if stacks[i].name == '_run_code':
            startindex = i
        elif stacks[i].name == 'catch':
            endindex = i
    return stacks[startindex+1:endindex:]
def detailtrace(info):
    retStr = ""
    curindex=0
    f = sys._getframe()
    f = f.f_back    # first frame is detailtrace, ignore it
    while hasattr(f, "f_code"):
        co = f.f_code
        retStr = "%s(%s:%s)->"%(os.path.basename(co.co_filename),
                co.co_name,
                f.f_lineno) + retStr
        f = f.f_back
    print(retStr+info)

def errorcatch(level_or_type):
    '''
    这是类装饰，方法装饰准备移出来，原因：pylance对混合装饰器不会提供智能提示
    '''
    catchobj = Errorcatch_Cls(level_or_type)
    return catchobj.parse()
def errorcatch_func(level_or_type):
    catchobj = Errorcatch_Func(level_or_type)
    return catchobj.parse()


class DingException(Exception):
    def __init__(self,msg,elocals={},url=''):
        super().__init__(msg)
        self.elocals = elocals
        self.url = url
class StopException(DingException):
    def __init__(self,msg,elocals={},url=''):
         super().__init__(msg,elocals,url)

















# ------------------------------------------
# 下面是测试用例
# ------------------------------------------
@errorcatch(HIGH)
class test(object):
    def __init__(self,who):
        pass
    def inter(self):
        raise Exception("内部方法测试")
    @staticmethod
    def rrr():
        raise Exception("静态方法测试")

class test2(object):
    def __init__(self,who):
        pass
    @errorcatch_func(HIGH)
    def inter(self):
        raise Exception("内部方法单独捕获测试")
    @errorcatch_func(HIGH)
    @staticmethod
    def rrr():
        raise Exception("静态方法单独捕获测试")


def dictdesc(tp):
    return tp
@dictdesc
class dicttest(object):
    def __init__(self):
        pass
@errorcatch_func
def outfunc1(prp):
    mprint(prp)
    raise Exception("外部方法测试无参数")

@errorcatch_func(LOW)
def outfunc2(prp):
    mprint(prp)
    raise Exception("外部方法测试")

if __name__ == "__main__":

    a = test('yang')
    test.rrr()
    a.inter()
    
    b = test2('du')
    test2.rrr()
    b.inter()

    outfunc1('外部方法测试无参数')
    outfunc2('外部方法测试有参数')
    
    # from comlib.ftpm import FTPManager
    # fff = FTPManager('192.168.2.147',21,'ftp','123456')
    # fff.ftpsep2local('papapaa')
    pass