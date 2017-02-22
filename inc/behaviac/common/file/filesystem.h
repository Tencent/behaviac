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

#ifndef _BEHAVIAC_COMMON_FILESYSTEM_H_
#define _BEHAVIAC_COMMON_FILESYSTEM_H_

#include "behaviac/common/base.h"
#include "behaviac/common/string/stringutils.h"

#include <vector>
#include <string>

#if BEHAVIAC_CCDEFINE_MSVC
#define FILE_SYSTEM_INVALID_HANDLE (INVALID_HANDLE_VALUE)
#elif BEHAVIAC_CCDEFINE_ANDROID && (BEHAVIAC_CCDEFINE_ANDROID_VER > 8)
#include <android/asset_manager.h>

#define FILE_SYSTEM_INVALID_HANDLE (0)
#else
#define FILE_SYSTEM_INVALID_HANDLE (0)
#endif//#if BEHAVIAC_CCDEFINE_MSVC

typedef uint64_t FileTime;

#ifndef _DEBUG
#   define BEHAVIAC_FILE_SYSTEM_UTF8
#endif // _DEBUG

#ifdef BEHAVIAC_FILE_SYSTEM_UTF8
#   define STRING2WSTRING(x) behaviac::StringUtils::Char2Wide(x)
#   define WSTRING2STRING(x) behaviac::StringUtils::Wide2Char(x)
#else
#   define STRING2WSTRING(x) behaviac::StringUtils::Char2Wide(x)
#   define WSTRING2STRING(x) behaviac::StringUtils::Wide2Char(x)
#endif // BEHAVIAC_FILE_SYSTEM_UTF8

namespace behaviac {
    class BEHAVIAC_API CFileSystem {
    public:

        static const uint32_t kMAX_PATH = 260;
        static const uint32_t kMAX_FILE_PATH_SIZE = (kMAX_PATH);
        static const uint32_t kMAX_FILE_SHORT_PATH_SIZE = (13); // 8.3

        typedef void* Handle;

        struct SFileInfo {
            FileTime    creationTime;
            FileTime    lastAccessTime;
            FileTime    lastWriteTime;
            uint64_t    fileSize;

            static const uint32_t ATTRIB_DIRECTORY = 0;
            static const uint32_t ATTRIB_FILE = 1;

            uint32_t   fileAttributes;
            char     fileName[kMAX_FILE_PATH_SIZE + 1];
            char     alternFileName[kMAX_FILE_SHORT_PATH_SIZE + 1];
        };

        enum EOpenMode {
            EOpenMode_Invalid = 0,
            EOpenMode_Read = 1,
            EOpenMode_Write = 2,
            EOpenMode_ReadWrite = 4,
            EOpenMode_WriteAppend = 8
        };

        enum ESeekMode {
            ESeekMode_Cur = 0,
            ESeekMode_End = 1,
            ESeekMode_Begin = 2,
            ESeekMode_Set = 3,
            ESeekMode_Force32Bits = 0xFFFFFFFF
        };

        static bool GetFileInfo(const char* inFileName, SFileInfo& fileInfo);
        static bool GetFileInfo(Handle hFile, SFileInfo& fileInfo);

        static Handle OpenCreateFile(const char* szFullPath, EOpenMode openAccess);
        static void CloseFile(Handle file);

        static bool ReadFile(Handle file, void* pBuffer, uint32_t nNumberOfBytesToRead, uint32_t* pNumberOfBytesRead);
        static bool WriteFile(Handle hFile, const void* pBuffer, uint32_t nNumberOfBytesToWrite, uint32_t* pNumberOfBytesWritten);

        static int64_t SetFilePointer(Handle file, int64_t distanceToMove, ESeekMode moveMethod);
        static void FlushFile(Handle file);

        static bool FileExist(const char* szFullPath);
        static uint64_t GetFileSize(Handle hFile);

        static bool Move(const char* srcFullPath, const char* destFullPath);
        static void MakeSureDirectoryExist(const char* filename);

        static void ListFiles(behaviac::vector<behaviac::string>& files, const char* szDirName, bool bRecurrsive);

        static void HandleSeekError(const char* szFilename);

        static bool IsFullPath(const char* szFilename);

        static void ReadError(Handle);

        static bool StartMonitoringDirectory(const wchar_t* dir);
        static void StopMonitoringDirectory();
        static void GetModifiedFiles(behaviac::vector<behaviac::string>& modifiedFiles);
    };
}

#endif // #ifndef _BEHAVIAC_COMMON_FILESYSTEM_H_
