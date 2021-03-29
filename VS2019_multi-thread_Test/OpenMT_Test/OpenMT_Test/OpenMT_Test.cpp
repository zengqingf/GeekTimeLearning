// OpenMT_Test.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>

#include <omp.h>
#include <stdio.h>
#include <stdlib.h>

/*
VS2019开启多线程支持

Solution Property -> Configuration: (All Configurations) |  Platform: (All Platforms)
Solution Property -> Configuration Properties -> C/C++ -> Language -> Comformance mode (No)
Solution Property -> Configuration Properties -> C/C++ -> Language -> Open MP Support (Yes)

*/

int main()
{
    std::cout << "Hello World!\n";

#pragma omp parallel
    {
        printf("Hello World... from thread = %d\n", omp_get_thread_num());

#pragma omp for             //在并行区域运行多个循环    
        for (int i =0;i<10;++i)
        {
            printf("Hello World... from for loop (1) = %d\n", i);
        }
    }

#pragma omp parallel for   //N个线程会衍生出N个线程来执行循环
		for (int i = 0; i < 10; ++i)
		{
			printf("Hello World... from for loop (2) = %d\n", i);
		}
}

// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file
