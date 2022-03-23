# coding: utf-8
from sqlalchemy import ARRAY, BigInteger, Boolean, Column, DateTime, Integer, JSON, Text, text
from sqlalchemy.ext.declarative import declarative_base


from sqlalchemy.schema import CreateTable

Base = declarative_base()
metadata = Base.metadata


class ChannelModel(object):
    def __init__(self,CrashInfocls,IssueListcls,CrawLogcls,YestodaySummarycls):
        super().__init__()
        self.CrashInfocls = CrashInfocls
        self.IssueListcls = IssueListcls
        self.CrawLogcls = CrawLogcls
        self.YestodaySummarycls = YestodaySummarycls

class CrashInfo(Base):
    # __tablename__ = 'CrashInfo'
    __abstract__ = True

    crashHash = Column(Text, primary_key=True)
    apn = Column(Text)
    appInBack = Column(Boolean)
    brand = Column(Text)
    buildNumber = Column(Text)
    bundleId = Column(Text)
    country = Column(Text)
    cpuName = Column(Text)
    cpuType = Column(Text)
    deviceId = Column(Text)
    diskSize = Column(Text)
    expAddr = Column(Text)
    expName = Column(Text)
    freeStorage = Column(BigInteger)
    freeSdCard = Column(Text)
    freeMem = Column(Text)
    isRooted = Column(Boolean)
    memSize = Column(Text)
    model = Column(Text)
    modelOriginalName = Column(Text)
    osVer = Column(Text)
    processName = Column(Text)
    productVersion = Column(Text)
    rom = Column(Text)
    sdkVersion = Column(Text)
    sendProcess = Column(Text)
    sendType = Column(Text)
    totalSD = Column(Text)
    threadName = Column(Text)
    userId = Column(Text)
    locate = Column(Text)
    playerName = Column(Text)
    isSimulater = Column(Boolean)
    simulater = Column(Text)
    issueId = Column(Integer)
    callStack = Column(Text)
    retraceCrashDetail = Column(Text)
    detailMap = Column(JSON)
    crashTime = Column(DateTime)
    startTime = Column(DateTime)
    playerLevel = Column(Integer)
    locateId = Column(Text)
    session = Column(Text)
    branch = Column(Text)
    channel = Column(Text)
    appid = Column(Text)
    platformId = Column(Text)
    isDumped = Column(Boolean)
    crashId = Column(BigInteger)


class IssueList(Base):
    # __tablename__ = 'IssueList'
    __abstract__ = True

    id = Column(Integer, primary_key=True)
    count = Column(Integer, nullable=False, server_default=text("0"))
    exceptionName = Column(Text, nullable=False)
    imeiCount = Column(Integer, nullable=False, server_default=text("0"))
    keyStack = Column(Text)
    status = Column(Text, nullable=False)
    lastestUploadTime = Column(DateTime)
    createTime = Column(DateTime)
    branch = Column(Text)
    crashHashs = Column(ARRAY(Text()))
    crashHashCount = Column(Integer, nullable=False, server_default=text("0"))
    versions = Column(ARRAY(Text()), nullable=False, server_default=text("ARRAY[]::text[]"))
    
    
class CrawLog(Base):
    # __tablename__ = 'CrawLog'
    __abstract__ = True
    id = Column(Integer, primary_key=True)
    time = Column(DateTime, nullable=False)
    version = Column(Text, nullable=False)
    issueIds = Column(ARRAY(Integer()), nullable=False, server_default=text("ARRAY[]::integer[]"))

class YestodaySummary(Base):
    # __tablename__ = 'YestodaySummary'
    __abstract__ = True
    id = Column(Integer, primary_key=True)
    updatetime = Column(DateTime, nullable=False)
    issueIds = Column(ARRAY(Integer()), nullable=False, server_default=text("ARRAY[]::integer[]"))
