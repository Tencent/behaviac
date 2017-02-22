@echo off

echo build designer. CurrentDirectory: %CD%

set VSEXE="%VS120COMNTOOLS%..\IDE\devenv.exe"

cd ..\tools\Designer\
echo CurrentDirectory: %CD%

del .\out\BehaviacDesigner*.exe /q

%VSEXE% ".\BehaviacDesigner.sln" /Rebuild "Debug|Any CPU" /Out build.log
if not exist .\out\BehaviacDesigner.exe goto l_error

rename .\out\BehaviacDesigner.exe BehaviacDesigner_d.exe

%VSEXE% ".\BehaviacDesigner.sln" /Rebuild "Release|Any CPU" /Out build.log
if not exist .\out\BehaviacDesigner.exe goto l_error

rem editbin
cd .\out\
echo CurrentDirectory: %CD%

"%VS120COMNTOOLS%..\..\VC\bin\editbin" /LARGEADDRESSAWARE BehaviacDesigner.exe
"%VS120COMNTOOLS%..\..\VC\bin\editbin" /LARGEADDRESSAWARE BehaviacDesigner_d.exe

rem back out
cd ..

rem back tools\Designer\
cd ..\..\build

goto l_exit

:l_error
echo building designer error!
exit /b 1


:l_exit





