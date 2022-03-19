

do
   -- Lua获取table长度算法
    --[[
    Lua很少使用求长度的算法，
    假设table的类型是数组，可以使用tbale.getn(table)或者#
    但是也有风险，如果数组中有元素的值是nil，那么计算长度就会出错
    假设tbale是键值对，那么tbale.getn(table)或者#都无法使用，只能使用pairs()迭代器的方式
    ]]

    local t1={1,2,3,4,5,6,7,8,9}
    local t2={a=1,b=2,c=3,d=4,e=5}

    -- 计算数组的长度
    --print("t1 length is ",table.getn(t1))
    print("t1 length is ",#t1)

    -- 计算键值对的长度
    local l = 0
    for _,_ in pairs(t2)
    do
        l=l+1
    end
    print("t2 length is ",l)

    -- 计算table的长度(Metatable)
    --[[
        Lua 5.1版本不支持__len
        function len_event (op)
        if type(op) == "string" then
            return strlen(op)         -- 原生的取字符串长度
        elseif type(op) == "table" then
            return #op                -- 原生的取 table 长度
        else
            local h = metatable(op).__len
            if h then
            -- 调用操作数的处理器
            return h(op)
            else  -- 没有处理器：缺省行为
            error(···)
            end
        end
        end
    ]] 
end


--[[
    Lua - Execute a Function Stored in a Table
]]
do
    local function func(a,b,c) return a,b,c end
    local a = {myfunc = func}                               
    print(a.myfunc(3,4,5)) -- prints 3,4,5

    local a = {myfunc = function(a,b,c) return a,b,c end}
    print(a.myfunc(3,4,5)) -- prints 3,4,5


    local tableFunc = require("lua_base_table_2")
    print(tableFunc:func2(3, 4, 5))
    --print(tableFunc:func3(3, 4, 5)) --错误
    print(tableFunc:func4(3, 4, 5))
end