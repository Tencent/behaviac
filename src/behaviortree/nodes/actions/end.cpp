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

#include "behaviac/behaviortree/nodes/actions/end.h"

namespace behaviac {
	End::End() : m_endStatus(NULL), m_endOutside(false){
    }

	End::~End() {
		BEHAVIAC_DELETE(m_endStatus);
	}

    void End::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);

        for (propertie_const_iterator_t it = properties.begin(); it != properties.end(); ++it) {
            const property_t& p = (*it);

			if (StringUtils::StringEqual(p.name, "EndStatus")) {
				if (StringUtils::IsValidString(p.value)) {
					const char* pParenthesis = strchr(p.value, '(');

					if (pParenthesis == 0) {
						behaviac::string typeName;
						this->m_endStatus = AgentMeta::ParseProperty(p.value);
					}
					else {
						this->m_endStatus = AgentMeta::ParseMethod(p.value);
					}
				}
			}
			else if (StringUtils::StringEqual(p.name, "EndOutside")) {
				m_endOutside = StringUtils::StringEqual(p.value, "true");
			}
        }
    }

	EBTStatus End::GetStatus(Agent* pAgent) const {
		return m_endStatus ? *(EBTStatus*)m_endStatus->GetValue(pAgent) : BT_SUCCESS;
    }

	bool End::GetEndOutside() const {
		return m_endOutside;
	}

    BehaviorTask* End::createTask() const {
        EndTask* pTask = BEHAVIAC_NEW EndTask();
        return pTask;
    }

	EBTStatus EndTask::GetStatus(Agent* pAgent) const {
		const End* pEndNode = End::DynamicCast(this->GetNode());
		EBTStatus status = pEndNode ? pEndNode->GetStatus(pAgent) : BT_SUCCESS;
		BEHAVIAC_ASSERT(status == BT_SUCCESS || status == BT_FAILURE);
		return status;
	}

	bool EndTask::GetEndOutside() const {
		const End* pEndNode = End::DynamicCast(this->GetNode());
		return pEndNode ? pEndNode->GetEndOutside() : false;
	}

    bool EndTask::onenter(Agent* pAgent) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        return true;
    }

    void EndTask::onexit(Agent* pAgent, EBTStatus s) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(s);
    }

    EBTStatus EndTask::update(Agent* pAgent, EBTStatus childStatus) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(childStatus);

		BehaviorTreeTask* rootTask = NULL;
		if (!this->GetEndOutside()) {
			rootTask = this->GetRootTask();
		}
		else if (pAgent) {
			rootTask = pAgent->btgetcurrent();
		}

		if (rootTask) {
			rootTask->setEndStatus(this->GetStatus(pAgent));
		}

		return BT_RUNNING;
    }
}
