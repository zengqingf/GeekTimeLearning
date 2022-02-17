--[[
    lua for loop
    ref: https://www.cnblogs.com/tangxin-blog/p/10300714.html

    1. 数值型
        for var exp1, exp2, exp3 do
            something
        end

        var是被for自动声明的局部变量，初始值为exp1，作用范围仅限于for循环内部。
        var的值小于等于exp2之前的每次循环中都会执行something，
        每次循环结束后都会将步长（exp3）增加到var上，exp3是可选的，如果没有指定，默认步长为1。
        如果想跳出循环可以使用break。

    2. 泛型
        for var-list in exp-list do
            body
        end
        
        var-list:一个或多个变量组成的列表，由逗号分隔                  var-list中的第一个变量称为 控制变量（control variable）  一定不为nil，为nil时循环结束
        exp-list:一个或多个表达式组成的列表，同样由逗号分隔             
                                                                    -- _f 迭代函数
                                                                    -- _s 不可变状态
                                                                    -- _v 控制变量初始值
                                                                    local _f, _s, _v = exp-list     表达式exp-list返回值最多保留3个，不足3个用nil补齐

                                                                    泛型for把_s和_v作为参数调用_f，如果返回的第一个值（控制变量）为nil，表示循环结束，
                                                                    否则，把_s和新的返回值作为参数，继续调用_f。其调用过程类似于数列

                                                                        local _v1, ... = _f(_s, _v)
                                                                        local _v2, ... = _f(_s, _v1)
                                                                        local _v3, ... = _f(_s, _v2)
                                                                        ...
                                                                        local _vn, ... = _f(_s, _vn-1)

                                                                    每次循环产生一次调用。这里_f可以返回多个值，但是只有第一个返回值才是控制变量，用来决定循环是否还要继续。
       for var_1, var_2, ... in explist do something end

       ==> 泛型for
       local _f, _s, _v = exp-list
        while true do
            local var_1, var_2, ... = _f(_s, _v)
            _v = var_1
            if not _v then break end
            something
        end

        和基本迭代器相比，这里的迭代函数_f有一个显著的区别，就是_f可以接收参数了，并且参数的值就包含了当前迭代器的状态，也就是说迭代器自身不需要保存状态了。
        
        ==> 无状态迭代器
        无状态迭代器就是一种不需要保存任何状态的迭代器。
        因此在多个循环中可以使用同一个迭代器，从而避免了创建闭包的开销，让代码在性能上得到了提升。
        ipairs就是一个典型的无状态迭代器
]]

do

    --表达式只执行一次
    local function exp1() print("exp1") return 1 end        --初始值
    local function exp2() print("exp2") return 5 end        --循环次数
    local function exp3() print("exp3") return 1 end        --步长

    for i = exp1(), exp2(), exp3() do
        print(i)
    end

    --for i = 1, math.huge do
        --print(i)                                            --无限循环
    --end
end


--[[
    copy from lua_base_7.lua
]]
do
    local function values(t)          --创建闭包和封装变量的“工厂”
        local i = 0
        print("create iter")
        return function()             --闭包本身
            i = i + 1
            print("call iter", i)
            return t[i]
        end
    end

    local iter = values({1, 2, 3, 4})
    while true do
        local v = iter()
        if not v then
            break
        end
        print(v)
    end

    --[使用泛型for  同上]
    for f in values({1, 2, 3, 4}) do                --values(...) 表达式只返回了迭代函数  不可变状态和控制变量初始值 为nil
        print(f)
    end
end

--[[
    仅适用于for loop的无状态迭代器
]]
do

    --[[
        pairs函数也是一个无状态迭代器，它调用的是lua中一个基本的迭代函数：next(t, k)。pairs的原型可以描述如下：
            next(t, k)的返回值有两个：
                1. 随机次序返回k的下一个键，当key==nil的时候返回表中的第一个键，当所有表遍历完毕，返回nil
                2. k对应的值，当k==nil的时候返回表中的第一个值
        function pairs(t)
            return next, t, nil
        end
        --可以不适用pairs() 直接调用next
        for i, v in next, {"hello", "lua", "for"} do
            print(i, v)
        end
    ]]

    
    for i, v in ipairs({"hello", "lua", "for"}) do
        print(i, v)
    end


    --自实现
    --[[
    
    for循环第一步: 对my_ipairs求值，_f, _s, _v分别获取返回值
        _f = my_iter
        _s = t
        _v = 0

    第一次循环：执行
        i， v = _f(t, _v)  --等价于 i, v = my_iter(t, 0)
        -- i = 1， v = t[1]

    第二次循环：泛型for会把第一次循环中_f(t, _v)的第一个返回值(i)作为控制变量，进行第二次调用：
        i， v = _f(t, _v)  --等价于 i, v = my_iter(t, 1)
        -- i = 2, v = t[2]


    一直循环到 i = nil
    ]]
    local function my_iter(t, i)
        print("call:"..i)
        i = i + 1
        local v = t[i]
        if v then 
            return i, v
        end
    end
    
    local function my_ipairs(t)
        print("init")
        return my_iter, t, 0
    end
    
    for i, v in my_ipairs({"hello", "lua", "for"}) do
        print(i, v)
    end
end