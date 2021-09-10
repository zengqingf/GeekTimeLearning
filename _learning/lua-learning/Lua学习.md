# Lua学习

### lua基础

[Lua 5.1 参考手册](https://www.codingnow.com/2000/download/lua_manual.html)

[Lua 5.3 参考手册](https://cloudwu.github.io/lua53doc/manual.html)

[Lua图解-lua堆栈](./Lua图解)

[Lua和C++交互详细总结](https://www.cnblogs.com/sevenyuan/p/4511808.html)

* lua 堆栈

  在Lua中，Lua堆栈就是一个struct，堆栈索引的方式可是是正数也可以是负数，区别是：正数索引1永远表示栈底，负数索引-1永远表示栈顶。



---



### lua with c++

* lua 和 c++通信

  ``` tex
  lua c++ 通信约定：
  所有的lua中的值由lua来管理, c++中产生的值lua不知道
  如果你(c/c++)想要什么, 你告诉我(lua), 我来产生, 然后放到栈上, 你只能通过api来操作这个值, 我只管我的世界
  
  "如果你想要什么, 你告诉我, 我来产生"就可以保证, 凡是lua中的变量, lua要负责这些变量的生命周期和垃圾回收, 
  	所以, 必须由lua来创建这些值(在创建时就加入了生命周期管理要用到的簿记信息)
  
  "然后放到栈上, 你只能通过api来操作这个值", lua api给c提供了一套完备的操作界面, 这个就相当于约定的通信协议, 
  	如果lua客户使用这个操作界面, 那么lua本身不会出现任何"意料之外"的错误.
  
  "我只管我的世界"这句话体现了lua和c/c++作为两个不同系统的分界, c/c++中的值, lua是不知道的, lua只负责它的世界
  
  
  Lua和C++是通过一个虚拟栈来交互的。
  C++调用Lua实际上是：由C++先把数据放入栈中，由Lua去栈中取数据，然后返回数据对应的值到栈顶，再由栈顶返回C++。
  Lua调C++也一样：先编写自己的C模块，然后注册函数到Lua解释器中，然后由Lua去调用这个模块的函数。
  ```

  

* lua value 和 c value的对应关系

  |               | c                        | lua                                          |
  | ------------- | ------------------------ | -------------------------------------------- |
  | nil           | 无                       | {value=0, tt = t_nil}                        |
  | boolean       | int 非0, 0               | {value=非0/0， tt = t_boolean}               |
  | number        | int/float等  1.5         | {value=1.5, tt = t_number}                   |
  | lightuserdata | void*, int*, 各种* point | {value=point, tt = t_lightuserdata}          |
  | string        | char str[]               | {value=gco, tt = t_string}  gco=TString obj  |
  | table         | 无                       | {value=gco, tt = t_table} gco=Table obj      |
  | userdata      | 无                       | {value=gco, tt = t_udata} gco=Udata obj      |
  | closure       | 无                       | {value=gco, tt = t_function} gco=Closure obj |

  ``` tex
  nil值, c中没有对应, 但是可以通过lua_pushnil向lua中压入一个nil值
  
  
  c value –> lua value的流向, 不管是想把一个简单的5放入lua的世界, 还是创建一个table, 都会导致
  1. 栈顶新分配元素    2. 绑定或赋值
  一个c value入栈就是进入了lua的世界, lua会生成一个对应的结构并管理起来, 从此就不再依赖这个c value
  
  lua value –> c value时, 是通过 lua_to* 族api实现
  取出对应的c中的域的值就行了, 只能转化那些c中有对应值的lua value, 
  比如table就不能to c value, 所以api中夜没有提供 lua_totable这样的接口.
  ```

  

* lua to cpp

  * 方法一：直接将cpp模块写到lua源码中

    ``` tex
    可以在lua.c中加入代码，函数规范：
    typedef int (*lua_CFunction) (lua_State *L);
    所有的函数必须接收一个lua_State作为参数，同时返回一个整数值。
    （因为这个函数使用Lua栈作为参数，所以它可以从栈里面读取任意数量和任意类型的参数）
    这个函数的返回值则表示函数返回时有多少返回值被压入Lua栈
    （因为Lua的函数是可以返回多个值的）
    
    一般不建议修改lua源码，最好自己编写独立的c/c++模块，供lua调用
    ```

    ```  c++
    //lua.c
    
    // This is my function  
    static int getTwoVar(lua_State *L)  
    {  
        // 向函数栈中压入2个值  
        lua_pushnumber(L, 10);  
        lua_pushstring(L,"hello");  
       
        return 2;  
    }  
    
    //在pmain函数中，luaL_openlibs函数后加入以下代码：
    //注册函数  
    lua_pushcfunction(L, getTwoVar); //将函数放入栈中  
    lua_setglobal(L, "getTwoVar");   //设置lua全局变量getTwoVar
    
    //整合注册函数方法
    // #define lua_register(L,n,f) (lua_pushcfunction(L, (f)), lua_setglobal(L, (n)))
    lua_register(L,"getTwoVar",getTwoVar);
    ```

  * 方法二：使用静态依赖

    代码见示例工程 CPlusPlus_Test/Cpp_Lua_1/Cpp_Lua_1/TestLua_2.h / .cpp

    ``` tex
    在C++中写一个模块函数，将函数注册到Lua解释器中，然后由C++去执行我们的Lua文件，然后在Lua中调用刚刚注册的函数。
    ```

  * 方法三：使用dll动态链接

    代码见示例工程 CPlusPlus_Test/Cpp_Lua_1/LuaLib_2/LuaLib_2.h / .cpp

    ​							CPlusPlus_Test/Cpp_Lua_1/Cpp_Lua_1/LuaDemo/test_lualib_2.lua

    ``` tex
    require "mLualib"的内部实现如下
    	1. local path = "mLualib.dll"    
    	2. local f = package.loadlib(path,"luaopen_mLualib")   -- 返回luaopen_mLualib函数  
    	3. f()   
    
    函数参数里的lua_State是私有的，每一个函数都有自己的栈。当一个C/C++函数把返回值压入Lua栈以后，该栈会自动被清空
    ```

    

  

* cpp to lua

  ``` tex
  堆栈操作是基于栈顶的，就是说它只会去操作栈顶的值
  
  lua函数调用流程是先将函数入栈，参数入栈，然后用lua_pcall调用函数，此时栈顶为参数，栈底为函数，
  所以栈过程大致会是：参数出栈->保存参数->参数出栈->保存参数->函数出栈->调用函数->返回结果入栈。
  ```

  

