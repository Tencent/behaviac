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
using System.Collections.Generic;
using System.Text;
using Behaviac.Design.Nodes;
using Behaviac.Design.Attributes;

namespace Behaviac.Design.ObjectUI
{
    public class ObjectUIPolicy
    {
        protected object _obj = null;
        protected List<DesignerPropertyEditor> _allPropertyEditors = new List<DesignerPropertyEditor>();

        public void Initialize(object obj)
        {
            _obj = obj;
            _allPropertyEditors.Clear();
        }

        public void AddEditor(DesignerPropertyEditor editor)
        {
            editor.ValueWasChanged += editor_ValueWasChanged;

            _allPropertyEditors.Add(editor);
        }

        public virtual bool ShouldAddProperty(DesignerPropertyInfo property)
        {
            return true;
        }

        public virtual void Update(object sender, DesignerPropertyInfo property)
        {
        }

        public virtual bool ShouldUpdatePropertyGrids(DesignerPropertyInfo property)
        {
            return false;
        }

        public virtual string GetLabel(DesignerPropertyInfo property)
        {
            return property.Attribute.DisplayName;
        }

        private void editor_ValueWasChanged(object sender, DesignerPropertyInfo property)
        {
            Update(sender, property);
        }

        protected DesignerPropertyEditor GetEditor(object obj, string propertyName)
        {
            if (obj != null)
            {
                DesignerPropertyInfo propInfo = DesignerProperty.GetDesignerProperty(obj.GetType(), propertyName);

                if (propInfo.Property != null)
                {
                    foreach (DesignerPropertyEditor editor in _allPropertyEditors)
                    {
                        DesignerPropertyInfo prop = editor.GetProperty();

                        if (prop.Property == propInfo.Property)
                        {
                            return editor;
                        }
                    }
                }
            }

            return null;
        }

        protected object GetProperty(object obj, string propertyName)
        {
            if (obj != null)
            {
                DesignerPropertyInfo propInfo = DesignerProperty.GetDesignerProperty(obj.GetType(), propertyName);
                return propInfo.GetValue(obj);
            }

            return null;
        }

        protected bool SetProperty(object obj, string propertyName, object value)
        {
            if (obj != null)
            {
                DesignerPropertyInfo propInfo = DesignerProperty.GetDesignerProperty(obj.GetType(), propertyName);

                if (propInfo.Property != null)
                {
                    propInfo.Property.SetValue(obj, value, null);
                    return true;
                }
            }

            return false;
        }
    }
}
