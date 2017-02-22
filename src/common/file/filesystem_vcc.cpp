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

#include "behaviac/common/file/filesystem.h"
#include "behaviac/common/file/filemanager.h"
#include "behaviac/common/thread/thread.h"
#include "behaviac/common/string/stringutils.h"

#include "./listfiles.h"

void ListFiles_internal(behaviac::vector<behaviac::string>& files, const char* szDirName, bool bRecurrsive) {
    listfiles_dir_t dir;
    listfiles_open(&dir, szDirName);

    bool bEndsWithSlash = behaviac::StringUtils::EndsWith(szDirName, "/");

    if (!bEndsWithSlash) {
        bEndsWithSlash = behaviac::StringUtils::EndsWith(szDirName, "\\");
    }

    while (dir.has_next) {
        listfiles_file_t file;
        listfiles_readfile(&dir, &file);

        // skip . and ..
        if (behaviac::StringUtils::StringEqual(file.name, ".") || behaviac::StringUtils::StringEqual(file.name, "..")) {
            //
        } else {
            behaviac::string fileName;

            if (bEndsWithSlash) {
                fileName = behaviac::FormatString("%s%s", szDirName, file.name);
            } else {
                fileName = behaviac::FormatString("%s/%s", szDirName, file.name);
            }

            files.push_back(fileName);

            if (file.is_dir && bRecurrsive) {
                ListFiles_internal(files, fileName.c_str(), true);
            }
        }

        listfiles_next(&dir);
    }

    listfiles_close(&dir);
}


#if BEHAVIAC_CCDEFINE_MSVC
#include <windows.h>
#endif

#if BEHAVIAC_CCDEFINE_MSVC
namespace behaviac {
    bool CFileSystem::GetFileInfo(const char* szFilename, CFileSystem::SFileInfo& fileInfo) {
        bool bFound = false;
        WIN32_FIND_DATAW win32FileInfo;
        HANDLE hFind = ::FindFirstFileW(STRING2WSTRING(szFilename).c_str(), &win32FileInfo);

        if (hFind != INVALID_HANDLE_VALUE) {
            bFound = true;

            if (win32FileInfo.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) {
                fileInfo.fileAttributes = SFileInfo::ATTRIB_DIRECTORY;

            } else {
                fileInfo.fileAttributes = SFileInfo::ATTRIB_FILE;
            }

            string_ncpy(fileInfo.fileName, WSTRING2STRING(win32FileInfo.cFileName).c_str(), kMAX_FILE_PATH_SIZE);
            fileInfo.fileName[kMAX_FILE_PATH_SIZE] = 0;
            string_ncpy(fileInfo.alternFileName, WSTRING2STRING(win32FileInfo.cAlternateFileName).c_str(), kMAX_FILE_SHORT_PATH_SIZE);
            fileInfo.alternFileName[kMAX_FILE_SHORT_PATH_SIZE] = 0;
            fileInfo.creationTime = BEHAVIAC_MAKE64(win32FileInfo.ftCreationTime.dwLowDateTime, win32FileInfo.ftCreationTime.dwHighDateTime);
            fileInfo.lastAccessTime = BEHAVIAC_MAKE64(win32FileInfo.ftLastAccessTime.dwLowDateTime, win32FileInfo.ftLastAccessTime.dwHighDateTime);
            fileInfo.lastWriteTime = BEHAVIAC_MAKE64(win32FileInfo.ftLastWriteTime.dwLowDateTime, win32FileInfo.ftLastWriteTime.dwHighDateTime);
            fileInfo.fileSize = BEHAVIAC_MAKE64(win32FileInfo.nFileSizeLow, win32FileInfo.nFileSizeHigh);
            FindClose(hFind);
        }

        return bFound;
    }

    bool CFileSystem::GetFileInfo(Handle hFile, SFileInfo& fileInfo) {
        BY_HANDLE_FILE_INFORMATION win32FileInfo;
        BOOL bOk = ::GetFileInformationByHandle(hFile, &win32FileInfo);

        if (bOk) {
            fileInfo.fileAttributes = SFileInfo::ATTRIB_FILE;
            fileInfo.fileName[0] = 0;  // no file name given
            fileInfo.alternFileName[0] = 0;
            fileInfo.creationTime = BEHAVIAC_MAKE64(win32FileInfo.ftCreationTime.dwLowDateTime, win32FileInfo.ftCreationTime.dwHighDateTime);
            fileInfo.lastAccessTime = BEHAVIAC_MAKE64(win32FileInfo.ftLastAccessTime.dwLowDateTime, win32FileInfo.ftLastAccessTime.dwHighDateTime);
            fileInfo.lastWriteTime = BEHAVIAC_MAKE64(win32FileInfo.ftLastWriteTime.dwLowDateTime, win32FileInfo.ftLastWriteTime.dwHighDateTime);
            fileInfo.fileSize = BEHAVIAC_MAKE64(win32FileInfo.nFileSizeLow, win32FileInfo.nFileSizeHigh);
        }

        return !!bOk;
    }

    CFileSystem::Handle CFileSystem::OpenCreateFile(const char* szFullPath, EOpenMode openAccess) {
        HANDLE hFile = FILE_SYSTEM_INVALID_HANDLE;
        uint32_t dwFileAccess = 0;
        uint32_t dwCreationType = 0;
        uint32_t dwFlags = FILE_ATTRIBUTE_NORMAL;

        if (openAccess & EOpenMode_Read) {
            dwFileAccess |= FILE_READ_DATA;
            dwCreationType = OPEN_EXISTING;
        }

        if (openAccess & EOpenMode_Write) {
            dwFileAccess |= FILE_WRITE_DATA;
            dwCreationType = CREATE_ALWAYS;

            if (openAccess & (EOpenMode_Read)) {
                dwCreationType = OPEN_ALWAYS;
            }
        }

        if (openAccess & EOpenMode_WriteAppend) {
            dwFileAccess |= FILE_APPEND_DATA;
            dwCreationType = OPEN_ALWAYS;
        }

        hFile = ::CreateFileW(STRING2WSTRING(szFullPath).c_str(), dwFileAccess, FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, dwCreationType, dwFlags, NULL);

        if (hFile == FILE_SYSTEM_INVALID_HANDLE) {
            uint32_t error = ::GetLastError();
            BEHAVIAC_UNUSED_VAR(error);
            BEHAVIAC_LOGERROR("Invalid file '%s' (error code %u)\n", szFullPath, error);
        }

        return hFile;
    }

    void CFileSystem::CloseFile(Handle file) {
        if (::CloseHandle(file) != 0) {
            ::SetLastError(ERROR_SUCCESS);
        }
    }

    bool CFileSystem::ReadFile(Handle file, void* pBuffer, uint32_t nNumberOfBytesToRead, uint32_t* pNumberOfBytesRead) {
        BEHAVIAC_ASSERT((pBuffer || nNumberOfBytesToRead) && "CFileSystem::ReadFile - (CODE/ERROR) Invalid Target Buffer");
        BOOL bOk = ::ReadFile(file, pBuffer, nNumberOfBytesToRead, (LPDWORD)pNumberOfBytesRead, 0);
        return !!bOk;
    }

    bool CFileSystem::WriteFile(Handle hFile,
                                const void* pBuffer,
                                uint32_t nNumberOfBytesToWrite,
                                uint32_t* pNumberOfBytesWritten) {
        return !!::WriteFile(hFile, pBuffer, nNumberOfBytesToWrite, (LPDWORD)pNumberOfBytesWritten, 0);
    }

    int64_t CFileSystem::SetFilePointer(Handle file, int64_t distanceToMove, ESeekMode moveMethod) {
        LARGE_INTEGER newPos;
        newPos.QuadPart = -1ll;
        LARGE_INTEGER dist;
        dist.QuadPart = distanceToMove;

        int offsetBy = FILE_BEGIN;

        if (moveMethod == ESeekMode_Cur) {
            offsetBy = FILE_CURRENT;

        } else if (moveMethod == ESeekMode_End) {
            offsetBy = FILE_END;

        } else if (moveMethod == ESeekMode_Begin || moveMethod == ESeekMode_Set) {
            offsetBy = FILE_BEGIN;
        }

        ::SetFilePointerEx(file, dist, &newPos, moveMethod);
        return newPos.QuadPart;
    }

    void CFileSystem::FlushFile(Handle file) {
        ::FlushFileBuffers(file);
    }

    bool CFileSystem::FileExist(const char* szFullPath) {
        return (::GetFileAttributesW(STRING2WSTRING(szFullPath).c_str()) != (uint32_t) - 1);
    }

    uint64_t CFileSystem::GetFileSize(Handle hFile) {
        LARGE_INTEGER sizeOfFile;
        sizeOfFile.QuadPart = 0ll;
        ::GetFileSizeEx(hFile, &sizeOfFile);
        return sizeOfFile.QuadPart;
    }

    bool CFileSystem::Move(const char* srcFullPath, const char* destFullPath) {
        return !!::MoveFileW(STRING2WSTRING(srcFullPath).c_str(), STRING2WSTRING(destFullPath).c_str());
    }

    void CFileSystem::MakeSureDirectoryExist(const char* filename) {
        char directory[MAX_PATH];
		string_ncpy(directory, filename, MAX_PATH - 1);

        char* iter = directory;

        while (*iter != 0) {
            if (*iter == '\\' || *iter == '/') {
                char c = *iter;
                *iter = 0;
                ::CreateDirectoryW(STRING2WSTRING(directory).c_str(), NULL);
                *iter = c;
            }

            iter++;
        }
    }

    void CFileSystem::ListFiles(behaviac::vector<behaviac::string>& files, const char* szDirName, bool bRecurrsive) {
        ListFiles_internal(files, szDirName, bRecurrsive);
    }

    void CFileSystem::HandleSeekError(const char* szFilename) {
        BEHAVIAC_UNUSED_VAR(szFilename);
#ifndef _DEBUG
        //	uint32_t error = GetLastError();
        //	FatalError::DamagedDisc( szFilename );
#endif
    }

    bool CFileSystem::IsFullPath(const char* szFilename) {
        return strchr(szFilename, ':') != NULL || (szFilename[0] == '\\' && szFilename[1] == '\\');
    }

    void CFileSystem::ReadError(Handle file) {
        BEHAVIAC_UNUSED_VAR(file);

        uint32_t error = GetLastError();
        char text[256];
        text[0] = '\0';
        string_sprintf(text, "Bad disc... %u", (uint32_t)error);
        //This will trigger a bad disc in _DEBUG. It must be fix
        BEHAVIAC_ASSERT(false, text);
        //This will stop everything we are doom!!!!
        HandleSeekError("Unknown file with ReadFile()");
    }

    // ----------------------------------------------------------------------------
    // File monitor related

#if BEHAVIAC_ENABLE_HOTRELOAD
    class CXCritSec {
    public:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(CXCritSec);

        class CLocker {
        public:
            CLocker(CXCritSec& crit) : m_crit(crit) {
                m_crit.Enter();
            }
            CLocker(CXCritSec* pCrit) : m_crit(*pCrit) {
                m_crit.Enter();
            }
            ~CLocker() {
                m_crit.Leave();
            }

            CLocker& operator=(const CLocker& rhs) {
                m_crit = rhs.m_crit;
                return *this;
            }

        private:
            CXCritSec& m_crit;
        };

        CXCritSec() {
            ::InitializeCriticalSectionAndSpinCount(&m_crit, 4000);
        }
        ~CXCritSec() {
            ::DeleteCriticalSection(&m_crit);
        }

        void Enter() {
            ::EnterCriticalSection(&m_crit);
        }
        void Leave() {
            ::LeaveCriticalSection(&m_crit);
        }

    private:
        CRITICAL_SECTION m_crit;
    };

    class CPrivilegeUtil {
    public:
        CPrivilegeUtil()  { }
        ~CPrivilegeUtil() { }

        static BOOL Add(LPCTSTR pszPrivName, BOOL bEnable = TRUE) {
            bool bSuccess = FALSE;
            HANDLE hToken = NULL;

            if (!OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES, &hToken)) {
                return FALSE;
            }

            TOKEN_PRIVILEGES tp = { 1 };

            if (LookupPrivilegeValue(NULL, pszPrivName, &tp.Privileges[0].Luid)) {
                tp.Privileges[0].Attributes = bEnable ? SE_PRIVILEGE_ENABLED : 0;

                AdjustTokenPrivileges(hToken, FALSE, &tp, sizeof(tp), NULL, NULL);
                bSuccess = (GetLastError() == ERROR_SUCCESS);
            }

            CloseHandle(hToken);

            //return bSuccess;
            return TRUE;
        }
    };

    struct FILE_NOTIFY_INFORMATION_TYPE {
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(FILE_NOTIFY_INFORMATION_TYPE);

        DWORD NextEntryOffset;
        DWORD Action;
        DWORD FileNameLength;
        WCHAR FileName[1];
    };

    struct SDir {
        //BEHAVIAC_DECLARE_MEMORY_OPERATORS(SDir);

        OVERLAPPED						ol;
        HANDLE							hFile;
        DWORD							dwNotifyFilters;
        BOOL							bSubTree;
        behaviac::wstring				strDir;
        FILE_NOTIFY_INFORMATION_TYPE*	pBuff;

        SDir() {
            pBuff = NULL;
            bSubTree = FALSE;
        }

        ~SDir() {
            if (pBuff) {
                BEHAVIAC_DELETE_ARRAY pBuff;
                pBuff = NULL;
            }
        }
    };

    struct SChange {
        behaviac::wstring	strFilePath;
        DWORD				dwAction;
    };

    typedef behaviac::vector<SChange>	VECCHANGES;

    enum {
        E_FILESYSMON_SUCCESS,
        E_FILESYSMON_ERRORUNKNOWN,
        E_FILESYSMON_ERRORNOTINIT,
        E_FILESYSMON_ERROROUTOFMEM,
        E_FILESYSMON_ERROROPENFILE,
        E_FILESYSMON_ERRORADDTOIOCP,
        E_FILESYSMON_ERRORREADDIR,
        E_FILESYSMON_NOCHANGE,
        E_FILESYSMON_ERRORDEQUE
    };

    const int MAX_BUFF_SIZE = 100;

    class CFileSysMon {
        //BEHAVIAC_DECLARE_MEMORY_OPERATORS(CFileSysMon);

    protected:
        HANDLE		m_hIOCP;
        SDir*		m_pDir;
        int			m_nLastError;
        int			m_nThreads;

    public:
        CFileSysMon() {
            m_hIOCP = NULL;
            m_pDir = NULL;
        }
        ~CFileSysMon() {
            Uninit();
        }

        bool Init(int nThreads = 2) {
            if (!CPrivilegeUtil::Add(SE_BACKUP_NAME) ||
                !CPrivilegeUtil::Add(SE_RESTORE_NAME) ||
                !CPrivilegeUtil::Add(SE_CHANGE_NOTIFY_NAME)) {
                m_nLastError = GetLastError();
                return false;
            }

            m_hIOCP = CreateIoCompletionPort((HANDLE)INVALID_HANDLE_VALUE, NULL, 0, nThreads);

            if (!m_hIOCP) {
                m_nLastError = GetLastError();
                return false;
            }

            m_nThreads = nThreads;

            return true;
        }

        void Uninit() {
            //also stops any pending GetQueuedStatus() calls
            RemoveDir();

            if (m_hIOCP) {
                CloseHandle(m_hIOCP);
                m_hIOCP = NULL;
            }
        }

        int SetDir(const wchar_t* pDir,
                   DWORD dwNotifyFilters = FILE_NOTIFY_CHANGE_LAST_WRITE | FILE_ACTION_ADDED | FILE_ACTION_MODIFIED,
                   bool bSubTree = true) {
            RemoveDir();

            m_pDir = BEHAVIAC_NEW SDir;
            BEHAVIAC_ASSERT(m_pDir);

            if (!m_pDir) {
                return E_FILESYSMON_ERROROUTOFMEM;
            }

            // Open handle to the directory to be monitored, note the FILE_FLAG_OVERLAPPED
            if ((m_pDir->hFile = CreateFileW(pDir,
                                             FILE_LIST_DIRECTORY,
                                             FILE_SHARE_READ | FILE_SHARE_DELETE | FILE_SHARE_WRITE,
                                             NULL,
                                             OPEN_EXISTING,
                                             FILE_FLAG_BACKUP_SEMANTICS | FILE_FLAG_OVERLAPPED,
                                             NULL)) == INVALID_HANDLE_VALUE) {
                m_nLastError = GetLastError();

                BEHAVIAC_DELETE m_pDir;
                m_pDir = NULL;

                return E_FILESYSMON_ERROROPENFILE;
            }

            // Allocate notification buffers (will be filled by the system when a notification occurs
            memset(&m_pDir->ol, 0, sizeof(m_pDir->ol));

            if ((m_pDir->pBuff = (BEHAVIAC_NEW_ARRAY FILE_NOTIFY_INFORMATION_TYPE[MAX_BUFF_SIZE])) == NULL) {
                CloseHandle(m_pDir->hFile);
                BEHAVIAC_DELETE m_pDir;
                m_pDir = NULL;

                return E_FILESYSMON_ERROROUTOFMEM;
            }

            // Associate directory handle with the IO completion port
            if (CreateIoCompletionPort(m_pDir->hFile, m_hIOCP, (ULONG_PTR)m_pDir->hFile, 0) == NULL) {
                m_nLastError = GetLastError();

                CloseHandle(m_pDir->hFile);
                BEHAVIAC_DELETE m_pDir;
                m_pDir = NULL;

                return E_FILESYSMON_ERRORADDTOIOCP;
            }

            // Start monitoring for changes
            DWORD dwBytesReturned = 0;
            m_pDir->dwNotifyFilters = dwNotifyFilters;
            m_pDir->bSubTree = bSubTree;
            m_pDir->strDir = pDir;

            if (!ReadDirectoryChangesW(m_pDir->hFile,
                                       m_pDir->pBuff,
                                       MAX_BUFF_SIZE * sizeof(FILE_NOTIFY_INFORMATION_TYPE),
                                       bSubTree,
                                       dwNotifyFilters,
                                       &dwBytesReturned,
                                       &m_pDir->ol,
                                       NULL)) {
                m_nLastError = GetLastError();

                CloseHandle(m_pDir->hFile);
                BEHAVIAC_DELETE m_pDir;
                m_pDir = NULL;

                return E_FILESYSMON_ERRORREADDIR;
            }

            return E_FILESYSMON_SUCCESS;
        }

        void RemoveDir() {
            if (m_pDir) {
                CancelIo(m_pDir->hFile);
                CloseHandle(m_pDir->hFile);
                BEHAVIAC_DELETE m_pDir;
                m_pDir = NULL;
            }
        }

        int GetQueuedStatus(VECCHANGES& vecChanges, DWORD dwTimeOut) {
            BEHAVIAC_ASSERT(m_pDir);

            DWORD		dwBytesXFered = 0;
            ULONG_PTR	ulKey = 0;
            OVERLAPPED*	pOl = NULL;

            vecChanges.clear();

            if (!GetQueuedCompletionStatus(m_hIOCP, &dwBytesXFered, &ulKey, &pOl, dwTimeOut)) {
                if ((m_nLastError = GetLastError()) == WAIT_TIMEOUT) {
                    return E_FILESYSMON_NOCHANGE;
                }

                return E_FILESYSMON_ERRORDEQUE;
            }

            if (ulKey != (ULONG_PTR)m_pDir->hFile) { // not found
                return E_FILESYSMON_ERRORUNKNOWN;
            }

            SChange myChange;
            FILE_NOTIFY_INFORMATION_TYPE* pIter = m_pDir->pBuff;

            while (pIter) {
                pIter->FileName[pIter->FileNameLength / sizeof(wchar_t)] = 0;

                myChange.strFilePath = pIter->FileName;
                myChange.dwAction = pIter->Action;

                vecChanges.push_back(myChange);

                if (pIter->NextEntryOffset == 0UL) {
                    break;
                }

                if ((DWORD)((BYTE*)pIter - (BYTE*)m_pDir->pBuff) > (MAX_BUFF_SIZE * sizeof(FILE_NOTIFY_INFORMATION_TYPE))) {
                    pIter = m_pDir->pBuff;
                }

                pIter = (FILE_NOTIFY_INFORMATION_TYPE*)((LPBYTE)pIter + pIter->NextEntryOffset);
            }

            // Continue reading for changes

            DWORD dwBytesReturned = 0;

            if (!ReadDirectoryChangesW(m_pDir->hFile,
                                       m_pDir->pBuff,
                                       MAX_BUFF_SIZE * sizeof(FILE_NOTIFY_INFORMATION_TYPE),
                                       m_pDir->bSubTree,
                                       m_pDir->dwNotifyFilters,
                                       &dwBytesReturned,
                                       &m_pDir->ol,
                                       NULL)) {
                m_nLastError = GetLastError();
                RemoveDir();

                return E_FILESYSMON_ERRORREADDIR;
            }

            return E_FILESYSMON_SUCCESS;
        }
    };

    struct ModifiedFile_t {
        behaviac::wstring	filePath;

        uint64_t			fileSize;

        ModifiedFile_t() : fileSize(0) {
        }

        ModifiedFile_t(const behaviac::wstring& f) : filePath(f) {
            this->fileSize = this->GetFileSize();
        }

        uint64_t GetFileSize() {
            uint64_t fsize = behaviac::CFileManager::GetInstance()->FileGetSize(WSTRING2STRING(this->filePath).c_str());

            return fsize;
        }

        bool CheckIfReady() {
            uint64_t fsize = this->GetFileSize();

            if (this->fileSize == fsize) {
                return true;
            }

            this->fileSize = fsize;

            return false;
        }
    };

    static CFileSysMon* s_pFileSysMon = NULL;
    static HANDLE s_hThread = NULL;

    static behaviac::vector<behaviac::wstring> s_ModifiedFiles;
    static CXCritSec* s_csDirs = NULL;
    static bool s_bThreadFinish = false;

    DWORD WINAPI ThreadFunc(LPVOID lpvd) {
        BEHAVIAC_UNUSED_VAR(lpvd);

        behaviac::vector<ModifiedFile_t> modifiedFiles;

        VECCHANGES vecChanges;

        while (!s_bThreadFinish) {
            const DWORD kWaitTimeOut = 1;

            if (s_pFileSysMon->GetQueuedStatus(vecChanges, kWaitTimeOut) == E_FILESYSMON_SUCCESS) {
                size_t uiCount = vecChanges.size();

                for (size_t i = 0; i < uiCount; ++i) {
                    if ((vecChanges.at(i).dwAction & FILE_NOTIFY_CHANGE_LAST_WRITE) == FILE_NOTIFY_CHANGE_LAST_WRITE ||
                        (vecChanges.at(i).dwAction & FILE_ACTION_ADDED) == FILE_ACTION_ADDED ||
                        (vecChanges.at(i).dwAction & FILE_ACTION_MODIFIED) == FILE_ACTION_MODIFIED) {
                        const behaviac::wstring& filePath = vecChanges.at(i).strFilePath;

                        bool bFound = false;

                        for (uint32_t j = 0; j < modifiedFiles.size(); ++j) {
                            ModifiedFile_t& file = modifiedFiles[j];

                            if (file.filePath == filePath) {
                                bFound = true;
                                break;
                            }
                        }

                        if (!bFound) {
                            ModifiedFile_t file(filePath);
                            modifiedFiles.push_back(file);
                        }
                    }
                }
            }

            behaviac::thread::Sleep(1);

            uint32_t readyFiles = 0;

            //check if files are ready by checking if the file size is not changing
            for (uint32_t i = 0; i < modifiedFiles.size(); ++i) {
                ModifiedFile_t& file = modifiedFiles[i];

                if (file.CheckIfReady()) {
                    CXCritSec::CLocker locker(s_csDirs);

                    s_ModifiedFiles.push_back(file.filePath);

                    //free it
                    file.fileSize = 0;
                }

                if (file.fileSize == 0) {
                    readyFiles++;
                }
            }

            //all files are ready, clear the array
            if (readyFiles > 0 && readyFiles == modifiedFiles.size()) {
                modifiedFiles.clear();
            }
        }

        s_bThreadFinish = false;
        return 0;
    }

    bool CFileSystem::StartMonitoringDirectory(const wchar_t* dir) {
        BEHAVIAC_UNUSED_VAR(dir);

        s_pFileSysMon = BEHAVIAC_NEW CFileSysMon;
        BEHAVIAC_ASSERT(s_pFileSysMon);

        if (!s_pFileSysMon->Init()) {
            return false;
        }

        if (s_pFileSysMon->SetDir(dir) != E_FILESYSMON_SUCCESS) {
            return false;
        }

        s_csDirs = BEHAVIAC_NEW CXCritSec;
        BEHAVIAC_ASSERT(s_csDirs);

        DWORD dwThreadID = 0;
        s_hThread = CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)ThreadFunc, NULL, 0, &dwThreadID);

        return true;
    }

    void CFileSystem::StopMonitoringDirectory() {
        if (s_pFileSysMon) {
            s_bThreadFinish = true;

            if (s_hThread) {
                WaitForSingleObject(s_hThread, INFINITE);

                CloseHandle(s_hThread);
                s_hThread = NULL;
            }

            s_pFileSysMon->Uninit();

            BEHAVIAC_DELETE s_pFileSysMon;
            s_pFileSysMon = NULL;
        }

        s_ModifiedFiles.clear();

        if (s_csDirs) {
            BEHAVIAC_DELETE s_csDirs;
            s_csDirs = NULL;
        }
    }

    void CFileSystem::GetModifiedFiles(behaviac::vector<behaviac::string>& modifiedFiles) {
        BEHAVIAC_UNUSED_VAR(modifiedFiles);
        modifiedFiles.clear();

        if (s_ModifiedFiles.size() > 0) {
            BEHAVIAC_ASSERT(s_csDirs);
            CXCritSec::CLocker locker(s_csDirs);
            std::sort(s_ModifiedFiles.begin(), s_ModifiedFiles.end());
            s_ModifiedFiles.erase(std::unique(s_ModifiedFiles.begin(), s_ModifiedFiles.end()), s_ModifiedFiles.end());

            //s_ModifiedFiles.swap(modifiedFiles);
            for (behaviac::vector<behaviac::wstring>::iterator it = s_ModifiedFiles.begin(); it != s_ModifiedFiles.end(); ++it) {
                behaviac::wstring& sW = *it;

                behaviac::string s = behaviac::StringUtils::Wide2Char(sW);
                modifiedFiles.push_back(s);
            }

            s_ModifiedFiles.clear();
        }
    }
#endif //BEHAVIAC_ENABLE_HOTRELOAD
}//namespace behaviac

#endif//#if BEHAVIAC_CCDEFINE_MSVC
