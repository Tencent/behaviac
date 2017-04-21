#ifndef _BEHAVIAC_COMMON_MYFILEMANAGER_H_
#define _BEHAVIAC_COMMON_MYFILEMANAGER_H_

#include "behaviac/common/file/filemanager.h"

class BEHAVIAC_API MyFileManager : public behaviac::CFileManager
{
public:
	BEHAVIAC_DECLARE_MEMORY_OPERATORS(MyFileManager);

	MyFileManager();
	virtual ~MyFileManager();

	virtual behaviac::IFile* FileOpen(const char* fileName, behaviac::CFileSystem::EOpenMode iOpenAccess = behaviac::CFileSystem::EOpenMode_Read);

	virtual void FileClose(behaviac::IFile* file);
	virtual bool FileExists(const char* fileName);
	virtual bool FileExists(const behaviac::string& filePath, const behaviac::string& ext);

	virtual uint64_t FileGetSize(const char* fileName);
	virtual behaviac::wstring GetCurrentWorkingDirectory();
};

#endif // #ifndef _BEHAVIAC_COMMON_MYFILEMANAGER_H_
