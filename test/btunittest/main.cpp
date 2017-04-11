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

#include "test.h"
#include "behaviac/behaviac.h"
#include "BehaviacWorkspace.h"
#include "btloadtestsuite.h"

#if BEHAVIAC_CCDEFINE_MSVC
#include <windows.h>
#endif

using namespace test;

class CommandLineParameterParser
{
    int				m_argc;
    const char**	m_argv;
public:
    CommandLineParameterParser(int argc, char** argv) : m_argc(argc), m_argv((const char**)argv)
    {
    }

    bool ParameterExist(const char* szParam)
    {
        bool bMatch = false;

        for (int i = 0; i < m_argc; ++i)
        {
            if (strcmp(m_argv[i], szParam) == 0)
            {
                bMatch = true;
                break;
            }
        }

        return bMatch;
    }
};


#if ENABLE_MEMORY_LEAKTEST
void memory_leak_test(behaviac::Workspace::EFileFormat format);
#endif//

int my_main(bool bVerbose)
{
#if !ENABLE_MEMORY_LEAKTEST
    TestSuite& testSuite = TestSuite::getInstance();

    testSuite.setVerbose(bVerbose);

    std::cout << "UNIT TEST:" << std::endl;

#if !BEHAVIAC_CCDEFINE_MSVC
	behaviac::Config::SetHotReload(false);
#endif
#endif

    behaviac::Workspace::GetInstance()->SetFilePath("../test/btunittest/BehaviacData/exported");
#if ENABLE_MEMORY_LEAKTEST
	memory_leak_test(behaviac::Workspace::EFF_xml);

	return 0;
#else
    bool allPassed = testSuite.runAllTests();

    if (allPassed)
    {
        std::cout << std::endl << "ALL TESTS SUCCEED." << std::endl << std::flush;
    }

    return allPassed ? 0 : 1;
#endif
}

#if BEHAVIAC_CCDEFINE_ANDROID
#include <jni.h>
#include <android/log.h>
#include "behaviac/common/file/filemanager.h"

#if BEHAVIAC_CCDEFINE_ANDROID && (BEHAVIAC_CCDEFINE_ANDROID_VER > 8)
#include <android/asset_manager.h>
#include <android/asset_manager_jni.h>
#endif

#undef BEHAVIAC_ASSERT
#define BEHAVIAC_ASSERT(e) if (!(e)) { BEHAVIAC_LOGERROR("assert failed"); (*(int*)0) = 0; }

#undef CHECK_EQUAL
#define CHECK_EQUAL(E, A)                                     \
    if (!((E) == (A)))                                        \
    {                                                         \
        BEHAVIAC_LOGERROR("CHECK_EQUAL:%s:%d", __FILE__, __LINE__);        \
        BEHAVIAC_ASSERT(0);									  \
    }

void btunitest_game(behaviac::Workspace::EFileFormat format);

extern "C" JNIEXPORT void JNICALL Java_com_tencent_tag_behaviac_MainActivity_btutmain(JNIEnv* env,
        jclass tis, jobject assetManager)
{
    BEHAVIAC_UNUSED_VAR(env);
    BEHAVIAC_UNUSED_VAR(tis);
    BEHAVIAC_LOGINFO("btutmain begin");
#if BEHAVIAC_CCDEFINE_ANDROID && (BEHAVIAC_CCDEFINE_ANDROID_VER > 8)
    AAssetManager* mgr = AAssetManager_fromJava(env, assetManager);
    BEHAVIAC_ASSERT(mgr);

    behaviac::CFileManager::GetInstance()->SetAssetManager(mgr);
#endif

    btunitest_game(behaviac::Workspace::EFF_xml);

    //my_main();
    BEHAVIAC_LOGINFO("btutmain end");
}
#endif//BEHAVIAC_CCDEFINE_ANDROID

static void SetExePath()
{
#if BEHAVIAC_CCDEFINE_MSVC
    TCHAR szCurPath[_MAX_PATH];

    GetModuleFileName(NULL, szCurPath, _MAX_PATH);

    char* p = szCurPath;

    while (strchr(p, '\\'))
    {
        p = strchr(p, '\\');
        p++;
    }

    *p = '\0';

    SetCurrentDirectory(szCurPath);

	printf("Working Directory: %s\n", szCurPath);
#endif
}

int main(int argc, char** argv)
{
    // int BEHAVIAC_UNIQUE_NAME(intVar) = 0;
    // float BEHAVIAC_UNIQUE_NAME(floatVar) = 0.0f;
    SetExePath();

	printf("BEHAVIAC_CCDEFINE_NAME=%s\n", BEHAVIAC_CCDEFINE_NAME);

    CommandLineParameterParser CLPP(argc, argv);
    //if to wait for the key to end
    bool bWait = CLPP.ParameterExist("-wait");
	BEHAVIAC_UNUSED_VAR(bWait);

    //CConfig::GetInstance()->LoadConfig("setting.xml");

    //if (!bWait)
    //{
    //	const char* pWait = CConfig::Get("settings", "Wait");
    //	if (pWait && strcmp(pWait, "1") == 0)
    //	{
    //		bWait = true;
    //	}
    //}

    //CTimer::GetInstance()->Init();
#if ENABLE_MEMORYDUMP
    _CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
    //_CrtDumpMemoryLeaks();

    //_crtBreakAlloc
    //_lRequestCurr
    static long s_breakalloc = -1;
    _CrtSetBreakAlloc(s_breakalloc);
#endif

	int result = my_main(false);

	printf("Press any key to continue...");
	int ret = getchar();
	BEHAVIAC_UNUSED_VAR(ret);

    return result;
}
