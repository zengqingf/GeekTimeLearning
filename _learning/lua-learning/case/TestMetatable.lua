--[[
Lua中定义的常用的Metamethod如下所示：
算术运算符的Metamethod：
__add（加运算）、__mul（乘）、__sub(减)、__div(除)、__unm(负)、__pow(幂)，__concat（定义连接行为）。
关系运算符的Metamethod：
__eq（等于）、__lt（小于）、__le（小于等于），其他的关系运算自动转换为这三个基本的运算。
库定义的Metamethod：
__tostring（tostring函数的行为）、__metatable（对表getmetatable和setmetatable的行为）。
]]

do
    TestMetatable = {}
    TestMetatable.mt = {}                       --共享一个metatable

    function TestMetatable.new(t)               --新建一个表
        local set = {}
        setmetatable(set, TestMetatable.mt)     --共享一个metatable
        print("t len="..#t)
        for _, v in ipairs(t)
        do
            print("t value="..v)
            set[v] = true
        end
        return set
    end

    function TestMetatable.union(a, b)          --并集
        local res = TestMetatable.new{}         --注意这是大括号 用({})也可以
        for i in pairs(a) 
        do 
            print("union, a, i="..i)
            res[i] = true 
        end
        for i in pairs(b) 
        do 
            print("union, b, i="..i)
            res[i] = true 
        end
        return res
    end

    function TestMetatable.intersection(a, b)  --交集
        local res = TestMetatable.new{}        --注意这是大括号
        for i in pairs(a) 
        do
            print("intersection, a, i=".. i .. ", b[i]=".. tostring(b[i]))
            res[i] = b[i]
        end
        return res
    end

    function TestMetatable.diff(a, b)          --差集
        local res = TestMetatable.new{}
        local unionRes = a + b
        local intersection = a * b
        for i in pairs(unionRes)
        do
            if intersection[i] == nil
            then
                res[i] = unionRes[i]
            end
        end
        return res
    end

    function TestMetatable.sub(a, b)            --差集2
        local res = TestMetatable.new{}
        for k, v in pairs(a)
        do
            if b[k] == nil then
                res[k] = true
            end
        end
        return res
    end

    function TestMetatable.tostring(set)       --打印函数输出结果的调用函数
        local s = "{"
        local sep = ""
        for i in pairs(set) do
            s = s .. sep .. i
            sep = ","
        end
        return s.."}"
    end

    function TestMetatable.print(set)          --打印函数输出结果
        print(TestMetatable.tostring(set))
    end

    TestMetatable.mt.__add = TestMetatable.union
    TestMetatable.mt.__mul = TestMetatable.intersection
    TestMetatable.mt.__sub = TestMetatable.sub
end