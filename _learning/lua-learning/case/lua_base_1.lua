--单行注释

--[[多行注释1
    多行注释2]]

--[[
多行注释1
多行注释2
--]]


-- 代码块
do
    print("hello")
end

do
    -- return 一定要在end之前
    -- return
end
--! return 单独不能使用return

--打印
do 
    print("hello world")
    --字符串打印
    print(1 .. 2)
    print("2" .. 3)
end -- do end block

--[[
变量

lua变量默认为全局变量，即使在函数中声明，也是全局的，除非使用local前置声明，才为局部变量

局部变量作用域为从声明开始到所在语句块结束

变量默认值都为nil

赋值
当变量个数和值的个数不一致时，Lua会一直以变量个数为基础采取以下策略
a. 变量个数 > 值的个数             按变量个数补足nil
b. 变量个数 < 值的个数             多余的值会被忽略

@注意：尽可能使用局部变量！！！
    避免命名冲突
    访问局部变量速度快于全局变量
]]

print("################ variable start ###############")

--局部变量
do
    local i = 1
    --允许同时赋值
    local j, k = 3, 4

    J, K = K, J                   -- 多值赋值语句特性（先计算=右边的值，再执行赋值操作），进行变量交换
    function Return()
        return 1, "test"
    end
    local key, value = Return()   -- 多值赋值 用于接收函数多个返回值

    --多值赋值注意，需要依次对每个变量赋值
    local a, b, c = 0
    print(a,b,c)             --> 0   nil   nil
    a, b, c = 0, 0, 0
    print(a,b,c)             --> 0   0   0

    function Joke()
        Sex = 5    -- 为全局变量
        local age = 10
    end

    print(Sex, age) -- output: 5 nil

    --全局变量（推荐大写）
    Name = "mjx"
    do
        local Name = "mjj"
        Age = 2
        print(Name, Age)
    end
    print(Name, Age)


    -- 虚变量  表现为："_" 下划线
    -- 用于存储丢弃不需要的数值
    -- 用一些不太会用到的名字命名一个回收箱变量，然后把不需要的值都丢到里面去，这种做法被称作虚拟变量法。
        --当一个方法返回多个值时，有些返回值有时候用不到，要是声明很多变量来一一接收，显然 不太合适（不是不能）。Lua 提供了一个虚变量(dummy variable)，以单个下划线('_')来命 名，用它来丢弃不需要的数值，仅仅起到占位的作用。
    do
        print(string.find("example", "am"))   -- output 3 4

        local _, s = string.find("example", "am")
        print(s)  -- 4
        print(_)  -- 3  存储了不需要的变量值
    end

end -- do end block

print("################ variable end ###############")


print("################ operator start ###############")

--[[
运算符

优先级： 从高到低
        ^
        not    - (unary)
        *      /       %
        +      -
        ..
        <      >      <=     >=     ~=     ==
        and
        or

除 ^ 和 .. 以外的二元运算符都是左连接的
]]

do
    --算术运算符
    print(1 + 2)
    print(1 - 2)
    print(1 * 2)
    print(1 / 2)
    print(1 ^ 2) -- 乘方 幂次
    print(1 % 2) -- 取余

    --比较运算符
    print(1 < 2)
    print(1 > 2)
    print(1 <= 2)
    print(1 >= 2)
    print(1 == 2)
    print(1 ~= 2)  -- @注意： not !=

    --[[
        lua中 只有false 和 nil 才为 false, 其他数据都是true, 0也是true !!! 
        and 和 or 运算结果不是true 和 false， 而是和它的操作数有关， 类似于 xx ？ is true ：is false
    ]]
    -- and 与
    -- a and b, a为false，则返回a; 否则返回b
    print(1 and 2)
    -- or 或
    -- a or b，a为true，则返回a；否则返回b
    print(1 or 2)
    -- not 非
    print(not(1 and 2)) -- output: false

    --[[实现三目运算
        使用 and or 模拟三目运算符（通过and or的短路求值特性，只有在需要时才去评估第二个操作数）

        ref:    https://blog.csdn.net/coffeecato/article/details/77546887
    ]]
    local isSucc = false
    print(isSucc and "succ" or "failed")

    --[[ a > b and a or b
    
        b = true
            a = true
                a and b –> true
                b or c –> b
            a = false
                a and b –> false
                b or c –> c
        b = false
            a = true
                a and b –> false
                b or c –> c
            a = false
                a and b –> false
                b or c –> c

        当b = false时，Lua模拟的a and b or c始终返回c并不能还原三目运算符的原貌。

        使用 (a and {b} or {c})[1] 替换   用table包装b和c能保证b和c都为true 但是用table包装由性能损耗  
    ]]


    --[[
      # 获取长度 :  获取一个数值键值（@注意：键值从1开始！！！）的表(table)的长度；
                    当获取的table中存在nil时，获取长度无意义
                    只能获取完全数值表的长度，并且从1开始计数
    ]]
    do
        local tab = {}
        tab[1] = "a"
        tab[2] = "b"
        tab[4] = "d"
        print("tab len is " .. #tab)   --> 4
        tab[5] = nil                   --任何一个nil 值都有可能被当成数组的结束
        tab[6] = "f"
        print("tab len is " .. #tab)   --> 4


        --如果table中存在nil，则#获取长度不正确
        local t1 = {"a", "b", nil, "c", nil}
        print(#t1)  --==>  2   not   4  or 3 or 5
        local t11 = {"a", "b", nil, "c", "nil"}
        print(#t11) --==>  5   not 2                    @注意：这里竟然是长度5
        local t111 = {"a", "b", "nil", "c", nil}
        print(#t111) --==>  4   not 5

        --完全数值表
        local t2 = {
            [0] = "0",
            [1] = "1",
            [2] = "2",
            [3] = "3"
        }
        print(#t2)  --==>  3  not 4

        --非完全数值表
        local t3 = {
            [1] = "1",
            a = "1",
            b = "2",
            c = "3"
        }
        print(#t3)  --==> 1 not  4
    end

end -- do end block

print("################ operator end ###############")


--[[
判断和循环
]]
print("################ loop and if start ###############")

do 
    N = 10
    while( N < 20)
    do
        print("while loop, N:".. N)
        N = N+1
    end


    --- lua不支持 continue ： 原因：https://www.luafaq.org/#T1.26
    --[[
        lua中repeat until ~~  c++ do while, 但是机制上有区别：
            在Lua的until的条件表达式中，表达式中的变量可以是repeat until代码块内声明的局部变量，
            但在C++中，while的条件表达式中的变量不允许出现do while内部声明的临时变量，必须是do while外部声明的变量。
    
        local a = 1 --outer
        repeat
            if f() then
                continue
            end
            local a = 1  --inner
        until a == 0     --如果存在continue 跳过内部变量a的定义时，until使用a会产生歧义
    ]]
    --continue 替代方法
    for i = 1, 10 do
        repeat                  --repeat until 这一块相当于 continue的作用
            if i == 5 then
                break       
            end
            print("lua continue test: ".. i)
        until true              
    end

    --- for的三个表达式在循环开始前一次性求值，以后不再进行求值。比如上面的f(x)只会在循环开始前执行一次，其结果用在后面的循环中。
    --- for中 第三个为步长， 默认为1， 可默认不填
    function ForLoop(x)
        print("Call For Loop")
        return x * 2
    end

    -- @注意： for 循环中，循环的索引 i 为外部索引，修改循环语句中的内部索引 i，不会影响循环次数:
    for i=1, ForLoop(5), 2 
    do
        i = i * 2
        print("for loop, step len = 2, i (not index) value:".. i)
    end

    -- 泛型for
    --[[

迭代的下标是从1开始的!!!

        pair vs. ipair
        pairs可以遍历表中所有的key，并且除了迭代器本身以及遍历表本身还可以返回nil;
        ipairs则不能返回nil,只能返回数字0，如果遇到nil则退出。
        ipairs 这个迭代器只能遍历所有数组下标的值，这是前提，也是和 pairs 的最根本区别，也就是说如果 ipairs 在迭代过程中是会直接跳过所有手动设定key值的变量。

        pairs: 迭代 table，可以遍历表中所有的 key 可以返回 nil
        ipairs: 迭代数组，不能返回 nil,如果遇到 nil 则退出

    ]]
    do
        local a = {"one", "two", "three"}
        for i, v in ipairs(a)           
        do
            print(i, v)
        end

        local b = {"one", "two", ["key"]=1, "three", nil, "four"}  --循环到["key"]时会跳过，循环到nil时会中断
        for i, v in ipairs(b)           
        do
            print(i, v)
        end
    end

    --#region 迭代器 （无状态、有状态）

    --[[ 三个关键
    迭代函数
    状态常量
    控制变量
    ]]

    do
        local array = {"google", "baidu"}
        for key,value in ipairs(array)     --泛型for迭代器
        do
            print(key, value)
        end
    end

    local function square(iteratorMaxCount,currentNumber)
        if currentNumber<iteratorMaxCount
        then
           currentNumber = currentNumber+1
        return currentNumber, currentNumber*currentNumber
        end
     end
    -- 无状态迭代器（不保留状态的迭代器，避免创建闭包花费额外代价）
    -- 每一次迭代，迭代函数都是用两个变量（状态常量和控制变量）的值作为参数被调用，一个无状态的迭代器只利用这两个值可以获取下一个元素。
    --                                  这种无状态迭代器的典型的简单的例子是 ipairs，它遍历数组的每一个元素，元素的索引需要是数值。
    --                                  迭代的状态包括被遍历的表（循环过程中不会改变的状态常量）和当前的索引下标（控制变量）
     for i,n in square,3,0
     do
        print(i,n)
     end

     -- 自实现ipairs
    do
        --迭代函数
        local function iter(a, i)
            i = i + 1
            local v = a[i]
            if v then
                return i, v
            end
        end

        local function ipairs(a)
            return iter, a, 0       -- 返回迭代函数iter  状态常量a   控制变量初始值0
        end

        --[[
            分析：当 Lua 调用 ipairs(a) 开始循环时，他获取三个值：迭代函数 iter、状态常量 a、控制变量初始值 0；
            然后 Lua 调用 iter(a,0) 返回 1, a[1]（除非 a[1]=nil）；第二次迭代调用 iter(a,1) 返回 2, a[2]……直到第一个 nil 元素。
        ]]
    end

    --多状态迭代器
    --[[
        迭代器需要保存多个状态信息而不是简单的状态常量和控制变量，最简单的方法是使用闭包，
        还有一种方法就是将所有的状态信息封装到 table 内，将 table 作为迭代器的状态常量，
        因为这种情况下可以将所有的信息存放在 table 内，所以迭代函数通常不需要第二个参数。
    ]]
    do
        local array = {"google", "baidu"}
        local function elementIterator(collection)
            local index = 0
            local count = #collection
            -- 闭包函数(匿名)
            return function ()
                index = index + 1
                if index <= count
                then
                    -- 返回迭代器当前元素
                    return collection[index]
                end
            end
        end

        -- 验证迭代函数和闭包函数在循环中的调用次数
        for element in elementIterator(array)
        do
            print(element)
        end

        local function eleiter(t)
            local index = 0
            print('in eleiter function')  --> 每次调用迭代函数都说一句：in eleiter function
            return function()
                print('I am here.')       --> 每次调用闭包函数都说一句：I am here
                index = index + 1
                return t[index]
            end
        end
        
        local t = {'one','two','three','four','five'}
        for ele in eleiter(t) do
            print(ele)
        end
    end

    do
        print("----------------------------------------------------")
        --创建迭代器：(一定要注意迭代器只是一个生成器，他自己本身不带循环)
        local function list_iter(t)
            local i=0
            --local n=table.getn(t)
            local n = #t
            return function()
                i=i+1
                if i<=n then 
                    return t[i] 
                end
            end
        end
        --这里的list_iter是一个工厂，每次调用都会产生一个新的闭包该闭包内部包括了upvalue(t,i,n)
        --因此每调用一次该函数产生的闭包那么该闭包就会根据记录上一次的状态，以及返回list的下一个
        --使用迭代器,while中使用：
        local t={10, 20, 90}
        local iter = list_iter(t)   --调用迭代器产生一个闭包
        while true do
            local element=iter()
            if element==nil then 
                break 
            else
                print(element)
            end
        end
        --使用迭代器,泛型for使用：
        local t = {10, 0, 29}
        for element in list_iter(t) do --这里的list_iter()工厂函数只会被调用一次产生一个闭包函数，后面的每一次迭代都是用该闭包函数，而不是工厂函数
            print(element)
        end
        print("----------------------------------------------------")
    end

    --#endregion  迭代器


    do
        -- 判断语句
        if(0)
        then
            print("0 is true")
        elseif(nil)
        then
            print("nil is false")
        else
            print("true or !nil is true")
        end
    end

end -- do end block

print("################ loop and if end ###############")

--[[
lua数据类型

    nil       空值，所有没有使用过的变量都是nil；nil既是值，也是数据类型; 条件判断为false
    boolean   true or false
    number    相当于c中的double，双精度浮点数
    string    可以包含 "\0" 的字符串，可以使用单引号或者双引号表示
    table     关系类型
    function  函数类型，可以用函数类型声明变量，可以由C或lua编写
    userdata  与lua宿主交互，宿主通常为c/c++，userdata可以为任意宿主类型，常用为struct和pointer
    thread    线程类型，lua中没有真正的线程， 为协程（和线程一样，拥有独立的栈、局部变量和指令指针，可以和其他协同程序共享全局变量和其他大部分内容）
                                                 （和线程同时运行多个不同，协程只能运行一个，并且处于运行状态的协程被挂起（suspend）时才能被暂停）
]]

print("################ type start ###############")

do 
    print(type("hello world"))
    print(type(print))
    print(type(nil))
    print(type(type(X)))
end -- do end block

print("################ type end ###############")



-- 函数
--[[
函数
第一类值（First-Class Value） 
可以存在变量里，变量当成函数使用
可以作为匿名函数，通过参数传递到函数里
]]
print("################ func start ###############")

do 
    --局部方法
    local function add(x, y)
        return x+y, x*y      --支持多个返回值
    end

    function Sum(v1, v2)
        return v1 + v2
    end

    -- 声明函数变量
    local mul = function (v1, v2)
        return v1 + v2
    end

    print(Sum(2, 3))
    print(mul(3, 4))

    function Factorial1(n)
        if n == 0 then
            return 1
        else
            return n * Factorial1(n - 1)
        end
    end
    print(Factorial1(5))
    Factorial2 = Factorial1  -- 函数直接赋值给变量 变量当成函数使用
    print(Factorial2(5))

    function TestFunc(tab, func)
        for k, v in pairs(tab) do
            print(func(k, v))
        end
    end

    local tab = {key1 = "a", key2 = "b"}
    TestFunc(tab, 
        function(key, val)      --作为匿名函数传递到函数中
            return key.."="..val
        end
    )


    --- 函数作为参数传递
    local myprint = function(param)
        print("这是打印函数 -   ##", param ,"##")
    end
    local function add(num1,num2,functionPrint)
        local result = num1 + num2
        -- 调用传递的函数参数
        functionPrint(result)
    end
    myprint(10)
    -- myprint 函数作为参数传递
    add(2,5,myprint)

    print("----------------------------------------------------")

    --- 多返回值
    local s, e = string.find("www.baidu.com", "baidu")
    print(s, e)

    --[[
代码一把函数的结果存放在临时变量里再作为参数传给其它函数，代码二直接将函数返回值作为参数传给其它函数。
看上去，代码二比代码一简短了一些，少用了个变量名。
可是，如果myfunc返回多个值的话，代码二将不能正确运行，因为myfunc的所有返回值均会传递给table.insert。和其它语言完全不一样。
lua返回的不是像其它语言那样的是一种复合类型的值，而真真正正的是多个值，以一种超乎直觉的方式存在着。
    ]]
    local function myFunc()
        return 1, 2, 3
    end
    local t = {}
    local item = myFunc()
    table.insert(t, item)
    item = nil
    local t2 = {}
    --!table.insert(t, myFunc())       --wrong number of arguments to 'insert'
    print("----------------------------------------------------")


    ---可变参数
    local function add(...)
        local s = 0
          for i, v in ipairs{...} do   --> {...} 表示一个由所有变长参数构成的数组
            s = s + v
          end
          return s
    end  
    print(add(3,4,5,6,7))  --->25

    local function average(...)
        local result = 0
        local arg={...}    --> arg 为一个表，局部变量
        for i,v in ipairs(arg) do
           result = result + v
        end
        print("总共传入 " .. #arg .. " 个数")
        return result/#arg
    end
    print("平均值为",average(10,5,3,4,5,6))

    --通过select("#",...) 获取可变参数数量
    local function average(...)
        local result = 0
        local arg={...}
        for i,v in ipairs(arg) do
           result = result + v
        end
        print("总共传入 " .. select("#",...) .. " 个数")
        return result/select("#",...)
    end
    print("平均值为",average(10,5,3,4,5,6))

    --固定参数加上可变参数，固定参数必须放在变长参数之前
    local function fwrite(fmt, ...)  ---> 固定的参数fmt
        return io.write(string.format(fmt, ...))
    end
    fwrite("baidu\n")        --->fmt = "baidu", 没有变长参数
    fwrite("%d%d\n", 1, 2)   --->fmt = "%d%d", 变长参数为 1 和 2

    --[[
        select
    通常在遍历变长参数的时候只需要使用 {…}，然而变长参数可能会包含一些 nil，
    那么就可以用 select 函数来访问变长参数了：select('#', …) 或者 select(n, …)
        select('#', …) 返回可变参数的长度。
        select(n, …) 用于返回从起点 n 开始到结束位置的所有参数列表。
    调用 select 时，必须传入一个固定实参 selector(选择开关) 和一系列变长参数。
    如果 selector 为数字 n，那么 select 返回参数列表中从索引 n 开始到结束位置的所有参数列表，
    否则只能为字符串 #，这样 select 返回变长参数的总数。
    ]]
    local function f(...)
        local a = select(3, ...)  --从第三个位置开始，只接收一个参数到a中
        print(a)
        print(select(3, ...))     --返回的是多个参数，而不是一个table
    end
    f(0, 1, 2, 3, 4, 5)

    do
        local function foo(...)
            for i = 1, select('#', ...) do  -->获取参数总数
                local arg = select(i, ...)  -->读取参数，arg 对应的是右边变量列表的第一个参数
                print("arg", arg)
            end
        end
        foo(1, 2, 3, 4)
    end

    -- 多返回值函数进行赋值时，仅仅只有在所有逗号后的那个函数才会展开所有返回值
    local function add()
        return 1,0
    end
    local b,c,d,e,f,g = add(),add(),add()
    print(b) -- 1
    print(c) -- 1
    print(d) -- 1
    print(e) -- 0
    print(f) -- nil
    print(g) -- nil
end -- do end block

print("################ func end ###############")



-- 类
print("################ class start ###############")
do

    Lewis = {
        age = 24,
        work = function(self, msg)
            -- 函数体
            self.age = self.age + 1
            print(self.age .. msg)             --[[
                                                        --print("### base frame: " .. self.test1) --无法访问base class local variable
                                                        print("### base frame: " .. self.test2) --可以访问base class self variable
                                                    ]]
        end
    }

    -- 调用1
    print(Lewis.age)
    Lewis.work(Lewis, "上班1")

    -- 调用2
    Lewis:work("上班2")

    print("----------------------------------------------------")
    -- Lua中的self ref: https://zhuanlan.zhihu.com/p/115159195
    do
        local t = {a = 1, b = 2}
        function t:Add()
            return (self.a + self.b)
        end
        print(t:Add())
    end

    do
        local t = {a = 1, b = 2}
        function t:Add()
            return (self.a + self.b)
        end
        function t.Sub(self)
            return (self.a - self.b)
        end
        print(t.Add(t))
        print(t:Sub())
    end

    do
        local tA = {a = 1, b = 2}
        function tA.Add(self)
            return (self.a + self.b)
        end
        print(tA.Add(tA))
    end

    do
        local tB = {a = 1, b = 2}
        function tB:Add()
            return (self.a + self.b)
        end
        print(tB:Add())
    end
    print("----------------------------------------------------")

end -- do end block
print("################ class end ###############")



--[[
nil使用
    nil 可用于清空全局变量和table  （可以触发lua垃圾回收）
    
    nil 用于type(XX)的比较时，需要使用 "nil"
]]
print("################ nil test start ###############")
do 
    local m = nil
    local n
    print(m == n)        -- 变量m和n为nil 不存在

    type(X)
    print(type(X) == nil)
    print(type(X) == "nil") -- 返回的为字符串

    local table_3 = {key1 = "v", key2 = "i", "y"}
    for k, v in pairs(table_3) do
        print(k .. " - " .. v)
    end
    --删掉table中key1
    table_3.key1 = nil
    for k, v in pairs(table_3) do
        print(k .. " - " .. v)
    end

end -- do end block
print("################ nil test end ###############")


--[[
关系类型 table / array

@注意：lua中table默认初始索引一般以1开始 ！！！
table不会固定长度，会自动增长

table分为   值索引  和  字符串索引
    字符串索引有两种表示方法：a["name"] === a.name
]]
print("################ table start ###############")
do

    --初始化空table
    local table_1 = {}   
    table_1[0] = 34
    table_1["name"] = "mjx"
    table_1.gender = "male"
    table_1["son"] = {name = "dd", gender = "male"}
    print(table_1.son.name)
    print(table_1)

    --初始化表
    local table_2 = {

        10,  -- [1] = 10
        [100] = 40,

        lewis = {       --可以写成 ["lewis"] = 
            age = 34,
            gender = "male",
        },

        20, -- [2] = 20
    }
    print(table_2[2])
    table_2["key"] = "value"
    print(table_2.key)          --当索引为字符串类型时，支持 . 访问索引（本质上是函数调用 getIndex(table_2.key)）
    print(table_2)

    -- table默认序号从 1 开始
    local table_4 = {"a", "b", "c", "d"}
    for k, v in pairs(table_4) do
        print("key", k)
    end

    local newTable_4 = table_4  --指向同一个table
    newTable_4 = nil            --清空newtable4  触发GC
    print("key "..table_4[4])
    table_4 = nil               --清空table4

    --table内置函数
    do
        local fruits = {"banana", "orange", "apple"}        
        print("concat str:", table.concat(fruits))
        print("concat str:", table.concat(fruits, ","))
        print("concat str:", table.concat(fruits, ",", 2, 3))   --输出一个字符串，由列表中元素连接而成

        table.insert(fruits, "mango")
        print(fruits[4])
        table.insert(fruits, 2, "grapes")
        print(fruits[2])
        print(fruits[5])
        table.remove(fruits)  -- 移除最后一个元素
        print(fruits[5])

        --排序
        for k, v in ipairs(fruits) 
        do
            print(k, v)
        end
        table.sort(fruits)
        for k, v in ipairs(fruits) 
        do
            print(k, v)
        end

        --排序自定义规则
        local t1 =                          --【1】
        {
            [1] = {A = 5, B = 3},
            [2] = {A = 6, B = 2},
            [3] = {A = 7, B = 1},
            [4] = {A = 8, B = 0},
            [5] = {A = 9, B = -1},
        }
        local t11 =                        --【2】 注意这个不等价于【1】
        {
            {A = 5, B = 3},
            {A = 6, B = 2},
            {A = 7, B = 1},
            {A = 8, B = 0},
            {A = 9, B = -1},
        }
        table.sort(t1, function(a, b)
            return a.A > b.A
        end)   -- (a, b) return true 则a在b前，否则相反
        for k, v in pairs(t1)
        do
            for k2, v2 in pairs(v) do
                print(k, k2, v2)                -- TODO 输出结果不对 ！ 不能sort使用 [1] = {...} 的table
            end
        end
        for k, v in pairs(t11)
        do
            for k2, v2 in pairs(v) do
                print(k, k2, v2)                -- TODO 输出结果正常
            end
        end

        local test2 ={                    
            {id=1, name="deng"},
            {id=9, name="luo"},
            {id=2, name="yang"},
            {id=8, name="ma"},
            {id=5, name="wu"},
        }
        table.sort(test2,function(a,b) return a.id<b.id end )
        for i in pairs(test2) do
        print(test2[i].id,test2[i].name)         -- 输出结果正常
        end

        local t2 ={2,3,5,52,6,74,4}
        table.sort(t2, function (a, b)
            return a > b
        end)
        for k, v in ipairs(t2) do print(k, v) end

        --！table.maxn() lua 5.2后已废弃
        local function table_maxn(t)
            local mn = nil
            for k, v in pairs(t)
            do
                if mn == nil then
                    mn = v
                end
                if mn < v then
                    mn = v
                end
            end
            return mn
        end
        local tbl = {[1] = 2, [2] = 6, [3] = 34, [26] =5}
        print("tbl max value: ", table_maxn(tbl))
        print("tbl length: ", #tbl)

        -- 获取table长度
        --[[
            当我们获取 table 的长度的时候无论是使用 # 还是 table.getn 其都会在索引中断的地方停止计数，而导致无法正确取得 table 的长度。
        ]]
        -- 替代方法
        local function table_len(t)
            local len = 0
            for k, v in pairs(t) do
                len = len + 1
            end
            return len
        end


    end


    --[[
    数组
    ]]
    do
        local array = {"lua", "tutorial"}
        for i=0, #array
        do
            print(array[i])   --i==0, return nil
        end
    end

    do
        local array = {}
        for i=-2, 2
        do
            array[i] = i * 2
        end

        for i=-2, #array
        do
            print(array[i])
        end
    end

    --多维数组
    do
        local array = {}
        for i=1,3 
        do
            array[i] = {}
            for j=1,3 
            do
                array[i][j] = i*j
            end
        end

        for i=1,3
        do
            for j=1,3
            do
                print(array[i][j])
            end
        end
    end

    do
        local array = {}
        local maxRows = 3
        local maxColumns = 3
        for row=1, maxRows 
        do
            for col=1, maxColumns
            do
                array[row * maxColumns + col] = row * col
            end
        end

        for row=1, maxRows do
            for col=1, maxColumns do
                print(array[row * maxColumns + col])
            end
        end
    end


end -- do end block
print("################ table end ###############")