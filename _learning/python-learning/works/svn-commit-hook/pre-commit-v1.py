# coding=utf-8

import sys
import re
import sys

if __name__ == "__main__":
    reload(sys)
    print sys.setdefaultencoding('utf-8')


    arglen = len(sys.argv)

    #print 'start re %s' % arglen

    if 3 > arglen:
        exit(1)

    respinfo = sys.argv[1]
    logmsg = sys.argv[2]

    #print respinfo
    print "logmssg:"+logmsg+"*end"
    

    r = re.search('^(fix|bug|Bug|BUG|新功能|需求修改|版本制作|代码整理|辅助工具|解决编译不过|阶段性递交|追加递交)(（|\()(.+)/{0,1}(.+)(\)|）)(:|：)(.+)'.encode("gbk"), logmsg, re.M | re.I)

    if r:
        #msg='提交类型：%s' % r.group(0)
        #print msg.encode("gb2312")

        #msg='提交系统：%s/%s' % (r.group(3) , r.group(4))
        #print msg.encode("gb2312")
        
        exit(0)
    else:
        msg = '''格式粗错!!!!!!!!!!!

详情请访问：
https://www.notion.so/etond/1975699ff69845a6bc074b8d220f02a9

例子：
新功能(关卡/深渊): 深渊入口界面添加快速进入和提示

1. 添加深渊票不足的提示
2. 添加深渊票的快速购买
3. 界面上的剩余深渊票的数量显示

提交类型：
bug|Bug|BUG|新功能|需求修改|版本制作|代码整理|解决编译不过|阶段性递交|追加递交

交类型只能选择上面列出来的一个，不含|
'''
        print msg.encode("gb2312")
        exit(1)
