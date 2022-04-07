
--获取数值保留几位小数后的字符串，向下取整
---@param num number 计算后的数值
---@param n number 需要保留的小数位数，默认保留1为小数
---@return number
function GetNumStrByKeepDecimalFloor(num, n)
    if type(num) ~= "number" then
        return num
    end
    n = n or 1
    if n < 0 then
        n = 0
        return tostring(math.floor(num))                    --保留整数 math.floor(-0.1) ==> -1
    end
    local tmp1, tmp2 = math.modf(num)                       --取小数位
    local tmp2L = tmp2 % (0.1 ^ n)                          --取精确到小数位后剩余的值
    if math.abs(tmp2) == 0 then                             --如果小数位为0时
        local rate = 10 ^ n
        return tostring(math.floor(num * rate + 0.5) / rate)
    else
        if math.abs(tmp2L) >= 0.0999 then                    --如果精确到的小数位后剩余的值 为0 即用0.1取余后近似到0.1
            local rate = 10 ^ n
            return tostring(math.floor(num * rate + 0.5) / rate)
        elseif math.abs(tmp2L) >= (0.4999 * (0.1 ^ n)) then  --如果精确到的小数位后剩余的值 大于等于 0.5，不要五入
            return tostring(num - num % (0.1 ^ n))
        else
            local rate = 10 ^ n
            return tostring(math.floor(num * rate + 0.5) / rate)
        end
    end
    --[[
    local keepDecimal = "%." .. tostring(n) .. "f"          --%.0f 不保留小数
    return string.format(keepDecimal, num)
    ]]
end

--四舍五入
function math.getPreciseDecimalRound(nNum, n)
    if type(nNum) ~= "number" then
        return nNum
    end
	n = n or 0;
	n = math.floor(n)
	if n < 0 then
        n = 0;
    end
	local format = "%." .. n .. "f"
	local nRet = tonumber(string.format(format, nNum))
    return nRet
end

--精确小数位数（去末尾0）,不四舍五入
--[[
    不使用 求余方法
]]
function math.getPreciseDecimalFloor(nNum, n)
    if type(nNum) ~= "number" then
        return nNum
    end
    n = n or 0
    n = math.floor(n)
    if n < 0 then
        n = 0
    end
    local nDecimal = 10 ^ n
    local nTemp = math.floor(nNum * nDecimal)
    local nRet = nTemp / nDecimal
    return nRet
end

--[[
    该方法错误，
    在lua中 0.7 % 0.1 = 0.1
    导致结果错误
]]
function error.getPreciseDecimalFloor(nNum, n)
    if type(nNum) ~= "number" then
        return nNum;
    end
    n = n or 0;
    n = math.floor(n)
    if n < 0 then
        n = 0;
    end
    local nDecimal = 1/(10 ^ n)
    if nDecimal == 1 then
        nDecimal = nNum;
    end
    local nLeft = nNum % nDecimal;
    return nNum - nLeft;
end

-- 转换数值
--[[
	转换规则：
	7位数以下全显示  X,XXX,XXX
	8位数显示 XX,XXX K
	9-12位数 XXX,XXX M
	如果最终显示的 数字少于 5 位，就保留2位小数，否则不保留
--]]
function ConvertNumber(num,exactvalue)
	num = tonumber(num)
	exactvalue = exactvalue or 0;
    if num < 0 then
        return "0"
    end
    local newNum = ""
    -- 小于五位数
    if num < 10000 then
        newNum = math.getPreciseDecimalFloor(num,exactvalue)
    elseif num < 10000000 then
        num = math.floor(num)
        newNum = tostring(num)
    else
        local len = string.len(math.floor(num))
        -- 八位数
        if len == 8 then
            num = num / 1000
            -- 小于五位 保留两位小数
            if string.len(math.floor(num)) < 5 then
                num = math.getPreciseDecimalFloor(num,exactvalue)
            else
                num = math.floor(num)
            end
            newNum = num .. " K"
        else
            num = num / 1000000
            if string.len(math.floor(num)) < 5 then
                num = math.getPreciseDecimalFloor(num,exactvalue)
            else
                num = math.floor(num)
            end
            newNum = num .. " M"
        end
    end
    return newNum
end

--大数格式化输出
function math.bigNumberToShow(number)
	if number == nil then
		print("数字格式错误")
	else
		if number / 10^8 >1 then
			number = math.floor(number / 10^6)
			return(string.format("%.2f", number/10^2).."亿")
		elseif number / 10^5 > 1 then
			number = math.floor(number / 10^2)
			return(string.format("%.2f", number/10^2).."万")
		else
			return number
		end
	end
end


--[[取余，取整]]
do

    --向上取整
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
    print(tmp1, tmp2)       --tmp1是整数部分,tmp2是小数部分,小数部分返回的float类型

    local line = math.modf(10 / 3)      --取整数
    local mod = math.fmod(10, 3)        --取余数
    print(line)                 -->3
    print(mod)                  -->1

    print(string.format("%.0f", 2.2 * 100))  --output: 220
    print(string.format("%.0f", 2.233 * 100))  --output: 223
    print(string.format("%.2f", 3.1415926))  --output: 3.14

    --四舍五入
    print(string.format("%.1f", 2.24))  --output: 2.2
    print(string.format("%.1f", 2.25))  --output: 2.2
    print(string.format("%.1f", 2.26))  --output: 2.3

    --保留整数，直接舍去小数
    print(math.floor(2.24))  --output: 2
    print(math.floor(2.25))  --output: 2
    print(math.floor(2.26))  --output: 2
    
    --error
    --print(string.format("%d", 2.4)) --output: 2
    --print(string.format("%d", 2.5)) --output: 2
    --print(string.format("%d", 2.6)) --output: 2

    --保留几位小数的四舍五入
    --[[
        公式为：math.floor(x * num+ 0.5) / num
        保留2位 num为100， 3位为1000，…
    ]]
    print(math.floor(2.24 * 10 + 0.5) / 10) --output: 2.2
    print(math.floor(2.25 * 10 + 0.5) / 10) --output: 2.3
    print(math.floor(2.26 * 10 + 0.5) / 10) --output: 2.3
    print(math.floor(2.26 + 0.5)) --output: 2
    print(math.floor(2.4 + 0.5)) --output: 2
    print(math.floor(2.5 + 0.5)) --output: 3
    print(math.floor(2.6 + 0.5)) --output: 3

    --保留几位小数，不四舍五入
    print(2.244 - 2.244 % 0) --output: nan
    print(2.244 - 2.244 % 1) --output: 2.0
    print(2.244 - 2.244 % 0.1) --output: 2.2
    print(2.244 - 2.244 % 0.01) --output: 2.24
    print(2.254 - 2.254 % 0.1) --output: 2.2
    print(2.254 - 2.254 % 0.01) --output: 2.25
    print(2.264 - 2.264 % 0.1) --output: 2.2
    print(2.264 - 2.264 % 0.01) --output: 2.26
    print(2.266 - 2.266 % 0.1) --output: 2.2
    print(2.266 - 2.266 % 0.01) --output: 2.26

    print(123.0 - 123.0 % 0.1) --output: 122.9
    print(123.00 - 123.00 % 0.01) --output: 122.99

    local x1 = 123.0
    local tmp1, tmp2 = math.modf(x1)
    if  math.abs(tmp2) == 0 then
        print(math.floor(x1 * 10 + 0.5) / 10)       --output: 123.0
    else
        print(x1 - x1 % 0.1)                
    end 

    local x1 = -0.153
    local tmp1, tmp2 = math.modf(x1)
    if math.abs(tmp2) == 0 then
        print(math.floor(x1 * 10 + 0.5) / 10)
    else
        if x1 < 0 then    
            print(-(math.abs(x1) - math.abs(x1) % 0.1))  --output: -0.1
        else
            print(x1 - x1 % 0.1)                        
        end 
    end 

    local x1 = -0.00
    local tmp1, tmp2 = math.modf(x1)
    if  math.abs(tmp2) == 0 then
        print(math.floor(x1 * 10 + 0.5) / 10)      --output: 0.0
    else
        print(x1 - x1 % 0.1)                
    end 


    print(math.floor(-1))                     --output: -1  
    print(math.floor(-math.abs(0.1)))         --output: -1
    print(math.floor(-0.1))                   --output: -1
    print(math.floor(-0.5))                   --output: -1

    print((2.5 * 100 / 10 - (2.5 * 100 / 10) % 0.1) / 10)
    local num = 4.5444
    local n = 1
    n = n or 1
    if n < 0 then
        n = 0
        print(tostring(math.floor(num)))                    --保留整数 math.floor(-0.1) ==> -1
    end
    local tmp1, tmp2 = math.modf(num)
    print(tmp2)
    local tmp2L = tmp2 % (0.1 ^ n)
    if math.abs(tmp2) == 0 then
        local rate = 10 ^ n
        print(tostring(math.floor(num * rate + 0.5) / rate))
    else
        print(math.abs(tmp2L))
        if math.abs(tmp2L) >= 0.0999 then
            print(1)
            local rate = (10 ^ n)
            print(math.floor(num * rate + 0.5) / rate)
        elseif math.abs(tmp2L) >= (0.4999 * (0.1 ^ n)) then
            print(2)
            print(tostring(num - num % (0.1 ^ n)))
        else
            print(3)
            local rate = (10 ^ n)
            print(math.floor(num * rate + 0.5) / rate)
        end
    end

    print(GetNumStrByKeepDecimalFloor(4.435, 2))

    print(2.5 - 2.5 % 0.1) --output: 2.4
    print(math.floor(2.66 * 10 + 0.5) / 10) --output: 2.7
    print(math.floor(2.65 * 10 + 0.5) / 10) --output: 2.7
    print(math.floor(2.64 * 10 + 0.5) / 10) --output: 2.6
    print(math.floor(2.6 * 10 + 0.5) / 10) --output: 2.6

    print(math.getPreciseDecimalFloor(4.53, 1)) --output: 4.5
    print(math.getPreciseDecimalFloor(4.56, 1)) --output: 4.5
    print(math.getPreciseDecimalFloor(4.50, 1)) --output: 4.5
    print(math.getPreciseDecimalFloor(4.0, 1))  --output: 4.0
    print(math.getPreciseDecimalFloor(4.0, 0))  --output: 4.0
    print(math.getPreciseDecimalFloor(4.54, 0))  --output: 4.0
    print(math.getPreciseDecimalFloor(4.8, 0))  --output: 4.0
    print(math.getPreciseDecimalFloor(4.8, 1))  --output: 4.8

    --[[
        结论：
        四舍五入：
            返回string  用 string.format("%.1f", 2.24)
            返回number 用 math.floor(2.24 * (10 ^ n) + 0.5) / (10 ^ n)

        不四舍五入：
            用 math.floor(nNum * (10 ^ n)) / (10 ^ n)
    ]]
end


--[[
    lua版本 对于10和10.0的差异
]]

---如果小数位数为0，则只保留整数
function math.formatNum (num)
	if num <= 0 then
		return 0
	else
		local t1, t2 = math.modf(num)
		---小数如果为0，则去掉
		if t2 > 0 then
			return num
		else
			return t1
		end
	end
end

--[[
浮点数精度问题：https://blog.csdn.net/qq_39574690/article/details/113632077
例如：local number = 0,   一直加一个分数例如 1/3 ，现实情况中 1/3 * 3 = 1，但是计算机会识别为 1/3 = 0.3333333...  然后这3个数相加，得出0.9999999999... 因此 1/3 * 3 近似等于1
此时如果你代码写了  number >= 1.0  是不成立的， 但是你打印的number 却是 1.0， 因为lua的打印会将 0.9999999...四舍五入为 1.0 打印出来给你看。
真实数据打印方法可以用   math.floor(number * 10000000) / 10000000 之类的方法，即先乘以一个大整数，再取整，再除以大整数，即可得到真实浮点数。
number >= 1.0 不成立，但是可以用 number >= 0.99 这样子解决问题，0.9999999.... 肯定大于等于 0.99。
]]
do
    print(100 / 10)                 --5.3以上输出10.0， 5.1输出10
    print(math.formatNum(100/10))   --修正后输出10
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
