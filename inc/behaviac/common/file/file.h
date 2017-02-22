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

#ifndef _BEHAVIAC_COMMON_FILE_H_
#define _BEHAVIAC_COMMON_FILE_H_

#include "behaviac/common/rttibase.h"
#include "behaviac/common/file/filesystem.h"

#include <vector>

namespace behaviac {
    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    class BEHAVIAC_API IFile : public CRTTIBase {
    public:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(IFile);
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(IFile, CRTTIBase)

        virtual ~IFile() {};

        virtual uint32_t Read(void* pBuffer, uint32_t numberOfBytesToRead) = 0;
        virtual uint32_t	Write(const void* pBuffer, uint32_t numberOfBytesToWrite) = 0;
        virtual int64_t		Seek(int64_t distanceToMove, CFileSystem::ESeekMode moveMethod) = 0;
        virtual uint64_t	GetSize() = 0;
        virtual void		Flush() { };

        template<class T>
        uint32_t Read(T& elem) {
            return Read(&elem, sizeof(T));
        }

        template<class T>
        uint32_t Write(const T& elem) {
            return Write(&elem, sizeof(T));
        }

        virtual int64_t GetCurrentPosition() {
            return Seek(0, CFileSystem::ESeekMode_Cur);
        }
    };

    //////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////
    class BEHAVIAC_API CDiskFile : public IFile {
    public:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(CDiskFile);
        BEHAVIAC_DECLARE_DYNAMIC_TYPE(CDiskFile, IFile)

    public:
        CDiskFile(CFileSystem::Handle handle, bool isRemovableDevice = false);
        virtual ~CDiskFile();

        virtual uint32_t	Read(void* pBuffer, uint32_t numberOfBytesToRead);
        virtual uint32_t	Read(void* pBuffer, uint32_t offsetOfBytesToRead, uint32_t numberOfBytesToRead);
        virtual uint32_t	Write(const void* pBuffer, uint32_t numberOfBytesToWrite);
        virtual int64_t		Seek(int64_t distanceToMove, CFileSystem::ESeekMode moveMethod);
        virtual uint64_t	GetSize();
        virtual void		Flush();

    protected:
        CFileSystem::Handle     m_handle;
        bool m_isRemovableDevice;
    };

}//namespace behaviac

#endif // _BEHAVIAC_COMMON_FILE_H_
