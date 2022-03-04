#include <stdio.h>
#include <unistd.h>
#include <pthread.h>

int sum = 0;
void* task(void* ptr)
{
    for(int i = 0; i < 1000; ++i)
    {
        disable_interrupt();            //关闭中断，禁止线程调度，消除所有潜在竞争对手，影响较大
        sum++;
        enable_interrupt();             //开启中断，等待执行完后，允许其他线程执行

        pthread_mutex_lock(&lock);      //相比于中断，使用互斥锁，可以让不符合条件的线程放弃执行，进入等待状态，
        sum++
        pthread_mutex_unlock(&lock);

        usleep(10);
    }
}

int main()
{
    pthread_t thread1, thread2;
    pthread_create(&thread1, 0, task, 0);
    pthread_create(&thread2, 0, task, 0);
    pthread_join(thread1, 0);
    pthread_join(thread2, 0);
    printf("%d\n", sum);
}

/*
命令行执行

g++ test_2.cpp -lpthread

for i in {1..10}; do ./a.out; done
*/