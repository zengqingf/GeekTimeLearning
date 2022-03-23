# -*- encoding: utf-8 -*-
import shutil,json,time,socket

from airtest.core.android.adb import ADB
from airtest.core.android.javacap import Javacap
from airtest.core.android.minicap import Minicap

from comlib.comobj import *
from comlib import ThreadManager,Factory
from subprocess import Popen, PIPE
from queue import Queue,Empty
from typing import Tuple,List
import cv2,numpy


from comlib import Path,JsonFile


thisdir = com.getExecDir()


def recv_with_timeout(sk:socket.socket, size, timeout=3):
    sk.settimeout(timeout)
    try:
        ret = sk.recv(size)
    except socket.timeout:
        ret = None
    finally:
        sk.settimeout(None)
    return ret

def safe_recv_block(sk:socket.socket,timeout=3):
    '''
    堵塞
    '''
    header = b''
    c = b''
    while c != b'\n':
        header += c
        c = recv_with_timeout(sk,1,timeout)
        if c in (None,b''):
            return None
        
    length = header.decode(encoding='utf-8')
    data = recv_with_timeout(sk,int(length),timeout)
    if data != None:
        data = data.decode(encoding='utf-8')
    return data

ffmpegp = os.path.join(workdir,'bin','ffmpeg.exe')
fpspath = os.path.join(workdir,'bin','mp4fpsmod.exe')
# 一条线程生成视频 一条截图
# 生成timecode和时间帧数对应文件
# 完成之后使用mp4fpsmod进行帧长调整
# 最终成品：调整后的视频文件、timecode文件、时间帧数对应文件、输出指定帧位置的可执行文件（跳转用）
class GameRecord():
    def __init__(self,devicename,serialno,desirepower=5,clientport=44444) -> None:
        
        
        self.dataDir = os.path.join(workdir,f'record_{G_timemark}_{devicename}')
        self.rawmp4file = os.path.join(self.dataDir,"raw.mp4")
        self.modmp4file = os.path.join(self.dataDir,"mod.mp4")
        self.timecodefile = os.path.join(self.dataDir,'timecode.txt')
        self.framecodefile = os.path.join(self.dataDir,'framecode.txt')
        self.imgs = Queue()
        self.devicename = devicename
        self.serialno = serialno
        self.clientport = clientport

        self.adb = ADB(serialno=self.serialno)
        self.cap = Minicap(self.adb,ori_function=self.adb.get_display_info)
        self.subp = None
        self.tc_fs = None
        self.fc_fs = None
        self.count = 1
        self.isFirst = True
        self.desirepower = desirepower
        

    def setImageResolution(self):
        # a={'width': 1080, 'height': 2340, 'density': 2.75, 'orientation': 0, 'rotation': 0, 'max_x': 1079, 'max_y': 2339}
        # b={'width': 1080, 'height': 2340, 'density': 2.75, 'orientation': 1, 'rotation': 90, 'max_x': 1079, 'max_y': 2339}
        displayinfo = self.adb.get_display_info()
        width = displayinfo['width']
        height = displayinfo['height']
        # 需要保证长宽被2整除，不然windows media player不能播放
        wvs = com.质因数二分(width)
        hvs = com.质因数二分(height)
        # 获取长宽因数相同的值(小于二分)
        okpowers = [1]
        for wv in wvs:
            for hv in hvs:
                if wv[0] == hv[0]:
                    okpowers.append(wv[0])
        okpowers.sort()
        selectpower = okpowers[0]
        for okpower in okpowers:
            if self.desirepower <= okpower:
                selectpower = okpower
                break
        targetwidth = int(width/selectpower)
        targetheight = int(height/selectpower)
        
        self.cap.projection = (targetwidth,targetheight)
        print(f'[pixelpower] desirepower={self.desirepower}  selectpower={selectpower} targetwidth={targetwidth} targetheight={targetheight}')


    def start(self):
        self.setImageResolution()

        cmd = f'{ffmpegp} -y -f image2pipe -vcodec mjpeg -an -i - -vcodec mpeg4 -framerate 30 {self.rawmp4file}'
        # cmd2 = f'{ffmpegp} -y -f image2 -i {self.imgsdir}\\%d.jpg -vcodec mpeg4 -framerate 30 out.mp4'
        self.subp = com.cmd_subp(cmd,stdin=PIPE)

        # adb连接
        self.adb.cmd(f'forward tcp:{self.clientport} tcp:55555')

        Path.ensure_direxsits(self.dataDir)
        self.tc_fs = open(self.timecodefile,'w')
        self.fc_fs = open(self.framecodefile,'w')

        capFactory = Factory(self.runCap,outputQueue=self.imgs,baseName='cap')
        mediaFactory = Factory(self.runMediaGenerate,baseName='mediaGenerate')
        capFactory.addNext(mediaFactory)
        capthds = capFactory.run()
        mediathds = mediaFactory.run()
        ThreadManager.waitall(capthds + mediathds)
        # 清理
        self.tc_fs.close()
        self.fc_fs.close()
        self.cap.teardown_stream()
        self.adb.cmd(f'forward --remove tcp:{self.clientport}')

        self.subp.stdin.flush()
        time.sleep(3)
        self.subp.stdin.close()
        self.subp.wait()

        cmd = f'{fpspath} -t {self.timecodefile} {self.rawmp4file} -o {self.modmp4file}'
        com.cmd(cmd,getstdout=False)

    def runCap(self,factoryIns:Factory,inputQ:Queue,outputQ:Queue):
        sk = socket.socket(socket.AF_INET,socket.SOCK_STREAM)
        try:
            sk.connect(('127.0.0.1',self.clientport))
            sk.send(b'1')
            jdata = safe_recv_block(sk,timeout=5)
            # 接收超时，代表游戏进程已经关闭
            if jdata == None:
                factoryIns.quit()
                return
            data = json.loads(jdata)
            flag = data['flag']
            timecode = data['timecode']
            framecount = data['framecount']
            if flag == 'ok':
                img = self.cap.get_frame_from_stream()
                outputQ.put((img,timecode,framecount))
            elif flag == 'skip':
                sleeptime = 0.002
                time.sleep(sleeptime)
                
            elif flag == 'fail':
                pass
        except Exception as e:
            raise e
        finally:
            sk.close()
    def runMediaGenerate(self,factoryIns:Factory,inputQ:Queue,outputQ:Queue):
        val:Tuple[bytes,str,str] = com.tryDequeue(inputQ)
        if val == None:
            return
        img,timecode,framecount = val

        timecode_ms = round(float(timecode)*1000)
        
        # 如果是第一张图片，则将其转化为灰度图，然后同时将灰度和rpg图写入
        if self.isFirst:
            imga = numpy.asarray(bytearray(img),dtype='uint8')
            cvimg = cv2.imdecode(imga,cv2.IMREAD_GRAYSCALE)
            cvimg = cv2.cvtColor(cvimg,cv2.COLOR_GRAY2RGB)
            suc,grayimga = cv2.imencode('.jpg',cvimg)
            grayimg = grayimga.tobytes()

            self.subp.stdin.buffer.write(grayimg)
            self.tc_fs.write(f'{0}\n')
            self.fc_fs.write(f'{0} {1}\n' )
            self.isFirst = False

        self.subp.stdin.buffer.write(img)

        self.tc_fs.write(f'{timecode_ms}\n')
        self.tc_fs.flush()
        self.fc_fs.write(f'{timecode_ms} {framecount}\n')
        self.fc_fs.flush()

        pass



def mp4frameshow(modmp4file,mp4frameindex,extname=''):
    '''
    ffmpeg的视频帧是从0开始的，习惯上认为帧是从1开始的，这里需要适应
    '''
    cmd = f'{ffmpegp} -y -i {modmp4file} -vf "select=eq(n\,{mp4frameindex})" -vframes 1 -f image2 -'

    data,err = com.cmd(cmd,getstdout=True,decoderet=False)
    imga = numpy.asarray(bytearray(data),dtype='uint8')
    cv2img = cv2.imdecode(imga,cv2.IMREAD_COLOR)
    
    cv2.imshow(f'video frame:{mp4frameindex}{extname}',cv2img)
def mp4gameframeshow():
    videodir = sys.argv[1]
    videofile = os.path.join(videodir,'mod.mp4')
    framecodefile = os.path.join(videodir,'framecode.txt')

    tarframeindex = int(sys.argv[2]) # 5204 -> 5204 - 5110 + 1 = 95 加1是因为游戏是从第1帧开始的不是第0帧,计算按0算，显示则加1
    framecodes = com.readall(framecodefile).splitlines()
    frameindexs = [int(x.split(' ')[1]) for x in framecodes[1::]]
    seekindex = 1
    shiftindex = frameindexs[0]
    isfinded = False
    isbetween = False
    videoprevindex = 1
    videonextindex = 1
    gameprevindex = 0
    gamenextindex = 0
    for i,frameindex in enumerate(frameindexs):
        if frameindex == tarframeindex:
            seekindex = tarframeindex - shiftindex
            isfinded = True
            break
        if 0 < i:
            if frameindexs[i - 1] < tarframeindex < frameindexs[i]:
                videoprevindex = i - 1 
                videonextindex = i 
                gameprevindex = frameindexs[i - 1] - shiftindex
                gamenextindex = frameindexs[i] - shiftindex
                isbetween = True
        
    def mp4gameframeshow(modmp4file,mp4frameindex,extname=''):
        # 加1是因为第一帧(index:0)是第二帧(index:1)置灰生成的，所以真正的游戏第一帧是视频第二帧(index:1)
        mp4frameshow(modmp4file,mp4frameindex + 1,extname)

    if isfinded:
        mp4gameframeshow(videofile,seekindex,extname=f',game frame:{tarframeindex}')
    else:
        if isbetween:
            mp4gameframeshow(videofile,videoprevindex,extname=f',game frame:{gameprevindex + shiftindex},before game frame {tarframeindex}')
            mp4gameframeshow(videofile,videonextindex,extname=f',game frame:{gamenextindex + shiftindex},after game frame {tarframeindex}')
        else:
            # 在录制前的帧数
            if tarframeindex < frameindexs[0]:
                mp4gameframeshow(videofile,frameindexs[0] - shiftindex,extname=f'target game frame {tarframeindex}')
            # 在录制后的帧数
            if tarframeindex > frameindexs[-1]:
                mp4gameframeshow(videofile,frameindexs[-1] - shiftindex,extname=f'target game frame {tarframeindex}')

    cv2.waitKey(0)

if __name__ == "__main__":
    if sys.argv.__len__() == 1:
        jf = JsonFile(os.path.join(workdir,'devices_setting.json'))
        port = 44444
        runningrecord = []
        for devicename,info in jf.getkv():
            serialno = info['设备序列号']
            desirepower = int(info['录像分辨率缩放倍数'])
            gr = GameRecord(devicename,serialno,desirepower,port)
            thds = ThreadManager.go(lambda : gr.start(),count=1)
            runningrecord += thds
            port += 1
        ThreadManager.waitall(runningrecord)
    elif sys.argv.__len__() == 3:
        mp4gameframeshow()
