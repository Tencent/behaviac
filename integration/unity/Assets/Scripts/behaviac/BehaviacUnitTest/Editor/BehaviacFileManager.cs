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

using System.Collections;
using System;
using System.IO;

public class BehaviacFileManager: behaviac.FileManager
{
    public BehaviacFileManager() {
    }

    public override void FileClose(string filePath, string ext, byte[] fileHandle) {
        base.FileClose(filePath, ext, fileHandle);
    }

    public override byte[] FileOpen(string filePath, string ext) {
        if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.WindowsPlayer)
        {
            string behaviacPath = filePath + ext;
            string stdPath = behaviacPath.Replace("/", "\\");
            FileStream fs = new FileStream(stdPath, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            byte[] buffer = br.ReadBytes((int)fs.Length);
            fs.Close();
            return buffer;

        } else {
            return base.FileOpen(filePath, ext);
        }
    }
}
