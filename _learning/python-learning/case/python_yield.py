#!/usr/bin/env python
# -*- encoding: utf-8 -*-
'''
@File           : python_yield.py
@Time           : 2020/10/13 11:22:26
@Author         : tm
@Version        : 1.0.0
@Desc           : Python协程学习
'''

def consumer():
    r = ''
    while True:
        n = yield r
        if not n:
            return 
        print('[Consumer] Consuming %s ...' % n)
        r = '200 OK'

def producer(c):
    c.send(None)  #启动生成器
    n = 0
    while n < 5:
        n = n + 1
        print('[Producer] Producing %s ... ' % n)
        r = c.send(n) # 切换到生成器 consumer执行
        print('[Producer] Consumer return %s ... ' % r)
    c.close()

if __name__ == "__main__":
    c = consumer()
    producer(c)