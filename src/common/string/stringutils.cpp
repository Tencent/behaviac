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

#include "behaviac/common/base.h"
#include "behaviac/common/string/stringutils.h"

#if BEHAVIAC_CCDEFINE_MSVC
#include <windows.h>
#endif//#if BEHAVIAC_CCDEFINE_MSVC

namespace behaviac {
    namespace StringUtils {
        // convert multibyte string to wide char string
        bool MBSToWCS(behaviac::wstring& resultString, const behaviac::string& str, const char* locale) {
            BEHAVIAC_UNUSED_VAR(resultString);
            BEHAVIAC_UNUSED_VAR(str);
            BEHAVIAC_UNUSED_VAR(locale);

            bool ret = false;

#if BEHAVIAC_CCDEFINE_MSVC
            const int cp = (strcmp(locale, LOCALE_CN_UTF8) == 0) ? CP_UTF8 : CP_ACP;
            const int dwNum = MultiByteToWideChar(cp, 0, str.c_str(), -1, 0, 0);
            wchar_t* buffer = (wchar_t*)BEHAVIAC_MALLOC_WITHTAG(dwNum * 2 + 2, "MBSToWCS");

            if (buffer) {
                MultiByteToWideChar(cp, 0, str.c_str(), -1, buffer, dwNum);

                resultString = buffer;
                BEHAVIAC_FREE(buffer);

                ret = true;
            }

#else
            uint32_t dwNum = (str.size() + 1) * 4;
            wchar_t* buffer = (wchar_t*)BEHAVIAC_MALLOC_WITHTAG(dwNum, "MBSToWCS");

            if (buffer) {
                //remember it to restore it later
                char* currrentLocale = setlocale(LC_ALL, 0);

                char* loc = setlocale(LC_ALL, locale);

                if (loc) {
                    mbstowcs(buffer, str.c_str(), dwNum);
                    ret = true;
                }

                //restore
                setlocale(LC_ALL, currrentLocale);

                resultString = buffer;
                BEHAVIAC_FREE(buffer);
            }

#endif//#if BEHAVIAC_CCDEFINE_MSVC

            return ret;
        }

        // convert multibyte string to wide char string
        behaviac::wstring MBSToWCS(const behaviac::string& str, const char* locale) {
            behaviac::wstring resultString;
            MBSToWCS(resultString, str, locale);
            return resultString;
        }

        // convert wide char string to multibyte string
        bool WCSToMBS(behaviac::string& resultString, const behaviac::wstring& wstr, const char* locale) {
            BEHAVIAC_UNUSED_VAR(resultString);
            BEHAVIAC_UNUSED_VAR(wstr);
            BEHAVIAC_UNUSED_VAR(locale);

            bool ret = false;

#if BEHAVIAC_CCDEFINE_MSVC
            const int cp = (strcmp(locale, LOCALE_CN_UTF8) == 0) ? CP_UTF8 : CP_ACP;
            const int dwNum = WideCharToMultiByte(cp, 0, wstr.c_str(), -1, NULL, 0, NULL, NULL);
            char* buffer = (char*)BEHAVIAC_MALLOC_WITHTAG(dwNum * 2 + 2, "WCSToMBS");

            if (buffer) {
                WideCharToMultiByte(cp, 0, wstr.c_str(), -1, buffer, dwNum, NULL, NULL);

                resultString = buffer;
                BEHAVIAC_FREE(buffer);

                ret = true;
            }

#else
            uint32_t dwNum = (wstr.size() + 1) * 4;
            char* buffer = (char*)BEHAVIAC_MALLOC_WITHTAG(dwNum, "WCSToMBS");

            if (buffer) {
                //remember it to restore it later
                char* currrentLocale = setlocale(LC_ALL, 0);

                char* loc = setlocale(LC_ALL, locale);

                if (loc) {
                    wcstombs(buffer, wstr.c_str(), dwNum);
                    ret = true;
                }

                //restore
                setlocale(LC_ALL, currrentLocale);

                resultString = buffer;
                BEHAVIAC_FREE(buffer);
            }

#endif//#if BEHAVIAC_CCDEFINE_MSVC

            return ret;
        }

        // convert wide char string to multibyte string
        behaviac::string WCSToMBS(const behaviac::wstring& wstr, const char* locale) {
            behaviac::string resultString;
            WCSToMBS(resultString, wstr, locale);
            return resultString;
        }
    }
}
