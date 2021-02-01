#!/usr/bin/env python
# -*- encoding: utf-8 -*-
'''
@File           : duck_typing.py
@Time           : 2021/01/28 11:36:00
@Author         : mjx
@Version        : 1.0.0
@Desc           : 
'''

class Logger:
    def record(self):
        print("I write a log into file.")

class DB:
    def record(self):
        print("I insert data into db.")


def test(recorder):
    recorder.record()

if __name__ == "__main__":
    logger = Logger()
    db = DB()
    test(logger)
    test(db)