#include <Windows.h>
#include <iostream>
using namespace std;

int main()
{
	cout << "start" << endl;

	Sleep(1000 * 10);
	int* a = nullptr;
	a[1] = 2;
	cout << "end" << endl;

	return 0;
}