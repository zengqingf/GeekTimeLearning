# -*- encoding: utf-8 -*-
import sys,os
thisdir = os.path.abspath(os.path.dirname(__file__))
sys.path.append(os.path.abspath(os.path.join(thisdir,'..')))
from comlib.exception import errorcatch,DingException,StopException,LOW,NORMAL,HIGH
from comlib import com

import psycopg2













# conn = psycopg2.connect(database='weblearn',user='postgres',password='123456',host='localhost',port='44444')

# cur = conn.cursor()
class PGSQLManager(object):
    def __init__(self,database,user,password,host,port='5432'):
        super().__init__()
        self.connect(database=database,user=user,password=password,host=host,port=port)

    def connect(self,database,user,password,host,port='5432'):
        self.conn = psycopg2.connect(database=database,user=user,password=password,host=host,port=port)
        self.cursor = self.conn.cursor()
    # def getcursor(self):
    #     return self.conn.cursor()
    def isTableExists(self,name,schema='public',table_type='BASE TABLE'):
        self.cursor.execute(f'''SELECT count(*) FROM information_schema.tables WHERE table_schema='{schema}' and table_type='{table_type}' and table_name='{name}';''')
        rows = self.cursor.fetchall()
        if rows == [(0,)]:
            return False
        return True
    def close(self):
        self.conn.commit()
        self.conn.close()

sq = PGSQLManager(database='postgres',user='postgres',password='123456',host='192.168.2.147')



if not sq.isTableExists('IssueList'):
    sq.cursor.execute('''CREATE TABLE public."IssueList"
(
    "id" integer NOT NULL,
    count integer NOT NULL DEFAULT 0,
    "exceptionName" text COLLATE pg_catalog."default" NOT NULL,
    "imeiCount" integer NOT NULL DEFAULT 0,
    "keyStack" text COLLATE pg_catalog."default",
    "lastestUploadTime" text COLLATE pg_catalog."default" NOT NULL,
    status text COLLATE pg_catalog."default" NOT NULL,
    "crashIds" integer[],
    CONSTRAINT "IssueList_PK" PRIMARY KEY ("id")
)

TABLESPACE pg_default;

ALTER TABLE public."IssueList"
    OWNER to postgres;

    ''')
    # ("issueId",count,"exceptionName","imeiCount","keyStack","lastestUploadTime",status,"crashIds") 
# sq.conn.commit()
sq.cursor.execute('''INSERT INTO public."IssueList" VALUES (1,2,'expname',123312,'kkkkkey','time','st1',array[2]) RETURNING "id";''')
rows = sq.cursor.fetchall()
print(rows)
sq.cursor.execute('''UPDATE public."IssueList" SET "crashIds"=array_prepend(55,"crashIds") WHERE id = 1;''')
if not sq.isTableExists('CrashInfo'):
    sq.cursor.execute('''CREATE TABLE public."CrashInfo"
(
    id integer NOT NULL,
    "crashHash" text COLLATE pg_catalog."default",
    apn text COLLATE pg_catalog."default",
    "appInBack" boolean,
    brand text COLLATE pg_catalog."default",
    "buildNumber" text COLLATE pg_catalog."default",
    "bundleId" text COLLATE pg_catalog."default",
    country text COLLATE pg_catalog."default",
    "cpuName" text COLLATE pg_catalog."default",
    "cpuType" text COLLATE pg_catalog."default",
    "crashTime" text COLLATE pg_catalog."default",
    "deviceId" text COLLATE pg_catalog."default",
    "diskSize" text COLLATE pg_catalog."default",
    "expAddr" text COLLATE pg_catalog."default",
    "expName" text COLLATE pg_catalog."default",
    "freeStorage" bigint,
    "freeSdCard" text COLLATE pg_catalog."default",
    "freeMem" text COLLATE pg_catalog."default",
    "isRooted" boolean,
    "memSize" text COLLATE pg_catalog."default",
    model text COLLATE pg_catalog."default",
    "modelOriginalName" text COLLATE pg_catalog."default",
    "osVer" text COLLATE pg_catalog."default",
    "processName" text COLLATE pg_catalog."default",
    "productVersion" text COLLATE pg_catalog."default",
    rom text COLLATE pg_catalog."default",
    "sdkVersion" text COLLATE pg_catalog."default",
    "sendProcess" text COLLATE pg_catalog."default",
    "sendType" text COLLATE pg_catalog."default",
    "startTime" text COLLATE pg_catalog."default",
    "totalSD" text COLLATE pg_catalog."default",
    "threadName" text COLLATE pg_catalog."default",
    "userId" text COLLATE pg_catalog."default",
    "position" text COLLATE pg_catalog."default",
    player text COLLATE pg_catalog."default",
    "isSimulater" boolean,
    simulater text COLLATE pg_catalog."default",
    "issueId" integer,
    "callStack" text COLLATE pg_catalog."default",
    "retraceCrashDetail" text COLLATE pg_catalog."default",
    "detailMap" json,
    CONSTRAINT "CrashInfo_PK" PRIMARY KEY (id)
)

TABLESPACE pg_default;

ALTER TABLE public."CrashInfo"
    OWNER to postgres;
    ''')

    # conn.commit()
# conn.close()
sq.close()
print('ok')



