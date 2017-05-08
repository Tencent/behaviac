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

#include "behaviac/common/base.h"
#include "behaviac/common/file/filesystem.h"
#include "behaviac/common/file/filemanager.h"
#include "behaviac/common/thread/mutex_lock.h"

void ListFiles_internal(behaviac::vector<behaviac::string>& files, const char* szDirName, bool bRecurrsive);

#if !BEHAVIAC_CCDEFINE_MSVC
#include <sys/types.h>
#include <sys/stat.h>

#if BEHAVIAC_CCDEFINE_GCC_LINUX
#include <stdio.h>
#include <sys/inotify.h>
#include <sys/stat.h>
#include <unistd.h>
#include <map>
#include <string>
#include <sys/fcntl.h>
#include <stdint.h>
#include <dirent.h>
#include <fcntl.h>
#include <deque>
#include <pthread.h>
#endif

#if BEHAVIAC_CCDEFINE_ANDROID && (BEHAVIAC_CCDEFINE_ANDROID_VER > 8)
#include <android/asset_manager.h>
#else
static const char* openMode[] = {
    "invalid",
    "rb",
    "wb",
    "w+b",
    "a"
};
#endif//#if BEHAVIAC_CCDEFINE_ANDROID

namespace behaviac {
    bool CFileSystem::GetFileInfo(const char* szFilename, CFileSystem::SFileInfo& fileInfo) {
        BEHAVIAC_UNUSED_VAR(szFilename);
        BEHAVIAC_UNUSED_VAR(fileInfo);

        BEHAVIAC_ASSERT(0);
        bool bFound = false;

        return bFound;
    }

    bool CFileSystem::GetFileInfo(Handle hFile, SFileInfo& fileInfo) {
        BEHAVIAC_UNUSED_VAR(hFile);
        BEHAVIAC_UNUSED_VAR(fileInfo);

        BEHAVIAC_ASSERT(0);

        return false;
    }

    CFileSystem::Handle CFileSystem::OpenCreateFile(const char* szFullPath, EOpenMode openAccess) {
#if BEHAVIAC_CCDEFINE_ANDROID && (BEHAVIAC_CCDEFINE_ANDROID_VER > 8)
        AAssetManager* mgr = behaviac::CFileManager::GetInstance()->GetAssetManager();
        Handle hFile = FILE_SYSTEM_INVALID_HANDLE;

        if (openAccess != EOpenMode_Read) {
            BEHAVIAC_LOGERROR("Only read mode is supported for asset files");
            return hFile;
        }

        if (behaviac::StringUtils::StartsWith(szFullPath, "assets:/")) {
            //skip "assets:/"
            const char* fileName = szFullPath + 8;

            AAsset* asset = AAssetManager_open(mgr, fileName, AASSET_MODE_UNKNOWN);

            hFile = (Handle)asset;
        }

#else
        Handle hFile = fopen(szFullPath, openMode[openAccess]);

        if (!hFile) {
            BEHAVIAC_LOGERROR("Could not open file %s", szFullPath);
            return hFile;
        }

#endif
        return hFile;
    }

    void CFileSystem::CloseFile(Handle file) {
#if BEHAVIAC_CCDEFINE_ANDROID && (BEHAVIAC_CCDEFINE_ANDROID_VER > 8)
        AAsset_close((AAsset*)file);
#else
        fclose((FILE*)file);
#endif
    }

    bool CFileSystem::ReadFile(Handle file, void* pBuffer, uint32_t nNumberOfBytesToRead, uint32_t* pNumberOfBytesRead) {
        if (!file) {
            BEHAVIAC_LOGERROR("File not open");
            return 0;
        }

#if BEHAVIAC_CCDEFINE_ANDROID && (BEHAVIAC_CCDEFINE_ANDROID_VER > 8)
        size_t ret = AAsset_read((AAsset*)file, pBuffer, nNumberOfBytesToRead);
#else
        size_t ret = fread(pBuffer, 1, nNumberOfBytesToRead, (FILE*)file);
#endif

        if (pNumberOfBytesRead) {
            *pNumberOfBytesRead = ret;
        }

        return true;
    }

    bool CFileSystem::WriteFile(Handle hFile,
                                const void* pBuffer,
                                uint32_t nNumberOfBytesToWrite,
                                uint32_t* pNumberOfBytesWritten) {
        size_t ret = fwrite(pBuffer, 1, nNumberOfBytesToWrite, (FILE*)hFile);

        if (pNumberOfBytesWritten) {
            *pNumberOfBytesWritten = ret;
        }

        return true;
    }

    int64_t CFileSystem::SetFilePointer(Handle file, int64_t distanceToMove, ESeekMode moveMethod) {
        int64_t ret = 0;

        if (moveMethod == ESeekMode_Cur) {
            ret = fseek((FILE*)file, distanceToMove, SEEK_CUR);

        } else if (moveMethod == ESeekMode_End) {
            ret = fseek((FILE*)file, distanceToMove, SEEK_END);

        } else if (moveMethod == ESeekMode_Begin) {
            ret = fseek((FILE*)file, distanceToMove, SEEK_SET);

        } else if (moveMethod == ESeekMode_Set) {
            ret = fseek((FILE*)file, distanceToMove, SEEK_SET);
        }

        return ret;
    }

    void CFileSystem::FlushFile(Handle file) {
        fflush((FILE*)file);
    }

    bool CFileSystem::FileExist(const char* szFullPath) {
        struct stat st;

        if (stat(szFullPath, &st) || (st.st_mode & S_IFDIR)) {
            return false;
        }

        return true;
    }

    uint64_t CFileSystem::GetFileSize(Handle hFile) {
#if BEHAVIAC_CCDEFINE_ANDROID && (BEHAVIAC_CCDEFINE_ANDROID_VER > 8)
        off_t fileSize = AAsset_getLength((AAsset*)hFile);

        return fileSize;
#else
        struct stat buf;

        int fd = fileno((FILE*)hFile);
        fstat(fd, &buf);
        uint64_t size_ = buf.st_size;

        return size_;
#endif
    }

    bool CFileSystem::Move(const char* srcFullPath, const char* destFullPath) {
        BEHAVIAC_UNUSED_VAR(srcFullPath);
        BEHAVIAC_UNUSED_VAR(destFullPath);

        BEHAVIAC_ASSERT(0);
        return false;
    }

    void CFileSystem::MakeSureDirectoryExist(const char* filename) {
        const int kMAX_PATH = 260;
        char directory[kMAX_PATH];
        string_cpy(directory, filename);
        char* iter = directory;

        mode_t  old = umask(0);

        while (*iter != 0) {
            if (*iter == '\\' || *iter == '/') {
                char c = *iter;
                *iter = 0;
                mkdir(directory, S_IWUSR);
                *iter = c;
            }

            iter++;
        }

        umask(old);
    }

#if BEHAVIAC_CCDEFINE_ANDROID && (BEHAVIAC_CCDEFINE_ANDROID_VER > 8)
	void ListFiles_android(behaviac::vector<behaviac::string>& files, const char* szDirName, bool bRecurrsive)
	{
        AAssetManager* mgr = behaviac::CFileManager::GetInstance()->GetAssetManager();
		if (mgr != NULL)
		{
			const char* validDir = szDirName;
            //skip "assets:/"
			if (behaviac::StringUtils::StartsWith(validDir, "assets:/"))
			{
				validDir = validDir + 8;
			}

			AAssetDir* dir = AAssetManager_openDir(mgr, validDir);
			if (dir != NULL)
			{
				bool bEndsWithSlash = behaviac::StringUtils::EndsWith(szDirName, "/");
				if (!bEndsWithSlash)
				{
					bEndsWithSlash = behaviac::StringUtils::EndsWith(szDirName, "\\");
				}

				while (true)
				{
					const char* fileName = AAssetDir_getNextFileName(dir);
					if (fileName == NULL)
					{
						break;
					}

                    if (bEndsWithSlash)
					{
						fileName = behaviac::FormatString("%s%s", szDirName, fileName);
					}
					else
					{
						fileName = behaviac::FormatString("%s/%s", szDirName, fileName);
					}

					files.push_back(fileName);
				}

				AAssetDir_close(dir);
			}
		}
	}
#endif

    void CFileSystem::ListFiles(behaviac::vector<behaviac::string>& files, const char* szDirName, bool bRecurrsive) {
#if BEHAVIAC_CCDEFINE_ANDROID && (BEHAVIAC_CCDEFINE_ANDROID_VER > 8)
		ListFiles_android(files, szDirName, bRecurrsive);
#else
        ListFiles_internal(files, szDirName, bRecurrsive);
#endif
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

        BEHAVIAC_ASSERT(0);
    }

#if BEHAVIAC_CCDEFINE_GCC_LINUX
    class InotifyDir {
    public:
        enum EventType {
            UNKNOWN = 0, //unknow event
            ADD = 1,     //file added
            MODIFY = 2,  //file modified
            DELETE = 3,  //file deleted
        };
        struct Event {
            void clear() {
                type = UNKNOWN;
                name.clear();
            }
            Event() {}
            Event(const std::string& s, EventType t)
                : name(s), type(t)
            {}
            std::string name;
            EventType type;
        };

    public:
        InotifyDir() {
            m_buffIdx = 0;
            m_buffLen = 0;
            m_wfd = -1;
        }
        bool EventPoll(Event& event);
        void AddPathEvent(const char* path, EventType type);
        bool Watch(const char* path);

        bool init() {
            m_wfd = inotify_init();
            int flag = fcntl(m_wfd, F_GETFL, 0);
            fcntl(m_wfd, F_SETFL, flag | O_NONBLOCK);

            if (m_wfd < 0) {
                return false;
            }

            return true;
        }
    private:
        void Unwatch(int wd) {
            inotify_rm_watch(m_wfd, wd);
            m_watchFd.erase(wd);
        }
        int m_wfd;
        std::map<int, std::string> m_watchFd;
        char m_buff[1000];
        int m_buffLen;
        int m_buffIdx;
        std::deque<Event> m_pendingEv;
    };
    bool InotifyDir::Watch(const char* path) {
        if (m_wfd == -1) {
            return false;
        }

        struct stat statbuff;

        stat(path, &statbuff);

        if (!(statbuff.st_mode & S_IFDIR)) {
            return false;
        }

        int wd = inotify_add_watch(m_wfd, path,
                                   IN_MODIFY | IN_MOVE | IN_CREATE | IN_DELETE | IN_MOVE_SELF);

        if (wd < 0) {
            return false;
        }

        m_watchFd[wd] = path;

        if (*m_watchFd[wd].rbegin() != '/') {
            m_watchFd[wd].push_back('/');
        }

        struct dirent* item;

        DIR* dir = opendir(path);

        if (dir == NULL) {
            close(wd);
            return false;
        }

        while ((item = readdir(dir)) != NULL) {
            if (StringUtils::StringEqual(item->d_name, ".")
                || StringUtils::StringEqual(item->d_name, "..")) {
                continue;
            }

            if (item->d_type == DT_DIR) {
                std::string s = m_watchFd[wd] + item->d_name;
                Watch(s.c_str());
            }
        }

        closedir(dir);
        return true;
    }

    void InotifyDir::AddPathEvent(const char* path, EventType type) {

        struct dirent* item;
        DIR* dir = opendir(path);

        if (dir == NULL) {
            return;
        }

        while ((item = readdir(dir)) != NULL) {
            if (StringUtils::StringEqual(item->d_name, ".")
                || StringUtils::StringEqual(item->d_name, "..")) {
                continue;
            }

            if (item->d_type == DT_REG) {
                std::string s = path;
                s += item->d_name;
                AddPathEvent(s.c_str(), type);
            } else {
                std::string s = path;
                s += item->d_name;
                Event e(s, type);
                m_pendingEv.push_back(e);
            }
        }

        closedir(dir);
    }

    bool InotifyDir::EventPoll(Event& event) {
        for (;;) {
            if (!m_pendingEv.empty()) {
                event = m_pendingEv.front();
                m_pendingEv.pop_front();
                return true;
            }

            if (m_buffIdx >= m_buffLen) {
                m_buffIdx = 0;
                m_buffLen = read(m_wfd, m_buff, sizeof(m_buff));

                if (m_buffLen <= 0) {
                    return false;
                }
            }

            inotify_event* ev = (inotify_event*)&m_buff[m_buffIdx];
            m_buffIdx += ev->len + sizeof(inotify_event);

            if (ev->mask & (IN_IGNORED | IN_UNMOUNT)) {
                Unwatch(ev->wd);
                continue;
            }

            if (ev->mask & (IN_CREATE | IN_MOVED_TO)) {
                if (ev->mask & IN_ISDIR) {
                    std::string s = m_watchFd[ev->wd] + ev->name;

                    if (ev->mask & IN_MOVED_TO) {
                        AddPathEvent(s.c_str(), ADD);
                    }

                    continue;
                }
            }

            if (ev->mask & IN_MOVED_FROM && ev->mask & IN_ISDIR) {
                //event can't catch
                continue;
            }

            if (ev->mask & IN_MOVE_SELF) {
                continue;
            }

            if (ev->mask & IN_DELETE) {
                if (ev->mask & IN_ISDIR) {
                    continue;
                }
            }

            event.clear();

            if (ev->mask & (IN_MOVED_TO | IN_CREATE)) {
                event.type = ADD;
            } else if (ev->mask & (IN_MOVED_FROM | IN_DELETE)) {
                event.type = DELETE;
            } else if (ev->mask & IN_MODIFY) {
                event.type = MODIFY;
            } else {
                event.type = UNKNOWN;
            }

            event.name = m_watchFd[ev->wd] + ev->name;
            return true;
        }
    }

    static bool s_bThreadFinish = true;
    static pthread_t s_tid;
    static behaviac::Mutex			s_mutex;
    static behaviac::vector<behaviac::string> s_ModifiedFiles;
    static void* ThreadFunc(void* arg) {
        bool isLock = false;
        const char* path = (char*)arg;
        InotifyDir inotify;
        InotifyDir::Event ev;

        inotify.init();
        inotify.Watch(path);

        while (!s_bThreadFinish) {
            if (!inotify.EventPoll(ev)) {
                if (isLock) {
                    isLock = false;
                    s_mutex.Unlock();
                }

                usleep(100);
                continue;
            }

            if (!isLock) {
                isLock = true;
                s_mutex.Lock();
            }

            if (ev.type == InotifyDir::MODIFY || ev.type == InotifyDir::ADD) {
                ev.name.erase(0, strlen(path));
                s_ModifiedFiles.push_back(ev.name.c_str());
            }
        }

        if (isLock) {
            isLock = false;
            s_mutex.Unlock();
        }

        return NULL;
    }
#endif

    bool CFileSystem::StartMonitoringDirectory(const wchar_t* dir) {
        BEHAVIAC_UNUSED_VAR(dir);

#if BEHAVIAC_CCDEFINE_GCC_LINUX

        if (!s_bThreadFinish) {
            return true;
        }

        s_bThreadFinish = false;

        behaviac::wstring dirW = dir;

        //to use static!
        static behaviac::string s_buffer = "";

        s_buffer = behaviac::StringUtils::Wide2Char(dirW);

        if (pthread_create(&s_tid, NULL, ThreadFunc, const_cast<char*>(s_buffer.c_str())) < 0) {
            return false;
        }

        //pthread_detach(tid);
#endif

        return true;
    }

    void CFileSystem::StopMonitoringDirectory() {
#if BEHAVIAC_CCDEFINE_GCC_LINUX

        if (!s_bThreadFinish) {
            s_bThreadFinish = true;

            pthread_join(s_tid, 0);

            s_ModifiedFiles.clear();
        }

#endif
    }

    void CFileSystem::GetModifiedFiles(behaviac::vector<behaviac::string>& modifiedFiles) {
        BEHAVIAC_UNUSED_VAR(modifiedFiles);
#if BEHAVIAC_CCDEFINE_GCC_LINUX
        modifiedFiles.clear();

        if (s_ModifiedFiles.empty()) {
            return;
        }

        behaviac::ScopedLock lock(s_mutex);
        std::sort(s_ModifiedFiles.begin(), s_ModifiedFiles.end());
        s_ModifiedFiles.erase(std::unique(s_ModifiedFiles.begin(), s_ModifiedFiles.end()), s_ModifiedFiles.end());

        s_ModifiedFiles.swap(modifiedFiles);
        //for (behaviac::vector<behaviac::string>::iterator it = s_ModifiedFiles.begin(); it != s_ModifiedFiles.end(); ++it)
        //{
        //	behaviac::string& s = *it;

        //	modifiedFiles.push_back(s);
        //}

        //s_ModifiedFiles.clear();
#endif
    }

}//namespace behaviac

#endif//#if !BEHAVIAC_CCDEFINE_MSVC
