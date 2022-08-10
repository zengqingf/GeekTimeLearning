--[[
元表
可以修改一个值在面对一个未知操作时的行为。例如，假设a和b都是表，那么可以通过元表定义Lua语言如何计算表达式a+b。
当Lua语言试图将两个表相加时，它会先检查两者之一是否有元表（metatable）且该元表中是否有__add字段。
如果Lua语言找到了该字段，就调用该字段对应的值，即所谓的元方法（metamethod）（是一个函数），在本例中就是用于计算表的和的函数。

可以认为，元表是面向对象领域中的受限制类。
像类一样，元表定义的是实例的行为。
不过，由于元表只能给出预先定义的操作集合的行为，所以元表比类更受限；同时，元表也不支持继承。
]]

do
    local t = {}
    print("metatable is ", getmetatable(t))

    local t1 = {}
    setmetatable(t, t1)
    print("metatable is ", getmetatable(t) == t1)

    --[[
        lua中 只能为 table设置 元表
        对其他类型的值设置元表，需要通过C代码或调试库完成（防止过度使用对某种类型的所有值生效的元表，旧版本lua表明，这样全局设置经常导致不可宠用的代码）

        字符串标准库默认设置了一个元表，其他类型默认没有元表

        一个表可以成为任意值的元表

        一组相关的表也可以共享一个描述了它们共同行为的通用元表

        一个表可以成为自己的元表，用于描述自身特有的行为
    ]]
    print("metatable is ", getmetatable("hi"))
    print("metatable is ", getmetatable("jz"))
    print("metatable is ", getmetatable(10))
    print("metatable is ", getmetatable(print))
end


do
    require("Metatable_Set")     --require加载模块，模块文件名和模块内的table名不需要一致
    print(Set.TestCount)
    local s1 = Set.new{10, 20, 30, 50}
    local s2 = Set.new({30, 1})
    print(s1)
    print(s2)

    --元表相同
    print(getmetatable(s1))     --table: 0000000000a9a5d0
    print(getmetatable(s2))     --table: 0000000000a9a5d0

    --把两个集合相加时，使用哪个元表是确定的
    local s3 = s1 + s2
    print(Set.tostring(s3))         --{1, 20, 10, 30, 50}
    print(Set.tostring(s3 * s1))    --{20, 50, 10, 30}
    print(Set.tostring(s3 * s2))    --{1, 30}


    --[[
        Lua语言会按照如下步骤来查找元方法：
        如果第一个值有元表且元表中存在所需的元方法，那么Lua语言就使用这个元方法，与第二个值无关；
        如果第二个值有元表且元表中存在所需的元方法，Lua语言就使用这个元方法；
        否则，Lua语言就抛出异常。因此，上例会调用Set.union，
        而表达式10+s和"hello"+s同理（由于数值和字符串都没有元方法__add）
    ]]
    local s4 = Set.new{1, 2, 3}
    --s4 = s4 + 8                 --.\chapter_20.lua:65: attempt to 'add' a set with a non-set value
    print(Set.tostring(s4))
end

do
    --[[
        元表：指定关系运算符  __eq ==   __lt <  __le <=
             其他关系运算符没有单独元方法，需要通过上述运算符转换
             a ~= b  ==>  not(a == b)
             a > b ==>  b < a
             a >= b ==>  b <= a
    ]]

    --[[
        注意：部分有序（partial order）的情况下，a<=b转换为not (b<a) 可能不成立，
        部分有序：并非所有类型的元素都能够被正确排序，如，遇到Not a Number(NaN)时，大多数机器的浮点数并不是完全可以排序的，
        根据IEEE 754标准， NaN代表未定义的值（如0 / 0 结果为未定义），即涉及NaN的比较，返回都是false，NaN <= x ==> false，x < NaN ==> false
        即 a<=b转换为not (b<a) 不成立
    ]]
    --[[
        集合中， <= 表示集合包含， a <= b 表示 a是b的一个子集，
        需要自定义实现 __le（子集关系） 和  __lt （真子集关系）
    ]]

    local s1 = Set.new{2, 4}
    local s2 = Set.new{4, 10, 2}
    print(s1 <= s2)
    print(s1 < s2)
    print(s1 >= s1)
    print(s1 > s1)
    print(s1 == s2 * s1)
end