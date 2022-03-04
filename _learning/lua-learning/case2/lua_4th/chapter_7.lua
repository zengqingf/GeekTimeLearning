--[[
    I/O模型
    '
    两种模型：
    1.简单模型：虚拟了一个当前输入和输出流（即使用了C语言的stdin和stdout）
        io.read() 可以从标准输入中读取一行
        io.input() 和 io.output() 可以改变当前的输入输出流
            io.input() 可以以只读模式打开指定文件，并将文件设置为当前输入流，之后输入都来自该文件
            io.output()类似
            都会抛出异常，需要通过完成I/O模型才能直接处理异常

        io.write() 可以读取任意数量的字符串（或者数字）并将其写入当前输出流
            使用 io.write(a, b, c) 可以使用多个参数 而非 io.write(a..b..c) 避免过多的连接

            io.write() vs. print()
                print() 用在 用后即弃 或者 调试代码中
                io.write() 在需要完全控制输出时


    2.完整模型：
]]
do
    
end