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
#include "behaviac/behaviortree/nodes/actions/action.h"
#include "behaviac/common/profiler/profiler.h"
#include "behaviac/common/meta.h"
#include "behaviac/common/member.h"

namespace behaviac {
    Action::Action() : m_method(0), m_resultOption(BT_INVALID), m_resultFunctor(0) {
    }

    Action::~Action() {
        BEHAVIAC_DELETE(m_method);
        BEHAVIAC_DELETE(m_resultFunctor);
    }

    const char* strrchr(const char* start, const char* end, char c) {
        while (end > start) {
            if (*end == c) {
                return end;
            }

            end--;
        }

        return 0;
    }

    static const size_t kNameLength = 256;
    const char* Action::ParseMethodNames(const char* fullName, char* agentIntanceName, char* agentClassName, char* methodName) {
        //Self.test_ns::AgentActionTest::Action2(0)
        //string str;

        const char*  pClassBegin = strchr(fullName, '.');
        BEHAVIAC_ASSERT(pClassBegin);

        size_t posClass = pClassBegin - fullName;
        BEHAVIAC_ASSERT(posClass < kNameLength);
        string_ncpy(agentIntanceName, fullName, posClass);
        agentIntanceName[posClass] = '\0';

        const char* pBeginAgentClass = pClassBegin + 1;

        const char* pBeginP = strchr(pBeginAgentClass, '(');
        BEHAVIAC_ASSERT(pBeginP);

        //test_ns::AgentActionTest::Action2(0)
        const char* pBeginMethod = strrchr(pBeginAgentClass, pBeginP, ':');
        BEHAVIAC_ASSERT(pBeginMethod);
        //skip '::'
        BEHAVIAC_ASSERT(pBeginMethod[0] == ':' && pBeginMethod[-1] == ':');
        pBeginMethod += 1;

        size_t pos1 = pBeginP - pBeginMethod;
        BEHAVIAC_ASSERT(pos1 < kNameLength);

        string_ncpy(methodName, pBeginMethod, pos1);
        methodName[pos1] = '\0';

        size_t pos = pBeginMethod - 2 - pBeginAgentClass;
        BEHAVIAC_ASSERT(pos < kNameLength);

        string_ncpy(agentClassName, pBeginAgentClass, pos);
        agentClassName[pos] = '\0';

        return pBeginP;
    }

    ////suppose params are seprated by ','
    //static void ParseForParams(const behaviac::string& tsrc, behaviac::vector<behaviac::string>& params)
    //{
    //    int tsrcLen = (int)tsrc.size();
    //    int startIndex = 0;
    //    int index = 0;
    //    int quoteDepth = 0;

    //    for (; index < tsrcLen; ++index)
    //    {
    //        if (tsrc[index] == '"')
    //        {
    //            quoteDepth++;

    //            //if (quoteDepth == 1)
    //            //{
    //            //	startIndex = index;
    //            //}

    //            if ((quoteDepth & 0x1) == 0)
    //            {
    //                //closing quote
    //                quoteDepth -= 2;
    //                BEHAVIAC_ASSERT(quoteDepth >= 0);
    //            }
    //        }
    //        else if (quoteDepth == 0 && tsrc[index] == ',')
    //        {
    //            //skip ',' inside quotes, like "count, count"
    //            int lengthTemp = index - startIndex;
    //            behaviac::string strTemp = tsrc.substr(startIndex, lengthTemp);
    //            params.push_back(strTemp);
    //            startIndex = index + 1;
    //        }
    //    }//end for

    //    // the last param
    //    int lengthTemp = index - startIndex;

    //    if (lengthTemp > 0)
    //    {
    //        behaviac::string strTemp = tsrc.substr(startIndex, lengthTemp);
    //        params.push_back(strTemp);

    //        //params.push_back(strTemp);
    //    }
    //}

    void Action::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);

        for (propertie_const_iterator_t it = properties.begin(); it != properties.end(); ++it) {
            const property_t& p = (*it);

            if (StringUtils::StringEqual(p.name, "Method")) {
                if (p.value[0] != '\0') {
                    //this->m_method = Action::LoadMethod(p.value);
                    this->m_method = AgentMeta::ParseMethod(p.value);
                }
            } else if (StringUtils::StringEqual(p.name, "ResultOption")) {
                if (StringUtils::StringEqual(p.value, "BT_INVALID")) {
                    m_resultOption = BT_INVALID;

                } else if (StringUtils::StringEqual(p.value, "BT_FAILURE")) {
                    m_resultOption = BT_FAILURE;

                } else if (StringUtils::StringEqual(p.value, "BT_RUNNING")) {
                    m_resultOption = BT_RUNNING;

                } else {
                    m_resultOption = BT_SUCCESS;
                }
            } else if (StringUtils::StringEqual(p.name, "ResultFunctor")) {
                if (p.value[0] != '\0') {
                    this->m_resultFunctor = AgentMeta::ParseMethod(p.value);
                }
            } else {
                //BEHAVIAC_ASSERT(0, "unrecognised property %s", p.name);
            }
        }
    }

    bool Action::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (!Action::DynamicCast(pTask->GetNode())) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    }

    BehaviorTask* Action::createTask() const {
        ActionTask* pTask = BEHAVIAC_NEW ActionTask();

        return pTask;
    }

    uint32_t SetNodeId(uint32_t nodeId);
    void ClearNodeId(uint32_t slot);

    //Execute(Agent* pAgent)method hava be change to Execute(Agent* pAgent, EBTStatus childStatus)
    EBTStatus Action::Execute(const Agent* pAgent, EBTStatus childStatus) {
        EBTStatus result = BT_SUCCESS;

        if (this->m_method) {
            //#if BEHAVIAC_ENABLE_PROFILING
            //			BEHAVIAC_PROFILE("Node");
            //#endif
            //uint32_t nodeId = this->GetId();

            //uint32_t slot = SetNodeId(nodeId);
            //         BEHAVIAC_ASSERT(slot != (uint32_t)-1, "no empty slot found!");

            //         const Agent* pParent = this->m_method->GetParentAgent(pAgent);
            //         this->m_method->run(pParent, pAgent);

            if (this->m_resultOption != BT_INVALID) {
                this->m_method->run((Agent*)pAgent);
                result = this->m_resultOption;
            } else {
                if (this->m_resultFunctor != NULL) {
                    IValue* returnValue = this->m_resultFunctor->GetIValueFrom((Agent*)pAgent, this->m_method);

                    result = ((TValue<EBTStatus>*)returnValue)->value;
                } else {
                    IValue* returnValue = this->m_method->GetIValue((Agent*)pAgent);

                    //Debug.Check(returnValue is TValue<EBTStatus>, "method's return type is not EBTStatus");
                    BEHAVIAC_ASSERT((TValue<EBTStatus>*)(returnValue), "method's return type is not EBTStatus");

                    result = ((TValue<EBTStatus>*)returnValue)->value;
                }

            }

            //if (this->m_resultFunctor)
            //{
            //    const Agent* pParentCheckResult = this->m_resultFunctor->GetParentAgent(pAgent);

            //    result = (EBTStatus)this->m_method->CheckReturn(pParent, pParentCheckResult, this->m_resultFunctor);
            //}
            //else
            //{
            //    this->m_method->CheckReturn(pParent, result);
            //}
            //ClearNodeId(slot);
        } else {
            //#if BEHAVIAC_ENABLE_PROFILING
            //			BEHAVIAC_PROFILE("ActionGenerated");
            //#endif
            result = this->update_impl((Agent*)pAgent, childStatus);
        }

        return result;
    }

    ActionTask::ActionTask() : LeafTask() {
    }

    ActionTask::~ActionTask() {
    }

    void ActionTask::copyto(BehaviorTask* target) const {
        super::copyto(target);
    }

    void ActionTask::save(IIONode* node) const {
        super::save(node);
    }

    void ActionTask::load(IIONode* node) {
        super::load(node);
    }

    bool ActionTask::onenter(Agent* pAgent) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        return true;
    }

    void ActionTask::onexit(Agent* pAgent, EBTStatus s) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(s);
    }

    EBTStatus ActionTask::update(Agent* pAgent, EBTStatus childStatus) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(childStatus);

        BEHAVIAC_ASSERT(Action::DynamicCast(this->GetNode()));
        Action* pActionNode = (Action*)(this->GetNode());

        EBTStatus result = pActionNode->Execute(pAgent, childStatus);

        return result;
    }
}
