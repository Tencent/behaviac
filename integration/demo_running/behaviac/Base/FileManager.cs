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

using System.IO;
using System.Collections;
using System.Collections.Generic;

// please define BEHAVIAC_NOT_USE_UNITY in your project file if you are not using unity
#if !BEHAVIAC_NOT_USE_UNITY
// if you have compiling errors complaining the following using 'UnityEngine',
//usually, you need to define BEHAVIAC_NOT_USE_UNITY in your project file
using UnityEngine;
#endif//!BEHAVIAC_NOT_USE_UNITY

namespace behaviac
{
    public class FileManager
    {
        #region Singleton

        private static FileManager ms_instance = null;

        public FileManager()
        {
            Debug.Check(ms_instance == null);
            ms_instance = this;
        }

        //~FileManager()
        //{
        //    ms_instance = null;
        //}

        #endregion Singleton

        public static FileManager Instance
        {
            get
            {
                if (ms_instance == null)
                {
                    ms_instance = new FileManager();
                }

                return ms_instance;
            }
        }

        /// <summary>
        /// open the specified file, this function should be consistent with
        /// Workspace.SetWorkspaceSettings's first param 'workspaceExportPath' and Workspace.Load's first param 'relativePath'
        /// as 'filePath' is the conbination of workspaceExportPath and relativePath
        ///
        /// you may need to override this function if you gave a customized 'workspaceExportPath' or used a AssetBundle.
        /// </summary>
        /// <returns>The open.</returns>
        /// <param name="filePath">without extension</param>
        /// <param name="ext">'ext' coult be .xml or .bson</param>
        public virtual byte[] FileOpen(string filePath, string ext)
        {
            try
            {
#if !BEHAVIAC_NOT_USE_UNITY

                if (Application.platform == RuntimePlatform.WindowsEditor ||
                    Application.platform == RuntimePlatform.OSXEditor)
#endif
                {
                    if (ext == ".bson")
                    {
                        ext += ".bytes";
                    }

                    filePath += ext;
                    byte[] pBuffer = File.ReadAllBytes(filePath);

                    return pBuffer;
                }

#if !BEHAVIAC_NOT_USE_UNITY
                else
                {
                    if (ext == ".bson")
                    {
                        filePath += ext;
                    }

                    //skip 'Resources/'
                    int k0 = filePath.IndexOf("Resources");

                    if (k0 != -1)
                    {
                        k0 += 10;
                        string filePathInResources = filePath.Substring(k0);

                        TextAsset ta = Resources.Load(filePathInResources) as TextAsset;

                        if (ta == null)
                        {
                            string msg = string.Format("FileManager::FileOpen failed:'{0}' not loaded", filePath);
                            behaviac.Debug.LogWarning(msg);

                            return null;
                        }
                        else
                        {
                            byte[] pBuffer = ta.bytes;

                            return pBuffer;
                        }
                    }
                    else
                    {
                        string msg = string.Format("FileManager::FileOpen failed:'{0}' should be in /Resources", filePath);
                        behaviac.Debug.LogWarning(msg);
                    }
                }

#endif
            }
            catch (System.Exception e)
            {
                string msg = string.Format("FileManager::FileOpen exception:'{0}'", filePath);
                behaviac.Debug.LogWarning(msg + e.Message + e.StackTrace);
            }

            return null;
        }

        public virtual void FileClose(string filePath, string ext, byte[] pBuffer)
        {
        }

        public virtual List<byte[]> DirOpen(string szDir, string ext)
        {
            List<byte[]> buffers = new List<byte[]>();

            try
            {
                //#if BEHAVIAC_CS_ONLY
                //#endif
#if !BEHAVIAC_NOT_USE_UNITY
                if (Application.platform == RuntimePlatform.WindowsEditor ||
                    Application.platform == RuntimePlatform.OSXEditor)
#endif//
                {
                    string searchPattern = (ext == ".bson") ? "*.bson.bytes" : "*.xml";

                    string[] allFiles = Directory.GetFiles(szDir, searchPattern, SearchOption.TopDirectoryOnly);

                    foreach (string filePath in allFiles)
                    {
                        //string filePath = file.Replace('\\', '/');
                        byte[] pBuffer = FileOpen(filePath, "");
                        buffers.Add(pBuffer);
                    }

                    return buffers;
                }

#if !BEHAVIAC_NOT_USE_UNITY
                else
                {
                    //Debug.LogWarning("szDir.ext:" + szDir + " " + ext);

                    int resourceIndex = szDir.IndexOf("Resources");

                    if (resourceIndex != -1)
                    {
                        string localPath = szDir.Substring(resourceIndex + 10);

                        //Debug.LogWarning("localPath: " + localPath);
                        //TextAsset[] as2 = Resources.LoadAll<TextAsset>("behaviac/exported/meta");
                        TextAsset[] metaFiles = Resources.LoadAll<TextAsset>(localPath);

                        foreach (TextAsset mf in metaFiles)
                        {
                            //Debug.LogWarning("mf.name: " + mf.name);

                            if (!string.IsNullOrEmpty(mf.name))
                            {
                                //skip
                                bool bIsBson = mf.name.IndexOf(".bson") > -1;
                                bool bAskBson = (ext == ".bson");

                                if ((bIsBson && bAskBson) || (!bIsBson && !bAskBson))
                                {
                                    byte[] pBuffer = mf.bytes;
                                    buffers.Add(pBuffer);
                                }
                            }
                        }

                        return buffers;
                    }
                }

#endif
            }
            catch (System.Exception e)
            {
                string msg = string.Format("FileManager::DirOpen exception:'{0}'", szDir);
                behaviac.Debug.LogWarning(msg + e.Message + e.StackTrace);
            }

            return null;
        }

        public virtual bool FileExist(string filePath, string ext)
        {
            return File.Exists(filePath + ext);
        }
    }
}
