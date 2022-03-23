# -*- coding:utf-8 -*-
import os,sys,time,shutil,hashlib,subprocess,json,zipfile,re,socket,textwrap
# from typing import Tuple
sys.path.append(os.path.abspath(os.path.join(__file__,'..','..')))
import random
from queue import Queue,Empty
from datetime import datetime,timedelta
workdir = os.path.abspath(os.getcwd())

from typing import Tuple,List,TypeVar

T = TypeVar('T')

# com唯一可以依赖的模块
from comlib.logm import LogLevel,Log

# from com.ftpm import FTPManager
# from com.httpm import httpm

def loadfile_json(fp):
    '''
    用JsonFile代替
    '''
    # print(os.getcwd())
    with open(fp,'r',encoding='utf-8') as fs:
        return json.load(fs)

def loadfile_json_bykey(fp,key):
    '''
    通过key读json值
    '''
    # print(os.getcwd())
    with open(fp,'r',encoding='utf-8') as fs:
        data = json.load(fs)
        return data[key] if key in data else None


def dumpfile_json(js,fp):
    '''
    将字典按json格式储存到文件
    '''
    with open(fp,'w',encoding='utf-8') as fs:
        json.dump(js,fs,ensure_ascii=False,indent=4)

def dumpfile_json_bykey(js,key,value):
    '''
    通过key写json值
    '''
    with open(js,'r+',encoding='utf-8') as fs:
        data = json.load(fs)
        data[key] = value
        fs.seek(0)
        json.dump(data,fs,ensure_ascii=False,indent=4)
        fs.truncate()

def dict2jsonstr(d,indent=4):
    '''
    字典转json字符串
    '''
    jsonstr = json.dumps(d,ensure_ascii=False,indent=4)
    return jsonstr
def jsonstr2dict(jsonstr):
    '''
    json字符串转字典
    '''
    d = json.loads(jsonstr,encoding='utf-8')
    return d

cmd_divide_win='\r\n'
cmd_divide_linux='\n'

def getcmddivide():
    '''
    获取当前系统换行符
    '''
    return getvalue4plat(cmd_divide_win,cmd_divide_linux)

def cmd_builder(header,*param):
    '''
    已废弃
    '''
    safe_param = '"'+'" "'.join(param)+'"'
    return "%s %s"%(header,safe_param)

def cmd(cmd, filterstr=None, getstdout=True, showlog=True,decoderet=True, encoding=None, errException=None,getPopen=False,logfile=None,**kv):
    '''
    执行命令，getPopen已废弃，用cmd_subp代替
    '''
    if filterstr != None:
        cmd += '|'
        if sys.platform == 'win32':
            cmd += f'findstr "{filterstr}"'
        elif sys.platform == 'linux':
            cmd += f'egrep "{filterstr}"'
        elif sys.platform == 'darwin’':
            cmd += f'egrep "{filterstr}"'
        else:
            cmd += f'egrep "{filterstr}"'
    if encoding == None:
        encoding = getvalue4plat('gbk','utf-8')
    
    shell = True
    code = 0
    r = ''
    if logfile != None:
        redirect = f' > {logfile} 2>&1'
        cmd += redirect

    if showlog:
        logout('[run] '+cmd)
    try:
        if getPopen:
            if logfile == None:
                sub = subprocess.Popen(cmd,shell=shell,universal_newlines=True, stdin=subprocess.PIPE, stdout=subprocess.PIPE, stderr=subprocess.PIPE,**kv)
            else:
                sub = subprocess.Popen(cmd,shell=shell,universal_newlines=True,**kv)
            r = sub
        elif getstdout:
            r = subprocess.check_output(cmd,shell=shell,**kv)
            if decoderet:
                r = r.decode(encoding=encoding)
        else:
            code = subprocess.call(cmd,shell=shell,**kv)
    except subprocess.CalledProcessError as e:
        code = e.returncode
        r = e.output.decode(encoding=encoding)
        logout('error code %s'%code)
    finally:
        if errException != None and code != 0:
            logout(r)
            logout(code)
            raise errException
        return r,code
def cmd_subp(cmdstr,showlog=True, errException=None,**kv):
    '''
    执行命令，使用Popen创建子进程
    '''
    if showlog:
        logout(f'[subprocess.Popen run] {cmdstr}')
    sub = subprocess.Popen(cmdstr,shell=True,universal_newlines=True,**kv)
    return sub
def cmd_async(cmd, workspace):
    r = cmd_subp(cmd,cwd=workspace)
    return r
def killproc(proc):
    if sys.platform == 'win32':
        cmd(f'TASKKILL /F /PID {proc.pid} /T')
    else:
        proc.terminate()
    


def make_executable(path):
    '''
    只在unix系统有效
    '''
    if not isWindows():
        cmd(f'chmod +x {path}')
def logout(s):
    '''
    输出日志，这个暂时作为标记
    '''
    Log.debug(s,stacklevel=3)
    # print(s,flush=True)
def get_md5(filepath,debug=False):
    '''
    获取文件md5
    '''
    d5 = hashlib.md5()
    md5 = ''
    with open(filepath,'rb') as f:
        md5 = get_md5_filelike(f)
    if debug:
        logout('[GetMD5]  '+filepath +'    MD5:'+md5)
    return md5
def get_md5_filelike(filelike):
    '''
    获取文件类型的md5
    '''
    d5 = hashlib.md5()
    while True:
        data = filelike.read(2048)
        if not data:
            break
        d5.update(data)
    md5 = d5.hexdigest()
    return md5
def get_md5_str(s):
    '''
    获取字符串的md5
    '''
    d5 = hashlib.md5()
    d5.update(bytes(s.encode(encoding='utf-8')))
    md5 = d5.hexdigest()
    return md5
def get_md5_fileinzip(zippath,filepath):
    '''
    获取zip中的文件md5
    '''
    d5 = hashlib.md5()
    zfs = getfilestream4zip(zippath,filepath,'rb')
    while True:
        data = zfs.read(2048)
        if not data:
            break
        d5.update(data)
    zfs.close()
    md5 = d5.hexdigest()
    logout('[GetMD5 fileinzip]  '+zippath+'    '+filepath +'    MD5:'+md5)
    return md5

def getfilestream4zip(zippath,filepath,mode):
    '''
    获取zip内文件的字符流，需要手动close zip
    '''
    zf = zipfile.ZipFile(zippath)
    fs = zf.open(filepath,mode.replace('b','').replace('+',''))
    return fs
def buildfolder(*ps,remove_exists=True):
    '''
    创建多层路径
    '''
    p = os.path.sep.join(ps)
    logout(ps)
    logout('------------------------'+p)

    if os.path.exists(p) and remove_exists:
        shutil.rmtree(p)
    return buildfolder_tree(p)
def buildfolder_tree(leafpath,exfunc=os.path.exists,mdfunc=os.mkdir,sep=os.path.sep):
    '''
    用buildfolder
    '''
    q = []
    pt = leafpath
    
    while not exfunc(pt):
        q.append(pt)
        if pt.find(sep) != -1:
            # pt = pt.replace(sep+os.path.basename(pt),'')
            pt = pt[0:pt.__len__()-os.path.basename(pt).__len__()-1]
        else:
            pt = '.'
            break
    while q.__len__() != 0:
        mdfunc(q.pop())
    return leafpath
def getlocaltime(divide='_')->str:
    '''
    获取本地时间，输出自定义日期格式
    '''
    t = time.localtime(time.time())
    return '%d%s%02d%02d%s%02d%02d%02d' % (t[0],divide,t[1],t[2],divide,t[3],t[4],t[5])
def tsp2datetime(tsp):
    '''
    unix tsp时间戳转datetime类型
    '''
    return datetime.fromtimestamp(int(tsp) / 1000)
def tsp2readable(tsp,divide='-')->str:
    '''
    时间戳转自定义可读格式
    '''
    date = tsp2datetime(tsp)
    return '%d%s%02d%02d%s%02d%02d%02d' % (date.year,divide,date.month,date.day,divide,date.hour,date.minute,date.second)
def getdatetimenow():
    '''
    精确到秒
    '''
    return datetimeFromat(datetime.now())
def getdatetimenow_full():
    '''
    完整的datetime.now()，，:转_
    '''
    return datetime.now().__str__().replace(':','_')
def gettsp():
    '''
    获取当前时间的unix tsp时间戳
    '''
    now = datetime.now()
    return int(now.timestamp()*1000)
def datetimeFromat(dt):
    '''
    移除对datetime的小数部分
    '''
    return dt.__str__().split('.')[0]
def datetimenow():
    '''
    获取当前时间字符串
    '''
    return datetime.now()
def datetimenow_day():
    '''
    获取datetime类型的当前时间，最小单位是天
    '''
    now_tmp = datetimenow()
    return datetime(now_tmp.year,now_tmp.month,now_tmp.day)
def getdatetime_afterDelta(date,delta):
    '''
    对datetime类型进行日期加减
    '''
    deltadate = timedelta(int(delta))
    date -= deltadate
    return date
def pythonTsp2unixTsp(pythonTsp:float):
    return int(pythonTsp * 1000)

# zip操作都废弃了，用zipmanager的方法
def unzip(zippath,unzippath):
    zf = zipfile.ZipFile(zippath,'r')
    zf.extractall(unzippath)
def mzip(filelist,zipname,basedir=None,compression=zipfile.ZIP_STORED,debug=False):
    if debug:
        logout('[zip] zipname  '+zipname)
        logout('[zip] basedir  '+basedir)
        logout('[zip] compression  %s'%compression)
    zf = zipfile.ZipFile(zipname,'w',compression=compression)
    for f in filelist:
        filepathinzip=f

        if basedir != None:
            filepathinzip = f.replace(basedir,'').strip(os.path.sep)
        if debug:
            logout('[zip] %s --> %s'%(f,filepathinzip))
        zf.write(f,filepathinzip)
    zf.close()
def zipall(tarpath,zippath,compression=zipfile.ZIP_STORED,containtarfolder=False,debug=False):
    '''
    压缩所有
    '''
    def tmpiter():
        for dirpath, dirnames, filenames in os.walk(tarpath):
            for fn in filenames:
                fn = os.path.join(dirpath,fn)
                yield fn
    tmp = tarpath
    if containtarfolder:
        tmp = os.path.dirname(tarpath)
        
    mzip(tmpiter(),zippath,tmp,compression,debug)
def zip_addfile(zippath,from_and_to:list):
    '''
    往zip放文件
    '''
    zf = zipfile.ZipFile(zippath,'a')
    for fr,to in from_and_to:
        zf.write(fr,to,zipfile.ZIP_DEFLATED)
    zf.close()
def zip_write(zippath,data,filepath_inzip):
    '''
    往zip里写数据
    '''
    zf = zipfile.ZipFile(zippath,'a')
    zf.writestr(filepath_inzip,data)
    zf.close()
def isWindows():
    return sys.platform == 'win32'
def isMac():
    return sys.platform == 'darwin'
def isLinux():
    return sys.platform == 'linux'
def getvalue4plat(windows_value,linux_value,mac_value=None):
    '''
    根据平台获取对应的值
    '''
    if sys.platform == 'win32':
        return windows_value
    if mac_value == None:
        return linux_value

    if sys.platform == 'darwin':
        return mac_value
    else:
        return linux_value

def rewrite(writeable,s):
    '''
    对文件重新写入
    '''
    writeable.seek(0)
    writeable.truncate()
    writeable.write(s)
def re_replace(pattern,repl,filepath,encoding='utf-8',lineMode=False)->str:
    '''
    替换所有满足正则的字符段，只替换第一个匹配到的
    '''
    allfind=[]
    with open(filepath,'r+',encoding=encoding) as fs:
        if lineMode:
            lines = fs.readlines()
            fs.seek(0)
            fs.truncate()
            
            for line in lines:
                m = re.search(pattern,line)
                if m == None:
                    continue
                line = line.replace(m.group(0),repl)
                fs.write(line)
            
        else:
            allstr = fs.read()
            m = re.search(pattern,allstr)
            if m == None:
                # raise Exception(f"{pattern}没找到")
                return None
            firstfind = m.group(0)
            allfind = m.groups()
            allstr = allstr.replace(firstfind,repl)
            rewrite(fs,allstr)
    return allfind
def replace_filecontent(filepath,old,new):
    with open(filepath,'r+',encoding='utf-8') as fs:
        content = fs.read().replace(old,new)
        fs.seek(0)
        fs.truncate()
        fs.write(content)
def combinefolder(to,*froms,method='copy',cover=True,debug=False,fileextfilter=None,dirfilters=[]):
    '''
    合并文件夹
    '''
    combineFiles = []
    coverFiles = []
    combineDirs = []
    
    for fr in froms:
        for dirpath, dirnames, filenames in os.walk(fr):
            abspath_to = os.path.join(to,dirpath.replace(fr,'',1).strip(os.sep)).rstrip(os.sep)
            
            relpath:str = dirpath.replace(fr,'',1)
            relpath = relpath[1::] if relpath.startswith(os.path.sep) else relpath

            def isInDirFilters(curDirname):
                if dirfilters.__len__() == 0:
                    return False
                for dirfilter in dirfilters:
                    if os.path.join(relpath,curDirname).startswith(dirfilter):
                        return True
                return False
            for dirname in dirnames:
                if isInDirFilters(dirname):
                    if debug:
                        logout(f'[skip] {dirname}')
                    continue

                dn = os.path.join(abspath_to,dirname)
                if not os.path.exists(dn):
                    if debug:
                        logout(f'mkdir {dn}')
                    # 丢权限
                    os.mkdir(dn)
                    combineDirs.append(dn)
            for filename in filenames:
                if isInDirFilters(os.path.dirname(filename)):
                    if debug:
                        logout(f'[skip] {filename}')
                    continue
                if fileextfilter != None and filename.endswith(fileextfilter):
                    continue


                tofn = os.path.join(abspath_to,filename)
                fromfn = os.path.join(dirpath,filename)
                if os.path.exists(tofn):
                    coverFiles.append((fromfn,tofn))
                    if cover:
                        if debug:
                            logout(f'cover {fromfn} -> {tofn}')
                        os.remove(tofn)
                        if method == 'move':
                            shutil.move(fromfn,tofn)
                        else:
                            shutil.copy2(fromfn,tofn)
                    else:
                        if debug:
                            logout(f'skip {fromfn} -> {tofn}')
                else:
                    if debug:
                        logout(f'move {fromfn} -> {tofn}')
                    if method == 'move':
                        shutil.move(fromfn,tofn)
                    else:
                        shutil.copy2(fromfn,tofn)
                    combineFiles.append(tofn)
    return combineFiles,coverFiles,combineDirs

def safeCoverFiles(coverFiles):
    tmpref = []
    for fromfile,tofile in coverFiles:
        tmp_tofile = get_random_filename(os.getcwd())
        # logout(f'[重复资源备份] {tofile} => {tmp_tofile}')
        shutil.move(tofile,tmp_tofile)
        shutil.copy2(fromfile,tofile)
        tmpref.append((tofile,tmp_tofile))
    return tmpref
def rollbackCoverFiles(tmpref:list):
    for tofile,tmp_tofile in tmpref:
        # logout(f'[重复资源还原] {tmp_tofile} => {tofile}')
        os.remove(tofile)
        shutil.move(tmp_tofile,tofile)
    tmpref.clear()
def safepath(*paths):
    '''
    给路径添加双引号
    '''
    filter_paths = []
    for p in paths:
        if p not in ('',None):
            filter_paths.append(str(p))

    return '"'+'" "'.join(filter_paths)+'"'


def win2unixformat(filep):
    '''
    windows换行转为unix换行
    '''
    f = open(filep,'r',encoding='utf-8')
    s = f.read().replace(r'\r\n',r'\n')
    f.close()
    f = open(filep,'wb')
    f.write(bytes(s,encoding='utf8'))
    f.close()


def strjoin(sep,*l):
    '''
    会忽略空字符串的 os.path.join
    '''
    return sep.join(filter(lambda x: x not in ('',None),l))

def get_file_dirname(filepath):
    '''
    获取当前文件所在文件夹路径
    '''
    return os.path.abspath(os.path.join(filepath,'..'))

def get_random_filename(dirname):
    '''
    获取随机文件路径
    '''
    filename = os.path.join(dirname,str(random.randint(0,sys.maxsize)))
    while os.path.exists(filename):
        filename = os.path.join(dirname,random.randint(0,sys.maxsize))
    return filename
def get_ftp_savepath_base():
    from comlib.conf.loader import Loader
    from comlib.conf.ref import ftpconf
    conf = Loader.load(ftpconf,use_defalt=True)
    return f'/{conf.rootdir}/{conf.packagesave}/{Loader.getgame()}'
def get_http_savepath_base():
    from comlib.conf.loader import Loader
    from comlib.conf.ref import ftpconf
    conf = Loader.load(ftpconf,use_defalt=True)
    return f'{conf.packagesave}/{Loader.getgame()}'
def get_ftp_savepath(plat,branchDesc,channel):
    '''
    获取上传ftp的文件夹路径
    '''
    from comlib.conf.loader import Loader
    from comlib.conf.ref import ftpconf
    conf = Loader.load(ftpconf,use_defalt=True)
    return f'/{conf.rootdir}/{conf.packagesave}/{Loader.getgame()}/{plat}/{branchDesc}_{channel}'
def get_ftp_tempsavepath(pathInTemp):
    from comlib.conf.loader import Loader
    from comlib.conf.ref import ftpconf
    from comlib.comobj import G_timemark
    conf = Loader.load(ftpconf,use_defalt=True)
    return f'/{conf.rootdir}/{conf.tempsave}/{pathInTemp}'

def get_ftp_logsavepath(scriptname,extpath=None):
    from comlib.conf.loader import Loader
    from comlib.conf.ref import ftpconf
    from comlib.comobj import G_timemark
    conf = Loader.load(ftpconf,use_defalt=True)
    basepath = f'/{conf.rootdir}/{conf.devopssave}/{scriptname}'
    if extpath != None:
        basepath = f'{basepath}/{extpath}'
    basepath = f'{basepath}/{G_timemark}'
    return basepath
def creat_download_html(url,path):
    '''
    创建下载页面，jenkins用
    '''
    import urllib.parse as up

    
    htmlcontent = f'<html><body><a href="{url}">Download Package</a></body></html>'
    htmlfile = os.path.join(path,'download.html')
    savedata(htmlcontent,htmlfile)

def applydata(data,filepath):
    '''
    附加所有数据到文件路径
    '''
    with open(filepath,'a',encoding='utf-8') as fs:
        fs.write(data)
def savedata(data,filepath,mode='w',encoding='utf-8',newline=None):
    '''
    储存所有数据到文件路径
    '''
    if 'b' in mode:
        encoding=None
    with open(filepath,mode,encoding=encoding,newline=newline) as fs:
        if isinstance(data,int):
            data = str(data)
        fs.write(data)
def savelines(lines,filepath,mode='w',encoding='utf-8',divide=None):
    '''
    储存所有行(带换行符)到文件路径,divide=None则使用系统的换行符
    '''
    if 'b' in mode:
        raise Exception('不支持二进制行储存')
    with open(filepath,mode,encoding=encoding) as fs:
        if divide == None:
            divide = cmd_divide_linux
        
        fs.write(divide.join(lines))

        
def readall(filepath,mode='r',encoding='utf-8'):
    '''
    读取文件所有数据
    '''
    if 'b' in mode:
        encoding=None
    with open(filepath,mode,encoding=encoding) as fs:
        if 'b' not in mode:
            allstr = fs.read()
            # okstr = allstr[0:allstr.__len__() - 1]
            okstr = allstr.rstrip()
            return okstr
        return fs.read()
def readlines(filepath,mode='r',encoding='utf-8'):
    '''
    读取文件所有行
    '''
    if 'b' in mode:
        raise Exception('不支持二进制文件行读取')
    lines = []
    with open(filepath,mode,encoding=encoding) as fs:
        lines = fs.readlines()
    return lines

def getEncoding(filepath,confidence=0.9,raiseErr=False):
    import chardet
    data = readall(filepath,'rb')
    res = chardet.detect(data)
    if res['confidence'] >= confidence:
        return res['encoding']
    elif raiseErr:
        raise Exception(f'confidence确认失败，res={res}')
    return None


def convertEncoding(filepath,fromEncoding,toEncoding):
    '''
    将文本格式转换为其他格式
    '''
    data = readall(filepath,encoding=fromEncoding)
    edata = data.encode(toEncoding,errors='ignore').decode(toEncoding,errors='ignore')
    savedata(edata,filepath,encoding=toEncoding)
def convertEncoding2UTF8NOBOM(filepath,confidence=0.9,raiseErr=False):
    encoding = getEncoding(filepath,confidence,raiseErr)
    convertEncoding(filepath,encoding,'utf-8')
    
def num2block(num:int,divide,shift=0):
    '''
    将数字切割为divide值的块\n
    com.num2block(100,10,2) -> [12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 2]
    '''
    noshift = [divide]* int(num / divide) + [num % divide]
    shifted = list(map(lambda x: x+shift,noshift))
    return shifted
def num2stepblock(num:int,divide,shift=0,contain_start=False,contain_end=True):
    '''
    将数字转为相邻值差相同的数组 \n
    com.num2stepblock(100,10,2,True,True) -> [2, 12, 22, 32, 42, 52, 62, 72, 82, 92, 102, 102]
    '''
    block = num2block(num,divide,shift=0)
    stepblock = []
    for i in range(0,block.__len__()):
        stepblock.append(sum(block[0:i+1]))
    if contain_start:
        stepblock = [0] + stepblock
    if not contain_end:
        stepblock.pop()
    shifted = list(map(lambda x: x+shift,stepblock))
    return shifted
def list2Queue(l):
    '''
    将数组转为队列
    '''
    from queue import Queue
    q = Queue()
    for x in l:
        q.put(x)
    return q
def Queue_putlist(q,l):
    '''
    将数组中的值依次放入队列中
    '''
    [q.put(x) for x in l]

def valueFormat(value):
    '''
    用于对数据库传值，将python数据转为数据库数据格式
    '''
    if isinstance(value,datetime):
        return datetimeFromat(value)
    return value

def isNoneOrEmpty(val):
    '''
    判断字符串是否为空
    '''
    if val == None:
        return True
    if hasattr(val,'__len__') and val.__len__() == 0:
        return True
    return False
def unpackList(ls:list):
    '''
    将多层级数组解为最低层级的数组
    '''
    if ls.__len__() == 0:
        return ls
    if not isinstance(ls,list):
        return ls
    for index in range(0,ls.__len__()):
        if isinstance(ls[index],list) and ls[index].__len__() == 1:
            ls[index] = ls[index][0]
    return ls

def removeList(sourceList:list,remove:list):
    '''
    从数组中移除指定数组中的值
    '''
    for x in remove:
        sourceList.remove(x)
    
def convertsep(path:str):
    '''
    将路径分隔符替换当前平台分隔符
    '''
    if sys.platform == 'win32':
        return path.replace('/','\\')
    else:
        return path.replace('\\','/')
def convertsep2Unix(path:str):
    '''
    将路径分隔符替换unix分隔符
    '''
    return path.replace('\\','/')
def convertsep2Win32(path:str):
    '''
    将路径分隔符替换windows分隔符
    '''
    return path.replace('/','\\')
def convertsep2Target(path:str,sep):
    '''
    将路径分隔符替换为指定分隔符
    '''
    return path.replace('\\',sep).replace('/',sep)


def convert2CSBoolean(val:bool)->str:
    '''
    将python布尔值转为c#布尔字符串
    '''
    if val == False:
        return 'false'
    elif val == True:
        return 'true'
    raise Exception(f'val {val} 非法')

def get_host_ip():
    '''
    获取当前ip,
    已经缓存到ip了
    '''
    try:
        s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        s.connect(('192.168.2.199', 8080))
        ip = s.getsockname()[0]
    finally:
        s.close()
    return ip
def choose(flag,left,right):
    '''
    根据flag条件，true返回left，false返回right
    '''
    if flag:
        return left
    else:
        return right
def remove_match(l:list,func):
    '''
    移除数组中满足func条件的值，返回所有移除值的数组
    '''
    needremove = []
    for e in l:
        if func(e):
            needremove.append(e)
    removeList(l,needremove)
    return needremove

def listdir_fullpath(path,filterfunc=None) -> list:
    '''
    列出路径内所有文件路径，路径以绝对路径表示
    '''
    paths = map(lambda x: os.path.join(path,x),os.listdir(path))
    if filterfunc != None:
        paths = filter(filterfunc,paths)
    return list(paths)
def get_logfile_path(filename):
    '''
    生成日志文件储存路径
    '''
    dirname = os.path.join(workdir,'_log',timemark)
    if not os.path.exists(dirname):
        os.makedirs(dirname)
    return os.path.join(dirname,filename)
def remove_space(l:list):
    '''
    移除数组中的空字符
    '''
    return list(filter(lambda x: x != None and (not isinstance(x,str) or x.strip() != ''),l))
def isnumeric(s:str):
    '''
    判断字符串是否是数字字符串
    '''
    m = re.match('^[\d]*$',s)
    if m != None:
        return True
    return False
def isalpha(s:str):
    '''
    判断字符串是否是字母字符串
    '''
    m = re.match('^[a-zA-Z]*$',s)
    if m != None:
        return True
    return False

from urllib import parse
def urlencode(url,plus=False):
    '''
    编码url
    '''
    if plus:
        return parse.quote_plus(url)
    else:
        return parse.quote(url)
def urldecode(url,plus=False):
    '''
    解码url
    '''
    if plus:
        return parse.unquote_plus(url)
    else:
        return parse.unquote(url)
def gethomepath():
    '''
    获取当前用户的用户目录
    '''
    return os.path.expanduser('~')

def get_physical_memory_GB():
    '''
    获取当前可用物理内存
    '''
    def win():
        out,code = cmd(f'Powershell -Command "& {{(Get-WmiObject -Class Win32_ComputerSystem).TotalPhysicalMemory /1gb}}"')
        return out.strip()
    def mac():
        out,code = cmd('top -l 1 | head -n 10',getstdout=True,errException=Exception('top命令执行失败'))
        m = re.search(r'PhysMem: (.*?) used \(.*? wired\), (.*?) unused',out)
        if m == None:
            raise Exception('获取内存信息失败了')
        usedMem:str = m.group(1)
        unusedMem:str = m.group(2)
        def getGB(numstr)->int:
            if numstr[-1] == 'M':
                return int(int(numstr.rstrip('M'))/1000)
            elif numstr[-1] == 'G':
                return int(numstr.rstrip('G'))
            else:
                raise Exception(f'不支持的内存信息{out}')
        return getGB(usedMem) + getGB(unusedMem)
    def linux():
        raise Exception('不支持')
    return getvalue4plat(win,linux,mac)()
    # meminfo = open('/proc/meminfo').read()
    # matched = re.search(r'^MemTotal:\s+(\d+)',meminfo)
    # return int(matched.groups()[0])

def str2boolean(v):
    '''
    将类似是否的字符串转为python布尔值，主要用于jenkins传参解析
    '''
    if isinstance(v, bool):
        return v
    if v.lower() in ('yes', 'true', 't', 'y', '1'):
        return True
    elif v.lower() in ('no', 'false', 'f', 'n', '0'):
        return False
    else:
        raise Exception(f'非法布尔值{v}')

def str2list(v:str):
    '''
    逗号、分号、空格分隔的字符串转为数组，主要用于jenkins传参解析
    '''
    paramList = []
    if ',' in v:
        paramList = v.split(',')
    elif ';' in v:
        paramList = v.split(';')
    elif ' ' in v:
        paramList = v.split(' ')
    else:
        if not isNoneOrEmpty(v):
            paramList.append(v)
    return paramList
def str2Noneable(v):
    '''
    将None字符串转为None，主要用于jenkins传参解析
    '''
    if v == 'None':
        return None
    return v
def str2enum(enumtype:T,v)->T:
    defaultName = enumtype._member_names_[0]
    return enumtype._member_map_.get(v,enumtype[defaultName])
def str2enum_byValue(enumtype:T,v)->T:
    defaultName = enumtype._member_names_[0]
    return enumtype._value2member_map_.get(v,enumtype[defaultName])
def enumNameTranslate(en):
    if hasattr(en,'translate'):
        return en.translate.value[en.name]
    return en.name

def filterStr(str, rep = ''):
    '''
    过滤特殊字符
    '''
    return re.sub('\W+', rep, str)

def bool2lowerstr(b):
    if b:
        return 'true'
    else:
        return 'false'
def safeBoolCompare(left,right:bool)->bool:
    '''
    是否相等比较
    '''
    return isinstance(left,bool) and left == right
def isFolderEmpty(path):
    ls = os.listdir(path)
    if ls.__len__() == 0:
        return True
    return False
def remove_blank(ls:list):
    '''
    移除数组中的空字符
    '''
    # return filter(lambda x: x in ('',None) or re.match(r'[\s]*',x))
    return remove_space(ls)
def ispath(s):
    """
    判断字符串是否是路径
    """
    ispath = True
    if s.__len__() > 256:
        ispath = False
    else:
        ispath = os.path.exists(s)
    return ispath

def getClassMethods(cls):
    """
    获取类的所有非内置函数
    """
    ret = {}
    for k,v in cls.__dict__.items():
        k:str
        if not isalpha(k) or not callable(v):
            continue
        
        ret[k] = v
    return ret
def counter(ls,func):
    '''
    根据func条件返回ls数组中满足条件的个数
    '''
    count = 0
    for e in ls:
        if func(e):
            count += 1
    return count
def list2block(ls,step):
    '''
    将一个列表拆分为多个块
    '''
    blocks = [ls[i:i + step] for i in range(0, len(ls), step)]
    return blocks
def abspath2relpath(abspath:str,workpath:str):
    """
    根据工作路径将绝对路径转换成相对路径
    """
    return abspath.replace(workpath,'',1).strip(os.path.sep)

def dotstrcompare(left:str,right:str):
    '''
    x.x.x.x类似的字符串的大小比较
    left >= right 返回true
    '''
    leftsp = left.split('.')
    rightsp = right.split('.')


    a = leftsp
    b = rightsp
    isreverted = False
    if rightsp.__len__() > leftsp.__len__():
        a = rightsp
        b = leftsp
        isreverted = True
    def f():
        for index,v in enumerate(a):
            if index >= b.__len__():
                return True
            if int(a[index]) < int(b[index]):
                return False
            elif int(a[index]) > int(b[index]):
                return True
        return True

    ret = f()
    if isreverted:
        ret = not ret
    return ret


def findone(path,findfunc=None):
    '''
    获得文件夹内第一个文件
    '''
    ls = os.listdir(path)
    if ls.__len__() == 0:
        raise Exception(f'{path}空文件夹')

    targetname = ls[0]

    if findfunc == None:
        return os.path.join(path,targetname)
    for filename in ls:
        if findfunc(filename):
            return os.path.join(path,filename)
    
    raise Exception(f'{path}未匹配到文件，{ls}')
def findone_list(ls,findfunc):
    for filename in ls:
        if findfunc(filename):
            return filename
    return None

def listdir_typelist(path):
    '''
    return dirs,files,links,mounts
    获取文件夹内各种类型文件数组（全路径）
    '''
    objs = os.listdir(path)
    dirs = []
    files = []
    links = []
    mounts = []
    for obj in objs:
        file_fullpath = os.path.join(path,obj)
        if os.path.isdir(file_fullpath):
            dirs.append(file_fullpath)
        elif os.path.isfile(file_fullpath):
            files.append(file_fullpath)
        elif os.path.islink(file_fullpath):
            links.append(file_fullpath)
        elif os.path.ismount(file_fullpath):
            mounts.append(file_fullpath)
        else:
            logout(f'unkonw type: {file_fullpath} ')
    return dirs,files,links,mounts
    


def whoami():
    out,code = cmd('whoami',showlog=False,getstdout=True,errException=Exception('获取当前用户名失败'))
    return out.strip()

def sort(ls,compare):
    length = len(ls)
    if length < 2:
        return ls
    # 选择
    for count in range(0,length):
        for index,val in enumerate(ls):
            if not compare(ls[count],val):
                tmp = ls[count]
                ls[count] = ls[index]
                ls[index] = tmp

    # 冒泡
    # for count in range(0,length):
    #     for index,val in enumerate(ls[:length-count-1]):
    #         if compare(val,ls[index + 1]):
    #             tmp = ls[index + 1]
    #             ls[index + 1] = ls[index]
    #             ls[index] = tmp

    return ls



    # curindex = 1
    
    # length = len(ls)
    # for index1,val1 in enumerate(ls):
    #     if curindex > length - 1:
    #         break
    #     for index2,val2 in enumerate(ls[curindex:],start=curindex):
    #         if compare(val1,val2):
    #             tmp = ls[index1]
    #             ls[index1] = ls[index2]
    #             ls[index2] = tmp
    #     curindex += 1
    # return ls
def isPacked():
    '''
    pyinstaller:是否处于打包为可执行文件状态
    '''
    # execname = os.path.basename(sys.executable)
    # if execname in ['python.exe','python']:
    # 这里为True时，用pyinstaller打包,会造成死循环，原因：errorcatch_func(HIGH)装饰器导致
    if not hasattr(sys,'_MEIPASS'):
        return False
    return True
def isPackedSingleFile():
    '''
    pyinstaller:是否打包为单可执行文件
    '''
    basename:str = os.path.basename(sys._MEIPASS)
    if basename.startswith('_MEI'):
        return True
    return False
def isPackedFolder():
    return not isPackedSingleFile()
def convertMEIPASS2localpath_packedSingleFile(filepath):
    '''
    pyinstaller:将单可执行文件的MEIPASS（__file__）转化为磁盘路径
    '''
    execdir = getExecDir()
    localfilepath = filepath.replace(sys._MEIPASS,execdir)
    path = os.path.dirname(os.path.realpath(localfilepath))
    return path
def getExecDir():
    execdir = os.path.dirname(os.path.realpath(sys.executable))
    return execdir
def tryDequeue(Q:Queue,timeout=3):
    val = None
    try:
        val = Q.get(True,timeout=timeout)
    except Empty as e:
        pass
    except Exception as e:
        raise e

    return val
# sys.stdin.detach()

def 质因数二分(val):
    d:List[Tuple[int,int]] = []
    val = int(val)
    for i in range(val - 1):
        # 从2开始
        i += 2
        if val % i == 0:
            div = int(val/i)
            if div < i:
                break
            d.append((i,div))
    return d

def IsDebugMode(modeStr:str):
    modeStr = modeStr.lower()
    if modeStr in ('debug','dev','development','develop'):
        return True
    elif modeStr in ('release','shipping'):
        return False
    raise Exception(f'未知模式{modeStr}')

def deleteFile(*files):
    for file in files:
        if os.path.exists(file):
            os.remove(file)
def getNullfile():
    return getvalue4plat('nul','/dev/null')

def getX509CertFriendlyName(certFilePath,pwd=''):
    '''
    只支持windows
    '''
    assert isWindows()
    pwdarg = ''
    if not isNoneOrEmpty(pwd):
        pwdarg = f',\'{pwd}\''
    out,code = cmd(f'powershell -c "$o = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2 -Args \'{certFilePath}\'{pwdarg};$o.FriendlyName"')
    friendlyName = out.strip()
    return friendlyName

def strCompare(left:str,right:str,toLow=False):
    if toLow:
        left = left.lower()
        right = right.lower()
    
    if left == right:
        return True
    return False

def addAuth2URL(user,password,url:str)->str:
    from urllib3.util import parse_url,Url
    
    urlStruct:Url = parse_url(url)
    ret = Url(urlStruct.scheme,f'{user}:{password}',urlStruct.host,urlStruct.port,urlStruct.path,urlStruct.query,urlStruct.fragment)
    return ret.url

def getPythonRootPath() ->List[str]:
    import sysconfig
    return set(sysconfig.get_path("data", s) for s in sysconfig.get_scheme_names())
def getPythonShareRoot() ->List[str]:
    sharePaths = []
    for p in getPythonRootPath():
        sharePath = os.path.join(p,'share')
        if os.path.exists(sharePath):
            sharePaths.append(sharePath)
    return sharePaths
def getFirstPythonShareFile(relPath):
    for p in getPythonShareRoot():
        targetP = os.path.join(p,relPath)
        if os.path.exists(targetP):
            return targetP


def AddPath2Env(path):
    if 'PYTHONPATH' in os.environ:
        tmp = os.environ['PYTHONPATH']
        os.environ['PYTHONPATH'] = f'{tmp}:{path}'
    else:
        os.environ['PYTHONPATH'] = path



def runShell(shellCmd,**kv):
    c = f'/bin/sh {shellCmd}'
    return cmd(c,**kv)

# 已废弃，用G_timemark
timemark = getlocaltime('-')
# 已废弃，用G_timemark_datetime
timemark_datetime = getdatetimenow()