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
#include "behaviac/behaviac.h"

#include "../btloadtestsuite.h"

//#if BEHAVIAC_CCDEFINE_MSVC
#if 1
void memory_leak_test(behaviac::Workspace::EFileFormat format)
{
#if ENABLE_MEMORYDUMP
    _CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
    _CrtDumpMemoryLeaks();

    static long s_breakalloc = -1;
    _CrtSetBreakAlloc(s_breakalloc);
#endif
    behaviac::IMemAllocator& allocator = behaviac::GetMemoryAllocator();
	size_t allocatedSize = allocator.GetAllocatedSize();

#if ENABLE_MEMORYDUMP
    _CrtMemState s1;
    _CrtMemCheckpoint(&s1);
#endif
    EmployeeParTestAgent::clearAllStaticMemberVariables();

    behaviac::Profiler::CreateInstance();
    behaviac::Config::SetSocketing(false);
    behaviac::Config::SetLogging(false);

    registerAllTypes();
    EmployeeParTestAgent* myTestAgent = EmployeeParTestAgent::DynamicCast(behaviac::Agent::Create<EmployeeParTestAgent>());
    behaviac::Agent::SetIdMask(1);
    myTestAgent->SetIdFlag(1);

    //myTestAgent->btload("par_test/register_name_as_left_value_and_param");
    myTestAgent->btsetcurrent("par_test/register_name_as_left_value_and_param");

    ParTestRegNameAgent::clearAllStaticMemberVariables();

    behaviac::Agent::Create<ParTestRegNameAgent>("ParTestRegNameAgent");
    ParTestRegNameAgent* regNameAgent = behaviac::Agent::GetInstance<ParTestRegNameAgent>("ParTestRegNameAgent");
    regNameAgent->resetProperties();

    myTestAgent->resetProperties();
    myTestAgent->btexec();

    behaviac::Agent::Destroy(regNameAgent);
    behaviac::Agent::UnRegisterInstanceName<ParTestRegNameAgent>("ParTestRegNameAgent");

    behaviac::Agent::Destroy(myTestAgent);
    behaviac::Workspace::GetInstance()->UnLoadAll();

	//behaviac::Workspace::GetInstance()->Cleanup();
	//behaviac::Workspace* pWorkspace = behaviac::Workspace::GetInstance();
	//BEHAVIAC_DELETE pWorkspace;

    EmployeeParTestAgent::clearAllStaticMemberVariables();
    ParTestRegNameAgent::clearAllStaticMemberVariables();

    unregisterAllTypes();

    behaviac::Profiler::DestroyInstance();

	size_t allocatedSize1 = allocator.GetAllocatedSize();
	size_t allocateDiff = allocatedSize1 - allocatedSize;

#if !ENABLE_MEMORYDUMP
    //if CStringCRC is used before this test, CStringCRC::Cleaup() will free more memory
    //allocateDiff would be negative
    //CHECK_EQUAL(true, allocateDiff <= 0);
    CHECK_EQUAL(true, allocateDiff == 0);
#endif

#if ENABLE_MEMORYDUMP
    _CrtMemState s2;
    _CrtMemState s3;

    BEHAVIAC_UNUSED_VAR(s1);
    BEHAVIAC_UNUSED_VAR(s2);
    BEHAVIAC_UNUSED_VAR(s3);

    _CrtMemCheckpoint(&s2);

    //CHECK_EQUAL(0, _CrtMemDifference(&s3, &s1, &s2));
    if (_CrtMemDifference(&s3, &s1, &s2))
    {
        _CrtMemDumpStatistics(&s3);
        _CrtDumpMemoryLeaks();
    }

#endif
    BEHAVIAC_ASSERT(true);
}

TEST(btunittest, memory_leak)
{
    behaviac::Workspace::EFileFormat format = behaviac::Workspace::EFF_xml;
    memory_leak_test(format);

    //format = behaviac::Workspace::EFF_bson;
    //memory_leak_test(format);
}

#endif//#if BEHAVIAC_CCDEFINE_MSVC

void BehaviorNodeLoadedHandler(const char* nodeType, const behaviac::properties_t& properties)
{
    BEHAVIAC_UNUSED_VAR(nodeType);
    BEHAVIAC_UNUSED_VAR(properties);
}

void btunitest_game(behaviac::Workspace::EFileFormat format)
{
    EmployeeParTestAgent::clearAllStaticMemberVariables();

    behaviac::Profiler::CreateInstance();
    behaviac::Config::SetSocketing(false);
    behaviac::Config::SetLogging(false);

    registerAllTypes();
    EmployeeParTestAgent* myTestAgent = EmployeeParTestAgent::DynamicCast(behaviac::Agent::Create<EmployeeParTestAgent>());
    behaviac::Agent::SetIdMask(1);
    myTestAgent->SetIdFlag(1);

    //myTestAgent->btload("par_test/register_name_as_left_value_and_param");
    myTestAgent->btsetcurrent("par_test/register_name_as_left_value_and_param");

    ParTestRegNameAgent::clearAllStaticMemberVariables();

    behaviac::Agent::Create<ParTestRegNameAgent>("ParTestRegNameAgent");
    ParTestRegNameAgent* regNameAgent = behaviac::Agent::GetInstance<ParTestRegNameAgent>("ParTestRegNameAgent");
    regNameAgent->resetProperties();

    myTestAgent->resetProperties();
    myTestAgent->btexec();

    behaviac::Agent::Destroy(regNameAgent);
    behaviac::Agent::UnRegisterInstanceName<ParTestRegNameAgent>("ParTestRegNameAgent");

    BEHAVIAC_DELETE(myTestAgent);
    behaviac::Workspace::GetInstance()->UnLoadAll();

    unregisterAllTypes();

    behaviac::Profiler::DestroyInstance();

    BEHAVIAC_ASSERT(true);
}

//<
TEST(btunittest, agentInstance)
{
    behaviac::Profiler::CreateInstance();
    behaviac::Config::SetSocketing(false);
    behaviac::Config::SetLogging(false);

    registerAllTypes();

    behaviac::Agent::RegisterInstanceName<AgentNodeTest>();
    behaviac::Agent::RegisterInstanceName<behaviac::Agent>("Name_Agent_0");
    behaviac::Agent::RegisterInstanceName<behaviac::Agent>("Name_Agent_1");
    behaviac::Agent::RegisterInstanceName<behaviac::Agent>("Name_Agent_2");

    behaviac::Agent* testAgentA = behaviac::Agent::Create<behaviac::Agent>("Name_Agent_0");
    AgentNodeTest* testAgentB = behaviac::Agent::Create<AgentNodeTest>(NULL);

    behaviac::Agent* testAgent_0 = behaviac::Agent::GetInstance<behaviac::Agent>("Name_Agent_0");
    AgentNodeTest* testAgent_1 = behaviac::Agent::GetInstance<AgentNodeTest>();
#if !BEHAVIAC_RELEASE
    AgentNodeTest* testAgent_3 = AgentNodeTest::DynamicCast(behaviac::Agent::GetAgent("AgentNodeTest"));
#endif//BEHAVIAC_RELEASE

    CHECK_EQUAL(testAgent_0, testAgentA);
    CHECK_EQUAL(testAgent_1, testAgentB);
#if !BEHAVIAC_RELEASE
    CHECK_EQUAL(testAgent_1, testAgent_3);
#endif//BEHAVIAC_RELEASE
    BEHAVIAC_ASSERT(testAgent_0);
    BEHAVIAC_ASSERT(testAgent_1);

    behaviac::Agent::UnbindInstance("Name_Agent_0");
    behaviac::Agent::UnbindInstance("AgentNodeTest");

#if !BEHAVIAC_RELEASE
    testAgent_3 = AgentNodeTest::DynamicCast(behaviac::Agent::GetAgent("AgentNodeTest#AgentNodeTest"));
    CHECK_EQUAL(testAgentB, testAgent_3);
#endif//BEHAVIAC_RELEASE

    behaviac::Agent* testAgent_0_0 = behaviac::Agent::GetInstance<behaviac::Agent>("Name_Agent_0");
    AgentNodeTest* testAgent_1_0 = behaviac::Agent::GetInstance<AgentNodeTest>();

    BEHAVIAC_ASSERT(!testAgent_0_0);
    BEHAVIAC_ASSERT(!testAgent_1_0);

    behaviac::Agent::BindInstance(testAgent_0, "Name_Agent_1");
    behaviac::Agent* testAgent_0_1 = behaviac::Agent::GetInstance<behaviac::Agent>("Name_Agent_1");
    BEHAVIAC_ASSERT(testAgent_0_1);
    BEHAVIAC_UNUSED_VAR(testAgent_0_1);

    behaviac::Agent::BindInstance(testAgent_0, "Name_Agent_2");
    behaviac::Agent* testAgent_0_2 = behaviac::Agent::GetInstance<behaviac::Agent>("Name_Agent_2");
    BEHAVIAC_ASSERT(testAgent_0_2);
    BEHAVIAC_UNUSED_VAR(testAgent_0_2);

    CHECK_EQUAL(testAgent_0_1, testAgent_0_2);

    behaviac::Agent::UnbindInstance("Name_Agent_1");
    behaviac::Agent::UnbindInstance("Name_Agent_2");

    behaviac::Agent::Destroy(testAgent_0_0);
    behaviac::Agent::Destroy(testAgent_1_0);

    behaviac::Agent::UnRegisterInstanceName<behaviac::Agent>("Name_Agent_2");
    behaviac::Agent::UnRegisterInstanceName<behaviac::Agent>("Name_Agent_1");
    behaviac::Agent::UnRegisterInstanceName<behaviac::Agent>("Name_Agent_0");
    behaviac::Agent::UnRegisterInstanceName<AgentNodeTest>();

    unregisterAllTypes();

    behaviac::Profiler::DestroyInstance();
};

TEST(btunittest, context)
{
    EmployeeParTestAgent::clearAllStaticMemberVariables();

    behaviac::Profiler::CreateInstance();
    behaviac::Config::SetSocketing(false);
    behaviac::Config::SetLogging(false);

    registerAllTypes();
    behaviac::Agent::RegisterInstanceName<ParTestAgent>("ParTestAgent");

    ParTestAgent* testAgent_0 = behaviac::Agent::Create<ParTestAgent>(0, 1);
    ParTestAgent* testAgent_1 = behaviac::Agent::Create<ParTestAgent>(0, 2);
    ParTestAgent* testAgent_2 = behaviac::Agent::Create<ParTestAgent>(0, 3);

    CHECK_NOT_EQUAL(testAgent_0, testAgent_1);
    CHECK_NOT_EQUAL(testAgent_1, testAgent_2);
    CHECK_NOT_EQUAL(testAgent_2, testAgent_0);

    testAgent_0->resetProperties();
    testAgent_1->resetProperties();
    testAgent_2->resetProperties();

    testAgent_0->TV_BOOL_0 = true;
    CHECK_EQUAL(true, testAgent_0->TV_BOOL_0);
    CHECK_EQUAL(false, testAgent_1->TV_BOOL_0);
    CHECK_EQUAL(false, testAgent_2->TV_BOOL_0);

    testAgent_1->TV_BOOL_0 = true;
    CHECK_EQUAL(true, testAgent_0->TV_BOOL_0);
    CHECK_EQUAL(true, testAgent_1->TV_BOOL_0);
    CHECK_EQUAL(false, testAgent_2->TV_BOOL_0);

    testAgent_2->TV_BOOL_0 = true;
    CHECK_EQUAL(true, testAgent_0->TV_BOOL_0);
    CHECK_EQUAL(true, testAgent_1->TV_BOOL_0);
    CHECK_EQUAL(true, testAgent_1->TV_BOOL_0);

    behaviac::Agent::Destroy(testAgent_0);
    behaviac::Agent::Destroy(testAgent_1);
    behaviac::Agent::Destroy(testAgent_2);

    behaviac::Context::Cleanup(1);
    behaviac::Context::Cleanup(2);
    behaviac::Context::Cleanup(3);

    behaviac::Agent::UnRegisterInstanceName<ParTestAgent>("ParTestAgent");
    unregisterAllTypes();

    behaviac::Profiler::DestroyInstance();
}

//TEST(btunittest, agentInvoke)
//{
//    EmployeeParTestAgent::clearAllStaticMemberVariables();
//
//    behaviac::Profiler::CreateInstance();
//    behaviac::Config::SetSocketing(false);
//    behaviac::Config::SetLogging(false);
//
//    registerAllTypes();
//
//    behaviac::Agent* testAgent = behaviac::Agent::Create<AgentNodeTest>();
//    AgentNodeTest* nodeTestAgent = AgentNodeTest::DynamicCast(testAgent);
//
//    CHECK_EQUAL(true, behaviac::Agent::Invoke(testAgent, "AgentNodeTest::setTestVar_0", 999));
//    CHECK_EQUAL(999, nodeTestAgent->testVar_0);
//
//    CHECK_EQUAL(true, behaviac::Agent::Invoke(testAgent, "AgentNodeTest::setTestVar_0_2", 8999, 1000.99f));
//    CHECK_EQUAL(8999, nodeTestAgent->testVar_0);
//    CHECK_FLOAT_EQUAL(1000.99f, nodeTestAgent->testVar_2);
//
//    //float returnValue = 0.0f;
//    //CHECK_EQUAL(true, behaviac::Agent::Invoke(testAgent, "AgentNodeTest::setTestVar_R"));
//    //behaviac::Agent::GetInvokeReturn(testAgent, "AgentNodeTest::setTestVar_R", returnValue);
//    //CHECK_EQUAL(9999.99f, returnValue);
//
//    behaviac::Agent::Destroy(testAgent);
//
//    unregisterAllTypes();
//
//    behaviac::Profiler::DestroyInstance();
//}
//<
class CFileManager_Test : public behaviac::CFileManager
{
public:
    BEHAVIAC_DECLARE_MEMORY_OPERATORS(CFileManager_Test);

	CFileManager_Test() : behaviac::CFileManager()
    {
    }

    virtual ~CFileManager_Test()
    {
    }

	virtual behaviac::IFile* FileOpen(const char* fileName, behaviac::CFileSystem::EOpenMode iOpenAccess = behaviac::CFileSystem::EOpenMode_Read)
    {
		return behaviac::CFileManager::FileOpen(fileName, iOpenAccess);
    }

	virtual void FileClose(behaviac::IFile* file)
    {
		return behaviac::CFileManager::FileClose(file);
    }
};

TEST(btunittest, filemanager)
{
	behaviac::CFileManager::Cleanup();

	behaviac::CFileManager* pFileManager = BEHAVIAC_NEW CFileManager_Test();

	behaviac::IFile* fp = behaviac::CFileManager::GetInstance()->FileOpen("../inc/behaviac/behaviac.h");
    CHECK_NOT_EQUAL(0, fp);
    behaviac::CFileManager::GetInstance()->FileClose(fp);

    BEHAVIAC_DELETE(pFileManager);
}

TEST(btunittest, filesystem_findfiles)
{
	behaviac::vector<behaviac::string> fileList;
	behaviac::CFileSystem::ListFiles(fileList, "./", false);
}

TEST(btunittest, loadtest)
{
    behaviac::Profiler::GetInstance();
    behaviac::Profiler::DestroyInstance();

    loadtest::LoadTestSuite& loadTestSuite = loadtest::LoadTestSuite::getInstance();

    std::cout << std::endl << std::endl << "XML LOAD TEST:" << std::endl;
    loadTestSuite.runAllLoadTests(behaviac::Workspace::EFF_xml);

    std::cout << std::endl << "BSON LOAD TEST:" << std::endl;
    loadTestSuite.runAllLoadTests(behaviac::Workspace::EFF_bson);

    std::cout << std::endl << "CPP LOAD TEST:" << std::endl;
    loadTestSuite.runAllLoadTests(behaviac::Workspace::EFF_cpp);

    std::cout << std::endl;
}
