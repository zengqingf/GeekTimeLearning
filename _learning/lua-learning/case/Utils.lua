do 
    Utils={}                        --定义一个名为 Utils 的模块 
    Utils.constant = "Utility"

    -- https://www.runoob.com/lua/lua-metatables.html
    -- 计算表中最大值，table.maxn在Lua5.2以上版本中已废弃
    -- 自定义计算表中最大键值函数 table_maxn，即计算表的元素个数
    function Utils.table_max_key(table)
        local mn = 0
        for k, v in pairs(table) do
            if mn < k then
                mn = k
            end
        end
        return mn
    end

    function Utils.table_max_value(table)
        local mn = nil
        for k, v in pairs(table) do
            if mn == nil then
                mn = v
            end
            if mn < k then
                mn = v
            end
        end
        return mn
    end

    return MyModule
end