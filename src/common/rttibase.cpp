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

#include "behaviac/common/rttibase.h"

namespace behaviac {
    char* MakeStringName1(const char* aT1, const char* aT2) {
        size_t size = strlen(aT1) + strlen(aT2);
        char* str = (char*)BEHAVIAC_MALLOC_WITHTAG(size + 1, "CRTTIBase");
        string_cpy(str, aT1);
        string_cat(str, aT2);
        return str;
    }
    char* MakeStringName2(const char* aT1, const char* aT2, const char* aT3) {
        size_t size = strlen(aT1) + strlen(aT2) + strlen(aT3);
        char* str = (char*)BEHAVIAC_MALLOC_WITHTAG(size + 1, "CRTTIBase");
        string_cpy(str, aT1);
        string_cat(str, aT2);
        string_cat(str, aT3);
        return str;
    }
    char* MakeStringName3(const char* aT1, const char* aT2, const char* aT3, const char* aT4) {
        size_t size = strlen(aT1) + strlen(aT2) + strlen(aT3) + strlen(aT4);
        char* str = (char*)BEHAVIAC_MALLOC_WITHTAG(size + 1, "CRTTIBase");
        string_cpy(str, aT1);
        string_cat(str, aT2);
        string_cat(str, aT3);
        string_cat(str, aT4);
        return str;
    }
    char* MakeStringName4(const char* aT1, const char* aT2, const char* aT3, const char* aT4, const char* aT5) {
        size_t size = strlen(aT1) + strlen(aT2) + strlen(aT3) + strlen(aT4) + strlen(aT5);
        char* str = (char*)BEHAVIAC_MALLOC_WITHTAG(size + 1, "CRTTIBase");
        string_cpy(str, aT1);
        string_cat(str, aT2);
        string_cat(str, aT3);
        string_cat(str, aT4);
        string_cat(str, aT5);
        return str;
    }
    char* MakeStringName5(const char* aT1, const char* aT2, const char* aT3, const char* aT4, const char* aT5, const char* aT6) {
        size_t size = strlen(aT1) + strlen(aT2) + strlen(aT3) + strlen(aT4) + strlen(aT5) + strlen(aT6);
        char* str = (char*)BEHAVIAC_MALLOC_WITHTAG(size + 1, "CRTTIBase");
        string_cpy(str, aT1);
        string_cat(str, aT2);
        string_cat(str, aT3);
        string_cat(str, aT4);
        string_cat(str, aT5);
        string_cat(str, aT6);
        return str;
    }

    template<>
    void CRTTIBase::TLayerInfoDecl<1>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* parent_) {
        InternalInitClassLayerInfo(szCassTypeName, parent_);
    }

    template<>
    void CRTTIBase::TLayerInfoDecl<2>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* parent_) {
        InternalInitClassLayerInfo(szCassTypeName, parent_);
    }
    template<>
    void CRTTIBase::TLayerInfoDecl<3>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* parent_) {
        InternalInitClassLayerInfo(szCassTypeName, parent_);
    }
    template<>
    void CRTTIBase::TLayerInfoDecl<4>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* parent_) {
        InternalInitClassLayerInfo(szCassTypeName, parent_);
    }
    template<>
    void CRTTIBase::TLayerInfoDecl<5>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* parent_) {
        InternalInitClassLayerInfo(szCassTypeName, parent_);
    }
    template<>
    void CRTTIBase::TLayerInfoDecl<6>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* parent_) {
        InternalInitClassLayerInfo(szCassTypeName, parent_);
    }
    template<>
    void CRTTIBase::TLayerInfoDecl<7>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* parent_) {
        InternalInitClassLayerInfo(szCassTypeName, parent_);
    }
    template<>
    void CRTTIBase::TLayerInfoDecl<8>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* parent_) {
        InternalInitClassLayerInfo(szCassTypeName, parent_);
    }
    template<>
    void CRTTIBase::TLayerInfoDecl<9>::InitClassLayerInfo(char const* szCassTypeName, const CRTTIBase::CLayerInfoBase* parent_) {
        InternalInitClassLayerInfo(szCassTypeName, parent_);
    }
}//