local lualib_2 = require "LuaLib_2"   -- copy from LuaLib_2 dll project gen
                     -- lua 内部实现 ：
                     -- 1. local path = "LuaLib_2.all"
                     -- 2. local f = package.loadlib(path, "luaopen_LuaLib_2")  返回luaopen_LuaLib_2函数
                     -- 3. f()    执行函数
local ave,sum = lualib_2.average(1,2,3,4,5) --参数对应栈中数据
print(ave, sum)
lualib_2.sayHello() 
--1.如果不使用lualib_2访问 并且不使用 lua_register()从c中注册方法 则不能执行 获取不到  attempt to call a nil value (global 'average')
--2.如果不使用lualib_2访问 但使用lua_register()注册  可以执行
--3.需要使用LuaLib_2.dll（使用的5.3.6）对应版本的lua.exe c:\Users\tenmove\.vscode\extensions\actboy168.lua-debug-1.41.0\runtime\windows\x86\lua53\lua.exe 