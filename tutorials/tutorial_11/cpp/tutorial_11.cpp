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

#include "behaviac_generated/types/behaviac_types.h"

#if BEHAVIAC_CCDEFINE_ANDROID
#include <android/log.h>
#include <jni.h>

#if (BEHAVIAC_CCDEFINE_ANDROID_VER > 8)
#include <android/asset_manager.h>
#include <android/asset_manager_jni.h>
#endif

//#define LOGI(...) ((void)__android_log_print(ANDROID_LOG_INFO, "tutorial_11", __VA_ARGS__))
#define LOGI(...)
#else
#define LOGI printf

#if BEHAVIAC_CCDEFINE_MSVC
#include <windows.h>
#include <tchar.h>
#endif

#endif

#if !BEHAVIAC_CCDEFINE_ANDROID
static void SetExePath()
{
#if BEHAVIAC_CCDEFINE_MSVC
	TCHAR szCurPath[_MAX_PATH];

	GetModuleFileName(NULL, szCurPath, _MAX_PATH);

	TCHAR* p = szCurPath;

	while (_tcschr(p, L'\\'))
	{
		p = _tcschr(p, L'\\');
		p++;
	}

	*p = L'\0';

	SetCurrentDirectory(szCurPath);
#endif
}
#endif

FirstAgent* g_FirstAgent = NULL;

const char* InitBehavic()
{
	LOGI("InitBehavic\n");

#if !BEHAVIAC_CCDEFINE_ANDROID
	behaviac::Workspace::GetInstance()->SetFilePath("../tutorials/tutorial_11/cpp/exported");
#else
    behaviac::Workspace::GetInstance()->SetFilePath("assets:/behaviac/exported");
#endif

    behaviac::Workspace::GetInstance()->SetFileFormat(behaviac::Workspace::EFF_xml);

    return "InitBehavic\n";
}

const char* InitPlayer()
{
	LOGI("InitPlayer\n");

	g_FirstAgent = behaviac::Agent::Create<FirstAgent>();

	g_FirstAgent->btload("FirstBT");

	g_FirstAgent->btsetcurrent("FirstBT");

    return "InitPlayer\n";
}

std::string UpdateLoop()
{
	LOGI("UpdateLoop\n");

	int frames = 0;
	behaviac::EBTStatus status = behaviac::BT_RUNNING;

	while (status == behaviac::BT_RUNNING)
	{
		LOGI("frame %d\n", ++frames);

		status = g_FirstAgent->btexec();
	}

	std::string ret = "UpdateLoop\n";
	return ret + g_FirstAgent->GetSayContent().c_str();
}

const char* CleanupPlayer()
{
	LOGI("CleanupPlayer\n");

	behaviac::Agent::Destroy(g_FirstAgent);

    return "CleanupPlayer\n";
}

const char* CleanupBehaviac()
{
	LOGI("CleanupBehaviac\n");

	behaviac::Workspace::GetInstance()->Cleanup();

    return "CleanupBehaviac\n";
}

#if !BEHAVIAC_CCDEFINE_ANDROID
int main(int argc, char** argv)
{
	BEHAVIAC_UNUSED_VAR(argc);
	BEHAVIAC_UNUSED_VAR(argv);

    SetExePath();

	LOGI("BEHAVIAC_CCDEFINE_NAME=%s\n", BEHAVIAC_CCDEFINE_NAME);

    InitBehavic();

    InitPlayer();

    UpdateLoop();

    CleanupPlayer();

    CleanupBehaviac();

	printf("Press any key to continue...");
	int ret = getchar();
	BEHAVIAC_UNUSED_VAR(ret);

    return 0;
}
#else
std::string TestBehaviac()
{
    LOGI("BEHAVIAC_CCDEFINE_NAME=%s\n", BEHAVIAC_CCDEFINE_NAME);

    std::string retStr;

    retStr += InitBehavic();

    retStr += InitPlayer();

    retStr += UpdateLoop();

    retStr += CleanupPlayer();

    retStr += CleanupBehaviac();

    return  retStr;
}

extern "C"
JNIEXPORT jstring JNICALL
Java_com_tencent_behaviac_behaviac_1android_MainActivity_TestMain(JNIEnv* env, jclass cls, jobject assetManager)
{
    AAssetManager* mgr = AAssetManager_fromJava(env, assetManager);
    BEHAVIAC_ASSERT(mgr);

    behaviac::CFileManager::GetInstance()->SetAssetManager(mgr);

    std::string str = TestBehaviac();

    return env->NewStringUTF(str.c_str());
}

#endif
