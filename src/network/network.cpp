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

#include "behaviac/network/network.h"
#include "behaviac/agent/agent.h"

#if BEHAVIAC_ENABLE_NETWORKD
namespace behaviac {
    Network* Network::ms_pNetwork = 0;

    Network::Network() {
        BEHAVIAC_ASSERT(ms_pNetwork == 0);
        ms_pNetwork = this;
    }

    Network::~Network() {
        BEHAVIAC_ASSERT(ms_pNetwork);
        ms_pNetwork = 0;
    }

    //Network* Network::CreateInstance()
    //{
    //	ms_pNetwork = BEHAVIAC_NEW Network;

    //	return ms_pNetwork;
    //}

    //void Network::DestroyInstance(Network* pInstance)
    //{
    //	BEHAVIAC_ASSERT(pInstance == ms_pNetwork);

    //	if (pInstance == ms_pNetwork)
    //	{
    //		BEHAVIAC_DELETE(ms_pNetwork);
    //		ms_pNetwork = 0;
    //	}
    //}

    Network* Network::GetInstance() {
        Network* pNetwork = Network::ms_pNetwork;

        //BEHAVIAC_ASSERT(pNetwork);

        return pNetwork;
    }

    bool Network::ShouldHandle(behaviac::NetworkRole netRole) {
        if ((!this->IsAuthority() && netRole == NET_ROLE_NONAUTHORITY) ||
            (this->IsAuthority() && netRole == NET_ROLE_AUTHORITY)) {
            return true;
        }

        return false;
    }

    void Network::BindToEvent(behaviac::NetworkRole netRole, const char* eventName, Agent* pAgent, behaviac::CMethodBase* pMethod) {
        BEHAVIAC_ASSERT(netRole != NET_ROLE_DEFAULT && !this->IsSinglePlayer());

        if (this->ShouldHandle(netRole)) {
            RemoteEventInstanceMethods_t::iterator it = m_remoteEventInstanceMethods.find(eventName);

            MethodInstance_t mi(pAgent, pMethod);

            if (it == m_remoteEventInstanceMethods.end()) {
                InstanceMethods_t a;

                a.push_back(mi);
                m_remoteEventInstanceMethods[eventName] = a;

            } else {
                InstanceMethods_t& a = m_remoteEventInstanceMethods[eventName];

                a.push_back(mi);
            }

            this->SubscribeToRemoteEvent(eventName, pAgent, pMethod);
        }
    }

    void Network::UnBindToEvent(behaviac::NetworkRole netRole, const char* eventName, Agent* pAgent) {
        BEHAVIAC_ASSERT(netRole != NET_ROLE_DEFAULT && !this->IsSinglePlayer());

        if (this->ShouldHandle(netRole)) {
            RemoteEventInstanceMethods_t::iterator it = m_remoteEventInstanceMethods.find(eventName);

            if (it != m_remoteEventInstanceMethods.end()) {
                InstanceMethods_t& a = m_remoteEventInstanceMethods[eventName];

                for (InstanceMethods_t::iterator ita = a.begin(); ita != a.end();) {
                    MethodInstance_t& minfo = *ita;

                    if (minfo.agent == pAgent) {
                        InstanceMethods_t::iterator d = ita++;
                        a.erase(d);

                        this->UnSubscribeToRemoteEvent(eventName, pAgent);
                        break;

                    } else {
                        ++ita;
                    }
                }
            }
        }
    }

    void Network::tick(float deltaTime) {
        BEHAVIAC_UNUSED_VAR(deltaTime);
    }
}//namespace behaviac
#endif//#if BEHAVIAC_ENABLE_NETWORKD
