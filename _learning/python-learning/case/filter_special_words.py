#!/usr/bin/env python
# -*- encoding: utf-8 -*-

import re


'''
sub(pattern,repl,string)	把字符串中的所有匹配表达式pattern中的地方替换成repl
[^**]	表示不匹配此字符集中的任何一个字符
\u4e00-\u9fa5	汉字的unicode范围
\u0030-\u0039	数字的unicode范围
\u0041-\u005a	大写字母unicode范围
\u0061-\u007a	小写字母unicode范围
\uAC00-\uD7AF	韩文的unicode范围
\u3040-\u31FF	日文的unicode范围
'''

if __name__ == "__main__":
    s1 = '1123*#$ 中abc国='
    str1 = re.sub('\W+', '-', s1)  # ok   推荐！
    print(s1)
    print(str1)
    print(str1.replace('=', ''))

    str1_1 = re.sub('\W+', '', s1)  # ok   推荐！
    print(str1_1)

    str2 = re.sub('[\001\002\003\004\005\006\007\x08\x09\x0a\x0b\x0c\x0d\x0e\x0f\x10\x11\x12\x13\x14\x15\x16\x17\x18\x19\x1a]+', '-', s1)
    print(str2)

    s2 = "123我123456abcdefgABCVDFF？/ ，。,.:;:''';'''[]{}()（）《》"
    print(s2)
    str3 = re.sub(u"([^\u4e00-\u9fa5\u0030-\u0039\u0041-\u005a\u0061-\u007a])", "-", s2)  # ok
    print(str3)

    str1_2 = re.sub('\W+', '-', s2)  # ok
    print(str1_2)

    str2_2 = re.sub('[\001\002\003\004\005\006\007\x08\x09\x0a\x0b\x0c\x0d\x0e\x0f\x10\x11\x12\x13\x14\x15\x16\x17\x18\x19\x1a]+', '-', s2)
    print(str2_2)

    s3 = ''
    str1_3 = re.sub('\W+', '-', s3)  # ok   推荐！
    print(str1_3)

    