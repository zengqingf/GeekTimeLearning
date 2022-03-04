#include <stdio.h>
#include <unistd.h>

//int a = 1;
int a = 2;
int main()
{
    printf("address: 0x%p, value: %d", &a, a);
    sleep(10000000);
}

/*
g++ .\test_1.cpp -o p1
g++ .\test_1.cpp -o p2
*/