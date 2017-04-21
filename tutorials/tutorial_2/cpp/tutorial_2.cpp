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

#define LOGI(...) ((void)__android_log_print(ANDROID_LOG_INFO, "tutorial_2", __VA_ARGS__))
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

bool InitBehavic()
{
	LOGI("InitBehavic\n");

	behaviac::Workspace::GetInstance()->SetFilePath("../tutorials/tutorial_2/cpp/exported");

	behaviac::Workspace::GetInstance()->SetFileFormat(behaviac::Workspace::EFF_xml);

    return true;
}

bool InitPlayer(const char* btName)
{
	LOGI("InitPlayer : %s\n", btName);

	g_FirstAgent = behaviac::Agent::Create<FirstAgent>();

	bool bRet = g_FirstAgent->btload(btName);

	g_FirstAgent->btsetcurrent(btName);

    return bRet;
}

void UpdateLoop()
{
	LOGI("UpdateLoop\n");

	int frames = 0;
	behaviac::EBTStatus status = behaviac::BT_RUNNING;

	while (status == behaviac::BT_RUNNING)
	{
		LOGI("frame %d\n", ++frames);

		status = g_FirstAgent->btexec();
	}
}

void CleanupPlayer()
{
	LOGI("CleanupPlayer\n");

	behaviac::Agent::Destroy(g_FirstAgent);
}

void CleanupBehaviac()
{
	LOGI("CleanupBehaviac\n");

	behaviac::Workspace::GetInstance()->Cleanup();
}

#if !BEHAVIAC_CCDEFINE_ANDROID
int main(int argc, char** argv)
{
	BEHAVIAC_UNUSED_VAR(argc);
	BEHAVIAC_UNUSED_VAR(argv);

    SetExePath();

	LOGI("BEHAVIAC_CCDEFINE_NAME=%s\n", BEHAVIAC_CCDEFINE_NAME);

    InitBehavic();

	LOGI("\nInput 1 : LoopBT    2 : SequenceBT    3 : SelectBT    Other Number : Exit\n\n");

	for (int input_key = getchar(); input_key > (int)'0' && input_key < (int)'4';)
	{
		bool bInit = false;

		switch (input_key)
		{
		case '1':
			bInit = InitPlayer("LoopBT");
			break;

		case '2':
			bInit = InitPlayer("SequenceBT");
			break;

		case '3':
			bInit = InitPlayer("SelectBT");
			break;
		}

		if (bInit)
		{
			UpdateLoop();

			CleanupPlayer();
		}

		LOGI("\nInput 1 : LoopBT    2 : SequenceBT    3 : SelectBT    Other Number : Exit\n\n");

		input_key = getchar(); // ignore one
		input_key = getchar();
	}

    CleanupBehaviac();

	printf("Press any key to continue...");
	int ret = getchar();
	BEHAVIAC_UNUSED_VAR(ret);

    return 0;
}

#endif
