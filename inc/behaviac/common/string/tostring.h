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

#ifndef _BEHAVIAC_COMMON_TOSTRING_H_
#define _BEHAVIAC_COMMON_TOSTRING_H_

#include "behaviac/common/base.h"
#include "behaviac/common/container/string.h"
#include "behaviac/common/container/vector.h"
#include "behaviac/common/container/list.h"
#include "behaviac/common/container/map.h"
#include "behaviac/common/container/set.h"
#include "behaviac/common/meta/meta.h"
#include "behaviac/common/string/stringcrc.h"


namespace behaviac {
    template<typename T>
    behaviac::string EnumValueToString(const T& v);

	template<typename T>
	inline const char* GetClassTypeName(T*);

    namespace StringUtils {
        template<typename T>
        behaviac::string ToString(const T& val);

		template<typename T>
		inline behaviac::string ToString_Struct(T& val, const char* szClassName);

        namespace internal {
            template<typename T>
            inline behaviac::string ToString(const T& val) {
                BEHAVIAC_UNUSED_VAR(val);

                return "";
            }

            inline behaviac::string ToString(bool val) {
                return val ? "true" : "false";
            }
            inline behaviac::string ToString(char val) {
                char str[2];
                string_sprintf(str, "%c", val);
                return str;
            }
            inline behaviac::string ToString(signed char val) {
                char str[5];
                string_sprintf(str, "%i", val);
                return str;
            }
            inline behaviac::string ToString(unsigned char val) {
                char str[5];
                string_sprintf(str, "%u", val);
                return str;
            }
            inline behaviac::string ToString(signed short val) {
                char str[7];
                string_sprintf(str, "%i", val);
                return str;
            }
            inline behaviac::string ToString(unsigned short val) {
                char str[7];
                string_sprintf(str, "%u", val);
                return str;
            }
            inline behaviac::string ToString(signed int val) {
                char str[12];
                string_sprintf(str, "%i", val);
                return str;
            }
            inline behaviac::string ToString(unsigned int val) {
                char str[12];
                string_sprintf(str, "%u", val);
                return str;
            }

            inline behaviac::string ToString(signed long val) {
                char str[12];
                string_sprintf(str, "%li", val);
                return str;
            }
            inline behaviac::string ToString(unsigned long val) {
                char str[12];
                string_sprintf(str, "%lu", val);
                return str;
            }
#if !BEHAVIAC_CCDEFINE_64BITS
            inline behaviac::string ToString(int64_t val) {
                char str[23];
                string_sprintf(str, "%lli", val);
                return str;
            }
            inline behaviac::string ToString(uint64_t val) {
                char str[23];
                string_sprintf(str, "%llu", val);
                return str;
            }
#else
            inline behaviac::string ToString(long long val) {
                char str[23];
                string_sprintf(str, "%lli", val);
                return str;
            }
            inline behaviac::string ToString(unsigned long long val) {
                char str[23];
                string_sprintf(str, "%llu", val);
                return str;
            }
#endif
            inline behaviac::string ToString(float val) {
                char str[32];
                string_sprintf(str, "%f", val);
                return str;
            }
            inline behaviac::string ToString(double val) {
                char str[32];
                string_sprintf(str, "%g", val);
                return str;
            }
            inline behaviac::string ToString(void* val) {
                char str[32];
                string_sprintf(str, "%p", val);
                return str;
            }

            inline behaviac::string ToString(const char* val) {
                behaviac::string ret(val);
                return ret;
            }

            inline behaviac::string ToString(const std::string& val) {
                return behaviac::string(val.c_str());
            }

            inline behaviac::string ToString(const std::wstring& val) {
                behaviac::wstring t(val.c_str());
                behaviac::string r = behaviac::StringUtils::Wide2Char(t);

                return behaviac::string(r.c_str());
            }

#if BEHAVIAC_USE_CUSTOMSTRING
            inline behaviac::string ToString(const behaviac::string& val) {
                return val;
            }

            inline behaviac::string ToString(const behaviac::wstring& val) {
                return behaviac::StringUtils::Wide2Char(val);
            }
#endif//

            inline behaviac::string ToString(const CStringCRC& val) {
                return val.LogStr();
            }

            template <typename T> behaviac::string ContainerToString(const T& contVal) {
                behaviac::string str;
                char count[64];
                string_sprintf(count, "%d:", (int)contVal.size());
                str = count;
                typename T::const_iterator iter = contVal.begin();
                typename T::const_iterator iterEnd = contVal.end();

                while (iter != iterEnd) {
                    str += behaviac::StringUtils::ToString(*iter);
                    str += "|";
                    ++iter;
                }

                return str;
            }
            template <typename T> behaviac::string ToString(const behaviac::vector<T>& contVal) {
                return ContainerToString(contVal);
            }
            template <typename T> behaviac::string ToString(const behaviac::list<T>& contVal) {
                return ContainerToString(contVal);
            }
            template <typename T> behaviac::string ToString(const behaviac::set<T>& contVal) {
                return ContainerToString(contVal);
            }
        }//namespace internal

        namespace Detail {
            template<typename T, bool bIsPtr>
            struct ToStringPtrHanler {
                static behaviac::string ToString(const T& v) {
                    behaviac::string valueStr = internal::ToString(v);

                    return valueStr;
                }
            };

            template<typename T>
            struct ToStringPtrHanler<T, true> {
                static behaviac::string ToString(const T& v) {
                    behaviac::string valueStr = (v == 0 ? "null" : FormatString("0x%p", v));

                    return valueStr;
                }
            };

            template<>
            struct ToStringPtrHanler<const char*, true> {
                static behaviac::string ToString(const char* v) {
                    behaviac::string valueStr = v;

                    return valueStr;
                }
            };

            template<>
            struct ToStringPtrHanler<char*, true> {
                static behaviac::string ToString(char* v) {
                    behaviac::string valueStr = v;

                    return valueStr;
                }
            };

            template<typename T, bool bIsEnum>
            struct ToStringEnumHanler {
                static behaviac::string ToString(const T& v) {
                    return ToStringPtrHanler<T, behaviac::Meta::IsPtr<T>::Result>::ToString(v);
                }
            };

            template<typename T>
            struct ToStringEnumHanler<T, true> {
                static behaviac::string ToString(const T& v) {
                    return behaviac::EnumValueToString(v);
                }
            };

            template<typename T, bool bHasToString>
            struct ToStringStructHanler {
                static behaviac::string ToString(const T& val) {
                    return ToStringEnumHanler<T, behaviac::Meta::IsEnum<T>::Result>::ToString(val);
                }
            };

            template<typename T>
            struct ToStringStructHanler<T, true> {
                static behaviac::string ToString(const T& val) {
					const char* szClassName = behaviac::GetClassTypeName((T*)0);
					return behaviac::StringUtils::ToString_Struct(val, szClassName);
                }
            };
        }//namespace Detail

        template<typename T>
        inline behaviac::string ToString(const T& val) {
            return Detail::ToStringStructHanler<T, behaviac::Meta::HasToString<T>::Result>::ToString(val);
        }
    }
}

#endif // #ifndef _BEHAVIAC_COMMON_TOSTRING_H_
