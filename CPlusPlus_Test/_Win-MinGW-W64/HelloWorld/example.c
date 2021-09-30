#include <stdio.h>

long func(int a){
    long sum = 0;
    for(int j=1;j<=a;j++){
    	sum += j;
    }
    return sum;
}

int main(void){
    int a =100;
    long sum = func(a);
    printf("%ld",sum);
    getchar();
    return 0;
}