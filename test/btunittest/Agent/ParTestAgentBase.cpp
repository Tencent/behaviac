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

#include "ParTestAgentBase.h"

//< static member variable defines
eColor ParTestAgentBase::STV_ECOLOR_0 = WHITE;
bool ParTestAgentBase::STV_BOOL_0 = false;
char ParTestAgentBase::STV_CHAR_0 = L'\0';

behaviac::vector<eColor> ParTestAgentBase::STV_LIST_ECOLOR_0;
behaviac::vector<bool> ParTestAgentBase::STV_LIST_BOOL_0;
behaviac::vector<char> ParTestAgentBase::STV_LIST_CHAR_0;
behaviac::vector<signed char> ParTestAgentBase::STV_LIST_SBYTE_0;

ParTestAgentBase::ParTestAgentBase()
{
    TV_ECOLOR_0 = WHITE;
    TV_BOOL_0 = false;
    TV_CHAR_0 = L'\0';
    TV_BYTE_0 = 0;
    TV_SBYTE_0 = 0;
}

ParTestAgentBase::~ParTestAgentBase()
{
}

void ParTestAgentBase::resetProperties()
{
    TV_ECOLOR_0 = WHITE;
    STV_ECOLOR_0 = WHITE;
    TV_BOOL_0 = false;
    STV_BOOL_0 = false;
    TV_CHAR_0 = L'\0';
    STV_CHAR_0 = L'\0';
    TV_BYTE_0 = 0;
    TV_SBYTE_0 = 0;

    TV_LIST_ECOLOR_0.clear();
    STV_LIST_ECOLOR_0.clear();
    TV_LIST_BOOL_0.clear();
    STV_LIST_BOOL_0.clear();
    TV_LIST_CHAR_0.clear();
    STV_LIST_CHAR_0.clear();
    TV_LIST_BYTE_0.clear();
    TV_LIST_SBYTE_0.clear();
    STV_LIST_SBYTE_0.clear();
}

