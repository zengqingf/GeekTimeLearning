#include <stdio.h>

#ifndef TEST_SIZEOF_VARIABLE
#define TEST_SIZEOF_VARIABLE

void PrintSizeofVariable()
{
	printf("bool .. %d \n", (int)sizeof(bool)); //C99以后支持用_Bool		//1
	printf("char ..%d \n", (int)sizeof(char));								//1
	printf("unsigned char ..%d \n", (int)sizeof(unsigned char));			//1
	printf("short ..%d \n", (int)sizeof(short));							//2
	printf("int ..%d \n", (int)sizeof(int));								//4
	printf("long ..%d \n", (int)sizeof(long));								//4
	printf("long long ..%d \n", (int)sizeof(long long));					//8
	printf("float ..%d \n", (int)sizeof(float));							//4
	printf("double ..%d \n", (int)sizeof(double));							//8

	printf("bool .. %zd \n", sizeof(bool)); //C99以后支持用_Bool		//1			//C99 支持 %zd 打印 size_t
	printf("char ..%zd \n", sizeof(char));								//1
	printf("unsigned char ..%zd \n", sizeof(unsigned char));			//1
	printf("short ..%zd \n", sizeof(short));							//2
	printf("int ..%zd \n", sizeof(int));								//4
	printf("long ..%zd \n", sizeof(long));								//4
	printf("long long ..%zd \n", sizeof(long long));					//8
	printf("float ..%zd \n", sizeof(float));							//4
	printf("double ..%zd \n", sizeof(double));							//8
}

#endif //TEST_SIZEOF_VARIABLE