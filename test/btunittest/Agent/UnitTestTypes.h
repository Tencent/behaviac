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

#pragma once
#include "behaviac/agent/agent.h"
#include "behaviac/common/container/string.h"

namespace TNS
{
    namespace NE
    {
        namespace NAT
        {
            enum eColor
            {
                RED,
                GREEN,
                BLUE,
                YELLOW,
                WHITE,
            };
        }
    }

    namespace ST
    {
        struct kCar
        {
            behaviac::string	brand;
            int		price;
            NE::NAT::eColor	color;

            void reset()
            {
                brand = "Volkswage";
                price = 0;
                color = NE::NAT::RED;
            }

            DECLARE_BEHAVIAC_STRUCT(TNS::ST::kCar);
        };

        namespace PER
        {
            //<
            namespace WRK
            {
                struct kEmployee
                {
                    int		id;
                    behaviac::string 	name;
                    char 	code;
                    float 	weight;
                    bool 	isMale;
                    NE::NAT::eColor	skinColor;
                    kCar	car;
                    behaviac::Agent*	boss;

                    void resetProperties()
                    {
                        id = -1;
                        name = "";
                        code = 'A';
                        weight = 0.0f;
                        isMale = true;
                        skinColor = NE::NAT::YELLOW;
                        car.reset();
                        boss = NULL;
                    }

                    DECLARE_BEHAVIAC_STRUCT(TNS::ST::PER::WRK::kEmployee);
                };
            }
        }
        //<
    }
}

DECLARE_BEHAVIAC_ENUM(TNS::NE::NAT::eColor, eColor);

