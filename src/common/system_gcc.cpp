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
#include "behaviac/common/assert.h"

#include "behaviac/common/thread/thread.h"

#if !BEHAVIAC_CCDEFINE_MSVC
#if BEHAVIAC_CCDEFINE_GCC_LINUX
#include <sys/syscall.h>
#include <linux/unistd.h>
#else
//
#endif

#include <sys/types.h>
#include <unistd.h>

#ifdef System
#undef System
#endif

namespace behaviac {
    THREAD_ID_TYPE GetTID() {
#if BEHAVIAC_CCDEFINE_GCC_LINUX
        //pthread_t t = pthread_self();

        //pthread_id_np_t   tid = pthread_getthreadid_np();

        //#define __NR_gettid 224
        pid_t tid = syscall(__NR_gettid);
        //pid_t tid = gettid();

        return (THREAD_ID_TYPE)tid;
#else
        pthread_t tid = pthread_self();

#if BEHAVIAC_CCDEFINE_APPLE
        return (THREAD_ID_TYPE)(uintptr_t)tid;
#else
        return (THREAD_ID_TYPE)tid;
#endif
#endif
    }


}

#endif//#if !BEHAVIAC_CCDEFINE_MSVC
