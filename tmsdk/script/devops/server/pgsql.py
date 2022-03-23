from server import models
from server.pgsql_util import PGSQL_Util




from sqlalchemy import create_engine
from sqlalchemy import Column, String, Integer
from sqlalchemy.orm import sessionmaker
from sqlalchemy.ext.declarative import declarative_base

from comlib import com

# 以下是密码加密用
from werkzeug.security import generate_password_hash, check_password_hash




class PGSession(object):
    '''
    只在关闭时提交s
    '''
    def __init__(self,session):
        super().__init__()
        self.session = session
    def add(self,table,data):
        obj = PGSQL_Util.CreateObj(table,data)
        self.session.add(obj)
        return obj
    def select(self,obj):
        return self.session.query(obj)
    def close(self):
        self.session.commit()
        self.session.close()
    def addOrUpdate(self,table,data,filter_column):
        oldobj = self.getFirst(table,filter_column,data[filter_column])
        flag = 'add'
        if oldobj != None:
            flag = 'update'
            fData = PGSQL_Util.filterData(table,data)
            [setattr(oldobj,k,v) for k,v in fData.items()]
            newobj = oldobj
        else:
            newobj = self.add(table,data)
        return newobj,flag
    def addOrNothing(self,table,data,filter_column):
        obj = self.getFirst(table,filter_column,data[filter_column])
        flag = 'nothing'
        if obj == None:
            obj = self.add(table,data)
            flag = 'add'
        return obj,flag
    def array_addfirst(self,table,column,arraydata,schema='public'):
        '''
        向数组头部添加值46304
        '''
        self.execute(f'''UPDATE {schema}."{table.__tablename__}" SET "{column}"=array_prepend('{arraydata}',"{column}") WHERE "id"='{table.id}'; ''')
    def array_addfirst_notcontain(self,table,column,arraydata,schema='public'):
        '''
        向数组头部添加值之前，判断是否在数组内已经包含
        '''
        self.execute(f'''UPDATE {schema}."{table.__tablename__}" SET "{column}"=array_prepend('{arraydata}',"{column}") WHERE "id"='{table.id}' AND NOT '{arraydata}'=ANY("{column}"); ''')

    def execute(self,cmd):
        return self.session.execute(cmd)
    def executeAndFetchAll(self,cmd,hasColumeName=True):
        resProxy = self.session.execute(cmd)
        allres = resProxy.fetchall()
        res_list = []
        for res in allres:
            res_list.append(PGSQL_Util.fetchres2dict(res,hasColumeName=hasColumeName))
        return res_list
    def getFirst(self,table,filter_column,value):
        obj = self.select(table).filter_by(**{filter_column:value}).first()
        return obj
    def getLast(self,table,columnName='id'):
        '''
        适用于有自增id的表
        '''
        res = self.executeAndFetchAll(f'''SELECT last_value from public."{table.__tablename__}_{columnName}_seq"'''
        ,hasColumeName=False)
        
        res = com.unpackList(res)
        curId = res[0]
        res = self.getFirst(table,columnName,curId)
        return res
channel_table_ref = {}
class DBManager(object):
    def __init__(self,uri):
        super().__init__()
        #生成引擎
        self.engine = create_engine(uri,echo=False)
        alltable = self.engine.table_names()
        for tablename in alltable:
            if '_' in tablename:
                tmp = tablename.split('_')
                channel = tmp[1]
                if channel not in channel_table_ref:
                    self._createChannelModel(channel)


        #常见ORM基类
        self.Base = models.Base
        # 表不存在创建表
        self.Base.metadata.create_all(self.engine)

        # 3、创建DBSession类型:
        self.DBSession = sessionmaker(bind=self.engine)
    def getSession(self):
        # 4、创建session对象:
        return PGSession(self.DBSession())
    def add(self,*table_and_data):
        s = self.getSession()
        for table,data in table_and_data:
            s.add(table,data)
        s.close()

    def refresh(self):
        # 表不存在创建表
        self.Base.metadata.create_all(self.engine)
    def _createChannelModel(self,channel):
        CrashInfocls = type(f'CrashInfo_{channel}',(models.CrashInfo,),{'__tablename__':f'CrashInfo_{channel}'})
        IssueListcls = type(f'IssueList_{channel}',(models.IssueList,),{'__tablename__':f'IssueList_{channel}'})
        CrawLogcls = type(f'CrawLog_{channel}',(models.CrawLog,),{'__tablename__':f'CrawLog_{channel}'})
        YestodaySummarycls = type(f'YestodaySummary_{channel}',(models.YestodaySummary,),{'__tablename__':f'YestodaySummary_{channel}'})
        channel_table_ref[channel] = models.ChannelModel(CrashInfocls,IssueListcls,CrawLogcls,YestodaySummarycls)
    def getChannelModel(self,channel):
        if channel not in channel_table_ref:
            self._createChannelModel(channel)
            self.refresh()
        return channel_table_ref[channel]
    def getAllChannel(self):
        return channel_table_ref.keys()

    def getAllModels(self):
        return channel_table_ref.items()


buffer = {}

def GetDBM(dbname,user,password,host,port=5432):
    uri = f'postgresql://{user}:{password}@{host}:{port}/{dbname}'
    if uri not in buffer.keys():
        dbm = DBManager(uri)
        buffer[uri] = dbm
    else:
        dbm = buffer[uri]
    return dbm
def GetBuglyDBM():
    return GetDBM('postgres','postgres','123456','192.168.2.147')
def release():
    for dbm in buffer.values():
        # dbm.close()
        pass