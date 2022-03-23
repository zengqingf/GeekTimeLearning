# -*- encoding: utf-8 -*-
from logging import exception
from os import stat
import sys,os
from sys import path
thisdir = os.path.abspath(os.path.dirname(__file__))
workdir = os.path.abspath(os.getcwd())
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com,workspace,XMLFile
from comlib.conf.loader import Loader
from comlib.conf.ref import *




import argparse,multiprocessing

import subprocess,inspect
from xml.dom import minidom
import copy

# fp = r'D:\_WorkSpace\_sdk2\DevOps_Scripts\pyscripts\autotest\flow.drawio'
fp = os.path.join(thisdir,'flow_new.drawio')

class DirectedMapNode():
    '''
    图的节点
    '''
    def __init__(self,tag):
        self.inputs = []
        self.outputs = []
        self.tag = tag
    def link(self,to,edge):
        edge.source = self
        edge.target = to
        self.outputs.append(edge)
        to.inputs.append(edge)
        return edge
class DirectedEdge():
    '''
    图的有向边
    '''
    def __init__(self,source:DirectedMapNode,target:DirectedMapNode,spend=-1):
        self.source = source
        self.target = target
        self.spend = spend
    def __hash__(self):
        s = f'{self.source.tag}->{self.target.tag}'
        hs = abs(hash(s)) % (10 ** 8)
        return hs
class TestEdge(DirectedEdge):
    def __init__(self, source, target,testname, spend=-1):
        super().__init__(source, target, spend=spend)
        self.name = testname
    
class TestState(DirectedMapNode):
    def __init__(self,drawioid,statename):
        super().__init__(statename)
        self.id = drawioid

        self.testlists = []
    def getTestList(self,level):
        '''
        [TestCase]
        '''
        for testlist in self.testlists:
            testlist:TestList
            if testlist.level == level:
                return testlist.tests
        return []
class TestList():
    def __init__(self,tests:list,level):
        self.tests = tests
        self.level = level
class TestPath():
    def __init__(self,paths):
        self.paths = paths
        self.devicesCurTest = {}
        self.unpackpaths = []
    def isFinish(self,flag):
        index = self.devicesCurTest[flag]
        if index == self.unpackpaths.__len__():
            return True
        return False
    def registe(self,flag):
        root = self.unpackpaths[0]
        self.devicesCurTest[flag] = 0
        return root
    def getNext(self,flag):
        index = self.devicesCurTest[flag]
        index += 1
        self.devicesCurTest[flag] = index
        if index < self.unpackpaths.__len__():
            return self.unpackpaths[index]
        return None,None
    
    def unpackByLevel(self,level):
        unpackpaths = []
        for path in self.paths:
            target:TestState = path.target
            unpackpaths.append(('_流程图',path.name))
            for testcase in target.getTestList(level):
                testcase:TestCase
                if (target.tag,testcase.name) not in unpackpaths:
                    unpackpaths.append((target.tag,testcase.name))
        self.unpackpaths = unpackpaths
    def unpack(self,targetTest=None):
        unpackpaths = []
        for path in self.paths:
            target:TestState = path.target
            unpackpaths.append(('_流程图',path.name))
        if targetTest != None:
            unpackpaths.append((self.paths[-1].target.tag,targetTest))
        self.unpackpaths = unpackpaths
        pass
class TestMap(object):
    def __init__(self):
        self.states = {}
        self.edges = []
        self.rootid = '2'
    
    def addstate(self,id,testlists,statename):
        if id in self.states:
            self.states[id].tag = statename
            self.states[id].id = id
            self.states[id].testlists = testlists
        else:
            self.states[id] = TestState(id,statename)
    def tryaddstate(self,id,statename):
        if id in self.states:
            pass
        else:
            self.states[id] = TestState(id,statename)
    def addedge(self,sourceid,targetid,testname,spend):
        if sourceid not in self.states:
            self.states[sourceid] = TestState(-1,'tmp')
        if targetid not in self.states:
            self.states[targetid] = TestState(-1,'tmp')
        frm = self.states[sourceid]
        tar = self.states[targetid]
        edge = frm.link(tar,TestEdge(frm,tar,testname,spend))
        # edge = self.states[sourceid].link(self.states[targetid],spend)
        self.edges.append(edge)
    def getPath(self,sourcestate:TestState,targetstatename):
        bestpath = []
        bestspend = sys.maxsize

        unable = set()
        
        def dfs(curpath:list,curindex,curspend,curstate:TestState):
            if curstate.tag == targetstatename:
                nonlocal bestspend,bestpath
                if bestspend > curspend:
                    bestpath =  curpath[:curindex]
                    bestspend = curspend
            else:
                for outputedge in curstate.outputs:
                    target = outputedge.target
                    if outputedge not in unable:
                        unable.add(outputedge)
                        if curindex < curpath.__len__():
                            curpath[curindex] = outputedge
                        else:
                            curpath.append(outputedge)
                        curindex += 1
                        spend = outputedge.spend
                        curspend += spend
                        dfs(curpath,curindex,curspend,target)
                        curindex -= 1
                        unable.remove(outputedge)
        dfs(bestpath,0,0,sourcestate)
        return bestpath,bestspend
    def getRoot(self):
        return self.states[self.rootid]
    def getAllTestPath(self,level):
        bestpath = []
        bestspend = sys.maxsize
        
        unableedge = copy.copy(self.edges)

        def inner(curpath:list,curspend,curstate:TestState):
            if unableedge.__len__() == 0:
                nonlocal bestspend,bestpath
                if bestspend > curspend:
                    bestpath =  copy.copy(curpath)
                    bestspend = curspend
            else:
                isfind = False
                for outputedge in curstate.outputs:
                    target = outputedge.target
                    if outputedge in unableedge:
                        unableedge.remove(outputedge)
                        spend = outputedge.spend
                        curspend += spend
                        curpath.append(outputedge)
                        isfind = True
                        inner(curpath,curspend,target)
                        curpath.pop()
                if not isfind:
                    edge = unableedge.pop()
                    paths,spend = self.getPath(curstate,edge.source.tag)
                    curspend += spend
                    target = edge.target
                    paths.append(edge)
                    curpath += paths
                    inner(curpath,curspend,target)
                    curpath = curpath[:curpath.__len__() - paths.__len__()]

            
        inner(bestpath,0,self.getRoot())
        # bestpath.pop(0)
        tp = TestPath(bestpath)
        tp.unpackByLevel(level)
        return tp
        
    def getTestPath(self,curstate:TestState,targetstatename,targettestname=None) -> TestPath:
        paths,spend = self.getPath(curstate,targetstatename)

        tp = TestPath(paths)
        tp.unpack(targettestname)
        return tp
    def getTestState(self,statename):
        for id,state in self.states.items():
            state:TestState
            if state.tag == statename:
                return state
        raise Exception(f'{statename}找不到')
# def getid(cell):
#     return cell.getAttribute('id')
# def getparent(cell):
#     return cell.getAttribute('parent')
# def getstyle(cell):
#     return cell.getAttribute('style')
# def getValue(cell):
#     return cell.getAttribute('value')
# def getSource(cell):
#     return cell.getAttribute('source')
# def getTarget(cell):
#     return cell.getAttribute('target')
# def isEdge(cell):
#     if cell.getAttribute('edge') == '1':
#         return True
#     return False
def getid(cell):
    return cell.get('id')
def getparent(cell):
    return cell.get('parent')
def getstyle(cell):
    return cell.get('style')
def getValue(cell):
    return cell.get('value')
def getSource(cell):
    return cell.get('source')
def getTarget(cell):
    return cell.get('target')
def isEdge(cell):
    if cell.get('edge','0') == '1':
        return True
    return False
def isEdgeLabel(cell):
    style = getstyle(cell)
    if style == None or 'edgeLabel' not in style:
        return False
    return True
def isLayer(cell):
    if getparent(cell) == '0':
        return True
    return False
def isTestList(cell):
    style = getstyle(cell)
    if style == None or 'swimlane' not in style:
        return False
    return True
def isText(cell):
    style = getstyle(cell)
    if style == None or 'text' not in style:
        return False
    return True
def isParent(parentcell,child):
    if getparent(child) == getid(parentcell):
        return True
    return False

def loadedge(cells,point):
    rootcell = cells[point]
    point += 1
    edgeobj = drawioedge(rootcell)
    if point >= cells.__len__():
        return edgeobj,point
    nextcell = cells[point]
    if isEdgeLabel(nextcell) and isParent(rootcell,nextcell):
        point += 1
        edgeobj.setdesc(nextcell)
    
    return edgeobj,point
def loadstate(cells,point):
    state = drawiostate(cells[point])
    point += 1
    return state,point
def loadtestlist(cells,point):
    rootcell = cells[point]
    point += 1
    testlistobj = drawiotestlist(rootcell)
    if point >= cells.__len__():
        return testlistobj,point
    nextcell = cells[point]
    
    while isText(nextcell) and isParent(rootcell,nextcell):
        testlistobj.addtest(nextcell)
        point += 1
        if point >= cells.__len__():
            break
        nextcell = cells[point]

    return testlistobj,point
def loadcellobject(cells,point):
    if point >= cells.__len__():
        return None,point,True
    rootcell = cells[point]
    if isLayer(rootcell):
        return None,point,True
    if isEdge(rootcell):
        rootobj,point = loadedge(cells,point)
    elif isTestList(rootcell):
        rootobj,point = loadtestlist(cells,point)
    # 状态
    else:
        rootobj,point = loadstate(cells,point)

    return rootobj,point,False

class drawioroot():
    def __init__(self,cells):
        self.root = drawiolayerroot(cells[0])
        point = 1
        self.layers = self._loadlayers(cells,point)
        
        self.maplayer:drawiolayer = self.layers[0]
        self.midlayer:drawiolayer = self.layers[1]
        self.highlayer:drawiolayer = self.layers[2]
        self.toplayer:drawiolayer = self.layers[3]
        self.singlelayer:drawiolayer = self.layers[4]
    def _loadlayers(self,cells:list,point):
        layers = []
        def loadlayer():
            nonlocal point
            layer = drawiolayer(cells[point])
            point += 1
            
            if point >= cells.__len__():
                return layer,point
            
            cellobj,point,isend = loadcellobject(cells,point)
            while not isend:
                layer.subcells[cellobj.id] = cellobj
                cellobj,point,isend = loadcellobject(cells,point)
            return layer,point
        layer,point = loadlayer()
        layers.append(layer)
        while point < cells.__len__():
            layer,point = loadlayer()
            layers.append(layer)
        return layers

class drawiocellbase():
    def __init__(self,cell,tp):
        self.id = getid(cell)
        self.parent = getparent(cell)
        self.tp = tp

class drawiolayerroot(drawiocellbase):
    def __init__(self, cell):
        super().__init__(cell,'layerroot')
class drawiolayer(drawiocellbase):
    def __init__(self,cell):
        super().__init__(cell,'layer')
        self.subcells = {}

class drawioedge(drawiocellbase):
    def __init__(self,cell,desccell=None):
        super().__init__(cell,'edge')
        self.source = getSource(cell)
        self.target = getTarget(cell)
        self.desc = None
        self.spend = 100
        if desccell != None:
            self.setdesc(desccell)
    def setdesc(self,desccell):
        val = getValue(desccell)
        tmp = val.split('|')
        self.desc = tmp[0]
        if tmp.__len__() == 2:
            self.spend = int(tmp[1])
        
class drawiostate(drawiocellbase):
    def __init__(self,cell):
        super().__init__(cell,'state')
        self.name = getValue(cell)

class TestCase():
    def __init__(self,name):
        self.name = name
class drawiotestlist(drawiocellbase):
    def __init__(self, cell):
        super().__init__(cell,'testlist')
        self.testlist = []
    def addtest(self,cell):
        self.testlist.append(TestCase(getValue(cell)))
def gettestmap()->TestMap:
    # from xml.dom.minidom import parse
    # import xml.dom.minidom

    # document = xml.dom.minidom.parse(fp)
    # collection = document.documentElement
    # diagram = collection.getElementsByTagName('diagram')[0]
    # mxGraphModel = diagram.getElementsByTagName('mxGraphModel')[0]
    # root = mxGraphModel.getElementsByTagName('root')[0]
    # mxCells = root.getElementsByTagName('mxCell')

    xmlf = XMLFile(fp)
    tmp = xmlf.root
    diagram = xmlf.find(tmp,'diagram')
    mxGraphModel = xmlf.find(diagram,'mxGraphModel')
    root = xmlf.find(mxGraphModel,'root')

    mxCells = xmlf.findall(root,'mxCell')
    cc = list(mxCells)
    dr = drawioroot(cc)
    tmap = TestMap()
    # 处理流程图
    # 链表字典混合更快
    for key,cell in dr.maplayer.subcells.items():
        if cell.tp == 'edge':
            sourceobj = dr.maplayer.subcells[cell.source]
            tmap.tryaddstate(sourceobj.id,sourceobj.name)
            targetobj = dr.maplayer.subcells[cell.target]
            tmap.tryaddstate(targetobj.id,targetobj.name)
            tmap.addedge(cell.source,cell.target,cell.desc,cell.spend)
    for key,cell in dr.singlelayer.subcells.items():
        if cell.tp == 'edge':
            sourceobj:TestState = tmap.states[cell.source]
            targetobj = dr.singlelayer.subcells[cell.target]
            sourceobj.testlists.append(TestList(targetobj.testlist,'single'))
    for key,cell in dr.toplayer.subcells.items():
        if cell.tp == 'edge':
            sourceobj:TestState = tmap.states[cell.source]
            targetobj = dr.toplayer.subcells[cell.target]
            sourceobj.testlists.append(TestList(targetobj.testlist,'top'))
    for key,cell in dr.highlayer.subcells.items():
        if cell.tp == 'edge':
            sourceobj:TestState = tmap.states[cell.source]
            targetobj = dr.highlayer.subcells[cell.target]
            sourceobj.testlists.append(TestList(targetobj.testlist,'high'))
    for key,cell in dr.midlayer.subcells.items():
        if cell.tp == 'edge':
            sourceobj:TestState = tmap.states[cell.source]
            targetobj = dr.midlayer.subcells[cell.target]
            sourceobj.testlists.append(TestList(targetobj.testlist,'mid'))
    return tmap
    # tmap.getPath('传奇之路')
# tmap = gettestmap()
# tmap.getPath(tmap.getRoot(),'角色选择')
# allpath = tmap.getTestAllPath()
