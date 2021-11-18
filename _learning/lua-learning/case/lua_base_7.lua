--[[
    lua 闭包

    ref: js闭包https://www.cnblogs.com/sandaizi/p/11582488.html

    闭包函数：一个函数有权访问另外一个函数作用域中的变量

    区分匿名函数和闭包函数：
            闭包是站在作用域的角度上来定义的。 如果inner访问到outer作用域的变量，那么inner就是一个闭包函数。

    闭包可能存在的问题：
        1. 引用的外部作用域的变量可能发生改变；（不要在函数外部改变父函数内部的变量）
        2. 内存泄漏，外部函数已经调用结束，但是闭包引用外部变量未释放 （退出函数前，需要清理不使用的局部变量，否则被闭包引用的变量仍会保留在内存中）

    闭包解决问题：
        迭代器的实现（迭代器需要保留上一次成功调用的状态和下一次成功调用的状态，也就是他知道来自于哪里和将要前往哪里。闭包提供的机制可以很容易实现这个任务.）
        作为高阶函数的参数（table.sort的自定义排序的参数）



    ref:https://blog.csdn.net/shimazhuge/article/details/24841143

    Lua中的函数是带有词法定界（lexical scoping）的第一类值（first-class values）。
    第一类值：在Lua中函数和其他值（数值、字符串）一样，函数可以被存放在变量中，也可以存放在表中，可以作为函数的参数，还可以作为函数的返回值。
    词法定界：被嵌套的函数可以访问他外部函数中的变量。这一特性给Lua提供了强大的编程能力。
            当一个函数内部嵌套另一个函数定义时，内部的函数体可以访问外部的函数的局部变量，这种特征我们称作词法定界。
    UpValue：一个函数使用它函数体之外的局部变量（external localvariable）称为这个函数的upvalue。

    函数闭包：一个函数和它所使用的所有upvalue构成了一个函数闭包。

    闭包是一个内部函数，它可以访问一个或者多个外部函数的外部局部变量。每次闭包的成功调用后这些外部局部变量都保存他们的值（状态）。
    当然如果要创建一个闭包必须要创建其外部局部变量。所以一个典型的闭包的结构包含两个函数：一个是闭包自己；另一个是工厂（创建闭包的函数）。
    （支持闭包特性通常需要一个嵌套函数 , 通过执行嵌套函数来改变所在父函数的局部变量状态 , 父函数保存调用上下文状态 , 而嵌套函数负责修改状态的改变 . （简单来说就是得支持函数嵌套））


    Lua函数闭包与C函数的比较：
        Lua函数闭包使函数具有保持它自己的状态的能力，从这个意义上说，可以与带静态局部变量的C函数相类比。
        但二者有显著的不同：对Lua来说，函数是一种基本数据类型——代表一种（可执行）对象，可以有自己的状态；
        但是对带静态局部变量的C函数来说，它并不是C的一种数据类型，更不会产生什么对象实例，它只是一个静态地址的符号名称。
]]

do
    print("----------------------------------------------------")
    local function outer() 
        local result = {}
        for i = 1, 10 do
            result[i] = function ()
                print(i)
                return i
            end
        end
        return result
    end

    local tb = outer()
    for i, v in ipairs(tb) do
        print(i)
    end
    print("----------------------------------------------------")
end


--[[
    lua中table是引用传递
    test中置空table只是将指向originTb的指针置空了
]]
do
    print("----------------------------------------------------")
    local function test(inputTb)
        inputTb = nil
    end
    local originTb = {[1] = "test"}
    test(originTb)
    for i, v in pairs(originTb) do
        print(i, v)
    end
    print("----------------------------------------------------")

    local function test2(inputTb)
        while #inputTb > 0 do
            table.remove(inputTb, #inputTb)
        end
    end
    test2(originTb)
    for i, v in pairs(originTb) do
        print(i, v)
    end
    print("----------------------------------------------------")
end


--[[闭包改变了父函数的局部变量]]
do
    print("----------------------------------------------------")
    local function counter()
        local count = 0
        return function ()
            count = count + 1
            return count
        end
    end

    local func = counter()
    print(func())               --output: 1
    print(func())               --output: 2
    print(func())               --output: 3
    print("----------------------------------------------------")
end

do
    print("----------------------------------------------------")
    local function createCountdownTimer(second)
        local ms=second * 1000;
        local function countDown()
           ms = ms - 1;
           return ms;
         end
         return countDown;
     end
      
     local timer1 = createCountdownTimer(1);
     for i=1,3 do
        print(timer1());
     end
     print("------------");
     local timer2 = createCountdownTimer(1);
     for i=0,2 do
        print(timer2());
     end
     print("----------------------------------------------------")
end

do
    print("----------------------------------------------------")

    --[[
        基于对象的闭包实现方式：把需要隐藏的成员放在一张表里，把该表作为成员函数的upvalue

        局限：不涉及继承和多态
    ]]
    local function create(name,id)
        local data={name = name,id=id};
        local obj={};
        function obj.GetName()
          return data.name;
        end
        function obj.GetID()
           return data.id;
        end
        function obj.SetName(name)
           data.name=name;
        end
        function obj.SetID(id)
           data.id=id
        end
        return obj;
    end
     
    local o1 = create("Sam", 001)
    local o2 = create("Bob", 007)
    o1.SetID(100)
    print("o1's id:", o1.GetID(), "o2's id:",o2.GetID())
    o2.SetName("Lucy")
    print("o1's name:", o1.GetName(),"o2's name:", o2.GetName())
     
    --o1's id:        100     o2's id:        7
    --o1's name:      Sam     o2's name:      Lucy
    print("----------------------------------------------------")
end

do
    --反面递归例子（递归必须在初始化以后才能调用）
    local func = function(n)
        if n > 0 then 
            return func(n - 1) --此处调用错误
        end
    end
   
   --正确例子1
   local func = nil
   func = function(n)
        if n > 0 then 
            return func(n - 1) --此处调用错误
        end
   end
   
   --正确例子2(此处函数展开后解释为例子1的代码再执行)
   function func(n)
        if n > 0 then 
            return func(n - 1) --此处调用错误
        end
   end
   
   --如果是两个函数嵌套递归(超前递归,必须先声明)
   local g = nil
   local f = nil
   --这里不能加local..不然等于声明了多一个局部变量了,递归的对象就不对了
   function g()
     f()
   end
   --这里不能加local..不然等于声明了多一个局部变量了,递归的对象就不对了
   function f()
     g()
   end
end


--[[lua尾调用： 由于LUA尾递归调用这个性质,我们可以用GOTO来实现状态机了


Lua中函数的另一个有趣的特征是可以正确的处理尾调用（proper tail recursion，一些书使用术语“尾递归”，虽然并未涉及到递归的概念）。
尾调用是一种类似在函数结尾的goto调用，当函数最后一个动作是调用另外一个函数时，我们称这种调用尾调用。

lua 支持尾调用的特性"尾调用消除"：当在进行尾调用的时候不消耗任何栈空间，这种实现就称为尾调用消除。

函数调用自身，称为递归。如果尾调用自身，就称为尾递归。
尾递归由于只存在一个调用记录（空间复杂度O(1)），所以永远不会发生"栈溢出"错误。

ref: https://www.jianshu.com/p/9b388de0899b
一些编译器比如Lua解释器利用这种特性在处理尾调用时不使用额外的栈，我们称这种语言支持正确的尾调用。
由于尾调用不需要使用栈空间，那么尾调用递归的层次可以无限制的。例如下面调用不论n为何值不会导致栈溢出。
一些调用者函数调用其他函数后也没有做其他的事情但不属于尾调用。
Lua中类似return g(...)这种格式的调用是尾调用。但是g和g的参数都可以是复杂表达式，因为Lua会在调用之前计算表达式的值。
可以将尾调用理解成一种goto，在状态机的编程领域尾调用是非常有用的。状态机的应用要求函数记住每一个状态，改变状态只需要goto(or call)一个特定的函数。
如果没有正确的尾调用，每次移动都要创建一个栈，多次移动后可能导致栈溢出。但正确的尾调用可以无限制的尾调用，因为每次尾调用只是一个goto到另外一个函数并不是传统的函数调用。


在使用Lua进行开发的时候，能使用尾调用的时候尽量改写成尾调用，能使用尾递归的时候尽量改写成尾递归！能节省不少内存空间！
]]
do
    --尾调用消除
    function g()
        return a,b
    end
    
    --正确例子
    function f()
        return g() --正确的尾调用消除（调用g()后不需要返回到f()，即尾调用后程序不需要在栈中保留关于调用者的任何信息）
    end
    
    --错误例子1
    function f(x)
        return g() + 1 --最后执行的是加法
    end
    
    --错误例子2
    function f(x)
        return (g()) --最后执行的是强制返回1个值
    end
    
    --错误例子3
    function f(x)
        return x or g() 
    end

    function f(x)  --不属于尾调用
        g(x)
        return
    end
end

do
    local function f(n)
        if n <= 0 then
            return 0
        end
        local a = f(n - 1)
        return n * a
    end
    --! f(10000000000)  stack overflow

    local function f2(n, now)
        if n <= 0 then
            return now
        end
        return f2(n - 1, n * now)
    end
    --f2(10000000000, 1)  运行很久也不会溢出
end


--[[
迷宫有很多个房间，每个房间有东西南北四个门，每一步输入一个移动的方向，如果该方向存在即到达该方向对应的房间，否则程序打印警告信息。目标是：从开始的房间到达目的房间。
这个迷宫游戏是典型的状态机，每个当前的房间是一个状态。我们可以对每个房间写一个函数实现这个迷宫游戏，我们使用尾调用从一个房间移动到另外一个房间。一个四个房间的迷宫代码
]]
do
    function room4 ()
        print("congratilations!")
    end

    function room3 ()
        local move = io.read()
        if move == "north" then
           return room1()
        elseif move == "east" then
           return room4()
        else
           print("invalid move")
           return room3()
        end
    end

    function room2 ()
        local move = io.read()
        if move == "south" then
           return room4()
        elseif move == "west" then
           return room1()
        else
           print("invalid move")
           return room2()
        end
    end

    function room1 ()
        local move = io.read()
        if move == "south" then
           return room3()
        elseif move == "east" then
           return room2()
        else
           print("invalid move")
           return room1()   -- stay in the same room
        end
    end
    --start game
    --room1()
end