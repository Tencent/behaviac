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

#ifndef _BEHAVIAC_COMMON_PARSESTRING_H_
#define _BEHAVIAC_COMMON_PARSESTRING_H_

#include "behaviac/common/base.h"
#include "behaviac/common/container/string.h"
#include "behaviac/common/container/vector.h"
#include "behaviac/common/container/list.h"
#include "behaviac/common/container/map.h"
#include "behaviac/common/container/set.h"

#include "behaviac/common/meta/meta.h"


namespace behaviac {
    template<typename T>
    bool EnumValueFromString(const char* valueStr, T& v);

    namespace StringUtils {
        template<typename T>
        bool ParseString(const char* str, T& val);

		template<typename T>
		inline bool FromString_Struct(const char* str, T& val);

        namespace internal {
            template<typename T, bool bAgent>
            struct FromStringSelector {
                static bool ParseString(const char* str, T& val) {
                    BEHAVIAC_UNUSED_VAR(str);
                    BEHAVIAC_UNUSED_VAR(val);

                    return false;
                }
            };

            template<typename T>
            struct FromStringSelector<T, true> {
                static bool ParseString(const char* str, T& val) {
                    BEHAVIAC_UNUSED_VAR(str);
                    BEHAVIAC_ASSERT(string_nicmp(str, "null", 4) == 0);
                    val = 0;
                    return true;
                }
            };

            template<typename T>
            inline bool ParseString(const char* str, T& val) {
                bool result = FromStringSelector<T, behaviac::Meta::IsRefType<T>::Result>::ParseString(str, val);

                return result;
            }

            inline bool ParseString(const char* str, bool& val) {
                if ((str[0] == '0' || str[0] == '1') && str[1] == '\0') {
                    val = *str == '1';
                    return true;

                } else {
                    if (string_nicmp(str, "true", 4) == 0) {
                        val = true;
                        return true;

                    } else if (string_nicmp(str, "false", 5) == 0) {
                        val = false;
                        return true;
                    }
                }

                return false;
            }
            inline bool ParseString(const char* str, char& val) {
                return sscanf(str, "%c", &val) == 1;
            }
            inline bool ParseString(const char* str, signed char& val) {
                int i;

                if (sscanf(str, "%i", &i) == 1) {
                    BEHAVIAC_ASSERT(i >= -128 && i <= 127);
                    val = (char)i;
                    return true;
                }

                return false;
            }
            inline bool ParseString(const char* str, unsigned char& val) {
                unsigned int u;

                if (sscanf(str, "%u", &u) == 1) {
                    BEHAVIAC_ASSERT(u <= 255);
                    val = (unsigned char)u;
                    return true;
                }

                return false;
            }
            inline bool ParseString(const char* str, signed short& val) {
                int i;

                if (sscanf(str, "%i", &i) == 1) {
                    BEHAVIAC_ASSERT(i >= -32768 && i <= 32767);
                    val = (short)i;
                    return true;
                }

                return false;
            }
            inline bool ParseString(const char* str, unsigned short& val) {
                unsigned int u;

                if (sscanf(str, "%u", &u) == 1) {
                    BEHAVIAC_ASSERT(u <= 65535);
                    val = (unsigned short)u;
                    return true;
                }

                return false;
            }
            inline bool ParseString(const char* str, signed int& val) {
                return sscanf(str, "%i", &val) == 1;
            }
            inline bool ParseString(const char* str, unsigned int& val) {
                return sscanf(str, "%u", &val) == 1;
            }

            inline bool ParseString(const char* str, signed long& val) {
                return sscanf(str, "%li", &val) == 1;
            }
            inline bool ParseString(const char* str, unsigned long& val) {
                return sscanf(str, "%lu", &val) == 1;
            }
#if !BEHAVIAC_CCDEFINE_64BITS
            inline bool ParseString(const char* str, int64_t& val) {
                return sscanf(str, "%lli", &val) == 1;
            }
            inline bool ParseString(const char* str, uint64_t& val) {
                return sscanf(str, "%llu", &val) == 1;
            }
#else
            inline bool ParseString(const char* str, long long& val) {
                return sscanf(str, "%lli", &val) == 1;
            }
            inline bool ParseString(const char* str, unsigned long long& val) {
                return sscanf(str, "%llu", &val) == 1;
            }
#endif
            inline bool ParseString(const char* str, float& val) {
                return sscanf(str, "%f", &val) == 1;
            }

            inline bool ParseString(const char* str, double& val) {
                return sscanf(str, "%lg", &val) == 1;
            }

            inline bool ParseString(const char* str, void*& val) {
                return sscanf(str, "%p", &val) == 1;
            }

            inline bool ParseString(const char* str, behaviac::string& val) {
                if (str) {
                    if (str[0] == '"') {
                        //strip the start/end ""
                        int length = (int)strlen(str) - 2;
                        val.resize(length);

                        char* destP = (char*)val.data();

                        const char* p = str + 1;
                        const char* endP = p + length;

                        while (p != endP) {
                            *destP++ = *p++;
                        }

                        return true;

                    } else if (str[0] == '\0') {
                        val = "";
                        return true;

                    } else {
                        val = str;
                        return true;
                    }
                } else {
                    val = "";
                    return true;
                }
            }

            inline bool ParseString(const char* str, behaviac::wstring& val) {
                if (str) {
                    if (str[0] == '"') {
                        //strip the start/end ""
                        behaviac::string valT = str + 1;

                        int length = (int)valT.size();
                        valT[length - 1] = '\0';

                        behaviac::StringUtils::Char2Wide(val, valT);

                        return true;
                    }

                    val = behaviac::StringUtils::Char2Wide(str);

                    return true;

                } else {
                    val = L"";
                }

                return false;
            }

            //inline bool ParseString(const char* str, const char*& val)
            //{
            //	val = str;
            //	return true;
            //}

            template <typename T> void ContainerReserve(behaviac::vector<T>& container, uint32_t size) {
                container.reserve(size);
            }
            template <typename T> void ContainerReserve(behaviac::list<T>& container, uint32_t size) {}
            template <typename T> void ContainerReserve(behaviac::set<T>& container, uint32_t size) {}
            template <typename T> void ContainerInsert(behaviac::vector<T>& container, T item) {
                container.push_back(item);
            }
            template <typename T> void ContainerInsert(behaviac::list<T>& container, T item) {
                container.push_back(item);
            }
            template <typename T> void ContainerInsert(behaviac::set<T>& container, T item) {
                container.insert(item);
            }

            template <typename T, typename ContentType> bool ContainerFromStringPrimtive(const char* str, T& contVal, const ContentType&) {
                contVal.clear();
                const char* pos = str;

                uint32_t count = 0;

                if (sscanf(pos, "%u:", &count) == 1) {
                    if (count > 0) {
                        ContainerReserve(contVal, count);
                        pos = strchr(pos, ':');

                    } else {
                        return true;
                    }
                } else {
                    BEHAVIAC_LOGWARNING("Fail read container count from behaviac::string");
                    return false;
                }

                do {
                    ContentType itemVal;

                    //this only works for primitive type, if the element type is a struct, it can't work
                    behaviac::string itemTemp;
                    const char* posNext = strchr(pos + 1, '|');

                    if (posNext) {
                        //*(char*)posNext = '\0';
                        size_t len = posNext - (pos + 1);
                        itemTemp.resize(len);
                        char* data = (char*)itemTemp.data();

                        for (size_t i = 0; i < len; ++i) {
                            data[i] = pos[i + 1];
                        }
                    } else {
                        itemTemp = pos + 1;
                    }

                    if (behaviac::StringUtils::ParseString(itemTemp.c_str(), itemVal)) {
                        ContainerInsert(contVal, itemVal);

                        if (pos[1] == '{') {
                            //struct array
                            pos = SkipPairedBrackets(pos + 1);
                        }

                        const char* posNext1 = strchr(pos + 1, '|');
                        pos = posNext1;
                    } else {
                        BEHAVIAC_LOGWARNING("Fail read container from behaviac::string at position %u", static_cast<unsigned int>(pos - str));
                        return false;
                    }
                } while (pos && *(pos + 1));

                return true;
            }

            template <typename T, typename ContentType> bool ContainerFromStringStruct(const char* str, T& contVal, const ContentType&) {
                contVal.clear();
                const char* pos = str;

                uint32_t count = 0;

                if (sscanf(pos, "%u:", &count) == 1) {
                    if (count > 0) {
                        ContainerReserve(contVal, count);
                        pos = strchr(pos, ':');

                    } else {
                        return true;
                    }
                } else {
                    BEHAVIAC_LOGWARNING("Fail read container count from behaviac::string");
                    return false;
                }

                do {
                    ContentType itemVal;

                    if (behaviac::StringUtils::ParseString(pos + 1, itemVal)) {
                        ContainerInsert(contVal, itemVal);

                        if (pos[1] == '{') {
                            //struct array
                            pos = SkipPairedBrackets(pos + 1);
                        }

                        const char* posNext = strchr(pos + 1, '|');
                        pos = posNext;
                    } else {
                        BEHAVIAC_LOGWARNING("Fail read container from behaviac::string at position %u", static_cast<unsigned int>(pos - str));
                        return false;
                    }
                } while (pos && *(pos + 1));

                return true;
            }

            template<typename ContentType, bool bHasFromString>
            struct ContainerFromStringSelector {
                template<typename T>
                static bool ParseString(const char* str, T& val, const ContentType& element) {
                    return ContainerFromStringPrimtive(str, val, element);
                }
            };

            template<typename ContentType>
            struct ContainerFromStringSelector < ContentType, true > {
                template<typename T>
                static bool ParseString(const char* str, T& val, const ContentType& element) {
                    return ContainerFromStringStruct(str, val, element);
                }
            };

            template <typename T> bool ParseString(const char* str, behaviac::vector<T>& contVal) {
                return ContainerFromStringSelector<T, behaviac::Meta::HasFromString<T>::Result>::ParseString(str, contVal, T());
            }
            template <typename T> bool ParseString(const char* str, behaviac::list<T>& contVal) {
                return ContainerFromStringSelector<T, behaviac::Meta::HasFromString<T>::Result>::ParseString(str, contVal, T());
            }
            template <typename T> bool ParseString(const char* str, behaviac::set<T>& contVal) {
                return ContainerFromStringSelector<T, behaviac::Meta::HasFromString<T>::Result>::ParseString(str, contVal, T());
            }
        }//namespace internal

        namespace Detail {
            ////////////////////////////////////////////////////////////////////////
            template<typename T, bool bIsEnum>
            struct FromStringEnumHanler {
                static bool ParseString(const char* valueStr, T& v) {
                    if (internal::ParseString(valueStr, v)) {
                        return true;
                    }

                    return false;
                }
            };

            template<typename T>
            struct FromStringEnumHanler<T, true> {
                static bool ParseString(const char* valueStr, T& v) {
                    return behaviac::EnumValueFromString(valueStr, v);
                }
            };

            template<typename T, bool bHasFromString>
            struct FromStringStructHanler {
                static bool ParseString(const char* str, T& val) {
                    return FromStringEnumHanler<T, behaviac::Meta::IsEnum<T>::Result>::ParseString(str, val);
                }
            };

            template<typename T>
            struct FromStringStructHanler<T, true> {
                static bool ParseString(const char* str, T& val) {
					return behaviac::StringUtils::FromString_Struct(str, val);
                }
            };
        }//namespace Detail

        template<typename T>
        inline bool ParseString(const char* str, T& val) {
            return Detail::FromStringStructHanler<T, behaviac::Meta::HasFromString<T>::Result>::ParseString(str, val);
        }
    }
}

#endif // #ifndef _BEHAVIAC_COMMON_PARSESTRING_H_
