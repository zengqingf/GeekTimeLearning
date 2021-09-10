--[[
lua 模块

由变量、函数等组成的table
table需要定义一个模块同名的全局table

同时模块需要单独一份和模块同名的lua文件

（Lua 的模块是由变量、函数等已知元素组成的 table，因此创建一个模块很简单，就是创建一个 table，
    然后把需要导出的常量、函数放入其中，最后返回这个 table 就行)
]]

-- 创建 MyModule.lua
do 
    MyModule={}                        --定义一个名为 MyModule 的模块 
    MyModule.constant = "const value"
    MyModule.defaultValue = {size = 34, content = "hello"}
    MyModule.mt = {}                    --创建元表

    function MyModule.func1()
        io.write("test func1\n")
    end

    local function  func2()             --外部不能访问
        print("test func2\n")
    end

    function MyModule.func3()
        func2()
    end

    function MyModule.new(tb)
        setmetatable(tb, MyModule.mt)
        return tb
    end

    --元方法 __index
    MyModule.mt.__index = function (tb, key)
        return MyModule.defaultValue[key]
    end

    return MyModule
end