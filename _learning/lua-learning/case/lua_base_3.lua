--[[
    协程 coroutine
Lua 协同程序(coroutine)与线程比较类似：拥有独立的堆栈，独立的局部变量，独立的指令指针，同时又与其它协同程序共享全局变量和其它大部分东西。

一个具有多个线程的程序可以同时运行几个线程，而协同程序却需要彼此协作的运行。
在任一指定时刻只有一个协同程序在运行，并且这个正在运行的协同程序只有在明确的被要求挂起的时候才会被挂起。
表现形式类似于同步的多线程，即在等待同一个线程锁的几个线程


ref: https://www.cnblogs.com/tangxin-blog/p/10310317.html

lua协程 存在 表 coroutine 中
有四个状态：
    挂起(suspended)：一个协程被创建的时候，处于挂起状态，不会自动运行。
    运行(running)：coroutine.resume()用于启动或者再次启动一个协程，使其变成运行状态。
    正常(normal)：协程A唤醒协程B的时候，协程B处于运行状态，协程A就处于正常状态。
    死亡(dead)：协程中包含的函数执行完毕，协程就变成了死亡状态。

Lua协程的特点
Lua协程是一种非对称协程（asymmetric coroutine）,需要两个函数来控制协程的执行，一个用于挂起协程，一个用于恢复协程。
相对于其他语言提供的对称协程（symmetric coroutine），只提供一个函数用于切换不同协程之间的控制权。
]]

do

    --创建，返回值为coroutine （thread）
    --[[
        coroutine.creat方法只要建立了一个协程 ，那么这个协程的状态默认就是suspend。
        使用resume方法启动后，会变成running状态；
        遇到yield时将状态设为suspend；
        如果遇到return，那么将协程的状态改为dead。
    ]]

    --[[
        创建协程：coroutine.create(func)
        接受一个函数作为参数，返回一个thread类型的值。
    ]]
    local cor1 = coroutine.create(
        function(i)
            print(i);
        end
    )

    --重启
    --[[
        只要调用就会返回一个boolean值
        如果调用成功，那么返回true；
        如果有yield方法，同时返回yield括号里的参数；
        如果没有yield，那么继续运行直到协程结束；
        直到遇到return，将协程的状态改为dead，并同时返回return的值。

        如果调用失败(调用状态为dead的协程会导致失败)，那么返回false，并且带上一句"cannot resume dead coroutine"
    ]]
    coroutine.resume(cor1, 1)

    --[[
        启动协程：coroutine.resume(co, arg1, arg2, ...)
        参数：可以分为两种情况。
            1.协程中不包含yield()：第一个参数是被启动的协程，后面的参数传递给协程封装的函数作为参数。

            2.协程中包含yield()：第一个参数还是被启动的线程，在首次调用resume时，后面的参数传递给协程封装的函数作为参数；
                而再次调用（非首次）resume()时，后面的参数将作为yield()的返回值。

        返回值：分为三种情况。
            1.协程没有结束，resume()第一个返回值是true，后面的返回值是yield(...)中的参数。

            2.协程结束时，resume()第一个返回值是true，后面的返回值是协程中函数的返回值。

            3.协程结束后，此时不应该继续调用resume，如果调用了，resume()第一个返回值是false，表示调用失败，第二个返回值是报错信息。
                (resume运行在保护模式中，如果协程在执行过程中遇到了错误，Lua不会打印错误信息，而是把错误信息作为resume()的返回值。)
    ]]
    do
        local co = coroutine.create(
            function(a, b) 
                print("a + b =", a + b) 
            end
        )
        coroutine.resume(co, 1, 2)          -- a + b = 3
    end

    do
        local co = coroutine.create(
            function(x) 
                print("co1", x) 
                print("co2", coroutine.yield()) 
            end
        )
        coroutine.resume(co, "hello")           -- co1  hello
        coroutine.resume(co, "world")           -- co2  world
    end

    do
        local co = coroutine.create(
            function()
                coroutine.yield("hello", "world")
                return "hello", "lua"
            end
        )
        print(coroutine.resume(co))         -- true     hello      world
        print(coroutine.resume(co))         -- true     hello      lua
        print(coroutine.resume(co))         -- false    cannnot resume dead coroutine
    end


    --[[
        挂起协程：coroutine.yield(arg1, arg2, ...)
        参数：yield()将作为本次唤醒协程的resume()的返回值。
        返回值：下次唤醒协程的resume()的参数，将作为yield()的返回值。
    ]]
    do
        local co = coroutine.create(
            function()
                local ret = coroutine.yield("hello")
                print(ret)
            end
        )
        local state, ret = coroutine.resume(co)
        print(state)                                    -- true
        print(ret)                                      -- hello
        coroutine.resume(co, "world")                   -- world
    end

    --查看状态
    --返回值：dead / suspended / running
    print(coroutine.status(cor1))

    print("--------------------------")


    --创建2，返回值为函数 function，调用函数即进入coroutine
    local cor2 = coroutine.wrap(
        function (i)
            print(i)
        end
    )

    cor2(2)

    print("--------------------------")

    Cor3 = coroutine.create(
        function ()
            for i=1, 10 do
                print(i)
                if i == 3 then
                    print(coroutine.status(Cor3))
                    print(coroutine.running())          --返回正在运行的cor, 可以认为一个cor即一个线程，返回值为一个cor的线程号
                end
                coroutine.yield()                       --挂起cor, 配合resume使用
                                                        --[[
                                                             yield 除了挂起协程外，还可以同时返回数据给 resume ,并且还可以同时定义下一次唤醒时需要传递的参数。
                                                        ]]
            end
        end
    )

    coroutine.resume(Cor3)
    coroutine.resume(Cor3)
    coroutine.resume(Cor3)

    print(coroutine.status(Cor3))
    print(coroutine.running())

    print("--------------------------")

end


--[[
coroutine.running就可以看出来,coroutine在底层实现就是一个线程。
当create一个coroutine的时候就是在新线程中注册了一个事件。
当使用resume触发事件的时候，create的coroutine函数就被执行了，当遇到yield的时候就代表挂起当前线程，等候再次resume触发事件。
]]
do
    local function foo(a)
        print("foo output:", a)
        return coroutine.yield(2 * a)
    end

    local cor = coroutine.create(function (a, b)
        print("first cor output:", a, b)
        local r = foo(a + 1)

        print("second cor output:", r)
        local r, s = coroutine.yield(a + b, a - b)

        print("third cor output:", r, s)
        return b, "end cor"
    end)

    print("mian", coroutine.resume(cor, 1, 10))
    print("---###---")
    print("mian", coroutine.resume(cor, "rt"))
    print("---###---")
    print("mian", coroutine.resume(cor, "x", "y"))
    print("---###---")
    print("mian", coroutine.resume(cor, "x", "y"))
    print("---###---")

    --[[
        resume和yield的配合强大之处在于，resume处于主程中，它将外部状态（数据）传入到协同程序内部；
        而yield则将内部的状态（数据）返回到主程中。     (上例中为打印输出)
    ]]
end

--[[生产者、消费者]]
do
    local newProducer

    local function receive()
        local status, value = coroutine.resume(newProducer)
        return value
    end

    local function send(x)
        coroutine.yield(x)
    end

    local function productor()
        local i =0
        while true do
            i = i + 1
            print("productor: "..i)
            send(i)                     --发送给消费者            
        end
    end

    local function consumer()
        while true do 
            local i = receive()        --从生产者中获取
            print("consumer: "..i)
        end
    end

    --newProducer = coroutine.create(productor)
    --consumer()
end

do
    -- 消费者驱动型 consumer-driven
    local producer_co = coroutine.create(
        function()
            for i = 1, 5 do
                print("produce:", i)
                coroutine.yield(i)
            end
        end
    )

    function consumer()
        while true do
            local status, value = coroutine.resume(producer_co)
            print("producer:", coroutine.status(producer_co))
            if not value then
                break
            end
            print("consume:", value)
        end
    end

    consumer()
end

do
    -- 生产者驱动型 producer-driven
    local consumer_co = coroutine.create(
        function(x)
            while true do
                print("consume:", x)
                x = coroutine.yield()
                if not x then
                break
                end
            end
        end
    )

    local function producer()
        for i = 1, 5 do
        print("produce:", i)
        coroutine.resume(consumer_co, i)
        print("consumer:", coroutine.status(consumer_co))
        end
        coroutine.resume(consumer_co)
    end

    producer()
    print("consumer:", coroutine.status(consumer_co))
end

--[[将协程用于迭代器]]
do
    -- 产生         全排列      的迭代器
    local function permutation_gen(a, n)
        n = n or #a
        if n < 1 then
            coroutine.yield(a)
        else
            for i = 1, n do
                a[n], a[i] = a[i], a[n]
                permutation_gen(a, n - 1)
                a[n], a[i] = a[i], a[n]
            end
        end
    end

    local function permutations(a)
        local co = coroutine.create(function() permutation_gen(a) end)
        return function()
            local code, res = coroutine.resume(co)
            return res
        end
    end

    --[[
        coroutine.warp(func)方法就是创建一个封装func的协程，然后返回一个调用该协程的函数。
    ]]
    local function permutations_wrap(a)
        return coroutine.wrap(function() permutation_gen(a) end)
    end

    for p in permutations({1, 2, 3}) do
        for i = 1, #p do io.write(p[i], " ") end
        io.write("\n")
    end
    print("----")
    for p in permutations_wrap({1, 2, 3}) do
        for i = 1, #p do io.write(p[i], " ") end
        io.write("\n")
    end

end

--测试resume 1
do
    local co = coroutine.create(function (a)
        local r = coroutine.yield(a+1)                  -- yield 返回a+1的值2 给调用它的resume
        print("r="..r)                                  -- r的值是第2次resume()传进来的 100
    end)
    local status, r = coroutine.resume(co, 1)           -- resume的返回两个值，一个是自身的状态true，一个是yield的返回值2
    print(status, r)
    local st1, r1 = coroutine.resume(co, 100)            -- resume 返回true                 
    print(st1, r1)
    local st2, r2 = coroutine.resume(co, 2000)          -- resume 返回false             
    print(st2, r2)
end


--测试resume 2
do
    local co = coroutine.create(
        function ()
            coroutine.yield()
            coroutine.yield(1)
            return 2
        end
    )

    for i = 1, 4 do
        print("i: " .. i .. " call cor, res: ", coroutine.resume(co) )
    end
end

