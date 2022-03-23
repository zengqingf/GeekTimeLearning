/*
 * cu_types.h
 *
 *  Created on: 2014-3-20
 *      Author: mickeyxu
 */

#ifndef CU_TYPES_H_
#define CU_TYPES_H_

#include "cu_os.h"

typedef unsigned char cu_uint8;
typedef signed   char cu_int8;

typedef unsigned short cu_uint16;
typedef signed   short cu_int16;

typedef unsigned int  cu_uint32;
typedef signed int    cu_int32;

typedef unsigned long long cu_uint64;
typedef signed long long   cu_int64;


#define CHECK(x,siz) ( (sizeof(x) == (siz)) ? 1 : (-1))

struct _tagCheck
{
     unsigned char  __c1[CHECK(cu_uint8,1)];
     unsigned char  __c2[CHECK(cu_int8,1)];

     unsigned char  __c3[CHECK(cu_uint16,2)];
     unsigned char  __c4[CHECK(cu_int16,2)];

     unsigned char  __c5[CHECK(cu_uint32,4)];
     unsigned char  __c6[CHECK(cu_int32,4)];

     unsigned char  __c7[CHECK(cu_uint64,8)];
     unsigned char  __c8[CHECK(cu_int64,8)];
};


#ifdef WIN32
#define cu_min min
#define cu_max max
#else
#define cu_min(a,b)            (((a) < (b)) ? (a) : (b))
#define cu_max(a,b)            (((a) > (b)) ? (a) : (b))
#endif

#endif /* CU_TYPES_H_ */
