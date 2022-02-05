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
]]
do
    local function add(...)
        local s = 0
        for _, v in ipairs{...} do
            s = s + v
        end
        return s
    end
    print(add(3, 4, 5, 10, 24))
end