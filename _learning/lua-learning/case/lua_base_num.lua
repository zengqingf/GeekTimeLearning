
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