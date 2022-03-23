# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com
from comlib.dictm import DictUtil


from xml.etree.ElementTree import ElementTree,Element
import xml.etree.ElementTree as ET
import re
import io

class XMLFile(object):
    def __init__(self,file):
        super().__init__()
        self.filepath = None

        if com.ispath(file):
            self.filepath = file
            self.tree = ElementTree(file=file)
        else:
            filept = io.StringIO(file)      
            self.tree = ElementTree(file=filept)
        # ET.register_namespace('','http://schemas.microsoft.com/developer/msbuild/2003')

        
        self.root = self.tree.getroot()
        self.namespace = ''
        if self.root.tag[0] == '{':
            self.namespace = re.match(r'(\{.*?\}).*?',self.root.tag).group(1)
        
        ET.register_namespace('',self.namespace.strip('{}'))
    def add(self,parent:Element,sube_tag,sube_text='', sube_attrib={}):
        sube = Element(sube_tag,sube_attrib)
        sube.text = sube_text
        parent.append(sube)
        return sube
    def find(self,e:Element,tag,**attrdict)->Element:
        subes = self.findall(e,tag,**attrdict)
        val = None
        try:
            val = next(subes)
        except StopIteration as e:
            pass
        return val
    def findpath(self,e:Element,tagpath:str)->Element:
        paths = tagpath.split('->')
        for path in paths:
            tmp = path.split('|')
            path = tmp[0]
            attrs = tmp[1].split(',')
            attrdict = {}
            for attr in attrs:
                tmp = attr.split('=')
                k = tmp[0]
                v = tmp[1]
                attrdict[k] = v
            e = self.find(e,path,**attrdict)
            if e == None:
                break
        return e
    def findall(self,e:Element,tag,**attrdict):
        subes = e.findall(self.namespace + tag)
        for sube in subes:
            if attrdict == {}:
                yield sube
            elif DictUtil.isSubDict(sube.attrib,attrdict):
                yield sube
    def remove(self,e:Element,sube_tag,**attrdict):
        subes = self.findall(e,sube_tag,**attrdict)
        for sube in subes:
            e.remove(sube)
                
    def save(self,path=None,encoding='utf-8'):
        savepath = self.filepath
        if path != None:
            savepath = path
        self.tree.write(savepath,encoding=encoding)

