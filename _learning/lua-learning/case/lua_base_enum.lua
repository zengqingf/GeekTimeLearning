--[[
    ref: lua枚举 https://www.cnblogs.com/vsirWaiter/p/8183941.html
]]

local GameEnumTest = {}

function GameEnumTest.CreateEnumTable(tbl, index) 
    local enumtbl = {} 
    local enumindex = index or 0 
    for i, v in ipairs(tbl) do 
        enumtbl[v] = enumindex + i 
    end 
    return enumtbl
end

local TestEnumType = {
    "None",
    "First",
    "Seconed",
    "Threed",
    "Fourth",
    "Fifth",
    "Sixth"}

function GameEnumTest.OnInitOK()
    TestEnumType = GameEnumTest.CreateEnumTable(TestEnumType, -1)   --后面的参数为-1时，下面的打印结果为0123456，为0时打印结果为1234567
    print(TestEnumType.None,
        TestEnumType.First,
        TestEnumType.Seconed,
        TestEnumType.Fourth,
        TestEnumType.Fifth,
        TestEnumType.Sixth)
end

return GameEnumTest