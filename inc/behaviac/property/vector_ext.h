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

#ifndef _BEHAVIAC_VECTOR_EXT_H_
#define _BEHAVIAC_VECTOR_EXT_H_

#include "behaviac/property/operators.inl"

//simulate c#'s IList and System.Object
namespace System {
    struct BEHAVIAC_API Object {
    private:
        void* data;
    public:
        Object() : data(0) {
        }

        Object(const Object& c) : data(c.data) {

        }

        BEHAVIAC_DECLARE_OBJECT(System::Object);
    } BEHAVIAC_ALIAS;
}

struct BEHAVIAC_API IList {
    BEHAVIAC_DECLARE_OBJECT(IList);

    virtual ~IList() {}

    struct IListPool {
        virtual ~IListPool() {}
        virtual void cleanup() = 0;
    };

    static behaviac::vector<IListPool**>* ms_pools;
    static behaviac::vector<IListPool**>& GetPools();

    static void Cleanup();

    virtual int size() const {
        BEHAVIAC_ASSERT(false);
        return 0;
    }

    virtual void add(const System::Object& o) {
        BEHAVIAC_UNUSED_VAR(o);
        BEHAVIAC_ASSERT(false);
    }

    virtual void remove(const System::Object& o) {
        BEHAVIAC_UNUSED_VAR(o);
        BEHAVIAC_ASSERT(false);
    }

    virtual void clear() {
        BEHAVIAC_ASSERT(false);
    }

    virtual bool contains(const System::Object& o) {
        BEHAVIAC_UNUSED_VAR(o);
        BEHAVIAC_ASSERT(false);
        return false;
    }

    virtual void release() {
        BEHAVIAC_ASSERT(false);
    }
};

template<typename T>
struct TList : public IList {
private:
    template <typename TYPE>
    struct find_predcate {
        const TYPE& d;
        find_predcate(const TYPE& d_) : d(d_) {

        }

        find_predcate(const find_predcate& c) : d(c.d)
        {}

        bool operator()(const TYPE& v) {
            if (behaviac::PrivateDetails::Equal(d, v)) {
                return true;
            }

            return false;
        }

        bool operator>(const TYPE& v) {
            if (behaviac::PrivateDetails::Less(d, v)) {
                return true;
            }

            return false;
        }

        find_predcate& operator=(const find_predcate& c) {
            this->d = c.d;

            return *this;
        }
    };

public:
    typedef typename behaviac::Meta::IsVector<T>::ElementType ElementType;
    BEHAVIAC_DECLARE_OBJECT(TList);
    bool bRelease;

    TList(T* pVector) : bRelease(false), vector_(pVector) { }
    TList(T* pVector, bool bRelease_) : bRelease(bRelease_), vector_(pVector) { }

    ~TList() { }

    behaviac::vector<ElementType>* vector_;

    virtual int size() const {
        return (int)this->vector_->size();
    }

    virtual void add(const System::Object& o) {
        const ElementType& d = *(ElementType*)&o;
        this->vector_->push_back(d);
    }

    virtual void remove(const System::Object& o) {
        const ElementType& d = *(ElementType*)&o;

        find_predcate<ElementType> p(d);

        typename behaviac::vector<ElementType>::iterator it = std::find_if(this->vector_->begin(), this->vector_->end(), p);

        if (it != this->vector_->end()) {
            this->vector_->erase(it);
        }
    }

    virtual void clear() {
        this->vector_->clear();
    }

    virtual bool contains(const System::Object& o) {
        const ElementType& d = *(ElementType*)&o;
        find_predcate<ElementType> p(d);
        typename behaviac::vector<ElementType>::iterator it = std::find_if(this->vector_->begin(), this->vector_->end(), p);
        return it != this->vector_->end();
    }

    struct TListPool : public IListPool {
        behaviac::vector<TList<T>*>* pool;

        TListPool() {
            pool = BEHAVIAC_NEW behaviac::vector<TList<T>*>;
        }

        virtual void cleanup() {
            if (this->pool) {
                for (typename behaviac::vector<TList<T>*>::iterator it = this->pool->begin(); it != this->pool->end(); ++it) {
                    TList<T>* pList = *it;
                    BEHAVIAC_DELETE pList;
                }

                this->pool->clear();
                BEHAVIAC_DELETE this->pool;
                this->pool = 0;
            }
        }
    };

    static void* ms_pool;
    static behaviac::Mutex ms_mutex;
    static TListPool& GetListPool() {
        if (!ms_pool) {
            ms_pool = BEHAVIAC_NEW TListPool();
            behaviac::vector<IListPool**>& listPool = IList::GetPools();

            listPool.push_back((IListPool**)&ms_pool);
        }

        return *(TListPool*)ms_pool;
    }

    static IList* CreateList(const T* pVector) {
        TListPool& listPool = GetListPool();
        behaviac::ScopedLock lock(ms_mutex);

        size_t n = listPool.pool->size();

        if (n > 0) {
            // get the last
            TList<T>* pList = (*listPool.pool)[n - 1];

            listPool.pool->pop_back();

            pList->setList((T*)pVector);

            return pList;
        }

        TList* pList = BEHAVIAC_NEW TList<T>((T*)pVector, true);

        return pList;
    }

    void setList(T* pVector) {
        this->vector_ = pVector;
    }

    virtual void release() {
        if (this->bRelease) {
            TListPool& listPool = GetListPool();
            behaviac::ScopedLock lock(ms_mutex);

            listPool.pool->push_back(this);
        }
    }
};

template<typename T>
void* TList<T>::ms_pool = 0;

template<typename T>
behaviac::Mutex TList<T>::ms_mutex;

#endif//_BEHAVIAC_VECTOR_EXT_H_