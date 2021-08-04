#include "chapter_1.h"

void Chapter_1::test_cout_cin()
{
	//链式
	std::cout << "hello" << "world" << std::endl;  //推荐加 endl
	//endl : 结束当前行，并将与设备关联的缓冲区(buffer)中的内容刷到设备中，保证所有输出真正写入输出流，而不是停留在内存中等待写入流


	int v1 = 0, v2 = 0;
	std::cin >> v1 >> v2;//接受一个istream作为左侧运算对象，接受一个对象作为其右侧运算对象，从给定的istream中读入数据，并存入给定对象中
						 //返回其左侧对象作为其计算结果
	std::cout << v1 << " " << v2 << " " << v1 + v2 << std::endl;
}

void Chapter_1::test_for_while()
{
	int sum = 0, value = 0;
	//std::cin 即istream 对象作为循环判断条件  其实是检测流的状态  如果流是有效的，即流未遇到错误，则条件为true
	//当遇到文件结束符(end-of-file： windows Ctrl+Z, Enter   //   UNIX Ctrl+D ) 或者遇到一个无效输入时（如读入的值非整数），此时处于无效状态，条件为false
	
	std::cout << "请输入一些数，按Ctrl+Z表示结束" << std::endl;

	//while实现
	while (std::cin >> value)
		sum += value;
	std::cout << "Sum is" << sum << std::endl;

	//for实现
	for (; std::cin >> value;)
		sum += value;
	std::cout << "Sum is " << sum << std::endl;
}

void Chapter_1::test_if_block()
{
	int currVal = 0, val = 0;
	if (std::cin >> currVal){
		int cnt = 1;
		while (std::cin >> val){
			if (val == currVal)
				++cnt;
			else {
				std::cout << currVal << " occurs "
					<< cnt << " times" << std::endl;
				currVal = val;
				cnt = 1;
			}
		}
		std::cout << currVal << " occurs "
			<< cnt << " times" << std::endl;
	}
}
