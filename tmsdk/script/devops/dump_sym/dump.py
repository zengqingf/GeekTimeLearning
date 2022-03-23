# -*- encoding: utf-8 -*-
import sys,os,shutil
sys.path.append(os.path.abspath(os.path.join(__file__,'..','..')))

from comlib import com
from comlib import HTTPManager
from comlib.ftpm import FTPManager
import requests,re

tmpname = 'tmp.sym'
LOG_URL = 'http://39.108.138.140:58981'
FTP_ADDR = ''
rawcrashzipsave = 'rawcrashzip'
rawcrashsave = 'rawcrash'
class Dump(object):
    @staticmethod
    def dumpsym(rawpath,version,buildtsp,plat):
        foldername = Dump.getsymfoldername(version,buildtsp)
        execp = 'bin/dump_syms_'+plat
        if not os.path.exists(execp):
            com.logout(execp + ' NOT EXISTS!!!!')
            return
        
        # if os.path.exists(plat):
        #     shutil.rmtree(plat)

        rawzip = 'rawsym.zip'
        rawsympath = 'rawsym'
        if rawpath.startswith('http'):
            HTTPManager.download_http(rawpath,rawzip)
            com.unzip(rawzip,rawsympath)
        elif rawpath.startswith('ftp'):
            com.buildfolder(rawsympath)
            m = re.search('//(.*?)(/.*$)',rawpath)
            if m == None:
                raise Exception('路径非法 %s'%rawpath)
            host = m.group(1)
            port = 21
            path = m.group(2)
            ftp = FTPManager(host,port,'ftp','123456')
            ftp.download(rawsympath,path)
        rawlist = os.listdir(rawsympath)
        if rawlist.__len__() == 1:
            tarpath = os.path.join(rawsympath,rawlist[0])
        else:
            tarpath = rawsympath

        if plat == 'linux':
            com.cmd('chmod 755 %s'%execp,getstdout=False)
            rawsopath = tarpath
            for dirpath, dirnames, filenames in os.walk(rawsopath):
                print(filenames)

                for f in filenames:
                    if '.so' in f and f != 'libil2cpp.so':
                        sopath = os.path.join(dirpath,f)
                        com.cmd('%s %s > %s 2>/dev/null'%(execp,sopath,tmpname),getstdout=False)

                        Dump._buildsym(plat,foldername)
                        
            # 加固需要拷贝一份id为000000000000000000000000000000000的il2cpp
            il2cpp_path = os.path.join(plat,foldername,'libil2cpp.so')
            baseid = os.listdir(il2cpp_path)[0]
            noid = '000000000000000000000000000000000'
            shutil.copytree(os.path.join(il2cpp_path,baseid),os.path.join(il2cpp_path,noid))

        elif plat == 'mac':
            com.cmd('chmod 755 %s'%execp,getstdout=False)

            dSYMpath = tarpath
            for arch in ['arm64','arm64e','armv7','armv7s']:
                com.cmd('%s -a %s %s > %s'%(execp,arch,dSYMpath,tmpname),getstdout=False)
                Dump._buildsym(plat,foldername)
        else:
            com.logout(plat + ' NOT SUPPORT!!!!')
            return
        # com.cmd('zip -r %s.zip %s'%(foldername,plat),getstdout=False)
        # SEND

        # clear
        # os.remove(rawzip)
        # shutil.rmtree(rawsympath)

    @staticmethod    
    def _buildsym(plat,foldername):
        out,code = com.cmd('head -n 1 '+tmpname)

        if code != 0 or out in (None,'','\n','\r\n'):
            com.logout('ERROR')
            return
        
        desc = out.strip().split(' ')
        print(out)
        elfname = desc[-1]
        elfdesc = desc[-2]
        symname = elfname+'.sym'
        path = os.path.join(plat,foldername,elfname,elfdesc)
        com.buildfolder(path)
        sympath = os.path.join(path,symname)
        if not os.path.exists(sympath):
            shutil.move(tmpname,sympath)
    @staticmethod
    def addr2http(filepath,zipname,plat):
        with open(filepath,'r+') as fs:
            line = fs.readline()
            filterstr = ''
            while line != None:
                m = re.findall(r'[(.*?\.cpp)',line)
                if m != None:
                    for cppfile in m:
                        baseurl = 'http://192.168.2.147/dnl/__androidSymbols/%s/source_il2cpp/%s'%(zipname,cppfile)
                        
                        line.replace(cppfile,baseurl)
    @staticmethod
    def getsymfoldername(packversion,packbuildtsp):
        return packversion+'-'+packbuildtsp
    
    @staticmethod
    def dumpcrashs(path,outpath,packbuildtsp):

        for dirpath, dirnames, filenames in os.walk(path):
            for vaildcrash in filter(lambda x: os.path.getsize(os.path.join(dirpath,x)) > 10*1024 and '.dmp' in x,filenames):
                

                # data = filenames.replace('.zip','').split('_')
                # crash
                tarp = dirpath.replace(path,outpath)
                # # date + time
                tarp = os.path.dirname(tarp)
                # if not os.path.exists(tarp):
                #     com.buildfolder(tarp)

                sp = os.path.split(tarp)
                tmpp = sp[0]
                # # filename = sp[1]
                version = os.path.split(tmpp)[1]
                # # tarp = os.path.join(rawcrashsave,plat,channel,version)


                if 'Android' in tarp:
                    plat = 'linux'
                elif 'ios' in tarp:
                    plat = 'mac'
                else:
                    continue
                # print(tarp + '     '+os.path.join(dirpath,vaildcrash))
                Dump.dumpcrash(os.path.join(dirpath,vaildcrash),dirpath,version,packbuildtsp,plat)
    @staticmethod
    def dumpcrash(crashfilepath,outpath,packversion,packbuildtsp,plat):
        symfoldername = Dump.getsymfoldername(packversion,packbuildtsp)
        symfolder = os.path.join(plat,symfoldername)
        # print('bin/minidump_stackwalk_linux %s %s > %s'%(crashfilepath,symfolder,os.path.join(outpath,os.path.split(crashfilepath)[1].replace('dmp','txt'))))
        dumpname = os.path.basename(crashfilepath).replace('.dmp','.txt')
        dumppath = os.path.join(outpath,dumpname)
        com.cmd('bin/minidump_stackwalk_linux %s %s > %s 2>/dev/null'%(crashfilepath,symfolder,dumppath),getstdout=False)
    @staticmethod
    def downloadcrash():
        req = requests.get(LOG_URL)
        findstr = r'href="(Crash.*?)">'
        savedcrash = 'saved.json'

        m = re.findall(findstr,req.content.decode(encoding='utf-8'))
        if m != None:
            com.buildfolder(rawcrashzipsave)
            com.buildfolder(rawcrashsave)
            filtercrash = set(m)
            if os.path.exists(savedcrash):
                localcrashset = set(com.loadfile_json(savedcrash))
                filtercrash = filtercrash.difference(localcrashset)
                print(localcrashset)
                print(filtercrash)
            com.dumpfile_json(m,savedcrash)
            for fn in filtercrash:
                zippath = os.path.join(rawcrashzipsave,fn)
                HTTPManager.download_http(LOG_URL+'/'+fn,zippath)
                data = fn.replace('.zip','').split('_')
                version = data[1]
                plat = data[2]
                channel = data[3]
                date = data[-2]
                time = data[-1]
                unzippath = os.path.join(rawcrashsave,plat,channel,version,date+'_'+time)
                com.buildfolder(unzippath)
                com.unzip(zippath,unzippath)



if __name__ == "__main__":

    Dump.downloadcrash()
    Dump.dumpcrashs(rawcrashsave,'dumps','0')
    # Dump.dumpsym('http://192.168.2.65/dnf/__dSymbols/debug.zip','1.2.3.4','123413241','linux')
    



    # Dump.dumpcrashs('test_breakpad','dumps','1.2.3.4','123413241','linux')
    # Dump.dumpcrash('test_breakpad/20200304/crash_log_20200304/bd4097f6-0217-4ff9-7b1c12b4-17a0e8ee.dmp','out4.txt','1.2.3.4','123413241','linux')
    pass