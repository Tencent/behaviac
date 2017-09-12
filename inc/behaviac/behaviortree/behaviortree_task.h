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

#ifndef _BEHAVIAC_BEHAVIORTREE_TASK_H_
#define _BEHAVIAC_BEHAVIORTREE_TASK_H_

#include "behaviac/common/base.h"

#include "behaviac/common/rttibase.h"
#include "behaviac/common/object/tagobject.h"
#include "behaviac/behaviortree/behaviortree.h"

namespace behaviac {
    class Agent;
    class BehaviorNode;
    class BehaviorTask;
    class AttachmentTask;
    class IInstantiatedVariable;
    // ============================================================================

    /**
    return the exit status.

    this function is only valid when it is called inside an 'ExitAction' of any Node.

    it should not be called in any other functions.
    */
    EBTStatus GetNodeExitStatus();

    /**
    return the node id.

    this function is only valid when it is called inside an 'Action' Node.
    other it returns INVALID_NODE_ID.
    */
    int GetNodeId();

    /**
    trigger mode to control the bt switching and back
    */
    enum TriggerMode {
        TM_Transfer,
        TM_Return
    };

    ///return false to stop traversing
    typedef bool(*NodeHandler_t)(BehaviorTask*, Agent*, void* user_data);

    class BranchTask;

    /**
    Base class for the BehaviorTreeTask's runtime execution management.
    */
    class BEHAVIAC_API BehaviorTask : public CRTTIBase {
    public:
        static void DestroyTask(BehaviorTask*);
        static behaviac::string GetTickInfo(const behaviac::Agent* pAgent, const behaviac::BehaviorNode* n, const char* action);
        static behaviac::string GetTickInfo(const behaviac::Agent* pAgent, const behaviac::BehaviorTask* b, const char* action);

    public:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(BehaviorTask);
        BEHAVIAC_DECLARE_ROOT_DYNAMIC_TYPE(BehaviorTask, CRTTIBase);

        virtual void Init(const BehaviorNode* node) = 0;
        virtual void copyto(BehaviorTask* target) const = 0;
        virtual void save(IIONode* node) const = 0;
        virtual void load(IIONode* node) = 0;

        const behaviac::string& GetClassNameString() const;
        uint16_t GetId() const;
        void SetId(uint16_t id);

        EBTStatus exec(Agent* pAgent);
        EBTStatus exec(Agent* pAgent, EBTStatus childStatus);

        behaviac::vector<BehaviorTask*> GetRunningNodes(bool onlyLeaves = true);

        void abort(Agent* pAgent);

        ///reset the status to invalid
        void reset(Agent* pAgent);

        EBTStatus GetStatus() const;

		BehaviorTreeTask* GetRootTask();

        const BehaviorNode* GetNode() const;

        void SetParent(BranchTask* parent) {
            this->m_parent = parent;
        }

        const BranchTask* GetParent() const {
            return this->m_parent;
        }

        BranchTask* GetParent() {
            return this->m_parent;
        }

        void SetHasManagingParent(bool bHasManagingParent) {
            this->m_bHasManagingParent = bHasManagingParent;
        }

        virtual void traverse(bool childFirst, NodeHandler_t handler, Agent* pAgent, void* user_data) = 0;

        virtual void SetCurrentTask(BehaviorTask* node) {
            BEHAVIAC_UNUSED_VAR(node);
        }

        virtual const BehaviorTask* GetCurrentTask() const {
            return 0;
        }

        /**
        return false if the event handling needs to be stopped

        an event can be configured to stop being checked if triggered
        */
        bool CheckEvents(const char* eventName, Agent* pAgent, behaviac::map<uint32_t, IInstantiatedVariable*>* eventParams) const;

        /**
        return false if the event handling  needs to be stopped
        return true, the event hanlding will be checked furtherly
        */
        virtual bool onevent(Agent* pAgent, const char* eventName, behaviac::map<uint32_t, IInstantiatedVariable*>* eventParams);

        virtual const BehaviorTask* GetTaskById(int id) const;
        virtual int GetNextStateId() const;

    protected:
        BehaviorTask();
        virtual ~BehaviorTask();

        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);
        virtual EBTStatus update_current(Agent* pAgent, EBTStatus childStatus);

        virtual void onreset(Agent* pAgent);
        virtual bool onenter(Agent* pAgent);
        virtual void onexit(Agent* pAgent, EBTStatus status);

        void Clear();

    private:
        bool CheckParentUpdatePreconditions(Agent* pAgent);
        BranchTask*		GetTopManageBranchTask();

        friend bool getRunningNodes_handler(BehaviorTask* task, Agent* pAgent, void* user_data);
		friend bool end_handler(BehaviorTask* task, Agent* pAgent, void* user_data);
        friend bool abort_handler(BehaviorTask* task, Agent* pAgent, void* user_data);
        friend bool reset_handler(BehaviorTask* task, Agent* pAgent, void* user_data);
        friend bool checkevent_handler(BehaviorTask* task, Agent* pAgent, void* user_data);

        void Attach(AttachmentTask* pAttachment);

        bool onenter_action(Agent* pAgent);
        void onexit_action(Agent* pAgent, EBTStatus status);

        void FreeAttachments();
    protected:
        EBTStatus				m_status;
        const BehaviorNode* 	m_node;
        BranchTask*				m_parent;
        typedef behaviac::vector<AttachmentTask*> Attachments;
        Attachments*			m_attachments;
        uint16_t				m_id;
        bool					m_bHasManagingParent;
    private:

        //access m_status
        friend class BranchTask;
        friend class DecoratorTask;

        //access update
        friend class BehaviorTreeTask;
    public:
        virtual bool CheckPreconditions(const Agent* pAgent, bool bIsAlive) const;
    };

    // ============================================================================
    class BEHAVIAC_API AttachmentTask : public BehaviorTask {
    public:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(AttachmentTask);
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(AttachmentTask, BehaviorTask);

    protected:
        AttachmentTask();
        virtual ~AttachmentTask();

        virtual void Init(const BehaviorNode* node);
        virtual void copyto(BehaviorTask* target) const;
        virtual void save(IIONode* node) const;
        virtual void load(IIONode* node);
    public:
        virtual void traverse(bool childFirst, NodeHandler_t handler, Agent* pAgent, void* user_data);
    };

    // ============================================================================
    class BEHAVIAC_API LeafTask : public BehaviorTask {
    public:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(LeafTask);
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(LeafTask, BehaviorTask);

        virtual void traverse(bool childFirst, NodeHandler_t handler, Agent* pAgent, void* user_data);
    protected:
        LeafTask();
        virtual ~LeafTask();

        virtual void Init(const BehaviorNode* node);
        virtual void copyto(BehaviorTask* target) const;
        virtual void save(IIONode* node) const;
        virtual void load(IIONode* node);

        virtual bool onevent(Agent* pAgent, const char* eventName, behaviac::map<uint32_t, IInstantiatedVariable*>* eventParams);
    };

    // ============================================================================
    class BEHAVIAC_API BranchTask : public BehaviorTask {
    public:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(BranchTask);
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(BranchTask, BehaviorTask);

        virtual void SetCurrentTask(BehaviorTask* task);

        virtual const BehaviorTask* GetCurrentTask() const {
            return this->m_currentTask;
        }

        int							GetCurrentNodeId();
        void						SetCurrentNodeId(int id);

    protected:
        BranchTask();
        virtual ~BranchTask();

        virtual void Init(const BehaviorNode* node);
        virtual void copyto(BehaviorTask* target) const;
        virtual void save(IIONode* node) const;
        virtual void load(IIONode* node);

        EBTStatus execCurrentTask(Agent* pAgent, EBTStatus childStatus);

        virtual bool onevent(Agent* pAgent, const char* eventName, behaviac::map<uint32_t, IInstantiatedVariable*>* eventParams);

        virtual bool onenter(Agent* pAgent);
        virtual void onexit(Agent* pAgent, EBTStatus s);
        virtual EBTStatus update_current(Agent* pAgent, EBTStatus childStatus);
        EBTStatus resume_branch(Agent* pAgent, EBTStatus status);
    private:
        bool oneventCurrentNode(Agent* pAgent, const char* eventName, behaviac::map<uint32_t, IInstantiatedVariable*>* eventParams);

    protected:
        //bookmark the current ticking node, it is different from m_activeChildIndex
        int					m_currentNodeId;
        BehaviorTask*		m_currentTask;
    };

    // ============================================================================
    class BEHAVIAC_API CompositeTask : public BranchTask {
    public:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(CompositeTask);
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(CompositeTask, BranchTask);

        virtual void traverse(bool childFirst, NodeHandler_t handler, Agent* pAgent, void* user_data);
        BehaviorTask* GetChildById(int nodeId) const;
    protected:
        CompositeTask();
        virtual ~CompositeTask();

        virtual void Init(const BehaviorNode* node);
        virtual void copyto(BehaviorTask* target) const;
        virtual void save(IIONode* node) const;
        virtual void load(IIONode* node);

        virtual void addChild(BehaviorTask* pBehavior);
        virtual const BehaviorTask* GetTaskById(int id) const;
    protected:
        typedef behaviac::vector<BehaviorTask*> BehaviorTasks_t;
        BehaviorTasks_t			m_children;

        //book mark the current child
        int						m_activeChildIndex;
        static int				InvalidChildIndex;
    };

    // ============================================================================
    class BEHAVIAC_API SingeChildTask : public BranchTask {
    public:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(SingeChildTask);
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(SingeChildTask, BranchTask);

        virtual void traverse(bool childFirst, NodeHandler_t handler, Agent* pAgent, void* user_data);
    protected:
        SingeChildTask();
        virtual ~SingeChildTask();

        virtual void Init(const BehaviorNode* node);
        virtual void copyto(BehaviorTask* target) const;
        virtual void save(IIONode* node) const;
        virtual void load(IIONode* node);

        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);

        virtual void addChild(BehaviorTask* pBehavior);

        virtual const BehaviorTask* GetTaskById(int id) const;
    protected:
        BehaviorTask*	m_root;
    };

    // ============================================================================
    class BEHAVIAC_API DecoratorTask : public SingeChildTask {
    public:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(DecoratorTask);
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(DecoratorTask, SingeChildTask);

    protected:
        DecoratorTask();
        virtual ~DecoratorTask();

        virtual void Init(const BehaviorNode* node);
        virtual void copyto(BehaviorTask* target) const;
        virtual void save(IIONode* node) const;
        virtual void load(IIONode* node);

        virtual bool onenter(Agent* pAgent);
        virtual EBTStatus update_current(Agent* pAgent, EBTStatus childStatus);
        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);

        /**
        called when the child's exec returns success or failure.
        please note, it is not called if the child's exec returns running
        */
        virtual EBTStatus decorate(EBTStatus status) = 0;

    private:
        bool m_bDecorateWhenChildEnds;
    };

    // ============================================================================
    class BEHAVIAC_API BehaviorTreeTask : public SingeChildTask {
    public:
        behaviac::map<uint32_t, IInstantiatedVariable*> m_localVars;

    public:
        void SetRootTask(BehaviorTask* pRoot);

        void CopyTo(BehaviorTreeTask* target);

        void Save(IIONode* node) const;
        void Load(IIONode* node);

        EBTStatus resume(Agent* pAgent, EBTStatus status);

        template<typename VariableType>
        void SetVariable(const char* variableName, VariableType value);

        /**
        return the path relative to the workspace path
        */
        void AddVariables(behaviac::map<uint32_t, IInstantiatedVariable*>* vars);
        const behaviac::string& GetName() const;

        void Clear();

		void setEndStatus(EBTStatus status);

	private:
		void end(Agent* pAgent, EBTStatus status);

    protected:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(BehaviorTreeTask);
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(BehaviorTreeTask, SingeChildTask);

        BehaviorTreeTask();
        virtual ~BehaviorTreeTask();

        virtual void Init(const BehaviorNode* node);
        virtual void copyto(BehaviorTask* target) const;
        virtual void save(IIONode* node) const;
        virtual void load(IIONode* node);

        virtual bool onenter(Agent* pAgent);
        virtual void onexit(Agent* pAgent, EBTStatus s);

        virtual EBTStatus update_current(Agent* pAgent, EBTStatus childStatus);
        virtual EBTStatus update(Agent* pAgent, EBTStatus childStatus);

        //virtual bool NeedRestart() const;

        /**
        return false if the event handling  needs to be stopped
        return true, the event hanlding will be checked furtherly
        */
        virtual bool onevent(Agent* pAgent, const char* eventName, behaviac::map<uint32_t, IInstantiatedVariable*>* eventParams);

    private:
        bool load(const char* file);

		BehaviorTreeTask*	m_lastTreeTask;
		EBTStatus			m_endStatus;
    };
} // namespace behaviac

#endif//_BEHAVIAC_BEHAVIORTREE_TASK_H_
