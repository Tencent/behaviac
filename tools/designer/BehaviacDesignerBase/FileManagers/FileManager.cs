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
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Behaviac.Design.Properties;

namespace Behaviac.Design.FileManagers
{
    /// <summary>
    /// The result of the save process.
    /// </summary>
    public enum SaveResult
    {
        Succeeded,
        Failed,
        Cancelled
    }

    /// <summary>
    /// This is the base class for a file manager which allows you to load and save behaviours.
    /// </summary>
    public abstract class FileManager : ICloneable
    {
        protected Nodes.BehaviorNode _behavior;

        /// <summary>
        /// The node which will be saved or was loaded.
        /// </summary>
        public Nodes.BehaviorNode Behavior
        {
            get
            {
                return _behavior;
            }
            set
            {
                _behavior = value;
            }
        }

        protected string _filename;

        /// <summary>
        /// The filename which will be loaded from or saved to.
        /// </summary>
        public string Filename
        {
            get
            {
                return _filename;
            }
            set
            {
                _filename = Path.GetFullPath(value);
            }
        }

        /// <summary>
        /// Creates a new file manager.
        /// </summary>
        /// <param name="filename">The filename we want to load from or save to.</param>
        /// <param name="node">The behaviour we want to save. For loading use null.</param>
        public FileManager(string filename, Nodes.BehaviorNode node)
        {
            Debug.Check(Path.IsPathRooted(filename));

            _filename = filename;
            _behavior = node;
        }

        public abstract object Clone();

        /// <summary>
        /// Checks a given file be readonly.
        /// </summary>
        /// <param name="filename">The file which is supposed to be readonly.</param>
        public static bool IsReadOnly(string filename)
        {
            if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
            {
                FileAttributes fatts = File.GetAttributes(filename);
                return (fatts & FileAttributes.ReadOnly) != 0;
            }

            return false;
        }

        /// <summary>
        /// Makes a given file writable.
        /// </summary>
        /// <param name="filename">The file which is supposed to be writable.</param>
        /// <returns>Returns the result when the behaviour is saved.</returns>
        public static SaveResult MakeWritable(string filename, string dialogTitle)
        {
            if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
            {
                FileAttributes fatts = File.GetAttributes(filename);

                if ((fatts & FileAttributes.ReadOnly) != 0)
                {
                    string msg = string.Format(Resources.SaveWarningInfo,
                                               Path.GetFileNameWithoutExtension(filename));

                    switch (MessageBox.Show(msg, dialogTitle, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning))
                    {
                        case DialogResult.Yes:
                        {
                            fatts -= FileAttributes.ReadOnly;
                            File.SetAttributes(filename, fatts);

                            return SaveResult.Succeeded;
                        }

                        case DialogResult.No:
                        {
                            return SaveResult.Failed;
                        }

                        case DialogResult.Cancel:
                        {
                            return SaveResult.Cancelled;
                        }
                    }
                }
            }

            return SaveResult.Succeeded;
        }

        public static string GetRelativePath(string fullpath)
        {
            Debug.Check(Workspace.Current != null && !string.IsNullOrEmpty(Workspace.Current.SourceFolder));

            string wksPath = Path.GetFullPath(Workspace.Current.SourceFolder);
            fullpath = MakeRelative(wksPath, fullpath);
            fullpath = fullpath.Replace("\\", "/");
            fullpath = fullpath.Replace(".xml", "");

            return fullpath;
        }

        public static string GetFullPath(string relativePath, string ext = ".xml")
        {
            Debug.Check(Workspace.Current != null && !string.IsNullOrEmpty(Workspace.Current.SourceFolder));

            relativePath = relativePath.Replace("/", "\\");
            relativePath = MakeAbsolute(Workspace.Current.SourceFolder, relativePath);
            relativePath = Path.ChangeExtension(relativePath, ext);

            return relativePath;
        }

        /// <summary>
        /// Makes a path relative to another.
        /// </summary>
        /// <param name="relativeTo">The path we want to make the other one relative to.</param>
        /// <param name="makeRelative">The path which will be made relative.</param>
        /// <returns>Returns relative path of makeRelative.</returns>
        public static string MakeRelative(string relativeTo, string makeRelative)
        {
            // remove trailing \
            if (!string.IsNullOrEmpty(relativeTo) && relativeTo[relativeTo.Length - 1] == '\\')
            {
                relativeTo = relativeTo.Substring(0, relativeTo.Length - 1);
            }

            if (!string.IsNullOrEmpty(makeRelative) && makeRelative[makeRelative.Length - 1] == '\\')
            {
                makeRelative = makeRelative.Substring(0, makeRelative.Length - 1);
            }

            string[] root = relativeTo.Split('\\');
            string[] child = makeRelative.Split('\\');

            // check how much the path match
            int lastEqualIndex = -1;

            for (int i = 0; i < root.Length && i < child.Length; ++i)
            {
                if (root[i].Equals(child[i], StringComparison.InvariantCultureIgnoreCase))
                {
                    lastEqualIndex = i;
                }

                else
                {
                    break;
                }
            }

            // paths are completely different
            if (lastEqualIndex == -1)
            {
                return makeRelative;
            }

            // paths are completely equal
            if (lastEqualIndex == root.Length - 1 && lastEqualIndex == child.Length - 1)
            {
                return Path.GetFileName(makeRelative);
            }

            string relativePath = string.Empty;
            int stepBackCount = root.Length - lastEqualIndex - 1;

            // add step back so we meet in the last equal directory
            for (int i = 0; i < stepBackCount; ++i)
            {
                relativePath += "..\\";
            }

            // now take the path of the relative filename
            for (int i = lastEqualIndex + 1; i < child.Length - 1; ++i)
            {
                relativePath += child[i] + '\\';
            }

            // add the filename
            relativePath += child[child.Length - 1];

            return relativePath;
        }

        /// <summary>
        /// Makes a path absolute.
        /// </summary>
        /// <param name="absolutePath">The path the other one is relative to.</param>
        /// <param name="relativePath">The path which will be applied to the absolute path.</param>
        /// <returns>Returns the relative path as an absolute one.</returns>
        public static string MakeAbsolute(string absolutePath, string relativePath)
        {
            // remove trailing \
            if (absolutePath[absolutePath.Length - 1] == '\\')
            {
                absolutePath = absolutePath.Substring(0, absolutePath.Length - 1);
            }

            if (!string.IsNullOrEmpty(relativePath))
            {
                relativePath = relativePath.Replace('/', '\\');

                if (relativePath[relativePath.Length - 1] == '\\')
                {
                    relativePath = relativePath.Substring(0, relativePath.Length - 1);
                }
            }

            string[] root = absolutePath.Split('\\');
            string[] child = relativePath.Split('\\');

            // find all the step backs we have
            int rootLength = root.Length;
            int lastStepBack = -1;

            for (int i = 0; i < child.Length; ++i)
            {
                if (child[i] == "..")
                {
                    rootLength--;
                    lastStepBack = i;

                }
                else
                {
                    break;
                }
            }

            if (rootLength < 1)
            {
                //throw new Exception("Relative path goes further up than the depth of the absolute path");
                rootLength = 1;
            }

            // add absolute path base
            string absolute = string.Empty;

            for (int i = 0; i < rootLength; ++i)
            {
                absolute += root[i] + '\\';
            }

            // add relative path without step backs
            for (int i = lastStepBack + 1; i < child.Length; ++i)
            {
                absolute += child[i] + '\\';
            }

            // cut the last \
            return absolute.Substring(0, absolute.Length - 1);
        }

        /// <summary>
        /// Merges two folders by moving all files from one folder to another.
        /// </summary>
        /// <param name="sourceFolder">The folder which we want to merge into another one.</param>
        /// <param name="targetFolder">The folder the other one will be merged into.</param>
        public static void MergeFolders(string sourceFolder, string targetFolder)
        {
            // if the target folder does not exist we simply rename the source folder
            if (!Directory.Exists(targetFolder))
            {
                Directory.Move(sourceFolder, targetFolder);
                return;
            }

            // move all the files to the new folder
            string[] files = Directory.GetFiles(sourceFolder);

            for (int i = 0; i < files.Length; ++i)
            {
                File.Move(files[i], files[i].Replace(sourceFolder, targetFolder));
            }

            // move all the new folders to the new folder
            string[] folders = Directory.GetDirectories(sourceFolder);

            for (int i = 0; i < folders.Length; ++i)
            {
                MergeFolders(folders[i], folders[i].Replace(sourceFolder, targetFolder));
            }

            // delete the no longer needed source folder
            Directory.Delete(sourceFolder);
        }

        /// <summary>
        /// Generates a list of all files and folders which contain behaviours which may be processed.
        /// </summary>
        /// <param name="fileManagers">A list of all avilable file managers which could load a behaviour.</param>
        /// <param name="rootFolder">The folder to start in.</param>
        /// <param name="files">A list of all files found.</param>
        /// <param name="folders">A list of all folders found.</param>
        public static void CollectBehaviors(List<FileManagerInfo> fileManagers, string rootFolder, out IList<string> files, out IList<string> folders)
        {
            folders = new List<string>();
            files = new List<string>();

            // search all folders for behaviours which must be added to the tree
            folders.Add(rootFolder);

            for (int i = 0; i < folders.Count; ++i)
            {
                // add any subfolders
                string[] subfolders = Directory.GetDirectories(folders[i]);

                for (int s = 0; s < subfolders.Length; ++s)
                {
                    // we skip hidden and system folders
                    if ((File.GetAttributes(subfolders[s]) & (FileAttributes.Hidden | FileAttributes.System)) == 0)
                    {
                        folders.Add(subfolders[s]);
                    }
                }

                // search the files of the current folder
                string[] foundFiles = Directory.GetFiles(folders[i], "*.*", SearchOption.TopDirectoryOnly);

                for (int f = 0; f < foundFiles.Length; ++f)
                {
                    // we only add files which can be loaded by a file manager
                    bool hasFileManger = false;

                    foreach (FileManagerInfo fileman in fileManagers)
                    {
                        if (foundFiles[f].ToLowerInvariant().EndsWith(fileman.FileExtension))
                        {
                            hasFileManger = true;
                            break;
                        }
                    }

                    // if there is no filemanager for this file, we skip it
                    if (!hasFileManger)
                    {
                        continue;
                    }

                    // we skip hidden and system files
                    if ((File.GetAttributes(foundFiles[f]) & (FileAttributes.Hidden | FileAttributes.System)) != 0)
                    {
                        continue;
                    }

                    // add to list
                    files.Add(foundFiles[f]);
                }
            }

            files = ((List<string>)files).AsReadOnly();
            folders = ((List<string>)folders).AsReadOnly();
        }

        /// <summary>
        /// Used to return the behavior from the loading process, before all other nodes are created.
        /// </summary>
        /// <param name="behavior">The behavior node which was created.</param>
        public delegate void GetBehaviorNode(Nodes.BehaviorNode behavior);

        /// <summary>
        /// Loads the behaviour of the given file and stores it in the Node member.
        /// </summary>
        public abstract void Load(List<Nodes.Node.ErrorCheck> result, GetBehaviorNode getBehaviorNode);

        /// <summary>
        /// Saves the given behaviour into the given file.
        /// </summary>
        /// <returns>Returns the result when the behaviour is saved.</returns>
        public abstract SaveResult Save();
    }
}
