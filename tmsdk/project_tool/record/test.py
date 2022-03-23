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



cmd = r'D:\_workspace\_project\_sdk\DevOps_Scripts\pyscripts\recordproj\prepack\bin\ffmpeg.exe -y -i D:\_workspace\_project\_sdk\DevOps_Scripts\pyscripts\recordproj\prepack\record_2021-0121-173747\out.mp4 -vf "select=eq(n\,0)" -vframes 1 -f image2 -'


data,err = com.cmd(cmd,getstdout=True,decoderet=False)
# print(data)
# com.savedata(data,'t.jpg','wb')
imga = numpy.asarray(bytearray(data),dtype='uint8')
cv2img = cv2.imdecode(imga,cv2.IMREAD_COLOR)
# imgpath = r'D:\_workspace\_project\_sdk\DevOps_Scripts\pyscripts\recordproj\prepack\record_2021-0121-173747\out.jpg'
# cv2img = cv2.imread(imgpath)
cv2.imshow('img',cv2img)
cv2.imshow('img2',cv2img)
cv2.waitKey(0)