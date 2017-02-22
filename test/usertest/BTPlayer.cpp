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

#include "BTPlayer.h"
#include <time.h>
#include <iostream>

using namespace std;
using namespace behaviac;

extern bool g_bMovePrint;
extern unsigned int g_uiRunCount;

CBTPlayer::CBTPlayer()
{
    //SetVariable<int>("CurStep", 0);
    m_iBaseSpeed = 1;
    m_Frames = 0;
    m_iX = 0;
    m_iY = 0;
}

CBTPlayer::~CBTPlayer()
{
}

time_t CBTPlayer::GetCurTime()
{
    return time(NULL);
}

void CBTPlayer::MoveAhead(int speed)
{
    m_iX += (m_iBaseSpeed + speed);

    SetVariable<int>("CurStep", m_iX);

    int iVal = GetVariable<int>("CurStep");

    if (g_bMovePrint)
    {
        cout << "[" << GetCurTime() << "] Name:[" << GetName() << "] MoveAhead CurStep:" << iVal << " CurSpeed:" << speed << endl;
    }

    g_uiRunCount ++;
}

void CBTPlayer::MoveBack(int speed)
{
    m_iX -= (m_iBaseSpeed + speed);

    SetVariable<int>("CurStep", m_iX);

    int iVal = GetVariable<int>("CurStep");

    if (g_bMovePrint)
    {
        cout << "[" << GetCurTime() << "] Name:[" << GetName() << "] MoveBack CurStep:" << iVal << " CurSpeed:" << speed  << endl;
    }

    g_uiRunCount ++;
}

bool CBTPlayer::Condition()
{
    m_Frames = 0;
    cout << "\tCondition\n";
    return true;
}

behaviac::EBTStatus CBTPlayer::Action1()
{
    cout << "\tAction1\n";

    return behaviac::BT_SUCCESS;
}

behaviac::EBTStatus CBTPlayer::Action3()
{
    cout << "\tAction3\n";

    m_Frames++;

    if (m_Frames == 3)
    {
        return behaviac::BT_SUCCESS;
    }

    return behaviac::BT_RUNNING;
}
