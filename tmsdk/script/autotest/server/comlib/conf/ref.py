# -*- encoding: utf-8 -*-
class globalconf():
    def __init__(self,game,debug,ding,dingserver,engine):
        self.game = game
        self.debug = debug
        self.ding = ding
        self.dingserver = dingserver
        self.engine = engine
class ftpconf():
    def __init__(self,host,port,httpport,httproot,rootdir,packagesave,tempsave,hotfixsave,devopssave):
        self.host = host
        self.port = port
        self.httpport = httpport
        self.httproot = httproot
        self.rootdir = rootdir
        self.packagesave = packagesave
        self.tempsave = tempsave
        self.hotfixsave = hotfixsave
        self.devopssave = devopssave
class baiduacc():
    def __init__(self,baidu_bduss,baidu_ptoken,baidu_stoken):
        self.baidu_bduss = baidu_bduss
        self.baidu_ptoken = baidu_ptoken
        self.baidu_stoken = baidu_stoken
class android_keystoreconf():
    def __init__(self,storepass,keypass,alias):
        self.storepass = storepass
        self.keypass = keypass
        self.alias = alias
class bangconf():
    def __init__(self,usebackground,ip,port,username,password,reinforce_type,appkey,app_password):
        self.usebackground = usebackground
        self.ip = ip
        self.port = port
        self.username = username
        self.password = password
        self.reinforce_type = reinforce_type
        self.appkey = appkey
        self.app_password = app_password
class projectconf():
    def __init__(self,programpath,projectpath,reporoot):
        self.programpath = programpath
        self.projectpath = projectpath
        self.reporoot = reporoot
class buglycrawlerconf():
    def __init__(self,sdk):
        self.sdk = sdk
class scmpath_devops():
    def __init__(self,keystore):
        self.keystore = keystore
class uwaprojscanconf():
    def __init__(self,name,user,password,project):
        self.name = name
        self.user = user
        self.password = password
        self.project = project
class channelconf():
    def __init__(self,usesdk,minsdkversion,targetsdkversion,applicationid,applicationname,keystorename):
        self.usesdk = usesdk
        self.minsdkversion = minsdkversion
        self.targetsdkversion = targetsdkversion
        self.applicationid = applicationid
        self.applicationname = applicationname
        self.keystorename = keystorename
class ioschannelconf():
    def __init__(self,usesdk,applicationid,applicationname,keystorename):
        self.usesdk = usesdk
        self.applicationid = applicationid
        self.applicationname = applicationname
        self.keystorename = keystorename
class compileconf():
    def __init__(self,msbuildpath,editor_dotnet_marco,release_dotnet_marco,standalone_win_normalmarco,standalone_mac_normalmarco,android_normalmarco,ios_normalmarco,vs_normalmarco):
        self.msbuildpath = msbuildpath
        self.editor_dotnet_marco = editor_dotnet_marco
        self.release_dotnet_marco = release_dotnet_marco
        self.standalone_win_normalmarco = standalone_win_normalmarco
        self.standalone_mac_normalmarco = standalone_mac_normalmarco
        self.android_normalmarco = android_normalmarco
        self.ios_normalmarco = ios_normalmarco
        self.vs_normalmarco = vs_normalmarco
class dingrobotsendconf():
    def __init__(self,客户端维护,优化群,发包对接群,出包群,测试群,脚本调试群):
        self.客户端维护 = 客户端维护
        self.优化群 = 优化群
        self.发包对接群 = 发包对接群
        self.出包群 = 出包群
        self.测试群 = 测试群
        self.脚本调试群 = 脚本调试群
class envconf():
    def __init__(self,user,password,androidsdkpath,androidjdkpath,ndkpath,enginepath):
        self.user = user
        self.password = password
        self.androidsdkpath = androidsdkpath
        self.androidjdkpath = androidjdkpath
        self.ndkpath = ndkpath
        self.enginepath = enginepath
class dingrobotconf():
    def __init__(self,webhook,key):
        self.webhook = webhook
        self.key = key
class memberconf():
    def __init__(self,svnname,phone):
        self.svnname = svnname
        self.phone = phone
class engineconf():
    def __init__(self,branchName,binRepoUrl,srcRepoUrl):
        self.branchName = branchName
        self.binRepoUrl = binRepoUrl
        self.srcRepoUrl = srcRepoUrl