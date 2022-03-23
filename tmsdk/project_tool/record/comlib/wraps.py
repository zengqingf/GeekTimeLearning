# -*- encoding: utf-8 -*-
# TODO 解耦wraps
from comlib.comobjm import *

from time import sleep
thisdir = os.path.dirname(__file__)
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))

from functools import wraps




def call(func,*params,**kv):
    if callable(func):
        return func(*params,**kv)
    elif hasattr(func,'__func__'):
        if callable(func.__func__):
            return func.__func__(*params,**kv)
    else:
        Log.warning(f'{func} not call')
def instance(cls:Type[T])->T:
    '''
    直接创建类实例
    '''
    return cls()

def singleton(cls:T)->T:
    '''
    传统单例，懒加载
    '''
    def _singleton(*args, **kargs):
        if not hasattr(cls, '_instance'):
            cls._instance = cls(*args,**kargs)
        return cls._instance
    return _singleton

def workspace(func):
    '''
    这个装饰器会切换进工作空间目录，在函数执行完毕后删除。
    外部路径用绝对路径，临时文件用 . 相对路径
    在errorcatch装饰器下面
    '''
    @wraps(func)
    def desc(*a,**kv):
        import shutil
        flag = 'GJJWKS_'
        curworkspace = os.path.abspath(os.getcwd())
        # 清理旧的临时目录
        for fileName in os.listdir(curworkspace):
            if fileName.startswith(flag):
                oldWorkspace = os.path.join(curworkspace,fileName)
                Log.info(f'[workspace] del old workspace  ==> {oldWorkspace}')
                shutil.rmtree(oldWorkspace)
        
        tarworkspace = os.path.join(f'{flag}{G_timemark}')
        os.mkdir(tarworkspace)
        Log.info(f'[workspace] {curworkspace} ==> {tarworkspace}')
        os.chdir(tarworkspace)
        try:
            return call(func,*a,**kv)
            # func(*a,**kv)
        except Exception as e:
            raise e
        finally:
            os.chdir(curworkspace)
            sleep(1)
            Log.info(f'[workspace] {tarworkspace} ==> {curworkspace}')
            shutil.rmtree(tarworkspace)
    return desc

# 用文件锁 写个lock装饰器
# def lock_unity_proj(projpath):
#     com.logout(f'[工程加锁] {projpath}')
#     with open(os.path.join(projpath,'lock'),'w') as fs:
#         pass
# def islock_unity_proj(projpath):
#     if os.path.exists(os.path.join(projpath,'lock')):
#         return True
#     return False
# def unlock_unity_proj(projpath):
#     com.logout(f'[工程解锁] {projpath}')
#     Path.ensure_pathnotexsits(os.path.join(projpath,'lock'))


# def waitfor_proj_openable(projpath):
#     com.logout(f'[等待工程解锁] {projpath}')
#     ThreadManager.waitfor(lambda : not os.path.exists(os.path.join(projpath,'lock')))


# def projlock(projpath):
#     '''
#     在errorcatch装饰器上面
#     '''
#     def desc(func):
#         @wraps(func)
#         def truedesc(*params,**kv):
#             waitfor_proj_openable(projpath)
#             lock_unity_proj(projpath)
#             try:
#                 return call(func,*params,**kv)
#             except Exception as e:
#                 raise e
#             finally:
#                 unlock_unity_proj(projpath)
#         return truedesc
#     return desc