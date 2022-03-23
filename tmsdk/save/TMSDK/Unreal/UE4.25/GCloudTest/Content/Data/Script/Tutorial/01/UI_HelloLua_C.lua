--
-- DESCRIPTION
--
-- @COMPANY **
-- @AUTHOR **
-- @DATE ${date} ${time}
--

require "UnLua"

local UI_HelloLua_C = Class()

--function UI_HelloLua_C:Initialize(Initializer)
--end

--function UI_HelloLua_C:PreConstruct(IsDesignTime)
--end

function UI_HelloLua_C:Construct()
    print("lua访问创建出来的蓝图UMG中的全局变量：" .. self.Title)

    -- 调用蓝图中的自定义事件
    --self:ShowButton()

    -- 播放UMG中定义的UI动画，效果同上
    self:PlayAnimation(self.HelloLuaBtnAnim, 0, 1)

    --添加UMG中的按钮事件
    self.ButtonHelloLua.OnPressed:Add(self, UI_HelloLua_C.OnHellLuaBtnClick)
end

--function UI_HelloLua_C:Tick(MyGeometry, InDeltaTime)
--end

function UI_HelloLua_C:OnHellLuaBtnClick()
    local world = self:GetWorld()
    if not world then
        return
    end
    --在C++中创建路径需要 Blueprint'/Game/Blueprint/Tutorial/01_HelloLuaWorld/HelloLuaActor.HelloLuaActor_C'
    local actorClass = UE4.UClass.Load("/Game/Blueprint/Tutorial/01_HelloLuaWorld/HelloLuaActor.HelloLuaActor")
    local actor = world:SpawnActor(actorClass, FVector(), UE4.ESpawnActorCollisionHandlingMethod.AlwaysSpawn, self, self, "")
    self.Actor = actor

    print("Hello Lua Actor' Name : " .. self.Actor.Name)
end

return UI_HelloLua_C
