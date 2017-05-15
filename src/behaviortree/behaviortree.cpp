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

#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/agent/registermacros.h"
#include "behaviac/behaviortree/propertymember.h"
#include "behaviac/behaviortree/attachments/event.h"
#include "behaviac/behaviortree/attachments/effector.h"
#include "behaviac/behaviortree/nodes/actions/action.h"
#include "behaviac/behaviortree/attachments/precondition.h"
#include "behaviac/behaviortree/attachments/effector.h"
#include "behaviac/fsm/state.h"
#include "behaviac/common/meta.h"
#include "behaviac/common/member.h"

BEHAVIAC_BEGIN_ENUM_EX(behaviac::EBTStatus, EBTStatus)
{
	BEHAVIAC_ENUMCLASS_DISPLAYNAME_EX(L"BT״̬");
	BEHAVIAC_ENUMCLASS_DESC_EX(L"BT״̬");

	BEHAVIAC_ENUM_ITEM_EX(behaviac::BT_INVALID, "BT_INVALID");
	BEHAVIAC_ENUM_ITEM_EX(behaviac::BT_SUCCESS, "BT_SUCCESS");
	BEHAVIAC_ENUM_ITEM_EX(behaviac::BT_FAILURE, "BT_FAILURE");
	BEHAVIAC_ENUM_ITEM_EX(behaviac::BT_RUNNING, "BT_RUNNING");
}
BEHAVIAC_END_ENUM_EX()

namespace behaviac {
    namespace rapidxml {
        //! When exceptions are disabled by defining RAPIDXML_NO_EXCEPTIONS,
        //! this function is called to notify user about the error.
        //! It must be defined by the user.
        //! <br><br>
        //! This function cannot return. If it does, the results are undefined.
        //! <br><br>
        //! A very simple definition might look like that:
        //! <pre>
        //! void %rapidxml::%parse_error_handler(const char *what, void *where)
        //! {
        //!     std::cout << "Parse error: " << what << "\n";
        //!     std::abort();
        //! }
        //! </pre>
        //! \param what Human readable description of the error.
        //! \param where Pointer to character data where error was detected.
        void parse_error_handler(const char* what, void* where) {
            BEHAVIAC_UNUSED_VAR(where);

            BEHAVIAC_LOGERROR("rapidxml parse error: %s\n", what);
            BEHAVIAC_ASSERT(0);
        }
    }
}

namespace behaviac {
    static const char* kStrBehavior = "behavior";
    static const char* kStrAgentType = "agenttype";

    static const char* kStrId = "id";

    static const char* kStrPars = "pars";
    static const char* kStrPar = "par";

    static const char* kStrNode = "node";
    static const char* kStrCustom = "custom";
    // static const char* kStrFlag = "flag";
    //static const char* kStrProperties = "properties";
    static const char* kStrProperty = "property";
    //static const char* kStrAttachments = "attachments";
    static const char* kStrAttachment = "attachment";
    static const char* kStrClass = "class";

    static const char* kStrName = "name";
    static const char* kStrType = "type";
    static const char* kStrValue = "value";
    // static const char* kEventParam = "eventParam";

    static const char* kStrVersion = "version";
    // static const char* kPrecondition = "precondition";
    // static const char* kEffector = "effector";
    // static const char* kTransition = "transition";
    //bson deserizer

    //keep this version equal to designers' NewVersion
    const int SupportedVersion = 5;

    //BEHAVIAC_BEGIN_STRUCT(BehaviorTree::Descriptor_t);
    //{
    //    BEHAVIAC_REGISTER_STRUCT_PROPERTY(Descriptor);
    //    BEHAVIAC_REGISTER_STRUCT_PROPERTY(Reference);
    //}
    //BEHAVIAC_END_STRUCT();

    CFactory<BehaviorNode>* BehaviorNode::ms_factory;
    CFactory<BehaviorNode>& BehaviorNode::Factory() {
        if (!ms_factory) {
            ms_factory = BEHAVIAC_NEW CFactory<BehaviorNode>;
        }

        BEHAVIAC_ASSERT(ms_factory);

        return *ms_factory;
    }

    void BehaviorNode::Cleanup() {
        if (ms_factory) {
            BEHAVIAC_DELETE(ms_factory);
            ms_factory = 0;
        }
    }

    BehaviorNode* BehaviorNode::Create(const char* className) {
        CStringCRC classId(className);
        BehaviorNode* pBehaviorNode = Factory().CreateObject(classId);
        return pBehaviorNode;
    }

    BehaviorNode::BehaviorNode() : m_id(INVALID_NODE_ID),
        m_enter_precond(0), m_update_precond(0), m_both_precond(0),
        m_success_effectors(0), m_failure_effectors(0), m_both_effectors(0),
        m_parent(0), m_children(0), m_customCondition(0),
        m_bHasEvents(false), m_loadAttachment(false) {
    }

    BehaviorNode::~BehaviorNode() {
        this->Clear();
    }

    void BehaviorNode::Clear() {

        if (this->m_children) {
            for (size_t i = 0; i < this->m_children->size(); ++i) {
                BehaviorNode* pChild = (*m_children)[i];
                BEHAVIAC_DELETE(pChild);
            }

            this->m_children->clear();
            BEHAVIAC_DELETE(this->m_children);
            this->m_children = 0;
        }

        if (this->m_customCondition) {
            BEHAVIAC_DELETE this->m_customCondition;
            this->m_customCondition = 0;
        }
    }

    BehaviorTask* BehaviorNode::CreateAndInitTask() const {
        BehaviorTask* pTask = this->createTask();
        BEHAVIAC_ASSERT(pTask);

        pTask->Init(this);

        return pTask;
    }
    bool BehaviorNode::CheckPreconditions(const Agent* pAgent, bool bIsAlive) const {
        Precondition::EPhase phase = bIsAlive ? Precondition::E_UPDATE : Precondition::E_ENTER;

        //satisfied if there is no preconditions
        if (this->m_preconditions.size() == 0) {
            return true;
        }

        if (this->m_both_precond == 0) {
            if (phase == Precondition::E_ENTER && this->m_enter_precond == 0) {
                return true;
            }

            if (phase == Precondition::E_UPDATE && this->m_update_precond == 0) {
                return true;
            }
        }

        bool firstValidPrecond = true;
        bool lastCombineValue = false;

        for (uint32_t i = 0; i < this->m_preconditions.size(); ++i) {
            Precondition* pPrecond = (Precondition*)this->m_preconditions[i];

            if (pPrecond != NULL) {
                Precondition::EPhase ph = pPrecond->GetPhase();

                if (ph == Precondition::E_BOTH || ph == phase) {
                    bool taskBoolean = pPrecond->Evaluate((Agent*)pAgent);

                    CombineResults(firstValidPrecond, lastCombineValue, pPrecond, taskBoolean);
                }
            }
        }

        return lastCombineValue;
    }

    void BehaviorNode::CombineResults(bool& firstValidPrecond, bool& lastCombineValue, Precondition* pPrecond, bool taskBoolean) {
        if (firstValidPrecond) {
            firstValidPrecond = false;
            lastCombineValue = taskBoolean;

        } else {
            bool andOp = pPrecond->IsAnd();

            if (andOp) {
                lastCombineValue = lastCombineValue && taskBoolean;

            } else {
                lastCombineValue = lastCombineValue || taskBoolean;
            }
        }
    }
    void BehaviorNode::ApplyEffects(Agent* pAgent, BehaviorNode::EPhase  phase) const {
        if (this->m_effectors.size() == 0) {
            return;
        }

        if (this->m_both_effectors == 0) {
            if (phase == Effector::E_SUCCESS && this->m_success_effectors == 0) {
                return;
            }

            if (phase == Effector::E_FAILURE && this->m_failure_effectors == 0) {
                return;
            }
        }

        for (uint32_t i = 0; i < this->m_effectors.size(); ++i) {
            Effector* pEffector = (Effector*)this->m_effectors[i];

            if (pEffector != NULL) {
                Effector::EPhase ph = pEffector->GetPhase();

                if (phase == Effector::E_BOTH || ph == Effector::E_BOTH || ph == phase) {
                    pEffector->Evaluate((Agent*)pAgent);
                }
            }
        }
    }

    bool BehaviorNode::IsManagingChildrenAsSubTrees() const {
        return false;
    }

    void BehaviorNode::Attach(BehaviorNode* pAttachment, bool bIsPrecondition, bool bIsEffector, bool bIsTransition) {
        BEHAVIAC_UNUSED_VAR(bIsTransition);
        BEHAVIAC_ASSERT(bIsTransition == false);

        if (bIsPrecondition) {
            BEHAVIAC_ASSERT(!bIsEffector);

            Precondition* predicate = (Precondition*)pAttachment;
            BEHAVIAC_ASSERT(predicate != NULL);
            this->m_preconditions.push_back(predicate);

            Precondition::EPhase phase = predicate->GetPhase();

            if (phase == Precondition::E_ENTER) {
                this->m_enter_precond++;

            } else if (phase == Precondition::E_UPDATE) {
                this->m_update_precond++;

            } else if (phase == Precondition::E_BOTH) {
                this->m_both_precond++;

            } else {
                BEHAVIAC_ASSERT(false);
            }
        } else if (bIsEffector) {
            BEHAVIAC_ASSERT(!bIsPrecondition);

            Effector* effector = (Effector*)pAttachment;
            BEHAVIAC_ASSERT(effector != NULL);
            this->m_effectors.push_back(effector);

            Effector::EPhase phase = effector->GetPhase();

            if (phase == Effector::E_SUCCESS) {
                this->m_success_effectors++;

            } else if (phase == Effector::E_FAILURE) {
                this->m_failure_effectors++;

            } else if (phase == Effector::E_BOTH) {
                this->m_both_effectors++;

            } else {
                BEHAVIAC_ASSERT(false);
            }
        } else {
            this->m_events.push_back(pAttachment);
        }
    }

    bool BehaviorNode::CheckEvents(const char* eventName, Agent* pAgent, behaviac::map<uint32_t, IInstantiatedVariable*>* eventParams) const {
        if (this->m_events.size() > 0) {
            //bool bTriggered = false;
            for (uint32_t i = 0; i < this->m_events.size(); ++i) {
                BehaviorNode* pA = this->m_events[i];
                Event* pE = (Event*)pA;;

                //check events only

                if (pE != NULL && !StringUtils::IsNullOrEmpty(eventName)) {
                    const char* pEventName = pE->GetEventName();

                    if (!StringUtils::IsNullOrEmpty(pEventName) && StringUtils::StringEqual(pEventName, eventName)) {
                        pE->switchTo(pAgent, eventParams);

                        if (pE->TriggeredOnce()) {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }

    bool BehaviorNode::HasEvents() const {
        return this->m_bHasEvents;
    }

    void BehaviorNode::SetHasEvents(bool hasEvents) {
        this->m_bHasEvents = hasEvents;
    }

    uint32_t BehaviorNode::GetChildrenCount() const {
        if (this->m_children) {
            return (uint32_t)this->m_children->size();
        }

        return 0;
    }
    BehaviorNode* BehaviorNode::GetChildById(int16_t nodeId) const {
        size_t m_childCount = this->m_children->size();

        if (m_childCount > 0) {
            for (size_t i = 0; i < m_childCount; ++i) {
                BehaviorNode* c = (*this->m_children)[i];

                if (c->GetId() == nodeId) {
                    return c;
                }
            }
        }

        return 0;
    }

    const BehaviorNode* BehaviorNode::GetChild(uint32_t index) const {
        if (this->m_children && index < this->m_children->size()) {
            return (*this->m_children)[index];
        }

        return 0;
    }

    void BehaviorNode::SetClassNameString(const char* className) {
        this->m_className = className;
    }

    const behaviac::string& BehaviorNode::GetClassNameString() const {
        return this->m_className;
    }

    int16_t BehaviorNode::GetId() const {
        //BEHAVIAC_ASSERT(this->m_id != INVALID_NODE_ID);

        return this->m_id;
    }

    void BehaviorNode::SetId(int16_t id) {
        this->m_id = id;
    }

    void BehaviorNode::SetAgentType(const behaviac::string& agentType) {
        BEHAVIAC_UNUSED_VAR(agentType);
#if !BEHAVIAC_RELEASE
        this->m_agentType = agentType;
#endif
    }

    void BehaviorTree::AddPar(const char* agentType, const char* typeName, const char* name, const char* valueStr) {
        this->AddLocal(agentType, typeName, name, valueStr);
    }

    void BehaviorTree::AddLocal(const char* agentType, const char* typeName, const char* name, const char* valueStr) {
		BEHAVIAC_UNUSED_VAR(agentType);

		uint32_t varId = MakeVariableId(name);
        IProperty* prop = AgentMeta::CreateProperty(typeName, varId, name, valueStr);
        this->m_localProps[varId] = prop;

        behaviac::string m_elementType = StringUtils::GetElementTypeFromName(typeName);

        if (m_elementType.size() > 0) {
            prop = AgentMeta::CreateArrayItemProperty(m_elementType.c_str(), varId, name);
            char strTemp[200] = { 0 };
			string_ncpy_s(strTemp, name, 200);
            string_cat(strTemp, "[]");
            varId = MakeVariableId(strTemp);
            this->m_localProps[varId] = prop;
        }
    }

    bool BehaviorNode::EvaluteCustomCondition(const Agent* pAgent) {
        if (this->m_customCondition != NULL) {
            return this->m_customCondition->Evaluate((Agent*)pAgent);
        }

        return false;
    }

    void BehaviorNode::SetCustomCondition(BehaviorNode* node) {
        this->m_customCondition = node;
    }

    //behaviac::CMethodBase* LoadMethod(const char* value_);

    bool BehaviorNode::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(pTask);
#if !BEHAVIAC_RELEASE
        BEHAVIAC_ASSERT(!this->m_agentType.empty());
        CStringCRC btAgentClass(this->m_agentType.c_str());

        return pAgent->IsAKindOf(btAgentClass);
#else
        return true;
#endif//#if !BEHAVIAC_RELEASE
    }

    void BehaviorNode::load(int version, const char* agentType, const properties_t& properties) {
        BEHAVIAC_UNUSED_VAR(version);
        BEHAVIAC_UNUSED_VAR(agentType);
        BEHAVIAC_UNUSED_VAR(properties);

        {
            const char* nodeType = this->GetObjectTypeName();
            Workspace::GetInstance()->BehaviorNodeLoaded(nodeType, properties);
        }
    }

    void BehaviorNode::load_properties(int version, const char* agentType, rapidxml::xml_node<>* node) {
#if !BEHAVIAC_RELEASE
        this->m_agentType = agentType;
#endif//

        properties_t properties;

        rapidxml::xml_node<>* nodesProperty = node->first_node(kStrProperty);

        for (rapidxml::xml_node<>* attachmentNode = nodesProperty; attachmentNode; attachmentNode = attachmentNode->next_sibling()) {
            if (StringUtils::StringEqual(attachmentNode->name(), kStrProperty)) {
                if (rapidxml::xml_attribute<>* attr = attachmentNode->first_attribute()) {
                    //std::cout << attr->name() << ":" << attr->value() << std::endl;
                    const char* pPropertyName = attr->name();
                    const char* pPropertyValue = attr->value();
                    properties.push_back(property_t(pPropertyName, pPropertyValue));
                }
            }
        }

        if (properties.size() > 0) {
            this->load(version, agentType, properties);
        }
    }

    void BehaviorNode::load_properties_pars(int version, const char* agentType, rapidxml::xml_node<>* node) {
        this->load_properties(version, agentType, node);

        //pars
        rapidxml::xml_node<>* pars = node->first_node(kStrPars);

        if (pars) {
            rapidxml::xml_node<>* parNode = pars->first_node(kStrPar);

            for (; parNode; parNode = parNode->next_sibling()) {
                this->load_local(version, agentType, parNode);
            }
        }
    }

    void BehaviorNode::AddChild(BehaviorNode* pChild) {
        pChild->m_parent = this;

        if (!this->m_children) {
            this->m_children = BEHAVIAC_NEW Nodes;
        }

        this->m_children->push_back(pChild);
    }

    EBTStatus BehaviorNode::update_impl(Agent* pAgent, EBTStatus childStatus) {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(childStatus);
        return BT_FAILURE;
    }

    DecoratorNode::DecoratorNode() : m_bDecorateWhenChildEnds(false)
    {}

    DecoratorNode::~DecoratorNode()
    {}

    bool DecoratorNode::IsManagingChildrenAsSubTrees() const {
        //return !this->m_bDecorateWhenChildEnds;
        return true;
    }

    void DecoratorNode::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);

        for (propertie_const_iterator_t it = properties.begin(); it != properties.end(); ++it) {
            const property_t& p = (*it);

            if (StringUtils::StringEqual(p.name, "DecorateWhenChildEnds")) {
                if (p.value[0] != '\0') {
                    if (StringUtils::StringEqual(p.value, "true")) {
                        this->m_bDecorateWhenChildEnds = true;
                    }
                }//if (p.value[0] != '\0')
            } else {
                //BEHAVIAC_ASSERT(0, "unrecognised property %s", p.name);
            }
        }
    }

    bool DecoratorNode::IsValid(Agent* pAgent, BehaviorTask* pTask) const {
        if (!DecoratorNode::DynamicCast(pTask->GetNode())) {
            return false;
        }

        return super::IsValid(pAgent, pTask);
    };

    BehaviorTree::BehaviorTree() : BehaviorNode() {
        this->m_bIsFSM = false;
    }

    BehaviorTree::~BehaviorTree() {
        //this->m_descriptorRefs.clear();
        if (this->m_localProps.size() > 0) {
            this->m_localProps.clear();
        }
    }

    //
    void BehaviorTree::load(int version, const char* agentType, const properties_t& properties) {
        super::load(version, agentType, properties);

        if (properties.size() > 0) {
            for (propertie_const_iterator_t it = properties.begin(); it != properties.end(); it++) {
                const property_t& p = (*it);

                if (StringUtils::StringEqual(p.name, "Domains")) {
                    m_domains = p.value;

                } else if (StringUtils::StringEqual(p.name, "DescriptorRefs")) {
                    //StringUtils::ParseString(p.value, this->m_descriptorRefs);

                    //for (size_t i = 0; i < this->m_descriptorRefs.size(); ++i)
                    //{
                    //    Descriptor_t& d = this->m_descriptorRefs[i];

                    //    if (d.Descriptor != NULL)
                    //    {
                    //        //d.Descriptor->SetDefaultValue(d.Reference);
                    //        BEHAVIAC_ASSERT(false);
                    //    }
                    //}
                } else {
                    //BEHAVIAC_ASSERT(0, "unrecognised property %s", p.name);
                }
            }
        }
    }

    bool BehaviorTree::IsManagingChildrenAsSubTrees() const {
        return true;
    }

    const behaviac::string& BehaviorTree::GetDomains() const {
        return this->m_domains;
    }

    void BehaviorTree::SetDomains(const behaviac::string& domains) {
        this->m_domains = domains;
    }

    //const BehaviorTree::Descriptors_t BehaviorTree::GetDescriptors() const
    //{
    //    return m_descriptorRefs;
    //}

    //void BehaviorTree::SetDescriptors(const char* descriptors)
    //{
    //    behaviac::StringUtils::ParseString(descriptors, this->m_descriptorRefs);

    //    for (size_t i = 0; i < this->m_descriptorRefs.size(); ++i)
    //    {
    //        Descriptor_t& d = this->m_descriptorRefs[i];

    //        if (d.Descriptor)
    //        {
    //            BEHAVIAC_ASSERT(false);
    //        }
    //    }
    //}

    bool BehaviorTree::IsFSM() {
        return m_bIsFSM;
    }

    void BehaviorTree::SetIsFSM(bool isFsm) {
        m_bIsFSM = isFsm;
    }
    void BehaviorTree::load_local(int version, const char* agentType, BsonDeserizer& d) {
        BEHAVIAC_UNUSED_VAR(version);
        BEHAVIAC_UNUSED_VAR(agentType);

        d.OpenDocument();

        const char* name = d.ReadString();
        const char* type = d.ReadString();
        const char* value = d.ReadString();

        this->AddLocal(agentType, type, name, value);

        d.CloseDocument(true);
    }

    void BehaviorTree::load_local(int version, const char* agentType, rapidxml::xml_node<>* node) {
        BEHAVIAC_UNUSED_VAR(version);
        BEHAVIAC_UNUSED_VAR(agentType);

        if (!StringUtils::StringEqual(node->name(), kStrPar)) {
            BEHAVIAC_ASSERT(0);
            return;
        }

        const char* name = node->first_attribute(kStrName)->value();
        const char* type = node->first_attribute(kStrType)->value();
        const char* value = node->first_attribute(kStrValue)->value();

        this->AddLocal(agentType, type, name, value);
    }
    void BehaviorNode::load_local(int version, const char* agentType, rapidxml::xml_node<>* node) {
		BEHAVIAC_UNUSED_VAR(version);
		BEHAVIAC_UNUSED_VAR(agentType);
		BEHAVIAC_UNUSED_VAR(node);
		BEHAVIAC_ASSERT(false);
    }

    void BehaviorNode::load_properties_pars_attachments_children(bool bNode, int version, const char* agentType, rapidxml::xml_node<>* node) {
#if !BEHAVIAC_RELEASE
        SetAgentType(agentType);
#endif//

        bool bHasEvents = this->HasEvents();
        rapidxml::xml_node<>* children = node->first_node();

        if (children != NULL) {
            properties_t properties;

            for (rapidxml::xml_node<>* c = children; c; c = c->next_sibling()) {
                if (!load_property_pars(properties, c, version, agentType)) {
                    if (bNode) {
                        if (StringUtils::StringEqual(c->name(), kStrAttachment)) {
                            bHasEvents |= this->load_attachment(version, agentType, bHasEvents, c);
                        } else if (StringUtils::StringEqual(c->name(), kStrCustom)) {
                            rapidxml::xml_node<>*  customNode = (rapidxml::xml_node<>*)c->first_node(kStrNode);
                            BEHAVIAC_ASSERT(customNode);
                            BehaviorNode* pChildNode = BehaviorNode::load(agentType, customNode, version);
                            this->m_customCondition = pChildNode;
                        } else if (StringUtils::StringEqual(c->name(), kStrNode)) {
                            BehaviorNode* pChildNode = BehaviorNode::load(agentType, c, version);
                            BEHAVIAC_ASSERT(pChildNode);
                            bHasEvents |= pChildNode->m_bHasEvents;

                            this->AddChild(pChildNode);
                        }
                    } else {
                        if (StringUtils::StringEqual(c->name(), kStrAttachment)) {
                            bHasEvents = this->load_attachment(version, agentType, bHasEvents, c);
                        }
                    }
                }
            }

            if (properties.size() > 0) {
                this->load(version, agentType, properties);
            }
        }

        this->m_bHasEvents |= bHasEvents;
    }

    void BehaviorNode::load_attachment_transition_effectors(int version, const char* agentType, rapidxml::xml_node<>* c) {
        this->m_loadAttachment = true;

        this->load_properties_pars_attachments_children(false, version, agentType, c);

        this->m_loadAttachment = false;
    }

    bool BehaviorNode::load_attachment(int version, const char* agentType, bool bHasEvents, rapidxml::xml_node<>*  c) {
        rapidxml::xml_attribute<>* pAttachClassAttr = c->first_attribute("class");

        if (pAttachClassAttr == NULL) {
            this->load_attachment_transition_effectors(version, agentType, c);
            return true;
        }

        const char* pAttachClassName = pAttachClassAttr->value();

        BehaviorNode* pAttachment = BehaviorNode::Create(pAttachClassName);

        BEHAVIAC_ASSERT(pAttachment != NULL);

        if (pAttachment != NULL) {
            pAttachment->SetClassNameString(pAttachClassName);
            const char* idStr = c->first_attribute("id")->value();
            pAttachment->SetId((uint16_t)atoi(idStr));

            bool bIsPrecondition = false;
            bool bIsEffector = false;
            bool bIsTransition = false;
            const char* flagStr = c->first_attribute("flag")->value();

            if (StringUtils::StringEqual(flagStr, "precondition")) {
                bIsPrecondition = true;

            } else if (StringUtils::StringEqual(flagStr, "effector")) {
                bIsEffector = true;

            } else if (StringUtils::StringEqual(flagStr, "transition")) {
                bIsTransition = true;
            }

            pAttachment->load_properties_pars_attachments_children(false, version, agentType, c);

            this->Attach(pAttachment, bIsPrecondition, bIsEffector, bIsTransition);

            bHasEvents |= (Event::DynamicCast(pAttachment) != 0);
        }

        return bHasEvents;
    }

    /**
    Parse the property of node

    Parse the node's property or properties, and store it/them in prperties

    @return
    return true if successfully loaded
    */
    bool BehaviorNode::load_property_pars(properties_t& properties, rapidxml::xml_node<>* c, int version, const char* agentType) {
        if (StringUtils::StringEqual(c->name(), kStrProperty)) {
            if (rapidxml::xml_attribute<>* attr = c->first_attribute()) {
                //std::cout << attr->name() << ":" << attr->value() << std::endl;
                const char* pPropertyName = attr->name();
                const char* pPropertyValue = attr->value();
                properties.push_back(property_t(pPropertyName, pPropertyValue));
            }

            return true;

        } else if (StringUtils::StringEqual(c->name(), kStrPars)) {
            rapidxml::xml_node<>* children = c->first_node();

            if (children != NULL) {
                for (rapidxml::xml_node<>* child = children; child; child = child->next_sibling()) {
                    if (StringUtils::StringEqual(child->name(), kStrPar)) {
                        this->load_local(version, agentType, child);
                    }
                }
            }

            return true;
        }

        return false;
    }

    BehaviorNode* BehaviorNode::load(const char* agentType, rapidxml::xml_node<>* node, int version) {
        //BEHAVIAC_ASSERT(node.Tag == "node");
        BEHAVIAC_ASSERT(StringUtils::StringEqual(node->name(), "node"));

        if (rapidxml::xml_attribute<>* attr = node->first_attribute(kStrClass)) {
            BEHAVIAC_ASSERT(StringUtils::StringEqual(attr->name(), kStrClass));
            const char* pClassName = attr->value();
            BehaviorNode* pNode = BehaviorNode::Create(pClassName);

            if (!pNode) {
                BEHAVIAC_LOGWARNING("invalid node class '%s'\n", pClassName);
            }

            //BEHAVIAC_ASSERT(pNode != NULL, "unsupported class {0}", pClassName);
            BEHAVIAC_ASSERT(pNode != NULL, "unsupported class %s", pClassName);

            if (pNode != NULL) {
                pNode->SetClassNameString(pClassName);
                const char* idStr = node->first_attribute(kStrId)->value();//node.Attribute("id");
                BEHAVIAC_ASSERT(idStr);
                pNode->SetId((uint16_t)atoi(idStr));

                pNode->load_properties_pars_attachments_children(true, version, agentType, node);
            }

            return pNode;
        }

        return 0;
    }

    void BehaviorNode::load_properties(int version, const char* agentType, BsonDeserizer& d) {
#if !BEHAVIAC_RELEASE
        this->m_agentType = agentType;
#endif
        d.OpenDocument();

        //load property after loading par as property might reference par

        properties_t properties;

        behaviac::BsonDeserizer::BsonTypes type = d.ReadType();

        while (type == BsonDeserizer::BT_String) {
            const char* propertyName = d.ReadString();
            const char* propertyValue = d.ReadString();

            properties.push_back(property_t(propertyName, propertyValue));

            type = d.ReadType();
        }

        if (properties.size() > 0) {
            this->load(version, agentType, properties);
        }

        //BEHAVIAC_ASSERT(type == BsonDeserizer.BsonTypes.BT_None);
        BEHAVIAC_ASSERT(type == BsonDeserizer::BT_None);
        d.CloseDocument(false);
    }

    void BehaviorNode::load_pars(int version, const char* agentType, BsonDeserizer& d) {
        d.OpenDocument();

        BsonDeserizer::BsonTypes type = d.ReadType();

        while (type == BsonDeserizer::BT_ParElement) {
            this->load_local(version, agentType, d);

            type = d.ReadType();
        }

        BEHAVIAC_ASSERT(type == BsonDeserizer::BT_None);
        //BEHAVIAC_ASSERT(type == BsonDeserizer.BsonTypes.BT_None);
        d.CloseDocument(false);
    }

    void BehaviorNode::load_custom(int version, const char* agentType, BsonDeserizer& d) {
        d.OpenDocument();

        BsonDeserizer::BsonTypes type = d.ReadType();
        BEHAVIAC_UNUSED_VAR(type);
        //BEHAVIAC_ASSERT(type == BsonDeserizer.BsonTypes.BT_NodeElement);
        BEHAVIAC_ASSERT(type == BsonDeserizer::BT_NodeElement);

        d.OpenDocument();

        BehaviorNode* pChildNode = this->load(agentType, d, version);
        this->m_customCondition = pChildNode;

        d.CloseDocument(false);

        d.CloseDocument(false);

        type = d.ReadType();
        //BEHAVIAC_ASSERT(type == BsonDeserizer.BsonTypes.BT_None);
        BEHAVIAC_ASSERT(type == BsonDeserizer::BT_None);
    }

    void BehaviorNode::load_properties_pars_attachments_children(int version, const char*  agentType, BsonDeserizer& d, bool bIsTransition) {
        BsonDeserizer::BsonTypes type = d.ReadType();

        while (type != BsonDeserizer::BT_None) {
            if (type == BsonDeserizer::BT_PropertiesElement) {
                this->load_properties(version, agentType, d);
            } else if (type == BsonDeserizer::BT_ParsElement) {
                this->load_pars(version, agentType, d);
            } else if (type == BsonDeserizer::BT_AttachmentsElement) {
                this->load_attachments(version, agentType, d, bIsTransition);

                this->m_bHasEvents |= this->HasEvents();
            } else if (type == BsonDeserizer::BT_Custom) {
                this->load_custom(version, agentType, d);
            } else if (type == BsonDeserizer::BT_NodeElement) {
                this->load_children(version, agentType, d);
            } else {
                BEHAVIAC_ASSERT(false);
                //BEHAVIAC_ASSERT(false);
            }

            type = d.ReadType();
        }
    }

    BehaviorNode* BehaviorNode::load(const char*  agentType, BsonDeserizer& d, int version) {
        const char*  pClassName = d.ReadString();
        BehaviorNode* pNode = BehaviorNode::Create(pClassName);
        //BEHAVIAC_ASSERT(pNode != NULL, pClassName);
        BEHAVIAC_ASSERT(pNode != NULL, pClassName);

        if (pNode != NULL) {
            pNode->SetClassNameString(pClassName);
            const char* idString = d.ReadString();
            BEHAVIAC_ASSERT(idString);
            pNode->SetId((uint16_t)atoi(idString));

            pNode->load_properties_pars_attachments_children(version, agentType, d, false);
        }

        return pNode;
    }

    void BehaviorNode::load_attachments(int version, const char*  agentType, BsonDeserizer& d, bool bIsTransition) {
        d.OpenDocument();

        BsonDeserizer::BsonTypes type = d.ReadType();

        while (type == BsonDeserizer::BT_AttachmentElement) {
            {
                d.OpenDocument();

                if (bIsTransition) {
                    this->m_loadAttachment = true;
                    this->load_properties_pars_attachments_children(version, agentType, d, false);
                    this->m_loadAttachment = false;
                } else {
                    const char*  attachClassName = d.ReadString();

                    BehaviorNode* pAttachment = BehaviorNode::Create(attachClassName);
                    //Debug::Check(pAttachment != NULL, attachClassName);
                    BEHAVIAC_ASSERT(pAttachment != NULL, attachClassName);

                    if (pAttachment != NULL) {
                        pAttachment->SetClassNameString(attachClassName);

                        const char*  idString = d.ReadString();
                        pAttachment->SetId((uint16_t)atoi(idString));

                        bool bIsPrecondition = d.ReadBool();
                        bool bIsEffector = d.ReadBool();
                        bool bAttachmentIsTransition = d.ReadBool();

                        pAttachment->load_properties_pars_attachments_children(version, agentType, d, bAttachmentIsTransition);

                        this->Attach(pAttachment, bIsPrecondition, bIsEffector, bAttachmentIsTransition);

                        this->m_bHasEvents |= (Event::DynamicCast(pAttachment) != 0); //(pAttachment is Event);
                    }
                }

                d.CloseDocument(false);
            }

            type = d.ReadType();
        }

        if (type != BsonDeserizer::BT_None) {
            if (type == BsonDeserizer::BT_ParsElement) {
                this->load_pars(version, agentType, d);

            } else if (type == BsonDeserizer::BT_AttachmentsElement) {
                this->load_attachments(version, agentType, d, bIsTransition);

                this->m_bHasEvents |= this->HasEvents();
            } else {
                //BEHAVIAC_ASSERT(false);
                BEHAVIAC_ASSERT(false);
            }

            type = d.ReadType();
        }

        //BEHAVIAC_ASSERT(type == BsonDeserizer.BsonTypes.BT_None);
        BEHAVIAC_ASSERT(type == BsonDeserizer::BT_None);

        d.CloseDocument(false);
    }

    BehaviorNode* BehaviorNode::load_node(int version, const char*  agentType, BsonDeserizer& d) {
        d.OpenDocument();

        BsonDeserizer::BsonTypes type = d.ReadType();
        BEHAVIAC_UNUSED_VAR(type);
        //BEHAVIAC_ASSERT(type == BsonDeserizer.BsonTypes.BT_NodeElement);
        BEHAVIAC_ASSERT(type == BsonDeserizer::BT_NodeElement);
        d.OpenDocument();
        BehaviorNode* node = this->load(agentType, d, version);

        d.CloseDocument(false);

        type = d.ReadType();
        BEHAVIAC_ASSERT(type == BsonDeserizer::BT_None);
        //BEHAVIAC_ASSERT(type == BsonDeserizer.BsonTypes.BT_None);
        d.CloseDocument(false);

        return node;
    }

    void BehaviorNode::Attach(BehaviorNode* pAttachment, bool bIsPrecondition, bool bIsEffector) {
        this->Attach(pAttachment, bIsPrecondition, bIsEffector, false);
    }

    /**
    <?xml version="1.0" encoding="utf-8"?>
    <behavior agenttype="AgentTest">
    <!--EXPORTED BY TOOL, DON'T MODIFY IT!-->
    <!--Source File: ... -->
    <node class="DecoratorLoopTask">
    <property Count="10" />
    <node class="SelectorTask">
    ...
    </node>
    </node>
    </behavior>
    */
    bool BehaviorTree::load_xml(char* pBuffer) {
        BEHAVIAC_ASSERT(pBuffer != NULL);//BEHAVIAC_ASSERT(pBuffer != NULL);
        rapidxml::xml_document<> doc;
        doc.parse<0>(pBuffer);
        rapidxml::xml_node<>* behaviorNode = doc.first_node(kStrBehavior);//SecurityElement BehaviorNode* = xmlDoc.ToXml();

        //if (behaviorNode.Tag != "behavior" && (behaviorNode.Children == NULL || behaviorNode.Children.Count != 1))
        //{
        //	return false;
        //}
        if (!behaviorNode || !StringUtils::StringEqual(behaviorNode->name(), kStrBehavior)) {
            return false;
        }

        if (rapidxml::xml_attribute<>* attrName = behaviorNode->first_attribute(kStrName)) {
            //this->m_name = behaviorNode.Attribute("name");
            this->m_name = attrName->value();
        }

        rapidxml::xml_attribute<>* attrAgentType = behaviorNode->first_attribute(kStrAgentType); //string agentType = behaviorNode.Attribute("agenttype").Replace("::", ".");
        const char* agentType = attrAgentType->value();
        rapidxml::xml_attribute<>* versionStr = behaviorNode->first_attribute(kStrVersion); //string versionStr = behaviorNode.Attribute("version");
        rapidxml::xml_attribute<>* fsmAttr = behaviorNode->first_attribute("fsm");
        char* fsm = NULL;

        if (fsmAttr) {
            fsm = fsmAttr->value();
        }

        //int version = int.Parse(versionStr);
        int version = 0;

        if (versionStr) {
            version = atoi(versionStr->value());
        }

        if (version != SupportedVersion) {
#if !BEHAVIAC_RELEASE
            LogManager::GetInstance()->Error("'%s' Version(%d), while Version(%d) is supported, please update runtime or rexport data using the latest designer", this->GetName().c_str(), version, SupportedVersion);
            BEHAVIAC_ASSERT(false);
#endif//BEHAVIAC_RELEASE
        }

        this->SetClassNameString("BehaviorTree");
        this->SetId((uint16_t) - 1);

        if (fsm && StringUtils::StringEqual(fsm, "true")) {
            this->m_bIsFSM = true;

        } else {
            this->m_bIsFSM = false;
        }

        this->load_properties_pars_attachments_children(true, version, agentType, behaviorNode);

        return true;
    }

    BehaviorTask* BehaviorTree::createTask() const {
        BehaviorTreeTask* pTask = BEHAVIAC_NEW BehaviorTreeTask();

        return pTask;
    }

#define LITTLE_ENDIAN_ONLY		1
#define USE_STRING_COUNT_HEAD	1
    //#define USE_DOCUMENET			1

    static bool isLittleEndian() {
        static bool s_bDetected = false;
        static bool s_bLittleEndian = true;

        if (!s_bDetected) {
            int number = 0x1;
            char* numPtr = (char*)&number;

            s_bLittleEndian = (numPtr[0] == 1);
            s_bDetected = true;
        }

        return s_bLittleEndian;
    }

    BsonDeserizer::BsonDeserizer() : m_pBuffer(0), m_pPtr(0)
#if USE_DOCUMENET
        , m_document(0), m_documentStackTop(0)
#endif
    {
    }

    BsonDeserizer::~BsonDeserizer() {
    }

    bool BsonDeserizer::Init(const char* pBuffer) {
        this->m_pBuffer = pBuffer;

        if (this->m_pBuffer) {
            this->m_pPtr = this->m_pBuffer;

            if (this->OpenDocument()) {
                return true;
            }
        }

        BEHAVIAC_ASSERT(0);

        return false;
    }

    bool BsonDeserizer::OpenDocument() {
        const char* head = this->m_pPtr;
        uint32_t size = this->ReadInt32();
        //uint16_t size = this->ReadUInt16();

        const char* end = head + size - 1;

        if (*end == 0) {
#if USE_DOCUMENET
            BEHAVIAC_ASSERT(this->m_documentStackTop >= 0 && this->m_documentStackTop < kDocumentStackMax - 1);
            this->m_documentStack[this->m_documentStackTop++] = this->m_document;
            this->m_document = head;
#endif
            return true;
        }

        BEHAVIAC_ASSERT(0);

        return false;
    }

    void BsonDeserizer::CloseDocument(bool bEatEod) {
        BEHAVIAC_ASSERT(this->m_pPtr && this->m_pPtr > this->m_pBuffer);

        if (bEatEod) {
            const char* endLast = this->m_pPtr++;
            BEHAVIAC_ASSERT(*endLast == 0);
            BEHAVIAC_UNUSED_VAR(endLast);

        } else {
            const char* endLast = this->m_pPtr - 1;
            BEHAVIAC_ASSERT(*endLast == 0);
            BEHAVIAC_UNUSED_VAR(endLast);
        }

#if USE_DOCUMENET
        BEHAVIAC_ASSERT(this->m_documentStackTop > 0 && this->m_documentStackTop < kDocumentStackMax);
        this->m_document = this->m_documentStack[--this->m_documentStackTop];
#endif
    }

    BsonDeserizer::BsonTypes BsonDeserizer::ReadType() {
        char t = *this->m_pPtr++;

        return (BsonTypes)t;
    }

    int32_t BsonDeserizer::ReadInt32() {
        int32_t* pInt32 = (int32_t*)this->m_pPtr;

        this->m_pPtr += 4;
#if LITTLE_ENDIAN_ONLY
        bool bIsLittleEndian = isLittleEndian();
        BEHAVIAC_UNUSED_VAR(bIsLittleEndian);
        BEHAVIAC_ASSERT(bIsLittleEndian);

        return *pInt32;
#else

        if (isLittleEndian()) {
            return *pInt32;
        }

        uint8_t* pByte = (uint8_t*)pInt32;

        int32_t uint32 = (pByte[0] << 24 | pByte[1] << 16 | pByte[2] << 8 | pByte[3]);

        return uint32;
#endif//LITTLE_ENDIAN_ONLY
    }

    uint16_t BsonDeserizer::ReadUInt16() {
        uint16_t* pUInt16 = (uint16_t*)this->m_pPtr;

        this->m_pPtr += 2;
#if LITTLE_ENDIAN_ONLY
        BEHAVIAC_ASSERT(isLittleEndian());

        return *pUInt16;
#else

        if (isLittleEndian()) {
            return *pUInt16;
        }

        uint8_t* pByte = (uint8_t*)pUInt16;

        int32_t uint32 = (pByte[0] << 8 | pByte[1]);

        return uint32;
#endif//LITTLE_ENDIAN_ONLY
    }

    float BsonDeserizer::ReadFloat() {
        float* pFloat = (float*)this->m_pPtr;

        this->m_pPtr += 4;
#if LITTLE_ENDIAN_ONLY
        BEHAVIAC_ASSERT(isLittleEndian());

        return *pFloat;
#else

        if (isLittleEndian()) {
            return *pFloat;
        }

        uint8_t* pByte = (uint8_t*)pFloat;

        int32_t uint32 = (pByte[0] << 24 | pByte[1] << 16 | pByte[2] << 8 | pByte[3]);

        return (float&)uint32;
#endif//LITTLE_ENDIAN_ONLY
    }

    bool BsonDeserizer::ReadBool() {
        char* pByte = (char*)this->m_pPtr;

        this->m_pPtr += 1;

        return *pByte != 0 ? true : false;
    }

    const char* BsonDeserizer::ReadString() {
#if USE_STRING_COUNT_HEAD
        uint16_t count = ReadUInt16();

        const char* pStr = this->m_pPtr;

        this->m_pPtr += count;
        BEHAVIAC_ASSERT(*(this->m_pPtr - 1) == 0);

        return pStr;
#else
        const char* pStr = this->m_pPtr;

        while (*this->m_pPtr) {
            this->m_pPtr++;
        }

        //skip the ending 0
        this->m_pPtr++;

        return pStr;
#endif
    }

    bool BsonDeserizer::eod() const {
        char c = *this->m_pPtr;
        return c == 0;
    }

    bool BehaviorTree::load_bson(const char* pBuffer) {
        BsonDeserizer* d = BEHAVIAC_NEW BsonDeserizer();

        if (d->Init(pBuffer)) {
            BsonDeserizer::BsonTypes type = d->ReadType();

            if (type == BsonDeserizer::BT_BehaviorElement) {
                bool bOk = d->OpenDocument();
                BEHAVIAC_UNUSED_VAR(bOk);
                //BEHAVIAC_ASSERT(bOk);
                BEHAVIAC_ASSERT(bOk);
                this->m_name = d->ReadString();
                const char* agentType = d->ReadString();
                bool bFsm = d->ReadBool();
                const char* versionStr = d->ReadString();
                int version = atoi(versionStr);

                if (version != SupportedVersion) {
#if !BEHAVIAC_RELEASE
                    LogManager::GetInstance()->Error("'%s' Version(%d), while Version(%d) is supported, please update runtime or rexport data using the latest designer", this->GetName().c_str(), version, SupportedVersion);
                    BEHAVIAC_ASSERT(false);
#endif//BEHAVIAC_RELEASE
                }

                this->SetClassNameString("BehaviorTree");
                this->SetId((uint16_t) - 1);

                this->m_bIsFSM = bFsm;

                this->load_properties_pars_attachments_children(version, agentType, *d, false);

                d->CloseDocument(false);

                return true;
            }

            BEHAVIAC_DELETE d;
        }

        BEHAVIAC_ASSERT(false);//BEHAVIAC_ASSERT(false, e.Message);
        return false;
    }

    void BehaviorTree::InstantiatePars(behaviac::map<uint32_t, IInstantiatedVariable*>& vars) {
        if (this->m_localProps.size() != 0) {
            for (Properties_t::iterator it = this->m_localProps.begin(); it != this->m_localProps.end(); ++it) {
                vars[it->first] = it->second->Instantiate();
            }

        }
    }

    void BehaviorTree::UnInstantiatePars(behaviac::map<uint32_t, IInstantiatedVariable*>& vars) {
        if (vars.size() != 0) {
            for (behaviac::map<uint32_t, IInstantiatedVariable*>::iterator it = vars.begin(); it != vars.end(); ++it) {
                IInstantiatedVariable* pVar = it->second;
                BEHAVIAC_DELETE pVar;
            }

            vars.clear();
        }
    }

    void BehaviorNode::load_children(int version, const char* agentType, BsonDeserizer& d) {
        d.OpenDocument();

        BehaviorNode* pChildNode = this->load(agentType, d, version);
        BEHAVIAC_ASSERT(pChildNode);
        bool bHasEvents = pChildNode->m_bHasEvents;

        this->AddChild(pChildNode);

        this->m_bHasEvents |= bHasEvents;

        d.CloseDocument();
    }

    void BehaviorNode::load_local(int version, const char* agentType, BsonDeserizer& d) {
		BEHAVIAC_UNUSED_VAR(version);
		BEHAVIAC_UNUSED_VAR(agentType);
		BEHAVIAC_UNUSED_VAR(d);
		BEHAVIAC_ASSERT(false);
    }

}//namespace behaviac
