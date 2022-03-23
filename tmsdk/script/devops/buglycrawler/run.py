# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com


from comlib import 内网发包群苦工,一点零注意了机器人,SVNManager,ThreadManager,BinManager,Factory
from comlib.conf.ref import buglycrawlerconf
from comlib.conf.loader import Loader


from buglycrawler.datahandler import DataHandler

import requests
from http import cookiejar
from requests.adapters import HTTPAdapter
from requests.packages.urllib3.util.ssl_ import create_urllib3_context

import time
import re,pickle,execjs,random,subprocess
import json
import threading



from queue import Queue,Empty


# This is the 2.11 Requests cipher string, containing 3DES.
CIPHERS = (
    'ECDH+AESGCM:DH+AESGCM:ECDH+AES256:DH+AES256:ECDH+AES128:DH+AES:ECDH+HIGH:'
    'DH+HIGH:ECDH+3DES:DH+3DES:RSA+AESGCM:RSA+AES:RSA+HIGH:RSA+3DES:!aNULL:'
    '!eNULL:!MD5'
)


class DESAdapter(HTTPAdapter):
    """
    A TransportAdapter that re-enables 3DES support in Requests.
    """
    def init_poolmanager(self, *args, **kwargs):
        context = create_urllib3_context(ciphers=CIPHERS)
        kwargs['ssl_context'] = context
        return super(DESAdapter, self).init_poolmanager(*args, **kwargs)

    def proxy_manager_for(self, *args, **kwargs):
        context = create_urllib3_context(ciphers=CIPHERS)
        kwargs['ssl_context'] = context
        return super(DESAdapter, self).proxy_manager_for(*args, **kwargs)



class FSNException(Exception):
    pass

class IssueStruct(object):
    def __init__(self,crashNum,exceptionName,exceptionMessage,keyStack,lastestUploadTime,issueId:str,imeiCount,processor,status,tagInfoList,count,version,ftName,issueVersions):
        super().__init__()
        self.crashNum = crashNum
        self.exceptionName = exceptionName
        self.exceptionMessage=exceptionMessage
        self.keyStack=keyStack
        self.lastestUploadTime=lastestUploadTime
        self.issueId=issueId
        self.imeiCount=imeiCount
        self.processor=processor
        self.status=status
        self.tagInfoList=tagInfoList
        self.count=count
        self.version=version
        self.ftName=ftName
        self.issueVersions=issueVersions
    def __hash__(self):
        return self.issueId


first_num = random.randint(70, 83)
third_num = random.randint(3200, 4103)
fourth_num = random.randint(0, 140)

class FakeChromeUA:
    os_type = [
                '(Windows NT 6.1; WOW64)', '(Windows NT 10.0; WOW64)', '(X11; Linux x86_64)',
                '(Macintosh; Intel Mac OS X 10_12_6)'
               ]
 
    chrome_version = 'Chrome/{}.0.{}.{}'.format(first_num, third_num, fourth_num)
 
    @classmethod
    def get_ua(cls):
        return ' '.join(['Mozilla/5.0', random.choice(cls.os_type), 'AppleWebKit/537.36',
                         '(KHTML, like Gecko)', cls.chrome_version, 'Safari/537.36', 'Edg/83.0.478.64']
                        )

class NetworkManager(object):
    def __init__(self,requester,newRequesterFunc,headers,proxies=None):
        super().__init__()
        self.reconnectThds = []
        self.reconnectLock = threading.Lock()
        self.requester = requester
        self.newRequesterFunc = newRequesterFunc
        self.headers = headers
        self.proxies = proxies

        self.isReconecting = False
        
    def get(self,url,headers,encoding='utf-8',toJson=True,allow_redirects=False):
        ret = None
        
        if self.reconnectLock.locked():
            ThreadManager.waitall(self.reconnectThds)
        maxRetry = 10
        curRetry = 0
        while curRetry < maxRetry and ret == None:
            try:
                ret = self.requester.get(url,headers=headers,proxies=self.proxies,timeout=30,allow_redirects=allow_redirects)
                self.reconnectLock.acquire()
                # python每一行非复合语句都具有原子性，所以这里是线程安全的
                if self.isReconecting:
                    self.isReconecting = False
                    self.reconnectThds.clear()
                self.reconnectLock.release()
            except (requests.exceptions.ProxyError,requests.exceptions.ReadTimeout,requests.exceptions.SSLError,requests.exceptions.ConnectionError) as e:
                self.reconnectLock.acquire()
                # python每一行非复合语句都具有原子性，所以这里是线程安全的
                if not self.isReconecting:    
                    print(f'{threading.current_thread().name}进入重连了')
                    self.reconnect()
                self.reconnectLock.release()
                curRetry += 1                
        if ret == None:
            raise Exception()

        if ret.ok:
            if toJson:
                return json.loads(ret.content.decode(encoding=encoding))
            else:
                return ret.content.decode(encoding=encoding)
        else:
            print(ret.headers)
            print(ret.content)
            print('请求失败了！！！')
            exit(255)

    def reconnect(self):
        self.isReconecting = True
        def work():
            maxRetry = 10
            curRetry = 0
            ret = None
            testURL = 'https://bugly.qq.com/'
            cookieDict = {}
            while curRetry < maxRetry and ret == None:
                com.logout("[reconnect]")
                self.requester = self.newRequesterFunc(self.requester)

                try:
                    ret = self.requester.get(testURL,headers=self.headers,proxies=self.proxies,timeout=30,allow_redirects=False)
                except Exception as e:
                    print(e)
                curRetry += 1
                time.sleep(5)
            if ret == None:
                print('重连失败')
                exit(255)
            
        thds = ThreadManager.go(work,count=1)
        self.reconnectThds = thds
        ThreadManager.waitall(self.reconnectThds)
        return thds



class BuglyCrawler(object):
    def __init__(self,requester,branch,channel,appId,version,headers,platformId='1'):
        super().__init__()
        self.isReconecting = False
        self.requester = requester
        self.branch = branch
        self.channel = channel
        self.version = version
        self.appId = appId
        self.platformId = platformId

        self.headers = headers
        
        self.issueIds = []
        self.crashHashs = []


        self.basepath = 'https://bugly.qq.com/v2'

        getfsnstr = '''
        function getfsn() {
            var n = 0;
            var t = undefined;
            var s = [];
            for (var i = 0; i < 16; ++i)
                s.push(parseInt(Math.random()*(255-0+1)+0,10));
                
            if (s[6] = 15 & s[6] | 64,s[8] = 63 & s[8] | 128,t)
                for (var i = 0; i < 16; ++i)
                    t[n + i] = s[i];

            var shift = 0;
            var hex = ["00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0a", "0b", "0c", "0d", "0e", "0f", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1a", "1b", "1c", "1d", "1e", "1f", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2a", "2b", "2c", "2d", "2e", "2f", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "3a", "3b", "3c", "3d", "3e", "3f", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "4a", "4b", "4c", "4d", "4e", "4f", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "5a", "5b", "5c", "5d", "5e", "5f", "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "6a", "6b", "6c", "6d", "6e", "6f", "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "7a", "7b", "7c", "7d", "7e", "7f", "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "8a", "8b", "8c", "8d", "8e", "8f", "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "9a", "9b", "9c", "9d", "9e", "9f", "a0", "a1", "a2", "a3", "a4", "a5", "a6", "a7", "a8", "a9", "aa", "ab", "ac", "ad", "ae", "af", "b0", "b1", "b2", "b3", "b4", "b5", "b6", "b7", "b8", "b9", "ba", "bb", "bc", "bd", "be", "bf", "c0", "c1", "c2", "c3", "c4", "c5", "c6", "c7", "c8", "c9", "ca", "cb", "cc", "cd", "ce", "cf", "d0", "d1", "d2", "d3", "d4", "d5", "d6", "d7", "d8", "d9", "da", "db", "dc", "dd", "de", "df", "e0", "e1", "e2", "e3", "e4", "e5", "e6", "e7", "e8", "e9", "ea", "eb", "ec", "ed", "ee", "ef", "f0", "f1", "f2", "f3", "f4", "f5", "f6", "f7", "f8", "f9", "fa", "fb", "fc", "fd", "fe", "ff"];
            return [hex[s[shift++]], hex[s[shift++]], hex[s[shift++]], hex[s[shift++]], "-", hex[s[shift++]], hex[s[shift++]], "-", hex[s[shift++]], hex[s[shift++]], "-", hex[s[shift++]], hex[s[shift++]], "-", hex[s[shift++]], hex[s[shift++]], hex[s[shift++]], hex[s[shift++]], hex[s[shift++]], hex[s[shift++]]].join("");

        }
        '''
        
        self.getfsnfunc = execjs.compile(getfsnstr)

    def getfsn(self):
        return self.getfsnfunc.call('getfsn')

    def getIssueStruct(self,start,rows=50):
        '''
        rows =  10 20 50 防止被检查
        '''
        url = f'{self.basepath}/issueList?start={start}&searchType=errorType&exceptionTypeList=Crash,Native&pid={self.platformId}&platformId={self.platformId}&sortOrder=desc&version={self.version}&rows={rows}&sortField=crashCount&appId={self.appId}&fsn={self.getfsn()}'

        data = self.get(url)
        if data == None:
            return None,None
        issueDesc = []
        issueList = data['ret']['issueList']
        numFound = data['ret']['numFound']
        for d in issueList:
            issueDesc.append(IssueStruct(**d))
        return issueDesc,numFound

    def getCrashHash(self,issuesId,crashDataType='undefined'):
        url = f'{self.basepath}/lastCrashInfo/appId/{self.appId}/platformId/{self.platformId}/issues/{issuesId}/crashDataType/{crashDataType}?fsn={self.getfsn()}'
        data = self.get(url)
        if data == None:
            return None
        # 100000 成功
        if data['code'] == 100009:
            raise FSNException()
        
        crashHash = data['data']['crashHash']
        
        return crashHash
    def getCrashInfo(self,crashHash):
        
        if crashHash == None:
            return None
        url = f'{self.basepath}/crashDoc/appId/{self.appId}/platformId/{self.platformId}/crashHash/{crashHash}?fsn={self.getfsn()}'
        data = self.get(url)
        return data

    def get(self,url):
        trycount = 3
        while trycount > 0:
            trycount -= 1
            data = self.requester.get(url,self.headers)
            
            if 'code' in data:
                if data['code'] == 100006:
                    continue
            if 'status' in data:
                if data['status'] != 200:
                    raise FSNException()
            if 'ret' in data:
                if 'code' in data['ret']:
                    if data['ret']['code'] == 501004:
                        continue
            return data
        raise Exception(f'尝试失败 data={data}')





datetime = com.getlocaltime('-')
cookiespath = os.path.join(thisdir,'cookies.json')
version = '1.54.1.211022'
appid = 'fc1ccbfe45'
conf = Loader.load(buglycrawlerconf)


class ProxiesManager(object):
    def __init__(self):
        self.proxies = None
        self.needRelease = []
    def use_clash_trojan(self):
        # clash 的 trojan 代理！
        self.proxies = {'http': '127.0.0.1:7890','https': '127.0.0.1:7890'}
        # 开启clash
        clashP,code = BinManager.clash()
        def release():
            if sys.platform == 'win32':
                time.sleep(1)
                clashP.send_signal(subprocess.signal.CTRL_C_EVENT)
                # os.kill(0,subprocess.signal.CTRL_C_EVENT)
            else:
                clashP.send_signal(subprocess.signal.SIGINT)
        self.needRelease.append(release)
    def release(self):
        for n in self.needRelease:
            n()


class BuglyWebStruct(object):
    def __init__(self,cookieFilePath,subPorjectConf):
        '''
        branch是额外字段
        '''
        super().__init__()
        self.cookieFilePath = cookieFilePath

        session = self._init_session()
        self.network = NetworkManager(session,self.newSession,self.headers,proxiesmng.proxies)
        self._init_network()

        self.subProjectConf = subPorjectConf

        self.crawlers = []
        for conf in self.subProjectConf:
            isopen = conf['open']
            if not isopen:
                continue
            channel = conf['channel']
            appid = conf['appid']
            branch = conf['branch']
            version = conf['version'] # 从其他地方根据渠道拿
            self.crawlers.append(BuglyCrawler(self.network,branch,channel,appid,version,self.headers))
    def newSession(self,oldSession):
        cookies = oldSession.cookies
        oldSession.close()
        session = requests.Session()
        session.cookies = cookies
        return session

    def getStartData(self):
        stepList = []
        f1outList = []
        
        for crawler in self.crawlers:
            # 第一步 获取总崩溃数
            issues,numFound = crawler.getIssueStruct(0,rows=10)
            
            stepblock = com.num2stepblock(numFound,50,shift=10,contain_start=True,contain_end=False)
            com.logout(f'issue总数{numFound}')
            com.logout(f'快照{stepblock}')
            # stepList += self.均匀插值(stepList,list(map(lambda x: (crawler,x),stepblock)))
            # f1outList += self.均匀插值(f1outList,list(map(lambda x: (crawler,x),issues)))
            stepList += list(map(lambda x: (crawler,x),stepblock))
            f1outList += list(map(lambda x: (crawler,x),issues))
        return stepList,f1outList
    def 均匀插值(self,list1:list,list2:list):
        len1 = list1.__len__()
        len2 = list2.__len__()
        if len1 == 0:
            return list2
        step = int(len1/(len2 + 1))
        shift = int((len1 - step * len2 + step) /2) - step
        for i in range(0,len2):
            list1.insert((step * ( i + 1 )) + i + shift,list2[i])
            pass
        return list1

    def _init_session(self):
        session = requests.Session()
        session.mount('https://bugly.qq.com',DESAdapter())

        coo = requests.cookies.RequestsCookieJar()
        cookiesDict = com.loadfile_json(self.cookieFilePath)
        for k,v in cookiesDict.items():
            coo.set(k,v,path='/',domain='bugly.qq.com')

        self.headers = {
            'Connection':'keep-alive',
            'User-Agent':FakeChromeUA.get_ua()
        }
        func = '''
        function getXtoken(tokenskey){    
            t = tokenskey;
            a = 5381;
            for (var n = 0, r = t.length; n < r; ++n) a += (a << 5 & 2147483647) + t.charAt(n).charCodeAt();
            return 2147483647 & a
        }
        '''
        self.getXtokenfunc = execjs.compile(func)

        session.cookies.update(coo)
        return session
    def _init_network(self):
        url = 'https://bugly.qq.com/v3/bugly/BlankPage?targetUrl=https%3A%2F%2Fbugly.qq.com%2Fv2%2Fworkbench%2Fapps'

        ret = self.network.get(url,self.headers,toJson=False,allow_redirects=True)


        self.csrfToken = self.network.requester.cookies.get('csrfToken')
        self.tokenskey = self.network.requester.cookies.get('token-skey')
        self.bugly_session = self.network.requester.cookies.get('bugly_session')
        self.Xtoken = str(self.getXtokenfunc.call('getXtoken',self.tokenskey))
        
        self.headers['x-csrf-token']=self.csrfToken
        self.headers['X-token']=self.Xtoken

    def savecookies(self):
        coodict = requests.utils.dict_from_cookiejar(self.network.requester.cookies)
        com.dumpfile_json(coodict,self.cookieFilePath)
        SVNManager.upgrade(self.cookieFilePath)
        SVNManager.update(self.cookieFilePath)
        SVNManager.commit(self.cookieFilePath,'需求修改([sdk]/[bugly]): 更新cookies')

    def saveLog(self):
        for crawler in self.crawlers:
            com.logout(f'[上传记录] channel={crawler.channel} version={crawler.version} issueIdsCount={crawler.issueIds.__len__()}')
            DataHandler.sendLog(crawler.channel,crawler.version,crawler.issueIds)



    def Ding(self):
        # from comlib import 内网发包群苦工
        # 一点零注意了机器人 = 内网发包群苦工
        baseurl = 'http://192.168.2.104:5000/?channel={}&issueId={}'
        title = 'bugly新增数据更新了'
        content = 一点零注意了机器人.markdown_textlevel(title,2)
        content += 一点零注意了机器人.markdown_drawline()

        for crawler in self.crawlers:
            channel = crawler.channel
            version = crawler.version
            res,_ = DataHandler.getSummry(channel,version)
            
            for channel,issues_lastTime in res.items():
                content += f'''{channel}-ver:{version}\n\n快照{issues_lastTime['lasttime']}到{com.getdatetimenow()}\n\n'''
                for issue in issues_lastTime['issues']:
                    issueId = issue['id']
                    url = baseurl.format(channel,issueId)
                    desc = f"{channel}-{issueId}"
                    
                    link = 一点零注意了机器人.markdown_textlink(desc,一点零注意了机器人.getOutUrl(url))
                    content += link +'||'

                content += '\n\n'+一点零注意了机器人.markdown_drawline()
            
        indexurl = 'http://192.168.2.104:5000/'
        searchurl = 'http://192.168.2.104:5000/search/'
        yestodayurl = 'http://192.168.2.104:5000/yestoday/'
        data = 一点零注意了机器人.build_actioncard_mult('bugly数据更新了',content,主页=indexurl,预览=searchurl,昨日新增统计=yestodayurl)

        一点零注意了机器人.send(data)



    def _getRedirectUrl(self,channel,issueId):
        return f'''http://192.168.2.104:5000/?channel={channel}&issueId={issueId}'''
    def _getRedirecthtml(self,channel,issueId):
        return f'''<a href="{self._getRedirectUrl(channel,issueId)}">{channel+'-'+issueId}</a>'''
    def lastDaySummry(self):
        isFirstCraw = DataHandler.isFirstCrawToday()
        from comlib import DictUtil
        for crawler in self.crawlers:
            channel = crawler.channel
            version = crawler.version

            isFirst = DictUtil.tryGetValue(isFirstCraw,channel)
            if isFirst == None or not isFirst:
                continue
            allNewIssues:list = DataHandler.getDeltaDaySummry(channel,1,version)
            i = 0
            htmlcontent = ''
            for issue in allNewIssues:
                htmlurl = self._getRedirecthtml(channel,issue['id'])
                htmlcontent += htmlurl
                if i == 9:
                    i = 0
                    htmlcontent += '<br>'
                else:
                    i += 1
            htmlcontent += '<br>'


            
        






cookieFilePath = os.path.join(thisdir,'cookies.json')
def runall():
    # import grequests
    # grequests.
    bs = BuglyWebStruct(cookieFilePath,conf.sdk)

    def getAllIssue(self:Factory,inputQ,outputQ):
        try:
            bc,start = inputQ.get(True,5)
            com.logout(f'快照{start}')
            issues,num = bc.getIssueStruct(start,rows=50)
            if issues == None:
                inputQ.put((bc,start))
            else:
                bc.issueIds += list(map(lambda x: int(x.issueId),issues))
                com.Queue_putlist(outputQ,map(lambda x: (bc,x),issues))
        except Empty as e:
            pass
        except (requests.exceptions.ProxyError,requests.exceptions.ReadTimeout,requests.exceptions.SSLError,requests.exceptions.ConnectionError) as e:
            inputQ.put((bc,start))
        except FSNException as fe:
            inputQ.put((bc,start))

    def getCrashInfo(self:Factory,inputQ,outputQ):
        try:
            bc,issue = inputQ.get(True,5)
            issueID = issue.issueId
            crashHash = bc.getCrashHash(issueID)
            if not DataHandler.isExistsCrashInDB(bc.channel,crashHash):
                com.logout(f'爬取crashHash={crashHash} channel={bc.channel} issueId={issue.issueId}')
                # 获取详细数据
                info = bc.getCrashInfo(crashHash)
                if issue == None:
                    inputQ.put((bc,issue))
                else:
                    outputQ.put((bc,issue,info))
            else:
                com.logout(f'crashHash = {crashHash}存在 channel={bc.channel} issueId={issue.issueId}')
        except Empty as e:
            pass
        except (requests.exceptions.ProxyError,requests.exceptions.ReadTimeout,requests.exceptions.SSLError,requests.exceptions.ConnectionError,requests.exceptions.ChunkedEncodingError) as e:
            inputQ.put((bc,issue))
        except FSNException as fe:
            inputQ.put((bc,issue))

    def parseDate(self:Factory,inputQ,outputQ):
        try:
            bc,issue,info = inputQ.get(True,5)
            crashHash = info['ret']['crashMap']['id']
            
            okinfo = DataHandler.filter_crashinfo(info,bc.branch,bc.channel,bc.appId,bc.platformId)
            DataHandler.upload_DB(bc.branch,issue,okinfo)
            outputQ.put((bc,issue,okinfo))

            
        except Empty as e:
            pass
    

    from multiprocessing import cpu_count

    stepList,f1outList = bs.getStartData()
    stepQ = com.list2Queue(stepList)
    f1outQ = com.list2Queue(f1outList)

    getAllIssue_pc = 4
    # getCrashInfo_pc = 2
    getCrashInfo_pc = int(sys.argv[2])
    parseDate_pc = cpu_count()
    print(f'[parallelCount] getAllIssue => {getAllIssue_pc}')
    print(f'[parallelCount] getCrashInfo => {getCrashInfo_pc}')
    print(f'[parallelCount] parseDate => {parseDate_pc}')
    f1 = Factory(getAllIssue,parallelCount=getAllIssue_pc,inputQueue=stepQ,outputQueue=f1outQ,baseName='getAllIssue')
    f2 = Factory(getCrashInfo,parallelCount=getCrashInfo_pc,baseName='getCrashInfo')
    f3 = Factory(parseDate,parallelCount=parseDate_pc,baseName='parseDate')
    f1.addNext(f2)
    f2.addNext(f3)
    f1thds = f1.run()
    f2thds = f2.run()
    f3thds = f3.run()

    ThreadManager.waitall(f1thds + f2thds)

    bs.savecookies()
    ThreadManager.waitall(f3thds)
    bs.Ding()
    time.sleep(1)
    # bs.lastDaySummry() 放服务器了
    bs.saveLog()
    pass

def keepalive():
    bs = BuglyWebStruct(cookieFilePath,conf.sdk)
    bs.savecookies()

if __name__ == "__main__":
    
    # tet()


    proxiesmng = ProxiesManager()
    proxiesmng.use_clash_trojan()
    try:
        if sys.argv[1] == 'run':
            runall()
        else:
            keepalive()
    finally:
        # pass
        proxiesmng.release()
    


# ret = session.get(getIssueURL('1.54.1.207376','fc1ccbfe45',0),headers=h)
# issueDesc = parsedata(ret.content)
# for i in issueDesc:
#     print(i.issueId)
# print(issueDesc.__len__())


# curl -I 'https://bugly.qq.com/v3/bugly/BlankPage?targetUrl=https%3A%2F%2Fbugly.qq.com%2Fv2%2Fworkbench%2Fapps' \
#   -H 'Connection: keep-alive' \
#   -H 'User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36 Edg/83.0.478.58' \
#   -H 'Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9' \
#   -H 'Accept-Language: zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6' \
#   -H 'Cookie: bugly_session=eyJpdiI6IkpmUVh2VWxyclZzZ3FQY1pcL2xDTXN3PT0iLCJ2YWx1ZSI6IjRkRjZMcDFUQnlaeXNMZWEwdlJUb0s2dzJPckVacnhvUm9DVXRLVnplNGp3M2h2UWtpTFpHXC9SWGFiQm40elNPY1NmSE9GTUlyWTVYSlF0NGsyUUFaZz09IiwibWFjIjoiN2UyZjEwMzQ4YmExNmNiOTQ2ZDQ3ZWMwMDBkNWU1YzVmNzExMzA2YzMyMTIwNjE2NWVlNWI0NDg4OGJiZDRmZiJ9' \
#   --compressed


# 获取x-csrf-token  第三步

# curl -I 'https://bugly.qq.com/v3/bugly/BlankPage?targetUrl=https%3A%2F%2Fbugly.qq.com%2Fv2%2Fworkbench%2Fapps' \
#   -H 'Connection: keep-alive' \
#   -H 'User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36 Edg/83.0.478.58' \
#   -H 'Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9' \
#   -H 'Accept-Language: zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6' \
#   -H 'Cookie: bugly_session=eyJpdiI6ImNPenBPME1hWWFuenJrYXVyRU1FT0E9PSIsInZhbHVlIjoiQWhXWmhLaXFXSVBNSXBlUlZZXC9PRVlLVUJiU1ZoWUoyb1c3OStNb2RyWnJoZUp4SXN3MEdxZ0tvY3hDWml6T05LdE5ib0dURlJDZ2R6Z3VQRTFiQXh3PT0iLCJtYWMiOiIzMTQ1MTYyYjExODc2MGRhM2IxYjFjNThlMGE2NzI1Mjk3ZDhhYzA0Njg3YzIyNTRiMThjZDkzN2JiNmE1MzQ0In0%3D' \
#   --compressed


# 获取bugly_session token-skey  第一步

# curl -I 'https://bugly.qq.com/v2/workbench/apps' \
#   -H 'Connection: keep-alive' \
#   -H 'User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36 Edg/83.0.478.58' \
#   -H 'Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9' \
#   -H 'Accept-Language: zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6' \
#   -H 'Cookie: bugly_session=eyJpdiI6Im00ald6M2Rpem56R2doYmltSjQxZnc9PSIsInZhbHVlIjoiWStGRjIybjc4ZVwvc0xkOG1DMjRnVG85WGI5VWt3VmR2amNUaUZKdlFTbmUwSEJEVG1ETThOTGNsa2xKV2UxbG5Mc2pjZFlOaGtpOE9UdnQ0V2tMaEZRPT0iLCJtYWMiOiIyN2RhZDAxNTFkOTQ4ZGRlODljNjM5NzkxNjI1OTAwZGVlMjQwYTk3ZmEyMTM1MzQ2ODc5MjUwYjNmOTg3M2I4In0%3D' \
#   --compressed


# 从token-skey计算X-token  第二步

# function getXtoken(tokenskey){    
#             t = tokenskey;
#             a = 5381;
#             for (var n = 0, r = t.length; n < r; ++n) a += (a << 5 & 2147483647) + t.charAt(n).charCodeAt();
#             return 2147483647 & a
# }
#         

                #  eyJpdiI6Im96dFwvOURHXC9pd0x0Sndkdkt6V2o4dz09IiwidmFsdWUiOiJcL3hnQW9kSXpjN3NaRkdkbGhPOE1VeGJqMGEwYk8zbVJCR1I3VXh2VFJ0dW1QWGRJRG5NTWRcL01GdExrd0hlSDVxZ2JLaHM4SlwvajZFdUplOWhZbmtSZz09IiwibWFjIjoiMzI1NjU4MjgwODcyNTc5MzliMTQyNDMyZGNkNzI0NDFiMWM0ZGFiM2YxMTBmNmUyMzc0MmUzNzBmYWNkZDE4YyJ9



# curl -I 'https://bugly.qq.com/v3/cas/node-cas?ticket=ST-19951653-IOk3cLooL1WWKVqEWoBh-login.rdm.org' \
#   -H 'Connection: keep-alive' \
#   -H 'Upgrade-Insecure-Requests: 1' \
#   -H 'User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36 Edg/83.0.478.58' \
#   -H 'Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9' \
#   -H 'Sec-Fetch-Site: cross-site' \
#   -H 'Sec-Fetch-Mode: navigate' \
#   -H 'Sec-Fetch-Dest: document' \
#   -H 'Accept-Language: zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6' \
#   -H 'Cookie: before_login_referer=https%3A%2F%2Fbugly.qq.com%2Fv2%2Findex; _qpsvr_localtk=0.367653526602989; pgv_pvi=550747136; pgv_si=s6845535232; RK=nfh8mAXiQ+; ptcz=8697f7ec7256bce71b70664d175f0523cbe79427e102e6096c56436c2962c09b; referrer=eyJpdiI6Im1KVHlTZWhXNXV0NW1YdUtVWENGQ2c9PSIsInZhbHVlIjoiRG1cL2RRVEVBS3paRnRWbFZVbE5ab2FXRWpyRVM3S1lZZDAyZHUrWnU2UEtLeVNGRHZDSUViWXJsK3JlN2JCU0tlSDUwTk5lWFdnSFdvTm1uRytPdXVZQmdPQnNJeDNXUGdETzJtN0ozZUt4bmR6TGthS2ZmQnFkdU93MTdsT3hFblwvSm1jd2NibXozK2hZRzhqcjg2eUlDSERMT05YcHEwMGw2bDZjbVkyMHVPaWRwVmZzK1lRVW9SaDFUczRUUkEybDhoN29cL1hEVUJnMlB1YXEweklQdnR4aXUxYWliZmhCa1wveFwvbk02ZnY0eFRwQ1dqdDJnSHJjdFI2a0gweFwvejVXNHFmWmRBS01XMVdnS0J3RXRJb3RvWHlEQ24wT21NUlZadU5cL1BEVGdZQlhlK1liczZ2OVZmR294eStOZEI1ZVoxa2FPSmFWT1wvUk81Yk9acnJTVU04YWxkWklYRU10NUdiZVRKeWpDckZvUUVUMERiNzNnUlNpT0YxZExqUDRrYW54ZE1LMkkyU0V6ZFpNMXRLdjJLTTBpc0lhZWZUT2ZkNEo5aHJsSzhiT080WnZ4bVwvYWtJeVpWOTRPSVJ3ZGdEVU1wOGN4a1lzcVpYM1liZGdiVm9PcVVJbFdMS1drdHB1Q2Z2ZEdHamQ2YzhaVUM2YVprZlZhdk1oSllId1lNcmI5eFRrVTBGY01VblB4a2hwTDFkUFwvc3FJcWdEK1hNNEg4U3E5VHh6MEdjaXJRT1JneVNJbVFHOWdYNnF5bnNlQU5xQlwvV1lMV0taMzRkaGNIQVwvSWRBallhTncxaGpNUlwvRTkzZTBMSW41ZFhZdDByNnlRelE1VnZjcXVoS0w1cGM1UHpNcnM4dkl0cGtlalNBMk43eExMQlwvbGh3cG1kYUlxQ0lIME5VVWtkM0E5RDVGRWsyWW1kZk16NWZ5S0dcL2tGNWF4M3hWNFBxVjFcL3U0NndSZz09IiwibWFjIjoiM2ViODE3NDNmMTVjYTM1YjMzMjg5MWRiOGQwODc2ZTdlOWFjMzJkYTZlY2MyZjFlOTM4ZmY2OGI3Y2ZjNmU3NyJ9; token-skey=c347e423-d5c8-1fb9-39a0-4dbc4d362c7e; token-lifeTime=1594647253; csrfToken=yMJ4nUpWpnbw601Tgd64DNkV; NODINX_SESS=MCuf1XNGd8sC-oGi5S8TyA2e5wJY3vIl7RBZlj0d-LJYVXH-XJrIG2TszLtQ9Jxo; _ga=GA1.2.1052835566.1594629254; _gid=GA1.2.559306016.1594629254; _gat=1; bugly_session=eyJpdiI6Im55NmxVYjZ5b3hsWkV4QVlOQnhCNmc9PSIsInZhbHVlIjoiSU8xZXVURVFHUHpCemxmSUt3QWJGUTc1a3NIWnE3Y1N0bnUzVUpaT1pjOFVvdDlQRjJiR0VrVmlRN25KaXN0eU43ajRCUzFUaFQrT3pwekgwRmgreUE9PSIsIm1hYyI6IjZjOGY2N2U1M2ZjZjg1MmQ1ZWU4N2I2Yjc2MjIwOTlkZTJjZTc0ODBlZDg2NWU0OTFmOWQyZjkwMjk2YjQ1YjAifQ%3D%3D' \
#   --compressed

# curl -I 'https://bugly.qq.com/v3/bugly/BlankPage?targetUrl=https://bugly.qq.com/v2/workbench/apps' \
#   -H 'Connection: keep-alive' \
#   -H 'Upgrade-Insecure-Requests: 1' \
#   -H 'User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36 Edg/83.0.478.58' \
#   -H 'Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9' \
#   -H 'Sec-Fetch-Site: same-origin' \
#   -H 'Sec-Fetch-Mode: navigate' \
#   -H 'Sec-Fetch-Dest: document' \
#   -H 'Referer: https://bugly.qq.com/v2/workbench/apps' \
#   -H 'Accept-Language: zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6' \
#   -H 'Cookie: before_login_referer=https%3A%2F%2Fbugly.qq.com%2Fv2%2Findex; _qpsvr_localtk=0.367653526602989; pgv_pvi=550747136; pgv_si=s6845535232; RK=nfh8mAXiQ+; ptcz=8697f7ec7256bce71b70664d175f0523cbe79427e102e6096c56436c2962c09b; bugly_session=eyJpdiI6IkVkS3hSTDJCNENMb1BsUjVvc0VIYWc9PSIsInZhbHVlIjoiRndVZU9WWGw4NnhSenFzbFwvMTV5T2oxMWFlMUdFOFwvSk1XRm5iSUlGekJFR3RkR0hVcXU2SDF2bFR0RzNuVVhwN2V1R3NyVmM4S1lDamw1K3Z0UVwvTUE9PSIsIm1hYyI6ImQ2M2VkM2EwNmJjZjZmZDBlNjY5ZGU3N2I2ZjBjMjAzM2MwMGUxNjQwZTgxOWE3NjcwN2U4ZTA1OGE4ZDk3NTQifQ%3D%3D; referrer=eyJpdiI6Im1KVHlTZWhXNXV0NW1YdUtVWENGQ2c9PSIsInZhbHVlIjoiRG1cL2RRVEVBS3paRnRWbFZVbE5ab2FXRWpyRVM3S1lZZDAyZHUrWnU2UEtLeVNGRHZDSUViWXJsK3JlN2JCU0tlSDUwTk5lWFdnSFdvTm1uRytPdXVZQmdPQnNJeDNXUGdETzJtN0ozZUt4bmR6TGthS2ZmQnFkdU93MTdsT3hFblwvSm1jd2NibXozK2hZRzhqcjg2eUlDSERMT05YcHEwMGw2bDZjbVkyMHVPaWRwVmZzK1lRVW9SaDFUczRUUkEybDhoN29cL1hEVUJnMlB1YXEweklQdnR4aXUxYWliZmhCa1wveFwvbk02ZnY0eFRwQ1dqdDJnSHJjdFI2a0gweFwvejVXNHFmWmRBS01XMVdnS0J3RXRJb3RvWHlEQ24wT21NUlZadU5cL1BEVGdZQlhlK1liczZ2OVZmR294eStOZEI1ZVoxa2FPSmFWT1wvUk81Yk9acnJTVU04YWxkWklYRU10NUdiZVRKeWpDckZvUUVUMERiNzNnUlNpT0YxZExqUDRrYW54ZE1LMkkyU0V6ZFpNMXRLdjJLTTBpc0lhZWZUT2ZkNEo5aHJsSzhiT080WnZ4bVwvYWtJeVpWOTRPSVJ3ZGdEVU1wOGN4a1lzcVpYM1liZGdiVm9PcVVJbFdMS1drdHB1Q2Z2ZEdHamQ2YzhaVUM2YVprZlZhdk1oSllId1lNcmI5eFRrVTBGY01VblB4a2hwTDFkUFwvc3FJcWdEK1hNNEg4U3E5VHh6MEdjaXJRT1JneVNJbVFHOWdYNnF5bnNlQU5xQlwvV1lMV0taMzRkaGNIQVwvSWRBallhTncxaGpNUlwvRTkzZTBMSW41ZFhZdDByNnlRelE1VnZjcXVoS0w1cGM1UHpNcnM4dkl0cGtlalNBMk43eExMQlwvbGh3cG1kYUlxQ0lIME5VVWtkM0E5RDVGRWsyWW1kZk16NWZ5S0dcL2tGNWF4M3hWNFBxVjFcL3U0NndSZz09IiwibWFjIjoiM2ViODE3NDNmMTVjYTM1YjMzMjg5MWRiOGQwODc2ZTdlOWFjMzJkYTZlY2MyZjFlOTM4ZmY2OGI3Y2ZjNmU3NyJ9; token-skey=c347e423-d5c8-1fb9-39a0-4dbc4d362c7e; token-lifeTime=1594647253; csrfToken=yMJ4nUpWpnbw601Tgd64DNkV; NODINX_SESS=MCuf1XNGd8sC-oGi5S8TyA2e5wJY3vIl7RBZlj0d-LJYVXH-XJrIG2TszLtQ9Jxo' \
#   --compressed




# curl -I 'https://bugly.qq.com/v3/bugly/BlankPage?targetUrl=https%3A%2F%2Fbugly.qq.com%2Fv2%2Fworkbench%2Fapps' \
#   -H 'Connection: keep-alive' \
#   -H 'Upgrade-Insecure-Requests: 1' \
#   -H 'User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36 Edg/83.0.478.58' \
#   -H 'Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9' \
#   -H 'Sec-Fetch-Site: cross-site' \
#   -H 'Sec-Fetch-Mode: navigate' \
#   -H 'Sec-Fetch-Dest: document' \
#   -H 'Accept-Language: zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6' \
#   -H 'Cookie: NODINX_SESS=MCuf1XNGd8sC-oGi5S8TyA2e5wJY3vIl7RBZlj0d-LJYVXH-XJrIG2TszLtQ9Jxo;' \
#   --compressed





