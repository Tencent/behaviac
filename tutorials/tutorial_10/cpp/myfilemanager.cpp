#include "myfilemanager.h"

MyFileManager::MyFileManager()
{
}

MyFileManager::~MyFileManager()
{
}

behaviac::IFile* MyFileManager::FileOpen(const char* fileName, behaviac::CFileSystem::EOpenMode iOpenAccess)
{
	return CFileManager::FileOpen(fileName, iOpenAccess);
}

void MyFileManager::FileClose(behaviac::IFile* file)
{
	CFileManager::FileClose(file);
}

bool MyFileManager::FileExists(const behaviac::string& filePath, const behaviac::string& ext)
{
	return CFileManager::FileExists(filePath, ext);
}

bool MyFileManager::FileExists(const char* fileName)
{
	return CFileManager::FileExists(fileName);
}

uint64_t MyFileManager::FileGetSize(const char* fileName)
{
	return CFileManager::FileGetSize(fileName);
}

behaviac::wstring MyFileManager::GetCurrentWorkingDirectory()
{
	return CFileManager::GetCurrentWorkingDirectory();
}
