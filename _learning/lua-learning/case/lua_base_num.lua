
--[[取余，取整]]
do

    --取整
    print(math.ceil(220.5)) --output: 221
    print(math.ceil(2.2 * 100)) --output: 221
    print(math.ceil(12.0))      --output: 12
    print(math.ceil(12.2))      --output: 13
    print(math.ceil(12.7))      --output: 13

    --取余操作
    print(3.14 % 1)             --小数部分
    print(3.14 - 3.14 % 1)      --整数部分

    --[[
        使用string格式化方式
        如果需要的结果是字符串，则可以使用string.format()函数
        保留2位小数：string.format(“%.2f”, x)

        使用数字计算方式
        使用%运算符，得到的结果是数字
        x%1 表示x的小数部分，x-x%1 表示x的整数部分。
        类似的，x-x%0.01 将x精确到小数点后2位。
    ]]
    local x = 3.1415926
    print(string.format("%.2f", x))           
    print(x-x%1)
    print(x-x%0.01)

    print(8 % 3)
    print(1 % 2)
    print(math.floor(8 % 3))    --output：2
    print(math.floor(1 % 2))    --output: 1     返回不大于 x 的最大整数值

    print(8 / 3)
    local tmp1, tmp2 = math.modf(8 / 3)
    print(tmp1, tmp2)       --tmp1是整数部分,tmp2是小数部分

    local line = math.modf(10 / 3)      --取整数
    local mod = math.fmod(10, 3)        --取余数
    print(line)                 -->3
    print(mod)                  -->1

    print(string.format("%.0f", 2.2 * 100))  --output: 220
    print(string.format("%.2f", 3.1415926))  --output: 3.14
    
end

--[[
    lua位运算
    ref: https://blog.csdn.net/zuimrs/article/details/81104092
]]
do
    --lua 5.1
    -- 引入bit库  bitlib
    --[[
    require "bit"
    -- and操作
    bit.band(a,b)
    -- or操作
    bit.bor(a,b)
    -- xor操作
    bit.bxor(a,b)
    -- not操作
    bit.bnot(a,b)
    -- 左移n位
    bit.lshift(a,n)
    -- 右移n位
    bit.rshift(a,n)
    ]]

    --lua5.2
    -- 引入内置bit32库
    --[[
    require "bit32"

    -- and操作
    bit32.band(a,b)
    -- or操作
    bit32.bor(a,b)
    -- xor操作
    bit32.bxor(a,b)
    -- not操作
    bit32.bnot(a,b)
    -- 左移n位
    bit32.lshift(a,n)
    -- 右移n位
    bit32.rshift(a,n)
    ]]

    --lua5.3
    --[[
    -- and操作
    a & b
    -- or操作
    a | b
    -- xor操作
    a ~ b
    -- not操作
    ~a
    -- 左移n位
    a << n
    -- 右移n位
    a >> n
    ]]
end