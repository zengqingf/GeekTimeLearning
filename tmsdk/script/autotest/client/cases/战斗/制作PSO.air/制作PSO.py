# -*- encoding=utf8 -*-
__author__ = "tengmu"

# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..','..','..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib.comobj import *
from comlib.pathm import Path
from comlib import com,workspace,TMApkManager,TMIPAManager,ApkInfo
from models.testhelp import TestHelper,sleeptouch,logplus,exists_and_touch,adb
#curDevice = G.DEVICE
curSerialno = G.DEVICE.serialno
#curSerialno = 'a15d5e7e'

#deviceinfo = TestHelper.getDeviceInfo()

psopath = os.path.abspath(os.path.join(thisdir,'..','..','..'))



import random,string

from poco.drivers.ue4 import UE4Poco
from models.pocohelp import *
#stop_app("com.tenmove.a8.dev")     
#start_app("com.tenmove.a8.dev")
poco = UE4Poco()

def KillEnemy():
    if not exists(Template(r"tpl1628671761801.png", record_pos=(-0.175, -0.179), resolution=(2400, 1080))):
        touch(Template(r"tpl1628671179685.png", record_pos=(-0.416, -0.177), resolution=(2400, 1080)))
    touch(Template(r"tpl1628671761801.png", record_pos=(-0.175, -0.179), resolution=(2400, 1080)))

    

def DoPlaySkill():   
    #btn_name = "SkillButton_{}"
    #for i in range(16):
        #skill_btn=poco(btn_name.format(i))
        #print("click %s " % skill_btn)
        #sclick(skill_btn,3)
    exists_and_touch(Template(r"tpl1629279132512.png", record_pos=(0.138, 0.05), resolution=(2400, 1080)),5)
    exists_and_touch(Template(r"tpl1629278942265.png", record_pos=(0.132, 0.117), resolution=(2400, 1080)),10)
    exists_and_touch( Template(r"tpl1629279012964.png", record_pos=(0.194, 0.103), resolution=(2400, 1080)),5)
    exists_and_touch( Template(r"tpl1629279429263.png", record_pos=(0.212, 0.175), resolution=(2400, 1080)),5)
    exists_and_touch(Template(r"tpl1629279449105.png", record_pos=(0.214, 0.037), resolution=(2400, 1080)) ,5)
    exists_and_touch( Template(r"tpl1629279455796.png", record_pos=(0.285, -0.033), resolution=(2400, 1080)),5)
    exists_and_touch( Template(r"tpl1629279466777.png", record_pos=(0.285, 0.04), resolution=(2400, 1080)),5)
    exists_and_touch( Template(r"tpl1629279479194.png", record_pos=(0.264, 0.105), resolution=(2400, 1080)),5)
    exists_and_touch(Template(r"tpl1629279511087.png", record_pos=(0.282, 0.173), resolution=(2400, 1080)),5)
    exists_and_touch( Template(r"tpl1629279537433.png", record_pos=(0.358, -0.04), resolution=(2400, 1080)),5)
    exists_and_touch( Template(r"tpl1629279582944.png", record_pos=(0.432, 0.113), resolution=(2400, 1080)),5)
    exists_and_touch( Template(r"tpl1629279549623.png", record_pos=(0.36, 0.021), resolution=(2400, 1080)),5)
    exists_and_touch(Template(r"tpl1629279555972.png", record_pos=(0.355, 0.116), resolution=(2400, 1080)) ,5)
    exists_and_touch( Template(r"tpl1629279576046.png", record_pos=(0.428, 0.041), resolution=(2400, 1080)),5)
    pass

def UploadPSO():
    #if not exists(Template(r"tpl1628671761801.png", record_pos=(-0.175, -0.179), resolution=(2400, 1080))):
    #    touch(Template(r"tpl1628671179685.png", record_pos=(-0.416, -0.177), resolution=(2400, 1080)))
    #sclick(poco("btnUploadPSOCache"))
    psopathuppath = os.path.join(psopath,'CollectedPSOs/')
    Path.ensure_pathnotexsits(psopathuppath)
    adb(curSerialno,f'pull sdcard/UE4Game/NextGenActionGame/NextGenActionGame/Saved/CollectedPSOs/ {psopath}')
    ftppath = com.get_ftp_tempsavepath(f'pso/android/PipelineCaches')
   
    print(f'psopathuppath: {psopathuppath} ftppath {ftppath}')
    G_ftp.upload(psopathuppath,ftppath)
  
wait(Template(r"tpl1628671179685.png", record_pos=(-0.416, -0.177), resolution=(2400, 1080)),120)
        
KillEnemy()
DoPlaySkill()
UploadPSO()

#adb(Serialno,f'devices')













