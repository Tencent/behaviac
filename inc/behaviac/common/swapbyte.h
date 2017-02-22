/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Tencent is pleased to support the open source community by making behaviac available.
//
// Copyright (C) 2015-2017 THL A29 Limited, a Tencent company. All rights reserved.
//
// Licensed under the BSD 3-Clause License (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at http://opensource.org/licenses/BSD-3-Clause
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef _BEHAVIAC_COMMON_SWAPBYTE_H_
#define _BEHAVIAC_COMMON_SWAPBYTE_H_

#include "behaviac/common/assert.h"
#include "behaviac/common/staticassert.h"

//////////////////////////////////////////////////////////////////////////
#if defined(_WIN32)&&!defined(_DEBUG)

#define BEHAVIAC_BYTESWAPED_16(x)    (_byteswap_ushort(x))
#define BEHAVIAC_BYTESWAPED_32(x)    (_byteswap_ulong(x))
#define BEHAVIAC_BYTESWAPED_64(x)    (_byteswap_uint64(x))

#else

#define BEHAVIAC_BYTESWAPED_16(x)    ((((x) >> 8) & 0xff) | (((x) & 0xff) << 8))

#define BEHAVIAC_BYTESWAPED_32(x)    ((((x) >> 24) & 0x000000ff) |          \
                                      (((x) >>  8) & 0x0000ff00) |          \
                                      (((x) <<  8) & 0x00ff0000) |          \
                                      (((x) << 24) & 0xff000000))

#define BEHAVIAC_BYTESWAPED_64(x)    ((((x) >> 56) & 0x00000000000000ffLL) |  \
                                      (((x) >> 40) & 0x000000000000ff00LL) |  \
                                      (((x) >> 24) & 0x0000000000ff0000LL) |  \
                                      (((x) >>  8) & 0x00000000ff000000LL) |  \
                                      (((x) <<  8) & 0x000000ff00000000LL) |  \
                                      (((x) << 24) & 0x0000ff0000000000LL) |  \
                                      (((x) << 40) & 0x00ff000000000000LL) |  \
                                      (((x) << 56) & 0xff00000000000000LL))
#endif

//////////////////////////////////////////////////////////////////////////
namespace behaviac {
    class CSwapByteAll {
    private:
        CSwapByteAll();
        CSwapByteAll(const CSwapByteAll&);
        CSwapByteAll& operator=(const CSwapByteAll&);
    public:
        template< int SIZE >
        static inline void SwapSized(char*);

        template< typename T >
        static inline void Swap(T& value);
    };

    template<>
    inline void CSwapByteAll::SwapSized< 1 >(char*) {
    }

    template<>
    inline void CSwapByteAll::SwapSized< 2 >(char* p) {
        unsigned short& v = *reinterpret_cast<unsigned short*>(p);
        v = BEHAVIAC_BYTESWAPED_16(v);
    }

    template<>
    inline void CSwapByteAll::SwapSized< 4 >(char* p) {
        unsigned int& v = *reinterpret_cast<unsigned int*>(p);
        v = BEHAVIAC_BYTESWAPED_32(v);
    }

    template<>
    inline void CSwapByteAll::SwapSized< 8 >(char* p) {
        unsigned long long& v = *reinterpret_cast<unsigned long long*>(p);
        v = BEHAVIAC_BYTESWAPED_64(v);
    }

    template< typename T >
    inline void CSwapByteAll::Swap(T& v) {
        SwapSized<sizeof(T)>(reinterpret_cast<char*>(&v));
    }

    template<>
    inline void CSwapByteAll::Swap(float& v) {
        BEHAVIAC_STATIC_ASSERT(sizeof(float) == sizeof(uint32_t));
        union {
            uint32_t n;
            float f;
        } temp;
        temp.f = v;
        temp.n = BEHAVIAC_BYTESWAPED_32(temp.n);
        v = temp.f;
    }

    template<>
    inline void CSwapByteAll::Swap(double& v) {
        BEHAVIAC_STATIC_ASSERT(sizeof(double) == sizeof(uint64_t));
        union {
            uint64_t n;
            double f;
        } temp;
        temp.f = v;
        temp.n = BEHAVIAC_BYTESWAPED_64(temp.n);
        v = temp.f;
    }

    //////////////////////////////////////////////////////////////////////////
    class CSwapByteNo {
    private:
        CSwapByteNo();
        CSwapByteNo(const CSwapByteNo&);
        CSwapByteNo& operator=(const CSwapByteNo&);

    public:
        template< int SIZE >
        static inline void SwapSized(char*) {
        }

        template< typename T >
        static inline void Swap(T& value) {
            BEHAVIAC_UNUSED_VAR(value);
        }
    };

    template <class Swapper, bool ConvertARGB2RGBA, bool Swap4Byte>
    class CSwapByteVertex : public Swapper {
    private:
        CSwapByteVertex();
        CSwapByteVertex(const CSwapByteVertex&);
        CSwapByteVertex& operator=(const CSwapByteVertex&);
    public:
        static const bool ConvertVertexColor = ConvertARGB2RGBA;
        static const bool SwapVertex4Byte = Swap4Byte;
    };

    //////////////////////////////////////////////////////////////////////////
#if BEHAVIAC_CCDEFINE_BIGENDIAN
    typedef CSwapByteAll	CSwapByteNativeToLittle;
    typedef CSwapByteAll	CSwapByteLittleToNative;
    typedef CSwapByteNo	CSwapByteNativeToBig;
    typedef CSwapByteNo	CSwapByteBigToNative;
#else
    typedef CSwapByteNo	CSwapByteNativeToLittle;
    typedef CSwapByteNo	CSwapByteLittleToNative;
    typedef CSwapByteAll	CSwapByteNativeToBig;
    typedef CSwapByteAll	CSwapByteBigToNative;
#endif

    typedef CSwapByteLittleToNative CSwapByteDefault;
}//

//////////////////////////////////////////////////////////////////////////
namespace behaviac {
    template< typename SWAPPER, typename T >
    inline void SwapByteSizedImplement(T& t) {
        SWAPPER::SwapSized< sizeof(t) >((char*)&t);
    }

    template< typename SWAPPER, typename T >
    inline void SwapByteImplement(T& t) {
        BEHAVIAC_UNUSED_VAR(t);
    }

    //
    template< typename SWAPPER, typename IT >
    inline void SwapByteImplement(IT itBegin, const IT& itEnd) {
        while (itBegin != itEnd) {
            SwapByteImplement< SWAPPER >(*itBegin);
            ++itBegin;
        };
    }

    template< typename SWAPPER, typename T >
    inline void SwapByteArrayImplement(T* array, uint32_t arraySize) {
        for (unsigned int i = 0; i < arraySize; ++i) {
            SwapByteImplement< SWAPPER >(array[i]);
        }
    }
}//

#define behaviacSwapByte		behaviac::SwapByteImplement	  < behaviac::CSwapByteDefault >
#define behaviacSwapByteSized	behaviac::SwapByteSizedImplement< behaviac::CSwapByteDefault >
#define behaviacSwapByteArray   behaviac::SwapByteArrayImplement< behaviac::CSwapByteDefault >

#define BEHAVIAC_MEMBER_SWAPBYTE_IMPLEMENT( base_type )     \
    namespace behaviac {									\
        template< typename SWAPPER >                        \
        inline void SwapByteImplement(base_type & value)    \
        {                                                   \
            SWAPPER::Swap(value);                           \
        }													\
    }//

BEHAVIAC_MEMBER_SWAPBYTE_IMPLEMENT(bool)
BEHAVIAC_MEMBER_SWAPBYTE_IMPLEMENT(char)   // "char" is not "signed char" or "unsigned char"
BEHAVIAC_MEMBER_SWAPBYTE_IMPLEMENT(unsigned char)
BEHAVIAC_MEMBER_SWAPBYTE_IMPLEMENT(signed char)
BEHAVIAC_MEMBER_SWAPBYTE_IMPLEMENT(unsigned short)
BEHAVIAC_MEMBER_SWAPBYTE_IMPLEMENT(signed short)
BEHAVIAC_MEMBER_SWAPBYTE_IMPLEMENT(unsigned int)
BEHAVIAC_MEMBER_SWAPBYTE_IMPLEMENT(signed int)
BEHAVIAC_MEMBER_SWAPBYTE_IMPLEMENT(unsigned long)
BEHAVIAC_MEMBER_SWAPBYTE_IMPLEMENT(signed long)
BEHAVIAC_MEMBER_SWAPBYTE_IMPLEMENT(unsigned long long)
BEHAVIAC_MEMBER_SWAPBYTE_IMPLEMENT(signed long long)
BEHAVIAC_MEMBER_SWAPBYTE_IMPLEMENT(float)
BEHAVIAC_MEMBER_SWAPBYTE_IMPLEMENT(double)

#undef BEHAVIAC_MEMBER_SWAPBYTE_IMPLEMENT

//////////////////////////////////////////////////////////////////////////
template< typename SWAPPER >
inline void SwapByteImplement(const char*& s) {
    BEHAVIAC_UNUSED_VAR(s);
}

template< typename SWAPPER >
inline void SwapByteImplement(behaviac::string& s) {
    BEHAVIAC_UNUSED_VAR(s);
}

template<typename SWAPPER>
inline void SwapByteImplement(behaviac::CStringCRC& value) {
    behaviac::CStringCRC::IDType id = value.GetUniqueID();
    behaviacSwapByte(id);

    value.SetUniqueID(id);
}

//////////////////////////////////////////////////////////////////////////
template<typename SWAPPER, typename T>
inline void SwapByteImplement(behaviac::vector<T>& value) {
    for (unsigned int i = 0; i < value.size(); ++i) {
        SwapByteImplement< SWAPPER >(value[i]);
    }
}

template<typename SWAPPER>
inline void SwapByteImplement(behaviac::vector<bool>& value) {
    BEHAVIAC_UNUSED_VAR(value);
}



#endif // #ifndef _BEHAVIAC_COMMON_SWAPBYTE_H_
