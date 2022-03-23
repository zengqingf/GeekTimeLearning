# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com


FailFlag = 'Fail'
class PGSQL_Util():
    @staticmethod
    def fetchres2dict(fetchres,hasColumeName=True):
        if fetchres == None:
            return None
        if hasColumeName:
            dd = {}
            for col,val in fetchres.items():
                val = com.valueFormat(val)
                dd[col] = val
        else:
            dd = []
            for col,val in fetchres.items():
                val = com.valueFormat(val)
                dd.append(val)
        return dd
    @staticmethod
    def row2dict(row):
        dd = {}
        for k,v in row.__dict__.items():
            if k != '_sa_instance_state':
                v = com.valueFormat(v)
                dd[k] = v
        return dd
    @classmethod    
    def removeKey(cls,removeList,d):
        for rk in removeList:
            d.pop(rk)
        return d
    @classmethod
    def filterData(cls,struct,data):

        removekey = []
        for k,v in data.items():
            if k not in struct.__dict__.keys():
                # print(f'not {k}')
                removekey.append(k)
        data = cls.removeKey(removekey,data)
        return data
    @classmethod
    def CreateObj(cls,struct,data):
        data = cls.filterData(struct,data)
        return struct(**data)
