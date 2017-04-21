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
#include <iostream>

#if BEHAVIAC_CCDEFINE_ANDROID
#include <android/log.h>

#define LOGI(...) ((void)__android_log_print(ANDROID_LOG_INFO, "demo_running", __VA_ARGS__))
#else
#define LOGI printf
#endif

CBTPlayer::CBTPlayer()
{
    //SetVariable<int>("CurStep", 0);
    m_iBaseSpeed = 1;
    m_Frames = 0;
}

CBTPlayer::~CBTPlayer()
{
}

bool CBTPlayer::Condition()
{
    m_Frames = 0;
    LOGI("\tCondition\n");
    return true;
}

behaviac::EBTStatus CBTPlayer::Action1()
{
	LOGI("\tAction1\n");

    return behaviac::BT_SUCCESS;
}

behaviac::EBTStatus CBTPlayer::Action3()
{
	LOGI("\tAction3\n");

    m_Frames++;

    if (m_Frames == 3)
    {
        return behaviac::BT_SUCCESS;
    }

    return behaviac::BT_RUNNING;
}
