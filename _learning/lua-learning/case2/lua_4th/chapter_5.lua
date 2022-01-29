--[[
    Table 表
    lua唯一的数据结构
    使用表，Lua语言可以以一种简单、统一且高效的方式表示数组、集合、记录和其他很多数据结构。
    Lua语言也使用表来表示包（package）和其他对象。

    调用math.sin() 其实是调用了以 字符串sin为键检索表math

    表本质是一个辅助数组（associative array），是一种动态分配的对象

        表可以看作引用类型 lua不会进行隐藏的拷贝（hidden copies）或者创建新的表

        表的索引可以是 数值 字符串 和 其他任意类型的值  nil不能做为键  
        表的值都是对象，数值或者变量

        表是匿名的 保存表的变量指向表的引用

        GC: 表没有指向其的引用时，GC会触发，回收内存


    表里可以存不同类型的索引，并且可以按需增长以容纳新的元素

    表 a.name == a["name"]
    a.name 倾向于当作 结构体 使用
            表是固定的 预先定义的键组成的集合
                name是 变量name对应值的索引
    a["name"] 表使用任意字符串作为键 操作的是指定的键
                name是 字符串“name”的索引

    不确定表索引的真实数据类型时，可以使用显式类型转换
    如 字符串“+1” “01” “1”

    整形和浮点型 的表索引 
    如 2 == 2.0  作为表索引是等价的

    浮点型作为表索引时，能转换成整型的浮点数都会被转换成整型
    不能被转换为整型的（2.2 != 2）则只作为浮点型索引
]]

do
   local a = {}
   local k = "x"
   a[k] = 10
   a[20] = "great"
   print(a["x"])    -->10
   k = 20
   print(a[k])      -->great
   a["x"] = a["x"] + 1
   print(a["x"])    -->11
end

do
    local a = {}
    a["x"] = 10
    local b = a
    print(b["x"])   -->10
    b["x"] = 20
    print(a["x"])   -->20
    a = nil
    b = nil
end

do
    local a = {}
    for i = 1, 1000 do
        a[i] = i * 2
    end
    print(a[9])         --> 18
    a["x"] = 10
    print(a["x"])       --> 10
    print(a["y"])       --> nil
end

do
    local a = {}
    a.x = 10            
    print(a.x)          --> 10
    print(a["x"])       --> 10  
    print(a.y)          --> nil

    local x = "y"
    a[x] = 10
    print(a[x])         --> 10
    print(a.x)          --> nil
    print(a.y)          --> 10
end

do
    local i = 10; local j = "10"; local k = "+10"
    local a = {}
    a[i] = "number key"
    a[j] = "string key"
    a[k] = "another string key"
    print(a[i])
    print(a[j])
    print(a[k])

    print(a[tonumber(j)])       --> number key
    print(a[tonumber(k)])       --> number key
end



--[[
    表构造器
    创建和初始化表

    使用(空)构造器 {}

    初始化
    
    记录式（record-like）
    a = {x = 10, y = 20}                -- 推荐使用，可以提前判断表的大小 运行速度更快
    ==>
    a={}; a.x = 10; a.y = 20

    列表式 （list-style）                -- 推荐使用
    b = {1, "2"}
    ==>
    b={}; b[1] = 1; b[2] = "2"           --可以从指定 b = {[0] = 0} 使索引从0开始


    增加删除表元素
    a = {x = 10, y = 20, label = "console"}
    a[1] = "another field" 将键1添加到表a中
    a["2"] = ";;"          将键"2"添加到表a中
    a.x = nil               删除字段x



    在同一个构造器中 可以混用记录式和列表式 （list-style）
    实现表的嵌套

    记录式和列表式
    不能使用负数索引 初始化表元素
    不能用不符合规范的标识符作为索引

    通用构造器

    构造器的最后一个元素后可以加一个逗号（旧版本还支持分号），
    可选（用于不需要对构造器最后一个元素特殊处理）
]]
do
    local a = {x = 10, y = 20, label = "console"}
    a[1] = "another field" --将键1添加到表a中
    a["2"] = ";;"          --将键"2"添加到表a中
    print(a[1])
    print(a[2])
    print(a["2"])
    print(a.x)              --> 10
    a.x = nil               --删除字段x
    print(a.x)              --> nil
end


do
    local a = {
        x = "y",
        y = 1,
        z = 2,
        {m = 0, n= 1},      -- a[1]
        {m = 1, n =2}       -- a[2]
    }

    print(a[2].n)

    local b = {
        [0] = 1,
        [1] = 2
    }
    print(b[0])             -- 1

    
    local c = {
        [-1] = 1,
        [0] = 2
    }
    print(b[-1])             -- nil  不能用负数索引初始化


--[[
    通用构造器，特殊形式：记录式和列表式构造器
]]

    local opnames = {["+"] = "add", ["-"] = "sub", ["*"] = "mul", ["/"] = "div"}
    local i = 20; local s = "-"
    local a2 = {[i + 0] = s, [i + 1] = s..s, [i + 2] = s..s..s}
    print(opnames[s])           --> sub
    print(a2[22])               --> ---

end

do
    --[[
        lua 实现数组array、列表list
        使用整型作为索引的表，不需要预先声明表的大小

        lua中可以用任意数字作为第一个元素的索引，默认是以 1 开始的

        对于 不存在空洞 hole 的列表 可以称为 序列sequence

        使用 # 获取 表对应序列的长度
        使用 # 也可以获取字符串所包含的字节数

        对于存在空洞的表，#取长度不可靠，获取到nil会中断

        a = {10, 20, 30} 等价于
        a = {10, 20, 30, nil, nil}
    ]]

    local seq = {1, 2, 3, 4, 5}
    for i=1, #seq do
        print(seq[i])
    end
    print(seq[#seq])    --输出最后一个值
    seq[#seq] = nil     --移除最后一个值
    seq[#seq + 1] = 6  --在序列最后加一个元素
end

do
    --[[
        遍历表

        使用 pairs迭代器遍历表中的键值对，输出元素的顺序可能是随机的（lua底层机制决定的）
        执行多次，也可能产生不同的顺序，但每个元素只会出现一次

        使用 ipairs迭代器 可以保证桉顺序输出
    ]]
    local t = {10, print, x=12, k = "hi", function ()do
        print(3)
    end
        
    end}
    for k, v in pairs(t) do
        print(k, v)
    end
    --[[
        1       10
        2       function: 0x10abddca0
        3       function: 0x7f87f5508a70
        x       12
        k       hi
    ]]


    --[[
                pair vs. ipair
        pairs可以遍历表中所有的key，并且除了迭代器本身以及遍历表本身还可以返回nil;
        ipairs则不能返回nil,只能返回数字0，如果遇到nil则退出。
        ipairs 这个迭代器只能遍历所有数组下标的值，这是前提，也是和 pairs 的最根本区别，也就是说如果 ipairs 在迭代过程中是会直接跳过所有手动设定key值的变量。
        @注意：ipairs不会被手动键值对中断遍历

        pairs: 迭代 table，可以遍历表中所有的 key 可以返回 nil
        ipairs: 迭代数组，不能返回 nil,如果遇到 nil 则退出


    ]]
    for k, v in ipairs(t) do
        print(k, v)
    end
    --[[
        1       10
        2       function: 0x10abddca0
        3       function: 0x7f87f5508a70
    ]]

    for k = 1, #t do
        print(k, t[k])
    end
    --[[
        1       10
        2       function: 0x10abddca0
        3       function: 0x7f87f5508a70
    ]]
end

do
    --[[
        表的安全访问

        if lib and lib.foo then
        end

        如果以下是一次成功的访问，
        @注意：对表进行6次访问而非3次
        使用 . 操作符就是对表进行访问
        if lib and 
            lib.foo and
            lib.foo.goo and
            lib.foo.goo.joo then
        end

        C#提供了 ?. 安全访问操作符
        a?.b 当a为nil时 不会访问下一个 返回结果为nil而非异常

        lua并不提供安全访问操作符，因为可能会导致程序出现无意的错误

        但可以模拟

        a or {} 当a为nil时，结果为nil
        if ((((lib or {}).foo or {}).goo or {}).joo then
        end
        只会对表访问3次，避免引入新的操作符

        使用 E = {} 可以复用
        if ((((lib or E).foo or E).goo or E).joo then
        end
    ]]
end