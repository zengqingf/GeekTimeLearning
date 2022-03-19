#pragma once

/*
1. 转换函数
格式：
operator 转换对象() const {				//转换对象名 作为 函数名
	return (转换对象)(待转换对象)
}
@注意： 没有returnType 直接返回函数名（转换对象）
		转换函数不能改变所在class中的变量，所以用const

*/


//分数
class Fraction_1
{
public:
	Fraction_1(int num, int den = 1)
		: m_numerator(num), m_denominator(den) {}

	//转换函数
	operator double() const {
		return (double)(m_numerator / m_denominator);
	}
private:
	int m_numerator;
	int m_denominator;
};