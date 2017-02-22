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

#include "behaviac/common/config.h"
#include "behaviac/common/logger/logger.h"

#include "behaviac/common/file/filemanager.h"
#include "behaviac/common/file/file.h"

#include <functional>
#include <algorithm>

namespace behaviac {
    //////////////////////////////////////////////////////////////////////////
    CDiskFile::CDiskFile(CFileSystem::Handle handle, bool removableDevice)
        : m_handle(handle)
        , m_isRemovableDevice(removableDevice) {
    }

    CDiskFile::~CDiskFile() {
        CFileSystem::CloseFile(m_handle);
    }

    uint32_t CDiskFile::Read(void* pBuffer, uint32_t numberOfBytesToRead) {
        uint32_t numberOfBytesRead = 0;

        if (!CFileSystem::ReadFile(m_handle, pBuffer, numberOfBytesToRead, &numberOfBytesRead)) {
			CFileSystem::ReadError(m_handle);
        }

        return numberOfBytesRead;
    }

    uint32_t CDiskFile::Read(void* pBuffer, uint32_t offsetOfBytesToRead, uint32_t numberOfBytesToRead) {
        CFileSystem::SetFilePointer(m_handle, offsetOfBytesToRead, CFileSystem::ESeekMode_Begin);
        uint32_t numberOfBytesRead = 0;

        if (!CFileSystem::ReadFile(m_handle, pBuffer, numberOfBytesToRead, &numberOfBytesRead)) {
			CFileSystem::ReadError(m_handle);
        }

        return numberOfBytesRead;
    }

    uint32_t CDiskFile::Write(const void* pBuffer, uint32_t nNumberOfBytesToWrite) {
        uint32_t numberOfBytesWritten = 0;

        CFileSystem::WriteFile(m_handle, pBuffer, nNumberOfBytesToWrite, &numberOfBytesWritten);

        return numberOfBytesWritten;
    }

    int64_t CDiskFile::Seek(int64_t distanceToMove, CFileSystem::ESeekMode moveMethod) {
        return CFileSystem::SetFilePointer(m_handle, distanceToMove, moveMethod);
    }

    uint64_t CDiskFile::GetSize() {
        return CFileSystem::GetFileSize(m_handle);
    }

    void CDiskFile::Flush() {
        return CFileSystem::FlushFile(m_handle);
    }

}//namespace behaviac
