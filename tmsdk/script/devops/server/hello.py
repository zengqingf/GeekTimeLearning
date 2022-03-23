# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com

from comlib import DictUtil,ThreadManager

from flask import Flask,redirect,url_for,request,render_template

from server import models
from server import pgsql
from server.pgsql_util import PGSQL_Util



import json

from flask_cors import CORS
from werkzeug.datastructures import Headers
from werkzeug.wrappers import Response




class MyResponse(Response):
    def __init__(self, response=None, **kwargs):
        # kwargs['headers'] = ''
        headers = kwargs.get('headers')
        # 跨域控制 
        origin = ('Access-Control-Allow-Origin', '*')
        methods = ('Access-Control-Allow-Methods', 'HEAD, OPTIONS, GET, POST, DELETE, PUT')
        if headers:
            headers.add(*origin)
            headers.add(*methods)
        else:
            headers = Headers([origin, methods])
        kwargs['headers'] = headers
        return super().__init__(response, **kwargs)



app = Flask(__name__)
# app.response_class = MyResponse
CORS(app, supports_credentials=True)



dbm = pgsql.GetBuglyDBM()

def getSucRetContent(data):
    content = {
        'code': 200,
        'data':data
    }
    return json.dumps(content)
def getFailRetContent(code,msg):
    content = {
        'code':code,
        'msg':msg
    }
    return json.dumps(content)


searchType = ['崩溃地点','角色名']
def getJsonData(rawData):
    return json.loads(rawData['data'],encoding='utf-8')

def removeKey(removeList,d):
    for rk in removeList:
        d.pop(rk)
    return d
def CreateObj(struct,data):
    removekey = []
    for k,v in data.items():
        if k not in struct.__dict__.keys():
            print(f'not {k}')
            removekey.append(k)
    data = removeKey(removekey,data)
    return struct(**data)


@app.route('/',methods=['GET'])
def index():
    issueId = ''
    channel = ''
    if request.method == 'GET':
        data = tryGetData_base(request)
        channel = DictUtil.tryGetValue(data,'channel')
        issueId = DictUtil.tryGetValue(data,'issueId')
    if issueId == None:
        issueId = ''
    return render_template('index.html',channel=channel,issueId=issueId)
@app.route('/yestoday/',methods=['GET'])
def yestoday():
    if request.method == 'GET':
        session = dbm.getSession()
        lastDaySummry = {}
        for channel,channelModel in dbm.getAllModels():
            row = session.getLast(channelModel.YestodaySummarycls)
            lastDaySummry[channel] = {'updatetime':row.updatetime,'issueids':row.issueIds}
        
        return render_template('lastDaySummry.html',lastDaySummry=lastDaySummry)
    return ''
@app.route('/save/',methods=['POST'])
def save():
    if request.method == 'POST':
        code = 200
        try:
            method,data = tryGetData(request)
            if method == 'saveData':
                save_saveData(data)
            elif method == 'saveLog':
                save_saveLog(data)
            
        except Exception as e:
            import traceback
            traceback.print_exc()
            print(method)

            print(data)
            code = 44444
        finally:
            if code == 200:
                return getSucRetContent('suc')
            else:
                return getFailRetContent(code,'save fail')

def save_saveLog(data):
    if not DictUtil.checkRequire(data,'channel','time','version','issueIds'):
        return getFailRetContent(654321,'require params not enough')


    init_lastDaySummry(data['channel'],data['version'])


    channelModel = dbm.getChannelModel(data['channel'])
    session = dbm.getSession()
    # print(data)
    row = session.add(channelModel.CrawLogcls,data)
    session.close()




def save_saveData(data):
    issueJSON = data['issue']
    crashInfoJSON = data['info']
    channel = crashInfoJSON['channel']
    channelModel = dbm.getChannelModel(channel)

    session = dbm.getSession()
    infoObj,flag = session.addOrNothing(channelModel.CrashInfocls,crashInfoJSON,'crashHash')
    session.close()
    if flag == 'nothing':
        print(f"[Exists]{crashInfoJSON['crashHash']}")
    else:
        if crashInfoJSON['crashTime'] not in ('',None):
            issueJSON['lastestUploadTime'] = crashInfoJSON['crashTime']
        
        session = dbm.getSession()
        row,flag = session.addOrUpdate(channelModel.IssueListcls,issueJSON,'id')
        if flag == 'update':
            session.array_addfirst(row,'crashHashs',crashInfoJSON['crashHash'])
            session.array_addfirst_notcontain(row,'versions',crashInfoJSON['productVersion'])
        else:
            row.crashHashs = [crashInfoJSON['crashHash']]
            row.versions = [crashInfoJSON['productVersion']]
            row.createTime = com.getdatetimenow()
        session.close()
        print(f"[save] ----------------------------------------------------------------")
        print(f"[save] issueJSON {issueJSON['id']} crashInfoJSON {crashInfoJSON['crashHash']} ")
        print(f"[save] ----------------------------------------------------------------")

support={
    'time':{
        'day1':'今天',
        'day2':'两天内',
        'day3':'三天内',
        'day7':'七天内',
        'day30':'三十天内'
    },
    'searchType':{
        'name':'姓名查询',
        'locate':'地点查询'
    }
}
# lastDaySummry = {}

def init():
    init_branch_task = ThreadManager.taskGo(init_channel,count=1,timeSpan=3)
    # init_lastDaySummry_task

def init_lastDaySummry(channel,version):
    # allChannel = dbm.getAllChannel()
    
    isFirst = json.loads(search_isFirstCrawToday(''))['data']
    if not isFirst[channel]:
        return

    data = {'channel':channel,'deltaday':1,'version':version}
    res = json.loads(search_getDeltaDaySummry(data))
    channelModel = dbm.getChannelModel(channel)
    session = dbm.getSession()
    issues = res['data']
    issueIds = [x['id'] for x in issues]
    updatetime = com.getdatetimenow()
    session.add(channelModel.YestodaySummarycls,{'issueIds':issueIds,'updatetime':updatetime})

    # lastDaySummry[channel] = {'issues':issues,'updatetime':com.getdatetimenow()}
    
    session.close()

def init_channel(self):
    allChannel = dbm.getAllChannel()
    session = dbm.getSession()
    channelDict = {}
    for channel in allChannel:
        
        versionList = session.executeAndFetchAll(f'''
        SELECT DISTINCT "productVersion"
        FROM public."CrashInfo_{channel}"
        ORDER BY "productVersion" DESC
        LIMIT 10;
        ''',hasColumeName=False)
        versionList = com.unpackList(versionList)
        key = f'{channel}'
        channelDict[key] = {'version':versionList}

    support['channel'] = channelDict
    session.close()



def tryGetData_base(request):
    js = {}
    
    if request.args.__len__() != 0:
        js = request.args.to_dict()
    elif request.form.__len__() != 0:
        js = request.form.to_dict()
    elif request.json != None:
        js = request.json
    elif request.data != None:
        pass
    elif request.files.__len__() != 0:
        pass

    return js
def tryGetData(request):
    js = {}
    
    if request.args.__len__() != 0:
        js = request.args.to_dict()
    elif request.form.__len__() != 0:
        js = request.form.to_dict()
    elif request.json != None:
        js = request.json
    elif request.data != None:
        pass
    elif request.files.__len__() != 0:
        pass
    data = DictUtil.tryGetValue(js,'data')
    method = DictUtil.tryGetValue(js,'method')
    return method,data
        


@app.route('/search/',endpoint='search',methods=['POST','GET'])
def search():
    if request.method == 'POST':
        method,data = tryGetData(request)
        if method == None:
            return getFailRetContent(123456,'method empty')
        print(f'[search] [{method}] {data}')
        # ret = None
        if method == 'lastCrash':
            return search_lastCrash(data)
        elif method == 'crashSearch':
            return search_crashSearch(data)
        elif method == 'showAll':
            return search_showAll(data)
        elif method == 'isExistsCrash':
            return search_isExistsCrash(data)
        elif method == 'getIssue':
            return search_getIssue(data)
        elif method == 'getSummry':
            return search_getSummry(data)
        elif method == 'getDeltaDaySummry':
            return search_getDeltaDaySummry(data)
        elif method == 'isFirstCrawToday':
            return search_isFirstCrawToday(data)
        return getFailRetContent(123456,'method unsupport')
    elif request.method == 'GET':
        method = request.args.get('method')
        if method != None:
            if method == 'init':
                return json.dumps(support)
        return render_template('search.html')
def search_isFirstCrawToday(data):
    if not DictUtil.checkRequire(data):
        return getFailRetContent(654321,'require params not enough')

    
    session = dbm.getSession()
    curDate = com.datetimenow()
    curYear = curDate.year
    curMot = curDate.month
    curDay = curDate.day

    ret = {}
    for channel,channelmodels in dbm.getAllModels():
        log = getLastCrawLog(session,channelmodels)
        if log == None or curYear != log.time.year or curMot != log.time.month or curDay != log.time.day:
            ret[channel] = True
        else:
            ret[channel] = False

    return getSucRetContent(ret)
         

def search_getDeltaDaySummry(data):
    if not DictUtil.checkRequire(data,'channel','deltaday','version'):
        return getFailRetContent(654321,'require params not enough')

    channel = data['channel']
    version = data['version']
    # fromday = data['fromday']
    # today = data['today']
    deltaday = data['deltaday']
    nowdatestr = com.getdatetimenow()
    # tmp = com.datetimenow_day()
    tmp = com.datetimenow()
    fromdate = com.getdatetime_afterDelta(tmp,deltaday)
    fromdatestr = com.datetimeFromat(fromdate)
    sql = f'''
    SELECT * FROM public."IssueList_{channel}" 
    WHERE '{version}'=ANY("versions")
    AND ("lastestUploadTime" BETWEEN '{fromdatestr}' AND '{nowdatestr}')
    AND ("createTime" BETWEEN '{fromdatestr}' AND '{nowdatestr}');
    '''
    print(sql)
    session = dbm.getSession()
    res = session.executeAndFetchAll(sql)
    session.close()

    return getSucRetContent(res)
def getLastCrawLog(session,channelmodels):
    res = session.executeAndFetchAll(f'''SELECT last_value from public."{channelmodels.CrawLogcls.__tablename__}_id_seq"'''
    ,hasColumeName=False)
    
    res = com.unpackList(res)
    curId = res[0]
    res = session.getFirst(channelmodels.CrawLogcls,'id',curId)
    return res

def search_getSummry(data):
    # if not DictUtil.checkRequire(data,'version'):
    #     return getFailRetContent(654321,'require params not enough')
    
    session = dbm.getSession()
    newIssueDict = {}
    for channel,channelmodels in dbm.getAllModels():
        if channel not in data:
            continue
        curdata = data[channel]
        # 需求version 选填lasttime
        res = session.executeAndFetchAll(f'''SELECT last_value from public."{channelmodels.CrawLogcls.__tablename__}_id_seq"'''
        ,hasColumeName=False)
        
        res = com.unpackList(res)

        curId = res[0]
        
        res = session.getFirst(channelmodels.CrawLogcls,'id',curId)
        version = ''
        issueIds = []
        if res != None:
            lastTime = com.datetimeFromat(res.time)
            version = res.version
            issueIds = res.issueIds
        # 新表无数据
        else:
            # 从昨天0时开始
            lastTime = com.datetimeFromat(com.getdatetime_afterDelta(com.datetimenow_day(),1))
            version = curdata['version']
        if 'lasttime' in curdata:
            lastTime = curdata['lasttime']
        
        if version != curdata['version']:
            version = curdata['version']

        nowTimestr = com.getdatetimenow()
        sql = f'''
        SELECT * FROM public."IssueList_{channel}" 
        WHERE '{version}'=ANY("versions")
        AND "createTime" BETWEEN '{lastTime}' AND '{nowTimestr}';
        '''
        res = session.executeAndFetchAll(sql)
        newIssue = []
        for r in res:
            if r['id'] not in issueIds:
                newIssue.append(r)
        newIssueDict[channel] = {'issues':newIssue,'lasttime':lastTime}

    session.close()

    return getSucRetContent(newIssueDict)
    # 计算上次爬取时间差
    # 获取时间差内的issue
    # 根据新增issue获取是否dump
    # 发送
def search_getIssue(data):
    if not DictUtil.checkRequire(data,'channel','id'):
        return getFailRetContent(654321,'require params not enough')

    channelModel = dbm.getChannelModel(data['channel'])
    session = dbm.getSession()
    res = session.getFirst(channelModel.IssueListcls,'id',data['id'])
    ret = None
    if res != None:
        ret = PGSQL_Util.row2dict(res)
        
    session.close()
    return getSucRetContent(ret)

def search_isExistsCrash(data):
    if not DictUtil.checkRequire(data,'channel','crashHash'):
        return getFailRetContent(654321,'require params not enough')

    channelModel = dbm.getChannelModel(data['channel'])
    session = dbm.getSession()
    res = session.getFirst(channelModel.CrashInfocls,'crashHash',data['crashHash'])
    session.close()
    if res != None:
        return getSucRetContent('1')
    return getSucRetContent('0')


def search_showAll(data):
    if not DictUtil.checkRequire(data,'channel','version','time'):
        return getFailRetContent(654321,'require params not enough')

    channel = data['channel']
    searchTime_sql = get_searchtime_sql(data['time'])
    IssueListTabelName = f'public."IssueList_{channel}"'
    CrashInfoTabelName = f'public."CrashInfo_{channel}"'
    sql = f'''
    SELECT "apn","appInBack","brand","buildNumber","bundleId","country","cpuName","cpuType","deviceId","diskSize","expAddr","expName",
    "freeStorage","freeSdCard","freeMem","isRooted","memSize","model","modelOriginalName","osVer","processName","productVersion",
    "rom","sdkVersion","sendProcess","sendType","totalSD","threadName","userId","locate","playerName","isSimulater","simulater",
    "issueId","crashTime","startTime","playerLevel","locateId","session",
    {IssueListTabelName}."id","count",{IssueListTabelName}."lastestUploadTime",
    {CrashInfoTabelName}."branch",{CrashInfoTabelName}."channel",
    "isDumped"
    FROM {IssueListTabelName}
    INNER JOIN {CrashInfoTabelName}
    ON {IssueListTabelName}."crashHashs"[1] = {CrashInfoTabelName}."crashHash"
    WHERE "channel"='{channel}'
    AND "productVersion"='{data['version']}' 
    AND {searchTime_sql}
    ORDER BY "crashTime" DESC
    LIMIT 200;
    '''
    print(sql)
    session = dbm.getSession()
    allres = session.executeAndFetchAll(sql)
    jsonstr = com.dict2jsonstr(allres)
    session.close()
    return jsonstr
    


def get_searchtime_sql(deltatime,totime=None):
    from datetime import datetime,timedelta

    if totime == None:
        now = com.getdatetimenow()

    now_tmp = datetime.fromisoformat(now)
    fromtime = datetime(now_tmp.year,now_tmp.month,now_tmp.day)
    
    if deltatime == 'day1':
        delta = timedelta()
    elif deltatime == 'day2':
        delta = timedelta(1)
    elif deltatime == 'day3':
        delta = timedelta(2)
    elif deltatime == 'day7':
        delta = timedelta(6)
    # elif data['time'] == 'day30':
    else:
        delta = timedelta(29)
    fromtime -= delta
    fromtime = com.datetimeFromat(fromtime)
    searchTime_sql = f'''"crashTime" BETWEEN '{fromtime}' AND '{now}' '''

    return searchTime_sql
def search_crashSearch(data):
    if not DictUtil.checkRequire(data,'channel','version','searchType','time','searchStr'):
        return getFailRetContent(654321,'require params not enough')

    if data['searchType'] == 'locate':
        searchType_sql = f'''"locate"='{data['searchStr']}' '''
    else:
        searchType_sql = f'''"playerName"='{data['searchStr']}' '''
    
    searchTime_sql = get_searchtime_sql(data['time'])

    channel = data['channel']
    IssueListTabelName = f'public."IssueList_{channel}"'
    CrashInfoTabelName = f'public."CrashInfo_{channel}"'

    sql = f'''
    SELECT "apn","appInBack","brand","branch","buildNumber","bundleId","country","cpuName","cpuType","deviceId","diskSize","expAddr","expName",
    "freeStorage","freeSdCard","freeMem","isRooted","memSize","model","modelOriginalName","osVer","processName","productVersion",
    "rom","sdkVersion","sendProcess","sendType","totalSD","threadName","userId","locate","playerName","isSimulater","simulater",
    "issueId","crashTime","startTime","playerLevel","locateId","session","isDumped"
    FROM {CrashInfoTabelName}
    WHERE "channel"='{channel}'
    AND "productVersion"='{data['version']}' 
    AND {searchType_sql} 
    AND {searchTime_sql}
    ORDER BY "crashTime" DESC
    LIMIT 200;
    '''
    print(sql)
    session = dbm.getSession()
    allres = session.executeAndFetchAll(sql)
    jsonstr = com.dict2jsonstr(allres)
    session.close()
    return jsonstr


def search_lastCrash_SQL(channel,issueId):
    IssueListTabelName = f'public."IssueList_{channel}"'
    CrashInfoTabelName = f'public."CrashInfo_{channel}"'

    session = dbm.getSession()
    resProxy = session.execute(f'''
    CREATE TEMP TABLE tmp1 ON COMMIT DROP AS (select *
    FROM {IssueListTabelName}
    WHERE {IssueListTabelName}."id" = '{issueId}');

    SELECT "apn","appInBack","brand","buildNumber","bundleId","country","cpuName","cpuType","deviceId","diskSize","expAddr","expName",
    "freeStorage","freeSdCard","freeMem","isRooted","memSize","model","modelOriginalName","osVer","processName","productVersion",
    "rom","sdkVersion","sendProcess","sendType","totalSD","threadName","userId","locate","playerName","isSimulater","simulater",
    "issueId","callStack","retraceCrashDetail","detailMap","crashTime","startTime","playerLevel","locateId","session","count",
    "imeiCount","exceptionName","status","lastestUploadTime","createTime"
    FROM tmp1
    INNER JOIN {CrashInfoTabelName}
    ON tmp1."crashHashs"[1] = {CrashInfoTabelName}."crashHash";
    ''')
    res = resProxy.fetchone()
    
    res_dict = PGSQL_Util.fetchres2dict(res)
    res_jsonstr = com.dict2jsonstr(res_dict)
    session.close()
    return res_jsonstr
def search_lastCrash(d):
    # session = dbm.getSession()
    if not DictUtil.checkRequire(d,'channel','issueId'):
        return getFailRetContent(654321,'require params not enough')
    if d['issueId'] == None:
        return getFailRetContent(654321,'require params not enough')
    return search_lastCrash_SQL(d['channel'],d['issueId'])


if __name__ == "__main__":
    init()
    # init_lastDaySummry('mg','1.37.1.211795')
    # init_lastDaySummry('7977','1.37.1.211800')
    app.run('192.168.2.104', 5000,debug=False)    
    pgsql.release()

    # data = {'channel':'mg','time':com.timemark_datetime,'version':'1.2.3.4','issueIds':[1,2,3,4]}
    # channelModel = dbm.getChannelModel(data['channel'])
    # session = dbm.getSession()
    # print(data)
    # row = session.add(channelModel.CrawLogcls,data)
    # session.close()

    pass

