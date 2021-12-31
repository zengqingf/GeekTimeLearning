--require "IntelliSense.LuaClass"


function Clone(object)
    local lookup_table = {}
    local function _copy(object)
        if type(object) ~= "table" then
            return object
        elseif lookup_table[object] then
            return lookup_table[object]
        end
        local new_table = {}
        lookup_table[object] = new_table
        for key, value in pairs(object) do
            new_table[_copy(key)] = _copy(value)
        end
        return setmetatable(new_table, getmetatable(object))
    end
    return _copy(object)
end

local function Index(t, k)
    local mt = getmetatable(t)
    local Super = mt
    while Super do
        local v = rawget(Super, k)
        if v~= nil then
            if rawequal(v, NotExist) then
                return nil
            end
            rawset(t, k, v)
            return v
        end
        Super = rawget(Super, "Super")
    end
    local p = mt[k]

    if p ~= nil then
        if type(p) == "userdata" then
            return GetUProperty(t, p)
        elseif type(p) == "function" then
            rawset(t, k, p)
        elseif rawequal(p, NotExist) then
            return nil
        end
    else
        rawset(mt, k, NotExist)
    end
    return p
end

local function NewIndex(t, k, v)
    local mt = getmetatable(t)
    local p = mt[k]
    if type(p) == "userdata" then
        return SetUProperty(t, p, v)
    end
    rawset(t, k, v)
end

---Create an class.
---@diagnostic disable-next-line:lowercase-global
function class(classname, Super)
    local superType = type(Super)
    local cls
    
    if superType ~= "function" and superType ~= "table" then
        superType = nil
        Super = nil
    end

    if superType == "function" or (Super and Super.__ctype == 1) then
        -- inherited from native C++ Object
        cls = {}

        if superType == "table" then
            -- copy fields from Super
            for k,v in pairs(Super) do cls[k] = v end
            cls.__create = Super.__create
            cls.Super    = Super
        else
            cls.__create = Super
        end

        cls.ctor    = function() end
        cls.cname = classname
        cls.__ctype = 1

        function cls.New(...)
            local instance = cls.__create(...)
            -- copy fields from class to native object
            for k,v in pairs(cls) do instance[k] = v end
            instance.class = cls
            instance:Ctor(...)
            return instance
        end

    else
        -- inherited from Lua Object
        if Super then
            cls = Clone(Super)
            cls.Super = Super
        else
            cls = {ctor = function() end}
        end

        cls.cname = classname
        cls.__ctype = 2 -- lua
        cls.__index = Index
        cls.__newindex = NewIndex

        function cls.New(...)
            local instance = setmetatable({}, cls)
            instance.class = cls
            instance:Ctor(...)
            return instance
        end
    end

    return cls
end

---@diagnostic disable-next-line:lowercase-global
function try(block)

    -- get the try function
    local try = block[1]
    assert(try)

    -- get catch and finally functions
    local funcs = block[2]
    if funcs and block[3] then
        funcs.finally = block[3]
    end

    -- try to call it
    local ok, errors = pcall(try)
    if not ok then
        print('error:',errors)
        -- run the catch function
        if funcs and funcs.catch then
            funcs.catch(errors)
        end
    end

    -- run the finally function
    if funcs and funcs.finally then
        funcs.finally(ok, errors)
    end

    -- ok?
    if ok then
        return errors
    end
end


function dump_value_(v)
    if type(v) == "string" then
        v = "\"" .. v .. "\""
    end
    return tostring(v)
end

function split(input, delimiter)
    input = tostring(input)
    delimiter = tostring(delimiter)
    if (delimiter == "") then return false end
    local pos, arr = 0, {}
    for st, sp in function() return string.find(input, delimiter, pos, true) end do
        table.insert(arr, string.sub(input, pos, st - 1))
        pos = sp + 1
    end
    table.insert(arr, string.sub(input, pos))
    return arr
end

function trim(input)
    return (string.gsub(input, "^%s*(.-)%s*$", "%1"))
end
 
-- input:要分割的字符串
-- delimiter:分隔符
function string.split(input, delimiter)
    input = tostring(input)
    delimiter = tostring(delimiter)
    if (delimiter=='') then return false end
    local pos,arr = 0, {}
    -- for each divider found
    for st,sp in function() return string.find(input, delimiter, pos, true) end do
        table.insert(arr, string.sub(input, pos, st - 1))
        pos = sp + 1
    end
    table.insert(arr, string.sub(input, pos))
    return arr
end

-- 删除字符串两端的空白字符
function string.trim(input)
    input = string.gsub(input, "^[ \t\n\r]+", "")
    return string.gsub(input, "[ \t\n\r]+$", "")
end

 
--[[
打印table的工具函数
@params value 需要打印的内容
@params desciption 描述
@params nesting 打印内容的嵌套级数，默认3级
]]
function dump(value, desciption, nesting)
    if type(nesting) ~= "number" then nesting = 3 end
 
    local lookupTable = {}
    local result = {}
 
    local traceback = split(debug.traceback("", 2), "\n")
    -- print("dump from: " .. trim(traceback[3]))
 
    local function dump_(value, desciption, indent, nest, keylen)
        desciption = desciption or "<var>"
        local spc = ""
        if type(keylen) == "number" then
            spc = string.rep(" ", keylen - string.len(dump_value_(desciption)))
        end
        if type(value) ~= "table" then
            result[#result +1 ] = string.format("%s%s%s = %s", indent, dump_value_(desciption), spc, dump_value_(value))
        elseif lookupTable[tostring(value)] then
            result[#result +1 ] = string.format("%s%s%s = *REF*", indent, dump_value_(desciption), spc)
        else
            lookupTable[tostring(value)] = true
            if nest > nesting then
                result[#result +1 ] = string.format("%s%s = *MAX NESTING*", indent, dump_value_(desciption))
            else
                result[#result +1 ] = string.format("%s%s = {", indent, dump_value_(desciption))
                local indent2 = indent.."    "
                local keys = {}
                local keylen = 0
                local values = {}
                for k, v in pairs(value) do
                    keys[#keys + 1] = k
                    local vk = dump_value_(k)
                    local vkl = string.len(vk)
                    if vkl > keylen then keylen = vkl end
                    values[k] = v
                end
                table.sort(keys, function(a, b)
                    if type(a) == "number" and type(b) == "number" then
                        return a < b
                    else
                        return tostring(a) < tostring(b)
                    end
                end)
                for i, k in ipairs(keys) do
                    dump_(values[k], k, indent2, nest + 1, keylen)
                end
                result[#result +1] = string.format("%s}", indent)
            end
        end
    end
    dump_(value, desciption, "- ", 1)
 
    for i, line in ipairs(result) do
        print(line)
    end
end