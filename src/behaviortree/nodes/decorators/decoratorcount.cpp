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

#include "behaviac/common/base.h"
#include "behaviac/behaviortree/nodes/decorators/decoratorcount.h"
#include "behaviac/behaviortree/nodes/conditions/condition.h"
#include "behaviac/common/meta.h"

namespace behaviac {
    DecoratorCount::DecoratorCount() : m_count(0)
    {}

    DecoratorCount::~DecoratorCount() {
    }

    //Property* LoadRight(const char* value, const behaviac::string& propertyName, behaviac::string& typeName);

    void DecoratorCount::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);

        for (propertie_const_iterator_t it = properties.begin(); it != properties.end(); ++it) {
            const property_t& p = (*it);

			if (StringUtils::StringEqual(p.name, "Count")) {
                behaviac::string typeName;
                behaviac::string  propertyName;
                //this->m_count = Condition::LoadRight(p.value, typeName);
                this->m_count = AgentMeta::ParseProperty(p.value);
            }
        }
    }

    int DecoratorCount::GetCount(Agent* pAgent) const {
        if (this->m_count) {
            uint64_t count = *(uint64_t*)this->m_count->GetValue(pAgent);
			return (count == ((uint64_t)-1) ? -1 : (int)(count & 0x0000FFFF));
        }

        return 0;
    }

    DecoratorCountTask::DecoratorCountTask() : DecoratorTask(), m_n(0) {
    }

    DecoratorCountTask::~DecoratorCountTask() {
    }

    int DecoratorCountTask::GetCount(Agent* pAgent) const {
        BEHAVIAC_ASSERT(DecoratorCount::DynamicCast(this->GetNode()));
        const DecoratorCount* pDecoratorCountNode = (const DecoratorCount*)(this->GetNode());

        return pDecoratorCountNode ? pDecoratorCountNode->GetCount(pAgent) : 0;
    }

    void DecoratorCountTask::copyto(BehaviorTask* target) const {
        super::copyto(target);

        BEHAVIAC_ASSERT(DecoratorCountTask::DynamicCast(target));
        DecoratorCountTask* ttask = (DecoratorCountTask*)target;

        ttask->m_n = this->m_n;
    }

    void DecoratorCountTask::save(IIONode* node) const {
        super::save(node);

        if (this->m_status != BT_INVALID) {
            CIOID  countId("count");
            node->setAttr(countId, this->m_n);
        }
    }

    void DecoratorCountTask::load(IIONode* node) {
        super::load(node);

        if (this->m_status != BT_INVALID) {
            CIOID  countId("count");
            behaviac::string attrStr;
            node->getAttr(countId, attrStr);
            StringUtils::ParseString(attrStr.c_str(), this->m_n);
        }
    }

    bool DecoratorCountTask::onenter(Agent* pAgent) {
        super::onenter(pAgent);

        {
            int count = this->GetCount(pAgent);

            if (count == 0) {
                return false;
            }

            this->m_n = count;

        }

        return true;
    }
}//namespace behaviac
