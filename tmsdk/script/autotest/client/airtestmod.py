# -*- encoding: utf-8 -*-


import sys,os,textwrap



def replacedata(filepath,old,new):
    with open(filepath,'r+',encoding='utf-8') as fs:
        newdata = fs.read()
        if old in newdata:
            print(f'ok  {filepath}')
        else:
            print(f'no  {filepath}')

        newdata = newdata.replace(old,new)
        fs.seek(0)
        fs.truncate()
        fs.write(newdata)
def javacapmod(path):
    oldcode = '''
        proc, nbsp, localport = self._setup_stream_server()
        s = SafeSocket()
        s.connect((self.adb.host, localport))
        t = s.recv(24)
    '''
    newcode = r'''
        from time import sleep
        count = 0
        while count < 3:
            proc, nbsp, localport = self._setup_stream_server()
            s = SafeSocket()
            s.connect((self.adb.host, localport))
            try:
                t = s.recv(24)
                break
            except OSError as e:
                s.close()
                nbsp.kill()
                proc.kill()
                self.adb.remove_forward("tcp:%s" % localport)
                count += 1
                self.adb.shell('am force-stop com.netease.nie.yosemite')
                sleep(3)
                print('[retry]*******************************************************')

    '''

    replacedata(path,oldcode,newcode)

def airtestparsermod(path):
    old = textwrap.dedent('''
    def runner_parser(ap=None):
        if not ap:
            ap = argparse.ArgumentParser()
        ap.add_argument("script", help="air path")
        ap.add_argument("--device", help="connect dev by uri string, e.g. Android:///", nargs="?", action="append")
        ap.add_argument("--log", help="set log dir, default to be script dir", nargs="?", const=True)
        ap.add_argument("--compress", required=False, type=int, choices=range(1, 100), help="set snapshot quality, 1-99", default=10)
        ap.add_argument("--recording", help="record screen when running", nargs="?", const=True)
        ap.add_argument("--no-image", help="Do not save screenshots", nargs="?", const=True)
        return ap
    ''')
    new = textwrap.dedent('''
    def runner_parser(ap=None):
        if not ap:
            ap = argparse.ArgumentParser()
        ap.add_argument("script", help="air path")
        ap.add_argument("--device", help="connect dev by uri string, e.g. Android:///", nargs="?", action="append")
        ap.add_argument("--log", help="set log dir, default to be script dir", nargs="?", const=True)
        ap.add_argument("--compress", required=False, type=int, choices=range(1, 100), help="set snapshot quality, 1-99", default=10)
        ap.add_argument("--recording", help="record screen when running", nargs="?", const=True)
        ap.add_argument("--no-image", help="Do not save screenshots", nargs="?", const=True)
        ap.add_argument("--paramfile", help="air脚本内可用参数",required=False)
        return ap
    ''')

    replacedata(path,old,new)

def airtestrunnermod(path):
    old = '''
    def setUp(self):
        if self.args.log and self.args.recording:
            for dev in G.DEVICE_LIST:
                try:
                    dev.start_recording()
                except:
                    traceback.print_exc()'''
    new = '''
    def setUp(self):
        self.err = None
        if self.args.log and self.args.recording:
            for dev in G.DEVICE_LIST:
                try:
                    dev.start_recording()
                except:
                    traceback.print_exc()'''
    replacedata(path,old,new)
    old = '''
        try:
            exec(compile(code.encode("utf-8"), pyfilepath, 'exec'), self.scope)
        except Exception as err:
            log(err, desc="Final Error", snapshot=True)
            six.reraise(*sys.exc_info())'''
    new = '''
        try:
            exec(compile(code.encode("utf-8"), pyfilepath, 'exec'), self.scope)
        except BaseException as err:
            self.err = err
            log(err, desc="Final Error", snapshot=True)
            six.reraise(*sys.exc_info())'''
    replacedata(path,old,new)
    old = '''
    if not result.wasSuccessful():
        sys.exit(-1)'''
    new = '''
    if not result.wasSuccessful():
        for obj,errstr in result.errors:
            if isinstance(obj.err,SystemExit):
                sys.exit(obj.err.code)
        sys.exit(-1)'''
    replacedata(path,old,new)


def airtestconstantmod(path):
    old = 'THISPATH = decode_path(os.path.dirname(os.path.realpath(__file__)))'
    new = textwrap.dedent('''
    # pyinstaller单文件打包
    if hasattr(sys,'_MEIPASS'):
        execdir = os.path.dirname(os.path.realpath(sys.executable))
        filepath = __file__.replace(sys._MEIPASS,execdir)
        THISPATH = decode_path(os.path.dirname(os.path.realpath(filepath)))
    else:
        THISPATH  = decode_path(os.path.dirname(os.path.realpath(__file__)))
    ''')
    replacedata(path,old,new)
def airtestreportmod(path):
    old = '''
        def ignore_export_dir(dirname, filenames):
            # 忽略当前导出的目录，防止递归导出
            if os.path.commonprefix([dirpath, dirname]) == dirpath:
                return filenames
            return []'''
    new = '''
        def ignore_export_dir(dirname, filenames):
            base = ['.svn']
            # 忽略当前导出的目录，防止递归导出
            if os.path.commonprefix([dirpath, dirname]) == dirpath:
                return filenames
            return base'''
    replacedata(path,old,new)


def pocoinputmod(path):
    replacedata(path,'self.use_render_resolution = False','self.use_render_resolution = True')

if __name__ == "__main__":
    libpath = sys.argv[1]
    # libpath = r'C:\Users\tengmu\AppData\Local\Programs\Python\Python37-32\Lib\site-packages'
    airtestpath = os.path.join(libpath,'airtest')
    pocopath = os.path.join(libpath,'poco')
    if os.path.exists(airtestpath):
        javacapmod(os.path.join(airtestpath,'core','android','javacap.py'))
        airtestparsermod(os.path.join(airtestpath,'cli','parser.py'))
        airtestrunnermod(os.path.join(airtestpath,'cli','runner.py'))
        airtestconstantmod(os.path.join(airtestpath,'core','android','constant.py'))
        airtestreportmod(os.path.join(airtestpath,'report','report.py'))

    if os.path.exists(pocopath):
        pocoinputmod(os.path.join(pocopath,'utils','airtest','input.py'))
