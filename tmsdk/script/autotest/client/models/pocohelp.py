# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace
from comlib.conf.loader import Loader
from comlib.conf.ref import *

from models.ensure import Ensure
import time

from airtest.core.api import *
from poco.drivers.unity3d import UnityPoco
from poco.proxy import UIObjectProxy
from poco.utils.query_util import build_query

poco = UnityPoco()

def sclick(btn,t=0.1):
    try:
        if btn.exists():
            btn.click()
            time.sleep(t)
        else:
            print("%s btn is not exists" % btn)
    except:
        print("%s btn maybe disable" % btn)

def sclick_long(btn,t=3):
    if btn.exists():
        btn.long_click(duration=t)
        #time.sleep(t)
    else:
        print("%s btn is not exists" % btn)

def waitbtn_click(btn,t=60):
    btn.wait_for_appearance(timeout=t)
    if btn.exists():
        sclick(btn,1)


def input_info(content,info = 'default'):
    content.set_text(info)

def swipeByTime(content , where , time):
    content.swipe(where,duration=time)

class GameHelper(object):
    @staticmethod
    def closeAllAds():
        '''
        关闭角斗场结算的界面
        '''
        if(poco("PKSeasonStart(Clone)").exists()):
            poco(name="Ok").click()
        if(poco("PKSeasonAttrChange(Clone)").exists()):
            poco("Ok").click()
        while True:
            if(poco(name="KnowBtn").exists()):
                poco(name="KnowBtn").click()
            elif(poco("ActiveFuliFrame(Clone)").exists()) :
                poco("ActiveFuliFrame(Clone)").offspring("close").click()
            else:
                break;
        return     
    @staticmethod
    def inputGM(GM):
        '''
        #600000001 金币   
        #600000002 点券
        #300000106 透明晶石
        #200000004 深渊票
        #200000003 远古票        
        #输入GM  固定是附近频道       
        '''
        poco("BtnChat").click()
        poco("tabs").child("tab(Clone)")[3].child("Label").click()
        poco("Input").click()
        text(GM)
        poco("tabs").child("tab(Clone)")[3].child("Label").click()
        poco("Send").child("Text").click()
        sleep(5)
        if(poco(text="点击空白位置关闭").exists()):
            poco(text="点击空白位置关闭").click()
        poco("UI2DRoot").focus([0.8,0.5]).click()
        return    
    @staticmethod
    def closeAny():
        '''
        关闭各种界面 
        '''
        with poco.freeze() as fpoco:
            if(fpoco(nameMatches = "^.*(C|c)lose.*$",type = 'Button').exists()):
                fpoco(nameMatches = "^.*(C|c)lose.*$",type = 'Button').click()
                return True
            elif(fpoco(nameMatches = "^.*(C|c)lose.*$",type = 'Image').exists()):
                fpoco(nameMatches = "^.*(C|c)lose.*$",type = 'Image').click()
                return True
            elif(fpoco(nameMatches = "^.*back.*$",type = 'Button').exists()):
                fpoco(nameMatches = "^.*back.*$",type = 'Button').click()
                return True
        
        return True
    # @staticmethod
    # def closeAll(count=10):

    #     while GameHelper.closeAny() or count > 0:
    #         count -= 1
    @staticmethod
    def rlGame(info):
        '''
        重启游戏。参数为包名\n
        需要重新声明赋值初始化 poco
        '''
        global poco
        Ensure.app_newestopen(info)
        sleep(5)
        poco = UnityPoco()
        sleep(1)
        return
    @staticmethod
    def getVipLevel():
        '''
        获得vip等级  
        返回值int类型
        '''
        level=poco("VipLevel").offspring("Text").get_text()[1:]
        return int(level)
        
    @staticmethod
    def getCurPiLao():
        '''
        获取当前疲劳。返回值为int类型     
        '''
        pilao=poco("MainHead").offspring("PiLao").offspring("content").get_text()
        pilaoindex=pilao.index("/")
        return int(pilao[0:pilaoindex])


    @staticmethod
    def login(serverType,serverName,account):
        '''
        登录。需要输入大区名称，服务器名称，账号
        '''
        if(poco(text="确 认").exists()):
            poco(text="确 认").click()
        sleep(1)
        # 打开服务器列表界面
        poco("LoginFrame(Clone)").offspring("tips").click()
        sleep(1)
        poco("ServerList(Clone)").offspring("Tab").swipe('down')
        #服务器大区列表集合
        fpoco = poco
        # with poco.freeze() as fpoco:
        
        server_type_list=fpoco("UI2DRoot").offspring("Content").child("ServerListTabUnit(Clone)")
        
        for i in range(len(server_type_list)):
            curSerType=server_type_list[i].child("Text").get_text()
            if(curSerType==serverType):
                server_type_list[i].click()
                break
        sleep(3)
        #大区下具体服务器集合
        # with poco.freeze() as fpoco:
        server_name_list=fpoco("UI2DRoot").offspring("Content").child("ServerListUnitUnitServer(Clone)") 
        for j in range(len(server_name_list)):
            curSerName=server_name_list[j].child("name").get_text()
            if(curSerName==serverName):
                server_name_list[j].click()
                break         
        # with poco.freeze() as fpoco:
        fpoco("LoginFrame(Clone)").offspring("AccountInput").click()
        sleep(2)
        text(account)
        fpoco("LoginFrame(Clone)").offspring("AccountInput").click()
        fpoco("LoginFrame(Clone)").offspring("Enter").click()
        sleep(3)
        return

    
    @staticmethod
    def choseRole(roleNumber):
        '''
        选择第几个角色进入游戏 从0开始
        '''
        if(roleNumber>7):
            poco("ArrowRight").click()
            roleNumber=roleNumber-8
        poco("Role"+str(roleNumber)).click()
        poco("BtnStart").click()
        sleep(2)
        return
    
    @staticmethod
    def changeRole(roleNumber):
        '''
        切换角色 从0开始算第一个角色
        '''
        poco("MainHead").child("Icon").click()
        poco("RoleChangeBtn").click()
        sleep(5)
        if(roleNumber>7):
            poco("ArrowRight").click()
            roleNumber=roleNumber-8
        if(roleNumber<=7 and poco("ArrowLeft").exists()):
            poco("ArrowLeft").click()
        poco("Role"+str(roleNumber)).click()
        poco("BtnStart").click()
        return  
    
    @staticmethod
    def creatRole(zhiYe,xiaoZhiYe):
        '''
        创建角色。输入正确的职业中文
        '''
        poco("SelectRoleFrame(Clone)").offspring("BtnCreate").offspring("Text").click()
        job=["战士","枪手","武术师","神谕者","法师"]
        jobId=job.index(zhiYe)
        poco("BaseJobScrollView").offspring("Content").child("Toggle(Clone)")[jobId].click()
        sleep(3)
        job2=poco("ChangeJobSelected").child("Toggle(Clone)")
        for proJob in job2:
            if(proJob.offspring("Text").get_text()==xiaoZhiYe):
                proJob.click()
                sleep(3)
                break
        poco("BtnRandomName").click()
        poco("CreateRoleFrameNew(Clone)").offspring("BtnCreate").focus([0.5,0.5]).click()
        sleep(5)
    @staticmethod
    def challenge(dun,dun2,level):
        '''
        打开秘境 进入地下城  challenge（“深渊地下城”，“龙脊深渊”，“王者”）
        '''
        poco("challenge").child("icon").click()
        dungeonList=poco("modelTab").offspring("ViewScroll").offspring("ViewPort").offspring("Content").child()
        dungeonList2=poco("viewRoot").offspring("ItemRoot").child("ChallengeMapItemPrefab(Clone)")
        dungeonDiffcultList=poco("levelRoot").offspring("Content").child()
        #查找地下城分类是否符合
        for dungeon in dungeonList:
            dungeon.click()
            if(dungeon.offspring("selectedTabGo").exists()==False):
                continue
            if(dungeon.offspring("selectedTabGo").offspring("selectedTabNameLabel").get_text()==dun):
                break
        #查找地下城是否符合
        for dungeon2 in dungeonList2:
            if(dungeon2.offspring("Name").get_text()==dun2):
                dungeon2.click()
                break
        #查找难度是否符合
        for dunLevel in dungeonDiffcultList:
            if(dunLevel.get_name()=="levelItem_Template"):
                continue
            if(dunLevel.offspring("levelName").get_text()==level):
                dunLevel.click()
                break
        poco("GroupStart").click()        
        sleep(10)
        #检测自动战斗按钮是否开启
        if(not poco("AutoFight").offspring("EffUI_Autofight(Clone)").exists()):
            poco("AutoFight").offspring("Background").click()
        #等待阶段    
        poco("DungeonNormalFinish(Clone)").child("r").offspring("Score").wait_for_appearance(timeout=600)  
        #等待翻牌
        sleep(20)    
        #检查是否出现商人
        while True:
            if(poco("TalkBottom(Clone)").exists()):
                poco("Btn (1)").click()
                sleep(1)
            else:
                GameHelper.closeAny()
                sleep(1)
                break
        poco("UI2DRoot").offspring("BackTown").click() 
        sleep(10)
        GameHelper.closeAny()
        sleep(1)
        GameHelper.closeAny()
        sleep(1)
        return
    
    @staticmethod
    def autoSkill():
        '''
        自动配置所有技能
        '''
        poco("skill").click()
        poco("UI2DRoot").offspring("btSkillPlan").click()
        poco("UI2DRoot").offspring("skillConfigButton").click()
        if(poco("rightButton").exists()):
            poco("rightButton").click()
        GameHelper.closeAny()
        if(poco("rightButton").exists()):
            poco("rightButton").click()
        return

    @staticmethod
    def resolveEquip():
        '''
        分解装备
        '''
        poco("packge").click()
        poco("UI2DRoot").offspring("PackageNew(Clone)").offspring("QuickDecompose").click()
        T1=poco("UI2DRoot").offspring("PackageNew(Clone)").offspring("Toggle1")
        T2=poco("UI2DRoot").offspring("PackageNew(Clone)").offspring("Toggle2")
        T3=poco("UI2DRoot").offspring("PackageNew(Clone)").offspring("Toggle3")
        if(not T1.attr("toggleIsOn")):
            T1.offspring("Checkmark").click()
        if(not T2.attr("toggleIsOn")):
            T2.offspring("Checkmark").click()
        if(not T3.attr("toggleIsOn")):
            T3.offspring("Checkmark").click()
        if(poco("UI2DRoot").offspring("PackageNew(Clone)").offspring("Confirm").exists()):
            poco("UI2DRoot").offspring("PackageNew(Clone)").offspring("Confirm").click()
        #没有可以分解的装备
        if(poco("AlertText").exists()):
            poco("btOK").click()
        #等待分解成功的动画
        sleep(3)    
        if(poco("UI2DRoot").offspring("PackageNew(Clone)").offspring("Cancel").exists()):
            poco("UI2DRoot").offspring("PackageNew(Clone)").offspring("Cancel").click()
        if(poco(text="装备分解成功").exists()):
            poco("UI2DRoot").offspring("DecomposeResult(Clone)").offspring("Close").offspring("Image").click()
        
        GameHelper.closeAny()       
        return

    
    @staticmethod
    def sellEquip():
        '''
        自动出售白色蓝色紫色品质装备
        '''
        poco("packge").click()
        poco("UI2DRoot").offspring("PackageNew(Clone)").offspring("QuickSell").click()
        T1=poco("UI2DRoot").offspring("PackageNew(Clone)").offspring("Toggle1")
        T2=poco("UI2DRoot").offspring("PackageNew(Clone)").offspring("Toggle2")
        T3=poco("UI2DRoot").offspring("PackageNew(Clone)").offspring("Toggle3")
        if(not T1.attr("toggleIsOn")):
            T1.offspring("Checkmark").click()
        if(not T2.attr("toggleIsOn")):
            T2.offspring("Checkmark").click()
        if(not T3.attr("toggleIsOn")):
            T3.offspring("Checkmark").click()
        poco("UI2DRoot").offspring("PackageNew(Clone)").offspring("Confirm").child("Text").click()
        if(poco("btOK").exists()):
            poco("btOK").click()
        if(poco("UI2DRoot").offspring("Desc").exists()):
            poco("UI2DRoot").offspring("Desc").click()
        #没有可以出售的装备
        if(poco("AlertText").exists()):
            poco("btOK").click()
            poco("UI2DRoot").offspring("PackageNew(Clone)").offspring("Cancel").click()
        GameHelper.closeAny()
        return

    
    @staticmethod
    def openInventory():
        '''
        开启背包所有格子
        '''
        if(poco("Title").exists()):
            if(poco("UI2DRoot").offspring("PackageNew(Clone)").exists()):
                print("已经打开背包了")
            else:
                print("当前不是背包界面。关闭")
                GameHelper.closeAny()
                sleep(1)
                poco("packge").child("icon").click()
        else:
            poco("packge").child("icon").click()
        slotId=["18","28","38","48","58","68","78"]
        slotId2=["30","40","50","60","70","80","90"]
        i=0
        beibao=poco("UI2DRoot").offspring("PackageNew(Clone)").offspring("ItemListView")
        while True:
            beibao.offspring("ItemRoot_"+slotId[i]).offspring("SlotGroup").swipe("up")
            sleep(1)
            beibao.offspring("ItemRoot_"+slotId[i]).offspring("SlotGroup").swipe("up")
            sleep(1)
            if(beibao.offspring("ItemRoot_"+slotId2[i]).offspring("SlotGroup").offspring("Locked").exists()):
                beibao.offspring("ItemRoot_"+slotId2[i]).offspring("SlotGroup").click()
                sleep(1)
                poco("btOK").click()
                if(poco("UI2DRoot").offspring("FixTitle").exists()):
                    print("金币不足呀兄弟~！")
                    poco(type="Text",name="Text",text="前往").click()
                    GameHelper.closeAny()
                    return  
            i=i+1
            if(i==7):
                break
        return
    
    @staticmethod
    def betterEquip():
        '''
        自动穿上更好的装备
        '''
        poco("packge").click()
        poco("UI2DRoot").offspring("PackageNew(Clone)").offspring("Arrange").click()
        equip=poco("UI2DRoot").offspring("PackageNew(Clone)").offspring("ItemListView").offspring("Content").child()
        
        for qui in equip:
            if(qui.offspring("BetterArrow").exists()):
                qui.offspring("Icon").click()
                poco("UI2DRoot").offspring("ItemTip(Clone)").offspring("Special").click()
                if(poco("btOK").exists()):
                    poco("btOK").click()
                else:
                    continue
            else:
                continue
        sleep(1)
        GameHelper.closeAny()
        return
    @staticmethod
    def openAllMail():
        '''
        领取邮件
        '''
        if(poco("NewMailNotice").exists()):
            poco("NewMailNotice").click()
        else:
            return
        if(poco("MailFrameNew(Clone)").offspring("BtnAcceptAll").exists()):
            poco("MailFrameNew(Clone)").offspring("BtnAcceptAll").click()
        #切换到公告页签
        poco("TabItem_0").child("name").click()
        if(not poco(name="NoMailTip").exists()):
            print("公告是空的。没有邮件")
        #少个领取按钮的，公告没有邮件。暂时获取不到
            
            
        GameHelper.closeAny()
        return

    @staticmethod
    def chapterDungeon(chaNo):
        '''
        输入章节。进入对应的章节并选择第一个地图自动战斗 章节从0开始 0-9
        '''
        GameHelper.closeAny()
        #判断是否在两个特殊地图内 第八章和 第十章
        poco("viewport").click()
        if(poco("scene6037JumpNode").exists()):
            poco("scene6037JumpNode").click()
            sleep(20)
        if(poco("scene6020JumpNode").exists()):
            poco("scene6020JumpNode").click()   
            sleep(20)
        GameHelper.closeAny()
        sleep(1)
        #如果是贺顿内城。就直接打开地图。如果不是。就前往贺顿内城
        curPosition=poco("minimap").offspring("title").child("Text").get_text()
        if not(curPosition=="赫顿内城"):
            poco("viewport").click()
            sleep(2)
            poco("scene6001").child("npc1 (1)").child("Face").click()
            sleep(20)
            GameHelper.closeAny()
        #打开地图   
        poco("viewport").click()
        sleep(2)
        #第8章节 战国
        if(chaNo==7):
            poco("scene6024JumpNode").click()
            sleep(20)
        #第10章节 格鲁小镇
        if(chaNo==9):
            poco("scene6043JumpNode").click()
            sleep(20)
        
        chapter=["LV 1~7","LV 7~20","LV 21~30","LV 30~32","LV 31~40","LV 41~49","LV 49~54","LV 55~59","LV 59~62","LV 63~65",]
        scenes=poco("scenes").child()
        for scene in scenes:
            if(scene.offspring("Text").exists()):
                if(scene.offspring("Text").get_text() == chapter[chaNo]):
                    #通过搜索等级来判断章节 然后点击等级
                    scene.offspring("Text").focus([0.5,-1]).click()
                    sleep(20)
                    break
        #战斗 选择第一个关卡
        #第九章选择第二个关卡。第一个是飞行关卡。没有自动战斗。
        if(chaNo==8):
            poco("Level1").click()
            sleep()
        else:
            poco("Level0").click()
            sleep(1)
        poco("GroupStart").click()        
        sleep(10)
        #检测自动战斗按钮是否开启
        if(not poco("AutoFight").offspring("EffUI_Autofight(Clone)").exists()):
            poco("AutoFight").offspring("Background").click()
        #等待阶段    
        poco("DungeonNormalFinish(Clone)").child("r").offspring("Score").wait_for_appearance(timeout=600)  
        #等待翻牌
        sleep(20)    
        #检查是否出现商人
        while True:
            if(poco("TalkBottom(Clone)").exists()):
                poco("Btn (1)").click()
                sleep(1)
            else:
                GameHelper.closeAny()
                sleep(1)
                break
        poco("UI2DRoot").offspring("BackTown").click() 
        sleep(10)
        GameHelper.closeAny()
        sleep(1)
        poco("viewport").click()
        #出来后返回贺顿内城
        if(chaNo==7):
            poco("scene6020JumpNode").click()
            sleep(20)
        if(chaNo==9):
            poco("scene6037JumpNode").click()
            sleep(20)
        poco("scene6001").child("npc1 (1)").child("Face").click()
        sleep(20)
        return


    @staticmethod
    def closeNewItemTips():
        '''
        关闭新道具提醒
        '''
        while True:
            if(poco("EquipmentChangedFrame(Clone)").offspring("Title").exists()):
                GameHelper.closeAny()
            else:
                break
        return





