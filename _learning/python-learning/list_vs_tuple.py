#!/usr/bin/env python
# -*- encoding: utf-8 -*-
'''
@File           : list_vs_tuple.py
@Time           : 2020/11/18 16:03:15
@Author         : mjx
@Version        : 1.0.0
@Desc           : 
'''

'''
列表是动态的，长度大小不固定，可以随意地增加、删减或者改变元素（mutable）。


元组是静态的，长度大小固定，无法增加删减或者改变（immutable）。
'''


ls = [1, 2, 3, 4]
ls[3] = 40
print(ls)

tup = (1, 2, 3, 4)
#tup[3] = 40

# 开辟一块新内存 创建新的元组 然后把两个元组依次加进去
new_tup = tup + (5,)  # 不能为 (5)
print(new_tup)


print(ls[-1])
print(tup[-2])

print(ls[1:3])
print(tup[0:2])


# 可以互相嵌套
list_list = [[1, 2, 3], [4, 5]]
tup_tup = ((1, 2, 3), (4, 5, 6))
list_tup = [(1, 2, 3), (4, 5, 6)]
tup_list = ([1, 2, 3], [4, 5])

print(list_list)
print(tup_tup)
print(list_tup)
print(tup_list)


# 如果上文中定义变量为 list 则会覆盖原生 list函数
tup_to_list = list(tup)
list_to_tup = tuple(ls)
print(tup_to_list)
print(list_to_tup)



ls_size = [1, 2, 3]
print(ls_size.__sizeof__())

tup_size = (1, 2, 3)
print(tup_size.__sizeof__())

