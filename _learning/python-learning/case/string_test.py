#!/usr/bin/env python
# -*- encoding: utf-8 -*-
'''
@File           : string_test.py
@Time           : 2020/12/18 12:42:04
@Author         : mjx
@Version        : 1.0.0
@Desc           : 
'''


def str_join_test_1():
    s = ''
    for n in range(0, 100000):
        s += str(n)

def str_join_test_2():
    l = []
    for n in range(0, 100000):
        l.append(str(n))
    s = ' '.join(l)


# split(',') === split(",")
def str_split_test(macros):
    macroDesc=""
    macroArray = macros.split(",")
    if len(macroArray) <= 0:
        print(f'### 输入宏为空或格式不对')
        return
    index = 0
    for m in macroArray:
        print(m)
        macroArray[index] = "\"%s\"" % m
        index+=1
    macroDesc = ','.join(macroArray)
    print(f'### 预设宏为：{macroDesc}')

if __name__ == "__main__":
    str_split_test("TMSDK_LOG,TMSDK_LOGERROR")
    pass