#pragma once

#define EVENT_FUNC(func) [&](EventParam& __param) { func(__param); }

#define STL_VECTOR_SAFE_CLEAR(list)\
for(size_t i = 0; i< (list).size();i++)\
{\
    if((list)[i] != nullptr)\
        delete (list)[i];\
}\
(list).clear();
/**********************************************************************************/

#define STL_VECTOR_FIND(v,e) (find(v.begin(), v.end(), e) != v.end())
#define STL_VECTOR_PTR_FIND(v,e) (find(v->begin(), v->end(), e) != v->end())

/**********************************************************************************/
//source code:
/*
_GLIBCXX_WEAK_DEFINITION void
operator delete(void* ptr) throw ()
{
  if (ptr)
    std::free(ptr);
}
*/
//不需要判空，delete操作内置判空了
/*
#define SAFE_DELETE_PTR(p)\
delete (p);\
(p) = nullptr;\


#define SAFE_DELETE_ARRAY_PTR(p)\
delete[] (p);\
(p) = nullptr;\
*/
#define SAFE_DELETE_PTR(p)\
if((p) != nullptr)\
{\
    delete (p);\
    (p) = nullptr;\
}

#define SAFE_DELETE_ARRAY_PTR(p)\
if((p) != nullptr)\
{\
delete[] (p);\
(p) = nullptr;\
}

/**********************************************************************************/
//如果map中存在则替换map中已存在元素，否则添加元素
#define ADD_MAP_ITME_OR_REPLACE_MAP_ITEM(key,value,mapcontainer)\
if(mapcontainer.find(key) == mapcontainer.end())\
{\
    mapcontainer.insert({ key, value });\
}\
else\
{\
    mapcontainer[key] = value; \
}
/**********************************************************************************/
//如果map中不存在则添加元素
#define ADD_MAP_ITME_IF_NOT_EXIST(key,value,mapcontainer)\
if (mapcontainer.find(key) == mapcontainer.end())\
{\
    mapcontainer.insert({ key, value }); \
}
/**********************************************************************************/
