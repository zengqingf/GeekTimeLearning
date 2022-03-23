--
-- DESCRIPTION
--
-- @COMPANY **
-- @AUTHOR **
-- @DATE ${date} ${time}
--

--[[
UnLua可以访问和扩展的内容
1.不使用原生lua和c++的交互，则UnLua和蓝图访问权限一致
2.C++中对象设置宏：UProperty[Blueprint  readonly/readwrite]  和 UFunction[Blueprint Callable]   蓝图可以访问，lua也可以访问
3.扩展C++函数, 指定UFunction[BlueprintImplementableEvent / BlueprintNativeEvent], 蓝图可以拓展，lua也可以扩展

lua可以访问蓝图： UMG(组件名，需要在Details中设置为Is Variable)  其他蓝图Actor, 关卡

lua访问静态方法和成员方法
    静态方法用 . 访问
    成员方法用 : 访问
]]

require "UnLua"

local Screen = require "Example.Screen"

local HelloLua_C = Class()

-- 所有绑定到Lua的对象初始化时都会调用Initialize的实例方法
function HelloLua_C:Initialize()
    local msg = [[
    Hello World!

    —— 本示例来自 "Content/Data/Script/Tutorial/01/HelloWorld.lua"
    ]]
    print(msg)
    Screen.Print(msg)

    print("访问C++中全局静态函数--蓝图可以访问--UnLua也可以访问: " .. UE4.UHelloLuaUtils.GetInt())
end

--function HelloLua_C:Initialize(Initializer)
--end

--function HelloLua_C:UserConstructionScript()
--end

--function HelloLua_C:ReceiveBeginPlay()
--end

--function HelloLua_C:ReceiveEndPlay()
--end

--function HelloLua_C:ReceiveTick(DeltaSeconds)
--end

--function HelloLua_C:ReceiveAnyDamage(Damage, DamageType, InstigatedBy, DamageCauser)
--end

--function HelloLua_C:ReceiveActorBeginOverlap(OtherActor)
--end

--function HelloLua_C:ReceiveActorEndOverlap(OtherActor)
--end

return HelloLua_C
