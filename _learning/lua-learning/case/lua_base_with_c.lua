
--[[Lua 获取毫秒精度时间 ref:https://www.cnblogs.com/zhanggaofeng/p/13796127.html
    
    安装 socket.tgz]]
do
    -- 设置动态库路径
    package.cpath = './luaclib/?.so;'
    -- 加载socket
    local socket = require('socket.core')
    -- 获取当前毫秒时间
    local current = socket.gettime() * 1000
    print(string.format('current time is %d ',current))
end