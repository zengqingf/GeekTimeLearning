--[[
    类和面向对象

    . 与 : 的区别在于使用 : 定义的函数隐含 self 参数，使用 : 调用函数会自动传入 table 至 self 参数
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