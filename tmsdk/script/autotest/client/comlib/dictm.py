# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.dirname(__file__)
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com



import json,copy



class DictUtil(object):
    @staticmethod
    def isEmpty(d:dict):
        if d == None or d.__len__() == 0:
            return True
        return False
    @staticmethod
    def isSubDict(bigDict:dict,smallDict:dict):
        issub = True
        for k,v in smallDict.items():
            if k not in bigDict or bigDict[k] != v:
                issub = False
                break
        return issub
    @staticmethod
    def isSame(d1:dict,d2:dict):
        if d1 == None and d2 == None:
            return True
        if d1 == None or d2 == None:
            return False
        if d1.__len__() != d2.__len__():
            return False
        return DictUtil.isSubDict(d1,d2)
    @staticmethod
    def combine(fromD,toD) ->dict:
        '''
        只结合字典第一层级
        '''
        newToD = copy.deepcopy(toD)
        for k,v in fromD.items():
            newToD[k] = v
        return newToD
        
    @staticmethod
    def hasKey(d:dict,*keys):
        curD = d
        for key in keys:
            if key in curD:
                curD = curD[key]
            else:
                return False
        return True
    @staticmethod
    def tryGetValue(d,*keys):
        curD = d
        for key in keys:
            if key in curD:
                curD = curD[key]
            else:
                return None
        return curD
        # if key in d.keys():
        #     return d[key]
        # return None
    
    @staticmethod
    def findvalue(d:dict,value):
        '''
        深度优先搜索值为value的键路径
        '''
        pass
    @staticmethod
    def trysetvalue(d:dict,*keys,value=None):
        lastD = d
        curD = d
        klen = keys.__len__()
        for i in range(0,klen):
            key = keys[i]
            if i == klen -1:
                curD[key] = value
            elif key in curD:
                lastD = curD
                curD = curD[key]
            else:
                return False
        return True
    @staticmethod
    def hasVaildValue(d:dict,key):
        if DictUtil.hasKey(d,key) and d[key] not in (None,''):
            return True
        else:
            return False
    @staticmethod
    def checkRequire(d,*keys):
        for key in keys:
            if not DictUtil.hasKey(d,key):
                return False
        return True
    @staticmethod
    def setvalue(d,*keys,value=None):
        lastD = d
        curD = d
        klen = keys.__len__()
        for i in range(0,klen):
            key = keys[i]
            if i == klen -1:
                curD[key] = value
            elif key in curD:
                lastD = curD
                curD = curD[key]
            else:
                curD[key] = {}
                lastD = curD
                curD = curD[key]
    @staticmethod
    def remove(d,*keys):
        lastD:dict = d
        curD = d
        curKey = keys[0]
        klen = keys.__len__()
        for i in range(0,klen):
            curKey = keys[i]
            lastD = curD
            curD = curD[curKey]
        
        data = lastD.pop(curKey)
        return data
        
class JsonFile(object):
    def __init__(self,filepath='',jsondata_default='{}'):
        super().__init__()
        self.filepath = filepath
        self.jsondata = jsondata_default
        # if os.path.exists(filepath_or_jsondata):
        #     self.filepath = filepath_or_jsondata
        #     self._load()
        # else:
        #     self.jsondata = json.loads(filepath_or_jsondata)

        self._load()

        
    def _load(self):
        if os.path.exists(self.filepath):
            with open(self.filepath,'r',encoding='utf-8') as fs:
                self.jsondata = json.load(fs)
        elif self.jsondata.__len__() != 0:
            self.jsondata = json.loads(self.jsondata)

            
    
    def save(self,svncommit=False,msg='配置提交',savepath=None):
        if savepath == None:
            savepath = self.filepath
        with open(savepath,'w',encoding='utf-8') as fs:
            json.dump(self.jsondata,fs,ensure_ascii=False,indent=4)
        if svncommit:
            from comlib.svnm import SVNManager
            SVNManager.commit(savepath,f'需求修改([sdk]/[config]): {msg}')
    def haskey(self,*keys):
        return DictUtil.hasKey(self.jsondata,*keys)
    def trygetvalue(self,*keys):
        return DictUtil.tryGetValue(self.jsondata,*keys)
    def setvalue(self,*keys,value=None):
        DictUtil.setvalue(self.jsondata,*keys,value=value)
    def trysetvalue(self,*keys,value=None):
        return DictUtil.trysetvalue(self.jsondata,*keys,value=value)
    def getkv(self):
        return self.jsondata.items()
    def getkeys(self):
        return self.jsondata.keys()
    def remove(self,*keys):
        """
        删除指定key树的值
        """
        return DictUtil.remove(self.jsondata,*keys)
    def tryappendvalue2list(self,ls:list,*keys):
        value = self.trygetvalue(*keys)
        if value != None:
            ls.append(value)
    def clearEmpty(self):
        def inner(dic:dict):
            for k,v in list(dic.items()):
                if v in (None,{}):
                    dic.pop(k)
                elif isinstance(v,dict):
                    inner(v)
                    if v in (None,{}):
                        dic.pop(k)
                    else:
                        dic[k] = v
        inner(self.jsondata)


    def __len__(self):
        return self.jsondata.__len__()