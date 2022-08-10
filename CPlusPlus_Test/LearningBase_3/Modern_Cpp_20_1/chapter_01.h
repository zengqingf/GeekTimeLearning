#pragma once

/*
统一初始化
*/

namespace Initializer_List_Test
{
	class MyClass
	{
	private:
		int m_Member;

	public:
		MyClass() = default;
		MyClass(const MyClass& rhs) = default;
	};

	void Test1()
	{
		MyClass objA;
		MyClass objB(MyClass());
	}
}