@echo off

echo please visit http://www.behaviac.com/docs/zh/articles/build/ for more information
echo ---------------------------------------------------------------------------------

where cmake
IF %ERRORLEVEL% NEQ 0 GOTO l_cmake_error

mkdir cmake_binary
cd cmake_binary

echo ---------------------------------------------------------------------------------
mkdir vs2013
cd vs2013
REM cmake -G "Visual Studio 12 2013 Win64" --build ../../..
REM cmake -G "Visual Studio 12 2013" -DBEHAVIAC_VERSION_MODE=ForeUseRelease --build ../../..
cmake -G "Visual Studio 12 2013" --build ../../..
cd ..

REM echo ---------------------------------------------------------------------------------
REM mkdir vs2015
REM cd vs2015
REM cmake -G "Visual Studio 14 2015 Win64" --build ../../..
REM cmake -G "Visual Studio 14 2015" -DBEHAVIAC_VERSION_MODE=ForeUseRelease --build ../../..
REM cmake -G "Visual Studio 14 2015" --build ../../..
REM cd ..


where make
IF %ERRORLEVEL% NEQ 0 GOTO l_no_make

mkdir cygwin
cd cygwin

echo ---------------------------------------------------------------------------------
mkdir debug
cd debug
REM cmake -G "Unix Makefiles" -DCMAKE_BUILD_TYPE=Debug -DBUILD_USE_64BITS=ON -DBUILD_ENABLE_LUA=ON --build ../../../..
cmake -G "Unix Makefiles" -DCMAKE_BUILD_TYPE=Debug --build ../../../..
cd ..

echo ---------------------------------------------------------------------------------
mkdir release
cd release
REM cmake -G "Unix Makefiles" -DCMAKE_BUILD_TYPE=Release -DBEHAVIAC_VERSION_MODE=ForeUseRelease --build ../../../..
cmake -G "Unix Makefiles" -DCMAKE_BUILD_TYPE=Release --build ../../../..
cd ..

rem cygwin
cd ..

echo ---------------------------------------------------------------------------------
mkdir sublime_debug
cd sublime_debug
cmake -G "Sublime Text 2 - Unix Makefiles" -DCMAKE_BUILD_TYPE=Debug --build ../../..
cd ..

mkdir sublime_release
cd sublime_release
cmake -G "Sublime Text 2 - Unix Makefiles" -DCMAKE_BUILD_TYPE=Release --build ../../..
cd ..

:l_no_make

REM if not exist ..\..\example\airbattledemo\cocos_create.bat goto l_end

REM if not exist ..\..\example\airbattledemo\CMakeLists.txt (
	REM pushd ..\..\example\airbattledemo\ 
	REM call cocos_create.bat
	REM popd
REM )


REM echo ---------------------------------------------------------------------------------
REM mkdir example_airbattledemo_vs2013
REM cd example_airbattledemo_vs2013
rem use vs2013 only, it seems cocos vs2015 version is buggy
REM cmake -G "Visual Studio 12 2013" --build ../../../example/airbattledemo
REM cd ..

rem back cmake_binary
cd ..

goto l_end

:l_cmake_error
echo please make sure you have installed cmake 3.3 above (https://cmake.org/files/)
echo and please add cmake's Path to the environment 'PATH'

pause

:l_end
