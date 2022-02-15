--[[
    函数 
]]
do
    --多返回值
    local function maximun(a)
        local mi = 1
        local m = a[mi]
        for i = 1, #a do
            if a[i] > m then
                mi = i; m = a[i]
            end
        end
        return m, mi
    end

    print(maximun({1, 12, 15, 78, 65}))  -- 78  4

    local function foo0()
    end

    local function foo1()
        return "a"
    end

    local function foo2()
        return "a", "b"
    end

    local x, y = foo2()
    print(x, y)
    local x = foo2()
    print(x)
    local x, y ,z = 10, foo2()
    print(x, y, z)

    local x, y = foo0()
    print(x, y)
    local x, y = foo1()
    print(x, y)
    local x, y, z = foo2()
    print(x, y, z)

    --[[
        只有当函数调用是一系列表达式中的最后（或者是唯一）一个表达式时才能返回多值结果，否则只能返回一个结果
    ]]
    local x, y = foo0(), 20, 30
    print(x, y)                     -- x = nil y = 20      （30 被丢弃）
    local x, y = foo2(), 20
    print(x, y)                     -- x = "a" y = 20       (b 被丢弃)

    --[[
        当一个函数调用是另一个函数调用的最后一个（或者是唯一）实参时，第一个函数的所有返回值都会被作为实参传给第二个函数
    ]]
    print(foo0())           --没有结果
    print(foo1())           -- a
    print(foo2())           -- a   b
    print(foo2(), 1)        -- a   1   (b 被丢弃)
    print(foo2() .. "x")    -- ax     （b 被丢弃）

    --[[
        只有当函数调用是表达式列表中的最后一个时，表构造器会完整接收函数调用的所有返回值 不会调整返回值的个数
        在其他位置时，调用只返回一个结果
    ]]
    local t = {foo0()}
    print(t)
    local t = {foo1()}
    print(t)                        -- t = {"a"}
    local t = {foo2()}
    print(t)                        -- t = {"a", "b"}

    local t = {foo0(), foo2(), 4}
    print(t)                        --t[1] = nil, t[2] = "a", t[3] = 4

    --[[
        形如 return f() 会返回 f的所有返回值

        @注意：return (f()) 只返回一个结果
    ]]
    local function foo(i)
        if i == 0 then 
            return foo0()
        elseif i == 1 then
            return foo1()
        elseif i == 2 then
            return foo2()
        end
    end
    print(foo(0)) -- 五结果
    print(foo(1)) -- a
    print(foo(2)) -- a b
    print(foo(3)) -- 无结果

    --[[
        函数调用用一对圆括号括起来 可以强制只返回一个结果
    ]]
    print((foo0()))  -- nil
    print((foo1()))  -- a
    print((foo2()))  -- a 
end

--[[
    可变长参数

    可变长参数表达式 ==> 类似于一个具有多个返回值的函数 返回当前函数的所有可变长参数

    可变长参数前可以放任意数量的固定参数，且固定参数必须放在可变长参数前
]]
do
    local function add(...)
        local s = 0

        local a, b = ...                        --可变长参数表达式
        print(a, b)                            
        print(...)

        for _, v in ipairs{...} do              --表达式 {...} 表示由可变长参数组成的列表
            s = s + v
        end
        return s
    end
    print(add(3, 4, 5, 10, 24))

    --通过变长参数来模拟Lua语言中普通的参数传递机制
    local function foo(a, b, c)
    end
    -- 可以写成
    local function foo(...)
        local a, b, c = ...
    end


    --应用： 追踪某个特定函数调用 使用   多值恒等式函数（只是将调用它时所传入的所有参数简单地返回）
    local function foo1 (...)
        print("calling foo:", ...)
        return foo(...)
    end

    --应用：lua格式化输出
    --扩展 io.write 支持可变长参数
    local function fwrite(fmt, ...)
        return io.write(string.format(fmt, ...))
    end

    --遍历可变长参数
    --为了避免可变长参数中存在nil 导致使用{...}遍历中断
    --使用table.pack  
    --[[
        lua5.2引入
        该函数像表达式{...}一样保存所有的参数，然后将其放在一个表中返回，但是这个表还有一个保存了参数个数的额外字段"n"
    ]]
    local function nonils(...)
        local args = table.pack(...)
        for i = 1, args.n do
            if args[i] == nil then
                return false
            end
        end
        return true
    end
    print(nonils(2,3,nil))
    print(nonils(2,3))
    print(nonils())                 -- true
    print(nonils(nil))

    --遍历可变长参数
    --使用 select()
    --[[
        函数select总是具有一个固定的参数selector，以及数量可变的参数。
        如果selector是数值n，那么函数select则返回第n个参数后的所有参数；
        否则，selector应该是字符串"#"，以便函数select返回额外参数的总数。

        把select（n,...）认为是返回第n个额外参数的表达式
    ]]
    print(select(1, "a", "b", "c"))         -- a b c
    print(select(2, "a", "b", "c"))         -- b c
    print(select(3, "a", "b", "c"))         -- c
    print(select("#", "a", "b", "c"))       -- 3

    local function add(...)
        local s = 0
        for i = 1, select("#", ...) do
            s = s + select(i, ...)
        end
        return s
    end
    print(add(1, 2, 3))         -- 6

    --比较两个 add(...)
    --[[
        对于参数较少的情况，第二个版本的add更快，因为该版本避免了每次调用时创建一个新表。
        不过，对于参数较多的情况，多次带有很多参数调用函数select会超过创建表的开销，因此第一个版本会更好
        （特别地，由于迭代的次数和每次迭代时传入参数的个数会随着参数的个数增长， 因此第二个版本的时间开销是二次代价（quadratic cost）的）
    ]]
end


--[[
    table.unpack 处理多返回值

    该函数的参数是一个数组，返回值为数组内的所有元素

    函数table.unpack与函数table.pack的功能相反。
    pack把参数列表转换成Lua语言中一个真实的列表（一个表），
    而unpack则把Lua语言中的真实的列表（一个表）转换成一组返回值，进而可以作为另一个函数的参数被使用。
]]
do
    print(table.unpack({10, 20, 30}))       --10    20  30
    print(table.unpack{10, 20, 30})         --10    20  30
    local a, b = table.unpack{10, 20, 30}   -- 30被丢弃
    
    --应用
    --[[
        泛型调用 (generic call)

        泛型调用机制允许我们动态地调用具有任意参数的任意函数
        c语言不支持泛型调用

        lua支持：泛型调用机制允许我们动态地调用具有任意参数的任意函数
        f(table.unpack(a))
    ]]
    print(string.find("hello", "ll"))
    --代码转换
    local f = string.find
    local fs = {"hello", "ll"}
    print(f(table.unpack(fs)))

    --[[
        table.unpack使用长度操作符获取返回值的个数（仅用于序列，即没有nil）
    ]]
    print(table.unpack({"sun", "mon", "tue", "wed"}, 2, 3))     --指定返回的元素范围  -- mon tue

    --[[
        lua原生实现 内置c实现的unpack()
    ]]
    local function unpack_bylua(t, i, n)
        i = i or 1
        n = n or #t
        if i <= n then
            return t[i], unpack_bylua(t, i + 1, n)
        end
    end
    print(unpack_bylua({"sun", "mon", "tue", "wed"}, 2, 3))     -- mon tue
end


--[[
    lua 尾调用

    只有形如 return func(args) 的调用才是尾调用   func及其参数可以为复杂表达式

    Lua语言是支持尾调用消除（tail-callelimination）的。这意味着Lua语言可以正确地（properly）尾递归（tailrecursive），虽然尾调用消除的概念并没有直接涉及递归

    尾调用（tail call）是被当作函数调用使用的跳转[插图]。当一个函数的最后一个动作是调用另一个函数而没有再进行其他工作时，就形成了尾调用。

    function f(x)  x = x + 1; return g(x) end
    函数g 为尾调用
    函数f调用完函数g后，f不再进行其他工作，当调用函数执行结束后，程序就不再需要返回最初的调用者
    因此，在尾调用之后，程序也就不需要在调用栈中保存有关调用函数的任何信息。当g返回时，程序的执行路径会直接返回到调用f的位置。

    应用：
    Lua语言解释器：使得在进行尾调用时不使用任何额外的栈空间，尾调用消除（tail-call elimination）

    由于尾调用不会使用栈空间，所以一个程序中能够嵌套的尾调用的数量是无限的。不会stack overflow
]]
do
    --[[
        关键点：判断是否是尾调用
    ]]
    local function g(x) return 0 end
    local function f(x)
        g(x)               --不是尾调用 函数f还需要丢弃函数g的返回值
    end

    local function f(x)
        return g(x) + 1    --不是尾调用 函数f还需要进行加法
    end
    local function f(x)
        return x or g(x)   --不是尾调用 函数f还需要把返回值限定为1个   @注意
    end
    local function f(x)
       return (g(x))      --不是尾调用 函数f还需要把返回值限定为1个   @注意
    end

    local function goo(x, y)
    end
    local function foo(x, i, j, a, b)
        return x[i].goo(x[j] + a*b, i + j)      --是尾调用
    end
end