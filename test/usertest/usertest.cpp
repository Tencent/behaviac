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
#include "./behaviac/exported/behaviac_generated/types/behaviac_types.h"

#if BEHAVIAC_CCDEFINE_MSVC
#include <windows.h>
#include <tchar.h>
#endif

#include <time.h>
#include <iostream>

using namespace std;
using namespace behaviac;

CBTPlayer* g_player = NULL;
CBTPlayer* g_player1 = NULL;

bool g_bMovePrint = true;
unsigned int g_uiRunCount = 0;

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

bool InitBehavic(behaviac::Workspace::EFileFormat ff)
{
#if !BEHAVIAC_CCDEFINE_MSVC
	behaviac::Config::SetHotReload(false);
#endif
    //behaviac::Config::SetSocketBlocking(false);
    //behaviac::Config::SetSocketPort(8081);

    behaviac::Workspace::GetInstance()->SetFilePath("../test/usertest/behaviac/exported");
    behaviac::Workspace::GetInstance()->SetFileFormat(ff);

    //behaviac::Agent::SetIdMask(kIdMask_Wolrd | kIdMask_Opponent);

    return true;
}

bool InitPlayer(const char* pszTreeName)
{
    g_player = behaviac::Agent::Create<CBTPlayer>();
    g_player1 = behaviac::Agent::Create<CBTPlayer>("player1");

    bool bRet = false;
    bRet = g_player->btload(pszTreeName);
    assert(bRet);

    g_player->btsetcurrent(pszTreeName);

    bRet = g_player1->btload(pszTreeName);
    assert(bRet);

    g_player1->btsetcurrent(pszTreeName);

    return bRet;
}

void CleanupPlayer()
{
    behaviac::Agent::Destroy(g_player);
    behaviac::Agent::Destroy(g_player1);
}

void CleanupBehaviac()
{
	behaviac::Workspace::GetInstance()->Cleanup();
}

//cmdline: behaviorTreePath Count ifprint fileformat
int main(int argc, char** argv)
{
    SetExePath();

	printf("BEHAVIAC_CCDEFINE_NAME=%s\n", BEHAVIAC_CCDEFINE_NAME);

    cout << "usage: [btPath] [loopCout] [0|1] [xml|cpp|bson]\n\n";

	const char* szTreeName = "player";
	if (argc > 1)
	{
		szTreeName = argv[1];
	}

    int iCount = argc > 2 ? atoi(argv[2]) : 1000;
    g_bMovePrint = argc > 3 && atoi(argv[3]) != 0 ? true : false;

    const char* ffParam = argc > 4 ? argv[4] : "xml";

    behaviac::Workspace::EFileFormat ff = behaviac::Workspace::EFF_xml;

    if (behaviac::StringUtils::StringEqualNoCase(ffParam, "xml"))
    {
        ff = behaviac::Workspace::EFF_xml;
    }
    else if (behaviac::StringUtils::StringEqualNoCase(ffParam, "bson"))
    {
        ff = behaviac::Workspace::EFF_bson;
    }
    else if (behaviac::StringUtils::StringEqualNoCase(ffParam, "cpp"))
    {
        ff = behaviac::Workspace::EFF_cpp;
    }
    else
    {
        BEHAVIAC_ASSERT(false);
        cout << "unrecognized file format " << ffParam << std::endl;
		return -1;
    }

	cout << "bt:" << szTreeName << "\n";
	cout << "loop:" << iCount << "\n";
	cout << "format:" << ffParam << "\n" << "\n";

    InitBehavic(ff);
    InitPlayer(szTreeName);

    clock_t start = clock();

	int i = 0;

	while (i++ < iCount)
	{
		behaviac::Workspace::GetInstance()->Update();
	}

    clock_t finish = clock();

    if (iCount > 0)
    {
        float duration = (float)(finish - start) / CLOCKS_PER_SEC;

		cout << "format " << ffParam << " duration(seconds):" << duration << " RunCount:" << iCount << endl;
    }

    CleanupPlayer();
    CleanupBehaviac();

	printf("Press any key to continue...");
	int ret = getchar();
	BEHAVIAC_UNUSED_VAR(ret);

    return 0;
}
