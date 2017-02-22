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
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Behaviac.Design
{
    public class RecentMenu
    {
        public class RecentMenuItem : MenuItem
        {
            protected String szFileName = string.Empty;
            public String Filename
            {
                get
                {
                    return szFileName;
                }
                set
                {
                    szFileName = value;
                }
            }

            public RecentMenuItem(String _szFileName, String szEntryName, EventHandler eventHandler)
            : base(szEntryName, eventHandler)
            {
                szFileName = _szFileName;
            }
        }

        protected ToolStripMenuItem recentFileMenuItem;
        protected ClickedHandler clickedHandler;
        protected String registryKeyName;
        protected int numEntries = 0;
        protected int maxEntries = 5;
        protected int maxShortenPathLength = 48;

        public RecentMenu(ToolStripMenuItem _recentFileMenuItem, ClickedHandler _clickedHandler, String _registryKeyName)
        {
            recentFileMenuItem = _recentFileMenuItem;
            recentFileMenuItem.Checked = false;
            recentFileMenuItem.Enabled = false;

            clickedHandler = _clickedHandler;

            if (_registryKeyName != null)
            {
                RegistryKeyName = _registryKeyName;
                LoadFromRegistry();
            }
        }

        public delegate void ClickedHandler(String szFileName);

        protected void OnClick(object sender, System.EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            clickedHandler(menuItem.Text);
        }

        public virtual ToolStripItemCollection MenuItems
        {
            get
            {
                return recentFileMenuItem.DropDownItems;
            }
        }

        public virtual int StartIndex
        {
            get
            {
                return 0;
            }
        }

        public virtual int EndIndex
        {
            get
            {
                return numEntries;
            }
        }

        public int NumEntries
        {
            get
            {
                return numEntries;
            }
        }

        public int MaxShortenPathLength
        {
            get
            {
                return maxShortenPathLength;
            }
            set
            {
                maxShortenPathLength = value < 16 ? 16 : value;
            }
        }

        protected virtual void Enable()
        {
            recentFileMenuItem.Enabled = true;
        }

        protected virtual void Disable()
        {
            recentFileMenuItem.Enabled = false;
            recentFileMenuItem.DropDownItems.RemoveAt(0);
        }

        protected virtual void SetFirstFile(ToolStripItem menuItem)
        {
        }

        public void SetFirstFile(int number)
        {
            if (number > 0 && numEntries > 1 && number < numEntries)
            {
                ToolStripItem menuItem = MenuItems[StartIndex + number];
                MenuItems.Remove(menuItem);
                MenuItems.Insert(0, menuItem);
                SetFirstFile(menuItem);
            }
        }

        public static String FixupEntryname(int number, String szEntryName)
        {
            if (number < 9)
            {
                return "&" + (number + 1) + "  " + szEntryName;
            }

            else if (number == 9)
            {
                return "1&0" + "  " + szEntryName;
            }

            else
            {
                return (number + 1) + "  " + szEntryName;
            }
        }

        public int FindFilenameNumber(String szFileName)
        {
            if (szFileName == null)
            {
                throw new ArgumentNullException("szFileName");
            }

            if (szFileName.Length == 0)
            {
                throw new ArgumentException("szFileName");
            }

            if (numEntries > 0)
            {
                int number = 0;

                for (int i = StartIndex; i < EndIndex; i++, number++)
                {
                    if (i >= 0 && i < MenuItems.Count &&
                        String.Compare(((ToolStripMenuItem)MenuItems[i]).Text, szFileName, true) == 0)
                    {
                        return number;
                    }
                }
            }

            return -1;
        }

        public int FindFilenameMenuIndex(String szFileName)
        {
            int number = FindFilenameNumber(szFileName);
            return number < 0 ? -1 : StartIndex + number;
        }

        public String GetEntryAt(int number)
        {
            if (number < 0 || number >= numEntries)
            {
                return string.Empty;
            }

            return ((ToolStripMenuItem)MenuItems[StartIndex + number]).Text;
        }

        static public String ShortenPathname(String szPathname, int maxLength)
        {
            if (szPathname.Length <= maxLength)
            {
                return szPathname;
            }

            String rootPath = Path.GetPathRoot(szPathname);

            if (rootPath.Length > 3)
            {
                rootPath += Path.DirectorySeparatorChar;
            }

            String[] elemtsArray = szPathname.Substring(rootPath.Length).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            int szFileNameIndex = elemtsArray.GetLength(0) - 1;

            if (elemtsArray.GetLength(0) == 1)
            {
                // pathname is just a root and szFileName
                if (elemtsArray[0].Length > 5)
                {
                    // long enough to shorten
                    // if path is a UNC path, root may be rather long
                    if (rootPath.Length + 6 >= maxLength)
                    {
                        return rootPath + elemtsArray[0].Substring(0, 3) + "...";

                    }
                    else
                    {
                        return szPathname.Substring(0, maxLength - 3) + "...";
                    }
                }

            }
            else if ((rootPath.Length + 4 + elemtsArray[szFileNameIndex].Length) > maxLength)
            {
                // pathname is just a root and szFileName
                rootPath += "...\\";

                int len = elemtsArray[szFileNameIndex].Length;

                if (len < 6)
                {
                    return rootPath + elemtsArray[szFileNameIndex];
                }

                if ((rootPath.Length + 6) >= maxLength)
                {
                    len = 3;

                }
                else
                {
                    len = maxLength - rootPath.Length - 3;
                }

                return rootPath + elemtsArray[szFileNameIndex].Substring(0, len) + "...";

            }
            else if (elemtsArray.GetLength(0) == 2)
            {
                return rootPath + "...\\" + elemtsArray[1];

            }
            else
            {
                int len = 0;
                int begin = 0;

                for (int i = 0; i < szFileNameIndex; i++)
                {
                    if (elemtsArray[i].Length > len)
                    {
                        begin = i;
                        len = elemtsArray[i].Length;
                    }
                }

                int totalLength = szPathname.Length - len + 3;
                int end = begin + 1;

                while (totalLength > maxLength)
                {
                    if (begin > 0)
                    {
                        totalLength -= elemtsArray[--begin].Length - 1;
                    }

                    if (totalLength <= maxLength)
                    {
                        break;
                    }

                    if (end < szFileNameIndex)
                    {
                        totalLength -= elemtsArray[++end].Length - 1;
                    }

                    if (begin == 0 && end == szFileNameIndex)
                    {
                        break;
                    }
                }

                for (int i = 0; i < begin; i++)
                {
                    rootPath += elemtsArray[i] + '\\';
                }

                rootPath += "...\\";

                for (int i = end; i < szFileNameIndex; i++)
                {
                    rootPath += elemtsArray[i] + '\\';
                }

                return rootPath + elemtsArray[szFileNameIndex];
            }

            return szPathname;
        }

        public void AddEntry(String szFileName, bool saveFullname = true)
        {
            String pathname = saveFullname ? Path.GetFullPath(szFileName) : szFileName;
            AddEntry(pathname, ShortenPathname(pathname, MaxShortenPathLength));
        }

        public void AddEntry(String szFileName, String szEntryName)
        {
            if (szFileName == null)
            {
                throw new ArgumentNullException("szFileName");
            }

            if (szFileName.Length == 0)
            {
                throw new ArgumentException("szFileName");
            }

            if (numEntries > 0)
            {
                int index = FindFilenameMenuIndex(szFileName);

                if (index >= 0)
                {
                    SetFirstFile(index - StartIndex);
                    return;
                }
            }

            if (numEntries < maxEntries)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem(szFileName, null, new System.EventHandler(OnClick));
                MenuItems.Insert(StartIndex, menuItem);
                SetFirstFile(menuItem);

                if (numEntries++ == 0)
                {
                    Enable();
                }

            }
            else if (numEntries > 1)
            {
                int index = StartIndex + numEntries - 1;

                if (index < MenuItems.Count)
                {
                    ToolStripMenuItem menuItem = (ToolStripMenuItem)MenuItems[index];
                    //menuItem.Text = FixupEntryname(0, szEntryName);
                    menuItem.Text = szFileName;
                    SetFirstFile(menuItem);
                }
            }
        }

        public void RemoveByIndex(int index)
        {
            if (index >= 0 && index < numEntries)
            {
                if (--numEntries == 0)
                {
                    Disable();

                }
                else
                {
                    int startIndex = StartIndex;

                    if (index == 0)
                    {
                        SetFirstFile(MenuItems[startIndex + 1]);
                    }

                    MenuItems.RemoveAt(startIndex + index);
                }
            }
        }

        public void RemoveEntry(String szFileName)
        {
            if (numEntries > 0)
            {
                RemoveByIndex(FindFilenameNumber(szFileName));
            }
        }

        public void RemoveAll()
        {
            if (numEntries > 0)
            {
                for (int index = EndIndex - 1; index > StartIndex; index--)
                {
                    MenuItems.RemoveAt(index);
                }

                Disable();
                numEntries = 0;
            }
        }

        public String RegistryKeyName
        {
            get
            {
                return registryKeyName;
            }
            set
            {
                registryKeyName = value.Trim();

                if (registryKeyName.Length == 0)
                {
                    registryKeyName = null;
                }
            }
        }

        public void LoadFromRegistry(String keyName)
        {
            RegistryKeyName = keyName;
            LoadFromRegistry();
        }

        public void LoadFromRegistry()
        {
            if (registryKeyName != null)
            {
                RemoveAll();

                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(registryKeyName);

                if (regKey != null)
                {
                    maxEntries = (int)regKey.GetValue("max", maxEntries);

                    for (int number = maxEntries; number > 0; number--)
                    {
                        String szFileName = (String)regKey.GetValue("File" + number.ToString());

                        if (szFileName != null && szFileName != "")
                        {
                            AddEntry(szFileName, false);
                        }
                    }

                    regKey.Close();
                }
            }
        }

        public void SaveToRegistry(String keyName)
        {
            RegistryKeyName = keyName;
            SaveToRegistry();
        }

        public void SaveToRegistry()
        {
            if (registryKeyName != null)
            {
                RegistryKey regKey = Registry.CurrentUser.CreateSubKey(registryKeyName);

                if (regKey != null)
                {
                    regKey.SetValue("max", maxEntries);

                    int number = 1;
                    int i = StartIndex;

                    for (; i < EndIndex; i++, number++)
                    {
                        regKey.SetValue("File" + number.ToString(), ((ToolStripMenuItem)MenuItems[i]).Text);
                    }

                    for (; number <= 16; number++)
                    {
                        regKey.DeleteValue("File" + number.ToString(), false);
                    }

                    regKey.Close();
                }
            }
        }
    }
}
