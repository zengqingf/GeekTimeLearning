--确定Lua编译器是运行32位还是64位

--func 1
local arch
if (os.getenv"os" or ""):match"^Windows" then
   print"Your system is Windows"
   arch = os.getenv"PROCESSOR_ARCHITECTURE"
else
   print"Your system is Linux"
   arch = io.popen"uname -m":read"*a"
end
if (arch or ""):match"64" then
   print"Your system is 64-bit"
else
   print"Your system is 32-bit"
end



--func 2
--在32位Lua中，最大整数数为0xffffffff（8'f's），而0xfffffffff（9'f's）将溢出 尝试流代码
function _86or64()
    if(0xfffffffff==0xffffffff) then return 32 else return 64 end
end

print(_86or64());