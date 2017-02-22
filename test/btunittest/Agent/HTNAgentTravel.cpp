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

#include "HTNAgentTravel.h"

HTNAgentTravel::HTNAgentTravel(): city_sh(1), city_sz(2), airport_sh_hongqiao(0), airport_sh_pudong(1), airport_sz_baoan(2), sh_td(3),
    sz_td(4),
    sh_home(5),
    sz_hotel(6)
{
    _start = sh_td;
    _finish = sz_td;
}

HTNAgentTravel::~HTNAgentTravel()
{
}

