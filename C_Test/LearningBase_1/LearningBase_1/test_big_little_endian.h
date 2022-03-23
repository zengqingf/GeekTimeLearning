#include <stdio.h>

#ifndef TEST_BIG_LITTLE_ENDIAN
#define TEST_BIG_LITTLE_ENDIAN

typedef unsigned char *byte_pointer;
void show_bytes(byte_pointer start, int len) {
	int i;
	for (i = 0; i < len; ++i) {
		printf("%p %.2x\n", &start[i], start[i]);
	}
	printf("\n");
}

void show_int(int x) {
	show_bytes((byte_pointer)&x, sizeof(int));
}

void show_float(float x) {
	show_bytes((byte_pointer)&x, sizeof(float));
}


void show_pointer(void *x) {
	show_bytes((byte_pointer)&x, sizeof(void *));
}


/*
ref:https://zhuanlan.zhihu.com/p/144718837
 借助联合体union的特性实现(联合体类型数据所占的内存空间等于其最大的成员所占的空间，
 对联合体内部所有成员的存取都是相对于该联合体基地址的偏移量为 0 处开始，
 也就都是从该联合体所占内存的首地址位置开始。
*/
void test_big_little_endian_1() {
	union {
		int a;  //4 bytes						可以换成 short a; a=0x1234; 0x12 == num.b
		char b; //1 byte
	}data;
	data.a = 1;  //占4 bytes  0x 00 00 00 01						
	/*
		0x 01				---> 内存低位   由于union中b占用a中的低位地址，b指向这里时，为小端，b==1
		0x 00
		0x 00
		0x 00				---> 内存高位   由于union中b占用a中的低位地址，b指向这里时，为小端, b==0
	*/

	/*
	 b因为是char型只占1Byte，a因为是int型占4Byte
    所以，在联合体data所占内存中，b所占内存等于a所占内存的低地址部分 
	*/
	if (1 == data.b) {			//表明a的低字节取给b用了，a的低字节存在了联合体所占内存的（起始）低地址，符合小端模式; 1 == 0x01
		printf("### Litte Endian ###\n");
	}
	else {
		printf("### Big Endian ###\n");
	}
}


void test_big_little_endian_2() {
	int a = 1;					//占4 bytes，十六进制可表示为 0x 00 00 00 01

	// //b相当于取了a的低地址部分 
	char*b = (char *)&a;		//占1 byte
	if (1 == *b) {				//走该case说明a的低字节，被取给到了b，即a的低字节对应a所占内存的低地址，符合小端模式特征
		printf("### Litte Endian ###\n");
	}
	else {
		printf("### Big Endian ###\n");
	}
}

/*
用MSB和LSB讲大端和小端的描述，需要注意，大端和小端描述的是字节之间的关系，
而MSB、LSB描述的是Bit位之间的关系。字节是存储空间的基本计量单位，
所以通过高位字节和低位字节来理解大小端存储是最为直接的。
MSB: Most Significant Bit ------- 最高有效位(指二进制中最高值的比特)
LSB: Least Significant Bit ------- 最低有效位(指二进制中最高值的比特)
*/
void test_big_little_endian_3() {
	union {
		struct {
			char a : 1;//定义位域为 1 bit
		}s;
		char b;
	}data;

	data.b = 8;//8(Decimal) == 1000(Binary)，MSB is 1，LSB is 0

	//在联合体data所占内存中，data.s.a所占内存bit等于data.b所占内存低地址部分的bit
	if (1 == data.s.a) {//走该case说明data.b的MSB是被存储在union所占内存的低地址中，符合大端序的特征
		printf("Big_Endian\n");
	}
	else {
		printf("Little_Endian\n");
	}
}


void TestBigLittleEndian()
{
	//八进制和十六进制表示（打印）
	int x = 100;
	printf("dec = %d, octal = %o, hex = %x\n", x, x, x);    //八进制 %o   十六进制 %x
	printf("dec = %d, octal = %#o, hex = %#x\n", x, x, x);  //会输出 0..   0x..

	// unsigned int （无符号整型） 能表示 比 int 更大的数

	// signed 关键字 强调使用有符号数的意图  
	// short == short int == signed short = signed short int

	//整数溢出
	//常见例子：在超过最大值时  unsigned int 从 0 开始    int从 -2147483648 开始
	int ii = 2147483647;
	unsigned int jj = 4294967295;
	printf("%d, %d, %d\n", ii, ii + 1, ii + 2);
	printf("%u, %u, %u\n", jj, jj + 1, jj + 2);

	//getchar();

	/*测试大端小端*/
	int ival = 0x12345678;
	float fval = (float)ival;
	int *pval = &ival;
	show_int(ival);
	show_float(fval);
	show_pointer(pval);
	/*
	output:
	006FF80C 78			小端
	006FF80D 56
	006FF80E 34
	006FF80F 12

	006FF80C b4			小端
	006FF80D a2
	006FF80E 91
	006FF80F 4d

	006FF80C f8			小端
	006FF80D f8
	006FF80E 6f
	006FF80F 00
	*/
}

#endif //TEST_BIG_LITTLE_ENDIAN