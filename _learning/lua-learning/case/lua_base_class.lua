--[[
    lua 面向对象

    class实现:
    ref:https://blog.codingnow.com/cloud/LuaOO
    ref:https://www.jianshu.com/p/b8f794927f3d

    Lua中类、对象都通过table实现。为了概念上的清晰，这里将其分开，视为不同的东西：class用于定义类结构，new用于创建对象。

    用Table作为类结构，也就是生成对象的模板。
    直接定义在类上的成员（函数、变量），都是在所有对象间共享的（此处和Python一样）
    类结构中，用base变量引用一个基类结构，实现了单继承。
    通过__index元方法，实现了高效的基类成员检索（继承）

    直接用元表完成对象结构的创建
    _Create实现了C#的对象构造顺序（基类->子类）
    在构造函数Ctor中对self赋值的变量，才是成员变量（Python方式）。当然由于动态语言的特性，在任意位置给self赋值都有效。
]]

local _class = {}
function class(super)
    local class_type = {}
    class_type.ctor = false
    class_type.super = super
    class_type.new = function (...)
        local obj = {}
        do
            local create = nil
            create = function(c, ...)
                if c.super then
                    create(c.super, ...)
                end
                if c.ctor then
                   c.ctor(obj, ...)
                end
            end
            create(class_type, ...)
        end

        setmetatable(obj, {__index = _class[class_type]})
        return obj
    end

    local vtbl = {}
    _class[class_type] = vtbl

    setmetatable(class_type, {__newindex = 
    function (t, k, v)
        vtbl[k] = v
    end})

    if super then
        setmetatable(vtbl, {__index = 
        function (t, k)
            local ret = _class[super][k]
            vtbl[k] = ret
            return ret
        end})
    end

    return class_type
end


do
    local base_type = class()           --基类
    function base_type:ctor(x)          --构造函数
        print("base type ctor")
        self.x = x
    end

    function base_type:printX()         --成员函数
        print(self.x)
    end

    function base_type:hello()          --成员函数
        print("base type hello")
    end


    ---------------------------------------------------------

    local test = class(base_type)
    function test:ctor()
        print("test ctor")
    end

    function test:hello()               --重载基类成员函数
        print("hello test")
    end



    ---------------------------------------------------------

    local a = test.new(10)              -- base type ctor       test ctor  对象被正确构建
    a:printX()                          -- 10                   基类 base_type 中的成员函数。
    a:hello()                           -- hello test           test类重载基类成员函数
end