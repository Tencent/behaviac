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
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using Behaviac.Design.Properties;

namespace Behaviac.Design
{
    /// <summary>
    /// This class is the window which appears when creating or editing a workspace.
    /// </summary>
    internal partial class EditWorkspaceDialog : Form
    {
        internal EditWorkspaceDialog()
        {
            InitializeComponent();

            if (string.IsNullOrEmpty(this.languageComboBox.Text))
            {
                this.languageComboBox.SelectedIndex = 0;
            }

            checkButtons();

            setMetaVisible(false);

            this.MaximumSize = new System.Drawing.Size(int.MaxValue, 272);
        }

        private string _filename = string.Empty;

        /// <summary>
        /// The workspace which is currently edited or created.
        /// </summary>
        private Workspace _workspace = null;

        /// <summary>
        /// The workspace created or edited by the dialogue.
        /// </summary>
        internal Workspace Workspace
        {
            get
            {
                return _workspace;
            }
        }

        private string _sourceLanguage = "";

        /// <summary>
        /// Sets the workspace you want to edit.
        /// </summary>
        /// <param name="ws">The workspace which will be edited.</param>
        internal void EditWorkspace(Workspace ws)
        {
            _workspace = ws;

            this.nameTextBox.Text = ws.Name;
            this.languageComboBox.Text = ws.Language;
            this.XMLTextBox.Text = ws.MetaFilename;
            this.workspaceTextBox.Text = Path.GetDirectoryName(ws.FileName);
            this.sourceTextBox.Text = ws.SourceFolder;
            this.exportTextBox.Text = ws.DefaultExportFolder;
            this.typesExportTextBox.Text = ws.GetExportAbsoluteFolder(ws.Language);
            this.useIntValueCheckBox.Checked = ws.UseIntValue;

            if (string.IsNullOrEmpty(this.languageComboBox.Text))
            {
                this.languageComboBox.SelectedIndex = 0;
            }

            this.workspaceTextBox.Enabled = false;
            this.workspaceButton.Enabled = false;

            this._filename = ws.FileName;

            this.Text = Resources.WorkspaceEditTiltle;

            SetSourceLanguage();

            checkButtons();

            setMetaVisible(ws.Version <= 0);
        }

        private void setMetaVisible(bool metaVisible)
        {
            this.metaFileLabel.Visible = metaVisible;
            this.XMLTextBox.Visible = metaVisible;
            this.XMLButton.Visible = metaVisible;
        }

        private void checkButtons()
        {
            string wksName = nameTextBox.Text.Trim();
            string wksLocation = workspaceTextBox.Text.Trim();

            doneButton.Enabled = true;
            workspaceTextBox.Enabled = true;
            workspaceButton.Enabled = true;
            sourceTextBox.Enabled = true;
            sourceButton.Enabled = true;
            exportTextBox.Enabled = true;
            typesExportTextBox.Enabled = true;
            typesExportButton.Enabled = true;
            exportButton.Enabled = true;
            XMLTextBox.Enabled = true;
            XMLButton.Enabled = true;

            if (string.IsNullOrEmpty(wksName))
            {
                doneButton.Enabled = false;
                workspaceTextBox.Enabled = false;
                workspaceButton.Enabled = false;
                sourceTextBox.Enabled = false;
                sourceButton.Enabled = false;
                exportTextBox.Enabled = false;
                exportButton.Enabled = false;
                typesExportTextBox.Enabled = false;
                typesExportButton.Enabled = false;
                XMLTextBox.Enabled = false;
                XMLButton.Enabled = false;
            }
            else if (string.IsNullOrEmpty(wksLocation))
            {
                doneButton.Enabled = false;
                sourceTextBox.Enabled = false;
                sourceButton.Enabled = false;
                exportTextBox.Enabled = false;
                exportButton.Enabled = false;
                typesExportTextBox.Enabled = false;
                typesExportButton.Enabled = false;
                XMLTextBox.Enabled = false;
                XMLButton.Enabled = false;
            }
        }

        /// <summary>
        /// Handles when the done button is pressed.
        /// </summary>
        private void doneButton_Click(object sender, EventArgs e)
        {
            string wksName = nameTextBox.Text.Trim();
            string wksLocation = workspaceTextBox.Text.Trim();
            string xmlFile = XMLTextBox.Text.Trim();

            if (string.IsNullOrEmpty(wksName) ||
                string.IsNullOrEmpty(wksLocation) || !Directory.Exists(wksLocation))
            {
                MessageBox.Show(Resources.WorkspaceSettingWarning, Resources.Warning, MessageBoxButtons.OK);
                return;
            }

            string driveStr0 = Path.GetPathRoot(wksLocation);

            // create the given behavior folder if it does not exist
            string behaviorFolder = this.sourceTextBox.Text;
            if (string.IsNullOrEmpty(behaviorFolder))
            {
                behaviorFolder = Path.Combine(wksLocation, "behaviors");
            }

            string driveStr1 = Path.GetPathRoot(behaviorFolder);
            //Debug.Check(driveStr1 == driveStr0);
            if (driveStr1 != driveStr0)
            {
                MessageBox.Show(Resources.WorkspaceSourceRootWarning, Resources.Warning, MessageBoxButtons.OK);
                return;
            }

            if (!Directory.Exists(behaviorFolder))
            {
                Directory.CreateDirectory(behaviorFolder);
            }

            // create the given export folder if it does not exist
            string exportFolder = this.exportTextBox.Text;
            if (string.IsNullOrEmpty(exportFolder))
            {
                exportFolder = Path.Combine(wksLocation, "exported");
            }

            string driveStr2 = Path.GetPathRoot(exportFolder);
            //Debug.Check(driveStr2 == driveStr0);
            if (driveStr2!= driveStr0)
            {
                MessageBox.Show(Resources.WorkspaceExportRootWarning, Resources.Warning, MessageBoxButtons.OK);
                return;
            }

            if (!Directory.Exists(exportFolder))
            {
                Directory.CreateDirectory(exportFolder);
            }

            string language = string.IsNullOrEmpty(_sourceLanguage) ? this.languageComboBox.Text : _sourceLanguage;

            bool useIntValue = this.useIntValueCheckBox.Checked;

            // create the updated or new workspace
            if (_workspace != null)
            {
                _workspace = new Workspace(useIntValue, language, _filename, wksName, xmlFile, behaviorFolder, exportFolder, _workspace.ExportDatas);
            }
            else
            {
                _workspace = new Workspace(useIntValue, language, _filename, wksName, xmlFile, behaviorFolder, exportFolder);
            }

            if (!Plugin.IsASCII(_workspace.RelativeDefaultExportFolder))
            {
                string errorMsg = string.Format("The relative export path '{0}' can only be ASCII", _workspace.RelativeDefaultExportFolder);
                MessageBox.Show(errorMsg, Resources.FileError, MessageBoxButtons.OK);
                return;
            }

            // create the types export folder if it does not exist
            string typesExportFolder = this.typesExportTextBox.Text;
            string driveStr3 = Path.GetPathRoot(typesExportFolder);
            //Debug.Check(driveStr3 == driveStr0);
            if (driveStr3 != driveStr0)
            {
                MessageBox.Show(Resources.WorkspaceExportRootWarning, Resources.Warning, MessageBoxButtons.OK);
                return;
            }

            if (string.IsNullOrEmpty(typesExportFolder))
            {
                typesExportFolder = exportFolder;
            }

            if (!Directory.Exists(typesExportFolder))
            {
                Directory.CreateDirectory(typesExportFolder);
            }

            string wsFilename = _filename.Replace('/', '\\');
            typesExportFolder = typesExportFolder.Replace('/', '\\');
            typesExportFolder = Workspace.MakeRelative(typesExportFolder, wsFilename, true, true);
            typesExportFolder = typesExportFolder.Replace('\\', '/');

            _workspace.SetExportInfo(language, _workspace.ShouldBeExported(language), _workspace.GetExportFileCount(language), typesExportFolder);
            _workspace.MetaFilename = XMLTextBox.Text;

            Workspace.SaveWorkspaceFile(_workspace);

            Close();
        }

        /// <summary>
        /// Handles when the cancel button is pressed.
        /// </summary>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            // we did not edit or create a workspace
            _workspace = null;

            Close();
        }

        private void setFilename()
        {
            string name = this.nameTextBox.Text.Trim();
            string location = this.workspaceTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(location))
            {
                _filename = Path.Combine(location, name);
                _filename = Path.ChangeExtension(_filename, "workspace.xml");
            }
        }

        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            setFilename();

            checkButtons();
        }

        private void workspaceButton_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.Description = Resources.SelectWorkspaceFolder;
            folderBrowserDialog.ShowNewFolderButton = true;

            // assign the user path entered by the user to the browse dialogue
            if (!string.IsNullOrEmpty(this.workspaceTextBox.Text))
            {
                folderBrowserDialog.SelectedPath = Path.GetFullPath(this.workspaceTextBox.Text);
            }

            // assign the path selected by the user to the textbox
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.workspaceTextBox.Text = folderBrowserDialog.SelectedPath;

                if (string.IsNullOrEmpty(this.sourceTextBox.Text))
                {
                    this.sourceTextBox.Text = Path.Combine(workspaceTextBox.Text, "behaviors");
                }

                if (string.IsNullOrEmpty(this.exportTextBox.Text))
                {
                    this.exportTextBox.Text = Path.Combine(workspaceTextBox.Text, "exported");
                }

                if (string.IsNullOrEmpty(this.typesExportTextBox.Text))
                {
                    this.typesExportTextBox.Text = Path.Combine(workspaceTextBox.Text, "exported");
                }

                //if (string.IsNullOrEmpty(this.xmlFolderTextBox.Text))
                //{
                //    this.xmlFolderTextBox.Text = Path.Combine(locationTextBox.Text, "xmlmeta");
                //}

                setFilename();
            }

            checkButtons();
        }

        /// <summary>
        /// Handles when the browse button for the default xml folder is clicked.
        /// </summary>
        private void XMLButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_filename))
            {
                MessageBox.Show(Resources.WorkspaceNameWarning, Resources.Warning, MessageBoxButtons.OK);
                return;
            }

            using(OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = Resources.SetMetaFile;
                openFileDialog.Filter = "*.xml|*.xml";
                openFileDialog.Multiselect = false;

                if (!string.IsNullOrEmpty(this.XMLTextBox.Text))
                {
                    openFileDialog.InitialDirectory = Path.GetDirectoryName(this.XMLTextBox.Text);
                    openFileDialog.FileName = Path.GetFileNameWithoutExtension(this.XMLTextBox.Text);
                }

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string driveStr0 = Path.GetPathRoot(this.workspaceTextBox.Text);
                    string driveStr1 = Path.GetPathRoot(openFileDialog.FileName);

                    if (driveStr1 != driveStr0)
                    {
                        MessageBox.Show(Resources.WorkspaceXmlMetaRootWarning, Resources.Warning, MessageBoxButtons.OK);
                        return;
                    }

                    this.XMLTextBox.Text = openFileDialog.FileName;
                }
            }

            checkButtons();

            SetSourceLanguage();
        }

        private void buttonSource_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.Description = Resources.SelectWorkspaceFolder;
            folderBrowserDialog.ShowNewFolderButton = true;

            // assign the user path entered by the user to the browse dialogue
            if (!string.IsNullOrEmpty(this.sourceTextBox.Text))
            {
                folderBrowserDialog.SelectedPath = Path.GetFullPath(this.sourceTextBox.Text);
            }

            // assign the path selected by the user to the textbox
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string driveStr0 = Path.GetPathRoot(this.workspaceTextBox.Text);
                string driveStr1 = Path.GetPathRoot(folderBrowserDialog.SelectedPath);

                if (driveStr1 != driveStr0)
                {
                    MessageBox.Show(Resources.WorkspaceSourceRootWarning, Resources.Warning, MessageBoxButtons.OK);
                    return;
                }

                this.sourceTextBox.Text = folderBrowserDialog.SelectedPath;
            }

            checkButtons();
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.Description = Resources.SelectWorkspaceFolder;
            folderBrowserDialog.ShowNewFolderButton = true;

            // assign the user path entered by the user to the browse dialogue
            if (!string.IsNullOrEmpty(this.exportTextBox.Text))
            {
                folderBrowserDialog.SelectedPath = Path.GetFullPath(this.exportTextBox.Text);
            }

            // assign the path selected by the user to the textbox
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string driveStr0 = Path.GetPathRoot(this.workspaceTextBox.Text);
                string driveStr1 = Path.GetPathRoot(folderBrowserDialog.SelectedPath);

                if (driveStr1 != driveStr0)
                {
                    MessageBox.Show(Resources.WorkspaceExportRootWarning, Resources.Warning, MessageBoxButtons.OK);
                    return;
                }

                this.exportTextBox.Text = folderBrowserDialog.SelectedPath;
            }

            checkButtons();
        }

        private void typesExportButton_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.Description = Resources.SelectWorkspaceFolder;
            folderBrowserDialog.ShowNewFolderButton = true;

            // assign the user path entered by the user to the browse dialogue
            if (!string.IsNullOrEmpty(this.typesExportTextBox.Text))
            {
                folderBrowserDialog.SelectedPath = Path.GetFullPath(this.typesExportTextBox.Text);
            }

            // assign the path selected by the user to the textbox
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string driveStr0 = Path.GetPathRoot(this.workspaceTextBox.Text);
                string driveStr1 = Path.GetPathRoot(folderBrowserDialog.SelectedPath);

                if (driveStr1 != driveStr0)
                {
                    MessageBox.Show(Resources.WorkspaceExportRootWarning, Resources.Warning, MessageBoxButtons.OK);
                    return;
                }

                this.typesExportTextBox.Text = folderBrowserDialog.SelectedPath;
            }

            checkButtons();
        }

        private ToolTip _toolTip = new ToolTip();

        private void useIntValueCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            checkButtons();
        }

        private void SetSourceLanguage()
        {
            _sourceLanguage = "";

            this.languageComboBox.Enabled = true;

            string metaPath = this.XMLTextBox.Text;

            if (!string.IsNullOrEmpty(metaPath))
            {
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    Encoding utf8WithoutBom = new UTF8Encoding(false);
                    using(StreamReader fileStream = new StreamReader(metaPath, utf8WithoutBom))
                    {
                        xmlDoc.Load(fileStream);
                    }

                    XmlNode rootNode = xmlDoc.DocumentElement;
                    XmlNode languageNode = rootNode.Attributes["language"];

                    _sourceLanguage = (languageNode != null) ? languageNode.Value : "";

                    if (!string.IsNullOrEmpty(_sourceLanguage))
                    {
                        this.languageComboBox.Enabled = false;
                        this.languageComboBox.Text = _sourceLanguage;
                    }
                }
                catch
                {
                }
            }
        }

        private void locationLabel_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(this.workspaceTextBox.Text);
            }
            catch
            {
            }
        }

        private void sourceLabel_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(this.sourceTextBox.Text);
            }
            catch
            {
            }
        }

        private void exportLabel_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(this.exportTextBox.Text);
            }
            catch
            {
            }
        }

        private void generateLabel_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(this.typesExportTextBox.Text);
            }
            catch
            {
            }
        }

        private void metaFileLabel_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(this.XMLTextBox.Text);
            }
            catch
            {
            }
        }

        private void useIntValueCheckBox_MouseEnter(object sender, EventArgs e)
        {
            _toolTip.AutoPopDelay = 5000;
            _toolTip.InitialDelay = 500;
            _toolTip.ReshowDelay = 5000;
            _toolTip.ShowAlways = true;

            _toolTip.Show(Resources.UseIntValueInfo, this.useIntValueCheckBox);
        }

        private void useIntValueCheckBox_MouseLeave(object sender, EventArgs e)
        {
            _toolTip.Hide(this.useIntValueCheckBox);
        }
    }
}
