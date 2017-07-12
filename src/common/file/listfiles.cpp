/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Tencent is pleased to support the open source community by making behaviac available.
//
// Copyright (C) 2015-2017 THL A29 Limited, a Tencent company. All rights reserved.
//
// Licensed under the BSD 3-Clause License (the "License"); you may not use this pFile except in compliance with
// the License. You may obtain a copy of the License at http://opensource.org/licenses/BSD-3-Clause
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#include "behaviac/common/memory/memory.h"
#include "./listfiles.h"

#ifndef _MSC_VER
#else
# pragma warning(push)
# pragma warning (disable : 4996)
#endif

static void _listfiles_get_ext(listfiles_file_t* pFile);

#define _LISTFILES_MALLOC(_size) BEHAVIAC_MALLOC(_size)
#define _LISTFILES_FREE(_ptr)    BEHAVIAC_FREE(_ptr)


int listfiles_open(listfiles_dir_t* pDir, const char* szPath) {
#ifndef _MSC_VER
#else
    char path_buf[_LISTFILES_PATH_MAX];
#endif
    char* pathp;

    if (pDir == NULL || szPath == NULL || _listfiles_strlen(szPath) == 0) {
        errno = EINVAL;
        return -1;
    }

    if (_listfiles_strlen(szPath) + _LISTFILES_PATH_EXTRA >= _LISTFILES_PATH_MAX) {
        errno = ENAMETOOLONG;
        return -1;
    }

    pDir->_files = NULL;
#ifdef _MSC_VER
    pDir->_h = INVALID_HANDLE_VALUE;
#else
    pDir->_d = NULL;
#endif
    listfiles_close(pDir);

    _listfiles_strcpy(pDir->path, szPath);

    pathp = &pDir->path[_listfiles_strlen(pDir->path) - 1];

    while (pathp != pDir->path && (*pathp == _LISTFILES_STRING('\\') || *pathp == _LISTFILES_STRING('/'))) {
        *pathp = _LISTFILES_STRING('\0');
        pathp++;
    }

#ifdef _MSC_VER
    _listfiles_strcpy(path_buf, pDir->path);
    _listfiles_strcat(path_buf, _LISTFILES_STRING("\\*"));
    pDir->_h = FindFirstFileA(path_buf, &pDir->_f);

    if (pDir->_h == INVALID_HANDLE_VALUE) {
        errno = ENOENT;
#else
    pDir->_d = _listfiles_opendir(szPath);

    if (pDir->_d == NULL) {
#endif
        goto bail;
    }

    // read first pFile
    pDir->has_next = 1;
#ifndef _MSC_VER
    pDir->_e = _listfiles_readdir(pDir->_d);

    if (pDir->_e == NULL) {
        pDir->has_next = 0;
    }

#endif

    return 0;

bail:
    listfiles_close(pDir);
    return -1;
}

void listfiles_close(listfiles_dir_t* pDir) {
    if (pDir == NULL) {
        return;
    }

    memset(pDir->path, 0, sizeof(pDir->path));
    pDir->has_next = 0;
    pDir->n_files = 0;
    _LISTFILES_FREE(pDir->_files);
    pDir->_files = NULL;
#ifdef _MSC_VER

    if (pDir->_h != INVALID_HANDLE_VALUE) {
        FindClose(pDir->_h);
    }

    pDir->_h = INVALID_HANDLE_VALUE;
#else

    if (pDir->_d) {
        _listfiles_closedir(pDir->_d);
    }

    pDir->_d = NULL;
    pDir->_e = NULL;
#endif
}

int listfiles_next(listfiles_dir_t* pDir) {
    if (pDir == NULL) {
        errno = EINVAL;
        return -1;
    }

    if (!pDir->has_next) {
        errno = ENOENT;
        return -1;
    }

#ifdef _MSC_VER

    if (FindNextFileA(pDir->_h, &pDir->_f) == 0)
#else

    pDir->_e = _listfiles_readdir(pDir->_d);

    if (pDir->_e == NULL)
#endif
    {
        pDir->has_next = 0;
#ifdef _MSC_VER

        if (GetLastError() != ERROR_SUCCESS &&
            GetLastError() != ERROR_NO_MORE_FILES) {
            listfiles_close(pDir);
            errno = EIO;
            return -1;
        }

#endif
    }

    return 0;
}

int listfiles_readfile(const listfiles_dir_t* pDir, listfiles_file_t* pFile) {
    if (pDir == NULL || pFile == NULL) {
        errno = EINVAL;
        return -1;
    }

#ifdef _MSC_VER

    if (pDir->_h == INVALID_HANDLE_VALUE)
#else
    if (pDir->_e == NULL)
#endif
    {
        errno = ENOENT;
        return -1;
    }

    if (_listfiles_strlen(pDir->path) +
        _listfiles_strlen(
#ifdef _MSC_VER
            pDir->_f.cFileName
#else
            pDir->_e->d_name
#endif
        ) + 1 + _LISTFILES_PATH_EXTRA >=
        _LISTFILES_PATH_MAX) {
        errno = ENAMETOOLONG;
        return -1;
    }

    if (_listfiles_strlen(
#ifdef _MSC_VER
            pDir->_f.cFileName
#else
            pDir->_e->d_name
#endif
        ) >= _LISTFILES_FILENAME_MAX) {
        errno = ENAMETOOLONG;
        return -1;
    }

    _listfiles_strcpy(pFile->path, pDir->path);
    _listfiles_strcat(pFile->path, _LISTFILES_STRING("/"));
    _listfiles_strcpy(pFile->name,
#ifdef _MSC_VER
                      pDir->_f.cFileName
#else
                      pDir->_e->d_name
#endif
                     );
    _listfiles_strcat(pFile->path, pFile->name);
#ifndef _MSC_VER
    if (stat(
            pFile->path, &pFile->_s) == -1) {
        return -1;
    }
#endif
    _listfiles_get_ext(pFile);

    pFile->is_dir =
#ifdef _MSC_VER
        !!(pDir->_f.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY);
#else
        S_ISDIR(pFile->_s.st_mode);
#endif
    pFile->is_reg =
#ifdef _MSC_VER
        !!(pDir->_f.dwFileAttributes & FILE_ATTRIBUTE_NORMAL) || (!(pDir->_f.dwFileAttributes & FILE_ATTRIBUTE_DEVICE) && !(pDir->_f.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) &&
                                                                  !(pDir->_f.dwFileAttributes & FILE_ATTRIBUTE_ENCRYPTED) &&
#ifdef FILE_ATTRIBUTE_INTEGRITY_STREAM
                                                                  !(pDir->_f.dwFileAttributes & FILE_ATTRIBUTE_INTEGRITY_STREAM) &&
#endif
#ifdef FILE_ATTRIBUTE_NO_SCRUB_DATA
                                                                  !(pDir->_f.dwFileAttributes & FILE_ATTRIBUTE_NO_SCRUB_DATA) &&
#endif
                                                                  !(pDir->_f.dwFileAttributes & FILE_ATTRIBUTE_OFFLINE) && !(pDir->_f.dwFileAttributes & FILE_ATTRIBUTE_TEMPORARY));
#else
        S_ISREG(pFile->_s.st_mode);
#endif

    return 0;
}


void _listfiles_get_ext(listfiles_file_t* pFile) {
    char* period = _listfiles_strrchr(pFile->name, _LISTFILES_STRING('.'));

    if (period == NULL) {
        pFile->extension = &(pFile->name[_listfiles_strlen(pFile->name)]);
    } else {
        pFile->extension = period + 1;
    }
}

# if defined (_MSC_VER)
# pragma warning(pop)
# endif
