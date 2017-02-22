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

#ifndef _BEHAVIAC_COMMON_FILE_LISTFILES_H_
#define _BEHAVIAC_COMMON_FILE_LISTFILES_H_

#if ((defined _UNICODE) && !(defined UNICODE))
#define UNICODE
#endif

#if ((defined UNICODE) && !(defined _UNICODE))
#define _UNICODE
#endif

#include <errno.h>
#include <stdlib.h>
#include <string.h>
#ifdef _MSC_VER
# define WIN32_LEAN_AND_MEAN
# include <windows.h>
# include <tchar.h>
#else
# include <dirent.h>
# include <libgen.h>
# include <sys/stat.h>
# include <stddef.h>
#endif
#ifdef __MINGW32__
# include <tchar.h>
#endif

#if (defined _MSC_VER || defined __MINGW32__)
#include <windows.h>
#define _LISTFILES_PATH_MAX MAX_PATH
#elif defined  __linux__
#include <linux/limits.h>
#define _LISTFILES_PATH_MAX PATH_MAX
#else
#define _LISTFILES_PATH_MAX 4096
#endif

#ifdef _MSC_VER
# define _LISTFILES_PATH_EXTRA 2
#else
# define _LISTFILES_PATH_EXTRA 0
#endif

#define _LISTFILES_FILENAME_MAX 256

#if (defined _MSC_VER || defined __MINGW32__)
#define _LISTFILES_DRIVE_MAX 3
#endif

#if defined _MSC_VER || defined __MINGW32__
#define _listfiles_char_t TCHAR
#define _LISTFILES_STRING(s) _TEXT(s)
#define _listfiles_strlen _tcslen
#define _listfiles_strcpy _tcscpy
#define _listfiles_strcat _tcscat
#define _listfiles_strcmp _tcscmp
#define _listfiles_strrchr _tcsrchr
#define _listfiles_strncmp _tcsncmp
#else
#define _listfiles_char_t char
#define _LISTFILES_STRING(s) s
#define _listfiles_strlen strlen
#define _listfiles_strcpy string_cpy
#define _listfiles_strcat string_cat
#define _listfiles_strcmp strcmp
#define _listfiles_strrchr strrchr
#define _listfiles_strncmp strncmp
#endif

#ifdef _LISTFILES_USE_READDIR_R
// Use readdir by default
#else
# define _LISTFILES_USE_READDIR
#endif

// mingw32 has two versions of dirent, ASCII and UNICODE
#ifndef _MSC_VER
#if (defined __MINGW32__) && (defined _UNICODE)
#define _LISTFILES_DIR _WDIR
#define _listfiles_dir_tent _wdirent
#define _listfiles_opendir _wopendir
#define _listfiles_readdir _wreaddir
#define _listfiles_closedir _wclosedir
#else
#define _LISTFILES_DIR DIR
#define _listfiles_dir_tent dirent
#define _listfiles_opendir opendir
#define _listfiles_readdir readdir
#define _listfiles_closedir closedir
#endif
#endif

typedef struct listfiles_file_t {
    _listfiles_char_t path[_LISTFILES_PATH_MAX];
    _listfiles_char_t name[_LISTFILES_FILENAME_MAX];
    _listfiles_char_t* extension;
    int is_dir;
    int is_reg;

#ifndef _MSC_VER
#ifdef __MINGW32__
    struct _stat _s;
#else
    struct stat _s;
#endif
#endif
} listfiles_file_t;

typedef struct listfiles_dir_t {
    _listfiles_char_t path[_LISTFILES_PATH_MAX];
    int has_next;
    size_t n_files;

    listfiles_file_t* _files;
#ifdef _MSC_VER
    HANDLE _h;
    WIN32_FIND_DATA _f;
#else
    _LISTFILES_DIR* _d;
    struct _listfiles_dir_tent* _e;
#ifndef _LISTFILES_USE_READDIR
    struct _listfiles_dir_tent* _ep;
#endif
#endif
} listfiles_dir_t;


int listfiles_open(listfiles_dir_t* pDir, const _listfiles_char_t* szPath);
void listfiles_close(listfiles_dir_t* pDir);

int listfiles_readfile(const listfiles_dir_t* pDir, listfiles_file_t* pFile);
int listfiles_next(listfiles_dir_t* pDir);


#endif //_BEHAVIAC_COMMON_FILE_LISTFILES_H_
