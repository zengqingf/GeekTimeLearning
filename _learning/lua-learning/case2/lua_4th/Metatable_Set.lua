--[[
    除了加法和乘法外，还有减法（__sub）、除法（__div）、floor除法（__idiv）、负数（__unm）、取模（__mod）和幂运算（__pow）。
    类似地，位操作也有元方法：按位与（__band）、按位或（__bor）、按位异或（__bxor）、按位取反（__bnot）、向左移位（__shl）和向右移位（__shr）。
    我们还可以使用字段__concat来定义连接运算符的行为。
]]
do
    Set = {}
    Set.TestCount = 10000
    local mt = {}
    
    function Set.new(l)
        local set = {}
        setmetatable(set, mt)
        for _, v in ipairs(l) do
            set[v] = true
        end
        return set
    end
    
    function Set.union(a, b)

        if getmetatable(a) ~= mt or getmetatable(b) ~= mt then
            error("attempt to 'add' a set with a non-set value", 2)
        end

        local res = Set.new{}           --注意是 {} 也可以用 ({})
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



    mt.__add = Set.union
    mt.__mul = Set.intersection

    --[[
        集合中， <= 表示集合包含， a <= b 表示 a是b的一个子集，根据部分有序的定义，a<=b和b<a可能同时为假
        需要自定义实现 __le（子集关系） 和  __lt （真子集关系）
    ]]
    mt.__le = function(a, b)                --子集
        for k in pairs(a) do
            if not b[k] then
                return false
            end
        end
        return true
    end

    mt.__lt = function(a, b)                --真子集
        return a <= b and not(b <= a)
    end

     --[[相等比较有一些限制。如果两个对象的类型不同，
        那么相等比较操作不会调用任何元方法而直接返回false。
        因此，不管元方法如何，集合永远不等于数字。]]
    mt.__eq = function(a, b)                --集合相等
        return a <= b and b <= a
    end
    
    return Set 
end