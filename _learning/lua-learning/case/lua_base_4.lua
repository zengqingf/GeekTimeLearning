--[[
    文件IO

    r	以只读方式打开文件，该文件必须存在。
    w	打开只写文件，若文件存在则文件长度清为0，即该文件内容会消失。若文件不存在则建立该文件。
    a	以附加的方式打开只写文件。若文件不存在，则会建立该文件，如果文件存在，写入的数据会被加到文件尾，即文件原先的内容会被保留。（EOF符保留）
    r+	以可读写方式打开文件，该文件必须存在。
        （当前文件位置指向的是文件头部，执行读取和写入都是从头部开始的，读取完一部分数据后，
        位置指向会向后偏移，偏移的长度就是读取的位置长度，同理写入数据也是一样；）

    w+	打开可读写文件，若文件存在则文件长度清为零，即该文件内容会消失。若文件不存在则建立该文件。
    a+	与a类似，但此文件可读可写
        （当前文件位置指向的是文件尾部，执行读取操作时是从尾部开始的，这样读取的数据为nil，
        而写入也是从尾部开始的，因此写入的数据会以追加的形式写入到当前文件的末尾，然后指向位置也会向后偏移到尾部。）

    b	二进制模式，如果文件是二进制文件，可以加上b
    +	号表示对文件既可以读也可以写


    io.read(*arg)
    "*n"	读取一个数字并返回它。例：file.read("*n")
    "*a"	从当前位置读取整个文件。例：file.read("*a")
    "*l"（默认）	读取下一行，在文件尾 (EOF) 处返回 nil。例：file.read("*l")
    number	返回一个指定字符个数的字符串，或在 EOF 时返回 nil。例：file.read(5)


    其他io方法：
        io.tmpfile():       返回一个临时文件句柄，该文件以更新模式打开，程序结束时自动删除
        io.type(file):      检测obj是否一个可用的文件句柄
        io.flush():         向文件写入缓冲中的所有数据
        io.lines(optional file name): 返回一个迭代函数,每次调用将获得文件中的一行内容,当到文件尾时，将返回nil,但不关闭文件



    完全模式：

    其他方法：
    file:seek(optional whence, optional offset): 设置和获取当前文件位置,成功则返回最终的文件位置(按字节),失败则返回nil加错误信息。
    参数 whence 值可以是:
        "set": 从文件头开始
        "cur": 从当前位置开始[默认]
        "end": 从文件尾开始
        offset:默认为0
        不带参数file:seek()则返回当前位置,file:seek("set")则定位到文件头,file:seek("end")则定位到文件尾并返回文件大小
    
    file:flush(): 向文件写入缓冲中的所有数据
        
]]

do
    --简单IO模式
    local file = io.open("lua_base_4.lua", "r")
    io.input(file)          --设置默认输入文件为lua_base_4.lua
    print(io.read())        --读取第一行
    io.close(file)          --关闭打开的文件

    file = io.open("lua_base_4.lua", "a")
    io.output(file)         --设置默认输出文件为lua_base_4.lua
    --io.write("-- 添加文件末尾注释\n")
    io.close(file)


    --[[
io.lines(optional file name): 
打开指定的文件filename为读模式并返回一个迭代函数,每次调用将获得文件中的一行内容,当到文件尾时，将返回nil,并自动关闭文件。
若不带参数时io.lines() <=> io.input():lines(); 读取默认输入设备的内容，但结束时不关闭文件
    ]]
    --for line in io.lines("lua_base_4.lua") do
    --  print(line)
    --end
end

do
    local file = io.open("lua_base_4.lua", "r")
    print(file:read())
    file:close()

    file=io.open("lua_base_4.lua", "a")
    --完全方法
    --file:write("-- 添加文件末尾注释2\n")
    file:close()

    print("test io seek...")
    --定位到文件倒数第 25 个位置并使用 read 方法的 *a 参数，即从当期位置(倒数第 25 个位置)读取整个文件
    file=io.open("lua_base_4.lua", "a")
    file:seek("end", -25)
    print(file:read("*a"))
    file:close()
end

--[[
    使用 *n 作为参数读取文件中的数字的时候，只有文件中第一个字符是数字（或者空格加数字）的情况下才能读取到并返回这个数字

@注意：
    简单模式下：
    io.write后如果需要io.read，需要先io.flush（将内存缓存区中的内容写入文件）或者io.close(会默认flush)
    io.write 写入的数据只是在缓冲区中，并未真正写入到文件中！！！
    io.open是会指定文件读取位置，如果此时缓存中如果写入后，文件读取指针位置会发生偏移，导致错乱

    io.close后，之前设置的默认输入和输出文件会失效，需要重新设置
]]







--[[
    错误处理和调试
]]
do
    local function add(a, b)
        assert(type(a) == "number")
        assert(type(b) == "string")
        return a, b
    end
    add(10, "1")

    --error("error 1", 1)
    --error("error 2", 2)
    --error("error 0", 0)

    pcall(
    function(i) 
        print(i)
        error("error..." .. i)
    end,
    34)


    local function myFunc(n)
        local res = n / nil
    end
    local function myErrorHandler(err)
        print("error: ", err)
    end
    local status = xpcall(myFunc, myErrorHandler)
    print(status)

--ref: https://www.runoob.com/lua/lua-debug.html
    local function myFunc2()
        print(debug.traceback("stack trace"))
        print(debug.getinfo(1))
        print("stack trace end")
        return 0
    end
    myFunc2()
    print(debug.getinfo(1))
end


--[[
    GC
collectgarbage("setpause", 200) ： 内存增大 2 倍（200/100）时自动释放一次内存 （200 是默认值）。
collectgarbage("setstepmul", 200) ：收集器单步收集的速度相对于内存分配速度的倍率，设置 200 的倍率等于 2 倍（200/100）。（200 是默认值）


collectgarbage("collect"): 做一次完整的垃圾收集循环。通过参数 opt 它提供了一组不同的功能：
collectgarbage("count"): 以 K 字节数为单位返回 Lua 使用的总内存数。 这个值有小数部分，所以只需要乘上 1024 就能得到 Lua 使用的准确字节数（除非溢出）。
collectgarbage("restart"): 重启垃圾收集器的自动运行。
collectgarbage("setpause"): 将 arg 设为收集器的 间歇率。 返回 间歇率 的前一个值。
collectgarbage("setstepmul"): 返回 步进倍率 的前一个值。
collectgarbage("step"): 单步运行垃圾收集器。 步长"大小"由 arg 控制。 传入 0 时，收集器步进（不可分割的）一步。 传入非 0 值， 收集器收集相当于 Lua 分配这些多（K 字节）内存的工作。 如果收集器结束一个循环将返回 true 。
collectgarbage("stop"): 停止垃圾收集器的运行。 在调用重启前，收集器只会因显式的调用运行。
]]
do
    local mytable = {"a", "b", "c"}
    print(collectgarbage("count"))

    mytable = nil
    print(collectgarbage("count"))
    print(collectgarbage("collect"))
    print(collectgarbage("count"))
end