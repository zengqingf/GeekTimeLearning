# -*- encoding=utf8 -*-
__author__ = "tengmu"

# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..','..','..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace,TMApkManager,TMIPAManager,ApkInfo
from models.testhelp import TestHelper,sleeptouch,logplus,exists_and_touch
curDevice = G.DEVICE
curSerialno = G.DEVICE.serialno



import random,string

from models.pocohelp import *
poco = UnityPoco()


auto_setup(__file__)
if True:
    device().focus_rect = [0,30,0,0]


def SelectRole(index = 0):
    #poco(texture="UI_Chuangjiao_Guijianshi_Di")
    poco("Content").child("Toggle(Clone)")[index].child("JobName").click()
    
def random_name(name_len):       
    str_list = random.sample(string.digits + string.ascii_letters,name_len)    				
    random_str = ''.join(str_list)    
    return random_str



def joystickSwipe(where,time = 3):
    joystick = poco("ETCJoystick")
    joystick.swipe(where,duration = time)
    
def doSkip():
    next_btn = poco('BtnNext')
    skip_btn = poco('BtnStepOver')
    cmp_btn = poco('BtnComplete')
    cmp_btn_1 = poco('BtnComplete (1)')
    while True:
        if cmp_btn_1.exists():
            sclick(cmp_btn_1)
        elif skip_btn.exists():
            sclick(skip_btn)
        elif cmp_btn.exists():
            sclick(cmp_btn)
        elif next_btn.exists():
            sclick(next_btn)
        else:
            break

def Auto_Fight(srange = 3):
    attack_btn = poco("Btn_Attack")
    skill_btn_str = "Btn_Skill{}"
    sclick_long(attack_btn,3)
    for i in range(1,srange):
        skill_btn=poco(skill_btn_str.format(i))
        sclick(skill_btn)

def Go_next_Door(where = 'right'): #地图转一圈总会过门的s
    joystick = poco("ETCJoystick")
    if where == 'right':
        #sclick_long(joystick.focus([0.5,0.9]),3)
        sclick_long(joystick.focus([0.9,0.5]),9)
        sclick_long(joystick.focus([0.5,0.1]),3)
        sclick_long(joystick.focus([0.5,0.9]),3)
    elif where == 'down':
        sclick_long(joystick.focus([0.5,0.9]),3)
        sclick_long(joystick.focus([0.9,0.5]),9)
        sclick_long(joystick.focus([0.1,0.5]),9)
        #sclick_long(joystick.focus([0.5,0.9]),3)
    time.sleep(3)

def DoCreate_Guide():
    if poco("CommonMsgBoxOKCancel(Clone)").exists():
        poco("btCancel").click()
    if poco("Skip").exists():
        sclick(poco("Skip"),1)
    doSkip()
    time.sleep(3)
    joystick = poco("ETCJoystick")
    sclick_long(joystick.focus([0.9,0.5]),3)
    poco("Btn_Attack").click()
    sclick_long(poco("Btn_Attack"),4)
    time.sleep(3)
    Go_next_Door()
    doSkip()
    poco("Btn_JumpBack").wait_for_appearance()
    sclick(poco("Btn_JumpBack"),5)
    while True:       
        if poco("ArrowRightGoTips(Clone)").exists():
            Go_next_Door()
            break
        if poco('TalkBottom(Clone)').exists(): #放技能直接过门了
            doSkip()
            break 
        else: 
            Auto_Fight(4)
    time.sleep(4)
    doSkip()
    #sclick_long(attack_btn,3)
    while True:
        if poco("TalkBottom(Clone)").exists():
            doSkip()
            break               
        else:    
            Auto_Fight(5)

def EnterGameorCreate(iscreate = True):
    if poco(text = '创建角色').exists():
        iscreate = True
    else:
        if poco("OldPlayerFrame(Clone)").exists():
            sclick(poco("OldPlayerFrame(Clone)").child('close'))
    if iscreate:
        sclick(poco("BtnCreate"),2)
        poco("BtnRandomName").click()
        #create_btn = poco("BtnCreate")
        create_btn = poco("CreateRoleFrameNew(Clone)").child('BtnCreate')
        sclick(create_btn,10)
        #if poco("CommonMsgBoxOKCancel(Clone)").exists():
         #   poco("btOK").click()
          #  poco("BtnRandomName").click()
           # sclick(create_btn,10)
        DoCreate_Guide()
    else:
        print ("do enter game")

def Do_Task_Go(): #任务的处理
    auto_content = poco(textMatches = "^.*[主].*$")
    next_btn = poco('BtnNext')
    skip_btn = poco('BtnStepOver')
    cmp_btn = poco('BtnComplete')
    enter_btn = poco(texture = 'UI_Daily_Kaishitiaozhan_Zi')
    while True:
        if is_Have_GuideFinger():
            Do_Click_Finger()
        elif auto_content.exists() and not next_btn.exists() and not cmp_btn.exists():
            sclick(auto_content,5)
        elif skip_btn.exists():
            sclick(skip_btn)
        elif cmp_btn.exists():
            sclick(cmp_btn)
        elif next_btn.exists():
            sclick(next_btn)
        elif enter_btn.exists():
            break
        else:
            break
            
def Do_Click_Finger():
    fight_btn = poco('Finger')
    while True:
        if fight_btn.exists():
            sclick(fight_btn,1)
        else:
            break
def is_Have_GuideFinger():
    fight_btn = poco('Finger')
    if fight_btn.exists():
        return True
    else:
        return False

def EndFirstFight():
    poco(text = '点击前往交付').wait_for_appearance()
    value = poco("MissionDesc").get_text()
    assert_equal(value,"点击前往交付","第一场战斗结束！！！")
    poco("TalkBottom(Clone)").wait_for_appearance()
    doSkip()
    poco('Finger').wait_for_appearance()
    poco('Finger').click()
    sclick(poco("ETCJoystick").focus([0.9,0.5]))  #终止自动寻路

def FirstFight():
    Do_Task_Go()
    # 开启自动战斗
    poco("AutoFight").child('GameObject').child('Background').click()
    EndFirstFight()

#跳过引导
def doSkip():
    next_btn = poco('BtnNext')
    skip_btn = poco(text = '跳过')
    cmp_btn = poco('BtnComplete')
    cmp_btn_1 = poco('BtnComplete (1)')
    while True:
        if cmp_btn_1.exists():
            sclick(cmp_btn_1)
        elif skip_btn.exists():
            sclick(skip_btn)
        elif cmp_btn.exists():
            sclick(cmp_btn)
        elif next_btn.exists():
            sclick(next_btn)
        else:
            break
#跳过指引字
def doSkipbyText():
    next_btn = poco(text = '点击任意位置进行下一步')
    back_btn = poco(text = '点击空白位置关闭')
    while True:
        if next_btn.exists():
            BackByClickBlack()
        elif back_btn.exists():
            BackByClickBlack()
        else:
            break
#引导手指
def Do_Click_Finger():
    fight_btn = poco('Finger')
    while True:
        if fight_btn.exists():
            sclick(fight_btn,1)
        else:
            break
def BackByClickBlack(x=0.1,y=0.1):    
    sclick(poco("UI2DRoot").focus([x,y]))

    #输入GM命令
def GMInput(gm):
    sclick(poco("BtnChat"))
    sclick(poco(text="附近"))
    poco("Input").set_text(gm)
    sclick(poco("Send"))
    poco(text = "点击空白位置关闭").click()
    BackByClickBlack(0.8,0.5)
def UseFeiShengYao():
    sclick(poco("packge"),1)
    poco("Title3").click()
    #poco("5482024240376807901").click()
    #poco(texture="consumable_17")
    poco("ItemRoot_0").offspring("Icon").click()
    sclick(poco("Special"),5)
    doSkip()
    #sclick(poco('Close'))
def ChooseJob():
    sclick(poco("Job1")) #选择转职职业
    sclick(poco("btChangeJob"),1)
    if poco("CommonMsgBoxOKCancel(Clone)").exists():
        poco("btOK").click()
    doSkip()
    time.sleep(5)
    if exists(Template(r"tpl1602471828623.png", record_pos=(-0.452, -0.252), resolution=(1280, 720))):
        touch(Template(r"tpl1602471828623.png", record_pos=(-0.452, -0.252), resolution=(1280, 720)))#这个地方节点有问题只能用图片

    
    #sclick(poco('Close'))
    

def DoZhuanZhi():
    auto_content = poco(textMatches = "^.*转职.*$",type = 'Text')
    sclick(auto_content)
    if  poco("Job1").exists():
        ChooseJob()    
        sclick(auto_content)
    sclick(poco("FinishButton"),10)
def ZhuanZhiFight():
    #Misson_flag = poco("MissionFlag")
    Misson_flag = poco("Level3")
    waitbtn_click(Misson_flag)
    sclick(poco(texture = 'UI_Daily_Kaishitiaozhan_Zi'))
    if poco("CommonMsgBoxOKCancelNewFrame(Clone)").exists():
        sclick(poco("leftButton"))
    time.sleep(10)
    doSkipbyText()
def EndFight():
    #需要添加战斗结束宠物商人等的出现，处理这些

    #finshtask.wait_for_appearance()
    poco("TalkBottom(Clone)").wait_for_appearance(timeout=1200)
    #检查是否出现商人
    while True:
        if(poco("TalkBottom(Clone)").exists()):
            poco("Btn (1)").click()
            sleep(1)
        else:
            GameHelper.closeAny()
            sleep(1)
            break
    #poco(text = '恭喜你，转职成功了').wait_for_appearance(timeout=300)
    #value = poco("Talk").get_text()
    #assert_equal(value,"恭喜你，转职成功了","转职流程成功了！！！")
    doSkip()
    BackByClickBlack()
    Do_Click_Finger()
    sclick(poco("skillConfigButton"))
    sclick(poco('Close'))
    if poco("CommonMsgBoxOKCancelNewFrame(Clone)").exists():
        sclick(poco("rightButton"))
    doSkipbyText()

def ZhuanZhi():
    auto_content = poco(textMatches = "^.*转职.*$",type = 'Text')
    sclick(auto_content)
    doSkip()
    





EnterGameorCreate()
time.sleep(10)
FirstFight()




GMInput("!!additem id=800001535") #获得飞升药
UseFeiShengYao() #嗑药
ChooseJob()#选择转职职业
DoZhuanZhi()#转职引导
ZhuanZhiFight()#进转职战斗
EndFight()#战斗结束转职成功
ZhuanZhi()#交任务

TestHelper.gotostate('城镇','角色选择')











