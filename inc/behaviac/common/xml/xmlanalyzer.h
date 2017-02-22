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

#ifndef _BEHAVIAC_COMMON_XMLANALYZER_H_
#define _BEHAVIAC_COMMON_XMLANALYZER_H_

#include "behaviac/common/xml/ixml.h"

namespace behaviac {
    class IFile;
    class BEHAVIAC_API XmlAnalyzer {
    public:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(XmlAnalyzer);

        XmlNodeReference parse(const char* fileName, const char* rootNodeName = 0, const char* suffix = 0);
        XmlNodeReference parse(IFile* file, const char* rootNodeName = 0, const char* suffix = 0, bool handleError = true);
        XmlNodeReference parseBuffer(const char* buffer, const char* rootNodeName = 0);
        XmlNodeReference parseBuffer(char* buffer, int size, const char* rootNodeName = 0);

        const char* getErrorString() const {
            return m_errorString.c_str();
        }
    private:
        behaviac::string m_errorString;
    };
}//namespace behaviac

#endif //_BEHAVIAC_COMMON_XMLANALYZER_H_
