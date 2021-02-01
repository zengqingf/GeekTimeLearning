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

if __name__ == "__main__":
    pass