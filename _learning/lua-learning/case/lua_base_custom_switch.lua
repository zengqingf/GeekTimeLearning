--[[
    lua 自定义switch
]]

--传入 string
do
    --ref: https://zhuanlan.zhihu.com/p/85780770
    ---以匿名函数实现
    local _switch_anonymous = {
        ["1"] = function(msg)
            print("1 anonymous message:", msg)
        end,
        ["2"] = function(msg)
            print("2 anonymous message:", msg)
        end,
        ["3"] = function(msg)
            print("3 anonymous message:", msg)
        end
    }
    ---指定函数名称实现，支持多个函数
    local _switch = {
        ["1"] = {
            _print = function(msg)
                print('1 message:', msg)
            end
        },
        ["2"] = {
            _print = function(msg)
                print('2 message:', msg)
            end
        },
        ["3"] = {
            _print = function(msg)
                print('3 message:', msg)
            end
        }
    }

    print('_switch_anonymous type is : ', type(_switch_anonymous))
    for i, v in pairs(_switch_anonymous) do
        print(i, '=', v)
    end

    for i, v in pairs(_switch) do
        print(i, '=', v)
    end

    local flg = "2";
    local _f_anon = _switch_anonymous[flg]
    if _f_anon then
        _f_anon("switch anonymous function case test")
    else
        print("anonymous default case")
    end
    local _f = _switch[flg];
    if _f then
        _f._print("switch case test")
    else
        print("default case")
    end
end


--传入 number
do
    local function test_switch(index)
        local switch = {
            [1] = "a",
            [2] = "b",
            [3] = "c"
        }
        if index <= 0 or index > #switch then
            return "error"
        end
        return switch[index]
    end

    print(test_switch(0))
    print(test_switch(4))
    print(test_switch(2))
end