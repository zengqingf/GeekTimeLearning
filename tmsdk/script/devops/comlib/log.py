# -*- encoding: utf-8 -*-
import sys,os
sys.path.append(os.path.abspath(os.path.join(__file__,'..','..')))



from comlib.exception import errorcatch,LOW,NORMAL,HIGH

@errorcatch(HIGH)
class Logging:
    # TODO maybe the right way to do this is to use something like colorama?
    RED     = '\033[31m'
    GREEN   = '\033[32m'
    YELLOW  = '\033[33m'
    MAGENTA = '\033[35m'
    RESET   = '\033[0m'
    
    
    @staticmethod
    def _print(s, color=None):
        try:
            if color and sys.stdout.isatty() and sys.platform != 'win32':
                print(color + s + Logging.RESET)
            else:
                print(s)
            
            sys.stdout.flush()
        except Exception as e:
            print(str(e))

    @staticmethod
    def debug(s):
        Logging._print(s, Logging.MAGENTA)
    
    @staticmethod
    def info(s):
        Logging._print(s, Logging.GREEN)
    
    @staticmethod
    def warning(s):
        Logging._print(s, Logging.YELLOW)
    
    @staticmethod
    def error(s):
        Logging._print(s, Logging.RED)
