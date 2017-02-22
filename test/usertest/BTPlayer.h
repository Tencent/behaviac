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

/**
 * @file  BTPlayer.h
 * @author  erictu@tencent.com
 * @date  2015-10-12 15:53:28
 *
 **/

#ifndef _BTPLAYER_H_
#define _BTPLAYER_H_
#include "behaviac/behaviac.h"
#include <time.h>

using namespace behaviac;


class CBTPlayer: public Agent
{
public:
	BEHAVIAC_DECLARE_AGENTTYPE(CBTPlayer, Agent);


    CBTPlayer();
    virtual ~CBTPlayer();

    time_t GetCurTime();

    void MoveAhead(int speed);
    void MoveBack(int speed);

    bool Condition();
    behaviac::EBTStatus Action1();
    EBTStatus Action3();

private:
    int                 m_iX;
    int                 m_iY;
    unsigned int        m_iBaseSpeed;

    int					m_Frames;
};

#endif  ///_BTPLAYER_H_

