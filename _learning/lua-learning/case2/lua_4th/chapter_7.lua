--[[
    I/O模型
    '
    两种模型：
    1.简单模型：虚拟了一个当前输入和输出流（即使用了C语言的stdin和stdout）
        io.read() 可以从标准输入中读取一行
                    io.read("a")    从当前位置读取整个文件，如果当前位置在文件末尾或者文件为空时，则返回一个空字符串
                    io.read("l")    读取下一行（丢弃换行符）
                    io.read("L")    读取下一行（保留换行符）
                    io.read("n")    从当前输入流中读取一个数值，此时read返回值为数值（整型或者浮点型，与Lua语法扫描器规则一致），而非字符串的唯一情况
                                    如果跳过空格后，read无法从当前位置读取到数值（格式错误或者到文件末尾），则返回nil
                    io.read(n)      读取一个数值
                    io.read(num)    以字符串读取num个字符

        io.input() 和 io.output() 可以改变当前的输入输出流
            io.input() 可以以只读模式打开指定文件，并将文件设置为当前输入流，之后输入都来自该文件
            io.output()类似
            都会抛出异常，需要通过完成I/O模型才能直接处理异常

        io.write() 可以读取任意数量的字符串（或者数字）并将其写入当前输出流
            使用 io.write(a, b, c) 可以使用多个参数 而非 io.write(a..b..c) 避免过多的连接

            io.write() vs. print()
                print() 用在 用后即弃 或者 调试代码中；
                        会添加制表符、换行符；
                        只能使用标准输出；
                        自动对参数进行tostring()操作
                        
                io.write() 在需要完全控制输出时；
                        不会添加制表符、换行符；
                        允许对输出进行重定向


    2.完整模型：
]]
do
    
    local function test_io_write()
        io.write("sin(3) = ", math.sin(3), '\n')
        io.write(string.format("sin(3) = %.4f\n", math.sin(3)))
    end
    test_io_write()


    --[[
        lua可以高效处理长字符串，可以编写filter：
            将整个文件读取到一个字符串，然后对字符串进行处理
    ]]
    local function test_io_read_1()
        local t = io.read("a")
        t = string.gsub(t, "bad", "good")
        io.write(t)
    end
    --test_io_read_1()


    local function test_io_read_2()
        local t = io.read("all")
        -- 代码含义：匹配所有等号及非ASCII字符（从128到255） 并调用指定函数进行替换
        -- 将文本内容使用MIME可打印字符引用编码（quoted-printable）进行编码
        -- 将所有非ASCII字符编码为 =xx  （xx是这个字符的十六进制，为保证编码一致性，=也会被编码）
        t = string.gsub(t, "([\128-\255=])", function (c)
            return string.format("=%02x", string.byte(c))
        end)
        io.write(t)
    end
    --test_io_read_2()


    --[[
        io.read("l") 返回当前输入流的下一行，不包括换行符，"l"是read的默认参数，通常在逐行读取数据中使用
        io.read("L") 返回当前输入流的下一行，保留换行符
    ]]

    --将当前输入复制到当前输出中，同时对每行进行编号
    local function test_io_read_3()
        for count = 1, math.huge do
            local line = io.read("L")
            if line == nil then break end
            io.write(string.format("%6d ", count), line)
        end
    end

    -- 逐行迭代文件，使用io.lines() 迭代器
    local function test_io_read_4()
        local count = 0
        for line in io.lines() do             
            count = count + 1
            io.write(string.format("%6d ", count), line "\n")
        end
    end

    local function test_io_read_5()
        local lines = {}

        -- 将所有行读取到表
        for line in io.lines() do
            lines[#lines + 1] = line
        end

        -- 排序
        table.sort(lines)

        -- 输出所有行
        for _, l in ipairs(lines) do
            io.write(l, "\n")
        end
    end
    test_io_read_5()

    --[[
        io.read(n) 而非 io.read("n")
        从输入流中读取n个字符，返回一个由流中最多n个字符组成的字符串
        如果无法读取到字符（处于文件末尾），则返回nil

        io.read(0) 常用于测试是否到达了文件末尾，如果仍有数据可读取，返回一个空字符串，否则返回nil
    ]]
    -- 将文件从stdin复制到stdout的高效方法
    local function test_io_read_6()
        while true do
            local block = io.read(2 ^ 13)               -- 块大小为8KB
            if not block then break end
            io.write(block)
        end
    end

    --[[
        read可以指定多个选项，函数根据每个参数返回对应结果
        例：文件内容如下
        6.0     -3.23      15e12
        4.3     234        1000001
        ...
    ]]
    local function test_io_read_7()
        while true do
            local n1, n2, n3 = io.read("n", "n", "n")
            if not n1 then break end
            print(math.max(n1, n2, n3))
        end
    end
end

do
    
end