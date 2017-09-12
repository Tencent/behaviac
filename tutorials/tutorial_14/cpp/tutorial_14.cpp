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

#define LOGI(...) ((void)__android_log_print(ANDROID_LOG_INFO, "tutorial_14", __VA_ARGS__))
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

	behaviac::Workspace::GetInstance()->SetFilePath("../tutorials/tutorial_14/cpp/exported");

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

void ExecuteBT()
{
	behaviac::EBTStatus status = g_FirstAgent->btexec();

	const char* statusStr = "";

	switch (status)
	{
	case behaviac::BT_SUCCESS:
		statusStr = "BT_SUCCESS";
		break;

	case behaviac::BT_FAILURE:
		statusStr = "BT_FAILURE";
		break;

	case behaviac::BT_RUNNING:
		statusStr = "BT_RUNNING";
		break;

	case behaviac::BT_INVALID:
		statusStr = "BT_INVALID";
		break;
	}

	LOGI("ExecuteBT : %s\n", statusStr);
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

	LOGI("\nInput 1 : subtree1    2 : subtree2    3 : maintree1    4 : maintree2    Other Number : Exit\n\n");

	for (int input_key = getchar(); input_key > (int)'0' && input_key < (int)'5';)
	{
		bool bInit = false;

		switch (input_key)
		{
		case '1':
			bInit = InitPlayer("subtree1");
			break;

		case '2':
			bInit = InitPlayer("subtree2");
			break;

		case '3':
			bInit = InitPlayer("maintree1");
			break;

		case '4':
			bInit = InitPlayer("maintree2");
			break;
		}

		if (bInit)
		{
			ExecuteBT();

			CleanupPlayer();
		}

		LOGI("\nInput 1 : subtree1    2 : subtree2    3 : maintree1    4 : maintree2    Other Number : Exit\n\n");

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
