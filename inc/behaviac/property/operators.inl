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

#ifndef _BEHAVIAC_OPERATORS_INL_H_
#define _BEHAVIAC_OPERATORS_INL_H_

#include "behaviac/common/meta/meta.h"

namespace behaviac
{
	bool Equal_Struct(void* lhs, void* rhs, const char* szClassName);

	namespace PrivateDetails
    {
        namespace Meta
        {
            template<typename Type>
            struct HasEqual
            {
            private:

                template<typename U, bool (U::*)(const U&) const> struct TPROTOTYPE {};

                template< typename U >
				static behaviac::Meta::Yes TYesNoTester(TPROTOTYPE<U, &U::_Object_Equal_>*);

                template<typename U>
                static behaviac::Meta::No TYesNoTester(...);

            public:

                enum
                {
                    Result = sizeof(TYesNoTester<Type>(0)) == sizeof(behaviac::Meta::Yes)
                };
            };
        }

        template<typename T, bool bHasEqual>
        struct TCompareOperatorStruct
        {
            //------------------------------------------------------------------------
            static bool Equal(const T& lhs, const T& rhs)
            {
                BEHAVIAC_UNUSED_VAR(lhs);
                BEHAVIAC_UNUSED_VAR(rhs);

                //please specialize your call if your code runs here
                return true;
            }

            static bool Greater(const T& lhs, const T& rhs)
            {
                BEHAVIAC_UNUSED_VAR(lhs);
                BEHAVIAC_UNUSED_VAR(rhs);

                //please specialize your call if your code runs here
                return true;
            }

            static bool GreaterEqual(const T& lhs, const T& rhs)
            {
                BEHAVIAC_UNUSED_VAR(lhs);
                BEHAVIAC_UNUSED_VAR(rhs);

                //please specialize your call if your code runs here
                return true;
            }

            static bool Less(const T& lhs, const T& rhs)
            {
                BEHAVIAC_UNUSED_VAR(lhs);
                BEHAVIAC_UNUSED_VAR(rhs);

                //please specialize your call if your code runs here
                return false;
            }

            static bool LessEqual(const T& lhs, const T& rhs)
            {
                BEHAVIAC_UNUSED_VAR(lhs);
                BEHAVIAC_UNUSED_VAR(rhs);

                return false;
            }
        };

        template<bool bHasEqual>
        struct TCompareOperatorStruct<void*, bHasEqual>
        {
            //------------------------------------------------------------------------
            static bool Equal(const void* lhs, const void* rhs)
            {
                return lhs == rhs;
            }

            static bool Greater(const void* lhs, const void* rhs)
            {
                return lhs > rhs;
            }

            static bool GreaterEqual(const void* lhs, const void* rhs)
            {
                return lhs >= rhs;
            }

            static bool Less(const void* lhs, const void* rhs)
            {
                return lhs < rhs;
            }

            static bool LessEqual(const void* lhs, const void* rhs)
            {
                return lhs <= rhs;
            }
        };

        template<typename T>
        struct TCompareOperatorStruct<T, true>
        {
            static bool Equal(const T& lhs, const T& rhs)
            {
				const char* szClassName = GetClassTypeName((T*)0);
				return behaviac::Equal_Struct(lhs, rhs, szClassName);
            }

            static bool Greater(const T& lhs, const T& rhs)
            {
                BEHAVIAC_UNUSED_VAR(lhs);
                BEHAVIAC_UNUSED_VAR(rhs);

                return true;
            }

            static bool GreaterEqual(const T& lhs, const T& rhs)
            {
                BEHAVIAC_UNUSED_VAR(lhs);
                BEHAVIAC_UNUSED_VAR(rhs);

                return true;
            }

            static bool Less(const T& lhs, const T& rhs)
            {
                BEHAVIAC_UNUSED_VAR(lhs);
                BEHAVIAC_UNUSED_VAR(rhs);

                return false;
            }

            static bool LessEqual(const T& lhs, const T& rhs)
            {
                BEHAVIAC_UNUSED_VAR(lhs);
                BEHAVIAC_UNUSED_VAR(rhs);

                return false;
            }
        };

        template <typename T, bool bIsEnum>
        struct TCompareOperatorEnum
        {
            static bool Equal(const T& lhs, const T& rhs)
            {
                return TCompareOperatorStruct<T, PrivateDetails::Meta::HasEqual<T>::Result>::Equal(lhs, rhs);
            }

            static bool Greater(const T& lhs, const T& rhs)
            {
                return TCompareOperatorStruct<T, PrivateDetails::Meta::HasEqual<T>::Result>::Greater(lhs, rhs);
            }

            static bool GreaterEqual(const T& lhs, const T& rhs)
            {
                return TCompareOperatorStruct<T, PrivateDetails::Meta::HasEqual<T>::Result>::GreaterEqual(lhs, rhs);
            }

            static bool Less(const T& lhs, const T& rhs)
            {
                return TCompareOperatorStruct<T, PrivateDetails::Meta::HasEqual<T>::Result>::Less(lhs, rhs);
            }

            static bool LessEqual(const T& lhs, const T& rhs)
            {
                return TCompareOperatorStruct<T, PrivateDetails::Meta::HasEqual<T>::Result>::LessEqual(lhs, rhs);
            }
        };

        template <typename T>
        struct TCompareOperatorEnum<T, true>
        {
            //------------------------------------------------------------------------
            static bool Equal(const T& lhs, const T& rhs)
            {
                return (unsigned int)lhs == (unsigned int)rhs;
            }

            static bool Greater(const T& lhs, const T& rhs)
            {
                return (unsigned int)lhs > (unsigned int)rhs;
            }

            static bool GreaterEqual(const T& lhs, const T& rhs)
            {
                return (unsigned int)lhs >= (unsigned int)rhs;
            }

            static bool Less(const T& lhs, const T& rhs)
            {
                return (unsigned int)lhs < (unsigned int)rhs;
            }

            static bool LessEqual(const T& lhs, const T& rhs)
            {
                return (unsigned int)lhs <= (unsigned int)rhs;
            }
        };

        template <typename T, bool bPtr>
        struct TCompareOperatorPtr
        {
            //------------------------------------------------------------------------
            static bool Equal(const T& lhs, const T& rhs)
            {
                return TCompareOperatorEnum<T, behaviac::Meta::IsEnum<T>::Result>::Equal(lhs, rhs);
            }

            static bool Greater(const T& lhs, const T& rhs)
            {
                return TCompareOperatorEnum<T, behaviac::Meta::IsEnum<T>::Result>::Greater(lhs, rhs);
            }

            static bool GreaterEqual(const T& lhs, const T& rhs)
            {
                return TCompareOperatorEnum<T, behaviac::Meta::IsEnum<T>::Result>::GreaterEqual(lhs, rhs);
            }

            static bool Less(const T& lhs, const T& rhs)
            {
                return TCompareOperatorEnum<T, behaviac::Meta::IsEnum<T>::Result>::Less(lhs, rhs);
            }

            static bool LessEqual(const T& lhs, const T& rhs)
            {
                return TCompareOperatorEnum<T, behaviac::Meta::IsEnum<T>::Result>::LessEqual(lhs, rhs);
            }
        };

        template <typename T>
        struct TCompareOperatorPtr<T, true>
        {
            //------------------------------------------------------------------------
            static bool Equal(const T& lhs, const T& rhs)
            {
                return (behaviac::Address)lhs == (behaviac::Address)rhs;
            }

            static bool Greater(const T& lhs, const T& rhs)
            {
                return (behaviac::Address)lhs > (behaviac::Address)rhs;
            }

            static bool GreaterEqual(const T& lhs, const T& rhs)
            {
                return (behaviac::Address)lhs >= (behaviac::Address)rhs;
            }

            static bool Less(const T& lhs, const T& rhs)
            {
                return (behaviac::Address)lhs < (behaviac::Address)rhs;
            }

            static bool LessEqual(const T& lhs, const T& rhs)
            {
                return (behaviac::Address)lhs <= (behaviac::Address)rhs;
            }
        };

        //------------------------------------------------------------------------
        template< typename T >
        BEHAVIAC_FORCEINLINE bool Equal(const T& lhs, const T& rhs)
        {
            return TCompareOperatorPtr<T, behaviac::Meta::IsPtr<T>::Result>::Equal(lhs, rhs);
        }

        template< typename T >
        BEHAVIAC_FORCEINLINE bool Greater(const T& lhs, const T& rhs)
        {
            return TCompareOperatorPtr<T, behaviac::Meta::IsPtr<T>::Result>::Greater(lhs, rhs);
        }

        template< typename T >
        BEHAVIAC_FORCEINLINE bool GreaterEqual(const T& lhs, const T& rhs)
        {
            return TCompareOperatorPtr<T, behaviac::Meta::IsPtr<T>::Result>::GreaterEqual(lhs, rhs);
        }

        template< typename T >
        BEHAVIAC_FORCEINLINE bool Less(const T& lhs, const T& rhs)
        {
            return TCompareOperatorPtr<T, behaviac::Meta::IsPtr<T>::Result>::Less(lhs, rhs);
        }

        template< typename T >
        BEHAVIAC_FORCEINLINE bool LessEqual(const T& lhs, const T& rhs)
        {
            return TCompareOperatorPtr<T, behaviac::Meta::IsPtr<T>::Result>::LessEqual(lhs, rhs);
        }

        //------------------------------------------------------------------------
        template<>
        BEHAVIAC_FORCEINLINE bool Equal(const float& lhs, const float& rhs)
        {
            return behaviac::IsEqualWithEpsilon(lhs, rhs);
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Greater(const float& lhs, const float& rhs)
        {
            return lhs > rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool GreaterEqual(const float& lhs, const float& rhs)
        {
            return lhs >= rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Less(const float& lhs, const float& rhs)
        {
            return lhs < rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool LessEqual(const float& lhs, const float& rhs)
        {
            return lhs <= rhs;
        }

        //------------------------------------------------------------------------
        template<>
        BEHAVIAC_FORCEINLINE bool Equal(const double& lhs, const double& rhs)
        {
            return behaviac::IsEqualWithEpsilon(lhs, rhs);
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Greater(const double& lhs, const double& rhs)
        {
            return lhs > rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool GreaterEqual(const double& lhs, const double& rhs)
        {
            return lhs >= rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Less(const double& lhs, const double& rhs)
        {
            return lhs < rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool LessEqual(const double& lhs, const double& rhs)
        {
            return lhs <= rhs;
        }

        //------------------------------------------------------------------------
        template<>
        BEHAVIAC_FORCEINLINE bool Equal(const int& lhs, const int& rhs)
        {
            return lhs == rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Greater(const int& lhs, const int& rhs)
        {
            return lhs > rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool GreaterEqual(const int& lhs, const int& rhs)
        {
            return lhs >= rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Less(const int& lhs, const int& rhs)
        {
            return lhs < rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool LessEqual(const int& lhs, const int& rhs)
        {
            return lhs <= rhs;
        }

        //------------------------------------------------------------------------
        template<>
        BEHAVIAC_FORCEINLINE bool Equal(const unsigned int& lhs, const unsigned int& rhs)
        {
            return lhs == rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Greater(const unsigned int& lhs, const unsigned int& rhs)
        {
            return lhs > rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool GreaterEqual(const unsigned int& lhs, const unsigned int& rhs)
        {
            return lhs >= rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Less(const unsigned int& lhs, const unsigned int& rhs)
        {
            return lhs < rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool LessEqual(const unsigned int& lhs, const unsigned int& rhs)
        {
            return lhs <= rhs;
        }

        //------------------------------------------------------------------------
        template<>
        BEHAVIAC_FORCEINLINE bool Equal(const short& lhs, const short& rhs)
        {
            return lhs == rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Greater(const short& lhs, const short& rhs)
        {
            return lhs > rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool GreaterEqual(const short& lhs, const short& rhs)
        {
            return lhs >= rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Less(const short& lhs, const short& rhs)
        {
            return lhs < rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool LessEqual(const short& lhs, const short& rhs)
        {
            return lhs <= rhs;
        }

        //------------------------------------------------------------------------
        template<>
        BEHAVIAC_FORCEINLINE bool Equal(const unsigned short& lhs, const unsigned short& rhs)
        {
            return lhs == rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Greater(const unsigned short& lhs, const unsigned short& rhs)
        {
            return lhs > rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool GreaterEqual(const unsigned short& lhs, const unsigned short& rhs)
        {
            return lhs >= rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Less(const unsigned short& lhs, const unsigned short& rhs)
        {
            return lhs < rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool LessEqual(const unsigned short& lhs, const unsigned short& rhs)
        {
            return lhs <= rhs;
        }

        //------------------------------------------------------------------------
        template<>
        BEHAVIAC_FORCEINLINE bool Equal(const char& lhs, const char& rhs)
        {
            return lhs == rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Greater(const char& lhs, const char& rhs)
        {
            return lhs > rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool GreaterEqual(const char& lhs, const char& rhs)
        {
            return lhs >= rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Less(const char& lhs, const char& rhs)
        {
            return lhs < rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool LessEqual(const char& lhs, const char& rhs)
        {
            return lhs <= rhs;
        }

        //------------------------------------------------------------------------
        template<>
        BEHAVIAC_FORCEINLINE bool Equal(const signed char& lhs, const signed char& rhs)
        {
            return lhs == rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Greater(const signed char& lhs, const signed char& rhs)
        {
            return lhs > rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool GreaterEqual(const signed char& lhs, const signed char& rhs)
        {
            return lhs >= rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Less(const signed char& lhs, const signed char& rhs)
        {
            return lhs < rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool LessEqual(const signed char& lhs, const signed char& rhs)
        {
            return lhs <= rhs;
        }

        //------------------------------------------------------------------------
        template<>
        BEHAVIAC_FORCEINLINE bool Equal(const unsigned char& lhs, const unsigned char& rhs)
        {
            return lhs == rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Greater(const unsigned char& lhs, const unsigned char& rhs)
        {
            return lhs > rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool GreaterEqual(const unsigned char& lhs, const unsigned char& rhs)
        {
            return lhs >= rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Less(const unsigned char& lhs, const unsigned char& rhs)
        {
            return lhs < rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool LessEqual(const unsigned char& lhs, const unsigned char& rhs)
        {
            return lhs <= rhs;
        }

        //------------------------------------------------------------------------
        template<>
        BEHAVIAC_FORCEINLINE bool Equal(const bool& lhs, const bool& rhs)
        {
            return lhs == rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Greater(const bool& lhs, const bool& rhs)
        {
            return lhs > rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool GreaterEqual(const bool& lhs, const bool& rhs)
        {
            return lhs >= rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Less(const bool& lhs, const bool& rhs)
        {
            return lhs < rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool LessEqual(const bool& lhs, const bool& rhs)
        {
            return lhs <= rhs;
        }

        //------------------------------------------------------------------------
        template<>
        BEHAVIAC_FORCEINLINE bool Equal(const long& lhs, const long& rhs)
        {
            return lhs == rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Greater(const long& lhs, const long& rhs)
        {
            return lhs > rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool GreaterEqual(const long& lhs, const long& rhs)
        {
            return lhs >= rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Less(const long& lhs, const long& rhs)
        {
            return lhs < rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool LessEqual(const long& lhs, const long& rhs)
        {
            return lhs <= rhs;
        }

        //------------------------------------------------------------------------
        template<>
        BEHAVIAC_FORCEINLINE bool Equal(const unsigned long& lhs, const unsigned long& rhs)
        {
            return lhs == rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Greater(const unsigned long& lhs, const unsigned long& rhs)
        {
            return lhs > rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool GreaterEqual(const unsigned long& lhs, const unsigned long& rhs)
        {
            return lhs >= rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Less(const unsigned long& lhs, const unsigned long& rhs)
        {
            return lhs < rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool LessEqual(const unsigned long& lhs, const unsigned long& rhs)
        {
            return lhs <= rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Equal(const std::string& lhs, const std::string& rhs)
        {
            return lhs == rhs;
        }

#if BEHAVIAC_USE_CUSTOMSTRING
        //------------------------------------------------------------------------
        template<>
        BEHAVIAC_FORCEINLINE bool Equal(const behaviac::string& lhs, const behaviac::string& rhs)
        {
            return lhs == rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Greater(const behaviac::string& lhs, const behaviac::string& rhs)
        {
            return lhs > rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool GreaterEqual(const behaviac::string& lhs, const behaviac::string& rhs)
        {
            return lhs >= rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Less(const behaviac::string& lhs, const behaviac::string& rhs)
        {
            return lhs < rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool LessEqual(const behaviac::string& lhs, const behaviac::string& rhs)
        {
            return lhs <= rhs;
        }
#endif

#if !BEHAVIAC_CCDEFINE_64BITS
        //------------------------------------------------------------------------
        template<>
        BEHAVIAC_FORCEINLINE bool Equal(const int64_t& lhs, const int64_t& rhs)
        {
            return lhs == rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Greater(const int64_t& lhs, const int64_t& rhs)
        {
            return lhs > rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool GreaterEqual(const int64_t& lhs, const int64_t& rhs)
        {
            return lhs >= rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Less(const int64_t& lhs, const int64_t& rhs)
        {
            return lhs < rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool LessEqual(const int64_t& lhs, const int64_t& rhs)
        {
            return lhs <= rhs;
        }

        //------------------------------------------------------------------------
        template<>
        BEHAVIAC_FORCEINLINE bool Equal(const uint64_t& lhs, const uint64_t& rhs)
        {
            return lhs == rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Greater(const uint64_t& lhs, const uint64_t& rhs)
        {
            return lhs > rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool GreaterEqual(const uint64_t& lhs, const uint64_t& rhs)
        {
            return lhs >= rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Less(const uint64_t& lhs, const uint64_t& rhs)
        {
            return lhs < rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool LessEqual(const uint64_t& lhs, const uint64_t& rhs)
        {
            return lhs <= rhs;
        }
#else
        //------------------------------------------------------------------------
        template<>
        BEHAVIAC_FORCEINLINE bool Equal(const long long& lhs, const long long& rhs)
        {
            return lhs == rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Greater(const long long& lhs, const long long& rhs)
        {
            return lhs > rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool GreaterEqual(const long long& lhs, const long long& rhs)
        {
            return lhs >= rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Less(const long long& lhs, const long long& rhs)
        {
            return lhs < rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool LessEqual(const long long& lhs, const long long& rhs)
        {
            return lhs <= rhs;
        }

        //------------------------------------------------------------------------
        template<>
        BEHAVIAC_FORCEINLINE bool Equal(const unsigned long long& lhs, const unsigned long long& rhs)
        {
            return lhs == rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Greater(const unsigned long long& lhs, const unsigned long long& rhs)
        {
            return lhs > rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool GreaterEqual(const unsigned long long& lhs, const unsigned long long& rhs)
        {
            return lhs >= rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool Less(const unsigned long long& lhs, const unsigned long long& rhs)
        {
            return lhs < rhs;
        }

        template<>
        BEHAVIAC_FORCEINLINE bool LessEqual(const unsigned long long& lhs, const unsigned long long& rhs)
        {
            return lhs <= rhs;
        }

#endif
        template< typename T >
        BEHAVIAC_FORCEINLINE bool Equal(const behaviac::vector<T>& lhs, const behaviac::vector<T>& rhs)
        {
            if (lhs.size() != rhs.size())
            {
                return false;
            }

            for (typename behaviac::vector<T>::size_type i = 0; i < lhs.size(); ++i)
            {
                if (!Equal(lhs[i], rhs[i]))
                {
                    return false;
                }
            }

            return true;
        }

        template< typename T >
        BEHAVIAC_FORCEINLINE bool Greater(const behaviac::vector<T>& lhs, const behaviac::vector<T>& rhs)
        {
            BEHAVIAC_UNUSED_VAR(lhs);
            BEHAVIAC_UNUSED_VAR(rhs);

            //please specialize your call if your code runs here
            return true;
        }

        template< typename T >
        BEHAVIAC_FORCEINLINE bool GreaterEqual(const behaviac::vector<T>& lhs, const behaviac::vector<T>& rhs)
        {
            BEHAVIAC_UNUSED_VAR(lhs);
            BEHAVIAC_UNUSED_VAR(rhs);

            //please specialize your call if your code runs here
            return true;
        }

        template< typename T >
        BEHAVIAC_FORCEINLINE bool Less(const behaviac::vector<T>& lhs, const behaviac::vector<T>& rhs)
        {
            BEHAVIAC_UNUSED_VAR(lhs);
            BEHAVIAC_UNUSED_VAR(rhs);

            //please specialize your call if your code runs here
            return false;
        }

        template< typename T >
        BEHAVIAC_FORCEINLINE bool LessEqual(const behaviac::vector<T>& lhs, const behaviac::vector<T>& rhs)
        {
            BEHAVIAC_UNUSED_VAR(lhs);
            BEHAVIAC_UNUSED_VAR(rhs);

            return false;
        }
    }//namespace PrivatePrivateDetails
}//namespace behaviac

#endif//_BEHAVIAC_OPERATORS_INL_H_
