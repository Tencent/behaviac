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

#ifndef _BEHAVIAC_COMMON_MEMBER_H_
#define _BEHAVIAC_COMMON_MEMBER_H_

#include "behaviac/common/config.h"
#include "behaviac/common/assert.h"
#include "behaviac/common/base.h"
#include "behaviac/common/meta.h"
#include "behaviac/agent/agent.h"
#include "behaviac/common/operation.h"
#include "behaviac/common/logger/logmanager.h"

namespace behaviac {
    class BehaviorTreeTask;
    class IIONode;

    template <typename T>
    class CVariable;

    template<typename T>
    class CArrayItemVariable;

    template<typename T, bool bStruct>
    struct CastValue {
        static void get_as(int typeId, const void* pValueAddr, T& value) {
			BEHAVIAC_UNUSED_VAR(typeId);
			BEHAVIAC_UNUSED_VAR(pValueAddr);
			BEHAVIAC_UNUSED_VAR(value);
			BEHAVIAC_ASSERT(false);
        }
    };

    template<typename T>
    struct CastValue<T, false> {
        static void get_as(int typeId, const void* pValueAddr, T& value) {
            if (typeId == GetClassTypeNumberId<char>()) {
                value = (T) * (char*)pValueAddr;
            } else if (typeId == GetClassTypeNumberId<signed char>()) {
                value = (T) * (signed char*)pValueAddr;
            } else if (typeId == GetClassTypeNumberId<unsigned char>()) {
                value = (T) * (unsigned char*)pValueAddr;
            } else if (typeId == GetClassTypeNumberId<short>()) {
                value = (T) * (short*)pValueAddr;
            } else if (typeId == GetClassTypeNumberId<unsigned short>()) {
                value = (T) * (unsigned short*)pValueAddr;
            } else if (typeId == GetClassTypeNumberId<int>()) {
                value = (T) * (int*)pValueAddr;
            } else if (typeId == GetClassTypeNumberId<unsigned int>()) {
                value = (T) * (unsigned int*)pValueAddr;
            } else if (typeId == GetClassTypeNumberId<long>()) {
                value = (T) * (long*)pValueAddr;
            } else if (typeId == GetClassTypeNumberId<unsigned long>()) {
                value = (T) * (unsigned long*)pValueAddr;
            } else if (typeId == GetClassTypeNumberId<long long>()) {
                value = (T) * (long long*)pValueAddr;
            } else if (typeId == GetClassTypeNumberId<unsigned long long>()) {
                value = (T) * (unsigned long long*)pValueAddr;
            } else if (typeId == GetClassTypeNumberId<float>()) {
                value = (T) * (float*)pValueAddr;
            } else if (typeId == GetClassTypeNumberId<double>()) {
                value = (T) * (double*)pValueAddr;
            } else {
                BEHAVIAC_ASSERT(false);
            }
        }
    };

    template<>
    struct CastValue<bool, false> {
        static void get_as(int typeId, const void* pValueAddr, bool& value) {
			BEHAVIAC_UNUSED_VAR(typeId);
			BEHAVIAC_UNUSED_VAR(pValueAddr);
			BEHAVIAC_UNUSED_VAR(value);
            BEHAVIAC_ASSERT(false);//don't support covert other type to bool
        }
    };

    template<typename T, bool bEnum>
    struct CastValueEnum {
        static void get_as(int typeId, const void* pValueAddr, T& value) {
            int v = 0;
            CastValue<int, behaviac::Meta::IsStruct<int>::Result>::get_as(typeId, pValueAddr, v);
            value = (T)v;
        }
    };

    template<typename T>
    struct CastValueEnum<T, false> {
        static void get_as(int typeId, const void* pValueAddr, T& value) {
            CastValue<T, behaviac::Meta::IsStruct<T>::Result>::get_as(typeId, pValueAddr, value);
        }
    };

    template<typename T, bool bPtr>
    struct CastValuePtr {
        static void get_as(int typeId, const void* pValueAddr, T& value) {
            CastValueEnum<T, behaviac::Meta::IsEnum<T>::Result>::get_as(typeId, pValueAddr, value);
        }
    };

    template<typename T>
    struct CastValuePtr<T, true> {
        static void get_as(int typeId, const void* pValueAddr, T& value) {
			BEHAVIAC_UNUSED_VAR(typeId);
			value = (T)(behaviac::Address) * (behaviac::Address*)pValueAddr;
        }
    };

    template<typename T, bool bIsAgent>
    struct ParamHandler_ {
        typedef T& Type;

        static Type GetValue(T* _v) {
            return *_v;
        }
    };

    template<typename T>
    struct ParamHandler_<T, true> {
        typedef T Type;

        static Type GetValue(T* _v) {
            return (Type)_v;
        }
    };

    template<>
    struct ParamHandler_<char*, false> {
        typedef char* Type;

        static Type GetValue(char** _v) {
            return (char*)(_v);
        }
    };

    template<>
    struct ParamHandler_<const char*, false> {
        typedef const char* Type;
        static Type GetValue(const char** _v) {
            return (const char*)(_v);
        }
    };

    template<typename T>
    struct ParamHandler {
        typedef typename ParamHandler_<T, behaviac::Meta::IsAgent<T>::Result>::Type Type;

        static Type GetValue(T* _v) {
            return  ParamHandler_<T, behaviac::Meta::IsAgent<T>::Result>::GetValue(_v);
        }
    };

    template<typename T>
    class ValueHandler {
    public:
        static const void* GetValue(const T& value, const int returnTypeId) {
			BEHAVIAC_UNUSED_VAR(returnTypeId);
			//return ValuePtr<T, behaviac::Meta::IsPtr<T>::Result>::GetPtr(value);
            return &value;
        }
    };

    template<>
    class ValueHandler<behaviac::string> {
    public:
        static const void* GetValue(const behaviac::string& value, const int returnTypeId) {
            if (behaviac::GetClassTypeNumberId<char*>() == returnTypeId || behaviac::GetClassTypeNumberId<const char*>() == returnTypeId) {
                return value.c_str();//return char** not char*
            } else {
                BEHAVIAC_ASSERT(behaviac::GetClassTypeNumberId<behaviac::string>() == returnTypeId);
            }

            return &value;
        }
    };

    template<>
    class ValueHandler<char*> {
    public:
        static const void* GetValue(char* value, const int returnTypeId) {
			BEHAVIAC_UNUSED_VAR(returnTypeId);

            return value;
        }
    };

    template<>
    class ValueHandler<const char*> {
    public:
        static const void* GetValue(const char* value, const int returnTypeId) {
			BEHAVIAC_UNUSED_VAR(returnTypeId);

            return value;
        }
    };

    template<>
    class ValueHandler<behaviac::string*> {
    public:
        static const void* GetValue(const behaviac::string* value, const int returnTypeId) {
            if (value) {
                if (behaviac::GetClassTypeNumberId<char*>() == returnTypeId || behaviac::GetClassTypeNumberId<const char*>() == returnTypeId) {
                    return value->c_str();
                }

                BEHAVIAC_ASSERT(0);
            }

            return 0;
        }
    };

    template<typename ParamType>
    struct StoredTypeSelector {
        typedef PARAM_BASETYPE(ParamType)								ParamBaseType;
        typedef PARAM_POINTERTYPE(ParamType)							ParamPointerType;
        typedef typename Meta::IfThenElse<Meta::IsRefType<ParamType>::Result, VALUE_TYPE(ParamType), ParamBaseType>::Result StoredType;
    };

    template<>
    struct StoredTypeSelector<char*> {
        typedef char*	ParamBaseType;
        typedef char**	ParamPointerType;
        typedef char*	StoredType;
    };

    template<>
    struct StoredTypeSelector<char*&> {
        typedef char*	ParamBaseType;
        typedef char**	ParamPointerType;
        typedef char*	StoredType;
    };

    template<>
    struct StoredTypeSelector<const char*> {
        typedef const char*		ParamBaseType;
        typedef const char**	ParamPointerType;
        typedef const char*		StoredType;
    };

    template<>
    struct StoredTypeSelector<const char*&> {
        typedef const char*		ParamBaseType;
        typedef const char**	ParamPointerType;
        typedef const char*		StoredType;
    };

    //template<typename T>
    //struct TValueGetter
    //{
    //	typedef typename behaviac::Meta::ParamCalledType<T>::Result CalledType;
    //	typedef typename ParamHandler<T>::Type	ReturnType;

    //	static ReturnType GetValue(Agent* self, IInstanceMember* pParam)
    //	{
    //		T* pData = (T*)pParam->GetValue(self, behaviac::Meta::IsVector<T>::Result, behaviac::GetClassTypeNumberId<T>());

    //		ReturnType ppData = ParamHandler<T>::GetValue(pData);

    //		return ppData;
    //	}
    //};

    class IValue {
    public:
        virtual ~IValue() {
        }

        virtual void Log(const behaviac::Agent* agent, const char* name, bool bForce) {
			BEHAVIAC_UNUSED_VAR(agent);
			BEHAVIAC_UNUSED_VAR(name);
			BEHAVIAC_UNUSED_VAR(bForce);

            BEHAVIAC_ASSERT(false);
        }
    };

    template<typename T>
    class  TValue : public IValue {
        typedef VALUE_TYPE(T) RealBaseType;

    public:
        RealBaseType value;

    public:
        TValue() {
            value = RealBaseType();
        };

        TValue(T v) {
            value = v;
        };

        TValue(TValue& rhs) {
            value = rhs.value;
        };

        virtual IValue* clone() {
            return BEHAVIAC_NEW TValue(*this);
        };

        virtual ~TValue() {
        }

    public:
        virtual void Log(const behaviac::Agent* pAgent, const char* name, bool bForce) {
#if !BEHAVIAC_RELEASE
            const RealBaseType& currentValue = pAgent->GetVariable<RealBaseType>(name);

            if (bForce || OperationUtils::Compare<RealBaseType>(currentValue, this->value, E_NOTEQUAL)) {
				behaviac::string valueStr = StringUtils::ToString(currentValue);
                behaviac::string typeName = GetClassTypeName((RealBaseType*)0);

                LogManager::GetInstance()->Log(pAgent, typeName.c_str(), name, valueStr.c_str());

                this->value = currentValue;
            }

#endif
        }
    };

    class IInstanceMember {
    protected:
        const static int kInstanceNameMax = 128;

    public:
        virtual ~IInstanceMember() {
            BEHAVIAC_ASSERT(1);
        }

        virtual int GetClassTypeNumberId() const {
            BEHAVIAC_ASSERT(false);
            return 0;
        }

        virtual int GetCount(behaviac::Agent* self) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_ASSERT(false);
            return 0;
        }

        virtual void SetValue(behaviac::Agent* self, const void* pValue) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(pValue);
			BEHAVIAC_ASSERT(false);
        }

        // only being overridden for vector in TInstanceMember
        // @param bVector, tyep is a behaviac::vector<T> if true, otherwise, type is an IList or an element type
        virtual const void* GetValue(const behaviac::Agent* self, bool bVector = false, const int returnTypeId = 0) const {
			BEHAVIAC_UNUSED_VAR(bVector);

            const void* pData = this->GetValueAddress(self, returnTypeId);
            return pData;
        }

        virtual const void* GetValueAddress(const behaviac::Agent* self, const int returnTypeId) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(returnTypeId);
            BEHAVIAC_ASSERT(false);
            return NULL;
        }

        virtual void SetValueCast(behaviac::Agent* self, IInstanceMember* right, bool bCast = false) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(right);
			BEHAVIAC_UNUSED_VAR(bCast);
            BEHAVIAC_ASSERT(false);
        }

        virtual void SetValueElement(behaviac::Agent* self, IInstanceMember* right, int index) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(right);
			BEHAVIAC_UNUSED_VAR(index);
            BEHAVIAC_ASSERT(false);
        }

        const void* GetValueObject(const behaviac::Agent* self) const {
            const void* pData = this->GetValue(self);
            return pData;
        }

        virtual bool Compare(behaviac::Agent* self, IInstanceMember* right, EOperatorType comparisonType) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(right);
			BEHAVIAC_UNUSED_VAR(comparisonType);
            BEHAVIAC_ASSERT(false);
            return false;
        }

        virtual void Compute(behaviac::Agent* self, IInstanceMember* right1, IInstanceMember* right2, EOperatorType computeType) {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(right1);
			BEHAVIAC_UNUSED_VAR(right2);
			BEHAVIAC_UNUSED_VAR(computeType);
            BEHAVIAC_ASSERT(false);
        }

        virtual bool IsMethod() const {
            return false;
        }

        /////////////////////////////////////////////////////////////////////////////////
        //method stuff
        virtual IInstanceMember* clone() {
            BEHAVIAC_ASSERT(false);
            return 0;
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
			BEHAVIAC_UNUSED_VAR(instance);
			BEHAVIAC_UNUSED_VAR(paramStrs);
            BEHAVIAC_ASSERT(false);
        }

        virtual void run(behaviac::Agent* self) {
			BEHAVIAC_UNUSED_VAR(self);
			//BEHAVIAC_ASSERT(false);
        }

        virtual IValue* GetIValue(behaviac::Agent* self) {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_ASSERT(false);
            return NULL;
        }

        virtual IValue* GetIValueFrom(behaviac::Agent* self, IInstanceMember* firstParam) {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(firstParam);
			BEHAVIAC_ASSERT(false);
            return NULL;
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(treeTask);
			BEHAVIAC_ASSERT(false);
        }
        /////////////////////////////////////////////////////////////////////////////////
    };

    class IInstantiatedVariable {
    public:
        virtual ~IInstantiatedVariable() {
            BEHAVIAC_ASSERT(true);
        }

        virtual const void* GetValueObject(const behaviac::Agent* self) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_ASSERT(false);
            return NULL;
        }

        virtual const void* GetValueObject(const behaviac::Agent* self, int index) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(index);
			BEHAVIAC_ASSERT(false);
            return NULL;
        }

        virtual void SetValue(behaviac::Agent* self, const void* pValueAddr) {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(pValueAddr);
			BEHAVIAC_ASSERT(false);
        }

        virtual void SetValueFromString(const char* valueStr) {
			BEHAVIAC_UNUSED_VAR(valueStr);
			BEHAVIAC_ASSERT(false);
        }

        virtual behaviac::string GetValueToString() {
            BEHAVIAC_ASSERT(false);
            return "";
        }

        virtual void SetValueElement(behaviac::Agent* self, const void* value, int index) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(value);
			BEHAVIAC_UNUSED_VAR(index);
			BEHAVIAC_ASSERT(false);
        }

        virtual void Log(const behaviac::Agent* self) {
			BEHAVIAC_UNUSED_VAR(self);
#if !BEHAVIAC_RELEASE
            BEHAVIAC_ASSERT(false);
#endif
        }

        virtual IInstantiatedVariable* clone() {
            BEHAVIAC_ASSERT(false);
            return NULL;
        }

        virtual void CopyTo(Agent* pAgent) {
			BEHAVIAC_UNUSED_VAR(pAgent);
			BEHAVIAC_ASSERT(false);
        }

        virtual void Save(IIONode* node) {
			BEHAVIAC_UNUSED_VAR(node);
			BEHAVIAC_ASSERT(false);
        }
    };

    class IProperty {
    public:
        virtual ~IProperty() {
            BEHAVIAC_ASSERT(1);
        }

        virtual const char* Name() const {
            BEHAVIAC_ASSERT(false);
            return NULL;
        }

        virtual const char* GetClassNameString() const {
            BEHAVIAC_ASSERT(false);
            return 0;
        }

        virtual void SetValueFrom(behaviac::Agent* self, IInstanceMember* right) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(right);
			BEHAVIAC_ASSERT(false);
        }

        virtual const void* GetValue(const behaviac::Agent* self, bool bVector = false, const int returnTypeId = 0) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(bVector);
			BEHAVIAC_UNUSED_VAR(returnTypeId);
            BEHAVIAC_ASSERT(false);
            return NULL;
        }

        virtual behaviac::string GetValueToString(const behaviac::Agent* self) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_ASSERT(false);
            return "";
        }

        virtual void SetValueFromString(behaviac::Agent* self, const char* valueStr) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(valueStr);
			BEHAVIAC_ASSERT(false);
        }

		virtual bool Equal(const behaviac::Agent* lhs, const behaviac::Agent* rhs) const {
			BEHAVIAC_UNUSED_VAR(lhs);
			BEHAVIAC_UNUSED_VAR(rhs);
			BEHAVIAC_ASSERT(false);

            return false;
        }

        virtual IInstanceMember* CreateInstance(const char* instance, IInstanceMember* indexMember) {
			BEHAVIAC_UNUSED_VAR(instance);
			BEHAVIAC_UNUSED_VAR(indexMember);
			BEHAVIAC_ASSERT(false);
            return NULL;
        }

        virtual bool IsCustomized() const {
            return false;
        }

        //for customized property
        virtual IInstantiatedVariable* Instantiate() const {
            BEHAVIAC_ASSERT(this->IsCustomized(), "the customized is overridden");
            BEHAVIAC_ASSERT(false);
            return NULL;
        }

        virtual IValue* CreateIValue() const {
            BEHAVIAC_ASSERT(false);
            return NULL;
        }

//#if !BEHAVIAC_RELEASE
        virtual bool IsArrayItem() const {
            return false;
        }
//#endif
    };

    template<typename T, bool bTypeIsVector>
    class CInstanceMember : public IInstanceMember {
        // const int& -> int
    public:
        typedef VALUE_TYPE(T) RealBaseType;
        /* ParamVariable<T> data;*/
        //typedef T GetReturnType;

    protected:
        char _instance[kInstanceNameMax];
        IInstanceMember*    _indexMember;

    public:
        CInstanceMember() {
            _indexMember = NULL;
        }

        CInstanceMember(const char* instance, IInstanceMember* indexMember) {
            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);

            _indexMember = indexMember;
        }

        CInstanceMember(CInstanceMember& rhs) {
            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, rhs._instance);

            _indexMember = rhs._indexMember;
        }

    public:
        virtual int GetClassTypeNumberId() const {
            return behaviac::GetClassTypeNumberId<RealBaseType>();
        }

        virtual int GetCount(behaviac::Agent* self) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_ASSERT(false);
            return 0;
        }

        virtual void SetValueElement(behaviac::Agent* self, IInstanceMember* right, int index) const {
            BEHAVIAC_ASSERT(right->GetClassTypeNumberId() == behaviac::GetClassTypeNumberId<behaviac::vector<RealBaseType> >());

            //Agent* agent = Agent::GetParentAgent(self, right->_instance);
            const void*  pRightValueData = right->GetValueObject(self);
            const TList<behaviac::vector<RealBaseType> >& list = *(const TList<behaviac::vector<RealBaseType> >*)pRightValueData;

            //const TList<behaviac::vector<RealBaseType> >::ElementType& item = (*list.vector_)[index];
            const RealBaseType& item = (*list.vector_)[index];

            this->SetValue(self, &item);
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CInstanceMember(*this);
        }

        virtual const void* GetValueAddress(const behaviac::Agent* self, const int returnTypeId) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(returnTypeId);
			BEHAVIAC_ASSERT(false);
            return 0;
        }

        virtual void SetValueCast(behaviac::Agent* self, IInstanceMember* right, bool bCast) const {
			BEHAVIAC_UNUSED_VAR(right);
			BEHAVIAC_UNUSED_VAR(bCast);

			//TODO: 3 mark for remove
            //SetValue(self, (CInstanceMember<T, Meta::IsVector<T>::Result>*)(right), bCast);

            CInstanceMember<RealBaseType, behaviac::Meta::IsVector<RealBaseType>::Result>* cRight = (CInstanceMember<RealBaseType, behaviac::Meta::IsVector<RealBaseType>::Result>*)right;
            bool isVector = behaviac::Meta::IsVector<RealBaseType>::Result;
            int valueTypeId = behaviac::GetClassTypeNumberId<RealBaseType>();

            if (bCast) {
                RealBaseType v;

                cRight->GetValueAs(self, isVector, valueTypeId, v);
                SetValue(self, &v);
            } else {
                const RealBaseType* pData = (const RealBaseType*)cRight->GetValue(self, isVector, valueTypeId);
                SetValue(self, pData);
            }
        }

        virtual void SetValue(behaviac::Agent* self, const void* pValue) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(pValue);
			BEHAVIAC_ASSERT(false);
        }

        template<typename RT>
        void GetValueAs(const behaviac::Agent* self, bool bVector, const int returnTypeId, RT& returnValue) {
            BEHAVIAC_UNUSED_VAR(bVector);
            const void* pValueAddr = this->GetValueAddress(self, returnTypeId);

            int valueTypeId = this->GetClassTypeNumberId();

            CastValuePtr<RT, behaviac::Meta::IsPtr<RT>::Result>::get_as(valueTypeId, pValueAddr, returnValue);
        }

        virtual bool Compare(behaviac::Agent* self, IInstanceMember* right, EOperatorType comparisonType) const {
			const RealBaseType* leftValue = (const RealBaseType*)this->GetValue(self, behaviac::Meta::IsVector<RealBaseType>::Result, behaviac::GetClassTypeNumberId<RealBaseType>());
			const RealBaseType* rightValue = (const RealBaseType*)right->GetValue(self, behaviac::Meta::IsVector<RealBaseType>::Result, behaviac::GetClassTypeNumberId<RealBaseType>());

            return OperationUtils::Compare(*leftValue, *rightValue, comparisonType);
        }

        virtual void Compute(behaviac::Agent* self, IInstanceMember* right1, IInstanceMember* right2, EOperatorType computeType) {
            const RealBaseType* rightValue1 = (const RealBaseType*)right1->GetValue(self);
            const RealBaseType* rightValue2 = (const RealBaseType*)right2->GetValue(self);

            RealBaseType _value = OperationUtils::Compute(*rightValue1, *rightValue2, computeType);
            SetValue(self, &_value);
        }

        //private:
        //TODO: 3 mark for remove
        //void SetValue(behaviac::Agent* self, CInstanceMember<RealBaseType, behaviac::Meta::IsVector<RealBaseType>::Result>* right, bool bCast)
        //{
        //    bool isVector = behaviac::Meta::IsVector<RealBaseType>::Result;
        //    int valueTypeId = behaviac::GetClassTypeNumberId<RealBaseType>();

        //    if (bCast)
        //    {
        //        RealBaseType v;

        //        right->GetValueAs(self, isVector, valueTypeId, v);
        //        SetValue(self, &v);
        //    }
        //    else
        //    {
        //        const RealBaseType* pData = (const RealBaseType*)right->GetValue(self, isVector, valueTypeId);
        //        SetValue(self, pData);
        //    }
        //}
    };

    template<typename T>
    class CInstanceMember<T, true> : public CInstanceMember<T, false> {
    public:
        typedef typename behaviac::Meta::IsVector<T>::ElementType ElementType;

        CInstanceMember() {
        }

        CInstanceMember(const char* instance, IInstanceMember* indexMember) {
            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            this->_indexMember = indexMember;
        }

        virtual int GetCount(behaviac::Agent* self) const {
            int returnTypeId = 0;
            const void* pData = this->GetValueAddress(self, returnTypeId);

            const behaviac::vector<ElementType>& vec = *(const behaviac::vector<ElementType>*)pData;

            return (int)vec.size();
        }

        CInstanceMember(CInstanceMember& rhs) {
            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, rhs._instance);
            //_instance = rhs._instance;
            this->_indexMember = rhs._indexMember;
            //data = rhs.data;
        }

        virtual const void* GetValue(const behaviac::Agent* self, bool bVector = false, const int returnTypeId = 0) const {
            const void* pData = this->GetValueAddress(self, returnTypeId);

            if (bVector) {
                return pData;
            } else {
                const behaviac::vector<ElementType>& vec = *(const behaviac::vector<ElementType>*)pData;

                const IList* pList = TList<T>::CreateList(&vec);

                return pList;
            }
        }
    };

    template <typename T, bool bPtr>
    struct SetFromString_t {
        static void Set(const char* valueStr, T& value) {
            behaviac::StringUtils::ParseString(valueStr, value);
        }

        static behaviac::string Get(T& value) {
            return behaviac::StringUtils::ToString(value);
        }
    };

    template <typename T>
    struct SetFromString_t<T, true> {
        static void Set(const char* valueStr, T& value) {
			BEHAVIAC_ASSERT(StringUtils::StringEqual(valueStr, "null"));
            value = 0;
        }

        static behaviac::string Get(T& value) {
            return behaviac::StringUtils::ToString(value);
        }
    };

    template <typename T>
    class CInstanceProperty;

    template<typename T>
    class CProperty : public IProperty {
    public:
        behaviac::string _name;
        virtual const char* Name() const {
            return this->_name.c_str();
        }

//#if !BEHAVIAC_RELEASE
    public:
		virtual bool IsArrayItem() const {
            return false;
        }
//#endif

    public:
        CProperty(const char* name) {
            this->_name = name;
        }
        virtual ~CProperty() {
            BEHAVIAC_ASSERT(1);
        }

    public:
        IInstanceMember* CreateInstance(const char* instance, IInstanceMember* indexMember) {
            return BEHAVIAC_NEW CInstanceProperty<T>(instance, indexMember, this);
        }

        virtual IValue* CreateIValue() const {
            return BEHAVIAC_NEW TValue<T>();
        }

        virtual const void* GetValueObject(const behaviac::Agent* self, int index) const {
            const T* v = (const T*)GetValueElement(self, index);
            return (void*)v;
        }

        virtual behaviac::string GetValueToString(const behaviac::Agent* self) const {
            T* pValue = (T*)this->GetValue(self);
            behaviac::string result = SetFromString_t<T, behaviac::Meta::IsPtr<T>::Result>::Get(*pValue);

            return result;
        }

		virtual bool Equal(const behaviac::Agent* lhs, const behaviac::Agent* rhs) const {
			T* lhsValue = (T*)this->GetValue(lhs);
			T* rhsValue = (T*)this->GetValue(rhs);

			return behaviac::PrivateDetails::Equal(*lhsValue, *rhsValue);
		}

        virtual void SetValueFromString(behaviac::Agent* self, const char* valueStr) const {
            T value;

            SetFromString_t<T, behaviac::Meta::IsPtr<T>::Result>::Set(valueStr, value);
            this->SetValue(self, &value);
        }

        virtual void SetValueFrom(behaviac::Agent* self, IInstanceMember* right) const {
            const T* rightValue = (const T*)right->GetValue(self);

            SetValue(self, rightValue);
        }

        virtual void SetValue(behaviac::Agent* self, const void* pValue) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(pValue);
			BEHAVIAC_ASSERT(false);
        }

        virtual void SetValueElement(behaviac::Agent* self, const void* value, int index) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(value);
			BEHAVIAC_UNUSED_VAR(index);
			BEHAVIAC_ASSERT(false);
        }

        virtual const void* GetValue(const behaviac::Agent* self, bool bVector = false, const int returnTypeId = 0) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(bVector);
			BEHAVIAC_UNUSED_VAR(returnTypeId);
			BEHAVIAC_ASSERT(false);
            return 0;
        }

        virtual const void* GetValueElement(const behaviac::Agent* self, int index) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(index);
            BEHAVIAC_ASSERT(false);
            return 0;
        }

        virtual const char* GetClassNameString() const {
            return GetClassTypeName((T*)0);
        }
    };

    template <typename T>
    class CInstanceProperty : public CInstanceMember<T, behaviac::Meta::IsVector<T>::Result> {
        CProperty<T>* _property;

    public:
        CInstanceProperty(const char* instance, IInstanceMember* indexMember, CProperty<T>* prop)
            : CInstanceMember<T, Meta::IsVector<T>::Result>(instance, indexMember) {
            _property = prop;
        }

    public:
        virtual const void* GetValueAddress(const behaviac::Agent* self, const int returnTypeId) const {
            Agent* agent = Agent::GetParentAgent(self, this->_instance);

            if (this->_indexMember != NULL) {
                int indexValue = *(int*)this->_indexMember->GetValue(self);
                const void* pValue = _property->GetValueElement(agent, indexValue);
                return pValue;
            }

            const T* pValue = (const T*)_property->GetValue(agent);

            return ValueHandler<T>::GetValue(*pValue, returnTypeId);
        }

        virtual void SetValue(behaviac::Agent* self, const void* pValue) const {
            Agent* agent = Agent::GetParentAgent(self, this->_instance);

            if (this->_indexMember != NULL) {
                int indexValue = *(int*)this->_indexMember->GetValue(self);
                _property->SetValueElement(agent, pValue, indexValue);
            } else {
                _property->SetValue(agent, pValue);
            }
        }
    };

    template<typename T>
    class CStaticMemberProperty : public CProperty<T> {
        typedef VALUE_TYPE(T) ValueType;
    public:
        typedef void(*SetFunctionPointer)(T v);
        typedef const void* (*GetFunctionPointer)();

        SetFunctionPointer _sfp;
        GetFunctionPointer _gfp;

    public:
        CStaticMemberProperty(const char* name, SetFunctionPointer sfp, GetFunctionPointer gfp)
            : CProperty<T>(name) {
            _sfp = sfp;
            _gfp = gfp;
        }

        virtual const void* GetValue(const behaviac::Agent* self, bool bVector = false, const int returnTypeId = 0) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(bVector);
			BEHAVIAC_UNUSED_VAR(returnTypeId);
            BEHAVIAC_ASSERT(_gfp != NULL);

            const void* pData = _gfp();
            return pData;
        }

        virtual void SetValue(behaviac::Agent* self, const void* pValue) const {
			BEHAVIAC_UNUSED_VAR(self);
            BEHAVIAC_ASSERT(_sfp != NULL);

            _sfp(*(ValueType*)pValue);
        }
    };

    template<typename T>
    class CStaticMemberArrayItemProperty : public CProperty<T> {
        typedef VALUE_TYPE(T) ValueType;
    public:
        typedef void(*SetFunctionPointer)(T v, int index);
        //void* SetFunctionPointer(T v, int index);
        typedef const void* (*GetFunctionPointer)(int index);
        //T* GetFunctionPointer(int index);

        SetFunctionPointer _sfp;
        GetFunctionPointer _gfp;

    public:
        CStaticMemberArrayItemProperty(const char* name, SetFunctionPointer sfp, GetFunctionPointer gfp)
            : CProperty<T>(name) {
            _sfp = sfp;
            _gfp = gfp;
        }

//#if !BEHAVIAC_RELEASE
    public:
		virtual bool IsArrayItem() const {
            return true;
        }
//#endif

    public:
        virtual const void* GetValueElement(const behaviac::Agent* self, int index) const {
			BEHAVIAC_UNUSED_VAR(self);
            BEHAVIAC_ASSERT(_gfp != NULL);

            const void* pData = _gfp(index);
            return pData;
        }

        virtual void SetValueElement(behaviac::Agent* self, const void* value, int index) const {
			BEHAVIAC_UNUSED_VAR(self);
            BEHAVIAC_ASSERT(_sfp != NULL);

            _sfp(*(T*)value, index);
        }
    };

    template<typename T>
    class CMemberProperty : public CProperty<T> {
        typedef VALUE_TYPE(T) ValueType;
    public:
        //delegate void SetFunctionPointer(behaviac::Agent* a, T v);
        typedef void(*SetFunctionPointer)(behaviac::Agent* a, T v);
        typedef const void* (*GetFunctionPointer)(behaviac::Agent* a);

        SetFunctionPointer _sfp;
        GetFunctionPointer _gfp;

    public:
        CMemberProperty(const char* name, SetFunctionPointer sfp, GetFunctionPointer gfp)
            : CProperty<T>(name) {
            _sfp = sfp;
            _gfp = gfp;
        }

        virtual const void* GetValue(const behaviac::Agent* self, bool bVector = false, const int returnTypeId = 0) const {
            BEHAVIAC_ASSERT(_gfp != NULL);
#if BEHAVIAC_USE_HTN
            if (self->m_planningTop > -1) {
                uint32_t id = MakeVariableId(this->Name());
                IInstantiatedVariable* pVar = self->m_variables->GetVariable(id);

                if (pVar != NULL) {
                    CVariable<T>* pTVar = (CVariable<T>*)pVar;
                    return pTVar->GetValue(self);
                }
            }
#endif
            const void* pData = _gfp((behaviac::Agent*)self);
            return pData;
        }

        virtual void SetValue(behaviac::Agent* self, const void* pValue) const {
            BEHAVIAC_ASSERT(_sfp != NULL);

#if BEHAVIAC_USE_HTN

            if (self->m_planningTop > -1) {
                uint32_t id = MakeVariableId(this->Name());
                IInstantiatedVariable* pVar = self->m_variables->GetVariable(id);

                if (pVar == NULL) {
                    pVar = BEHAVIAC_NEW CVariable<T>(this->Name(), *(ValueType*)pValue);
                    self->m_variables->AddVariable(id, pVar, 1);
                } else {
                    CVariable<T>* pTVar = (CVariable<T>*)pVar;
                    pTVar->SetValue(self, pValue);
                }
            } else
#endif//
            {
                _sfp(self, *(T*)pValue);
            }
        }
    };

    template<typename T>
    class CMemberArrayItemProperty : public CProperty<T> {
        typedef VALUE_TYPE(T) ValueType;
    public:
        typedef void(*SetFunctionPointer)(behaviac::Agent* a, T v, int index);
        typedef const void* (*GetFunctionPointer)(behaviac::Agent* a, int index);

        SetFunctionPointer _sfp;
        GetFunctionPointer _gfp;

    public:
        CMemberArrayItemProperty(const char* name, SetFunctionPointer sfp, GetFunctionPointer gfp)
            : CProperty<T>(name) {

            _sfp = sfp;
            _gfp = gfp;
        }

//#if !BEHAVIAC_RELEASE
    public:
		virtual bool IsArrayItem() const {
            return true;
        }
//#endif

    public:
        virtual const void* GetValueElement(const behaviac::Agent* self, int index) const {
            BEHAVIAC_ASSERT(_gfp != NULL);

            const void* pData = _gfp((behaviac::Agent*)self, index);
            return pData;
        }

        virtual void SetValueElement(behaviac::Agent* self, const void* value, int index) const {
            BEHAVIAC_ASSERT(_sfp != NULL);

            _sfp(self, *(T*)value, index);
        }
    };

    template<typename T>
    class CCustomizedProperty : public CProperty<T> {
    private:
        uint32_t _id;
        T _defaultValue;

    public:
        CCustomizedProperty(uint32_t id, const char* name, const char* valueStr)
            : CProperty<T>(name) {
            _id = id;

            SetFromString_t<T, behaviac::Meta::IsPtr<T>::Result>::Set(valueStr, this->_defaultValue);
        }

        virtual const void* GetValue(const behaviac::Agent* self, bool bVector = false, const int returnTypeId = 0) const {
			BEHAVIAC_UNUSED_VAR(bVector);
			BEHAVIAC_UNUSED_VAR(returnTypeId);

			T* pInstanceValue = 0;

            if (self && self->GetVarValue<T>(_id, pInstanceValue)) {
                return pInstanceValue;
            }

            //BEHAVIAC_ASSERT(false);

            return &_defaultValue;
        }

        IInstanceMember* CreateInstance(const char* instance, IInstanceMember* indexMember) {
            return CProperty<T>::CreateInstance(instance, indexMember);
        }

        virtual void SetValue(behaviac::Agent* self, const void* pValue) const {
            bool bOk = self->SetVarValue(_id, (T*)pValue);
            BEHAVIAC_ASSERT(bOk);
            BEHAVIAC_UNUSED_VAR(bOk);
        }

    public:
        virtual bool IsCustomized() const {
            return true;
        }

        virtual IInstantiatedVariable* Instantiate() const {
            return BEHAVIAC_NEW CVariable<T>(this->_name.c_str(), this->_defaultValue);
        }
    };

    template <typename T>
    class CVariable : public IInstantiatedVariable {
        typedef VALUE_TYPE(T) RealBaseType;
        T _value;
    public:
        const char* _name;

#if !BEHAVIAC_RELEASE
    public:
        bool m_bIsModified;
#endif

    public:
        CVariable(const char* name, T value) {
            _value = value;
            _name = name;

#if !BEHAVIAC_RELEASE
            m_bIsModified = true;
#endif
        }

        virtual ~CVariable() {
            BEHAVIAC_ASSERT(true);
        }

    public:
        virtual const void* GetValue(const behaviac::Agent* self, bool bVector = false, const int returnTypeId = 0) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(bVector);
			BEHAVIAC_UNUSED_VAR(returnTypeId);
			return &_value;
        }

        virtual const void* GetValueObject(const behaviac::Agent* self) const {
			BEHAVIAC_UNUSED_VAR(self);
			return &_value;
        }

        virtual const void* GetValueObject(const behaviac::Agent* self, int index) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(index);
			BEHAVIAC_ASSERT(false);

            return NULL;
        }

        virtual void SetValue(behaviac::Agent* self, const void* pValueAddr) {
			BEHAVIAC_UNUSED_VAR(self);

			_value = *(T*)(pValueAddr);

#if !BEHAVIAC_RELEASE
            m_bIsModified = true;
#endif
        }

        virtual void SetValueFromString(const char* valueStr) {
            SetFromString_t<T, behaviac::Meta::IsPtr<T>::Result>::Set(valueStr, this->_value);

#if !BEHAVIAC_RELEASE
            m_bIsModified = true;
#endif
        }

        virtual behaviac::string GetValueToString() {
            behaviac::string result = SetFromString_t<T, behaviac::Meta::IsPtr<T>::Result>::Get(this->_value);

            return result;
        }

        virtual void SetValueElement(behaviac::Agent* self, const void* value, int index) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(value);
			BEHAVIAC_UNUSED_VAR(index);
			BEHAVIAC_ASSERT(false);
        }

    public:
        virtual void Log(const behaviac::Agent* self) {
#if !BEHAVIAC_RELEASE
            if (m_bIsModified) {
                behaviac::string valueStr = StringUtils::ToString(this->_value);
                behaviac::string typeName = GetClassTypeName((RealBaseType*)0);

                LogManager::GetInstance()->Log(self, typeName.c_str(), this->_name, valueStr.c_str());

                // clear it
                this->m_bIsModified = false;
            }
#endif
        }

        virtual void CopyTo(behaviac::Agent* pAgent) {
            pAgent->SetVariable(this->_name, this->_value);
        }

        virtual void Save(IIONode* node) {
            //base.Save(node);
            CIOID variableId("var");
            IIONode* varNode = node->newNodeChild(variableId);

            CIOID nameId("name");
            varNode->setAttr(nameId, this->_name);

            CIOID valueId("value");
            varNode->setAttr(valueId, this->_value);
        }

        IInstantiatedVariable* clone() {
            CVariable* p = BEHAVIAC_NEW CVariable<T>(this->_name, this->_value);

            return p;
        }
    };

    template <typename T>
    class CArrayItemVariable : public  IInstantiatedVariable {
        uint32_t _parentId;
        const char* _name;

    public:
        uint32_t ParentId() {
            return _parentId;
        }

        CArrayItemVariable(uint32_t parentId, const char* name) {
            _parentId = parentId;
            _name = name;
        }

        virtual const void* GetValueElement(const behaviac::Agent* self, int index) const {
            IInstantiatedVariable* v = self->GetInstantiatedVariable(this->_parentId);

            if (behaviac::Meta::IsRef<T>::Result == 0 || behaviac::Meta::IsPtr<T>::Result == 0) {
                CVariable<behaviac::vector<T> >* arrayVar = (CVariable<behaviac::vector<T> >*)v;

                BEHAVIAC_ASSERT(arrayVar != NULL);
                const T& _v = (*(behaviac::vector<T>*)arrayVar->GetValue(self))[index];
                return &_v;
            }

            return (T*)v->GetValueObject(self, index);
        }

        virtual void SetValueElement(behaviac::Agent* self, const void* value, int index) const {
            IInstantiatedVariable* v = self->GetInstantiatedVariable(this->_parentId);
            CVariable<behaviac::vector<T> >* arrayVar = (CVariable<behaviac::vector<T> >*)v;

            if (arrayVar != NULL) {
                behaviac::vector<T> temp = *(behaviac::vector<T>*)(arrayVar->GetValue(self));
                temp[index] = *(T*)value;

#if !BEHAVIAC_RELEASE
                arrayVar->m_bIsModified = true;
#endif
            }
        }

        virtual const void* GetValueObject(const behaviac::Agent* self) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_ASSERT(false);
            return 0;
        }

        virtual const void* GetValueObject(const behaviac::Agent* self, int index) const {
            return GetValueElement(self, index);
        }

        virtual void SetValue(behaviac::Agent* self, const void* pValueAddr) {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(pValueAddr);
			BEHAVIAC_ASSERT(false);
        }

    public:
        virtual void Log(const behaviac::Agent* self) {
			BEHAVIAC_UNUSED_VAR(self);

#if !BEHAVIAC_RELEASE
            IInstantiatedVariable* v = self->GetInstantiatedVariable(this->_parentId);
            CVariable<behaviac::vector<T> >* arrayVar = (CVariable<behaviac::vector<T> >*)v;

            if (arrayVar != NULL && arrayVar->m_bIsModified) {
                //behaviac::string valueStr = StringUtils::ToString(this->value);
                //behaviac::string typeName = GetClassTypeName((RealBaseType*)0);

                //LogManager::GetInstance()->Log(pAgent, typeName.c_str(), name, valueStr.c_str());;
                BEHAVIAC_ASSERT(0);
            }
#endif
        }

        virtual void CopyTo(behaviac::Agent* pAgent) {
			BEHAVIAC_UNUSED_VAR(pAgent);
            //BEHAVIAC_ASSERT(false);
        }

        virtual void Save(IIONode* node) {
			BEHAVIAC_UNUSED_VAR(node);
			//BEHAVIAC_ASSERT(false);
        }

        IInstantiatedVariable* clone() {
            CArrayItemVariable<T>* p = BEHAVIAC_NEW CArrayItemVariable<T>(this->_parentId, this->_name);
            return p;
        }
    };

    template <>
    class CArrayItemVariable<bool> : public  IInstantiatedVariable {
        uint32_t _parentId;
        const char* _name;

    public:
        uint32_t ParentId() {
            return _parentId;
        }

        CArrayItemVariable(uint32_t parentId, const char* name) {
            _parentId = parentId;
            _name = name;
        }

        virtual const void* GetValueElement(const behaviac::Agent* self, int index) const {
            IInstantiatedVariable* v = self->GetInstantiatedVariable(this->_parentId);

            CVariable<behaviac::vector<bool> >* arrayVar = (CVariable<behaviac::vector<bool> >*)v;

            BEHAVIAC_ASSERT(arrayVar != NULL);
            static ThreadBool buffer;
            bool b = (*(behaviac::vector<bool>*)arrayVar->GetValue(self))[index];
            buffer.set(b);
            return buffer.value();
        }

        virtual void SetValueElement(behaviac::Agent* self, const void* value, int index) const {
            IInstantiatedVariable* v = self->GetInstantiatedVariable(this->_parentId);
            CVariable<behaviac::vector<bool> >* arrayVar = (CVariable<behaviac::vector<bool> >*)v;

            if (arrayVar != NULL) {
                behaviac::vector<bool> temp = *(behaviac::vector<bool>*)(arrayVar->GetValue(self));
                temp[index] = *(bool*)value;
#if !BEHAVIAC_RELEASE
                arrayVar->m_bIsModified = true;
#endif
            }
        }

        virtual const void* GetValueObject(const behaviac::Agent* self) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_ASSERT(false);
            return 0;
        }

        virtual const void* GetValueObject(const behaviac::Agent* self, int index) const {
            return GetValueElement(self, index);
        }

        virtual void SetValue(behaviac::Agent* self, const void* pValueAddr) {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(pValueAddr);
			BEHAVIAC_ASSERT(false);
        }

    public:
        virtual void Log(const behaviac::Agent* self) {
			BEHAVIAC_UNUSED_VAR(self);

#if !BEHAVIAC_RELEASE
            IInstantiatedVariable* v = self->GetInstantiatedVariable(this->_parentId);
            CVariable<behaviac::vector<bool> >* arrayVar = (CVariable<behaviac::vector<bool> >*)v;

            if (arrayVar != NULL && arrayVar->m_bIsModified) {
                //behaviac::string valueStr = StringUtils::ToString(this->value);
                //behaviac::string typeName = GetClassTypeName((RealBaseType*)0);

                //LogManager::GetInstance()->Log(pAgent, typeName.c_str(), name, valueStr.c_str());;
                BEHAVIAC_ASSERT(0);
            }
#endif
        }

        virtual void CopyTo(behaviac::Agent* pAgent) {
			BEHAVIAC_UNUSED_VAR(pAgent);
			BEHAVIAC_ASSERT(false);
        }

        virtual void Save(IIONode* node) {
			BEHAVIAC_UNUSED_VAR(node);
			BEHAVIAC_ASSERT(false);
        }

        IInstantiatedVariable* clone() {
            CArrayItemVariable<bool>* p = BEHAVIAC_NEW CArrayItemVariable<bool>(this->_parentId, this->_name);
            return p;
        }
    };

    template<typename T>
    class CCustomizedArrayItemProperty : public CProperty<T> {
        uint32_t _parentId;

    public:
        CCustomizedArrayItemProperty(uint32_t parentId, const char* parentName)
            : CProperty<T>(parentName) {
            _parentId = parentId;
        }

//#if !BEHAVIAC_RELEASE
    public:
		virtual bool IsArrayItem() const {
            return true;
        }
//#endif

    public:
        virtual bool IsCustomized() const {
            return true;
        }

        virtual const void* GetValueElement(const behaviac::Agent* self, int index) const {
            const behaviac::vector<T>& arrayValue = self->GetVariable<behaviac::vector<T> >(_parentId);
			int len = (int)arrayValue.size();
            BEHAVIAC_UNUSED_VAR(len);

            BEHAVIAC_ASSERT(len != 0);

            BEHAVIAC_ASSERT(index >= 0 && index < len);

            const T& v = (self->GetVariable<behaviac::vector<T> >(_parentId))[index];
            return &v;
        }

        IInstanceMember* CreateInstance(const char* instance, IInstanceMember* indexMember) {
            return CProperty<T>::CreateInstance(instance, indexMember);
        }

        virtual void SetValueElement(behaviac::Agent* self, const void* value, int index) const {
            const behaviac::vector<T>& arrayValue = self->GetVariable<behaviac::vector<T> >(_parentId);
            BEHAVIAC_ASSERT(arrayValue.size() != 0);
            behaviac::vector<T>* _array = (behaviac::vector<T>*) &arrayValue;
            (*_array)[index] = *(T*)value;
        }

        virtual IInstantiatedVariable* Instantiate() const {
            return BEHAVIAC_NEW CArrayItemVariable<T>(_parentId, this->_name.c_str());
        }
    };

    template<>
    class CCustomizedArrayItemProperty<bool> : public CProperty<bool> {
        uint32_t _parentId;

    public:
        CCustomizedArrayItemProperty(uint32_t parentId, const char* parentName)
            : CProperty<bool>(parentName) {
            _parentId = parentId;
        }

//#if !BEHAVIAC_RELEASE
    public:
		virtual bool IsArrayItem() const {
            return true;
        }
//#endif

    public:
        virtual bool IsCustomized() const {
            return true;
        }

        virtual const void* GetValueElement(const behaviac::Agent* self, int index) const {
            const behaviac::vector<bool>& arrayValue = self->GetVariable<behaviac::vector<bool> >(_parentId);
			int len = (int)arrayValue.size();
            BEHAVIAC_UNUSED_VAR(len);

            BEHAVIAC_ASSERT(len != 0);

            BEHAVIAC_ASSERT(index >= 0 && index < len);

            static ThreadBool buffer;
            bool b = (self->GetVariable<behaviac::vector<bool> >(_parentId))[index];
            buffer.set(b);
            return buffer.value();
        }

        IInstanceMember* CreateInstance(const char* instance, IInstanceMember* indexMember) {
            return CProperty<bool>::CreateInstance(instance, indexMember);
        }

        virtual void SetValueElement(behaviac::Agent* self, const void* value, int index) const {
            const behaviac::vector<bool>& arrayValue = self->GetVariable<behaviac::vector<bool> >(_parentId);
            BEHAVIAC_ASSERT(arrayValue.size() != 0);
            behaviac::vector<bool>* _array = (behaviac::vector<bool>*) &arrayValue;
            (*_array)[index] = *(bool*)value;
        }

        virtual IInstantiatedVariable* Instantiate() const {
            return BEHAVIAC_NEW CArrayItemVariable<bool>(_parentId, this->_name.c_str());
        }
    };

    template<typename T>
    class CInstanceCustomizedProperty : public CInstanceMember<T, behaviac::Meta::IsVector<T>::Result> {
        typedef VALUE_TYPE(T) RealBaseType;

        uint32_t _id;

    public:
        CInstanceCustomizedProperty(const char* instance, IInstanceMember* indexMember, uint32_t id)
            : CInstanceMember<T, behaviac::Meta::IsVector<T>::Result>(instance, indexMember) { //TODO
            _id = id;
        }

        virtual const void* GetValueAddress(const behaviac::Agent* self, const int returnTypeId) const {
            RealBaseType* pValue = NULL;

            if (self != NULL) {
                Agent* agent = Agent::GetParentAgent(self, this->_instance);

                if (this->_indexMember != NULL) {
                    int indexValue = *(int*)this->_indexMember->GetValue(self);
                    pValue = (RealBaseType*)&agent->GetVariable<T>(_id, indexValue);
                } else {
                    pValue = (RealBaseType*)&agent->GetVariable<T>(_id);
                }
            }

            if (pValue != NULL) {
                return ValueHandler<RealBaseType>::GetValue(*pValue, returnTypeId);
            }

            return pValue;
        }

        virtual void SetValue(behaviac::Agent* self, const void* pValue) const {
            Agent* agent = Agent::GetParentAgent(self, this->_instance);

            if (this->_indexMember != NULL) {
                int indexValue = *(int*)this->_indexMember->GetValue(self);
                agent->SetVariable<T>("", _id, *(T*)pValue, indexValue);
            } else {
                agent->SetVariable<T>("", _id, *(T*)pValue);
            }
        }
    };

    template<typename T>
    class CInstanceConstBase : public CInstanceMember<T, behaviac::Meta::IsVector<T>::Result> {

    public:
        T _value;
        CInstanceConstBase(T& value, int dummy) {
			BEHAVIAC_UNUSED_VAR(dummy);
            _value = value;
        }

        CInstanceConstBase(const char* valueStr) {
            SetFromString_t<T, behaviac::Meta::IsPtr<T>::Result>::Set(valueStr, this->_value);
        }

        virtual const void* GetValueAddress(const behaviac::Agent* self, const int returnTypeId) const {
			BEHAVIAC_UNUSED_VAR(self);
			return ValueHandler<T>::GetValue(_value, returnTypeId);
        }

        virtual void SetValue(behaviac::Agent* self, const void* pValueAddr) const {
			BEHAVIAC_UNUSED_VAR(self);
			const_cast<CInstanceConstBase*>(this)->_value = *(T*)pValueAddr;
        }
    };

    template<typename T>
	class CInstanceConst : public CInstanceConstBase<T> {
    public:
		CInstanceConst(T& value) : CInstanceConstBase<T>(value, 0) {
        }

		CInstanceConst(const char* valueStr) : CInstanceConstBase<T>(valueStr) {
		}
    };

	template<>
	class CInstanceConst<const char*> : public CInstanceConstBase<const char*>{
	public:
		CInstanceConst(const char* valueStr) : CInstanceConstBase<const char*>(valueStr) {
		}
	};

    template<>
    class CInstanceConst<behaviac::string> : public CInstanceConstBase<behaviac::string> {
    public:
        CInstanceConst(behaviac::string& value) : CInstanceConstBase<behaviac::string>(value, 0) {
        }

        CInstanceConst(const char* valueStr) : CInstanceConstBase<behaviac::string>(valueStr) {
        }

        virtual const void* GetValueAddress(const behaviac::Agent* self, const int returnTypeId) const {
			BEHAVIAC_UNUSED_VAR(self);
			return ValueHandler<behaviac::string>::GetValue(_value, returnTypeId);
        }

        virtual void SetValue(behaviac::Agent* self, const void* pValueAddr) const {
			BEHAVIAC_UNUSED_VAR(self);
			const_cast<CInstanceConst*>(this)->_value = (const char*)pValueAddr;
        }
    };

    template<typename T>
    class CAgentMethodBase : public CInstanceMember<T, behaviac::Meta::IsVector<T>::Result> {
    protected:
        TValue<T>* _returnValue;

        CAgentMethodBase() {
            _returnValue = BEHAVIAC_NEW TValue<T>();
        }

        CAgentMethodBase(CAgentMethodBase<T>& rhs)
			: CInstanceMember<T, behaviac::Meta::IsVector<T>::Result>()
		{
            _returnValue = (TValue<T>*)rhs._returnValue->clone();
        }

        virtual ~CAgentMethodBase() {
            BEHAVIAC_DELETE this->_returnValue;
        }

    public:
        virtual IInstanceMember* clone() {
            BEHAVIAC_ASSERT(false);
            return NULL;
        }

		virtual bool IsMethod() const {
			return true;
		}

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
			BEHAVIAC_UNUSED_VAR(instance);
			BEHAVIAC_UNUSED_VAR(paramStrs);
			BEHAVIAC_ASSERT(false);
        }

        virtual void run(behaviac::Agent* self) {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_ASSERT(false);
        }

        virtual const void* GetValueAddress(const behaviac::Agent* self, const int returnTypeId) const {
            if (self != NULL) {
                const_cast<CAgentMethodBase*>(this)->run((behaviac::Agent*)self);
            }

            if (self == NULL) {
                return NULL;
            }

            return ValueHandler<T>::GetValue(this->_returnValue->value, returnTypeId);
        }

        virtual IValue* GetIValue(behaviac::Agent* self) {
            if (self != NULL) {
                run(self);
            }

            return _returnValue;
        }

        virtual IValue* GetIValueFrom(behaviac::Agent* self, IInstanceMember* firstParam) {
			BEHAVIAC_ASSERT(firstParam->IsMethod());
			Agent* agent = Agent::GetParentAgent(self, this->_instance);
			firstParam->run(agent);

			return GetIValue(self);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(treeTask);
			BEHAVIAC_ASSERT(false);
        }
    };

    template<typename T>
    inline IProperty* AgentMeta::CreatorProperty(uint32_t propId, const char* propName, const char* valueStr) {
        typedef T CreateType;

        return BEHAVIAC_NEW CCustomizedProperty<CreateType>(propId, propName, valueStr);
    }

    template<typename T>
    inline IProperty* AgentMeta::CreatorArrayItemProperty(uint32_t parentId, const char* parentName) {
        typedef T CreateType;
        return BEHAVIAC_NEW CCustomizedArrayItemProperty<CreateType>(parentId, parentName);
    }

    template<typename T>
    inline IInstanceMember* AgentMeta::CreatorInstanceProperty(const char* instance, IInstanceMember* indexMember, uint32_t varId) {
        typedef T CreateType;
        return BEHAVIAC_NEW CInstanceCustomizedProperty<CreateType>(instance, indexMember, varId);
    }

    template<typename T>
    inline IInstanceMember* AgentMeta::CreatorInstanceConst(const char* value) {
        typedef T CreateType;
        return BEHAVIAC_NEW CInstanceConst<CreateType>(value);
    }

    template<typename T>
    inline IProperty* AgentMeta::CreatorCustomizedProperty(uint32_t propId, const char* propName, const char* valueStr) {
        typedef T CreateType;
        return BEHAVIAC_NEW CCustomizedProperty<CreateType>(propId, propName, valueStr);
    }

    template<typename T>
    inline IProperty* AgentMeta::CreatorCustomizedArrayItemProperty(uint32_t parentId, const char* parentName) {
        typedef T CreateType;
        return BEHAVIAC_NEW CCustomizedArrayItemProperty<CreateType>(parentId, parentName);
    }

    template<typename T>
    class CAgentMethod : public CAgentMethodBase<T> {
        typedef T(*FunctionPointer)(behaviac::Agent* a);

        FunctionPointer     _fp;

    public:
        CAgentMethod(FunctionPointer f) {
            _fp = f;
        }

        CAgentMethod(CAgentMethod<T>& rhs)
            : CAgentMethodBase<T>(rhs) { //: base(rhs)
            _fp = rhs._fp;
        }

        virtual IInstanceMember* clone() {
            CAgentMethod<T>* pT = BEHAVIAC_NEW CAgentMethod<T>(*this);
            IInstanceMember* pI = pT;

            return pI;
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
			BEHAVIAC_UNUSED_VAR(paramStrs);
			BEHAVIAC_ASSERT(paramStrs.size() == 0);

            //_instance = instance;
            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
        }

        virtual void run(behaviac::Agent* self) {
            Agent* agent = Agent::GetParentAgent(self, this->_instance);

            this->_returnValue->value = _fp(agent);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(treeTask);
		}
    };

    template<typename VariableType>
    void BehaviorTreeTask::SetVariable(const char* variableName, VariableType value) {
        uint32_t variableId = MakeVariableId(variableName);
        IInstantiatedVariable* v = this->m_localVars.find(variableId)->second;
        CVariable<VariableType>* var = (CVariable<VariableType>*)(v);

        if (var != NULL) {
            var->SetValue(NULL, &value);
            return;
        }
    }

    template<typename T, typename P1>
    class CAgentMethod_1 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;

        typedef T(*FunctionPointer)(behaviac::Agent* a, P1 p1);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;

    public:
        CAgentMethod_1(FunctionPointer f) : _p1(0) {
            _fp = f;
        }

        virtual ~CAgentMethod_1() {
            BEHAVIAC_DELETE this->_p1;
        }

        CAgentMethod_1(CAgentMethod_1<T, P1>& rhs)
            : CAgentMethodBase<T>(rhs) { //:base(rhs)
            _fp = rhs._fp;
            _p1 = rhs._p1;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethod_1<T, P1>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 1);

            //_instance = instance;
            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);

            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);

            Agent* agent = Agent::GetParentAgent(self, this->_instance);
            const bool bVector = behaviac::Meta::IsVector<StoredType1>::Result;
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, bVector, behaviac::GetClassTypeNumberId<StoredType1>());
            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);

            this->_returnValue->value = _fp(agent, (PARAM_CALLEDTYPE(P1))(pv1));
        }

        virtual IValue* GetIValueFrom(behaviac::Agent* self, IInstanceMember* firstParam) {
            BEHAVIAC_ASSERT(_p1 != NULL);

            Agent* agent = Agent::GetParentAgent(self, this->_instance);
            StoredType1* v1 = (StoredType1*)firstParam->GetValue(self);

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);

            this->_returnValue->value = _fp(agent, (PARAM_CALLEDTYPE(P1))pv1);

            return this->_returnValue;
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);
        }
    };

    template<typename T, typename P1, typename P2>
    class CAgentMethod_2 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;

        typedef T(*FunctionPointer)(behaviac::Agent* a, P1 p1, P2 p2);

        FunctionPointer         _fp;
        IInstanceMember*        _p1;
        IInstanceMember*        _p2;

    public:
        CAgentMethod_2(FunctionPointer f) : _p1(0), _p2(0) {
            _fp = f;
        }

        CAgentMethod_2(CAgentMethod_2<T, P1, P2>& rhs)
            : CAgentMethodBase<T>(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
        }

        virtual ~CAgentMethod_2() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethod_2<T, P1, P2>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 2);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);

            Agent* agent = Agent::GetParentAgent(self, this->_instance);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);

            this->_returnValue->value = _fp(agent,
                                            (PARAM_CALLEDTYPE(P1))pv1,
                                            (PARAM_CALLEDTYPE(P2))pv2);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);
        }
    };

    template<typename T, typename P1, typename P2, typename P3>
    class CAgentMethod_3 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;

        typedef T(*FunctionPointer)(behaviac::Agent* a, P1 p1, P2 p2, P3 p3);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;

    public:
        CAgentMethod_3(FunctionPointer f) : _p1(0), _p2(0), _p3(0) {
            _fp = f;
        }

        CAgentMethod_3(CAgentMethod_3<T, P1, P2, P3>& rhs)
            : CAgentMethodBase<T>(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
        }

        virtual ~CAgentMethod_3() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethod_3<T, P1, P2, P3>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 3);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);

            Agent* agent = Agent::GetParentAgent(self, this->_instance);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);

            this->_returnValue->value = _fp(agent,
                                            (PARAM_CALLEDTYPE(P1))pv1,
                                            (PARAM_CALLEDTYPE(P2))pv2,
                                            (PARAM_CALLEDTYPE(P3))pv3);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);
        }
    };

    template<typename T, typename P1, typename P2, typename P3, typename P4>
    class CAgentMethod_4 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;

        typedef T(*FunctionPointer)(behaviac::Agent* a, P1 p1, P2 p2, P3 p3, P4 p4);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;

    public:
        CAgentMethod_4(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0) {
            _fp = f;
        }

        CAgentMethod_4(CAgentMethod_4<T, P1, P2, P3, P4>& rhs)
            : CAgentMethodBase<T>(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
        }

        virtual ~CAgentMethod_4() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethod_4<T, P1, P2, P3, P4>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 4);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);

            Agent* agent = Agent::GetParentAgent(self, this->_instance);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);

            this->_returnValue->value = _fp(agent,
                                            (PARAM_CALLEDTYPE(P1))pv1,
                                            (PARAM_CALLEDTYPE(P2))pv2,
                                            (PARAM_CALLEDTYPE(P3))pv3,
                                            (PARAM_CALLEDTYPE(P4))pv4);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);
        }
    };

    template<typename T, typename P1, typename P2, typename P3, typename P4, typename P5>
    class CAgentMethod_5 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;

        typedef T(*FunctionPointer)(behaviac::Agent* a, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;

    public:
        CAgentMethod_5(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0) {
            _fp = f;
        }

        CAgentMethod_5(CAgentMethod_5<T, P1, P2, P3, P4, P5>& rhs)
            : CAgentMethodBase<T>(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
        }

        virtual ~CAgentMethod_5() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethod_5<T, P1, P2, P3, P4, P5>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 5);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);

            Agent* agent = Agent::GetParentAgent(self, this->_instance);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);

            this->_returnValue->value = _fp(agent,
                                            (PARAM_CALLEDTYPE(P1))pv1,
                                            (PARAM_CALLEDTYPE(P2))pv2,
                                            (PARAM_CALLEDTYPE(P3))pv3,
                                            (PARAM_CALLEDTYPE(P4))pv4,
                                            (PARAM_CALLEDTYPE(P5))pv5);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);
        }
    };

    template<typename T, typename P1, typename P2, typename P3, typename P4, typename P5, typename P6>
    class CAgentMethod_6 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;

        typedef T(*FunctionPointer)(behaviac::Agent* a, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;

    public:
        CAgentMethod_6(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0) {
            _fp = f;
        }

        CAgentMethod_6(CAgentMethod_6<T, P1, P2, P3, P4, P5, P6>& rhs)
            : CAgentMethodBase<T>(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
        }

        virtual ~CAgentMethod_6() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethod_6<T, P1, P2, P3, P4, P5, P6>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 6);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);

            Agent* agent = Agent::GetParentAgent(self, this->_instance);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);

            this->_returnValue->value = _fp(agent,
                                            (PARAM_CALLEDTYPE(P1))pv1,
                                            (PARAM_CALLEDTYPE(P2))pv2,
                                            (PARAM_CALLEDTYPE(P3))pv3,
                                            (PARAM_CALLEDTYPE(P4))pv4,
                                            (PARAM_CALLEDTYPE(P5))pv5,
                                            (PARAM_CALLEDTYPE(P6))pv6);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);
        }
    };

    template <typename T, typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7>
    class CAgentMethod_7 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;

        typedef T(*FunctionPointer)(behaviac::Agent* a, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;

    public:

        CAgentMethod_7(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0) {
            _fp = f;
        }

        virtual ~CAgentMethod_7() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
        }

        CAgentMethod_7(CAgentMethod_7<T, P1, P2, P3, P4, P5, P6, P7>& rhs)
            : CAgentMethodBase<T>(rhs) {

            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethod_7<T, P1, P2, P3, P4, P5, P6, P7>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 7);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);

            Agent* agent = Agent::GetParentAgent(self, this->_instance);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);

            this->_returnValue->value = _fp(agent,
                                            (PARAM_CALLEDTYPE(P1))pv1,
                                            (PARAM_CALLEDTYPE(P2))pv2,
                                            (PARAM_CALLEDTYPE(P3))pv3,
                                            (PARAM_CALLEDTYPE(P4))pv4,
                                            (PARAM_CALLEDTYPE(P5))pv5,
                                            (PARAM_CALLEDTYPE(P6))pv6,
                                            (PARAM_CALLEDTYPE(P7))pv7);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);
        }
    };

    template<typename T, typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8>
    class CAgentMethod_8 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;

        typedef T(*FunctionPointer)(behaviac::Agent* a, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;

    public:
        CAgentMethod_8(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0) {
            _fp = f;
        }

        virtual ~CAgentMethod_8() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
        }

        CAgentMethod_8(CAgentMethod_8<T, P1, P2, P3, P4, P5, P6, P7, P8>& rhs) :
            CAgentMethodBase<T>(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethod_8<T, P1, P2, P3, P4, P5, P6, P7, P8>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 8);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);

            Agent* agent = Agent::GetParentAgent(self, this->_instance);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);

            this->_returnValue->value = _fp(agent,
                                            (PARAM_CALLEDTYPE(P1))pv1,
                                            (PARAM_CALLEDTYPE(P2))pv2,
                                            (PARAM_CALLEDTYPE(P3))pv3,
                                            (PARAM_CALLEDTYPE(P4))pv4,
                                            (PARAM_CALLEDTYPE(P5))pv5,
                                            (PARAM_CALLEDTYPE(P6))pv6,
                                            (PARAM_CALLEDTYPE(P7))pv7,
                                            (PARAM_CALLEDTYPE(P8))pv8);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);
        }
    };

    template<typename T, typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8, typename P9>
    class CAgentMethod_9 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;
        typedef typename StoredTypeSelector<P9>::StoredType				StoredType9;

        typedef T(*FunctionPointer)(behaviac::Agent* a, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;
        IInstanceMember*    _p9;

    public:
        CAgentMethod_9(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0), _p9(0) {
            _fp = f;
        }

        virtual ~CAgentMethod_9() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
            BEHAVIAC_DELETE this->_p9;
        }

        CAgentMethod_9(CAgentMethod_9<T, P1, P2, P3, P4, P5, P6, P7, P8, P9>& rhs) :
            CAgentMethodBase<T>(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
            _p9 = rhs._p9;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethod_9<T, P1, P2, P3, P4, P5, P6, P7, P8, P9>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 9);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
            _p9 = AgentMeta::TParseProperty<StoredType9>(paramStrs[8].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);
            BEHAVIAC_ASSERT(_p9 != NULL);

            Agent* agent = Agent::GetParentAgent(self, this->_instance);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);

            this->_returnValue->value = _fp(agent,
                                            (PARAM_CALLEDTYPE(P1))pv1,
                                            (PARAM_CALLEDTYPE(P2))pv2,
                                            (PARAM_CALLEDTYPE(P3))pv3,
                                            (PARAM_CALLEDTYPE(P4))pv4,
                                            (PARAM_CALLEDTYPE(P5))pv5,
                                            (PARAM_CALLEDTYPE(P6))pv6,
                                            (PARAM_CALLEDTYPE(P7))pv7,
                                            (PARAM_CALLEDTYPE(P8))pv8,
                                            (PARAM_CALLEDTYPE(P9))pv9);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 8);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P9))pv9);
        }
    };

    template<typename T, typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8, typename P9, typename P10>
    class CAgentMethod_10 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;
        typedef typename StoredTypeSelector<P9>::StoredType				StoredType9;
        typedef typename StoredTypeSelector<P10>::StoredType			StoredType10;

        typedef T(*FunctionPointer)(behaviac::Agent* a, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;
        IInstanceMember*    _p9;
        IInstanceMember*    _p10;

    public:
        CAgentMethod_10(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0), _p9(0), _p10(0) {
            _fp = f;
        }

        virtual ~CAgentMethod_10() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
            BEHAVIAC_DELETE this->_p9;
            BEHAVIAC_DELETE this->_p10;
        }

        CAgentMethod_10(CAgentMethod_10<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>& rhs) :
            CAgentMethodBase<T>(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
            _p9 = rhs._p9;
            _p10 = rhs._p10;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethod_10<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 10);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
            _p9 = AgentMeta::TParseProperty<StoredType9>(paramStrs[8].c_str());
            _p10 = AgentMeta::TParseProperty<StoredType10>(paramStrs[9].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);
            BEHAVIAC_ASSERT(_p9 != NULL);
            BEHAVIAC_ASSERT(_p10 != NULL);

            Agent* agent = Agent::GetParentAgent(self, this->_instance);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);

            this->_returnValue->value = _fp(agent,
                                            (PARAM_CALLEDTYPE(P1))pv1,
                                            (PARAM_CALLEDTYPE(P2))pv2,
                                            (PARAM_CALLEDTYPE(P3))pv3,
                                            (PARAM_CALLEDTYPE(P4))pv4,
                                            (PARAM_CALLEDTYPE(P5))pv5,
                                            (PARAM_CALLEDTYPE(P6))pv6,
                                            (PARAM_CALLEDTYPE(P7))pv7,
                                            (PARAM_CALLEDTYPE(P8))pv8,
                                            (PARAM_CALLEDTYPE(P9))pv9,
                                            (PARAM_CALLEDTYPE(P10))pv10);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 8);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P9))pv9);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 9);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P10))pv10);
        }
    };

    template<typename T, typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8, typename P9, typename P10, typename P11>
    class CAgentMethod_11 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;
        typedef typename StoredTypeSelector<P9>::StoredType				StoredType9;
        typedef typename StoredTypeSelector<P10>::StoredType			StoredType10;
        typedef typename StoredTypeSelector<P11>::StoredType			StoredType11;

        typedef T(*FunctionPointer)(behaviac::Agent* a, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;
        IInstanceMember*    _p9;
        IInstanceMember*    _p10;
        IInstanceMember*    _p11;

    public:
        CAgentMethod_11(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0), _p9(0), _p10(0), _p11(0) {
            _fp = f;
        }

        virtual ~CAgentMethod_11() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
            BEHAVIAC_DELETE this->_p9;
            BEHAVIAC_DELETE this->_p10;
            BEHAVIAC_DELETE this->_p11;
        }

        CAgentMethod_11(CAgentMethod_11<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>& rhs) :
            CAgentMethodBase<T>(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
            _p9 = rhs._p9;
            _p10 = rhs._p10;
            _p11 = rhs._p11;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethod_11<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 11);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
            _p9 = AgentMeta::TParseProperty<StoredType9>(paramStrs[8].c_str());
            _p10 = AgentMeta::TParseProperty<StoredType10>(paramStrs[9].c_str());
            _p11 = AgentMeta::TParseProperty<StoredType11>(paramStrs[10].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);
            BEHAVIAC_ASSERT(_p9 != NULL);
            BEHAVIAC_ASSERT(_p10 != NULL);
            BEHAVIAC_ASSERT(_p11 != NULL);

            Agent* agent = Agent::GetParentAgent(self, this->_instance);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);

            this->_returnValue->value = _fp(agent,
                                            (PARAM_CALLEDTYPE(P1))pv1,
                                            (PARAM_CALLEDTYPE(P2))pv2,
                                            (PARAM_CALLEDTYPE(P3))pv3,
                                            (PARAM_CALLEDTYPE(P4))pv4,
                                            (PARAM_CALLEDTYPE(P5))pv5,
                                            (PARAM_CALLEDTYPE(P6))pv6,
                                            (PARAM_CALLEDTYPE(P7))pv7,
                                            (PARAM_CALLEDTYPE(P8))pv8,
                                            (PARAM_CALLEDTYPE(P9))pv9,
                                            (PARAM_CALLEDTYPE(P10))pv10,
                                            (PARAM_CALLEDTYPE(P11))pv11);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 8);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P9))pv9);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 9);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P10))pv10);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 10);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P11))pv11);
        }
    };

    template<typename T, typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8, typename P9, typename P10, typename P11, typename P12>
    class CAgentMethod_12 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;
        typedef typename StoredTypeSelector<P9>::StoredType				StoredType9;
        typedef typename StoredTypeSelector<P10>::StoredType			StoredType10;
        typedef typename StoredTypeSelector<P11>::StoredType			StoredType11;
        typedef typename StoredTypeSelector<P12>::StoredType			StoredType12;

        typedef T(*FunctionPointer)(behaviac::Agent* a, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, P12 p12);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;
        IInstanceMember*    _p9;
        IInstanceMember*    _p10;
        IInstanceMember*    _p11;
        IInstanceMember*    _p12;

    public:
        CAgentMethod_12(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0), _p9(0), _p10(0), _p11(0), _p12(0) {
            _fp = f;
        }

        virtual ~CAgentMethod_12() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
            BEHAVIAC_DELETE this->_p9;
            BEHAVIAC_DELETE this->_p10;
            BEHAVIAC_DELETE this->_p11;
            BEHAVIAC_DELETE this->_p12;
        }

        CAgentMethod_12(CAgentMethod_12<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12>& rhs) :
            CAgentMethodBase<T>(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
            _p9 = rhs._p9;
            _p10 = rhs._p10;
            _p11 = rhs._p11;
            _p12 = rhs._p12;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethod_12<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 12);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
            _p9 = AgentMeta::TParseProperty<StoredType9>(paramStrs[8].c_str());
            _p10 = AgentMeta::TParseProperty<StoredType10>(paramStrs[9].c_str());
            _p11 = AgentMeta::TParseProperty<StoredType11>(paramStrs[10].c_str());
            _p12 = AgentMeta::TParseProperty<StoredType12>(paramStrs[11].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);
            BEHAVIAC_ASSERT(_p9 != NULL);
            BEHAVIAC_ASSERT(_p10 != NULL);
            BEHAVIAC_ASSERT(_p11 != NULL);
            BEHAVIAC_ASSERT(_p12 != NULL);

            Agent* agent = Agent::GetParentAgent(self, this->_instance);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());
            StoredType12* v12 = (StoredType12*)_p12->GetValue(self, behaviac::Meta::IsVector<StoredType12>::Result, behaviac::GetClassTypeNumberId<StoredType12>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);
            typename ParamHandler<StoredType12>::Type pv12 = ParamHandler<StoredType12>::GetValue(v12);

            this->_returnValue->value = _fp(agent,
                                            (PARAM_CALLEDTYPE(P1))pv1,
                                            (PARAM_CALLEDTYPE(P2))pv2,
                                            (PARAM_CALLEDTYPE(P3))pv3,
                                            (PARAM_CALLEDTYPE(P4))pv4,
                                            (PARAM_CALLEDTYPE(P5))pv5,
                                            (PARAM_CALLEDTYPE(P6))pv6,
                                            (PARAM_CALLEDTYPE(P7))pv7,
                                            (PARAM_CALLEDTYPE(P8))pv8,
                                            (PARAM_CALLEDTYPE(P9))pv9,
                                            (PARAM_CALLEDTYPE(P10))pv10,
                                            (PARAM_CALLEDTYPE(P11))pv11,
                                            (PARAM_CALLEDTYPE(P12))pv12);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());
            StoredType12* v12 = (StoredType12*)_p12->GetValue(self, behaviac::Meta::IsVector<StoredType12>::Result, behaviac::GetClassTypeNumberId<StoredType12>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);
            typename ParamHandler<StoredType12>::Type pv12 = ParamHandler<StoredType12>::GetValue(v12);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 8);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P9))pv9);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 9);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P10))pv10);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 10);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P11))pv11);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 11);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P12))pv12);
        }
    };

    template<typename T, typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8, typename P9, typename P10, typename P11, typename P12, typename P13>
    class CAgentMethod_13 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;
        typedef typename StoredTypeSelector<P9>::StoredType				StoredType9;
        typedef typename StoredTypeSelector<P10>::StoredType			StoredType10;
        typedef typename StoredTypeSelector<P11>::StoredType			StoredType11;
        typedef typename StoredTypeSelector<P12>::StoredType			StoredType12;
        typedef typename StoredTypeSelector<P13>::StoredType			StoredType13;

        typedef T(*FunctionPointer)(behaviac::Agent* a, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, P12 p12, P13 p13);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;
        IInstanceMember*    _p9;
        IInstanceMember*    _p10;
        IInstanceMember*    _p11;
        IInstanceMember*    _p12;
        IInstanceMember*    _p13;

    public:
        CAgentMethod_13(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0), _p9(0), _p10(0), _p11(0), _p12(0), _p13(0) {
            _fp = f;
        }

        virtual ~CAgentMethod_13() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
            BEHAVIAC_DELETE this->_p9;
            BEHAVIAC_DELETE this->_p10;
            BEHAVIAC_DELETE this->_p11;
            BEHAVIAC_DELETE this->_p12;
            BEHAVIAC_DELETE this->_p13;
        }

        CAgentMethod_13(CAgentMethod_13<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13>& rhs) :
            CAgentMethodBase<T>(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
            _p9 = rhs._p9;
            _p10 = rhs._p10;
            _p11 = rhs._p11;
            _p12 = rhs._p12;
            _p13 = rhs._p13;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethod_13<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 13);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
            _p9 = AgentMeta::TParseProperty<StoredType9>(paramStrs[8].c_str());
            _p10 = AgentMeta::TParseProperty<StoredType10>(paramStrs[9].c_str());
            _p11 = AgentMeta::TParseProperty<StoredType11>(paramStrs[10].c_str());
            _p12 = AgentMeta::TParseProperty<StoredType12>(paramStrs[11].c_str());
            _p13 = AgentMeta::TParseProperty<StoredType13>(paramStrs[12].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);
            BEHAVIAC_ASSERT(_p9 != NULL);
            BEHAVIAC_ASSERT(_p10 != NULL);
            BEHAVIAC_ASSERT(_p11 != NULL);
            BEHAVIAC_ASSERT(_p12 != NULL);
            BEHAVIAC_ASSERT(_p13 != NULL);

            Agent* agent = Agent::GetParentAgent(self, this->_instance);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());
            StoredType12* v12 = (StoredType12*)_p12->GetValue(self, behaviac::Meta::IsVector<StoredType12>::Result, behaviac::GetClassTypeNumberId<StoredType12>());
            StoredType13* v13 = (StoredType13*)_p13->GetValue(self, behaviac::Meta::IsVector<StoredType13>::Result, behaviac::GetClassTypeNumberId<StoredType13>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);
            typename ParamHandler<StoredType12>::Type pv12 = ParamHandler<StoredType12>::GetValue(v12);
            typename ParamHandler<StoredType13>::Type pv13 = ParamHandler<StoredType13>::GetValue(v13);

            this->_returnValue->value = _fp(agent,
                                            (PARAM_CALLEDTYPE(P1))pv1,
                                            (PARAM_CALLEDTYPE(P2))pv2,
                                            (PARAM_CALLEDTYPE(P3))pv3,
                                            (PARAM_CALLEDTYPE(P4))pv4,
                                            (PARAM_CALLEDTYPE(P5))pv5,
                                            (PARAM_CALLEDTYPE(P6))pv6,
                                            (PARAM_CALLEDTYPE(P7))pv7,
                                            (PARAM_CALLEDTYPE(P8))pv8,
                                            (PARAM_CALLEDTYPE(P9))pv9,
                                            (PARAM_CALLEDTYPE(P10))pv10,
                                            (PARAM_CALLEDTYPE(P11))pv11,
                                            (PARAM_CALLEDTYPE(P12))pv12,
                                            (PARAM_CALLEDTYPE(P13))pv13);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());
            StoredType12* v12 = (StoredType12*)_p12->GetValue(self, behaviac::Meta::IsVector<StoredType12>::Result, behaviac::GetClassTypeNumberId<StoredType12>());
            StoredType13* v13 = (StoredType13*)_p13->GetValue(self, behaviac::Meta::IsVector<StoredType13>::Result, behaviac::GetClassTypeNumberId<StoredType13>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);
            typename ParamHandler<StoredType12>::Type pv12 = ParamHandler<StoredType12>::GetValue(v12);
            typename ParamHandler<StoredType13>::Type pv13 = ParamHandler<StoredType13>::GetValue(v13);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 8);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P9))pv9);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 9);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P10))pv10);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 10);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P11))pv11);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 11);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P12))pv12);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 12);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P13))pv13);
        }
    };

    template<typename T, typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8, typename P9, typename P10, typename P11, typename P12, typename P13, typename P14>
    class CAgentMethod_14 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;
        typedef typename StoredTypeSelector<P9>::StoredType				StoredType9;
        typedef typename StoredTypeSelector<P10>::StoredType			StoredType10;
        typedef typename StoredTypeSelector<P11>::StoredType			StoredType11;
        typedef typename StoredTypeSelector<P12>::StoredType			StoredType12;
        typedef typename StoredTypeSelector<P13>::StoredType			StoredType13;
        typedef typename StoredTypeSelector<P14>::StoredType			StoredType14;

        typedef T(*FunctionPointer)(behaviac::Agent* a, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, P12 p12, P13 p13, P14 p14);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;
        IInstanceMember*    _p9;
        IInstanceMember*    _p10;
        IInstanceMember*    _p11;
        IInstanceMember*    _p12;
        IInstanceMember*    _p13;
        IInstanceMember*    _p14;

    public:
        CAgentMethod_14(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0), _p9(0), _p10(0), _p11(0), _p12(0), _p13(0), _p14(0) {
            _fp = f;
        }

        virtual ~CAgentMethod_14() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
            BEHAVIAC_DELETE this->_p9;
            BEHAVIAC_DELETE this->_p10;
            BEHAVIAC_DELETE this->_p11;
            BEHAVIAC_DELETE this->_p12;
            BEHAVIAC_DELETE this->_p13;
            BEHAVIAC_DELETE this->_p14;
        }

        CAgentMethod_14(CAgentMethod_14<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14>& rhs) :
            CAgentMethodBase<T>(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
            _p9 = rhs._p9;
            _p10 = rhs._p10;
            _p11 = rhs._p11;
            _p12 = rhs._p12;
            _p13 = rhs._p13;
            _p14 = rhs._p14;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethod_14<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 14);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
            _p9 = AgentMeta::TParseProperty<StoredType9>(paramStrs[8].c_str());
            _p10 = AgentMeta::TParseProperty<StoredType10>(paramStrs[9].c_str());
            _p11 = AgentMeta::TParseProperty<StoredType11>(paramStrs[10].c_str());
            _p12 = AgentMeta::TParseProperty<StoredType12>(paramStrs[11].c_str());
            _p13 = AgentMeta::TParseProperty<StoredType13>(paramStrs[12].c_str());
            _p14 = AgentMeta::TParseProperty<StoredType14>(paramStrs[13].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);
            BEHAVIAC_ASSERT(_p9 != NULL);
            BEHAVIAC_ASSERT(_p10 != NULL);
            BEHAVIAC_ASSERT(_p11 != NULL);
            BEHAVIAC_ASSERT(_p12 != NULL);
            BEHAVIAC_ASSERT(_p13 != NULL);
            BEHAVIAC_ASSERT(_p14 != NULL);

            Agent* agent = Agent::GetParentAgent(self, this->_instance);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());
            StoredType12* v12 = (StoredType12*)_p12->GetValue(self, behaviac::Meta::IsVector<StoredType12>::Result, behaviac::GetClassTypeNumberId<StoredType12>());
            StoredType13* v13 = (StoredType13*)_p13->GetValue(self, behaviac::Meta::IsVector<StoredType13>::Result, behaviac::GetClassTypeNumberId<StoredType13>());
            StoredType14* v14 = (StoredType14*)_p14->GetValue(self, behaviac::Meta::IsVector<StoredType14>::Result, behaviac::GetClassTypeNumberId<StoredType14>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);
            typename ParamHandler<StoredType12>::Type pv12 = ParamHandler<StoredType12>::GetValue(v12);
            typename ParamHandler<StoredType13>::Type pv13 = ParamHandler<StoredType13>::GetValue(v13);
            typename ParamHandler<StoredType14>::Type pv14 = ParamHandler<StoredType14>::GetValue(v14);

            this->_returnValue->value = _fp(agent,
                                            (PARAM_CALLEDTYPE(P1))pv1,
                                            (PARAM_CALLEDTYPE(P2))pv2,
                                            (PARAM_CALLEDTYPE(P3))pv3,
                                            (PARAM_CALLEDTYPE(P4))pv4,
                                            (PARAM_CALLEDTYPE(P5))pv5,
                                            (PARAM_CALLEDTYPE(P6))pv6,
                                            (PARAM_CALLEDTYPE(P7))pv7,
                                            (PARAM_CALLEDTYPE(P8))pv8,
                                            (PARAM_CALLEDTYPE(P9))pv9,
                                            (PARAM_CALLEDTYPE(P10))pv10,
                                            (PARAM_CALLEDTYPE(P11))pv11,
                                            (PARAM_CALLEDTYPE(P12))pv12,
                                            (PARAM_CALLEDTYPE(P13))pv13,
                                            (PARAM_CALLEDTYPE(P14))pv14);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());
            StoredType12* v12 = (StoredType12*)_p12->GetValue(self, behaviac::Meta::IsVector<StoredType12>::Result, behaviac::GetClassTypeNumberId<StoredType12>());
            StoredType13* v13 = (StoredType13*)_p13->GetValue(self, behaviac::Meta::IsVector<StoredType13>::Result, behaviac::GetClassTypeNumberId<StoredType13>());
            StoredType14* v14 = (StoredType14*)_p14->GetValue(self, behaviac::Meta::IsVector<StoredType14>::Result, behaviac::GetClassTypeNumberId<StoredType14>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);
            typename ParamHandler<StoredType12>::Type pv12 = ParamHandler<StoredType12>::GetValue(v12);
            typename ParamHandler<StoredType13>::Type pv13 = ParamHandler<StoredType13>::GetValue(v13);
            typename ParamHandler<StoredType14>::Type pv14 = ParamHandler<StoredType14>::GetValue(v14);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 8);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P9))pv9);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 9);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P10))pv10);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 10);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P11))pv11);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 11);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P12))pv12);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 12);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P13))pv13);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 13);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P14))pv14);
        }
    };

    template<typename T>
    class CAgentStaticMethod : public CAgentMethodBase<T> {
        typedef T(*FunctionPointer)();

        FunctionPointer     _fp;

    public:
        CAgentStaticMethod(FunctionPointer f) {
            _fp = f;
        }

        CAgentStaticMethod(CAgentStaticMethod<T>& rhs)
            : CAgentMethodBase<T>(rhs) {
            _fp = rhs._fp;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethod<T>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 0);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
        }

        virtual void run(behaviac::Agent* self) {
            this->_returnValue->value = _fp();
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
        }
    };

    template<typename T, typename P1>
    class CAgentStaticMethod_1 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;

        typedef T(*FunctionPointer)(P1 p1);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;

    public:
        CAgentStaticMethod_1(FunctionPointer f) : _p1(0) {
            _fp = f;
        }

        CAgentStaticMethod_1(CAgentStaticMethod_1<T, P1>& rhs)
            : CAgentMethodBase<T>(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
        }

        virtual ~CAgentStaticMethod_1() {
            BEHAVIAC_DELETE this->_p1;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethod_1<T, P1>(*this);
        }
        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 1);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            bool isVector = behaviac::Meta::IsVector<StoredType1>::Result;
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, isVector, behaviac::GetClassTypeNumberId<StoredType1>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);

            this->_returnValue->value = _fp((PARAM_CALLEDTYPE(P1))pv1);
        }

        virtual IValue* GetIValueFrom(behaviac::Agent* self, IInstanceMember* firstParam) {
            BEHAVIAC_ASSERT(_p1 != NULL);

            StoredType1* v1 = (StoredType1*)firstParam->GetValue(self);

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);

            this->_returnValue->value = _fp((PARAM_CALLEDTYPE(P1))pv1);

            return this->_returnValue;
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);
        }
    };

    template<typename T, typename P1, typename P2>
    class CAgentStaticMethod_2 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;

        typedef T(*FunctionPointer)(P1 p1, P2 p2);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;

    public:
        CAgentStaticMethod_2(FunctionPointer f) : _p1(0), _p2(0) {
            _fp = f;
        }

        CAgentStaticMethod_2(CAgentStaticMethod_2<T, P1, P2>& rhs)
            : CAgentMethodBase<T>(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
        }

        virtual ~CAgentStaticMethod_2() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethod_2<T, P1, P2>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 2);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);

            this->_returnValue->value = _fp((PARAM_CALLEDTYPE(P1))pv1, (PARAM_CALLEDTYPE(P2))pv2);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);
        }
    };

    template<typename T, typename P1, typename P2, typename P3>
    class CAgentStaticMethod_3 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;

        typedef T(*FunctionPointer)(P1 p1, P2 p2, P3 p3);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;

    public:
        CAgentStaticMethod_3(FunctionPointer f) : _p1(0), _p2(0), _p3(0) {
            _fp = f;
        }

        CAgentStaticMethod_3(CAgentStaticMethod_3<T, P1, P2, P3>& rhs)
            : CAgentMethodBase<T>(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
        }

        virtual ~CAgentStaticMethod_3() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethod_3<T, P1, P2, P3>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 3);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);

            this->_returnValue->value = _fp(
                                            (PARAM_CALLEDTYPE(P1))pv1,
                                            (PARAM_CALLEDTYPE(P2))pv2,
                                            (PARAM_CALLEDTYPE(P3))pv3);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);
        }
    };

    template<typename T, typename P1, typename P2, typename P3, typename P4>
    class CAgentStaticMethod_4 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;

        typedef T(*FunctionPointer)(P1 p1, P2 p2, P3 p3, P4 p4);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;

    public:
        CAgentStaticMethod_4(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0) {
            _fp = f;
        }

        CAgentStaticMethod_4(CAgentStaticMethod_4<T, P1, P2, P3, P4>& rhs)
            : CAgentMethodBase<T>(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
        }

        virtual ~CAgentStaticMethod_4() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethod_4<T, P1, P2, P3, P4>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 4);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);

            this->_returnValue->value = _fp(
                                            (PARAM_CALLEDTYPE(P1))pv1,
                                            (PARAM_CALLEDTYPE(P2))pv2,
                                            (PARAM_CALLEDTYPE(P3))pv3,
                                            (PARAM_CALLEDTYPE(P4))pv4);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);
        }
    };

    template<typename T, typename P1, typename P2, typename P3, typename P4, typename P5>
    class CAgentStaticMethod_5 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;

        typedef T(*FunctionPointer)(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;

    public:
        CAgentStaticMethod_5(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0) {
            _fp = f;
        }

        CAgentStaticMethod_5(CAgentStaticMethod_5<T, P1, P2, P3, P4, P5>& rhs)
            : CAgentMethodBase<T>(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
        }

        virtual ~CAgentStaticMethod_5() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethod_5<T, P1, P2, P3, P4, P5>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 5);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);

            this->_returnValue->value = _fp(
                                            (PARAM_CALLEDTYPE(P1))pv1,
                                            (PARAM_CALLEDTYPE(P2))pv2,
                                            (PARAM_CALLEDTYPE(P3))pv3,
                                            (PARAM_CALLEDTYPE(P4))pv4,
                                            (PARAM_CALLEDTYPE(P5))pv5);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);
        }
    };

    template<typename T, typename P1, typename P2, typename P3, typename P4, typename P5, typename P6>
    class CAgentStaticMethod_6 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;

        typedef T(*FunctionPointer)(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;

    public:
        CAgentStaticMethod_6(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0) {
            _fp = f;
        }

        CAgentStaticMethod_6(CAgentStaticMethod_6<T, P1, P2, P3, P4, P5, P6>& rhs)
            : CAgentMethodBase<T>(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
        }

        virtual ~CAgentStaticMethod_6() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethod_6<T, P1, P2, P3, P4, P5, P6>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 6);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);

            this->_returnValue->value = _fp(
                                            (PARAM_CALLEDTYPE(P1))pv1,
                                            (PARAM_CALLEDTYPE(P2))pv2,
                                            (PARAM_CALLEDTYPE(P3))pv3,
                                            (PARAM_CALLEDTYPE(P4))pv4,
                                            (PARAM_CALLEDTYPE(P5))pv5,
                                            (PARAM_CALLEDTYPE(P6))pv6);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);
        }
    };

    template <typename T, typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7>
    class CAgentStaticMethod_7 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;

        typedef T(*FunctionPointer)(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;

    public:
        CAgentStaticMethod_7(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0) {
            _fp = f;
        }

        CAgentStaticMethod_7(CAgentStaticMethod_7<T, P1, P2, P3, P4, P5, P6, P7>& rhs)
            : CAgentMethodBase<T>(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
        }

        virtual ~CAgentStaticMethod_7() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethod_7<T, P1, P2, P3, P4, P5, P6, P7>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 7);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);

            this->_returnValue->value = _fp(
                                            (PARAM_CALLEDTYPE(P1))pv1,
                                            (PARAM_CALLEDTYPE(P2))pv2,
                                            (PARAM_CALLEDTYPE(P3))pv3,
                                            (PARAM_CALLEDTYPE(P4))pv4,
                                            (PARAM_CALLEDTYPE(P5))pv5,
                                            (PARAM_CALLEDTYPE(P6))pv6,
                                            (PARAM_CALLEDTYPE(P7))pv7);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);
        }
    };

    template<typename T, typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8>
    class CAgentStaticMethod_8 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;

        typedef T(*FunctionPointer)(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;

    public:
        CAgentStaticMethod_8(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0) {
            _fp = f;
        }

        CAgentStaticMethod_8(CAgentStaticMethod_8<T, P1, P2, P3, P4, P5, P6, P7, P8>& rhs)
            : CAgentMethodBase<T>(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
        }
        virtual ~CAgentStaticMethod_8() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
        }
        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethod_8<T, P1, P2, P3, P4, P5, P6, P7, P8>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 8);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);

            this->_returnValue->value = _fp(
                                            (PARAM_CALLEDTYPE(P1))pv1,
                                            (PARAM_CALLEDTYPE(P2))pv2,
                                            (PARAM_CALLEDTYPE(P3))pv3,
                                            (PARAM_CALLEDTYPE(P4))pv4,
                                            (PARAM_CALLEDTYPE(P5))pv5,
                                            (PARAM_CALLEDTYPE(P6))pv6,
                                            (PARAM_CALLEDTYPE(P7))pv7,
                                            (PARAM_CALLEDTYPE(P8))pv8);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);
        }
    };

    template<typename T, typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8, typename P9>
    class CAgentStaticMethod_9 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;
        typedef typename StoredTypeSelector<P9>::StoredType				StoredType9;

        typedef T(*FunctionPointer)(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;
        IInstanceMember*    _p9;

    public:
        CAgentStaticMethod_9(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0), _p9(0) {
            _fp = f;
        }

        CAgentStaticMethod_9(CAgentStaticMethod_9<T, P1, P2, P3, P4, P5, P6, P7, P8, P9>& rhs)
            : CAgentMethodBase<T>(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
            _p9 = rhs._p9;
        }
        virtual ~CAgentStaticMethod_9() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
            BEHAVIAC_DELETE this->_p9;
        }
        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethod_9<T, P1, P2, P3, P4, P5, P6, P7, P8, P9>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 9);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
            _p9 = AgentMeta::TParseProperty<StoredType9>(paramStrs[8].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);
            BEHAVIAC_ASSERT(_p9 != NULL);

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);

            this->_returnValue->value = _fp(
                                            (PARAM_CALLEDTYPE(P1))pv1,
                                            (PARAM_CALLEDTYPE(P2))pv2,
                                            (PARAM_CALLEDTYPE(P3))pv3,
                                            (PARAM_CALLEDTYPE(P4))pv4,
                                            (PARAM_CALLEDTYPE(P5))pv5,
                                            (PARAM_CALLEDTYPE(P6))pv6,
                                            (PARAM_CALLEDTYPE(P7))pv7,
                                            (PARAM_CALLEDTYPE(P8))pv8,
                                            (PARAM_CALLEDTYPE(P9))pv9);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 8);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P9))pv9);
        }
    };

    template<typename T, typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8, typename P9, typename P10>
    class CAgentStaticMethod_10 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;
        typedef typename StoredTypeSelector<P9>::StoredType				StoredType9;
        typedef typename StoredTypeSelector<P10>::StoredType			StoredType10;

        typedef T(*FunctionPointer)(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;
        IInstanceMember*    _p9;
        IInstanceMember*    _p10;

    public:
        CAgentStaticMethod_10(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0), _p9(0), _p10(0) {
            _fp = f;
        }

        CAgentStaticMethod_10(CAgentStaticMethod_10<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>& rhs)
            : CAgentMethodBase<T>(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
            _p9 = rhs._p9;
            _p10 = rhs._p10;
        }
        virtual ~CAgentStaticMethod_10() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
            BEHAVIAC_DELETE this->_p9;
            BEHAVIAC_DELETE this->_p10;
        }
        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethod_10<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 10);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
            _p9 = AgentMeta::TParseProperty<StoredType9>(paramStrs[8].c_str());
            _p10 = AgentMeta::TParseProperty<StoredType10>(paramStrs[9].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);
            BEHAVIAC_ASSERT(_p9 != NULL);
            BEHAVIAC_ASSERT(_p10 != NULL);

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);

            this->_returnValue->value = _fp(
                                            (PARAM_CALLEDTYPE(P1))pv1,
                                            (PARAM_CALLEDTYPE(P2))pv2,
                                            (PARAM_CALLEDTYPE(P3))pv3,
                                            (PARAM_CALLEDTYPE(P4))pv4,
                                            (PARAM_CALLEDTYPE(P5))pv5,
                                            (PARAM_CALLEDTYPE(P6))pv6,
                                            (PARAM_CALLEDTYPE(P7))pv7,
                                            (PARAM_CALLEDTYPE(P8))pv8,
                                            (PARAM_CALLEDTYPE(P9))pv9,
                                            (PARAM_CALLEDTYPE(P10))pv10);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 8);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P9))pv9);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 9);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P10))pv10);
        }
    };

    template<typename T, typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8, typename P9, typename P10, typename P11>
    class CAgentStaticMethod_11 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;
        typedef typename StoredTypeSelector<P9>::StoredType				StoredType9;
        typedef typename StoredTypeSelector<P10>::StoredType			StoredType10;
        typedef typename StoredTypeSelector<P11>::StoredType			StoredType11;

        typedef T(*FunctionPointer)(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;
        IInstanceMember*    _p9;
        IInstanceMember*    _p10;
        IInstanceMember*    _p11;

    public:
        CAgentStaticMethod_11(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0), _p9(0), _p10(0), _p11(0) {
            _fp = f;
        }

        CAgentStaticMethod_11(CAgentStaticMethod_11<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>& rhs)
            : CAgentMethodBase<T>(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
            _p9 = rhs._p9;
            _p10 = rhs._p10;
            _p11 = rhs._p11;
        }
        virtual ~CAgentStaticMethod_11() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
            BEHAVIAC_DELETE this->_p9;
            BEHAVIAC_DELETE this->_p10;
            BEHAVIAC_DELETE this->_p11;
        }
        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethod_11<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 11);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
            _p9 = AgentMeta::TParseProperty<StoredType9>(paramStrs[8].c_str());
            _p10 = AgentMeta::TParseProperty<StoredType10>(paramStrs[9].c_str());
            _p11 = AgentMeta::TParseProperty<StoredType11>(paramStrs[10].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);
            BEHAVIAC_ASSERT(_p9 != NULL);
            BEHAVIAC_ASSERT(_p10 != NULL);
            BEHAVIAC_ASSERT(_p11 != NULL);

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);

            this->_returnValue->value = _fp(
                                            (PARAM_CALLEDTYPE(P1))pv1,
                                            (PARAM_CALLEDTYPE(P2))pv2,
                                            (PARAM_CALLEDTYPE(P3))pv3,
                                            (PARAM_CALLEDTYPE(P4))pv4,
                                            (PARAM_CALLEDTYPE(P5))pv5,
                                            (PARAM_CALLEDTYPE(P6))pv6,
                                            (PARAM_CALLEDTYPE(P7))pv7,
                                            (PARAM_CALLEDTYPE(P8))pv8,
                                            (PARAM_CALLEDTYPE(P9))pv9,
                                            (PARAM_CALLEDTYPE(P10))pv10,
                                            (PARAM_CALLEDTYPE(P11))pv11);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 8);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P9))pv9);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 9);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P10))pv10);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 10);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P11))pv11);
        }
    };

    template<typename T, typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8, typename P9, typename P10, typename P11, typename P12>
    class CAgentStaticMethod_12 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;
        typedef typename StoredTypeSelector<P9>::StoredType				StoredType9;
        typedef typename StoredTypeSelector<P10>::StoredType			StoredType10;
        typedef typename StoredTypeSelector<P11>::StoredType			StoredType11;
        typedef typename StoredTypeSelector<P12>::StoredType			StoredType12;

        typedef T(*FunctionPointer)(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, P12 p12);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;
        IInstanceMember*    _p9;
        IInstanceMember*    _p10;
        IInstanceMember*    _p11;
        IInstanceMember*    _p12;

    public:
        CAgentStaticMethod_12(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0), _p9(0), _p10(0), _p11(0), _p12(0) {
            _fp = f;
        }

        CAgentStaticMethod_12(CAgentStaticMethod_12<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12>& rhs)
            : CAgentMethodBase<T>(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
            _p9 = rhs._p9;
            _p10 = rhs._p10;
            _p11 = rhs._p11;
            _p12 = rhs._p12;
        }
        virtual ~CAgentStaticMethod_12() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
            BEHAVIAC_DELETE this->_p9;
            BEHAVIAC_DELETE this->_p10;
            BEHAVIAC_DELETE this->_p11;
            BEHAVIAC_DELETE this->_p12;
        }
        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethod_12<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 12);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
            _p9 = AgentMeta::TParseProperty<StoredType9>(paramStrs[8].c_str());
            _p10 = AgentMeta::TParseProperty<StoredType10>(paramStrs[9].c_str());
            _p11 = AgentMeta::TParseProperty<StoredType11>(paramStrs[10].c_str());
            _p12 = AgentMeta::TParseProperty<StoredType12>(paramStrs[11].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);
            BEHAVIAC_ASSERT(_p9 != NULL);
            BEHAVIAC_ASSERT(_p10 != NULL);
            BEHAVIAC_ASSERT(_p11 != NULL);
            BEHAVIAC_ASSERT(_p12 != NULL);

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());
            StoredType12* v12 = (StoredType12*)_p12->GetValue(self, behaviac::Meta::IsVector<StoredType12>::Result, behaviac::GetClassTypeNumberId<StoredType12>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);
            typename ParamHandler<StoredType12>::Type pv12 = ParamHandler<StoredType12>::GetValue(v12);

            this->_returnValue->value = _fp(
                                            (PARAM_CALLEDTYPE(P1))pv1,
                                            (PARAM_CALLEDTYPE(P2))pv2,
                                            (PARAM_CALLEDTYPE(P3))pv3,
                                            (PARAM_CALLEDTYPE(P4))pv4,
                                            (PARAM_CALLEDTYPE(P5))pv5,
                                            (PARAM_CALLEDTYPE(P6))pv6,
                                            (PARAM_CALLEDTYPE(P7))pv7,
                                            (PARAM_CALLEDTYPE(P8))pv8,
                                            (PARAM_CALLEDTYPE(P9))pv9,
                                            (PARAM_CALLEDTYPE(P10))pv10,
                                            (PARAM_CALLEDTYPE(P11))pv11,
                                            (PARAM_CALLEDTYPE(P12))pv12);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());
            StoredType12* v12 = (StoredType12*)_p12->GetValue(self, behaviac::Meta::IsVector<StoredType12>::Result, behaviac::GetClassTypeNumberId<StoredType12>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);
            typename ParamHandler<StoredType12>::Type pv12 = ParamHandler<StoredType12>::GetValue(v12);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 8);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P9))pv9);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 9);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P10))pv10);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 10);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P11))pv11);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 11);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P12))pv12);
        }
    };

    template<typename T, typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8, typename P9, typename P10, typename P11, typename P12, typename P13>
    class CAgentStaticMethod_13 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;
        typedef typename StoredTypeSelector<P9>::StoredType				StoredType9;
        typedef typename StoredTypeSelector<P10>::StoredType			StoredType10;
        typedef typename StoredTypeSelector<P11>::StoredType			StoredType11;
        typedef typename StoredTypeSelector<P12>::StoredType			StoredType12;
        typedef typename StoredTypeSelector<P13>::StoredType			StoredType13;

        typedef T(*FunctionPointer)(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, P12 p12, P13 p13);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;
        IInstanceMember*    _p9;
        IInstanceMember*    _p10;
        IInstanceMember*    _p11;
        IInstanceMember*    _p12;
        IInstanceMember*    _p13;

    public:
        CAgentStaticMethod_13(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0), _p9(0), _p10(0), _p11(0), _p12(0), _p13(0) {
            _fp = f;
        }

        CAgentStaticMethod_13(CAgentStaticMethod_13<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13>& rhs)
            : CAgentMethodBase<T>(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
            _p9 = rhs._p9;
            _p10 = rhs._p10;
            _p11 = rhs._p11;
            _p12 = rhs._p12;
            _p13 = rhs._p13;
        }
        virtual ~CAgentStaticMethod_13() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
            BEHAVIAC_DELETE this->_p9;
            BEHAVIAC_DELETE this->_p10;
            BEHAVIAC_DELETE this->_p11;
            BEHAVIAC_DELETE this->_p12;
            BEHAVIAC_DELETE this->_p13;
        }
        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethod_13<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 13);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
            _p9 = AgentMeta::TParseProperty<StoredType9>(paramStrs[8].c_str());
            _p10 = AgentMeta::TParseProperty<StoredType10>(paramStrs[9].c_str());
            _p11 = AgentMeta::TParseProperty<StoredType11>(paramStrs[10].c_str());
            _p12 = AgentMeta::TParseProperty<StoredType12>(paramStrs[11].c_str());
            _p13 = AgentMeta::TParseProperty<StoredType13>(paramStrs[12].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);
            BEHAVIAC_ASSERT(_p9 != NULL);
            BEHAVIAC_ASSERT(_p10 != NULL);
            BEHAVIAC_ASSERT(_p11 != NULL);
            BEHAVIAC_ASSERT(_p12 != NULL);
            BEHAVIAC_ASSERT(_p13 != NULL);

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());
            StoredType12* v12 = (StoredType12*)_p12->GetValue(self, behaviac::Meta::IsVector<StoredType12>::Result, behaviac::GetClassTypeNumberId<StoredType12>());
            StoredType13* v13 = (StoredType13*)_p13->GetValue(self, behaviac::Meta::IsVector<StoredType13>::Result, behaviac::GetClassTypeNumberId<StoredType13>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);
            typename ParamHandler<StoredType12>::Type pv12 = ParamHandler<StoredType12>::GetValue(v12);
            typename ParamHandler<StoredType13>::Type pv13 = ParamHandler<StoredType13>::GetValue(v13);

            this->_returnValue->value = _fp(
                                            (PARAM_CALLEDTYPE(P1))pv1,
                                            (PARAM_CALLEDTYPE(P2))pv2,
                                            (PARAM_CALLEDTYPE(P3))pv3,
                                            (PARAM_CALLEDTYPE(P4))pv4,
                                            (PARAM_CALLEDTYPE(P5))pv5,
                                            (PARAM_CALLEDTYPE(P6))pv6,
                                            (PARAM_CALLEDTYPE(P7))pv7,
                                            (PARAM_CALLEDTYPE(P8))pv8,
                                            (PARAM_CALLEDTYPE(P9))pv9,
                                            (PARAM_CALLEDTYPE(P10))pv10,
                                            (PARAM_CALLEDTYPE(P11))pv11,
                                            (PARAM_CALLEDTYPE(P12))pv12,
                                            (PARAM_CALLEDTYPE(P13))pv13);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());
            StoredType12* v12 = (StoredType12*)_p12->GetValue(self, behaviac::Meta::IsVector<StoredType12>::Result, behaviac::GetClassTypeNumberId<StoredType12>());
            StoredType13* v13 = (StoredType13*)_p13->GetValue(self, behaviac::Meta::IsVector<StoredType13>::Result, behaviac::GetClassTypeNumberId<StoredType13>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);
            typename ParamHandler<StoredType12>::Type pv12 = ParamHandler<StoredType12>::GetValue(v12);
            typename ParamHandler<StoredType13>::Type pv13 = ParamHandler<StoredType13>::GetValue(v13);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 8);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P9))pv9);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 9);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P10))pv10);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 10);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P11))pv11);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 11);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P12))pv12);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 12);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P13))pv13);
        }
    };

    template<typename T, typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8, typename P9, typename P10, typename P11, typename P12, typename P13, typename P14>
    class CAgentStaticMethod_14 : public CAgentMethodBase<T> {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;
        typedef typename StoredTypeSelector<P9>::StoredType				StoredType9;
        typedef typename StoredTypeSelector<P10>::StoredType			StoredType10;
        typedef typename StoredTypeSelector<P11>::StoredType			StoredType11;
        typedef typename StoredTypeSelector<P12>::StoredType			StoredType12;
        typedef typename StoredTypeSelector<P13>::StoredType			StoredType13;
        typedef typename StoredTypeSelector<P14>::StoredType			StoredType14;

        typedef T(*FunctionPointer)(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, P12 p12, P13 p13, P14 p14);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;
        IInstanceMember*    _p9;
        IInstanceMember*    _p10;
        IInstanceMember*    _p11;
        IInstanceMember*    _p12;
        IInstanceMember*    _p13;
        IInstanceMember*    _p14;

    public:
        CAgentStaticMethod_14(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0), _p9(0), _p10(0), _p11(0), _p12(0), _p13(0), _p14(0) {
            _fp = f;
        }

        CAgentStaticMethod_14(CAgentStaticMethod_14<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14>& rhs)
            : CAgentMethodBase<T>(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
            _p9 = rhs._p9;
            _p10 = rhs._p10;
            _p11 = rhs._p11;
            _p12 = rhs._p12;
            _p13 = rhs._p13;
            _p14 = rhs._p14;
        }
        virtual ~CAgentStaticMethod_14() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
            BEHAVIAC_DELETE this->_p9;
            BEHAVIAC_DELETE this->_p10;
            BEHAVIAC_DELETE this->_p11;
            BEHAVIAC_DELETE this->_p12;
            BEHAVIAC_DELETE this->_p13;
            BEHAVIAC_DELETE this->_p14;
        }
        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethod_14<T, P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 14);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
            _p9 = AgentMeta::TParseProperty<StoredType9>(paramStrs[8].c_str());
            _p10 = AgentMeta::TParseProperty<StoredType10>(paramStrs[9].c_str());
            _p11 = AgentMeta::TParseProperty<StoredType11>(paramStrs[10].c_str());
            _p12 = AgentMeta::TParseProperty<StoredType12>(paramStrs[11].c_str());
            _p13 = AgentMeta::TParseProperty<StoredType13>(paramStrs[12].c_str());
            _p14 = AgentMeta::TParseProperty<StoredType14>(paramStrs[13].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);
            BEHAVIAC_ASSERT(_p9 != NULL);
            BEHAVIAC_ASSERT(_p10 != NULL);
            BEHAVIAC_ASSERT(_p11 != NULL);
            BEHAVIAC_ASSERT(_p12 != NULL);
            BEHAVIAC_ASSERT(_p13 != NULL);
            BEHAVIAC_ASSERT(_p14 != NULL);

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());
            StoredType12* v12 = (StoredType12*)_p12->GetValue(self, behaviac::Meta::IsVector<StoredType12>::Result, behaviac::GetClassTypeNumberId<StoredType12>());
            StoredType13* v13 = (StoredType13*)_p13->GetValue(self, behaviac::Meta::IsVector<StoredType13>::Result, behaviac::GetClassTypeNumberId<StoredType13>());
            StoredType14* v14 = (StoredType14*)_p14->GetValue(self, behaviac::Meta::IsVector<StoredType14>::Result, behaviac::GetClassTypeNumberId<StoredType14>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);
            typename ParamHandler<StoredType12>::Type pv12 = ParamHandler<StoredType12>::GetValue(v12);
            typename ParamHandler<StoredType13>::Type pv13 = ParamHandler<StoredType13>::GetValue(v13);
            typename ParamHandler<StoredType14>::Type pv14 = ParamHandler<StoredType14>::GetValue(v14);

            this->_returnValue->value = _fp(
                                            (PARAM_CALLEDTYPE(P1))pv1,
                                            (PARAM_CALLEDTYPE(P2))pv2,
                                            (PARAM_CALLEDTYPE(P3))pv3,
                                            (PARAM_CALLEDTYPE(P4))pv4,
                                            (PARAM_CALLEDTYPE(P5))pv5,
                                            (PARAM_CALLEDTYPE(P6))pv6,
                                            (PARAM_CALLEDTYPE(P7))pv7,
                                            (PARAM_CALLEDTYPE(P8))pv8,
                                            (PARAM_CALLEDTYPE(P9))pv9,
                                            (PARAM_CALLEDTYPE(P10))pv10,
                                            (PARAM_CALLEDTYPE(P11))pv11,
                                            (PARAM_CALLEDTYPE(P12))pv12,
                                            (PARAM_CALLEDTYPE(P13))pv13,
                                            (PARAM_CALLEDTYPE(P14))pv14);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());
            StoredType12* v12 = (StoredType12*)_p12->GetValue(self, behaviac::Meta::IsVector<StoredType12>::Result, behaviac::GetClassTypeNumberId<StoredType12>());
            StoredType13* v13 = (StoredType13*)_p13->GetValue(self, behaviac::Meta::IsVector<StoredType13>::Result, behaviac::GetClassTypeNumberId<StoredType13>());
            StoredType14* v14 = (StoredType14*)_p14->GetValue(self, behaviac::Meta::IsVector<StoredType14>::Result, behaviac::GetClassTypeNumberId<StoredType14>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);
            typename ParamHandler<StoredType12>::Type pv12 = ParamHandler<StoredType12>::GetValue(v12);
            typename ParamHandler<StoredType13>::Type pv13 = ParamHandler<StoredType13>::GetValue(v13);
            typename ParamHandler<StoredType14>::Type pv14 = ParamHandler<StoredType14>::GetValue(v14);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 8);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P9))pv9);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 9);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P10))pv10);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 10);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P11))pv11);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 11);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P12))pv12);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 12);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P13))pv13);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 13);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P14))pv14);
        }
    };

    class CAgentMethodVoidBase : public IInstanceMember {
    protected:
        char _instance[kInstanceNameMax];

    public:
        CAgentMethodVoidBase() {
            memset(_instance, 0, kInstanceNameMax);
        }

        CAgentMethodVoidBase(CAgentMethodVoidBase& rhs) {
            //_instance = rhs._instance;
            memset(_instance, 0, kInstanceNameMax);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, rhs._instance);
        }

        virtual IInstanceMember* clone() {
            BEHAVIAC_ASSERT(false);
            return NULL;
        }
        virtual bool IsMethod() const {
            return true;
        }
        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
			BEHAVIAC_UNUSED_VAR(paramStrs);
			BEHAVIAC_ASSERT(false);

            //_instance = instance;
            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
        }

        virtual int GetCount(behaviac::Agent* self) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_ASSERT(false);
            return 0;
        }

        virtual void SetValueElement(behaviac::Agent* self, IInstanceMember* right, int index) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(right);
			BEHAVIAC_UNUSED_VAR(index);
			BEHAVIAC_ASSERT(false);
        }

        virtual void run(behaviac::Agent* self) {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_ASSERT(false);
        }

        virtual IValue* GetIValue(behaviac::Agent* self) {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_ASSERT(false);
            return NULL;
        }

        virtual const void* GetValueObject(const behaviac::Agent* self) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_ASSERT(false);
            return NULL;
        }

        virtual IValue* GetIValueFrom(behaviac::Agent* self, IInstanceMember* firstParam) {
			BEHAVIAC_UNUSED_VAR(firstParam);
			return GetIValue(self);
        }

        virtual void SetValue(behaviac::Agent* self, const void* pValue) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(pValue);
            BEHAVIAC_ASSERT(false);
        }

        virtual void SetValueCast(behaviac::Agent* self, IInstanceMember* right, bool bCast) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(right);
			BEHAVIAC_UNUSED_VAR(bCast);
			BEHAVIAC_ASSERT(false);
        }

        virtual bool Compare(behaviac::Agent* self, IInstanceMember* right, EOperatorType comparisonType) const {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(right);
			BEHAVIAC_UNUSED_VAR(comparisonType);
			BEHAVIAC_ASSERT(false);
            return false;
        }

        virtual void Compute(behaviac::Agent* self, IInstanceMember* right1, IInstanceMember* right2, EOperatorType computeType) {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(right1);
			BEHAVIAC_UNUSED_VAR(right2);
			BEHAVIAC_UNUSED_VAR(computeType);
			BEHAVIAC_ASSERT(false);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(treeTask);
			BEHAVIAC_ASSERT(false);
        }
    };

    class CAgentMethodVoid : public CAgentMethodVoidBase {
        typedef void(*FunctionPointer)(behaviac::Agent* a);

        FunctionPointer     _fp;

    public:
        CAgentMethodVoid(FunctionPointer f) {
            _fp = f;
        }

        CAgentMethodVoid(CAgentMethodVoid& rhs) : CAgentMethodVoidBase(rhs) {

            _fp = rhs._fp;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethodVoid(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
			BEHAVIAC_UNUSED_VAR(paramStrs);
			BEHAVIAC_ASSERT(paramStrs.size() == 0);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);

        }

        virtual void run(behaviac::Agent* self) {
			BEHAVIAC_UNUSED_VAR(self);
			behaviac::Agent* agent = Agent::GetParentAgent(self, this->_instance);

            _fp(agent);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(treeTask);
		}
    };

    template<typename P1>
    class CAgentMethodVoid_1 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;

        typedef void(*FunctionPointer)(behaviac::Agent* a, P1 p1);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;

    public:
        CAgentMethodVoid_1(FunctionPointer f) : _p1(0) {
            _fp = f;
        }

        CAgentMethodVoid_1(CAgentMethodVoid_1<P1>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
        }

        virtual ~CAgentMethodVoid_1() {
            BEHAVIAC_DELETE this->_p1;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethodVoid_1<P1>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 1);

            //_instance = instance;
            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            Agent* agent = Agent::GetParentAgent(self, this->_instance);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());

            typedef typename ParamHandler<StoredType1>::Type ReturnType;
            ReturnType pv1 = ParamHandler<StoredType1>::GetValue(v1);// all parameter value need ParamHandler process, otherwise char* will be treat as char** (char** is wrong type)

            //_fp(agent, pC);
            _fp(agent, (PARAM_CALLEDTYPE(P1))pv1);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1*  v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);
            //treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))ParamHandler<StoredType1>::GetValue(*v1));
        }
    };

    template<typename P1, typename P2>
    class CAgentMethodVoid_2 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;

        typedef void(*FunctionPointer)(behaviac::Agent* a, P1 p1, P2 p2);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;

    public:
        CAgentMethodVoid_2(FunctionPointer f) : _p1(0), _p2(0) {
            _fp = f;
        }

        CAgentMethodVoid_2(CAgentMethodVoid_2<P1, P2>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
        }

        virtual ~CAgentMethodVoid_2() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethodVoid_2<P1, P2>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 2);

            //_instance = instance;
            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);

            Agent* agent = Agent::GetParentAgent(self, this->_instance);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);

            _fp(agent, (PARAM_CALLEDTYPE(P1))pv1,
                (PARAM_CALLEDTYPE(P2))pv2);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);
        }
    };

    template<typename P1, typename P2, typename P3>
    class CAgentMethodVoid_3 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;

        typedef void(*FunctionPointer)(behaviac::Agent* a, P1 p1, P2 p2, P3 p3);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;

    public:
        CAgentMethodVoid_3(FunctionPointer f) : _p1(0), _p2(0), _p3(0) {
            _fp = f;
        }

        CAgentMethodVoid_3(CAgentMethodVoid_3<P1, P2, P3>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
        }

        virtual ~CAgentMethodVoid_3() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethodVoid_3<P1, P2, P3>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 3);

            //_instance = instance;
            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);

            Agent* agent = Agent::GetParentAgent(self, this->_instance);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);

            _fp(agent,
                (PARAM_CALLEDTYPE(P1))pv1,
                (PARAM_CALLEDTYPE(P2))pv2,
                (PARAM_CALLEDTYPE(P3))pv3);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);
        }
    };

    template<typename P1, typename P2, typename P3, typename P4>
    class CAgentMethodVoid_4 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;

        typedef void(*FunctionPointer)(behaviac::Agent* a, P1 p1, P2 p2, P3 p3, P4 p4);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;

    public:
        CAgentMethodVoid_4(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0) {
            _fp = f;
        }

        CAgentMethodVoid_4(CAgentMethodVoid_4<P1, P2, P3, P4>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
        }

        virtual ~CAgentMethodVoid_4() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethodVoid_4<P1, P2, P3, P4>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 4);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            Agent* agent = Agent::GetParentAgent(self, this->_instance);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);

            _fp(agent,
                (PARAM_CALLEDTYPE(P1))pv1,
                (PARAM_CALLEDTYPE(P2))pv2,
                (PARAM_CALLEDTYPE(P3))pv3,
                (PARAM_CALLEDTYPE(P4))pv4);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);
        }
    };

    template<typename P1, typename P2, typename P3, typename P4, typename P5>
    class CAgentMethodVoid_5 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;

        typedef void(*FunctionPointer)(behaviac::Agent* a, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;

    public:
        CAgentMethodVoid_5(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0) {
            _fp = f;
        }

        CAgentMethodVoid_5(CAgentMethodVoid_5<P1, P2, P3, P4, P5>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
        }

        virtual ~CAgentMethodVoid_5() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethodVoid_5<P1, P2, P3, P4, P5>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 5);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);

            Agent* agent = Agent::GetParentAgent(self, this->_instance);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);

            _fp(agent,
                (PARAM_CALLEDTYPE(P1))pv1,
                (PARAM_CALLEDTYPE(P2))pv2,
                (PARAM_CALLEDTYPE(P3))pv3,
                (PARAM_CALLEDTYPE(P4))pv4,
                (PARAM_CALLEDTYPE(P5))pv5);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);
        }
    };

    template<typename P1, typename P2, typename P3, typename P4, typename P5, typename P6>
    class CAgentMethodVoid_6 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;

        typedef void(*FunctionPointer)(behaviac::Agent* a, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;

    public:
        CAgentMethodVoid_6(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0) {
            _fp = f;
        }

        CAgentMethodVoid_6(CAgentMethodVoid_6<P1, P2, P3, P4, P5, P6>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
        }

        virtual ~CAgentMethodVoid_6() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethodVoid_6<P1, P2, P3, P4, P5, P6>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 6);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);

            Agent* agent = Agent::GetParentAgent(self, this->_instance);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);

            _fp(agent,
                (PARAM_CALLEDTYPE(P1))pv1,
                (PARAM_CALLEDTYPE(P2))pv2,
                (PARAM_CALLEDTYPE(P3))pv3,
                (PARAM_CALLEDTYPE(P4))pv4,
                (PARAM_CALLEDTYPE(P5))pv5,
                (PARAM_CALLEDTYPE(P6))pv6);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);
        }
    };

    template<typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7>
    class CAgentMethodVoid_7 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;

        typedef void(*FunctionPointer)(behaviac::Agent* a, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;

    public:
        CAgentMethodVoid_7(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0) {
            _fp = f;
        }

        CAgentMethodVoid_7(CAgentMethodVoid_7<P1, P2, P3, P4, P5, P6, P7>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
        }

        virtual ~CAgentMethodVoid_7() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethodVoid_7<P1, P2, P3, P4, P5, P6, P7>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 7);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);

            Agent* agent = Agent::GetParentAgent(self, this->_instance);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);

            _fp(agent,
                (PARAM_CALLEDTYPE(P1))pv1,
                (PARAM_CALLEDTYPE(P2))pv2,
                (PARAM_CALLEDTYPE(P3))pv3,
                (PARAM_CALLEDTYPE(P4))pv4,
                (PARAM_CALLEDTYPE(P5))pv5,
                (PARAM_CALLEDTYPE(P6))pv6,
                (PARAM_CALLEDTYPE(P7))pv7);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);
        }
    };

    template<typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8>
    class CAgentMethodVoid_8 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;

        typedef void(*FunctionPointer)(behaviac::Agent* a, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;

    public:
        CAgentMethodVoid_8(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0) {
            _fp = f;
        }
        virtual ~CAgentMethodVoid_8() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
        }

    public:
        CAgentMethodVoid_8(CAgentMethodVoid_8<P1, P2, P3, P4, P5, P6, P7, P8>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethodVoid_8<P1, P2, P3, P4, P5, P6, P7, P8>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 8);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);

            Agent* agent = Agent::GetParentAgent(self, this->_instance);

            _fp(agent,
                (PARAM_CALLEDTYPE(P1))pv1,
                (PARAM_CALLEDTYPE(P2))pv2,
                (PARAM_CALLEDTYPE(P3))pv3,
                (PARAM_CALLEDTYPE(P4))pv4,
                (PARAM_CALLEDTYPE(P5))pv5,
                (PARAM_CALLEDTYPE(P6))pv6,
                (PARAM_CALLEDTYPE(P7))pv7,
                (PARAM_CALLEDTYPE(P8))pv8);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);
        }
    };

    template<typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8, typename P9>
    class CAgentMethodVoid_9 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;
        typedef typename StoredTypeSelector<P9>::StoredType				StoredType9;

        typedef void(*FunctionPointer)(behaviac::Agent* a, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;
        IInstanceMember*    _p9;

    public:
        CAgentMethodVoid_9(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0), _p9(0) {
            _fp = f;
        }
        virtual ~CAgentMethodVoid_9() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
            BEHAVIAC_DELETE this->_p9;
        }

    public:
        CAgentMethodVoid_9(CAgentMethodVoid_9<P1, P2, P3, P4, P5, P6, P7, P8, P9>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
            _p9 = rhs._p9;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethodVoid_9<P1, P2, P3, P4, P5, P6, P7, P8, P9>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 9);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
            _p9 = AgentMeta::TParseProperty<StoredType9>(paramStrs[8].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);
            BEHAVIAC_ASSERT(_p9 != NULL);

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);

            Agent* agent = Agent::GetParentAgent(self, this->_instance);

            _fp(agent,
                (PARAM_CALLEDTYPE(P1))pv1,
                (PARAM_CALLEDTYPE(P2))pv2,
                (PARAM_CALLEDTYPE(P3))pv3,
                (PARAM_CALLEDTYPE(P4))pv4,
                (PARAM_CALLEDTYPE(P5))pv5,
                (PARAM_CALLEDTYPE(P6))pv6,
                (PARAM_CALLEDTYPE(P7))pv7,
                (PARAM_CALLEDTYPE(P8))pv8,
                (PARAM_CALLEDTYPE(P9))pv9);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 8);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P9))pv9);
        }
    };

    template<typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8, typename P9, typename P10>
    class CAgentMethodVoid_10 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;
        typedef typename StoredTypeSelector<P9>::StoredType				StoredType9;
        typedef typename StoredTypeSelector<P10>::StoredType			StoredType10;

        typedef void(*FunctionPointer)(behaviac::Agent* a, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;
        IInstanceMember*    _p9;
        IInstanceMember*    _p10;

    public:
        CAgentMethodVoid_10(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0), _p9(0), _p10(0) {
            _fp = f;
        }
        virtual ~CAgentMethodVoid_10() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
            BEHAVIAC_DELETE this->_p9;
            BEHAVIAC_DELETE this->_p10;
        }

    public:
        CAgentMethodVoid_10(CAgentMethodVoid_10<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
            _p9 = rhs._p9;
            _p10 = rhs._p10;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethodVoid_10<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 10);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
            _p9 = AgentMeta::TParseProperty<StoredType9>(paramStrs[8].c_str());
            _p10 = AgentMeta::TParseProperty<StoredType10>(paramStrs[9].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);
            BEHAVIAC_ASSERT(_p9 != NULL);
            BEHAVIAC_ASSERT(_p10 != NULL);

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);

            Agent* agent = Agent::GetParentAgent(self, this->_instance);

            _fp(agent,
                (PARAM_CALLEDTYPE(P1))pv1,
                (PARAM_CALLEDTYPE(P2))pv2,
                (PARAM_CALLEDTYPE(P3))pv3,
                (PARAM_CALLEDTYPE(P4))pv4,
                (PARAM_CALLEDTYPE(P5))pv5,
                (PARAM_CALLEDTYPE(P6))pv6,
                (PARAM_CALLEDTYPE(P7))pv7,
                (PARAM_CALLEDTYPE(P8))pv8,
                (PARAM_CALLEDTYPE(P9))pv9,
                (PARAM_CALLEDTYPE(P10))pv10);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 8);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P9))pv9);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 9);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P10))pv10);
        }
    };

    template<typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8, typename P9, typename P10, typename P11>
    class CAgentMethodVoid_11 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;
        typedef typename StoredTypeSelector<P9>::StoredType				StoredType9;
        typedef typename StoredTypeSelector<P10>::StoredType			StoredType10;
        typedef typename StoredTypeSelector<P11>::StoredType			StoredType11;

        typedef void(*FunctionPointer)(behaviac::Agent* a, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;
        IInstanceMember*    _p9;
        IInstanceMember*    _p10;
        IInstanceMember*    _p11;

    public:
        CAgentMethodVoid_11(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0), _p9(0), _p10(0), _p11(0) {
            _fp = f;
        }
        virtual ~CAgentMethodVoid_11() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
            BEHAVIAC_DELETE this->_p9;
            BEHAVIAC_DELETE this->_p10;
            BEHAVIAC_DELETE this->_p11;
        }

    public:
        CAgentMethodVoid_11(CAgentMethodVoid_11<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
            _p9 = rhs._p9;
            _p10 = rhs._p10;
            _p11 = rhs._p11;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethodVoid_11<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 11);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
            _p9 = AgentMeta::TParseProperty<StoredType9>(paramStrs[8].c_str());
            _p10 = AgentMeta::TParseProperty<StoredType10>(paramStrs[9].c_str());
            _p11 = AgentMeta::TParseProperty<StoredType11>(paramStrs[10].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);
            BEHAVIAC_ASSERT(_p9 != NULL);
            BEHAVIAC_ASSERT(_p10 != NULL);
            BEHAVIAC_ASSERT(_p11 != NULL);

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);

            Agent* agent = Agent::GetParentAgent(self, this->_instance);

            _fp(agent,
                (PARAM_CALLEDTYPE(P1))pv1,
                (PARAM_CALLEDTYPE(P2))pv2,
                (PARAM_CALLEDTYPE(P3))pv3,
                (PARAM_CALLEDTYPE(P4))pv4,
                (PARAM_CALLEDTYPE(P5))pv5,
                (PARAM_CALLEDTYPE(P6))pv6,
                (PARAM_CALLEDTYPE(P7))pv7,
                (PARAM_CALLEDTYPE(P8))pv8,
                (PARAM_CALLEDTYPE(P9))pv9,
                (PARAM_CALLEDTYPE(P10))pv10,
                (PARAM_CALLEDTYPE(P11))pv11);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 8);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P9))pv9);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 9);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P10))pv10);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 10);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P11))pv11);
        }
    };

    template<typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8, typename P9, typename P10, typename P11, typename P12>
    class CAgentMethodVoid_12 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;
        typedef typename StoredTypeSelector<P9>::StoredType				StoredType9;
        typedef typename StoredTypeSelector<P10>::StoredType			StoredType10;
        typedef typename StoredTypeSelector<P11>::StoredType			StoredType11;
        typedef typename StoredTypeSelector<P12>::StoredType			StoredType12;

        typedef void(*FunctionPointer)(behaviac::Agent* a, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, P12 p12);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;
        IInstanceMember*    _p9;
        IInstanceMember*    _p10;
        IInstanceMember*    _p11;
        IInstanceMember*    _p12;

    public:
        CAgentMethodVoid_12(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0), _p9(0), _p10(0), _p11(0), _p12(0) {
            _fp = f;
        }
        virtual ~CAgentMethodVoid_12() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
            BEHAVIAC_DELETE this->_p9;
            BEHAVIAC_DELETE this->_p10;
            BEHAVIAC_DELETE this->_p11;
            BEHAVIAC_DELETE this->_p12;
        }

    public:
        CAgentMethodVoid_12(CAgentMethodVoid_12<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
            _p9 = rhs._p9;
            _p10 = rhs._p10;
            _p11 = rhs._p11;
            _p12 = rhs._p12;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethodVoid_12<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 12);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
            _p9 = AgentMeta::TParseProperty<StoredType9>(paramStrs[8].c_str());
            _p10 = AgentMeta::TParseProperty<StoredType10>(paramStrs[9].c_str());
            _p11 = AgentMeta::TParseProperty<StoredType11>(paramStrs[10].c_str());
            _p12 = AgentMeta::TParseProperty<StoredType12>(paramStrs[11].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);
            BEHAVIAC_ASSERT(_p9 != NULL);
            BEHAVIAC_ASSERT(_p10 != NULL);
            BEHAVIAC_ASSERT(_p11 != NULL);
            BEHAVIAC_ASSERT(_p12 != NULL);

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());
            StoredType12* v12 = (StoredType12*)_p12->GetValue(self, behaviac::Meta::IsVector<StoredType12>::Result, behaviac::GetClassTypeNumberId<StoredType12>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);
            typename ParamHandler<StoredType12>::Type pv12 = ParamHandler<StoredType12>::GetValue(v12);

            Agent* agent = Agent::GetParentAgent(self, this->_instance);

            _fp(agent,
                (PARAM_CALLEDTYPE(P1))pv1,
                (PARAM_CALLEDTYPE(P2))pv2,
                (PARAM_CALLEDTYPE(P3))pv3,
                (PARAM_CALLEDTYPE(P4))pv4,
                (PARAM_CALLEDTYPE(P5))pv5,
                (PARAM_CALLEDTYPE(P6))pv6,
                (PARAM_CALLEDTYPE(P7))pv7,
                (PARAM_CALLEDTYPE(P8))pv8,
                (PARAM_CALLEDTYPE(P9))pv9,
                (PARAM_CALLEDTYPE(P10))pv10,
                (PARAM_CALLEDTYPE(P11))pv11,
                (PARAM_CALLEDTYPE(P12))pv12);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());
            StoredType12* v12 = (StoredType12*)_p12->GetValue(self, behaviac::Meta::IsVector<StoredType12>::Result, behaviac::GetClassTypeNumberId<StoredType12>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);
            typename ParamHandler<StoredType12>::Type pv12 = ParamHandler<StoredType12>::GetValue(v12);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 8);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P9))pv9);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 9);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P10))pv10);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 10);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P11))pv11);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 11);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P12))pv12);
        }
    };

    template<typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8, typename P9, typename P10, typename P11, typename P12, typename P13>
    class CAgentMethodVoid_13 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;
        typedef typename StoredTypeSelector<P9>::StoredType				StoredType9;
        typedef typename StoredTypeSelector<P10>::StoredType			StoredType10;
        typedef typename StoredTypeSelector<P11>::StoredType			StoredType11;
        typedef typename StoredTypeSelector<P12>::StoredType			StoredType12;
        typedef typename StoredTypeSelector<P13>::StoredType			StoredType13;

        typedef void(*FunctionPointer)(behaviac::Agent* a, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, P12 p12, P13 p13);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;
        IInstanceMember*    _p9;
        IInstanceMember*    _p10;
        IInstanceMember*    _p11;
        IInstanceMember*    _p12;
        IInstanceMember*    _p13;

    public:
        CAgentMethodVoid_13(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0), _p9(0), _p10(0), _p11(0), _p12(0), _p13(0) {
            _fp = f;
        }
        virtual ~CAgentMethodVoid_13() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
            BEHAVIAC_DELETE this->_p9;
            BEHAVIAC_DELETE this->_p10;
            BEHAVIAC_DELETE this->_p11;
            BEHAVIAC_DELETE this->_p12;
            BEHAVIAC_DELETE this->_p13;
        }

    public:
        CAgentMethodVoid_13(CAgentMethodVoid_13<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
            _p9 = rhs._p9;
            _p10 = rhs._p10;
            _p11 = rhs._p11;
            _p12 = rhs._p12;
            _p13 = rhs._p13;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethodVoid_13<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 13);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
            _p9 = AgentMeta::TParseProperty<StoredType9>(paramStrs[8].c_str());
            _p10 = AgentMeta::TParseProperty<StoredType10>(paramStrs[9].c_str());
            _p11 = AgentMeta::TParseProperty<StoredType11>(paramStrs[10].c_str());
            _p12 = AgentMeta::TParseProperty<StoredType12>(paramStrs[11].c_str());
            _p13 = AgentMeta::TParseProperty<StoredType13>(paramStrs[12].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);
            BEHAVIAC_ASSERT(_p9 != NULL);
            BEHAVIAC_ASSERT(_p10 != NULL);
            BEHAVIAC_ASSERT(_p11 != NULL);
            BEHAVIAC_ASSERT(_p12 != NULL);
            BEHAVIAC_ASSERT(_p13 != NULL);

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());
            StoredType12* v12 = (StoredType12*)_p12->GetValue(self, behaviac::Meta::IsVector<StoredType12>::Result, behaviac::GetClassTypeNumberId<StoredType12>());
            StoredType13* v13 = (StoredType13*)_p13->GetValue(self, behaviac::Meta::IsVector<StoredType13>::Result, behaviac::GetClassTypeNumberId<StoredType13>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);
            typename ParamHandler<StoredType12>::Type pv12 = ParamHandler<StoredType12>::GetValue(v12);
            typename ParamHandler<StoredType13>::Type pv13 = ParamHandler<StoredType13>::GetValue(v13);

            Agent* agent = Agent::GetParentAgent(self, this->_instance);

            _fp(agent,
                (PARAM_CALLEDTYPE(P1))pv1,
                (PARAM_CALLEDTYPE(P2))pv2,
                (PARAM_CALLEDTYPE(P3))pv3,
                (PARAM_CALLEDTYPE(P4))pv4,
                (PARAM_CALLEDTYPE(P5))pv5,
                (PARAM_CALLEDTYPE(P6))pv6,
                (PARAM_CALLEDTYPE(P7))pv7,
                (PARAM_CALLEDTYPE(P8))pv8,
                (PARAM_CALLEDTYPE(P9))pv9,
                (PARAM_CALLEDTYPE(P10))pv10,
                (PARAM_CALLEDTYPE(P11))pv11,
                (PARAM_CALLEDTYPE(P12))pv12,
                (PARAM_CALLEDTYPE(P13))pv13);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());
            StoredType12* v12 = (StoredType12*)_p12->GetValue(self, behaviac::Meta::IsVector<StoredType12>::Result, behaviac::GetClassTypeNumberId<StoredType12>());
            StoredType13* v13 = (StoredType13*)_p13->GetValue(self, behaviac::Meta::IsVector<StoredType13>::Result, behaviac::GetClassTypeNumberId<StoredType13>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);
            typename ParamHandler<StoredType12>::Type pv12 = ParamHandler<StoredType12>::GetValue(v12);
            typename ParamHandler<StoredType13>::Type pv13 = ParamHandler<StoredType13>::GetValue(v13);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 8);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P9))pv9);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 9);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P10))pv10);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 10);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P11))pv11);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 11);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P12))pv12);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 12);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P13))pv13);
        }
    };

    template<typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8, typename P9, typename P10, typename P11, typename P12, typename P13, typename P14>
    class CAgentMethodVoid_14 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;
        typedef typename StoredTypeSelector<P9>::StoredType				StoredType9;
        typedef typename StoredTypeSelector<P10>::StoredType			StoredType10;
        typedef typename StoredTypeSelector<P11>::StoredType			StoredType11;
        typedef typename StoredTypeSelector<P12>::StoredType			StoredType12;
        typedef typename StoredTypeSelector<P13>::StoredType			StoredType13;
        typedef typename StoredTypeSelector<P14>::StoredType			StoredType14;

        typedef void(*FunctionPointer)(behaviac::Agent* a, P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, P12 p12, P13 p13, P14 p14);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;
        IInstanceMember*    _p9;
        IInstanceMember*    _p10;
        IInstanceMember*    _p11;
        IInstanceMember*    _p12;
        IInstanceMember*    _p13;
        IInstanceMember*    _p14;

    public:
        CAgentMethodVoid_14(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0), _p9(0), _p10(0), _p11(0), _p12(0), _p13(0), _p14(0) {
            _fp = f;
        }
        virtual ~CAgentMethodVoid_14() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
            BEHAVIAC_DELETE this->_p9;
            BEHAVIAC_DELETE this->_p10;
            BEHAVIAC_DELETE this->_p11;
            BEHAVIAC_DELETE this->_p12;
            BEHAVIAC_DELETE this->_p13;
            BEHAVIAC_DELETE this->_p14;
        }

    public:
        CAgentMethodVoid_14(CAgentMethodVoid_14<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
            _p9 = rhs._p9;
            _p10 = rhs._p10;
            _p11 = rhs._p11;
            _p12 = rhs._p12;
            _p13 = rhs._p13;
            _p14 = rhs._p14;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentMethodVoid_14<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 14);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
            _p9 = AgentMeta::TParseProperty<StoredType9>(paramStrs[8].c_str());
            _p10 = AgentMeta::TParseProperty<StoredType10>(paramStrs[9].c_str());
            _p11 = AgentMeta::TParseProperty<StoredType11>(paramStrs[10].c_str());
            _p12 = AgentMeta::TParseProperty<StoredType12>(paramStrs[11].c_str());
            _p13 = AgentMeta::TParseProperty<StoredType13>(paramStrs[12].c_str());
            _p14 = AgentMeta::TParseProperty<StoredType14>(paramStrs[13].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);
            BEHAVIAC_ASSERT(_p9 != NULL);
            BEHAVIAC_ASSERT(_p10 != NULL);
            BEHAVIAC_ASSERT(_p11 != NULL);
            BEHAVIAC_ASSERT(_p12 != NULL);
            BEHAVIAC_ASSERT(_p13 != NULL);
            BEHAVIAC_ASSERT(_p14 != NULL);

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());
            StoredType12* v12 = (StoredType12*)_p12->GetValue(self, behaviac::Meta::IsVector<StoredType12>::Result, behaviac::GetClassTypeNumberId<StoredType12>());
            StoredType13* v13 = (StoredType13*)_p13->GetValue(self, behaviac::Meta::IsVector<StoredType13>::Result, behaviac::GetClassTypeNumberId<StoredType13>());
            StoredType14* v14 = (StoredType14*)_p14->GetValue(self, behaviac::Meta::IsVector<StoredType14>::Result, behaviac::GetClassTypeNumberId<StoredType14>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);
            typename ParamHandler<StoredType12>::Type pv12 = ParamHandler<StoredType12>::GetValue(v12);
            typename ParamHandler<StoredType13>::Type pv13 = ParamHandler<StoredType13>::GetValue(v13);
            typename ParamHandler<StoredType14>::Type pv14 = ParamHandler<StoredType14>::GetValue(v14);

            Agent* agent = Agent::GetParentAgent(self, this->_instance);

            _fp(agent,
                (PARAM_CALLEDTYPE(P1))pv1,
                (PARAM_CALLEDTYPE(P2))pv2,
                (PARAM_CALLEDTYPE(P3))pv3,
                (PARAM_CALLEDTYPE(P4))pv4,
                (PARAM_CALLEDTYPE(P5))pv5,
                (PARAM_CALLEDTYPE(P6))pv6,
                (PARAM_CALLEDTYPE(P7))pv7,
                (PARAM_CALLEDTYPE(P8))pv8,
                (PARAM_CALLEDTYPE(P9))pv9,
                (PARAM_CALLEDTYPE(P10))pv10,
                (PARAM_CALLEDTYPE(P11))pv11,
                (PARAM_CALLEDTYPE(P12))pv12,
                (PARAM_CALLEDTYPE(P13))pv13,
                (PARAM_CALLEDTYPE(P14))pv14);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());
            StoredType12* v12 = (StoredType12*)_p12->GetValue(self, behaviac::Meta::IsVector<StoredType12>::Result, behaviac::GetClassTypeNumberId<StoredType12>());
            StoredType13* v13 = (StoredType13*)_p13->GetValue(self, behaviac::Meta::IsVector<StoredType13>::Result, behaviac::GetClassTypeNumberId<StoredType13>());
            StoredType14* v14 = (StoredType14*)_p14->GetValue(self, behaviac::Meta::IsVector<StoredType14>::Result, behaviac::GetClassTypeNumberId<StoredType14>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);
            typename ParamHandler<StoredType12>::Type pv12 = ParamHandler<StoredType12>::GetValue(v12);
            typename ParamHandler<StoredType13>::Type pv13 = ParamHandler<StoredType13>::GetValue(v13);
            typename ParamHandler<StoredType14>::Type pv14 = ParamHandler<StoredType14>::GetValue(v14);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 8);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P9))pv9);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 9);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P10))pv10);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 10);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P11))pv11);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 11);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P12))pv12);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 12);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P13))pv13);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 13);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P14))pv14);
        }
    };

    class CAgentStaticMethodVoid : public CAgentMethodVoidBase {

        typedef void(*FunctionPointer)();

        FunctionPointer     _fp;

    public:
        CAgentStaticMethodVoid(FunctionPointer f) {
            _fp = f;
        }

        CAgentStaticMethodVoid(CAgentStaticMethodVoid& rhs) : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethodVoid(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
			BEHAVIAC_UNUSED_VAR(paramStrs);
			BEHAVIAC_ASSERT(paramStrs.size() == 0);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
        }

        virtual void run(behaviac::Agent* self) {
			BEHAVIAC_UNUSED_VAR(self);
			_fp();
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
			BEHAVIAC_UNUSED_VAR(self);
			BEHAVIAC_UNUSED_VAR(treeTask);
		}
    };

    template<typename P1>
    class CAgentStaticMethodVoid_1 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;

        typedef void(*FunctionPointer)(P1 p1);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;

    public:
        CAgentStaticMethodVoid_1(FunctionPointer f) : _p1(0) {
            _fp = f;
        }

        CAgentStaticMethodVoid_1(CAgentStaticMethodVoid_1<P1>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
        }

        virtual ~CAgentStaticMethodVoid_1() {
            BEHAVIAC_DELETE this->_p1;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethodVoid_1<P1>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 1);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);

            _fp((PARAM_CALLEDTYPE(P1))pv1);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);
        }
    };

    template<typename P1, typename P2>
    class CAgentStaticMethodVoid_2 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;

        typedef void(*FunctionPointer)(P1& p1, P2& p2);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
    public:
        CAgentStaticMethodVoid_2(FunctionPointer f) : _fp(f), _p1(0), _p2(0) {
        }

        CAgentStaticMethodVoid_2(CAgentStaticMethodVoid_2<P1, P2>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
        }

        virtual ~CAgentStaticMethodVoid_2() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethodVoid_2<P1, P2>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 2);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);

            _fp(
                (PARAM_CALLEDTYPE(P1))pv1,
                (PARAM_CALLEDTYPE(P2))pv2);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);
        }
    };

    template<typename P1, typename P2, typename P3>
    class CAgentStaticMethodVoid_3 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;

        typedef void(*FunctionPointer)(P1 p1, P2 p2, P3 p3);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;

    public:
        CAgentStaticMethodVoid_3(FunctionPointer f) : _p1(0), _p2(0), _p3(0) {
            _fp = f;
        }

        CAgentStaticMethodVoid_3(CAgentStaticMethodVoid_3<P1, P2, P3>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
        }

        virtual ~CAgentStaticMethodVoid_3() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethodVoid_3<P1, P2, P3>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 3);

            //_instance = instance;
            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);

            _fp((PARAM_CALLEDTYPE(P1))pv1,
                (PARAM_CALLEDTYPE(P2))pv2,
                (PARAM_CALLEDTYPE(P3))pv3);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);
        }
    };

    template<typename P1, typename P2, typename P3, typename P4>
    class CAgentStaticMethodVoid_4 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;

        typedef void(*FunctionPointer)(P1 p1, P2 p2, P3 p3, P4 p4);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;

    public:
        CAgentStaticMethodVoid_4(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0) {
            _fp = f;
        }

        CAgentStaticMethodVoid_4(CAgentStaticMethodVoid_4<P1, P2, P3, P4>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
        }

        virtual ~CAgentStaticMethodVoid_4() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethodVoid_4<P1, P2, P3, P4>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 4);

            //_instance = instance;
            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);

            _fp((PARAM_CALLEDTYPE(P1))pv1,
                (PARAM_CALLEDTYPE(P2))pv2,
                (PARAM_CALLEDTYPE(P3))pv3,
                (PARAM_CALLEDTYPE(P4))pv4);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);
        }
    };

    template<typename P1, typename P2, typename P3, typename P4, typename P5>
    class CAgentStaticMethodVoid_5 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;

        typedef void(*FunctionPointer)(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;

    public:
        CAgentStaticMethodVoid_5(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0) {
            _fp = f;
        }

        CAgentStaticMethodVoid_5(CAgentStaticMethodVoid_5<P1, P2, P3, P4, P5>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
        }

        virtual ~CAgentStaticMethodVoid_5() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethodVoid_5<P1, P2, P3, P4, P5>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 5);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);

            _fp((PARAM_CALLEDTYPE(P1))pv1,
                (PARAM_CALLEDTYPE(P2))pv2,
                (PARAM_CALLEDTYPE(P3))pv3,
                (PARAM_CALLEDTYPE(P4))pv4,
                (PARAM_CALLEDTYPE(P5))pv5);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);
        }
    };

    template<typename P1, typename P2, typename P3, typename P4, typename P5, typename P6>
    class CAgentStaticMethodVoid_6 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;

        typedef void(*FunctionPointer)(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;

    public:
        CAgentStaticMethodVoid_6(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0) {
            _fp = f;
        }

        CAgentStaticMethodVoid_6(CAgentStaticMethodVoid_6<P1, P2, P3, P4, P5, P6>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
        }

        virtual ~CAgentStaticMethodVoid_6() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethodVoid_6<P1, P2, P3, P4, P5, P6>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 6);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);

            _fp((PARAM_CALLEDTYPE(P1))pv1,
                (PARAM_CALLEDTYPE(P2))pv2,
                (PARAM_CALLEDTYPE(P3))pv3,
                (PARAM_CALLEDTYPE(P4))pv4,
                (PARAM_CALLEDTYPE(P5))pv5,
                (PARAM_CALLEDTYPE(P6))pv6);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);
        }
    };

    template<typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7>
    class CAgentStaticMethodVoid_7 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;

        typedef void(*FunctionPointer)(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;

    public:
        CAgentStaticMethodVoid_7(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0) {
            _fp = f;
        }

        CAgentStaticMethodVoid_7(CAgentStaticMethodVoid_7<P1, P2, P3, P4, P5, P6, P7>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
        }
        virtual ~CAgentStaticMethodVoid_7() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
        }
        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethodVoid_7<P1, P2, P3, P4, P5, P6, P7>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 7);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);

            _fp((PARAM_CALLEDTYPE(P1))pv1,
                (PARAM_CALLEDTYPE(P2))pv2,
                (PARAM_CALLEDTYPE(P3))pv3,
                (PARAM_CALLEDTYPE(P4))pv4,
                (PARAM_CALLEDTYPE(P5))pv5,
                (PARAM_CALLEDTYPE(P6))pv6,
                (PARAM_CALLEDTYPE(P7))pv7);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);
        }
    };

    template<typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8>
    class CAgentStaticMethodVoid_8 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;

        typedef void(*FunctionPointer)(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;

    public:
        CAgentStaticMethodVoid_8(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0) {
            _fp = f;
        }

        CAgentStaticMethodVoid_8(CAgentStaticMethodVoid_8<P1, P2, P3, P4, P5, P6, P7, P8>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
        }

        virtual ~CAgentStaticMethodVoid_8() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethodVoid_8<P1, P2, P3, P4, P5, P6, P7, P8>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 8);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);

            _fp((PARAM_CALLEDTYPE(P1))pv1,
                (PARAM_CALLEDTYPE(P2))pv2,
                (PARAM_CALLEDTYPE(P3))pv3,
                (PARAM_CALLEDTYPE(P4))pv4,
                (PARAM_CALLEDTYPE(P5))pv5,
                (PARAM_CALLEDTYPE(P6))pv6,
                (PARAM_CALLEDTYPE(P7))pv7,
                (PARAM_CALLEDTYPE(P8))pv8);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);
        }
    };

    template<typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8, typename P9>
    class CAgentStaticMethodVoid_9 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;
        typedef typename StoredTypeSelector<P9>::StoredType				StoredType9;

        typedef void(*FunctionPointer)(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;
        IInstanceMember*    _p9;

    public:
        CAgentStaticMethodVoid_9(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0), _p9(0) {
            _fp = f;
        }

        CAgentStaticMethodVoid_9(CAgentStaticMethodVoid_9<P1, P2, P3, P4, P5, P6, P7, P8, P9>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
            _p9 = rhs._p9;
        }

        virtual ~CAgentStaticMethodVoid_9() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
            BEHAVIAC_DELETE this->_p9;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethodVoid_9<P1, P2, P3, P4, P5, P6, P7, P8, P9>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 9);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
            _p9 = AgentMeta::TParseProperty<StoredType9>(paramStrs[8].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);
            BEHAVIAC_ASSERT(_p9 != NULL);

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);

            _fp((PARAM_CALLEDTYPE(P1))pv1,
                (PARAM_CALLEDTYPE(P2))pv2,
                (PARAM_CALLEDTYPE(P3))pv3,
                (PARAM_CALLEDTYPE(P4))pv4,
                (PARAM_CALLEDTYPE(P5))pv5,
                (PARAM_CALLEDTYPE(P6))pv6,
                (PARAM_CALLEDTYPE(P7))pv7,
                (PARAM_CALLEDTYPE(P8))pv8,
                (PARAM_CALLEDTYPE(P9))pv9);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 8);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P9))pv9);
        }
    };

    template<typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8, typename P9, typename P10>
    class CAgentStaticMethodVoid_10 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;
        typedef typename StoredTypeSelector<P9>::StoredType				StoredType9;
        typedef typename StoredTypeSelector<P10>::StoredType			StoredType10;

        typedef void(*FunctionPointer)(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;
        IInstanceMember*    _p9;
        IInstanceMember*    _p10;

    public:
        CAgentStaticMethodVoid_10(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0), _p9(0), _p10(0) {
            _fp = f;
        }

        CAgentStaticMethodVoid_10(CAgentStaticMethodVoid_10<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
            _p9 = rhs._p9;
            _p10 = rhs._p10;
        }

        virtual ~CAgentStaticMethodVoid_10() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
            BEHAVIAC_DELETE this->_p9;
            BEHAVIAC_DELETE this->_p10;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethodVoid_10<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 10);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
            _p9 = AgentMeta::TParseProperty<StoredType9>(paramStrs[8].c_str());
            _p10 = AgentMeta::TParseProperty<StoredType10>(paramStrs[9].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);
            BEHAVIAC_ASSERT(_p9 != NULL);
            BEHAVIAC_ASSERT(_p10 != NULL);

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);

            _fp((PARAM_CALLEDTYPE(P1))pv1,
                (PARAM_CALLEDTYPE(P2))pv2,
                (PARAM_CALLEDTYPE(P3))pv3,
                (PARAM_CALLEDTYPE(P4))pv4,
                (PARAM_CALLEDTYPE(P5))pv5,
                (PARAM_CALLEDTYPE(P6))pv6,
                (PARAM_CALLEDTYPE(P7))pv7,
                (PARAM_CALLEDTYPE(P8))pv8,
                (PARAM_CALLEDTYPE(P9))pv9,
                (PARAM_CALLEDTYPE(P10))pv10);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 8);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P9))pv9);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 9);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P10))pv10);
        }
    };

    template<typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8, typename P9, typename P10, typename P11>
    class CAgentStaticMethodVoid_11 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;
        typedef typename StoredTypeSelector<P9>::StoredType				StoredType9;
        typedef typename StoredTypeSelector<P10>::StoredType			StoredType10;
        typedef typename StoredTypeSelector<P11>::StoredType			StoredType11;

        typedef void(*FunctionPointer)(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;
        IInstanceMember*    _p9;
        IInstanceMember*    _p10;
        IInstanceMember*    _p11;

    public:
        CAgentStaticMethodVoid_11(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0), _p9(0), _p10(0), _p11(0) {
            _fp = f;
        }

        CAgentStaticMethodVoid_11(CAgentStaticMethodVoid_11<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
            _p9 = rhs._p9;
            _p10 = rhs._p10;
            _p11 = rhs._p11;
        }

        virtual ~CAgentStaticMethodVoid_11() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
            BEHAVIAC_DELETE this->_p9;
            BEHAVIAC_DELETE this->_p10;
            BEHAVIAC_DELETE this->_p11;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethodVoid_11<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 11);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
            _p9 = AgentMeta::TParseProperty<StoredType9>(paramStrs[8].c_str());
            _p10 = AgentMeta::TParseProperty<StoredType10>(paramStrs[9].c_str());
            _p11 = AgentMeta::TParseProperty<StoredType11>(paramStrs[10].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);
            BEHAVIAC_ASSERT(_p9 != NULL);
            BEHAVIAC_ASSERT(_p10 != NULL);
            BEHAVIAC_ASSERT(_p11 != NULL);

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);

            _fp((PARAM_CALLEDTYPE(P1))pv1,
                (PARAM_CALLEDTYPE(P2))pv2,
                (PARAM_CALLEDTYPE(P3))pv3,
                (PARAM_CALLEDTYPE(P4))pv4,
                (PARAM_CALLEDTYPE(P5))pv5,
                (PARAM_CALLEDTYPE(P6))pv6,
                (PARAM_CALLEDTYPE(P7))pv7,
                (PARAM_CALLEDTYPE(P8))pv8,
                (PARAM_CALLEDTYPE(P9))pv9,
                (PARAM_CALLEDTYPE(P10))pv10,
                (PARAM_CALLEDTYPE(P11))pv11);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 8);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P9))pv9);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 9);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P10))pv10);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 10);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P11))pv11);
        }
    };

    template<typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8, typename P9, typename P10, typename P11, typename P12>
    class CAgentStaticMethodVoid_12 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;
        typedef typename StoredTypeSelector<P9>::StoredType				StoredType9;
        typedef typename StoredTypeSelector<P10>::StoredType			StoredType10;
        typedef typename StoredTypeSelector<P11>::StoredType			StoredType11;
        typedef typename StoredTypeSelector<P12>::StoredType			StoredType12;

        typedef void(*FunctionPointer)(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, P12 p12);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;
        IInstanceMember*    _p9;
        IInstanceMember*    _p10;
        IInstanceMember*    _p11;
        IInstanceMember*    _p12;

    public:
        CAgentStaticMethodVoid_12(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0), _p9(0), _p10(0), _p11(0), _p12(0) {
            _fp = f;
        }

        CAgentStaticMethodVoid_12(CAgentStaticMethodVoid_12<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
            _p9 = rhs._p9;
            _p10 = rhs._p10;
            _p11 = rhs._p11;
            _p12 = rhs._p12;
        }

        virtual ~CAgentStaticMethodVoid_12() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
            BEHAVIAC_DELETE this->_p9;
            BEHAVIAC_DELETE this->_p10;
            BEHAVIAC_DELETE this->_p11;
            BEHAVIAC_DELETE this->_p12;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethodVoid_12<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 12);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
            _p9 = AgentMeta::TParseProperty<StoredType9>(paramStrs[8].c_str());
            _p10 = AgentMeta::TParseProperty<StoredType10>(paramStrs[9].c_str());
            _p11 = AgentMeta::TParseProperty<StoredType11>(paramStrs[10].c_str());
            _p12 = AgentMeta::TParseProperty<StoredType12>(paramStrs[11].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);
            BEHAVIAC_ASSERT(_p9 != NULL);
            BEHAVIAC_ASSERT(_p10 != NULL);
            BEHAVIAC_ASSERT(_p11 != NULL);
            BEHAVIAC_ASSERT(_p12 != NULL);

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());
            StoredType12* v12 = (StoredType12*)_p12->GetValue(self, behaviac::Meta::IsVector<StoredType12>::Result, behaviac::GetClassTypeNumberId<StoredType12>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);
            typename ParamHandler<StoredType12>::Type pv12 = ParamHandler<StoredType12>::GetValue(v12);

            _fp((PARAM_CALLEDTYPE(P1))pv1,
                (PARAM_CALLEDTYPE(P2))pv2,
                (PARAM_CALLEDTYPE(P3))pv3,
                (PARAM_CALLEDTYPE(P4))pv4,
                (PARAM_CALLEDTYPE(P5))pv5,
                (PARAM_CALLEDTYPE(P6))pv6,
                (PARAM_CALLEDTYPE(P7))pv7,
                (PARAM_CALLEDTYPE(P8))pv8,
                (PARAM_CALLEDTYPE(P9))pv9,
                (PARAM_CALLEDTYPE(P10))pv10,
                (PARAM_CALLEDTYPE(P11))pv11,
                (PARAM_CALLEDTYPE(P12))pv12);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());
            StoredType12* v12 = (StoredType12*)_p12->GetValue(self, behaviac::Meta::IsVector<StoredType12>::Result, behaviac::GetClassTypeNumberId<StoredType12>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);
            typename ParamHandler<StoredType12>::Type pv12 = ParamHandler<StoredType12>::GetValue(v12);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 8);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P9))pv9);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 9);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P10))pv10);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 10);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P11))pv11);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 11);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P12))pv12);
        }
    };

    template<typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8, typename P9, typename P10, typename P11, typename P12, typename P13>
    class CAgentStaticMethodVoid_13 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;
        typedef typename StoredTypeSelector<P9>::StoredType				StoredType9;
        typedef typename StoredTypeSelector<P10>::StoredType			StoredType10;
        typedef typename StoredTypeSelector<P11>::StoredType			StoredType11;
        typedef typename StoredTypeSelector<P12>::StoredType			StoredType12;
        typedef typename StoredTypeSelector<P13>::StoredType			StoredType13;

        typedef void(*FunctionPointer)(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, P12 p12, P13 p13);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;
        IInstanceMember*    _p9;
        IInstanceMember*    _p10;
        IInstanceMember*    _p11;
        IInstanceMember*    _p12;
        IInstanceMember*    _p13;

    public:
        CAgentStaticMethodVoid_13(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0), _p9(0), _p10(0), _p11(0), _p12(0), _p13(0) {
            _fp = f;
        }

        CAgentStaticMethodVoid_13(CAgentStaticMethodVoid_13<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
            _p9 = rhs._p9;
            _p10 = rhs._p10;
            _p11 = rhs._p11;
            _p12 = rhs._p12;
            _p13 = rhs._p13;
        }

        virtual ~CAgentStaticMethodVoid_13() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
            BEHAVIAC_DELETE this->_p9;
            BEHAVIAC_DELETE this->_p10;
            BEHAVIAC_DELETE this->_p11;
            BEHAVIAC_DELETE this->_p12;
            BEHAVIAC_DELETE this->_p13;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethodVoid_13<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 13);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
            _p9 = AgentMeta::TParseProperty<StoredType9>(paramStrs[8].c_str());
            _p10 = AgentMeta::TParseProperty<StoredType10>(paramStrs[9].c_str());
            _p11 = AgentMeta::TParseProperty<StoredType11>(paramStrs[10].c_str());
            _p12 = AgentMeta::TParseProperty<StoredType12>(paramStrs[11].c_str());
            _p13 = AgentMeta::TParseProperty<StoredType13>(paramStrs[12].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);
            BEHAVIAC_ASSERT(_p9 != NULL);
            BEHAVIAC_ASSERT(_p10 != NULL);
            BEHAVIAC_ASSERT(_p11 != NULL);
            BEHAVIAC_ASSERT(_p12 != NULL);
            BEHAVIAC_ASSERT(_p13 != NULL);

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());
            StoredType12* v12 = (StoredType12*)_p12->GetValue(self, behaviac::Meta::IsVector<StoredType12>::Result, behaviac::GetClassTypeNumberId<StoredType12>());
            StoredType13* v13 = (StoredType13*)_p13->GetValue(self, behaviac::Meta::IsVector<StoredType13>::Result, behaviac::GetClassTypeNumberId<StoredType13>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);
            typename ParamHandler<StoredType12>::Type pv12 = ParamHandler<StoredType12>::GetValue(v12);
            typename ParamHandler<StoredType13>::Type pv13 = ParamHandler<StoredType13>::GetValue(v13);

            _fp((PARAM_CALLEDTYPE(P1))pv1,
                (PARAM_CALLEDTYPE(P2))pv2,
                (PARAM_CALLEDTYPE(P3))pv3,
                (PARAM_CALLEDTYPE(P4))pv4,
                (PARAM_CALLEDTYPE(P5))pv5,
                (PARAM_CALLEDTYPE(P6))pv6,
                (PARAM_CALLEDTYPE(P7))pv7,
                (PARAM_CALLEDTYPE(P8))pv8,
                (PARAM_CALLEDTYPE(P9))pv9,
                (PARAM_CALLEDTYPE(P10))pv10,
                (PARAM_CALLEDTYPE(P11))pv11,
                (PARAM_CALLEDTYPE(P12))pv12,
                (PARAM_CALLEDTYPE(P13))pv13);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());
            StoredType12* v12 = (StoredType12*)_p12->GetValue(self, behaviac::Meta::IsVector<StoredType12>::Result, behaviac::GetClassTypeNumberId<StoredType12>());
            StoredType13* v13 = (StoredType13*)_p13->GetValue(self, behaviac::Meta::IsVector<StoredType13>::Result, behaviac::GetClassTypeNumberId<StoredType13>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);
            typename ParamHandler<StoredType12>::Type pv12 = ParamHandler<StoredType12>::GetValue(v12);
            typename ParamHandler<StoredType13>::Type pv13 = ParamHandler<StoredType13>::GetValue(v13);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 8);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P9))pv9);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 9);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P10))pv10);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 10);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P11))pv11);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 11);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P12))pv12);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 12);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P13))pv13);
        }
    };

    template<typename P1, typename P2, typename P3, typename P4, typename P5, typename P6, typename P7, typename P8, typename P9, typename P10, typename P11, typename P12, typename P13, typename P14>
    class CAgentStaticMethodVoid_14 : public CAgentMethodVoidBase {
        typedef typename StoredTypeSelector<P1>::StoredType				StoredType1;
        typedef typename StoredTypeSelector<P2>::StoredType				StoredType2;
        typedef typename StoredTypeSelector<P3>::StoredType				StoredType3;
        typedef typename StoredTypeSelector<P4>::StoredType				StoredType4;
        typedef typename StoredTypeSelector<P5>::StoredType				StoredType5;
        typedef typename StoredTypeSelector<P6>::StoredType				StoredType6;
        typedef typename StoredTypeSelector<P7>::StoredType				StoredType7;
        typedef typename StoredTypeSelector<P8>::StoredType				StoredType8;
        typedef typename StoredTypeSelector<P9>::StoredType				StoredType9;
        typedef typename StoredTypeSelector<P10>::StoredType			StoredType10;
        typedef typename StoredTypeSelector<P11>::StoredType			StoredType11;
        typedef typename StoredTypeSelector<P12>::StoredType			StoredType12;
        typedef typename StoredTypeSelector<P13>::StoredType			StoredType13;
        typedef typename StoredTypeSelector<P14>::StoredType			StoredType14;

        typedef void(*FunctionPointer)(P1 p1, P2 p2, P3 p3, P4 p4, P5 p5, P6 p6, P7 p7, P8 p8, P9 p9, P10 p10, P11 p11, P12 p12, P13 p13, P14 p14);

        FunctionPointer     _fp;
        IInstanceMember*    _p1;
        IInstanceMember*    _p2;
        IInstanceMember*    _p3;
        IInstanceMember*    _p4;
        IInstanceMember*    _p5;
        IInstanceMember*    _p6;
        IInstanceMember*    _p7;
        IInstanceMember*    _p8;
        IInstanceMember*    _p9;
        IInstanceMember*    _p10;
        IInstanceMember*    _p11;
        IInstanceMember*    _p12;
        IInstanceMember*    _p13;
        IInstanceMember*    _p14;

    public:
        CAgentStaticMethodVoid_14(FunctionPointer f) : _p1(0), _p2(0), _p3(0), _p4(0), _p5(0), _p6(0), _p7(0), _p8(0), _p9(0), _p10(0), _p11(0), _p12(0), _p13(0), _p14(0) {
            _fp = f;
        }

        CAgentStaticMethodVoid_14(CAgentStaticMethodVoid_14<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14>& rhs)
            : CAgentMethodVoidBase(rhs) {
            _fp = rhs._fp;
            _p1 = rhs._p1;
            _p2 = rhs._p2;
            _p3 = rhs._p3;
            _p4 = rhs._p4;
            _p5 = rhs._p5;
            _p6 = rhs._p6;
            _p7 = rhs._p7;
            _p8 = rhs._p8;
            _p9 = rhs._p9;
            _p10 = rhs._p10;
            _p11 = rhs._p11;
            _p12 = rhs._p12;
            _p13 = rhs._p13;
            _p14 = rhs._p14;
        }

        virtual ~CAgentStaticMethodVoid_14() {
            BEHAVIAC_DELETE this->_p1;
            BEHAVIAC_DELETE this->_p2;
            BEHAVIAC_DELETE this->_p3;
            BEHAVIAC_DELETE this->_p4;
            BEHAVIAC_DELETE this->_p5;
            BEHAVIAC_DELETE this->_p6;
            BEHAVIAC_DELETE this->_p7;
            BEHAVIAC_DELETE this->_p8;
            BEHAVIAC_DELETE this->_p9;
            BEHAVIAC_DELETE this->_p10;
            BEHAVIAC_DELETE this->_p11;
            BEHAVIAC_DELETE this->_p12;
            BEHAVIAC_DELETE this->_p13;
            BEHAVIAC_DELETE this->_p14;
        }

        virtual IInstanceMember* clone() {
            return BEHAVIAC_NEW CAgentStaticMethodVoid_14<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14>(*this);
        }

        virtual void load(const char* instance, behaviac::vector<behaviac::string>& paramStrs) {
            BEHAVIAC_ASSERT(paramStrs.size() == 14);

            StringUtils::StringCopySafe(this->kInstanceNameMax, this->_instance, instance);
            //_instance = instance;
            _p1 = AgentMeta::TParseProperty<StoredType1>(paramStrs[0].c_str());
            _p2 = AgentMeta::TParseProperty<StoredType2>(paramStrs[1].c_str());
            _p3 = AgentMeta::TParseProperty<StoredType3>(paramStrs[2].c_str());
            _p4 = AgentMeta::TParseProperty<StoredType4>(paramStrs[3].c_str());
            _p5 = AgentMeta::TParseProperty<StoredType5>(paramStrs[4].c_str());
            _p6 = AgentMeta::TParseProperty<StoredType6>(paramStrs[5].c_str());
            _p7 = AgentMeta::TParseProperty<StoredType7>(paramStrs[6].c_str());
            _p8 = AgentMeta::TParseProperty<StoredType8>(paramStrs[7].c_str());
            _p9 = AgentMeta::TParseProperty<StoredType9>(paramStrs[8].c_str());
            _p10 = AgentMeta::TParseProperty<StoredType10>(paramStrs[9].c_str());
            _p11 = AgentMeta::TParseProperty<StoredType11>(paramStrs[10].c_str());
            _p12 = AgentMeta::TParseProperty<StoredType12>(paramStrs[11].c_str());
            _p13 = AgentMeta::TParseProperty<StoredType13>(paramStrs[12].c_str());
            _p14 = AgentMeta::TParseProperty<StoredType14>(paramStrs[13].c_str());
        }

        virtual void run(behaviac::Agent* self) {
            BEHAVIAC_ASSERT(_p1 != NULL);
            BEHAVIAC_ASSERT(_p2 != NULL);
            BEHAVIAC_ASSERT(_p3 != NULL);
            BEHAVIAC_ASSERT(_p4 != NULL);
            BEHAVIAC_ASSERT(_p5 != NULL);
            BEHAVIAC_ASSERT(_p6 != NULL);
            BEHAVIAC_ASSERT(_p7 != NULL);
            BEHAVIAC_ASSERT(_p8 != NULL);
            BEHAVIAC_ASSERT(_p9 != NULL);
            BEHAVIAC_ASSERT(_p10 != NULL);
            BEHAVIAC_ASSERT(_p11 != NULL);
            BEHAVIAC_ASSERT(_p12 != NULL);
            BEHAVIAC_ASSERT(_p13 != NULL);
            BEHAVIAC_ASSERT(_p14 != NULL);

            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());
            StoredType12* v12 = (StoredType12*)_p12->GetValue(self, behaviac::Meta::IsVector<StoredType12>::Result, behaviac::GetClassTypeNumberId<StoredType12>());
            StoredType13* v13 = (StoredType13*)_p13->GetValue(self, behaviac::Meta::IsVector<StoredType13>::Result, behaviac::GetClassTypeNumberId<StoredType13>());
            StoredType14* v14 = (StoredType14*)_p14->GetValue(self, behaviac::Meta::IsVector<StoredType14>::Result, behaviac::GetClassTypeNumberId<StoredType14>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);
            typename ParamHandler<StoredType12>::Type pv12 = ParamHandler<StoredType12>::GetValue(v12);
            typename ParamHandler<StoredType13>::Type pv13 = ParamHandler<StoredType13>::GetValue(v13);
            typename ParamHandler<StoredType14>::Type pv14 = ParamHandler<StoredType14>::GetValue(v14);

            _fp((PARAM_CALLEDTYPE(P1))pv1,
                (PARAM_CALLEDTYPE(P2))pv2,
                (PARAM_CALLEDTYPE(P3))pv3,
                (PARAM_CALLEDTYPE(P4))pv4,
                (PARAM_CALLEDTYPE(P5))pv5,
                (PARAM_CALLEDTYPE(P6))pv6,
                (PARAM_CALLEDTYPE(P7))pv7,
                (PARAM_CALLEDTYPE(P8))pv8,
                (PARAM_CALLEDTYPE(P9))pv9,
                (PARAM_CALLEDTYPE(P10))pv10,
                (PARAM_CALLEDTYPE(P11))pv11,
                (PARAM_CALLEDTYPE(P12))pv12,
                (PARAM_CALLEDTYPE(P13))pv13,
                (PARAM_CALLEDTYPE(P14))pv14);
        }

        virtual void SetTaskParams(behaviac::Agent* self, BehaviorTreeTask* treeTask) {
            char paramName[1024];
            StoredType1* v1 = (StoredType1*)_p1->GetValue(self, behaviac::Meta::IsVector<StoredType1>::Result, behaviac::GetClassTypeNumberId<StoredType1>());
            StoredType2* v2 = (StoredType2*)_p2->GetValue(self, behaviac::Meta::IsVector<StoredType2>::Result, behaviac::GetClassTypeNumberId<StoredType2>());
            StoredType3* v3 = (StoredType3*)_p3->GetValue(self, behaviac::Meta::IsVector<StoredType3>::Result, behaviac::GetClassTypeNumberId<StoredType3>());
            StoredType4* v4 = (StoredType4*)_p4->GetValue(self, behaviac::Meta::IsVector<StoredType4>::Result, behaviac::GetClassTypeNumberId<StoredType4>());
            StoredType5* v5 = (StoredType5*)_p5->GetValue(self, behaviac::Meta::IsVector<StoredType5>::Result, behaviac::GetClassTypeNumberId<StoredType5>());
            StoredType6* v6 = (StoredType6*)_p6->GetValue(self, behaviac::Meta::IsVector<StoredType6>::Result, behaviac::GetClassTypeNumberId<StoredType6>());
            StoredType7* v7 = (StoredType7*)_p7->GetValue(self, behaviac::Meta::IsVector<StoredType7>::Result, behaviac::GetClassTypeNumberId<StoredType7>());
            StoredType8* v8 = (StoredType8*)_p8->GetValue(self, behaviac::Meta::IsVector<StoredType8>::Result, behaviac::GetClassTypeNumberId<StoredType8>());
            StoredType9* v9 = (StoredType9*)_p9->GetValue(self, behaviac::Meta::IsVector<StoredType9>::Result, behaviac::GetClassTypeNumberId<StoredType9>());
            StoredType10* v10 = (StoredType10*)_p10->GetValue(self, behaviac::Meta::IsVector<StoredType10>::Result, behaviac::GetClassTypeNumberId<StoredType10>());
            StoredType11* v11 = (StoredType11*)_p11->GetValue(self, behaviac::Meta::IsVector<StoredType11>::Result, behaviac::GetClassTypeNumberId<StoredType11>());
            StoredType12* v12 = (StoredType12*)_p12->GetValue(self, behaviac::Meta::IsVector<StoredType12>::Result, behaviac::GetClassTypeNumberId<StoredType12>());
            StoredType13* v13 = (StoredType13*)_p13->GetValue(self, behaviac::Meta::IsVector<StoredType13>::Result, behaviac::GetClassTypeNumberId<StoredType13>());
            StoredType14* v14 = (StoredType14*)_p14->GetValue(self, behaviac::Meta::IsVector<StoredType14>::Result, behaviac::GetClassTypeNumberId<StoredType14>());

            typename ParamHandler<StoredType1>::Type pv1 = ParamHandler<StoredType1>::GetValue(v1);
            typename ParamHandler<StoredType2>::Type pv2 = ParamHandler<StoredType2>::GetValue(v2);
            typename ParamHandler<StoredType3>::Type pv3 = ParamHandler<StoredType3>::GetValue(v3);
            typename ParamHandler<StoredType4>::Type pv4 = ParamHandler<StoredType4>::GetValue(v4);
            typename ParamHandler<StoredType5>::Type pv5 = ParamHandler<StoredType5>::GetValue(v5);
            typename ParamHandler<StoredType6>::Type pv6 = ParamHandler<StoredType6>::GetValue(v6);
            typename ParamHandler<StoredType7>::Type pv7 = ParamHandler<StoredType7>::GetValue(v7);
            typename ParamHandler<StoredType8>::Type pv8 = ParamHandler<StoredType8>::GetValue(v8);
            typename ParamHandler<StoredType9>::Type pv9 = ParamHandler<StoredType9>::GetValue(v9);
            typename ParamHandler<StoredType10>::Type pv10 = ParamHandler<StoredType10>::GetValue(v10);
            typename ParamHandler<StoredType11>::Type pv11 = ParamHandler<StoredType11>::GetValue(v11);
            typename ParamHandler<StoredType12>::Type pv12 = ParamHandler<StoredType12>::GetValue(v12);
            typename ParamHandler<StoredType13>::Type pv13 = ParamHandler<StoredType13>::GetValue(v13);
            typename ParamHandler<StoredType14>::Type pv14 = ParamHandler<StoredType14>::GetValue(v14);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 0);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P1))pv1);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 1);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P2))pv2);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 2);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P3))pv3);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 3);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P4))pv4);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 4);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P5))pv5);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 5);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P6))pv6);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 6);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P7))pv7);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 7);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P8))pv8);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 8);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P9))pv9);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 9);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P10))pv10);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 10);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P11))pv11);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 11);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P12))pv12);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 12);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P13))pv13);

            string_sprintf(paramName, "%s%d", BEHAVIAC_LOCAL_TASK_PARAM_PRE, 13);
            treeTask->SetVariable(paramName, (PARAM_CALLEDTYPE(P14))pv14);
        }
    };
}
#endif // _BEHAVIAC_COMMON_MEMBER_H_
