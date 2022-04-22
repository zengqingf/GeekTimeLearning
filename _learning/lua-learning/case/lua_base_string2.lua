
--[[
    字符串拼接

ref: https://drecik.top/2018/06/16/11/

日常使用情况下都不应该使用table.concat函数来进行拼接字符串，
因为每次拼接都会有创建table的性能消耗，并且也会带来gc压力

如果使用number比较多的情况下的拼接的字符串，使用string.format性能比..好，
反之，如果number较少使用..性能更好；

测试发现，日常情况下（总拼接参数个数小于8个），
如果number数量:string数量<=1的情况下，使用..性能会好点，其他情况下使用string.format

在已经存在的table去调用字符串拼接，不论table里面元素多少，都是table.concat快，因为没有了table创建的开销

大量字符串拼接时，
先将字符串放入一个表里，然后调用table.concat一次性将他们串连起来
]]
do
    -- 随机生成字符串备用
    local randomStringSzie = 200000
    local strTable = {}
    local function makeStrTable()
        for i=1, randomStringSzie do
            strTable[#strTable + 1] = tostring(math.random(1, randomStringSzie*10))
        end
    end

    -- 辅助函数，用来打印时间
    local function showtime(f)
        local start = os.clock()
        f()
        print(os.clock() - start)
    end

    local testCount = 50000

    -- string.format
    local function test1_1_0()
        io.write("test1_1_0: use string.format: ")
        for i=1, testCount do
            local str = string.format("%s-string1-%s-string2-%s", strTable[math.random(1, randomStringSzie)], 
                        strTable[math.random(1, randomStringSzie)],
                        strTable[math.random(1, randomStringSzie)])
        end
    end

    -- ..
    local function test1_2_0()
        io.write("test1_2_0: use ..:            ")
        for i=1, testCount do
            local str = strTable[math.random(1, randomStringSzie)] .. "-string1-" ..
                        strTable[math.random(1, randomStringSzie)] .. "-string2-" ..
                        strTable[math.random(1, randomStringSzie)]
        end
    end

    -- table.concat
    local function test1_3_0()
        io.write("test1_3_0: use table.concat:  ")
        for i=1, testCount do
            local str = table.concat({strTable[math.random(1, randomStringSzie)], "-string1-",
                                    strTable[math.random(1, randomStringSzie)], "-string2-",
                                    strTable[math.random(1, randomStringSzie)]})
        end
    end

    -- string.format with number
    local function test1_1_1()
        io.write("test1_1_1: use string.format: ")
        for i=1, testCount do
            local str = string.format("%d-string1-%d-string2-%d",
                            math.random(1, randomStringSzie),
                            math.random(1, randomStringSzie),
                            math.random(1, randomStringSzie))
        end
    end

    -- ..   with number
    local function test1_2_1()
        io.write("test1_2_1: use ..:            ")
        for i=1, testCount do
            local str = math.random(1, randomStringSzie) .. "-string1-" ..
                        math.random(1, randomStringSzie) .. "-string2-" ..
                        math.random(1, randomStringSzie)
        end
    end

    -- table.concat with number
    local function test1_3_1()
        io.write("test1_3_1: use table.concat:  ")
        for i=1, testCount do
            local str = table.concat({math.random(1, randomStringSzie), "-string1-",
                                    math.random(1, randomStringSzie), "-string2-",
                                    math.random(1, randomStringSzie)})
        end
    end

    makeStrTable()

    collectgarbage("collect")       -- 防止gc影响，先清除一遍
    showtime(test1_1_0)

    collectgarbage("collect")        -- 防止gc影响，先清除一遍
    showtime(test1_2_0)

    collectgarbage("collect")        -- 防止gc影响，先清除一遍
    showtime(test1_3_0)

    collectgarbage("collect")       -- 防止gc影响，先清除一遍
    showtime(test1_1_1)

    collectgarbage("collect")        -- 防止gc影响，先清除一遍
    showtime(test1_2_1)

    collectgarbage("collect")        -- 防止gc影响，先清除一遍
    showtime(test1_3_1)
end


--[[
    已存在的table 进行字符串拼接，
    
    使用table.concat快，因为没有了table创建开销
]]
do
    math.randomseed(os.time())

    local strTable = {}
    local function makeStrTable()
        for i=1,200000 do
            strTable[#strTable + 1] = tostring(math.random(1,10000000))
        end
    end

    local maxCount = 50000

    local function test2_1(arr, len)
        io.write("test2_1: ")

        local start = os.clock()
        
        for cnt=1, maxCount do
            local str = ""
            for i=1, len do
                str = string.format("%s%s", str, arr[i])
            end
        end

        print( os.clock() - start)
    end

    local function test2_2(arr, len)
        io.write("test2_2: ")

        local start = os.clock()
        
        for cnt=1, maxCount do
            local str = ""
            for i=1, len do
                str = str..arr[i]
            end
        end

        print( os.clock() - start)
    end

    local function test2_3(arr, len)
        io.write("test2_3: ")

        local start = os.clock()
        
        for cnt=1, maxCount do
            local str = table.concat(arr)
        end

        print( os.clock() - start)
    end

    makeStrTable()

    for i = 2, 10 do
        local concatTable = {}
        for j=1, i do
            concatTable[#concatTable + 1] = strTable[math.random(1,200000)]
        end
        collectgarbage("collect")
        test2_1(concatTable, i)
        collectgarbage("collect")
        test2_2(concatTable, i)
        collectgarbage("collect")
        test2_3(concatTable, i)
    end
end

--[[
    从某个table选取一些元素进行拼接，
    需要创建一个table并拼接

可以看到..的性能一直会高于string.format
（这里假设table里面的元素都是字符串的情况下，如果为全数字的情况经过测试string.format性能高于..）
当table长度在29的时候（全数字情况下测试为36），table.concat性能最好，
这个数字不一定准确，但是可以大致了解下这个分界值，
而且使用string.format与..会带来后续gc的性能影响（这个也比较难测），
所以可以大致把这个值估计在25（纯数字的情况下，可以稍微再高点），
如果超过这个值，就是用table.concat会更优，写起来也会更加方便
]]
do
    math.randomseed(os.time())

    local strTable = {}
    local function makeStrTable()
        for i=1,200000 do
            strTable[#strTable + 1] = tostring(math.random(1,10000000))
        end
    end

    local maxCount = 50000

    local function test2_1(arr, len)
        io.write("test2_1: len: " .. len .. ": ")

        local start = os.clock()
        
        for cnt=1, maxCount do
            local str = ""
            for i=1, len do
                str = string.format("%s%s", str, arr[i])
            end
        end

        print(os.clock() - start)
    end

    local function test2_2(arr, len)
        io.write("test2_2: len: " .. len .. ": ")

        local start = os.clock()
        
        for cnt=1, maxCount do
            local str = ""
            for i=1, len do
                str = str..arr[i]
            end
        end

        print(os.clock() - start)
    end

    local function test2_3(arr, len)
        io.write("test2_3: len: " .. len .. ": ")

        local start = os.clock()
        
        for cnt=1, maxCount do
            local tab = {}
            for i=1, len do
                tab[#tab+1] = arr[i]
            end

            local str = table.concat(tab)
        end

        print(os.clock() - start)
    end

    makeStrTable()

    --[[
    for i = 2, 50 do
        local concatTable = {}
        for j=1, i do
            concatTable[#concatTable + 1] = strTable[math.random(1,200000)]
        end
        collectgarbage("collect")
        test2_1(concatTable, i)
        collectgarbage("collect")
        test2_2(concatTable, i)
        collectgarbage("collect")
        test2_3(concatTable, i)
    end
    ]]
end

--[[
    一般使用分析：  .. vs. table.concat

    ref: https://blog.csdn.net/qq_26958473/article/details/79392222

1. 使用运算符..
每次拼接都需要申请新的空间，旧的 result 对应的空间会在某时刻被Lua的垃圾回收期GC，
    且随着result不断增长，越往后会开辟更多新的空间，并进行拷贝操作，产生更多需要被GC的空间，所以性能降低。
2. 使用 table.concat (table [, sep [, start [, end]\]\]) 函数
--table.concat 底层拼接字符串的方式也是使用运算符.. ，但是其使用算法减少了使用运算符..的次数，减少了GC，从而提高效率。
    主要思路：采用二分思想，用栈存储字符串，新入栈的字符串与下方的字符串比较长度，大于则使用运算符..拼接成新字符串，并移除栈顶的字符串，不断向下直至遇到长度更大的字符串或者栈底，
    这样保持最大的字符串位于栈底，栈呈现金字塔的形状，最终在使用运算符..将栈中的字符串拼接成最终的字符串。
]]
do
    local function operatorConcat(str,count)
        local result = ""
        for i=1,count do
            result = result .. str
        end
        return result
    end
    
    local function tableConcat(str,count)
        local tbl = {}
        for i=1,count do
            table.insert( tbl,str)
        end
        return table.concat(tbl)
    end
    
    
    local str = "a"
    local count = 100000
    
    local start_time = os.clock()
    operatorConcat(str,count)
    print("operatorConcatTime:" .. os.clock() - start_time)     --0.226
    
    start_time = os.clock()
    tableConcat(str,count)
    print("tableConcatTime:" .. os.clock() - start_time)        --0.006
end