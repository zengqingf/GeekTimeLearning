#include "pre_class_allocator_1.h"

Screen* Screen::freeStore = 0;
const int Screen::screenChunk = 24;

void * Screen::operator new(size_t size)
{
	//不重载时
	//return malloc(size);

	Screen *p;
	if (!freeStore) {
		//linked list 为空，需要申请一块内存
		size_t chunk = screenChunk * size;
		freeStore = p =
			reinterpret_cast<Screen*>(new char[chunk]);
		//将一大块内存分割成片，当作linked list串接起来
		for (; p != &freeStore[screenChunk - 1]; ++p)
		{
			p->next = p + 1;
		}
		p->next = 0;
	}
	p = freeStore;
	freeStore = freeStore->next;
	return p;
}

void Screen::operator delete(void *p, size_t)
{
	//不重载时
	//free(p);
	//return;

	(static_cast<Screen*>(p))->next = freeStore;
	freeStore = static_cast<Screen*>(p);
}
