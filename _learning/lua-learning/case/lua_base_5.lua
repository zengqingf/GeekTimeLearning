--[[
    类和面向对象

    . 与 : 的区别在于
    使用 : 定义的函数隐含 self 参数，
    使用 : 调用函数会自动传入 table 至 self 参数，相当于c++中的this指针，指向当前对象

    在lua中模拟面向对象编程中，可以用 . 来表示类方法，而 ： 可以用来表示成员方法。



    self 与 local 的区别在于
    self 指向类（即表）的实例，self可以修改表里的内容
    local 指代局部变量，不属于类（即表），且仅在当前作用域内生效
]]

do
    Rectangle = {area = 0, length = 0, breadth = 0}

    function Rectangle:new(o, length, breadth)
        o = o or {}
        setmetatable(o, self)
        self.__index = self
        self.length = length or 0
        self.breadth = breadth or 0
        self.area = length * breadth
        return o
    end

    function Rectangle:printArea()
        print("rect area: ", self.area)
    end

    local r = Rectangle:new(nil, 10, 20)            --内存在对象初始化时分配
    print(r:printArea())                            --访问成员函数使用 :
    print(r.length)                                 --访问属性（成员变量）


    --优化：每次new新实例的时候都需要将第一个变量的值设为nil
    function Rectangle:new(len, wid)
        local o = {
            length = len or 0,
            width = wid or 0,
            area = len * wid
        }
        setmetatable(o, {__index = self})          --将自身的表映射到新new的表中
        return o
    end

    function Rectangle:getInfo()
        return self.length, self.width, self.area
    end

    local r2 = Rectangle:new(10, 20)
    print(r2:getInfo())

end


--继承
do
    Shape = {area = 0}
    function Shape:new(o, side)
        o = o or {}
        setmetatable(o, self)
        self.__index = self
        side = side or 0
        self.area = side * side
        return o
    end

    function Shape:printArea()
        print("area is ", self.area)
    end

    local myShape = Shape:new(nil , 10)
    myShape:printArea()



    Square = Shape:new()
    function Square:new(o, side)
        o = o or Shape:new(o, side)
        setmetatable(o, self)
        self.__index = self
        return o
    end

    function Square:printArea()
        print("square area is ", self.area)
    end

    local mySquare = Square:new(nil, 10)
    mySquare:printArea()



    Rectangle = Shape:new()
    -- 派生类方法 new
    function Rectangle:new (o,length,breadth)
    o = o or Shape:new(o)
    setmetatable(o, self)
    self.__index = self
    self.area = length * breadth
    return o
    end

    -- 派生类方法 printArea
    function Rectangle:printArea ()
    print("rect area is ",self.area)
    end

    -- 创建对象
    local myrectangle = Rectangle:new(nil,10,20)
    myrectangle:printArea()
end

do
    --[[
    等价：
    function class:func() end
    function class.func(self) end
    ]]

    --[[
        function class:ctor()
            self:test(1,2,3)
        end

        function class:test(a, b, c)
            print(a, b, c)
        end

        output:  1 2 3
    ]]

    --[[
        function class:ctor()
            self:test(1,2,3)
        end

        function class.test(a, b, c)
            print(a, b, c)
        end

        output:  userdata 1 2      使用:调用方法时，默认传递self为第一个参数，但是函数声明时使用. 不会有默认隐式self去接收传递进来的self, 3被舍弃
    ]]

    --[[
        function class:ctor()
            self.test(1,2,3)
        end

        function class.test(a, b, c)
            print(a, b, c)
        end

        output: 1 2 3
    ]]

    --[[
        function class:ctor()
            self.test(1,2,3)
        end

        function class:test(a, b, c)
            print(self)
            print(a,b,c)
        end

        output: 1               使用.调用方法时，不会传递self, 只传递1 2 3，  但是函数声明使用: 默认第一位有一个self去接收，即上述test()可以接收4个参数 self a b c
                2 3 nil         c没有接收到参数
    ]]
end