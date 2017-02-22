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

#include "../btloadtestsuite.h"
#include "behaviac/common/profiler/profiler.h"

#include "../Agent/CustomPropertyAgent.h"
#include "../Agent/FSMAgentTest.h"

PropertyReadonlyAgent* initTestEnvProperty(const char* treePath, behaviac::Workspace::EFileFormat format)
{
    behaviac::Profiler::CreateInstance();
    behaviac::Config::SetSocketing(false);
    behaviac::Config::SetLogging(false);


    registerAllTypes();
    PropertyReadonlyAgent* testAgent = PropertyReadonlyAgent::DynamicCast(behaviac::Agent::Create<PropertyReadonlyAgent>());
    behaviac::Agent::SetIdMask(1);
    testAgent->SetIdFlag(1);

    testAgent->btload(treePath);
    testAgent->btsetcurrent(treePath);
    return testAgent;
}

void finlTestEnvProperty(PropertyReadonlyAgent* testAgent)
{
    BEHAVIAC_DELETE(testAgent);
    unregisterAllTypes();

    behaviac::Profiler::DestroyInstance();
}

//< par_as_ref_param
LOAD_TEST(btunittest, readonly_default)
{
    PropertyReadonlyAgent* testAgent = initTestEnvProperty("par_test/readonly_default", format);
    testAgent->resetProperties();
    //testAgent->btexec();

    // base class 0 test
    int PropertyGetterOnly = testAgent->GetVariable<int>("PropertyGetterOnly");
    CHECK_EQUAL(1, PropertyGetterOnly);
    CHECK_EQUAL(2, testAgent->MemberReadonly);
    CHECK_EQUAL(3, testAgent->MemberReadonlyAs);

    testAgent->btexec();

    int c_IntReadonly = testAgent->GetVariable<int>("c_IntReadonly");
    CHECK_EQUAL(10, c_IntReadonly);

    int PropertyGetterSetter = testAgent->GetVariable<int>("PropertyGetterSetter");
    // PropertyGetterSetter = MemberReadonly, while MemberReadonly = 2
    CHECK_EQUAL(2, PropertyGetterSetter);

    PropertyGetterOnly = testAgent->GetVariable<int>("PropertyGetterOnly");
    // PropertyGetterOnly is passed in as the param of PassInProperty
    CHECK_EQUAL(1, PropertyGetterOnly);

    // MemberReadonly is readonly, not changed
    CHECK_EQUAL(2, testAgent->MemberReadonly);

    // MemberReadonlyAs is modified in PassInProperty and assigned to be PropertyGetterOnly
    CHECK_EQUAL(1, testAgent->MemberReadonlyAs);

    // m_Int is as the ref param of FnWithOutParam and set to 4
    int c_Int = testAgent->GetVariable<int>("c_Int");
    CHECK_EQUAL(4, c_Int);

    // c_ResultStatic = MemberReadonly + PropertyGetterOnly, 2 + 1 = 3
    int c_ResultStatic = testAgent->GetVariable<int>("c_ResultStatic");
    //BEHAVIAC_ASSERT(false);
    CHECK_EQUAL(3, c_ResultStatic);

    testAgent->SetVariable("StaticPropertyGetterSetter", 1.0f);
    float StaticPropertyGetterSetter = testAgent->GetVariable<float>("StaticPropertyGetterSetter");
    CHECK_FLOAT_EQUAL(1.0f, StaticPropertyGetterSetter);

    finlTestEnvProperty(testAgent);
}
