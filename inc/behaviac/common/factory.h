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

#ifndef _BEHAVIAC_COMMON_FACTORY_H_
#define _BEHAVIAC_COMMON_FACTORY_H_

#include "behaviac/common/config.h"

#include "behaviac/common/thread/mutex_lock.h"
#include "behaviac/common/string/stringcrc.h"

#include "behaviac/common/logger/logger.h"

#include <vector>
#include <algorithm>
namespace behaviac {
    // factory bag
    struct SFactoryBag_t {
        SFactoryBag_t(const CStringCRC& typeID, void* typeConstructor) : m_typeID(typeID), m_typeConstructor(typeConstructor) {}

        bool operator==(const SFactoryBag_t& rhs)const {
            return m_typeID == rhs.m_typeID;
        }

        CStringCRC		m_typeID;
        void*           m_typeConstructor;
    };

    struct FactoryContainer_t {
        std::vector<SFactoryBag_t>						m_vector;
        typedef std::vector<SFactoryBag_t>::iterator	iterator;

        Mutex										m_mutex;

        iterator begin() {
            return m_vector.begin();
        }

        iterator end() {
            return m_vector.end();
        }

        void push_back(SFactoryBag_t it) {
            m_vector.push_back(it);
        }

        void erase(iterator it) {
            m_vector.erase(it);
        }

        void Lock() {
            m_mutex.Lock();
        }

        void Unlock() {
            m_mutex.Unlock();
        }
    };

    typedef FactoryContainer_t::iterator CreatorIt;

    BEHAVIAC_API bool FactoryRegister_(FactoryContainer_t* creators, const CStringCRC& typeID, void* typeConstructor);
    BEHAVIAC_API bool FactoryUnregister_(FactoryContainer_t* creators, const CStringCRC& typeID);

    template< typename T >
    class CFactory {
        class IConstructType {
        public:
            virtual ~IConstructType() {}
            virtual T* Create() = 0;
        };

        template< typename FINAL_TYPE >
        class CConstructType : public CFactory< T >::IConstructType {
        public:
            virtual T* Create() {
                return BEHAVIAC_NEW FINAL_TYPE;
            }
        };

    public:
        virtual ~CFactory() {
            CreatorIt it(creators_.begin());
            CreatorIt itEnd(creators_.end());

            while (it != itEnd) {
                SFactoryBag_t& item = *it++;
                BEHAVIAC_FREE(item.m_typeConstructor);
            }

            creators_.m_vector.clear();
        }

        virtual T* CreateObject(const CStringCRC& typeID);

        typedef T* (*InstantiateFunctionPointer)(const CStringCRC& typeID);

        bool Register(const CStringCRC& typeID, InstantiateFunctionPointer instantiate, bool overwrite = false);
        bool UnRegister(const CStringCRC& typeID);

        template< typename FINAL_TYPE >
        bool Register() {
            typedef CConstructType<FINAL_TYPE> FinalType;
            void* p = BEHAVIAC_MALLOC(sizeof(FinalType));
            IConstructType* typeConstructor = new(p)FinalType;
            return FactoryRegister_(&creators_, FINAL_TYPE::GetClassTypeId(), typeConstructor);
        }

        template< typename FINAL_TYPE >
        bool UnRegister() {
            return FactoryUnregister_(&creators_, FINAL_TYPE::GetClassTypeId());
        }

        bool IsRegistered(const CStringCRC& typeID);

    private:
        FactoryContainer_t creators_;
    };

    template< typename T >
    BEHAVIAC_FORCEINLINE bool CFactory<T>::IsRegistered(const CStringCRC& typeID) {
        creators_.Lock();
        SFactoryBag_t bucket(typeID, NULL);
        CreatorIt itEnd(creators_.end());
        CreatorIt itFound(std::find(creators_.begin(), itEnd, bucket));

        if (itFound != itEnd) {
            creators_.Unlock();
            return true;
        } else {
            creators_.Unlock();
            return false;
        }
    }

    template< typename T >
    T* CFactory<T>::CreateObject(const CStringCRC& typeID) {
        creators_.Lock();
        SFactoryBag_t bucket(typeID, NULL);
        CreatorIt itEnd(creators_.end());
        CreatorIt itFound(std::find(creators_.begin(), itEnd, bucket));

        if (itFound != itEnd) {
            IConstructType* contructType = (IConstructType*)(*itFound).m_typeConstructor;
            creators_.Unlock();
            return contructType->Create();

        } else {
            BEHAVIAC_LOGWARNING("Trying to create an unregister type 0x%08X", typeID.GetUniqueID());

            creators_.Unlock();
            return NULL;
        }
    }

    template< typename T >
    bool CFactory<T>::Register(const CStringCRC& typeID, InstantiateFunctionPointer instantiate, bool overwrite) {
        creators_.Lock();
        BEHAVIAC_ASSERT(instantiate);
        SFactoryBag_t bucket(typeID, (void*)instantiate);
        CreatorIt itEnd(creators_.end());
        CreatorIt itFound(std::find(creators_.begin(), itEnd, bucket));
        bool wasThere = (itFound != itEnd);

        if (!wasThere) {
            creators_.push_back(bucket);

        } else if (overwrite) {
            *itFound = bucket;
            creators_.Unlock();
            return true;

        } else {
            BEHAVIAC_ASSERT(0, "Trying to register an already registered type %d", typeID.GetUniqueID());
        }

        creators_.Unlock();
        return !wasThere;
    }

    template< typename T >
    bool CFactory<T>::UnRegister(const CStringCRC& typeID) {
        creators_.Lock();
        SFactoryBag_t bucket(typeID, NULL);
        CreatorIt itEnd(creators_.end());
        CreatorIt itFound(std::find(creators_.begin(), itEnd, bucket));

        if (itFound != itEnd) {
            creators_.erase(itFound);
            creators_.Unlock();
            return true;
        }

        BEHAVIAC_ASSERT("Cannot find the specified factory entry\n");
        creators_.Unlock();
        return false;
    }
}//
#endif //_BEHAVIAC_COMMON_FACTORY_H_
