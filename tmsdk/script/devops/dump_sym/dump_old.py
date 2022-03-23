# -*- encoding: utf-8 -*-
import sys,os,shutil
sys.path.append(os.path.abspath(os.path.join(__file__,'..','..')))

from comlib import com
from comlib import HTTPManager


tmpname = 'tmp.sym'

class Dump(object):
    @staticmethod
    def dumpsym(rawpath,version,buildtsp,plat):
        foldername = Dump.getsymfoldername(version,buildtsp)
        execp = 'bin/dump_syms_'+plat
        if not os.path.exists(execp):
            com.logout(execp + ' NOT EXISTS!!!!')
            return
        if os.path.exists(plat):
            shutil.rmtree(plat)

        rawzip = 'rawsym.zip'
        rawsympath = 'rawsym'
        if rawpath.startswith('http'):
            HTTPManager.download_http(rawpath,rawzip)
        com.unzip(rawzip,rawsympath)
        
        rawlist = os.listdir(rawsympath)
        if rawlist.__len__() == 1:
            tarpath = os.path.join(rawsympath,rawlist[0])
        else:
            tarpath = rawsympath

        if plat == 'linux':
            com.cmd('chmod 755 %s'%execp,getstdout=False)
            rawsopath = tarpath
            for dirpath, dirnames, filenames in os.walk(rawsopath):
                for f in filenames:
                    if '.so' in f:
                        sopath = os.path.join(dirpath,f)
                        p = com.cmd_async('%s %s'%(execp,sopath),workspace='.',getstdout=True)
                        
                        with open(tmpname,'w',encoding='utf-8') as fs:
                            while p.poll() == None:
                                chunk,error = p.communicate()
                                chunk = chunk.decode(encoding='utf-8')
                                fs.write(chunk)
                                fs.flush()
                            

                        Dump._buildsym(plat,foldername)
        elif plat == 'mac':
            com.cmd('chmod 755 %s'%execp,getstdout=False)

            dSYMpath = tarpath
            for arch in ['arm64','arm64e','armv7','armv7s']:
                com.cmd('%s -a %s %s > %s'%(execp,arch,dSYMpath,tmpname),getstdout=False)
                Dump._buildsym(plat,foldername)
        else:
            com.logout(plat + ' NOT SUPPORT!!!!')
            return
        com.cmd('zip -r %s.zip %s'%(foldername,plat),getstdout=False)
        # SEND

        # clear
        os.remove(rawzip)
        shutil.rmtree(rawsympath)

    @staticmethod    
    def _buildsym(plat,foldername):
        out,code = com.cmd('head -n 1 '+tmpname)

        if code != 0 or out == None:
            return com.logout('ERROR')
        
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
    def getsymfoldername(packversion,packbuildtsp):
        return packversion+'-'+packbuildtsp
    @staticmethod
    def dumpcrash(crashfilepath,outfilepath,packversion,packbuildtsp,plat):
        symfoldername = Dump.getsymfoldername(packbuildtsp,packversion)
        symfolder = os.path.join(plat,symfoldername)
        com.cmd('bin/minidump_stackwalk_linux %s %s > %s'%(crashfilepath,symfolder,outfilepath),getstdout=False)



if __name__ == "__main__":
    Dump.dumpsym('http://192.168.2.65/dnf/__dSymbols/debug.zip','1.2.3.4','123413241','linux')
    Dump.dumpcrash('76583ae2-5585-4be8-e69ea0bf-b4798146.dmp','outtttt.txt','1.2.3.4','123413241','linux')
    pass