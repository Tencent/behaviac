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

#ifndef _TEST_BEHAVIOR_TEST_H_
#define _TEST_BEHAVIOR_TEST_H_

#include "behaviac/common/base.h"
#include "behaviac/agent/agent.h"

#include "behaviac/behaviortree/behaviortree.h"

#include "behaviac/behaviortree/nodes/decorators/decoratorloop.h"
#include "behaviac/behaviortree/nodes/decorators/decoratorsuccessuntil.h"
#include "behaviac/behaviortree/nodes/decorators/decoratorfailureuntil.h"
#include "behaviac/behaviortree/nodes/decorators/decoratoralwaysrunning.h"
#include "behaviac/behaviortree/nodes/actions/noop.h"

#include "behaviac/behaviortree/nodes/conditions/condition.h"
#include "behaviac/common/meta.h"

//behaviac::Property* LoadRight(const char* value, const behaviac::string& propertyName, behaviac::string& typeName);

class BEHAVIAC_API DecoratorLoopTaskMask : public behaviac::DecoratorLoopTask
{
    bool m_success;
public:
    BEHAVIAC_DECLARE_DYNAMIC_TYPE(DecoratorLoopTaskMask, behaviac::DecoratorLoopTask);

    DecoratorLoopTaskMask(bool s) : behaviac::DecoratorLoopTask(), m_success(s)
    {
    }

    virtual behaviac::EBTStatus decorate(behaviac::EBTStatus status)
    {
        behaviac::EBTStatus s = super::decorate(status);

        if (s == behaviac::BT_SUCCESS)
        {
            return m_success ? behaviac::BT_SUCCESS : behaviac::BT_FAILURE;
        }

        if (s == behaviac::BT_FAILURE)
        {
            return m_success ? behaviac::BT_FAILURE : behaviac::BT_SUCCESS;
        }

        return s;
    }
};

class DecoratorCountMock : public behaviac::DecoratorCount
{
    bool m_success;
public:
    DecoratorCountMock(int count, bool s = true) : m_success(s)
    {
		char temp[1024];

        string_sprintf(temp, "const int %d", count);
        behaviac::string typeName;
        behaviac::string propertyName;

		this->m_count = behaviac::AgentMeta::ParseProperty(temp);

        behaviac::Noop* pNoop = BEHAVIAC_NEW behaviac::Noop();

        this->AddChild(pNoop);
    }

    virtual behaviac::BehaviorTask* createTask() const
    {
        behaviac::BehaviorTask* pTask = BEHAVIAC_NEW DecoratorLoopTaskMask(m_success);
        pTask->Init(this);

        return pTask;
    }

    virtual bool IsValid(behaviac::Agent* pAgent, behaviac::BehaviorTask* pTask) const
    {
        BEHAVIAC_UNUSED_VAR(pAgent);
        BEHAVIAC_UNUSED_VAR(pTask);

        return true;
    }
};

class FailureAfter : public behaviac::DecoratorLoopTask
{
public:
    FailureAfter(const DecoratorCountMock* node) : behaviac::DecoratorLoopTask()
    {
        this->m_node = node;

        behaviac::NoopTask* pNoop = BEHAVIAC_NEW behaviac::NoopTask();

        this->addChild(pNoop);
    }

    ~FailureAfter()
    {
    }

    virtual behaviac::EBTStatus decorate(behaviac::EBTStatus status)
    {
        behaviac::EBTStatus s = behaviac::DecoratorLoopTask::decorate(status);

        if (s == behaviac::BT_SUCCESS)
        {
            return behaviac::BT_FAILURE;
        }

        if (s == behaviac::BT_FAILURE)
        {
            return behaviac::BT_SUCCESS;
        }

        return s;
    }
};

class SuccessAfter : public behaviac::DecoratorLoopTask
{
public:
    SuccessAfter(const DecoratorCountMock* node) : behaviac::DecoratorLoopTask()
    {
        this->m_node = node;
        behaviac::NoopTask* pNoop = BEHAVIAC_NEW behaviac::NoopTask();

        this->addChild(pNoop);
    }

    ~SuccessAfter()
    {
    }

    virtual behaviac::EBTStatus decorate(behaviac::EBTStatus status)
    {
        return behaviac::DecoratorLoopTask::decorate(status);
    }
};

class FailureUntil : public behaviac::DecoratorFailureUntilTask
{
public:
    FailureUntil(const DecoratorCountMock* node) : behaviac::DecoratorFailureUntilTask()
    {
        this->m_node = node;
        behaviac::NoopTask* pNoop = BEHAVIAC_NEW behaviac::NoopTask();

        this->addChild(pNoop);
    }

    ~FailureUntil()
    {
    }

    virtual behaviac::EBTStatus decorate(behaviac::EBTStatus status)
    {
        return behaviac::DecoratorFailureUntilTask::decorate(status);
    }
};

class SuccessUntil : public behaviac::DecoratorSuccessUntilTask
{
public:
    SuccessUntil(const DecoratorCountMock* node) : behaviac::DecoratorSuccessUntilTask()
    {
        this->m_node = node;
        behaviac::NoopTask* pNoop = BEHAVIAC_NEW behaviac::NoopTask();

        this->addChild(pNoop);
    }

    ~SuccessUntil()
    {
    }

    virtual behaviac::EBTStatus decorate(behaviac::EBTStatus status)
    {
        return behaviac::DecoratorSuccessUntilTask::decorate(status);
    }
};

class AlwaysRunning : public behaviac::DecoratorAlwaysRunningTask
{
public:
    AlwaysRunning() : behaviac::DecoratorAlwaysRunningTask()
    {
        behaviac::NoopTask* pNoop = BEHAVIAC_NEW behaviac::NoopTask();

        this->addChild(pNoop);
    }

    ~AlwaysRunning()
    {
    }

    virtual behaviac::EBTStatus decorate(behaviac::EBTStatus status)
    {
        return behaviac::DecoratorAlwaysRunningTask::decorate(status);
    }
};

#endif//_TEST_BEHAVIOR_TEST_H_
