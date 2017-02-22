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

void registerAllTypes()
{
	//behaviac::TypeRegister::Register<UnityEngine::Vector3>("UnityEngine::Vector3");
	//behaviac::TypeRegister::Register<FSMAgentTest::EMessage>("FSMAgentTest::EMessage");

	////< new types
	//behaviac::Agent::Register<AgentNodeTest>();
	//behaviac::Agent::Register<ChildNodeTest>();
	//behaviac::Agent::Register<ChildNodeTestSub>();

	//behaviac::TypeRegister::Register<TNS::ST::PER::WRK::kEmployee>("TNS::ST::PER::WRK::kEmployee");
	//behaviac::TypeRegister::Register<TNS::ST::kCar>("TNS::ST::kCar");
	//behaviac::TypeRegister::Register<TNS::NE::NAT::eColor>("TNS::NE::NAT::eColor");
	//behaviac::TypeRegister::Register<UnityEngine::GameObject>("UnityEngine::GameObject");
	//behaviac::TypeRegister::Register<TestNS::Node>("TestNS::Node");
	//behaviac::TypeRegister::Register<TestNS::Float2>("TestNS::Float2");
	//behaviac::TypeRegister::Register<EnumTest>("EnumTest");
	//behaviac::TypeRegister::Register<Act>("Act");
	//behaviac::TypeRegister::Register<BSASN::SpatialCoord>("BSASN::SpatialCoord");
	//behaviac::TypeRegister::Register<BSASN::TransitPlan>("BSASN::TransitPlan");
	//behaviac::TypeRegister::Register<TestNamespace::ClassAsValueType>("TestNamespace::ClassAsValueType");
	//behaviac::TypeRegister::Register<TestNamespace::Float2>("TestNamespace::Float2");

	//behaviac::Agent::Register<EmployeeParTestAgent>();

	//behaviac::Agent::Register<StaticAgent>();
	////behaviac::Agent::Register<ParTestAgentBase>();
	////behaviac::Agent::Register<ParTestAgent>();
	//behaviac::Agent::Register<ParTestRegNameAgent>();
	//behaviac::Agent::Register<HTNAgentHouse>();
	//behaviac::Agent::Register<FSMAgentTest>();
	//behaviac::Agent::Register<HTNAgentTravel>();
	//behaviac::Agent::Register<CustomPropertyAgent>();
	//behaviac::Agent::Register<TestNS::AgentArrayAccessTest>();
	//behaviac::Agent::Register<PropertyReadonlyAgent>();
	//behaviac::Agent::Register<PreconEffectorAgent>();
	//behaviac::Agent::RegisterInstanceName<StaticAgent>("StaticAgent");
	//behaviac::Agent::RegisterInstanceName<ParTestRegNameAgent>("ParTestRegNameAgent");
}

void unregisterAllTypes()
{
	//behaviac::Agent::UnRegisterInstanceName<StaticAgent>("StaticAgent");
	//behaviac::Agent::UnRegisterInstanceName<ParTestRegNameAgent>("ParTestRegNameAgent");
	//behaviac::Agent::UnRegister<StaticAgent>();
	//behaviac::Agent::UnRegister<ParTestRegNameAgent>();
	//behaviac::Agent::UnRegister<EmployeeParTestAgent>();
	////behaviac::Agent::UnRegister<ParTestAgentBase>();
	////behaviac::Agent::UnRegister<ParTestAgent>();

	//behaviac::Agent::UnRegister<HTNAgentHouse>();
	//behaviac::Agent::UnRegister<FSMAgentTest>();
	//behaviac::Agent::UnRegister<HTNAgentTravel>();
	//behaviac::Agent::UnRegister<CustomPropertyAgent>();
	//behaviac::Agent::UnRegister<TestNS::AgentArrayAccessTest>();
	//behaviac::Agent::UnRegister<PropertyReadonlyAgent>();
	//behaviac::Agent::UnRegister<PreconEffectorAgent>();

	//behaviac::TypeRegister::UnRegister<TestNS::Node>("TestNS::Node");
	//behaviac::TypeRegister::UnRegister<TestNS::Float2>("TestNS::Float2");

	//behaviac::TypeRegister::UnRegister<UnityEngine::GameObject>("UnityEngine::GameObject");
	//behaviac::TypeRegister::UnRegister<TNS::NE::NAT::eColor>("TNS::NE::NAT::eColor");
	//behaviac::TypeRegister::UnRegister<TNS::ST::PER::WRK::kEmployee>("TNS::ST::PER::WRK::kEmployee");

	//behaviac::Agent::UnRegister<ChildNodeTestSub>();
	//behaviac::Agent::UnRegister<ChildNodeTest>();
	//behaviac::Agent::UnRegister<AgentNodeTest>();

	//behaviac::TypeRegister::UnRegister<UnityEngine::Vector3>("UnityEngine::Vector3");
	//behaviac::TypeRegister::UnRegister<FSMAgentTest::EMessage>("FSMAgentTest::EMessage");
	//behaviac::TypeRegister::UnRegister<EnumTest>("EnumTest");
	//behaviac::TypeRegister::UnRegister<Act>("Act");
	//behaviac::TypeRegister::UnRegister<BSASN::SpatialCoord>("BSASN::SpatialCoord");
	//behaviac::TypeRegister::UnRegister<BSASN::TransitPlan>("BSASN::TransitPlan");
	//behaviac::TypeRegister::UnRegister<TestNamespace::ClassAsValueType>("TestNamespace::ClassAsValueType");
	//behaviac::TypeRegister::UnRegister<TestNamespace::Float2>("TestNamespace::Float2");

    behaviac::Workspace::GetInstance()->Cleanup();
}

AgentNodeTest* initTestEnvNode(const char* treePath, behaviac::Workspace::EFileFormat format)
{
    behaviac::Profiler::CreateInstance();
    behaviac::Config::SetSocketing(false);
    behaviac::Config::SetLogging(false);

    registerAllTypes();
    AgentNodeTest* testAgent = AgentNodeTest::DynamicCast(behaviac::Agent::Create<AgentNodeTest>());
    behaviac::Agent::SetIdMask(1);
    testAgent->SetIdFlag(1);

    testAgent->btload(treePath);
    testAgent->btsetcurrent(treePath);
    return testAgent;
}

void finlTestEnvNode(AgentNodeTest* testAgent)
{
    //BEHAVIAC_DELETE(testAgent);
	behaviac::Agent::Destroy(testAgent);

    unregisterAllTypes();

    behaviac::Profiler::DestroyInstance();
}

EmployeeParTestAgent* initTestEnvPar(const char* treePath, behaviac::Workspace::EFileFormat format)
{
    EmployeeParTestAgent::clearAllStaticMemberVariables();

    behaviac::Profiler::CreateInstance();
    behaviac::Config::SetSocketing(false);
    behaviac::Config::SetLogging(false);

    registerAllTypes();
    EmployeeParTestAgent* testAgent = EmployeeParTestAgent::DynamicCast(behaviac::Agent::Create<EmployeeParTestAgent>());
    behaviac::Agent::SetIdMask(1);
    testAgent->SetIdFlag(1);
    testAgent->btload(treePath);
    testAgent->btsetcurrent(treePath);
    return testAgent;
}

void finlTestEnvPar(EmployeeParTestAgent* testAgent)
{
    //BEHAVIAC_DELETE(testAgent);
	behaviac::Agent::Destroy(testAgent);
    unregisterAllTypes();

    behaviac::Profiler::DestroyInstance();
}

//< par_as_ref_param
LOAD_TEST(btunittest, par_as_ref_param)
{
    EmployeeParTestAgent* myTestAgent = initTestEnvPar("par_test/par_as_ref_param", format);
    myTestAgent->resetProperties();
    myTestAgent->btexec();

    // base class 0 test
    eColor ecolor_0 = myTestAgent->GetVariable<eColor>("par0_ecolor_0");
    CHECK_EQUAL(BLUE, ecolor_0);

    bool boolean_0 = myTestAgent->GetVariable<bool>("par0_boolean_0");
    CHECK_EQUAL(true, boolean_0);

    char char_0 = myTestAgent->GetVariable<char>("par0_char_0");
    CHECK_EQUAL('X', char_0);

    unsigned char byte_0 = myTestAgent->GetVariable<unsigned char>("par0_byte_0");
    CHECK_EQUAL(2, byte_0);

    const behaviac::vector<unsigned char>& byte_list_0 = myTestAgent->GetVariable<behaviac::vector<unsigned char> >("par0_byte_list_0");
    CHECK_EQUAL(1, byte_list_0.size());
    CHECK_EQUAL(8, byte_list_0[0]);

    signed char sbyte_0 = myTestAgent->GetVariable<signed char>("par0_sbyte_0");
    CHECK_EQUAL(-2, sbyte_0);

    const behaviac::vector<eColor>& ecolor_list_0 = myTestAgent->GetVariable<behaviac::vector<eColor> >("par0_ecolor_list_0");
    CHECK_EQUAL(1, ecolor_list_0.size());
    CHECK_EQUAL(RED, ecolor_list_0[0]);

    const behaviac::vector<bool>& boolean_list_0 = myTestAgent->GetVariable<behaviac::vector<bool> >("par0_boolean_list_0");
    CHECK_EQUAL(1, boolean_list_0.size());
    CHECK_EQUAL(true, boolean_list_0[0]);

    const behaviac::vector<char>& char_list_0 = myTestAgent->GetVariable<behaviac::vector<char> >("par0_char_list_0");
    CHECK_EQUAL(1, char_list_0.size());
    CHECK_EQUAL('k', char_list_0[0]);

    const behaviac::vector<signed char>& sbyte_list_0 = myTestAgent->GetVariable<behaviac::vector<signed char> >("par0_sbyte_list_0");
    CHECK_EQUAL(1, sbyte_list_0.size());
    CHECK_EQUAL(-4, sbyte_list_0[0]);

    // base class 1 test
    signed short short_0 = myTestAgent->GetVariable<signed short>("par1_short_0");
    CHECK_EQUAL(1, short_0);

    signed int int_0 = myTestAgent->GetVariable<signed int>("par1_int_0");
    CHECK_EQUAL(2, int_0);

    signed long long_0 = myTestAgent->GetVariable<signed long>("par1_long_0");
    CHECK_EQUAL(3L, long_0);

    unsigned short ushort_0 = myTestAgent->GetVariable<unsigned short>("par1_ushort_0");
    CHECK_EQUAL(4, ushort_0);

    kEmployee kemployee_0 = myTestAgent->GetVariable<kEmployee>("par1_kemployee_0");
    CHECK_EQUAL(3, kemployee_0.id);
    CHECK_STR_EQUAL("Tom", kemployee_0.name.c_str());
    CHECK_EQUAL('X', kemployee_0.code);
    CHECK_FLOAT_EQUAL(58.7f, kemployee_0.weight);
    CHECK_EQUAL(true, kemployee_0.isMale);
    CHECK_EQUAL(GREEN, kemployee_0.skinColor);
    CHECK_EQUAL(myTestAgent, kemployee_0.boss);
    CHECK_STR_EQUAL("Honda", kemployee_0.car.brand.c_str());
    CHECK_EQUAL(RED, kemployee_0.car.color);
    CHECK_EQUAL(23000, kemployee_0.car.price);

    const behaviac::vector<signed int>& int_list_0 = myTestAgent->GetVariable<behaviac::vector<signed int> >("par1_int_list_0");
    CHECK_EQUAL(1, int_list_0.size());
    CHECK_EQUAL(5, int_list_0[0]);

    const behaviac::vector<kEmployee>& kemployee_list_0 = myTestAgent->GetVariable<behaviac::vector<kEmployee> >("par1_kemployee_list_0");
    CHECK_EQUAL(1, kemployee_list_0.size());
    CHECK_EQUAL(3, kemployee_list_0[0].id);
    CHECK_STR_EQUAL("Tom", kemployee_list_0[0].name.c_str());
    CHECK_EQUAL('X', kemployee_list_0[0].code);
    CHECK_FLOAT_EQUAL(58.7f, kemployee_list_0[0].weight);
    CHECK_EQUAL(true, kemployee_list_0[0].isMale);
    CHECK_EQUAL(GREEN, kemployee_list_0[0].skinColor);
    CHECK_EQUAL(myTestAgent, kemployee_list_0[0].boss);
    CHECK_STR_EQUAL("Honda", kemployee_list_0[0].car.brand.c_str());
    CHECK_EQUAL(RED, kemployee_list_0[0].car.color);
    CHECK_EQUAL(23000, kemployee_list_0[0].car.price);

    // base class 2 test
    unsigned int uint_0 = myTestAgent->GetVariable<unsigned int>("par2_uint_0");
    CHECK_EQUAL(1, uint_0);

    unsigned long ulong_0 = myTestAgent->GetVariable<unsigned long>("par2_ulong_0");
    CHECK_EQUAL(2, ulong_0);

    float single_0 = myTestAgent->GetVariable<float>("par2_single_0");
    CHECK_FLOAT_EQUAL(3.0f, single_0);

    double double_0 = myTestAgent->GetVariable<double>("par2_double_0");
    CHECK_FLOAT_EQUAL(4.0, double_0);

    //for the unity version reason, par2_longlong_0 is in fact as long instead of long long
    long long ll_0 = myTestAgent->GetVariable<long long>("par2_longlong_0");
    CHECK_EQUAL(-866L, ll_0);

    unsigned long long ull_0 = myTestAgent->GetVariable<unsigned long long>("par2_ulonglong_0");
    CHECK_EQUAL(866L, ull_0);

    behaviac::string string_0 = myTestAgent->GetVariable<behaviac::string>("par2_string_0");
    CHECK_STR_EQUAL("This is a behaviac::string ref in test!", string_0.c_str());

    CHECK_STR_EQUAL("this is const char* test", myTestAgent->TV_CSzString.c_str());
    CHECK_STR_EQUAL("this is char* test", myTestAgent->TV_SzString_0.c_str());

    behaviac::Agent* agent_0 = myTestAgent->GetVariable<behaviac::Agent*>("par2_agent_0");
    CHECK_EQUAL(myTestAgent, agent_0);

    const behaviac::vector<float>& single_list_0 = myTestAgent->GetVariable<behaviac::vector<float> >("par2_single_list_0");
    CHECK_EQUAL(1, single_list_0.size());
    CHECK_FLOAT_EQUAL(1.0f, single_list_0[0]);

    const behaviac::vector<behaviac::string>& string_list_0 = myTestAgent->GetVariable<behaviac::vector<behaviac::string> >("par2_string_list_0");
    CHECK_EQUAL(1, string_list_0.size());
    CHECK_STR_EQUAL("This is a behaviac::string ref in test!", string_list_0[0].c_str());

    const behaviac::vector<behaviac::Agent*>& agent_list_0 = myTestAgent->GetVariable<behaviac::vector<behaviac::Agent*> >("par2_agent_list_0");
    CHECK_EQUAL(1, agent_list_0.size());
    CHECK_EQUAL(myTestAgent, agent_list_0[0]);


    //const char * and char* test
    //CHECK_STR_EQUAL("this is const char* test",myTestAgent->TV_CSzString.c_str());
    //CHECK_STR_EQUAL("this is char* test", myTestAgent->TV_SzString_0.c_str());
    finlTestEnvPar(myTestAgent);
}

//< par_test_param_in_return
LOAD_TEST(btunittest, par_as_left_value_and_param)
{
    EmployeeParTestAgent* myTestAgent = initTestEnvPar("par_test/par_as_left_value_and_param", format);
    myTestAgent->resetProperties();
    myTestAgent->btexec();

    // base class 0 test
    eColor ecolor_0 = myTestAgent->GetVariable<eColor>("par0_ecolor_0");
    CHECK_EQUAL(RED, ecolor_0);

    bool boolean_0 = myTestAgent->GetVariable<bool>("par0_boolean_0");
    CHECK_EQUAL(false, boolean_0);

    char char_0 = myTestAgent->GetVariable<char>("par0_char_0");
    CHECK_EQUAL('C', char_0);

    unsigned char byte_0 = myTestAgent->GetVariable<unsigned char>("par0_byte_0");
    CHECK_EQUAL(209, byte_0);

    const behaviac::vector<unsigned char>& byte_list_0 = myTestAgent->GetVariable<behaviac::vector<unsigned char> >("par0_byte_list_0");
    CHECK_EQUAL(4, byte_list_0.size());
    CHECK_EQUAL(167, byte_list_0[0]);
    CHECK_EQUAL(23, byte_list_0[1]);
    CHECK_EQUAL(152, byte_list_0[2]);
    CHECK_EQUAL(126, byte_list_0[3]);

    signed char sbyte_0 = myTestAgent->GetVariable<signed char>("par0_sbyte_0");
    CHECK_EQUAL(-65, sbyte_0);

    const behaviac::vector<eColor>& ecolor_list_0 = myTestAgent->GetVariable<behaviac::vector<eColor> >("par0_ecolor_list_0");
    CHECK_EQUAL(3, ecolor_list_0.size());
    CHECK_EQUAL(RED, ecolor_list_0[0]);
    CHECK_EQUAL(GREEN, ecolor_list_0[1]);
    CHECK_EQUAL(YELLOW, ecolor_list_0[2]);

    const behaviac::vector<bool>& boolean_list_0 = myTestAgent->GetVariable<behaviac::vector<bool> >("par0_boolean_list_0");
    CHECK_EQUAL(3, boolean_list_0.size());
    CHECK_EQUAL(false, boolean_list_0[0]);
    CHECK_EQUAL(true, boolean_list_0[1]);
    CHECK_EQUAL(false, boolean_list_0[2]);

    const behaviac::vector<char>& char_list_0 = myTestAgent->GetVariable<behaviac::vector<char> >("par0_char_list_0");
    CHECK_EQUAL(5, char_list_0.size());
    CHECK_EQUAL('d', char_list_0[0]);
    CHECK_EQUAL('j', char_list_0[1]);
    CHECK_EQUAL('F', char_list_0[2]);
    CHECK_EQUAL('A', char_list_0[3]);
    CHECK_EQUAL('m', char_list_0[4]);

    const behaviac::vector<signed char>& sbyte_list_0 = myTestAgent->GetVariable<behaviac::vector<signed char> >("par0_sbyte_list_0");
    CHECK_EQUAL(4, sbyte_list_0.size());
    CHECK_EQUAL(127, sbyte_list_0[0]);
    CHECK_EQUAL(-128, sbyte_list_0[1]);
    CHECK_EQUAL(0, sbyte_list_0[2]);
    CHECK_EQUAL(-126, sbyte_list_0[3]);

    // base class 1 test
    signed short short_0 = myTestAgent->GetVariable<signed short>("par1_short_0");
    CHECK_EQUAL(328, short_0);

    signed int int_0 = myTestAgent->GetVariable<signed int>("par1_int_0");
    CHECK_EQUAL(347, int_0);

    signed long long_0 = myTestAgent->GetVariable<signed long>("par1_long_0");
    CHECK_EQUAL(1950L, long_0);

    unsigned short ushort_0 = myTestAgent->GetVariable<unsigned short>("par1_ushort_0");
    CHECK_EQUAL(2551, ushort_0);

    kEmployee kemployee_0 = myTestAgent->GetVariable<kEmployee>("par1_kemployee_0");
    CHECK_EQUAL(86, kemployee_0.id);
    CHECK_STR_EQUAL("TomJerry", kemployee_0.name.c_str());
    CHECK_EQUAL('V', kemployee_0.code);
    CHECK_FLOAT_EQUAL(117.5f, kemployee_0.weight);
    CHECK_EQUAL(true, kemployee_0.isMale);
    CHECK_EQUAL(BLUE, kemployee_0.skinColor);
    CHECK_EQUAL(myTestAgent, kemployee_0.boss);
    CHECK_STR_EQUAL("AlphaJapan", kemployee_0.car.brand.c_str());
    CHECK_EQUAL(YELLOW, kemployee_0.car.color);
    CHECK_EQUAL(8700, kemployee_0.car.price);

    const behaviac::vector<signed int>& int_list_0 = myTestAgent->GetVariable<behaviac::vector<signed int> >("par1_int_list_0");
    CHECK_EQUAL(4, int_list_0.size());
    CHECK_EQUAL(9999, int_list_0[0]);
    CHECK_EQUAL(12345, int_list_0[1]);
    CHECK_EQUAL(0, int_list_0[2]);
    CHECK_EQUAL(235, int_list_0[3]);

    const behaviac::vector<kEmployee>& kemployee_list_0 = myTestAgent->GetVariable<behaviac::vector<kEmployee> >("par1_kemployee_list_0");
    CHECK_EQUAL(2, kemployee_list_0.size());

    const kEmployee& employ_0 = kemployee_list_0[0];
    CHECK_EQUAL(9, employ_0.id);
    CHECK_STR_EQUAL("John", employ_0.name.c_str());
    CHECK_EQUAL('q', employ_0.code);
    CHECK_FLOAT_EQUAL(110.0f, employ_0.weight);
    CHECK_EQUAL(true, employ_0.isMale);
    CHECK_EQUAL(GREEN, employ_0.skinColor);
    CHECK_EQUAL(NULL, employ_0.boss);
    CHECK_STR_EQUAL("Lexus", employ_0.car.brand.c_str());
    CHECK_EQUAL(BLUE, employ_0.car.color);
    CHECK_EQUAL(93840, employ_0.car.price);

    const kEmployee& employ_1 = kemployee_list_0[1];
    CHECK_EQUAL(4, employ_1.id);
    CHECK_STR_EQUAL("Jerry", employ_1.name.c_str());
    CHECK_EQUAL('J', employ_1.code);
    CHECK_FLOAT_EQUAL(60.0f, employ_1.weight);
    CHECK_EQUAL(false, employ_1.isMale);
    CHECK_EQUAL(WHITE, employ_1.skinColor);
    CHECK_EQUAL(NULL, employ_1.boss);
    CHECK_STR_EQUAL("Toyota", employ_1.car.brand.c_str());
    CHECK_EQUAL(YELLOW, employ_1.car.color);
    CHECK_EQUAL(43000, employ_1.car.price);

    // base class 2 test
    unsigned int uint_0 = myTestAgent->GetVariable<unsigned int>("par2_uint_0");
    CHECK_EQUAL(63, uint_0);

    unsigned long ulong_0 = myTestAgent->GetVariable<unsigned long>("par2_ulong_0");
    CHECK_EQUAL(389, ulong_0);

    float single_0 = myTestAgent->GetVariable<float>("par2_single_0");
    CHECK_FLOAT_EQUAL(170.5f, single_0);

    double double_0 = myTestAgent->GetVariable<double>("par2_double_0");
    CHECK_FLOAT_EQUAL(48.6, double_0);

    //for the unity version reason, par2_longlong_0 is in fact as long instead of long long
    long long ll_0 = myTestAgent->GetVariable<long long>("par2_longlong_0");
    CHECK_EQUAL(-866L, ll_0);

    unsigned long long ull_0 = myTestAgent->GetVariable<unsigned long long>("par2_ulonglong_0");
    CHECK_EQUAL(866L, ull_0);

    behaviac::string string_0 = myTestAgent->GetVariable<behaviac::string>("par2_string_0");
    CHECK_STR_EQUAL("originextra", string_0.c_str());


    behaviac::Agent* agent_0 = myTestAgent->GetVariable<behaviac::Agent*>("par2_agent_0");
    CHECK_EQUAL(myTestAgent, agent_0);

    ParTestAgentBase* parTestAgentBase_0 = myTestAgent->GetVariable<ParTestAgentBase*>("par2_ParTestAgentBase_0");
    CHECK_EQUAL(myTestAgent, parTestAgentBase_0);

    const behaviac::vector<float>& single_list_0 = myTestAgent->GetVariable<behaviac::vector<float> >("par2_single_list_0");
    CHECK_EQUAL(3, single_list_0.size());
    CHECK_FLOAT_EQUAL(5.1f, single_list_0[0]);
    CHECK_FLOAT_EQUAL(6.2f, single_list_0[1]);
    CHECK_FLOAT_EQUAL(93.7f, single_list_0[2]);

    const behaviac::vector<behaviac::string>& string_list_0 = myTestAgent->GetVariable<behaviac::vector<behaviac::string> >("par2_string_list_0");
    CHECK_EQUAL(5, string_list_0.size());
    CHECK_STR_EQUAL("string0", string_list_0[0].c_str());
    CHECK_STR_EQUAL("string1", string_list_0[1].c_str());
    CHECK_STR_EQUAL("string2", string_list_0[2].c_str());
    CHECK_STR_EQUAL("string3", string_list_0[3].c_str());
    CHECK_STR_EQUAL("extra", string_list_0[4].c_str());

    const behaviac::vector<behaviac::Agent*>& agent_list_0 = myTestAgent->GetVariable<behaviac::vector<behaviac::Agent*> >("par2_agent_list_0");
    CHECK_EQUAL(3, agent_list_0.size());
    CHECK_EQUAL(NULL, agent_list_0[0]);
    CHECK_EQUAL(NULL, agent_list_0[1]);
    CHECK_EQUAL(myTestAgent, agent_list_0[2]);

    const behaviac::vector<ParTestAgentBase*>& parTestAgentBase_list_0 = myTestAgent->GetVariable<behaviac::vector<ParTestAgentBase*> >("par2_ParTestAgentBaseList_0");
    CHECK_EQUAL(3, parTestAgentBase_list_0.size());
    CHECK_EQUAL(NULL, parTestAgentBase_list_0[0]);
    CHECK_EQUAL(NULL, parTestAgentBase_list_0[1]);
    CHECK_EQUAL(myTestAgent, parTestAgentBase_list_0[2]);

    finlTestEnvPar(myTestAgent);
}

//< property_as_left_value
LOAD_TEST(btunittest, property_as_left_value)
{
    EmployeeParTestAgent* myTestAgent = initTestEnvPar("par_test/property_as_left_value", format);
    myTestAgent->resetProperties();
    myTestAgent->btexec();

    // base class 0 test
    CHECK_EQUAL(RED, myTestAgent->TV_ECOLOR_0);
    CHECK_EQUAL(false, myTestAgent->TV_BOOL_0);
    CHECK_EQUAL('C', myTestAgent->TV_CHAR_0);
    CHECK_EQUAL(209, myTestAgent->TV_BYTE_0);

    const behaviac::vector<unsigned char>& byte_list_0 = myTestAgent->TV_LIST_BYTE_0;
    CHECK_EQUAL(4, byte_list_0.size());
    CHECK_EQUAL(167, byte_list_0[0]);
    CHECK_EQUAL(23, byte_list_0[1]);
    CHECK_EQUAL(152, byte_list_0[2]);
    CHECK_EQUAL(126, byte_list_0[3]);
    CHECK_EQUAL(-65, myTestAgent->TV_SBYTE_0);

    CHECK_EQUAL(3, myTestAgent->TV_LIST_ECOLOR_0.size());
    CHECK_EQUAL(RED, myTestAgent->TV_LIST_ECOLOR_0[0]);
    CHECK_EQUAL(GREEN, myTestAgent->TV_LIST_ECOLOR_0[1]);
    CHECK_EQUAL(YELLOW, myTestAgent->TV_LIST_ECOLOR_0[2]);

    CHECK_EQUAL(3, myTestAgent->TV_LIST_BOOL_0.size());
    CHECK_EQUAL(false, myTestAgent->TV_LIST_BOOL_0[0]);
    CHECK_EQUAL(true, myTestAgent->TV_LIST_BOOL_0[1]);
    CHECK_EQUAL(false, myTestAgent->TV_LIST_BOOL_0[2]);

    CHECK_EQUAL(5, myTestAgent->TV_LIST_CHAR_0.size());
    CHECK_EQUAL('d', myTestAgent->TV_LIST_CHAR_0[0]);
    CHECK_EQUAL('j', myTestAgent->TV_LIST_CHAR_0[1]);
    CHECK_EQUAL('F', myTestAgent->TV_LIST_CHAR_0[2]);
    CHECK_EQUAL('A', myTestAgent->TV_LIST_CHAR_0[3]);
    CHECK_EQUAL('m', myTestAgent->TV_LIST_CHAR_0[4]);

    const behaviac::vector<signed char>& sbyte_list_0 = myTestAgent->TV_LIST_SBYTE_0;
    CHECK_EQUAL(4, sbyte_list_0.size());
    CHECK_EQUAL(127, sbyte_list_0[0]);
    CHECK_EQUAL(-128, sbyte_list_0[1]);
    CHECK_EQUAL(0, sbyte_list_0[2]);
    CHECK_EQUAL(-126, sbyte_list_0[3]);

    // base class 1 test
    CHECK_EQUAL(328, myTestAgent->TV_SHORT_0);
    CHECK_EQUAL(347, myTestAgent->TV_INT_0);
    CHECK_EQUAL(1950L, myTestAgent->TV_LONG_0);
    CHECK_EQUAL(2551, myTestAgent->TV_USHORT_0);

    CHECK_EQUAL(-666L, myTestAgent->TV_LL_0);
    CHECK_EQUAL(666L, myTestAgent->TV_ULL_0);

    kEmployee& kemployee_0 = myTestAgent->TV_KEMPLOYEE_0;
    CHECK_EQUAL(86, kemployee_0.id);
    CHECK_STR_EQUAL("TomJerry", kemployee_0.name.c_str());
    CHECK_EQUAL('V', kemployee_0.code);
    CHECK_FLOAT_EQUAL(117.5f, kemployee_0.weight);
    CHECK_EQUAL(true, kemployee_0.isMale);
    CHECK_EQUAL(BLUE, kemployee_0.skinColor);
    CHECK_EQUAL(myTestAgent, kemployee_0.boss);
    CHECK_STR_EQUAL("AlphaJapan", kemployee_0.car.brand.c_str());
    CHECK_EQUAL(YELLOW, kemployee_0.car.color);
    CHECK_EQUAL(8700, kemployee_0.car.price);

    CHECK_EQUAL(4, myTestAgent->TV_LIST_INT_0.size());
    CHECK_EQUAL(9999, myTestAgent->TV_LIST_INT_0[0]);
    CHECK_EQUAL(12345, myTestAgent->TV_LIST_INT_0[1]);
    CHECK_EQUAL(0, myTestAgent->TV_LIST_INT_0[2]);
    CHECK_EQUAL(235, myTestAgent->TV_LIST_INT_0[3]);

    const behaviac::vector<kEmployee>& kemployee_list_0 = myTestAgent->TV_LIST_KEMPLOYEE_0;
    CHECK_EQUAL(2, kemployee_list_0.size());

    CHECK_EQUAL(9, kemployee_list_0[0].id);
    CHECK_STR_EQUAL("John", kemployee_list_0[0].name.c_str());
    CHECK_EQUAL('q', kemployee_list_0[0].code);
    CHECK_FLOAT_EQUAL(110.0f, kemployee_list_0[0].weight);
    CHECK_EQUAL(true, kemployee_list_0[0].isMale);
    CHECK_EQUAL(GREEN, kemployee_list_0[0].skinColor);
    CHECK_EQUAL(NULL, kemployee_list_0[0].boss);
    CHECK_STR_EQUAL("Lexus", kemployee_list_0[0].car.brand.c_str());
    CHECK_EQUAL(BLUE, kemployee_list_0[0].car.color);
    CHECK_EQUAL(93840, kemployee_list_0[0].car.price);

    CHECK_EQUAL(4, kemployee_list_0[1].id);
    CHECK_STR_EQUAL("Jerry", kemployee_list_0[1].name.c_str());
    CHECK_EQUAL('J', kemployee_list_0[1].code);
    CHECK_FLOAT_EQUAL(60.0f, kemployee_list_0[1].weight);
    CHECK_EQUAL(false, kemployee_list_0[1].isMale);
    CHECK_EQUAL(WHITE, kemployee_list_0[1].skinColor);
    CHECK_EQUAL(NULL, kemployee_list_0[1].boss);
    CHECK_STR_EQUAL("Toyota", kemployee_list_0[1].car.brand.c_str());
    CHECK_EQUAL(YELLOW, kemployee_list_0[1].car.color);
    CHECK_EQUAL(43000, kemployee_list_0[1].car.price);

    // base class 2 test
    CHECK_EQUAL(63, myTestAgent->TV_UINT_0);
    CHECK_EQUAL(389, myTestAgent->TV_ULONG_0);
    CHECK_FLOAT_EQUAL(170.5f, myTestAgent->TV_F_0);
    CHECK_FLOAT_EQUAL(48.6, myTestAgent->TV_D_0);
    CHECK_STR_EQUAL("originextra", myTestAgent->TV_STR_0.c_str());
    CHECK_EQUAL(myTestAgent, myTestAgent->TV_AGENT_0);

    const behaviac::vector<float>& single_list_0 = myTestAgent->TV_LIST_F_0;
    CHECK_EQUAL(3, single_list_0.size());
    CHECK_FLOAT_EQUAL(5.1f, single_list_0[0]);
    CHECK_FLOAT_EQUAL(6.2f, single_list_0[1]);
    CHECK_FLOAT_EQUAL(93.7f, single_list_0[2]);

    const behaviac::vector<behaviac::string>& string_list_0 = myTestAgent->TV_LIST_STR_0;
    CHECK_EQUAL(5, string_list_0.size());
    CHECK_STR_EQUAL("string0", string_list_0[0].c_str());
    CHECK_STR_EQUAL("string1", string_list_0[1].c_str());
    CHECK_STR_EQUAL("string2", string_list_0[2].c_str());
    CHECK_STR_EQUAL("string3", string_list_0[3].c_str());
    CHECK_STR_EQUAL("extra", string_list_0[4].c_str());

    const behaviac::vector<behaviac::Agent*>& agent_list_0 = myTestAgent->TV_LIST_AGENT_0;
    CHECK_EQUAL(3, agent_list_0.size());
    CHECK_EQUAL(NULL, agent_list_0[0]);
    CHECK_EQUAL(NULL, agent_list_0[1]);
    CHECK_EQUAL(myTestAgent, agent_list_0[2]);
    finlTestEnvPar(myTestAgent);
}

//< property_as_ref_param
LOAD_TEST(btunittest, property_as_ref_param)
{
    EmployeeParTestAgent* myTestAgent = initTestEnvPar("par_test/property_as_ref_param", format);

	ParTestRegNameAgent::clearAllStaticMemberVariables();
	behaviac::Agent::Create<ParTestRegNameAgent>("ParTestRegNameAgent");
    ParTestRegNameAgent* regNameAgent = behaviac::Agent::GetInstance<ParTestRegNameAgent>("ParTestRegNameAgent");
    regNameAgent->resetProperties();

    myTestAgent->resetProperties();
    myTestAgent->btexec();

    // base class 0 test
    CHECK_EQUAL(BLUE, myTestAgent->TV_ECOLOR_0);
    CHECK_EQUAL(true, myTestAgent->TV_BOOL_0);
    CHECK_EQUAL('X', myTestAgent->TV_CHAR_0);
    CHECK_EQUAL(2, myTestAgent->TV_BYTE_0);

    const behaviac::vector<unsigned char>& byte_list_0 = myTestAgent->TV_LIST_BYTE_0;
    CHECK_EQUAL(1, byte_list_0.size());
    CHECK_EQUAL(8, byte_list_0[0]);

    CHECK_EQUAL(-2, myTestAgent->TV_SBYTE_0);

    CHECK_EQUAL(1, myTestAgent->TV_LIST_ECOLOR_0.size());
    CHECK_EQUAL(RED, myTestAgent->TV_LIST_ECOLOR_0[0]);

    CHECK_EQUAL(1, myTestAgent->TV_LIST_BOOL_0.size());
    CHECK_EQUAL(true, myTestAgent->TV_LIST_BOOL_0[0]);

    CHECK_EQUAL(1, myTestAgent->TV_LIST_CHAR_0.size());
    CHECK_EQUAL('k', myTestAgent->TV_LIST_CHAR_0[0]);

    const behaviac::vector<signed char>& sbyte_list_0 = myTestAgent->TV_LIST_SBYTE_0;
    CHECK_EQUAL(1, sbyte_list_0.size());
    CHECK_EQUAL(-4, sbyte_list_0[0]);

    // base class 1 test
    CHECK_EQUAL(1, myTestAgent->TV_SHORT_0);
    CHECK_EQUAL(2, myTestAgent->TV_INT_0);
    CHECK_EQUAL(3L, myTestAgent->TV_LONG_0);
    CHECK_EQUAL(4, myTestAgent->TV_USHORT_0);

    CHECK_EQUAL(200L, myTestAgent->TV_ULL_0);
    CHECK_EQUAL(-200L, myTestAgent->TV_LL_0);

    kEmployee& kemployee_0 = myTestAgent->TV_KEMPLOYEE_0;
    CHECK_EQUAL(3, kemployee_0.id);
    CHECK_STR_EQUAL("Tom", kemployee_0.name.c_str());
    CHECK_EQUAL('X', kemployee_0.code);
    CHECK_FLOAT_EQUAL(58.7f, kemployee_0.weight);
    CHECK_EQUAL(true, kemployee_0.isMale);
    CHECK_EQUAL(GREEN, kemployee_0.skinColor);
    CHECK_EQUAL(myTestAgent, kemployee_0.boss);
    CHECK_STR_EQUAL("Honda", kemployee_0.car.brand.c_str());
    CHECK_EQUAL(RED, kemployee_0.car.color);
    CHECK_EQUAL(23000, kemployee_0.car.price);

    const behaviac::vector<signed int>& int_list_0 = myTestAgent->TV_LIST_INT_0;
    CHECK_EQUAL(1, int_list_0.size());
    CHECK_EQUAL(5, int_list_0[0]);

    const behaviac::vector<kEmployee>& kemployee_list_0 = myTestAgent->TV_LIST_KEMPLOYEE_0;
    CHECK_EQUAL(1, kemployee_list_0.size());
    CHECK_EQUAL(3, kemployee_list_0[0].id);
    CHECK_STR_EQUAL("Tom", kemployee_list_0[0].name.c_str());
    CHECK_EQUAL('X', kemployee_list_0[0].code);
    CHECK_FLOAT_EQUAL(58.7f, kemployee_list_0[0].weight);
    CHECK_EQUAL(true, kemployee_list_0[0].isMale);
    CHECK_EQUAL(GREEN, kemployee_list_0[0].skinColor);
    CHECK_EQUAL(myTestAgent, kemployee_list_0[0].boss);
    CHECK_STR_EQUAL("Honda", kemployee_list_0[0].car.brand.c_str());
    CHECK_EQUAL(RED, kemployee_list_0[0].car.color);
    CHECK_EQUAL(23000, kemployee_list_0[0].car.price);

    // base class 2 test
    CHECK_EQUAL(1, myTestAgent->TV_UINT_0);
    CHECK_EQUAL(2, myTestAgent->TV_ULONG_0);
    CHECK_FLOAT_EQUAL(3.0f, myTestAgent->TV_F_0);
    CHECK_FLOAT_EQUAL(4.0, myTestAgent->TV_D_0);
    CHECK_STR_EQUAL("This is a behaviac::string ref in test!", myTestAgent->TV_STR_0.c_str());
    CHECK_STR_EQUAL("This is a behaviac::string ref in test!", regNameAgent->TV_STR_0.c_str());
    CHECK_EQUAL(myTestAgent, myTestAgent->TV_AGENT_0);

    const behaviac::vector<float>& single_list_0 = myTestAgent->TV_LIST_F_0;
    CHECK_EQUAL(1, single_list_0.size());
    CHECK_FLOAT_EQUAL(1.0f, single_list_0[0]);

    const behaviac::vector<behaviac::string>& string_list_0 = myTestAgent->TV_LIST_STR_0;
    CHECK_EQUAL(1, string_list_0.size());
    CHECK_STR_EQUAL("This is a behaviac::string ref in test!", string_list_0[0].c_str());

    const behaviac::vector<behaviac::Agent*>& agent_list_0 = myTestAgent->TV_LIST_AGENT_0;
    CHECK_EQUAL(1, agent_list_0.size());
    CHECK_EQUAL(myTestAgent, agent_list_0[0]);

    behaviac::Agent::Destroy(regNameAgent);

    finlTestEnvPar(myTestAgent);
}

//< property_as_left_value_and_param
LOAD_TEST(btunittest, property_as_left_value_and_param)
{
    EmployeeParTestAgent* myTestAgent = initTestEnvPar("par_test/property_as_left_value_and_param", format);
    myTestAgent->resetProperties();
    myTestAgent->btexec();

    // base class 0 test
    CHECK_EQUAL(BLUE, myTestAgent->TV_ECOLOR_0);
    CHECK_EQUAL(true, myTestAgent->TV_BOOL_0);
    CHECK_EQUAL('D', myTestAgent->TV_CHAR_0);
    CHECK_EQUAL(12, myTestAgent->TV_BYTE_0);

    const behaviac::vector<unsigned char>& byte_list_0 = myTestAgent->TV_LIST_BYTE_0;
    CHECK_EQUAL(1, byte_list_0.size());
    CHECK_EQUAL(126, byte_list_0[0]);

    CHECK_EQUAL(-5, myTestAgent->TV_SBYTE_0);

    CHECK_EQUAL(1, myTestAgent->TV_LIST_ECOLOR_0.size());
    CHECK_EQUAL(YELLOW, myTestAgent->TV_LIST_ECOLOR_0[0]);

    CHECK_EQUAL(1, myTestAgent->TV_LIST_BOOL_0.size());
    CHECK_EQUAL(false, myTestAgent->TV_LIST_BOOL_0[0]);

    CHECK_EQUAL(1, myTestAgent->TV_LIST_CHAR_0.size());
    CHECK_EQUAL('m', myTestAgent->TV_LIST_CHAR_0[0]);

    const behaviac::vector<signed char>& sbyte_list_0 = myTestAgent->TV_LIST_SBYTE_0;
    CHECK_EQUAL(1, sbyte_list_0.size());
    CHECK_EQUAL(-126, sbyte_list_0[0]);

    // base class 1 test
    CHECK_EQUAL(250, myTestAgent->TV_SHORT_0);
    CHECK_EQUAL(350, myTestAgent->TV_INT_0);
    CHECK_EQUAL(450L, myTestAgent->TV_LONG_0);
    CHECK_EQUAL(550, myTestAgent->TV_USHORT_0);

    kEmployee& kemployee_0 = myTestAgent->TV_KEMPLOYEE_0;
    CHECK_EQUAL(2, kemployee_0.id);
    CHECK_STR_EQUAL("Jerry", kemployee_0.name.c_str());
    CHECK_EQUAL('V', kemployee_0.code);
    CHECK_FLOAT_EQUAL(20.2f, kemployee_0.weight);
    CHECK_EQUAL(false, kemployee_0.isMale);
    CHECK_EQUAL(BLUE, kemployee_0.skinColor);
    CHECK_EQUAL(myTestAgent, kemployee_0.boss);
    CHECK_STR_EQUAL("VolkswageJapan", kemployee_0.car.brand.c_str());
    CHECK_EQUAL(RED, kemployee_0.car.color);
    CHECK_EQUAL(3000, kemployee_0.car.price);

    const behaviac::vector<signed int>& int_list_0 = myTestAgent->TV_LIST_INT_0;
    CHECK_EQUAL(1, int_list_0.size());
    CHECK_EQUAL(235, int_list_0[0]);

    const behaviac::vector<kEmployee>& kemployee_list_0 = myTestAgent->TV_LIST_KEMPLOYEE_0;
    CHECK_EQUAL(4, kemployee_list_0[0].id);
    CHECK_STR_EQUAL("Jerry", kemployee_list_0[0].name.c_str());
    CHECK_EQUAL('J', kemployee_list_0[0].code);
    CHECK_FLOAT_EQUAL(60.0f, kemployee_list_0[0].weight);
    CHECK_EQUAL(false, kemployee_list_0[0].isMale);
    CHECK_EQUAL(WHITE, kemployee_list_0[0].skinColor);
    CHECK_EQUAL(NULL, kemployee_list_0[0].boss);
    CHECK_STR_EQUAL("Toyota", kemployee_list_0[0].car.brand.c_str());
    CHECK_EQUAL(YELLOW, kemployee_list_0[0].car.color);
    CHECK_EQUAL(43000, kemployee_list_0[0].car.price);

    // base class 2 test
    CHECK_EQUAL(54, myTestAgent->TV_UINT_0);
    CHECK_EQUAL(89, myTestAgent->TV_ULONG_0);
    CHECK_EQUAL(-200, myTestAgent->TV_LL_0);
    CHECK_EQUAL(200, myTestAgent->TV_ULL_0);
    CHECK_FLOAT_EQUAL(72.3f, myTestAgent->TV_F_0);
    CHECK_FLOAT_EQUAL(42.9, myTestAgent->TV_D_0);
    CHECK_STR_EQUAL("extra", myTestAgent->TV_STR_0.c_str());
    CHECK_EQUAL(myTestAgent, myTestAgent->TV_AGENT_0);

    const behaviac::vector<float>& single_list_0 = myTestAgent->TV_LIST_F_0;
    CHECK_EQUAL(1, single_list_0.size());
    CHECK_FLOAT_EQUAL(93.7f, single_list_0[0]);

    const behaviac::vector<behaviac::string>& string_list_0 = myTestAgent->TV_LIST_STR_0;
    CHECK_EQUAL(1, string_list_0.size());
    CHECK_STR_EQUAL("extra", string_list_0[0].c_str());

    const behaviac::vector<behaviac::Agent*>& agent_list_0 = myTestAgent->TV_LIST_AGENT_0;
    CHECK_EQUAL(1, agent_list_0.size());
    CHECK_EQUAL(myTestAgent, agent_list_0[0]);
    finlTestEnvPar(myTestAgent);
}

//< static_property_as_ref_param
LOAD_TEST(btunittest, static_property_as_ref_param)
{
    EmployeeParTestAgent* myTestAgent = initTestEnvPar("par_test/static_property_as_ref_param", format);
    myTestAgent->resetProperties();
    myTestAgent->btexec();

    // base class 0 test
    CHECK_EQUAL(BLUE, EmployeeParTestAgent::STV_ECOLOR_0);
    CHECK_EQUAL(true, EmployeeParTestAgent::STV_BOOL_0);
    CHECK_EQUAL('X', EmployeeParTestAgent::STV_CHAR_0);

    CHECK_EQUAL(1, EmployeeParTestAgent::STV_LIST_ECOLOR_0.size());
    CHECK_EQUAL(RED, EmployeeParTestAgent::STV_LIST_ECOLOR_0[0]);

    CHECK_EQUAL(1, EmployeeParTestAgent::STV_LIST_BOOL_0.size());
    CHECK_EQUAL(true, EmployeeParTestAgent::STV_LIST_BOOL_0[0]);

    CHECK_EQUAL(1, EmployeeParTestAgent::STV_LIST_CHAR_0.size());
    CHECK_EQUAL('k', EmployeeParTestAgent::STV_LIST_CHAR_0[0]);

    const behaviac::vector<signed char>& sbyte_list_0 = EmployeeParTestAgent::STV_LIST_SBYTE_0;
    CHECK_EQUAL(1, sbyte_list_0.size());
    CHECK_EQUAL(-4, sbyte_list_0[0]);

    // base class 1 test
    CHECK_EQUAL(2, EmployeeParTestAgent::STV_INT_0);

    kEmployee& kemployee_0 = EmployeeParTestAgent::STV_KEMPLOYEE_0;
    CHECK_EQUAL(3, kemployee_0.id);
    CHECK_STR_EQUAL("Tom", kemployee_0.name.c_str());
    CHECK_EQUAL('X', kemployee_0.code);
    CHECK_FLOAT_EQUAL(58.7f, kemployee_0.weight);
    CHECK_EQUAL(true, kemployee_0.isMale);
    CHECK_EQUAL(GREEN, kemployee_0.skinColor);
    CHECK_EQUAL(myTestAgent, kemployee_0.boss);
    CHECK_STR_EQUAL("Honda", kemployee_0.car.brand.c_str());
    CHECK_EQUAL(RED, kemployee_0.car.color);
    CHECK_EQUAL(23000, kemployee_0.car.price);

    const behaviac::vector<signed int>& int_list_0 = EmployeeParTestAgent::STV_LIST_INT_0;
    CHECK_EQUAL(1, int_list_0.size());
    CHECK_EQUAL(5, int_list_0[0]);

    const behaviac::vector<kEmployee>& kemployee_list_0 = EmployeeParTestAgent::STV_LIST_KEMPLOYEE_0;
    CHECK_EQUAL(1, kemployee_list_0.size());
    CHECK_EQUAL(3, kemployee_list_0[0].id);
    CHECK_STR_EQUAL("Tom", kemployee_list_0[0].name.c_str());
    CHECK_EQUAL('X', kemployee_list_0[0].code);
    CHECK_FLOAT_EQUAL(58.7f, kemployee_list_0[0].weight);
    CHECK_EQUAL(true, kemployee_list_0[0].isMale);
    CHECK_EQUAL(GREEN, kemployee_list_0[0].skinColor);
    CHECK_EQUAL(myTestAgent, kemployee_list_0[0].boss);
    CHECK_STR_EQUAL("Honda", kemployee_list_0[0].car.brand.c_str());
    CHECK_EQUAL(RED, kemployee_list_0[0].car.color);
    CHECK_EQUAL(23000, kemployee_list_0[0].car.price);

    // base class 2 test
    CHECK_FLOAT_EQUAL(3.0f, EmployeeParTestAgent::STV_F_0);
    CHECK_STR_EQUAL("This is a behaviac::string ref in test!", EmployeeParTestAgent::STV_STR_0.c_str());
    CHECK_EQUAL(myTestAgent, EmployeeParTestAgent::STV_AGENT_0);

    const behaviac::vector<float>& single_list_0 = EmployeeParTestAgent::STV_LIST_F_0;
    CHECK_EQUAL(1, single_list_0.size());
    CHECK_FLOAT_EQUAL(1.0f, single_list_0[0]);

    const behaviac::vector<behaviac::string>& string_list_0 = EmployeeParTestAgent::STV_LIST_STR_0;
    CHECK_EQUAL(1, string_list_0.size());
    CHECK_STR_EQUAL("This is a behaviac::string ref in test!", string_list_0[0].c_str());

    const behaviac::vector<behaviac::Agent*>& agent_list_0 = EmployeeParTestAgent::STV_LIST_AGENT_0;
    CHECK_EQUAL(1, agent_list_0.size());
    CHECK_EQUAL(myTestAgent, agent_list_0[0]);
    finlTestEnvPar(myTestAgent);
}

//< static_property_as_left_value_and_param
LOAD_TEST(btunittest, static_property_as_left_value_and_param)
{
    EmployeeParTestAgent* myTestAgent = initTestEnvPar("par_test/static_property_as_left_value_and_param", format);
    myTestAgent->resetProperties();
    myTestAgent->btexec();

    // base class 0 test
    CHECK_EQUAL(BLUE, EmployeeParTestAgent::STV_ECOLOR_0);
    CHECK_EQUAL(true, EmployeeParTestAgent::STV_BOOL_0);
    CHECK_EQUAL('D', EmployeeParTestAgent::STV_CHAR_0);

    CHECK_EQUAL(1, EmployeeParTestAgent::STV_LIST_ECOLOR_0.size());
    CHECK_EQUAL(YELLOW, EmployeeParTestAgent::STV_LIST_ECOLOR_0[0]);

    CHECK_EQUAL(1, EmployeeParTestAgent::STV_LIST_BOOL_0.size());
    CHECK_EQUAL(false, EmployeeParTestAgent::STV_LIST_BOOL_0[0]);

    CHECK_EQUAL(1, EmployeeParTestAgent::STV_LIST_CHAR_0.size());
    CHECK_EQUAL('m', EmployeeParTestAgent::STV_LIST_CHAR_0[0]);

    const behaviac::vector<signed char>& sbyte_list_0 = EmployeeParTestAgent::STV_LIST_SBYTE_0;
    CHECK_EQUAL(1, sbyte_list_0.size());
    CHECK_EQUAL(-126, sbyte_list_0[0]);

    // base class 1 test
    CHECK_EQUAL(350, EmployeeParTestAgent::STV_INT_0);

    kEmployee& kemployee_0 = EmployeeParTestAgent::STV_KEMPLOYEE_0;
    CHECK_EQUAL(2, kemployee_0.id);
    CHECK_STR_EQUAL("Jerry", kemployee_0.name.c_str());
    CHECK_EQUAL('V', kemployee_0.code);
    CHECK_FLOAT_EQUAL(20.2f, kemployee_0.weight);
    CHECK_EQUAL(false, kemployee_0.isMale);
    CHECK_EQUAL(BLUE, kemployee_0.skinColor);
    CHECK_EQUAL(myTestAgent, kemployee_0.boss);
    CHECK_STR_EQUAL("VolkswageJapan", kemployee_0.car.brand.c_str());
    CHECK_EQUAL(RED, kemployee_0.car.color);
    CHECK_EQUAL(3000, kemployee_0.car.price);

    const behaviac::vector<signed int>& int_list_0 = EmployeeParTestAgent::STV_LIST_INT_0;
    CHECK_EQUAL(1, int_list_0.size());
    CHECK_EQUAL(235, int_list_0[0]);

    const behaviac::vector<kEmployee>& kemployee_list_0 = EmployeeParTestAgent::STV_LIST_KEMPLOYEE_0;
    CHECK_EQUAL(4, kemployee_list_0[0].id);
    CHECK_STR_EQUAL("Jerry", kemployee_list_0[0].name.c_str());
    CHECK_EQUAL('J', kemployee_list_0[0].code);
    CHECK_FLOAT_EQUAL(60.0f, kemployee_list_0[0].weight);
    CHECK_EQUAL(false, kemployee_list_0[0].isMale);
    CHECK_EQUAL(WHITE, kemployee_list_0[0].skinColor);
    CHECK_EQUAL(NULL, kemployee_list_0[0].boss);
    CHECK_STR_EQUAL("Toyota", kemployee_list_0[0].car.brand.c_str());
    CHECK_EQUAL(YELLOW, kemployee_list_0[0].car.color);
    CHECK_EQUAL(43000, kemployee_list_0[0].car.price);

    // base class 2 test
    CHECK_FLOAT_EQUAL(72.3f, EmployeeParTestAgent::STV_F_0);
    CHECK_STR_EQUAL("extra", EmployeeParTestAgent::STV_STR_0.c_str());
    CHECK_EQUAL(myTestAgent, EmployeeParTestAgent::STV_AGENT_0);

    const behaviac::vector<float>& single_list_0 = EmployeeParTestAgent::STV_LIST_F_0;
    CHECK_EQUAL(1, single_list_0.size());
    CHECK_FLOAT_EQUAL(93.7f, single_list_0[0]);

    const behaviac::vector<behaviac::string>& string_list_0 = EmployeeParTestAgent::STV_LIST_STR_0;
    CHECK_EQUAL(1, string_list_0.size());
    CHECK_STR_EQUAL("extra", string_list_0[0].c_str());

    const behaviac::vector<behaviac::Agent*>& agent_list_0 = EmployeeParTestAgent::STV_LIST_AGENT_0;
    CHECK_EQUAL(1, agent_list_0.size());
    CHECK_EQUAL(myTestAgent, agent_list_0[0]);
    finlTestEnvPar(myTestAgent);
}

//< register_name_as_left_value_and_param
LOAD_TEST(btunittest, register_name_as_left_value_and_param)
{
    EmployeeParTestAgent* myTestAgent = initTestEnvPar("par_test/register_name_as_left_value_and_param", format);

    ParTestRegNameAgent::clearAllStaticMemberVariables();
    behaviac::Agent::Create<ParTestRegNameAgent>("ParTestRegNameAgent");
    ParTestRegNameAgent* regNameAgent = behaviac::Agent::GetInstance<ParTestRegNameAgent>("ParTestRegNameAgent");
    regNameAgent->resetProperties();

    myTestAgent->resetProperties();
    myTestAgent->btexec();

    CHECK_EQUAL('D', regNameAgent->TV_CHAR_0);
    CHECK_EQUAL(12, regNameAgent->TV_BYTE_0);
    CHECK_EQUAL(-5, regNameAgent->TV_SBYTE_0);
    CHECK_STR_EQUAL("extra", regNameAgent->TV_STR_0.c_str());
    CHECK_EQUAL(regNameAgent, regNameAgent->TV_AGENT_0);

    kEmployee& kemployee_0 = regNameAgent->TV_KEMPLOYEE_0;
    CHECK_EQUAL(2, kemployee_0.id);
    CHECK_STR_EQUAL("Jerry", kemployee_0.name.c_str());
    CHECK_EQUAL('V', kemployee_0.code);
    CHECK_FLOAT_EQUAL(20.2f, kemployee_0.weight);
    CHECK_EQUAL(false, kemployee_0.isMale);
    CHECK_EQUAL(BLUE, kemployee_0.skinColor);
    CHECK_EQUAL(regNameAgent, kemployee_0.boss);
    CHECK_STR_EQUAL("VolkswageJapan", kemployee_0.car.brand.c_str());
    CHECK_EQUAL(RED, kemployee_0.car.color);
    CHECK_EQUAL(3000, kemployee_0.car.price);

    kEmployee& kemployee_1 = ParTestRegNameAgent::STV_KEMPLOYEE_0;
    CHECK_EQUAL(2, kemployee_1.id);
    CHECK_STR_EQUAL("Jerry", kemployee_1.name.c_str());
    CHECK_EQUAL('V', kemployee_1.code);
    CHECK_FLOAT_EQUAL(20.2f, kemployee_1.weight);
    CHECK_EQUAL(false, kemployee_1.isMale);
    CHECK_EQUAL(BLUE, kemployee_1.skinColor);
    CHECK_EQUAL(regNameAgent, kemployee_1.boss);
    CHECK_STR_EQUAL("VolkswageJapan", kemployee_1.car.brand.c_str());
    CHECK_EQUAL(RED, kemployee_1.car.color);
    CHECK_EQUAL(3000, kemployee_1.car.price);

    const behaviac::vector<signed char>& sbyte_list_0 = ParTestRegNameAgent::STV_LIST_SBYTE_0;
    CHECK_EQUAL(1, sbyte_list_0.size());
    CHECK_EQUAL(-126, sbyte_list_0[0]);

    const behaviac::vector<kEmployee>& kemployee_list_0 = regNameAgent->TV_LIST_KEMPLOYEE_0;
    CHECK_EQUAL(4, kemployee_list_0[0].id);
    CHECK_STR_EQUAL("Jerry", kemployee_list_0[0].name.c_str());
    CHECK_EQUAL('J', kemployee_list_0[0].code);
    CHECK_FLOAT_EQUAL(60.0f, kemployee_list_0[0].weight);
    CHECK_EQUAL(false, kemployee_list_0[0].isMale);
    CHECK_EQUAL(WHITE, kemployee_list_0[0].skinColor);
    CHECK_EQUAL(NULL, kemployee_list_0[0].boss);
    CHECK_STR_EQUAL("Toyota", kemployee_list_0[0].car.brand.c_str());
    CHECK_EQUAL(YELLOW, kemployee_list_0[0].car.color);
    CHECK_EQUAL(43000, kemployee_list_0[0].car.price);

    const behaviac::vector<kEmployee>& kemployee_list_1 = ParTestRegNameAgent::STV_LIST_KEMPLOYEE_0;
    CHECK_EQUAL(4, kemployee_list_1[0].id);
    CHECK_STR_EQUAL("Jerry", kemployee_list_1[0].name.c_str());
    CHECK_EQUAL('J', kemployee_list_1[0].code);
    CHECK_FLOAT_EQUAL(60.0f, kemployee_list_1[0].weight);
    CHECK_EQUAL(false, kemployee_list_1[0].isMale);
    CHECK_EQUAL(WHITE, kemployee_list_1[0].skinColor);
    CHECK_EQUAL(NULL, kemployee_list_1[0].boss);
    CHECK_STR_EQUAL("Toyota", kemployee_list_1[0].car.brand.c_str());
    CHECK_EQUAL(YELLOW, kemployee_list_1[0].car.color);
    CHECK_EQUAL(43000, kemployee_list_1[0].car.price);

    behaviac::Agent::Destroy(regNameAgent);

    finlTestEnvPar(myTestAgent);
}

//< const_param
LOAD_TEST(btunittest, const_param)
{
    EmployeeParTestAgent* parTestAgent = initTestEnvPar("par_test/const_param", format);
    parTestAgent->resetProperties();
    parTestAgent->btexec();


    CHECK_EQUAL(false, parTestAgent->TV_BOOL_0);
    CHECK_EQUAL(13, parTestAgent->TV_BYTE_0);

    CHECK_EQUAL(3, parTestAgent->TV_LIST_BOOL_0.size());
    CHECK_EQUAL(true, parTestAgent->TV_LIST_BOOL_0[0]);
    CHECK_EQUAL(true, parTestAgent->TV_LIST_BOOL_0[1]);
    CHECK_EQUAL(false, parTestAgent->TV_LIST_BOOL_0[2]);
    finlTestEnvPar(parTestAgent);
}

LOAD_TEST(btunittest, cast_param)
{
	EmployeeParTestAgent* parTestAgent = initTestEnvPar("par_test/cast_param", format);
	parTestAgent->resetProperties();
	parTestAgent->TV_UINT_0 = 10;
	parTestAgent->btexec();

	CHECK_EQUAL(72, parTestAgent->TV_BYTE_0);
	CHECK_EQUAL(10, parTestAgent->TV_SHORT_0);
	CHECK_EQUAL(0, parTestAgent->TV_AGENT_0);

	finlTestEnvPar(parTestAgent);
}

//< static_member_function_test_0
LOAD_TEST(btunittest, static_member_function_test_0)
{
    EmployeeParTestAgent* myTestAgent = initTestEnvPar("par_test/static_member_function_test_0", format);
    myTestAgent->resetProperties();
    myTestAgent->btexec();

    CHECK_EQUAL('D', myTestAgent->TV_CHAR_0);
    CHECK_EQUAL(12, myTestAgent->TV_BYTE_0);
    CHECK_EQUAL(-5, myTestAgent->TV_SBYTE_0);
    CHECK_STR_EQUAL("extra", myTestAgent->TV_STR_0.c_str());

    CHECK_EQUAL(1, myTestAgent->TV_LIST_CHAR_0.size());
    CHECK_EQUAL('m', myTestAgent->TV_LIST_CHAR_0[0]);

    const behaviac::vector<signed char>& sbyte_list_0 = myTestAgent->TV_LIST_SBYTE_0;
    CHECK_EQUAL(1, sbyte_list_0.size());
    CHECK_EQUAL(-126, sbyte_list_0[0]);

    kEmployee& kemployee_0 = myTestAgent->TV_KEMPLOYEE_0;
    CHECK_EQUAL(2, kemployee_0.id);
    CHECK_STR_EQUAL("Jerry", kemployee_0.name.c_str());
    CHECK_EQUAL('V', kemployee_0.code);
    CHECK_FLOAT_EQUAL(20.2f, kemployee_0.weight);
    CHECK_EQUAL(false, kemployee_0.isMale);
    CHECK_EQUAL(BLUE, kemployee_0.skinColor);
    CHECK_EQUAL(NULL, kemployee_0.boss);
    CHECK_STR_EQUAL("VolkswageJapan", kemployee_0.car.brand.c_str());
    CHECK_EQUAL(RED, kemployee_0.car.color);
    CHECK_EQUAL(3000, kemployee_0.car.price);

    const behaviac::vector<kEmployee>& kemployee_list_0 = myTestAgent->TV_LIST_KEMPLOYEE_0;
    CHECK_EQUAL(4, kemployee_list_0[0].id);
    CHECK_STR_EQUAL("Jerry", kemployee_list_0[0].name.c_str());
    CHECK_EQUAL('J', kemployee_list_0[0].code);
    CHECK_FLOAT_EQUAL(60.0f, kemployee_list_0[0].weight);
    CHECK_EQUAL(false, kemployee_list_0[0].isMale);
    CHECK_EQUAL(WHITE, kemployee_list_0[0].skinColor);
    CHECK_EQUAL(NULL, kemployee_list_0[0].boss);
    CHECK_STR_EQUAL("Toyota", kemployee_list_0[0].car.brand.c_str());
    CHECK_EQUAL(YELLOW, kemployee_list_0[0].car.color);
    CHECK_EQUAL(43000, kemployee_list_0[0].car.price);

    CHECK_EQUAL(89, myTestAgent->TV_ULONG_0);
    CHECK_STR_EQUAL("extra", myTestAgent->TV_STR_0.c_str());

    const behaviac::vector<behaviac::string>& string_list_0 = myTestAgent->TV_LIST_STR_0;
    CHECK_EQUAL(1, string_list_0.size());
    CHECK_STR_EQUAL("extra", string_list_0[0].c_str());

    const behaviac::vector<behaviac::Agent*>& agent_list_0 = myTestAgent->TV_LIST_AGENT_0;
    CHECK_EQUAL(1, agent_list_0.size());
    CHECK_EQUAL(NULL, agent_list_0[0]);
    finlTestEnvPar(myTestAgent);
}
