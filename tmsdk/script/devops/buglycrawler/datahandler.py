# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com

from comlib import Path
from comlib import BinManager,DictUtil,workspace

import json
from comlib import Version,loadversion,loadversion_str,HTTPManager,一点零注意了机器人
import requests,threading,re

from comlib.comobj import *
def get_sum_default():
    return {
        'datetime':'0',
        'datetime_last':'0',
        'version':'1.1.1.1',
        'issue_count':0,
        'issue_delta':0,
        'crash_count':0,
        'crash_delta':0,
        'new_issues':[],
        'issues':{}
    }


def BooleanStandards(d):
    for k,v in d.items():
        if isinstance(v,str):
            if v.lower() == 'false':
                d[k] = False
            elif v.lower() == 'true':
                d[k] = True
    return d
def DatetimeStandards(tsp):
    date = com.tsp2datetime(tsp)
    ok = date.__str__().split('.')[0]
    return ok



downloadSymLock = threading.Lock()


def getSendJsonData(rawdata,method=None):
    data = {'data':rawdata}
    if method != None:
        data['method'] = method
    return data
def postdata(route,jsondata,method=None):
    data = getSendJsonData(jsondata,method)
    url = f'http://192.168.2.104:5000/{route}/'

    ret = requests.post(url,json=data)
    if ret.ok:
        data = json.loads(ret.content,encoding='utf-8')
        if not isOk(data):
            return data['msg'],False
        return getRetData(data),True
    return None,False
    
def isOk(data):
    if data['code'] != 200:
        return False
    return True
def getRetData(data):
    return data['data']

@errorcatch
class DataHandler(object):
    @staticmethod
    def isFirstCrawToday():
        res,ok = postdata('search',{},method='isFirstCrawToday')
        return res
    @staticmethod
    def getDeltaDaySummry(channel,deltaday,version):
        ret,ok = postdata('search',{'channel':channel,'deltaday':deltaday,'version':version},method='getDeltaDaySummry')
        return ret
    @staticmethod
    def getSummry(channel,version):
        data= {channel:{'version':version}}
        ret = postdata('search',data,'getSummry')
        return ret
    @staticmethod
    def sendLog(channel,version,issueIds):
        time = com.timemark_datetime
        postdata('save',{'channel':channel,'time':time,'version':version,'issueIds':issueIds},method='saveLog')
        
    @staticmethod
    def isExistsCrashInDB(channel,crashHash):
        data = {'channel':channel,'crashHash':crashHash}
        ret,ok = postdata('search',data,method='isExistsCrash')
        if not ok:
            return False
        if ret == '1':
            return True
        return False
    @staticmethod
    def upload_DB(branch,issue,crashinfo):   
        issueDict = issue.__dict__
        issueDict['id'] = int(issueDict['issueId'])
        issueDict.pop('issueId')
        # issueDict['createTime'] = com.getdatetimenow()
        # 等会改
        issueDict['branch'] = branch
        
        crashinfo = crashinfo['crashMap']
        data = {'issue':issueDict,'info':crashinfo}
        data,ok = postdata('save',data,'saveData')
        if not ok:
            print('**********************************************************************')
            print('**********************************************************************')
            print(issueDict)
            print('**********************************************************************')
            print(crashinfo)
            print('**********************************************************************')
            print(data)
            print('**********************************************************************')
    @staticmethod
    def filter_crashinfo(crashinfo,branch,channel,appid,platformId):
        need = {}
        # 去除无用的数据
        need['crashMap'] = crashinfo['ret']['crashMap']
        need['crashMap']['detailMap'] = crashinfo['ret']['detailMap']

        # need['crashMap'].pop('mappingMD5')
        
        crashMap = need['crashMap']
        detailMap = need['crashMap']['detailMap']

        # bugly数据不标准
        crashMap['crashHash'] = crashMap['id']
        # crashMap['id'] = crashMap['crashId']
        crashMap.pop('id')
        # 'false' => False
        crashMap = BooleanStandards(crashMap)
        try:
            # 1594462960728 => 2020-07-11 18:31:07
            if int(crashMap['startTime']) < 0:
                crashMap['startTime'] = 0
            crashMap['startTime'] = DatetimeStandards(crashMap['startTime'])
        except Exception as e:
            print(1)

        
        
        # 定制信息
        extData = crashMap['userId']
        DataHandler.parseExt(crashMap,extData)
        crashMap['branch'] = branch
        crashMap['channel'] = channel
        crashMap['appid'] = appid
        crashMap['platformId'] = platformId
        


        if DictUtil.hasKey(crashMap,'mappingMD5'):
            crashMap.pop('mappingMD5')

        if DictUtil.hasVaildValue(crashMap,'retraceCrashDetail'):
            rawcrash = crashMap['retraceCrashDetail']
        else:
            rawcrash = crashMap['callStack']
        # 有libil2cpp.so崩溃就dump
        if 'libil2cpp.so' in rawcrash:
            version = crashMap['productVersion']
            dumpcrash = DataHandler.dump_crash(rawcrash,version)
            crashMap['retraceCrashDetail'] = dumpcrash
            detailMap['retraceCrashDetail'] = dumpcrash
            crashMap['isDumped'] = True
        else:
            crashMap['isDumped'] = False
        return need
    @staticmethod
    def parseExt(crashMap,extData):
        def setSimulater(flag):
            if flag == 'MP':
                crashMap['isSimulater'] = False
            else:
                crashMap['isSimulater'] = True
                crashMap['simulater'] = flag
        def getHead(s,flagFunc):
            ret = ''
            for c in s:
                if flagFunc(c):
                    ret += c
                else:
                    break
            return ret
        def getTail(s,flagFunc,maxCount=2):
            ret = ''
            for c in s[-1:-1-maxCount:-1]:
                if flagFunc(c):
                    ret += c
                else:
                    break
            return ret[::-1]
        # 37058698 | 上上上         | town    |        1565
        tmp = extData.split('_')
        # 兼容旧的
        if tmp.__len__() == 1:
            crashMap['player'] = tmp[0]
        # 正式服
        elif tmp.__len__() == 2:
            crashMap['playerName'] = tmp[-2]
            crashMap['playerLevel'] = tmp[-1]
        # 体验服
        # 雷电_346 MB_0.00MB_Town残暴人被屠65
        # MP_1.97 GB_572.97MB_Town
        elif tmp.__len__() == 4:
            simuFlag = tmp[0]
            setSimulater(simuFlag)
            locate = getHead(tmp[-1],lambda x : re.match(r'[a-zA-Z]+',x) != None)
            level = getTail(tmp[-1],lambda x : re.match(r'[\d]+',x) != None)
            if level in (None,''):
                level = '0'
            playerName = tmp[-1][locate.__len__():tmp[-1].__len__() - level.__len__()]
            crashMap['locate'] = locate.lower()
            crashMap['playerLevel'] = int(level)
            crashMap['playerName'] = playerName
        elif tmp.__len__() == 5:
            simuFlag = tmp[0]
            #                  地下城id   session
            # 雷电_346 MB_0.00MB_856003_3388058663549985988残暴人被屠65
            if tmp[-1][0:10].isnumeric():
                setSimulater(simuFlag)
                session = getHead(tmp[-1],lambda x : re.match(r'[\d]+',x) != None)
                level = getTail(tmp[-1],lambda x : re.match(r'[\d]+',x) != None)
                if level in (None,''):
                    level = '0'
                playerName = tmp[-1][session.__len__():tmp[-1].__len__() - level.__len__()]
                crashMap['locate'] = 'dungeon'
                crashMap['locateId'] = tmp[-2]
                crashMap['session'] = session
                crashMap['playerName'] = playerName
                crashMap['playerLevel'] = int(level)
        # 雷电_346 MB_0.00MB_Town_残暴人被屠_65
        # MP_1.97 GB_572.97MB_Town__
        elif tmp.__len__() == 6:
            simuFlag = tmp[0]
            setSimulater(simuFlag)
            level = tmp[-1]
            if level in (None,''):
                level = '0'
            crashMap['locate'] = tmp[-3]
            crashMap['playerName'] = tmp[-2]

            crashMap['playerLevel'] = int(level)
        # 雷电_346 MB_0.00MB_856003_3388058663549985988_残暴人被屠_65
        # # MP_1.97 GB_572.97MB_856003_3388058663549985988__
        elif tmp.__len__() == 7:
            simuFlag = tmp[0]
            setSimulater(simuFlag)
            level = tmp[-1]
            if level in (None,''):
                level = '0'
            crashMap['locate'] = 'dungeon'
            crashMap['locateId'] = tmp[-4]
            crashMap['session'] = tmp[-3]
            crashMap['playerName'] = tmp[-2]
            crashMap['playerLevel'] = int(level)

    @workspace
    @staticmethod
    def upload_crashinfo(branch,crashinfo,datetime):
        json_tmpname = com.get_random_filename('.')
        jsstr = json.dumps(crashinfo,ensure_ascii=False,indent=4)
        
        com.savedata(jsstr,json_tmpname)
        # datetime = com.getlocaltime('-')
        issueID = crashinfo['crashMap']['issueId']
        # branch = "pre"
        ftppath_json = f'/DevOps/BuglyData/{branch}/data/{datetime}/issue/{issueID}.json'
        
        G_ftp.upload(json_tmpname,ftppath_json,showlog=True,overwrite=True)
    @workspace
    @staticmethod
    def summary(userID,branch,version,datetime,issues,crashinfo,pid='1'):

        DataHandler.upload_sum(userID,branch,version,datetime,issues,crashinfo)
        DataHandler.upload_datetime(branch,datetime)

    @staticmethod
    def upload_sum(userID,branch,version,datetime,issues,crashinfo,pid='1'):
        issue_count = crashinfo.__len__()
        crash_count = sum([i.count for i in issues])
        datetime_url = f'http://192.168.2.147/dnl/DevOps/BuglyData/{branch}/newest.txt'
        datetime_last = HTTPManager.getbody(datetime_url)
        sum_last = HTTPManager.getbody(f'http://192.168.2.147/dnl/DevOps/BuglyData/{branch}/data/{datetime_last}/sum.json',decode='json')
        if sum_last == None:
            sum_last = get_sum_default()
        issue_delta = issue_count - sum_last['issue_count']
        crash_delta = crash_count - sum_last['crash_count']
        allissues_last = sum_last['issues'].keys()

        newissues = set()
        issues_struct = {}
        for issue in issues:
            issues_struct[issue.issueId] = {
                'count':issue.count,
                'trend':0
                # 'exists_version':[] 改用数据库再加
            }
            if issue.issueId not in allissues_last:
                newissues.add(issue.issueId)
                # newissues.append(issue.issueId)
                issues_struct[issue.issueId]['trend'] = issue.count
            else:
                issues_struct[issue.issueId]['trend'] = issue.count - sum_last['issues'][issue.issueId]['count']
        sum_new = {
            'datetime':datetime,
            'datetime_last':datetime_last,
            'version':version,
            'issue_count':issue_count,
            'issue_delta':issue_delta,
            'crash_count':crash_count,
            'crash_delta':crash_delta,
            'new_issues':list(newissues),
            'issues':issues_struct
        }
        sum_filepath = com.get_random_filename('.')
        com.dumpfile_json(sum_new,sum_filepath)
        path = f'/DevOps/BuglyData/{branch}/data/{datetime}/sum.json'
        G_ftp.upload(sum_filepath,path,showlog=True,overwrite=False)

        title = f'{branch}_{version}_{datetime}新增issue'
        content = f'{title}\n\n'
        for ni in newissues:
            # https://bugly.qq.com/v2/crash-reporting/crashes/fc1ccbfe45/376680?pid=1
            url = f'https://bugly.qq.com/v2/crash-reporting/crashes/{userID}/{ni}?pid={pid}'
            content += 一点零注意了机器人.markdown_textlink(ni,url) +'===='
        data = 一点零注意了机器人.build_markdown(title,content)
        一点零注意了机器人.send(data)

    @staticmethod
    def upload_datetime(branch,datetime):
        
        tmpname = com.get_random_filename('.')
        path = f'/DevOps/BuglyData/{branch}/newest.txt'
        com.savedata(datetime,tmpname)
        G_ftp.upload(tmpname,path,showlog=True,overwrite=True)
    @staticmethod
    def dump_crash(crash:str,symversion):
        sympath = DataHandler.ensure_symexists(symversion)
        return DataHandler._dump_crash(crash,sympath)
    @staticmethod
    def _dump_crash(crash:str,sympath):
        # 必须指定架构
        sympath = os.path.join(sympath,'armeabi-v7a')
        sub,code = BinManager.ndkstack(f'-sym {sympath}',getPopen=True)
        # 添加头部
        if crash[0] != '*':
            crash = '*** *** *** *** *** *** *** *** *** *** *** *** *** *** *** ***\npid: 1, tid: 1\n' + crash
        out,err = sub.communicate(crash)
        ret = sub.poll()
        if isinstance(ret,int) and ret != 0:
            DingException('ndk解析出错了',locals())
        
        return out
        
    @staticmethod
    def ensure_symexists(version):
        downloadSymLock.acquire()

        inlocal = False
        sympath = ''
        
        v = loadversion_str(version)
        symversion_str = f'1_30_1_{v.client}'

        Path.ensure_direxsits('syms')
        for fd in os.listdir('syms'):
            if symversion_str in fd:
                inlocal = True
                sympath = os.path.join('syms',fd)
                break
        if not inlocal:
            allsyms = G_ftp.listdir('/__androidSymbols')
            for rfd in allsyms:
                if symversion_str in rfd:
                    G_ftp.download('syms',f'/__androidSymbols/{rfd}')
                    sympath = os.path.join('syms',rfd)
                    break
        if sympath == '':
            raise StopException('没找到符号表',locals())

        downloadSymLock.release()
        return sympath
def run():
    version = ''
    version = f'1_30_1_{version}'
    Path.ensure_direxsits('syms')
        
    pass


if __name__ == "__main__":
    # f = com.datetimeFromat(com.datetimenow_day())
    # ret,ok = postdata('search',{'channel':'mg','deltaday':1,'version':'1.37.1.211795'},method='getDeltaDaySummry')
    # print(ret)
    # DataHandler.sendLog('mg','1.2.3.4',[1,2,3,4])
    ret = DataHandler.isFirstCrawToday()
    print(ret)
    # res = DataHandler.isExistsCrashInDB('9243')
    # print(res)
    pass