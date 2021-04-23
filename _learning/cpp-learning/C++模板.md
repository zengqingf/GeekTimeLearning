# C++模板





---



* 实例1

  特化模板类时继承（Inheritance when specializing a template class）

  ``` c++
  template<typename T>
  class myClass{
      public:
          double Foo(double x){return x;}
      	string GetStr() const {return str;}					//注意  不能使用virtual !!!  即使在特化的类中 override 不会执行到
      private:													//str在特化类中的改动会设置到基类中
      	string str;
  }
  template<>
  class myClass<SpecialType>{
      public:
          double Bar(double x){return x+1.0;}
  }
  //期望
  /*
  myClass<SpecialType> A;
  double y = A.Foo(1.0);
  */
  
  //e.g. 1
  struct myClassBase
  {
      virtual double Foo(double x) const {return x+1.0;}
  };
  
  template<typename T> struct myClass : public myClassBase
  {
       //...
  };
  
  template<>
  struct myClass<SpecialType> : public myClassBase
  {
      double Bar(double x){return x+1.0;}
  }
  
  
  //e.g. 2
  template<typename Derived>
  struct myClassBase
  {
      double Foo(double x) const
      {
          return static_cast<Derived const&>(*this).specialMember(x);
      }
      //all other stuff independent of the derived class specialization
  
      //possibly define specialMember once:
      virtual double specialMember(double x) const { return x; }
  }
  
  template<typename T> struct myClass : public myClassBase<myClass<T> >
  {
      //... special member of Base class is sufficient
  };
  
  template<> struct myClass<SpecialType> : public myClassBase<myClass<SpecialType> >
  {
      virtual double specialMember(double x) const { return x+1.0; }
  };
  ```

* 实例2

  模板判断是否是类类型

  ``` c++
  template<typename T>
  class IsClassT {
  private:
      typedef char One;
      typedef struct { char a[2]; } Two;
      template<typename C> static One test(int C::*);
      template<typename C> static Two test(...);
  public:
      enum { Yes = sizeof(test<T>(0)) == 1 };
      enum { No = !Yes };
  };
  
  // check by passing type as template argument
  template <typename T>
  void check()
  {
      if (IsClassT<T>::Yes) {
          std::cout << " IsClassT " << std::endl;
      }
      else {
          std::cout << " !IsClassT " << std::endl;
      }
  }
  
  template <typename T>
  void checkT(T)
  {
      check<T>();
  }
  
  //check<int>();
  //checkT(myfunc);
  ```

  模板判断类型

  ``` c++
  //判断输入的两个模板类型是否一样
  
  template<typename T1, typename T2>
  struct is__same
  {
  	operator bool()
  	{
  		return false;
  	}
  };
   
  template<typename T1>
  struct is__same<T1, T1>
  {
  	operator bool()
  	{
  		return true;
  	}
  };
  //std::cout << is__same<int, char>() << endl;
  //std::cout << is__same<short int, short int>() << endl;
  
  
  
  //判断输入的模板类型是否为指定的类型
  
  template <typename T>
  struct is_double
  {
  	operator bool()
  	{
  		return false;
  	}
  };
   
  template <>
  struct is_double<double>
  {
  	operator bool()
  	{
  		return true;
  	}
  };
  
  /*
  	if (!is_double<int>())
  		std::cout << "this is not double type" << std::endl;
   
  	if (is_double<double>())
  		std::cout << "this is double type" << std::endl;
  */
  ```

  