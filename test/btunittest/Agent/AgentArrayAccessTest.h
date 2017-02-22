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

#ifndef BTUNITEST_AGENTARRAYACCESSTEST_H_
#define BTUNITEST_AGENTARRAYACCESSTEST_H_

#include "behaviac/common/base.h"
#include "behaviac/agent/agent.h"
#include "behaviac/agent/registermacros.h"

namespace TestNS
{
	class AgentArrayAccessTest : public behaviac::Agent
	{
	public:
		AgentArrayAccessTest();
		virtual ~AgentArrayAccessTest();

		BEHAVIAC_DECLARE_AGENTTYPE(TestNS::AgentArrayAccessTest, behaviac::Agent);

		void resetProperties()
		{
			ListInts.push_back(1);
			ListInts.push_back(2);
			ListInts.push_back(3);
			ListInts.push_back(4);
			ListInts.push_back(5);
			Int = 0;
		}

		void init()
		{
			//base.Init();
			resetProperties();
		}

		void finl()
		{
		}

		behaviac::vector<int> ListInts;

		int Int;
	};
}

#endif//BTUNITEST_AGENTARRAYACCESSTEST_H_
