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

#ifndef _BEHAVIAC_COMMON_WORKSPACE_H_
#define _BEHAVIAC_COMMON_WORKSPACE_H_

#include "behaviac/common/config.h"
#include "behaviac/common/assert.h"

#include "behaviac/common/base.h"
#include "behaviac/common/logger/logmanager.h"

namespace behaviac {
    class BEHAVIAC_API Config {
    private:
        static const bool ms_bIsDesktopPlatform;
        static bool ms_bIsLogging;
        static bool ms_bLoggingFlush;
        static bool ms_bIsSocketing;
        static bool ms_bProfiling;
        static bool ms_bSocketIsBlocking;
        static bool ms_bHotReload;
		static bool ms_bPreloadBehaviors;
        static unsigned short ms_socketPort;

    public:
        static void LogInfo();

        static bool IsProfiling();

        /**
        to determine if profiling facility is enabled.
        if enabled, the profiling info(node ticking time) can be collected and sent to the connected designer

        by default it is disabled, however, when conecting to the desinger,
        the settings in the designer is sent to the cpp side, that is it can be enabled/disabled by the designer.
        */
        static void SetProfiling(bool bEnabled);
        static bool IsDesktopPlatform();

        /**
        by default, logging is disabled
        */
        static bool IsLogging();
        static void SetLogging(bool bLogging);

        static bool IsLoggingFlush();
        static void SetLoggingFlush(bool bFlush);

        static bool IsSocketing();
        static void SetSocketing(bool bSocketing);

        static bool IsLoggingOrSocketing() {
            return IsLogging() || IsSocketing();
        }

        static bool IsSocketBlocking();
        static void SetSocketBlocking(bool bBlocking);

        static void SetSocketPort(unsigned short port);
        static unsigned short GetSocketPort();

        static bool IsHotReload();
        static void SetHotReload(bool bHotReload);

		static bool PreloadBehaviors();
		static void SetPreloadBehaviors(bool bPreloadBehaviors);
	};

    struct property_t {
        const char* name;
        const char* value;

        property_t(const char* n, const char* v) : name(n), value(v)
        {}
    };

    class BEHAVIAC_API GenerationManager {
    public:
        virtual ~GenerationManager() {}

        static void RegisterBehaviors();

    protected:
        virtual void RegisterBehaviorsImplement();

        static void SetInstance(GenerationManager* generationManager);

    private:
        static GenerationManager* ms_generationManager;
    };

    typedef behaviac::vector<property_t> properties_t;
    typedef behaviac::vector<property_t>::iterator propertie_iterator_t;
    typedef behaviac::vector<property_t>::const_iterator propertie_const_iterator_t;

    class BehaviorTree;
    class BehaviorNode;
    class BehaviorTreeTask;
    class Agent;

	/*! \addtogroup Workspace
	* @{
	* \addtogroup Workspace
	* @{ */

	/**
	Workspace is used to manage the file format, time, load/unload/update bt, debug and so on.
	*/
    class BEHAVIAC_API Workspace {
    public:
        BEHAVIAC_DECLARE_MEMORY_OPERATORS(Workspace);

        enum BEHAVIAC_API EFileFormat {
            EFF_xml = 1,								//specify to use xml only
            EFF_bson = 2,								//specify to use bson only
            EFF_cpp = 4,								//specify to use cpp only
            EFF_default = EFF_xml | EFF_bson | EFF_cpp	//first try to use xml, if xml file doesn't exist, it tries the bson, then tries cpp
        };

        /**
        version_str is used to make sure the lib compiling defines are the same with app's
        just use this default param and don't provode any param unless you know what you are doing
        */
        static Workspace* GetInstance(const char* version_str = BEHAVIAC_BUILD_CONFIG_STR);

        void SetUseIntValue(bool bUseIntValue);
        bool GetUseIntValue();

        ////////////////////////////////////////////////////////////////////////////////////////////
        /**
        "timeSinceStartup" and "frameSinceStartup" might be used by btexec in each frame, btexec is used to execute the BT.

        "timeSinceStartup" is the time in second since the game starts up.

        "frameSinceStartup" is the frames since the game starts up.
        */

        // Deprecated methods, use DoubleValueSinceStartup or IntValueSinceStartup insteadly.
        virtual void SetTimeSinceStartup(double timeSinceStartup);
        virtual double GetTimeSinceStartup();

        virtual void SetDoubleValueSinceStartup(double valueSinceStartup);
        virtual double GetDoubleValueSinceStartup();

        virtual void SetIntValueSinceStartup(long long valueSinceStartup);
        virtual long long GetIntValueSinceStartup();

        virtual void SetFrameSinceStartup(int frameSinceStartup);
        virtual int GetFrameSinceStartup();

        /**
        'FilePath' is the path in which the files are exported, which is configured in the workspace file.

        it can be either an absolute path or relative to the current path.
        */
        const char* GetFilePath() const;
        void SetFilePath(const char* szFilePath);

        /**
        'format' specify the format to use, xml or bson,
        */
        EFileFormat GetFileFormat();
        void SetFileFormat(EFileFormat ff);

        /**
        'MetaFile' is the path in which the meta file are exported by the designer.
        */
        const char* GetMetaFile_() const;
        void SetMetaFile_(const char* szMetaFile);

        bool IsExecAgents() const;
        void SetIsExecAgents(bool bExecAgents);

        virtual void Update();

        void DebugUpdate();

        /**
        this is called for every behavior node, in which users can do some custom stuff
        */
        typedef void (*BehaviorNodeLoader)(const char* nodeType, const properties_t& properties);
        void SetBehaviorNodeLoader(BehaviorNodeLoader loaderCallback);

        /**
        either override BehaviorNodeLoaded or call 'SetBehaviorNodeLoader' to set a callback
        */
        virtual void BehaviorNodeLoaded(const char* nodeType, const properties_t& properties);

        void Cleanup();

        void LogWorkspaceInfo();

        /**
        Load the specified behavior tree

        the workspace export path is provided by Workspace::GetFilePath
        the file format(xml/bson) is provided by Workspace::GetFileFormat

        generally, you need to derive Workspace and override GetFilePath and GetFileFormat,
        then, instantiate your derived Workspace at the very beginning

        @param relativePath
        a path relateve to the workspace exported path. relativePath should not include extension.
        @param bForce
        force to load, otherwise it just uses the one in the cache
        */
        bool Load(const char* relativePath, bool bForce = false);
        void UnLoad(const char* relativePath);
        void UnLoadAll();

        BehaviorTree* LoadBehaviorTree(const char* relativePath);

        int UpdateActionCount(const char* actionString);
        int GetActionCount(const char* actionString);

        /**
        check if it is set as a break point
        */
        bool CheckBreakpoint(const behaviac::Agent* pAgent, const behaviac::BehaviorNode* b, const char* action, behaviac::EActionResult actionResult);

        /**
        check if 'filter' is set to break
        */
        bool CheckAppLogFilter(const char* filter);

        /**
        wait for the continue request from the designer after the breakpoint
        */
        void WaitforContinue();

        /**
        hot reload the modified behaviors.
        */
        void RecordBTAgentMapping(const char* relativePath, Agent* agent);

        /**
        uses the behavior tree in the cache, if not loaded yet, it loads the behavior tree first
        */
        BehaviorTreeTask* CreateBehaviorTreeTask(const char* relativePath);
        void DestroyBehaviorTreeTask(BehaviorTreeTask* behaviorTreeTask, Agent* agent);

        void RegisterBasicNodes();
        void UnRegisterBasicNodes();

        typedef behaviac::map<behaviac::string, BehaviorTree*> BehaviorTrees_t;
        typedef bool(*BehaviorTreeCreator_t)(BehaviorTree*);
        typedef behaviac::map<behaviac::string, Workspace::BehaviorTreeCreator_t> BehaviorTreeCreators_t;

        const BehaviorTrees_t& GetBehaviorTrees();

        bool RegisterBehaviorTreeCreator(const char* relativePath, BehaviorTreeCreator_t creator);
        void UnRegisterBehaviorTreeCreators();

        bool PopFileFromBuffer(const char* file, const char* str, char* pBuffer, uint32_t bufferSize);
        void LogCurrentStates();

        void HandleFileFormat(const behaviac::string& fullPath, behaviac::string& ext, Workspace::EFileFormat& f);
        char* ReadFileToBuffer(const char* file, const char* ext, uint32_t& bufferSize);

    protected:
        Workspace();
        virtual ~Workspace();

    private:
        bool LoadWorkspaceSetting(const char* file, behaviac::string& workspaceFile);
        bool LoadWorkspaceFile(const char* file);

        char* ReadFileToBuffer(const char* file, uint32_t& bufferSize);

        void PopFileFromBuffer(char* pBuffer, uint32_t bufferSize);

        /**
        a shared buffer is kept for file loading.
        after all the files are loaded, you can call this to free those buffer.

        we assume all the loading is handled in the same thread so this is not thread-safe.
        */
        void FreeFileBuffer();

        void ParseBreakpoint(const behaviac::vector<behaviac::string>& tokens);
        void ParseProperty(const behaviac::vector<behaviac::string>& tokens);
        void ParseProfiling(const behaviac::vector<behaviac::string>& tokens);
        void ParseAppLogFilter(const behaviac::vector<behaviac::string>& tokens);

        void LogFrames();

        /**
        handle the requests from the designer

        @return true if the continue message is handled from the designer
        */
        bool HandleRequests();
        void HotReload();

        /**
        @return false if 'path' is not a valid path holding valid data
        */
        bool TryInit();

        //[breakpoint] set TestBehaviorGroup\btunittest.xml Sequence[3] enter
        //[breakpoint] set TestBehaviorGroup\btunittest.xml Sequence[3] exit
        //[breakpoint] clear TestBehaviorGroup\btunittest.xml Sequence[3] enter
        struct BreakpointInfo_t {
            behaviac::string	btname;

            unsigned short		hit_config;

            EActionResult		action_result;

            BreakpointInfo_t() : hit_config(0), action_result(EAR_all)
            {}
        };

        typedef behaviac::map<uint32_t, BreakpointInfo_t> BreakpointInfos_t;
        BreakpointInfos_t		m_breakpoints;

        typedef behaviac::map<CStringCRC, int> ActionCount_t;
        ActionCount_t			m_actions_count;

        behaviac::Mutex			m_cs;

        static const int kMaxPath = 260 * 2;

        char					m_szWorkspaceExportPath[kMaxPath];
        const char*				m_szMetaFile;

        static Workspace*		ms_instance;

        bool					m_bInited;
        bool					m_bExecAgents;

        Workspace::EFileFormat	m_fileFormat;

        behaviac::string		m_applogFilter;

        Workspace::BehaviorTrees_t m_behaviortrees;

#if BEHAVIAC_ENABLE_HOTRELOAD
        typedef behaviac::vector<BehaviorTreeTask*> BehaviorTreeTasks_t;

        struct BTItem_t {
            BehaviorTreeTasks_t			bts;
            behaviac::vector<Agent*>	agents;
        };
        typedef behaviac::map<behaviac::string, BTItem_t> AllBehaviorTreeTasks_t;
        AllBehaviorTreeTasks_t* m_allBehaviorTreeTasks;
#endif//BEHAVIAC_ENABLE_HOTRELOAD

        BehaviorNodeLoader		m_pBehaviorNodeLoader;
        BehaviorTreeCreators_t* m_behaviortreeCreators;

        // we keep 5 (kFileBuffers) buffers, each buffer is allocated once and used from buffer[0] until
        // it is used out then to try to use buffer[1], etc.
        // our goal is to use memory as less as possible and allocate as not often as possible
        const static int kFileBuffers = 5;

        struct FileBuffer_t {
            char* 		start;
            uint32_t	length;
            uint32_t	offset;

            FileBuffer_t() : start(0), length(0), offset(0)
            {}
        };

        FileBuffer_t m_fileBuffers[kFileBuffers];

        int m_frame;

    protected:
        bool m_useIntValue;
        double m_doubleValueSinceStartup;
        long long m_intValueSinceStartup;
        int m_frameSinceStartup;
    };
}//namespace behaviac

#if BEHAVIAC_CCDEFINE_MSVC
#if defined(_DEBUG) || defined(DEBUG)
#define DebugBreak_() _CrtDbgBreak()
#else
#define DebugBreak_() DebugBreak()
#endif
#else
#define DebugBreak_() *((int*)0) = 1
#endif

#define _MY_LOG_BREAK_(filter, appLog) \
    { \
        behaviac::LogManager::GetInstance()->Log(behaviac::ELM_breaked, filter, appLog); \
        behaviac::LogManager::GetInstance()->Flush(0); \
        behaviac::Socket::Flush(); \
        const char* filterStr = (filter == 0 || *filter == '\0') ? "empty" : filter; \
        char msg[1024];\
        string_sprintf(msg, "BehaviorTreeTask AppLog Breaked at: %s(%d)\n\n'%s:%s'\n\nOk to break, Cancel to continue.", __FILE__, __LINE__, filterStr, appLog); \
        if (IDOK == MessageBoxA(0, msg, "BehaviorTreeTask AppLog", MB_OKCANCEL | MB_ICONHAND | MB_SETFOREGROUND | MB_SYSTEMMODAL)) \
        { \
            DebugBreak_(); \
        } \
        behaviac::LogManager::GetInstance()->Log(behaviac::ELM_continue, filter, appLog); \
        behaviac::LogManager::GetInstance()->Flush(0); \
        behaviac::Socket::Flush(); \
    }

/**
an applog can be as a mark indiating a certain event happened, which can be seached and located in the designer when debugged

there are the following predefined filters:RED, ORANGE, YELLOW, GREEN, BLUE, INDIGO, PURPLE
*/
#define BEHAVIAC_APPLOG(filter, msg, contextId) \
    behaviac::LogManager::GetInstance()->Log(behaviac::ELM_tick, filter, msg); \
    if (behaviac::Workspace::CheckAppLogFilter(filter)) \
    { \
        behaviac::LogManager::GetInstance()->Flush(0); \
        behaviac::Socket::Flush(); \
        behaviac::BreakpointPromptHandler_fn fn = behaviac::GetBreakpointPromptHandler(); \
        if (fn == 0) \
        { \
            _MY_LOG_BREAK_(filter, msg); \
        } \
        else \
        { \
            fn(msg); \
        } \
    }

#endif // _BEHAVIAC_COMMON_WORKSPACE_H_
