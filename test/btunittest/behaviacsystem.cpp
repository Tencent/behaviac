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

//#include "behaviac/common/base.h"
//#include "behaviac/common/workspace.h"
//#include "behaviac/common/file/filemanager.h"
//#include "behaviac/agent/agent.h"
//
//namespace behaviac{
//	class BehaviacSystem{
//	private:
//		static BehaviacSystem* _instance;
//		static  behaviac::Workspace::EFileFormat ms_fileFormat;
//		static  bool ms_bInit ;
//	protected:
//		static CFileManager*	ms_fileSystem ;
//		static Workspace*		ms_workspace ;
//
//
//	public:
//		BehaviacSystem()
//		{
//			ms_fileFormat = behaviac::Workspace::EFileFormat::EFF_xml;
//			ms_bInit = false;
//			ms_fileSystem = NULL;
//			ms_workspace = NULL;
//		}
//		static BehaviacSystem* GetInstance()
//		{
//			if (_instance == NULL)
//			{
//				_instance = new BehaviacSystem();
//			}
//
//			return _instance;
//		}
//		bool Init()
//		{
//			if (ms_fileSystem == NULL)
//			{
//				ms_fileSystem = BEHAVIAC_NEW CFileManager();
//			}
//
//			bool bInit = false;
//			if (ms_workspace == NULL)
//			{
//				ms_workspace = new Workspace();
//				ms_fileFormat = behaviac::Workspace::GetInstance()->GetFileFormat();
//				bInit = true;
//			}
//
//			if (behaviac::Workspace::GetInstance()->GetFileFormat() != ms_fileFormat)
//			{
//				ms_fileFormat = behaviac::Workspace::GetInstance()->GetFileFormat();
//				bInit = true;
//			}
//
//			//only init it when the file format changed at the init is slow
//			if (bInit)
//			{
//				if (ms_bInit)
//				{
//					behaviac::Workspace::GetInstance()->UnLoadAll();
//
//					behaviac::Socket::ShutdownConnection();
//
//					behaviac::LogManager::GetInstance()->DestroyInstance();
//				}
//
//				ms_bInit = true;
//				//< write log file
//				behaviac::Config::SetLogging(true);
//				//behaviac::Config.IsSocketing = false;
//
//				//register names
//
//				behaviac::Agent::RegisterInstanceName<Agent>();
//
//				behaviac::Workspace::GetInstance()->Init();
//				LogManager::GetInstance()->Log("Behaviac meta data export over.");
//
//				bool isBlockSocket = false;
//				behaviac::Socket::SetupConnection(isBlockSocket);
//				behaviac::Agent::SetIdMask(0xffffffff);
//			}
//
//			return true;
//		}
//		void Uninit()
//		{
//			//do nothing here as it will be uninit just before reinit
//		}
//	};
//}
