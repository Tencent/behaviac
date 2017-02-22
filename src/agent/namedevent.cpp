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

#include "behaviac/agent/agent.h"
//#include "behaviac/property/property.h"

//void CNamedEvent::run(const behaviac::CTagObject* parent, const behaviac::CTagObject* parHolder)
//{
//	BEHAVIAC_UNUSED_VAR(parent);
//	BEHAVIAC_UNUSED_VAR(parHolder);
//	//const behaviac::Agent* pAgent = behaviac::Agent::DynamicCast(parent);
//	//if (pAgent)
//	//{
//	//	const CNamedEvent* pE = pAgent->FindEvent(this->GetName(), this->GetClassNameString());
//
//	//	if (pE && pE->IsFired())
//	//	{
//	//		*(AsyncValue<bool>*)this->m_return = true;
//	//	}
//	//}
//}
//
//void CNamedEvent::SetFired(behaviac::Agent* pAgent, bool bFired)
//{
//	this->m_bFired = bFired;
//
//	if (bFired && pAgent)
//	{
//		pAgent->btonevent(this->m_propertyName);
//	}
//}
