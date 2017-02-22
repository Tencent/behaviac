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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace behaviac
{
    public class Property
    {
        public Property(CMemberBase pMemberBase, bool bIsConst)
        {
            m_memberBase = pMemberBase;
            m_variableId = 0;
            m_bValidDefaultValue = false;
            m_bIsConst = bIsConst;
        }

        protected Property(Property copy)
        {
            m_variableName = copy.m_variableName;
            m_instanceName = copy.m_instanceName;
            m_variableId = copy.m_variableId;
            m_memberBase = copy.m_memberBase;

            m_parent = copy.m_parent;
            m_index = copy.m_index;

            m_bValidDefaultValue = copy.m_bValidDefaultValue;
            m_defaultValue = copy.m_defaultValue;
            m_bIsConst = copy.m_bIsConst;
            m_bIsStatic = copy.m_bIsStatic;
            m_bIsLocal = copy.m_bIsLocal;
            m_strNativeTypeName = copy.m_strNativeTypeName;
        }

        private IProperty m_bindingProperty;

        public IProperty BindingProperty
        {
            get
            {
                return m_bindingProperty;
            }
            set
            {
                m_bindingProperty = value;
            }
        }

        public string Name
        {
            get
            {
                return this.m_variableName;
            }
            set
            {
                this.m_variableName = value;

                this.m_variableId = Utils.MakeVariableId(this.m_variableName);
            }
        }

        public string InstanceName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.m_instanceName))
                {
                    return this.m_instanceName;
                }

                return m_memberBase != null ? m_memberBase.InstanceName : null;
            }
            set
            {
                this.m_instanceName = value;
            }
        }

        public uint VariableId
        {
            get
            {
                return m_variableId;
            }
        }

        private bool m_bIsStatic;

        public bool IsStatic
        {
            get
            {
                return this.m_bIsStatic;
            }
            set
            {
                this.m_bIsStatic = value;
            }
        }

        private bool m_bIsLocal;

        public bool IsLocal
        {
            get
            {
                return this.m_bIsLocal;
            }
            set
            {
                this.m_bIsLocal = value;
            }
        }

        public Type PropertyType
        {
            get
            {
                if (this.m_defaultValue != null)
                {
                    return this.m_defaultValue.GetType();
                }

                if (this.m_memberBase != null)
                {
                    return this.m_memberBase.MemberType;
                }

                return null;
            }
        }

        private string m_strNativeTypeName;
        public string NativeTypeName
        {
            get
            {
                return m_strNativeTypeName;
            }
            set
            {
                m_strNativeTypeName = Utils.GetNativeTypeName(value).Replace("::", ".");
            }
        }

        public float GetRange()
        {
            if (this.m_memberBase != null)
            {
                return this.m_memberBase.GetRange();
            }

            return 1.0f;
        }

#if K_TYPE_CREATOR_
        template<typename T>
        static bool Register(string typeName)
        {
            PropertyCreators()[typeName] = &Creator<T>;

            return true;
        }
        template<typename T>
        static void UnRegister(string typeName)
        {
            PropertyCreators().erase(typeName);
        }

        static void RegisterBasicTypes();
        static void UnRegisterBasicTypes();
#endif//#if K_TYPE_CREATOR_

        public virtual Property clone()
        {
            Property p = new Property(this);

            return p;
        }

        public static void Cleanup()
        {
        }

        protected string m_variableName;
        protected string m_instanceName;
        protected uint m_variableId;

        protected Property m_parent;
        protected Property m_index;

        protected readonly CMemberBase m_memberBase;

        protected object m_defaultValue;
        private bool m_bValidDefaultValue;
        protected readonly bool m_bIsConst;
    }


}//namespace behaviac
