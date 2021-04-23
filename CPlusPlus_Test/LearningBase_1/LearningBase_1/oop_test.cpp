
#include "oop_test.h"
#include <iostream>
using namespace std;


OOP_Base_1::OOP_Base_1()
{
	cout << "OOP Base 1 ctor" << endl;
}

OOP_Base_1::~OOP_Base_1()
{
	cout << "OOP Base 1 dtor" << endl;
}

OOP_Derived_1::OOP_Derived_1()
{
	cout << "OOP Derived 1 ctor" << endl;
}

OOP_Derived_1::~OOP_Derived_1()
{
	cout << "OOP Derived 1 dtor" << endl;
}

OOP_DerivedPart_1::OOP_DerivedPart_1()
{
	cout << "OOP Derived 1 Part ctor" << endl;
}

OOP_DerivedPart_1::~OOP_DerivedPart_1()
{
	cout << "OOP Derived 1 Part dtor" << endl;
}


/*----------------------------------------------------------------------*/


OOP_Base_2::OOP_Base_2()
{
	cout << "OOP Base 2 ctor" << endl;
}

OOP_Base_2::~OOP_Base_2()
{
	cout << "OOP Base 2 dtor" << endl;
}

OOP_BasePart_2::OOP_BasePart_2()
{
	cout << "OOP Base 2 Part ctor" << endl;
}

OOP_BasePart_2::~OOP_BasePart_2()
{
	cout << "OOP Base 2 Part dtor" << endl;
}

OOP_Derived_2::OOP_Derived_2()
{
	cout << "OOP Derived 2 ctor" << endl;
}

OOP_Derived_2::~OOP_Derived_2()
{
	cout << "OOP Derived 2 dtor" << endl;
}


/******************************************************/

//静态对象需要在class外部定义   clang/gcc同时需要在cpp中定义，而非hpp中
Image* Image::_prototypes[];
int Image::_nextSlot;
Image* Image::findAndClone(imageType type)
{
	for (int i = 0; i < _nextSlot; i++)
	{
		if (_prototypes[i]->returnType() == type)
			return _prototypes[i]->clone();
	}
}


LandSatImage LandSatImage::_landSatImage;
int LandSatImage::_count = 1;