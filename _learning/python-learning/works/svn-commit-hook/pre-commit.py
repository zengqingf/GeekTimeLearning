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

    print "msg:"+logmsg+"*"

    isError = False

    while True:
        r = re.search('^(fix|bug|Bug|BUG|新功能|需求修改|版本制作|代码整理|辅助工具|解决编译不过|阶段性递交|追加递交)'.encode("gbk"), logmsg, re.M | re.I)
        if not r:
            print '''`提交类型`应该从下列中任选一个:
BUG
新功能
需求修改
版本制作
代码整理
辅助工具
解决编译不过
阶段性递交
追加递交
'''.encode("gbk")
            isError = True
            break;
        else:
            replacestr = r.group()
            logmsg = logmsg.replace(replacestr, "")
            #print "*", replacestr, "*", logmsg

        r = re.search('^(（|\(|\[)'.encode("gbk"), logmsg, re.M | re.I)
        if not r:
            print '''`左括号`缺失, 应该从下列中任选一个:
(
[
（
'''.encode("gbk")
            isError = True
            break;

        r = re.search('^(（|\(|\[)(.+)/{0,1}(.*)(\)|）|\])'.encode("gbk"), logmsg, re.M | re.I)
        if not r:
            print '''`右括号`缺失, 应该从下列中任选一个:
)
]
）
'''.encode("gbk")
            isError = True
            break;
        else:
            #firsttag=r.group(1)
            #secondtag=r.group(4)
            #if firsttag=='[' and secondtag!=']':
            #    print '''`右括号`应该是]'''.encode("gbk")
            #    isError = True
            #    break

            #if firsttag=='(' and secondtag!=')':
            #    print '''`右括号`应该是), 英文括号呢'''.encode("gbk")
            #    isError = True
            #    break

            #if firsttag=='（' and secondtag!='）':
            #    print '''`右括号`应该是）,中文括号呢'''.encode("gbk")
            #    isError = True
            #    break
            
            replacestr = r.group()
            logmsg = logmsg.replace(replacestr, "")
            #print replacestr, logmsg

        r = re.search('^(:|：)'.encode("gbk"), logmsg, re.M | re.I)
        if not r:
            print '`冒号`缺失'.encode("gbk")
            isError = True
            break;
        else:
            replacestr = r.group()
            logmsg = logmsg.replace(replacestr, "")
            #print replacestr, logmsg

        if len(logmsg) <= 0:
            print '`修改描述` 为空，请添加'.encode("gbk")
            isError = True
            break;

        break

    if isError:
        msg = '''*****************************************************

详情请访问： 
https://www.notion.so/etond/1975699ff69845a6bc074b8d220f02a9

ps: 右键复制

例子：
新功能(关卡/深渊): 深渊入口界面添加快速进入和提示

1. 添加深渊票不足的提示
2. 添加深渊票的快速购买
3. 界面上的剩余深渊票的数量显示
'''
        print msg.encode("gb2312")
        exit(1)
