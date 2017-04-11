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

//#include "test.h"
#include "behaviac/common/base.h"
#include "behaviac/common/profiler/profiler.h"
#include "behaviac/agent/agent.h"
#include "behaviac/agent/registermacros.h"
#include "BehaviacWorkspace.h"
#include "btperformance.h"

#if BEHAVIAC_CCDEFINE_MSVC
#include <windows.h>
#endif

CPerformanceAgent::CPerformanceAgent()
{
    this->Clear();
}

void CPerformanceAgent::Clear()
{
    DistanceToEnemy = 100;
    HP = 100;

    Hungry = 0;
    Food = 0;

    m_internal = 0.0f;
}

#pragma  optimize("", off)

static void FakeSleep(int c)
{
    const int kCount = 500 * 500 * c;
    int result = 0;

    for (int i = 0; i < kCount; ++i) {
        result += i;

        if (result > 10000000) {
            result = 0;
        }
    }
}

//#define TEST_LOGINFO BEHAVIAC_LOGINFO
#define TEST_LOGINFO

behaviac::EBTStatus CPerformanceAgent::RunAway()
{
    BEHAVIAC_PROFILE("FunctionCall");

    Hungry += 0.015f;
    DistanceToEnemy += 0.5f;

    TEST_LOGINFO("RunAway HP=%f DistanceToEnemy=%f Hungry=%f Food=%f\n", HP, DistanceToEnemy, Hungry, Food);
    FakeSleep(1);
    return behaviac::BT_RUNNING;
}

void CPerformanceAgent::Fire()
{
    BEHAVIAC_PROFILE("FunctionCall");

    HP *= 0.3f;

    if (DistanceToEnemy > 3.0f) {
        DistanceToEnemy -= 0.01f;
    }

    TEST_LOGINFO("Fire HP=%f DistanceToEnemy=%f Hungry=%f Food=%f\n", HP, DistanceToEnemy, Hungry, Food);
    FakeSleep(1);
}

behaviac::EBTStatus CPerformanceAgent::SearchForFood()
{
    BEHAVIAC_PROFILE("FunctionCall");

    Food += 0.002f;

    TEST_LOGINFO("SearchForFood HP=%f DistanceToEnemy=%f Hungry=%f Food=%f\n", HP, DistanceToEnemy, Hungry, Food);
    FakeSleep(1);

    if (Food >= 3.0f) {
        return behaviac::BT_SUCCESS;
    }

    return behaviac::BT_RUNNING;
}

behaviac::EBTStatus CPerformanceAgent::Eat()
{
    BEHAVIAC_PROFILE("FunctionCall");

    Hungry -= 0.04f;
    HP += 0.01f;

    TEST_LOGINFO("Eat HP=%f DistanceToEnemy=%f Hungry=%f Food=%f\n", HP, DistanceToEnemy, Hungry, Food);
    FakeSleep(1);

    if (Hungry <= 0.0f) {
        Hungry = 0.0f;
        Food--;

        return behaviac::BT_SUCCESS;
    }

    return behaviac::BT_RUNNING;
}

behaviac::EBTStatus CPerformanceAgent::Wander()
{
    BEHAVIAC_PROFILE("FunctionCall");

    m_internal += 0.03f;
    DistanceToEnemy -= 0.01f;
    HP -= 0.02f;

    TEST_LOGINFO("Wander HP=%f DistanceToEnemy=%f Hungry=%f Food=%f\n", HP, DistanceToEnemy, Hungry, Food);
    FakeSleep(1);

    if (m_internal > 21.0f) {
        Hungry += m_internal;
        m_internal = 0.0f;
        return behaviac::BT_SUCCESS;
    }

    return behaviac::BT_RUNNING;
}

behaviac::EBTStatus CPerformanceAgent::Fidget()
{
    BEHAVIAC_PROFILE("FunctionCall");

    float smallDistance = this->GetVariable<float>("par_SmallDisance");

    if (DistanceToEnemy > smallDistance) {
        DistanceToEnemy -= smallDistance;
    }

    TEST_LOGINFO("Fidget HP=%f DistanceToEnemy=%f Hungry=%f Food=%f\n", HP, DistanceToEnemy, Hungry, Food);
    return behaviac::BT_SUCCESS;
}
#pragma  optimize("", on)

#include "behaviac_generated/behaviors/behaviac_generated_behaviors.h"

class CommandLineParameterParser
{
    int				m_argc;
    const char**	m_argv;
public:
    CommandLineParameterParser(int argc, char** argv) : m_argc(argc), m_argv((const char**)argv) {
    }

    bool ParameterExist(const char* szParam) {
        bool bMatch = false;

        for (int i = 0; i < m_argc; ++i) {
            if (strcmp(m_argv[i], szParam) == 0) {
                bMatch = true;
                break;
            }
        }

        return bMatch;
    }

    int ParameterEqualExist(const char* szParam) {
        int countAgents = 1;

        for (int i = 0; i < m_argc; ++i) {
            if (const char* p = strstr(m_argv[i], szParam)) {
                p += strlen(szParam);
                countAgents = atoi(p);
                break;
            }
        }

        return countAgents == 0 ? 1 : countAgents;
    }
};

void RegisterTypes();
void UnRegisterTypes();

void btagenttick(behaviac::Workspace::EFileFormat format, int countAgents);

static void SetExePath()
{
#if BEHAVIAC_CCDEFINE_MSVC
    TCHAR szCurPath[_MAX_PATH];

    GetModuleFileName(NULL, szCurPath, _MAX_PATH);

    char* p = szCurPath;

    while (strchr(p, '\\')) {
        p = strchr(p, '\\');
        p++;
    }

    *p = '\0';

    SetCurrentDirectory(szCurPath);
#endif
}

int main(int argc, char** argv)
{
    SetExePath();

    CommandLineParameterParser CLPP(argc, argv);
    //if to wait for the key to end
    bool bWait = CLPP.ParameterExist("-wait");
	BEHAVIAC_UNUSED_VAR(bWait);

    int countAgents = CLPP.ParameterEqualExist("-agents=");

    behaviac::Workspace::EFileFormat format = behaviac::Workspace::EFF_xml;

    bool bXml = CLPP.ParameterExist("-xml");

    if (bXml) {
        format = behaviac::Workspace::EFF_xml;
    }

    bool bCpp = CLPP.ParameterExist("-cpp");

    if (bCpp) {
        format = behaviac::Workspace::EFF_cpp;
    }

    bool bBson = CLPP.ParameterExist("-bson");

    if (bBson) {
        format = behaviac::Workspace::EFF_bson;
    }

    //behaviac::Socket::SetupConnection(false);

    btagenttick(format, countAgents);

    //behaviac::Socket::ShutdownConnection();

	printf("Press any key to continue...");
	int ret = getchar();
	BEHAVIAC_UNUSED_VAR(ret);

    return 0;
}

struct AgentItem_t {
    CPerformanceAgent*	pA;
    bool				bGo;

	AgentItem_t() : pA(0), bGo(true) {
    }
};

void MyMethod(int countAgents, AgentItem_t* agents)
{
    bool bLoop = true;

    while (bLoop) {
        int c = 0;

        for (int i = 0; i < countAgents; ++i) {
            if (agents[i].bGo) {
                behaviac::EBTStatus s = agents[i].pA->btexec();

                if (s != behaviac::BT_RUNNING) {
                    agents[i].bGo = false;
                    c++;
                }
            } else {
                c++;
            }
        }

        if (countAgents == c) {
            bLoop = false;
        }
    }
}

void btagenttick(behaviac::Workspace::EFileFormat format, int countAgents)
{
    const char* strFormat = "xml";

    if (format == behaviac::Workspace::EFF_xml) {
        strFormat = "xml";

    } else if (format == behaviac::Workspace::EFF_cpp) {
        strFormat = "cpp";
    }

    if (format == behaviac::Workspace::EFF_bson) {
        strFormat = "bson";
    }

    printf("\nAgents %d Format %s\n", countAgents, strFormat);
    BEHAVIAC_LOGINFO("\nAgents %d Format %s\n", countAgents, strFormat);

	behaviac::Workspace::GetInstance()->SetFilePath("../integration/unity_performance/Assets/Resources/behaviac/exported");
	behaviac::Workspace::GetInstance()->SetFileFormat(format);

    behaviac::Config::SetLogging(false);
	behaviac::Config::SetLoggingFlush(false);
    behaviac::Config::SetSocketing(false);
    behaviac::Config::SetProfiling(false);

    AgentItem_t* agents = BEHAVIAC_NEW AgentItem_t[countAgents];

    behaviac::Agent::SetIdMask(0xffffffff);

    for (int i = 0; i < countAgents; ++i) {
        agents[i].pA = behaviac::Agent::Create<CPerformanceAgent>();

        agents[i].pA->SetIdFlag(0);

        agents[i].pA->btload("performance/Performance");
        agents[i].pA->btsetcurrent("performance/Performance");
    }

    //run once to warm the oven
    MyMethod(countAgents, agents);

    behaviac::Config::SetProfiling(true);
    behaviac::Profiler::GetInstance()->SetOutputDebugBlock(true);
    behaviac::Profiler::GetInstance()->SetHierarchy(false);

    for (int i = 0; i < countAgents; ++i) {
        agents[i].bGo = true;
        agents[i].pA->Clear();
        agents[i].pA->btresetcurrent();
    }

    behaviac::Profiler::GetInstance()->BeginFrame();

    MyMethod(countAgents, agents);

    behaviac::Profiler::GetInstance()->EndFrame();

    const behaviac::string profile_data = behaviac::Profiler::GetInstance()->GetData(true, false);

    behaviac::string profile_data_m = "\n";
    profile_data_m += profile_data;
    profile_data_m += "\n";

    //BEHAVIAC_LOGINFO("\n%s\n", profile_data_m.c_str());
	behaviac::CLogger::PrintLines(BEHAVIAC_LOG_INFO, profile_data_m.c_str());

    behaviac::LogManager::GetInstance()->Flush(0);

    BEHAVIAC_DELETE(agents);

    printf("\ndone\n");
}
