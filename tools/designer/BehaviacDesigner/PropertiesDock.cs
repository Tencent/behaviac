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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Reflection;
using System.Runtime.InteropServices;
using Behaviac.Design.Data;
using Behaviac.Design.Properties;
using Behaviac.Design.Attributes;
using Behaviac.Design.ObjectUI;

namespace Behaviac.Design
{
    internal partial class PropertiesDock : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        private static List<PropertiesDock> __propertyGrids = new List<PropertiesDock>();

        /// <summary>
        /// The number of property grids available.
        /// </summary>
        internal static int Count
        {
            get
            {
                return __propertyGrids.Count;
            }
        }

        internal static void CloseAll()
        {
            PropertiesDock[] docks = __propertyGrids.ToArray();

            foreach (PropertiesDock dock in docks)
            {
                dock.Close();
            }
        }

        internal static bool ReadOnly
        {
            set
            {
                foreach (PropertiesDock dock in __propertyGrids)
                {
                    dock.Enabled = !value;
                }
            }
        }

        internal static PropertiesDock CreateFloatDock(Nodes.BehaviorNode rootBehavior, object node)
        {
            PropertiesDock newDock = null;

            // Ignore the first one.
            for (int i = 1; i < __propertyGrids.Count; ++i)
            {
                if (__propertyGrids[i].SelectedObject == node)
                {
                    newDock = __propertyGrids[i];
                    break;
                }
            }

            if (newDock == null)
            {
                newDock = new PropertiesDock();
                newDock.Show(MainWindow.Instance.DockPanel, WeifenLuo.WinFormsUI.Docking.DockState.Float);

            }
            else
            {
                newDock.Show();
            }

            newDock._rootBehavior = rootBehavior;
            newDock.SelectedObject = node;

            return newDock;
        }

        /// <summary>
        /// Forces all property grids to reinspect their objects.
        /// </summary>
        internal static void UpdatePropertyGrids()
        {
            foreach (PropertiesDock dock in __propertyGrids)
            {
                dock.SelectedObject = dock.SelectedObject;
            }
        }

        private Nodes.BehaviorNode _rootBehavior = null;
        private object _selectedObject = null;

        public object SelectedObject
        {
            get
            {
                return _selectedObject;
            }

            set
            {
                if (value == null && _selectedObject == value)
                {
                    return;
                }

                RecreatePropertyGrid();

                _selectedObject = value;

                string text = Resources.Properties;

                if (_selectedObject != null)
                {
                    string objCls = _selectedObject.ToString();

                    if (_selectedObject is Nodes.Node)
                    {
                        Nodes.Node node = _selectedObject as Nodes.Node;
                        objCls = node.ExportClass;

                    }
                    else if (_selectedObject is Attachments.Attachment)
                    {
                        Attachments.Attachment attach = _selectedObject as Attachments.Attachment;
                        objCls = attach.ExportClass;
                    }

                    text = string.Format(Resources.PropertiesOf, Plugin.GetResourceString(objCls));
                }

                Text = text;
                TabText = text;

                // this is a hack to work around a bug in the docking suite
                text += ' ';
                Text = text;
                TabText = text;

                propertyGrid.PropertiesVisible(false, false);

                if (_selectedObject != null)
                {
                    this.SuspendDrawing();
                    this.SuspendLayout();

                    IList<DesignerPropertyInfo> properties = DesignerProperty.GetDesignerProperties(_selectedObject.GetType(), DesignerProperty.SortByDisplayOrder);
                    this.UpdateProperties(properties);

                    propertyGrid.UpdateSizes();
                    propertyGrid.PropertiesVisible(true, true);

                    this.ResumeLayout();
                    this.ResumeDrawing();
                }
            }
        }

        private void RecreatePropertyGrid()
        {
            //propertyGrid.ClearProperties();

            propertyGrid.Dispose();
            propertyGrid = new CustomPropertyGridTest.DynamicPropertyGrid();
            this.SuspendLayout();
            this.propertyGrid.BackColor = System.Drawing.SystemColors.Control;
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(234, 414);
            this.propertyGrid.TabIndex = 0;
            this.Controls.Add(this.propertyGrid);
            this.ResumeLayout(false);
        }

        #region SuspendDrawing
        private int suspendCounter = 0;
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        private const int WM_SETREDRAW = 11;

        private void SuspendDrawing()
        {
            if (suspendCounter == 0)
            {
                SendMessage(this.Handle, WM_SETREDRAW, false, 0);
            }

            suspendCounter++;
        }

        private void ResumeDrawing()
        {
            suspendCounter--;

            if (suspendCounter == 0)
            {
                SendMessage(this.Handle, WM_SETREDRAW, true, 0);
                this.Refresh();
            }
        }
        #endregion

        internal static bool InspectObject(Nodes.BehaviorNode rootBehavior, object obj, bool byForce = true)
        {
            if (__propertyGrids.Count < 1)
            {
                return false;
            }

            if (byForce && Plugin.EditMode == EditModes.Design || __propertyGrids[0].SelectedObject != obj)
            {
                __propertyGrids[0]._rootBehavior = rootBehavior;
                __propertyGrids[0].SelectedObject = obj;
            }

            return true;
        }

        public PropertiesDock()
        {
            InitializeComponent();

            __propertyGrids.Add(this);

            propertyGrid.PropertiesVisible(false, false);

            this.Enabled = (Plugin.EditMode == EditModes.Design);

            this.Disposed += new EventHandler(PropertiesDock_Disposed);
        }

        void PropertiesDock_Disposed(object sender, EventArgs e)
        {
            this.Disposed -= PropertiesDock_Disposed;

            if (this.propertyGrid != null)
            {
                this.propertyGrid.Dispose();
                this.propertyGrid = null;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            SelectedObject = null;
            __propertyGrids.Remove(this);

            base.OnClosed(e);
        }

        class CategorySorter : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                //Node and Comment are below all other categories
                if (x == "CategoryComment")
                {
                    if (y == "NodeBasic")
                    {
                        return -1;
                    }

                    return 1;

                }
                else if (x == "NodeBasic")
                {
                    if (y == "CategoryComment")
                    {
                        return 1;
                    }

                    return 1;
                }

                if (y == "CategoryComment")
                {
                    if (x == "NodeBasic")
                    {
                        return 1;
                    }

                    return -1;

                }
                else if (y == "NodeBasic")
                {
                    if (x == "CategoryComment")
                    {
                        return -1;
                    }

                    return -1;
                }

                int ret = x.CompareTo(y);
                return ret;
            }
        }

        private ObjectUIPolicy uiPolicy = null;

        //private void UpdateProperties(IList<DesignerPropertyInfo> properties, List<MethodDef.Param> parameters, string parametersCategory)
        private void UpdateProperties(IList<DesignerPropertyInfo> properties)
        {
            DefaultObject obj = SelectedObject as DefaultObject;

            if (obj != null)
            {
                uiPolicy = obj.CreateUIPolicy();
                uiPolicy.Initialize(obj);
            }

            List<string> categories = new List<string>();

            foreach (DesignerPropertyInfo property in properties)
            {
                if (uiPolicy.ShouldAddProperty(property) &&
                    !property.Attribute.HasFlags(DesignerProperty.DesignerFlags.NoDisplay) &&
                    (property.Attribute.CategoryResourceString != "CategoryVersion" || Settings.Default.ShowVersionInfo) &&
                    !categories.Contains(property.Attribute.CategoryResourceString))
                {
                    categories.Add(property.Attribute.CategoryResourceString);
                }
            }

            categories.Sort(new CategorySorter());

            foreach (string category in categories)
            {
                propertyGrid.AddCategory(Plugin.GetResourceString(category), true);

                foreach (DesignerPropertyInfo property in properties)
                {
                    if (!property.Attribute.HasFlags(DesignerProperty.DesignerFlags.NoDisplay) &&
                        property.Attribute.CategoryResourceString == category)
                    {
                        if (uiPolicy != null && !uiPolicy.ShouldAddProperty(property))
                        {
                            continue;
                        }

                        object value_ = property.Property.GetValue(_selectedObject, null);
                        Type type = property.Attribute.GetEditorType(value_);
                        DesignerMethodEnum propertyMethod = property.Attribute as DesignerMethodEnum;

                        if (propertyMethod != null)
                        {
                            if ((propertyMethod.MethodType & MethodType.Task) == MethodType.Task)
                            {
                                type = typeof(DesignerMethodEnumEditor);
                            }
                        }

                        string displayName = property.Attribute.DisplayName;

                        if (uiPolicy != null)
                        {
                            displayName = uiPolicy.GetLabel(property);
                        }

                        Label label = propertyGrid.AddProperty(displayName, type, property.Attribute.HasFlags(DesignerProperty.DesignerFlags.ReadOnly));

                        // register description showing
                        label.MouseEnter += new EventHandler(label_MouseEnter);

                        // when we found an editor we connect it to the object
                        if (type != null)
                        {
                            DesignerPropertyEditor editor = (DesignerPropertyEditor)label.Tag;
                            editor.SetRootNode((Nodes.Node)this._rootBehavior);
                            editor.SetProperty(property, _selectedObject);
                            editor.ValueWasAssigned();
                            editor.MouseEnter += editor_MouseEnter;
                            editor.DescriptionWasChanged += editor_DescriptionWasChanged;
                            editor.ValueWasChanged += editor_ValueWasChanged;

                            if (uiPolicy != null)
                            {
                                uiPolicy.AddEditor(editor);
                            }
                        }

                        MethodDef method = null;
                        bool methodEditorEnable = true;

                        if (propertyMethod != null)
                        {
                            if (propertyMethod.MethodType != MethodType.Status)
                            {
                                method = value_ as MethodDef;
                                //methodEditorEnable = (propertyMethod.MethodType != MethodType.Event);
                            }

                        }
                        else
                        {
                            DesignerRightValueEnum propertyRV = property.Attribute as DesignerRightValueEnum;

                            if (propertyRV != null)
                            {
                                RightValueDef rv = value_ as RightValueDef;

                                if (rv != null && rv.IsMethod)
                                {
                                    method = rv.Method;
                                }
                            }
                        }

                        if (property.Attribute != null)
                        {
                            if (method != null)
                            {
                                if (property.Attribute.HasFlags(DesignerProperty.DesignerFlags.NoDisplayOnProperty))
                                {
                                    //don't dipslay on the property panel
                                }
                                else
                                {
                                    bool bReadonly = property.Attribute.HasFlags(DesignerProperty.DesignerFlags.ReadOnlyParams);

                                    createParamEditor(method, methodEditorEnable, bReadonly);
                                }

                            }
                            else
                            {
                                MethodDef.Param arrayIndexElement = null;

                                if (value_ is VariableDef)
                                {
                                    VariableDef var = value_ as VariableDef;
                                    arrayIndexElement = var.ArrayIndexElement;

                                }
                                else if (value_ is RightValueDef)
                                {
                                    RightValueDef varRV = value_ as RightValueDef;

                                    if (varRV.Var != null)
                                    {
                                        arrayIndexElement = varRV.Var.ArrayIndexElement;
                                    }
                                }

                                if (arrayIndexElement != null)
                                {
                                    createArrayIndexEditor("    ", arrayIndexElement);
                                }

                            }
                        }
                    }
                }
            }

            if (uiPolicy != null)
            {
                uiPolicy.Update(null, new DesignerPropertyInfo());
            }
        }

        void createArrayIndexEditor(string preBlank, MethodDef.Param arrayIndex)
        {
            Type editorType = typeof(DesignerParameterComboEnumEditor);
            string arugmentsName = preBlank + "Index";
            bool bReadonly = false;
            Label label = propertyGrid.AddProperty(arugmentsName, editorType, bReadonly);

            label.MouseEnter += new EventHandler(label_MouseEnter);

            DesignerPropertyEditor editor = (DesignerPropertyEditor)label.Tag;
            editor.Enabled = true;
            editor.SetParameter(arrayIndex, _selectedObject, bReadonly);
            editor.ValueWasAssigned();
            editor.MouseEnter += editor_MouseEnter;
            editor.DescriptionWasChanged += editor_DescriptionWasChanged;
            editor.ValueWasChanged += editor_ValueWasChanged;
        }

        MethodDef.Param lastListParam = null;
        void createParamEditor(MethodDef method, bool enable, bool bReadonlyParent)
        {
            List<MethodDef.Param> parameters = method.Params;

            foreach (MethodDef.Param p in parameters)
            {
                Type editorType = typeof(DesignerParameterComboEnumEditor);
                string arugmentsName = "    " + p.DisplayName;
                bool bReadonly = bReadonlyParent | p.Attribute.HasFlags(DesignerProperty.DesignerFlags.ReadOnly);
                Label label = propertyGrid.AddProperty(arugmentsName, editorType, bReadonly);

                label.MouseEnter += new EventHandler(label_MouseEnter);

                DesignerPropertyEditor editor = (DesignerPropertyEditor)label.Tag;

                if (p.Type.Name == "IList")
                {
                    lastListParam = p;
                }

                if (p.Type.Name == "System_Object" && lastListParam != null)
                {
                    p.ListParam = lastListParam;
                }

                editor.Enabled = enable;
                editor.SetParameter(p, _selectedObject, bReadonly);

                editor.ValueWasAssigned();
                editor.MouseEnter += editor_MouseEnter;
                editor.DescriptionWasChanged += editor_DescriptionWasChanged;
                editor.ValueWasChanged += editor_ValueWasChanged;
                //editor.ValueType = p.Attribute.ValueType;

                MethodDef.Param arrayIndexElement = null;

                if (p.Value is VariableDef)
                {
                    VariableDef var = p.Value as VariableDef;
                    arrayIndexElement = var.ArrayIndexElement;

                }
                else if (p.Value is RightValueDef)
                {
                    RightValueDef varRV = p.Value as RightValueDef;

                    if (varRV.Var != null)
                    {
                        arrayIndexElement = varRV.Var.ArrayIndexElement;
                    }
                }

                if (arrayIndexElement != null)
                {
                    createArrayIndexEditor("        ", arrayIndexElement);
                }
            }
        }

        void createParEditor(List<ParInfo> pars, Type editorType)
        {
            foreach (ParInfo par in pars)
            {
                if (par.Display)
                {
                    Label label = propertyGrid.AddProperty(par.BasicName, editorType, false);
                    label.MouseEnter += new EventHandler(label_MouseEnter);

                    DesignerPropertyEditor editor = (DesignerPropertyEditor)label.Tag;
                    editor.SetPar(par, SelectedObject);
                    editor.MouseEnter += editor_MouseEnter;
                    editor.ValueWasChanged += editor_ValueWasChanged;
                }
            }
        }

        void editor_ValueWasChanged(object sender, DesignerPropertyInfo property)
        {
            string text = _selectedObject == null ? Resources.Properties : string.Format(Resources.PropertiesOf, _selectedObject.ToString());
            Text = text;
            TabText = text;

            UndoManager.PreSave();

            if (_selectedObject != null)
            {
                Nodes.Node node = null;

                if (_selectedObject is Nodes.Node)
                {
                    node = (Nodes.Node)_selectedObject;
                }

                else if (_selectedObject is Attachments.Attachment)
                {
                    node = ((Attachments.Attachment)_selectedObject).Node;
                }

                if (node != null)
                {
                    if ((property.Attribute == null || !property.Attribute.HasFlags(DesignerProperty.DesignerFlags.NotPrefabRelated)) &&
                        !string.IsNullOrEmpty(node.PrefabName))
                    {
                        node.HasOwnPrefabData = true;
                    }

                    UndoManager.Save(this._rootBehavior);
                }
            }

            UndoManager.PostSave();

            // if we change a DesignerNodeProperty other properties of that object might be affected
            if (property.Attribute is DesignerNodeProperty ||
                uiPolicy != null && uiPolicy.ShouldUpdatePropertyGrids(property))
            {
                PropertiesDock.UpdatePropertyGrids();
            }

            if (BehaviorTreeViewDock.LastFocused != null && BehaviorTreeViewDock.LastFocused.BehaviorTreeView != null)
            {
                BehaviorTreeViewDock.LastFocused.BehaviorTreeView.Redraw();
            }
        }

        void label_MouseEnter(object sender, EventArgs e)
        {
            Label label = (Label)sender;
            DesignerPropertyEditor editor = (DesignerPropertyEditor)label.Tag;

            propertyGrid.ShowDescription(editor.DisplayName, editor.Description);
        }

        void editor_MouseEnter(object sender, EventArgs e)
        {
            DesignerPropertyEditor editor = (DesignerPropertyEditor)sender;

            propertyGrid.ShowDescription(editor.DisplayName, editor.Description);
        }

        void editor_DescriptionWasChanged(string displayName, string description)
        {
            propertyGrid.ShowDescription(displayName, description);
        }

        void item_Click(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;

            Type editorType = (Type)item.Tag;
            Label label = (Label)item.Parent.Tag;
            DesignerPropertyEditor editor = (DesignerPropertyEditor)label.Tag;

            Debug.Check(_selectedObject == editor.SelectedObject);

            Nodes.Node node = _selectedObject as Nodes.Node;

            if (node != null)
            {
                node.OnPropertyValueChanged(true);
            }

            Attachments.Attachment attach = _selectedObject as Attachments.Attachment;

            if (attach != null)
            {
                attach.OnPropertyValueChanged(true);
            }

            SelectedObject = _selectedObject;
        }
    }
}
