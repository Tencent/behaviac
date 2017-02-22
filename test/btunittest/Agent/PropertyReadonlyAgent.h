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

#ifndef BTUNITEST_PROPERTYREADONLYAGENT_H_
#define BTUNITEST_PROPERTYREADONLYAGENT_H_

#include "behaviac/common/base.h"
#include "behaviac/agent/agent.h"
#include "behaviac/agent/registermacros.h"

class PropertyReadonlyAgent : public behaviac::Agent
{
public:
    PropertyReadonlyAgent();
    virtual ~PropertyReadonlyAgent();

	BEHAVIAC_DECLARE_AGENTTYPE(PropertyReadonlyAgent, behaviac::Agent);

    PropertyReadonlyAgent& operator=(PropertyReadonlyAgent){ return *this; }

    void resetProperties()
    {
        _int_member = 0;
        _int_property_getteronly = 1;
        PropertyGetterOnly = 1;

        MemberReadonlyAs = 3;

        _s_float_member = 0.0f;
    }

    void init()
    {
        //base.Init();
        resetProperties();
    }

    void finl()
    {
    }

    //=====================================================
    int _int_member;

    const int& PropertyGetterSetter_get() const
    {
        return _int_member;
    }
    void PropertyGetterSetter_set(const int& value)
    {
        _int_member = value;
    }

    //=====================================================
    static	float _s_float_member;
    static float StaticPropertyGetterSetter;
    int PropertyGetterOnly;
    int PropertyGetterSetter;

    //=====================================================
    int _int_property_getteronly;

    //=====================================================
    const int MemberReadonly;

    //=====================================================
    int MemberReadonlyAs;

    //=====================================================

    void FnWithOutParam(int& param)
    {
        param = 4;
    }

    void PassInProperty(int param)
    {
        MemberReadonlyAs = param;
    }
};

#endif//BTUNITEST_PROPERTYREADONLYAGENT_H_
