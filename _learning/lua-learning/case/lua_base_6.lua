--[[
    lua传值 和 传引用（传地址）

    值传递，将值重新拷贝一份赋给新变量，即新变量拥有全新的内存地址，与原来的值无关联，改变新变量不会影响老变量的值
    引用传递，每次赋值时，只是将内存地址的引用赋值给新变量，新旧变量共同引用同一个值，指向同一块内存的值，改变新变量得值，即是改变了内存地址中的值，老变量中的值也跟着改变

    lua table属于引用传递，使用=只是浅拷贝，传递的只是引用
                          使用return返回table类型数据，返回的只是引用
                          传参时，函数内对形参table进行修改，会影响实参table
]]

do
    local function func_1()
        print("### first func")
    end
    
    local function func_2()
        print("### second func")
    end

    --值传递
    local x = func_1
    local y = x
    local x = func_2
    y()                 --output: first func
    x()                 --output: second func


    --引用传递（table） 形参table在函数内部的改变会导致实参table也跟着改变
    local mytable = {a=1, b="a"}
    local function test(table)
        table.a = 2
        table.b = 2
    end
    test(mytable)
    print(mytable.a)        --output: 2
    print(mytable.b)        --output: 2


    --深拷贝
    local function cloneTable(table)
        local function copy(target, res)
            for k,v in pairs(target) do 
                if type(v) ~= "table" then
                    res[k] = v
                else
                    res[k] = {}
                    copy(v, res[k])
                end
            end
        end

        local result = {}
        copy(table, result)
        return result
    end

    --测试
    local a = { pre = nil, next = nil }
    local b = { pre = nil, next = nil }
    a.pre = b
    b.next = a
    --local c = cloneTable(a)     --stack overflow

    --深拷贝
    local function clone(object)
        local table = {}
        local function _copy(object)
            if type(object) ~= "table" then
                return object
            elseif table[object] then
                return table[object]
            end

            local new_table = {}
            table[object] = new_table

            for k, v in pairs(object) do
                new_table[_copy(k)] = _copy(v)
            end
            return setmetatable(new_table, getmetatable(object))
        end
        return _copy(object)
    end
end

do
    local function Test(table1)
        --table1 = nil                --无法清空传入的table，参数table1为引用传递，=nil只是将指向实参table2内存地址的指针置空了
        while #table1 > 0 do           
            table.remove(table1, #table1)      --table remove遍历可以清空table
        end
    end

    local table2 = {[1] = "test"}
    Test(table2)
    for k, v in pairs(table2) do
        print(k, v)
    end
end