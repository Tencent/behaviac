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

using UnityEngine;
using System.Collections;
using System;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
using System.Runtime.InteropServices;
#endif

[AddComponentMenu("BehaviacSystem")]
public class BehaviacSystem
{
    #region singleon
    private static BehaviacSystem _instance;
    public static BehaviacSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BehaviacSystem();
            }

            return _instance;
        }
    }
    #endregion

    protected static BehaviacFileManager ms_fileSystem = null;
    private static behaviac.Workspace.EFileFormat ms_fileFormat = behaviac.Workspace.EFileFormat.EFF_default;
    private static bool ms_bInit = false;


    private behaviac.Workspace.EFileFormat FileFormat
    {
        get
        {
            string format = UnityTest.UnitTestView.Format;

            if (format == "Xml")
            {
                return behaviac.Workspace.EFileFormat.EFF_xml;
            }
            else if (format == "Bson")
            {
                return behaviac.Workspace.EFileFormat.EFF_bson;
            }
            else if (format == "CSharp")
            {
                return behaviac.Workspace.EFileFormat.EFF_cs;
            }

            return behaviac.Workspace.EFileFormat.EFF_default;
        }
    }

    //read from 'WorkspaceFile', prepending with 'WorkspacePath', relative to the exe's path
    private string FilePath
    {
        get
        {
            string relativePath = "/Resources/behaviac/exported";
            string path = "";

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                path = Application.dataPath + relativePath;
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                path = Application.dataPath + relativePath;
            }
            else
            {
                path = "Assets" + relativePath;
            }

            return path;
        }
    }


    public bool Init()
    {
        if (ms_fileSystem == null)
        {
            ms_fileSystem = new BehaviacFileManager();
        }

        bool bInit = false;

        if (this.FileFormat != ms_fileFormat)
        {
            ms_fileFormat = this.FileFormat;
            bInit = true;
        }

        //only init it when the file format changed at the init is slow
        if (bInit)
        {
            if (ms_bInit)
            {
                behaviac.Workspace.Instance.Cleanup();

                behaviac.LogManager.Instance.Close();
            }

            ms_bInit = true;

            //< write log file
            behaviac.Config.IsLogging = true;
            behaviac.Config.IsSocketing = false;

            behaviac.Workspace.Instance.FilePath = this.FilePath;
            behaviac.Workspace.Instance.FileFormat = this.FileFormat;

            //register names
            behaviac.Agent.RegisterInstanceName<ParTestRegNameAgent>();

            //behaviac.Workspace.Instance.ExportMetas("behaviac/workspace/xmlmeta/unittestmeta.xml");

            behaviac.Debug.Log("Behaviac meta data export over.");

            behaviac.Agent.SetIdMask(0xffffffff);
        }

        return true;
    }

    public void Uninit()
    {
        //do nothing here as it will be uninit just before reinit
    }
}
