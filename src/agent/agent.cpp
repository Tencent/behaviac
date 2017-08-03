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
#include "behaviac/agent/registermacros.h"
#include "behaviac/agent/context.h"
#include "behaviac/agent/state.h"
#include "behaviac/property/property.h"
#include "behaviac/common/file/filesystem.h"
#include "behaviac/behaviortree/behaviortree.h"
#include "behaviac/behaviortree/behaviortree_task.h"
#include "behaviac/common/profiler/profiler.h"
#include "./propertynode.h"
#include "behaviac/common/meta.h"
#include "behaviac/common/member.h"

#if BEHAVIAC_CCDEFINE_MSVC
#define _BEHAVIAC_M_STRING_COMPILER_NAME_ BEHAVIAC_JOIN_TOKENS("compiler ", BEHAVIAC_CCDEFINE_NAME)
#pragma message (_BEHAVIAC_M_STRING_COMPILER_NAME_)
#endif

BEHAVIAC_BEGIN_STRUCT(IList) {
    //
}
BEHAVIAC_END_STRUCT()

BEHAVIAC_BEGIN_STRUCT(System::Object) {
    //
}
BEHAVIAC_END_STRUCT()

namespace behaviac {
    bool TryStart();

    uint32_t Agent::ms_idMask = 0xffffffff;
    Agent::AgentTypeIndexMap_t* Agent::ms_agent_type_index = 0;

    uint32_t Agent::IdMask() {
        return Agent::ms_idMask;
    }

    void Agent::SetIdMask(uint32_t idMask) {
        Agent::ms_idMask = idMask;
    }

    int Agent::ms_agent_index = 0;
    CFactory<Agent>* Agent::ms_factory;

    Agent::AgentNames_t* Agent::ms_names;
    Agent::AgentNames_t* Agent::NamesPtr() {
        return ms_names;
    }

    Agent::AgentNames_t& Agent::Names() {
        if (!ms_names) {
            ms_names = BEHAVIAC_NEW AgentNames_t;
        }

        BEHAVIAC_ASSERT(ms_names);

        return *ms_names;
    }

    CFactory<Agent>& Agent::Factory() {
        if (!ms_factory) {
            typedef CFactory<Agent> FactoryAgent;
            ms_factory = BEHAVIAC_NEW FactoryAgent;
        }

        return *ms_factory;
    }

    //m_id == -1, not a valid agent
    Agent::Agent() : m_context_id(-1), m_currentBT(0), m_id(-1), m_priority(0), m_bActive(1), m_referencetree(false), _balckboard_bound(false), m_excutingTreeTask(0), m_variables(0), m_idFlag(0xffffffff), m_planningTop(-1) {
        bool bOk = TryStart();
        BEHAVIAC_ASSERT(bOk);
        BEHAVIAC_UNUSED_VAR(bOk);

#if !BEHAVIAC_RELEASE
        this->m_debug_verify = 0;
        this->m_debug_in_exec = 0;
#endif//#if !BEHAVIAC_RELEASE
    }

    //m_id == -1, not a valid agent
    Agent::Agent(const Agent& rA) {
		BEHAVIAC_UNUSED_VAR(rA);
		BEHAVIAC_ASSERT(false);
    }

    void Agent::SetName(const char* instanceName) {
        if (!instanceName) {
            uint32_t	typeId = 0;
            const char* typeFullName = this->GetObjectTypeName();

            const char* typeName = typeFullName;
            const char* pIt = strrchr(typeFullName, ':');

            if (pIt) {
                typeName = pIt + 1;
            }

            if (!ms_agent_type_index) {
                ms_agent_type_index = BEHAVIAC_NEW AgentTypeIndexMap_t;
            }

            AgentTypeIndexMap_t::iterator it = ms_agent_type_index->find(typeFullName);

            if (it == ms_agent_type_index->end()) {
                typeId = 0;
                (*ms_agent_type_index)[typeFullName] = 1;
            } else {
                typeId = (*ms_agent_type_index)[typeFullName]++;
            }

            char temp[1024];

            string_sprintf(temp, "%s_%d_%d", typeName, typeId, this->m_id);

            this->m_name += temp;
        } else {
            this->m_name = instanceName;
        }
    }

    Agent::~Agent() {
#if BEHAVIAC_ENABLE_NETWORKD
        this->UnSubsribeToNetwork();
#endif//#if BEHAVIAC_ENABLE_NETWORKD

#if !BEHAVIAC_RELEASE
        Agent::Agents_t* agents = Agents(true);

        if (agents) {
            const char* agentClassName = this->GetObjectTypeName();
            const behaviac::string& instanceName = this->GetName();

            char aName[1024];
            string_sprintf(aName, "%s#%s", agentClassName, instanceName.c_str());

            Agent::Agents_t::iterator it = agents->find(aName);

            if (it != agents->end()) {
                agents->erase(it);
            } else {
                for (it = agents->begin(); it != agents->end(); ++it) {
                    Agent* pAgent = it->second;

                    if (pAgent == this) {
                        agents->erase(it);
                        break;
                    }
                }
            }
        }

#endif

        for (BehaviorTreeTasks_t::iterator it = this->m_behaviorTreeTasks.begin(); it != m_behaviorTreeTasks.end(); ++it) {
            BehaviorTreeTask* bt = *it;

            Workspace::GetInstance()->DestroyBehaviorTreeTask(bt, this);
        }

        this->m_behaviorTreeTasks.clear();

        if (this->m_variables != NULL) {
            this->m_variables->Clear(true);
        }
    }

    void Agent::SetVariableFromString(const char* variableName, const char* valueStr) {
        //IInstanceMember* valueMember = AgentMeta::ParseProperty(valueStr);

		//if (valueMember)
		if (valueStr)
		{
			uint32_t variableId = MakeVariableId(variableName);
			IInstantiatedVariable* v = this->GetInstantiatedVariable(variableId);

            if (v != NULL) {
                //v->SetValue(this, (void*)valueMember->GetValueObject(this));
				v->SetValueFromString(valueStr);
                return;
            }

            IProperty* prop = this->GetProperty(variableId);

            if (prop) {
                //prop->SetValueFrom(this, valueMember);
				prop->SetValueFromString(this, valueStr);
            }
        }
    }

    void Agent::destroy_() {
        int contextId = this->GetContextId();
        Context& c = Context::GetContext(contextId);

        c.RemoveAgent(this);

        // It should be deleted absolutely here.
        if (!c.IsExecuting()) {
            delete this;
        }
    }

    void Agent::Init_(int contextId, Agent* pAgent, short priority, const char* agentInstanceName) {
#if !BEHAVIAC_RELEASE
        pAgent->m_debug_verify = kAGENT_DEBUG_VERY;
#endif//#if !BEHAVIAC_RELEASE

        BEHAVIAC_ASSERT(contextId >= 0, "invalid context id");

        pAgent->m_context_id = contextId;
        pAgent->m_id = ms_agent_index++;
        pAgent->m_priority = priority;

        pAgent->SetName(agentInstanceName);
        pAgent->InitVariableRegistry();

        Context& c = Context::GetContext(contextId);
        c.AddAgent(pAgent);

#if !BEHAVIAC_RELEASE
        Agent::Agents_t* agents = Agents(false);
        BEHAVIAC_ASSERT(agents);

		if (agents) {
			const char* agentClassName = pAgent->GetObjectTypeName();
			const behaviac::string& instanceName = pAgent->GetName();

			char aName[1024];
			string_sprintf(aName, "%s#%s", agentClassName, instanceName.c_str());

			(*agents)[aName] = pAgent;
		}
#endif//BEHAVIAC_RELEASE

#if BEHAVIAC_ENABLE_NETWORKD
        pAgent->SubsribeToNetwork();
#endif//#if BEHAVIAC_ENABLE_NETWORKD
    }

    bool Agent::SaveDataToFile(const char* fileName) {
        BEHAVIAC_ASSERT(fileName);
        const char* className = this->GetObjectTypeName();
        XmlNodeReference xmlInfo = CreateXmlNode(className);
        this->SaveToXML(xmlInfo);

        CFileSystem::MakeSureDirectoryExist(fileName);
        return xmlInfo->saveToFile(fileName);
    }

    bool Agent::LoadDataFromFile(const char* fileName) {
        BEHAVIAC_ASSERT(fileName);
        const char* className = this->GetObjectTypeName();
        XmlNodeReference xmlInfo = CreateXmlNode(className);
        CTextNode node(xmlInfo);

        if (node.LoadFromFile(fileName)) {
			CTagObject::Load(this, &node, className);
            return true;
        }

        return false;
    }

    bool Agent::SaveDataToFile(IFile* file) {
        const char* className = this->GetObjectTypeName();
        XmlNodeReference xmlInfo = CreateXmlNode(className);
        this->SaveToXML(xmlInfo);

        return xmlInfo->saveToFile(file);
    }

    bool Agent::LoadDataFromFile(IFile* file) {
        const char* className = this->GetObjectTypeName();
        XmlNodeReference xmlInfo = CreateXmlNode(className);
        CTextNode node(xmlInfo);

        if (node.LoadFromFile(file)) {
			CTagObject::Load(this, &node, className);
            return true;
        }

        return false;
    }

    void behaviac::Agent::LoadFromXML(const behaviac::XmlConstNodeRef& xmlNode) {
        behaviac::CTextNode textNode(xmlNode);
		const char* className = this->GetObjectTypeName();

		CTagObject::Load(this, &textNode, className);
    }

    void behaviac::Agent::SaveToXML(const behaviac::XmlNodeReference& xmlNode) {
        behaviac::CTextNode textNode(xmlNode);
		const char* className = this->GetObjectTypeName();

		CTagObject::Save(this, &textNode, className);
    }

    behaviac::map<uint32_t, IInstantiatedVariable*> Agent::GetCustomizedVariables() {
        const char* agentClassName = this->GetObjectTypeName();
        uint32_t agentClassId = MakeVariableId(agentClassName);
        AgentMeta* meta = AgentMeta::GetMeta(agentClassId);
        behaviac::map<uint32_t, IInstantiatedVariable*> vars;

        if (meta != NULL) {
            vars = meta->InstantiateCustomizedProperties();

            return vars;
        }

        BEHAVIAC_ASSERT(false);
        return vars;
    }

    AgentState* Agent::GetVariables() {
        if (m_variables == NULL) {
            behaviac::map<uint32_t, IInstantiatedVariable*> vars = this->GetCustomizedVariables();
            this->m_variables = BEHAVIAC_NEW AgentState(vars);
        }

        return this->m_variables;
    }

    bool Agent::NameComparator::operator()(const Agent::AgentName_t* _left, const Agent::AgentName_t* _right) {
        return _left->instantceName_ < _right->instantceName_;
    }

    struct ObjectDescriptorComparator {
        bool operator()(const char* _left, const char* _right) {
            if (string_icmp(_left, _right) < 0) {
                return true;
            }

            return false;
        }
    };

    void Agent::Cleanup() {
#if !BEHAVIAC_RELEASE

        if (ms_agents) {
            ms_agents->clear();
            BEHAVIAC_DELETE(ms_agents);
            ms_agents = 0;
        }

#endif//

        if (ms_names) {
            ms_names->clear();
            BEHAVIAC_DELETE(ms_names);
            ms_names = 0;
        }

        if (ms_agent_type_index) {
            ms_agent_type_index->clear();
            BEHAVIAC_DELETE(ms_agent_type_index);
            ms_agent_type_index = 0;
        }

        //enums meta
        CleanupEnumValueNameMaps();

        BEHAVIAC_DELETE(Agent::ms_factory);
        Agent::ms_factory = 0;

        Agent::ms_idMask = 0;
    }

#if BEHAVIAC_ENABLE_NETWORKD
    void Agent::SubsribeToNetwork() {
        behaviac::Network* pNw = behaviac::Network::GetInstance();

        if (pNw && !pNw->IsSinglePlayer()) {
        }
    }

    void Agent::UnSubsribeToNetwork() {
        behaviac::Network* pNw = behaviac::Network::GetInstance();

        if (pNw && !pNw->IsSinglePlayer()) {
        }
    }

    void Agent::ReplicateProperties() {
        behaviac::Network* pNw = behaviac::Network::GetInstance();

        if (pNw && !pNw->IsSinglePlayer()) {
            const CTagObjectDescriptor& od = this->GetDescriptor();
            od.ReplicateProperties(this);
        }
    }
#endif//#if BEHAVIAC_ENABLE_NETWORKD

    void Agent::LogVariables(bool bForce) {
        BEHAVIAC_UNUSED_VAR(bForce);
#if !BEHAVIAC_RELEASE

        if (Config::IsLoggingOrSocketing()) {
            BEHAVIAC_PROFILE("Agent::LogVariables");

            this->GetVariables()->Log(this, bForce);
            //property
            const char* className = this->GetObjectTypeName();
            uint32_t classId = MakeVariableId(className);
            AgentMeta* meta = AgentMeta::GetMeta(classId);

            if (meta != NULL) {
                //local var
                if (this->m_excutingTreeTask != NULL) {
                    for (behaviac::map<uint32_t, IInstantiatedVariable*>::iterator it =
                             this->m_excutingTreeTask->m_localVars.begin();
                         it != this->m_excutingTreeTask->m_localVars.end(); ++it) {
                        IInstantiatedVariable* pVar = it->second;
                        pVar->Log(this);
                    }
                }

                //customized property
                this->m_variables->Log(this, false);

                //member property
                behaviac::map<uint32_t, IProperty*> memberProperties = meta->GetMemberProperties();

                for (behaviac::map<uint32_t, IProperty*>::iterator it = memberProperties.begin();
                     it != memberProperties.end();
                     ++it) {
                    IProperty* pProperty = it->second;
                    uint32_t id = it->first;

                    if (!pProperty->IsArrayItem()) {
                        bool bNew = false;
                        IValue* pVar = NULL;

                        if (this->_members.find(id) != this->_members.end()) {
                            pVar = this->_members[id];
                        } else {
                            bNew = true;
                            pVar = pProperty->CreateIValue();
                            this->_members[id] = pVar;
                        }

                        pVar->Log(this, pProperty->Name(), bNew);
                    }
                }

            }

        }

#endif//BEHAVIAC_RELEASE
    }

	void Agent::LogRunningNodes()
	{
#if !BEHAVIAC_RELEASE
		if (Config::IsLoggingOrSocketing() && this->m_currentBT != NULL)
		{
			behaviac::vector<BehaviorTask*> runningNodes = this->m_currentBT->GetRunningNodes(false);

			for (unsigned int i = 0; i < runningNodes.size(); ++i)
			{
				string btStr = BehaviorTask::GetTickInfo(this, runningNodes[i], "enter");

				//empty btStr is for internal BehaviorTreeTask
				if (!StringUtils::IsNullOrEmpty(btStr.c_str()))
				{
					LogManager::GetInstance()->Log(this, btStr.c_str(), EAR_success, ELM_tick);
				}
			}
		}
#endif//BEHAVIAC_RELEASE
	}

    void Agent::ResetChangedVariables() {
        //TODO: Reset() method removed, the below code need to remove
        //this->m_variables.Reset();
    }

    void Agent::InitVariableRegistry() {
        this->ResetChangedVariables();

#if !BEHAVIAC_RELEASE
        if (Config::IsLoggingOrSocketing()) {
            const char* className = this->GetObjectTypeName();
            CPropertyNode properyNode(this, className);

			CTagObject::Save(this, &properyNode, className);
        }
#endif
    }

    void Agent::UpdateVariableRegistry() {
        //#if !BEHAVIAC_RELEASE
        //		if (Config::IsLoggingOrSocketing())
        //		{
        //			BEHAVIAC_PROFILE("Agent::UpdateVariableRegistry");
        //
        //			const char* className = this->GetObjectTypeName();
        //
        //			CPropertyNode properyNode(this, className);
        //
        //			this->Save(&properyNode);
        //		}
        //#endif
#if BEHAVIAC_ENABLE_NETWORKD
        this->ReplicateProperties();
#endif//#if BEHAVIAC_ENABLE_NETWORKD
    }
    const void* Agent::GetValueObject(IInstantiatedVariable* var) {
        return  var->GetValueObject(this);
    }
    struct bt_finder {
        behaviac::string bt;

        bool operator()(const behaviac::string& it) const {
            return it == bt;
        }

        bt_finder(const char* btName) : bt(btName)
        {}
    };

    bool IsValidPath(const char* relativePath);

    void Agent::_btsetcurrent(const char* relativePath, TriggerMode triggerMode, bool bByEvent) {
        bool bEmptyPath = (!relativePath || *relativePath == '\0');
        BEHAVIAC_ASSERT(bEmptyPath || behaviac::StringUtils::FindExtension(relativePath) == 0);
        BEHAVIAC_ASSERT(IsValidPath(relativePath));

        if (!bEmptyPath) {
            //if (this->m_currentBT != 0 && this->m_currentBT->GetName() == relativePath)
            //{
            //	//the same bt is set again
            //	return;
            //}

            bool bLoaded = Workspace::GetInstance()->Load(relativePath);

            if (!bLoaded) {
                behaviac::string agentName = this->GetClassTypeName();
                agentName += "::";
                agentName += this->m_name;

                BEHAVIAC_ASSERT(false);
                BEHAVIAC_LOGINFO("%s is not a valid loaded behavior tree of %s", relativePath, agentName.c_str());
            } else {
                Workspace::GetInstance()->RecordBTAgentMapping(relativePath, this);

                if (this->m_currentBT) {
                    //if trigger mode is 'return', just push the current bt 'oldBt' on the stack and do nothing more
                    //'oldBt' will be restored when the new triggered one ends
                    if (triggerMode == TM_Return) {
                        BehaviorTreeStackItem_t item(this->m_currentBT, triggerMode, bByEvent);
                        BEHAVIAC_ASSERT(this->m_btStack.size() < 200, "recursive?");
                        this->m_btStack.push_back(item);
                    } else if (triggerMode == TM_Transfer) {
                        //don't use the bt stack to restore, we just abort the current one.
                        //as the bt node has onenter/onexit, the abort can make them paired
                        //BEHAVIAC_ASSERT(this->m_currentBT->GetName() != relativePath);

                        this->m_currentBT->abort(this);
                        this->m_currentBT->reset(this);
                    }
                }

                BehaviorTreeTask* pTask = 0;

                for (BehaviorTreeTasks_t::iterator it = this->m_behaviorTreeTasks.begin(); it != this->m_behaviorTreeTasks.end(); ++it) {
                    BehaviorTreeTask* bt = *it;
                    BEHAVIAC_ASSERT(bt);

                    if (bt->GetName() == relativePath) {
                        pTask = bt;
                        break;
                    }
                }

                bool bRecursive = false;

                if (pTask) {
                    for (BehaviorTreeStack_t::iterator it = this->m_btStack.begin(); it != this->m_btStack.end(); ++it) {
                        BehaviorTreeStackItem_t& item = *it;

                        if (item.bt->GetName() == relativePath) {
                            bRecursive = true;
                            break;
                        }
                    }

                    if (pTask->GetStatus() != BT_INVALID) {
                        pTask->reset(this);
                    }
                }

                if (pTask == 0 || bRecursive) {
                    pTask = Workspace::GetInstance()->CreateBehaviorTreeTask(relativePath);
                    BEHAVIAC_ASSERT(pTask != 0);
                    this->m_behaviorTreeTasks.push_back(pTask);
                }

                this->_setCurrentTreeTask(pTask);

                //this->_balckboard_bound = false;
                //this->GetVariables()->Clear(false);
            }
        } else {
            BEHAVIAC_ASSERT(true);
        }
    }

    void Agent::btsetcurrent(const char* relativePath) {
        this->_btsetcurrent(relativePath, TM_Transfer, false);
    }

    void Agent::btreferencetree(const char* relativePath) {
        this->m_referencetree = true;
        this->_btsetcurrent(relativePath, TM_Return, false);
    }

    void Agent::bteventtree(const char* relativePath, TriggerMode triggerMode) {
        this->_btsetcurrent(relativePath, triggerMode, true);
    }

    void Agent::btresetcurrent() {
        if (this->m_currentBT != 0) {
            this->m_currentBT->reset(this);
        }
    }

    EBTStatus Agent::btexec_() {
        if (this->m_currentBT != NULL) {
            BehaviorTreeTask* pCurrent = this->m_currentBT;
            EBTStatus s = this->m_currentBT->exec(this);

            while (s != BT_RUNNING) {
                //this->m_currentBT->reset(this);
                if (this->m_btStack.size() > 0) {
                    //get the last one
                    BehaviorTreeStackItem_t& lastOne = this->m_btStack.back();
                    this->m_btStack.pop_back();
                    this->_setCurrentTreeTask(lastOne.bt);

                    bool bExecCurrent = false;

                    if (lastOne.triggerMode == TM_Return) {
                        if (!lastOne.triggerByEvent) {
                            if (this->m_currentBT != pCurrent) {
                                s = this->m_currentBT->resume(this, s);
                            } else {
                                BEHAVIAC_ASSERT(true);
                            }
                        } else {
                            bExecCurrent = true;
                        }
                    } else {
                        bExecCurrent = true;
                    }

                    if (bExecCurrent) {
                        pCurrent = this->m_currentBT;
                        s = this->m_currentBT->exec(this);
                        break;
                    }
                } else {
                    //don't clear it
                    //this->m_currentBT = 0;
                    break;
                }
            }

			if (s != BT_RUNNING) {
				this->m_excutingTreeTask = 0;
			}

            return s;
        } else {
            //BEHAVIAC_LOGWARNING("NO ACTIVE BT!\n");
        }

        return BT_INVALID;
    }

    EBTStatus Agent::btexec() {
#if BEHAVIAC_ENABLE_PROFILING
        BEHAVIAC_PROFILE("Agent::btexec");
#endif

        if (this->m_bActive) {
#if !BEHAVIAC_RELEASE
            BEHAVIAC_ASSERT(this->m_debug_verify == kAGENT_DEBUG_VERY, "Agent can only be created by Agent::Create or Agent::Create!");
            this->m_debug_count = 0;
            this->m_debug_in_exec = 1;
#endif//#if !BEHAVIAC_RELEASE

            this->UpdateVariableRegistry();

            EBTStatus s = this->btexec_();

            while (this->m_referencetree && s == BT_RUNNING) {
                this->m_referencetree = false;
                s = this->btexec_();
            }

            if (this->IsMasked()) {
                this->LogVariables(false);
            }

#if !BEHAVIAC_RELEASE
            this->m_debug_in_exec = 0;
#endif//#if !BEHAVIAC_RELEASE

            return s;
        }

        return BT_INVALID;
    }

    void Agent::btonevent(const char* btEvent, behaviac::map<uint32_t, IInstantiatedVariable*>* eventParams) {
        if (this->m_currentBT != NULL) {
            const char* agentClassName = this->GetObjectTypeName();
            uint32_t agentClassId = MakeVariableId(agentClassName);
            AgentMeta* meta = AgentMeta::GetMeta(agentClassId);

            if (meta != NULL) {
                uint32_t eventId = MakeVariableId(btEvent);
                IInstanceMember* e = meta->GetMethod(eventId);

                if (e != NULL) {
#if !BEHAVIAC_RELEASE
                    BEHAVIAC_ASSERT(this->m_debug_in_exec == 0, "FireEvent should not be called during the Agent is in btexec");

#endif
                    this->m_currentBT->onevent(this, btEvent, eventParams);
                } else {
                    BEHAVIAC_ASSERT(false, "unregistered event %s", btEvent);
                }
            }
        }
    }

    BehaviorTreeTask* Agent::btgetcurrent() {
        return m_currentBT;
    }

    const BehaviorTreeTask* Agent::btgetcurrent() const {
        return m_currentBT;
    }

    bool Agent::btload(const char* relativePath, bool bForce) {
        bool bOk = Workspace::GetInstance()->Load(relativePath, bForce);

        if (bOk) {
            Workspace::GetInstance()->RecordBTAgentMapping(relativePath, this);
        }

        return bOk;
    }

    void Agent::btunload(const char* relativePath) {
        BEHAVIAC_ASSERT(behaviac::StringUtils::FindExtension(relativePath) == 0, "no extention to specify");
        BEHAVIAC_ASSERT(IsValidPath(relativePath));

        //clear the current bt if it is the current bt
        if (this->m_currentBT && this->m_currentBT->GetName() == relativePath) {
            const BehaviorNode* btNode = this->m_currentBT->GetNode();
            BEHAVIAC_ASSERT(BehaviorTree::DynamicCast(btNode) != 0);
            const BehaviorTree* bt = (const BehaviorTree*)btNode;
            this->btunload_pars(bt);

            this->_setCurrentTreeTask(0);
        }

        //remove it from stack
        for (BehaviorTreeStack_t::iterator it = this->m_btStack.begin(); it != this->m_btStack.end(); ++it) {
            BehaviorTreeStackItem_t& item = *it;

            if (item.bt->GetName() == relativePath) {
                this->m_btStack.erase(it);
                break;
            }
        }

        for (BehaviorTreeTasks_t::iterator iti = this->m_behaviorTreeTasks.begin(); iti != m_behaviorTreeTasks.end(); ++iti) {
            BehaviorTreeTask* task = *iti;

            if (task->GetName() == relativePath) {
                Workspace::GetInstance()->DestroyBehaviorTreeTask(task, this);

                this->m_behaviorTreeTasks.erase(iti);
                break;
            }
        }

        Workspace::GetInstance()->UnLoad(relativePath);
    }

    void Agent::bthotreloaded(const BehaviorTree* bt) {
        this->btunload_pars(bt);
    }

    void Agent::btunload_pars(const BehaviorTree* bt) {
        if (bt->m_localProps.size() > 0) {
            BehaviorTree* bt_ = (BehaviorTree*)bt;

            for (BehaviorTree::Properties_t::iterator it = bt_->m_localProps.begin(); it != bt_->m_localProps.end(); ++it) {
                IProperty* property_ = it->second;

                BEHAVIAC_DELETE(property_);
            }

            bt_->m_localProps.clear();
        }
    }

    void Agent::btunloadall() {
        vector<const BehaviorTree*> bts;

        for (BehaviorTreeTasks_t::iterator it = this->m_behaviorTreeTasks.begin(); it != m_behaviorTreeTasks.end(); ++it) {
            BehaviorTreeTask* btTask = *it;

            const BehaviorNode* btNode = btTask->GetNode();
            BEHAVIAC_ASSERT(BehaviorTree::DynamicCast(btNode) != 0);
            const BehaviorTree* bt = (const BehaviorTree*)btNode;
            bool bFound = false;

            for (uint32_t i = 0; i < bts.size(); ++i) {
                const BehaviorTree* it1 = bts[i];

                if (it1 == bt) {
                    bFound = true;
                    break;
                }
            }

            if (!bFound) {
                bts.push_back(bt);
            }

            Workspace::GetInstance()->DestroyBehaviorTreeTask(btTask, this);
        }

        //for (uint32_t i = 0; i < bts.size(); ++i) {
        //    const BehaviorTree* it = bts[i];

        //    this->btunload_pars(it);

        //    const behaviac::string& btName = it->GetName();
        //    Workspace::GetInstance()->UnLoad(btName.c_str());
        //}

        this->m_behaviorTreeTasks.clear();

        this->_setCurrentTreeTask(0);
        this->m_btStack.clear();

        this->GetVariables()->Unload();
        this->_balckboard_bound = false;
    }

    void Agent::btreloadall() {
        this->_setCurrentTreeTask(0);
        this->m_btStack.clear();

        behaviac::vector<behaviac::string> bts;

        //collect the bts
        for (BehaviorTreeTasks_t::iterator it = this->m_behaviorTreeTasks.begin(); it != m_behaviorTreeTasks.end(); ++it) {
            BehaviorTreeTask* bt = *it;

            const behaviac::string& btName = bt->GetName();
            bool bFound = false;

            for (unsigned int i = 0; i < bts.size(); ++i) {
                if (bts[i] == btName) {
                    bFound = true;
                    break;
                }
            }

            if (!bFound) {
                bts.push_back(btName);
            }

            Workspace::GetInstance()->DestroyBehaviorTreeTask(bt, this);
        }

        for (unsigned int i = 0; i < bts.size(); ++i) {
            const behaviac::string& btName = bts[i];
            Workspace::GetInstance()->Load(btName.c_str(), true);
        }

        this->m_behaviorTreeTasks.clear();
        this->GetVariables()->Unload();
    }

    IProperty* Agent::GetProperty(uint32_t propId) const {
        const char* className = GetObjectTypeName();
        uint32_t classId = MakeVariableId(className);
        AgentMeta* meta = AgentMeta::GetMeta(classId);

        if (meta != NULL) {
            IProperty* prop = meta->GetProperty(propId);

            if (prop != NULL) {
                return prop;
            }
        }

        return NULL;
    }

    Agent* Agent::GetInstance(const Agent* pSelf, const char* agentInstanceName) {
        Agent* pParent = (Agent*)pSelf;

		if (agentInstanceName[0] != '\0' && !StringUtils::StringEqual(agentInstanceName, "Self")) {
            // global
            pParent = Agent::GetInstance(agentInstanceName, pSelf ? pSelf->GetContextId() : 0);

            // member
            if (!pParent && pSelf) {
                pParent = (Agent*)pSelf->GetVariable<Agent*>(agentInstanceName);
            }

            if (!pParent) {
                char errorInfo[1024] = { 0 };
                sprintf(errorInfo, "[instance] The instance \"%s\" can not be found, so please check the Agent::BindInstance(...) method has been called for this instance.\n", agentInstanceName);

                BEHAVIAC_ASSERT(false, errorInfo);

                LogManager::GetInstance()->Log(errorInfo);

                printf("%s", errorInfo);
            }
        }

        return pParent;
    }

    Agent* Agent::GetInstance(const char* agentInstanceName, int contextId) {
        Context& c = Context::GetContext(contextId);

        return c.GetInstance(agentInstanceName);
    }

    bool Agent::IsInstanceNameRegistered(const char* agentInstanceName) {
        AgentNames_t::iterator it = Agent::Names().find(agentInstanceName);

        if (it != Agent::Names().end()) {
            return true;
        }

        return false;
    }

    const char* Agent::GetRegisteredClassName(const char* agentInstanceName) {
        AgentNames_t::iterator it = Agent::Names().find(agentInstanceName);

        if (it != Agent::Names().end()) {
            AgentName_t& agentName = it->second;

            return agentName.classFullName_.c_str();
        }

        return 0;
    }

    bool Agent::BindInstance(Agent* pAgentInstance, const char* agentInstanceName, int contextId) {
        Context& c = Context::GetContext(contextId);

        if (!agentInstanceName) {
            agentInstanceName = pAgentInstance->GetClassTypeName();
        }

        return c.BindInstance(agentInstanceName, pAgentInstance);
    }

    bool Agent::UnbindInstance(const char* agentInstanceName, int contextId) {
        Context& c = Context::GetContext(contextId);

        return c.UnbindInstance(agentInstanceName);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    //behaviac::CMethodBase* Agent::CreateMethod(const CStringCRC& agentClassId, const CStringCRC& methodClassId)
    //{
    //    AgentMetas_t::iterator it = Agent::ms_metas->find(agentClassId);

    //    if (it != Agent::ms_metas->end())
    //    {
    //        MetaInfo_t& m = it->second;
    //        const CTagObjectDescriptor* pObejctDesc = m.descriptor;

    //        const CTagObjectDescriptor::MethodsContainer& methods = pObejctDesc->ms_methods;

    //        for (CTagObjectDescriptor::MethodsContainer::const_iterator it1 = methods.begin();
    //             it1 != methods.end(); ++it1)
    //        {
    //            const behaviac::CMethodBase* pM = *it1;

    //            if (pM->GetID().GetID() == methodClassId)
    //            {
    //                return pM->clone();
    //            }
    //        }
    //    }

    //    return 0;
    //}


    static const size_t kNameLength = 256;
    static const char* ParsePropertyNames(const char* fullPropertnName, char* agentClassName) {
        //test_ns::AgentActionTest::Property1
        const char* pBeginAgentClass = fullPropertnName;

        const char* pBeginProperty = strrchr(pBeginAgentClass, ':');

        if (pBeginProperty) {
            //skip '::'
            BEHAVIAC_ASSERT(pBeginProperty[0] == ':' && pBeginProperty[-1] == ':');
            pBeginProperty += 1;

            size_t pos = pBeginProperty - 2 - pBeginAgentClass;
            BEHAVIAC_ASSERT(pos < kNameLength);

            string_ncpy(agentClassName, pBeginAgentClass, pos);
            agentClassName[pos] = '\0';

            return pBeginProperty;
        }

        return 0;
    }

    const behaviac::IProperty* Agent::FindMemberBase(const char* propertyName) {
        char agentClassName[kNameLength];
        const char* propertyBaseName = ParsePropertyNames(propertyName, agentClassName);

        if (propertyBaseName) {
            CStringCRC agentClassId(agentClassName);
            CStringCRC propertyId(propertyBaseName);

            const behaviac::IProperty* m = FindMemberBase(agentClassId, propertyId);
            return m;
        }

        return 0;
    }

    const behaviac::IInstanceMember* Agent::FindMethodBase(const char* propertyName) {
        char agentClassName[kNameLength];
        const char* propertyBaseName = ParsePropertyNames(propertyName, agentClassName);

        if (propertyBaseName) {
            CStringCRC agentClassId(agentClassName);
            CStringCRC propertyId((propertyBaseName));

            const behaviac::IInstanceMember* m = FindMethodBase(agentClassId, propertyId);
            return m;
        }

        return 0;
    }

    const behaviac::IProperty* Agent::FindMemberBase(const CStringCRC& agentClassId, const CStringCRC& propertyId) {
        AgentMeta* pAgentMeta = AgentMeta::GetMeta(agentClassId.GetUniqueID());

        if (pAgentMeta) {
            IProperty* pProperty = pAgentMeta->GetMemberProperty(propertyId.GetUniqueID());
            return pProperty;
        }

        return 0;
    }

    const behaviac::IInstanceMember* Agent::FindMethodBase(const CStringCRC& agentClassId, const CStringCRC& propertyId) {
        AgentMeta* pAgentMeta = AgentMeta::GetMeta(agentClassId.GetUniqueID());

        if (pAgentMeta) {
            IInstanceMember* pMethod = pAgentMeta->GetMethod(propertyId.GetUniqueID());
            return pMethod;
        }

        return 0;
    }

    IInstantiatedVariable* Agent::CreateProperty(const char* agentClassName, const char* propertyName, const char* defaultValue) {
		BEHAVIAC_UNUSED_VAR(defaultValue);
		CStringCRC agentClassId(agentClassName);
        CStringCRC propertyId((propertyName));

        const behaviac::IProperty* pProperty = Agent::FindMemberBase(agentClassId, propertyId);

        if (pProperty) {
            IInstantiatedVariable* pInstantiatedVariable = pProperty->Instantiate();
            BEHAVIAC_ASSERT(0);
            //pInstantiatedVariable->SetInitialValue(defaultValue);

            return pInstantiatedVariable;
        }

        return 0;
    }

    //const CTagObjectDescriptor* Agent::GetDescriptorByName(const char* agentTypeClass)
    //{
    //    CStringCRC agentTypeClassId(agentTypeClass);

    //    AgentMetas_t::iterator it = Agent::ms_metas->find(agentTypeClassId);

    //    if (it != Agent::ms_metas->end())
    //    {
    //        return it->second.descriptor;
    //    }

    //    return 0;
    //}

    bool Agent::btsave(State_t& state) {
#if BEHAVIAC_ENABLE_PROFILING
        BEHAVIAC_PROFILE("Agent::btsave");
#endif
        state.m_agentType = this->GetObjectTypeName();
        this->GetVariables()->CopyTo(0, state.m_vars);

        if (this->m_currentBT) {
            Workspace::GetInstance()->DestroyBehaviorTreeTask(state.m_bt, this);

            const BehaviorNode* pNode = this->m_currentBT->GetNode();
            state.m_bt = (BehaviorTreeTask*)pNode->CreateAndInitTask();
            this->m_currentBT->CopyTo(state.m_bt);

            return true;
        }

        return false;
    }

    Agent* Agent::GetParentAgent(const Agent* pAgent, const char* instanceName) {
        Agent* pParent = const_cast<Agent*>(pAgent);

        if (!StringUtils::IsNullOrEmpty(instanceName) && !StringUtils::Compare(instanceName, "Self")) {
            pParent = Agent::GetInstance(instanceName, (pParent != NULL) ? pParent->GetContextId() : 0);

            //if (pAgent != NULL && pParent == NULL && !Utils.IsStaticClass(instanceName))
            if (pAgent != NULL && pParent == NULL /*&& !Utils.IsStaticClass(instanceName)*/) { //TODO how to handle Statice Class
                pParent = (Agent*)pAgent->GetVariable<Agent*>(instanceName);
                BEHAVIAC_ASSERT(pParent != NULL);
            }
        }

        return pParent;
    }

    bool Agent::btload(const State_t& state) {
#if BEHAVIAC_ENABLE_PROFILING
        BEHAVIAC_PROFILE("Agent::btload");
#endif
        state.m_vars.CopyTo(this, *this->m_variables);

        if (state.m_bt) {
            if (this->m_currentBT) {
                for (BehaviorTreeTasks_t::iterator iti = this->m_behaviorTreeTasks.begin(); iti != m_behaviorTreeTasks.end(); ++iti) {
                    BehaviorTreeTask* task = *iti;

                    if (task == this->m_currentBT) {
                        Workspace::GetInstance()->DestroyBehaviorTreeTask(task, this);

                        this->m_behaviorTreeTasks.erase(iti);
                        break;
                    }
                }
            }

            const BehaviorNode* pNode = state.m_bt->GetNode();
            this->m_currentBT = (BehaviorTreeTask*)pNode->CreateAndInitTask();
            state.m_bt->CopyTo(this->m_currentBT);

            return true;
        }

        return false;
    }

#if !BEHAVIAC_RELEASE
    Agent::Agents_t* Agent::ms_agents = 0;
    Agent::Agents_t* Agent::Agents(bool bCleanup) {
        if (!bCleanup) {
            if (!ms_agents) {
                ms_agents = BEHAVIAC_NEW Agent::Agents_t;
            }

            return ms_agents;
        }

        if (ms_agents) {
            return ms_agents;
        }

        return 0;
    }

    Agent* Agent::GetAgent(const char* agentName) {
        Agent* pAgent = Agent::GetInstance(agentName, 0);

        if (pAgent) {
            return pAgent;
        }

        Agent::Agents_t* agents = Agents(false);
        BEHAVIAC_ASSERT(agents);
		if (agents) {
			Agent::Agents_t::iterator it = agents->find(agentName);

			if (it != agents->end()) {
				Agent* pA = it->second;
				return pA;
			}
		}

        return 0;
    }
#endif//BEHAVIAC_RELEASE

    void Agent::LogMessage(const char* message) {
        int frames = behaviac::Workspace::GetInstance()->GetFrameSinceStartup();
        BEHAVIAC_LOGINFO("[%d]%s\n", frames, message);
    }

    int Agent::VectorLength(const IList& vector) {
        int n = vector.size();

        (*(IList*)&vector).release();

        return n;
    }

    void Agent::VectorAdd(IList& vector, const System::Object& element) {
        vector.add(element);
        vector.release();
    }

    void Agent::VectorRemove(IList& vector, const System::Object& element) {
        vector.remove(element);
        vector.release();
    }

    bool Agent::VectorContains(IList& vector, const System::Object& element) {
        bool bOk = vector.contains(element);
        vector.release();

        return bOk;
    }

    void Agent::VectorClear(IList& vector) {
        vector.clear();
        vector.release();
    }
}
