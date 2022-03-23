--
-- DESCRIPTION
--
-- @COMPANY **
-- @AUTHOR **
-- @DATE ${date} ${time}
--

require "UnLua"

local HelloLuaActor_C = Class()

--function HelloLuaActor_C:Initialize(Initializer)
--end

--function HelloLuaActor_C:UserConstructionScript()
--end

--function HelloLuaActor_C:ReceiveBeginPlay()
--end

--function HelloLuaActor_C:ReceiveEndPlay()
--end

function HelloLuaActor_C:ReceiveTick(DeltaSeconds)
    --print("Hello Lua Actor, lua调用蓝图C++类的成员函数 ：" .. self:GetIndex())
end

--function HelloLuaActor_C:ReceiveAnyDamage(Damage, DamageType, InstigatedBy, DamageCauser)
--end

--function HelloLuaActor_C:ReceiveActorBeginOverlap(OtherActor)
--end

--function HelloLuaActor_C:ReceiveActorEndOverlap(OtherActor)
--end

return HelloLuaActor_C
