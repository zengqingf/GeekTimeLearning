--
-- DESCRIPTION
--
-- @COMPANY **
-- @AUTHOR **
-- @DATE ${date} ${time}
--


--[[
    说明：覆盖蓝图事件时，只需要在返回的table中声明 Receive{EventName}

    例如：
    function M:ReceiveBeginPlay()
    end

    除了蓝图事件可以覆盖，也可以直接声明 {FunctionName} 来覆盖Function。
    如果需要调用被覆盖的蓝图Function，可以通过 self.Overridden.{FunctionName}(self, ...) 来访问


    例如：
    function M:SayHi(name)
        self.Overridden.SayHi(self, name)
    end
    注意：这里不可以写成 self.Overridden:SayHi(name)
]]



require "UnLua"

local Screen = require "Example.Screen"

local OverrideBlueprintEvents_C = Class()

--function OverrideBlueprintEvents_C:Initialize(Initializer)
--end

--function OverrideBlueprintEvents_C:UserConstructionScript()
--end

--
function OverrideBlueprintEvents_C:SayHi(Name)
    --local origin = self.Overridden.SayHi(self, Name)
    --return origin .. "\n\n" ..
    --[[这是来自lua的问候

    —— 本示例来自 "Content/Data/Script/Tutorial/02/OverrideBlueprintEvents.lua"
    ]]
end

function OverrideBlueprintEvents_C:ReceiveBeginPlay()
    print("OverrideBlueprintEvents_C:ReceiveBeginPlay")
    local msg = self:SayHi("陌生人")
    Screen.Print(msg)
end

--function OverrideBlueprintEvents_C:ReceiveEndPlay()
--end

-- function OverrideBlueprintEvents_C:ReceiveTick(DeltaSeconds)
-- end

--function OverrideBlueprintEvents_C:ReceiveAnyDamage(Damage, DamageType, InstigatedBy, DamageCauser)
--end

--function OverrideBlueprintEvents_C:ReceiveActorBeginOverlap(OtherActor)
--end

--function OverrideBlueprintEvents_C:ReceiveActorEndOverlap(OtherActor)
--end

return OverrideBlueprintEvents_C
