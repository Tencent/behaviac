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

#include "test.h"
#include "behaviortest.h"
#include "Agent/AgentNodeTest.h"
#include "Agent/EmployeeParTestAgent.h"
#include "Agent/ParTestRegNameAgent.h"

#include "Agent/UnitTestTypes.h"
#include "Agent/HTNAgentHouse.h"
#include "Agent/FSMAgentTest.h"
#include "Agent/HTNAgentTravel.h"
#include "Agent/CustomPropertyAgent.h"
#include "Agent/AgentArrayAccessTest.h"
#include "Agent/PropertyReadonlyAgent.h"
#include "Agent/PreconEffectorAgent.h"

#include "BehaviacWorkspace.h"

#include "behaviac_generated/types/behaviac_types.h"

void RegisterTypes();
void UnRegisterTypes();

namespace loadtest
{
    typedef void(*LoadTest)(behaviac::Workspace::EFileFormat);

    class LoadTestSuite
    {
    private:
        typedef std::pair<std::string, LoadTest> RegisteredLoadTest;
        typedef std::vector<RegisteredLoadTest> RegisteredLoadTests;

    public:
        LoadTestSuite() : m_verbose(false)
        {
        }

        void registerLoadTest(const std::string& testName, LoadTest loadtest)
        {
            m_loadtests.push_back(RegisteredLoadTest(testName, loadtest));
        }

        void setVerbose(bool b)
        {
            m_verbose = b;
        }

        void runAllLoadTests(behaviac::Workspace::EFileFormat format)
        {
            behaviac::Workspace::GetInstance()->SetFileFormat(format);

            for (RegisteredLoadTests::iterator loadtest = m_loadtests.begin(); loadtest != m_loadtests.end(); ++loadtest)
            {
                if (m_verbose)
                {
                    std::cout << loadtest->first << " " << std::flush;
                }

                LoadTest pLoadTestFn = loadtest->second;
                pLoadTestFn(format);

                if (m_verbose)
                {
                    std::cout << "PASS" << std::endl << std::flush;

                }
                else
                {
                    std::cout << "." << std::flush;
                }
            }

            if (!m_verbose)
            {
                std::cout << std::endl << std::flush;
            }
        }

        static LoadTestSuite& getInstance()
        {
            static LoadTestSuite instance;
            return instance;
        }

    private:
        bool m_verbose;
        RegisteredLoadTests m_loadtests;
    };

    class LoadTestRegister
    {
    public:
        LoadTestRegister(const std::string& name, LoadTest loadtest)
        {
            LoadTestSuite::getInstance().registerLoadTest(name, loadtest);
        }
    };
}

#define LOAD_TEST(SUITENAME, TESTNAME)                                      \
    void SUITENAME##_##TESTNAME(behaviac::Workspace::EFileFormat format);   \
    static loadtest::LoadTestRegister loadTestRegister_##SUITENAME##_##TESTNAME(#SUITENAME "_" #TESTNAME, SUITENAME##_##TESTNAME); \
    void SUITENAME##_##TESTNAME(behaviac::Workspace::EFileFormat format)

class AgentNodeTest;
class EmployeeParTestAgent;

extern void registerAllTypes();
extern void unregisterAllTypes();
extern AgentNodeTest* initTestEnvNode(const char* treePath, behaviac::Workspace::EFileFormat format);
extern void finlTestEnvNode(AgentNodeTest* testAgent);

extern EmployeeParTestAgent* initTestEnvPar(const char* treePath, behaviac::Workspace::EFileFormat format);
extern void finlTestEnvPar(EmployeeParTestAgent* testAgent);

#if BEHAVIAC_CCDEFINE_MSVC
#define CHECK_LESS(E, A)									\
    if (!((E) < (A)))											\
    {															\
        std::ostringstream os;									\
        os << "(" << #E << " == " << #A << ") ";				\
        os << "E:" << (E) << " actual:" << (A) << " ";			\
        os << __FILE__ << ":" << __LINE__;						\
        throw test::TestFailedException(os.str());				\
    }

#define CHECK_NOT_EQUAL(E, A)                                     \
    if (!((E) != (A)))                                        \
    {                                                         \
        std::ostringstream os;                                \
        os << "(" << #E << " == " << #A << ") ";              \
        os << "E:" << (E) << " actual:" << (A) << " "; \
        os << __FILE__ << ":" << __LINE__;                    \
        throw test::TestFailedException(os.str());            \
    }

#define CHECK_STR_EQUAL(E, A)                               \
    if (!(strcmp(E, A) == 0))                                   \
    {															\
        std::ostringstream os;									\
        os << "(" << #E << " == " << #A << ") ";				\
        os << "E:" << (E) << " actual:" << (A) << " ";			\
        os << __FILE__ << ":" << __LINE__;						\
        throw test::TestFailedException(os.str());				\
    }
#else
#define CHECK_LESS(E, A)									\
    if (!((E) < (A)))											\
    {															\
        std::ostringstream os;									\
        os << "(" << #E << " == " << #A << ") ";				\
        os << "E:" << (E) << " actual:" << (A) << " ";			\
        os << __FILE__ << ":" << __LINE__;						\
        BEHAVIAC_ASSERT(false);									\
    }

#define CHECK_NOT_EQUAL(E, A)                                     \
    if (!((E) != (A)))                                        \
    {                                                         \
        std::ostringstream os;                                \
        os << "(" << #E << " == " << #A << ") ";              \
        os << "E:" << (E) << " actual:" << (A) << " "; \
        os << __FILE__ << ":" << __LINE__;                    \
        BEHAVIAC_ASSERT(false);									\
    }
#define CHECK_STR_EQUAL(E, A)                               \
    if (!(strcmp(E, A) == 0))                                   \
    {															\
        std::ostringstream os;									\
        os << "(" << #E << " == " << #A << ") ";				\
        os << "E:" << (E) << " actual:" << (A) << " ";			\
        os << __FILE__ << ":" << __LINE__;						\
        BEHAVIAC_ASSERT(false);									\
    }
#endif
