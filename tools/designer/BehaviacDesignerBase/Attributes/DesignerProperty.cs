////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2009, Daniel Kollmann
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list of conditions
//   and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list of
//   conditions and the following disclaimer in the documentation and/or other materials provided
//   with the distribution.
//
// - Neither the name of Daniel Kollmann nor the names of its contributors may be used to endorse
//   or promote products derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY
// WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
////////////////////////////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// The above software in this distribution may have been modified by THL A29 Limited ("Tencent Modifications").
//
// All Tencent Modifications are Copyright (C) 2015-2017 THL A29 Limited.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;
using System.Reflection;
using Behaviac.Design.Properties;

namespace Behaviac.Design.Attributes
{
    public struct DesignerPropertyInfo
    {
        private PropertyInfo _property;
        public PropertyInfo Property
        {
            get
            {
                return _property;
            }
        }

        private DesignerProperty _attribute;
        public DesignerProperty Attribute
        {
            get
            {
                return _attribute;
            }
        }

        public DesignerPropertyInfo(PropertyInfo property)
        {
            _property = property;

            DesignerProperty[] attributes = (DesignerProperty[])_property.GetCustomAttributes(typeof(DesignerProperty), true);

            if (attributes.Length != 1)
            {
                throw new Exception(Resources.ExceptionMultipleDesignerAttributes);
            }

            _attribute = attributes[0];
        }

        public DesignerPropertyInfo(PropertyInfo property, DesignerProperty attribute)
        {
            _property = property;
            _attribute = attribute;
        }

        /// <summary>
        /// Returns the property's value.
        /// </summary>
        /// <param name="obj">The value we want to get the value from.</param>
        /// <returns>The value as an object.</returns>
        public object GetValue(object obj)
        {
            return (_property != null) ? _property.GetValue(obj, null) : null;
        }

        public Type GetTypeFallback()
        {
            return (_property != null) ? _property.PropertyType : null;
        }

        /// <summary>
        /// Returns the property's value as a string for displaying, skipping any encoding.
        /// </summary>
        /// <param name="obj">The value whose value we want to get.</param>
        /// <returns>The value as a string.</returns>
        public string GetDisplayValue(object obj)
        {
            return _attribute.GetDisplayValue(_property.GetValue(obj, null));
        }

        /// <summary>
        /// Returns the property's value as a string for exporting, skipping any encoding.
        /// </summary>
        /// <param name="obj">The value stored in the property unencoded.</param>
        /// <returns>The value as a string.</returns>
        public string GetExportValue(object obj)
        {
            return _attribute.GetExportValue(obj, _property.GetValue(obj, null));
        }

        /// <summary>
        /// Returns the property's value as a string for saving
        /// </summary>
        /// <param name="obj">The value stored in the property unencoded.</param>
        /// <returns>The value as a string.</returns>
        public string GetSaveValue(object obj)
        {
            return _attribute.GetSaveValue(obj, _property.GetValue(obj, null));
        }

        /// <summary>
        /// Returns the property's value as a string when generating codes.
        /// </summary>
        /// <param name="obj">The value stored in the property unencoded.</param>
        /// <returns>The value as a string.</returns>
        public string GetGeneratedValue(object obj)
        {
            return _attribute.GetGeneratedValue(obj, _property.GetValue(obj, null));
        }

        /// <summary>
        /// Sets the property's value based on a string.
        /// </summary>
        /// <param name="type">The type of the property.</param>
        /// <param name="str">The string representing the value to be set.</param>
        /// <returns>Returns the value encoded in the string in the correct type given.</returns>
        public object FromStringValue(List<Nodes.Node.ErrorCheck> result, DefaultObject node, object parentObject, Type type, string str)
        {
            return _attribute.FromStringValue(result, node, parentObject, type, str);
        }

        /// <summary>
        /// Sets the value of the property for the given node from a string.
        /// </summary>
        /// <param name="obj">The object we want to set the value on.</param>
        /// <param name="valueString">The string holding the value.</param>
        public void SetValueFromString(List<Nodes.Node.ErrorCheck> result, object obj, string valueString, DefaultObject node = null)
        {
            if (_property != null)
            {
                if (Plugin.IsCharType(_property.PropertyType))
                {
                    object v = FromStringValue(result, node, obj, _property.PropertyType, valueString);
                    string vStr = v.ToString();

                    char c = 'A';

                    if (vStr.Length >= 1)
                    {
                        c = vStr[0];
                    }

                    _property.SetValue(obj, c, null);
                }
                else
                {
                    object v = FromStringValue(result, node, obj, _property.PropertyType, valueString);

                    

                    _property.SetValue(obj, v, null);
                }
            }
        }

        /// <summary>
        /// Sets the value of the property for the given node from a string.
        /// </summary>
        /// <param name="node">The node we want to set the value on.</param>
        /// <param name="valueString">The string holding the value.</param>
        public void SetValueFromString(List<Nodes.Node.ErrorCheck> result, Nodes.Node node, string valueString)
        {
            if (_property != null)
            {
                _property.SetValue(node, FromStringValue(result, node, null, _property.PropertyType, valueString), null);
            }
        }

        /// <summary>
        /// Sets the value of the property for the given event from a string.
        /// </summary>
        /// <param name="evnt">The event we want to set the value on.</param>
        /// <param name="valueString">The string holding the value.</param>
        public void SetValueFromString(List<Nodes.Node.ErrorCheck> result, Attachments.Attachment attach, string valueString)
        {
            if (_property != null)
            {
                _property.SetValue(attach, FromStringValue(result, attach, null, _property.PropertyType, valueString), null);
            }
        }

        /// <summary>
        /// Sets the value of the property for the given comment from a string.
        /// </summary>
        /// <param name="evnt">The comment we want to set the value on.</param>
        /// <param name="valueString">The string holding the value.</param>
        public void SetValueFromString(List<Nodes.Node.ErrorCheck> result, Nodes.Node.Comment comment, string valueString)
        {
            if (_property != null)
            {
                _property.SetValue(comment, FromStringValue(result, null, null, _property.PropertyType, valueString), null);
            }
        }
    }

    public class DesignerArrayPropertyInfo
    {
        private string _name = string.Empty;
        public string Name
        {
            get
            {
                return _name;
            }
        }

        private Type _itemType = null;
        public Type ItemType
        {
            get
            {
                return _itemType;
            }
        }

        private System.Collections.IList _itemList = null;
        public System.Collections.IList ItemList
        {
            get
            {
                return _itemList;
            }
        }

        private int _itemIndex = -1;
        public int ItemIndex
        {
            get
            {
                return _itemIndex;
            }
            set
            {
                _itemIndex = value;
            }
        }

        public object Value
        {
            get
            {
                if (_itemList != null && _itemIndex > -1 && _itemIndex < _itemList.Count)
                {
                    return _itemList[_itemIndex];
                }

                return null;
            }

            set
            {
                if (_itemList != null && _itemIndex > -1 && _itemIndex < _itemList.Count)
                {
                    _itemList[_itemIndex] = value;
                }
            }
        }

        private bool _readOnly = false;
        public bool ReadOnly
        {
            get
            {
                return _readOnly;
            }
        }

        public DesignerArrayPropertyInfo(string name, Type itemType, System.Collections.IList itemList, int itemIndex, bool readOnly)
        {
            _name = name;
            _itemType = itemType;
            _itemList = itemList;
            _itemIndex = itemIndex;
            _readOnly = readOnly;
        }

        public DesignerArrayPropertyInfo(DesignerArrayPropertyInfo arrayProperty)
        {
            _name = arrayProperty.Name;
            _itemType = arrayProperty.ItemType;
            _itemList = arrayProperty.ItemList;
            _itemIndex = arrayProperty.ItemIndex;
            _readOnly = arrayProperty.ReadOnly;
        }
    }

    public class DesignerStructPropertyInfo
    {
        private string _name = string.Empty;
        public string Name
        {
            get
            {
                return _name;
            }
        }

        private Type _type = null;
        public Type Type
        {
            get
            {
                return _type;
            }
        }

        private object _owner = null;
        public object Owner
        {
            get
            {
                return _owner;
            }
        }

        private int _elmentIndexInArray = -1;

        //if -1, it is not an element of an array
        public int ElmentIndexInArray
        {
            get
            {
                return _elmentIndexInArray;
            }
        }

        public DesignerStructPropertyInfo(string name, Type type, object owner, int elmentIndexInArray = -1)
        {
            _name = name;
            _type = type;
            _owner = owner;
            _elmentIndexInArray = elmentIndexInArray;
        }
    }

    /// <summary>
    /// The base class for all designer attributes. Any designer attribute must be able to be saved and loaded.
    /// </summary>
    public abstract class DesignerProperty : Attribute
    {
        [Flags]
        public enum DesignerFlags
        {
            NoFlags = 0,
            ReadOnly = 1,
            NoDisplay = 2,
            NoSave = 4,
            NoExport = 8,
            NotPrefabRelated = 32,
            QueryRelated = 64,
            NoDisplayOnProperty = 128,
            NoReadonly = 256,
            ReadOnlyParams = 512,
        }

        /// <summary>
        /// The enumeration defines how this attribute will be visualised in the editor.
        /// </summary>
        public enum DisplayMode { NoDisplay, Parameter, List, ListTrue }

        /// <summary>
        /// This method is used to sort properties by their name.
        /// </summary>
        public static int SortByName(DesignerPropertyInfo a, DesignerPropertyInfo b)
        {
            return a.Property.Name.CompareTo(b.Property.Name);
        }

        /// <summary>
        /// This method is used to sort properties by their display order.
        /// </summary>
        public static int SortByDisplayOrder(DesignerPropertyInfo a, DesignerPropertyInfo b)
        {
            if (a.Attribute.DisplayOrder == b.Attribute.DisplayOrder)
            {
                return 0;
            }

            return a.Attribute.DisplayOrder < b.Attribute.DisplayOrder ? -1 : 1;
        }

        /// <summary>
        /// Returns a list of all properties which have a designer attribute attached.
        /// </summary>
        /// <param name="type">The type we want to get the properties from.</param>
        /// <returns>A list of all properties relevant to the designer.</returns>
        public static IList<DesignerPropertyInfo> GetDesignerProperties(Type type)
        {
            Debug.Check(type != null);
            if (type != null)
            {
                return GetDesignerProperties(type, SortByName);
            }

            return null;
        }

        /// <summary>
        /// Returns a list of all properties which have a designer attribute attached.
        /// </summary>
        /// <param name="type">The type we want to get the properties from.</param>
        /// <param name="comparison">The comparison used to sort the design properties.</param>
        /// <returns>A list of all properties relevant to the designer.</returns>
        public static IList<DesignerPropertyInfo> GetDesignerProperties(Type type, Comparison<DesignerPropertyInfo> comparison)
        {
            List<DesignerPropertyInfo> list = new List<DesignerPropertyInfo>();

            if (type != null)
            {
                PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                foreach (PropertyInfo property in properties)
                {
                    DesignerProperty[] attributes = (DesignerProperty[])property.GetCustomAttributes(typeof(DesignerProperty), true);

                    if (attributes.Length > 1)
                    {
                        throw new Exception(Resources.ExceptionMultipleDesignerAttributes);
                    }

                    if (attributes.Length == 1)
                    {
                        // all properties with a designer attribute must be able to be read and written
                        if (!property.CanRead)
                        {
                            throw new Exception(Resources.ExceptionPropertyCannotBeRead);
                        }

                        // all properties with a designer attribute must be able to be written or marked as read-only and no-save
                        if (!property.CanWrite && !attributes[0].HasFlags(DesignerFlags.ReadOnly | DesignerFlags.NoSave))
                        {
                            throw new Exception(Resources.ExceptionPropertyCannotBeWritten);

                        }
                        else
                        {
                            list.Add(new DesignerPropertyInfo(property, attributes[0]));
                        }
                    }
                }

                if (comparison != null)
                {
                    list.Sort(comparison);
                }
            }

            return list.AsReadOnly();
        }

        /// <summary>
        /// Returns a property of a given name which has a designer attribute attached.
        /// </summary>
        /// <param name="type">The type we want to get the property from.</param>
        /// <param name="name">The name of the property we want to get.</param>
        /// <returns>The property requested if it exists.</returns>
        public static DesignerPropertyInfo GetDesignerProperty(Type type, string name)
        {
            List<DesignerPropertyInfo> list = new List<DesignerPropertyInfo>();

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (PropertyInfo property in properties)
            {
                if (property.Name != name)
                {
                    continue;
                }

                DesignerProperty[] attributes = (DesignerProperty[])property.GetCustomAttributes(typeof(DesignerProperty), true);

                if (attributes.Length > 1)
                {
                    throw new Exception(Resources.ExceptionMultipleDesignerAttributes);
                }

                if (attributes.Length == 1)
                {
                    // all properties with a designer attribute must be able to be read and written
                    if (!property.CanRead)
                    {
                        throw new Exception(Resources.ExceptionPropertyCannotBeRead);
                    }

                    // all properties with a designer attribute must be able to be written or marked as read-only and no-save
                    if (!property.CanWrite && !attributes[0].HasFlags(DesignerFlags.ReadOnly | DesignerFlags.NoSave))
                    {
                        throw new Exception(Resources.ExceptionPropertyCannotBeWritten);
                    }

                    return new DesignerPropertyInfo(property, attributes[0]);
                }

                throw new Exception(string.Format(Resources.ExceptionLinkedPropertyNotFound, name, type));
            }

            throw new Exception(string.Format(Resources.ExceptionLinkedPropertyNotFound, name, type));
        }

        protected readonly string _displayName;
        protected readonly string _description;
        protected readonly string _category;
        protected readonly DisplayMode _displayMode;
        protected readonly DesignerFlags _flags;
        protected readonly Type _editorType;
        protected readonly int _displayOrder;
        protected readonly string _linkedToProperty;

        /// <summary>
        /// Gets the name shown on the node and in the property editor for the property.
        /// </summary>
        public string DisplayName
        {
            get
            {
                return Plugin.GetResourceString(_displayName);
            }
        }

        /// <summary>
        /// Gets the description shown in the property editor for the property.
        /// </summary>
        public string Description
        {
            get
            {
                return Plugin.GetResourceString(_description);
            }
        }

        /// <summary>
        /// Gets the category shown in the property editor for the property.
        /// </summary>
        public string Category
        {
            get
            {
                return Plugin.GetResourceString(_category);
            }
        }

        /// <summary>
        /// Returns the resource used for the category.
        /// </summary>
        public string CategoryResourceString
        {
            get
            {
                return _category;
            }
        }

        /// <summary>
        /// Gets how the property is visualised in the editor.
        /// </summary>
        public DisplayMode Display
        {
            get
            {
                return _displayMode;
            }
        }

        /// <summary>
        /// Gets the type of the editor used in the property grid.
        /// </summary>
        public virtual Type GetEditorType(object obj)
        {
            return _editorType;
        }

        /// <summary>
        /// Defines the order the properties will be sorted in when shown in the property grid. Lower come first.
        /// </summary>
        public int DisplayOrder
        {
            get
            {
                return _displayOrder;
            }
        }

        /// <summary>
        /// Returns if the property has given flags.
        /// </summary>
        /// <param name="flags">The flags we want to check.</param>
        /// <returns>Returns true when all given flags were found.</returns>
        public bool HasFlags(DesignerFlags flags)
        {
            return (_flags & flags) == flags;
        }

        protected Type _filterType = null;
        public Type FilterType
        {
            get
            {
                return _filterType;
            }
        }

        protected ValueTypes _valueType = ValueTypes.All;
        public ValueTypes ValueType
        {
            set
            {
                _valueType = value;
            }

            get
            {
                return _valueType;
            }
        }

        /// <summary>
        /// Returns the property's value as a string for displaying, skipping any encoding.
        /// </summary>
        /// <param name="obj">The value stored in the property unencoded.</param>
        /// <returns>The value as a string.</returns>
        public abstract string GetDisplayValue(object obj);

        /// <summary>
        /// Returns the property's value as a string for exporting, skipping any encoding.
        /// </summary>
        /// <param name="owner">The owner object storing the property.</param>
        /// <param name="obj">The value stored in the property unencoded.</param>
        /// <returns>The value as a string.</returns>
        public abstract string GetExportValue(object owner, object obj);

        /// <summary>
        /// Returns the property's value as a string for saving
        /// </summary>
        /// <param name="obj">The value stored in the property unencoded.</param>
        /// <returns>The value as a string.</returns>
        public virtual string GetSaveValue(object owner, object obj)
        {
            return GetExportValue(owner, obj);
        }

        /// <summary>
        /// Returns the property's value as a string when generating codes.
        /// </summary>
        /// <param name="owner">The owner object storing the property.</param>
        /// <param name="obj">The value stored in the property unencoded.</param>
        /// <returns>The value as a string.</returns>
        public virtual string GetGeneratedValue(object owner, object obj)
        {
            return GetExportValue(owner, obj);
        }

        /// <summary>
        /// Creates a new designer attribute.
        /// </summary>
        /// <param name="displayName">The name shown on the node and in the property editor for the property.</param>
        /// <param name="description">The description shown in the property editor for the property.</param>
        /// <param name="category">The category shown in the property editor for the property.</param>
        /// <param name="displayMode">Defines how the property is visualised in the editor.</param>
        /// <param name="displayOrder">Defines the order the properties will be sorted in when shown in the property grid. Lower come first.</param>
        /// <param name="flags">Defines the designer flags stored for the property.</param>
        /// <param name="editorType">The type of the editor used in the property grid.</param>
        /// <param name="linkedToProperty">The restrictions of this property are defined by another property.</param>
        protected DesignerProperty(string displayName, string description, string category, DisplayMode displayMode, int displayOrder, DesignerFlags flags, Type editorType, string linkedToProperty, ValueTypes filterType = ValueTypes.All)
        {
            _displayName = displayName;
            _description = description;
            _category = category;
            _displayMode = displayMode;
            _displayOrder = displayOrder;
            _flags = flags;
            _editorType = editorType;
            _linkedToProperty = linkedToProperty;
            _valueType = filterType;
            _filterType = Plugin.GetTypeFromValue(filterType);
        }

        /// <summary>
        /// Sets the property's value based on a string.
        /// </summary>
        /// <param name="type">The type of the property.</param>
        /// <param name="str">The string representing the value to be set.</param>
        /// <returns>Returns the value encoded in the string in the correct type given.</returns>
        public abstract object FromStringValue(List<Nodes.Node.ErrorCheck> result, DefaultObject node, object parentObject, Type type, string str);

        /// <summary>
        /// Returns the property this one is linked to.
        /// </summary>
        /// <param name="linkBroken">Is true if a link was found but it does not work.</param>
        /// <returns>The info of the property this is linked to.</returns>
        public DesignerPropertyInfo GetLinkedProperty(object obj, out bool linkBroken)
        {
            linkBroken = false;

            if (string.IsNullOrEmpty(_linkedToProperty))
            {
                return new DesignerPropertyInfo();
            }

            DesignerPropertyInfo dpi = DesignerProperty.GetDesignerProperty(obj.GetType(), _linkedToProperty);

            // if we are linked to a DesignerNodeProperty we get the information of its assigned property
            DesignerNodeProperty dnp = dpi.Attribute as DesignerNodeProperty;

            if (dnp == null)
            {
                return dpi;
            }

            Attachments.Attachment attach = (Attachments.Attachment)obj;

            // check if a valid property is associated
            object objvalue = dpi.Property.GetValue(obj, null);

            string value = dnp.GetDisplayValue(objvalue);

            if (string.IsNullOrEmpty(value) || value == Resources.DesignerNodePropertyNone)
            {
                linkBroken = true;

                return new DesignerPropertyInfo();
            }

            // return the property we are pointing at
            return DesignerProperty.GetDesignerProperty(attach.Node.GetType(), value);
        }
    }
}
