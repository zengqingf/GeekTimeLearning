---[[
print("hello world")
--]]
--[[
    print("hello world")
--]]

do

    local function norm (x, y)
        return math.sqrt(x^2 + y^2)
    end

    local function twice(x)
        return 2.0 * x
    end

    --进入交互模式  lua -i
    --交互模式下  dofile("lib1.lua")
    --          n = norm(3.4, 1.0)
    --          twice(n)

    local a = 1; local b = a * 2    --分号作为分隔符
end

do
    G_B = 10
    local b = 10
    b = nil        --回收变量
    print(b)
end