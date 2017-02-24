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

#ifndef _BEHAVIAC_COMMON_STRINGUTILS_H_
#define _BEHAVIAC_COMMON_STRINGUTILS_H_

#include "behaviac/common/base.h"
#include "behaviac/common/container/string.h"
#include "behaviac/common/container/vector.h"

#include <string>
#include <sstream>

namespace behaviac {
    namespace StringUtils {
#define LOCALE_CN_UTF8 "zh_CN.utf8"

        // convert multibyte string to wide char string
        BEHAVIAC_API bool MBSToWCS(behaviac::wstring& resultString, const behaviac::string& str, const char* locale = LOCALE_CN_UTF8);

        // convert multibyte string to wide char string
        BEHAVIAC_API behaviac::wstring MBSToWCS(const behaviac::string& str, const char* locale = LOCALE_CN_UTF8);

        // convert wide char string to multibyte string
        BEHAVIAC_API bool WCSToMBS(behaviac::string& resultString, const behaviac::wstring& wstr, const char* locale = LOCALE_CN_UTF8);

        // convert wide char string to multibyte string
        BEHAVIAC_API behaviac::string WCSToMBS(const behaviac::wstring& wstr, const char* locale = LOCALE_CN_UTF8);

		// Wide to UTF-8
		inline void Wide2Char(behaviac::string& resultString, const behaviac::wstring& wstr) {
			WCSToMBS(resultString, wstr);
		}

		// Wide to UTF-8
		inline behaviac::string Wide2Char(const behaviac::wstring& wstr) {
			behaviac::string resultString;
			Wide2Char(resultString, wstr);
			return resultString;
		}

		// UTF-8 to Wide
		inline void Char2Wide(behaviac::wstring& resultString, const behaviac::string& str) {
			MBSToWCS(resultString, str);
		}

		inline behaviac::wstring Char2Wide(const behaviac::string& str) {
			behaviac::wstring resultString;
			Char2Wide(resultString, str);
			return resultString;
		}

        //////////////////////////////////////////////////

        inline void StringCopySafe(int destMax, char* dest, const char* src) {
			int len = (int)::strlen(src);
            BEHAVIAC_ASSERT(len < destMax);
            strncpy(dest, src, len);
            dest[len] = 0;
        }

        inline const behaviac::string printf(const char* fmt, ...) {
            char tempStr[4096];
            va_list argPtr;
            va_start(argPtr, fmt);
            string_vnprintf(tempStr, 4096, fmt, argPtr);
            va_end(argPtr);
            return behaviac::string(tempStr);
        }

        inline const behaviac::wstring printf(const wchar_t* fmt, ...) {
            wchar_t tempStr[4096];
            va_list argPtr;
            va_start(argPtr, fmt);
            string_vnwprintf(tempStr, 4096, fmt, argPtr);
            va_end(argPtr);
            return behaviac::wstring(tempStr);
        }

        /////////////////////////////////////////////

        inline bool IsDigit(const char* p, bool allowNegatives = false) {
            if (allowNegatives) {
                if (p && *p == '-') {
                    p++;
                }
            }

            while (p && *p != 0 && *p >= '0' && *p <= '9') {
                p++;
            }

            return (p && *p == 0);
        }

        inline void RemoveTrailingSpaces(behaviac::string& str) {
            if (!str.empty()) {
                behaviac::string::size_type ind = str.find_last_not_of(" \t");

                if (ind == behaviac::string::npos) {
                    str.clear();

                } else {
                    str.resize(ind + 1);
                }
            }
        }

        inline void ReplaceWide(wchar_t* stringInOut, const wchar_t* stringToLookFor, wchar_t charToReplace) {
			const size_t lookForLength = wcslen(stringToLookFor);
			wchar_t* foundSomething = NULL;
            wchar_t* curPos = stringInOut;

            while ((foundSomething = wcsstr(curPos, stringToLookFor)) != NULL) {
                if (foundSomething) {
                    size_t remainingLength = wcslen(foundSomething) + 1;
                    (*foundSomething) = charToReplace;
                    ++foundSomething;

                    memmove(foundSomething, foundSomething + lookForLength - 1, (remainingLength - lookForLength) * sizeof(wchar_t));
                    curPos = foundSomething;
                }
            }
        }

        template <class TContainer>
        inline void SplitIntoArray(const behaviac::string& src, const behaviac::string& delim, TContainer& result) {
            behaviac::string tsrc = src;
            behaviac::string::size_type pos = tsrc.find(delim.c_str());
            behaviac::string::size_type length = delim.length();

            while (pos != behaviac::string::npos) {
                result.push_back(tsrc.substr(0, pos));
                tsrc = tsrc.substr(pos + length);
                pos = tsrc.find(delim.c_str());
            }

            if (tsrc.size()) {
                result.push_back(tsrc);
            }
        }

        inline bool StartsWith(const char* str, const char* token) {
            const char* p = strstr(str, token);
            return (p == str);
        }

        //get the behaviac::string before 'sep' in behaviac::string 'params' and store it into 'token'
        //@return the pointer after 'token', pointing to 'sep'
        inline const char* FirstToken(const char* params, char sep, behaviac::string& token) {
            //ex: const int 5
            const char* end = strchr(params, sep);

            if (end) {
                int length = (int)(end - params);
                token = behaviac::string(params, length);
                return end;
            }

            return 0;
        }

        inline const char* SecondeToken(const char* params, char sep, behaviac::string& token) {
            //ex: const int 5
            const char* end = strchr(params, sep);

            if (end) {
                //skip 'sep'
                end++;

                const char* end2 = strchr(end, sep);

                if (end2) {
                    int length = (int)(end2 - end);
                    token = behaviac::string(end, length);
                    return end2;

                } else {
                    //int Agent::Property
                    token = end;
                }
            }

            return 0;
        }

        inline const char* ThirdToken(const char* params, char sep, behaviac::string& token) {
            //ex: const int 5
            const char* end = strchr(params, sep);

            if (end) {
                //skip 'sep'
                end++;

                const char* end2 = strchr(end, sep);

                if (end2) {
                    end2++;
                    token = end2;
                }
            }

            return 0;
        }

        // test the string is valid string
        inline bool IsValidString(behaviac::string str) {
            if (str.length() == 0 || (str[0] == '\"' && str[1] == '\"')) {
                return false;
            }

            return true;
        }

        inline bool IsValidString(const char* str) {
            if ((!str || ::strlen(str) == 0) || (str[0] == '\"' && str[1] == '\"')) {
                return false;
            }

            return true;
        }

        // finds the behaviac::string in the array of strings
        // returns its 0-based index or -1 if not found, case-sensitive
        inline int FindString(const char* szString, const char* arrStringList[], unsigned int arrStrCount) {
            if (szString) {
                for (unsigned int i = 0; i < arrStrCount; ++i) {
                    if (0 == strcmp(arrStringList[i], szString)) {
                        return i;
                    }
                }
            }

            return -1; // behaviac::string was not found
        }

        /////////////////////////////////////////////
        // Removes the full extension of a file (".meta.xml").
        inline void StripFullFileExtension(behaviac::string& strFileName) {
            int dotPos = -1;

            while (true) {
                dotPos = (int)strFileName.find('.', dotPos + 1);

                if (dotPos < 0) {
                    break;
                }

                if (dotPos != -1 &&
                    dotPos + 1 < (int)strFileName.size() &&
                    strFileName[dotPos + 1] != '/' &&
                    strFileName[dotPos + 1] != '\\') {
                    strFileName.resize(dotPos);
                    break;
                }
            }
        }

        // Removes the full extension of a file (".meta.xml").
        inline void StripFullFileExtension(const char* in, char* out) {
            char c;

            while (*in) {
                if (*in == '.') {
                    c = in[1];

                    if (c != '.' && c != '/' && c != '\\') {
                        break;
                    }
                }

                *out++ = *in++;
            }

            *out = 0;
        }

        // searches and returns the pointer to the extension of the given file
        inline const char* FindExtension(const char* szFileName) {
            const char* szEnd = szFileName + ::strlen(szFileName);

            for (const char* p = szEnd - 1; p >= szFileName; --p) {
                if (*p == '.') {
                    return p + 1;

                } else if (*p == '/' || *p == '\\') {
                    return 0;
                }
            }

            return 0;
        }

        inline const wchar_t* FindExtension(const wchar_t* szFileName) {
            const wchar_t* szEnd = szFileName + wcslen(szFileName);

            for (const wchar_t* p = szEnd - 1; p >= szFileName; --p) {
                if (*p == '.') {
                    return p + 1;

                } else if (*p == '/' || *p == '\\') {
                    return 0;
                }
            }

            return 0;
        }

        // const version
        inline const char* FindFullExtension(const char* szFileName) {
            if (szFileName) {
                int slen = (int)::strlen(szFileName);
                const char* end = szFileName + slen;
                const char* ptr = end - 1;
                const char* dot = NULL;

                while (ptr != szFileName && *ptr != '/' && *ptr != '\\') {
                    if (*ptr == '.') {
                        dot = ptr;
                    }

                    --ptr;
                }

                return dot ? dot + 1 : end;
            }

            return szFileName;
        }


        inline  behaviac::string GetElementTypeFromName(behaviac::string typeName) {
            bool bArrayType = false;

            //array type
            if (typeName.find("vector<") != behaviac::string::npos) {
                bArrayType = true;
            }

            if (bArrayType) {
				size_t bracket0 = typeName.find('<');
				size_t bracket1 = typeName.find('>');
				size_t len = bracket1 - bracket0 - 1;

                string elementTypeName = typeName.substr(bracket0 + 1, len);

                return elementTypeName;
            }

            return "";
        }


        inline behaviac::string CombineDir(const char* path, const char* relative) {
            behaviac::string strFullPath;

            //if path hava / or \ in the end
			size_t len = ::strlen(path);

            if (path[len - 1] == '/' || path[len - 1] == '\\') {
                strFullPath = path;
            } else {
                strFullPath = path;
                strFullPath += '/';
            }

            //then process the relative path
            if (relative[0] == '/' || relative[0] == '\\') {
                const char* _relative = relative + 1;
                strFullPath += _relative;
            } else {
                strFullPath += relative;
            }

            return strFullPath;
        }

        inline behaviac::wstring CombineDir(const wchar_t* path, const wchar_t* relative) {
            behaviac::wstring strFullPath;

            //if path hava / or \ in the end
            if (path[wcslen(path) - 1] == '/' || path[wcslen(path) - 1] == '\\') {
                strFullPath = path;

            } else {
                strFullPath = path;
                strFullPath += '/';
            }

            //then process the relative path
            if (relative[0] == '/' || relative[0] == '\\') {
                const wchar_t* _relative = relative + 1;
                strFullPath += _relative;

            } else {
                strFullPath += relative;
            }

            return strFullPath;
        }

        inline bool Compare(const char* str1, const char* str2, bool bIgnoreCase = true) {
            if (bIgnoreCase) {
                return string_icmp(str1, str2) == 0;
            }

            return strcmp(str1, str2) == 0;
        }

        inline bool UnifySeparator(behaviac::string& str) {
            const char* p = str.c_str();

            char* t = (char*)p;

            while (*t) {
                if (*t == '\\') {
                    *t = '/';
                }

                ++t;
            }

            return true;
        }
        inline behaviac::string ReadToken(const char* str, int pB, int end) {
            behaviac::string strT("");
            int p = pB;

            while (p < end) {
                strT += str[p++];
            }

            return strT;
        }

        // replace all the 'search' string appear in 'subject' with string 'replace',
        // but create a copy of the string
        inline behaviac::string ReplaceString(behaviac::string subject, const behaviac::string& search, const behaviac::string& replace) {
            size_t pos = 0;

            while ((pos = subject.find(search, pos)) != std::string::npos) {
                subject.replace(pos, search.length(), replace);
                pos += replace.length();
            }

            return subject;
        }

        // replace all the 'search' string appear in 'subject' with string 'replace'
        // it does not create a copy of the string:
        inline void ReplaceStringInPlace(behaviac::string& subject, const behaviac::string& search, const behaviac::string& replace) {
            size_t pos = 0;

            while ((pos = subject.find(search, pos)) != std::string::npos) {
                subject.replace(pos, search.length(), replace);
                pos += replace.length();
            }
        }

		inline const char* SkipPairedBrackets(const char* src)
		{
			if (*src == '{')
			{
				int	depth = 0;
				const char* pos_it = src;

				while (*pos_it)
				{
					if (*pos_it == '{')
					{
						depth++;

					}
					else if (*pos_it == '}')
					{
						if (--depth == 0)
						{
							return pos_it;
						}
					}

					pos_it++;
				}
			}

			return 0;
		}

		inline unsigned int SkipPairedBrackets(const behaviac::string src)
		{
			if (src[0] == '{') {
				int	depth = 0;
				unsigned int pos = 0;

				while (pos < src.size())
				{
					if (src[pos] == '{')
					{
						depth++;

					}
					else if (src[pos] == '}')
					{
						if (--depth == 0)
						{
							return pos;
						}
					}

					pos++;
				}
			}

			return (unsigned int)-1;
		}

		//it returns true if 'str' starts with a count followed by ':'
		//3:{....}
		inline bool IsArrayString(const behaviac::string& str, size_t posStart, behaviac::string::size_type& posEnd) {
			//begin of the count of an array?
			//int posStartOld = posStart;

			bool bIsDigit = false;

			size_t strLen = str.size();

			while (posStart < strLen) {
				char c = str[posStart++];

				if (isdigit(c)) {
					bIsDigit = true;
				}
				else if (c == ':' && bIsDigit) {
					//transit_points = 3:{coordX = 0; coordY = 0; } | {coordX = 0; coordY = 0; } | {coordX = 0; coordY = 0; };
					//skip array item which is possible a struct
					int depth = 0;

					for (size_t posStart2 = posStart; posStart2 < strLen; posStart2++) {
						char c1 = str[posStart2];

						if (c1 == ';' && depth == 0) {
							//the last ';'
							posEnd = posStart2;
							break;
						}
						else if (c1 == '{') {
							BEHAVIAC_ASSERT(depth < 10);
							depth++;
						}
						else if (c1 == '}') {
							BEHAVIAC_ASSERT(depth > 0);
							depth--;
						}
					}

					return true;
				}
				else {
					break;
				}
			}

			return false;
		}

		inline behaviac::vector<behaviac::string> SplitTokensForStruct(const char* str)
		{
			behaviac::vector<behaviac::string> ret;
			behaviac::string src = str;

			//{color=0;id=;type={bLive=false;name=0;weight=0;};}
			//the first char is '{'
			//the last char is '}'
			behaviac::string::size_type posCloseBrackets = SkipPairedBrackets(src);
			BEHAVIAC_ASSERT(posCloseBrackets != behaviac::string::npos);

			//{color=0;id=;type={bLive=false;name=0;weight=0;};}
			//{color=0;id=;type={bLive=false;name=0;weight=0;};transit_points=3:{coordX=0;coordY=0;}|{coordX=0;coordY=0;}|{coordX=0;coordY=0;};}
			behaviac::string::size_type posBegin = 1;
			behaviac::string::size_type posEnd = src.find_first_of(';', posBegin);

			while (posEnd != behaviac::string::npos)
			{
				BEHAVIAC_ASSERT(src[posEnd] == ';');

				//the last one might be empty
				if (posEnd > posBegin)
				{
					behaviac::string::size_type posEqual = src.find_first_of('=', posBegin);
					BEHAVIAC_ASSERT(posEqual > posBegin);

					size_t length = posEqual - posBegin;
					behaviac::string memmberName = src.substr(posBegin, length);
					behaviac::string memmberValue;
					char c = src[posEqual + 1];

					if (c != '{')
					{
						//to check if it is an array
						IsArrayString(src, posEqual + 1, posEnd);
						length = posEnd - posEqual - 1;
						memmberValue = src.substr(posEqual + 1, length);
					}
					else
					{
						const char* pStructBegin = src.c_str();
						pStructBegin += posEqual + 1;
						const char* posCloseBrackets_ = SkipPairedBrackets(pStructBegin);
						length = posCloseBrackets_ - pStructBegin + 1;

						memmberValue = src.substr(posEqual + 1, length);

						posEnd = posEqual + 1 + length;
					}

					ret.push_back(memmberValue);
				}

				//skip ';'
				posBegin = posEnd + 1;

				//{color=0;id=;type={bLive=false;name=0;weight=0;};transit_points=3:{coordX=0;coordY=0;}|{coordX=0;coordY=0;}|{coordX=0;coordY=0;};}
				posEnd = src.find_first_of(';', posBegin);

				if (posEnd > posCloseBrackets)
				{
					break;
				}
			}

			return ret;
		}

        inline behaviac::vector<behaviac::string> SplitTokens(const char* str) {
            behaviac::vector<behaviac::string> ret;

            if (str[0] == '\"') { ///*&& str.EndsWith("\"")*/)
                BEHAVIAC_ASSERT(str[strlen(str) - 1] == '\"');
                ret.push_back(str);

                return ret;
            }

            //"int Self.AgentArrayAccessTest::ListInts[int Self.AgentArrayAccessTest::l_index]"
            int pB = 0;
            int i = 0;

            bool bBeginIndex = false;

			int strLen = (int)::strlen(str);

            while (i < strLen) {
                bool bFound = false;
                char c = str[i];

                if (c == ' ' && !bBeginIndex) {
                    bFound = true;

                } else  if (c == '[') {
                    bBeginIndex = true;
                    bFound = true;

                } else if (c == ']') {
                    bBeginIndex = false;
                    bFound = true;
                }

                if (bFound) {
                    string strT = ReadToken(str, pB, i);
                    //Debug.Check(strT.length() > 0);
                    BEHAVIAC_ASSERT(strT.length() > 0);
                    ret.push_back(strT);

                    pB = i + 1;
                }

                i++;
            }

            string t = ReadToken(str, pB, i);

            if (t.length() > 0) {
                ret.push_back(t);
            }

            return ret;
        }
        inline bool IsNullOrEmpty(const char* str) {
            if (str == NULL || str[0] == '\0') {
                return true;
            }

            return false;
        }

        inline bool EndsWith(const char* str, const char* suffix) {
            if (str == NULL || suffix == NULL) {
                return 0;
            }

            size_t str_len = ::strlen(str);
            size_t suffix_len = ::strlen(suffix);

            if (suffix_len > str_len) {
                return 0;
            }

            return 0 == strncmp(str + str_len - suffix_len, suffix, suffix_len);
        }

        inline const char* StringFind(const char* str1, const char sep) {
            return strchr(str1, sep);
        }

        inline bool StringEqual(const char* str1, const char* str2) {
            return strcmp(str1, str2) == 0;
        }

        inline bool StringEqualNoCase(const char* str1, const char* str2) {
            return string_icmp(str1, str2) == 0;
        }

        inline std::string& StringFormat(std::string& buff, const char* fmt_str, ...) {
            size_t n = 256;

            if (buff.size() < n) {
                buff.resize(n);
            } else {
                n = buff.size();
            }

            while (1) {
                va_list ap;
                va_start(ap, fmt_str);
                const int final_n = string_vnprintf(&buff[0], n, fmt_str, ap);
                va_end(ap);

                if (final_n < 0) { // encoding error
                    //n += size_t(-final_n);
                    buff = "encoding error";
                    break;
                }

                if (static_cast<size_t>(final_n) >= n) {
                    n += static_cast<size_t>(final_n) - n + 1;

                    if (n > 4096) { //
                        buff = "string too long, larger then 4096...";
                        break;
                    }

                    buff.resize(n);
                } else {
                    buff[final_n] = '\0';
                    buff.resize(final_n);
                    break;
                }
            }

            return buff;
        }

    }
}
#endif // #ifndef _BEHAVIAC_COMMON_STRINGUTILS_H_
