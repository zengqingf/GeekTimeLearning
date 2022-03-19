    --这里如果不用local function 或者 全局function 
    local tableFunc = {}

    local function func2(a, b, c)
        return a, b, c
    end

    local tb = {myfunc = tableFunc.func2}     
    local tb2 = {myfunc = func2}   
    
    function tableFunc:func2(a, b, c)
        return a, b, c
    end

    function tableFunc:func3(a, b, c)
        return tb.myfunc(a, b, c)               --这里a的myfunc为空 ！！！
    end

    function tableFunc:func4(a, b, c)
        return tb2.myfunc(a, b, c)              --正确
    end
    return tableFunc