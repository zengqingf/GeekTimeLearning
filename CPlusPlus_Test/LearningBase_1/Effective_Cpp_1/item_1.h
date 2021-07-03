#pragma once

#ifndef _ITEM_1_H_
#define _ITEM_1_H_

#include <iostream>

#define NUM_TURNS 5

int func(int max) { return max; }
#define CALL_WITH_MAX(a, b) func((a) > (b) ? (a) : (b))

class Item_1
{
private:
	static const int NUM_TURNS_CONST = 5;
	int scores[NUM_TURNS_CONST];

	enum { NUM_TURNS_ENUM = 5 };
	int scores2[NUM_TURNS_ENUM];

	int a = 5;
	int b = 0;
	int max = a;
public:
	void Test_1()
	{
		max = CALL_WITH_MAX(++a, b);
		std::cout << a << " , " << b << " , max: "<< max << std::endl;
		//output: 7 , 0 , max: 7    ---> a和b比较 先执行(a)即++a（a==6） 因a>b则再执行 (a)即++a （a==7）
		max = CALL_WITH_MAX(++a, b + 10);
		std::cout << a << " , " << b << " , max: " << max << std::endl;
		//output: 8 , 0 , max: 10   ---> a和b比较 先执行(a)即++a（a==6） 因a<b则再执行 (b)即 b+10（a==8）
	}
};

#endif //_ITEM_1_H_
