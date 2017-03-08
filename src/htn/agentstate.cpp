#include "behaviac/agent/agent.h"
#include "behaviac/agent/agentstate.h"
#include "behaviac/common/member.h"

namespace behaviac {

    behaviac::Mutex					AgentState::ms_mutex;
    behaviac::vector<AgentState*>	AgentState::pool;

    AgentState::AgentState() : parent(NULL), m_forced(false), m_pushed(0) {
    }

	AgentState::AgentState(AgentState* _parent) : parent(_parent) {
#if BEHAVIAC_ENABLE_PUSH_OPT
        m_forced = false;
        m_pushed = 0;
#endif
    }

    AgentState::AgentState(behaviac::map<uint32_t, IInstantiatedVariable*> vars) : Variables(vars), parent(0) {
#if BEHAVIAC_ENABLE_PUSH_OPT
        m_forced = false;
        m_pushed = 0;
#endif
    }

    AgentState::~AgentState() {
        this->Pop();
    }

    int AgentState::Depth() {
        size_t d = 1;

        if (this->state_stack.size() > 0) {
            for (int i = (int)this->state_stack.size() - 1; i >= 0; --i) {
                AgentState* t = this->state_stack[i];
#if BEHAVIAC_ENABLE_PUSH_OPT
                d += 1 + t->m_pushed;
#else
                d += 1;
#endif
            }
        }

        return (int)d;
    }

    int AgentState::Top() {
        if (this->state_stack.size() > 0) {
            return (int)this->state_stack.size() - 1;
        }

        return -1;
    }

    AgentState* AgentState::Push(bool bForcePush) {
#if BEHAVIAC_ENABLE_PUSH_OPT

        if (!bForcePush) {
            //if the top has nothing new added, to use it again
            if (this->state_stack.size() > 0) {
                AgentState* t = this->state_stack[this->state_stack.size() - 1];

                if (!t->m_forced && t->Vars().size() == 0) {
                    t->m_pushed++;
                    return t;
                }
            }
        }

#endif

        AgentState* newly = NULL;

        {
            behaviac::ScopedLock lock(ms_mutex);

            if (pool.size() > 0) {
                //last one
                newly = pool[pool.size() - 1];
                pool.pop_back();
                //set the parent
                newly->parent = this;
            } else {
                newly = BEHAVIAC_NEW AgentState(this);
            }

#if BEHAVIAC_ENABLE_PUSH_OPT
            newly->m_forced = bForcePush;
#endif

            if (bForcePush) {
                Variables::CopyTo(NULL, *newly);
            }
        }

        //add the newly one at the end of the list as the top
        this->state_stack.push_back(newly);

        return newly;
    }

    void AgentState::AddVariable(uint32_t varId, IInstantiatedVariable* pVar, int stackIndex) {
		int array_len = (int)this->state_stack.size();

        if (array_len > 0 &&
            stackIndex > 0 && stackIndex < array_len) {
            AgentState* t = this->state_stack[stackIndex];
            t->AddVariable(varId, pVar, -1);
        } else {
            Variables::AddVariable(varId, pVar, -1);
        }
    }

    IInstantiatedVariable* AgentState::GetVariable(uint32_t varId) const {
        if (this->state_stack.size() > 0) {
			for (int i = (int)this->state_stack.size() - 1; i >= 0; --i) {
                AgentState* t = this->state_stack[i];

                IInstantiatedVariable* pVar = t->GetVariable(varId);

                if (pVar != NULL) {
                    return pVar;
                }
            }
        }

        return Variables::GetVariable(varId);
    }

    void AgentState::Pop() {
        if (this->parent == 0) {
            //if parent is 0, it is not created on the heap and not on the stack, not pop
            return;
        }

#if BEHAVIAC_ENABLE_PUSH_OPT

        if (this->m_pushed > 0) {
            this->m_pushed--;

            if (this->m_variables.size() > 0) {
                this->m_variables.clear();
                return;
            }

            return;
        }

#endif

        if (this->state_stack.size() > 0) {
            AgentState* top = this->state_stack[this->state_stack.size() - 1];
            top->Pop();
            return;
        }

        this->Clear(true);
        //Debug.Check(this->state_stack == NULL);
        BEHAVIAC_ASSERT(this->state_stack.size() == 0);
        //Debug.Check(this->parent != NULL);
        BEHAVIAC_ASSERT(this->parent != NULL);
        this->parent->PopTop();
        this->parent = NULL;

        {
            behaviac::ScopedLock lock(ms_mutex);
            //BEHAVIAC_ASSERT(!pool.Contains(this));
            pool.push_back(this);
        }
    }

    void AgentState::PopTop() {
        BEHAVIAC_ASSERT(this->state_stack.size() > 0);
        //remove the last one
        this->state_stack.pop_back();//
    }

    void AgentState::Clear(bool bFull) {
        if (bFull) {
#if BEHAVIAC_ENABLE_PUSH_OPT
            this->m_forced = false;
            this->m_pushed = 0;
#endif

            if (this->state_stack.size() > 0) {
                for (int i = (int)this->state_stack.size() - 1; i >= 0; --i) {
                    AgentState* t = this->state_stack[i];

                    t->Clear(bFull);
                }

                this->state_stack.clear();
            }
        }

        Variables::Clear(bFull);
    }

    //    void AgentState::Log(Agent* pAgent, bool bForce)
    //    {
    //        BEHAVIAC_UNUSED_VAR(pAgent);
    //        BEHAVIAC_UNUSED_VAR(bForce);
    //#if !BEHAVIAC_RELEASE
    //
    //        if (Config::IsLoggingOrSocketing())
    //        {
    //            if (this->state_stack.size() > 0)
    //            {
    //                map<behaviac::string, bool> logged;
    //
    //                for (int i = (int)this->state_stack.size() - 1; i >= 0; --i)
    //                {
    //                    AgentState* t = this->state_stack[i];
    //                    behaviac::map<uint32_t, IInstantiatedVariable*>& _value = t->Vars();
    //
    //                    for (behaviac::map<uint32_t, IInstantiatedVariable*>::iterator it = _value.begin(); it != _value.end(); it++)
    //                    {
    //                        IInstantiatedVariable* pVar = it->second;
    //
    //                        if (bForce || pVar->IsChanged())
    //                        {
    //                            if (logged.find(pVar->Name()) == logged.end())
    //                            {
    //                                pVar->Log(pAgent);
    //                                logged.insert(map<behaviac::string, bool>::value_type(pVar->Name(), true));
    //                            }
    //                        }
    //                    }
    //                }//end of for
    //            }
    //            else
    //            {
    //                //base.Log(pAgent, bForce);
    //                Variables::Log(pAgent, bForce);
    //            }
    //        }
    //
    //#endif
    //        //}
    //    }
    //
}