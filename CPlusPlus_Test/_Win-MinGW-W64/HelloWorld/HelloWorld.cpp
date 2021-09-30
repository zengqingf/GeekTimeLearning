#include <iostream>
#include <vector>
#include <string>

using namespace std;

/*
win:无法定位程序输入点__gxx_personality_v0的一个解决方法
ref: https://blog.csdn.net/zyli14/article/details/51302153
windows系统，使用mingw32-g++编译一个简单的工程，编译链接过程都没有错误提示，但是运行的时候会弹出提示框提示“无法定位程序输入点__gxx_personality_v0”
在我这里是因为系统的环境变量的目录中有几个版本不同的libstdc++-6.dll。
一个解决方案是使用 -static 选项编译工程。
另一个解决方案是 删除掉其他含有libstdc++-6.dll 的PATH环境变量，只留下mingw的。
*/

/*
解决Win10用户VS Code的C/C++更新到1.6.0后无法调试的问题
CppDbg Debugger broken ( Version 1.6.0-insiders: August 12, 2021 ) · Issue #7971 · microsoft/vscode-cpptools · GitHub
https://github.com/microsoft/vscode-cpptools/issues/7971


删除以下下文件（夹）
用户文件夹\.vscode\extensions\ms-vscode.cpptools-1.6.0-insiders\install.lock
用户文件夹\.vscode\extensions\ms-vscode.cpptools-1.6.0-insiders\debugAdapters
重新安装 v1.6.0-insiders2 or up
*/

int main() //省略int argc, char const* argv[]
{
    vector<string> msg{"Hello", "C++", "World", "from", "VS Code"};
    for (const string &word : msg)
    {
        cout << word << " ";
    }
    cout << endl;

    //system("pause");
    //cin.get();
    //getchar();
    return 0;
}