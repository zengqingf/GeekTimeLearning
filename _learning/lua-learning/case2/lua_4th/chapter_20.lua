
do
    local Set = {}
    local mt = {}

    function Set.new(l)
        local set = {}
        setmetatable(set, mt)
        for _, v in ipairs(l) do
            set[v] = true
        end
    end

    return Set
end