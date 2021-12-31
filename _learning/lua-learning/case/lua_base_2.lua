--[[
使用module

模块加载机制：
    对于自定义的模块，模块文件不是放在哪个文件目录都行，函数 require 有它自己的文件路径加载策略，它会尝试从 Lua 文件或 C 程序库中加载模块。
    
    require 用于搜索 Lua 文件的路径是存放在全局变量 package.path 中，当 Lua 启动后，会以环境变量 LUA_PATH 的值来初始这个环境变量。
    如果没有找到该环境变量，则使用一个编译时定义的默认路径来初始化。

    添加环境变量（LUA_PATH）
    #LUA_PATH
    export LUA_PATH="~/lua/?.lua;;"     --文件路径以 ";" 号分隔，最后的 2 个 ";;" 表示新加的路径后面加上原来的默认路径。

]]
do
    require("MyModule")
    --require "MyModule"
    print(MyModule.constant)
    MyModule.func1()
    MyModule.func3()

    local m = require("MyModule")           --模块别名
    print(m.constant)
    m.func3()

    --创建元表
    local x = MyModule.new{content = "bye"}
    print(x.size, x.content)                 --34   bye
end


--[[
    Lua Require函数
    
    1. require会搜索目录加载文件
    require使用的路径和普通我们看到的路径还有些区别，我们一般见到的路径都是一个目录列表。require的路径是一个模式列表，每一个模式指明一种由虚文件名
    （require的参数）转成实文件名的方法。更明确地说，每一个模式是一个包含可选的问号的文件名。匹配的时候Lua会首先将问号用虚文件名替换，然后看是否有
    这样的文件存在。如果不存在继续用同样的方法用第二个模式匹配

    2. require会判断是否文件已经加载避免重复加载同一文件。由于上述特征，require在Lua中是加载库的更好的函数。
    require会避免重复加载同一个文件两次。Lua保留一张所有已经加载的文件的列表（使用table保存）。如果一个加载的文件在表中存在require简单的返回；
    表中保留加载的文件的虚名，而不是实文件名。所以如果你使用不同的虚文件名require同一个文件两次，将会加载两次该文件。比如require "foo"和require "foo.lua"，
    路径为"?;?.lua"将会加载foo.lua两次。我们也可以通过全局变量_LOADED访问文件名列表，这样我们就可以判断文件是否被加载过；同样我们也可以使用一点小技巧让
    require加载一个文件两次。比如，require "foo"之后_LOADED["foo"]将不为nil，我们可以将其赋值为nil，require "foo.lua"将会再次加载该文件。

    小结:require加载一个Lua文件，不论加载多少次，都是同一个对象
]]



--[[
元表（Metatable）
允许对两个table进行操作
]]

do
    --为指定表设置元表
    local myTable = {}
    local myMetatable = {}
    setmetatable(myTable, myMetatable)
    --上述等同于
    myTable = setmetatable({}, {})
    --返回元表
    getmetatable(myTable)
end

-- 元方法 __index 用于对表访问
--[[
    当你通过键来访问 table 的时候，如果这个键没有值，那么Lua就会寻找该table的metatable（假定有metatable）中的__index 键。
    如果__index包含一个表格，Lua会在表格中查找相应的键。

    如果__index包含一个函数的话，Lua就会调用那个函数，table和键会作为参数传递给函数。
    __index 元方法查看表中元素是否存在，如果不存在，返回结果为 nil；如果存在则由 __index 返回结果。
]]
do
    local other = {foo = 3}
    local t = setmetatable({}, {__index = other})
    print(t.foo)

    --给myTable设置了元表，元方法为__index
    --判断元表有没有__index方法，如果__index方法是一个函数，则调用该函数
    --元方法中查看是否传入 "key2" 键的参数（myTable.key2已设置！！！），如果传入 "key2" 参数返回 "metatablevalue"，否则返回 mytable 对应的键值。
    local myTable = setmetatable( {key1 = "value1"} , {
        __index = function (table, key)
            print(table.key1, key)
            --if key == "key2" or key == "key3" then                
            if key == "key2" or key == "key3" and key == "key6" then
            --if  key == "key6" and key == "key2" or key == "key3" then     
            --if  key == "key6" or key == "key2" or key == "key3" then            -- 上述四种情况 结果不同 （and优先级高于or 先结合）
                return "metatablevalue1"
            elseif key == "key4" and key == "key5" then
                return "metatablevalue2"
            else
                return nil
            end
        end
    })

    print(myTable.key1, myTable.key2, myTable.key3, myTable.key4, myTable.key5, myTable.key6)

    --可以简写
    local mytable = setmetatable({key1 = "value1"}, { __index = { key2 = "metatablevalue" } })
    print(mytable.key1,mytable.key2)
end

--[[
ref:https://www.runoob.com/lua/lua-metatables.html
Lua 查找一个表元素时的规则，其实就是如下 3 个步骤:

1.在表中查找，如果找到，返回该元素，找不到则继续
2.判断该表是否有元表，如果没有元表，返回 nil，有元表则继续。
3.判断元表有没有 __index 方法，如果 __index 方法为 nil，则返回 nil；如果 __index 方法是一个表，则重复 1、2、3；如果 __index 方法是一个函数，则返回该函数的返回值。
]]


--[[
元方法 __newindex 对表更新
当你给表的一个缺少的索引赋值，解释器就会查找__newindex 元方法：如果存在则调用这个函数而不进行赋值操作。
]]
do
    local myMetatable = {}
    local myTable = setmetatable({key1 = "value1"}, {__newindex = myMetatable})

    print(myTable.key1)
    myTable.newKey = "new value2"
    print(myTable.newKey, myMetatable.newKey)   -- nil      new value2
    --表设置了元方法 __newindex，在对新索引键（newkey）赋值时（mytable.newkey = "新值2"），会调用元方法，而不进行赋值

    myTable.key1 = "new value1"
    print(myTable.key1, myMetatable.key1)       -- new value1      nil
    --对已存在的索引键（key1），则会进行赋值，而不调用元方法 __newindex
end

-- 通过__newindex实现只读table
--[[
    __newindex 来实现下面是代码 lua 中 __newindex 的调用机制跟 __index 的调用机制是一样的，
    当访问 table 中一个不存在的 key，并对其赋值的时候，lua 解释器会查找 __newindex 元方法，
    如果存在，调用该方法，如果不存在，直接对原 table 索引进行赋值操作。

1、如果 __newindex 是一个函数，则在给 table 中不存在的字段赋值时，会调用这个函数，并且赋值不成功。
2、如果 __newindex 是一个 table，则在给 table 中不存在的字段赋值时，会直接给 __newindex的table 赋值。
]]
do
    local t = {}
    local prototype = {}
    local mt = {
    __index = 
    function (t,k)
        return prototype[k]
    end,
    __newindex = 
    function (t, k ,v)
        print("attempt to update a table k-v")
        prototype[k] = v
    end
    }

    setmetatable(t, mt)
    t[2] = "hello"
    print(t[2])

    local function readOnly(t)
        local proxy = {}          --定义一个空表
        local mt = {
            __index = t,
            __newindex = function (t, k ,v)
                error("attempt to update a read-only table", 2)
            end
        }
        setmetatable(proxy, mt)
        return proxy
    end

    local days = readOnly{"Sunday","Monday","Tuesday","Wednessday","Thursday","Friday","Saturday"} 
    print(days[1])
    --! days[2] = "hello"  --这一行就非法访问了
end

--使用rawset更新表
do
    local myTable = setmetatable({key1 = "value1"}, {
        __newindex = function (table, key, value)
            rawset(table, key, "\"" .. value .. "\"")
        end
    })

    myTable.key1 = "new value"
    myTable.key2 = 4
    print(myTable.key1, myTable.key2)
end


--[[
元方法 __add  两边相加

__add	对应的运算符 '+'.
__sub	对应的运算符 '-'.
__mul	对应的运算符 '*'.
__div	对应的运算符 '/'.
__mod	对应的运算符 '%'.
__unm	对应的运算符 '-'.
__concat	对应的运算符 '..'.
__eq	对应的运算符 '=='.
__lt	对应的运算符 '<'.
__le	对应的运算符 '<='.

]]
do
    require("Utils")

    -- 两表相加操作
    local myTable = setmetatable({1, 2, 3}, {
        __add = function (mytable, newTable)   -- mytable 当上下文使用了table内置函数时， 不要简写成 table 会产生歧义
            for i = 1, Utils.table_max_key(newTable) do
                table.insert(mytable, Utils.table_max_key(mytable) + 1, newTable[i])
            end
            return mytable
        end
    })

    local secondTable = {4,5,6}
    myTable = myTable + secondTable
    for k,v in ipairs(myTable) do
        print(k, v)
    end
end

--[[
元方法 __call
在 Lua 调用一个值时调用
]]
do
    local myTable = setmetatable({10}, {
        __call = function (table, newTable)
            local sum = 0
            for i = 1, Utils.table_max_key(table) do
                sum = sum + table[i]
            end
            for i = 1, Utils.table_max_key(newTable) do 
                sum = sum + newTable[i]
            end
            return sum
        end
    })
    local newTable = {10, 20, 30}
    print(myTable(newTable))
end


--[[
元方法 __tostring
用于修改表的输出行为
]]
do
    local myTable = setmetatable({10, 20, 30}, {
        __tostring = function (table)
            local sum = 0
            for k, v in pairs(table) do
                sum = sum + v
            end
            return "the sum of all values in table is ".. sum
        end
    })

    print(myTable)
end


-- 测试 TestMetatable
--[[
    用表进行了集合的并集和交集操作。

    Lua 选择 metamethod 的原则：如果第一个参数存在带有 __add 域的 metatable，Lua 使用它作为 metamethod，和第二个参数无关；
    否则第二个参数存在带有 __add 域的 metatable，Lua 使用它作为 metamethod 否则报错。
]]
do
    require("TestMetatable")
    local tm1 = TestMetatable.new{1, 2}
    local tm2 = TestMetatable.new{3, 4}
    print(getmetatable(tm1))
    print(getmetatable(tm2))
    local tm3 = tm1 + tm2
    TestMetatable.print(tm3)                 -- use __add
    TestMetatable.print((tm1 + tm2) * tm3)   -- use __mul
    local tm4 = TestMetatable.new{1, 3, 4}
    TestMetatable.print(tm1 - tm4)          -- use __sub
    TestMetatable.print(tm4 - tm2)          -- use __sub
end

--[[
    求两数组的交集并集
    ref: https://blog.csdn.net/qiuwen_521/article/details/107816450
]]
do
    
end