#include "TestLua_1.h"

void TestLua_1::Test1()
{
	//1.创建一个state
	lua_State *L = luaL_newstate();
	if (L == nullptr)
	{
		return;
	}

	//2.入栈
	lua_pushstring(L, "hello lua");
	lua_pushnumber(L, 1993);

	//3.取值
	if (lua_isstring(L, 1)) {
		std::cout << lua_tostring(L, 1) << std::endl;
	}
	if (lua_isstring(L, 2)) {
		std::cout << lua_tonumber(L, 2) << std::endl;
	}

	//4.关闭state
	lua_close(L);


	/*
	
int   lua_gettop (lua_State *L);            //返回栈顶索引（即栈长度）  
void  lua_settop (lua_State *L, int idx);   //ua_settop将栈顶设置为一个指定的位置，即修改栈中元素的数量。
											如果值比原栈顶高，则高的部分nil补足，如果值比原栈低，则原栈高出的部分舍弃。
											所以可以用lua_settop(0)来清空栈。                
void  lua_pushvalue (lua_State *L, int idx);//将idx索引上的值的副本压入栈顶  
void  lua_remove (lua_State *L, int idx);   //移除idx索引上的值  
void  lua_insert (lua_State *L, int idx);   //弹出栈顶元素，并插入索引idx位置  
void  lua_replace (lua_State *L, int idx);  //弹出栈顶元素，并替换索引idx位置的值

lua_getglobal(L,"var")会执行两步操作：1.将var放入栈中，2.由Lua去寻找变量var的值，并将变量var的值返回栈顶（替换var）。
lua_getfield(L,-1,"name")的作用等价于 lua_pushstring(L,"name") + lua_gettable(L,-2)
lua_setfield(L, 2, "name")，设置一个表的值，肯定要先将值出栈，保存，再去找表的位置。



（lua 和 c value的对应关系见对应文档 - Lua学习）
lua_push*族函数都有"创建一个类型的值并压入"的语义, 因为lua中所有的变量都是lua中创建并保存的
( 对于那些和c中有对应关系的lua类型, lua会通过api传来的附加参数, 创建出对应类型的lua变量放在栈顶, 对于c中没有对应类型的lua类型, lua直接创建出对应变量放在栈顶)

lua_pushstring(L, “string”) lua根据"string"创建一个 TString obj, 绑定到新分配的栈顶元素上
lua_pushcclosure(L,func, 0) lua根据func创建一个 Closure obj, 绑定到新分配的栈顶元素上
lua_pushnumber(L,5) lua直接修改新分配的栈顶元素, 将5赋值到对应的域
lua_createtable(L,0, 0)lua创建一个Tabke obj, 绑定到新分配的栈顶元素上
	*/
}

void TestLua_1::Test2()
{
	//1.创建一个state
	lua_State *L = luaL_newstate();
	if (L == nullptr)
	{
		return;
	}

	//2.加载lua文件
	int bRet = luaL_loadfile(L, "LuaDemo/test_1.lua");
	if (bRet)								//注意：true表示出错！
	{
		std::cout << "load lua file failed: " << bRet << std::endl;
		return;
	}

	//3.运行lua文件
	bRet = lua_pcall(L, 0, 0, 0);
	if (bRet)								//注意：true表示出错！
	{
		std::cout << "call lua file failed" << std::endl;
		return;
	}

	//读取变量
	lua_getglobal(L, "str");
	std::string str = lua_tostring(L, -1);
	std::cout << "str = " << str.c_str() << std::endl;

	//读取table
	lua_getglobal(L, "tb1");
	lua_getfield(L, -1, "name");
	str = lua_tostring(L, -1);
	std::cout << "tb1:name = " << str.c_str() << std::endl;

	//读取函数
	lua_getglobal(L, "add");		// 获取函数，压入栈中  
	lua_pushnumber(L, 10);			// 压入第一个参数
	lua_pushnumber(L, 20);			// 压入第二个参数 
	int iRet = lua_pcall(L, 2, 1, 0);// 调用函数，调用完成以后，会将返回值压入栈中，2表示参数个数，1表示返回结果个数。
									 // 调用完成后， 将2个参数出栈，函数出栈，压入函数返回结果  
	if (iRet)
	{
		const char* pErrorMsg = lua_tostring(L, -1);
		std::cout << pErrorMsg << std::endl;
		lua_close(L);
		return;
	}
	if (lua_isnumber(L, -1))
	{
		double fValue = lua_tonumber(L, -1);
		std::cout << "add() return is " << fValue << std::endl;
	}

	//至此，栈中的情况是：  
	//=================== 栈顶 ===================   
	//  索引  类型      值  
	//   4   int：      30   
	//   3   string：   lua   
	//   2   table:     tbl  
	//   1   string:    hello lua  
	//=================== 栈底 ===================   


	//压栈，栈顶元素改变
	lua_pushstring(L, "hello world");

	//=================== 栈顶 ===================   
	//  索引  类型      值  
	//   5   string：   hello world
	//   4   int：      30   
	//   3   string：   lua   
	//   2   table:     tbl  
	//   1   string:    hello lua  
	//=================== 栈底 ===================   

	//将栈顶元素设置到table（table所在栈的位置为2）,并将"hello world"出栈
	//lua_setfield(L, 2, "name");
	//或者这么写
	lua_setfield(L, -4, "name");

	//读取table，压入栈中
	lua_getglobal(L, "tb1");
	lua_getfield(L, -1, "name");
	str = lua_tostring(L, -1);
	std::cout << "tb1:name = " << str.c_str() << std::endl;


	//新建table，并压人栈
	lua_newtable(L);
	//在table中设置值
	lua_pushstring(L, "not support chinese");			//将值压入栈 
	lua_setfield(L, -2, "str");							//将值设置到table中，并将"not support chinese" 出栈

	//读取table
	lua_getfield(L, -1, "str");
	str = lua_tostring(L, -1);
	std::cout << "new table:str = " << str.c_str() << std::endl;

	//关闭state
	lua_close(L);
}
