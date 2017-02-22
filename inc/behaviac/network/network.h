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

#ifndef _BEHAVIAC_NETWORK_H_
#define _BEHAVIAC_NETWORK_H_

#include "behaviac/common/base.h"
#include "behaviac/common/rttibase.h"


namespace behaviac {
    class CMethodBase;

    /*! \addtogroup Network
    * @{ */
    enum NetworkRole {
        //executed on the bt side(usually it is the authority/server side), that is it is not networked
        NET_ROLE_DEFAULT,

        //executed only on the authority/server side, no matter which side the bt is executed
        NET_ROLE_AUTHORITY,

        //executed only on the non-authority/client side, no matter which side the bt is executed
        NET_ROLE_NONAUTHORITY
    };
#if BEHAVIAC_ENABLE_NETWORKD
    class IAny {
    public:
        IAny(int typeId) : typeId_(typeId)
        {}

        int GetType() const {
            return typeId_;
        }

        virtual void* GetData() const = 0;

        template<typename T>
        bool GetValue(T& v) {
            int type = this->GetType();

            typedef PARAM_BASETYPE(T) BaseType;
            int typeReturn = GetClassTypeNumberId<BaseType>();

            if (type == typeReturn) {
                void* pD = this->GetData();
                const BaseType& d = *(BaseType*)pD;

                v = d;

                return true;
            }

            return false;
        }

    protected:
        int typeId_;
    };

    template<typename T, bool bPtr, bool bAgent>
    class Any_t : public IAny {
    public:
        typedef PARAM_BASETYPE(T) BaseType;
        Any_t() : IAny(GetClassTypeNumberId<BaseType>())
        {}

        Any_t(T d) : IAny(GetClassTypeNumberId<BaseType>()), data(d) {
        }

        virtual void* GetData() const {
            return (void*)&this->data;
        }

        void SetValue(const BaseType& v) {
            data = v;
        }

    protected:
        BaseType	data;
    };

    template<typename T, bool bAgent>
    class Any_t<T, true, bAgent> : public IAny {
    public:
        typedef PARAM_BASETYPE(T) BaseType;

        Any_t() : IAny(GetClassTypeNumberId<BaseType>())
        {}

        Any_t(T d) : IAny(GetClassTypeNumberId<BaseType>()), data(*d) {
        }

        virtual void* GetData() const {
            return (void*)&this->data;
        }

        void SetValue(const BaseType* v) {
            data = *v;
        }

    protected:
        BaseType	data;
    };

    template<>
    class Any_t<const char*, true, false> : public IAny {
    public:
        typedef behaviac::string BaseType;

        Any_t() : IAny(GetClassTypeNumberId<BaseType>())
        {}

        Any_t(const char* d) : IAny(GetClassTypeNumberId<BaseType>()), data(d) {
        }

        virtual void* GetData() const {
            return (void*)&this->data;
        }

        void SetValue(const char* v) {
            data = v;
        }

    protected:
        BaseType	data;
    };

#define ANYTYPE(P)	behaviac::Any_t<P, behaviac::Meta::IsPtr<P>::Result, behaviac::Meta::IsRefType<P>::Result>

    typedef behaviac::vector<IAny*> Variants_t;

    class Agent;

    /**
    a singleton to deal with remote method register/sending.

    'remote events' are automatically registered and dispatched by the reflection system.
    however, in your derivative, you need to provide a function to subsribe to those 'registered remote events' in order to
    respond to it. it might look like:

    template<typename ObjectType>
    void SubscribeToEvent(const char* eventName, ObjectType* o,
    void (ObjectType::*methodPtr)(ustl::StringHash, ustl::VariantMap&))
    {
    o->SubscribeToEvent(StringHash(eventName), new ustl::EventHandlerImpl<ObjectType>(o, methodPtr));
    }

    you need to override it to provide the implementation to:
    virtual void SubscribeToEvent(const char* eventName, behaviac::Agent* pAgent, behaviac::CMethodBase* pMethod) = 0;
    virtual void UnSubscribeToEvent(const char* eventName, behaviac::Agent* pAgent) = 0;

    virtual bool IsSinglePlayer() = 0;
    virtual bool IsAuthority() = 0;

    virtual void RegisterRemoteEvent(behaviac::NetworkRole netRole, const char* evtName) = 0;

    virtual void ReplicateVariable(const char* className, const char* variableName, int typeId, void* pData) = 0;
    virtual void ReplicateVariable(behaviac::Agent* pAgent, const char* variableName, int typeId, void* pData) = 0;

    virtual void SendRemoteEvent(behaviac::NetworkRole netRole, const char* eventName, const behaviac::Variants_t& p) = 0;
    */
    class BEHAVIAC_API Network {
    private:
        virtual void SubscribeToRemoteEvent(const char* eventName, behaviac::Agent* pAgent, behaviac::CMethodBase* pMethod) = 0;
        virtual void UnSubscribeToRemoteEvent(const char* eventName, behaviac::Agent* pAgent) = 0;

    public:
        virtual bool IsSinglePlayer() = 0;
        virtual bool IsAuthority() = 0;

        virtual void RegisterRemoteEvent(behaviac::NetworkRole netRole, const char* evtName) = 0;

        virtual void ReplicateVariable(const char* className, const char* variableName, int typeId, void* pData, bool bSend) = 0;
        virtual void ReplicateVariable(behaviac::Agent* pAgent, const char* variableName, int typeId, void* pData, bool bSend) = 0;

        virtual void RegisterReplicatedProperty(behaviac::NetworkRole netRole, const char* evtName) {
            BEHAVIAC_UNUSED_VAR(netRole);
            BEHAVIAC_UNUSED_VAR(evtName);
        }

        /**
        @param bSendToNonAuthority
        indicate the sending direction, if true, send to nonauthority/client, if false, send to authority/server
        */
        virtual void SendRemoteEvent(behaviac::NetworkRole netRole, bool bSendToNonAuthority, const char* eventName, const behaviac::Variants_t& p) = 0;

        bool SendRemoteEvent(NetworkRole netRole, const char* eventName) {
            BEHAVIAC_ASSERT(!this->IsSinglePlayer());

            bool bSendToNonAuthority = true;

            if (netRole == behaviac::NET_ROLE_AUTHORITY) {
                if (!this->IsAuthority()) {
                    //event needs to run on authority and this is client
                    bSendToNonAuthority = false;
                    Variants_t vs;
                    this->SendRemoteEvent(netRole, bSendToNonAuthority, eventName, vs);

                    return true;
                }
            } else if (netRole == behaviac::NET_ROLE_NONAUTHORITY) {
                if (this->IsAuthority()) {
                    //event needs to run on non-authority and this is authority
                    Variants_t vs;
                    this->SendRemoteEvent(netRole, bSendToNonAuthority, eventName, vs);

                    return true;
                }
            }

            return false;
        }

        template<typename P>
        bool SendRemoteEvent(NetworkRole netRole, const char* eventName,
                             P param) {
            BEHAVIAC_ASSERT(!this->IsSinglePlayer());

            bool bSendToNonAuthority = true;

            if (netRole == behaviac::NET_ROLE_AUTHORITY) {
                if (!this->IsAuthority()) {
                    //event needs to run on authority and this is client
                    bSendToNonAuthority = false;
                    Variants_t vs;
                    ANYTYPE(P) p(param);
                    vs.push_back(&p);

                    this->SendRemoteEvent(netRole, bSendToNonAuthority, eventName, vs);

                    return true;
                }
            } else if (netRole == behaviac::NET_ROLE_NONAUTHORITY) {
                if (this->IsAuthority()) {
                    //event needs to run on non-authority and this is authority
                    Variants_t vs;
                    ANYTYPE(P) p(param);
                    vs.push_back(&p);

                    this->SendRemoteEvent(netRole, bSendToNonAuthority, eventName, vs);

                    return true;
                }
            }

            return false;
        }

        template<typename P1, typename P2>
        bool SendRemoteEvent(NetworkRole netRole, const char* eventName,
                             P1 param1, P2 param2) {
            BEHAVIAC_ASSERT(!this->IsSinglePlayer());

            bool bSendToNonAuthority = true;

            if (netRole == behaviac::NET_ROLE_AUTHORITY) {
                if (!this->IsAuthority()) {
                    //event needs to run on authority and this is client
                    bSendToNonAuthority = false;
                    Variants_t vs;
                    ANYTYPE(P1) p1(param1);
                    vs.push_back(&p1);
                    ANYTYPE(P2) p2(param2);
                    vs.push_back(&p2);

                    this->SendRemoteEvent(netRole, bSendToNonAuthority, eventName, vs);

                    return true;
                }
            } else if (netRole == behaviac::NET_ROLE_NONAUTHORITY) {
                if (this->IsAuthority()) {
                    //event needs to run on non-authority and this is authority
                    Variants_t vs;
                    ANYTYPE(P1) p1(param1);
                    vs.push_back(&p1);
                    ANYTYPE(P2) p2(param2);
                    vs.push_back(&p2);

                    this->SendRemoteEvent(netRole, bSendToNonAuthority, eventName, vs);

                    return true;
                }
            }

            return false;
        }

        template<typename P1, typename P2, typename P3>
        bool SendRemoteEvent(NetworkRole netRole, const char* eventName,
                             P1 param1, P2 param2, P3 param3) {
            BEHAVIAC_ASSERT(!this->IsSinglePlayer());

            bool bSendToNonAuthority = true;

            if (netRole == behaviac::NET_ROLE_AUTHORITY) {
                if (!this->IsAuthority()) {
                    //event needs to run on authority and this is client
                    bSendToNonAuthority = false;
                    Variants_t vs;
                    ANYTYPE(P1) p1(param1);
                    vs.push_back(&p1);
                    ANYTYPE(P2) p2(param2);
                    vs.push_back(&p2);
                    ANYTYPE(P3) p3(param3);
                    vs.push_back(&p3);

                    this->SendRemoteEvent(netRole, bSendToNonAuthority, eventName, vs);

                    return true;
                }
            } else if (netRole == behaviac::NET_ROLE_NONAUTHORITY) {
                if (this->IsAuthority()) {
                    //event needs to run on non-authority and this is authority
                    Variants_t vs;
                    ANYTYPE(P1) p1(param1);
                    vs.push_back(&p1);
                    ANYTYPE(P2) p2(param2);
                    vs.push_back(&p2);
                    ANYTYPE(P3) p3(param3);
                    vs.push_back(&p3);

                    this->SendRemoteEvent(netRole, bSendToNonAuthority, eventName, vs);

                    return true;
                }
            }

            return false;
        }

        template<typename P1, typename P2, typename P3, typename P4>
        bool SendRemoteEvent(NetworkRole netRole, const char* eventName,
                             P1 param1, P2 param2, P3 param3, P4 param4) {
            BEHAVIAC_ASSERT(!this->IsSinglePlayer());

            bool bSendToNonAuthority = true;

            if (netRole == behaviac::NET_ROLE_AUTHORITY) {
                if (!this->IsAuthority()) {
                    //event needs to run on authority and this is client
                    bSendToNonAuthority = false;
                    Variants_t vs;
                    ANYTYPE(P1) p1(param1);
                    vs.push_back(&p1);
                    ANYTYPE(P2) p2(param2);
                    vs.push_back(&p2);
                    ANYTYPE(P3) p3(param3);
                    vs.push_back(&p3);
                    ANYTYPE(P4) p4(param4);
                    vs.push_back(&p4);

                    this->SendRemoteEvent(netRole, bSendToNonAuthority, eventName, vs);

                    return true;
                }
            } else if (netRole == behaviac::NET_ROLE_NONAUTHORITY) {
                if (this->IsAuthority()) {
                    //event needs to run on non-authority and this is authority
                    Variants_t vs;
                    ANYTYPE(P1) p1(param1);
                    vs.push_back(&p1);
                    ANYTYPE(P2) p2(param2);
                    vs.push_back(&p2);
                    ANYTYPE(P3) p3(param3);
                    vs.push_back(&p3);
                    ANYTYPE(P4) p4(param4);
                    vs.push_back(&p4);

                    this->SendRemoteEvent(netRole, bSendToNonAuthority, eventName, vs);

                    return true;
                }
            }

            return false;
        }

        template<typename P1, typename P2, typename P3, typename P4, typename P5>
        bool SendRemoteEvent(NetworkRole netRole, const char* eventName,
                             P1 param1, P2 param2, P3 param3, P4 param4, P5 param5) {
            BEHAVIAC_ASSERT(!this->IsSinglePlayer());

            bool bSendToNonAuthority = true;

            if (netRole == behaviac::NET_ROLE_AUTHORITY) {
                if (!this->IsAuthority()) {
                    //event needs to run on authority and this is client
                    bSendToNonAuthority = false;
                    Variants_t vs;
                    ANYTYPE(P1) p1(param1);
                    vs.push_back(&p1);
                    ANYTYPE(P2) p2(param2);
                    vs.push_back(&p2);
                    ANYTYPE(P3) p3(param3);
                    vs.push_back(&p3);
                    ANYTYPE(P4) p4(param4);
                    vs.push_back(&p4);
                    ANYTYPE(P5) p5(param5);
                    vs.push_back(&p5);

                    this->SendRemoteEvent(netRole, bSendToNonAuthority, eventName, vs);

                    return true;
                }
            } else if (netRole == behaviac::NET_ROLE_NONAUTHORITY) {
                if (this->IsAuthority()) {
                    //event needs to run on non-authority and this is authority
                    Variants_t vs;
                    ANYTYPE(P1) p1(param1);
                    vs.push_back(&p1);
                    ANYTYPE(P2) p2(param2);
                    vs.push_back(&p2);
                    ANYTYPE(P3) p3(param3);
                    vs.push_back(&p3);
                    ANYTYPE(P4) p4(param4);
                    vs.push_back(&p4);
                    ANYTYPE(P5) p5(param5);
                    vs.push_back(&p5);

                    this->SendRemoteEvent(netRole, bSendToNonAuthority, eventName, vs);

                    return true;
                }
            }

            return false;
        }

        template<typename P1, typename P2, typename P3, typename P4, typename P5, typename P6>
        bool SendRemoteEvent(NetworkRole netRole, const char* eventName,
                             P1 param1, P2 param2, P3 param3, P4 param4, P5 param5, P6 param6) {
            BEHAVIAC_ASSERT(!this->IsSinglePlayer());

            bool bSendToNonAuthority = true;

            if (netRole == behaviac::NET_ROLE_AUTHORITY) {
                if (!this->IsAuthority()) {
                    //event needs to run on authority and this is client
                    bSendToNonAuthority = false;
                    Variants_t vs;
                    ANYTYPE(P1) p1(param1);
                    vs.push_back(&p1);
                    ANYTYPE(P2) p2(param2);
                    vs.push_back(&p2);
                    ANYTYPE(P3) p3(param3);
                    vs.push_back(&p3);
                    ANYTYPE(P4) p4(param4);
                    vs.push_back(&p4);
                    ANYTYPE(P5) p5(param5);
                    vs.push_back(&p5);
                    ANYTYPE(P6) p6(param6);
                    vs.push_back(&p6);

                    this->SendRemoteEvent(netRole, bSendToNonAuthority, eventName, vs);

                    return true;
                }
            } else if (netRole == behaviac::NET_ROLE_NONAUTHORITY) {
                if (this->IsAuthority()) {
                    //event needs to run on non-authority and this is authority
                    Variants_t vs;
                    ANYTYPE(P1) p1(param1);
                    vs.push_back(&p1);
                    ANYTYPE(P2) p2(param2);
                    vs.push_back(&p2);
                    ANYTYPE(P3) p3(param3);
                    vs.push_back(&p3);
                    ANYTYPE(P4) p4(param4);
                    vs.push_back(&p4);
                    ANYTYPE(P5) p5(param5);
                    vs.push_back(&p5);
                    ANYTYPE(P6) p6(param6);
                    vs.push_back(&p6);

                    this->SendRemoteEvent(netRole, bSendToNonAuthority, eventName, vs);

                    return true;
                }
            }

            return false;
        }

        template<typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7>
        bool SendRemoteEvent(NetworkRole netRole, const char* eventName,
                             P1 param1, P2 param2, P3 param3, P4 param4, P5 param5, P6 param6, P7 param7) {
            BEHAVIAC_ASSERT(!this->IsSinglePlayer());

            bool bSendToNonAuthority = true;

            if (netRole == behaviac::NET_ROLE_AUTHORITY) {
                if (!this->IsAuthority()) {
                    //event needs to run on authority and this is client
                    bSendToNonAuthority = false;
                    Variants_t vs;
                    ANYTYPE(P1) p1(param1);
                    vs.push_back(&p1);
                    ANYTYPE(P2) p2(param2);
                    vs.push_back(&p2);
                    ANYTYPE(P3) p3(param3);
                    vs.push_back(&p3);
                    ANYTYPE(P4) p4(param4);
                    vs.push_back(&p4);
                    ANYTYPE(P5) p5(param5);
                    vs.push_back(&p5);
                    ANYTYPE(P6) p6(param6);
                    vs.push_back(&p6);
                    ANYTYPE(P7) p7(param7);
                    vs.push_back(&p7);

                    this->SendRemoteEvent(netRole, bSendToNonAuthority, eventName, vs);

                    return true;
                }
            } else if (netRole == behaviac::NET_ROLE_NONAUTHORITY) {
                if (this->IsAuthority()) {
                    //event needs to run on non-authority and this is authority
                    Variants_t vs;
                    ANYTYPE(P1) p1(param1);
                    vs.push_back(&p1);
                    ANYTYPE(P2) p2(param2);
                    vs.push_back(&p2);
                    ANYTYPE(P3) p3(param3);
                    vs.push_back(&p3);
                    ANYTYPE(P4) p4(param4);
                    vs.push_back(&p4);
                    ANYTYPE(P5) p5(param5);
                    vs.push_back(&p5);
                    ANYTYPE(P6) p6(param6);
                    vs.push_back(&p6);
                    ANYTYPE(P7) p7(param7);
                    vs.push_back(&p7);

                    this->SendRemoteEvent(netRole, bSendToNonAuthority, eventName, vs);

                    return true;
                }
            }

            return false;
        }

        template<typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8>
        bool SendRemoteEvent(NetworkRole netRole, const char* eventName,
                             P1 param1, P2 param2, P3 param3, P4 param4, P5 param5, P6 param6, P7 param7, P8 param8) {
            BEHAVIAC_ASSERT(!this->IsSinglePlayer());

            bool bSendToNonAuthority = true;

            if (netRole == behaviac::NET_ROLE_AUTHORITY) {
                if (!this->IsAuthority()) {
                    //event needs to run on authority and this is client
                    bSendToNonAuthority = false;
                    Variants_t vs;
                    ANYTYPE(P1) p1(param1);
                    vs.push_back(&p1);
                    ANYTYPE(P2) p2(param2);
                    vs.push_back(&p2);
                    ANYTYPE(P3) p3(param3);
                    vs.push_back(&p3);
                    ANYTYPE(P4) p4(param4);
                    vs.push_back(&p4);
                    ANYTYPE(P5) p5(param5);
                    vs.push_back(&p5);
                    ANYTYPE(P6) p6(param6);
                    vs.push_back(&p6);
                    ANYTYPE(P7) p7(param7);
                    vs.push_back(&p7);
                    ANYTYPE(P8) p8(param8);
                    vs.push_back(&p8);

                    this->SendRemoteEvent(netRole, bSendToNonAuthority, eventName, vs);

                    return true;
                }
            } else if (netRole == behaviac::NET_ROLE_NONAUTHORITY) {
                if (this->IsAuthority()) {
                    //event needs to run on non-authority and this is authority
                    Variants_t vs;
                    ANYTYPE(P1) p1(param1);
                    vs.push_back(&p1);
                    ANYTYPE(P2) p2(param2);
                    vs.push_back(&p2);
                    ANYTYPE(P3) p3(param3);
                    vs.push_back(&p3);
                    ANYTYPE(P4) p4(param4);
                    vs.push_back(&p4);
                    ANYTYPE(P5) p5(param5);
                    vs.push_back(&p5);
                    ANYTYPE(P6) p6(param6);
                    vs.push_back(&p6);
                    ANYTYPE(P7) p7(param7);
                    vs.push_back(&p7);
                    ANYTYPE(P8) p8(param8);
                    vs.push_back(&p8);

                    this->SendRemoteEvent(netRole, bSendToNonAuthority, eventName, vs);

                    return true;
                }
            }

            return false;
        }

        void tick(float deltaTime);
    public:
        //static Network* CreateInstance();
        //static void DestroyInstance();
        static Network* GetInstance();

    protected:
        Network();
        bool ShouldHandle(behaviac::NetworkRole netRole);

    public:
        virtual ~Network();

        void BindToEvent(behaviac::NetworkRole netRole, const char* eventName, Agent* pAgent, behaviac::CMethodBase* pMethod);
        void UnBindToEvent(behaviac::NetworkRole netRole, const char* eventName, Agent* pAgent);

    private:
        static Network* ms_pNetwork;

    protected:
        struct MethodInstance_t {
            Agent*			agent;
            behaviac::CMethodBase*	method;

            MethodInstance_t() : agent(0), method(0)
            {}

            MethodInstance_t(Agent* a, behaviac::CMethodBase* m) : agent(a), method(m)
            {}

            MethodInstance_t(const MethodInstance_t& copy) : agent(copy.agent), method(copy.method)
            {}
        };

        typedef behaviac::vector<MethodInstance_t>					InstanceMethods_t;
        typedef behaviac::map<behaviac::string, InstanceMethods_t>	RemoteEventInstanceMethods_t;

        RemoteEventInstanceMethods_t	m_remoteEventInstanceMethods;
    };
#endif//BEHAVIAC_ENABLE_NETWORKD
    /*! @} */
}//namespace behaviac

#endif//_BEHAVIAC_NETWORK_H_
