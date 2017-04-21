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

#include "BTPlayer.h"

#if BEHAVIAC_CCDEFINE_ANDROID
#include <android/log.h>

#define LOGI(...) ((void)__android_log_print(ANDROID_LOG_INFO, "tutorial_3", __VA_ARGS__))
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

CBTPlayer* g_player = NULL;

bool InitBehavic(behaviac::Workspace::EFileFormat ff, 
	const char* szFilePath = "../test/demo_running/behaviac/exported", 
	const char* szExportMetaFile = "../test/demo_running/behaviac/demo_running.xml")
{
	LOGI("InitBehavic\n");

    //behaviac::Config::SetSocketing(false);
#if !BEHAVIAC_CCDEFINE_MSVC
    behaviac::Config::SetHotReload(false);
#endif
    //behaviac::Config::SetSocketBlocking(false);
    //behaviac::Config::SetSocketPort(60636);

    behaviac::Workspace::GetInstance()->SetFilePath(szFilePath);
    behaviac::Workspace::GetInstance()->SetFileFormat(ff);

    //behaviac::Agent::SetIdMask(kIdMask_Wolrd | kIdMask_Opponent);

    return true;
}

bool InitPlayer(const char* pszTreeName)
{
	LOGI("InitPlayer\n");
    g_player = behaviac::Agent::Create<CBTPlayer>();

    bool bRet = false;
    bRet = g_player->btload(pszTreeName);
    BEHAVIAC_ASSERT(bRet);

    g_player->btsetcurrent(pszTreeName);

    return bRet;
}

void UpdateLoop()
{
	int frames = 0;
	behaviac::EBTStatus status = behaviac::BT_RUNNING;

	LOGI("UpdateLoop\n");

	while (status == behaviac::BT_RUNNING)
	{
		LOGI("frame %d\n", ++frames);

		status = g_player->btexec();
	}
}

void CleanupPlayer()
{
	LOGI("CleanupPlayer\n");
    behaviac::Agent::Destroy(g_player);
}

void CleanupBehaviac()
{
	LOGI("CleanupBehaviac\n");

	behaviac::Workspace::GetInstance()->Cleanup();
}

void Run()
{
	const char* szTreeName = "demo_running";

	LOGI("bt: %s\n\n", szTreeName);

	behaviac::Workspace::EFileFormat ff = behaviac::Workspace::EFF_xml;

	InitBehavic(ff);
	InitPlayer(szTreeName);

	UpdateLoop();

	CleanupPlayer();
	CleanupBehaviac();
}

#if !BEHAVIAC_CCDEFINE_ANDROID
//cmdline: behaviorTreePath Count ifprint fileformat
int main(int argc, char** argv)
{
	BEHAVIAC_UNUSED_VAR(argc);
	BEHAVIAC_UNUSED_VAR(argv);

    SetExePath();

	LOGI("BEHAVIAC_CCDEFINE_NAME=%s\n", BEHAVIAC_CCDEFINE_NAME);

	Run();

	printf("Press any key to continue...");
	int ret = getchar();
	BEHAVIAC_UNUSED_VAR(ret);

    return 0;
}

#endif