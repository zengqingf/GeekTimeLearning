#include "base_4.h"

void TypeCast_Test_1::Test1()
{
	Base_A *a1 = new Derived_B();
	Derived_B *b1 = dynamic_cast<Derived_B *>(a1);  //error: a必须包含多态类型，必须含有虚函数（dynamic_cast必须有虚函数）
												   //把指向子类对象的父类指针强转成子类指针，用dynamic_cast正确但不推荐

	Derived_B *b2 = static_cast<Derived_B *>(a1);   //把指向子类对象的父类指针强转成子类指针，用static_cast正确但不推荐

	Base_A *a2 = new Base_A();
	Derived_B *b3 = static_cast<Derived_B *>(a2);  //把指向父类对象的父类指针强转成子类指针，用static_cast : 危险！访问子类m_szName成员越界
	Derived_B *b4 = dynamic_cast<Derived_B *>(a2); //把指向父类对象的父类指针强转成子类指针,用dynamic_cast，安全的。结果是NULL


	Base_A *ba_1 = new Derived_D();
	//Base_AA *baa_1 = static_cast<Base_AA *>(ba_1); //编译错误
	Base_AA *baa_2 = dynamic_cast<Base_AA *>(ba_1);

	Derived_B *db_1 = new Derived_B();
	//Derived_C *dc_1 = static_cast<Derived_C *>(db_1); //编译错误
	Derived_C *dc_2 = dynamic_cast<Derived_C *>(db_1);  //dc_2 == nullptr
}
