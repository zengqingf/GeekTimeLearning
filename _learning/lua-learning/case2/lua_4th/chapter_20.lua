--[[
元表
可以修改一个值在面对一个未知操作时的行为。例如，假设a和b都是表，那么可以通过元表定义Lua语言如何计算表达式a+b。
当Lua语言试图将两个表相加时，它会先检查两者之一是否有元表（metatable）且该元表中是否有__add字段。
如果Lua语言找到了该字段，就调用该字段对应的值，即所谓的元方法（metamethod）（是一个函数），在本例中就是用于计算表的和的函数。

可以认为，元表是面向对象领域中的受限制类。
像类一样，元表定义的是实例的行为。
不过，由于元表只能给出预先定义的操作集合的行为，所以元表比类更受限；同时，元表也不支持继承。
]]

do
    local t = {}
    print("metatable is ", getmetatable(t))

    local t1 = {}
    setmetatable(t, t1)
    print("metatable is ", getmetatable(t) == t1)

    --[[
        lua中 只能为 table设置 元表
        对其他类型的值设置元表，需要通过C代码或调试库完成（防止过度使用对某种类型的所有值生效的元表，旧版本lua表明，这样全局设置经常导致不可宠用的代码）

        字符串标准库默认设置了一个元表，其他类型默认没有元表

        一个表可以成为任意值的元表

        一组相关的表也可以共享一个描述了它们共同行为的通用元表

        一个表可以成为自己的元表，用于描述自身特有的行为
    ]]
    print("metatable is ", getmetatable("hi"))
    print("metatable is ", getmetatable("jz"))
    print("metatable is ", getmetatable(10))
    print("metatable is ", getmetatable(print))
end


do
    local Set = {}
    local mt = {}

    function Set.new(l)
        local set = {}
        setmetatable(set, mt)
        for _, v in ipairs(l) do
            set[v] = true
        end
    end

    function Set.union(a, b)
        local res = Set.new{}
        for k in pairs(a) do
            res[k] = true
        end
        for k in pairs(b) do
            res[k] = true
        end
        return res
    end

    function Set.intersection(a, b)
        local res = Set.new{}
        for k in pairs(a) do
            res[k] = b[k]
        end
        return res
    end

    function Set.tostring(set)
        local l = {}
        for e in pairs(set) do
            l[#l + 1] = tostring(e)
        end
        return "{" .. table.concat(l, ", ") .. "}"
    end

    return Set
end