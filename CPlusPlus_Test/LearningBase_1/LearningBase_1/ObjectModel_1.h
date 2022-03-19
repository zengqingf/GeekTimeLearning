#pragma once

class A {
public:
	virtual void vfun1();
	virtual void vfun2();
	void func1();
	void func2();

private:
	int m_data1, m_data2;
};

class B : public A {
public:
	virtual void vfun1();
	void func2();

private:
	int m_data3;
};

class C: public B {
public:
	virtual void vfun1();
	void func2();

private:
	int m_data1, m_data4;
};

class TestObjectModel
{
	C* c1 = new C();
};