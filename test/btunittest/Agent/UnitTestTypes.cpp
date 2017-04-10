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

#include "UnitTestTypes.h"
#include "behaviac/agent/registermacros.h"

//< eColor
BEHAVIAC_BEGIN_ENUM(TNS::NE::NAT::eColor, eColor)
{
    BEHAVIAC_ENUMCLASS_DISPLAY_INFO(L"色彩枚举", L"eColor 是一个用于测试枚举使用的类型");

    //BEHAVIAC_ENUM_ITEM(TNS::NE::NAT::RED, "RED").DISPLAY_INFO(L"红色 Red", L"Red 是一种非常激进的色彩");
	BEHAVIAC_ENUM_ITEM(TNS::NE::NAT::RED, "RED");
    BEHAVIAC_ENUM_ITEM(TNS::NE::NAT::GREEN, "GREEN");
    BEHAVIAC_ENUM_ITEM(TNS::NE::NAT::BLUE, "BLUE");
    BEHAVIAC_ENUM_ITEM(TNS::NE::NAT::YELLOW, "YELLOW");
    BEHAVIAC_ENUM_ITEM(TNS::NE::NAT::WHITE, "WHITE");
}
BEHAVIAC_END_ENUM()

//< Car
BEHAVIAC_BEGIN_STRUCT(TNS::ST::kCar)
{
    //BEHAVIAC_STRUCT_DISPLAYNAME(L"测试Struct")
    //BEHAVIAC_STRUCT_DESC(L"测试Struct的说明")

    BEHAVIAC_REGISTER_STRUCT_PROPERTY(brand);
    BEHAVIAC_REGISTER_STRUCT_PROPERTY(price);
    BEHAVIAC_REGISTER_STRUCT_PROPERTY(color);
}
BEHAVIAC_END_STRUCT()

//< kEmployee
BEHAVIAC_BEGIN_STRUCT(TNS::ST::PER::WRK::kEmployee)
{
    //BEHAVIAC_STRUCT_DISPLAYNAME(L"测试Struct")
    //BEHAVIAC_STRUCT_DESC(L"测试Struct的说明")

    BEHAVIAC_REGISTER_STRUCT_PROPERTY(id);
    BEHAVIAC_REGISTER_STRUCT_PROPERTY(name);
    BEHAVIAC_REGISTER_STRUCT_PROPERTY(code);
    BEHAVIAC_REGISTER_STRUCT_PROPERTY(weight);
    BEHAVIAC_REGISTER_STRUCT_PROPERTY(isMale);
    BEHAVIAC_REGISTER_STRUCT_PROPERTY(skinColor);
    BEHAVIAC_REGISTER_STRUCT_PROPERTY(car);
    BEHAVIAC_REGISTER_STRUCT_PROPERTY(boss);
}
BEHAVIAC_END_STRUCT()
