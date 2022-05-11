--[[
string
    使用\[\[\]\] 多行字符串
    对数字字符串进行算术操作，lua会尝试将字符串转成一个数字
]]
print("################ string start ###############")


--region UTF8 String Sub


--[[
UTF-8 变长编码：
一字节：0*******                                                   (0, 191]
两字节：110*****，10******                                         [192, 223]
三字节：1110****，10******，10******                               [224, 239]
四字节：11110***，10******，10******，10******                     [240, 247]
五字节：111110**，10******，10******，10******，10******            [248, 251]
六字节：1111110*，10******，10******，10******，10******，10******  [252, 253]
想判断UTF8字符的byte长度，只需要获取该字符的首个Byte，根据其值就可以判断出该字符由几个Byte表示
]]

--region UTF8 String Sub

--返回当前字符实际占用的字符数
local function subStringGetByteCount(str, index)
    local curByte = string.byte(str, index)
    local byteCount = 1;
    if curByte == nil then
        byteCount = 0
    elseif curByte > 0 and curByte <= 127 then
        byteCount = 1
    elseif curByte>=192 and curByte<=223 then
        byteCount = 2
    elseif curByte>=224 and curByte<=239 then
        byteCount = 3
    elseif curByte>=240 and curByte<=247 then
        byteCount = 4
    elseif curByte>=248 and curByte<=251 then
        byteCount = 5
    elseif curByte>=252 then
        byteCount = 6
    end
    return byteCount;
end

local function subStringGetTrueIndex(str, index)
    local curIndex = 0;
    local i = 1;
    local lastCount = 1;
    repeat 
        lastCount = subStringGetByteCount(str, i)
        i = i + lastCount;
        curIndex = curIndex + 1;
    until(curIndex >= index);
    return i - lastCount;
end

--获取中英混合UTF8字符串的真实字符数量
local function subStringGetTotalIndex(str)
    local curIndex = 0;
    local i = 1;
    local lastCount = 1;
    repeat 
        lastCount = subStringGetByteCount(str, i)
        i = i + lastCount;
        curIndex = curIndex + 1;
    until(lastCount == 0);
    return curIndex - 1;
end

--截取中英混合的UTF8字符串，endIndex可缺省
local function subStringUTF8(str, startIndex, endIndex)
    if startIndex < 0 then
        startIndex = subStringGetTotalIndex(str) + startIndex + 1;
    end

    if endIndex ~= nil and endIndex < 0 then
        endIndex = subStringGetTotalIndex(str) + endIndex + 1;
    end

    if endIndex == nil then 
        return string.sub(str, subStringGetTrueIndex(str, startIndex));
    else
        return string.sub(str, subStringGetTrueIndex(str, startIndex), subStringGetTrueIndex(str, endIndex + 1) - 1);
    end
end
--endregion

--中英文字符串截取 ref: https://www.cnblogs.com/slysky/p/6098981.html
local function charsize(ch)
    if not ch then return 0
        elseif ch >=252 then return 6
        elseif ch >= 248 and ch < 252 then return 5
        elseif ch >= 240 and ch < 248 then return 4
        elseif ch >= 224 and ch < 240 then return 3
        elseif ch >= 192 and ch < 224 then return 2
        elseif ch < 192 then return 1
    end
end
-- 计算utf8字符串字符数, 各种字符都按一个字符计算
-- 例如utf8len("1你好") => 3
local function utf8len(str)
    local len = 0
    local aNum = 0 --字母个数
    local cNum = 0 --汉字个数
    local currentIndex = 1
    while currentIndex <= #str do
        local char = string.byte(str, currentIndex)
        local cs = charsize(char)
        currentIndex = currentIndex + cs
        len = len +1
        if cs == 1 then 
            aNum = aNum + 1
        elseif cs >= 2 then 
            cNum = cNum + 1
        end
    end
    return len, aNum, cNum
end
-- 截取utf8 字符串
-- str:            要截取的字符串
-- startChar:    开始字符下标,从1开始
-- numChars:    要截取的字符长度
local function utf8sub(str, startChar, numChars)
    local startIndex = 1
    while startChar > 1 do
        local char = string.byte(str, startIndex)
        startIndex = startIndex + charsize(char)
        startChar = startChar - 1
    end

    local currentIndex = startIndex

    while numChars > 0 and currentIndex <= #str do
        local char = string.byte(str, currentIndex)
        currentIndex = currentIndex + charsize(char)
        numChars = numChars -1
    end
    return str:sub(startIndex, currentIndex - 1)
end

--[[
do
    print("----------------------------------------------------")
    local input = "中国a"
    local output = ""
    local tempTable = nil
    local uchar = nil
    for uchar in string.find(string.sub(input, 1), '[%z\1-\127\194-\244][\128-\191]*') do
        tempTable[#tempTable+1] = uchar
    end
    for j=1,200,1 do
        if tempTable[j]~=nil then
            output = output .. tempTable[j]
        end
    end
    print(output)
    print("----------------------------------------------------")
end
]]

do
    print("----------------------------------------------------")
        -- test utf8len
        assert(utf8len("你好1世界哈哈") == 7)
        assert(utf8len("你好世界1哈哈 ") == 8)
        assert(utf8len(" 你好世 界1哈哈") == 9)
        assert(utf8len("12345678") == 8)
        assert(utf8len("øpø你好pix") == 8)
    
        -- test utf8sub
        assert(utf8sub("你好1世界哈哈",2,5) == "好1世界哈")
        assert(utf8sub("1你好1世界哈哈",2,5) == "你好1世界")
        assert(utf8sub(" 你好1世界 哈哈",2,6) == "你好1世界 ")
        assert(utf8sub("你好世界1哈哈",1,5) == "你好世界1")
        assert(utf8sub("12345678",3,5) == "34567")
        assert(utf8sub("øpø你好pix",2,5) == "pø你好p")

        print("all test succ")
    print("----------------------------------------------------")
end


do
    print("----------------------------------------------------")
    local html = [[
    <html>
    <head></head>
    <body>
        <a href="http://www.baidu.com/">baidu</a>
    </body>
    </html>
    ]]

    print("2" + 3)
    print("2" + "4")
    print("2 + 4")
    print("-2e2" + "4")
    --! print("error" + 1)  --会报错
    --字符串拼接用 ..
    print("error" .. 1)
    -- 字符串长度
    local strLen = "www.google.com"
    print(#strLen)
    print(#"www.google.com")


    --内置字符串函数
    print(string.gsub("aaaa", "a", "z", 3))    --> zzza 3
    print(string.find("hello world", "wo", 1)) --> 7    8
    print(string.reverse("lua")) --> aul
    print(string.format("the value is : %d", 4))
    print(string.char(97, 98, 99, 100))  -->  abcd
    print(string.byte("ABCD", 4))        -->  68  D
    print(string.byte("ABCD"))           -->  65  A
    print(string.len("abc"))             --> 3
    print(string.rep("abcd", 2))         --> abcdabcd

    print("----------------------------------------------------")

    do 
        for word in string.gmatch("hello lua world", "%a+")
        do
            print(word)
        end
    end
    --如果 pattern 中单个匹配模式没有用小括号包含的话，最后会返回完整匹配的字符串  这样会导致只输出一个
    --如果 pattern中 每个匹配模式用小括号包含的话，最后会按照多返回值的方式
    print(string.format("%d, %q", string.match("I have 2 questions for you.", "(%d+) (%a+)")))

    -- 字符串匹配细节：https://www.runoob.com/lua/lua-strings.html
    do
        local s = "Deadline is 08/10/2021, tm"
        local date = "%d%d/%d%d/%d%d%d%d"
        print(string.sub(s, string.find(s, date)))    --> 08/10/2021    //string.sub(s, start, end)  end默认为-1 即最后一个字符
    end

    -- %s 与空白字符配对          %a 与任何字母配对
    -- %S 与任何非空白字符配对     %A 非字母的字符
    print(string.gsub("hello, up-down!", "%A", "."))     --> hello..up.down.   4 (表示替换4次，用.替换非字母字符)

    print("----------------------------------------------------")

    -- 示例
    do 
        print("----------------------------------------------------")
        local function numToCN(num)
            local size = #tostring(num)
            local CN = ""
            local StrCN = {"一","二","三","四","五","六","七","八","九"}
            for i = 1 , size do
                local index = tonumber(string.sub(tostring(num), i , i))
                local tempStrCN = ""
                if index == 0
                then
                    tempStrCN = "零"
                else
                    tempStrCN = StrCN[index]
                end
                CN = CN .. tempStrCN
            end
            return CN
        end
        print(numToCN(10103796))

        local function StrSplit(inputstr, sep)
            if sep == nil then
            sep = "%s"
            end
            local t={}
            local i=1
            for str in string.gmatch(inputstr, "([^"..sep.."]+)") do
            t[i] = str
            i = i + 1
            end
            return t
        end
        local a = "23245023496830,汉字。。。。"
        local b = ":"
        b = StrSplit(a,",")
        print(b[1])
        print(b[2])

        local function trim(s)
            return (string.gsub(s, "^%s*(.-)%s*$", "%1")) 
        end
        local string1 = "   Baidu        "
        local string2 = trim(string1)
        print(string2)

        --string.format
        print(string.format("%.0f", 2.2 * 100))  --output: 220
        print(string.format("%.2f", 3.1415926))  --output: 3.14
        --!print(string.format("%d", 17.325))       --output: 17

        print("----------------------------------------------------")
    end


    -- ref:https://www.shuzhiduo.com/A/nAJv46madr/  中文字符截取问题
    do
        print("----------------------------------------------------")
        --UTF-8编码，一个中文占3个字节
        local a1 = "你好啊a"
        
        print(string.byte(a1, 0, -1))--第1到第4个字节
        print(string.len(a1))--字节总数
        local startIndex, endIndex = string.find(a1, "你好")
        print(startIndex .. " " .. endIndex)--第1到第6个字节
        
        local test = "泰"
        local test2 = "法?"
        
        print(string.len(test))
        print(string.byte(test, 0, -1))
        print(string.byte(test2, 0, -1))
        
        --string.gsub的第二个参数为正则表达式，?表示匹配0个至1个
        --字节230179176中的230179被替换成989898
        local str = string.gsub(test, test2, function()
            print("gsub success!")
            return "bbb"
        end)
        print(str)
        print(string.byte(str,0,-1))
        print(string.byte("b"))
        print("----------------------------------------------------")
    end

    do
        print("----------------------------------------------------")
        --为了方便输出中文，这里使用ANSI编码
        --在ANSI编码中，1个中文占2个字节
        local test = "泰ab"
        local result
        
        print(string.byte(test, 0, -1))--泰:230179176 a:97 b:98
        print(type(string.byte(test, 0, -1)))--数字
        
        --string.gsub 逐字节匹配
        print("1.")
        result = string.gsub(test, "[204169]", "c")                     --应该是老版本lua的编码库的中文编码序号不同
        print(result)--[204169]:2,0,4,1,6,9的集合，因此匹配失败

        print("1.1.")
        result = string.gsub(test, "[230179176]", "c")
        print(result)--匹配失败
        
        print("2.")
        result = string.gsub(test, "[\204169]", "c")
        print(result)
        print(string.byte(result, 0, -1))--第1个字节204匹配失败
        print(string.byte("゛", 0, -1))--c:99 ゛:16997 b:98

        print("2.1.")
        result = string.gsub(test, "[\230179176]", "c")
        print(result)--匹配成功1次
        
        print("3.")
        result = string.gsub(test, "[\204\169]", "c")
        print(result)--匹配失败

        print("3.1.")
        result = string.gsub(test, "[\230\179\176]", "c")
        print(result)--匹配成功3次
        
        print("4.")
        result = string.gsub(test, "[\204][\169]", "c")
        print(result)--匹配成功失败，
        print("4.1.")
        result = string.gsub(test, "[\230][\179][\176]", "c")
        print(result)--匹配成功1次，将原字符串中的中文替换了
        print("----------------------------------------------------")
    end

    do
        --中文处理方式
        print("----------------------------------------------------")
        --获取字符数
        function GetWordCount(str)
            local _,count = string.gsub(str, "[^\128-\193]", "")
            return count
        end

        --将字符串转为table
        function GetWordTable(str)
            local temp = {}
            for uchar in string.gmatch(str, "[%z\1-\127\194-\244][\128-\191]*") do
                local index = #temp + 1
                temp[index] = uchar
            end
            return temp
        end

        --utf8
        local test = "泰ab好了."
        print(GetWordCount(test))
        local testT = GetWordTable(test) --%z:匹配0 *:表示0个至任意多个
        for i=1, #testT do
            print(testT[i])
        end
        print("----------------------------------------------------")
    end


    do
        --[[
            敏感词匹配：
            敏感字的处理主要体现在取名、聊天上，如果字符串中含有敏感字，则需要将其替换成“*”。一开始我使用的string.gsub方法，
            但是发现敏感字中有不少是带有特殊符号，从而使整个字符串变成了一个正则表达式了，发生了正则匹配的错误，而正确的做法应该是直接跟敏感字进行对比。
            后来采用的是string.find方法，因为它可以关闭正则匹配。
        ]]
        print("----------------------------------------------------")
        local sensitiveWordConfig = {"法?"};

        function GetWordCount(str)
            local _, count = string.gsub(str, "[^\128-\193]", "")
            return count;
        end
        
        --内部接口：将字符串中的敏感字替换成*(替换一个)
        function ReplaceSensitiveWord(originStr, sensitiveWord)
            local resultStr = originStr;
            --1:从索引1开始搜索 true:关闭模式匹配
            local startIndex, endIndex = string.find(originStr, sensitiveWord, 1, true);
            if (startIndex and endIndex) then
                local strLen = string.len(originStr);
                local maskWordCount = GetWordCount(sensitiveWord);
                local maskWord = "";
                for i=1, maskWordCount do
                    maskWord = maskWord .. "*";
                end
                -- print(string.format("startIndex: %d endIndex: %d", startIndex, endIndex));
                -- print(string.format("strLen: %s maskWord: %s", strLen, maskWord));
        
                if (startIndex == 1) then
                    resultStr = maskWord .. string.sub(originStr, endIndex + 1, -1);
                elseif (endIndex == strLen) then
                    resultStr = string.sub(originStr, 1, startIndex) .. maskWord;
                else
                    local str = string.sub(originStr, 1, startIndex);
                    local str2 = string.sub(originStr, endIndex + 1, -1);
                    resultStr = str .. maskWord .. str2;
                end
            end
            return resultStr;
        end
        
        --内部接口：将字符串中的敏感字替换成*(替换所有)
        function ReplaceSensitiveWordAll(originStr, sensitiveWord)
            local str = originStr;
            local str2 = ReplaceSensitiveWord(originStr, sensitiveWord);
            while (str ~= str2) do
                str = str2;
                str2 = ReplaceSensitiveWord(str2, sensitiveWord);
            end
            return str2;
        end
        
        --内部接口：是否有该敏感字
        function HasSensitiveWord(originStr, sensitiveWord)
            local startIndex, endIndex = string.find(originStr, sensitiveWord, 1, true);
            if (startIndex and endIndex) then
                -- print("敏感字：" .. sensitiveWord);
                return true;
            else
                return false;
            end
        end
        
        --外部接口：敏感字替换
        function ReplaceMaskWord(content)
            for k,v in pairs(sensitiveWordConfig) do
                content = ReplaceSensitiveWordAll(content, v);
            end
            return content;
        end
        
        --外部接口：是否有敏感字
        function HasMaskWord(content)
            for k,v in pairs(sensitiveWordConfig) do
                if (HasSensitiveWord(content, v)) then
                    return true;
                end
            end
            return false;
        end
        
        print(ReplaceSensitiveWord("法?123法?", "法?"));
        print(ReplaceSensitiveWordAll("法?123法?", "法?"));
        print(HasSensitiveWord("12中法?3文", "法?"));
        print(ReplaceMaskWord("1法?法?2"));
        print(HasMaskWord("1法?法?2"));
        print("----------------------------------------------------")
    end


    do
        print("----------------------------------------------------")
        --截取中英混合的UTF8字符串，endIndex可缺省
        function SubStringUTF8(str, startIndex, endIndex)
            if startIndex < 0 then
                startIndex = SubStringGetTotalIndex(str) + startIndex + 1;
            end

            if endIndex ~= nil and endIndex < 0 then
                endIndex = SubStringGetTotalIndex(str) + endIndex + 1;
            end

            if endIndex == nil then 
                return string.sub(str, SubStringGetTrueIndex(str, startIndex));
            else
                return string.sub(str, SubStringGetTrueIndex(str, startIndex), SubStringGetTrueIndex(str, endIndex + 1) - 1);
            end
        end

        --获取中英混合UTF8字符串的真实字符数量
        function SubStringGetTotalIndex(str)
            local curIndex = 0;
            local i = 1;
            local lastCount = 1;
            repeat 
                lastCount = SubStringGetByteCount(str, i)
                i = i + lastCount;
                curIndex = curIndex + 1;
            until(lastCount == 0);
            return curIndex - 1;
        end

        function SubStringGetTrueIndex(str, index)
            local curIndex = 0;
            local i = 1;
            local lastCount = 1;
            repeat 
                lastCount = SubStringGetByteCount(str, i)
                i = i + lastCount;
                curIndex = curIndex + 1;
            until(curIndex >= index);
            return i - lastCount;
        end

        --返回当前字符实际占用的字符数
        function SubStringGetByteCount(str, index)
            local curByte = string.byte(str, index)
            local byteCount = 1;
            if curByte == nil then
                byteCount = 0
            elseif curByte > 0 and curByte <= 127 then
                byteCount = 1
            elseif curByte>=192 and curByte<=223 then
                byteCount = 2
            elseif curByte>=224 and curByte<=239 then
                byteCount = 3
            elseif curByte>=240 and curByte<=247 then
                byteCount = 4
            end
            return byteCount;
        end
        print("----------------------------------------------------")
    end


    --[[
        ref:https://www.cnblogs.com/rollingyouandme/p/11726559.html
(1)  string.gmatch的使用: 返回一个迭代器函数，每一次调用这个函数，返回一个在字符串 s 找到的 下一个 符合 pattern 描述的子串。如果参数 pattern 描述的字符串没有找到，迭代函数返回 nil。【属于lua语法】
(2) 匹配模式 ：[数个字符类] : 与任何[]中包含的字符类配对. 例如[%w_]与任何字母/数字, 或下划线符号(_)配对 。【属于lua语法】
(3) Unix 系统中：每行结尾只有 "<换行>"，即 "\n"；
    Windows 系统中：每行结尾是 "<回车><换行>"，即 "\r\n"；
    Mac 系统中：每行结尾是 "<回车>"，即 "\r"。
    ]]
    do
        print("----------------------------------------------------")
        local str = [[
            哈哈哈哈哈哈

            1111

            222
        ]]
        local enterCodeNum = 0    --换行符的数目
        for _ in string.gmatch(str ,  "[\n\r]" ) do   
            enterCodeNum = enterCodeNum + 1
        end
        print("换行符数目",enterCodeNum)

        print("----------------------------------------------------")
    end

    --[[
（1）字符串替换  ： string.gsub  (  目标字符串,   被替换者,   替身,   替换次数(省略表示全部替换)   )
       例如：      string.gsub("aaaaa" , "a" , "b" , 3)    结果为  "bbbaa" .
（2）匹配模式：
    [ ] : 与任何[]中包含的字符类配对. 例如[%w_]与任何字母/数字, 或下划线符号(_)配对;
    最前面加上尖角号 ^ 将锚定从字符串的开头处做匹配， 在模式最后面加上符号  $  将使匹配过程锚定到字符串的结尾。 如果 ^ 和 $ 出现在其它位置，则均没有特殊含义，只表示自身;
    加号 "+"  表示匹配一或更多个该类的字符, 总是匹配尽可能长的串。
    
    【翻译】所以代码翻译为：先将字符串左端的所有空格、水平制表符、回车符、换行符 替换成空字符（也就是删去了）。
    然后再将字符串尾部的所有空格、水平制表符、回车符、换行符 替换成空字符。


    【关于匹配模式的拓展】
(1). 刚才的例子中，极其容易与下面这个含义混淆：
  [^数个字符类]: 与任何不包含在[]中的字符类配对. 例如[^%s]与任何非空白字符配对。
但这里的尖角号是在中括号里面的，而上例中的尖角号在括号外，只有它在最开头的时候才表示锚定从字符串的开头处做匹配。
 
(2). 一些常用的匹配规则：
.(点): 与任何字符配对
%a: 与任何字母配对
%c: 与任何控制符配对(例如\n)
%d: 与任何数字配对
%l: 与任何小写字母配对
%p: 与任何标点(punctuation)配对
%s: 与空白字符配对
%u: 与任何大写字母配对
%w: 与任何字母/数字配对
当上述的字符类用 大写 时, 表示与 非此字符类的任何字符配对. 例如, %S表示与任何非空白字符配对.例如，'%A'非字母的字符。

因为%是转义符，所以在匹配的时候，想表达真正的百分号 “%” 的时候就要用 “%%”。 也就是说： lua 匹配时 “%%” 才等同于 我们所理解的 “%” 。

如果想把   local a = "Hello%%pWorld"   中的 双百分号替换成单百分号 ：  a = a:gsub("%%%%","%%")
    ]]
    do
        print("----------------------------------------------------")
        --去除首尾空白字符 (先将字符串左端的所有空格、水平制表符、回车符、换行符 替换成空字符（也就是删去了）；然后再将字符串尾部的所有空格、水平制表符、回车符、换行符 替换成空字符。)
        function string.trim(input)
            input = string.gsub(input, "^[ \t\n\r]+", "")
            return string.gsub(input, "[ \t\n\r]+$", "")
        end
        print("str trim start:" .. string.trim("  123  456  78 9 ") .. ",str trim end")
        print("----------------------------------------------------")
    end

    --[=[
        string.find()
        

        ref:    https://www.cnblogs.com/meamin9/p/4502461.html
        string.find(s, pattern[, init[, plain]]
        1.在字符串s中匹配pattern，如果匹配成功返回第一个匹配到的子串的起始索引和结束索引，
        如果pattern中有分组，分组匹配的内容也会接着两个索引值之后返回。如果匹配失败返回nil。
        2.可选数值参数init表示从s中的哪个索引位置开始匹配，缺省值是1，可以为负索引。
        3.可选布尔值参数plain为true时，pattern作为普通字符串匹配，所有正则中的元字符都只被作为普通字符解析。（这个参数并不是匹配字符串的结束索引）

    ]=]
    do

        -- string.find(s, pattern [, init [, plain]] )
        -- s: 源字符串
        -- pattern: 待搜索模式串
        -- init: 可选， 起始位置

        --[[
            pattern说明
            .   任意字符 
            %a   字母 
            %c   控制字符 
            %d   数字 
            %l   小写字母 
            %p   标点字符 
            %s   空白符 
            %u   大写字母 
            %w   字母和数字 
            %x   十六进制数字 
            %z   代表 0的字符 
            特殊字符如下：
            (). % + - * ? [] ^ $ 

            () 分组
            %   也作为以上特殊字符的转义字符
            +   匹配前一字符 1 次或多次，最长匹配
            *   匹配前一字符 0 次或多次，最长匹配
            -   匹配前一字符 0 次或多次，最短匹配
            ?   匹配前一字符 0 次或 1次 
        ]]

        local start1,last1 = string.find("hello this world .","wor")
        -- 返回起始位置和截止位置
        print("start1=",start1)
        print("last1=",last1)

        -- 注意： lua 里面数组或者字符串的字符， 其下标索引是从 1 开始， 不是 0
        local start2,last2 = string.find("hello this 21 world .","[0-9]+")
        print("start2=",start2)
        print("last2=",last2)



        print("----------------------------------------------------")
        print(string.find("abcde", "bc"))  --output: 2, 3
        local a = string.find("abcde", 'bc')                --只获取第一返回值
        local _, b =  string.find("abcde", 'bc')            --虚变量，获取第二返回值

        --第三个参数表示：从哪里开始查找。 如果不写，就是默认从头开始查找。
        print(string.find("abcde", 'bc', 3))
        print(string.find("abcdebc", 'bc', 3) )

        --匹配模式中，在内部用 小括号 括起来的部分被称为 捕获物 。当匹配成功时，由 捕获物 匹配到的字符串中的子串被保存起来，并且会把他们作为返回值。
        --[^c]+ 就是对捕获物的描述，意思是捕获一个 非 “c” 的字符串并且尽可能长。（加号“+”表示尽可能长地去匹配），所以捕获到了 “ab”，捕获物会被作为返回值。
        print(string.find( "abcde","([^c]+)" )) 
        local a, b, c = string.find( "abcde",  "([^c]+)",  3 )
        print(a, b, c)
        --捕获物可以有多个
        --[[
        且星号 “*” 表示匹配前面指定的 0 或多个同类字符， 尽可能匹配更长的符合条件的字串，在本例中 %s* 是 空格符。
        (  星号 * 跟加号 + 的区别就是：* 可以找不到，是0或多；而 + 至少得有一个，是1 或多 。 )

        返回了两个捕获物：name 和 Anna。因为匹配格式中有两组 小括号
        接收捕获物的时候，是从第三个返回值开始接收
        ]]
        local pair = " name = Anna " -- “name”前后、“=”前后、“Anna”前后都有空格
        print(string.find(pair, "(%a+)%s*=%s*(%a+)"))       --output: 2     12    name    Anna
        print("----------------------------------------------------")


        print("-----------------------string find test----------------------")
        local link = '<link type="4" type2="0" target_id="1001" map_id="2001">与丽亚好好谈谈</>'
        local _,_,_,targetId = string.find(link, '(target_id)=("%d+")')
        --[[
            lua "1001" ~= tostring(1001)
        ]]
        print(targetId)
        print(string.gsub(targetId, '"', ''))
        print(tonumber(targetId))
        print(tostring(targetId))
        print(type(targetId))
        local tId = tonumber(targetId)
        print(tId)
        print(targetId == tostring(1001))
        --print(string.gsub(link,"[target_id='%w']","##"))
        print(os.time())


        local taskInfo = '<tag color="#ffff50">收集经验药水，并使用一个。</><tag color="#48ffc2" size="20">([step1]/1)</>\
        <tag color="#ffff50">收集疲劳药水，并使用两个。</><tag color="#48ffc2" size="20">([step2]/2)</>'
        local stepArr = {[1] = 1, [2] = 1}
        local index = 1
        local ts = taskInfo
        for s in string.gmatch(ts, '%[step(%d+)%]') do
            print(s)
            ts = string.gsub(ts, '%[step'..s..'%]', tostring(stepArr[index]))
            index = index + 1
        end
        print(ts)
        print("-----------------------string find test--------------------")

        print("-----------------------string find test2--------------------")
        print(string.find('Hanazawa Kana', 'na'))           --3       4
        print(string.find('Hanazawa Kana', '[%a]+'))        --1       8
        print(string.find('2015-5-12 13:53', '(%d+)-(%d+)-(%d+)'))  --1       9       2015    5       12
        print(string.find('2015-5-12 13:53', '(%d+)-(%d+)-(%d+)', 1, true))     --nil
        print(string.find('%a1234567890%a', '%a', 3, true))     --13      14            从第三个索引开始查找  匹配到的时最后那个 %a   13和14序号
        print("-----------------------string find test2--------------------")

        print("-----------------------string find test3--------------------")
        local link = '<link color="#ffff50" type="4" type2="1" target_id="100003" map_id="3001">前往探索无主之地(0/1)</>'
        print(string.find(link, "<link.+>(.+)</>"))
        print("-----------------------string find test3--------------------")

        print("-----------------------string find test4--------------------")

        local _,_,itemIds = string.find('[{"id":1, "condType":2, "maxStep":3,"itemIds":[110210901,110210902]}]', '%"itemIds%":%[([%d+,?]+)%]')
        print(itemIds)
        print(string.match(itemIds, '(%d+),?'))
        for k, v in string.gmatch(itemIds, '(%d+),?') do
            print(k, v)
        end
        print("-----------------------string find test4--------------------")
    end


    JSON = require("Json")

    --[[
        string.match
        所有用到match的地方都可以用find来实现。match是find的一个简化版

        ref:    https://www.cnblogs.com/meamin9/p/4502461.html
        string.match(s, pattern[, init])
        在字符串s中匹配pattern，如果匹配失败返回nil。否则，当pattern中没有分组时，返回第一个匹配到的子串；
        当pattern中有分组时，返回第一个匹配到子串的分组，多个分组就返回多个。
        可选参数init表示匹配字符串的起始索引，缺省为1，可以为负索引。


        string.gmatch(s, pattern)
        返回一个迭代器。每当迭代器调用时，返回下一个匹配到的子串，如果pattern中有分组，返回的是子串对应的分组。gmatch也可以用find和循环来实现。

    ]]
    do
        print("-----------------------string match test--------------------")
        print(string.match('2015-5-12 13:53', '%d+-%d+-%d+'))               --2015-5-12
        print(string.match('2015-5-12 13:53', '(%d+)-(%d+)-(%d+)'))         --2015    5       12
        print(string.match('2015-5-12 13:53', '((%d+)-(%d+)-(%d+))'))       --2015-5-12       2015    5       12


        print(string.match('[{"id":1, "condType":2, "maxStep":3,"itemIds":[110210901,110210902,110210903]}]', '%"itemIds%":%[([%d+,?]+)%]'))
        print("-----------------------string match test--------------------")


        print("-----------------------string gmatch test--------------------")
        for s in string.gmatch('2015-5-12 22:20', '%d+') do         
            print(s)            
        end
        --[[
            2015
            5
            12
            22
            20
        ]]

        for s in  string.gmatch('Hanazawa Kana', 'a(%a)a') do           --匹配形如 'a字母a' 中间的字母
            print(s)
        end
        --[[
            n
            w
            n

例子中，处于两个a字母中间的单个字母还有‘z’，但循环并没有输出。
原因是在'ana'匹配成功之后，接下来匹配是从'z'开始的，z没有被匹配到。正确的模式pattern应该不要捕获'a(%a)a'的后面的a，
用python的正则可以写成'a(\w)(?=a)',他不会消耗掉后面的a。
但是lua不支持(?=...)。（python中\w表示单词字符[a-zA-Z0-9_]，记不清就把这类元字符列出来 如 %a写成[a-zA-Z] 。）
        ]]

        for k, v in string.gmatch('a=214,b=233', '(%w+)=(%w+)') do
            print(k, v)
        end
        --[[
            a       214
            b       233
        ]]


        for k, v in string.gmatch('500000005_21400,500000001_10000', '(%d+)_(%d+)') do
            print(k, v)
        end


        for k, v in string.gmatch('<link type="4" type2="2" item_ids="110210901_1,110210902_2">使用一个经验药水([step1]/1)，使用两个个高级经验药水([step2]/2)</>', '(%d+)_(%d+)') do
            local iId = string.gsub(k, '"', '')
            local iNum = string.gsub(v, '"', '')
            print("<link> gmatch: ", iId, iNum)
        end
       
        local condDesc = '[{"id":1, "condType":2, "maxStep":3,"itemIds":[110210901,110210902]},{"id":1, "condType":2, "maxStep":1,"itemIds":[110210903,110210904]}]'
        --[[
        for itemInfos, _ in string.gmatch(condDesc, '{.+},?') do
            print(itemInfos)
            for info, _ in string.gmatch(itemInfos, '({.+}[,?]') do
                print(info)
            end
        end
        ]]

        local jsonObj = JSON:decode(condDesc)
        for i = 1, #jsonObj do
            local obj = jsonObj[i]
            print(string.format("id: %d, maxStep: %d", obj.id, obj.maxStep))
            local itemIds = obj["itemIds"]
            if itemIds ~= nil then
                for j = 1, #itemIds do
                    print(itemIds[j])
                end
            end
        end
    

        for itemIds, _ in string.gmatch(condDesc, '%"itemIds%":%[([%d+,?]+)%]') do
            print(itemIds)
            for itemId, _ in string.gmatch(itemIds, '(%d+),?') do
                print(itemId)
            end
        end
        for itemNum, _ in string.gmatch(condDesc, '%"maxStep%":(%d+)') do
            print(itemNum)
        end

        print("string gmatch 111")
        local sstr = '<link color="#ec933b" size="20" type="4" type2="frame" p1="PackageBag3DFrame">同时穿戴紫色太刀(0/1)</>\n<link color="#ec933b" size="20" type="4" type2="frame" p1="PackageBag3DFrame">穿戴50级紫色轻甲鞋子(0/1)</>'
        local rsstr = string.gsub(sstr, '\n', ';')
        local srstr = rsstr .. ";"
        for str in string.gmatch(srstr, "([^;]+)") do
            print(str)
            local s = string.match(str, "<link.+>(.+)</>")
            print(s)
        end

        print("string gmatch 222")
        local res = nil
        local content = [[666\n555abnocbne]]
        if content ~= nil then   
            local regex = [[\n]]
            local rstr = string.gsub(content, regex, '\n')
            print(rstr)
            content = content .. regex
            for str in string.gmatch(content, regex) do
                if str then
                    if res then
                        res = res .. '\n' .. str
                    else
                        res = str
                    end
                end
            end
        end
        print(res)

        print("string gmatch 333")
        local content2 = '"是否出售1件[<tag color="#FF7DAAFF">粉</>]物品强化券+14（绑定100%）？"'
        local rep = '{content:0}'
        local valueStr = string.gsub(content2, '%%', '%%'..'%%')            --替换时，遇到特殊字符（转义用）%，需要使用'%%' 来转义
                                                                            --但是不能在[]中使用[%%]
        print(valueStr)
        local res2 = string.gsub(rep, rep, valueStr)
        print(res2)

        print("string gmatch 444")
        local content3 = "2_开启后可获得以下道具：|3_开启后可随机获得以下道具中的1种或5种："
        local content4 = "开启后可自选以下道具中的1种：|开启后可获得以下道具：|开启后可随机获得以下道具中的10种："
        for k in string.gmatch(content3, '([^|]*)') do
            print(k)
            for t, d in string.gmatch(k, '(%d+)_(.+)') do
                print(t, d)
            end
        end


        print("string gmatch 555")
        --[[
            Split a string using string.gmatch() in Lua
        ]]
        local s = "one;two;;four"
        local words = {}
        for w in s:gmatch("([^;]*)") do        
        --for w in (s .. ";"):gmatch("([^;]*);") 也可用
            table.insert(words, w) 
        end
        for n, w in ipairs(words) do
            print(n .. ": " .. w)
        end        

        local ss = "50_1,30_2,20_3|40_1,30_2,30_3"
        local partReg = "([^|]*)"
        local wordss = {}
        for w in string.gmatch(ss, partReg) do
            table.insert(wordss, w)
        end
        for n, w in ipairs(wordss) do
            print(n .. ": " .. w)
        end      
        
        local s2 = "110210008|110210011,110210008;110210008"
        for w in string.gmatch(s2, "(%d+)") do
            print(w)
        end

        print("-----------------------string gmatch test--------------------")
    end



    --[[Lua 字符串相关操作: https://www.cnblogs.com/zhanggaofeng/p/12978129.html]]
    do
        -- 字符串格式化
        --[[
            string.format()
            原型：string.format (formatstring, ···)
            解释：返回第一个参数描述之后的参数的格式化版本，第一个参数必须为字符串，是对结果字符串的一种描述，
            这个格式化的字符串和C语言的printf()一族的函数遵循相同的规则，仅有的不同体现在参数选项的修改，
            其中参数描述符*,l,L,p和h不再支持，但是多了一个额外的选项q，这个q选项会以一种适合lua解释器安全读取的方式来格式化字符串：
            被写在双引号之间的字符串包括双引号、换行、空字符（'\0'或NULL）、反斜杠在被格式化时都能被正确的分离出来。

            格式控制符以%开头，常用的有以下几种
            %c - 接受一个数字,并将其转化为ASCII码表中对应的字符
            %d, %i - 接受一个数字并将其转化为有符号的整数格式
            %o - 接受一个数字并将其转化为八进制数格式
            %u - 接受一个数字并将其转化为无符号整数格式
            %x - 接受一个数字并将其转化为十六进制数格式,使用小写字母
            %X - 接受一个数字并将其转化为十六进制数格式,使用大写字母
            %e - 接受一个数字并将其转化为科学记数法格式,使用小写字母e
            %E - 接受一个数字并将其转化为科学记数法格式,使用大写字母E
            %f - 接受一个数字并将其转化为浮点数格式
            %g(%G) - 接受一个数字并将其转化为%e(%E,对应%G)及%f中较短的一种格式
            %q - 接受一个字符串并将其转化为可安全被Lua编译器读入的格式
            %s - 接受一个字符串并按照给定的参数格式化该字符串

            为进一步细化格式, 可以在%号后添加参数.参数将以如下的顺序读入:
            (1) 符号:一个+号表示其后的数字转义符将让正数显示正号.默认情况下只有负数显示符号.
            (2) 占位符: 一个0,在后面指定了字串宽度时占位用.不填时的默认占位符是空格.
            (3) 对齐标识: 在指定了字串宽度时,默认为右对齐,增加-号可以改为左对齐.
            (4) 宽度数值
            (5) 小数位数/字串裁切:在宽度数值后增加的小数部分n,若后接f(浮点数转义符,如%6.3f)则设定该浮点数的小数只保留n位,    若后接s(字符串转义符,如%5.3s)则设定该字符串只显示前n位.
        ]]

        print(string.format("---[%c]--",65))      -- 接受一个十进制数字，将其转化成ASCII码表中对应的字符(65 == A)
        print(string.format("---[%d]--",15))      -- 打印数字15
        print(string.format("---[%o]--",9))       -- 打印数字011
        print(string.format("---[%u]--",12))      -- 打印数字12
        print(string.format("---[%08x]--",47))    -- 打印0000002f

        print("-----------------------string format test--------------------")
        local itemInfoDescFormat = '%"'.. "itemIds" ..'%":%[([%d+,?]+)%]'
        print(itemInfoDescFormat)
        print("-----------------------string format test--------------------")
    end


    do
        -- 字符串替换
        --[[
            string.gsub()
            原型：string.gsub (s, pattern, repl [,m])
            参数说明:
            s            源字符串
            pattern      模式串
            repl         替换串

            返回值：返回一个和pattern匹配，并且用rep1替换的副本。第二个返回值n是代表匹配的个数。

            pattern说明
            .   任意字符 
            %a   字母 
            %c   控制字符 
            %d   数字 
            %l   小写字母 
            %p   标点字符 
            %s   空白符 
            %u   大写字母 
            %w   字母和数字 
            %x   十六进制数字 
            %z   代表 0的字符 
            特殊字符如下：
            (). % + - * ? [] ^ $ 

            () 分组
            %   也作为以上特殊字符的转义字符
            +   匹配前一字符 1 次或多次，最长匹配
            *   匹配前一字符 0 次或多次，最长匹配
            -   匹配前一字符 0 次或多次，最短匹配
            ?   匹配前一字符 0 次或 1次 

            repl说明
            这个函数会返回一个替换后的副本，原串中所有的符合参数pattern的子串都将被参数repl所指定的字符串所替换，如果指定了参数m，那么只替换查找过程的前m个匹配的子串，
            参数repl可以是一个字符串、表、或者是函数，并且函数可以将匹配的次数作为函数的第二个参数返回，接下来看看参数repl的含义：

            如果参数repl是一个常规字符串，成功匹配的子串会被repl直接替换，如果参数repl中包含转移字符%，
            那么可以采用%n的形式替换，当%n中的n取值1-9时，表示一次匹配中的第n个子串，当其中的n为0时，表示这次匹配的整个子串，%%表示一个单独的%。

            如果参数repl是一个表，那么每次匹配中的第一个子串将会作为整个表的键，取table[匹配子串]来替换所匹配出来的子串，
            当匹配不成功时，函数会使用整个字符串来作为table的键值。

            如果参数repl是一个函数，那么每一次匹配的子串都将作为整个函数的参数，取function(匹配子串)来替换所匹配出来的子串，
            当匹配不成功时，函数会使用整个字符串来作为函数的参数。如果函数的返回值是一个数字或者是字符串，那么会直接拿来替换，
            如果它返回false或者nil，替换动作将不会发生，如果返回其他的值将会报错。


            ref: https://www.cnblogs.com/meamin9/p/4502461.html
            string.gsub(s, pattern, repl[, n])
            替换字符串函数！这个功能应该是字符串处理中实用性最强的一个。
            把字符串中用模式pattern匹配到的所有子串替换为repl指代的子串，返回替换后的字符串和替换的次数。
            可选数值参数n表示最多可替换的次数。
            参数repl可以是正则表达式，也可以是函数。当repl是函数时，函数的参数是模式pattern捕获的子串，
            和match类似，有分组返回分组，无分组返回整个子串。函数最后应该返回一个字符串。
            如果repl是正则表达式，可以用分组序号引用匹配到的分组。
        ]]


        local p = "int x; /* x */  int y; /* y */"
        -- 匹配注释
        print(string.gsub(p,"/%*.-%*/","##"))
        -- 测试函数模式
        local str="abcdefg"
        string.gsub(str,"%l",function(ch)
            print(ch)
        end)
        --print(string.byte(str))



        print("-----------------------string gsub test--------------------")
        print(string.gsub('Hanazawa Kana', 'na', 'nya'))            --Hanyazawa Kanya 2
        print(string.gsub('Hanazawa Kana', 'na', function(s) return string.sub(s,1,1)..'y'..string.sub(s,2,2) end)) --Hanyazawa Kanya 2
        print(string.gsub('Hanazawa Kana', '(n)(a)', function(a,b) return a..'y'..b end))  --Hanyazawa Kanya 2
        print(string.gsub('Hanazawa Kana', '(n)(a)', '%1y%2'))      --Hanyazawa Kanya 2
        print("-----------------------string gsub test--------------------")
    end


end -- do end block



--[[
    lua正则
正则表达式由元字符按照规则(语法)组成。lua中的特殊字符是%.^$+-*?,一共12个。它们和一般字符按规则构成了lua的正则表达式。

]]

print("################ string end ###############")